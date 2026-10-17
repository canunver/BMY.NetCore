using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
using TNS.BKD;
using TNS.KHM;
using TNS.KYM;

namespace Istemci.KesinHesap
{
    public class FisGerekceKayitIstegi
    {
        public int Yil { get; set; }
        public string Tertip { get; set; }
        public string Gerekce { get; set; }
    }

    public class FisGerekceUretIstegi
    {
        public int Yil { get; set; }
        public string Tertip { get; set; }
    }

    /// <summary>
    /// Gider Cetveli Açıklaması Girişi sayfasının (eski KesinHesap/FisGiderGerekce.aspx) JavaScript'ten çağırdığı uçlar:
    ///   GET  /api/fisgidergerekce/tanim, /tertipler?yil=, /oku?yil=&amp;tertip=, /sablon?yil=&amp;tertip=
    ///   POST /api/fisgidergerekce/kaydet
    ///   GET  /api/fisgidergerekce/rapor?yil=&amp;tablosuz= (KHM\KH-GiderGerekce.dotx şablonundan Aspose.Words ile; Word .docx)
    ///   POST /api/fisgidergerekce/yukle (Verileri Yükle, Excel), /uret (Gerekçeleri üret): yalnızca sistem yöneticisi
    /// İşlemler eskisi gibi TNS.KHM.Arac.Tanimla() üzerinden yapılır; gerekçe şablonu ve ödenek hesabı eski sayfadaki kodun aynısıdır.
    /// </summary>
    public static class FisGiderGerekceUclari
    {
        private const string SayfaAdres = "../KesinHesap/FisGiderGerekce.aspx";
        private const string KayitHatasi = "İşlem sırasında hata oluştu: "; // eski Resources.KesinHesap.FRM018
        private const string KayitBasarili = "Bilgiler başarıyla saklandı."; // eski Resources.KesinHesap.FRM019
        private static readonly CultureInfo tr = new CultureInfo("tr-TR");

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/fisgidergerekce/tanim", Tanim);
            app.MapGet("/api/fisgidergerekce/tertipler", Tertipler);
            app.MapGet("/api/fisgidergerekce/oku", Oku);
            app.MapGet("/api/fisgidergerekce/sablon", Sablon);
            app.MapPost("/api/fisgidergerekce/kaydet", Kaydet);
            app.MapGet("/api/fisgidergerekce/rapor", Rapor);
            app.MapPost("/api/fisgidergerekce/yukle", Yukle).DisableAntiforgery();
            app.MapPost("/api/fisgidergerekce/uret", Uret);
        }

