using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.IKY;
using TNS.KYM;

namespace Istemci.InsanKaynaklari
{
    /// <summary>Kaydet isteği (eski btnKaydet_Click'te ekrandan toplanan alanlar). Tarihler "yyyy-MM-dd".</summary>
    public class PersonelIzinIstegi
    {
        public int personelKod { get; set; }
        public string tcKimlikNo { get; set; }
        public int kod { get; set; }
        public int izinTurKod { get; set; }
        public string baslangicTarihi { get; set; }
        public string bitisTarihi { get; set; }
        public string izinYeri { get; set; }
        public string aciklama { get; set; }
        public string telefon { get; set; }
        public string belgeNo { get; set; }
        public string belgeTarihi { get; set; }
        public string memuriyeti { get; set; }
        public string saglikKurulusu { get; set; }
        public string raporTarihi { get; set; }
        public string raporNo { get; set; }
        public int yatiliGecenSure { get; set; }
        public string yerKod { get; set; }
        public int imzalayan { get; set; }
        public int teklifEden { get; set; }
        public int yerineGorevli { get; set; }
        public string izinNotu2 { get; set; }
    }

    /// <summary>Sil isteği.</summary>
    public class PersonelIzinSilIstegi
    {
        public int personelKod { get; set; }
        public int kod { get; set; }
    }

    /// <summary>Yazdır öncesi imza güncelleme isteği (eski btnYazdir_Click'in ilk bölümü).</summary>
    public class PersonelIzinImzaIstegi
    {
        public int personelKod { get; set; }
        public int kod { get; set; }
        public int teklifEden { get; set; }
        public int imzalayan { get; set; }
        public string imzalayanAd { get; set; }
    }

    /// <summary>Belge silme isteği.</summary>
    public class PersonelIzinBelgeSilIstegi
    {
        public int personelKod { get; set; }
        public int kayitKod { get; set; }
        public int kod { get; set; }
    }

