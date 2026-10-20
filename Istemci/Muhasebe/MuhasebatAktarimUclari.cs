using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.MUH;

namespace Istemci.Muhasebe
{
    public class MuhasebatKaydetIstegi
    {
        public int Yil { get; set; }
        /// <summary>yyyy-MM-dd</summary>
        public string BelgeTarih { get; set; }
        public string BelgeNo { get; set; }
        public int Muhasebe { get; set; }
        public string Birim { get; set; }
        public int FisTur { get; set; }
        public string Aciklama { get; set; }
        /// <summary>Kriter tarihleri (yyyy-MM-dd; boş olabilir).</summary>
        public string Tarih1 { get; set; }
        public string Tarih2 { get; set; }
        public int YevmiyeNo1 { get; set; }
        public int YevmiyeNo2 { get; set; }
        /// <summary>Tablodaki satırlar; alan adları eski StoreListe alanlarıdır (SIRANO, HESAPKOD, TERTIP, BORC, ALACAK, DETAYBIRIM ...).</summary>
        public List<Dictionary<string, JsonElement>> Satirlar { get; set; }
    }

    public class MuhasebatDurumIstegi
    {
        public int Yil { get; set; }
        public string BelgeNo { get; set; }
        /// <summary>Tuş: onayla (ONAYLI), onaykaldir (ONAYKALDIR), iptal (IPTAL), gonder (MUHASEBATAGONDERILDI).</summary>
        public string Islem { get; set; }
    }

    public class MuhasebatMizanIstegi
    {
        public int Yil { get; set; }
        public int Muhasebe { get; set; }
        public string Birim { get; set; }
        public string Tarih1 { get; set; }
        public string Tarih2 { get; set; }
        public int YevmiyeNo1 { get; set; }
        public int YevmiyeNo2 { get; set; }
    }

