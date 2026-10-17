using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.MUH;
using static Istemci.Muhasebe.MuhTanimOrtak;

namespace Istemci.Muhasebe
{
    public class MutemetIstegi
    {
        public int kod { get; set; }
        public int yil { get; set; }
        public string tcKimlikNo { get; set; }
        public string kisiAdi { get; set; }
        public int muhasebeKod { get; set; }
        public string birimKod { get; set; }
        public string hesapKod { get; set; }
        public string hesapAdi { get; set; }
        public double tutar { get; set; }
    }

    public class TKDKSatirIstegi
    {
        public string KOD { get; set; }
        public string HESAPKOD { get; set; }
        public string HESAPADI { get; set; }
    }

    public class TKDKIstegi
    {
        public string kod { get; set; }
        public int yil { get; set; }
        public int ay { get; set; }
        public int talepTur { get; set; }
        public List<TKDKSatirIstegi> satirlar { get; set; }
    }

    /// <summary>
    /// Sol / üst listeli, seçilen satırı formda düzenleyen Muhasebe Tanımları sayfalarının uçları:
    ///   mutemet    : Mutemet Tanımlama (eski TanimMutemetLimit.aspx)
    ///   hesapkisit : Hesap Planı Kısıtlama Tanımlama (eski TanimHesapPlaniKisit.aspx)
    ///   tkdk       : Tanım Talep Formu (eski TanimTKDKTalepForm.aspx)
    /// </summary>
    public static class MuhTanimListeUclari
    {
        public static void Ekle(WebApplication app)
        {
            app.MapGet(Kok + "mutemet/liste", MutemetListe);
            app.MapPost(Kok + "mutemet/kaydet", MutemetKaydet);
            app.MapPost(Kok + "mutemet/sil", MutemetSil);
            app.MapPost(Kok + "mutemet/sonraki-yil", MutemetSonrakiYil);

            app.MapGet(Kok + "hesapkisit/liste", KisitListe);
            app.MapPost(Kok + "hesapkisit/kaydet", KisitKaydet);
            app.MapPost(Kok + "hesapkisit/sil", KisitSil);
            app.MapPost(Kok + "hesapkisit/sonraki-yil", KisitSonrakiYil);

            app.MapGet(Kok + "tkdk/liste", TKDKListe);
            app.MapGet(Kok + "tkdk/oku", TKDKOku);
            app.MapGet(Kok + "tkdk/hesap", TKDKHesap);
            app.MapPost(Kok + "tkdk/kaydet", TKDKKaydet);
            app.MapPost(Kok + "tkdk/sil", TKDKSil);
            app.MapPost(Kok + "tkdk/sonraki-ay", TKDKSonrakiAy);
        }

        private static IMUHServis Servis() { return TNS.MUH.Arac.Tanimla(); }

        private static string Tarih(TNSDateTime t)
        {
            try { return t == null ? "" : t.Oku().ToString("dd.MM.yyyy HH:mm"); } catch (Exception) { return ""; }
        }

        // ------------------------------------------------------------------ mutemet

        private static bool MutemetYetkili(Kullanici k, string klasor) { return Yetkili(k, klasor, MutemetAdresi); }

        private static object MutemetSatir(MutemetTanim y)
        {
            return new
            {
                KOD = y.kod,
                YIL = y.yil,
                TCKIMLIKNO = y.tcKimlikNo ?? "",
                KISIADI = y.kisiAdi ?? "",
                MUHASEBEKOD = y.muhasebeKod,
                BIRIMKOD = y.birimKod ?? "",
                HESAPKOD = y.hesapKod ?? "",
                HESAPADI = y.hesapAdi ?? "",
                TUTAR = y.tutar,
                ISLEMYAPANAD = y.islemYapanAd ?? "",
                ISLEMTARIHI = Tarih(y.islemTarihi)
            };
        }

