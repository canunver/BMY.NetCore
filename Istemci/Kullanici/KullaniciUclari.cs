using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.KYM;

namespace Istemci.Kullanicilar
{
    /// <summary>Kullanıcı Giriş sayfasının kaydet isteği. Kullanıcı tipleri ve yetki değerleri tarayıcıda tutulur, burada birlikte gelir.</summary>
    public class KullaniciKayitIstegi
    {
        public bool Yeni { get; set; }
        public string Kod { get; set; }
        public string Tc { get; set; }
        /// <summary>yyyy-MM-dd</summary>
        public string Dogum { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Telefon { get; set; }
        public string Eposta { get; set; }
        public string Kurum { get; set; }
        public string Unvan { get; set; }
        public string AkademikUnvan { get; set; }
        public string Birim { get; set; }
        public string SicilNo { get; set; }
        public string SertNo { get; set; }
        public string Rumuz { get; set; }
        /// <summary>Kullanıcının kullanıcı tipi kodları.</summary>
        public int[] Tipler { get; set; }
        /// <summary>Yetki tipi kodu -> değer (çoklu değerler ; ile ayrılır).</summary>
        public Dictionary<string, string> Yetkiler { get; set; }
    }

    public class KullaniciKodIstegi
    {
        public string Kod { get; set; }
    }

    public class KullaniciDurumIstegi
    {
        public string Kod { get; set; }
        public string Kurum { get; set; }
        public string Eposta { get; set; }
        /// <summary>Pasif yapılırken: yeniden aktif olacağı tarih (yyyy-MM-dd) ya da boş.</summary>
        public string Tarih { get; set; }
    }

    public class TipYetkiIstegi
    {
        public int Tip { get; set; }
        public Dictionary<string, string> Degerler { get; set; }
    }

    /// <summary>
    /// Kullanıcı Arama ve Kullanıcı Giriş sayfalarının (eski KYM/KullaniciArama.aspx, KullaniciGiris.aspx, KullaniciYetkiGiris.aspx,
    /// YetkiTipiDegerSec.aspx, YetkiTipiAgactanSec.aspx) JavaScript'ten çağırdığı uçlar:
    ///   GET  /api/kullanici/arama-tanimlar, /liste?...
    ///   GET  /api/kullanici/giris-tanimlar, /oku?kod=, /mernis?tc=&amp;dogum=, /yetki-liste?yt=&amp;ust=
    ///   POST /api/kullanici/tip-yetkileri, /kaydet, /parola-ata, /durum
    /// Eski sayfalar düzenlenen kullanıcıyı oturumda tutuyordu; burada kullanıcı tipleri ve yetki değerleri tarayıcıda tutulur,
    /// kaydederken kullanıcı veri tabanından yeniden okunup gelen değişiklikler yetki denetimiyle uygulanır.
    /// </summary>
    public static class KullaniciUclari
    {
        /// <summary>Kullanıcı yöneticisinin ekleyip düzenleyebildiği kullanıcı tipleri (eski KullaniciYetkiGiris.aspx).</summary>
        private static readonly ENUMKullaniciTipi[] kullaniciYoneticisiTipleri =
        {
            ENUMKullaniciTipi.PERSONEL, ENUMKullaniciTipi.TASINIRMALKAYDI, ENUMKullaniciTipi.ODEMEEMRI_MIF, ENUMKullaniciTipi.ALACAKTAKIP,
            ENUMKullaniciTipi.EVRAK, ENUMKullaniciTipi.EVRAKARSIVCI, ENUMKullaniciTipi.EVRAKBELGEYONETICISI, ENUMKullaniciTipi.EVRAKMISAFIR
        };

        private static readonly CultureInfo turkce = new CultureInfo("tr-TR");

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/kullanici/arama-tanimlar", AramaTanimlar);
            app.MapGet("/api/kullanici/liste", Liste);
            app.MapGet("/api/kullanici/giris-tanimlar", GirisTanimlar);
            app.MapGet("/api/kullanici/oku", Oku);
            app.MapGet("/api/kullanici/mernis", Mernis);
            app.MapGet("/api/kullanici/yetki-liste", YetkiListe);
            app.MapPost("/api/kullanici/tip-yetkileri", TipYetkileri);
            app.MapPost("/api/kullanici/kaydet", Kaydet);
            app.MapPost("/api/kullanici/parola-ata", ParolaAta);
            app.MapPost("/api/kullanici/durum", DurumDegistir);
        }

        // ------------------------------------------------------------------ yetki