    /// <summary>
    /// Muhasebat Aktarım İşlemleri (eski ButceMuhasebe/MuhasebatAktarimAna.aspx; sekmeleri MuhasebatAktarimTCCB.aspx "Belge Düzenleme" ve
    /// MuhasebatAktarimSorguTCCB.aspx "Belge Arama") uçları. Eski code-behind'lardaki işlemler aynen; iş kuralı MUH EntegrasyonMIF*.
    ///   GET  /api/muhasebataktarim/tanim          açılış: yıl, tarih, fiş türleri, detay birim var mı, kullanıcının entegrasyon muhasebe/birimi
    ///   GET  /api/muhasebataktarim/belge          ?yil&amp;belgeNo : eski btnListele_Click + BelgeFormaYaz
    ///   POST /api/muhasebataktarim/kaydet         eski btnKaydet_Click (+ DetayDoldur)
    ///   POST /api/muhasebataktarim/durum          eski BelgeDurumGuncelle: Onayla / Onay Kaldır / İptal Et / Muhasebat'a Gönder
    ///   GET  /api/muhasebataktarim/yazdir         ?yil&amp;belgeNo : eski btnYazdir_Click (RaporSablon/MUH/MUH-EntegrasyonMIF.XLT)
    ///   POST /api/muhasebataktarim/mizan          eski ListeleMizan_Click (mizandan satır oluşturur, entegrasyon muhasebe/birimini saklar)
    ///   GET  /api/muhasebataktarim/detay-birimler ?muhasebe : eski SaymanlikBirimDoldur (stoBirim)
    ///   GET  /api/muhasebataktarim/hesap/ara|ad|agac : hesap kodu önerisi (eski HesapStore_Refresh) ve hesap planı penceresi
    ///   GET  /api/muhasebataktarim/liste          eski MuhasebatAktarimSorguTCCB.Liste_Click
    ///   GET  /api/muhasebataktarim/tarihce        ?yil&amp;belgeNo : eski BelgeTarihce.aspx?belgeTur=999
    /// Yetki: menüdeki çerçeve sayfası (MuhasebatAktarimAna) ve eski sayfalardaki MUH.Yetki (Gorme, ODEMEEMRI).
    /// </summary>
    public static class MuhasebatAktarimUclari
    {
        public const string SayfaAdres = "../ButceMuhasebe/MuhasebatAktarimAna.aspx";
        public const string FormAdres = "../ButceMuhasebe/MuhasebatAktarimTCCB.aspx";
        public const string SorguAdres = "../ButceMuhasebe/MuhasebatAktarimSorguTCCB.aspx";
        private const string Uc = "/api/muhasebataktarim/";
        /// <summary>Eski SatirAc: bir seferde açılan boş satır sayısı.</summary>
        public const int BosSatirSayisi = 20;
        /// <summary>Eski TarihceGoster: BelgeTarihce.aspx?belgeTur=999.</summary>
        private const int TarihceBelgeTur = 999;

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "tanim", Tanim);
            app.MapGet(Uc + "belge", Belge);
            app.MapPost(Uc + "kaydet", Kaydet);
            app.MapPost(Uc + "durum", Durum);
            app.MapGet(Uc + "yazdir", Yazdir);
            app.MapPost(Uc + "mizan", Mizan);
            app.MapGet(Uc + "detay-birimler", DetayBirimler);
            app.MapGet(Uc + "hesap/ara", HesapAra);
            app.MapGet(Uc + "hesap/ad", HesapAd);
            app.MapGet(Uc + "hesap/agac", HesapAgac);
            app.MapGet(Uc + "liste", Liste);
            app.MapGet(Uc + "tarihce", Tarihce);
        }

        // ------------------------------------------------------------------ yetki ve ortak

        /// <summary>Çerçeve (menü) yetkisi ve eski sayfaların girişteki ödeme emri görme yetkisi.</summary>
        public static bool Yetkili(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (!SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, SayfaAdres)) return false;
            try
            {
                return TNS.MUH.Yetki.YetkisiVarMi(kullanan, (int)ENUMOGY.Gorme, (int)ENUMBelgeTuru.ODEMEEMRI);
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

        private static IMUHServis Servis() { return TNS.MUH.Arac.Tanimla(); }

        private static IResult Hata(string hata) { return Results.Json(new { basarili = false, hata = Duz(hata) }); }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = Temiz(hata);
            return hata == "" ? varsayilan : hata;
        }

        /// <summary>Eski mesajlardaki HTML (&lt;br&gt;, &lt;b&gt;) düz metne çevrilir.</summary>
        private static string Duz(string s)
        {
            if (string.IsNullOrEmpty(s)) return s ?? "";
            s = Regex.Replace(s, @"<br\s*/?>", "\n", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, "<[^>]+>", "");
            s = System.Net.WebUtility.HtmlDecode(s);
            return s.Trim('\n', ' ');
        }

        /// <summary>yyyy-MM-dd → tarih; boş ya da bozuksa null.</summary>
        private static DateTime? TarihCoz(string s)
        {
            s = Temiz(s);
            if (s != "" && DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d)) return d;
            return null;
        }

        private static string TarihYaz(TNSDateTime t)
        {
            if (t == null || t.isNull) return "";
            DateTime d = t.Oku();
            return d.Year <= 1900 ? "" : d.ToString("yyyy-MM-dd");
        }

        /// <summary>Eski btnListele_Click / btnYazdir_Click: 9 haneden kısa belge numarasının başına kurum kodu eklenir.</summary>
        private static string BelgeNoTamamla(string belgeNo)
        {
            belgeNo = Temiz(belgeNo);
            if (belgeNo != "" && belgeNo.Length < 9)
                belgeNo = (GenelIslemlerIstemci.VarsayilanKurumBul() ?? "").Replace(".", "").PadRight(4, '0') + belgeNo.PadLeft(6, '0');
            return belgeNo;
        }

        /// <summary>Eski MuhasebatAktarimSorguTCCB.DurumDonustur.</summary>
        public static string DurumDonustur(int durum)
        {
            if (durum == (int)ENUMBelgeDurumu.YENI)
                return "Yeni";
            else if (durum == (int)ENUMBelgeDurumu.DEGISTIRILDI)
                return "Değiştirildi";
            else if (durum == (int)ENUMBelgeDurumu.ONAYLI)
                return "Onaylı";
            else if (durum == (int)ENUMBelgeDurumu.MUHASEBATAGONDERILDI)
                return "Onaya Gönderildi";
            else if (durum == (int)ENUMBelgeDurumu.ONAYKALDIR)
                return "Onay Kaldırıldı";
            else if (durum == (int)ENUMBelgeDurumu.IPTAL)
                return "İptal";
            else
                return "Belirsiz - " + durum;
        }

        // ------------------------------------------------------------------ açılış (eski Page_Load'lar)

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                // Eski ButceMuhasebeGenel.FisTurDoldur
                List<object> fisTurler = new List<object>();
                foreach (ENUMFISTUR f in new[] { ENUMFISTUR.DONEMICI, ENUMFISTUR.ACILIS, ENUMFISTUR.KAPANIS, ENUMFISTUR.AMORTISMANKAPANISI, ENUMFISTUR.SONFIS })
                    fisTurler.Add(new { kod = (int)f, ad = TNS.MUH.Arac.FisTurAdBul(f) });

                // Eski Belge Arama durumStore
                List<object> durumlar = new List<object>
                {
                    new { kod = 1, ad = "Yeni Belgeler" },
                    new { kod = 5, ad = "Onaylı" },
                    new { kod = 9, ad = "İptal Edilenler" },
                    new { kod = 22, ad = "Bütün Belgeler (İptaller Hariç)" }
                };

                List<int> yillar = new List<int>();
                for (int i = 2005; i <= DateTime.Now.Year + 1; i++) yillar.Add(i);

                return Results.Json(new
                {
                    yil = DateTime.Now.Year,
                    yillar = yillar,
                    belgeTarih = DateTime.Now.ToString("yyyy-MM-dd"),
                    fisTurler = fisTurler,
                    durumlar = durumlar,
                    durum = 1,
                    detayBirim = TNS.MUH.Arac.DetayBirimVarMi(),
                    muhasebe = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBEENTEGRASYON") ?? "",
                    birim = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBEBIRIMENTEGRASYON") ?? "",
                    satirSayisi = BosSatirSayisi
                });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Sayfa bilgileri okunamadı."));
            }
        }

        // ------------------------------------------------------------------ belge oku (eski btnListele_Click + BelgeFormaYaz)

        private static IResult Belge(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string belgeNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            string hataStr = "";
            belgeNo = Temiz(belgeNo);
            if (belgeNo == "")
                hataStr = "Belge listelemesi için, Belge No alanına görmek istediğiniz belgenin nosunu yazmanız gerekmektedir.";
            if ((yil ?? 0) == 0)
                hataStr += "\nBelge listelemesi için, Yıl alanına görmek istediğiniz belgenin yılını yazmanız gerekmektedir.";
            if (hataStr != "") return Hata(hataStr);

            try
            {
                belgeNo = BelgeNoTamamla(belgeNo);

                OdemeEmriMIFForm kriter = new OdemeEmriMIFForm();
                kriter.yil = yil.Value;
                kriter.belgeNo = belgeNo;

                ObjectArray fler = Servis().EntegrasyonMIFListele(kullanan, kriter, true);
                if (!fler.sonuc.islemSonuc) return Hata(HataMetni(fler.sonuc.hataStr, "Belge okunamadı."));
                if (fler.objeler.Count == 0 || ((OdemeEmriMIFForm)fler.objeler[0]).kurum == "")
                    return Hata("Görmek istediğiniz " + belgeNo + " nolu belge bulunamadı");

                return Results.Json(new { basarili = true, belge = BelgeYaz((OdemeEmriMIFForm)fler.objeler[0]) });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Belge okunamadı."));
            }
        }

        /// <summary>Eski BelgeFormaYaz: formun alanları ve tablo satırları.</summary>
        private static object BelgeYaz(OdemeEmriMIFForm form)
        {
            List<object> satirlar = new List<object>();
            string altHesapKod = "";
            foreach (OdemeEmriMIFDetay detay in form.detay.objeler)
            {
                altHesapKod = detay.altHesapKod;
                if (altHesapKod != "") altHesapKod += " - " + detay.altHesapAd;
                satirlar.Add(new
                {
                    SIRANO = detay.satirNo,
                    HESAPKOD = detay.hesapKod + "-" + detay.aciklama,
                    HESAPAD = detay.aciklama,
                    TERTIP = detay.sorguTertip,
                    BORC = detay.borc,
                    ALACAK = detay.alacak,
                    ALTHESAPKOD = altHesapKod,
                    SATIRACIKLAMA = detay.satirAciklama,
                    DETAYBIRIM = detay.DetayBirim
                });
            }

            ENUMFISTUR[] ft = form.fisTur;
            return new
            {
                yil = form.yil,
                belgeTarih = TarihYaz(form.islemTarih),
                belgeNo = form.belgeNo,
                muhasebe = form.muhasebe,
                birim = form.birim,
                fisTur = ft != null && ft.Length > 0 ? (int)ft[0] : 0,
                aciklama = form.aciklama,
                tarih1 = TarihYaz(form.baslamaTarihi),
                tarih2 = TarihYaz(form.bitisTarihi),
                yevmiyeNo1 = form.yevmiyeNo1,
                yevmiyeNo2 = form.yevmiyeNo2,
                durum = form.formDurum,
                durumAd = DurumDonustur(form.formDurum),
                satirlar = satirlar
            };
        }

        // ------------------------------------------------------------------ kaydet (eski btnKaydet_Click)

        private static string M(Dictionary<string, JsonElement> row, string ad)
        {
            if (row == null || !row.TryGetValue(ad, out JsonElement e)) return null;
            if (e.ValueKind == JsonValueKind.String) return e.GetString();
            if (e.ValueKind == JsonValueKind.Number) return e.GetRawText();
            if (e.ValueKind == JsonValueKind.True) return "1";
            if (e.ValueKind == JsonValueKind.False) return "0";
            return null;
        }

        private static double D(Dictionary<string, JsonElement> row, string ad)
        {
            if (row == null || !row.TryGetValue(ad, out JsonElement e)) return 0;
            if (e.ValueKind == JsonValueKind.Number && e.TryGetDouble(out double d)) return d;
            if (e.ValueKind == JsonValueKind.String && double.TryParse(e.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out double s)) return s;
            return 0;
        }

        private static int I(Dictionary<string, JsonElement> row, string ad)
        {
            if (row == null || !row.TryGetValue(ad, out JsonElement e)) return 0;
            if (e.ValueKind == JsonValueKind.Number && e.TryGetInt32(out int i)) return i;
            if (e.ValueKind == JsonValueKind.String && int.TryParse(e.GetString(), out int s)) return s;
            return 0;
        }

        /// <summary>Eski DetayDoldur: tablodaki satırlardan detaylar (hesap kodu boş olanlar atlanır).</summary>
        private static ObjectArray DetayDoldur(int yil, List<Dictionary<string, JsonElement>> satirlar)
        {
            ObjectArray pler = new ObjectArray();
            if (satirlar == null) return pler;

            foreach (Dictionary<string, JsonElement> row in satirlar)
            {
                OdemeEmriMIFDetay p = new OdemeEmriMIFDetay(yil);
                p.hesapKod = M(row, "HESAPKOD");
                if (string.IsNullOrEmpty(p.hesapKod))
                    continue;

                p.satirNo = I(row, "SIRANO");

                if (!string.IsNullOrEmpty(p.hesapKod))
                {
                    string[] kod = p.hesapKod.Split('-');
                    p.hesapKod = kod[0];
                    if (kod.Length > 1)
                        p.aciklama = kod[1];
                }

                string tertip = M(row, "TERTIP");
                if (!string.IsNullOrEmpty(tertip))
                    p.sorguTertip = tertip;

                p.borc = Math.Round(D(row, "BORC"), 2);
                p.alacak = Math.Round(D(row, "ALACAK"), 2);

                p.satirAciklama = M(row, "SATIRACIKLAMA");
                p.detayBirim = M(row, "DETAYBIRIM") ?? "";

                pler.objeler.Add(p);
            }

            return pler;
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, MuhasebatKaydetIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            try
            {
                OdemeEmriMIFForm bilgi = new OdemeEmriMIFForm();

                bilgi.yil = istek.Yil;
                DateTime? belgeTarih = TarihCoz(istek.BelgeTarih);
                bilgi.islemTarih = belgeTarih.HasValue ? new TNSDateTime(belgeTarih.Value) : new TNSDateTime();
                bilgi.belgeNo = Temiz(istek.BelgeNo);
                bilgi.kurum = GenelIslemlerIstemci.VarsayilanKurumBul();

                bilgi.muhasebe = istek.Muhasebe;
                bilgi.birim = istek.Birim ?? "";
                bilgi.formDurum = (int)ENUMBelgeDurumu.YENI;

                string hata = "";
                double bakiye = 0;

                if (bilgi.yil == 0)
                    hata = "Yıl bilgisi boş bırakılamaz.";
                if (bilgi.muhasebe <= 0 || string.IsNullOrWhiteSpace(bilgi.birim))
                    hata += "\nBirim bilgisi boş bırakılamaz";
                if (TNSDateTime.IsNull(bilgi.islemTarih))
                    hata += "\nBelge Tarihi bilgisi boş bırakılamaz";
                else
                {
                    if (bilgi.islemTarih.Yil != bilgi.yil)
                        hata += "\n" + string.Format("İşlem yapılan Yıl ({0}) ile belge tarihi ({1}) arasında uyuşmazlık var.", bilgi.yil.ToString(), bilgi.islemTarih.ToString());
                }

                ObjectArray pler = DetayDoldur(bilgi.yil, istek.Satirlar);

                bool bulundu = false;
                int i = 0;

                foreach (OdemeEmriMIFDetay p in pler.objeler)
                {
                    string hesapNo = p.hesapKod.Replace(".", "").Trim();

                    if (hesapNo != "" && hesapNo != null)
                    {
                        bulundu = true;
                        double borc = p.borc;
                        double alacak = p.alacak;
                        bool borcKontrol = borc > 0;
                        bool alacakKontrol = alacak > 0;

                        bakiye += borc - alacak;
                        bakiye = Math.Round(bakiye, 2);

                        if (borcKontrol && alacakKontrol)
                            hata += "\n" + p.hesapKod + " koduna ait satırda, Borç: " + p.borc.ToString() + " Alacak: " + p.alacak.ToString()
                                + " - Bir belge detayında aynı satırda hem borç hemde alacak alanına rakam yazılamaz.";
                    }
                    else
                    {
                        double borc = p.borc;
                        double alacak = p.alacak;

                        if (borc != 0 || alacak != 0)
                            return Results.Json(new
                            {
                                basarili = false,
                                uyari = string.Format("Belgenin detayında hesap kodu girilmediği halde Borç veya Alacak alanlarına rakam girilmiş. Satır : {0}", (i + 1).ToString())
                            });
                    }
                    i++;
                }

                if (!bulundu)
                    return Hata("Belgenin detayında kayıt edilecek herhangi bir bilgi bulunamadı.");
                else if (bakiye != 0) // Eğer bütün satırlar tamamlanmadan yukarıdaki kontrolden çıkıldı ise bu kontrol yapılmasın
                    hata += "\nBakiye:" + bakiye + "-BORÇ tutarı ile ALACAK tutarı birbirine eşit değil.";

                if (hata != "")
                    return Hata(hata);

                bilgi.aciklama = istek.Aciklama ?? "";

                bilgi.islemYapan = kullanan.kullaniciKodu;
                bilgi.islemYapanAd = kullanan.ad + " " + kullanan.soyad;
                bilgi.fisTurYap((ENUMFISTUR)istek.FisTur);
                // Eski (DateTime)txtKriterTarih1.Value: boş tarih alanının değeri DateTime.MinValue idi.
                bilgi.baslamaTarihi = new TNSDateTime(TarihCoz(istek.Tarih1) ?? DateTime.MinValue);
                bilgi.bitisTarihi = new TNSDateTime(TarihCoz(istek.Tarih2) ?? DateTime.MinValue);
                bilgi.yevmiyeNo1 = istek.YevmiyeNo1;
                bilgi.yevmiyeNo2 = istek.YevmiyeNo2;

                foreach (OdemeEmriMIFDetay p in pler.objeler)
                {
                    OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(bilgi.yil);

                    string hesapNo = p.hesapKod;

                    if (!string.IsNullOrEmpty(hesapNo))
                    {
                        string tertip = TNS.MUH.Arac.TertipStrOl(p);

                        if (!string.IsNullOrEmpty(tertip))
                        {
                            TNS.MUH.Arac.TertipAta(ref detay, tertip);
                        }

                        detay.sorguTertip = p.sorguTertip;
                        detay.kesinti = p.kesinti;
                        detay.hesapKod = hesapNo.Replace(".", "").Trim();
                        detay.borc = Math.Round(p.borc, 2);
                        detay.alacak = Math.Round(p.alacak, 2);
                        detay.satirNo = p.satirNo;
                        detay.satirAciklama = p.satirAciklama;
                        detay.altHesapKod = p.altHesapKod;

                        string[] dBirim = (p.detayBirim ?? "").Split('.');
                        detay.detayBirim = dBirim[0];
                        for (int ii = 0; ii < dBirim.Length; ii++)
                        {
                            if (ii == 0) continue;
                            detay.kur += dBirim[ii];
                        }

                        bilgi.detay.objeler.Add(detay);
                    }
                }

                Sonuc sonuc = Servis().EntegrasyonMIFKaydet(kullanan, bilgi);
                if (sonuc.islemSonuc)
                    return Results.Json(new { basarili = true, bilgi = Duz(sonuc.bilgiStr), belgeNo = bilgi.belgeNo == "" ? sonuc.anahtar : bilgi.belgeNo });
                return Hata("Belge kaydı yapılamadı." + sonuc.hataStr + "\n" + sonuc.bilgiStr);
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Belge kaydedilemedi."));
            }
        }

        // ------------------------------------------------------------------ durum (eski BelgeDurumGuncelle)

        private static async Task<IResult> Durum(HttpContext baglam, IWebHostEnvironment ortam, MuhasebatDurumIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            int durum;
            switch (Temiz(istek.Islem))
            {
                case "onayla": durum = (int)ENUMBelgeDurumu.ONAYLI; break;           // eski btnOnayla_Click
                case "onaykaldir": durum = (int)ENUMBelgeDurumu.ONAYKALDIR; break;   // eski btnOnayKaldir_Click
                case "iptal": durum = (int)ENUMBelgeDurumu.IPTAL; break;             // eski btnIptal_Click
                case "gonder": durum = (int)ENUMBelgeDurumu.MUHASEBATAGONDERILDI; break; // eski btnOnayaGonder_Click
                default: return Results.BadRequest();
            }

            try
            {
                IMUHServis servisMUH = Servis();
                OdemeEmriMIFForm bilgi = new OdemeEmriMIFForm();
                Sonuc sonuc = new Sonuc(true, "", "");

                bilgi.yil = istek.Yil;
                bilgi.belgeNo = istek.BelgeNo ?? "";

                // Eski kod belge numarası boşsa hiçbir şey yapmadan dönerdi.
                if (string.IsNullOrWhiteSpace(bilgi.belgeNo))
                    return Results.Json(new { basarili = true, bos = true });

                if (durum == (int)ENUMBelgeDurumu.MUHASEBATAGONDERILDI)
                {
                    ObjectArray oebListe = servisMUH.EntegrasyonMIFListele(kullanan, bilgi, true);
                    sonuc = servisMUH.MIFEntegrasyonGonder(kullanan, oebListe);
                }

                if (sonuc.islemSonuc)
                {
                    bilgi.formDurum = durum;
                    sonuc = servisMUH.EntegrasyonMIFGuncelleDurum(kullanan, bilgi);
                }

                string olumluBilgi = "";
                string olumsuzBilgi = "";
                if (durum == (int)ENUMBelgeDurumu.ONAYLI)
                {
                    olumluBilgi = "Belge başarıyla Onaylandı.";
                    olumsuzBilgi = "Belgenin Onaylama işlemi gerçekleştirilemedi.";
                }
                else if (durum == (int)ENUMBelgeDurumu.MUHASEBATAGONDERILDI)
                {
                    olumluBilgi = "Seçili belgeler başarıyla Muhasebata Gönderildi.";
                    olumsuzBilgi = "Seçili belgelerin Muhasebata Gönderme işlemleri gerçekleştirilemedi.";
                }
                else if (durum == (int)ENUMBelgeDurumu.IPTAL)
                {
                    olumluBilgi = "Seçili belgelerin İptal edilme işlemleri başarıyla gerçekleştirildi.";
                    olumsuzBilgi = "Seçili belgelerin İptal edilme işlemleri gerçekleştirilemedi.";
                }
                else if (durum == (int)ENUMBelgeDurumu.ONAYKALDIR)
                {
                    olumluBilgi = "Belgenin Onayı başarıyla kaldırıldı.";
                    olumsuzBilgi = "Belgenin Onayı kaldırılamadı.";
                }

                if (sonuc.islemSonuc)
                    return Results.Json(new { basarili = true, bilgi = Duz(olumluBilgi + sonuc.bilgiStr), durum = durum, durumAd = DurumDonustur(durum) });
                return Hata(olumsuzBilgi + sonuc.hataStr);
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "İşlem yapılamadı."));
            }
        }

        // ------------------------------------------------------------------ yazdır (eski btnYazdir_Click)

        private static string SablonYol(string sablonAd)
        {
            string kurum = Path.Combine(Ayar.Oku("RaporSablonYol") ?? "", "MUH", sablonAd);
            if (File.Exists(kurum)) return kurum;
            foreach (string kok in new string[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
            {
                string uygulama = Path.Combine(kok, "RaporSablon", "MUH", sablonAd);
                if (File.Exists(uygulama)) return uygulama;
            }
            return kurum;
        }

        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string belgeNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            string hataStr = "";
            belgeNo = Temiz(belgeNo);
            if (belgeNo == "")
                hataStr = "Belge listelemesi için, Belge No alanına görmek istediğiniz belgenin nosunu yazmanız gerekmektedir.";
            if ((yil ?? 0) == 0)
                hataStr += "\nBelge listelemesi için, Yıl alanına görmek istediğiniz belgenin yılını yazmanız gerekmektedir.";
            if (hataStr != "") return Hata(hataStr);

            try
            {
                belgeNo = BelgeNoTamamla(belgeNo);

                OdemeEmriMIFForm kriter = new OdemeEmriMIFForm();
                kriter.yil = yil.Value;
                kriter.belgeNo = belgeNo;

                ObjectArray bilgiler = Servis().EntegrasyonMIFListele(kullanan, kriter, true);

                string kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
                string kurumAd = GenelIslemlerIstemci.VarsayilanKurumBulAd();

                string sablonAd = "MUH-EntegrasyonMIF.XLT";
                string sablon = SablonYol(sablonAd);
                if (!File.Exists(sablon)) return Hata("Şablon (MUH/" + sablonAd + ") bulunamadı.");

                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                int kaynakSatir = 0;
                int sutun = 0;
                int yazSatir = 0;

                string sablonDosyaAc = DosyaIslem.DosyaAdUret("EntegrasyonMIF", "") + ".xlsx";

                XLS.DosyaAc(sablon, sablonDosyaAc);
                XLS.HucreAdAdresCoz("BaslaSatir", ref yazSatir, ref sutun);

                foreach (OdemeEmriMIFForm item in bilgiler.objeler)
                {
                    XLS.HucreAdBulYaz("Yil", item.yil);
                    XLS.HucreAdBulYaz("BelgeNo", item.belgeNo);
                    XLS.HucreAdBulYaz("BelgeTarihi", item.islemTarih.ToString());
                    XLS.HucreAdBulYaz("KurumAd", kurum + " - " + kurumAd);
                    XLS.HucreAdBulYaz("SaymanlikAd", item.muhasebe + " - " + item.muhasebeAd);

                    foreach (OdemeEmriMIFDetay ditem in item.detay.objeler)
                    {
                        XLS.SatirAc(yazSatir, 1);
                        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 18, yazSatir, sutun);

                        XLS.HucreDegerYaz(yazSatir, 0, ditem.hesapKod);

                        if (!string.IsNullOrEmpty(ditem.kur))
                        {
                            XLS.HucreDegerYaz(yazSatir, 1, ditem.kur);
                        }

                        if (!string.IsNullOrEmpty(ditem.sorguTertip))
                        {
                            XLS.HucreDegerYaz(yazSatir, 2, ditem.sorguTertip);
                        }

                        XLS.HucreDegerYaz(yazSatir, 3, ditem.aciklama);
                        XLS.HucreDegerYaz(yazSatir, 4, ditem.borc);
                        XLS.HucreDegerYaz(yazSatir, 5, ditem.alacak);
                        XLS.HucreDegerYaz(yazSatir, 6, ditem.kalanBorc);
                        XLS.HucreDegerYaz(yazSatir, 7, ditem.kalanAlacak);

                        yazSatir++;
                    }
                }

                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(sablonDosyaAc, "EntegrasyonMIFForm_" + DateTime.Now.Ticks, true, GenelIslemler.ExcelTur());
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }

        // ------------------------------------------------------------------ mizandan listele (eski ListeleMizan_Click)

        private static async Task<IResult> Mizan(HttpContext baglam, IWebHostEnvironment ortam, MuhasebatMizanIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            try
            {
                IMUHServis servisMUH = Servis();

                int yil = istek.Yil;
                string kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
                int saymanlik = istek.Muhasebe;
                string birim = istek.Birim ?? "";
                if (birim == "000") birim = "";

                DateTime? t1 = TarihCoz(istek.Tarih1);
                DateTime? t2 = TarihCoz(istek.Tarih2);
                TNSDateTime tarih1 = t1.HasValue ? new TNSDateTime(t1.Value) : new TNSDateTime("01.01." + yil);
                TNSDateTime tarih2 = t2.HasValue ? new TNSDateTime(t2.Value) : new TNSDateTime("31.12." + yil);
                int yevmiyeNo1 = istek.YevmiyeNo1;
                int yevmiyeNo2 = istek.YevmiyeNo2;
                int seviye = 9;

                ENUMFISTUR[] fisTur = null;

                //Gerçek eko kod (normalde hesap kodundan eko kod çıkartılıyor) olacak - true set edildi Melih 22022022
                MizanBilgi mb = servisMUH.MizanTertip(kullanan, yil, kurum, saymanlik, birim, tarih1, tarih2, seviye, "", "", 1, fisTur, 0, "", true, yevmiyeNo1, yevmiyeNo2);

                // Eski kod burada hesap planını da okuyordu (HesapPlaniListesiDoldur); yalnızca kullanılmayan TertipAlaninaGoreDuzenle
                // içindi ("pebkodları ekokod bazında grupluyordu" diye kaldırılmış), Core'da okunmuyor.

                Hashtable htPrgIliski = TNS.BKD.Arac.ProgramButceIliskiHazirla(kullanan, yil);

                double toplamBorc = 0.0;
                double toplamAlacak = 0.0;

                List<object> liste = new List<object>();
                int satirNo = 1;
                for (int i = 0; i < mb.anaHesaplar.objeler.Count; i++)
                {
                    AnaHesap ah = (AnaHesap)mb.anaHesaplar[i];

                    for (int j = 0; j < ah.detayHesaplar.objeler.Count; j++)
                    {
                        HesapBilgi hb = (HesapBilgi)ah.detayHesaplar[j];
                        if (hb.hesapKod == "") continue;

                        if (string.IsNullOrWhiteSpace(hb.fon))
                        {
                            hb.eko = "";
                        }

                        toplamBorc += hb.borc;
                        toplamAlacak += hb.alacak;

                        string tertip = "";
                        if (hb.hesapKod.StartsWith("630") ||
                            hb.hesapKod.StartsWith("830") ||
                            hb.hesapKod.StartsWith("90"))
                        {
                            //Prog.Altprog.Faal.Altfaal.kkod1.kkod2.Finkod.Ekod1.Ekod2
                            tertip = TNS.MUH.Arac.ProgramTertipHazirla(hb, htPrgIliski);
                        }

                        if (hb.borc > 0)
                        {
                            liste.Add(new
                            {
                                SIRANO = satirNo,
                                HESAPKOD = hb.hesapKod + "-" + hb.hesapAd,
                                HESAPAD = hb.hesapAd,
                                TERTIP = tertip,
                                BORC = hb.borc,
                                ALACAK = 0,
                                ALTHESAPKOD = hb.altHesapKod,
                                SATIRACIKLAMA = "",
                                DETAYBIRIM = hb.birimKod + "." + hb.kur,
                            });
                        }
                        if (hb.alacak > 0)
                        {
                            liste.Add(new
                            {
                                SIRANO = satirNo++,
                                HESAPKOD = hb.hesapKod + "-" + hb.hesapAd,
                                HESAPAD = hb.hesapAd,
                                TERTIP = tertip,
                                BORC = 0,
                                ALACAK = hb.alacak,
                                ALTHESAPKOD = hb.altHesapKod,
                                SATIRACIKLAMA = "",
                                DETAYBIRIM = hb.birimKod + "." + hb.kur,
                            });
                        }
                        satirNo++;
                    }
                }

                // Eski DegiskenleriKaydet
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "MUHASEBEENTEGRASYON", istek.Muhasebe > 0 ? istek.Muhasebe.ToString() : "");
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "MUHASEBEBIRIMENTEGRASYON", istek.Birim ?? "");

                return Results.Json(new { basarili = true, satirlar = liste, toplamBorc = toplamBorc, toplamAlacak = toplamAlacak });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Mizan okunamadı."));
            }
        }

        // ------------------------------------------------------------------ detay birimler (eski SaymanlikBirimDoldur)

        private static IResult DetayBirimler(HttpContext baglam, IWebHostEnvironment ortam, int? muhasebe)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici _);
            if (ret != null) return ret;
            List<object> liste = new List<object>();
            try
            {
                int saymanlikKod = muhasebe ?? 0;
                if (saymanlikKod > 0)
                {
                    MuhasebeBirim mb = new MuhasebeBirim();
                    mb.muhasebe = saymanlikKod;
                    ObjectArray oa = Servis().MuhasebeBirimListele(mb);
                    if (oa.objeler.Count > 0)
                    {
                        // Eski combo: ValueField BirimKurum (005.12345678), DisplayField BirimKodAd; sonda boş seçenek.
                        foreach (MuhasebeBirim b in oa.objeler)
                            liste.Add(new { kod = b.BirimKurum, ad = b.BirimKodAd });
                        liste.Add(new { kod = "", ad = "" });
                    }
                }
                return Results.Json(new { birimler = liste });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Birimler okunamadı."));
            }
        }

        // ------------------------------------------------------------------ hesap kodu (eski HesapStore_Refresh / HesapListesiDoldur)

        private static int HesapYil(int? yil)
        {
            int y = yil ?? 0;
            return y <= 2000 ? DateTime.Now.Year : y;
        }

        private static object HesapDugum(HesapPlaniSatir h)
        {
            return new { kod = h.hesapKod, ad = h.hesapKod + " - " + h.aciklama, hesapAd = h.aciklama ?? "", altVar = !h.detay, detay = h.detay };
        }

        /// <summary>Eski HesapListesiDoldur: hesap kodu ya da adında geçen son düzey hesaplar.</summary>
        private static IResult HesapAra(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string metin, string sonKirilim)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            metin = Temiz(metin);
            if (metin.Length < 2) return Results.Json(new { satirlar = new List<object>() });
            try
            {
                HesapPlaniSatir h = new HesapPlaniSatir();
                h.yil = HesapYil(yil);
                h.hesapKodHesapAd = metin;
                h.detay = sonKirilim != "0";
                ObjectArray hesap = Servis().HesapPlaniListele(kullanan, h);
                if (hesap == null || !hesap.sonuc.islemSonuc) return Hata(HataMetni(hesap?.sonuc.hataStr, "Arama yapılamadı."));
                List<object> satirlar = new List<object>();
                foreach (HesapPlaniSatir d in hesap.objeler)
                {
                    satirlar.Add(HesapDugum(d));
                    if (satirlar.Count >= 500) break;
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Arama yapılamadı.");
            }
        }

        /// <summary>Elle yazılan hesap kodunun adı ("kod-ad" biçimine getirmek için).</summary>
        private static IResult HesapAd(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string kod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            kod = Temiz(kod).Split('-')[0].Trim();
            if (kod == "") return Results.Json(new { kod = "", ad = "", detay = false });
            try
            {
                int y = HesapYil(yil);
                string hesYapi = TNS.MUH.Arac.DegiskenDegerBul(y, "/KODYAPI/KOD8/KODYAPI");
                if (!string.IsNullOrEmpty(hesYapi)) kod = OrtakFonksiyonlar.KodNoktaKoy(hesYapi, kod.Replace(".", ""));
                HesapPlaniSatir h = new HesapPlaniSatir();
                h.yil = y;
                h.hesapKod = kod;
                ObjectArray hv = Servis().HesapPlaniListele(kullanan, h);
                if (hv != null && hv.objeler != null)
                    foreach (HesapPlaniSatir d in hv.objeler)
                        if (d.hesapKod == kod)
                            return Results.Json(new { kod = d.hesapKod, ad = d.aciklama ?? "", detay = d.detay });
                return Results.Json(new { kod = kod, ad = "", detay = false });
            }
            catch (Exception)
            {
                return Hata("Hesap kodu okunamadı.");
            }
        }

        /// <summary>Hesap planı penceresinin (Sgb.hesapPlaniSec) ağacı: düzey düzey.</summary>
        private static IResult HesapAgac(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string ust)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici _);
            if (ret != null) return ret;
            try
            {
                int y = HesapYil(yil);
                ust = Temiz(ust);
                List<object> dugumler = new List<object>();
                if (ust == "")
                {
                    TNS.UZY.IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
                    for (int i = 1; i < 10; i++)
                    {
                        string ad = servisUZY.UzayDegeriStr(new Kullanici(), "MUHHESAPPLANI", y.ToString() + "-" + i.ToString(), true);
                        dugumler.Add(new { kod = i.ToString(), ad = i.ToString() + "-" + ad, hesapAd = ad ?? "", altVar = true, detay = false });
                    }
                    return Results.Json(new { dugumler = dugumler });
                }

                string[] hesapKirilimlar = ust.Split('.');
                HesapPlaniSatir hs = new HesapPlaniSatir();
                hs.yil = y;
                hs.hesapKod = ust;
                if (ust.Length < 3) hs.seviye = 1;
                else hs.seviye = hesapKirilimlar.Length + 1;

                IMUHServis servisMUH = Servis();
                ObjectArray hv = new ObjectArray();
                for (int i = 0; i < 10; i++)
                {
                    hv = servisMUH.HesapPlaniListele(new Kullanici(), hs);
                    if (hv.objeler.Count == 0) hs.seviye++;
                    else break;
                }
                foreach (HesapPlaniSatir h in hv.objeler) dugumler.Add(HesapDugum(h));
                return Results.Json(new { dugumler = dugumler });
            }
            catch (Exception)
            {
                return Hata("Hesap planı okunamadı.");
            }
        }

        // ------------------------------------------------------------------ belge arama (eski MuhasebatAktarimSorguTCCB.Liste_Click)

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int? yil, int? durum, int? muhasebe, string birim,
            string belgeNo1, string belgeNo2, string belgeTarihi1, string belgeTarihi2)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            OdemeEmriMIFForm f = new OdemeEmriMIFForm();
            f.yil = yil ?? 0;
            f.formDurum = durum ?? 0;

            if (f.yil == 0 || f.formDurum == 0)
                return Hata("Sorgulama için yıl alanını boş bırakamazsınız.");

            try
            {
                f.kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
                f.muhasebe = muhasebe ?? 0;
                f.birim = Temiz(birim);

                belgeNo1 = Temiz(belgeNo1);
                belgeNo2 = Temiz(belgeNo2);
                if (belgeNo1 != "")
                    f.belgeNo = belgeNo1 + ";";
                if (belgeNo2 != "")
                    f.belgeNo += belgeNo2;

                DateTime? t1 = TarihCoz(belgeTarihi1);
                DateTime? t2 = TarihCoz(belgeTarihi2);
                if (t1.HasValue)
                    f.sorguIslemTarih = new TNSDateTime(t1.Value).ToString() + ";";
                if (t2.HasValue)
                    f.sorguIslemTarih += new TNSDateTime(t2.Value).ToString();

                ObjectArray bilgiler = Servis().EntegrasyonMIFListele(kullanan, f, false);
                if (bilgiler != null && !bilgiler.sonuc.islemSonuc && bilgiler.objeler.Count == 0 && !string.IsNullOrWhiteSpace(bilgiler.sonuc.hataStr))
                    return Hata(bilgiler.sonuc.hataStr);

                List<object> liste = new List<object>();
                foreach (OdemeEmriMIFForm item in bilgiler.objeler)
                {
                    liste.Add(new
                    {
                        belgeNo = item.belgeNo,
                        belgeTur = item.belgeTur,
                        belgeTarih = TarihYaz(item.islemTarih),
                        muhasebe = item.muhasebe,
                        birim = item.birim,
                        borc = item.borcToplam,
                        alacak = item.alacakToplam,
                        durum = DurumDonustur(item.formDurum),
                        aciklama = item.aciklama,
                        islemYapan = item.islemYapan,
                        ettn = item.ettn
                    });
                }
                return Results.Json(new { basarili = true, yil = f.yil, satirlar = liste });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Liste okunamadı."));
            }
        }

        /// <summary>Belge tarihçesi (eski BelgeTarihce.aspx?belgeTur=999).</summary>
        private static IResult Tarihce(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string belgeNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            belgeNo = Temiz(belgeNo);
            if ((yil ?? 0) == 0 || belgeNo == "") return Hata("Yıl ve/ya Belge No bilgisi alınamadı.");
            try
            {
                ObjectArray tVAN = Servis().TarihceListele(kullanan, yil.Value, (ENUMBelgeTuru)TarihceBelgeTur, belgeNo);
                if (!tVAN.sonuc.islemSonuc) return Hata(HataMetni(tVAN.sonuc.hataStr, "Tarihçe okunamadı."));
                List<object> satirlar = new List<object>();
                int sira = 0;
                foreach (TarihceBilgisi t in tVAN.objeler)
                {
                    sira++;
                    satirlar.Add(new { sira = sira, islemTarih = t.islemTarih.ToLongString(), islem = t.islem, islemYapan = t.islemYapan, islemYapanAd = t.islemYapanAd });
                }
                return Results.Json(new { basarili = true, belge = yil + " / " + belgeNo, satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Tarihçe okunamadı."));
            }
        }
    }
}
