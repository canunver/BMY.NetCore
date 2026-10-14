using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS.KYM;

namespace Istemci.Guncelleme
{
    public class KomutIstegi
    {
        public string Sql { get; set; }
        public string Baglanti { get; set; }
    }

    public class KomutNesneIstegi
    {
        public string Baglanti { get; set; }
        public string Sema { get; set; }
        public string Ad { get; set; }
    }

    /// <summary>Saklanan sorgu (App_Data/KomutCalistir/sorgular.json).</summary>
    public class SakliSorgu
    {
        public string Ad { get; set; }
        public string Sql { get; set; }
        public string Baglanti { get; set; }
        public DateTime Tarih { get; set; }
    }

    /// <summary>
    /// Komut Çalıştır sayfasının (eski YazilimGuncelle/KomutCalistir.aspx) uçları:
    ///   POST /api/guncelleme/komut       : SQL'i seçilen bağlantıda çalıştırır, sonucu tablo (HTML) olarak döndürür (iş kuralı TMM SQLCalistir)
    ///   POST /api/guncelleme/komut-excel : sorgu sonucunu Excel dosyası olarak döndürür (eski ExcelUret)
    ///   POST /api/guncelleme/komut-nesneler : bağlantıdaki tablolar ve görünümler (INFORMATION_SCHEMA.TABLES)
    ///   POST /api/guncelleme/komut-alanlar  : seçilen tablonun / görünümün alanları (INFORMATION_SCHEMA.COLUMNS)
    ///   GET  /api/guncelleme/komut-sorgular, POST /komut-sorgu-sakla, /komut-sorgu-sil : saklanan sorgular (eski sayfada yoktu)
    /// Eski sayfa SQL'i sayfa doğrulamasına takılmasın diye Base64 ile gönderiyordu; JSON'da gerekmez.
    /// </summary>
    public static class KomutCalistirUclari
    {
        public static void Ekle(WebApplication app)
        {
            app.MapPost("/api/guncelleme/komut", Calistir);
            app.MapPost("/api/guncelleme/komut-excel", Excel);
            app.MapPost("/api/guncelleme/komut-nesneler", Nesneler);
            app.MapPost("/api/guncelleme/komut-alanlar", Alanlar);
            app.MapGet("/api/guncelleme/komut-sorgular", Sorgular);
            app.MapPost("/api/guncelleme/komut-sorgu-sakla", SorguSakla);
            app.MapPost("/api/guncelleme/komut-sorgu-sil", SorguSil);
        }

        // ---------------------------------------------------------------- tablolar, görünümler ve alanlar

        /// <summary>SQL metin sabiti içine konacak değer (tek tırnaklar çiftlenir).</summary>
        private static string SqlMetin(string deger)
        {
            return "N'" + (deger ?? "").Replace("'", "''") + "'";
        }

        /// <summary>Sorgunun bütün satırları; her satır sütun değerlerinin metni. Hata olursa hata dolar.</summary>
        private static List<string[]> Satirlar(string sql, string baglanti, out string hata)
        {
            List<string[]> liste = new List<string[]>();
            TNS.UZY.VTSorgula vts = new TNS.UZY.VTSorgula(sql, baglanti, 0);
            while (true)
            {
                DataRow row = vts.satirOku();
                if (row == null) break;
                string[] degerler = new string[row.ItemArray.Length];
                for (int i = 0; i < degerler.Length; i++) degerler[i] = vts.StringAl(i);
                liste.Add(degerler);
            }
            hata = vts.hata;
            return liste;
        }

        private static async Task<IResult> Nesneler(HttpContext baglam, KomutNesneIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = GuncellemeOrtak.Denetle(baglam, GuncellemeOrtak.KomutYetkili, out _);
            if (ret != null) return ret;
            string baglanti = (istek?.Baglanti ?? "").Trim();
            if (baglanti == "") return GuncellemeOrtak.Hata("Bağlantıyı seçiniz.");
            try
            {
                List<string[]> satirlar = Satirlar("SELECT TABLE_SCHEMA, TABLE_NAME, TABLE_TYPE FROM INFORMATION_SCHEMA.TABLES ORDER BY TABLE_TYPE, TABLE_SCHEMA, TABLE_NAME", baglanti, out string hata);
                if (satirlar.Count == 0 && !string.IsNullOrEmpty(hata)) return GuncellemeOrtak.Hata(hata);
                return Results.Json(new
                {
                    basarili = true,
                    nesneler = satirlar.Select(x => new { sema = x[0], ad = x[1], tur = x[2].IndexOf("VIEW", StringComparison.OrdinalIgnoreCase) >= 0 ? "V" : "T" })
                });
            }
            catch (Exception ex)
            {
                return GuncellemeOrtak.Hata(ex.Message);
            }
        }

