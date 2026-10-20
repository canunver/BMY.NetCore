using System;
using System.Collections.Concurrent;
using System.IO;
using System.Xml;
using TNS.KYM;

namespace Istemci.OrtakSayfa
{
    /// <summary>
    /// Sayfaya giriş yetkisinin ortak denetimi (eski GenelIslemler.SayfayaGirisYapabilirMi). Sayfa modelleri ve sayfanın JSON uçları
    /// aynı denetimi kullanır:
    ///   - Sistem yöneticisi her sayfaya girer.
    ///   - Sayfa menu.xml'de tanımlıysa, kullanıcı o seçeneği görebiliyor olmalıdır (MenuOkuyucu.SayfaYetkili).
    ///   - Sayfa menüde yoksa (bir ekranın içinde açılan alt sayfa; ör. ProjeGiris içindeki ProjeDetay, AltProjeAmacGiris), yetki
    ///     parametre olarak verilen üst sayfanın menüdeki yetkisinden gelir: kullanıcı üst sayfalardan birini görebiliyorsa girer.
    ///     Üst sayfa verilmemişse girilemez (eski kodda menüde olmayan sayfalar denetlenmiyordu).
    /// Dosyalar, Komut Çalıştır ve Şifre Üret menu.xml'de yoktur (StratekMenu): bu sayfalara yalnızca sistem yöneticisi olan
    /// "stratek" kullanıcısı girer, başka sistem yöneticileri de giremez.
    /// Adresler menu.xml'deki biçimdedir: "../Klasor/Sayfa.aspx" (gerekirse ?parametre ile).
    /// Üst sayfa adı kullanıcıdan (adres satırından) değil, alt sayfanın kendi kodundan verilir; böylece kullanıcı adrese başka
    /// bir sayfanın adını yazarak yetkisini genişletemez.
    /// </summary>
    public static class SayfaYetki
    {
        /// <param name="kullanan">Oturumdaki kullanıcı (yoksa false).</param>
        /// <param name="uygulamaKlasoru">IWebHostEnvironment.ContentRootPath (menu.xml'in yeri bundan bulunur).</param>
        /// <param name="sayfaAdresi">Sayfanın menüdeki adresi, ör. "../ButceHazirlikv2/AltProjeDetay.aspx".</param>
        /// <param name="ustSayfalar">Sayfa menüde yoksa yetkisi denetlenecek üst sayfalar, ör. "../ButceHazirlikv2/ProjeGiris.aspx".</param>
        public static bool Yetkili(Kullanici kullanan, string uygulamaKlasoru, string sayfaAdresi, params string[] ustSayfalar)
        {
            if (kullanan == null || string.IsNullOrEmpty(kullanan.kullaniciKodu)) return false;
            if (StratekMenu.SayfaVar(sayfaAdresi)) return StratekMenu.Stratek(kullanan);
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)) return true;
            try
            {
                string dosya = MenuOkuyucu.DosyaYolu(uygulamaKlasoru);
                MenuOkuyucu okuyucu = new MenuOkuyucu(kullanan);
                if (!string.IsNullOrEmpty(sayfaAdresi) && MenudeVar(dosya, sayfaAdresi))
                    return okuyucu.SayfaYetkili(dosya, sayfaAdresi);
                if (ustSayfalar != null)
                    foreach (string ust in ustSayfalar)
                        if (!string.IsNullOrEmpty(ust) && okuyucu.SayfaYetkili(dosya, ust)) return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // menu.xml'deki adresler (dosya değişince yeniden okunur). Görünürlükten bağımsızdır: yalnızca sayfanın menüde tanımlı olup
        // olmadığına bakılır; tanımlıysa yetki MenuOkuyucu ile denetlenir.
        private class AdresListesi
        {
            public DateTime zaman;
            public ConcurrentDictionary<string, bool> adresler;
        }

        private static readonly ConcurrentDictionary<string, AdresListesi> onbellek = new ConcurrentDictionary<string, AdresListesi>(StringComparer.OrdinalIgnoreCase);

        private static bool MenudeVar(string dosya, string adres)
        {
            DateTime zaman = File.GetLastWriteTimeUtc(dosya);
            if (!onbellek.TryGetValue(dosya, out AdresListesi liste) || liste.zaman != zaman)
            {
                liste = new AdresListesi { zaman = zaman, adresler = new ConcurrentDictionary<string, bool>(StringComparer.OrdinalIgnoreCase) };
                XmlDocument belge = new XmlDocument();
                belge.Load(dosya);
                XmlNodeList dugumler = belge.SelectNodes("//*[@URL]");
                if (dugumler != null)
                    foreach (XmlNode d in dugumler)
                    {
                        string url = d.Attributes["URL"].Value;
                        if (!string.IsNullOrWhiteSpace(url)) liste.adresler[url.Trim()] = true;
                    }
                onbellek[dosya] = liste;
            }
            return liste.adresler.ContainsKey(adres);
        }
    }
}
