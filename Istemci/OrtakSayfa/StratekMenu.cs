using System;
using System.Xml;
using TNS.KYM;

namespace Istemci.OrtakSayfa
{
    /// <summary>
    /// Yalnızca "stratek" kullanıcısına açık teknik sayfalar (Dosyalar, Komut Çalıştır, Şifre Üret). Kullanıcı isteğiyle
    /// (2026-10-20) bu seçenekler menu.xml'de durmaz: menü dosyası okunduktan sonra, kullanıcı sistem yöneticisi olan "stratek"
    /// ise Güncelleme menüsüne (Menu6_2) koddan eklenir. Sol menü (MenuOkuyucu.Oku), menü yetkisi (MenuOkuyucu.SayfaYetkili),
    /// sayfa menüsü (SayfaMenuUclari) ve sayfa yetkisi (SayfaYetki) aynı listeyi kullanır.
    /// Seçenek adları menüdeki gibi ID ile kaynak dosyalarından çevrilir (Metin.Ortak).
    /// </summary>
    public static class StratekMenu
    {
        /// <summary>Bu sayfaları kullanabilen tek kullanıcı.</summary>
        public const string KullaniciKodu = "stratek";

        /// <summary>Seçeneklerin eklendiği menü başlığı (Güncelleme).</summary>
        public const string UstMenuID = "Menu6_2";

        /// <summary>Seçeneklerin arkasına eklendiği seçenek (Yazılım Güncelle); menüde yoksa başlığın sonuna eklenir.</summary>
        private const string OncekiSayfa = "../YazilimGuncelle/YazilimGuncelle.aspx";

        private static readonly string[][] Ogeler =
        {
            // ID, Text, URL
            new[] { "Menu6_2_4", "Dosyalar", "../YazilimGuncelle/Dosyalar.aspx" },
            new[] { "Menu6_2_5", "Komut Çalıştır", "../YazilimGuncelle/KomutCalistir.aspx" },
            new[] { "Menu6_2_6", "Şifre Üret", "../YazilimGuncelle/SifreUret.aspx" },
        };

        /// <summary>Sistem yöneticisi olan "stratek" kullanıcısı mı?</summary>
        public static bool Stratek(Kullanici kullanan)
        {
            return kullanan != null && kullanan.kullaniciKodu == KullaniciKodu
                   && kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
        }

        /// <summary>Adres (menu.xml biçiminde, ör. "../YazilimGuncelle/Dosyalar.aspx") stratek sayfalarından biri mi?</summary>
        public static bool SayfaVar(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            foreach (string[] o in Ogeler)
                if (string.Equals(o[2], url.Trim(), StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        /// <summary>
        /// Okunan menü belgesine, kullanıcı stratek ise seçenekleri ekler. Güncelleme başlığının ilk kolonuna (kolon yoksa başlığın
        /// kendisine) Yazılım Güncelle'nin arkasına konur. Aynı adres belgede zaten varsa tekrar eklenmez.
        /// </summary>
        public static void Ekle(XmlDocument belge, Kullanici kullanan)
        {
            if (belge == null || belge.DocumentElement == null || !Stratek(kullanan)) return;

            XmlElement baslik = belge.SelectSingleNode("//*[@ID='" + UstMenuID + "']") as XmlElement;
            if (baslik == null) return;

            XmlNode hedef = baslik;
            foreach (XmlNode alt in baslik.ChildNodes)
                if (alt is XmlElement e && e.GetAttribute("Tip") == "Kolon") { hedef = alt; break; }

            XmlNode onceki = null;
            foreach (XmlNode alt in hedef.ChildNodes)
                if (alt is XmlElement e && string.Equals(e.GetAttribute("URL").Trim(), OncekiSayfa, StringComparison.OrdinalIgnoreCase)) { onceki = alt; break; }

            foreach (string[] o in Ogeler)
            {
                if (AdresVar(belge, o[2])) continue;
                XmlElement d = belge.CreateElement(baslik.Name);
                d.SetAttribute("ID", o[0]);
                d.SetAttribute("Text", o[1]);
                d.SetAttribute("URL", o[2]);
                d.SetAttribute("Yetki", ((int)ENUMKullaniciTipi.SISTEMYONETICISI).ToString());
                d.SetAttribute("Durum", "Eski");
                if (onceki != null) hedef.InsertAfter(d, onceki);
                else hedef.AppendChild(d);
                onceki = d;
            }
        }

        private static bool AdresVar(XmlDocument belge, string url)
        {
            XmlNodeList dugumler = belge.SelectNodes("//*[@URL]");
            if (dugumler != null)
                foreach (XmlNode d in dugumler)
                    if (string.Equals(d.Attributes["URL"].Value.Trim(), url, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }
    }
}