        private class Rol
        {
            public bool Sistem;
            public bool KullaniciYoneticisi;
            public bool Destek;
            /// <summary>Kullanıcı Giriş sayfasını açabilir mi (eski KullaniciGiris.aspx Page_Load).</summary>
            public bool Duzenleyebilir { get { return Sistem || KullaniciYoneticisi || Destek; } }
        }

        private static Rol RolBul(TNS.KYM.Kullanici kullanan)
        {
            Rol r = new Rol();
            if (kullanan == null) return r;
            r.Sistem = kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
            r.KullaniciYoneticisi = kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI);
            r.Destek = Yetki.DestekYoneticisiMi(kullanan).islemSonuc;
            return r;
        }

        public static bool GirisYetkisiVar(TNS.KYM.Kullanici kullanan)
        {
            return RolBul(kullanan).Duzenleyebilir;
        }

        public static bool EntegrasyonVar()
        {
            return OrtakFonksiyonlar.ConvertToInt(Ayar.Oku("Entegrasyon", "0"), 0) > 0;
        }

        /// <summary>Bu kullanıcı tipini ekleyip yetki değerlerini düzenleyebilir mi (eski KullaniciYetkiGiris.aspx'teki seçenek listesi).</summary>
        private static bool TipDuzenleyebilir(Rol rol, ENUMKullaniciTipi kt)
        {
            if (rol.Sistem) return true;
            if (rol.KullaniciYoneticisi) return Array.IndexOf(kullaniciYoneticisiTipleri, kt) >= 0;
            if (rol.Destek) return TNS.KYM.Arac.DestekKullaniciTipindenMi((int)kt);
            return false;
        }

        /// <summary>Bu kullanıcı tipini kullanıcıdan silebilir mi (eski KullaniciGiris.aspx YetkiSil).</summary>
        private static bool TipSilebilir(Rol rol, ENUMKullaniciTipi kt)
        {
            if (!rol.Duzenleyebilir) return false;
            return !(rol.Destek && !rol.Sistem && !TNS.KYM.Arac.DestekKullaniciTipindenMi((int)kt));
        }

        // ------------------------------------------------------------------ Kullanıcı Arama

        private static IResult AramaTanimlar(HttpContext baglam)
        {
            TNS.KYM.Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            Rol rol = RolBul(kullanan);

            List<object> tipler = new List<object>();
            if (rol.Duzenleyebilir)
                foreach (ENUMKullaniciTipi kt in TNS.KYM.Arac.KTListesi(kullanan))
                {
                    int kod = TNS.KYM.Arac.KullaniciTipiKodu(kt);
                    if (rol.Destek && !TNS.KYM.Arac.DestekKullaniciTipindenMi(kod)) continue;
                    tipler.Add(new { kod = kod, ad = TNS.KYM.Arac.KullaniciTipiAdi(kt) ?? kt.ToString() });
                }

            return Results.Json(new
            {
                kurumlar = KurumListesi(kullanan, "", false),
                tipler = tipler,
                // Yönetici olmayan kullanıcının araması kendi kurumuyla başlar.
                varsayilanKurum = rol.Sistem ? "" : (kullanan.calistigiKurum ?? ""),
                yeniEklenebilir = rol.Duzenleyebilir && !EntegrasyonVar()
            });
        }

