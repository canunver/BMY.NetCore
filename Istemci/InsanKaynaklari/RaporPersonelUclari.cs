using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.IKY;
using TNS.KYM;

namespace Istemci.InsanKaynaklari
{
    /// <summary>
    /// Personel Raporları (eski InsanKaynaklari/RaporPersonel.aspx ve RaporPersonel.ashx; yeni sayfa InsanKaynaklari/RaporPersonel) uçları:
    ///   GET /api/raporpersonel/tanim                       rapor ağacı, rapor alanları, grup / tür / izin türü / dinamik alan listeleri (eski Page_Load)
    ///   GET /api/raporpersonel/turler?gruplar=a;b          seçilen gruplara göre personel türleri (eski ddlPersonelTipGrup_Changed)
    ///   GET /api/raporpersonel/orgagac?ust=&amp;ara=           birim seçimi (eski ExtOrgSecim, "Personel Saymanlık-Birim Yetkisi")
    ///   GET /api/raporpersonel/personeller?...             rapora girecek personelin seçimi (eski tfArama_Ara_Clicked)
    ///   GET /api/raporpersonel/rapor?tur=1|2|3&amp;...         raporu üretir (eski btnYazdir_Click + RaporPersonel.ashx):
    ///        tur=1 Göster: PDF, tarayıcıda açılır (eski ashx); tur=2 İndir: Excel / Word; tur=3 İndir (PDF).
    /// Raporlar eski sayfadaki gibi InsanKaynaklari.PersonelRaporlari / PersonelRaporlari2 RaporAl ile üretilir; şablonlar önce kurumun
    /// RaporSablonYol\IKY klasöründe, yoksa uygulamayla gelen RaporSablon\IKY klasöründe aranır.
    /// </summary>
    public static class RaporPersonelUclari
    {
        private const string Uc = "/api/raporpersonel/";
        public const string SayfaAdresi = "../InsanKaynaklari/RaporPersonel.aspx";
        private static readonly CultureInfo Tr = new CultureInfo("tr-TR");

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "tanim", Tanim);
            app.MapGet(Uc + "turler", Turler);
            app.MapGet(Uc + "orgagac", OrgAgac);
            app.MapGet(Uc + "personeller", Personeller);
            app.MapGet(Uc + "rapor", Rapor);
        }

        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, SayfaAdresi);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IIKYServis Servis() { return TNS.IKY.Arac.Tanimla(); }

        private static IResult Hata(string metin) { return Results.Json(new { hata = metin }); }

        // ------------------------------------------------------------------ açılış

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                Dictionary<string, object> raporlar = new Dictionary<string, object>();
                foreach (RaporPersonelTanim.RaporTanim r in RaporPersonelTanim.raporTanimlar)
                {
                    if (RaporPersonelTanim.Gizli(r.KOD)) continue;
                    List<string> siralamalar = new List<string>();
                    foreach (string s in (r.siralamaListe ?? "").Split(','))
                        if (!string.IsNullOrWhiteSpace(s)) siralamalar.Add(s.Trim());
                    raporlar[r.KOD] = new
                    {
                        kod = r.KOD,
                        ad = r.AD,
                        grup = r.raporGrubu,
                        doc = r.cmpCiktiTurDoc,
                        birim = r.cmpBirim,
                        kadroDerece = r.cmpKadroDerece,
                        islemSira = r.cmpIslemSira,
                        personelTur = r.cmpPersonelTurCoklu,
                        yukumlu = r.cmpYukumlu,
                        kisiArama = r.cmpKisiArama,
                        kisiAramaGrid = r.cmpKisiAramaGrid,
                        dinamik = r.cmpDinamik,
                        sonTarih = r.cmpSonTarih,
                        aktiflik = r.cmpAktiflik,
                        site = r.cmpSiteKod,
                        daire = r.cmpDaireBosluk,
                        yil = r.cmpYil,
                        ay = r.cmpAy,
                        yilArasi = r.cmpYilArasi,
                        tarihSorgu = r.tarihSorgu,
                        izinTur = r.izinTur,
                        siralamalar
                    };
                }

                List<object> gruplar = new List<object>();
                foreach (object o in TNS.IKY.Arac.PersonelTipGrupListe)
                    if (o is PersonelTipi g && !string.IsNullOrEmpty(g.ad)) gruplar.Add(new { ad = g.ad });

                // Eski Genel.IzinTipDondur(false)
                List<object> izinTurleri = new List<object>();
                if (TNS.IKY.Arac.izinTipListesi == null)
                    TNS.IKY.Arac.izinTipListesi = Servis().IzinTurListele(new IzinTur());
                if (TNS.IKY.Arac.izinTipListesi?.objeler != null)
                    foreach (object o in TNS.IKY.Arac.izinTipListesi.objeler)
                        if (o is IzinTur it) izinTurleri.Add(new { kod = it.kod, ad = it.ad });

                // Eski Genel.DinamikListeDoldur
                List<object> dinamikKolonlar = new List<object>();
                for (int i = 0; i < global::InsanKaynaklari.PersonelRaporlari.dinamikKolonAdlar.Length; i++)
                    dinamikKolonlar.Add(new { kod = i, baslik = global::InsanKaynaklari.PersonelRaporlari.dinamikKolonAdlar[i] });

                // Eski Genel.LojmanSiteDoldur + "Tümü" (yalnız Lojman raporları açıksa)
                List<object> siteler = new List<object>();
                if (RaporPersonelTanim.LojmanRaporlariGoster)
                {
                    siteler.Add(new { kod = -1, ad = "Tümü" });
                    ObjectArray sa = Servis().LojmanSiteListele(kullanan, new LojmanSite());
                    if (sa?.sonuc != null && sa.sonuc.islemSonuc)
                        foreach (object o in sa.objeler)
                            if (o is LojmanSite ls) siteler.Add(new { kod = ls.kod, ad = ls.siteAdi ?? "" });
                }

                DateTime simdi = DateTime.Now;
                return Results.Json(new
                {
                    agac = RaporPersonelTanim.Agac(),
                    raporlar,
                    gruplar,
                    turler = PersonelBilgiUclari.TurListesi(null),
                    izinTurleri,
                    dinamikKolonlar,
                    siteler,
                    yil = simdi.Year,
                    ay = simdi.Month,
                    donemYili = IzinArac.DonemYiliBul(simdi.Year, simdi.Month),
                    bugun = simdi.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                });
            }
            catch (Exception ex)
            {
                return Hata(PersonelBilgiOrtak.HataMetni(ex.Message, "Sayfa bilgileri okunamadı."));
            }
        }

        private static IResult Turler(HttpContext baglam, IWebHostEnvironment ortam, string gruplar)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                return Results.Json(new { turler = PersonelBilgiUclari.TurListesi(PersonelBilgiUclari.Gruplar(gruplar)) });
            }
            catch (Exception ex)
            {
                return Hata(PersonelBilgiOrtak.HataMetni(ex.Message, "Personel türleri okunamadı."));
            }
        }

        private static IResult OrgAgac(HttpContext baglam, IWebHostEnvironment ortam, int? ust, string ara)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return PersonelBilgiUclari.BirimSatirlari(kullanan, ust, ara);
        }

        // ------------------------------------------------------------------ personel seçimi

        /// <summary>Eski tfArama_Ara_Clicked: kriterler aynı aktarılır, iş kuralı IIKYServis.PersonelListele.</summary>
        private static IResult Personeller(HttpContext baglam, IWebHostEnvironment ortam, string raporKod, string ara, int? birim,
            string gruplar, string turler, int? aktif, string tarih, int? tarihTur, int? site)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                TNS.IKY.Personel p = new TNS.IKY.Personel();
                p.genelSorgu = ara ?? "";
                p.kadro.organizasyonKodu = birim ?? 0;

                string personelTurler = (turler ?? "").Trim();
                if (string.IsNullOrWhiteSpace(personelTurler))
                    p.tipGruplar = PersonelBilgiUclari.Gruplar(gruplar);
                else
                    p.personelTipleri = personelTurler;

                p.aktifDurum = aktif ?? 1;
                p.aktifTarih = new TNSDateTime(TarihOku(tarih) ?? DateTime.Now.Date);
                p.aktifTarihTur = tarihTur ?? 0;

                RaporPersonelTanim.RaporTanim raporTanim = RaporPersonelTanim.RaporBul(raporKod ?? "");
                if (raporTanim != null && raporTanim.raporGrubu == "GR012")
                {
                    int s = site ?? 0;
                    if (s == 0) s = -1;
                    p.personelSiteKod = s;
                }

                ObjectArray oa = Servis().PersonelListele(kullanan, p, false, false);
                if (!oa.sonuc.islemSonuc) return Hata(PersonelBilgiOrtak.HataMetni(oa.sonuc.hataStr, "Personel listesi okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (object o in oa.objeler)
                {
                    if (!(o is TNS.IKY.Personel x)) continue;
                    satirlar.Add(new { x.kod, x.tcKimlikNo, x.ad, x.soyad, x.unvanAd, x.personelTipAd, cinsiyetAd = x.nufus?.cinsiyetAd ?? "", x.resimYol });
                }
                return Results.Json(new { satirlar });
            }
            catch (Exception ex)
            {
                return Hata(PersonelBilgiOrtak.HataMetni(ex.Message, "Personel listesi okunamadı."));
            }
        }

        // ------------------------------------------------------------------ rapor

        /// <summary>
        /// Eski btnYazdir_Click (+ RaporPersonel.ashx, raporUret). Gizli alanların değeri eski sayfadaki gibi boş / 0 gider (aktiflik -1).
        /// Değişen: Yükümlülük seçimi eski sayfada hiç gönderilmiyordu (InputValtoInt her zaman 0 döndürüyordu); burada seçilen değer gider.
        /// </summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, int? tur, string raporKod, string gruplar, string turler,
            string islemSira, int? birim, string birimAd, int? altBirimli, int? kadroDerece, string secililer, string sonTarih,
            string dinamikler, string ara, int? aktif, int? site, int? daire, int? yil, int? ay, int? yil1, int? yil2, int? yukumlu,
            string tarih, int? tarihTur, string izinTurler, int? siralama)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            RaporPersonelTanim.RaporTanim r = RaporPersonelTanim.RaporBul(raporKod ?? "");
            if (r == null || RaporPersonelTanim.Gizli(r.KOD)) return Hata("Listeden görmek istediğiniz raporu seçiniz.");

            string dosya = null;
            try
            {
                IIKYServis servisIKY = Servis();

                string personelTurler = "";
                if (r.cmpPersonelTurCoklu)
                {
                    personelTurler = (turler ?? "").Trim();
                    string[] tipGruplar = PersonelBilgiUclari.Gruplar(gruplar);
                    if (string.IsNullOrWhiteSpace(personelTurler) && tipGruplar.Length > 0)
                    {
                        PersonelTipi pt = new PersonelTipi();
                        pt.tipGruplar = tipGruplar;
                        pt.aktif = 1;
                        ObjectArray ptipler = servisIKY.ListelePersonelTip(null, pt);
                        personelTurler = "";
                        foreach (PersonelTipi item in ptipler.objeler)
                        {
                            if (personelTurler != "") personelTurler += ";";
                            personelTurler += item.kod.ToString();
                        }
                    }
                }

                int t = tur ?? 1;
                string ciktiTur = t == 2 ? (r.cmpCiktiTurDoc ? "docx" : "xlsx") : "pdf";

                string seciliKodlar = r.cmpKisiAramaGrid ? KodListesi(secililer, ",") : "";
                string siralamalar = r.cmpDinamik ? KodListesi(dinamikler, ",") : "";

                string birimMetni = "";
                int birimKod = 0;
                if (r.cmpBirim)
                {
                    birimMetni = (birimAd ?? "").ToUpper(Tr);
                    birimKod = birim ?? 0;
                }

                string hata = "";
                if (r.raporGrubu == "GR009" && string.IsNullOrWhiteSpace(siralamalar))
                    hata += " Lütfen raporda görmek istediğiniz alanları seçiniz.";
                if (r.raporGrubu == "GR014" && birimKod == 0)
                    hata += " Rapor almak istediğiniz birimi seçiniz.";
                if (r.KOD == "IK045" && string.IsNullOrEmpty(seciliKodlar))
                    hata += " Lütfen personel seçiniz.";
                if (hata != "") return Hata(hata.Trim());

                string orgKod = KopruKodBul(birimKod) + (r.cmpBirim && (altBirimli ?? 1) == 1 ? "%" : "");
                bool yilArasi = r.cmpYilArasi || r.izinTur;

                Hashtable raporParam = new Hashtable();
                raporParam["personelTur"] = personelTurler;
                raporParam["IslemSiraNo"] = r.cmpIslemSira ? (islemSira ?? "") : "";
                raporParam["BirimAd"] = birimMetni;
                raporParam["ORGKOD"] = orgKod;
                raporParam["personelKod"] = seciliKodlar;
                raporParam["aktifDurumlar"] = "";
                raporParam["sonTarih"] = r.cmpSonTarih ? EskiTarih(sonTarih) : "";
                raporParam["siralamaListesi"] = siralamalar;
                raporParam["siralamaKod"] = siralama ?? -1;
                raporParam["sorgu"] = r.cmpKisiArama ? (ara ?? "") : "";
                raporParam["aktiflik"] = r.cmpAktiflik ? (aktif ?? 1) : -1;
                raporParam["site"] = r.cmpSiteKod ? (site ?? 0) : 0;
                raporParam["daireBosluk"] = r.cmpDaireBosluk ? (daire ?? 0) : 0;
                raporParam["yil"] = r.cmpYil ? (yil ?? 0) : 0;
                raporParam["ay"] = r.cmpAy ? (ay ?? 0) : 0;
                raporParam["yil1"] = yilArasi ? (yil1 ?? 0) : 0;
                raporParam["yil2"] = yilArasi ? (yil2 ?? 0) : 0;
                raporParam["yukumlu"] = r.cmpYukumlu ? (yukumlu ?? 0) : 0;
                raporParam["yerleskeler"] = "";
                raporParam["tarihSorgu"] = r.tarihSorgu ? EskiTarih(tarih) : "";
                raporParam["tarihSorguTur"] = r.tarihSorgu ? (tarihTur ?? 0) : 0;
                raporParam["izinTurler"] = r.izinTur ? KodListesi(izinTurler, ";") : "";

                string sablonAd = SablonYolu(ortam, r.DOSYAAD);
                if (sablonAd == null) return Hata("Rapor şablonu bulunamadı (IKY\\" + r.DOSYAAD + ").");
                string sonucDosyaAd = DosyaIslem.DosyaAdUret("IKY", "");
                global::InsanKaynaklari.PersonelRaporlari2.SirtlikLogoYolu = Path.Combine(ortam.WebRootPath ?? "", "app_themes", "images", "CBLogo.png");

                if (r.raporDosyasiNo == 1)
                    dosya = new global::InsanKaynaklari.PersonelRaporlari2().RaporAl(kullanan, servisIKY, sablonAd, sonucDosyaAd, ciktiTur, raporParam, r.KOD);
                else
                    dosya = new global::InsanKaynaklari.PersonelRaporlari().RaporAl(kullanan, servisIKY, sablonAd, sonucDosyaAd, ciktiTur, raporParam, r.KOD);

                if (string.IsNullOrWhiteSpace(dosya) || !File.Exists(dosya)) return Hata("Rapor hazırlanamadı.");

                string gidenAd = DosyaAdiTemizle(r.AD) + "_" + DateTime.Now.Ticks.ToString() + "." + ciktiTur;
                if (t == 1)
                {
                    // Göster: tarayıcı PDF'i kendi içinde açar (eski ashx, ekOlarakGonder = false).
                    byte[] icerik = File.ReadAllBytes(dosya);
                    try { File.Delete(dosya); } catch { }
                    baglam.Response.Headers.ContentDisposition = "inline; filename*=UTF-8''" + Uri.EscapeDataString(gidenAd);
                    return Results.Bytes(icerik, "application/pdf");
                }
                return DosyaIslem.DosyaGonder(dosya, gidenAd, true, ciktiTur);
            }
            catch (Exception ex)
            {
                return Hata(PersonelBilgiOrtak.HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }

        /// <summary>Şablon: önce kurumun RaporSablonYol\IKY, yoksa uygulamanın RaporSablon\IKY klasörü. Şablonsuz raporlarda klasör yolu.</summary>
        private static string SablonYolu(IWebHostEnvironment ortam, string dosyaAd)
        {
            if (string.IsNullOrWhiteSpace(dosyaAd))
            {
                string kurumYol = Ayar.Oku("RaporSablonYol");
                if (kurumYol != "" && Directory.Exists(Path.Combine(kurumYol, "IKY"))) return Path.Combine(kurumYol, "IKY") + Path.DirectorySeparatorChar;
                return Path.Combine(ortam.ContentRootPath, "RaporSablon", "IKY") + Path.DirectorySeparatorChar;
            }
            return PersonelOrtakRaporlar.SablonBul(ortam, "IKY", dosyaAd);
        }

        private static string KopruKodBul(int orgKod)
        {
            if (orgKod > 0)
            {
                TNS.ORG.OrganizasyonBirim b = TNS.ORG.Arac.OrgBirimBul(orgKod);
                return b?.kopruKod ?? "";
            }
            return "";
        }

        /// <summary>"1,2;3" gibi bir listeden yalnız sayıları alır, ayraçla birleştirir.</summary>
        private static string KodListesi(string liste, string ayrac)
        {
            List<string> l = new List<string>();
            foreach (string s in (liste ?? "").Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                if (int.TryParse(s.Trim(), out int k)) l.Add(k.ToString(CultureInfo.InvariantCulture));
            return string.Join(ayrac, l);
        }

        private static DateTime? TarihOku(string s)
        {
            s = (s ?? "").Trim();
            if (s == "") return null;
            string[] bicimler = { "yyyy-MM-dd", "dd.MM.yyyy", "d.M.yyyy" };
            if (DateTime.TryParseExact(s, bicimler, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d)) return d;
            return null;
        }

        /// <summary>Eski DateField.Text biçimi (gg.aa.yyyy); rapor sınıfları tarihi bu metinden okur.</summary>
        private static string EskiTarih(string s)
        {
            DateTime? d = TarihOku(s);
            return d.HasValue ? d.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : "";
        }

        private static string DosyaAdiTemizle(string ad)
        {
            foreach (char c in Path.GetInvalidFileNameChars()) ad = ad.Replace(c, '_');
            return ad.Replace('/', '_').Replace(' ', '_');
        }
    }
}
