using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Istemci.ButceHazirlik;
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
using TNS.MUH;
using TNS.Raporlama;

namespace Istemci.Muhasebe
{
    public class OgSatirIstegi
    {
        public string Tertip { get; set; }
        public double Miktar { get; set; }
    }

    public class OgKaydetIstegi
    {
        public int BelgeTur { get; set; }
        public string Talep { get; set; }
        public int Yil { get; set; }
        public string BelgeTarih { get; set; }
        public string BelgeNo { get; set; }
        public int Muhasebe { get; set; }
        public string Birim { get; set; }
        public int IsKodu { get; set; }
        public string Aciklama { get; set; }
        public string YaziNo { get; set; }
        public string YaziTarih { get; set; }
        public string IstekYaziNo { get; set; }
        public string IstekYaziTarih { get; set; }
        public int SurecYil { get; set; }
        public string SurecNo { get; set; }
        /// <summary>Dönem ödeneği gönder (eski chkPlanlananOdenek).</summary>
        public bool Planlanan { get; set; }
        /// <summary>Muh. Harcama Kontrolü Yapma (eski cbMuhasebeHarcamaKontrolu).</summary>
        public bool MuhHarcamaKontrolYapma { get; set; }
        public List<OgSatirIstegi> Satirlar { get; set; }
    }

    public class OgBelgeIstegi
    {
        public int BelgeTur { get; set; }
        public string Talep { get; set; }
        public int Yil { get; set; }
        public string BelgeNo { get; set; }
    }

    public class OgDurumIstegi
    {
        public int BelgeTur { get; set; }
        public string Talep { get; set; }
        public int Yil { get; set; }
        /// <summary>sgb, onayla, muhasebat, iptal, tahsis, yeniyap, degisimonayla, talepbelge, icmalno, muhaldi, ebutce, ac, kapa</summary>
        public string Islem { get; set; }
        public List<string> Belgeler { get; set; }
        public string Gerekce { get; set; }
        public string IcmalNo { get; set; }
    }

    /// <summary>
    /// Ödenek Gönderme / Tenkis belge işlemleri (eski ButceMuhasebe/OdenekGondermeTenkisFrame.aspx ve iki alt sayfası:
    /// OdenekGondermeTenkisMPExt.aspx = Belge Düzenleme, OdenekGondermeTenkisSorgu.aspx = Belge Arama). Menüdeki dört seçenek
    /// belgetur (3 ödenek gönderme, 4 tenkis) ve talep=1 ile ayrılır. Bütün uçlar bt (belge türü) ve talep alır; yetki menüdeki
    /// çerçeve adresinden ve eski TNS.MUH.Yetki.YetkisiVarMi denetiminden gelir.
    ///   Form : GET tanim, belge, satir, yazdir, hizli-rapor, tertipler, tertip-detay, prg, kodlar, kod-ad, ioi
    ///          POST kaydet, talep-gonder, revize-iptal, dosya-yukle (multipart)
    ///   Liste: GET liste, liste/detay, liste/yazdir, belgeler-yazdir (zip), icmal-yazdir, tarihce
    ///          POST durum (seçili belgelerin durumu, icmal no, eBütçe, birimlere aç / kapa)
    /// </summary>
    public static class OdenekGondermeUclari
    {
        public const string FrameAdres = "../ButceMuhasebe/OdenekGondermeTenkisFrame.aspx";
        public const int OdenekGondermeTuru = (int)ENUMBelgeTuru.ODENEKGONDERME;
        public const int TenkisTuru = (int)ENUMBelgeTuru.TENKIS;
        private const string Uc = "/api/odenekgonderme/";
        private const int EkleSatirSayisi = 20;

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "tanim", Tanim);
            app.MapGet(Uc + "belge", Belge);
            app.MapGet(Uc + "satir", Satir);
            app.MapPost(Uc + "kaydet", Kaydet);
            app.MapPost(Uc + "talep-gonder", TalepGonder);
            app.MapPost(Uc + "revize-iptal", RevizeIptal);
            app.MapGet(Uc + "yazdir", Yazdir);
            app.MapGet(Uc + "hizli-rapor", HizliRapor);
            app.MapPost(Uc + "dosya-yukle", DosyaYukle).DisableAntiforgery();
            app.MapGet(Uc + "tertipler", Tertipler);
            app.MapGet(Uc + "tertip-detay", TertipDetay);
            app.MapGet(Uc + "prg", Prg);
            app.MapGet(Uc + "kodlar", Kodlar);
            app.MapGet(Uc + "kod-ad", KodAdi);
            app.MapGet(Uc + "ioi", Ioi);