        private static async Task<IResult> Alanlar(HttpContext baglam, KomutNesneIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = GuncellemeOrtak.Denetle(baglam, GuncellemeOrtak.KomutYetkili, out _);
            if (ret != null) return ret;
            string baglanti = (istek?.Baglanti ?? "").Trim();
            if (baglanti == "" || string.IsNullOrWhiteSpace(istek.Ad)) return GuncellemeOrtak.Hata("Tabloyu seçiniz.");
            try
            {
                string sql = "SELECT COLUMN_NAME, DATA_TYPE, ISNULL(CONVERT(varchar(20), CHARACTER_MAXIMUM_LENGTH), ''), IS_NULLABLE" +
                             " FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = " + SqlMetin(istek.Sema) + " AND TABLE_NAME = " + SqlMetin(istek.Ad) +
                             " ORDER BY ORDINAL_POSITION";
                List<string[]> satirlar = Satirlar(sql, baglanti, out string hata);
                if (satirlar.Count == 0 && !string.IsNullOrEmpty(hata)) return GuncellemeOrtak.Hata(hata);
                return Results.Json(new
                {
                    basarili = true,
                    alanlar = satirlar.Select(x => new
                    {
                        ad = x[0],
                        tip = x[1] + (x[2] == "" ? "" : "(" + (x[2] == "-1" ? "max" : x[2]) + ")"),
                        bos = x[3] == "YES"
                    })
                });
            }
            catch (Exception ex)
            {
                return GuncellemeOrtak.Hata(ex.Message);
            }
        }

        // ---------------------------------------------------------------- saklanan sorgular

        private static readonly object sorguKilit = new object();

        private static string SorguDosyasi(IWebHostEnvironment ortam)
        {
            return Path.Combine(ortam.ContentRootPath, "App_Data", "KomutCalistir", "sorgular.json");
        }

        private static List<SakliSorgu> SorgulariOku(IWebHostEnvironment ortam)
        {
            string dosya = SorguDosyasi(ortam);
            if (!File.Exists(dosya)) return new List<SakliSorgu>();
            return JsonSerializer.Deserialize<List<SakliSorgu>>(File.ReadAllText(dosya)) ?? new List<SakliSorgu>();
        }

        private static void SorgulariYaz(IWebHostEnvironment ortam, List<SakliSorgu> liste)
        {
            string dosya = SorguDosyasi(ortam);
            Directory.CreateDirectory(Path.GetDirectoryName(dosya));
            File.WriteAllText(dosya, JsonSerializer.Serialize(liste, new JsonSerializerOptions { WriteIndented = true }));
        }

        private static object SorguListesi(List<SakliSorgu> liste)
        {
            return liste.OrderBy(x => x.Ad, StringComparer.CurrentCultureIgnoreCase)
                        .Select(x => new { ad = x.Ad, sql = x.Sql, baglanti = x.Baglanti, tarih = x.Tarih.ToString("dd.MM.yyyy HH:mm") });
        }

        private static IResult Sorgular(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = GuncellemeOrtak.Denetle(baglam, GuncellemeOrtak.KomutYetkili, out _);
            if (ret != null) return ret;
            try
            {
                lock (sorguKilit)
                    return Results.Json(new { basarili = true, sorgular = SorguListesi(SorgulariOku(ortam)) });
            }
            catch (Exception ex)
            {
                return GuncellemeOrtak.Hata(ex.Message);
            }
        }

        /// <summary>Aynı adla saklanmış sorgu varsa üzerine yazılır.</summary>
        private static async Task<IResult> SorguSakla(HttpContext baglam, IWebHostEnvironment ortam, SakliSorgu istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = GuncellemeOrtak.Denetle(baglam, GuncellemeOrtak.KomutYetkili, out _);
            if (ret != null) return ret;
            string ad = (istek?.Ad ?? "").Trim();
            if (ad == "") return GuncellemeOrtak.Hata("Sorgu adını yazınız.");
            if (string.IsNullOrWhiteSpace(istek.Sql)) return GuncellemeOrtak.Hata("SQL metnini yazınız.");
            try
            {
                lock (sorguKilit)
                {
                    List<SakliSorgu> liste = SorgulariOku(ortam);
                    liste.RemoveAll(x => string.Equals(x.Ad, ad, StringComparison.CurrentCultureIgnoreCase));
                    liste.Add(new SakliSorgu { Ad = ad, Sql = istek.Sql, Baglanti = (istek.Baglanti ?? "").Trim(), Tarih = DateTime.Now });
                    SorgulariYaz(ortam, liste);
                    return Results.Json(new { basarili = true, bilgi = "Sorgu saklandı.", sorgular = SorguListesi(liste) });
                }
            }
            catch (Exception ex)
            {
                return GuncellemeOrtak.Hata(ex.Message);
            }
        }

        private static async Task<IResult> SorguSil(HttpContext baglam, IWebHostEnvironment ortam, SakliSorgu istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = GuncellemeOrtak.Denetle(baglam, GuncellemeOrtak.KomutYetkili, out _);
            if (ret != null) return ret;
            string ad = (istek?.Ad ?? "").Trim();
            try
            {
                lock (sorguKilit)
                {
                    List<SakliSorgu> liste = SorgulariOku(ortam);
                    if (liste.RemoveAll(x => string.Equals(x.Ad, ad, StringComparison.CurrentCultureIgnoreCase)) == 0)
                        return GuncellemeOrtak.Hata("Sorgu bulunamadı.");
                    SorgulariYaz(ortam, liste);
                    return Results.Json(new { basarili = true, bilgi = "Sorgu silindi.", sorgular = SorguListesi(liste) });
                }
            }
            catch (Exception ex)
            {
                return GuncellemeOrtak.Hata(ex.Message);
            }
        }