        /// <summary>Ara (eski btnAra_Click) ve satır seçimi (kod ile).</summary>
        private static IResult MutemetListe(HttpContext baglam, IWebHostEnvironment ortam, int? kod, int? yil, string tcKimlikNo, string kisiAdi,
            int? muhasebeKod, string birimKod, string hesapKod)
        {
            return Oku(baglam, k => MutemetYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                MutemetTanim m = new MutemetTanim();
                m.kod = kod ?? 0;
                m.yil = yil ?? 0;
                m.tcKimlikNo = Temiz(tcKimlikNo);
                m.kisiAdi = Temiz(kisiAdi);
                m.muhasebeKod = muhasebeKod ?? 0;
                m.birimKod = Temiz(birimKod);
                m.hesapKod = Temiz(hesapKod);
                ObjectArray yler = Servis().MutemetTanimListele(kullanan, m);
                if (yler == null || !yler.sonuc.islemSonuc) return Hata(HataMetni(yler?.sonuc.hataStr, "Liste okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (MutemetTanim y in yler.objeler) satirlar.Add(MutemetSatir(y));
                return Results.Json(new { satirlar });
            });
        }

        private static Task<IResult> MutemetKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, MutemetIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => MutemetYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null) return Hata("İstek okunamadı.");
                List<string> hatalar = new List<string>();
                if (istek.yil <= 0) hatalar.Add("Yıl bilgisi boş bırakılmış.");
                if (Temiz(istek.tcKimlikNo) == "") hatalar.Add("Kişi bilgisi boş bırakılmış.");
                if (Temiz(istek.hesapKod) == "") hatalar.Add("Hesap kodu boş bırakılmış.");
                if (hatalar.Count > 0) return Hata(string.Join("\n", hatalar) + "\nKayıt işlemi yapılamadı.");
                MutemetTanim b = new MutemetTanim();
                b.kod = istek.kod;
                b.yil = istek.yil;
                b.tcKimlikNo = Temiz(istek.tcKimlikNo);
                b.kisiAdi = Temiz(istek.kisiAdi);
                b.muhasebeKod = istek.muhasebeKod;
                b.birimKod = Temiz(istek.birimKod);
                b.hesapKod = Temiz(istek.hesapKod);
                b.hesapAdi = Temiz(istek.hesapAdi);
                b.tutar = istek.tutar;
                b.islemTarihi = new TNSDateTime(DateTime.Now);
                b.islemYapanAd = kullanan.ADSOYAD;
                b.islemYapanKod = kullanan.TCKIMLIKNO;
                Sonuc s = Servis().MutemetTanimKaydet(kullanan, b);
                return s.islemSonuc ? Tamam("Kayıt işlemi başarıyla tamamlandı.") : Hata(HataMetni(s.hataStr, "Kayıt işlemi yapılamadı."));
            });
        }

        private static Task<IResult> MutemetSil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, MutemetIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => MutemetYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null || istek.kod <= 0) return Hata("Lütfen silmek için kayıt seçiniz.");
                ObjectArray l = new ObjectArray();
                l.objeler.Add(new MutemetTanim { kod = istek.kod });
                Sonuc s = Servis().MutemetTanimSil(kullanan, l);
                return s.islemSonuc ? Tamam("Silme işlemi başarıyla gerçekleşti.") : Hata(HataMetni(s.hataStr, "Silme işlemi yapılamadı."));
            });
        }

        private static Task<IResult> MutemetSonrakiYil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, YilIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => MutemetYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null || istek.yil <= 0) return Hata("Yıl bilgisi boş bırakılmış.");
                return SonucYaniti(Servis().MutemetAktar(kullanan, istek.yil, istek.yil + 1));
            });
        }

        // ------------------------------------------------------------------ hesap planı kısıtlama

        private static bool KisitYetkili(Kullanici k, string klasor) { return Yetkili(k, klasor, HesapKisitAdresi); }

        private static IResult KisitListe(HttpContext baglam, IWebHostEnvironment ortam, int? kod, int? yil, int? muhasebeKod, string birimKod, string hesapKod)
        {
            return Oku(baglam, k => KisitYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                HesapPlaniKisit m = new HesapPlaniKisit();
                m.kod = kod ?? 0;
                m.yil = yil ?? 0;
                m.muhasebeKod = muhasebeKod ?? 0;
                m.birimKod = Temiz(birimKod);
                m.hesapKod = Temiz(hesapKod);
                ObjectArray yler = Servis().HesapPlaniKisitListele(kullanan, m);
                if (yler == null || !yler.sonuc.islemSonuc) return Hata(HataMetni(yler?.sonuc.hataStr, "Liste okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (HesapPlaniKisit y in yler.objeler)
                    satirlar.Add(new
                    {
                        KOD = y.kod,
                        YIL = y.yil,
                        MUHASEBEKOD = y.muhasebeKod,
                        BIRIMKOD = y.birimKod ?? "",
                        HESAPKOD = y.hesapKod ?? "",
                        HESAPADI = y.hesapAdi ?? "",
                        ISLEMYAPANAD = y.islemYapanAd ?? "",
                        ISLEMTARIHI = Tarih(y.islemTarihi)
                    });
                return Results.Json(new { satirlar });
            });
        }

        private static Task<IResult> KisitKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, MutemetIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => KisitYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null) return Hata("İstek okunamadı.");
                List<string> hatalar = new List<string>();
                if (istek.yil <= 0) hatalar.Add("Yıl bilgisi boş bırakılmış.");
                if (Temiz(istek.hesapKod) == "") hatalar.Add("Hesap kodu boş bırakılmış.");
                if (hatalar.Count > 0) return Hata(string.Join("\n", hatalar) + "\nKayıt işlemi yapılamadı.");
                HesapPlaniKisit b = new HesapPlaniKisit();
                b.kod = istek.kod;
                b.yil = istek.yil;
                b.muhasebeKod = istek.muhasebeKod;
                b.birimKod = Temiz(istek.birimKod);
                b.hesapKod = Temiz(istek.hesapKod);
                b.hesapAdi = Temiz(istek.hesapAdi);
                b.islemTarihi = new TNSDateTime(DateTime.Now);
                b.islemYapanAd = kullanan.ADSOYAD;
                b.islemYapanKod = kullanan.TCKIMLIKNO;
                Sonuc s = Servis().HesapPlaniKisitKaydet(kullanan, b);
                return s.islemSonuc ? Tamam("Kayıt işlemi başarıyla tamamlandı.") : Hata(HataMetni(s.hataStr, "Kayıt işlemi yapılamadı."));
            });
        }

        private static Task<IResult> KisitSil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, MutemetIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => KisitYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null || istek.kod <= 0) return Hata("Lütfen silmek için kayıt seçiniz.");
                ObjectArray l = new ObjectArray();
                l.objeler.Add(new HesapPlaniKisit { kod = istek.kod });
                Sonuc s = Servis().HesapPlaniKisitSil(kullanan, l);
                return s.islemSonuc ? Tamam("Silme işlemi başarıyla gerçekleşti.") : Hata(HataMetni(s.hataStr, "Silme işlemi yapılamadı."));
            });
        }

        private static Task<IResult> KisitSonrakiYil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, YilIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => KisitYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null || istek.yil <= 0) return Hata("Yıl bilgisi boş bırakılmış.");
                return SonucYaniti(Servis().HesapPlaniKisitAktar(kullanan, istek.yil, istek.yil + 1));
            });
        }

        // ------------------------------------------------------------------ TKDK tanım talep formu

        /// <summary>Eski sayfa: ödeme emri görme yetkisi olmayan giremez; ayrıca menü yetkisi.</summary>
        public static bool TKDKYetkili(Kullanici k, string klasor)
        {
            if (k == null) return false;
            bool oeYetki;
            try { oeYetki = TNS.MUH.Yetki.YetkisiVarMi(k, (int)TNS.MUH.ENUMOGY.Gorme, (int)ENUMBelgeTuru.ODEMEEMRI); }
            catch (Exception) { oeYetki = false; }
            return oeYetki && SayfaYetki.Yetkili(k, klasor, TKDKAdresi);
        }

        private static readonly string[] AyAdlari = { "", "Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık" };

        private static string TalepTurAd(int t)
        {
            return t == 1 ? "Nakit" : t == 2 ? "Ödenek Aktarma" : t == 3 ? "Ek Ödenek" : "";
        }

        private static IResult TKDKListe(HttpContext baglam, IWebHostEnvironment ortam)
        {
            return Oku(baglam, k => TKDKYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                ObjectArray bilgiler = Servis().TalepTanimListele(kullanan, new TalepTanimForm());
                if (bilgiler == null || !bilgiler.sonuc.islemSonuc) return Hata(HataMetni(bilgiler?.sonuc.hataStr, "Liste okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (TalepTanimForm t in bilgiler.objeler)
                    satirlar.Add(new
                    {
                        KOD = t.kod ?? "",
                        YIL = t.yil,
                        AY = t.ay >= 1 && t.ay <= 12 ? AyAdlari[t.ay] : "",
                        TALEPTUR = TalepTurAd(t.talepTur)
                    });
                return Results.Json(new { satirlar });
            });
        }

        private static TalepTanimForm TKDKDetayOku(Kullanici kullanan, string kod)
        {
            TalepTanimForm kriter = new TalepTanimForm();
            kriter.kod = Temiz(kod);
            kriter.detayGoster = true;
            ObjectArray bilgiler = Servis().TalepTanimListele(kullanan, kriter);
            if (bilgiler == null || bilgiler.objeler.Count == 0) return null;
            return (TalepTanimForm)bilgiler.objeler[0];
        }

        /// <summary>Satır seçimi (eski SatirlariGetir).</summary>
        private static IResult TKDKOku(HttpContext baglam, IWebHostEnvironment ortam, string kod)
        {
            return Oku(baglam, k => TKDKYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (Temiz(kod) == "") return Hata("Kayıt seçilmedi.");
                TalepTanimForm f = TKDKDetayOku(kullanan, kod);
                if (f == null) return Hata("Kayıt bulunamadı.");
                List<object> satirlar = new List<object>();
                foreach (TalepTanimFormDetay d in f.satirlar.objeler)
                    satirlar.Add(new { KOD = d.kod ?? "", HESAPKOD = d.ekoKod ?? "", HESAPADI = d.ekoAdi ?? "" });
                return Results.Json(new { kod = f.kod ?? "", yil = f.yil, ay = f.ay, talepTur = f.talepTur, satirlar });
            });
        }

        /// <summary>Ekonomik kod arama (eski HesapListesiDoldur): 830 ile başlayan 3. düzey hesaplar.</summary>
        private static IResult TKDKHesap(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string metin)
        {
            return Oku(baglam, k => TKDKYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                metin = Temiz(metin);
                if (metin == "") return Results.Json(new { satirlar = new object[0] });
                HesapPlaniSatir h = new HesapPlaniSatir();
                h.yil = (yil ?? 0) > 0 ? yil.Value : DateTime.Now.Year;
                h.hesapKodHesapAd = "830" + metin;
                h.detay = false;
                h.seviye = 3;
                ObjectArray hesap = Servis().HesapPlaniListele(kullanan, h);
                List<object> satirlar = new List<object>();
                if (hesap != null)
                    foreach (HesapPlaniSatir d in hesap.objeler)
                    {
                        satirlar.Add(new { kod = (d.hesapKod ?? "").Replace("830.", "").Replace(".", ""), ad = d.aciklama ?? "", gorunen = (d.hesapKod ?? "").Replace("830.", "") + " - " + d.aciklama });
                        if (satirlar.Count >= 100) break;
                    }
                return Results.Json(new { satirlar });
            });
        }

        private static Task<IResult> TKDKKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TKDKIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => TKDKYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null) return Hata("İstek okunamadı.");
                TalepTanimForm bilgi = new TalepTanimForm();
                bilgi.kod = Temiz(istek.kod);
                bilgi.yil = istek.yil;
                bilgi.ay = istek.ay;
                bilgi.talepTur = istek.talepTur;
                foreach (TKDKSatirIstegi s in istek.satirlar ?? new List<TKDKSatirIstegi>())
                {
                    TalepTanimFormDetay fd = new TalepTanimFormDetay();
                    fd.kod = Temiz(s.KOD);
                    fd.ekoKod = Temiz(s.HESAPKOD);
                    fd.ekoAdi = Temiz(s.HESAPADI);
                    if (string.IsNullOrWhiteSpace(fd.ekoAdi)) continue;
                    bilgi.satirlar.Ekle(fd);
                }
                return SonucYaniti(Servis().TalepTanimKaydet(kullanan, bilgi), "Kayıt işlemi başarıyla tamamlandı.");
            });
        }

        private static Task<IResult> TKDKSil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TKDKIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => TKDKYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null || Temiz(istek.kod) == "") return Hata("Lütfen silmek için kayıt seçiniz.");
                return SonucYaniti(Servis().TalepTanimSil(kullanan, new TalepTanimForm { kod = Temiz(istek.kod) }), "Silme işlemi başarıyla gerçekleşti.");
            });
        }

        /// <summary>Sonraki Aya Aktar (eski btnSonrakiAyaAktar_Click): seçili formun bir sonraki aya kopyası.</summary>
        private static Task<IResult> TKDKSonrakiAy(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TKDKIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => TKDKYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null || Temiz(istek.kod) == "") return Hata("Lütfen aktarılacak kaydı seçiniz.");
                TalepTanimForm bilgi = TKDKDetayOku(kullanan, istek.kod);
                if (bilgi == null) return Hata("Kayıt bulunamadı.");
                bilgi.kod = "";
                TNSDateTime tarih = new TNSDateTime("01." + bilgi.ay + "." + bilgi.yil);
                tarih.AddMonths(1);
                bilgi.yil = tarih.Yil;
                bilgi.ay = tarih.Ay;
                foreach (TalepTanimFormDetay item in bilgi.satirlar.objeler) item.kod = "";
                return SonucYaniti(Servis().TalepTanimKaydet(kullanan, bilgi), "Kayıt işlemi başarıyla tamamlandı.");
            });
        }
    }
}