        /// <summary>Kullanıcı sayfayı kullanabilir mi? (Sayfa modeli de bunu kullanır.)</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (GenelGorevUclari.YetkiVar(kullanan)) return true;
            try
            {
                return new MenuOkuyucu(kullanan).SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), SayfaAdres);
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

        private static bool SistemYoneticisi(Kullanici kullanan)
        {
            return kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
        }

        private static IKHMServis Servis()
        {
            return TNS.KHM.Arac.Tanimla();
        }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        private static bool YilGecerli(int yil)
        {
            return yil >= 2007 && yil <= DateTime.Now.Year;
        }

        private static string Tutar(double d)
        {
            return d.ToString("#,##0.00", tr);
        }

        /// <summary>Eski sayfadaki tertip kurma: listede tire yoksa (yalnızca program) diğer kodlar sıfırla tamamlanır.</summary>
        private static Tertip TertipKur(int yil, string ttt)
        {
            Tertip tertip = new Tertip(yil, ENUMTabloKod.TABButce);
            ttt = (ttt ?? "").Trim();
            if (ttt.IndexOf("-") < 0)
                ttt = ttt + "-000000-0-0000";
            tertip.TertipAta(ttt);
            return tertip;
        }

        // ------------------------------------------------------------------ eski hesaplar (aynen)

        /// <summary>Eski OdenekHesapla.</summary>
        private static double OdenekHesapla(TertipBilgi tBilgi)
        {
            double eklenen = tBilgi.tertipBilgi.kiciEk + tBilgi.tertipBilgi.karaEk + tBilgi.tertipBilgi.yedekEk + tBilgi.tertipBilgi.odenekEkle + tBilgi.tertipBilgi.ozelOdenek;
            double dusulen = tBilgi.tertipBilgi.kiciDus + tBilgi.tertipBilgi.karaDus + tBilgi.tertipBilgi.yedekDus + tBilgi.tertipBilgi.yiliciIptal;
            double devir1 = tBilgi.tertipBilgi.akreditifdevir + tBilgi.tertipBilgi.taahhutdevir + tBilgi.tertipBilgi.disprojedevir + tBilgi.tertipBilgi.digerdevir;
            double devir2 = tBilgi.tertipBilgi.bagisdevir + tBilgi.tertipBilgi.ozeldevir;
            return tBilgi.tertipBilgi.kbo + eklenen - dusulen + devir1 + devir2;
        }

        private static TertipBilgi TertipDegerBul(Kullanici kullanan, int yil, Tertip tertip)
        {
            ObjectArray<TertipBilgi> tt = Servis().GiderTertipDegerListele(kullanan, yil, tertip);
            if (tt != null && tt.objeler != null && tt.objeler.Count == 1) return tt.objeler[0];
            return null;
        }

        /// <summary>Eski TertipAciklamaBul: ödenek rakamlarına göre gerekçe metni (Şablon).</summary>
        private static string TertipAciklamaBul(Kullanici kullanan, int yil, string ttt)
        {
            Tertip tertip = TertipKur(yil, ttt);
            TertipBilgi tBilgi = TertipDegerBul(kullanan, yil, tertip);
            if (tBilgi == null) return "";

            double odenek = OdenekHesapla(tBilgi);
            double harcama = tBilgi.tertipBilgi.harcama;
            double iptal = tBilgi.tertipBilgi.yilSonuIptal;
            double harcamaOran = (harcama / odenek) * 100;
            double iptalOran = (iptal / odenek) * 100;
            double eklenen = tBilgi.tertipBilgi.karaEk + tBilgi.tertipBilgi.kiciEk + tBilgi.tertipBilgi.odenekEkle + tBilgi.tertipBilgi.yedekEk;
            double dusulen = tBilgi.tertipBilgi.karaDus + tBilgi.tertipBilgi.kiciDus + tBilgi.tertipBilgi.yedekDus;
            double fark = eklenen - dusulen;

            string sablon;
            if (tBilgi.tertipBilgi.kbo == 0)
                sablon = "Bütçe Kanunu ile başlangıçta ödenek ayrılmamış fakat";
            else
                sablon = "Bütçe Kanunu ile " + Tutar(tBilgi.tertipBilgi.kbo) + " TL ödenek konulmuş,";

            if (odenek != 0)
            {
                if (fark > 0)
                    sablon += " aktarma işlemi ile " + Tutar(fark) + " TL eklenmiş, toplam " + Tutar(odenek) + " TL olan";
                else if (fark < 0)
                    sablon += " aktarma işlemi ile " + Tutar(0 - fark) + " TL düşülmüş, toplam " + Tutar(odenek) + " TL olan";
                if (tBilgi.tertipBilgi.harcama > 0)
                {
                    if (tBilgi.tertipBilgi.harcama > odenek)
                    {
                        sablon += " bu ödeneğe karşın " + Tutar(tBilgi.tertipBilgi.harcama) + " TL harcama yapılmış ve " + Tutar(tBilgi.tertipBilgi.harcama - odenek) + " TL ödenek üstü meydana gelmiştir.";
                    }
                    else if (tBilgi.tertipBilgi.yilSonuIptal == 0)
                    {
                        sablon += " ödeneğin tamamı (%100) harcanmıştır.";
                    }
                    else
                    {
                        sablon += " ödeneğin " + Tutar(tBilgi.tertipBilgi.harcama) + " TL’si harcanmış, kalan";
                        sablon += " " + Tutar(tBilgi.tertipBilgi.yilSonuIptal) + " TL yıl sonunda iptal edilmiştir.";
                        sablon += " Harcama %" + harcamaOran.ToString("0.00", tr);
                        sablon += ", iptal %" + iptalOran.ToString("0.00", tr);
                        sablon += " oranında gerçekleşmiştir.";
                    }
                }
                else
                    sablon += " ödenekten her hangi bir harcama yapılmamış, ödeneğin tamamı (%100) yıl sonunda iptal edilmiştir.";
            }
            else
                sablon += " ödeneğin tamamı (%100) aktarma işlemleri ile düşülmüş ve her hangi bir harcama yapılmamıştır.";

            sablon += Servis().FaaliyetAciklamaBul(kullanan, yil, tertip);
            return sablon;
        }

        // ------------------------------------------------------------------ okuma

        /// <summary>Açılış bilgileri (eski Page_Load): yıl aralığı 2007 - bu yıl, varsayılan geçen yıl; yönetici tuşları.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            int buYil = DateTime.Now.Year;
            return Results.Json(new { yil = buYil - 1, enAz = 2007, enCok = buYil, yonetici = SistemYoneticisi(kullanan) });
        }

        /// <summary>Eski TertipDoldur: yılın gider tertipleri.</summary>
        private static IResult Tertipler(HttpContext baglam, IWebHostEnvironment ortam, int yil)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!YilGecerli(yil)) return Results.Json(new { hata = "Yıl hatalı!" });
            try
            {
                ObjectArray<Tertip> tertipler = Servis().GiderTertipListele(kullanan, yil, new Tertip(yil, ENUMTabloKod.TABButce));
                if (tertipler.sonuc != null && !tertipler.sonuc.islemSonuc)
                    return Results.Json(new { hata = KayitHatasi + HataMetni(tertipler.sonuc.hataStr, "Tertipler okunamadı.") });
                List<string> liste = new List<string>();
                foreach (Tertip t in tertipler.objeler)
                    liste.Add(t.TertipStrOl());
                return Results.Json(new { tertipler = liste });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Tertipler okunamadı." });
            }
        }

        /// <summary>
        /// Eski btnListele_Click: tertibin kayıtlı gerekçesi ve bütçe değerleri (Top. Ödenek, Harcama, İptal Ödenek, Ödenek Üstü,
        /// Eklenen, Düşülen). Gerekçe boşsa eskisi gibi şablon metni gelir (sablon = true; ekranda kırmızı gösterilir).
        /// </summary>
        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int yil, string tertip)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!YilGecerli(yil) || string.IsNullOrWhiteSpace(tertip)) return Results.Json(new { hata = "Yıl ve tertip seçiniz." });
            try
            {
                Tertip t = TertipKur(yil, tertip);
                GiderGerekceVAN gV = Servis().GiderGerekceGetir(kullanan, yil, t);
                if (!gV.sonuc.islemSonuc)
                    return Results.Json(new { hata = KayitHatasi + HataMetni(gV.sonuc.hataStr, "Gerekçe okunamadı.") });

                object degerler = null;
                TertipBilgi tBilgi = TertipDegerBul(kullanan, yil, t);
                if (tBilgi != null)
                {
                    double eklenen = tBilgi.tertipBilgi.kiciEk + tBilgi.tertipBilgi.karaEk + tBilgi.tertipBilgi.yedekEk + tBilgi.tertipBilgi.odenekEkle + tBilgi.tertipBilgi.ozelOdenek;
                    double dusulen = tBilgi.tertipBilgi.kiciDus + tBilgi.tertipBilgi.karaDus + tBilgi.tertipBilgi.yedekDus + tBilgi.tertipBilgi.yiliciIptal;
                    degerler = new
                    {
                        odenek = OdenekHesapla(tBilgi),
                        harcama = tBilgi.tertipBilgi.harcama,
                        iptal = tBilgi.tertipBilgi.yilSonuIptal,
                        odenekUstu = tBilgi.tertipBilgi.odenekUstu,
                        eklenen = eklenen,
                        dusulen = dusulen
                    };
                }

                string gerekce = gV.gerekce == null ? "" : (gV.gerekce.gerekce ?? "");
                bool sablon = false;
                if (gerekce.Trim() == "")
                {
                    gerekce = TertipAciklamaBul(kullanan, yil, tertip);
                    sablon = true;
                }
                return Results.Json(new { gerekce = gerekce, sablon = sablon, degerler = degerler });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Gerekçe okunamadı." });
            }
        }

        /// <summary>Eski btnSablon_Click: ödenek rakamlarına göre gerekçe metni.</summary>
        private static IResult Sablon(HttpContext baglam, IWebHostEnvironment ortam, int yil, string tertip)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!YilGecerli(yil) || string.IsNullOrWhiteSpace(tertip)) return Results.Json(new { hata = "Yıl ve tertip seçiniz." });
            try
            {
                return Results.Json(new { gerekce = TertipAciklamaBul(kullanan, yil, tertip) });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Şablon hazırlanamadı." });
            }
        }

        // ------------------------------------------------------------------ yazma

        /// <summary>Eski btnKaydet_Click.</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, FisGerekceKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || !YilGecerli(istek.Yil) || string.IsNullOrWhiteSpace(istek.Tertip))
                return Results.Json(new { basarili = false, hata = "Yıl ve tertip seçiniz." });

            GiderGerekce gerekce = new GiderGerekce(istek.Yil);
            gerekce.gerekce = (istek.Gerekce ?? "").Trim();
            gerekce.tertip = TertipKur(istek.Yil, istek.Tertip);

            Sonuc sonuc;
            try
            {
                sonuc = Servis().GiderGerekceKaydet(kullanan, istek.Yil, gerekce);
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Kayıt işlemi başarısız." });
            }
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = KayitHatasi + HataMetni(sonuc.hataStr, "Kayıt işlemi başarısız.") });
            return Results.Json(new { basarili = true, bilgi = KayitBasarili });
        }

        /// <summary>
        /// Eski btnUret_Click (yalnızca sistem yöneticisi): seçili tertibin programındaki tertiplerden, seçili tertipten başlayarak
        /// sonrakilerin hepsine şablon gerekçesi üretilip kaydedilir.
        /// </summary>
        private static async Task<IResult> Uret(HttpContext baglam, IWebHostEnvironment ortam, FisGerekceUretIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!SistemYoneticisi(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null || !YilGecerli(istek.Yil) || string.IsNullOrWhiteSpace(istek.Tertip))
                return Results.Json(new { basarili = false, hata = "Yıl ve tertip seçiniz." });

            int yil = istek.Yil;
            string ttt = istek.Tertip.Trim();
            string[] tttler = ttt.Split('-');
            if (tttler.Length < 2) return Results.Json(new { basarili = false, hata = "Tertip hatalı!" });
            try
            {
                Tertip tertip = new Tertip(yil, ENUMTabloKod.TABButce);
                tertip.kodlar[0] = TNS.DEG.Arac.VarsayilanKurumBul();
                tertip.kodlar[1] = tttler[1];

                ObjectArray<Tertip> tertipler = Servis().GiderTertipListele(kullanan, yil, tertip);
                bool basla = false;
                int sayi = 0;
                foreach (Tertip t in tertipler.objeler)
                {
                    string simdTertip = t.TertipStrOl();
                    if (!basla && ttt == simdTertip)
                        basla = true;
                    if (basla)
                    {
                        GiderGerekce gerekce = new GiderGerekce(yil);
                        gerekce.gerekce = TertipAciklamaBul(kullanan, yil, simdTertip);
                        gerekce.tertip.TertipAta(simdTertip);
                        Servis().GiderGerekceKaydet(kullanan, yil, gerekce);
                        sayi++;
                    }
                }
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Gerekçeler üretilemedi." });
            }
            return Results.Json(new { basarili = true, bilgi = "Üretim işlemi tamamlandı, lütfen rapor alıp kontrol ediniz." });
        }

        /// <summary>
        /// Eski fuKesinHesap_Click (Verileri Yükle; yalnızca sistem yöneticisi): Excel'de 14. satırdan başlayarak E sütununda tertip
        /// (Prg-Kur-Fin-Eko), F K.B.Ö, G eklenen, H düşülen, N gönderilen, P harcama, S kurum içi ekleme, T yedek ekleme,
        /// U kurum içi düşme; E boş olan ilk satırda durulur. Tertiplerin verileri mevcut verilerle değiştirilir.
        /// </summary>
        private static async Task<IResult> Yukle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!SistemYoneticisi(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            IFormCollection form = await baglam.Request.ReadFormAsync();
            if (!int.TryParse(form["yil"], out int yil) || !YilGecerli(yil))
                return Results.Json(new { basarili = false, hata = "Yıl hatalı!" });
            IFormFile dosya = form.Files["dosya"];
            if (dosya == null || dosya.Length == 0) return Results.Json(new { basarili = false, hata = "Yüklemek istediğiniz dosyanın içi boş." });
            if (dosya.Length > 20 * 1024 * 1024) return Results.Json(new { basarili = false, hata = "Dosya çok büyük (en çok 20 MB)." });
            string uzanti = Path.GetExtension(dosya.FileName ?? "").ToLowerInvariant();
            if (uzanti != ".xls" && uzanti != ".xlsx") uzanti = ".xlsx";
            string dosyaAdxls = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul() + uzanti;

            Sonuc sonuc;
            try
            {
                using (FileStream hedef = File.Create(dosyaAdxls))
                    await dosya.CopyToAsync(hedef);
                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                XLS.DosyaOkuAc(dosyaAdxls);
                TNSCollection tc = new TNSCollection();
                try
                {
                    for (int satir = 13; ; satir++)
                    {
                        string tertip = XLS.HucreDegerAl(satir, 4);
                        if (string.IsNullOrWhiteSpace(tertip))
                            break;
                        string[] parcalar = tertip.Split('-');
                        // Eski kodda bu satırda satır sayacı artmadığından sonsuz döngüye giriyordu; burada satır atlanır.
                        if (parcalar.Length != 4)
                            continue;
                        TertipBilgi tertipBilgi = new TertipBilgi();
                        tertipBilgi.tertip = new Tertip(yil, ENUMTabloKod.TABButce);
                        tertipBilgi.tertip.kodlar[0] = parcalar[1];
                        tertipBilgi.tertip.kodlar[1] = parcalar[0];
                        tertipBilgi.tertip.kodlar[2] = parcalar[2];
                        tertipBilgi.tertip.kodlar[3] = parcalar[3];

                        tertipBilgi.tertipBilgi.kbo = XLS.HucreDegerAlDbl(satir, 5);
                        tertipBilgi.tertipBilgi.bo = XLS.HucreDegerAlDbl(satir, 5);
                        double eklenen = XLS.HucreDegerAlDbl(satir, 6);
                        double dusulen = XLS.HucreDegerAlDbl(satir, 7);
                        tertipBilgi.tertipBilgi.gonderilenodenek = XLS.HucreDegerAlDbl(satir, 13);
                        tertipBilgi.tertipBilgi.harcama = XLS.HucreDegerAlDbl(satir, 15);

                        tertipBilgi.tertipBilgi.kiciEk = XLS.HucreDegerAlDbl(satir, 18);
                        tertipBilgi.tertipBilgi.kiciDus = XLS.HucreDegerAlDbl(satir, 20);
                        tertipBilgi.tertipBilgi.yedekEk = XLS.HucreDegerAlDbl(satir, 19);
                        if (tertipBilgi.tertipBilgi.yedekEk < 0)
                        {
                            tertipBilgi.tertipBilgi.yedekDus = 0 - tertipBilgi.tertipBilgi.yedekEk;
                            tertipBilgi.tertipBilgi.yedekEk = 0;
                        }
                        tertipBilgi.tertipBilgi.karaEk = eklenen - tertipBilgi.tertipBilgi.kiciEk - tertipBilgi.tertipBilgi.yedekEk;
                        tertipBilgi.tertipBilgi.karaDus = dusulen - tertipBilgi.tertipBilgi.kiciDus - tertipBilgi.tertipBilgi.yedekDus;

                        tertipBilgi.tertipBilgi.devir = 0;
                        tertipBilgi.tertipBilgi.akreditif = 0;
                        tertipBilgi.tertipBilgi.yiliciIptal = 0;

                        tc.Add(tertipBilgi);
                    }
                }
                finally
                {
                    XLS.DosyaKapat();
                }
                sonuc = Servis().KesinHesapGiderBilgileriKaydet(kullanan, yil, tc);
            }
            catch (Exception)
            {
                sonuc = new Sonuc(false, "Dosya okunamadı.", "");
            }
            finally
            {
                try { File.Delete(dosyaAdxls); } catch { }
            }

            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = HataMetni(sonuc.hataStr, "Veriler yüklenemedi.") });
            return Results.Json(new { basarili = true, bilgi = KayitBasarili });
        }

        // ------------------------------------------------------------------ rapor (Aspose.Words, .dotx şablonu)

        private const string SablonAd = "KH-GiderGerekce.dotx";

        /// <summary>
        /// Eski Yazdir: Yazdır (tablolu) ve Yazdır (Tablosuz). Kurumun yıllık bütün gider gerekçeleri KHM\KH-GiderGerekce.dotx
        /// şablonundan Aspose.Words adres-mektup birleştirmesiyle Word (.docx) olarak üretilir.
        /// </summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, int yil, bool? tablosuz)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!YilGecerli(yil)) return Results.Json(new { hata = "Yıl hatalı!" });
            try
            {
                string sablon = SablonBul(ortam.ContentRootPath);
                if (sablon == null) return Results.Json(new { hata = "Rapor şablonu bulunamadı (KHM\\" + SablonAd + ")." });
                string dosya = GiderGerekceRaporu(kullanan, yil, tablosuz == true, sablon, out string hata);
                if (dosya == null) return Results.Json(new { hata = hata });
                return DosyaIslem.DosyaGonder(dosya, "GiderGerekce_" + yil, true, "docx");
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Rapor hazırlanamadı." });
            }
        }

        /// <summary>
        /// Şablon önce kurumun rapor şablonu klasöründe (RaporSablonYol\KHM) aranır, yoksa uygulamayla gelen
        /// SGB.Net\RaporSablon\KHM klasöründen alınır.
        /// </summary>
        private static string SablonBul(string uygulamaKlasoru)
        {
            string kurumYol = Ayar.Oku("RaporSablonYol");
            if (kurumYol != "")
            {
                string s = Path.Combine(kurumYol, "KHM", SablonAd);
                if (File.Exists(s)) return s;
            }
            string u = Path.Combine(uygulamaKlasoru, "RaporSablon", "KHM", SablonAd);
            return File.Exists(u) ? u : null;
        }

        /// <summary>
        /// Eski Yazdir(tabloYok) kodunun hesapları aynen; rapor EkRtf yerine Aspose.Words ile yazılır. Şablonda üst bilgide
        /// «kurum» ve «yil», gövdede Gerekce bölgesi (p, k, f, e, kod, gerekce) ve onun içinde Tablo alt bölgesi (ödenek,
        /// harcama, iptal, ödenek üstü ve oranları) vardır. Tablosuz raporda Tablo alt bölgesine satır verilmez, tablo kalkar.
        /// </summary>
        private static string GiderGerekceRaporu(Kullanici kullanan, int yil, bool tabloYok, string sablon, out string hata)
        {
            hata = null;
            IKHMServis servisKHM = Servis();
            Tertip tertip = new Tertip(yil, ENUMTabloKod.TABButce);
            tertip.TertipAta(TNS.DEG.Arac.VarsayilanKurumBul());

            ObjectArray<GiderGerekce> gg = servisKHM.GiderGerekceListele(kullanan, yil, tertip);
            if (!gg.sonuc.islemSonuc)
            {
                hata = KayitHatasi + HataMetni(gg.sonuc.hataStr, "Gerekçeler okunamadı.");
                return null;
            }

            ObjectArray<TertipBilgi> tt = servisKHM.GiderTertipDegerListele(kullanan, yil, tertip);
            Hashtable tertipler = new Hashtable();
            foreach (TertipBilgi tb in tt.objeler)
                tertipler[tb.tertip.TertipStrOl()] = tb;

            string kurum = TNS.DEG.Arac.VarsayilanKurumBul() ?? "";
            string kurumEk = "";
            if (kurum.Length == 2) kurumEk = "00";
            string kurumYaz = kurum + kurumEk + " " + (TNS.DEG.Arac.DegiskenDegerBul(0, "/KURUMAD") ?? "");

            DataSet ds = new DataSet();
            DataTable dtGerekce = ds.Tables.Add("Gerekce");
            dtGerekce.Columns.Add("no", typeof(int));
            foreach (string sutun in new[] { "yil", "tertip", "kod", "gerekce", "p", "k", "f", "e" })
                dtGerekce.Columns.Add(sutun);
            DataTable dtTablo = ds.Tables.Add("Tablo");
            dtTablo.Columns.Add("no", typeof(int));
            foreach (string sutun in new[] { "odenek", "harcama", "iptal", "odenekUst", "harcamaOran", "iptalOran", "ustOran" })
                dtTablo.Columns.Add(sutun);
            ds.Relations.Add(dtGerekce.Columns["no"], dtTablo.Columns["no"]);

            int no = 0;
            foreach (GiderGerekce g in gg.objeler)
            {
                if (g.tertip.kodlar[0] == null) // Kurumsal kod boş ise dön
                    continue;

                g.tertip.Noktala();
                string kur = g.tertip.kodlar[0];
                string fon = g.tertip.kodlar[1];
                string fin = g.tertip.kodlar[2];
                string eko = g.tertip.kodlar[3];
                string aciklama = GenelIslemler.KodAd(4, yil + "-" + eko, true);

                double odenek = 0;
                double harcama = 0;
                double iptal = 0;
                double odenekUstu = 0;
                double harcamaOran = 0;
                double iptalOran = 0;
                double ustOran = 0;

                TertipBilgi tBilgi = null;
                string t = kur.Replace(".", "") + "-" + fon.Replace(".", "") + "-" + fin.Replace(".", "") + "-" + eko.Replace(".", "");
                if (tertipler.ContainsKey(t))
                    tBilgi = (TertipBilgi)tertipler[t];
                if (tBilgi != null)
                {
                    odenek = OdenekHesapla(tBilgi);
                    harcama = tBilgi.tertipBilgi.harcama;
                    iptal = tBilgi.tertipBilgi.yilSonuIptal;
                    odenekUstu = tBilgi.tertipBilgi.odenekUstu;
                    if (harcama > 0 && odenek > 0)
                        harcamaOran = (harcama / odenek) * 100;
                    if (iptal > 0 && odenek > 0)
                        iptalOran = (iptal / odenek) * 100;
                    if (odenekUstu > 0 && odenek > 0)
                        ustOran = (odenekUstu / odenek) * 100;
                }

                no++;
                dtGerekce.Rows.Add(no, g.yil, g.tertip.TertipStrOl(), aciklama, g.gerekce,
                    g.tertip.kodlar[1], TNS.BKD.Arac.SifirlariAt(g.tertip.kodlar[0]), TNS.BKD.Arac.SifirlariAt(g.tertip.kodlar[2]), g.tertip.kodlar[3]);
                if (!tabloYok) // eski şablondaki \IF(Gerekce.tabloGoster='1')
                    dtTablo.Rows.Add(no, Tutar(odenek), Tutar(harcama), Tutar(iptal), Tutar(odenekUstu),
                        harcamaOran.ToString("##0.00", tr), iptalOran.ToString("##0.00", tr), ustOran.ToString("##0.00", tr));
            }

            AsposeLisans.Yukle();
            Aspose.Words.Document doc = new Aspose.Words.Document(sablon);
            // Tablosuz raporda boş kalan Tablo bölgesi (iç tablo, başlığıyla) kalkar; bölge alanlarının boşalan paragrafları silinir.
            // (Aspose.Words 14.6'da RemoveEmptyTables yok.)
            doc.MailMerge.CleanupOptions = Aspose.Words.Reporting.MailMergeCleanupOptions.RemoveUnusedRegions
                | Aspose.Words.Reporting.MailMergeCleanupOptions.RemoveEmptyParagraphs;
            doc.MailMerge.Execute(new[] { "yil", "kurum" }, new object[] { yil.ToString(), kurumYaz });
            doc.MailMerge.ExecuteWithRegions(ds);
            doc.UpdateFields(); // alt bilgideki tarih / saat

            string sonucDosyaad = DosyaIslem.DosyaAdUret("", ".docx");
            doc.Save(sonucDosyaad, Aspose.Words.SaveFormat.Docx);
            return sonucDosyaad;
        }
    }
}
