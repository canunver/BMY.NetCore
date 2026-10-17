using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Istemci.Oturum;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TNS.KYM;

namespace Istemci.OrtakSayfa
{
    /// <summary>Sayfa menüsünde bir satır: bölüm başlığı ("b") ya da sayfa bağlantısı ("a").</summary>
    public class SayfaMenuSatir
    {
        public string t { get; set; }
        public string ad { get; set; }
        /// <summary>Bölüm başlığının derinliği (1: kolonun ilk düzeyi).</summary>
        public int duzey { get; set; }
        public string id { get; set; }
        public string url { get; set; }
        /// <summary>Bağlantı eskiden alt pencerede açılıyordu (menu.xml: AltPencere).</summary>
        public bool yeni { get; set; }
        /// <summary>Adresin karşılığı olan sayfa yeni sistemde var mı (Pages altında .cshtml).</summary>
        public bool tasindi { get; set; }
        /// <summary>Yalnızca "tümü" isteğinde anlamlı: kullanıcı bu seçeneği menüde görmüyor (Gizli / Modul / Yetki / Kullanici).</summary>
        public bool gizli { get; set; }
    }

    /// <summary>
    /// Bir menü başlığının bütün seçeneklerini ayrı sayfada gösteren OrtakSayfa/SayfaMenu?menuID=... ekranının uçları
    /// (eski OrtakSayfa/SayfaMenu.aspx). menu.xml'de SayfadaGoster niteliği olan başlıklar sol menüde panel açmaz, bu sayfaya gider.
    /// Her bağlantı için sayfanın yeni sisteme taşınıp taşınmadığı da döner (adresin karşılığı derlenmiş Razor sayfaları arasında aranır).
    /// </summary>
    public static class SayfaMenuUclari
    {
        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/ortaksayfa/sayfa-menu", SayfaMenu);
        }

        private static IResult SayfaMenu(HttpContext baglam, IWebHostEnvironment ortam, IActionDescriptorCollectionProvider eylemler, string menuID, string tumu)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (string.IsNullOrWhiteSpace(menuID)) return Results.Json(new { hata = "Menü belirtilmedi." });

            bool yonetici = kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
            // Gizlenen seçenekleri de göstermek (taşıma durumunu bütün menü için görmek) yalnızca sistem yöneticisine açıktır.
            bool hepsi = yonetici && tumu == "1";

            XmlDocument belge = new XmlDocument();
            try
            {
                belge.Load(MenuOkuyucu.DosyaYolu(ortam.ContentRootPath));
            }
            catch (Exception e)
            {
                return Results.Json(new { hata = "Menü okunamadı: " + e.Message });
            }

            XmlNode ana = belge.SelectSingleNode("//*[@ID=" + XPathMetin(menuID.Trim()) + "]");
            if (ana == null) return Results.Json(new { hata = "Menüde " + menuID + " bulunamadı." });

            MenuOkuyucu okuyucu = new MenuOkuyucu(kullanan);
            // Başlığın kendisi ve üstündekiler kullanıcıya görünür olmalıdır (yönetici her menüyü açar).
            if (!yonetici)
                for (XmlNode d = ana; d != null && d.NodeType == XmlNodeType.Element && d != belge.DocumentElement; d = d.ParentNode)
                    if (!okuyucu.DugumGorunur(d)) return Results.StatusCode(StatusCodes.Status403Forbidden);

            HashSet<string> sayfalar = new HashSet<string>(
                eylemler.ActionDescriptors.Items.OfType<PageActionDescriptor>().Select(p => p.ViewEnginePath),
                StringComparer.OrdinalIgnoreCase);

            Okuma okuma = new Okuma { okuyucu = okuyucu, hepsi = hepsi, sayfalar = sayfalar };

            // Tip="Kolon" düğümleri yan yana sütunlardır; kolon dışında kalan seçenekler ayrı bir sütunda toplanır.
            List<List<SayfaMenuSatir>> kolonlar = new List<List<SayfaMenuSatir>>();
            List<SayfaMenuSatir> kolonsuz = new List<SayfaMenuSatir>();
            foreach (XmlNode alt in ana.ChildNodes)
            {
                if (alt.NodeType != XmlNodeType.Element) continue;
                if (Ozellik(alt, "Tip") == "Kolon")
                {
                    bool gizli = !okuyucu.DugumGorunur(alt);
                    if (gizli && !hepsi) continue;
                    List<SayfaMenuSatir> kolon = new List<SayfaMenuSatir>();
                    string kolonAd = Ad(alt);
                    if (kolonAd != "") kolon.Add(new SayfaMenuSatir { t = "b", ad = kolonAd, duzey = 1, id = Ozellik(alt, "ID"), gizli = gizli });
                    okuma.AltlariOku(alt, kolon, kolonAd != "" ? 2 : 1, gizli);
                    Toparla(kolon);
                    if (kolon.Count > 0) kolonlar.Add(kolon);
                }
                else
                    okuma.SatirOku(alt, kolonsuz, 1, false);
            }
            Toparla(kolonsuz);
            if (kolonsuz.Count > 0) kolonlar.Add(kolonsuz);

