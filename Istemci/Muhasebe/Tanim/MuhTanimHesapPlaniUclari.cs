using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public class HesapPlaniIstegi
    {
        public int yil { get; set; }
        public string hesapKod { get; set; }
        public string aciklama { get; set; }
        public int tertip { get; set; } = -1;
        public int kesinti { get; set; } = -1;
        public int altHesap { get; set; } = -1;
        public int bakiye { get; set; } = -1;
        public string nitelik { get; set; }
        public string dovizKurKod { get; set; }
        public string satisKarsilikTur { get; set; }
        public string alisKarsilikTur { get; set; }
        public string alacakHesabi { get; set; }
        public string borcHesabi { get; set; }
        public int ekoBasla { get; set; }
        /// <summary>Toplu nitelik değiştirmede: 0 Ekleme, 1 Kaldırma.</summary>
        public int islem { get; set; }
    }

    /// <summary>
    /// Hesap Planı Bilgileri Tanımlama (eski ButceMuhasebe/TanimHesapPlani.aspx) ve toplu nitelik değiştirme (eski HesapPlaniNitelik.aspx) uçları.
    /// </summary>
    public static class MuhTanimHesapPlaniUclari
    {
        /// <summary>Nitelik kodları ve adları (eski MUHGNL160_xx kaynakları; 07 tanımsız).</summary>
        public static readonly (int kod, string ad)[] Nitelikler =
        {
            (1, "Gider Bütçesi Hesabı"), (2, "Kasa Hesabı"), (3, "Özel Gider Hesabı"), (4, "Emanet / Borç Hesabı"), (5, "Avans Hesabı"),
            (6, "Gelir Bütçesi Hesabı"), (8, "Gider Yansıtma Hesabı"), (9, "Gelir Yansıtma Hesabı"), (10, "Yansıtma Yapılacak"),
            (11, "KDV Hesabı"), (12, "Yansıtma Zorunlu (Borç)"), (13, "Mahsup Yapılır"), (14, "Pasif"), (15, "Yansıtma Zorunlu (Alacak)"),
            (16, "Saymanlık Bazlı Bakiye Kontrol"), (17, "Avans Kontrolü Yapılmasın"), (18, "Birim Zorunlu")
        };

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Kok + "hesapplani/secenekler", Secenekler);
            app.MapGet(Kok + "hesapplani/liste", Liste);
            app.MapGet(Kok + "hesapplani/excel", Excel);
            app.MapPost(Kok + "hesapplani/kaydet", Kaydet);
            app.MapPost(Kok + "hesapplani/sil", Sil);
            app.MapPost(Kok + "hesapplani/sonraki-yil", SonrakiYil);
            app.MapPost(Kok + "hesapplani/detay-guncelle", DetayGuncelle);
            app.MapPost(Kok + "hesapplani/birim-doldur", BirimDoldur);
            app.MapPost(Kok + "hesapplani/nitelik-degistir", NitelikDegistir);

            // Yansıtmalar (eski TanimYansitma.aspx yerine): seçili hesabın yansıtma hesapları
            app.MapGet(Kok + "hesapplani/yansitma", YansitmaListe);
            app.MapGet(Kok + "hesapplani/yansitma-excel", YansitmaExcel);
            app.MapPost(Kok + "hesapplani/yansitma-ekle", YansitmaEkle);
            app.MapPost(Kok + "hesapplani/yansitma-sil", YansitmaSil);
            app.MapPost(Kok + "hesapplani/yansitma-sonraki-yil", YansitmaSonrakiYil);
        }

        private static IMUHServis Servis() { return TNS.MUH.Arac.Tanimla(); }

        private static bool HesapPlaniYetkili(Kullanici kullanan, string klasor)
        {
            return Yetkili(kullanan, klasor, HesapPlaniAdresi);
        }

        public static string NitelikMetni(string nitelik)
        {
            List<string> adlar = new List<string>();
            foreach (string n in (nitelik ?? "").Split(';'))
            {
                int kod = OrtakFonksiyonlar.ConvertToInt(n, 0);
                if (kod <= 0) continue;
                foreach ((int k, string ad) in Nitelikler)
                    if (k == kod) { adlar.Add(ad); break; }
            }
            return string.Join(", ", adlar);
        }

        /// <summary>Sayfanın seçenekleri: nitelikler, döviz kurları (HRC para birimleri), Detay Birim Doldur tuşu görünür mü.</summary>
        private static IResult Secenekler(HttpContext baglam, IWebHostEnvironment ortam)
        {
            return Oku(baglam, k => HesapPlaniYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                List<object> nitelikler = new List<object>();
                foreach ((int kod, string ad) in Nitelikler) nitelikler.Add(new { kod, ad });

                List<object> kurlar = new List<object>();
                try
                {
                    ObjectArray paralar = TNS.HRC.Arac.Tanimla().ParaBirimiListele(kullanan, new TNS.BKD.ParaBirimi());
                    if (paralar != null)
                        foreach (TNS.BKD.ParaBirimi p in paralar.objeler)
                            kurlar.Add(new { kod = p.kisaltma ?? "", ad = p.adi + "(" + p.kisaltma + ")" });
                }
                catch (Exception) { }

                bool birimDoldur = false;
                try { birimDoldur = TNS.MUH.Arac.DetayBirimVarMi(); } catch (Exception) { }

                return Results.Json(new { nitelikler, kurlar, birimDoldur, yonetici = Yonetici(kullanan) });
            });
        }

        private static HesapPlaniSatir Satir(HesapPlaniIstegi i)
        {
            HesapPlaniSatir h = new HesapPlaniSatir();
            h.yil = i.yil;
            h.hesapKod = Temiz(i.hesapKod);
            h.aciklama = Temiz(i.aciklama);
            h.tertip = i.tertip;
            h.kesinti = i.kesinti;
            h.altHesapKod = i.altHesap;
            h.bakiye = i.bakiye;
            h.nitelik = i.nitelik ?? "";
            h.dovizKurKod = Temiz(i.dovizKurKod);
            h.dovizKurTurSatis = Temiz(i.satisKarsilikTur);
            h.dovizKurTurAlis = Temiz(i.alisKarsilikTur);
            h.kurFarkiAlacakHesabi = Temiz(i.alacakHesabi);
            h.kurFarkiBorcHesabi = Temiz(i.borcHesabi);
            h.ekoBasla = i.ekoBasla;
            return h;
        }

        private static string TertipAd(int t) { return t == 0 ? "Zorunlu Değil" : t == 1 ? "Zorunlu" : ""; }
        private static string EvetHayir(int t) { return t == 0 ? "Hayır" : t == 1 ? "Evet" : ""; }
        private static string BakiyeAd(int b)
        {
            switch (b)
            {
                case 0: return "Borç-Alacak Çalışır";
                case 1: return "Borç Çalışır";
                case 2: return "Alacak Çalışır";
                default: return "";
            }
        }

        /// <summary>Bul (eski btnAra_Click + Listele): hesap kodu zorunlu; formdaki değerler kıstastır.</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string hesapKod, string aciklama,
            int? tertip, int? kesinti, int? altHesap, int? bakiye, string nitelik)
        {
            return Oku(baglam, k => HesapPlaniYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if ((yil ?? 0) <= 0) return Hata("Yıl bilgisi boş bırakılmış.");
                if (Temiz(hesapKod) == "") return Hata("Lütfen hesap Kodu alanını boş bırakmayın");
                HesapPlaniSatir y = new HesapPlaniSatir();
                y.yil = yil.Value;
                y.hesapKod = Temiz(hesapKod);
                y.aciklama = aciklama ?? "";
                y.tertip = tertip ?? -1;
                y.kesinti = kesinti ?? -1;
                y.altHesapKod = altHesap ?? -1;
                y.bakiye = bakiye ?? -1;
                y.nitelik = (nitelik ?? "").Trim(';');

                ObjectArray yler = Servis().HesapPlaniListele(kullanan, y);
                if (!yler.sonuc.islemSonuc) return Hata(HataMetni(yler.sonuc.hataStr, "Liste okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (HesapPlaniSatir h in yler.objeler)
                    satirlar.Add(new
                    {
                        yil = h.yil,
                        hesapKod = h.hesapKod ?? "",
                        aciklama = (h.aciklama ?? "").Replace("'", ""),
                        tertip = h.tertip,
                        tertipAd = TertipAd(h.tertip),
                        kesinti = h.kesinti,
                        kesintiAd = EvetHayir(h.kesinti),
                        altHesap = h.altHesapKod,
                        altHesapAd = EvetHayir(h.altHesapKod),
                        bakiye = h.bakiye,
                        bakiyeAd = BakiyeAd(h.bakiye),
                        nitelik = h.nitelik ?? "",
                        nitelikAd = NitelikMetni(h.nitelik),
                        dovizKurKod = h.dovizKurKod ?? "",
                        satisKarsilikTur = h.dovizKurTurSatis ?? "",
                        alisKarsilikTur = h.dovizKurTurAlis ?? "",
                        alacakHesabi = h.kurFarkiAlacakHesabi ?? "",
                        borcHesabi = h.kurFarkiBorcHesabi ?? "",
                        ekoBasla = h.ekoBasla
                    });
                if (satirlar.Count == 0) return Results.Json(new { satirlar, uyari = "Verilen kriterlere uygun kayıt bulunamadı." });
                return Results.Json(new { satirlar });
            });
        }

        /// <summary>Excele Aktar (eski btnExceleAktar_Click): yıl ve hesap kodu kıstasıyla MUH-Bos.xlt'ye.</summary>
        private static IResult Excel(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string hesapKod)
        {
            return Oku(baglam, k => HesapPlaniYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                HesapPlaniSatir kriter = new HesapPlaniSatir();
                kriter.yil = yil ?? 0;
                kriter.hesapKod = Temiz(hesapKod);
                ObjectArray yler = Servis().HesapPlaniListele(kullanan, kriter);
                if (!yler.sonuc.islemSonuc) return Hata(HataMetni(yler.sonuc.hataStr, "Liste okunamadı."));
                List<object[]> satirlar = new List<object[]>();
                foreach (HesapPlaniSatir y in yler.objeler)
                    satirlar.Add(new object[] { y.yil, y.hesapKod, y.aciklama, y.tertip, y.kesinti, y.nitelik });
                return BosExceleYaz(ortam.ContentRootPath, satirlar, "HesapPlan", (1, 17), (2, 50));
            }, "Rapor hazırlanamadı.");
        }

        private static Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, HesapPlaniIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => HesapPlaniYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null) return Hata("İstek okunamadı.");
                List<string> hatalar = new List<string>();
                if (istek.yil <= 0) hatalar.Add("Yıl bilgisi boş bırakılmış.");
                if (Temiz(istek.hesapKod) == "") hatalar.Add("Hesap kodu boş bırakılmış.");
                if (Temiz(istek.aciklama) == "") hatalar.Add("Açıklama boş bırakılmış.");
                if (hatalar.Count > 0) return Hata(string.Join("\n", hatalar) + "\nKayıt işlemi yapılamadı.");
                ObjectArray l = new ObjectArray();
                l.objeler.Add(Satir(istek));
                return SonucYaniti(Servis().HesapPlaniKaydet(kullanan, l), "Kayıt işlemi başarıyla tamamlandı.");
            });
        }

        private static Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, HesapPlaniIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => HesapPlaniYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null) return Hata("İstek okunamadı.");
                List<string> hatalar = new List<string>();
                if (istek.yil <= 0) hatalar.Add("Yıl bilgisi boş bırakılmış.");
                if (Temiz(istek.hesapKod) == "") hatalar.Add("Hesap kodu boş bırakılmış.");
                if (hatalar.Count > 0) return Hata(string.Join("\n", hatalar) + "\nKayıt işlemi yapılamadı.");
                ObjectArray l = new ObjectArray();
                l.objeler.Add(new HesapPlaniSatir { yil = istek.yil, hesapKod = Temiz(istek.hesapKod) });
                Sonuc s = Servis().HesapPlaniSil(kullanan, l);
                return s.islemSonuc ? Tamam("Silme işlemi başarıyla gerçekleşti.") : Hata(HataMetni(s.hataStr, "Silme işlemi yapılamadı."));
            });
        }

        private static Task<IResult> SonrakiYil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, YilIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => HesapPlaniYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null || istek.yil <= 0) return Hata("Yıl bilgisi boş bırakılmış.");
                return SonucYaniti(Servis().HesapPlaniAktar(kullanan, istek.yil, istek.yil + 1));
            });
        }

        private static Task<IResult> DetayGuncelle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, YilIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => HesapPlaniYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null || istek.yil <= 0) return Hata("Yıl bilgisi boş bırakılmış.");
                Servis().HesapPlaniDetaySayilariDuzenle(istek.yil);
                return Tamam("Detay sayıları güncellendi.");
            });
        }

        private static Task<IResult> BirimDoldur(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            return Yaz(baglam, sahtecilik, k => HesapPlaniYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                Servis().DetayBirimDoldur();
                return Tamam("Detay birim bilgileri dolduruldu.");
            });
        }

        /// <summary>Toplu Nitelik Değiştirme (eski HesapPlaniNitelik.aspx, btnGuncelle_Click): islem 0 Ekleme, 1 Kaldırma.</summary>
        private static Task<IResult> NitelikDegistir(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, HesapPlaniIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => HesapPlaniYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null) return Hata("İstek okunamadı.");
                List<string> hatalar = new List<string>();
                if (istek.yil <= 0) hatalar.Add("Yıl bilgisi boş bırakılmış.");
                if (Temiz(istek.hesapKod) == "") hatalar.Add("Hesap kodu boş bırakılmış.");
                if (hatalar.Count > 0) return Hata(string.Join("\n", hatalar));
                HesapPlaniSatir hs = new HesapPlaniSatir();
                hs.yil = istek.yil;
                hs.hesapKod = Temiz(istek.hesapKod);
                hs.tertip = istek.tertip;
                hs.kesinti = istek.kesinti;
                hs.altHesapKod = istek.altHesap;
                hs.bakiye = istek.bakiye;
                hs.ekoBasla = istek.ekoBasla;
                hs.nitelik = istek.nitelik ?? "";
                Sonuc s = Servis().HesapPlaniNitelikDegistir(kullanan, hs, istek.islem == 1 ? 1 : 0);
                return s.islemSonuc ? Tamam("Güncelleme işlemi başarıyla gerçekleşti.") : Hata(HataMetni(s.hataStr, "Güncelleme yapılamadı."));
            });
        }

        // ------------------------------------------------------------------ yansıtmalar

        private static string Noktasiz(string kod) { return Temiz(kod).Replace(".", ""); }

        /// <summary>Hesabın yansıtmaları, sıra numarasına göre. Servis LIKE ile aradığı için tam eşleşenler burada süzülür.</summary>
        private static List<HesapPlaniYansitma> Yansitmalar(int yil, string hesapKod, out string hata)
        {
            hata = "";
            List<HesapPlaniYansitma> sonuc = new List<HesapPlaniYansitma>();
            HesapPlaniYansitma kriter = new HesapPlaniYansitma { yil = yil, hesapKod = Temiz(hesapKod), yansitmaHesapKod = "" };
            ObjectArray yler = Servis().YansitmaListele(kriter);
            if (!yler.sonuc.islemSonuc) { hata = HataMetni(yler.sonuc.hataStr, "Yansıtmalar okunamadı."); return sonuc; }
            string aranan = Noktasiz(hesapKod);
            foreach (HesapPlaniYansitma y in yler.objeler)
                if (Noktasiz(y.hesapKod) == aranan) sonuc.Add(y);
            sonuc.Sort((a, b) => a.satir.CompareTo(b.satir));
            return sonuc;
        }

        /// <summary>Hesap planındaki kaydı (tam eşleşen kod); yoksa null.</summary>
        private static HesapPlaniSatir HesapBul(Kullanici kullanan, int yil, string hesapKod)
        {
            if (Temiz(hesapKod) == "") return null;
            try
            {
                ObjectArray hler = Servis().HesapPlaniListele(kullanan, new HesapPlaniSatir { yil = yil, hesapKod = Temiz(hesapKod), tertip = -1, kesinti = -1, altHesapKod = -1, bakiye = -1, nitelik = "" });
                if (hler.sonuc.islemSonuc)
                    foreach (HesapPlaniSatir h in hler.objeler)
                        if (Noktasiz(h.hesapKod) == Noktasiz(hesapKod)) return h;
            }
            catch (Exception) { }
            return null;
        }

        /// <summary>Yansıtmada hesap ve yansıtma hesabı hesap planında kayıtlı ve son seviye (detay) olmalı.</summary>
        private static string SonSeviyeDenetle(Kullanici kullanan, int yil, string hesapKod, string ad)
        {
            HesapPlaniSatir h = HesapBul(kullanan, yil, hesapKod);
            if (h == null) return ad + " " + Temiz(hesapKod) + " " + yil + " yılı hesap planında kayıtlı değil.";
            if (!h.detay) return ad + " " + Temiz(hesapKod) + " son seviye bir hesap değil.";
            return "";
        }

        private static IResult YansitmaListe(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string hesapKod)
        {
            return Oku(baglam, k => HesapPlaniYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if ((yil ?? 0) <= 0 || Temiz(hesapKod) == "") return Results.Json(new { satirlar = new List<object>() });
                List<HesapPlaniYansitma> yler = Yansitmalar(yil.Value, hesapKod, out string hata);
                if (hata != "") return Hata(hata);
                List<object> satirlar = new List<object>();
                foreach (HesapPlaniYansitma y in yler)
                    satirlar.Add(new
                    {
                        siraNo = y.satir,
                        yansitmaHesapKod = y.yansitmaHesapKod ?? "",
                        aciklama = HesapBul(kullanan, yil.Value, y.yansitmaHesapKod)?.aciklama ?? ""
                    });
                return Results.Json(new { satirlar });
            });
        }

        /// <summary>Yansıtmaları Excel'e aktar (eski TanimYansitma Excel'e Aktar): yılın bütün yansıtmaları, hesap kodu verilmişse onunla başlayanlar.</summary>
        private static IResult YansitmaExcel(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string hesapKod)
        {
            return Oku(baglam, k => HesapPlaniYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if ((yil ?? 0) <= 0) return Hata("Yıl bilgisi boş bırakılmış.");
                HesapPlaniYansitma kriter = new HesapPlaniYansitma { yil = yil.Value, hesapKod = Temiz(hesapKod), yansitmaHesapKod = "" };
                ObjectArray yler = Servis().YansitmaListele(kriter);
                if (!yler.sonuc.islemSonuc) return Hata(HataMetni(yler.sonuc.hataStr, "Liste okunamadı."));
                List<object[]> satirlar = new List<object[]>();
                foreach (HesapPlaniYansitma y in yler.objeler)
                    satirlar.Add(new object[] { y.yil, y.hesapKod, y.yansitmaHesapKod, y.satir });
                return BosExceleYaz(ortam.ContentRootPath, satirlar, "Yansitma", (1, 17), (2, 17));
            }, "Rapor hazırlanamadı.");
        }

        /// <summary>Ekle: hesap ve yansıtma hesabı kayıtlı ve son seviye olmalı. Sıra no verilmemişse hesabın son sıra numarasının bir fazlası. Aynı yansıtma hesabı varsa sıra no güncellenir.</summary>
        private static Task<IResult> YansitmaEkle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, YansitmaIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => HesapPlaniYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null) return Hata("İstek okunamadı.");
                List<string> hatalar = new List<string>();
                if (istek.yil <= 0) hatalar.Add("Yıl bilgisi boş bırakılmış.");
                if (Temiz(istek.hesapKod) == "") hatalar.Add("Önce hesabı seçin.");
                if (Temiz(istek.yansitmaHesapKod) == "") hatalar.Add("Yansıtma hesap kodu boş bırakılmış.");
                if (hatalar.Count > 0) return Hata(string.Join("\n", hatalar));
                if (Noktasiz(istek.hesapKod) == Noktasiz(istek.yansitmaHesapKod)) return Hata("Hesap kendisine yansıtılamaz.");
                foreach (string hata1 in new[]
                {
                    SonSeviyeDenetle(kullanan, istek.yil, istek.hesapKod, "Hesap"),
                    SonSeviyeDenetle(kullanan, istek.yil, istek.yansitmaHesapKod, "Yansıtma hesabı")
                })
                    if (hata1 != "") hatalar.Add(hata1);
                if (hatalar.Count > 0) return Hata(string.Join("\n", hatalar));

                List<HesapPlaniYansitma> mevcut = Yansitmalar(istek.yil, istek.hesapKod, out string hata);
                if (hata != "") return Hata(hata);
                int siraNo = OrtakFonksiyonlar.ConvertToInt(Temiz(istek.siraNo), 0);
                if (siraNo <= 0)
                {
                    foreach (HesapPlaniYansitma y in mevcut)
                    {
                        if (Noktasiz(y.yansitmaHesapKod) == Noktasiz(istek.yansitmaHesapKod)) return Hata("Bu yansıtma hesabı zaten ekli.");
                        if (y.satir > siraNo) siraNo = y.satir;
                    }
                    siraNo++;
                }

                ObjectArray l = new ObjectArray();
                l.objeler.Add(new HesapPlaniYansitma { yil = istek.yil, hesapKod = Temiz(istek.hesapKod), yansitmaHesapKod = Temiz(istek.yansitmaHesapKod), satir = siraNo });
                Sonuc s = Servis().YansitmaKaydet(l);
                return s.islemSonuc ? Tamam("Yansıtma eklendi.") : Hata(HataMetni(s.hataStr, "Kayıt işlemi yapılamadı."));
            });
        }

        private static Task<IResult> YansitmaSil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, YansitmaIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => HesapPlaniYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null || istek.yil <= 0 || Temiz(istek.hesapKod) == "" || Temiz(istek.yansitmaHesapKod) == "")
                    return Hata("Lütfen silmek istediğiniz yansıtmayı seçin.");
                ObjectArray l = new ObjectArray();
                l.objeler.Add(new HesapPlaniYansitma
                {
                    yil = istek.yil, hesapKod = Temiz(istek.hesapKod), yansitmaHesapKod = Temiz(istek.yansitmaHesapKod),
                    satir = OrtakFonksiyonlar.ConvertToInt(Temiz(istek.siraNo), 0)
                });
                Sonuc s = Servis().YansitmaSil(l);
                return s.islemSonuc ? Tamam("Yansıtma silindi.") : Hata(HataMetni(s.hataStr, "Silme işlemi yapılamadı."));
            });
        }

        private static Task<IResult> YansitmaSonrakiYil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, YilIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => HesapPlaniYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null || istek.yil <= 0) return Hata("Yıl bilgisi boş bırakılmış.");
                return SonucYaniti(Servis().YansitmaAktar(kullanan, istek.yil, istek.yil + 1));
            });
        }
    }
}