        private static IResult Liste(HttpContext baglam, string ad, string soyad, string kod, string tc, string birim, string kurum, string tip, string durum)
        {
            TNS.KYM.Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            Rol rol = RolBul(kullanan);
            IKYMServis servis = TNS.KYM.Arac.Tanimla();

            TNS.KYM.Kullanici kriter = new TNS.KYM.Kullanici();
            kriter.ad = (ad ?? "").Trim();
            kriter.soyad = (soyad ?? "").Trim();
            kriter.kullaniciKodu = (kod ?? "").Trim();
            kriter.mernis = (tc ?? "").Trim();
            kriter.calistigiBirim = (birim ?? "").Trim();
            // Yönetici olmayanlar, aramada gösterilmesi istenmeyen kullanıcıları göremez.
            if (!(rol.Sistem || rol.KullaniciYoneticisi)) kriter.aramadaGosterme = 0;
            if (!string.IsNullOrWhiteSpace(kurum)) kriter.calistigiKurum = kurum.Trim();
            int tipKod = OrtakFonksiyonlar.ConvertToInt((tip ?? "").Trim(), -1);
            if (tipKod >= 0 && Enum.IsDefined(typeof(ENUMKullaniciTipi), tipKod))
                kriter.kullaniciTipi = new[] { (ENUMKullaniciTipi)tipKod };
            if (durum == "1") kriter.aktifMi = ENUMAktiflik.AKTIF;
            else if (durum == "0") kriter.aktifMi = ENUMAktiflik.PASIF;

            ObjectArray<TNS.KYM.Kullanici> sonuc = servis.KullaniciAra(kriter, 1, 0);
            if (!sonuc.sonuc.islemSonuc)
                return Results.Json(new { hata = Metin.Kullanici("IslemHata") });

            List<object> satirlar = new List<object>();
            foreach (TNS.KYM.Kullanici k in sonuc.objeler)
            {
                // Kimin bilgileri açılabilir: yönetici ve destek yöneticisi herkesi, kullanıcı yöneticisi yönettiklerini.
                // (Eski sayfa kullanıcının kendisine de bağlantı veriyordu ama Giriş sayfası yönetici olmayanı almıyordu.)
                bool duzenlenebilir = rol.Sistem || rol.Destek || (rol.KullaniciYoneticisi && servis.YonetiyorMu(kullanan, k));
                satirlar.Add(new
                {
                    kod = k.kullaniciKodu,
                    adSoyad = ((k.ad ?? "") + " " + (k.soyad ?? "")).Trim(),
                    unvan = k.unvan,
                    birim = k.calistigiBirim,
                    telefon = k.telefon,
                    eposta = k.emailAdresi,
                    aktif = k.aktifMi == ENUMAktiflik.AKTIF,
                    sonGiris = TarihYaz(k.sonGirisTarihi),
                    duzenlenebilir = duzenlenebilir
                });
            }
            return Results.Json(new { satirlar = satirlar });
        }

        // ------------------------------------------------------------------ Kullanıcı Giriş: okuma

        private static IResult GirisTanimlar(HttpContext baglam)
        {
            TNS.KYM.Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            Rol rol = RolBul(kullanan);
            if (!rol.Duzenleyebilir) return Results.StatusCode(StatusCodes.Status403Forbidden);

            List<object> tipler = new List<object>();
            foreach (ENUMKullaniciTipi kt in TNS.KYM.Arac.KTListesi(kullanan))
                if (TipDuzenleyebilir(rol, kt) && TNS.KYM.Arac.KTKullaniliyorMu(kt))
                    tipler.Add(new { kod = TNS.KYM.Arac.KullaniciTipiKodu(kt), ad = TNS.KYM.Arac.KullaniciTipiAdi(kt) ?? kt.ToString() });

            return Results.Json(new
            {
                // Yönetici olmayan yalnızca kendi kurumunun altındaki birimleri görür.
                kurumlar = KurumListesi(kullanan, rol.Sistem ? "" : (kullanan.calistigiKurum ?? ""), true),
                eklenebilirTipler = tipler,
                varsayilanKurum = rol.Sistem ? "" : (kullanan.calistigiKurum ?? ""),
                sistemYoneticisi = rol.Sistem,
                entegrasyon = EntegrasyonVar()
            });
        }

        private static IResult Oku(HttpContext baglam, string kod)
        {
            TNS.KYM.Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            Rol rol = RolBul(kullanan);
            if (!rol.Duzenleyebilir) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (string.IsNullOrWhiteSpace(kod)) return Results.BadRequest();

            IKYMServis servis = TNS.KYM.Arac.Tanimla();
            KullaniciVAN kul = servis.KullaniciOku(kullanan, kod.Trim());
            if (EntegrasyonVar() && (kul == null || !kul.sonuc.islemSonuc))
                kul = servis.KullaniciOkuWebServisten(kullanan, kod.Trim(), "", false);
            if (kul == null || !kul.sonuc.islemSonuc || kul.kullanici == null)
                return Results.Json(new { hata = DuzYazi(kul == null ? "" : kul.sonuc.hataStr, Metin.Kullanici("FRMKUG028")) });

            TNS.KYM.Kullanici k = kul.kullanici;
            List<object> tipler = new List<object>();
            if (k.kullaniciTipi != null)
                foreach (ENUMKullaniciTipi kt in k.kullaniciTipi)
                {
                    if (kt == ENUMKullaniciTipi.TANIMSIZ) continue;
                    tipler.Add(new
                    {
                        kod = (int)kt,
                        ad = TNS.KYM.Arac.KullaniciTipiAdi(kt) ?? kt.ToString(),
                        duzenlenebilir = TipDuzenleyebilir(rol, kt),
                        silinebilir = TipSilebilir(rol, kt)
                    });
                }

            Dictionary<string, string> yetkiler = new Dictionary<string, string>();
            foreach (DictionaryEntry de in k.yetkiTipiDegerleri)
            {
                YetkiTipi yt = TNS.KYM.Arac.YetkiTipiBul((string)de.Key);
                if (yt != null) yetkiler[yt.kod.ToString()] = HashTableArrayList.ArrayListToStr(de.Value) ?? "";
            }

            return Results.Json(new
            {
                kod = k.kullaniciKodu,
                tc = k.mernis,
                dogum = TarihYaz(k.dogumTarihi),
                ad = k.ad,
                soyad = k.soyad,
                telefon = k.telefon,
                eposta = k.emailAdresi,
                kurum = k.calistigiKurum,
                unvan = k.unvan,
                akademikUnvan = k.akademikUnvan,
                birim = k.calistigiBirim,
                sicilNo = k.sicilNo,
                sertNo = k.sertNo,
                rumuz = k.rumuz,
                aktif = k.aktifMi == ENUMAktiflik.AKTIF,
                tipler = tipler,
                yetkiler = yetkiler
            });
        }

