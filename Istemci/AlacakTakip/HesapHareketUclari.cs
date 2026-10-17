using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.ATM;
using TNS.KYM;

namespace Istemci.AlacakTakip
{
    /// <summary>Üst griddeki bir hesap satırı (eski storeData alanları; adlar eski sayfadaki gibi).</summary>
    public class HesapHareketSatiri
    {
        public int KOD { get; set; }
        public int ANAKOD { get; set; }
        public int SIRANO { get; set; }
        public string HESAPKODU { get; set; }
        public string ADI { get; set; }
        /// <summary>Girilmemişse null (eski kodda TUTAR == "").</summary>
        public double? TUTAR { get; set; }
        /// <summary>0 borç, 1 alacak; bilinmiyorsa null (eski kodda BORCALACAK == "").</summary>
        public int? BORCALACAK { get; set; }
    }

    public class HesapHareketKaydetIstegi
    {
        public int yil { get; set; }
        public int dosyaNo { get; set; }
        /// <summary>Listeden seçilmiş kaydın kodu (eski hdnKod); yeni kayıtta 0.</summary>
        public int kod { get; set; }
        /// <summary>Seçilen işlemin (tasarımın) kodu (eski hdnTasarimKod).</summary>
        public int tasarimKod { get; set; }
        /// <summary>Üst griddeki bütün satırlar (eski getRowsValues({dirtyRowsOnly:false})).</summary>
        public List<HesapHareketSatiri> satirlar { get; set; }
    }

    public class HesapHareketSilIstegi
    {
        public int kod { get; set; }
    }

    /// <summary>
    /// Dosya Hesap Hareket Formu (eski AlacakTakip/HesapHareket.aspx) uçları. Sayfa Takip İşlemleri (AlacakTakipListe) ekranından
    /// Sgb.pencere('../AlacakTakip/HesapHareket?yil=..&amp;dosyaNo=..') ile açılır; menüde yoktur, yetkisi üst sayfalardan gelir.
    ///   GET  /api/alacaktakip/hesaphareket/tasarimlar               aktif işlem tasarımları (eski TasarimDoldur, İşlem Seçimi penceresi)
    ///   GET  /api/alacaktakip/hesaphareket/liste?yil=&amp;dosyaNo=  dosyanın kayıtlı hareketleri (eski ListeleAna)
    ///   GET  /api/alacaktakip/hesaphareket/satirlar?kod=            seçilen hareketin elle girilen satırları (eski SatirSecildi / ListeleCocuk)
    ///   GET  /api/alacaktakip/hesaphareket/tasarim?tasarimKod=      seçilen işlemin formülsüz satırları (eski btnSec_Click / ListeleTasarim)
    ///   POST /api/alacaktakip/hesaphareket/kaydet                   ana + satırlar; formül satırları Calculator ile üretilir (eski btnKaydet_Click)
    ///   POST /api/alacaktakip/hesaphareket/sil                      seçilen hareketi siler (eski btnSil_Click)
    /// </summary>
    public static class HesapHareketUclari
    {
        public const string SayfaAdresi = "../AlacakTakip/HesapHareket.aspx";

        public static readonly string[] UstSayfalar =
        {
            "../AlacakTakip/AlacakTakipListe.aspx",
            "../AlacakTakip/AlacakTakipListeAB.aspx"
        };

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/alacaktakip/hesaphareket/tasarimlar", Tasarimlar);
            app.MapGet("/api/alacaktakip/hesaphareket/liste", Liste);
            app.MapGet("/api/alacaktakip/hesaphareket/satirlar", Satirlar);
            app.MapGet("/api/alacaktakip/hesaphareket/tasarim", Tasarim);
            app.MapPost("/api/alacaktakip/hesaphareket/kaydet", Kaydet);
            app.MapPost("/api/alacaktakip/hesaphareket/sil", Sil);
        }