    /// <summary>
    /// Personel İşlemleri "İzin/Görev" sekmesi (eski InsanKaynaklari/PersonelIzin.aspx; yeni sayfa InsanKaynaklari/PersonelIzin) uçları:
    ///   GET  /api/personelbilgi/izin/tanim?personelKod=           izin türleri, telefonlar, dönem yılı (eski Page_Load, TelefonDoldur)
    ///   GET  /api/personelbilgi/izin/liste?personelKod=&amp;yil=       izinler + izin süre bilgileri (eski Listele, IzinSureBilgiHesapla)
    ///   GET  /api/personelbilgi/izin/gun?izinTurKod=&amp;baslangic=&amp;bitis=   (eski GunAl)
    ///   POST /api/personelbilgi/izin/kaydet, /sil                 (eski btnKaydet_Click, btnSil_Click)
    ///   POST /api/personelbilgi/izin/imza                         Yazdır'ın ilk adımı: yetki ve imza güncelleme
    ///   GET  /api/personelbilgi/izin/yazdir?personelKod=&amp;kod=&amp;yil= izin formu (IKY\PersonelYillikIzin / MazeretIzin / SihhiIzin.docx; PDF)
    ///   GET  /api/personelbilgi/izin/belge/liste, /belge/goster; POST /belge/yukle, /belge/sil   (eski Belge* yordamları)
    ///   GET  /api/personelbilgi/izin/personeller?birim=&amp;ara=      (eski ListePersonelSec.aspx: PersonelListeleIC)
    ///   GET  /api/personelbilgi/izin/birimler?ust=&amp;ara=           (ListePersonelSec'teki ExtOrgSecim; kullanıcının birimleri)
    ///   GET  /api/personelbilgi/izin/yerler?ust=&amp;seviye=          (eski ../ButceHazirlikv2/KodAgac.aspx?kodTur=20)
    /// İş kuralları eski sayfadaki gibi IIKYServis üzerinden çağrılır.
    /// </summary>
    public static class PersonelIzinUclari
    {
        private const string SayfaAd = "PersonelIzin";
        private const string Onek = "/api/personelbilgi/izin";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Onek + "/tanim", Tanim);
            app.MapGet(Onek + "/liste", Liste);
            app.MapGet(Onek + "/gun", Gun);
            app.MapPost(Onek + "/kaydet", Kaydet);
            app.MapPost(Onek + "/sil", Sil);
            app.MapPost(Onek + "/imza", Imza);
            app.MapGet(Onek + "/yazdir", Yazdir);
            app.MapGet(Onek + "/belge/liste", BelgeListe);
            app.MapGet(Onek + "/belge/goster", BelgeGoster);
            app.MapPost(Onek + "/belge/yukle", BelgeYukle).DisableAntiforgery();
            app.MapPost(Onek + "/belge/sil", BelgeSil);
            app.MapGet(Onek + "/yerler", Yerler);
        }

        private static IIKYServis Servis() { return TNS.IKY.Arac.Tanimla(); }

        private static string T(TNSDateTime t) { return PersonelBilgiOrtak.TarihYaz(t); }

        // ------------------------------------------------------------------ okuma

        /// <summary>Eski Page_Load: izin türleri (Genel.IzinTipDondur(false)), dönem yılı, telefonlar (TelefonDoldur), Manas ayarı.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam, int personelKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            IIKYServis servisIKY = Servis();

            // Genel.IzinTipDondur(false): önbellekteki izin türlerinin tamamı
            List<object> izinTurleri = new List<object>();
            if (TNS.IKY.Arac.izinTipListesi == null)
                TNS.IKY.Arac.izinTipListesi = servisIKY.IzinTurListele(new IzinTur());
            if (TNS.IKY.Arac.izinTipListesi != null && TNS.IKY.Arac.izinTipListesi.objeler != null)
                foreach (IzinTur item in TNS.IKY.Arac.izinTipListesi.objeler)
                    izinTurleri.Add(new { kod = item.kod, ad = item.ad, sure = item.sure });

            bool manas = false;
            try { manas = TNS.DEG.Arac.ManasKullaniyor(); } catch (Exception) { }

            // Eski TelefonDoldur
            List<object> telefonlar = new List<object>();
            string telefon = "";
            string izinYeri = "";
            try
            {
                PersonelIletisim p = servisIKY.PersonelAdresListele(kullanan, personelKod);
                if (p != null)
                {
                    telefonlar.Add(new { TIP = TelefonTipAdGetir((int)ENUMPERSONELTEL.CEP), NUMARA = p.cepTelefon ?? "" });
                    telefonlar.Add(new { TIP = TelefonTipAdGetir((int)ENUMPERSONELTEL.EV), NUMARA = p.evTelefon ?? "" });
                    telefonlar.Add(new { TIP = TelefonTipAdGetir((int)ENUMPERSONELTEL.IS), NUMARA = p.isTelefon ?? "" });
                    telefon = p.cepTelefon ?? "";
                    if (manas && !string.IsNullOrWhiteSpace(p.adres))
                        izinYeri = p.adres;
                }
            }
            catch (Exception)
            {
            }

            int donemYil = IzinArac.DonemYiliBul(DateTime.Now.Year, DateTime.Now.Month);
            return Results.Json(new
            {
                izinTurleri = izinTurleri,
                telefonlar = telefonlar,
                telefon = telefon,
                izinYeri = izinYeri,
                manas = manas,
                donemYil = donemYil,
                donemAd = IzinArac.DonemAdBul(donemYil),
                yilMin = 1980,
                yilMaks = DateTime.Now.Year
            });
        }

        /// <summary>Eski TelefonTipAdGetir (FRMIZN028-030).</summary>
        private static string TelefonTipAdGetir(int kod)
        {
            if (kod == (int)ENUMPERSONELTEL.CEP) return "Cep";
            else if (kod == (int)ENUMPERSONELTEL.IS) return "İş";
            else if (kod == (int)ENUMPERSONELTEL.EV) return "Ev";
            return "";
        }

        /// <summary>Eski Listele(donemYil) + IzinSureBilgiHesapla(donemYil).</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int personelKod, int yil)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            string donemAd = IzinArac.DonemAdBul(yil);
            if (personelKod <= 0)
                return Results.Json(new { satirlar = new List<object>(), izinSureler = new List<object>(), donemAd = donemAd, sureBaslik = string.Format("{0} İzin Süre Bilgileri", donemAd) });

            IIKYServis servisIKY = Servis();
            TNS.IKY.PersonelIzin kriter = new TNS.IKY.PersonelIzin();
            kriter.personelKod = personelKod;
            kriter.donemYil = yil;

            ObjectArray pler = servisIKY.PersonelIzinListele(kullanan, kriter);
            if (!pler.sonuc.islemSonuc)
                return Results.Json(new { hata = PersonelBilgiOrtak.HataMetni(pler.sonuc.hataStr, "İzin bilgileri okunamadı."), donemAd = donemAd });

            List<object> satirlar = new List<object>();
            foreach (TNS.IKY.PersonelIzin i in pler.objeler)
            {
                satirlar.Add(new
                {
                    i.personelKod,
                    i.kod,
                    i.izinTurKod,
                    izinTurAd = i.izinTurAd,
                    baslangicTarihi = T(i.baslangicTarihi),
                    bitisTarihi = T(i.bitisTarihi),
                    i.kullanilanGun,
                    izinYeri = i.izinYeri ?? "",
                    aciklama = i.aciklama ?? "",
                    saglikKurulusu = i.saglikKurulusu ?? "",
                    kayitTarihi = T(i.kayitTarihi),
                    memuriyeti = i.memuriyeti ?? "",
                    telefon = i.telefon ?? "",
                    belgeNo = i.belgeNo ?? "",
                    belgeTarihi = T(i.belgeTarihi),
                    i.yerineGorevli,
                    yerineGorevliAd = i.yerineGorevliAd ?? "",
                    i.teklifEden,
                    teklifEdenAd = i.teklifEdenAd ?? "",
                    i.imzalayan,
                    imzalayanAd = i.imzalayanAd ?? "",
                    i.onay,
                    onayAd = i.onayAd,
                    evrEvrakNo = i.evrEvrakNo ?? "",
                    raporTarihi = T(i.raporTarihi),
                    raporNo = i.raporNo ?? "",
                    i.yatiliGecenSure,
                    yerKod = i.yerKod ?? "",
                    yerAd = i.yerAd ?? "",
                    izinNotu2 = i.izinNotu2 ?? ""
                });
            }

            // Eski IzinSureBilgiHesapla
            IzinBilgi iBilgi = servisIKY.PersonelKullanilanIzinHesapla(kullanan, personelKod, yil, 0);
            List<object> liste = new List<object>();
            liste.Add(new { TURKOD = 1, TUR = TNS.IKY.PersonelIzin.IzinTipAdGetir(1), TOPLAM = iBilgi.toplam, KULLANDIGI = iBilgi.devirdenKullanilan, AKTIFKULLANDIGI = iBilgi.yildanKullanilan, KALAN = iBilgi.kalan, DEVREDEN = iBilgi.devreden, YILICI = iBilgi.yilIci });

            TNS.IKY.PersonelIzinDurum d = new TNS.IKY.PersonelIzinDurum();
            d.personelKod = personelKod;
            d.listelemeYili = yil;
            ObjectArray dler = servisIKY.PersonelListeleIzinDurum(kullanan, d);
            if (dler != null && dler.objeler != null)
            {
                foreach (PersonelIzinDurum iDurum in dler.objeler)
                {
                    if (iDurum.tip != 1)
                    {
                        if (iDurum.toplamGun == 0)
                            liste.Add(new { TURKOD = iDurum.tip, TUR = iDurum.ad, TOPLAM = 0, KULLANDIGI = 0, AKTIFKULLANDIGI = iDurum.kullanilanGun, KALAN = 0, DEVREDEN = 0, YILICI = 0 });
                        else
                            liste.Add(new { TURKOD = iDurum.tip, TUR = iDurum.ad, TOPLAM = iDurum.toplamGun, KULLANDIGI = 0, AKTIFKULLANDIGI = iDurum.kullanilanGun, KALAN = iDurum.toplamGun - iDurum.kullanilanGun, DEVREDEN = 0, YILICI = 0 });
                    }
                }
            }

            return Results.Json(new
            {
                satirlar = satirlar,
                izinSureler = liste,
                donemAd = donemAd,
                sureBaslik = string.Format("{0} İzin Süre Bilgileri", donemAd), // eski FRMIZN054
                toplamIzin = iBilgi.toplam,
                toplamKullanilanIzin = iBilgi.devirdenKullanilan,
                kalanIzin = iBilgi.kalan
            });
        }

        /// <summary>Eski GunAl: başlangıç ve dönüş tarihi doluysa IzinArac.IzinHesapla.</summary>
        private static IResult Gun(HttpContext baglam, IWebHostEnvironment ortam, int izinTurKod, string baslangic, string bitis)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out _);
            if (ret != null) return ret;

            TNSDateTime baslangicTarihi = PersonelBilgiOrtak.TnsTarih(baslangic);
            TNSDateTime bitisTarihi = PersonelBilgiOrtak.TnsTarih(bitis);
            if (baslangicTarihi.Yil != 0 && bitisTarihi.Yil != 0)
                return Results.Json(new { gun = TNS.IKY.IzinArac.IzinHesapla(izinTurKod, baslangicTarihi.Oku(), bitisTarihi.Oku()).ToString() });
            return Results.Json(new { gun = (string)null });
        }

        // ------------------------------------------------------------------ kaydet / sil

        /// <summary>Eski btnKaydet_Click.</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, PersonelIzinIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            TNS.IKY.PersonelIzin d = new TNS.IKY.PersonelIzin();
            d.personelKod = istek.personelKod;
            d.tcKimlikNo = istek.tcKimlikNo ?? "";
            d.kod = istek.kod;
            d.izinTurKod = istek.izinTurKod;
            d.baslangicTarihi = PersonelBilgiOrtak.TnsTarih(istek.baslangicTarihi);
            d.bitisTarihi = PersonelBilgiOrtak.TnsTarih(istek.bitisTarihi);
            d.izinYeri = istek.izinYeri ?? "";
            d.aciklama = istek.aciklama ?? "";
            d.telefon = istek.telefon ?? "";
            d.belgeNo = istek.belgeNo ?? "";
            d.belgeTarihi = PersonelBilgiOrtak.TnsTarih(istek.belgeTarihi);
            d.memuriyeti = istek.memuriyeti ?? "";
            d.saglikKurulusu = istek.saglikKurulusu ?? "";

            d.raporTarihi = PersonelBilgiOrtak.TnsTarih(istek.raporTarihi);
            d.raporNo = istek.raporNo ?? "";
            d.yatiliGecenSure = istek.yatiliGecenSure;
            d.yerKod = istek.yerKod ?? "";
            d.imzalayan = istek.imzalayan;
            d.teklifEden = istek.teklifEden;
            d.yerineGorevli = istek.yerineGorevli;
            d.izinNotu2 = istek.izinNotu2 ?? "";

            Sonuc sonuc = Servis().PersonelIzinKaydet(kullanan, d);
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "İzin bilgisi kaydedilemedi.") });
            return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti.", kod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, 0) }); // FRMPDL017
        }

        /// <summary>Eski btnSil_Click.</summary>
        private static async Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, PersonelIzinSilIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            if (istek.personelKod <= 0 || istek.kod <= 0)
                return Results.Json(new { basarili = false, hata = "Lütfen silmek istediğiniz İzin bilgisini seçin." }); // FRMIZN020

            TNS.IKY.PersonelIzin d = new TNS.IKY.PersonelIzin();
            d.personelKod = istek.personelKod;
            d.kod = istek.kod;
            Sonuc sonuc = Servis().PersonelIzinSil(kullanan, d);
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "İzin bilgisi silinemedi.") });
            return Results.Json(new { basarili = true, bilgi = "Silme işlemi başarıyla gerçekleşti." }); // FRMPDL018
        }

        // ------------------------------------------------------------------ yazdır

        /// <summary>
        /// Eski btnYazdir_Click'in ilk bölümü: seçim / Uygun Gören denetimi, PersonelIzinYetkiKontrol(YAZDIR) ve
        /// PersonelIzinImzaGuncelle. Başarılıysa sayfa /yazdir ile formu indirir.
        /// </summary>
        private static async Task<IResult> Imza(HttpContext baglam, IWebHostEnvironment ortam, PersonelIzinImzaIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            string hata = "";
            if (istek.kod == 0)
                hata = "Lütfen izin seçiniz.\n";
            if (string.IsNullOrWhiteSpace(istek.imzalayanAd))
                hata += "Uygun gören seçilmek zorundadır.\n";
            if (hata != "") return Results.Json(new { basarili = false, hata = hata.Trim() });

            IIKYServis servisIKY = Servis();
            Sonuc sonuc = servisIKY.PersonelIzinYetkiKontrol(kullanan, new TNS.IKY.PersonelIzin { personelKod = istek.personelKod, kod = istek.kod }, ENUMTARIHCEISLEMTIP.YAZDIR);
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Yazdırma yetkiniz yok.") });

            sonuc = servisIKY.PersonelIzinImzaGuncelle(kullanan, new TNS.IKY.PersonelIzin { kod = istek.kod, teklifEden = istek.teklifEden, imzalayan = istek.imzalayan });
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = "Uygun gören ve teklif eden alanları güncellenemediği için yazdıramazsınız." });

            return Results.Json(new { basarili = true });
        }

        /// <summary>
        /// Eski btnYazdir_Click'in rapor bölümü (aynen): izin türüne göre IKY\PersonelYillikIzin / PersonelMazeretIzin /
        /// PersonelSihhiIzin.docx şablonu Aspose.Words ile doldurulur, PDF olarak indirilir, tarihçeye YAZDIR yazılır.
        /// yil: sayfadaki dönem yılı (eski hdnToplamIzin bu yılın IzinSureBilgiHesapla sonucuydu).
        /// </summary>
        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, int personelKod, int kod, int yil)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            if (kod == 0) return Results.Json(new { hata = "Lütfen izin seçiniz." });

            try
            {
                IIKYServis servisIKY = Servis();
                Sonuc sonuc = servisIKY.PersonelIzinYetkiKontrol(kullanan, new TNS.IKY.PersonelIzin { personelKod = personelKod, kod = kod }, ENUMTARIHCEISLEMTIP.YAZDIR);
                if (!sonuc.islemSonuc)
                    return Results.Json(new { hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Yazdırma yetkiniz yok.") });

                TNS.IKY.PersonelIzin izin = new TNS.IKY.PersonelIzin();
                izin.kod = kod;
                izin.personelKod = personelKod;
                ObjectArray pler = servisIKY.PersonelIzinListele(kullanan, izin);
                if (pler.ObjeSayisi > 0)
                    izin = (TNS.IKY.PersonelIzin)pler.objeler[0];
                else
                    return Results.Json(new { hata = "İzin bilgisi bulunamadı." });

                if (izin.imzalayan <= 0)
                    return Results.Json(new { hata = "Uygun gören seçilmek zorundadır." });

                string raporAd = "";
                if (izin.izinTurKod == 1)
                    raporAd = "PersonelYillikIzin";
                else if (izin.izinTurKod == 2)
                    raporAd = "PersonelMazeretIzin";
                else if (izin.izinTurKod == 3)
                    raporAd = "PersonelSihhiIzin";
                else
                    return Results.Json(new { hata = "İzin şablon bilgisi bulunamadı." });

                string dosyaAd = SablonBul(ortam.ContentRootPath, raporAd + ".docx");
                if (dosyaAd == null) return Results.Json(new { hata = "Rapor şablonu bulunamadı (IKY\\" + raporAd + ".docx)." });
                string sonucDosyaad = Path.Combine(TNS.OrtakFonksiyonlar.GeciciYolBul(), Path.GetRandomFileName()) + ".pdf";

                AsposeLisans.Yukle();
                Aspose.Words.Document doc = new Aspose.Words.Document(dosyaAd);
                Aspose.Words.DocumentBuilder docb = new Aspose.Words.DocumentBuilder(doc);

                int toplamIzin = servisIKY.PersonelKullanilanIzinHesapla(kullanan, personelKod, yil, 0).toplam; // eski hdnToplamIzin
                int yilIci = 0; // OrtakFonksiyonlar.ConvertToInt(hdnYilIci.Value, 0);
                int devir = toplamIzin - yilIci;
                int onceKullanilanGun = servisIKY.PersonelKendindenKucukKullanilanIzinHesapla(izin.personelKod, izin.baslangicTarihi);
                int kalan = devir - (izin.kullanilanGun + onceKullanilanGun);

                if (kalan <= 0)
                {
                    yilIci = yilIci + kalan;
                    kalan = 0;
                }
                int toplam = yilIci + kalan;

                GenelIslemlerIstemci.AsposeAlanaYaz(docb, "ad", izin.personelAd);
                GenelIslemlerIstemci.AsposeAlanaYaz(docb, "gorevUnvan", izin.memuriyeti);
                GenelIslemlerIstemci.AsposeAlanaYaz(docb, "birim", izin.personelBirim);
                GenelIslemlerIstemci.AsposeAlanaYaz(docb, "telefon", izin.telefon);
                GenelIslemlerIstemci.AsposeAlanaYaz(docb, "sicilNo", izin.personelSicilNo);
                GenelIslemlerIstemci.AsposeAlanaYaz(docb, "devir", devir.ToString());
                GenelIslemlerIstemci.AsposeAlanaYaz(docb, "izinSuresi", System.DateTime.Now.Year - 1 + " Yılından " + kalan + " gün. " + System.DateTime.Now.Year + " Yılından " + yilIci + " gün. " + " Toplam : " + toplam + " gün.");
                GenelIslemlerIstemci.AsposeAlanaYaz(docb, "saglikKurulusu", izin.saglikKurulusu);
                GenelIslemlerIstemci.AsposeAlanaYaz(docb, "neden", izin.aciklama);
                GenelIslemlerIstemci.AsposeAlanaYaz(docb, "gun", izin.kullanilanGun.ToString());
                GenelIslemlerIstemci.AsposeAlanaYaz(docb, "adres", izin.izinYeri);
                // Eski TNSDateTime.ToString() (ToShortDateString, tr-TR sunucuda gg.aa.yyyy); sunucu kültüründen bağımsız olsun diye açıkça.
                GenelIslemlerIstemci.AsposeAlanaYaz(docb, "baslangicT", KisaTarih(izin.baslangicTarihi));
                GenelIslemlerIstemci.AsposeAlanaYaz(docb, "donusT", KisaTarih(izin.bitisTarihi));
                GenelIslemlerIstemci.AsposeAlanaYaz(docb, "vekaletEden", izin.yerineGorevliAd);
                GenelIslemlerIstemci.AsposeAlanaYaz(docb, "gununT", ("..../ " + System.DateTime.Now.Month + " / " + System.DateTime.Now.Year).ToString());
                GenelIslemlerIstemci.AsposeAlanaYaz(docb, "imzaAtan", izin.imzalayanAd + "\n" + izin.imzalayanUnvan);
                if (!string.IsNullOrEmpty(izin.teklifEdenAd))
                {
                    GenelIslemlerIstemci.AsposeAlanaYaz(docb, "teklifEden", izin.teklifEdenAd + "\n" + izin.teklifEdenUnvan);
                    GenelIslemlerIstemci.AsposeAlanaYaz(docb, "tUnvan", izin.teklifEdenUnvan);
                    GenelIslemlerIstemci.AsposeAlanaYaz(docb, "uygun", "Uygun görüşle arz ederim.");
                    GenelIslemlerIstemci.AsposeAlanaYaz(docb, "gununT1", ("..../ " + System.DateTime.Now.Month + " / " + System.DateTime.Now.Year).ToString());
                }
                else
                {
                    GenelIslemlerIstemci.AsposeAlanaYaz(docb, "teklifEden", "");
                    GenelIslemlerIstemci.AsposeAlanaYaz(docb, "uygun", "");
                    GenelIslemlerIstemci.AsposeAlanaYaz(docb, "gununT1", "");
                }

                string ekler = "";
                if (izin.izinTurKod == 6 || izin.izinTurKod == 15)
                {
                    PersonelBelgeler belge = new PersonelBelgeler();
                    belge.pKod = personelKod;
                    belge.tip = (int)ENUMBELGEFORMTIP.PERSONELIZIN;
                    belge.tipKayitKod = kod;
                    ObjectArray belgeler = servisIKY.PersonelBelgeListele(kullanan, belge);
                    int cnt = 1;
                    foreach (PersonelBelgeler bel in belgeler.objeler)
                    {
                        ekler += "Ek" + cnt + ": " + bel.belgeAdi;
                        if (cnt != belgeler.ObjeSayisi)
                            ekler += "\n";
                        cnt++;
                    }
                }
                GenelIslemlerIstemci.AsposeAlanaYaz(docb, "ekler", ekler);
                doc.Save(sonucDosyaad, Aspose.Words.SaveFormat.Pdf);
                servisIKY.PersonelTarihceKaydet(kullanan, new TNS.IKY.PersonelIzin(), izin, ENUMTARIHCE.IZINHAREKET, personelKod, ENUMTARIHCEISLEMTIP.YAZDIR);
                return DosyaIslem.DosyaGonder(sonucDosyaad, raporAd + "_" + DateTime.Now.Ticks.ToString(), true, "pdf");
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Rapor hazırlanamadı." });
            }
        }

        private static string KisaTarih(TNSDateTime t)
        {
            if (t == null || TNSDateTime.IsNull(t)) return "";
            return t.Oku().ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Şablon önce kurumun rapor şablonu klasöründe (RaporSablonYol\IKY; eski this.raporSablonYol + "\IKY\") aranır, yoksa
        /// uygulamayla gelen SGB.Net\RaporSablon\IKY klasöründen alınır.
        /// </summary>
        private static string SablonBul(string uygulamaKlasoru, string sablonAd)
        {
            string kurumYol = Ayar.Oku("RaporSablonYol");
            if (kurumYol != "")
            {
                string s = Path.Combine(kurumYol, "IKY", sablonAd);
                if (File.Exists(s)) return s;
            }
            string u = Path.Combine(uygulamaKlasoru, "RaporSablon", "IKY", sablonAd);
            return File.Exists(u) ? u : null;
        }

        // ------------------------------------------------------------------ belgeler

        private static IResult BelgeListe(HttpContext baglam, IWebHostEnvironment ortam, int personelKod, int kayitKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            return PersonelIzinGorevOrtak.BelgeListesi(kullanan, personelKod, ENUMBELGEFORMTIP.PERSONELIZIN, kayitKod);
        }

        private static IResult BelgeGoster(HttpContext baglam, IWebHostEnvironment ortam, int personelKod, int kayitKod, int kod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            return PersonelIzinGorevOrtak.BelgeGoster(kullanan, personelKod, ENUMBELGEFORMTIP.PERSONELIZIN, kayitKod, kod);
        }

        private static async Task<IResult> BelgeYukle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            return await PersonelIzinGorevOrtak.BelgeYukle(baglam, kullanan, ENUMBELGEFORMTIP.PERSONELIZIN);
        }

        private static async Task<IResult> BelgeSil(HttpContext baglam, IWebHostEnvironment ortam, PersonelIzinBelgeSilIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            return PersonelIzinGorevOrtak.BelgeSil(kullanan, istek.personelKod, ENUMBELGEFORMTIP.PERSONELIZIN, istek.kayitKod, istek.kod);
        }

        // Personel seçimi ortak bileşende: PersonelSecimUclari (/api/personelsec/*), js/personelsec.js.

        /// <summary>Yer seçimi (eski KodAgac.aspx?kodTur=20).</summary>
        private static IResult Yerler(HttpContext baglam, IWebHostEnvironment ortam, string ust, int? seviye)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            return PersonelIzinGorevOrtak.YerAgaci(kullanan, ust, seviye ?? 1);
        }
    }
}