            app.MapGet(Uc + "liste", Liste);
            app.MapGet(Uc + "liste/detay", ListeDetay);
            app.MapGet(Uc + "liste/yazdir", ListeYazdir);
            app.MapGet(Uc + "belgeler-yazdir", BelgelerYazdir);
            app.MapGet(Uc + "icmal-yazdir", IcmalYazdir);
            app.MapGet(Uc + "tarihce", Tarihce);
            app.MapPost(Uc + "durum", Durum);
        }

        // ------------------------------------------------------------------ yetki ve ortak

        public static bool TalepMi(string talep)
        {
            talep = (talep ?? "").Trim();
            return talep != "" && talep != "0";
        }

        /// <summary>Menüdeki adres: ../ButceMuhasebe/OdenekGondermeTenkisFrame.aspx?belgetur=3[&amp;talep=1]</summary>
        public static string Adres(int belgeTur, bool talep)
        {
            return FrameAdres + "?belgetur=" + belgeTur + (talep ? "&talep=1" : "");
        }

        /// <summary>Menü yetkisi ve eski sayfa girişi denetimi (talepte ödeme emri görme yetkisi).</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru, int belgeTur, bool talep)
        {
            if (kullanan == null) return false;
            if (belgeTur != (int)ENUMBelgeTuru.ODENEKGONDERME && belgeTur != (int)ENUMBelgeTuru.TENKIS) return false;
            if (!SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, Adres(belgeTur, talep))) return false;
            try
            {
                return TNS.MUH.Yetki.YetkisiVarMi(kullanan, (int)TNS.MUH.ENUMOGY.Gorme, talep ? (int)ENUMBelgeTuru.ODEMEEMRI : belgeTur);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, int? bt, string talep, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath, bt ?? 0, TalepMi(talep))) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static bool Yonetici(Kullanici kullanan)
        {
            return kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
        }

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        private static IResult Basari(string bilgi) { return Results.Json(new { basarili = true, bilgi = bilgi ?? "" }); }

        private static IResult Basarisiz(string hata) { return Results.Json(new { basarili = false, hata = hata ?? "" }); }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = Temiz(hata);
            return hata == "" ? varsayilan : hata;
        }

        /// <summary>yyyy-MM-dd -> TNSDateTime (boşsa boş tarih).</summary>
        private static TNSDateTime Tarih(string s)
        {
            s = Temiz(s);
            if (s != "" && DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime t))
                return new TNSDateTime(t);
            return new TNSDateTime(DateTime.MinValue);
        }

        private static string TarihYaz(TNSDateTime t)
        {
            return t == null || t.isNull ? "" : t.Oku().ToString("yyyy-MM-dd");
        }

        private static string Kurum()
        {
            return GenelIslemlerIstemci.VarsayilanKurumBul() ?? "";
        }

        /// <summary>Kısa belge numarasının başına kurum kodu (eski Kurumlu: 4 + 5 hane).</summary>
        public static string Kurumlu(string belgeNo)
        {
            belgeNo = Temiz(belgeNo);
            if (belgeNo.Length < 9)
                belgeNo = Kurum().Replace(".", "").PadRight(4, '0') + belgeNo.PadLeft(5, '0');
            return belgeNo;
        }

        private static string Baslik(int belgeTur, bool talep)
        {
            if (belgeTur == (int)ENUMBelgeTuru.ODENEKGONDERME)
                return talep ? "Ödenek Gönderme Belgesi İşlemleri (Talep)" : "Ödenek Gönderme Belgesi İşlemleri";
            return talep ? "Tenkis Belgesi İşlemleri (Talep)" : "Tenkis Belgesi İşlemleri";
        }

        private static string PrgKod(int yil, string[] kodlar)
        {
            string peb = OrtakFonksiyonlar.DiziElemanAl(kodlar, 4, "") ?? "";
            return TNS.BHMv2.Arac.ProgramButceKodaCevir(yil, 140, peb.Replace(".", "")) + "-" + OrtakFonksiyonlar.DiziElemanAl(kodlar, 0, "")
                + "-" + OrtakFonksiyonlar.DiziElemanAl(kodlar, 2, "") + "-" + OrtakFonksiyonlar.DiziElemanAl(kodlar, 3, "");
        }

        /// <summary>Eski ListeTertip PrgKodYap: faaliyet kodu yoksa boş.</summary>
        private static string PrgKodYap(int yil, string tertip)
        {
            string[] kodlar = (tertip ?? "").Split('-');
            if (string.IsNullOrWhiteSpace(TNS.MUH.Arac.DiziElemanAl(kodlar, 4, ""))) return "";
            return PrgKod(yil, kodlar);
        }

        // ------------------------------------------------------------------ açılış

        /// <summary>Kurum kodu listesi (eski BirimDoldur / DDLBirimDoldur).</summary>
        private static List<object> Kurumlar(Kullanici kullanan, bool talep)
        {
            List<object> kurumlar = new List<object>();
            ABSKodlarVAN akv = TNS.BKD.Arac.Tanimla().BirimListele(kullanan, DateTime.Today.Year, Kurum(), true);
            if (akv == null || !akv.sonuc.islemSonuc || akv.absKodlar == null) return kurumlar;
            bool yonetici = Yonetici(kullanan);
            string[] yetkili = (kullanan.YetkiTipiDegeriBul("Ödenek Gönderme Hazırlayacağı Birimler") ?? "").Split(';');
            if (talep) kurumlar.Add(new { kod = "", ad = "" });
            foreach (ABSKod ak in akv.absKodlar)
            {
                bool ekle = talep || yonetici;
                for (int i = 0; i < yetkili.Length && !ekle; i++)
                    if (!string.IsNullOrEmpty(yetkili[i]) && ak.kod.StartsWith(yetkili[i]))
                        ekle = true;
                if (ekle)
                    kurumlar.Add(new { kod = ak.kod, ad = ak.kod + " / " + ak.Ad(kullanan.aktifKultur) });
            }
            return kurumlar;
        }

        private static List<object> Durumlar(bool talep, int yil, out int varsayilan, out bool sgbGoster)
        {
            List<object> d = new List<object>();
            sgbGoster = false;
            void Ekle(int kod, string ad) { d.Add(new { kod = kod, ad = ad }); }
            if (talep)
            {
                Ekle(90, "Yeni Talepler");
                Ekle(99, "Yapılan Talepler");
                Ekle(89, "Tüm Talepler");
                Ekle(96, "Karşılanan Talepler");
                Ekle(5, "Muhasebatta Onaylananlar");
                Ekle(22, "Bütün Belgeler (İptaller Hariç)");
                Ekle(9, "İptal Edilenler");
                varsayilan = 90;
            }
            else
            {
                Ekle(1, "Yeni Belgeler");
                if (TNS.MUH.Arac.GelismisIcmalKontrol(yil))
                {
                    Ekle(6, "SGBye gönderilenler");
                    sgbGoster = true;
                }
                Ekle(20, "İcmal No Verilmemişler");
                Ekle(12, "Tahsis Edilenler");
                Ekle(99, "Talep Edilenler");
                Ekle(21, "Muhasebata Gönderilecekler");
                Ekle(3, "Muhasebata Gönderilenler");
                Ekle(4, "Muhasebatın Aldıkları");
                Ekle(5, "Muhasebatta Onaylananlar");
                Ekle(8, "Muhasebatın İade Ettikleri");
                Ekle(9, "İptal Edilenler");
                Ekle(22, "Bütün Belgeler (İptaller Hariç)");
                Ekle(96, "Karşılanan Talepler");
                varsayilan = 1;
            }
            return d;
        }

        /// <summary>Açılış (eski FormAyarla / Page_Load, iki alt sayfa için).</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam, int? bt, string talep)
        {
            IResult ret = Denetle(baglam, ortam, bt, talep, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                bool talepMi = TalepMi(talep);
                int belgeTur = bt.Value;
                int yil = DateTime.Now.Year;
                bool yonetici = Yonetici(kullanan);
                string kurum = Kurum();

                bool odenekDegisimGoster = false;
                if (belgeTur == (int)ENUMBelgeTuru.ODENEKGONDERME && !talepMi
                    && TNS.MUH.Arac.OdenekKontrolTurBul(yil) != ENUMOdenekKontrolTur.E2OG_E4OE_E2K)
                    odenekDegisimGoster = true;
                if (kurum.StartsWith("30") && !yonetici)
                    odenekDegisimGoster = false;

                bool muhHarcamaGoster = yonetici && TNS.MUH.Arac.DegiskenDegerBul(0, "SGBKBSKontrol") == "1";
                bool mpOnayli = TNS.BHMv2.Arac.Tanimla().MasrafPlaniGenelOnayDurumBul(kullanan, yil, "", ENUMMasrafPlanAsama.AHP) == 5;

                List<int> yillar = new List<int>();
                for (int i = yil; i >= 2006; i--) yillar.Add(i);

                List<object> durumlar = Durumlar(talepMi, yil, out int varsayilanDurum, out bool sgbGoster);
                bool muhasebatYok = TNS.MUH.Arac.MuhasebatYok();

                return Results.Json(new
                {
                    baslik = Baslik(belgeTur, talepMi),
                    yil = yil,
                    yillar = yillar,
                    bugun = DateTime.Today.ToString("yyyy-MM-dd"),
                    kurum = kurum,
                    kurumlar = Kurumlar(kullanan, talepMi),
                    yonetici = yonetici,
                    mpOnayli = mpOnayli,
                    planlananGoster = odenekDegisimGoster && yonetici,
                    muhHarcamaGoster = muhHarcamaGoster,
                    satirSayisi = EkleSatirSayisi,
                    durumlar = durumlar,
                    varsayilanDurum = varsayilanDurum,
                    sgbGoster = sgbGoster && !muhasebatYok,
                    muhasebatYok = muhasebatYok
                });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Sayfa bilgileri okunamadı."));
            }
        }

        // ------------------------------------------------------------------ form: belge

        private static object BelgeJson(Kullanici kullanan, GondermeTenkisForm form)
        {
            double belgeToplami = 0;
            string kurumsal = "";
            List<object> satirlar = new List<object>();
            foreach (GondermeTenkisDetay detay in form.detay.objeler)
            {
                satirlar.Add(new
                {
                    TERTIP = detay.tertip.TertipStrOl(),
                    TERTIPPRG = PrgKod(form.yil, detay.tertip.kodlar),
                    MIKTAR = detay.miktar,
                    KULLANILABILEN = detay.kullanilabilir,
                    GONDERILEN = detay.gonderilen,
                    HARCANAN = detay.harcanan,
                    DONEMMP = detay.donemMP,
                    TOPLAMMP = detay.toplamMP
                });
                kurumsal = detay.tertip.kodlar[0];
                belgeToplami += detay.miktar;
            }
            bool talepDurum = form.formDurum == (int)ENUMBelgeDurumu.TALEP || form.formDurum == (int)ENUMBelgeDurumu.YENITALEP;
            return new
            {
                belgeNo = form.belgeNo,
                belgeTarih = TarihYaz(form.islemTarih),
                muhasebe = form.muhasebe,
                muhasebeAd = form.muhasebeAd,
                birim = form.birim,
                birimAd = form.birimAd,
                isKodu = form.isKodu,
                isAdi = form.isAdi,
                aciklama = form.aciklama,
                yaziNo = form.yaziNo,
                yaziTarih = TarihYaz(form.yaziTarih),
                istekYaziNo = form.istekYaziNo,
                istekYaziTarih = TarihYaz(form.istekYaziTarih),
                surecYil = form.surecYil,
                surecNo = form.surecNo,
                surecYilNo = form.surecYilNo,
                formDurum = form.formDurum,
                durum = TNS.MUH.Arac.FormDurumAciklama(form.formDurum, form.belgeTur),
                vergiNo = form.vergiNo,
                ekDosyaAd1 = form.ekDosyaAd1,
                ekDosyaAd2 = form.ekDosyaAd2,
                ekliDosyalarGoster = talepDurum,
                talepGonderGoster = form.formDurum == (int)ENUMBelgeDurumu.YENITALEP,
                revizeIptalGoster = form.formDurum == (int)ENUMBelgeDurumu.MUHASEBATIADE && Yonetici(kullanan),
                kurumsal = kurumsal,
                toplam = belgeToplami,
                satirlar = satirlar
            };
        }

        /// <summary>Belge No ile listele (eski Listele / BelgeFormaYaz).</summary>
        private static IResult Belge(HttpContext baglam, IWebHostEnvironment ortam, int? bt, string talep, int? yil, string belgeNo)
        {
            IResult ret = Denetle(baglam, ortam, bt, talep, out Kullanici kullanan);
            if (ret != null) return ret;
            belgeNo = Temiz(belgeNo);
            string hata = "";
            if (belgeNo == "")
                hata = "Belge listelemesi için, Belge No alanına görmek istediğiniz belgenin nosunu yazmanız gerekmektedir.";
            if ((yil ?? 0) == 0)
                hata += (hata != "" ? " " : "") + "Belge listelemesi için, Yıl alanına görmek istediğiniz belgenin yılını yazmanız gerekmektedir.";
            if (hata != "") return Hata(hata);
            try
            {
                ObjectArray fler = OdenekGondermeYazdirma.Listele(kullanan, TNS.MUH.Arac.Tanimla(), yil.Value, (ENUMBelgeTuru)bt.Value, Kurumlu(belgeNo));
                if (!fler.sonuc.islemSonuc) return Hata(HataMetni(fler.sonuc.hataStr, "Belge okunamadı."));
                if (fler.objeler.Count == 0) return Hata(string.Format("Görmek istediğiniz {0} nolu belge bulunamadı", belgeNo));
                return Results.Json(new { belge = BelgeJson(kullanan, (GondermeTenkisForm)fler.objeler[0]) });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Belge okunamadı."));
            }
        }

        /// <summary>Tertip değişince satırın kullanılabilir / gönderilen / harcanan bilgileri (eski OdenekBilgiGoster).</summary>
        private static IResult Satir(HttpContext baglam, IWebHostEnvironment ortam, int? bt, string talep, int? yil, string tertip, string muhasebe, string birim, string planlanan)
        {
            IResult ret = Denetle(baglam, ortam, bt, talep, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                int iyil = yil ?? 0;
                GondermeTenkisDetay sd = new GondermeTenkisDetay();
                sd.tertip.TertipAta(Temiz(tertip));
                sd = TNS.MUH.Arac.Tanimla().SatirDurumBul(kullanan, iyil, Temiz(muhasebe), Temiz(birim), (ENUMBelgeTuru)bt.Value, sd, planlanan == "true" || planlanan == "1", TalepMi(talep));
                if (bt.Value == (int)ENUMBelgeTuru.ODENEKGONDERME && TalepMi(talep))
                    sd.kullanilabilir = 0;
                return Results.Json(new
                {
                    kullanilabilir = sd.kullanilabilir,
                    gonderilen = sd.gonderilen,
                    harcanan = sd.harcanan,
                    donemMP = sd.donemMP,
                    toplamMP = sd.toplamMP,
                    tertipPrg = PrgKod(iyil, sd.tertip.kodlar)
                });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Tertip bilgileri okunamadı."));
            }
        }

        /// <summary>Talepte kullanılamayacak ekonomik kodlar (eski KullanilmayacakEkoKod).</summary>
        private static Sonuc KullanilmayacakEkoKod(GondermeTenkisForm form)
        {
            Sonuc sonuc = new Sonuc();
            sonuc.islemSonuc = true;
            string kurum = Kurum();
            bool bulundu = kurum == "12" || kurum == "0023" || kurum == "5030";
            if (bulundu && form.belgeTur != ENUMBelgeTuru.TENKIS)
            {
                foreach (GondermeTenkisDetay d in form.detay.objeler)
                {
                    if (TNS.DEG.Arac.OdenekGondermeYasakliMi(d.tertip.kodlar[3]))
                    {
                        sonuc.hataStr += string.Format("{0} ekonomik koduna ait Ödenek Gönderme talebinde bulunamazsınız. Bu tertip ile ilgili talepler Harcama Yönetimi Modülünden Süreç oluşturarak gerçekleştirmeniz gerekmektedir.", d.tertip.kodlar[3]) + "\n";
                        sonuc.islemSonuc = false;
                    }
                }
            }
            return sonuc;
        }

        /// <summary>Kaydet (eski btnKaydet_Click).</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, OgKaydetIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            istek ??= new OgKaydetIstegi();
            IResult ret = Denetle(baglam, ortam, istek.BelgeTur, istek.Talep, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                bool talepMi = TalepMi(istek.Talep);
                GondermeTenkisForm form = new GondermeTenkisForm();
                form.belgeTur = (ENUMBelgeTuru)istek.BelgeTur;
                form.yil = istek.Yil;
                form.islemTarih = Tarih(istek.BelgeTarih);
                form.belgeNo = Temiz(istek.BelgeNo);
                form.kurum = Kurum();
                form.muhasebe = istek.Muhasebe;
                form.birim = Temiz(istek.Birim);
                form.isKodu = istek.IsKodu;
                form.aciklama = istek.Aciklama ?? "";
                form.islemYapan = kullanan.kullaniciKodu;
                form.yaziNo = talepMi ? "" : (istek.YaziNo ?? "");
                form.yaziTarih = talepMi ? Tarih("") : Tarih(istek.YaziTarih);
                form.istekYaziNo = istek.IstekYaziNo ?? "";
                form.istekYaziTarih = Tarih(istek.IstekYaziTarih);
                form.surecYil = istek.SurecYil;
                form.surecNo = istek.SurecNo ?? "";

                if (form.belgeNo == "")
                    form.icmalNo = 0;

                if (talepMi)
                    form.talep = "1";

                foreach (OgSatirIstegi s in istek.Satirlar ?? new List<OgSatirIstegi>())
                {
                    if (s == null || string.IsNullOrWhiteSpace(s.Tertip) || s.Miktar == 0) continue;
                    GondermeTenkisDetay detay = new GondermeTenkisDetay(form.yil);
                    detay.tertip.TertipAta(s.Tertip.Trim());
                    detay.miktar = s.Miktar;
                    form.detay.objeler.Add(detay);
                }

                Sonuc sonuc = form.talep == "1" ? KullanilmayacakEkoKod(form) : new Sonuc(true, "", "");
                if (!sonuc.islemSonuc) return Basarisiz(sonuc.hataStr);

                bool planlanan = istek.Planlanan && Yonetici(kullanan);
                bool muhHarcama = istek.MuhHarcamaKontrolYapma && Yonetici(kullanan);
                sonuc = TNS.MUH.Arac.Tanimla().GondermeTenkisKaydet(kullanan, form, true, planlanan, muhHarcama);
                if (!sonuc.islemSonuc)
                    return Basarisiz("Belge kaydı yapılamadı. Hata : " + sonuc.hataStr);

                // Belge numarası değiştiyse (ör. yeni belge) belge yeniden okunur.
                bool yenidenOku = form.belgeNo == "" || Kurumlu(form.belgeNo) != sonuc.anahtar;
                return Results.Json(new
                {
                    basarili = true,
                    bilgi = sonuc.bilgiStr ?? "",
                    belgeNo = sonuc.anahtar,
                    yenidenOku = yenidenOku
                });
            }
            catch (Exception ex)
            {
                return Basarisiz("Belge kaydı yapılamadı. Hata : " + ex.Message);
            }
        }

        /// <summary>Talep Gönder (eski btnTalepGonder_Click: durumu TALEP yapar).</summary>
        private static async Task<IResult> TalepGonder(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, OgBelgeIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            istek ??= new OgBelgeIstegi();
            IResult ret = Denetle(baglam, ortam, istek.BelgeTur, istek.Talep, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                GondermeTenkisForm form = new GondermeTenkisForm();
                form.belgeTur = (ENUMBelgeTuru)istek.BelgeTur;
                form.yil = istek.Yil;
                form.belgeNo = Temiz(istek.BelgeNo);
                form.kurum = Kurum();
                form.formDurum = (int)ENUMBelgeDurumu.TALEP;
                form.talep = "1";
                TNSCollection fler = new TNSCollection();
                fler.Add(form);
                Sonuc sonuc = TNS.MUH.Arac.Tanimla().BelgeDurumDegistirGonTen(kullanan, fler);
                return sonuc.islemSonuc ? Basari("Talep iletildi") : Basarisiz("Talep iletilemedi:" + sonuc.hataStr);
            }
            catch (Exception ex)
            {
                return Basarisiz("Talep iletilemedi:" + ex.Message);
            }
        }

        /// <summary>Revize İptal (eski btnMPRevizeIptal_Click; yalnızca sistem yöneticisi, muhasebatın iade ettiği belgede).</summary>
        private static async Task<IResult> RevizeIptal(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, OgBelgeIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            istek ??= new OgBelgeIstegi();
            IResult ret = Denetle(baglam, ortam, istek.BelgeTur, istek.Talep, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!Yonetici(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            MPRevizeForm mpRevizeForm = new MPRevizeForm();
            mpRevizeForm.formDurum = ENUMRevizeFormDurum.Iptal;
            mpRevizeForm.odGonNo = Temiz(istek.BelgeNo);
            mpRevizeForm.odGonTur = istek.BelgeTur;
            mpRevizeForm.odGonYil = istek.Yil;
            string belge = string.Format("{0}-{1}-{2}", mpRevizeForm.odGonYil, mpRevizeForm.odGonTur, mpRevizeForm.odGonNo);
            try
            {
                TNSCollection tc = new TNSCollection();
                tc.Add(mpRevizeForm);
                Sonuc s = TNS.BHMv2.Arac.Tanimla().MPRevizeFormDurumAta(kullanan, tc);
                return s.islemSonuc
                    ? Basari(belge + " belgesi ile ilgili revize iptal edildi")
                    : Basarisiz(belge + " belgesi ile ilgili revize iptal edilirken hata oluştu: " + s.hataStr);
            }
            catch (Exception ex)
            {
                return Basarisiz(belge + " belgesi ile ilgili revize iptal edilirken hata oluştu: " + ex.Message);
            }
        }

        /// <summary>Yazdır (eski btnYazdir_Click: belge çıktısı Excel).</summary>
        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, int? bt, string talep, int? yil, string belgeNo)
        {
            IResult ret = Denetle(baglam, ortam, bt, talep, out Kullanici kullanan);
            if (ret != null) return ret;
            if (Temiz(belgeNo) == "") return Hata("Yazdırmak için önce belgeyi kaydediniz ya da listeleyiniz.");
            try
            {
                AsposeLisans.Yukle();
                string hata = OdenekGondermeYazdirma.Yazdir(kullanan, yil ?? 0, (ENUMBelgeTuru)bt.Value, Kurumlu(belgeNo), "", out string dosya, out string ad);
                if (dosya == null) return Hata(HataMetni(hata, "Belge yazdırılamadı."));
                return DosyaIslem.DosyaGonder(dosya, ad, true, GenelIslemler.ExcelTur());
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Belge yazdırılamadı."));
            }
        }

        /// <summary>Hızlı rapor "KBS-SGB Fark" (eski ButceMuhasebeGenel.HizliRaporAl, kod 1: rapor 5000-13). Seyyar Görev sayfada açılır.</summary>
        private static IResult HizliRapor(HttpContext baglam, IWebHostEnvironment ortam, int? bt, string talep, string kod, int? yil, int? muhasebe, string birim)
        {
            IResult ret = Denetle(baglam, ortam, bt, talep, out Kullanici kullanan);
            if (ret != null) return ret;
            if (kod != "1") return Hata("Hazır Değil");
            int iyil = yil ?? -1;
            Hashtable raporParam = new Hashtable();
            raporParam["ButceYil"] = iyil.ToString();
            int ay = DateTime.Now.Month;
            if (iyil < DateTime.Now.Year) ay = 12;
            raporParam["AyNo"] = ay.ToString();
            raporParam["Muhasebe"] = (muhasebe ?? -1).ToString();
            raporParam["Birim"] = Temiz(birim);
            raporParam["Kur"] = Kurum();
            raporParam["EkoKirilim"] = "4";
            raporParam["CiktiYeri"] = "EXCEL";
            try
            {
                AsposeLisans.Yukle();
                string dosya = RaporlamaServis.TekRaporAl(5000, 13, kullanan, null, raporParam, null);
                if (RaporlamaServis.MesajMi(dosya)) return Hata(dosya);
                if (string.IsNullOrEmpty(dosya) || !File.Exists(dosya)) return Hata("Rapor hazırlanamadı.");
                string uzanti = Path.GetExtension(dosya).TrimStart('.').ToLowerInvariant();
                return DosyaIslem.DosyaGonder(dosya, "KBS_SGB_Fark_" + DosyaIslem.DosyaAdUretSade(), true, uzanti);
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }

        /// <summary>İmzalı Belge (dosyaNo 1) / Destekleyici Belge (2) yükleme (eski fuBelge_FileSelected; modül OGT, form yil-belgeTur-belgeNo).</summary>
        private static async Task<IResult> DosyaYukle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (!baglam.Request.HasFormContentType) return Results.BadRequest();
            IFormCollection istek = await baglam.Request.ReadFormAsync();
            int belgeTur = OrtakFonksiyonlar.ConvertToInt((string)istek["bt"], 0);
            IResult ret = Denetle(baglam, ortam, belgeTur, istek["talep"], out Kullanici kullanan);
            if (ret != null) return ret;

            int dosyaNo = OrtakFonksiyonlar.ConvertToInt((string)istek["dosyaNo"], 1);
            if (dosyaNo != 1 && dosyaNo != 2) dosyaNo = 1;
            IFormFile dosya = istek.Files["dosya"];
            if (dosya == null || dosya.Length == 0) return Basarisiz("Lütfen dosya seçiniz!");
            string belgeNo = Temiz(istek["belgeNo"]);
            if (belgeNo == "") return Basarisiz("Dosya yüklemek için önce belgeyi kaydediniz.");

            try
            {
                byte[] icerik;
                using (MemoryStream bellek = new MemoryStream())
                {
                    await dosya.CopyToAsync(bellek);
                    icerik = bellek.ToArray();
                }

                GondermeTenkisForm form = new GondermeTenkisForm();
                form.belgeTur = (ENUMBelgeTuru)belgeTur;
                form.yil = OrtakFonksiyonlar.ConvertToInt((string)istek["yil"], 0);
                form.belgeNo = belgeNo;

                string ad = Path.GetFileName(dosya.FileName);
                Sonuc s = TNS.KYM.Arac.Tanimla().KaydetEkliDosya(kullanan, new EkliDosyalar()
                {
                    ekliDosyaIcerik = icerik,
                    modul = "OGT",
                    form = form.DosyaAnahtarOl(),
                    dosyaNo = dosyaNo,
                    dosyaAdi = ad
                });
                return s.islemSonuc
                    ? Results.Json(new { basarili = true, bilgi = "Dosya yüklendi!", ad = ad })
                    : Basarisiz("Dosya yüklenemedi:" + s.hataStr);
            }
            catch (Exception ex)
            {
                return Basarisiz("Dosya yüklenemedi:" + ex.Message);
            }
        }

        // ------------------------------------------------------------------ tertip seçimi (eski ButceHazirlikv2/ListeTertip.aspx ve ButceMuhasebe/ListeTertip.aspx)

        internal static int TertipSayiKontrol(int gelenSay, ref int maxTertipSay, ref string uyari)
        {
            int satSay = gelenSay;
            if (satSay > maxTertipSay * 1.5)
            {
                uyari = string.Format("Verdiğiniz arama sonucunda {0} adet tertip listelenmiştir. Ekranda bu tertiplerden {1} adedi görüntülenmektedir. Lütfen kıstas alanlarına bilgi girerek gelecek listenin boyunu azaltınız!", satSay, maxTertipSay);
                satSay = 30;
                maxTertipSay += 30;
            }
            return satSay;
        }

        /// <summary>
        /// Tertip listesi. Ödenek göndermede (talep değilse) eski ButceHazirlikv2/ListeTertip (ödenek - gönderilen, birim bilgileriyle,
        /// çoklu seçim); diğerlerinde eski ButceMuhasebe/ListeTertip (TNS.BHMv2.Arac.TertipListele; ekonomik kodun alt düzeyi seçilerek).
        /// İlk açılışta (ilk=1) kurumsal kod boşsa muhasebe - birimin kurumsal kodları kullanılır; en az iki alan dolu olmalıdır.
        /// </summary>
        private static IResult Tertipler(HttpContext baglam, IWebHostEnvironment ortam, int? bt, string talep, int? yil, int? muhasebe, string birim,
            string kur, string eko, string peb, string hepsi, int? sayi, string ilk)
        {
            IResult ret = Denetle(baglam, ortam, bt, talep, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                int iyil = yil ?? DateTime.Now.Year;
                if (iyil > DateTime.Now.Year) iyil = DateTime.Now.Year;
                int belgeTur = bt.Value;
                bool bhm = belgeTur == (int)ENUMBelgeTuru.ODENEKGONDERME && !TalepMi(talep);
                int muh = muhasebe ?? 0;
                birim = Temiz(birim);
                kur = Temiz(kur).Replace(";", ",");
                eko = Temiz(eko).Replace(";", ",");
                peb = Temiz(peb).Replace(";", ",");
                bool tumu = hepsi == "1";
                int maxTertipSay = sayi ?? 30;
                if (maxTertipSay <= 0) maxTertipSay = 30;
                ENUMOdenekKontrolTur okt = TNS.MUH.Arac.OdenekKontrolTurBul(iyil);

                bool anaGoster = true;
                if (!bhm)
                {
                    // Eski ButceMuhasebe/ListeTertip açılışı: muhasebe - birimin kurumsal kodları.
                    if (ilk == "1" && muh > 0 && birim != "")
                    {
                        TNS.MUH.MuhasebeBirim mb = new TNS.MUH.MuhasebeBirim();
                        mb.muhasebe = muh;
                        mb.birimKod = birim;
                        ObjectArray mbler = TNS.MUH.Arac.Tanimla().MuhasebeBirimListele(mb);
                        string kurKod = "";
                        if (mbler != null && mbler.objeler != null)
                            foreach (TNS.MUH.MuhasebeBirim mm in mbler.objeler)
                            {
                                if (kurKod != "") kurKod += ",";
                                kurKod += mm.kurum;
                            }
                        if (kurKod != "") kur = kurKod;
                    }
                    if (ilk == "1" && kur == "" && muh > 0 && birim != "")
                        kur = Temiz(TNS.HRC.Arac.Tanimla().MuhasebeBirimKurumsalKodAl(muh, birim));

                    anaGoster = false;
                    if (okt != ENUMOdenekKontrolTur.E4POG_E4POE_E2K)
                    {
                        if (belgeTur == 4 || okt == ENUMOdenekKontrolTur.E4POG_E4POE_E4PK)
                            anaGoster = true;
                        else if (belgeTur == 3 && okt != ENUMOdenekKontrolTur.E2POG_E4POE_E2PK_E4PTALEP)
                            anaGoster = true;
                    }
                    else if (belgeTur == 4)
                        anaGoster = true;
                }

                int doluAlan = (kur != "" ? 1 : 0) + (eko != "" ? 1 : 0) + (peb != "" ? 1 : 0);
                if (doluAlan < 2)
                {
                    return Results.Json(new
                    {
                        bhm = bhm,
                        anaGoster = anaGoster,
                        kur = kur,
                        eko = eko,
                        uyari = ilk == "1" ? "" : "Lütfen kriter bölümünde, birden fazla alana değer giriniz.",
                        satirlar = new List<object>(),
                        sayi = maxTertipSay
                    });
                }

                string uyari = "";
                List<object> liste = new List<object>();
                bool mpOnayli;
                string odenekBaslik = "Ödenek";
                IBHMv2Servis servisBHMv2 = TNS.BHMv2.Arac.Tanimla();

                if (bhm)
                {
                    OdenekBilgi[] obler = BhmTertipler(kullanan, servisBHMv2, iyil, okt, kur, eko, peb, tumu, out mpOnayli, out odenekBaslik);
                    OdenekBilgi[] birimTertipleri = BhmBirimTertipler(kullanan, servisBHMv2, iyil, belgeTur, okt, muh, birim, kur, eko, peb, tumu);
                    int satSay = TertipSayiKontrol(obler.Length, ref maxTertipSay, ref uyari);
                    foreach (OdenekBilgi ob in obler)
                    {
                        if (satSay != 0 && liste.Count >= satSay) break;
                        string tertip = ob.tertip.TertipStrOl();
                        string[] kodlar = tertip.Split('-');
                        string pebKod = TNS.MUH.Arac.DiziElemanAl(kodlar, 4, "");
                        double odenek = (double)ob.ekRakamlar[0];
                        double gonderilen = (double)ob.ekRakamlar[1];
                        double birimHarcanan = 0, birimGonderilen = 0, birimBloke = 0, mp = 0;
                        OdenekBilgi bob = TertipBul(birimTertipleri, tertip);
                        if (bob != null)
                        {
                            birimHarcanan = (double)bob.ekRakamlar[1];
                            birimGonderilen = (double)bob.ekRakamlar[0];
                            birimBloke = (double)bob.ekRakamlar[2];
                            if (mpOnayli)
                                mp = (double)bob.ekRakamlar[3];
                        }
                        liste.Add(new
                        {
                            TERTIP = TNS.MUH.Arac.DiziElemanAl(kodlar, 0, "") + "-" + TNS.MUH.Arac.DiziElemanAl(kodlar, 1, "") + "-" + TNS.MUH.Arac.DiziElemanAl(kodlar, 2, "")
                                + "-" + TNS.MUH.Arac.DiziElemanAl(kodlar, 3, "") + (pebKod != "" ? "-" + pebKod : ""),
                            TERTIPPRG = TNS.BHMv2.Arac.ProgramTertipOl(iyil, TNS.MUH.Arac.DiziElemanAl(kodlar, 0, ""), TNS.MUH.Arac.DiziElemanAl(kodlar, 1, ""),
                                TNS.MUH.Arac.DiziElemanAl(kodlar, 2, ""), TNS.MUH.Arac.DiziElemanAl(kodlar, 3, ""), pebKod, false),
                            KALAN = odenek - gonderilen,
                            ODENEK = odenek,
                            GONDERILEN = gonderilen,
                            BIRIMKALAN = birimGonderilen - birimHarcanan - birimBloke,
                            BIRIMGONDERILEN = birimGonderilen,
                            BIRIMKULLANILAN = birimHarcanan + birimBloke,
                            MP = mp,
                            ACIKLAMA = ob.tertip.adlar.Length > 3 ? ob.tertip.adlar[3] : "",
                            FAALIYETACIKLAMA = ob.tertip.adlar.Length >= 5 ? ob.tertip.adlar[4] : ""
                        });
                    }
                }
                else
                {
                    ENUMOdenekUstuKontrol ouKontrol = TNS.MUH.Arac.OdenekUstuKontrolBul();
                    OdenekBilgi[] obler = TNS.BHMv2.Arac.TertipListele(kullanan, servisBHMv2, belgeTur, iyil, muh, birim, kur, "", "", eko, peb, okt, ouKontrol, tumu, out mpOnayli);
                    int satSay = TertipSayiKontrol(obler.Length, ref maxTertipSay, ref uyari);
                    foreach (OdenekBilgi ob in obler)
                    {
                        if (satSay != 0 && liste.Count >= satSay) break;
                        string tertip = ob.tertip.TertipStrOl();
                        string[] kodlar = tertip.Split('-');
                        string pebKod = TNS.MUH.Arac.DiziElemanAl(kodlar, 4, "");
                        double odenek = (double)ob.ekRakamlar[0];
                        double harcanan = (double)ob.ekRakamlar[1];
                        double bloke = (double)ob.ekRakamlar[2];
                        double mp = mpOnayli ? (double)ob.ekRakamlar[3] : 0;
                        liste.Add(new
                        {
                            TERTIP = TNS.MUH.Arac.DiziElemanAl(kodlar, 0, "") + "-" + TNS.MUH.Arac.DiziElemanAl(kodlar, 1, "") + "-" + TNS.MUH.Arac.DiziElemanAl(kodlar, 2, "")
                                + "-" + TNS.MUH.Arac.DiziElemanAl(kodlar, 3, "") + (pebKod != "" ? "-" + pebKod : ""),
                            TERTIPPRG = PrgKod(iyil, kodlar),
                            KALAN = odenek - harcanan - bloke,
                            ODENEK = odenek,
                            HARCANAN = harcanan,
                            BLOKE = bloke,
                            MP = mp,
                            ACIKLAMA = ob.tertip.adlar.Length > 3 ? ob.tertip.adlar[3] : "",
                            FAALIYETACIKLAMA = ob.tertip.adlar.Length >= 5 ? ob.tertip.adlar[4] : ""
                        });
                    }
                }

                return Results.Json(new
                {
                    bhm = bhm,
                    anaGoster = anaGoster,
                    mpOnayli = mpOnayli,
                    odenekBaslik = odenekBaslik,
                    kur = kur,
                    eko = eko,
                    uyari = uyari,
                    satirlar = liste,
                    sayi = maxTertipSay
                });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Tertipler okunamadı."));
            }
        }

        /// <summary>Eski ButceHazirlikv2/ListeTertip TertipleriListele: ödenek ve gönderilen (ListeTertipOl).</summary>
        internal static OdenekBilgi[] BhmTertipler(Kullanici kullanan, IBHMv2Servis servisBHMv2, int yil, ENUMOdenekKontrolTur okt,
            string kur, string eko, string peb, bool tumu, out bool mpOnayli, out string baslik, string fin = "")
        {
            string ekoKistas = eko;
            if (okt == ENUMOdenekKontrolTur.E2OG_E4OE_E2K || okt == ENUMOdenekKontrolTur.E2POG_E4POE_E2PK_E4PTALEP)
                ekoKistas = TNS.BHMv2.Arac.IkileEko(yil, ekoKistas, TNS.BKD.Arac.Eko2_3Bul(kullanan));

            string pebKistas = peb;
            if (okt == ENUMOdenekKontrolTur.E2OG_E4OE_E2K)
                pebKistas = "";
            Tertip kistasTertip = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, kur, "", fin ?? "", ekoKistas, pebKistas);

            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(0), kistasTertip.KodNoKirilimSay(0)));
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(1), kistasTertip.KodNoKirilimSay(1)));
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(2), kistasTertip.KodNoKirilimSay(2)));

            if (okt == ENUMOdenekKontrolTur.E2OG_E4OE_E2K || okt == ENUMOdenekKontrolTur.E2POG_E4POE_E2PK_E4PTALEP)
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), 2, TNS.BKD.Arac.Eko2_3Bul(kullanan)));
            else
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), 4));

            if (okt != ENUMOdenekKontrolTur.E2OG_E4OE_E2K)
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(4), kistasTertip.KodNoKirilimSay(4)));

            EkDegerParam[] ekBilgiler = new EkDegerParam[2];
            mpOnayli = servisBHMv2.MasrafPlaniGenelOnayDurumBul(kullanan, yil, "", ENUMMasrafPlanAsama.AHP) == 5;

            if (mpOnayli)
            {
                int ay = TNS.BHMv2.Arac.SerbestAyBul(yil) + 1;
                ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABMasrafPlaniBirim, 0, 0, "11;0/" + ay.ToString());
                baslik = "Masraf Pl. (1-" + (ay - 1).ToString() + ". aylar)";
            }
            else if (okt == ENUMOdenekKontrolTur.E2OG_E4OE_E2K || okt == ENUMOdenekKontrolTur.E4POG_E4POE_E4PK)
            {
                ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABButce, 0, 0, "06.00");
                baslik = "Ödenek (Son)";
            }
            else
            {
                ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, "06.00");
                baslik = "Baş. Ödeneği";
            }
            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABOdenekGonderme, 0, 0, "");
            return servisBHMv2.ListeTertipOl(kullanan, kistasTertip, kodBilgileri, ekBilgiler, !tumu, tumu) ?? new OdenekBilgi[0];
        }

        /// <summary>Eski ButceHazirlikv2/ListeTertip TertipleriListeleBirim: birime gönderilen, harcanan, bloke (ve masraf planı).</summary>
        internal static OdenekBilgi[] BhmBirimTertipler(Kullanici kullanan, IBHMv2Servis servisBHMv2, int yil, int belgeTur, ENUMOdenekKontrolTur okt,
            int muhasebe, string birim, string kur, string eko, string peb, bool tumu)
        {
            bool mpOnayli = servisBHMv2.MasrafPlaniGenelOnayDurumBul(kullanan, yil, "", ENUMMasrafPlanAsama.AHP) == 5;

            string ortak000 = "";
            try { ortak000 = TNS.DEG.Arac.DegiskenDegerBul(yil, "/Ortak000"); } catch { }
            if (ortak000 == "1")
                birim = "";

            Tertip kistasTertip = new Tertip(yil, ENUMTabloKod.TABOdenekGonderme, kur, "", "", "", "", "@" + muhasebe + "." + birim);

            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(0), kistasTertip.KodNoKirilimSay(0)));
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(1), kistasTertip.KodNoKirilimSay(1)));
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(2), kistasTertip.KodNoKirilimSay(2)));

            string ekoKistas = eko;
            if ((belgeTur != 4 && okt != ENUMOdenekKontrolTur.E4POG_E4POE_E4PK) ||
                (belgeTur == 4 && (okt == ENUMOdenekKontrolTur.E2OG_E4OE_E2K || okt == ENUMOdenekKontrolTur.E2POG_E4POE_E2PK_E4PTALEP)))
            {
                ekoKistas = TNS.BHMv2.Arac.IkileEko(yil, ekoKistas, TNS.BKD.Arac.Eko2_3Bul(kullanan));
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), 2, TNS.BKD.Arac.Eko2_3Bul(kullanan)));
            }
            else
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), kistasTertip.KodNoKirilimSay(3)));

            string pebKistas = peb;
            if ((belgeTur != 4 && (okt == ENUMOdenekKontrolTur.E2OG_E4OE_E2K || okt == ENUMOdenekKontrolTur.E4POG_E4POE_E2K)) ||
                (belgeTur == 4 && okt == ENUMOdenekKontrolTur.E2OG_E4OE_E2K))
                pebKistas = "";
            else
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(4), kistasTertip.KodNoKirilimSay(4)));

            kistasTertip.kodlar[3] = ekoKistas;
            kistasTertip.kodlar[4] = pebKistas;

            EkDegerParam[] ekBilgiler;
            if (mpOnayli)
            {
                ekBilgiler = new EkDegerParam[4];
                ekBilgiler[3] = new EkDegerParam(ENUMTabloKod.TABMasrafPlaniBirim, 0, 0, "11");
            }
            else
                ekBilgiler = new EkDegerParam[3];

            if (TNS.MUH.Arac.OdenekUstuKontrolBul() == ENUMOdenekUstuKontrol.ODENEK)
                ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABButce, 0, 0, "");
            else
                ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABOdenekGonderme, 0, 0, ";");

            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABOdemeEmri, 0, 0, "");
            ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABHYBloke, 0, 0, "");

            return servisBHMv2.ListeTertipOl(kullanan, kistasTertip, kodBilgileri, ekBilgiler, !tumu, tumu) ?? new OdenekBilgi[0];
        }

        internal static OdenekBilgi TertipBul(OdenekBilgi[] liste, string aranan)
        {
            if (liste == null) return null;
            foreach (OdenekBilgi ob in liste)
                if (ob != null && aranan == ob.tertip.TertipStrOl())
                    return ob;
            return null;
        }

        /// <summary>
        /// Satırın ekonomik kodunun alt kodları (eski ButceMuhasebe/ListeTertip BeforeExpand). Faaliyet kodu, ödenek kontrol türü
        /// E4POG_E4POE_E2K ise tertip tanımlarından seçilir.
        /// </summary>
        private static IResult TertipDetay(HttpContext baglam, IWebHostEnvironment ortam, int? bt, string talep, int? yil, string tertip, string eko)
        {
            IResult ret = Denetle(baglam, ortam, bt, talep, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                int iyil = yil ?? DateTime.Now.Year;
                if (iyil > DateTime.Now.Year) iyil = DateTime.Now.Year;
                string[] parca = Temiz(tertip).Split('-');
                if (parca.Length < 4) return Hata("Tertip hatalı.");
                string tertipOn = parca[0] + "-" + parca[1] + "-" + parca[2] + "-";
                string pebKod = TNS.MUH.Arac.DiziElemanAl(parca, 4, "");
                string tertipArka = pebKod != "" ? "-" + pebKod : "";
                ENUMOdenekKontrolTur okt = TNS.MUH.Arac.OdenekKontrolTurBul(iyil);

                ABSKodlarVAN ekoKodlar = TNS.BKD.Arac.Tanimla().KodListeleSeviye(iyil, "4", parca[3], 4, true, false, "", Temiz(eko).Replace(";", ","), null);

                List<object> pebler = new List<object>();
                string varsayilanPeb = "";
                if (okt == ENUMOdenekKontrolTur.E4POG_E4POE_E2K)
                {
                    Tertip kistasTertip = new Tertip(iyil, ENUMTabloKod.TABPEBOdenek, parca[0], parca[1], parca[2], parca[3]);
                    Tertip[] tler = TNS.BHMv2.Arac.Tanimla().TertipTanimlariListele(kullanan, kistasTertip);
                    List<string> eklenen = new List<string>();
                    if (tler != null && tler.Length > 0)
                    {
                        foreach (Tertip t in tler)
                        {
                            string icPeb = t.kodlar[4];
                            if (eklenen.Contains(icPeb)) continue;
                            eklenen.Add(icPeb);
                            pebler.Add(new { kod = icPeb, ad = GenelIslemler.KodAd(58, iyil + "-" + icPeb, true) });
                        }
                    }
                    else
                    {
                        eklenen.Add("01.01");
                        pebler.Add(new { kod = "01.01", ad = "Genel İşlemler" });
                    }
                    varsayilanPeb = eklenen[0];
                }

                List<object> satirlar = new List<object>();
                if (ekoKodlar != null && ekoKodlar.absKodlar != null)
                    foreach (ABSKod kod in ekoKodlar.absKodlar)
                    {
                        string t = tertipOn + kod.kod + tertipArka;
                        satirlar.Add(new { TERTIP = t, TERTIPPRG = PrgKodYap(iyil, t), EKO = kod.kod, PEB = varsayilanPeb, ACIKLAMA = kod.ad });
                    }
                return Results.Json(new { satirlar = satirlar, pebler = pebler });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Alt kodlar okunamadı."));
            }
        }

        /// <summary>Program bütçe kodu (eski ListeTertip TertipPrgBul).</summary>
        private static IResult Prg(HttpContext baglam, IWebHostEnvironment ortam, int? bt, string talep, int? yil, string tertip)
        {
            IResult ret = Denetle(baglam, ortam, bt, talep, out Kullanici kullanan);
            if (ret != null) return ret;
            try { return Results.Json(new { prg = PrgKodYap(yil ?? DateTime.Now.Year, Temiz(tertip)) }); }
            catch (Exception) { return Results.Json(new { prg = "" }); }
        }

        /// <summary>Kod ağacı (Liste süzgeçleri; 1 kurumsal, 2 fonksiyonel, 3 finans, 4 ekonomik, 14 faaliyet).</summary>
        private static IResult Kodlar(HttpContext baglam, IWebHostEnvironment ortam, int? bt, string talep, int yil, string tur, string ust)
        {
            IResult ret = Denetle(baglam, ortam, bt, talep, out Kullanici kullanan);
            if (ret != null) return ret;
            tur = Temiz(tur);
            if (tur != "1" && tur != "2" && tur != "3" && tur != "4" && tur != "14") return Hata("Kod türü hatalı.");
            return GorusmeCariUclari.KodDuzeyi(kullanan, yil, tur, ust, false);
        }

        /// <summary>Tertip penceresindeki kodların adı (eski ListeTertip KodAdGetir: Kur 1, Eko 4, Peb 58).</summary>
        private static IResult KodAdi(HttpContext baglam, IWebHostEnvironment ortam, int? bt, string talep, int? yil, string tur, string kod)
        {
            IResult ret = Denetle(baglam, ortam, bt, talep, out Kullanici kullanan);
            if (ret != null) return ret;
            kod = Temiz(kod);
            if (kod == "") return Results.Json(new { ad = "" });
            int kodTur = tur == "kur" ? 1 : tur == "eko" ? 4 : tur == "peb" ? 58 : 0;
            if (kodTur == 0) return Hata("Kod türü hatalı.");
            try
            {
                string ad = GenelIslemler.KodAd(kodTur, (yil ?? DateTime.Now.Year) + "-" + kod, true);
                return Results.Json(new { ad = string.IsNullOrWhiteSpace(ad) ? "Hatalı kod girilmiş." : ad });
            }
            catch (Exception)
            {
                return Results.Json(new { ad = "Hatalı kod girilmiş." });
            }
        }

        /// <summary>İÖİ / YİKOB iş seçimi (eski ButceHazirlikv2/IOIProjeSecim.aspx: ListeleIOIProjeGiris). isTur: 1 İÖİ, 2 YİKOB, 0 hepsi.</summary>
        private static IResult Ioi(HttpContext baglam, IWebHostEnvironment ortam, int? bt, string talep, int? yil, int? muhasebe, string birim, string ad, int? isTur)
        {
            IResult ret = Denetle(baglam, ortam, bt, talep, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                IOIProje p = new IOIProje();
                p.ioiProjeAdi = Temiz(ad);
                p.muhasebeKod = muhasebe ?? 0;
                p.birimKod = Temiz(birim);
                p.isTur = isTur ?? 0;

                ObjectArray oa = TNS.BHMv2.Arac.Tanimla().ListeleIOIProjeGiris(kullanan, p, false, yil ?? DateTime.Now.Year);
                if (!oa.sonuc.islemSonuc) return Hata(HataMetni(oa.sonuc.hataStr, "İşler okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (IOIProje i in oa.objeler)
                    satirlar.Add(new
                    {
                        siraNo = i.siraNo,
                        ioiProjeAdi = i.ioiProjeAdi,
                        ilAdi = i.ilAdi,
                        aktarilanTutar = i.aktarilanTutar,
                        islemleEklenen = i.islemleEklenen,
                        iOIHarc = i.iOIHarc,
                        gonderilen = i.gonderilen
                    });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "İşler okunamadı."));
            }
        }

        // ------------------------------------------------------------------ liste (eski OdenekGondermeTenkisSorgu)

        /// <summary>Sorgu kıstasları (eski KriterTopla). Hata olursa hata doldurulur, null döner.</summary>
        private static GondermeTenkisForm KriterTopla(HttpRequest r, int belgeTur, bool talep, out string hata)
        {
            hata = null;
            IQueryCollection q = r.Query;
            string Al(string ad) { return Temiz(q[ad]); }

            GondermeTenkisForm f = new GondermeTenkisForm();
            f.yil = OrtakFonksiyonlar.ConvertToInt(Al("yil"), 0);
            f.formDurum = OrtakFonksiyonlar.ConvertToInt(Al("durum"), 0);
            if (f.yil == 0 || f.formDurum == 0)
            {
                hata = "Sorgulama için yıl alanını boş bırakamazsınız.";
                return null;
            }

            f.belgeTur = (ENUMBelgeTuru)belgeTur;
            f.kurum = Kurum();
            f.muhasebe = OrtakFonksiyonlar.ConvertToInt(Al("muhasebe"), 0);
            f.birim = Al("birim");

            if (Al("icmalNo") != "")
                f.icmalNo = OrtakFonksiyonlar.ConvertToInt(Al("icmalNo"), 0);

            string b1 = Al("belgeNo1");
            string b2 = Al("belgeNo2");
            if (b1 != "" || b2 != "")
            {
                if (b1 != "" && b1.Length < 9)
                    b1 = f.kurum.Replace(".", "").PadRight(4, '0') + b1.PadLeft(6, '0');
                if (b2 != "" && b2.Length < 9)
                    b2 = f.kurum.Replace(".", "").PadRight(4, '0') + b2.PadLeft(6, '0');
                f.belgeNo = b1 + ";" + b2;
            }

            f.sorguIcmalTarih = Tarih(Al("icmalTarihi")).ToString();
            f.islemYapan = Al("islemYapan");
            f.aciklama = Al("aciklama");

            if (Al("belgeTarihi1") != "")
                f.sorguIslemTarih = Tarih(Al("belgeTarihi1")).ToString() + ";";
            if (Al("belgeTarihi2") != "")
                f.sorguIslemTarih += Tarih(Al("belgeTarihi2")).ToString();

            if (Al("durumTarihi1") != "")
                f.sorguFormDurumTarih = Tarih(Al("durumTarihi1")).ToString() + ";";
            if (Al("durumTarihi2") != "")
                f.sorguFormDurumTarih += Tarih(Al("durumTarihi2")).ToString();

            GondermeTenkisDetay detay = new GondermeTenkisDetay(f.yil);
            detay.tertip.kodlar[0] = Al("kur");
            detay.tertip.kodlar[1] = Al("fon");
            detay.tertip.kodlar[2] = Al("fin");
            detay.tertip.kodlar[3] = Al("eko");
            detay.tertip.kodlar[4] = Al("peb");
            f.tutarBE = OrtakFonksiyonlar.ConvertToDbl(Al("tutarbe"));
            f.tutarKE = OrtakFonksiyonlar.ConvertToDbl(Al("tutarke"));
            f.surecNo = Al("surecno");
            f.DortDuzeyDegisim = Al("dortDuzeyDegisim").ToLowerInvariant() == "true" ? 1 : 0;

            // taşra: talep sayfalarında bütün belgeler
            f.tasra = talep ? 1 : 0;

            f.detay.objeler.Add(detay);
            return f;
        }

        /// <summary>Listele (eski btnListe_Click).</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int? bt, string talep)
        {
            IResult ret = Denetle(baglam, ortam, bt, talep, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                GondermeTenkisForm f = KriterTopla(baglam.Request, bt.Value, TalepMi(talep), out string hata);
                if (f == null) return Hata(hata);
                ObjectArray fler = TNS.MUH.Arac.Tanimla().GondermeTenkisListele(kullanan, f, 0);
                if (fler == null || !fler.sonuc.islemSonuc) return Hata(HataMetni(fler?.sonuc.hataStr, "Belgeler listelenemedi."));
                List<object> satirlar = new List<object>();
                foreach (GondermeTenkisForm ff in fler.objeler)
                {
                    satirlar.Add(new
                    {
                        belgeNo = ff.belgeNo,
                        belgeTarih = TarihYaz(ff.islemTarih),
                        muhasebe = ff.muhasebe.ToString().PadLeft(5, '0') + " - " + ff.muhasebeAd,
                        birim = ff.birim + " - " + ff.birimAd,
                        tutar = ff.belgeToplami,
                        icmalNo = ff.icmalNo,
                        icmalTarih = TarihYaz(ff.icmalTarih),
                        muhasebatNo = ff.onayNo,
                        muhasebatTarih = TarihYaz(ff.onayTarih),
                        formDurum = ff.formDurum,
                        durum = TNS.MUH.Arac.FormDurumAciklama(ff.formDurum, ff.belgeTur),
                        islemTarih = TarihYaz(ff.formDurumTarih),
                        islemKisi = ff.islemYapan,
                        aciklama = ff.aciklama,
                        iptalNeden = ff.iptalNeden
                    });
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Belgeler listelenemedi."));
            }
        }

        /// <summary>Satır ayrıntısı: belgenin tertipleri (eski BeforeExpand).</summary>
        private static IResult ListeDetay(HttpContext baglam, IWebHostEnvironment ortam, int? bt, string talep, int? yil, string belgeNo)
        {
            IResult ret = Denetle(baglam, ortam, bt, talep, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                ObjectArray fler = OdenekGondermeYazdirma.Listele(kullanan, TNS.MUH.Arac.Tanimla(), yil ?? 0, (ENUMBelgeTuru)bt.Value, Temiz(belgeNo), TalepMi(talep) ? 1 : 0);
                if (!fler.sonuc.islemSonuc) return Hata(HataMetni(fler.sonuc.hataStr, "Belge okunamadı."));
                List<object> satirlar = new List<object>();
                if (fler.objeler.Count > 0)
                    foreach (GondermeTenkisDetay detay in ((GondermeTenkisForm)fler.objeler[0]).detay.objeler)
                        satirlar.Add(new { tertip = detay.tertip.TertipStrOl(), tutar = detay.miktar });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Belge okunamadı."));
            }
        }

        /// <summary>Liste Yazdır (eski btnListeYazdir_Click; tur 1 başlıklar, 2 detaylı eko 2, 3 detaylı faaliyet).</summary>
        private static IResult ListeYazdir(HttpContext baglam, IWebHostEnvironment ortam, int? bt, string talep, string tur)
        {
            IResult ret = Denetle(baglam, ortam, bt, talep, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                GondermeTenkisForm f = KriterTopla(baglam.Request, bt.Value, TalepMi(talep), out string hata);
                if (f == null) return Hata(hata);
                int raporNo = OrtakFonksiyonlar.ConvertToInt(tur, 1);
                if (raporNo < 1 || raporNo > 3) raporNo = 1;
                string sabDosyaAd = raporNo == 1 ? "MUH-OdenekGondermeSorguListe" : "MUH-OdenekGondermeSorguDetay";
                AsposeLisans.Yukle();
                string sonucDosyaAd = new OdenekGondermeRaporlar().RaporAl(kullanan, OdenekGondermeYazdirma.SablonYol(sabDosyaAd + ".xlt"),
                    OdenekGondermeRaporlar.RaporParamOl(f), raporNo.ToString());
                return DosyaIslem.DosyaGonder(sonucDosyaAd, sabDosyaAd + DateTime.Now.ToString("HHmmss"), true, GenelIslemler.ExcelTur());
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }

        private static List<string> BelgeListesi(string belgeler)
        {
            List<string> liste = new List<string>();
            foreach (string b in (belgeler ?? "").Split(';', ','))
                if (Temiz(b) != "" && !liste.Contains(Temiz(b))) liste.Add(Temiz(b));
            return liste;
        }

        /// <summary>Seçili belgelerin çıktıları tek zip dosyasında (eski btnBelgeYazdir_Click; System.IO.Compression).</summary>
        private static IResult BelgelerYazdir(HttpContext baglam, IWebHostEnvironment ortam, int? bt, string talep, int? yil, string belgeler)
        {
            IResult ret = Denetle(baglam, ortam, bt, talep, out Kullanici kullanan);
            if (ret != null) return ret;
            List<string> liste = BelgeListesi(belgeler);
            if (liste.Count == 0) return Hata("Listeden herhangi bir belge seçilmediği için işlem yapılamadı.");
            string klasor = DosyaIslem.DosyaAdUret("Belgeler_", ".dir");
            try
            {
                AsposeLisans.Yukle();
                Directory.CreateDirectory(klasor);
                string sonHata = "";
                foreach (string belgeNo in liste)
                {
                    string ad = string.Join("_", belgeNo.Split(Path.GetInvalidFileNameChars()));
                    string hata = OdenekGondermeYazdirma.Yazdir(kullanan, yil ?? 0, (ENUMBelgeTuru)bt.Value, belgeNo,
                        Path.Combine(klasor, ad + "." + GenelIslemler.ExcelTur()), out string dosya, out string gidenAd);
                    if (dosya == null) sonHata = hata;
                }
                if (Directory.GetFiles(klasor).Length == 0) return Hata(HataMetni(sonHata, "Belgeler yazdırılamadı."));

                string zip = DosyaIslem.DosyaAdUret("Belgeler_", ".zip");
                ZipFile.CreateFromDirectory(klasor, zip);
                return DosyaIslem.DosyaGonder(zip, "Belgeler.zip", true, "zip");
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Belgeler yazdırılamadı."));
            }
            finally
            {
                try { if (Directory.Exists(klasor)) Directory.Delete(klasor, true); } catch { }
            }
        }

        /// <summary>İcmal Yazdır (eski btnIcmalYazdir_Click).</summary>
        private static IResult IcmalYazdir(HttpContext baglam, IWebHostEnvironment ortam, int? bt, string talep, int? yil, string icmalNo)
        {
            IResult ret = Denetle(baglam, ortam, bt, talep, out Kullanici kullanan);
            if (ret != null) return ret;
            int no = OrtakFonksiyonlar.ConvertToInt(Temiz(icmalNo), 0);
            if (no == 0) return Hata("Yazdırılacak İcmal numarası belirtilmemiş.");
            try
            {
                AsposeLisans.Yukle();
                string hata = OdenekGondermeYazdirma.IcmalBelgesiYazdir(kullanan, yil ?? 0, (ENUMBelgeTuru)bt.Value, Kurum(), no, out string dosya);
                if (dosya == null) return Hata(HataMetni(hata, "İcmal yazdırılamadı."));
                return DosyaIslem.DosyaGonder(dosya, "IcmalListesi_" + no, true, GenelIslemler.ExcelTur());
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "İcmal yazdırılamadı."));
            }
        }

        /// <summary>Belge tarihçesi (eski ButceMuhasebe/BelgeTarihce.aspx).</summary>
        private static IResult Tarihce(HttpContext baglam, IWebHostEnvironment ortam, int? bt, string talep, int? yil, string belgeNo)
        {
            IResult ret = Denetle(baglam, ortam, bt, talep, out Kullanici kullanan);
            if (ret != null) return ret;
            if ((yil ?? 0) == 0 || Temiz(belgeNo) == "") return Hata("Yıl ve/ya Belge No bilgisi alınamadı.");
            try
            {
                ObjectArray tVAN = TNS.MUH.Arac.Tanimla().TarihceListele(kullanan, yil.Value, (ENUMBelgeTuru)bt.Value, Temiz(belgeNo));
                if (!tVAN.sonuc.islemSonuc) return Hata(HataMetni(tVAN.sonuc.hataStr, "Tarihçe okunamadı."));
                List<object> satirlar = new List<object>();
                int sira = 0;
                foreach (TarihceBilgisi t in tVAN.objeler)
                {
                    sira++;
                    satirlar.Add(new { sira = sira, islemTarih = t.islemTarih.ToLongString(), islem = t.islem, islemYapan = t.islemYapan, islemYapanAd = t.islemYapanAd });
                }
                return Results.Json(new { belge = yil + " / " + Temiz(belgeNo), satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Tarihçe okunamadı."));
            }
        }

        /// <summary>
        /// Seçili belgelerin durum değişiklikleri (eski BelgeDurumDegisikligi ve düğmeleri), eBütçe Servise Gönder
        /// (OdenekGondermeBelgeleriGonder) ve Birimlere Aç / Kapa (OnayKodAtaTasra).
        /// </summary>
        private static async Task<IResult> Durum(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, OgDurumIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            istek ??= new OgDurumIstegi();
            IResult ret = Denetle(baglam, ortam, istek.BelgeTur, istek.Talep, out Kullanici kullanan);
            if (ret != null) return ret;

            bool talepMi = TalepMi(istek.Talep);
            bool yonetici = Yonetici(kullanan);
            bool muhasebatYok = TNS.MUH.Arac.MuhasebatYok();
            string islem = Temiz(istek.Islem).ToLowerInvariant();
            IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();

            try
            {
                // Birimlere Aç / Kapa: belge seçimi gerekmez (yalnızca sistem yöneticisi).
                if (islem == "ac" || islem == "kapa")
                {
                    if (!yonetici) return Results.StatusCode(StatusCodes.Status403Forbidden);
                    ENUMTasraHazirlikOnayDurum onayKod = islem == "kapa" ? ENUMTasraHazirlikOnayDurum.KurumOnayla : ENUMTasraHazirlikOnayDurum.KurumOnayKaldir;
                    GondermeTenkisForm form = new GondermeTenkisForm();
                    form.belgeTur = (ENUMBelgeTuru)istek.BelgeTur;
                    form.yil = istek.Yil;
                    if (talepMi) form.talep = "1";
                    Sonuc s = TNS.BHMv2.Arac.Tanimla().OnayKodAtaTasra(kullanan, form.yil, 0, "000", form.OnayKodBul(), ENUMBirimOnayTur.ODENEKGONDERME, onayKod);
                    return s.islemSonuc ? Basari("Durum Değiştirildi") : Basarisiz(s.hataStr);
                }

                List<string> belgeler = new List<string>();
                foreach (string b in istek.Belgeler ?? new List<string>())
                    if (Temiz(b) != "" && !belgeler.Contains(Temiz(b))) belgeler.Add(Temiz(b));
                if (belgeler.Count == 0) return Basarisiz("Listeden herhangi bir belge seçilmediği için işlem yapılamadı.");

                if (islem == "ebutce")
                {
                    if (!yonetici || muhasebatYok) return Results.StatusCode(StatusCodes.Status403Forbidden);
                    servisMUH.OdenekGondermeBelgeleriGonder(kullanan, istek.Yil, (ENUMBelgeTuru)istek.BelgeTur, string.Join(";", belgeler));
                    return Basari("İşlem tamamlandı listeyi yenileyiniz.");
                }

                int durum;
                switch (islem)
                {
                    case "sgb": durum = (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER; break;
                    case "onayla": durum = (int)ENUMBelgeDurumu.ONAYLI; break;
                    case "muhasebat": durum = (int)ENUMBelgeDurumu.MUHASEBATAGONDERILDI; break;
                    case "iptal": durum = talepMi ? (int)ENUMBelgeDurumu.TALEPIPTAL : (int)ENUMBelgeDurumu.IPTAL; break;
                    case "tahsis": durum = (int)ENUMBelgeDurumu.TAHSIS; break;
                    case "yeniyap": durum = (int)ENUMBelgeDurumu.BELGEDURUMUYENIYAP; break;
                    case "degisimonayla": durum = (int)ENUMBelgeDurumu.DEGISIMONAYLA; break;
                    case "talepbelge": durum = (int)ENUMBelgeDurumu.TALEPBELGEYAP; break;
                    case "icmalno": durum = (int)ENUMBelgeDurumu.ICMALYEVMIYENOVER; break;
                    case "muhaldi": durum = (int)ENUMBelgeDurumu.MUHASEBATALDI; break;
                    default: return Basarisiz("Tanımsız işlem.");
                }
                if ((islem == "yeniyap" || islem == "muhaldi") && !yonetici) return Results.StatusCode(StatusCodes.Status403Forbidden);

                string icmalNo = Temiz(istek.IcmalNo);
                TNSCollection fler = new TNSCollection();
                foreach (string belgeNo in belgeler)
                {
                    GondermeTenkisForm f = new GondermeTenkisForm();
                    f.yil = istek.Yil;
                    f.kurum = Kurum();
                    f.belgeTur = (ENUMBelgeTuru)istek.BelgeTur;
                    f.belgeNo = belgeNo;
                    f.islemYapan = kullanan.kullaniciKodu;
                    f.formDurum = durum;
                    f.iptalNeden = islem == "iptal" ? istek.Gerekce : null;
                    if (icmalNo != "" && durum == (int)ENUMBelgeDurumu.ICMALYEVMIYENOVER)
                        f.icmalNo = OrtakFonksiyonlar.ConvertToInt(icmalNo, 0);

                    if (OrtakFonksiyonlar.ConvertToDbl(f.belgeNo) > 0)
                        fler.Add(f);
                }

                Sonuc sonuc = durum == (int)ENUMBelgeDurumu.TALEPBELGEYAP
                    ? servisMUH.TalepBelgeYapGonTen(kullanan, fler, false)
                    : servisMUH.BelgeDurumDegistirGonTen(kullanan, fler);

                string olumluBilgi = "";
                string olumsuzBilgi = "";
                if (durum == (int)ENUMBelgeDurumu.MUHASEBATAGONDERILDI || durum == (int)ENUMBelgeDurumu.MUHASEBATALDI)
                {
                    olumluBilgi = "Seçili belgeler başarıyla Muhasebata Gönderildi. ";
                    olumsuzBilgi = "Seçili belgelerin Muhasebata Gönderme işlemleri gerçekleştirilemedi. ";
                }
                else if (durum == (int)ENUMBelgeDurumu.IPTAL)
                {
                    olumluBilgi = "Seçili belgelerin İptal edilme işlemleri başarıyla gerçekleştirildi. ";
                    olumsuzBilgi = "Seçili belgelerin İptal edilme işlemleri gerçekleştirilemedi. ";
                }
                else if (durum == (int)ENUMBelgeDurumu.ICMALYEVMIYENOVER)
                    olumluBilgi = string.Format("Seçili belgelere İcmal Numarası olarak {0} başarıyla verildi. ", sonuc.anahtar);

                if (!sonuc.islemSonuc) return Basarisiz(olumsuzBilgi + sonuc.hataStr);
                return Results.Json(new
                {
                    basarili = true,
                    bilgi = (olumluBilgi + (sonuc.bilgiStr ?? "")).Trim(),
                    icmalNo = durum == (int)ENUMBelgeDurumu.ICMALYEVMIYENOVER ? sonuc.anahtar : null
                });
            }
            catch (Exception ex)
            {
                return Basarisiz(HataMetni(ex.Message, "İşlem yapılamadı."));
            }
        }
    }
}