        /// <summary>Menü yetkisi üst sayfadan + eski Page_Load'daki TNS.ATM.Yetki.SayfayaGirisYapabilirMi.</summary>
        public static bool Yetkili(Kullanici kullanan, string klasor)
        {
            if (!SayfaYetki.Yetkili(kullanan, klasor, SayfaAdresi, UstSayfalar)) return false;
            try
            {
                return TNS.ATM.Yetki.SayfayaGirisYapabilirMi(kullanan);
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
            if (!Yetkili(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Replace("<br>", " ").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        private static string TarihYaz(TNSDateTime t)
        {
            try
            {
                if (t == null || t.isNull) return "";
                DateTime d = t.Oku();
                return d <= DateTime.MinValue.AddDays(1) ? "" : d.ToString("yyyy-MM-dd");
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static bool Basarili(ObjectArray liste)
        {
            return liste != null && liste.sonuc != null && liste.sonuc.islemSonuc && liste.objeler != null;
        }

        // ---------------------------------------------------------------- okuma uçları

        /// <summary>Eski TasarimDoldur: aktif tasarımlar (İşlem Seçimi açılır listesi).</summary>
        private static IResult Tasarimlar(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                DosyaHesapHareketTasarimAna tasarimAna = new DosyaHesapHareketTasarimAna();
                tasarimAna.aktif = 1;
                ObjectArray liste = TNS.ATM.Arac.Tanimla().DosyaHesapHareketTasarimAnaListele(kullanan, tasarimAna);
                if (!Basarili(liste))
                    return Results.Json(new { hata = HataMetni(liste?.sonuc?.hataStr, "İşlemler okunamadı.") });
                List<object> satirlar = new List<object>();
                foreach (DosyaHesapHareketTasarimAna t in liste.objeler)
                    satirlar.Add(new { KOD = t.kod, ADI = t.ad ?? "" });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "İşlemler okunamadı." });
            }
        }

        /// <summary>Eski ListeleAna: dosyanın kayıtlı hesap hareketleri (alt grid).</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int? yil, int? dosyaNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if ((dosyaNo ?? 0) == 0) return Results.Json(new { satirlar = new List<object>() });
            try
            {
                DosyaHesapHareketAna kriter = new DosyaHesapHareketAna();
                kriter.kod = 0;
                kriter.dosyaNo = dosyaNo ?? 0;
                kriter.yil = yil ?? 0;
                ObjectArray bilgi = TNS.ATM.Arac.Tanimla().DosyaHesapHareketAnaListele(kullanan, kriter);
                if (!Basarili(bilgi))
                    return Results.Json(new { hata = HataMetni(bilgi?.sonuc?.hataStr, "Kayıtlı hareketler okunamadı.") });
                List<object> satirlar = new List<object>();
                foreach (DosyaHesapHareketAna hesap in bilgi.objeler)
                {
                    satirlar.Add(new
                    {
                        KOD = hesap.kod,
                        YIL = hesap.yil,
                        DOSYANO = hesap.dosyaNo.ToString("0000"),
                        TARIH = TarihYaz(hesap.tarih),
                        ODEMEEMRINO = hesap.odemeEmriNo ?? "",
                        TOPLAMTUTAR = hesap.toplamTutar,
                        ISLEMYAPANAD = hesap.islemYapanAd ?? "",
                        ISLEMYAPANKOD = hesap.islemYapanKod ?? "",
                        ISLEMTARIHI = TarihYaz(hesap.islemTarihi),
                        TASARIMADI = hesap.tasarimAdi ?? "",
                        TASARIMKODU = hesap.tasarimKod
                    });
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Kayıtlı hareketler okunamadı." });
            }
        }

        /// <summary>Eski SatirSecildi / ListeleCocuk: seçilen hareketin satırları; formülle üretilen satırlar (uretilenSatir &gt; 0) gelmez.</summary>
        private static IResult Satirlar(HttpContext baglam, IWebHostEnvironment ortam, int? kod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if ((kod ?? 0) <= 0) return Results.Json(new { satirlar = new List<object>() });
            try
            {
                DosyaHesapHareketCocuk kriter = new DosyaHesapHareketCocuk();
                kriter.anaKod = kod ?? 0;
                ObjectArray bilgi = TNS.ATM.Arac.Tanimla().DosyaHesapHareketCocukListele(kullanan, kriter);
                if (!Basarili(bilgi))
                    return Results.Json(new { hata = HataMetni(bilgi?.sonuc?.hataStr, "Hareket satırları okunamadı.") });
                List<object> satirlar = new List<object>();
                foreach (DosyaHesapHareketCocuk hesap in bilgi.objeler)
                {
                    if (hesap.uretilenSatir > 0) continue;

                    double tutar = 0.0;
                    int borcAlacak = 0;
                    if (hesap.borc > 0)
                    {
                        tutar = hesap.borc;
                    }
                    else
                    {
                        tutar = hesap.alacak;
                        borcAlacak = 1;
                    }

                    satirlar.Add(new
                    {
                        KOD = hesap.kod,
                        ANAKOD = hesap.anaKod,
                        SIRANO = hesap.siraNo,
                        HESAPKODU = hesap.hesapKodu ?? "",
                        ADI = hesap.adi ?? "",
                        BORC = hesap.borc,
                        ALACAK = hesap.alacak,
                        TUTAR = tutar,
                        ISLEMYAPANAD = hesap.islemYapanAd ?? "",
                        ISLEMYAPANKOD = hesap.islemYapanKod ?? "",
                        ISLEMTARIHI = TarihYaz(hesap.islemTarihi),
                        BORCALACAK = borcAlacak
                    });
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Hareket satırları okunamadı." });
            }
        }

        /// <summary>Eski btnSec_Click / ListeleTasarim(false): seçilen işlemin tasarımı; formül satırları listede gösterilmez.</summary>
        private static IResult Tasarim(HttpContext baglam, IWebHostEnvironment ortam, int? tasarimKod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if ((tasarimKod ?? 0) == 0) return Results.Json(new { hata = "Lütfen ilk önce, İşlem seçimi yapınız" });
            try
            {
                DosyaHesapHareketTasarim kriter = new DosyaHesapHareketTasarim();
                kriter.anaKod = tasarimKod ?? 0;
                ObjectArray bilgi = TNS.ATM.Arac.Tanimla().DosyaHesapHareketTasarimListele(kullanan, kriter);
                if (!Basarili(bilgi))
                    return Results.Json(new { hata = HataMetni(bilgi?.sonuc?.hataStr, "İşlem tasarımı okunamadı.") });
                List<object> satirlar = new List<object>();
                foreach (DosyaHesapHareketTasarim t in bilgi.objeler)
                {
                    if (!string.IsNullOrEmpty(t.formul)) continue; // Formül alanlarını listede gösterme

                    satirlar.Add(new
                    {
                        KOD = 0,
                        ANAKOD = 0,
                        SIRANO = t.siraNo,
                        HESAPKODU = t.hesapKodu ?? "",
                        ADI = t.adi ?? "",
                        TUTAR = 0,
                        BORCALACAK = t.borcAlacak
                    });
                }
                return Results.Json(new { satirlar = satirlar, bilgi = bilgi.objeler.Count == 0 ? "Kayıt bulunmamaktadır." : "" });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "İşlem tasarımı okunamadı." });
            }
        }

        // ---------------------------------------------------------------- Kaydet / Sil

        /// <summary>Eski btnKaydet_Click.</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, HesapHareketKaydetIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();

            if (istek.dosyaNo <= 0)
                return Results.Json(new { basarili = false, hata = "Lütfen hesap hareketi girilecek dosyayı seçiniz." });

            List<HesapHareketSatiri> ja = istek.satirlar ?? new List<HesapHareketSatiri>();
            ja.RemoveAll(s => s == null);
            if (ja.Count == 0)
                return Results.Json(new { basarili = false, hata = "Kaydedilecek satır bulunmamaktadır. Yeni Kayıt ile işlem seçiniz." });

            try
            {
                IATMServis servisATM = TNS.ATM.Arac.Tanimla();

                double borcAlacak = 0.0;
                double borc = 0.0;
                double alacak = 0.0;
                DosyaHesapHareketAna hesap = new DosyaHesapHareketAna();

                foreach (HesapHareketSatiri item in ja)
                {
                    if (item.TUTAR.HasValue)
                    {
                        if (item.BORCALACAK == 0)
                            borc += item.TUTAR.Value;
                        if (item.BORCALACAK == 1)
                            alacak += item.TUTAR.Value;
                    }
                }
                borcAlacak = borc > alacak ? borc : alacak;

                borcAlacak = Math.Round(borcAlacak, 2);
                int tasarimKod = istek.tasarimKod;

                hesap.kod = istek.kod;
                hesap.yil = istek.yil;
                hesap.dosyaNo = istek.dosyaNo;
                hesap.tarih = new TNSDateTime(DateTime.Now);
                hesap.toplamTutar = borcAlacak;
                hesap.islemYapanAd = kullanan.ADSOYAD;
                hesap.islemYapanKod = kullanan.TCKIMLIKNO;
                hesap.islemTarihi = new TNSDateTime(DateTime.Now);
                hesap.tasarimKod = tasarimKod;

                Sonuc sonuc = servisATM.DosyaHesapHareketAnaKaydet(kullanan, hesap);

                if (sonuc == null || !sonuc.islemSonuc)
                    return Results.Json(new { basarili = false, hata = HataMetni(sonuc?.hataStr, "Kayıt işlemi başarısız.") });

                string anahtar = Convert.ToString(sonuc.anahtar, CultureInfo.InvariantCulture) ?? "";
                // Eski kod anaKod'u yalnızca sonuc.anahtar'dan alıyordu; anahtar boş dönerse güncellenen kaydın kodu kullanılır.
                if (!int.TryParse(anahtar, NumberStyles.Integer, CultureInfo.InvariantCulture, out int anaKod) || anaKod == 0)
                {
                    anaKod = istek.kod;
                    anahtar = anaKod.ToString(CultureInfo.InvariantCulture);
                }

                // Değişken değerlerini toplamak için
                Hashtable htIn = new Hashtable();

                // Tasarımı bul
                DosyaHesapHareketTasarim hesapHareketTasarim = new DosyaHesapHareketTasarim();
                hesapHareketTasarim.anaKod = tasarimKod;
                ObjectArray tasarimListe = servisATM.DosyaHesapHareketTasarimListele(kullanan, hesapHareketTasarim);
                if (tasarimListe == null || tasarimListe.objeler == null) tasarimListe = new ObjectArray();

                DosyaHesapHareketCocuk cocuk = null;
                ObjectArray listeCocuk = new ObjectArray();

                foreach (HesapHareketSatiri item in ja)
                {
                    cocuk = new DosyaHesapHareketCocuk();
                    cocuk.kod = item.KOD;
                    cocuk.anaKod = anaKod;
                    cocuk.siraNo = item.SIRANO;
                    cocuk.hesapKodu = item.HESAPKODU ?? "";
                    cocuk.adi = item.ADI ?? "";

                    if (item.TUTAR.HasValue)
                    {
                        if (item.BORCALACAK.HasValue)
                        {
                            if (item.BORCALACAK.Value == 0)
                                cocuk.borc = item.TUTAR.Value;
                            else
                                cocuk.alacak = item.TUTAR.Value;
                        }
                    }

                    // Eğer tasarımda değişken olarak tanımlandı ise değerini ekleyelim
                    string alanAdi = TasarimAlanAdiVer(tasarimListe, cocuk.hesapKodu, tasarimKod, cocuk.borc > 0 ? 0 : 1);
                    if (alanAdi != "")
                        htIn[alanAdi] = cocuk.borc > 0 ? cocuk.borc : cocuk.alacak;

                    cocuk.islemYapanAd = kullanan.ADSOYAD;
                    cocuk.islemYapanKod = kullanan.TCKIMLIKNO;
                    cocuk.islemTarihi = new TNSDateTime(DateTime.Now);
                    listeCocuk.objeler.Add(cocuk);
                }

                FormulSatirlariniUret(kullanan, listeCocuk, tasarimListe, htIn, anahtar);

                sonuc = servisATM.DosyaHesapHareketCocukKaydet(kullanan, listeCocuk);

                if (sonuc == null || !sonuc.islemSonuc)
                    return Results.Json(new { basarili = false, hata = HataMetni(sonuc?.hataStr, "Kayıt işlemi başarısız.") });

                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi tamamlandı" });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Kayıt işlemi başarısız." });
            }
        }

        /// <summary>
        /// Eski FormulSatirlariniUret: tasarımdaki formül satırlarını, girilen satırların değişken değerleriyle (htIn) hesaplayıp
        /// listeye "üretilen satır" olarak ekler. Hesaplanan satırın da değişken adı varsa değeri sonraki formüllere geçer.
        /// </summary>
        private static void FormulSatirlariniUret(Kullanici kullanan, ObjectArray listeCocuk, ObjectArray tasarimListe, Hashtable htIn, string anaKod)
        {
            ArrayList degiskenler = new ArrayList();
            foreach (DosyaHesapHareketTasarim t in tasarimListe.objeler)
            {
                if (!string.IsNullOrEmpty(t.alanAdi))
                {
                    Variable d = new Variable(t.alanAdi, VariableType.Money);
                    degiskenler.Add(d);
                }
            }
            Variable[] v = (Variable[])degiskenler.ToArray(typeof(Variable));

            int.TryParse(anaKod, NumberStyles.Integer, CultureInfo.InvariantCulture, out int anaKodSayi);

            int siraNo = listeCocuk.objeler.Count + 1;
            foreach (DosyaHesapHareketTasarim t in tasarimListe.objeler)
            {
                if (!string.IsNullOrEmpty(t.formul))
                {
                    DosyaHesapHareketCocuk cocuk = new DosyaHesapHareketCocuk();
                    cocuk.kod = 0;
                    cocuk.anaKod = anaKodSayi;
                    cocuk.siraNo = siraNo;
                    cocuk.hesapKodu = t.hesapKodu;
                    cocuk.adi = t.adi;
                    cocuk.uretilenSatir = 1;

                    double tutar = FormulHesapla(t.formul, v, htIn);
                    if (t.borcAlacak > 0)
                        cocuk.alacak = tutar;
                    else
                        cocuk.borc = tutar;

                    // Eğer tasarımda değişken olarak tanımlandı ise değerini ekleyelim
                    if (!string.IsNullOrEmpty(t.alanAdi))
                        htIn[t.alanAdi] = tutar;

                    cocuk.islemYapanAd = kullanan.ADSOYAD;
                    cocuk.islemYapanKod = kullanan.TCKIMLIKNO;
                    cocuk.islemTarihi = new TNSDateTime(DateTime.Now);
                    listeCocuk.objeler.Add(cocuk);

                    siraNo++;
                }
            }
        }

        /// <summary>Eski FormulHesapla: TNS.Calculator ile tek formül ("SonucBul") hesaplar; hata olursa 0.</summary>
        private static double FormulHesapla(string formul, Variable[] v, Hashtable htIn)
        {
            try
            {
                Calculator c = new Calculator();

                Formula[] f = new Formula[1];
                f[0] = new Formula("SonucBul", VariableType.Money, formul);

                string hata = c.PrepareCalculator("Hesap", "Hesapla", "", v, f);
                if (string.IsNullOrEmpty(hata))
                {
                    Hashtable htOut = new Hashtable();
                    htOut["SonucBul"] = null;
                    c.ExecuteCalc(htIn, htOut);
                    if (htOut["SonucBul"] == null)
                        return 0.0;
                    else
                        return Math.Round(Convert.ToDouble(htOut["SonucBul"], CultureInfo.InvariantCulture), 2);
                }
            }
            catch { }

            return 0.0;
        }

        private static string TasarimAlanAdiVer(ObjectArray tasarim, string hesapKod, int tasarimAnaKod, int borcAlacak)
        {
            foreach (DosyaHesapHareketTasarim t in tasarim.objeler)
            {
                if (t.anaKod == tasarimAnaKod && (t.hesapKodu ?? "").Replace(".", "") == (hesapKod ?? "").Replace(".", "") && t.borcAlacak == borcAlacak)
                    return t.alanAdi ?? "";
            }

            return "";
        }

        /// <summary>Eski btnSil_Click.</summary>
        private static async Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, HesapHareketSilIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();
            if (istek.kod <= 0) return Results.Json(new { basarili = false, hata = "Silinecek kayıt seçiniz" });
            try
            {
                DosyaHesapHareketAna dosya = new DosyaHesapHareketAna();
                dosya.kod = istek.kod;

                Sonuc sonuc = TNS.ATM.Arac.Tanimla().DosyaHesapHareketAnaSil(kullanan, dosya);
                if (sonuc == null || !sonuc.islemSonuc)
                    return Results.Json(new { basarili = false, hata = HataMetni(sonuc?.hataStr, "Silme işlemi başarısız.") });
                return Results.Json(new { basarili = true, bilgi = "İstenilen Kayıt başarıyla silindi" });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Silme işlemi başarısız." });
            }
        }
    }
}
