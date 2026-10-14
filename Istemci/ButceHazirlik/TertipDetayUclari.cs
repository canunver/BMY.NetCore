using System;
using System.Threading.Tasks;
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
using TNS.UZY;

namespace Istemci.ButceHazirlik
{
    public class TertipDetayIstegi
    {
        public int Yil { get; set; }
        public string Senaryo { get; set; }
        public string Tertip { get; set; }
        public int Tahmin { get; set; }
        public double?[] Degerler { get; set; }
    }

    /// <summary>
    /// Tek tertibin bütçe teklifi detay sayfasının (eski ButceHazirlikv2/ButceTeklifHazirlaDetay.aspx; bütçe teklifi ekranında
    /// satırdaki detay simgesi) uçları:
    ///   GET  /api/tertipdetay/bilgi   kesin hesap, taahhüt (harcama süreci) ve taşınır bilgileri ile 3 yıllık teklif
    ///   POST /api/tertipdetay/kaydet  3 yıllık teklifi saklar (eski btnKaydet_Click)
    /// Sayfayı sistem yöneticisi ve bütçe teklifi ekranının (gider ya da gelir) yetkilisi kullanabilir.
    /// </summary>
    public static class TertipDetayUclari
    {
        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/tertipdetay/bilgi", Bilgi);
            app.MapPost("/api/tertipdetay/kaydet", Kaydet);
        }

        /// <summary>Eski sayfada yetki denetimi yoktu; burada sistem yöneticisi ya da bütçe teklifi ekranının yetkilisi.</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru, int tahmin)
        {
            return ButceRakamUclari.YetkiVar(kullanan, uygulamaKlasoru, 1, tahmin) || ButceRakamUclari.YetkiVar(kullanan, uygulamaKlasoru, 2, tahmin);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, int tahmin, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath, tahmin)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Replace("<br>", " ").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        // Eski GenelIslemler.YuzdeBulStr / DegisimOranBulStr
        private static string YuzdeBulStr(double miktar, double tum)
        {
            if (tum == 0) return "-";
            return ((miktar / tum) * 100).ToString("#,##0.00");
        }

        private static string DegisimOranBulStr(double yeni, double eski) { return YuzdeBulStr(yeni - eski, eski); }

        private static string Tutar(double d) { return d.ToString("#,##0"); }

        // Eski BHMv2Ortak.ParametrikHesapPlanDon: ekonomik kodla ilişkili taşınır hesap kodları (virgülle ayrılmış).
        internal static string ParametrikHesapPlanDon(string ekokod)
        {
            string donus = "";
            if (ekokod.Trim() == "") return donus;
            try
            {
                TNS.MUH.HesapPlaniYansitma[] liste = TNS.MUH.Arac.Tanimla().EkonomikKoddanHesapKodlariGetir(ekokod);
                for (int i = 0; i < liste.Length; i++)
                {
                    if (donus != "") donus += ",";
                    donus += liste[i].hesapKod;
                }
            }
            catch
            {
                return "";
            }
            return donus;
        }

        private static TNS.TMM.DepoDurum DepoKriter(int yil, string muhasebekod, string harcamakod, string hesapkod)
        {
            TNS.TMM.DepoDurum k = new TNS.TMM.DepoDurum();
            k.yil = yil;
            k.hesapPlanKod = hesapkod;
            k.yilDevri = false;
            k.muhasebeKod = muhasebekod;
            k.harcamaKod = harcamakod.Replace(".", "");
            return k;
        }

        // Eski BHMv2Ortak.TasinirDepoDurumuTutarlariGetir: giren, çıkan, kalan tutarlar.
        private static decimal[] TasinirDepoDurumuTutarlariGetir(TNS.TMM.ITMMServis servis, int yil, string muhasebekod, string harcamakod, string hesapkod)
        {
            decimal[] donus = { 0, 0, 0 };
            try
            {
                ObjectArray liste = servis.TasinirDepoDurumu(new Kullanici(), DepoKriter(yil, muhasebekod, harcamakod, hesapkod));
                foreach (TNS.TMM.DepoDurum f in liste.objeler)
                {
                    donus[0] += f.girenTutar;
                    donus[1] += f.cikanTutar;
                    donus[2] += f.kalanTutar;
                }
            }
            catch
            {
            }
            return donus;
        }

        // Eski BHMv2Ortak.DepoDurumGetirDevirsiz: depoda kalan tutar.
        private static decimal DepoDurumGetirDevirsiz(TNS.TMM.ITMMServis servis, int yil, string muhasebekod, string harcamakod, string hesapkod)
        {
            decimal donus = 0;
            try
            {
                ObjectArray liste = servis.TasinirDepoDurumu(new Kullanici(), DepoKriter(yil, muhasebekod, harcamakod, hesapkod));
                foreach (TNS.TMM.DepoDurum f in liste.objeler) donus += f.kalanTutar;
            }
            catch
            {
            }
            return donus;
        }

