using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.HRC;
using TNS.KYM;
using Gib = HRCServisC.tr.gov.sgb.uygulama.gib;

namespace Istemci.HarcamaSurecleri
{
    public class BorcuYokKaydetIstegi
    {
        public int Muhasebe { get; set; }
        public string VergiNo { get; set; }
        public int TalepNedeni { get; set; }
        public string IlgiliMakam { get; set; }
        /// <summary>Ödeme emri formundan gelen birim / belge yılı / belge no (eski adres satırındaki birim, belgeYil, belgeNo).</summary>
        public string Birim { get; set; }
        public int BelgeYil { get; set; }
        public string BelgeNo { get; set; }
    }

    public class BorcuYokSorgulaIstegi
    {
        public int Kod { get; set; }
        public int Muhasebe { get; set; }
    }

    /// <summary>
    /// Borcu Yoktur Belgesi / Vergi Borcu Sorgulama Girişi (eski HarcamaSurecleri/BorcuYokBelgesi.aspx) uçları:
    ///   GET  /api/borcuyok/tanim?muhasebe=&amp;vNo=   açılış: taahhütname imzalı mı, varsayılan muhasebe, belge verilecek makam,
    ///                                              talep nedenleri; vNo verilmişse son 14 gündeki başvurusu (eski OncekiBasvuruKontrolu)
    ///   GET  /api/borcuyok/kimlik?vergiNo=         vergi / TC kimlik no ile GİB'den unvan, vergi dairesi, adres (eski imgKimlik_Click)
    ///   POST /api/borcuyok/kaydet                  dilekçeyi GİB'e gönderir ve kaydeder (eski btnKaydet_Click, "Gönder")
    ///   GET  /api/borcuyok/yazdir?kod=             borcu yoktur / borcu vardır belgesi, Word (eski BelgeYazdir; adres tip=1)
    ///   POST /api/borcuyok/sorgula                 dilekçenin GİB'deki durumunu sorgular / sıradakini gönderir (eski BelgeSorgula; tip=2)
    ///   POST /api/borcuyok/taahhut                 Vergi Borç Sorgulama Kullanıcı Taahhütnamesi kabulü (eski TanimTaahhutname.aspx?tur=1 Kabul)
    /// GİB çağrıları iş katmanındaki SOAP istemcisiyle (HRCServisC.tr.gov.sgb.uygulama.gib.GIBServis, TNS.Soap.SoapIstemci) yapılır.
    /// Sayfa menüde yoktur; yetkisi Vergi Borcu Sorgulama (BorcuYokAna.aspx) ve Ödeme Emri/MİF Belgesi menülerinden gelir.
    /// </summary>
    public static class BorcuYokBelgesiUclari
    {
        public const string SayfaAdresi = "../HarcamaSurecleri/BorcuYokBelgesi.aspx";

        public static readonly string[] UstSayfalar =
        {
            "../HarcamaSurecleri/BorcuYokAna.aspx",
            "../ButceMuhasebe/OdemeEmriMIFFrame.aspx",
            "../ButceMuhasebe/OdemeEmriMIFFrame.aspx?yeni=1",
            "../ButceMuhasebe/OdemeEmriMIFFrame.aspx?yeni=1&baslikSayfasi=1"
        };

        private const string Uc = "/api/borcuyok/";

