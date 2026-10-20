using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.IKY;
using TNS.KYM;

namespace Istemci.InsanKaynaklari
{
    /// <summary>
    /// Personel İşlemleri (eski InsanKaynaklari/PersonelBilgi.aspx; yeni sayfa InsanKaynaklari/PersonelBilgi) uçları:
    ///   GET /api/personelbilgi/tanim                    açılış: unvanlar, personel tip grupları, türler, önerilen birim (eski Page_Load)
    ///   GET /api/personelbilgi/turler?gruplar=a;b       seçilen gruplara göre personel türleri (eski ddlPersonelTipGrup_Changed)
    ///   GET /api/personelbilgi/orgagac?ust=&amp;ara=        birim seçimi (eski ExtOrgSecim, "Personel Saymanlık-Birim Yetkisi")
    ///   GET /api/personelbilgi/liste?...                personel arama (eski ListeStore_ReadData / PersonelListele, 250'lik sayfalar)
    ///   GET /api/personelbilgi/personel?kod=            seçilen personelin özeti ve görebileceği sekmeler (eski EkranHazirla)
    ///   GET /InsanKaynaklari/ResimGoster.ashx?pKod=&amp;kucuk=   personel resmi (eski ResimGoster.ashx; Personel.resimBoy2 bu adresi verir)
    /// İş kuralları eski sayfadaki gibi: IIKYServis.PersonelListele, UnvanListele, ListelePersonelTip(Grup), KullanicininIslemleri,
    /// PersonelResimGetir.
    /// </summary>
    public static class PersonelBilgiUclari
    {
        private const string Uc = "/api/personelbilgi/";
        private const string SayfaAd = "PersonelBilgi";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "tanim", Tanim);
            app.MapGet(Uc + "turler", Turler);
            app.MapGet(Uc + "orgagac", OrgAgac);
            app.MapGet(Uc + "liste", Liste);
            app.MapGet(Uc + "personel", PersonelOku);
            app.MapGet("/InsanKaynaklari/ResimGoster.ashx", ResimGoster);
        }

        private static IIKYServis Servis() { return TNS.IKY.Arac.Tanimla(); }

        private static IResult Hata(string metin) { return Results.Json(new { hata = metin }); }

        // ------------------------------------------------------------------ açılış

        /// <summary>
        /// Eski Page_Load: Genel.UnvanDoldur (ilk satır boş), Genel.PersonelTipGrupDoldur, Genel.PersonelTurDondur(boş) ve
        /// kullanıcının yetkili birim köprü kodunun son üç hanesinden önerilen birim.
        /// </summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                List<object> unvanlar = new List<object>();
                unvanlar.Add(new { kod = 0, ad = "" });
                string hata = "";
                ObjectArray oa = Servis().UnvanListele(new Unvan());
                if (oa.sonuc.islemSonuc)
                {
                    foreach (Unvan u in oa.objeler) unvanlar.Add(new { kod = u.UnvanKod, ad = u.UnvanAd });
                }
                else hata = PersonelBilgiOrtak.HataMetni(oa.sonuc.hataStr, "Unvanlar okunamadı.");

                List<object> gruplar = new List<object>();
                foreach (object o in TNS.IKY.Arac.PersonelTipGrupListe)
                    if (o is PersonelTipi g && !string.IsNullOrEmpty(g.ad)) gruplar.Add(new { ad = g.ad });

                int birimKod = 0;
                string birimAd = "";
                try
                {
                    string kopru = Convert.ToString(kullanan.yetkiTipiDegerleri["KopruKod", 0]) ?? "";
                    if (!string.IsNullOrWhiteSpace(kopru) && kopru.Length >= 3)
                    {
                        TNS.ORG.OrganizasyonBirim b = TNS.ORG.Arac.OrgBirimBul(kopru.Substring(kopru.Length - 3, 3));
                        if (b != null) { birimKod = b.kod; birimAd = b.adi ?? ""; }
                    }
                }
                catch (Exception) { /* birim önerilmez */ }

                return Results.Json(new { unvanlar, gruplar, turler = TurListesi(null), birimKod, birimAd, hata });
            }
            catch (Exception ex)
            {
                return Hata(PersonelBilgiOrtak.HataMetni(ex.Message, "Sayfa bilgileri okunamadı."));
            }
        }

        private static IResult Turler(HttpContext baglam, IWebHostEnvironment ortam, string gruplar)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                return Results.Json(new { turler = TurListesi(Gruplar(gruplar)) });
            }
            catch (Exception ex)
            {
                return Hata(PersonelBilgiOrtak.HataMetni(ex.Message, "Personel türleri okunamadı."));
            }
        }

        internal static string[] Gruplar(string gruplar)
        {
            List<string> l = new List<string>();
            foreach (string g in (gruplar ?? "").Split(';'))
                if (!string.IsNullOrWhiteSpace(g)) l.Add(g.Trim());
            return l.ToArray();
        }

        /// <summary>Eski Genel.PersonelTurDondur.</summary>
        internal static List<object> TurListesi(string[] tipGruplar)
        {
            TNSCollection liste;
            if (tipGruplar == null || tipGruplar.Length == 0)
                liste = TNS.IKY.Arac.PersonelTipListe;
            else
            {
                PersonelTipi pt = new PersonelTipi();
                pt.tipGruplar = tipGruplar;
                liste = Servis().ListelePersonelTip(null, pt).objeler;
            }
            List<object> turler = new List<object>();
            if (liste != null)
                foreach (object o in liste)
                    if (o is PersonelTipi t) turler.Add(new { kod = t.kod, ad = t.ad });
            return turler;
        }

        // ------------------------------------------------------------------ birim seçimi

        /// <summary>
        /// Eski ExtOrgSecim (KullanicininBirimleri = "Personel Saymanlık-Birim Yetkisi"): kökte kullanıcının yetkili olduğu birimler
        /// (yetkisi yoksa bütün kök birimler); ara yazılınca adında geçen birimler düz liste olarak (il / ilçe adıyla) gelir.
        /// </summary>
        private static IResult OrgAgac(HttpContext baglam, IWebHostEnvironment ortam, int? ust, string ara)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            return BirimSatirlari(kullanan, ust, ara);
        }

        /// <summary>Birim seçimi satırları (Personel Raporları sayfası da kullanır).</summary>
        internal static IResult BirimSatirlari(Kullanici kullanan, int? ust, string ara)
        {
            try
            {
                ara = (ara ?? "").Trim();
                TNS.ORG.OrganizasyonBirim kriter = new TNS.ORG.OrganizasyonBirim();
                kriter.anaBirimKod = Math.Max(0, ust ?? 0);
                kriter.sablonMu = false;
                if (ara != "")
                {
                    kriter.anaBirimKod = 0;
                    kriter.adi = ara;
                }
                else if (kriter.anaBirimKod == 0)
                {
                    string yetkili = kullanan.YetkiTipiDegeriBul("Personel Saymanlık-Birim Yetkisi") ?? "";
                    bool birimYetkisiVar = false;
                    foreach (string y in yetkili.Split(';'))
                    {
                        string[] m = y.Split('-');
                        if (m.Length == 2 && (OrtakFonksiyonlar.ConvertToInt(m[0], 0) > 0 || OrtakFonksiyonlar.ConvertToInt(m[1], 0) > 0))
                            birimYetkisiVar = true;
                    }
                    if (birimYetkisiVar) kriter.yetkiliOlduguMuhasebeBirim = yetkili;
                    else kriter.kopruKod = "999";
                }

                ObjectArray<TNS.ORG.OrganizasyonBirim> sonuc = TNS.ORG.Arac.Tanimla().OrganizasyonBirimListele(kriter, false, false);
                if (sonuc == null || !sonuc.sonuc.islemSonuc) return Hata("Birimler okunamadı.");
                List<object> satirlar = new List<object>();
                foreach (TNS.ORG.OrganizasyonBirim b in sonuc.objeler)
                {
                    string ad = b.adi ?? "";
                    if (ara != "")
                    {
                        string ilIlce = TNS.ORG.Arac.OrgBirimIlIlceAdBul(b.kod);
                        if (!string.IsNullOrEmpty(ilIlce)) ad += " (" + ilIlce + ")";
                    }
                    satirlar.Add(new { kod = b.kod, ad = ad, birimAd = b.adi ?? "", altVar = ara == "" && b.altDalVar });
                    if (satirlar.Count >= 300) break;
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Birimler okunamadı.");
            }
        }

        // ------------------------------------------------------------------ personel listesi

        /// <summary>
        /// Eski PersonelListele: kriterler eski sayfadaki gibi aktarılır. Tür seçilmemişse seçilen gruplar (tipGruplar), seçilmişse
        /// türler (personelTipleri, ";" ile ayrılmış kodlar) gider. basla / sayfa: eski sayfalama (250'lik).
        /// </summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, string ara, int? birim, int? unvan, string gruplar,
            string turler, int? aktif, int? siralama, int? basla, int? sayfa)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                TNS.IKY.Personel p = new TNS.IKY.Personel();
                p.kadro.organizasyonKodu = birim ?? 0;
                p.kadro.unvan = unvan ?? 0;
                p.kadro.vekilUnvanKod = unvan ?? 0;
                p.genelSorgu = ara ?? "";

                string personelTurler = (turler ?? "").Trim();
                if (string.IsNullOrWhiteSpace(personelTurler))
                    p.tipGruplar = Gruplar(gruplar);
                else
                    p.personelTipleri = personelTurler;

                p.aktifDurum = aktif ?? 1;
                p.kayitBasla = Math.Max(0, basla ?? 0);
                p.kayitToplam = (sayfa ?? 250) > 0 ? (sayfa ?? 250) : 250;
                p.tarihce = false;
                int sira = siralama ?? 1;
                p.listeSiralama = Enum.IsDefined(typeof(ENUMPersonelSira), sira) ? (ENUMPersonelSira)sira : ENUMPersonelSira.Unvan;

                ObjectArray liste = Servis().PersonelListele(kullanan, p, false, true);
                if (!liste.sonuc.islemSonuc) return Hata(PersonelBilgiOrtak.HataMetni(liste.sonuc.hataStr, "Personel listesi okunamadı."));

                List<object> satirlar = new List<object>();
                foreach (object o in liste.objeler)
                {
                    if (!(o is TNS.IKY.Personel x)) continue;
                    satirlar.Add(new
                    {
                        x.kod,
                        x.sicilNo,
                        x.adSoyad,
                        x.personelTip,
                        x.personelTipAd,
                        x.birimAd,
                        x.resimBase64,
                        x.unvanAd,
                        x.unvanVekaletAd
                    });
                }
                return Results.Json(new { satirlar, toplam = liste.sonuc.kayitSay });
            }
            catch (Exception ex)
            {
                return Hata(PersonelBilgiOrtak.HataMetni(ex.Message, "Personel listesi okunamadı."));
            }
        }

        // ------------------------------------------------------------------ seçilen personel

        /// <summary>Sekme: eski TabPanelAd sırası, başlığı, sayfası ve yetki işlemi (Lojman Core'da yok).</summary>
        private class Sekme
        {
            public string id;
            public string baslik;
            public string sayfa;
            public ENUMYetkiIslem islem;
            public Sekme(string id, string baslik, string sayfa, ENUMYetkiIslem islem) { this.id = id; this.baslik = baslik; this.sayfa = sayfa; this.islem = islem; }
        }

        private static readonly Sekme Kisisel = new Sekme("tabKisisel", "Kişisel", "PersonelKisisel", ENUMYetkiIslem.OzlukDetay);
        private static readonly Sekme Kadro = new Sekme("tabKadro", "Hizmet Belgesi", "PersonelKadro", ENUMYetkiIslem.Hizmetler);
        private static readonly Sekme Askerlik = new Sekme("tabAskerlik", "Askerlik", "PersonelAskerlik", ENUMYetkiIslem.Askerlik);

        // Eski TabPanelleriOlustur sırası (Kişisel ilk, Askerlik yalnız erkeklerde, Lojman kaldırıldı).
        private static readonly Sekme[] DigerSekmeler =
        {
            Kadro,
            new Sekme("tabIzin", "İzin/Görev", "PersonelIzin", ENUMYetkiIslem.Izin),
            new Sekme("tabGorevler", "Görevlendirmeler", "PersonelGorev", ENUMYetkiIslem.Hizmetler),
            new Sekme("tabAile", "Aile", "PersonelAile", ENUMYetkiIslem.Aile),
            new Sekme("tabMaasParam", "Maaş Parametreleri", "PersonelMaasParam", ENUMYetkiIslem.MaasParametreleri),
            new Sekme("tabAdres", "İletişim/Adres", "PersonelAdres", ENUMYetkiIslem.Iletisim),
            new Sekme("tabKullanici", "Kullanıcı İşlemleri", "PersonelKullaniciIslemleri", ENUMYetkiIslem.KullaniciIslemleri),
            new Sekme("tabGider", "Giderler", "PersonelGider", ENUMYetkiIslem.Gider),
            new Sekme("tabMalBildirimi", "Mal Bildirimi", "PersonelMalBildirimi", ENUMYetkiIslem.MalBildirimi),
            new Sekme("tabOgrenim", "Öğrenim Durumu", "PersonelOgrenim", ENUMYetkiIslem.Ogrenim),
            new Sekme("tabYabanci", "Yabancı Dil", "PersonelDil", ENUMYetkiIslem.YabanciDil),
            new Sekme("tabHizmetIciEgitim", "Hizmet İçi Eğitim", "PersonelKurs", ENUMYetkiIslem.HizmetIciEgitim),
            new Sekme("tabCeza", "Ceza/Ödül", "PersonelCezaOdul", ENUMYetkiIslem.CezaOdul),
            Askerlik,
            new Sekme("tabIzinDevir", "İzin Devir", "PersonelIzinDevirTab", ENUMYetkiIslem.IzinDevir),
            new Sekme("tabTarihce", "Tarihçe", "PersonelTarihce", ENUMYetkiIslem.Tarihce),
            new Sekme("tabKart", "Kart", "PersonelKart", ENUMYetkiIslem.Kartlar),
        };

        /// <summary>Eski Genel.StringdeVarMi.</summary>
        private static bool StringdeVarMi(int islemKod, string islemlerListe)
        {
            if (string.IsNullOrWhiteSpace(islemlerListe)) return false;
            foreach (string s in islemlerListe.Split(',', ';'))
                if (OrtakFonksiyonlar.ConvertToInt(s, -1) == islemKod) return true;
            return false;
        }

        /// <summary>Eski TabPanelYetki: pasif personelde Kişisel ve Hizmet Belgesi her zaman görünür.</summary>
        private static bool SekmeYetkili(TNS.IKY.Personel p, Sekme s, string islemlerListe)
        {
            if (p.aktifMi == 0 && (s == Kadro || s == Kisisel)) return true;
            return StringdeVarMi((int)s.islem, islemlerListe);
        }

        private static object SekmeYaz(Sekme s)
        {
            return new { s.id, s.baslik, s.sayfa };
        }

        /// <summary>
        /// Eski EkranHazirla + TabPanelleriOlustur: kod > 0 ise personelin özeti ve görebileceği sekmeler; kod = 0 ise yeni kayıt
        /// (yalnız Kişisel sekmesi, yetkisi varsa — eski YeniKayitHazirla).
        /// </summary>
        private static IResult PersonelOku(HttpContext baglam, IWebHostEnvironment ortam, int? kod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                int pk = kod ?? 0;
                IIKYServis servis = Servis();
                TNS.IKY.Personel p = new TNS.IKY.Personel();
                if (pk > 0)
                {
                    p.kod = pk;
                    ObjectArray liste = servis.PersonelListele(kullanan, p, false, true);
                    if (!liste.sonuc.islemSonuc) return Hata(PersonelBilgiOrtak.HataMetni(liste.sonuc.hataStr, "Personel bilgileri okunamadı."));
                    if (liste.objeler.Count != 1) return Hata("Personel bulunamadı.");
                    p = (TNS.IKY.Personel)liste[0];
                }

                string islemlerListe = servis.KullanicininIslemleri(kullanan, p.kopruKodTam, TNS.MUH.ENUMOGY.Gorme);
                List<object> sekmeler = new List<object>();
                if (SekmeYetkili(p, Kisisel, islemlerListe)) sekmeler.Add(SekmeYaz(Kisisel));
                if (p.kod > 0)
                {
                    foreach (Sekme s in DigerSekmeler)
                    {
                        if (s == Askerlik && p.nufus.cinsiyeti != ENUMCINSIYET.ERKEK) continue;
                        if (SekmeYetkili(p, s, islemlerListe)) sekmeler.Add(SekmeYaz(s));
                    }
                }

                if (p.kod <= 0) return Results.Json(new { kod = 0, sekmeler });

                return Results.Json(new
                {
                    kod = p.kod,
                    tcKimlikNo = p.tcKimlikNo ?? "",
                    personelTipleri = p.personelTipleri ?? "",
                    baslik = string.Format("{0}-{1} {2}/{3}", p.TCKIMLIKNO, p.ad, p.soyad, p.sicilNo),
                    resim = p.resimBoy2,
                    tipUnvan = string.Format("{0}/{1}", p.personelTipAd, p.unvanAd),
                    kadroYeri = p.kadro.birimGorevAd ?? "",
                    iletisim = string.Format("{0}-{1}-{2}", p.cepTelefon, p.isTelefon, p.EPOSTA),
                    kurumaBaslama = PersonelBilgiOrtak.TarihYaz(p.kurumaBaslamaTarihi),
                    gorevYeri = p.kadro.gorevBirimAd ?? "",
                    vekilUnvan = p.kadro.vekilUnvanAd ?? "",
                    kadroDerece = p.KADRODERECE + "/" + p.emekliEsasEkGosterge,
                    kazanilmis = p.kazanilmisHakDerece + "-" + p.kazanilmisHakKademe + "/" + p.kazanilmisEkGosterge,
                    emekli = p.emekliEsasDerece + "-" + p.emekliEsasKademe + "/" + p.emekliEsasEkGosterge,
                    aktifMi = p.aktifMi,
                    sekmeler
                });
            }
            catch (Exception ex)
            {
                return Hata(PersonelBilgiOrtak.HataMetni(ex.Message, "Personel bilgileri okunamadı."));
            }
        }

        // ------------------------------------------------------------------ resim

        /// <summary>Eski ResimGoster.ashx: IIKYServis.PersonelResimGetir(kod, büyüklük, 0, 0), image/jpeg.</summary>
        private static IResult ResimGoster(HttpContext baglam, IWebHostEnvironment ortam, int? pKod, int? kucuk)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                byte[] resim = Servis().PersonelResimGetir(pKod ?? 0, kucuk ?? 0, 0, 0);
                if (resim == null || resim.Length == 0) return Results.NotFound();
                baglam.Response.Headers["Cache-Control"] = "no-store";
                return Results.File(resim, "image/jpeg");
            }
            catch (Exception)
            {
                return Results.NotFound();
            }
        }
    }
}