            return Results.Json(new
            {
                baslik = Ad(ana) != "" ? Ad(ana) : menuID,
                yonetici = yonetici,
                tumu = hepsi,
                kolonlar = kolonlar
            });
        }

        private class Okuma
        {
            public MenuOkuyucu okuyucu;
            public bool hepsi;
            public HashSet<string> sayfalar;

            public void AltlariOku(XmlNode ust, List<SayfaMenuSatir> satirlar, int duzey, bool ustGizli)
            {
                foreach (XmlNode alt in ust.ChildNodes)
                    if (alt.NodeType == XmlNodeType.Element) SatirOku(alt, satirlar, duzey, ustGizli);
            }

            public void SatirOku(XmlNode dugum, List<SayfaMenuSatir> satirlar, int duzey, bool ustGizli)
            {
                bool gizli = ustGizli || !okuyucu.DugumGorunur(dugum);
                if (gizli && !hepsi) return;

                string id = Ozellik(dugum, "ID");
                string ad = Ad(dugum);
                string url = Ozellik(dugum, "URL");

                if (AltElemanVar(dugum) || Ozellik(dugum, "Tip") == "AltBolum" || Ozellik(dugum, "Tip") == "Kolon")
                {
                    if (ad != "") satirlar.Add(new SayfaMenuSatir { t = "b", ad = ad, duzey = duzey, id = id, gizli = gizli });
                    AltlariOku(dugum, satirlar, ad != "" ? duzey + 1 : duzey, gizli);
                    return;
                }

                if (url == "") return;
                satirlar.Add(new SayfaMenuSatir
                {
                    t = "a",
                    ad = ad != "" ? ad : id,
                    id = id,
                    url = url,
                    yeni = Ozellik(dugum, "AltPencere") != "",
                    tasindi = sayfalar.Contains(SayfaYolu(url)),
                    gizli = gizli
                });
            }
        }

        /// <summary>Menü adresini Razor sayfa yoluna çevirir: "../Klasor/Sayfa.aspx?x=1" -> "/Klasor/Sayfa".</summary>
        public static string SayfaYolu(string url)
        {
            string yol = url.Trim();
            int soru = yol.IndexOfAny(new[] { '?', '#' });
            if (soru >= 0) yol = yol.Substring(0, soru);
            while (yol.StartsWith("../", StringComparison.Ordinal)) yol = yol.Substring(3);
            yol = yol.TrimStart('~', '/');
            if (yol.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase)) yol = yol.Substring(0, yol.Length - 5);
            return "/" + yol;
        }

        /// <summary>Ardından bağlantı gelmeyen bölüm başlıklarını atar (bağlantısı kalmayan bölüm gösterilmez).</summary>
        private static void Toparla(List<SayfaMenuSatir> satirlar)
        {
            for (int i = satirlar.Count - 1; i >= 0; i--)
            {
                if (satirlar[i].t != "b") continue;
                // Başlığın altında (daha derin düzeyde) en az bir bağlantı olmalı.
                bool dolu = false;
                for (int j = i + 1; j < satirlar.Count; j++)
                {
                    if (satirlar[j].t == "b" && satirlar[j].duzey <= satirlar[i].duzey) break;
                    if (satirlar[j].t == "a") { dolu = true; break; }
                }
                if (!dolu) satirlar.RemoveAt(i);
            }
        }

        private static bool AltElemanVar(XmlNode dugum)
        {
            foreach (XmlNode alt in dugum.ChildNodes)
                if (alt.NodeType == XmlNodeType.Element) return true;
            return false;
        }

        private static string Ad(XmlNode dugum)
        {
            return Metin.Ortak(Ozellik(dugum, "ID"), Ozellik(dugum, "Text"));
        }

        private static string Ozellik(XmlNode dugum, string ad)
        {
            XmlAttribute o = dugum.Attributes != null ? dugum.Attributes[ad] : null;
            return o != null && o.Value != null ? o.Value.Trim() : "";
        }

        /// <summary>XPath sorgusuna metin değişmezi olarak konacak değer (tırnak içerebilir).</summary>
        private static string XPathMetin(string deger)
        {
            if (deger.IndexOf('\'') < 0) return "'" + deger + "'";
            if (deger.IndexOf('"') < 0) return "\"" + deger + "\"";
            return "concat('" + deger.Replace("'", "',\"'\",'") + "')";
        }
    }
}