        /// <summary>Ekrandaki kesin hesap, taahhüt ve taşınır tabloları ile teklif değerleri (eski TabloDegerleriDoldur).</summary>
        private static IResult Bilgi(HttpContext baglam, IWebHostEnvironment ortam, int yil, string senaryo, string tertip, int? tahmin,
            string muhasebekod, string birimkod)
        {
            IResult ret = Denetle(baglam, ortam, tahmin ?? 0, out Kullanici kullanan);
            if (ret != null) return ret;
            senaryo = Temiz(senaryo);
            tertip = Temiz(tertip);
            if (yil < 2000 || senaryo == "" || tertip == "") return Hata("Tertip, yıl veya aşama kodu boş olamaz!");

            try
            {
                IUZYServis uzy = TNS.UZY.Arac.Tanimla();
                Tertip t = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
                t.TertipAta(tertip);
                if (t.kodlar == null || t.kodlar.Length < 5) return Hata("Tertip hatalı!");

                string kurkod = t.kodlar[0];
                string fonkod = t.kodlar[1];
                string finkod = t.kodlar[2];
                string ekokod = t.kodlar[3];
                string pebkod = t.kodlar[4];
                string ekokod2 = ekokod.Length >= 4 ? ekokod.Substring(0, 4) : ekokod;
                string y(int fark) => (yil + fark).ToString();
                double deger(string uzay, string kosul, string alan = null) =>
                    alan == null ? uzy.UzayDegeriDbl(kullanan, uzay, kosul, false) : uzy.UzayDegeriDbl(kullanan, uzay, kosul, false, alan);

                // Teklif (3 yıl)
                double[] teklif = new double[3];
                for (int i = 0; i < 3; i++)
                    teklif[i] = deger("ABSODENEK", TNS.UZY.Arac.UzayKosulOl(yil.ToString(), senaryo, i.ToString(), kurkod, fonkod, finkod, ekokod, pebkod));

                // Kesin hesap bilgileri (yıl-2, yıl-3; ekonomik kodun ilk iki düzeyi)
                object kesin(int fark)
                {
                    double kbo = deger("ABSODENEK", TNS.UZY.Arac.UzayKosulOl(y(fark), "06.00", "0", kurkod, fonkod, finkod, ekokod2));
                    double odenek = deger("ABSBUTCEGIDER", TNS.UZY.Arac.UzayKosulOl(y(fark), kurkod, fonkod, finkod, ekokod2));
                    double tenkis = deger("ABSHAZGONDERILEN", TNS.UZY.Arac.UzayKosulOl(y(fark), "", "", kurkod, fonkod, finkod, ekokod2, ""));
                    double harcama = deger("ABSHAZMUHASEBAT", TNS.UZY.Arac.UzayKosulOl(y(fark), "12", kurkod, fonkod, finkod, ekokod2));
                    double odenekUstu = odenek > harcama ? odenek - harcama : 0;
                    return new { yil = yil + fark, kbo = Tutar(kbo), gelenOdenek = Tutar(odenek), tenkis = Tutar(tenkis), harcama = Tutar(harcama), odenekUstu = Tutar(odenekUstu) };
                }

                // Kesin hesap detay (yıl-1 altı aylık, yıl-2 ve yıl-3 yıl sonu)
                double[] dKbo = new double[3], dGer = new double[3];
                for (int i = 0; i < 3; i++)
                {
                    int fark = -(i + 1);
                    dKbo[i] = deger("ABSODENEK", TNS.UZY.Arac.UzayKosulOl(y(fark), "06.00", "0", kurkod, fonkod, finkod, ekokod));
                    dGer[i] = deger("ABSHAZMUHASEBAT", TNS.UZY.Arac.UzayKosulOl(y(fark), i == 0 ? "6" : "12", kurkod, fonkod, finkod, ekokod));
                }
                object[] kesinDetay = new object[3];
                for (int i = 0; i < 3; i++)
                {
                    kesinDetay[i] = new
                    {
                        yil = yil - (i + 1),
                        kbo = Tutar(dKbo[i]),
                        gerceklesme = Tutar(dGer[i]),
                        gerceklesmeOran = YuzdeBulStr(dGer[i], dKbo[i]),
                        degisimKbo = i < 2 ? DegisimOranBulStr(dKbo[i], dKbo[i + 1]) : "-",
                        degisimGerceklesme = i < 2 ? DegisimOranBulStr(dGer[i], dGer[i + 1]) : "-"
                    };
                }

                // Taahhüt (harcama süreci) bilgileri (yıl-1 altı aylık, yıl-2 yıl sonu)
                object harcamaSatir(int fark, string donem)
                {
                    string kosul = TNS.UZY.Arac.UzayKosulOl(y(fark), donem, "", "", kurkod, fonkod, finkod, ekokod, "");
                    return new
                    {
                        yil = yil + fark,
                        onay = Tutar(deger("ABSHAZSUREC", kosul)),
                        alim = Tutar(deger("ABSHAZSUREC", kosul, "SUM(ALIM)")),
                        fatura = Tutar(deger("ABSHAZSUREC", kosul, "SUM(FATURA)")),
                        teslim = Tutar(deger("ABSHAZSUREC", kosul, "SUM(TESLIM)")),
                        odeme = Tutar(deger("ABSHAZSUREC", kosul, "SUM(ODEME)"))
                    };
                }

                // Taşınır bilgileri: ekonomik kodla ilişkili taşınır hesabı varsa (yoksa sekme gizli kalır)
                string hesapKod = ParametrikHesapPlanDon(ekokod);
                bool tasinirVar = hesapKod != "";
                object tasinirSatir(int fark)
                {
                    if (!tasinirVar) return new { yil = yil + fark, devir = "-", giris = "-", cikis = "-", kalan = "-" };
                    TNS.TMM.ITMMServis tmm = TNS.TMM.Arac.Tanimla();
                    string muhasebe = Temiz(muhasebekod);
                    string harcamaBirim = kurkod + "." + Temiz(birimkod);
                    decimal[] d = TasinirDepoDurumuTutarlariGetir(tmm, yil + fark, muhasebe, harcamaBirim, hesapKod);
                    double devir = OrtakFonksiyonlar.ConvertToDbl(DepoDurumGetirDevirsiz(tmm, yil + fark - 1, muhasebe, harcamaBirim, hesapKod));
                    // Eski sayfa gibi: tutarlar yuvarlanıp yazılır, kalan yazılmış (yuvarlanmış) değerlerden hesaplanır.
                    double giris = Math.Round((double)d[0], MidpointRounding.AwayFromZero), cikis = Math.Round((double)d[1], MidpointRounding.AwayFromZero);
                    return new { yil = yil + fark, devir = Tutar(devir), giris = Tutar(giris), cikis = Tutar(cikis), kalan = Tutar(Math.Round(devir, MidpointRounding.AwayFromZero) + giris - cikis) };
                }

                return Results.Json(new
                {
                    yil = yil,
                    kesinTertip = "(" + kurkod + "-" + fonkod + "-" + finkod + "-" + ekokod2 + ")",
                    kesinDetayTertip = "(" + kurkod + "-" + fonkod + "-" + finkod + "-" + ekokod + ")",
                    kesin = new[] { kesin(-2), kesin(-3) },
                    kesinDetay = kesinDetay,
                    harcama = new[] { harcamaSatir(-1, "0/6"), harcamaSatir(-2, "0/12") },
                    tasinirVar = tasinirVar,
                    tasinir = new[] { tasinirSatir(-2), tasinirSatir(-3) },
                    teklif = new[] { new { yil = yil, deger = teklif[0] }, new { yil = yil + 1, deger = teklif[1] }, new { yil = yil + 2, deger = teklif[2] } }
                });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Bilgiler okunamadı."));
            }
        }