        /// <summary>T.C. kimlik no ve doğum tarihinden ad soyad (Mernis) ve kullanıcı kodu önerileri.</summary>
        private static IResult Mernis(HttpContext baglam, string tc, string dogum)
        {
            TNS.KYM.Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!GirisYetkisiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);

            IKYMServis servis = TNS.KYM.Arac.Tanimla();
            KullaniciVAN sonuc = servis.MernisBilgiOku(kullanan, (tc ?? "").Trim(), TarihOku(dogum));
            if (sonuc == null || !sonuc.sonuc.islemSonuc || sonuc.kullanici == null)
                return Results.Json(new { hata = DuzYazi(sonuc == null ? "" : sonuc.sonuc.hataStr, Metin.Kullanici("IslemHata")) });

            return Results.Json(new { ad = sonuc.kullanici.ad, soyad = sonuc.kullanici.soyad, oneriler = Oneriler(servis, sonuc.kullanici) });
        }

        /// <summary>Bir kullanıcı tipinin yetki tipleri ve (verilen değerler için) seçili değerlerin adları. Eski KullaniciYetkiGiris.aspx.</summary>
        private static async Task<IResult> TipYetkileri(HttpContext baglam, TipYetkiIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            TNS.KYM.Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            Rol rol = RolBul(kullanan);
            if (!rol.Duzenleyebilir) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null || !Enum.IsDefined(typeof(ENUMKullaniciTipi), istek.Tip)) return Results.BadRequest();

            ENUMKullaniciTipi kt = (ENUMKullaniciTipi)istek.Tip;
            if (!TipDuzenleyebilir(rol, kt)) return Results.Json(new { hata = Metin.Kullanici("TipDuzenlenemez") });

            IKYMServis servis = TNS.KYM.Arac.Tanimla();
            int yil = DateTime.Now.Year;
            List<object> yetkiler = new List<object>();
            YetkiTipi[] yetkiTipleri = TNS.KYM.Arac.KullaniciTipiYetkiTipleriBul(kt) ?? new YetkiTipi[0];
            foreach (YetkiTipi yt in yetkiTipleri)
            {
                string deger = "";
                if (istek.Degerler != null) istek.Degerler.TryGetValue(yt.kod.ToString(), out deger);
                deger = deger ?? "";

                List<object> secili = new List<object>();
                if (yt.girisSekli == ENUMGirisSekli.LISTEDEN || yt.girisSekli == ENUMGirisSekli.AGACTAN)
                {
                    string[] kodlar = deger.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                    ListeSatir[] liste = null;
                    foreach (string ham in kodlar)
                    {
                        string k = ham.Trim();
                        if (k == "") continue;
                        if (yt.girisSekli == ENUMGirisSekli.AGACTAN)
                        {
                            string ad = k == "*" ? "* " + Metin.Kullanici("FRMYTD008") : servis.YetkiTipiAgacKodAd(kullanan, yt, yil, k, "");
                            secili.Add(new { kod = k, ad = string.IsNullOrEmpty(ad) ? k : ad });
                            continue;
                        }

                        if (liste == null) liste = servis.YetkiTipiGirisListesi(kullanan, yt, yil, "") ?? new ListeSatir[0];
                        bool bulundu = false;
                        foreach (ListeSatir li in liste)
                            if (li != null && li.kod == k)
                            {
                                secili.Add(new { kod = li.kod, ad = li.ad });
                                bulundu = true;
                                break;
                            }
                        if (!bulundu)
                        {
                            // Genel listede yoksa kodun kendisiyle sorulur (eski sayfadaki gibi).
                            ListeSatir[] tek = servis.YetkiTipiGirisListesi(kullanan, yt, yil, k);
                            bool eklendi = false;
                            if (tek != null)
                                foreach (ListeSatir li in tek)
                                    if (li != null && !string.IsNullOrEmpty(li.kod))
                                    {
                                        secili.Add(new { kod = li.kod, ad = li.ad });
                                        eklendi = true;
                                    }
                            if (!eklendi) secili.Add(new { kod = k, ad = k });
                        }
                    }
                }

                yetkiler.Add(new
                {
                    kod = yt.kod,
                    ad = yt.GosterilenAd() ?? yt.ad,
                    giris = (int)yt.girisSekli,
                    cok = yt.degerSayisi == ENUMDegerSayisi.COK,
                    deger = deger,
                    secili = secili
                });
            }

            return Results.Json(new { tip = istek.Tip, ad = TNS.KYM.Arac.KullaniciTipiAdi(kt) ?? kt.ToString(), yetkiler = yetkiler });
        }

        /// <summary>
        /// Bir yetki tipinin seçilebilir değerleri. Listeden seçilenlerde bütün liste, ağaçtan seçilenlerde verilen üst kodun
        /// bir alt düzeyi döner (eski YetkiTipiDegerSec.aspx ve YetkiTipiAgactanSec.aspx).
        /// </summary>
        private static IResult YetkiListe(HttpContext baglam, int yt, string ust)
        {
            TNS.KYM.Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!GirisYetkisiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);

            YetkiTipi yetkiTipi = TNS.KYM.Arac.YetkiTipiBul(yt);
            if (yetkiTipi == null) return Results.NotFound();

            IKYMServis servis = TNS.KYM.Arac.Tanimla();
            int yil = DateTime.Now.Year;
            ListeSatir[] liste;
            if (yetkiTipi.girisSekli == ENUMGirisSekli.AGACTAN)
                liste = servis.YetkiTipiAgacListesi(kullanan, yetkiTipi, yil, ust ?? "", "");
            else
            {
                liste = servis.YetkiTipiGirisListesi(kullanan, yetkiTipi, yil, "");
                if (liste == null || liste.Length == 0) liste = servis.YetkiTipiGirisListesi(kullanan, yetkiTipi, yil - 1, "");
            }

            List<object> satirlar = new List<object>();
            if (liste != null)
                foreach (ListeSatir li in liste)
                    if (li != null && !string.IsNullOrEmpty(li.kod))
                        satirlar.Add(new { kod = li.kod, ad = li.ad, cocuguVar = li.cocuguVar });
            return Results.Json(new { satirlar = satirlar });
        }

        // ------------------------------------------------------------------ Kullanıcı Giriş: yazma

        private static async Task<IResult> Kaydet(HttpContext baglam, KullaniciKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            TNS.KYM.Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            Rol rol = RolBul(kullanan);
            if (!rol.Duzenleyebilir) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null) return Results.BadRequest();
            if (istek.Yeni && EntegrasyonVar()) return Hata(Metin.Kullanici("EntegrasyonYeniYok"));

            string kod = (istek.Kod ?? "").Trim();
            string kurum = (istek.Kurum ?? "").Trim();
            if (kurum == "") return Hata(Metin.Kullanici("FRMKUG027"));
            // Eski formda boş bırakılamayan alanlar. Doğum tarihi yalnızca yeni kullanıcıda zorunludur: eski form var olan
            // kullanıcıda da her kayıtta tarihi yeniden istiyordu; burada boş bırakılırsa kayıtlı tarih korunur.
            if (kod == "" || Bos(istek.Tc) || (istek.Yeni && Bos(istek.Dogum)) || Bos(istek.Ad) || Bos(istek.Soyad) || Bos(istek.Unvan) || Bos(istek.Eposta) || Bos(istek.Telefon))
                return Hata(Metin.Kullanici("FRMKUG057"));

            IKYMServis servis = TNS.KYM.Arac.Tanimla();
            TNS.KYM.Kullanici kul;
            if (istek.Yeni)
                kul = new TNS.KYM.Kullanici();
            else
            {
                KullaniciVAN okunan = servis.KullaniciOku(kullanan, kod);
                if (okunan == null || !okunan.sonuc.islemSonuc || okunan.kullanici == null)
                    return Hata(Metin.Kullanici("FRMKUG028"));
                kul = okunan.kullanici;
                kod = kul.kullaniciKodu;
            }

            // Alanlar (eski KullaniciOl): "/KullaniciBilgileriYazildigiGibi" değişkeni yoksa ad, soyad vb. büyük harfe çevrilir.
            bool yazildigiGibi = !string.IsNullOrEmpty(TNS.DEG.Arac.DegiskenDegerBulCache(0, "/KullaniciBilgileriYazildigiGibi"));
            kul.mernis = (istek.Tc ?? "").Trim();
            if (istek.Yeni || !Bos(istek.Dogum)) kul.dogumTarihi = TarihOku(istek.Dogum);
            kul.ad = Buyut(istek.Ad, yazildigiGibi);
            kul.soyad = Buyut(istek.Soyad, yazildigiGibi);
            kul.calistigiBirim = Buyut(istek.Birim, yazildigiGibi);
            kul.sicilNo = Buyut(istek.SicilNo, yazildigiGibi);
            kul.unvan = Buyut(istek.Unvan, yazildigiGibi);
            kul.akademikUnvan = istek.AkademikUnvan ?? "";
            kul.emailAdresi = yazildigiGibi ? (istek.Eposta ?? "") : (istek.Eposta ?? "").ToLower(turkce);
            kul.telefon = istek.Telefon ?? "";
            kul.kullaniciKodu = kod;
            kul.calistigiKurum = kurum;
            kul.aramadaGosterme = 0;
            kul.sertNo = istek.SertNo ?? "";
            kul.rumuz = istek.Rumuz ?? "";

            string tipHata = TipleriUygula(kullanan, rol, kul, istek);
            if (tipHata != null) return Hata(tipHata);

            Sonuc sonuc = servis.KullaniciKaydet(kullanan, kul, istek.Yeni);
            if (!sonuc.islemSonuc)
                return Results.Json(new
                {
                    basarili = false,
                    hata = DuzYazi(sonuc.hataStr, Metin.Kullanici("IslemHata")),
                    // Kullanıcı kodu kullanılıyorsa seçilebilecek öneriler (eski KullaniciAdOner)
                    oneriler = istek.Yeni ? Oneriler(servis, kul) : new List<string>()
                });

            return Results.Json(new { basarili = true, kod = kul.kullaniciKodu, bilgi = Metin.Kullanici("Kaydedildi") });
        }

        /// <summary>
        /// İstekte gelen kullanıcı tipleri ve yetki değerlerini, yapanın yetkisi ölçüsünde kullanıcıya uygular.
        /// Düzenleyemeyeceği tipler ve onların yetki değerleri veri tabanındaki hâliyle kalır. Hata varsa metnini döndürür.
        /// </summary>
        private static string TipleriUygula(TNS.KYM.Kullanici kullanan, Rol rol, TNS.KYM.Kullanici kul, KullaniciKayitIstegi istek)
        {
            List<ENUMKullaniciTipi> eski = new List<ENUMKullaniciTipi>();
            if (kul.kullaniciTipi != null)
                foreach (ENUMKullaniciTipi kt in kul.kullaniciTipi)
                    if (kt != ENUMKullaniciTipi.TANIMSIZ && !eski.Contains(kt)) eski.Add(kt);

            List<ENUMKullaniciTipi> istenen = new List<ENUMKullaniciTipi>();
            foreach (int t in istek.Tipler ?? new int[0])
                if (Enum.IsDefined(typeof(ENUMKullaniciTipi), t) && (ENUMKullaniciTipi)t != ENUMKullaniciTipi.TANIMSIZ && !istenen.Contains((ENUMKullaniciTipi)t))
                    istenen.Add((ENUMKullaniciTipi)t);

            List<ENUMKullaniciTipi> yeni = new List<ENUMKullaniciTipi>();
            List<ENUMKullaniciTipi> silinen = new List<ENUMKullaniciTipi>();
            foreach (ENUMKullaniciTipi kt in eski)
            {
                // Silinmek istenmiş ama silme yetkisi yoksa tip kalır.
                if (istenen.Contains(kt) || !TipSilebilir(rol, kt)) yeni.Add(kt);
                else silinen.Add(kt);
            }
            foreach (ENUMKullaniciTipi kt in istenen)
            {
                if (yeni.Contains(kt)) continue;
                if (!TipDuzenleyebilir(rol, kt) || !TNS.KYM.Arac.KTKullaniliyorMu(kt)) return Metin.Kullanici("TipDuzenlenemez");
                yeni.Add(kt);
            }

            // Kalan tiplerin yetki tipleri (silinen tipin değeri, başka bir tip de kullanıyorsa korunur)
            HashSet<string> kalanYetkiAdlari = new HashSet<string>();
            foreach (ENUMKullaniciTipi kt in yeni)
                foreach (YetkiTipi yt in TNS.KYM.Arac.KullaniciTipiYetkiTipleriBul(kt) ?? new YetkiTipi[0])
                    kalanYetkiAdlari.Add(yt.ad);
            foreach (ENUMKullaniciTipi kt in silinen)
                foreach (YetkiTipi yt in TNS.KYM.Arac.KullaniciTipiYetkiTipleriBul(kt) ?? new YetkiTipi[0])
                    if (!kalanYetkiAdlari.Contains(yt.ad)) kul.yetkiTipiDegerleri.Remove(yt.ad);

            // Düzenleyebileceği tiplerin yetki değerleri
            foreach (ENUMKullaniciTipi kt in yeni)
            {
                if (!TipDuzenleyebilir(rol, kt)) continue;
                foreach (YetkiTipi yt in TNS.KYM.Arac.KullaniciTipiYetkiTipleriBul(kt) ?? new YetkiTipi[0])
                {
                    string deger;
                    if (istek.Yetkiler == null || !istek.Yetkiler.TryGetValue(yt.kod.ToString(), out deger)) continue;
                    deger = (deger ?? "").Replace('\t', ' ').Trim();

                    string onceki = kul.yetkiTipiDegerleri[yt.ad] ?? "";
                    if (deger == onceki) continue;

                    if (yt.girisSekli == ENUMGirisSekli.YAZIYLA)
                    {
                        string kisit = MuhasebeKisitiDenetle(kullanan, yt, deger);
                        if (kisit != null) return kisit;
                    }
                    kul.yetkiTipiDegerleri.Remove(yt.ad);
                    kul.yetkiTipiDegerleri[yt.ad, 0] = deger;
                }
            }

            kul.kullaniciTipi = yeni.ToArray();
            return null;
        }

        /// <summary>
        /// Kullanıcı yöneticisi, saymanlık/muhasebe yetkilerine yalnızca kendi muhasebe birimini yazabilir
        /// (eski KullaniciYetkiGiris.aspx btnKaydet_Click). Uygunsa null, değilse hata metni.
        /// </summary>
        private static string MuhasebeKisitiDenetle(TNS.KYM.Kullanici kullanan, YetkiTipi yt, string deger)
        {
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI)) return null;
            if (!(yt.ad.Contains("Saymanlık") || yt.ad.Contains("Muhasebe"))) return null;

            string kendi = kullanan.calistigiKurum ?? "";
            string kullanamaz = "";
            foreach (string parca in deger.Split(';'))
            {
                string ilk = parca.Split('-')[0];
                if (kendi.Length > 2 && ilk.Length > 2 && (kendi.EndsWith("00") || kendi.EndsWith("50")) &&
                    OrtakFonksiyonlar.ConvertToInt(ilk.Substring(0, 2), 0) == OrtakFonksiyonlar.ConvertToInt(kendi.Substring(0, 2), 0))
                    kullanamaz = "";
                else if (OrtakFonksiyonlar.ConvertToInt(ilk, 0) != OrtakFonksiyonlar.ConvertToInt(kendi, 0))
                {
                    kullanamaz = deger.Trim() == "" ? "x" : ilk;
                    break;
                }
            }
            if (kullanamaz == "") return null;

            if (OrtakFonksiyonlar.ConvertToInt(kullanamaz.Trim(), 0) == 0)
                return Metin.Kullanici("FRMKYG006") + " " + kendi + " " + Metin.Kullanici("FRMKYG007");
            return kullanamaz + " " + Metin.Kullanici("FRMKYG008") + " " + kendi + " " + Metin.Kullanici("FRMKYG009");
        }

        private static async Task<IResult> ParolaAta(HttpContext baglam, KullaniciKodIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            TNS.KYM.Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!GirisYetkisiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null || Bos(istek.Kod)) return Results.BadRequest();
            if (EntegrasyonVar()) return Results.StatusCode(StatusCodes.Status403Forbidden);

            IKYMServis servis = TNS.KYM.Arac.Tanimla();
            KullaniciVAN kul = servis.KullaniciOku(kullanan, istek.Kod.Trim());
            if (kul == null || !kul.sonuc.islemSonuc) return Hata(Metin.Kullanici("FRMKUG028"));

            Sonuc sonuc = servis.ParolaAta(kullanan, kul.kullanici);
            if (!sonuc.islemSonuc) return Hata(DuzYazi(sonuc.hataStr, Metin.Kullanici("IslemHata")));
            return Results.Json(new { basarili = true, bilgi = Metin.Kullanici("ParolaAtandi") });
        }

        /// <summary>Aktif kullanıcıyı pasif, pasif kullanıcıyı aktif yapar (eski btnAktifPasif_Click).</summary>
        private static async Task<IResult> DurumDegistir(HttpContext baglam, KullaniciDurumIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            TNS.KYM.Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!GirisYetkisiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null || Bos(istek.Kod)) return Results.BadRequest();
            if (EntegrasyonVar()) return Results.StatusCode(StatusCodes.Status403Forbidden);

            IKYMServis servis = TNS.KYM.Arac.Tanimla();
            KullaniciVAN kul = servis.KullaniciOku(kullanan, istek.Kod.Trim());
            if (kul == null || !kul.sonuc.islemSonuc) return Hata(Metin.Kullanici("FRMKUG028"));

            // Aktif yapılırken çalıştığı kurum ve e-posta da değiştirilebilir.
            if (!Bos(istek.Kurum)) kul.kullanici.calistigiKurum = istek.Kurum.Trim();
            if (!Bos(istek.Eposta)) kul.kullanici.emailAdresi = istek.Eposta.Trim();
            kul.kullanici.parolaBitisTarihi = TarihOku(istek.Tarih);

            Sonuc sonuc = kul.kullanici.aktifMi == ENUMAktiflik.AKTIF
                ? servis.KullaniciyiPasifYap(kullanan, kul.kullanici)
                : servis.KullaniciyiAktifYap(kullanan, kul.kullanici);
            if (!sonuc.islemSonuc) return Hata(DuzYazi(sonuc.hataStr, Metin.Kullanici("IslemHata")));
            return Results.Json(new { basarili = true, bilgi = Metin.Kullanici("DurumDegisti") });
        }

        // ------------------------------------------------------------------ yardımcılar

        /// <summary>0 numaralı yetki tipinin (muhasebe birimi / kurum) listesi. noktasiz: kodlar noktasız döner (kayıtta böyle tutulur).</summary>
        private static List<object> KurumListesi(TNS.KYM.Kullanici kullanan, string ustKod, bool noktasiz)
        {
            List<object> kurumlar = new List<object>();
            ListeSatir[] liste = TNS.KYM.Arac.Tanimla().YetkiTipiGirisListesi(kullanan, TNS.KYM.Arac.YetkiTipiBul(0), 0, ustKod ?? "");
            if (liste != null)
                foreach (ListeSatir li in liste)
                    if (li != null && !string.IsNullOrEmpty(li.kod))
                        kurumlar.Add(new { kod = noktasiz ? li.kod.Replace(".", "").Trim() : li.kod, ad = li.kod + " - " + (li.ad ?? "").Trim() });
            return kurumlar;
        }

        private static List<string> Oneriler(IKYMServis servis, TNS.KYM.Kullanici kul)
        {
            List<string> oneriler = new List<string>();
            try
            {
                ObjectArray<TNS.KYM.Kullanici> sonuc = servis.KullaniciAdOner(kul);
                if (sonuc.sonuc.islemSonuc)
                    foreach (TNS.KYM.Kullanici k in sonuc.objeler)
                        if (!string.IsNullOrEmpty(k.kullaniciKodu)) oneriler.Add(k.kullaniciKodu);
            }
            catch (Exception)
            {
                // Öneri üretilemezse kayıt hatası yine gösterilir.
            }
            return oneriler;
        }

        private static IResult Hata(string metin)
        {
            return Results.Json(new { basarili = false, hata = metin });
        }

        private static bool Bos(string deger)
        {
            return string.IsNullOrWhiteSpace(deger);
        }

        private static string Buyut(string deger, bool yazildigiGibi)
        {
            return yazildigiGibi ? (deger ?? "") : (deger ?? "").ToUpper(turkce);
        }

        /// <summary>
        /// Servisin hata metnini düz yazıya çevirir (iş kuralları metne HTML etiketi koyabiliyor). Boşsa ya da yakalanmış bir
        /// istisnanın dökümüyse varsayılan metin döner.
        /// </summary>
        private static string DuzYazi(string hata, string varsayilan)
        {
            hata = hata ?? "";
            if (hata.Trim() == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            hata = Regex.Replace(hata, @"<br\s*/?>", "\n", RegexOptions.IgnoreCase);
            hata = Regex.Replace(hata, "<[^>]+>", "");
            return System.Net.WebUtility.HtmlDecode(hata).Trim();
        }

        private static TNSDateTime TarihOku(string deger)
        {
            if (DateTime.TryParseExact(deger ?? "", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime t))
                return new TNSDateTime(t);
            return new TNSDateTime();
        }

        private static string TarihYaz(TNSDateTime tarih)
        {
            return tarih == null || tarih.isNull ? "" : tarih.Oku().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
    }
}