        /// <summary>Önceki başvuru bu kadar gün içindeyse sonucu yazdırılabilir (eski kontrolGun).</summary>
        private const int KontrolGun = 14;

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "tanim", Tanim);
            app.MapGet(Uc + "kimlik", Kimlik);
            app.MapPost(Uc + "kaydet", Kaydet);
            app.MapGet(Uc + "yazdir", Yazdir);
            app.MapPost(Uc + "sorgula", Sorgula);
            app.MapPost(Uc + "taahhut", Taahhut);
        }

        // ------------------------------------------------------------------ yetki ve ortak

        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, SayfaAdresi, UstSayfalar);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IHRCServis Servis() { return TNS.HRC.Arac.Tanimla(); }

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        private static IResult Basarisiz(string hata) { return Results.Json(new { basarili = false, hata = hata ?? "" }); }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        /// <summary>Kurum kodu, noktasız (eski GenelIslemlerIstemci.VarsayilanKurumBul().Replace(".", "")).</summary>
        private static string KurumKodNoktasiz()
        {
            return (GenelIslemlerIstemci.VarsayilanKurumBul() ?? "").Replace(".", "");
        }

        /// <summary>GİB servis istemcisi (eski WebServisler...GIB.GIBServis; Url = GIBServisURL, Proxy = ProxyAyarla).</summary>
        public static Gib.GIBServis GibServis()
        {
            Gib.GIBServis servis = new Gib.GIBServis();
            string url = Ayar.Oku("GIBServisURL");
            if (url != "") servis.Url = url; // ayar yoksa iş katmanındaki varsayılan adres kalır
            servis.Proxy = OrtakFonksiyonlar.ProxyAyarla();
            return servis;
        }

        private static string DurumAd(int durum)
        {
            if (durum == (int)ENUMBorcuYokDurum.GONDERILDI || durum == (int)ENUMBorcuYokDurum.CEVAPLANMADI || durum == (int)ENUMBorcuYokDurum.SIRADA)
                return "Cevap gelmedi";
            if (durum == (int)ENUMBorcuYokDurum.CEVAPOLUMLU) return "Borcu YOK";
            if (durum == (int)ENUMBorcuYokDurum.CEVAPOLUMSUZ) return "Borcu VAR";
            if (durum == (int)ENUMBorcuYokDurum.DILEKCESORGUHATASI) return "Dilekçe Sorgu Hatası";
            return "";
        }

        /// <summary>Eski TalepNedeniDoldur.</summary>
        private static List<object> TalepNedenleri()
        {
            return new List<object>
            {
                new { KOD = 0, AD = "Seçiniz." },
                new { KOD = (int)ENUMBorcuYokTalepNedeni.IHALE, AD = "Kamu İhale Mevzuatı/ İhaleye girmek için" },
                new { KOD = (int)ENUMBorcuYokTalepNedeni.ODEMEALMAK, AD = "6183 / 22-A / Kamu kurum ve kuruluşlarından ödeme alabilmek için" },
                new { KOD = (int)ENUMBorcuYokTalepNedeni.DIGER, AD = "Diğer" }
            };
        }

        // ------------------------------------------------------------------ açılış

        /// <summary>Eski Page_Load (!IsPostBack).</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam, string muhasebe, string vNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                IHRCServis servisHRC = Servis();
                ObjectArray kontrol = servisHRC.TaahhutnameListele(kullanan.mernis, (int)ENUMTaahhutname.VERGIBORCU, new TNSDateTime());
                bool taahhut = kontrol != null && kontrol.objeler != null && kontrol.objeler.Count > 0;

                string muh = Temiz(muhasebe);
                if (muh == "") muh = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBE");
                string makam = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "VSVerilecekMakam");

                object onceki = null;
                string vergiNo = Temiz(vNo);
                if (taahhut && vergiNo != "")
                    onceki = OncekiBasvuru(servisHRC, kullanan, vergiNo, OrtakFonksiyonlar.ConvertToInt(muh, 0));

                return Results.Json(new
                {
                    taahhut = taahhut,
                    muhasebe = muh,
                    ilgiliMakam = makam,
                    nedenler = TalepNedenleri(),
                    onceki = onceki
                });
            }
            catch (Exception)
            {
                return Hata("Sayfa bilgileri okunamadı.");
            }
        }

        /// <summary>
        /// Eski OncekiBasvuruKontrolu: kişi / firmanın bu muhasebedeki son 14 gün içindeki başvurusu varsa kodu, durumu ve onay metni.
        /// </summary>
        private static object OncekiBasvuru(IHRCServis servisHRC, Kullanici kullanan, string vergiNo, int muhasebe)
        {
            TNS.HRC.BorcuYokBelgesi kriter = new TNS.HRC.BorcuYokBelgesi(0);
            kriter.vergiKimlikNo = vergiNo;
            kriter.muhasebe = muhasebe;
            ObjectArray yler = servisHRC.BorcuYokListele(kullanan, kriter);
            if (yler == null || yler.objeler == null) return null;

            int kod = 0;
            int durum = 0;
            foreach (TNS.HRC.BorcuYokBelgesi y in yler.objeler)
            {
                if (y.kayitTarihi == null || y.kayitTarihi.isNull) continue;
                if (y.kayitTarihi.Oku().AddDays(KontrolGun) > DateTime.Now)
                {
                    kod = y.kod;
                    durum = y.durum;
                }
            }
            if (kod <= 0) return null;

            string durumAd = DurumAd(durum);
            return new
            {
                kod = kod,
                durum = durum,
                durumAd = durumAd,
                metin = vergiNo + " kimlik numaralı kişi/firma için " + KontrolGun + " gün içinde borç sorgulama sonucu bulundu.\n\nDurumu: "
                        + durumAd + "\n\nBu sonucun yazdırılmasını istiyor musunuz?"
            };
        }

        /// <summary>Eski TanimTaahhutname.aspx btnKabul_Click (tur = VERGIBORCU).</summary>
        private static async Task<IResult> Taahhut(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                Sonuc sonuc = Servis().TaahhutnameKaydet(kullanan, kullanan.mernis, (int)ENUMTaahhutname.VERGIBORCU, new TNSDateTime(DateTime.Now));
                if (sonuc != null && !sonuc.islemSonuc) return Basarisiz(sonuc.hataStr);
                return Results.Json(new { basarili = true, bilgi = "Taahhütname kabul edildi." });
            }
            catch (Exception)
            {
                return Basarisiz("Taahhütname kaydedilemedi.");
            }
        }

        // ------------------------------------------------------------------ kimlik sorgulama

        /// <summary>Eski Ortak.FirmaUnvan: GİB'den firma / kişi unvanı, vergi dairesi ve adresi.</summary>
        public static void FirmaUnvan(string vergiNo, string tcKimlikNoKullanici, ref string unvan, ref string vd, ref string adres)
        {
            string tut = Ayar.Oku("GelirlerBaglantisiVarMi").Trim();
            bool gelirlerBaglantisiVar = OrtakFonksiyonlar.ConvertToInt(tut, 0) != 0;
            if (gelirlerBaglantisiVar)
            {
                string kurumKod = GenelIslemlerIstemci.VarsayilanKurumBul();
                try
                {
                    Gib.FirmaBilgisi firma = GibServis().FirmaKisiBilgisiAl(vergiNo, tcKimlikNoKullanici, kurumKod);
                    unvan = firma.ad;
                    vd = firma.vergiDairesi;
                    adres = firma.adres;
                }
                catch (Exception ex)
                {
                    OrtakFonksiyonlar.HataDosyaYaz("Hata" + DateTime.Now.ToString("yyyyMMdd") + ".htm", "<br>***<br>" + ex.Message + "<br>***", false);
                    unvan = "Gelir İdaresi Başkanlığı'ndan bilgi alınamadı.";
                }
            }
            else
                unvan = "GİB Bağlantısı yapılmamış.";
            unvan = unvan ?? "";
            vd = vd ?? "";
            adres = adres ?? "";
        }

        /// <summary>Eski KimlikSorgula: 10 / 11 haneli numarada GİB bilgisi; hata metni ve başarı.</summary>
        private static bool KimlikSorgula(string vergiNo, string tcKullanici, out string unvan, out string vd, out string adres, ref string hataStr)
        {
            bool retValue = true;
            unvan = "";
            vd = "";
            adres = "";
            if (vergiNo != "" && (vergiNo.Length == 11 || vergiNo.Length == 10))
            {
                FirmaUnvan(vergiNo, tcKullanici, ref unvan, ref vd, ref adres);
                if (unvan.Contains("java.rmi.RemoteException"))
                {
                    hataStr = "Gelir İdaresi Başkanlığı sistemindeki yoğunluk nedeniyle dilekçeniz gönderilemedi.\nLütfen daha sonra tekrar deneyiniz.";
                    retValue = false;
                }
                if (unvan.Contains("bulunamadı."))
                {
                    hataStr = "Kayıtlı Mükellef/Vergi Kimlik No bulunamadı.";
                    retValue = false;
                }
            }
            else
                retValue = false;
            return retValue;
        }

        /// <summary>Eski imgKimlik_Click (Vergi / T.C. Kimlik alanının merceği).</summary>
        private static IResult Kimlik(HttpContext baglam, IWebHostEnvironment ortam, string vergiNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                string hataStr = "";
                bool tamam = KimlikSorgula(Temiz(vergiNo), kullanan.mernis, out string unvan, out string vd, out string adres, ref hataStr);
                return Results.Json(new { basarili = tamam, hata = tamam ? "" : hataStr, unvan = unvan, vergiDaire = vd, adres = adres });
            }
            catch (Exception)
            {
                return Basarisiz("Kimlik bilgisi okunamadı.");
            }
        }

        // ------------------------------------------------------------------ gönder (kaydet)

        /// <summary>Eski btnKaydet_Click ("Gönder"): dilekçe GİB'e gönderilir; GİB kabul ederse GONDERILDI, kuyruğa alınırsa SIRADA kaydedilir.</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, BorcuYokKaydetIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new BorcuYokKaydetIstegi();
            try
            {
                IHRCServis servisHRC = Servis();
                string hata = "";
                string kurumKod = KurumKodNoktasiz();
                string vergiNo = Temiz(istek.VergiNo);
                string ilgiliMakam = Temiz(istek.IlgiliMakam);
                DateTime bitisTarihi = DateTime.Today; // eski txtBitTarih = bugün

                string unvan, vd, adres;
                if (!KimlikSorgula(vergiNo, kullanan.mernis, out unvan, out vd, out adres, ref hata))
                {
                    if (hata == "")
                        hata = "Vergi veya TC. kimlik numarası bilgisi yanlış olabilir veya\n girdiğiniz vergi numarasının vergi mükellefiyeti bulunmamaktadır.\n";
                }

                if (istek.Muhasebe == 0)
                    hata = "Muhasebe bilgisi boş bırakılmış.\n";
                if (vergiNo == "")
                    hata += "Vergi veya TC Kimlik numarası alanı boş bırakılmış.\n";
                if (ilgiliMakam == "")
                    hata += "Belge verilecek makam alanı boş bırakılmış.\n";
                if (istek.TalepNedeni == 0)
                    hata += "Borcu yok yazısı talep nedeni seçiniz.\n";

                TNS.HRC.BorcuYokBelgesi kriter = new TNS.HRC.BorcuYokBelgesi(0);
                if (vergiNo.Length == 10 || vergiNo.Length == 11)
                    kriter.vergiKimlikNo = vergiNo;
                else
                    hata = "Lütfen 11 rakamlı Tc Kimlik veya 10 rakamlı Vergi Kimlik numarası giriniz.\n";

                if (hata != "")
                    return Results.Json(new { basarili = false, hata = hata + "Kayıt işlemi yapılamadı.", unvan = unvan, vergiDaire = vd, adres = adres });

                kriter.kod = 0;
                kriter.muhasebe = istek.Muhasebe;
                kriter.kayitTarihi = new TNSDateTime(DateTime.Today);
                kriter.bitisTarihi = new TNSDateTime(bitisTarihi);
                kriter.talepNedeni = istek.TalepNedeni;
                kriter.ilgiliMakam = ilgiliMakam.ToUpper(new System.Globalization.CultureInfo("tr-TR"));
                kriter.unvan = unvan;
                kriter.adres = adres;
                kriter.vergiDaire = vd;

                kriter.birim = Temiz(istek.Birim);
                kriter.ilgiliBelgeYil = istek.BelgeYil;
                kriter.ilgiliBelgeNo = Temiz(istek.BelgeNo);

                Sonuc sonuc = new Sonuc(true, "", "");

                Gib.borcuYokturKaydetOutput dko;
                Gib.borcuYokturKaydetInput dki = new Gib.borcuYokturKaydetInput();
                dki.dilekceBitisTarihi = bitisTarihi.ToString("yyyyMMdd");
                dki.yaziTalepNedeni = istek.TalepNedeni.ToString();
                if (kriter.vergiKimlikNo.Length == 10)
                    dki.vergiKimlikNumarasi = kriter.vergiKimlikNo;
                else if (kriter.vergiKimlikNo.Length == 11)
                    dki.tcKimlikNumarasi = kriter.vergiKimlikNo;

                try
                {
                    dko = GibServis().BorcuYokDilekceKaydet(dki, kullanan.mernis, kurumKod);
                }
                catch (Exception ex)
                {
                    kriter.hataMesaji = ex.Message;
                    dko = null;
                }

                object kimlik = new { unvan = unvan, vergiDaire = vd, adres = adres };

                if (dko == null)
                    return Results.Json(new { basarili = false, hata = "Servis bağlantısı hatası: " + kriter.hataMesaji, kimlik = kimlik });
                if (dko.durum == null)
                    return Results.Json(new { basarili = false, hata = "Servis bağlantısı hatası, belge durumu dönmedi!", kimlik = kimlik });

                kriter.cevapKoduGeldi = dko.durum.sonuc;
                if (dko.durum.durumKodu == "1000")
                {
                    kriter.durum = (int)ENUMBorcuYokDurum.GONDERILDI;
                    kriter.cevapId = dko.ID;

                    sonuc = servisHRC.BorcuYokKaydet(kullanan, kriter);
                    if (sonuc.islemSonuc)
                    {
                        GenelIslemler.KullaniciDegiskenSakla(kullanan, "VSVerilecekMakam", kriter.ilgiliMakam);
                        return Results.Json(new
                        {
                            basarili = true,
                            temizle = true,
                            bilgi = string.Format("{0} numaralı dilekçeniz başarıyla Gelir İdaresi Başkanlığı'na gönderilmiştir.", kriter.cevapId),
                            cevapId = kriter.cevapId,
                            durum = kriter.durum
                        });
                    }
                    OrtakFonksiyonlar.DosyaYaz("BorcuYokBelgesi", "Muhasebe Kod:" + kriter.muhasebe + "Vergi No:" + kriter.vergiKimlikNo + "-->Cevap Id:" + kriter.cevapId
                        + "(baş Tarih:" + kriter.baslangicTarihi + ",bit Tarih:" + kriter.bitisTarihi + ",talep Nedeni:" + kriter.talepNedeni + ",kayıt Tarih:"
                        + kriter.kayitTarihi + ",verilecek Makam:" + kriter.ilgiliMakam + ") HATA NEDEDİ : " + sonuc.hataStr, false);
                    return Results.Json(new { basarili = false, hata = sonuc.hataStr, kimlik = kimlik });
                }

                if (string.IsNullOrEmpty(dko.durum.durumKodAciklamasi))
                    return Results.Json(new { basarili = false, hata = "Gelir İdaresi Başkanlığı şu anda cevap vermiyor. Lütfen daha sonra deneyiniz. Kayıt yapılamadı.", kimlik = kimlik });
                if (dko.durum.durumKodAciklamasi.Contains("bulunmamakta"))
                    return Results.Json(new { basarili = false, hata = string.Format("{0} nolu vergi/kimlik numarası ile ilgili Vergi Mükellefiyeti bulunmamaktadır.", kriter.vergiKimlikNo), kimlik = kimlik });

                kriter.durum = (int)ENUMBorcuYokDurum.SIRADA;
                kriter.cevapId = "";
                sonuc = servisHRC.BorcuYokKaydet(kullanan, kriter);
                if (sonuc.islemSonuc)
                {
                    GenelIslemler.KullaniciDegiskenSakla(kullanan, "VSVerilecekMakam", kriter.ilgiliMakam);
                    return Results.Json(new
                    {
                        basarili = true,
                        temizle = true,
                        bilgi = string.Format("{0} numaralı dilekçeniz Gelir İdaresi Başkanlığı'na gönderilmek üzere SGB.Net sistemine kaydedilmiştir.", sonuc.anahtar),
                        cevapId = "",
                        durum = kriter.durum
                    });
                }
                return Results.Json(new { basarili = false, hata = sonuc.hataStr, kimlik = kimlik });
            }
            catch (Exception)
            {
                return Basarisiz("Kayıt işlemi yapılamadı.");
            }
        }

        // ------------------------------------------------------------------ sorgula (eski tip=2)

        private static async Task<IResult> Sorgula(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, BorcuYokSorgulaIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new BorcuYokSorgulaIstegi();
            if (istek.Kod <= 0) return Basarisiz("Dilekçe seçilmedi.");
            try
            {
                string mesaj = BorcSorgu.BorcSorgula(Servis(), kullanan, istek.Kod, istek.Muhasebe);
                if (!string.IsNullOrEmpty(mesaj)) return Basarisiz(mesaj);
                return Results.Json(new { basarili = true, bilgi = "Dilekçe sorgulandı." });
            }
            catch (Exception)
            {
                return Basarisiz("Dilekçe sorgulanamadı.");
            }
        }

        // ------------------------------------------------------------------ yazdır (eski tip=1 ve BelgeYazdirTamamla)

        /// <summary>Eski BelgeYazdir(kod): olumlu cevapta borcu yoktur, olumsuz cevapta borcu vardır belgesi (Word).</summary>
        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, int? kod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            int yer = 0;
            try
            {
                if (kod > 0)
                {
                    yer = 2;
                    TNS.HRC.BorcuYokBelgesi dilekce = Servis().BorcuYokAc(kullanan, new TNS.HRC.BorcuYokBelgesi(kod.Value));
                    yer = 3;
                    if (dilekce != null)
                    {
                        yer = 4;
                        string hata;
                        string dosya;
                        string ad;
                        if (dilekce.durum == (int)ENUMBorcuYokDurum.CEVAPOLUMLU)
                        {
                            yer = 5;
                            ad = "BorcuYokBelgesi";
                            dosya = BorcuYokYazdir(ortam.ContentRootPath, dilekce, out hata);
                        }
                        else if (dilekce.durum == (int)ENUMBorcuYokDurum.CEVAPOLUMSUZ)
                        {
                            yer = 6;
                            ad = "BorcuVarBelgesi";
                            dosya = BorcuVarYazdir(ortam.ContentRootPath, kullanan, dilekce, ref yer, out hata);
                        }
                        else
                        {
                            yer = 7;
                            throw new Exception("Dilekçe durumu yazdırmaya uygun değil:" + dilekce.durum);
                        }
                        if (dosya == null) return Hata(hata);
                        return DosyaIslem.DosyaGonder(dosya, ad, true, "docx");
                    }
                    yer = 8;
                    throw new Exception("Dilekçe okunamadı!");
                }
                yer = 9;
                throw new Exception("Kod boş geldi!");
            }
            catch (Exception e)
            {
                return Hata("Rapor alınamadı (" + yer + ") :" + e.Message);
            }
        }

        /// <summary>Şablon önce RaporSablonYol\HRC\, yoksa uygulamayla gelen SGB.Net\RaporSablon\HRC\ altında aranır.</summary>
        private static string SablonBul(string uygulamaKlasoru, string sablonAd)
        {
            string kurumYol = Ayar.Oku("RaporSablonYol");
            if (kurumYol != "")
            {
                string s = Path.Combine(kurumYol, "HRC", sablonAd);
                if (File.Exists(s)) return s;
            }
            string u = Path.Combine(uygulamaKlasoru, "RaporSablon", "HRC", sablonAd);
            return File.Exists(u) ? u : null;
        }

        private static string Tarih(TNSDateTime t)
        {
            if (t == null || t.isNull) return "";
            return t.Oku().ToString("dd.MM.yyyy");
        }

        /// <summary>Eski şablon değişkenleri (ekRtf1.CreateVar) ortak alanlar.</summary>
        private static Dictionary<string, string> OrtakAlanlar(TNS.HRC.BorcuYokBelgesi belge)
        {
            return new Dictionary<string, string>
            {
                { "VERGIDAIRE", belge.vergiDaire },
                { "EVRAKNO", belge.kod.ToString() },
                { "TARIH", DateTime.Now.ToString("dd.MM.yyyy") },
                { "FIRMAADI", belge.unvan },
                { "FIRMAADRES", belge.adres },
                { "BASVURUTARIH", Tarih(belge.kayitTarihi) },
                { "CEVAPID", belge.cevapId },
                { "VERGINO", belge.vergiKimlikNo },
                { "BASTARIH", Tarih(belge.baslangicTarihi) },
                { "BITTARIH", Tarih(belge.bitisTarihi) },
                { "ILGILIMAKAM", belge.ilgiliMakam }
            };
        }

        private static string Yaz(string sablon, Dictionary<string, string> alanlar, DataTable tablo)
        {
            AsposeLisans.Yukle();
            Aspose.Words.Document doc = new Aspose.Words.Document(sablon);
            doc.MailMerge.CleanupOptions = Aspose.Words.Reporting.MailMergeCleanupOptions.RemoveUnusedRegions;
            string[] adlar = new string[alanlar.Count];
            object[] degerler = new object[alanlar.Count];
            int i = 0;
            foreach (KeyValuePair<string, string> a in alanlar)
            {
                adlar[i] = a.Key;
                degerler[i] = a.Value ?? "";
                i++;
            }
            doc.MailMerge.Execute(adlar, degerler);
            if (tablo != null) doc.MailMerge.ExecuteWithRegions(tablo);
            doc.UpdateFields();
            string sonucDosyaad = DosyaIslem.DosyaAdUret("", ".docx");
            doc.Save(sonucDosyaad, Aspose.Words.SaveFormat.Docx);
            return sonucDosyaad;
        }

        /// <summary>Eski BorcuYokYazdir (RaporSablon\HRC\BorcuYokBelgesi.rtf → HRC\BorcuYokBelgesi.dotx).</summary>
        private static string BorcuYokYazdir(string uygulamaKlasoru, TNS.HRC.BorcuYokBelgesi belge, out string hata)
        {
            hata = null;
            if (string.IsNullOrEmpty(belge.cevapId))
            {
                hata = "Yazdırılacak bilgi bulunamadı. Lütfen ilk önce bilgileri kayıt edin.";
                return null;
            }
            string sablon = SablonBul(uygulamaKlasoru, "BorcuYokBelgesi.dotx");
            if (sablon == null)
            {
                hata = "Rapor şablonu bulunamadı (HRC\\BorcuYokBelgesi.dotx).";
                return null;
            }
            return Yaz(sablon, OrtakAlanlar(belge), null);
        }

        /// <summary>
        /// Eski BorcuVarYazdir (RaporSablon\HRC\BorcuVarBelgesi.rtf → HRC\BorcuVarBelgesi.dotx): GİB'den dilekçe sorgulanır, borç
        /// detayları "Borc" bölgesine (VergiDairesi, Yil, VergiTuru, BorcTutari, Muvafakat, IkincilSorumluluk) yazılır.
        /// </summary>
        private static string BorcuVarYazdir(string uygulamaKlasoru, Kullanici kullanan, TNS.HRC.BorcuYokBelgesi belge, ref int yer, out string hata)
        {
            hata = null;
            yer = 20;
            string kurumKod = KurumKodNoktasiz();
            if (kurumKod.StartsWith("38"))
            {
                hata = "Borcu vardır belgesi bu kurumda yazdırılmaz.";
                return null;
            }

            yer = 21;
            if (string.IsNullOrEmpty(belge.cevapId))
            {
                hata = "Yazdırılacak bilgi bulunamadı. Lütfen ilk önce bilgileri kayıt edin.";
                return null;
            }

            yer = 22;
            string sablon = SablonBul(uygulamaKlasoru, "BorcuVarBelgesi.dotx");
            if (sablon == null)
            {
                hata = "Rapor şablonu bulunamadı (HRC\\BorcuVarBelgesi.dotx).";
                return null;
            }

            yer = 23;
            Dictionary<string, string> alanlar = OrtakAlanlar(belge);
            alanlar["SorumluTutulmayacakToplam"] = "";
            alanlar["SorumluTutulacakToplam"] = "";
            alanlar["KesintiVergiDairesi"] = "";
            alanlar["ACIKLAMA"] = "";

            Gib.borcuYokturSorguOutput dso;
            Gib.dilekceSorguInput dsi = new Gib.dilekceSorguInput();
            dsi.dilekceID = belge.cevapId;
            string sorguHata = "";
            try
            {
                yer = 24;
                Gib.GIBServis servis = GibServis();
                yer = 25;
                dso = servis.BorcuYokDilekceSorgulama(dsi, kullanan.mernis, kurumKod);
                yer = 26;
            }
            catch (Exception ex)
            {
                yer = 27;
                sorguHata = "Borcu Yok Dilekçesi GİB'ten Sorgulanamadı: " + ex.Message;
                dso = null;
            }

            yer = 28;
            DataTable dtUye = null;
            if (dso != null && dso.durum != null && dso.durum.durumKodu == "1000")
            {
                yer = 29;
                dtUye = new DataTable("Borc");
                dtUye.Columns.Add("VergiDairesi");
                dtUye.Columns.Add("Yil");
                dtUye.Columns.Add("VergiTuru");
                dtUye.Columns.Add("BorcTutari");
                dtUye.Columns.Add("Muvafakat");
                dtUye.Columns.Add("IkincilSorumluluk");
                if (dso.cevaplandi)
                {
                    yer = 30;
                    if (!dso.borcuYoktur) // buraya girmesi beklenmektedir.
                    {
                        yer = 31;
                        if (dso.borcDetaylari != null)
                        {
                            yer = 32;
                            foreach (Gib.vergiDairesiBorcDetayi item in dso.borcDetaylari)
                            {
                                if (item == null) break;
                                yer = 33;
                                dtUye.Rows.Add(item.vergiDairesiKodu + "- " + item.vergiDairesiAdi, item.yil, item.vergiTuru,
                                    OrtakFonksiyonlar.ConvertToDbl((item.miktar ?? "").Replace(".", ",")).ToString("#,###.00"),
                                    EvetHayirDondur(item.muvafakat), EvetHayirDondur(item.ikincilSorumluluk));
                            }
                        }
                        yer = 34;
                        if (dso.sorumluTutulmayacakToplam == null) dso.sorumluTutulmayacakToplam = "";
                        if (dso.sorumluVeyaKesintiyeTabiTutulacakToplam == null) dso.sorumluVeyaKesintiyeTabiTutulacakToplam = "";
                        alanlar["SorumluTutulmayacakToplam"] = OrtakFonksiyonlar.ConvertToDbl(dso.sorumluTutulmayacakToplam.Replace(".", ",")).ToString("#,###.00");
                        alanlar["SorumluTutulacakToplam"] = OrtakFonksiyonlar.ConvertToDbl(dso.sorumluVeyaKesintiyeTabiTutulacakToplam.Replace(".", ",")).ToString("#,###.00");
                        alanlar["KesintiVergiDairesi"] = dso.kesintininAktarilacagiVergiDairesiKodu + " - " + dso.kesintininAktarilacagiVergiDairesiAdi
                            + Environment.NewLine + dso.kesintininAktarilacagiVergiDairesiIBAN;
                        yer = 36;
                        string aciklama = "";
                        if (dso.aciklama != null)
                        {
                            string yildiz = "";
                            for (int i = 0; i < dso.aciklama.Length; i++)
                            {
                                aciklama += Environment.NewLine + Environment.NewLine;
                                yildiz += "*";
                                aciklama += yildiz;
                                aciklama += dso.aciklama[i];
                            }
                        }
                        alanlar["ACIKLAMA"] = aciklama;
                    }
                }
            }
            else
            {
                if (dso != null)
                {
                    string aciklamaStr = dso.aciklama != null ? string.Join(" ", dso.aciklama) : "";
                    if (dso.durum != null)
                        sorguHata = dso.durum.durumKodAciklamasi + " - " + aciklamaStr;
                    else
                        sorguHata = "Durum açıklaması yok: " + aciklamaStr;
                }
                alanlar["ACIKLAMA"] = sorguHata;
            }

            return Yaz(sablon, alanlar, dtUye);
        }

        public static string EvetHayirDondur(string i)
        {
            if (i == "0" || string.IsNullOrWhiteSpace(i))
                return "Hayır";
            return "Evet";
        }
    }

    /// <summary>
    /// Eski HarcamaSurecleri.BorcSorgu (BorcuYokBelgesi.aspx.cs): dilekçenin GİB'deki durumunu sorgulayıp kaydeder; sırada bekleyeni
    /// GİB'e gönderir. Belge Arama (BorcuYokBelgesiSorgu) ekranı da bunu kullanır (çoklu seçip sorgulama).
    /// Hata varsa metni, yoksa boş döner.
    /// </summary>
    public static class BorcSorgu
    {
        public static string BorcSorgula(IHRCServis servisHRC, Kullanici kullanan, int kod, int muhasebe)
        {
            string kurumKod = (GenelIslemlerIstemci.VarsayilanKurumBul() ?? "").Replace(".", "");
            if (kurumKod.StartsWith("38"))
                return "";

            if (kod > 0)
            {
                TNS.HRC.BorcuYokBelgesi dilekce = servisHRC.BorcuYokAc(kullanan, new TNS.HRC.BorcuYokBelgesi(kod));
                if (dilekce == null) return "Dilekçe okunamadı!";

                Sonuc sonuc;
                TNS.HRC.BorcuYokBelgesi dilekceUpdate = new TNS.HRC.BorcuYokBelgesi(0);
                dilekceUpdate.muhasebe = muhasebe;
                switch (dilekce.durum)
                {
                    case (int)ENUMBorcuYokDurum.CEVAPLANMADI:
                    case (int)ENUMBorcuYokDurum.DILEKCESORGUHATASI:
                    case (int)ENUMBorcuYokDurum.GONDERILDI:
                        {
                            Gib.borcuYokturSorguOutput dso = new Gib.borcuYokturSorguOutput();
                            Gib.dilekceSorguInput dsi = new Gib.dilekceSorguInput();
                            dsi.dilekceID = dilekce.cevapId;
                            dilekceUpdate.kod = dilekce.kod;
                            try
                            {
                                Gib.borcuYokturSorguOutput gelen = BorcuYokBelgesiUclari.GibServis().BorcuYokDilekceSorgulama(dsi, kullanan.mernis, kurumKod);
                                if (gelen != null) dso = gelen;
                            }
                            catch { }
                            // dilekçe sorgulama başarılı değilse, hata mesajına bakıyoruz..
                            if (dso.durum != null && dso.durum.durumKodu != "1000")
                            {
                                dilekceUpdate.durum = (int)ENUMBorcuYokDurum.DILEKCESORGUHATASI;
                                dilekceUpdate.hataMesaji = dso.durum.durumKodAciklamasi;
                            }
                            else // dilekçe sorgulama başarılı ise
                            {
                                if (!dso.cevaplandi) // dilekçe henüz cevaplanmamış ise
                                    dilekceUpdate.durum = (int)ENUMBorcuYokDurum.CEVAPLANMADI;
                                else if (dso.borcuYoktur) // borcu yok ise
                                    dilekceUpdate.durum = (int)ENUMBorcuYokDurum.CEVAPOLUMLU;
                                else // borcu var ise
                                    dilekceUpdate.durum = (int)ENUMBorcuYokDurum.CEVAPOLUMSUZ;
                            }
                            sonuc = servisHRC.BorcuYokKaydet(kullanan, dilekceUpdate);
                        }
                        break;
                    case (int)ENUMBorcuYokDurum.SIRADA:
                        {
                            Gib.borcuYokturKaydetOutput dko;
                            Gib.borcuYokturKaydetInput dki = new Gib.borcuYokturKaydetInput();
                            dki.dilekceBitisTarihi = dilekce.bitisTarihi.Oku().ToString("yyyyMMdd");
                            dki.yaziTalepNedeni = dilekce.talepNedeni.ToString();
                            if (dilekce.vergiKimlikNo.Length == 10)
                                dki.vergiKimlikNumarasi = dilekce.vergiKimlikNo;
                            else if (dilekce.vergiKimlikNo.Length == 11)
                                dki.tcKimlikNumarasi = dilekce.vergiKimlikNo;

                            try
                            {
                                dko = BorcuYokBelgesiUclari.GibServis().BorcuYokDilekceKaydet(dki, kullanan.mernis, kurumKod);
                            }
                            catch (Exception ex)
                            {
                                return ex.Message;
                            }

                            if (dko != null && dko.durum != null)
                            {
                                dilekce.cevapKoduGeldi = dko.durum.sonuc;
                                if (dko.durum.durumKodu == "1000")
                                {
                                    dilekce.durum = (int)ENUMBorcuYokDurum.GONDERILDI;
                                    dilekce.cevapId = dko.ID;
                                    sonuc = servisHRC.BorcuYokKaydet(kullanan, dilekce);
                                }
                                else
                                {
                                    return string.IsNullOrEmpty(dilekce.hataMesaji) ? "Lütfen daha sonra tekrar deneyiniz." : dilekce.hataMesaji;
                                }
                            }
                        }
                        break;
                }
            }
            return "";
        }
    }
}