        /// <summary>Ekrandaki 3 yıllık teklif saklanır (eski Kaydet).</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery antiforgery, TertipDetayIstegi istek)
        {
            if (!await antiforgery.IsRequestValidAsync(baglam)) return Results.BadRequest();
            istek ??= new TertipDetayIstegi();
            IResult ret = Denetle(baglam, ortam, istek.Tahmin, out Kullanici kullanan);
            if (ret != null) return ret;

            string tertip = Temiz(istek.Tertip);
            string senaryo = Temiz(istek.Senaryo);
            double?[] d = istek.Degerler ?? new double?[0];
            string hata = "";
            for (int i = 0; i < 3; i++)
            {
                if (i >= d.Length || !d[i].HasValue)
                {
                    if (hata != "") hata += " ";
                    hata += string.Format("{0}. Yıl boş bırakılamaz", i + 1);
                }
            }
            if (tertip == "" || istek.Yil == 0 || senaryo == "")
            {
                if (hata != "") hata += " ";
                hata += "Tertip, yıl veya aşama kodu boş olamaz!";
            }
            if (hata != "") return Hata(hata);

            try
            {
                TertipOdenek to = new TertipOdenek(3);
                to.tertip = new Tertip(istek.Yil, ENUMTabloKod.TABPEBOdenek);
                to.tertip.TertipAta(tertip);
                for (int i = 0; i < 3; i++) to.odenek[i] = d[i].Value;

                Sonuc sonuc = TNS.BHMv2.Arac.Tanimla().ButceOdenekKaydet(kullanan, senaryo, new[] { to });
                if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc.hataStr, "Bilgiler saklanamadı."));
                return Results.Json(new { basarili = true, bilgi = "Bilgiler başarıyla saklandı." });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Bilgiler saklanamadı."));
            }
        }
    }
}
