using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.KYM;

namespace Istemci.OrtakSayfa
{
    public class SikKullanilanIstegi
    {
        /// <summary>Menü seçeneğinin menu.xml'deki ID'si.</summary>
        public string Id { get; set; }
        /// <summary>true: ekle, false: çıkar.</summary>
        public bool Ekle { get; set; }
    }

    /// <summary>
    /// Kullanıcının sık kullanılan ekranları. Eski KYM/MenuSikKullanilan.aspx'in yerine: sayfa başlığındaki +/− düğmesi
    /// açık sayfayı ekler ya da çıkarır; liste sol menünün başında ve açılış sayfasındaki bilgi ekranında (BE003) gösterilir.
    /// Kayıt eskisiyle aynıdır: kullanıcı değişkeni KULLANICIMENUSIKKULANILAN, virgülle ayrılmış menü ID'leri
    /// (GenelIslemler.KullaniciDegiskenGetir / KullaniciDegiskenSakla).
    /// </summary>
    public static class SikKullanilanUclari
    {
        /// <summary>Kullanıcı değişkeninin adı (eskisindeki yazımıyla).</summary>
        public const string DEGISKEN = "KULLANICIMENUSIKKULANILAN";

        private static readonly Regex idDeseni = new Regex(@"^[A-Za-z0-9_]{1,60}$", RegexOptions.Compiled);

        public static void Ekle(WebApplication app)
        {
            app.MapPost("/api/ortaksayfa/sik-kullanilan", Degistir);
            app.MapGet("/api/ortaksayfa/sik-kullanilan-liste", Liste);
        }

        /// <summary>Kullanıcının sık kullanılan menü ID'leri, kayıttaki sırasıyla (boşlar ve tekrarlar atılır).</summary>
        public static List<string> Oku(Kullanici kullanan)
        {
            List<string> liste = new List<string>();
            string deger = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, DEGISKEN);
            foreach (string parca in (deger ?? "").Split(','))
            {
                string id = parca.Trim();
                if (id != "" && !liste.Contains(id)) liste.Add(id);
            }
            return liste;
        }

        /// <summary>Açık sayfayı sık kullanılanlara ekler ya da çıkarır; güncel listeyi döndürür.</summary>
        private static async Task<IResult> Degistir(HttpContext baglam, SikKullanilanIstegi istek, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();

            string id = istek != null && istek.Id != null ? istek.Id.Trim() : "";
            if (!idDeseni.IsMatch(id)) return Results.BadRequest();

            try
            {
                List<string> liste = Oku(kullanan);
                if (istek.Ekle)
                {
                    // Eskisinde de yalnızca kullanıcının menüde görebildiği seçenekler işaretlenebiliyordu.
                    List<MenuGrup> gruplar = new MenuOkuyucu(kullanan).Oku(MenuOkuyucu.DosyaYolu(ortam.ContentRootPath));
                    if (Bul(gruplar, id) == null) return Results.Json(new { basarili = false, hata = "Bu ekran menünüzde bulunamadı." });
                    if (!liste.Contains(id)) liste.Add(id);
                }
                else
                    liste.Remove(id);

                Sonuc sonuc = GenelIslemler.KullaniciDegiskenSakla(kullanan, DEGISKEN, string.Join(",", liste));
                if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = sonuc.hataStr });
                return Results.Json(new { basarili = true, sikKullanilanlar = liste });
            }
            catch (Exception e)
            {
                return Results.Json(new { basarili = false, hata = e.Message });
            }
        }

        /// <summary>
        /// Açılış sayfasındaki "Sık Kullanılanlar" bilgi ekranı için liste: menü sırasıyla, modüle göre gruplu
        /// (eski Dashboard/Genel/SikKullanilan.ascx). Kullanıcının artık göremediği seçenekler listelenmez.
        /// </summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();

            try
            {
                HashSet<string> secili = new HashSet<string>(Oku(kullanan));
                List<object> moduller = new List<object>();
                if (secili.Count > 0)
                {
                    HashSet<string> eklenen = new HashSet<string>();
                    foreach (MenuGrup grup in new MenuOkuyucu(kullanan).Oku(MenuOkuyucu.DosyaYolu(ortam.ContentRootPath)))
                    {
                        List<object> ekranlar = new List<object>();
                        foreach (MenuBaslik menu in grup.menuler)
                            foreach (MenuSatir satir in Baglantilar(menu))
                                if (satir.id != null && secili.Contains(satir.id) && eklenen.Add(satir.id))
                                    ekranlar.Add(new { satir.id, satir.ad, satir.url, satir.yeni, yer = menu.ad });
                        if (ekranlar.Count > 0) moduller.Add(new { ad = grup.ad, ekranlar = ekranlar });
                    }
                }
                return Results.Json(new { basarili = true, moduller = moduller });
            }
            catch (Exception e)
            {
                return Results.Json(new { basarili = false, hata = e.Message });
            }
        }

        /// <summary>Menünün bütün bağlantıları: kolonlardakiler, ayrı sayfada gösterilenler ve doğrudan bağlantı olan başlık.</summary>
        private static IEnumerable<MenuSatir> Baglantilar(MenuBaslik menu)
        {
            if (menu.url != null && menu.ekSatirlar == null)
                yield return new MenuSatir { t = "a", id = menu.id, ad = menu.ad, url = menu.url };
            foreach (List<MenuSatir> kolon in menu.kolonlar)
                foreach (MenuSatir satir in kolon)
                    if (satir.t == "a") yield return satir;
            if (menu.ekSatirlar != null)
                foreach (MenuSatir satir in menu.ekSatirlar) yield return satir;
        }

        private static MenuSatir Bul(List<MenuGrup> gruplar, string id)
        {
            foreach (MenuGrup grup in gruplar)
                foreach (MenuBaslik menu in grup.menuler)
                    foreach (MenuSatir satir in Baglantilar(menu))
                        if (satir.id == id) return satir;
            return null;
        }
    }
}