        /// <summary>/* ... */ açıklamaları atılır (eski SqlCalistir). Kapanmayan açıklama varsa null.</summary>
        private static string AciklamaAt(string sql)
        {
            int yer1 = sql.IndexOf("/*");
            while (yer1 > -1)
            {
                int yer2 = sql.IndexOf("*/", yer1 + 2);
                if (yer2 < 0) return null;
                sql = sql.Substring(0, yer1) + sql.Substring(yer2 + 2);
                yer1 = sql.IndexOf("/*");
            }
            return sql;
        }

        private static async Task<IResult> Calistir(HttpContext baglam, KomutIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = GuncellemeOrtak.Denetle(baglam, GuncellemeOrtak.KomutYetkili, out Kullanici kullanan);
            if (ret != null) return ret;
            string sql = AciklamaAt(istek?.Sql ?? "");
            if (sql == null) return GuncellemeOrtak.Hata("Hatalı sql metni");
            if (sql.Trim() == "") return GuncellemeOrtak.Hata("SQL metnini yazınız.");
            try
            {
                string html = TNS.TMM.Arac.Tanimla().SQLCalistir(kullanan, (istek.Baglanti ?? "").Trim(), sql);
                return Results.Json(new { basarili = true, html = html });
            }
            catch (Exception ex)
            {
                return GuncellemeOrtak.Hata(ex.Message);
            }
        }

        private static async Task<IResult> Excel(HttpContext baglam, KomutIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = GuncellemeOrtak.Denetle(baglam, GuncellemeOrtak.KomutYetkili, out Kullanici kullanan);
            if (ret != null) return ret;
            string sql = AciklamaAt(istek?.Sql ?? "");
            if (sql == null) return GuncellemeOrtak.Hata("Hatalı sql metni");
            if (sql.Trim() == "") return GuncellemeOrtak.Hata("SQL metnini yazınız.");
            try
            {
                AsposeLisans.Yukle();
                string dosya = ExcelUret(sql, (istek.Baglanti ?? "").Trim());
                return DosyaIslem.DosyaGonder(dosya, "Sonuc_" + DateTime.Now.Ticks.ToString(), true, "xlsx");
            }
            catch (Exception ex)
            {
                return GuncellemeOrtak.Hata(ex.Message);
            }
        }

        /// <summary>Eski ExcelUret: ilk satırda bağlantı ve SQL, sonra sütun adları ve en çok 20000'erli sayfalarla okunan satırlar.</summary>
        private static string ExcelUret(string sql, string baglanti)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            string dosyaAd = DosyaIslem.DosyaAdUret() + ".xlsx";
            XLS.BosDosyaAc(dosyaAd);

            TNS.UZY.VTSorgula vts = new TNS.UZY.VTSorgula(sql, baglanti, 20000);

            int sat = 0;
            XLS.HucreDegerYaz(sat, 0, baglanti);
            XLS.HucreDegerYaz(sat, 1, sql);
            XLS.HucreBirlestir(sat, 1, sat, 5);
            sat++;
            Type[] tipler = null;
            int kolonSay = 0;
            while (true)
            {
                DataRow row = vts.satirOku();
                if (row == null) break;
                if (sat == 1)
                {
                    kolonSay = row.ItemArray.Length;
                    tipler = new Type[kolonSay];
                    for (int i = 0; i < kolonSay; i++)
                    {
                        tipler[i] = typeof(DBNull);
                        XLS.HucreDegerYaz(sat, i, row.Table.Columns[i].ColumnName);
                    }
                    sat++;
                }
                for (int i = 0; i < kolonSay; i++)
                {
                    Type t = row.ItemArray[i].GetType();
                    if (tipler[i] == typeof(DBNull))
                        tipler[i] = t;
                    if (t == typeof(int))
                        XLS.HucreDegerYaz(sat, i, vts.IntAl(i));
                    else if (t == typeof(double) || t == typeof(decimal))
                        XLS.HucreDegerYaz(sat, i, vts.DoubleAl(i));
                    else if (t == typeof(DateTime))
                        XLS.HucreDegerYaz(sat, i, vts.DTAl(i));
                    else
                        XLS.HucreDegerYaz(sat, i, vts.StringAl(i));
                }
                sat++;
            }
            if (sat == 1)
            {
                XLS.HucreDegerYaz(sat, 0, vts.hata ?? "");
            }
            else
            {
                for (int i = 0; i < kolonSay; i++)
                    if (tipler[i] == typeof(DateTime))
                        XLS.HucreFormatla(2, i, sat, i, "dd.mm.yyyy hh:mm");
            }
            XLS.DosyaSaklaTamYol();
            return dosyaAd;
        }
    }
}
