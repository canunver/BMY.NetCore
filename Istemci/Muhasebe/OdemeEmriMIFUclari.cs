using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
using TNS.BKD;
using TNS.KYM;
using TNS.MUH;
using TNS.Raporlama;

namespace Istemci.Muhasebe
{
    /// <summary>Ödeme emri / MİF tablosunun bir satırı (eski StoreListe kayıt alanları; adları aynı).</summary>
    public class OemSatir : Dictionary<string, JsonElement> { }

    public class OemKaydetIstegi
    {
        public int Yil { get; set; }
        public string BelgeTarih { get; set; }
        public string BelgeNo { get; set; }
        public int Muhasebe { get; set; }
        public string Birim { get; set; }
        public string Kurumsal { get; set; }
        public bool Baslik { get; set; }
        public string IlgiliAd { get; set; }
        public string IlgiliNo { get; set; }
        public string IlgiliVD { get; set; }
        public string IlgiliBankaAd { get; set; }
        public string IlgiliBankaNo { get; set; }
        public string Aciklama { get; set; }
        public int Ist1 { get; set; }
        public int Ist2 { get; set; }
        public string OEBTur { get; set; }
        public string OEBNo { get; set; }
        public string OEBTarih { get; set; }
        public string OEBTutar { get; set; }
        public string IOETarih { get; set; }
        public string IOEYevNo { get; set; }
        public string IOEKim { get; set; }
        public string AVKTarih { get; set; }
        public string AVKYevNo { get; set; }
        public string AVKTutar { get; set; }
        public string YEVVerNo { get; set; }
        public string YEVCekNo { get; set; }
        public string YEVCekTarih { get; set; }
        public string ProjeNo { get; set; }
        public string IhaleKod { get; set; }
        public string AlimTur { get; set; }
        public int FisTur { get; set; }
        public string IlgiliBelgeNo { get; set; }
        public string SurecNo { get; set; }
        /// <summary>false: kullanıcı mükerrer belge uyarısını onayladı (eski hdnMukerrerBelgeKontrol).</summary>
        public bool MukerrerKontrol { get; set; } = true;
        /// <summary>OYBS'de seçilen kayıtlar "tip-id;tip-id;" (eski hdnOybs).</summary>
        public string Oybs { get; set; }
        public List<OemSatir> Satirlar { get; set; }
    }

    public class OemDurumIstegi
    {
        public int Yil { get; set; }
        public string BelgeNo { get; set; }
        /// <summary>onayla, onaykaldir, iptal, yevver, yevkaldir</summary>
        public string Islem { get; set; }
        public string SurecNo { get; set; }
        public string YevmiyeNo { get; set; }
        public string YevmiyeAciklama { get; set; }
        /// <summary>Yevmiye tarihi belge tarihiyle aynı (eski chkIslemYevmiye).</summary>
        public bool BelgeTarihi { get; set; }
        public string YevmiyeTarih { get; set; }
        public string Oybs { get; set; }
    }

    public class OemBelgeIstegi
    {
        public int Yil { get; set; }
        public string BelgeNo { get; set; }
        public string SurecNo { get; set; }
        public int Muhasebe { get; set; }
        public string Birim { get; set; }
        public string IlgiliBelgeNo { get; set; }
        public int SiraNo { get; set; }
        public string AltHesapKod { get; set; }
    }

    public class OemSablonIstegi
    {
        public int Yil { get; set; }
        public string Kod { get; set; }
        public string Ad { get; set; }
        /// <summary>1 yalnız ben, 2 muhasebe birimi (eski grpIzin).</summary>
        public int Izin { get; set; }
        /// <summary>Var olan şablonun üzerine yaz (eski grpKaydet2).</summary>
        public bool Uzerine { get; set; }
        public int Muhasebe { get; set; }
        public string Birim { get; set; }
        public string Kurumsal { get; set; }
        public string IlgiliAd { get; set; }
        public string IlgiliNo { get; set; }
        public string IlgiliVD { get; set; }
        public string IlgiliBankaAd { get; set; }
        public string IlgiliBankaNo { get; set; }
        public string Aciklama { get; set; }
        public List<OemSatir> Satirlar { get; set; }
    }

    public class OemKurFarkiIstegi
    {
        public int Yil { get; set; }
        public string KurTarih { get; set; }
        public string HesapKod { get; set; }
        public string Aciklama { get; set; }
        public string BelgeNo { get; set; }
        public bool MuhasebeBirimineIsle { get; set; }
    }

    public class OemYansitmaIstegi
    {
        public int Yil { get; set; }
        public List<OemSatir> Satirlar { get; set; }
        public List<OemSatir> Yansitmalar { get; set; }
    }

    /// <summary>
    /// Ödeme Emri / MİF belgesi (eski ButceMuhasebe/OdemeEmriMIFFrame.aspx, "Belge Düzenleme" sekmesi OdemeEmriMIF2.aspx).
    /// Menüde üç seçenek bu çerçeveyi açar: düz, ?yeni=1 ve ?yeni=1&amp;baslikSayfasi=1 (yalnız başlık bilgileri). Yetki bu üç adresten
    /// birinin menü yetkisinden ve eski TNS.MUH.Yetki (ödeme emri görme) denetiminden gelir; alt sayfalar da aynı yetkiyi kullanır.
    ///   GET  tanim, belge, yazdir, hizli-rapor, doviz-kur, taahhut, detay-birimler, oybs/liste, dokuman/liste, dokuman/indir,
    ///        hesap/agac, hesap/ara, hesap/ad, althesap/ara, firma/ara, sablon/liste, sablon/ac
    ///   POST kaydet, durum, surec-bag, ilgili-belge, alt-hesap, sablon/kaydet, sablon/sil, kur-farki,
    ///        yansitma/liste, yansitma/yap, dosyadan-yukle (multipart), dokuman/yukle (multipart), dokuman/sil
    /// Belge arama uçları: OdemeEmriMIFSorguUclari.
    /// </summary>
    public static class OdemeEmriMIFUclari
    {
        public const string FrameAdres = "../ButceMuhasebe/OdemeEmriMIFFrame.aspx";
        public static readonly string[] MenuAdresleri = { FrameAdres, FrameAdres + "?yeni=1", FrameAdres + "?yeni=1&baslikSayfasi=1" };
        private const string Uc = "/api/odemeemrimif/";
        public const int BosSatirSayisi = 20;

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "tanim", Tanim);
            app.MapGet(Uc + "belge", Belge);
            app.MapPost(Uc + "kaydet", Kaydet);
            app.MapPost(Uc + "durum", Durum);
            app.MapGet(Uc + "yazdir", Yazdir);
            app.MapGet(Uc + "hizli-rapor", HizliRapor);
            app.MapGet(Uc + "seyyar", Seyyar);
            app.MapPost(Uc + "surec-bag", SurecBag);
            app.MapPost(Uc + "ilgili-belge", IlgiliBelge);
            app.MapPost(Uc + "alt-hesap", AltHesap);
            app.MapGet(Uc + "doviz-kur", DovizKur);
            app.MapGet(Uc + "taahhut", Taahhut);
            app.MapGet(Uc + "detay-birimler", DetayBirimler);

            app.MapGet(Uc + "hesap/agac", HesapAgac);
            app.MapGet(Uc + "hesap/ara", HesapAra);
            app.MapGet(Uc + "hesap/ad", HesapAd);
            app.MapGet(Uc + "althesap/ara", AltHesapAra);
            app.MapGet(Uc + "firma/ara", FirmaAra);

            app.MapGet(Uc + "sablon/liste", SablonListe);
            app.MapGet(Uc + "sablon/ac", SablonAc);
            app.MapPost(Uc + "sablon/kaydet", SablonKaydet);
            app.MapPost(Uc + "sablon/sil", SablonSil);
            app.MapPost(Uc + "kur-farki", KurFarki);

            app.MapPost(Uc + "yansitma/liste", YansitmaListe);
            app.MapPost(Uc + "yansitma/yap", YansitmaYap);
            app.MapPost(Uc + "dosyadan-yukle", DosyadanYukle).DisableAntiforgery();

            app.MapGet(Uc + "oybs/liste", OybsListe);

            app.MapGet(Uc + "dokuman/liste", DokumanListe);
            app.MapGet(Uc + "dokuman/indir", DokumanIndir);
            app.MapPost(Uc + "dokuman/yukle", DokumanYukle).DisableAntiforgery();
            app.MapPost(Uc + "dokuman/sil", DokumanSil);
        }

        // ------------------------------------------------------------------ yetki ve ortak

        /// <summary>Menü yetkisi (üç adresten biri) ve eski sayfa girişi denetimi (ödeme emri görme yetkisi).</summary>
        public static bool Yetkili(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (!SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, null, MenuAdresleri)) return false;
            try
            {
                return TNS.MUH.Yetki.YetkisiVarMi(kullanan, (int)ENUMOGY.Gorme, (int)ENUMBelgeTuru.ODEMEEMRI);
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!Yetkili(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IMUHServis Servis() { return TNS.MUH.Arac.Tanimla(); }

        internal static bool Yonetici(Kullanici kullanan)
        {
            return kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
        }

        internal static IResult Hata(string hata) { return Results.Json(new { hata = Duz(hata) }); }

        internal static IResult Basari(string bilgi) { return Results.Json(new { basarili = true, bilgi = Duz(bilgi) }); }

        internal static IResult Basarisiz(string hata) { return Results.Json(new { basarili = false, hata = Duz(hata) }); }

        internal static string Temiz(string s) { return (s ?? "").Trim(); }

        /// <summary>Eski mesajlardaki HTML (&lt;br&gt;, &lt;b&gt;, &lt;font&gt;) düz metne çevrilir.</summary>
        internal static string Duz(string s)
        {
            if (string.IsNullOrEmpty(s)) return s ?? "";
            s = Regex.Replace(s, @"<br\s*/?>", "\n", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, "<[^>]+>", "");
            s = System.Net.WebUtility.HtmlDecode(s);
            return s.Trim('\n', ' ');
        }

        internal static string HataMetni(string hata, string varsayilan)
        {
            hata = Temiz(hata);
            return hata == "" ? varsayilan : hata;
        }

        /// <summary>yyyy-MM-dd -> TNSDateTime (boşsa boş tarih).</summary>
        internal static TNSDateTime Tarih(string s)
        {
            s = Temiz(s);
            if (s != "" && DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime t))
                return new TNSDateTime(t);
            return new TNSDateTime();
        }

        internal static string TarihYaz(TNSDateTime t)
        {
            if (t == null || t.isNull) return "";
            DateTime d = t.Oku();
            return d.Year <= 1900 ? "" : d.ToString("yyyy-MM-dd");
        }

        internal static string Kurum()
        {
            return GenelIslemlerIstemci.VarsayilanKurumBul() ?? "";
        }

        /// <summary>
        /// Belge numarası (eski btnListele_Click): raporlardan yapıştırılan "460604001-MIF" gibi metinlerin ilk parçası alınır;
        /// 9 haneden kısa numaranın başına kurum kodu (4 hane) eklenir, numara 6 haneye tamamlanır.
        /// </summary>
        public static string BelgeNoDuzenle(string belgeNo)
        {
            belgeNo = Temiz(belgeNo);
            string[] parca = belgeNo.Split('-', ' ', '_');
            if (parca.Length > 0) belgeNo = parca[0];
            if (belgeNo != "" && belgeNo.Length < 9)
                belgeNo = Kurum().Replace(".", "").PadRight(4, '0') + belgeNo.PadLeft(6, '0');
            return belgeNo;
        }

        /// <summary>Eski OdemeEmriMIF2.SurecAta: "yil/no" ya da "no"; numara 6 haneye tamamlanır.</summary>
        public static void SurecAta(string surecText, out int surecYil, out string surecNo)
        {
            surecYil = 0;
            surecNo = "";
            if (string.IsNullOrWhiteSpace(surecText)) return;
            string[] surecParca = surecText.Split('/');
            if (surecParca.Length == 2)
            {
                surecYil = OrtakFonksiyonlar.ConvertToInt(surecParca[0], 0);
                surecNo = surecParca[1];
            }
            else if (surecParca.Length == 1)
            {
                surecYil = 0;
                surecNo = surecParca[0];
            }
            if (surecNo.Length < 6) surecNo = surecNo.PadLeft(6, '0');
        }

        public static void SurecAta(OdemeEmriMIFForm form, string surecText)
        {
            SurecAta(surecText, out form.surecYil, out form.surecNo);
        }

        /// <summary>Eski OdemeEmriMIFSorgu.DurumDonustur.</summary>
        public static string DurumDonustur(int durum)
        {
            if (durum == (int)ENUMBelgeDurumu.YENI) return "Yeni";
            if (durum == (int)ENUMBelgeDurumu.DEGISTIRILDI) return "Değiştirildi";
            if (durum == (int)ENUMBelgeDurumu.ONAYLI) return "Onaylı";
            if (durum == (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER) return "Yevmiyeli";
            if (durum == (int)ENUMBelgeDurumu.IPTAL) return "İptal";
            return "Belirsiz - " + durum;
        }

        internal static string BelgeTurAd(ENUMBelgeTuru belgeTur)
        {
            switch (belgeTur)
            {
                case ENUMBelgeTuru.ODEMEEMRI: return "Ödeme Emri";
                case ENUMBelgeTuru.MIF: return "Muhasebe İşlem Fişi";
                case ENUMBelgeTuru.ODENEKGONDERME: return "Ödenek Gönderme";
                case ENUMBelgeTuru.TENKIS: return "Tenkis";
                case ENUMBelgeTuru.ODEMEEMRI_YUKSEK: return "Ödeme Emri (Yüksek)";
                case ENUMBelgeTuru.MIF_YUKSEK: return "Muhasebe İşlem Fişi (Yüksek)";
                case ENUMBelgeTuru.ODENEKGONDERME_YUKSEK: return "Ödenek Gönderme (Yüksek)";
                case ENUMBelgeTuru.TENKIS_YUKSEK: return "Tenkis (Yüksek)";
                default: return "";
            }
        }

        // ------------------------------------------------------------------ satır alanları (eski VeriAlStr / VeriAlDbl / VeriAlInt)

        internal static string S(OemSatir r, string ad)
        {
            if (r == null || !r.TryGetValue(ad, out JsonElement e)) return "";
            switch (e.ValueKind)
            {
                case JsonValueKind.String: return e.GetString() ?? "";
                case JsonValueKind.Number: return e.GetRawText();
                case JsonValueKind.True: return "true";
                case JsonValueKind.False: return "false";
                default: return "";
            }
        }

        internal static double D(OemSatir r, string ad)
        {
            if (r == null || !r.TryGetValue(ad, out JsonElement e)) return 0;
            if (e.ValueKind == JsonValueKind.Number) return e.GetDouble();
            if (e.ValueKind == JsonValueKind.String)
            {
                string s = (e.GetString() ?? "").Trim();
                if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out double d)) return d;
                return OrtakFonksiyonlar.ConvertToDbl(s);
            }
            return 0;
        }

        internal static int I(OemSatir r, string ad)
        {
            if (r == null || !r.TryGetValue(ad, out JsonElement e)) return 0;
            if (e.ValueKind == JsonValueKind.Number) return e.TryGetInt32(out int i) ? i : (int)e.GetDouble();
            if (e.ValueKind == JsonValueKind.True) return 1;
            if (e.ValueKind == JsonValueKind.String) return OrtakFonksiyonlar.ConvertToInt((e.GetString() ?? "").Trim(), 0);
            return 0;
        }

        /// <summary>Eski DetayDoldur: tablodaki dolu satırlardan belge detayları.</summary>
        internal static ObjectArray DetayDoldur(int yil, List<OemSatir> satirlar)
        {
            ObjectArray pler = new ObjectArray();
            foreach (OemSatir row in satirlar ?? new List<OemSatir>())
            {
                OdemeEmriMIFDetay p = new OdemeEmriMIFDetay(yil);
                p.hesapKod = S(row, "HESAPKOD");
                if (string.IsNullOrEmpty(p.hesapKod))
                    continue;
                p.kesinti = I(row, "KESINTI");
                p.satirNo = I(row, "SIRANO");

                string[] kod = p.hesapKod.Split('-');
                p.hesapKod = kod[0].Trim();
                if (kod.Length > 1)
                    p.aciklama = kod[1];

                p.altHesapKod = S(row, "ALTHESAPKOD");
                if (!string.IsNullOrEmpty(p.altHesapKod))
                {
                    string[] altHler = p.altHesapKod.Split('-');
                    p.altHesapKod = altHler[0].Trim();
                    if (altHler.Length > 1)
                        p.altHesapAd = altHler[1].Trim();
                }

                string tertip = S(row, "TERTIP");
                if (!string.IsNullOrEmpty(tertip))
                    TNS.MUH.Arac.TertipAta(ref p, tertip);

                p.borc = Math.Round(D(row, "BORC"), 2);
                p.alacak = Math.Round(D(row, "ALACAK"), 2);
                p.doviz = S(row, "DOVIZ");
                p.dovizKurTur = S(row, "DOVIZKURTUR");
                p.satirAciklama = S(row, "SATIRACIKLAMA");
                p.dovizBorc = D(row, "DOVIZBORC");
                p.dovizAlacak = D(row, "DOVIZALACAK");
                p.dovizKur = (decimal)D(row, "DOVIZKUR");
                p.detayBirim = S(row, "DETAYBIRIM");
                p.mahsupBelgeNo = S(row, "MAHSUPBELGENO");
                p.isKod = I(row, "ISKOD");

                pler.objeler.Add(p);
            }
            return pler;
        }

        /// <summary>Tablonun boş satırı (eski ListeTemizle).</summary>
        internal static Dictionary<string, object> BosSatir(int sira)
        {
            return new Dictionary<string, object>
            {
                ["SIRANO"] = sira, ["KESINTI"] = 0, ["HESAPKOD"] = "", ["HESAPAD"] = "", ["TERTIP"] = "", ["TERTIPPRG"] = "",
                ["BORC"] = 0, ["ALACAK"] = 0, ["ALTHESAPKOD"] = "", ["SATIRACIKLAMA"] = "", ["DOVIZ"] = "", ["DOVIZKURTUR"] = "",
                ["DOVIZBORC"] = 0, ["DOVIZALACAK"] = 0, ["DOVIZKUR"] = 0, ["MAHSUPBELGENO"] = "", ["DETAYBIRIM"] = "", ["ISKOD"] = 0, ["ISAD"] = ""
            };
        }

        internal static Dictionary<string, object> Satir(int kesinti, string hesapKod, string hesapAd, string tertip, string tertipPrg, double borc, double alacak,
            string altHesapKod, string satirAciklama, string doviz, string dovizKurTur, double dovizBorc, double dovizAlacak, decimal dovizKur,
            string mahsupBelgeNo, string detayBirim, object isKod, string isAd)
        {
            return new Dictionary<string, object>
            {
                ["SIRANO"] = 0, ["KESINTI"] = kesinti, ["HESAPKOD"] = hesapKod ?? "", ["HESAPAD"] = hesapAd ?? "", ["TERTIP"] = tertip ?? "",
                ["TERTIPPRG"] = tertipPrg ?? "", ["BORC"] = borc, ["ALACAK"] = alacak, ["ALTHESAPKOD"] = altHesapKod ?? "",
                ["SATIRACIKLAMA"] = satirAciklama ?? "", ["DOVIZ"] = doviz ?? "", ["DOVIZKURTUR"] = dovizKurTur ?? "", ["DOVIZBORC"] = dovizBorc,
                ["DOVIZALACAK"] = dovizAlacak, ["DOVIZKUR"] = dovizKur, ["MAHSUPBELGENO"] = mahsupBelgeNo ?? "", ["DETAYBIRIM"] = detayBirim ?? "",
                ["ISKOD"] = isKod ?? 0, ["ISAD"] = isAd ?? ""
            };
        }

        /// <summary>Satırları sırayla numaralar, sona boş satırlar ekler (eski 20 boş satır).</summary>
        internal static List<Dictionary<string, object>> Numarala(List<Dictionary<string, object>> liste, bool bosEkle = true)
        {
            int sira = 1;
            foreach (Dictionary<string, object> s in liste) s["SIRANO"] = sira++;
            if (bosEkle)
                for (int i = 0; i < BosSatirSayisi; i++) liste.Add(BosSatir(sira++));
            return liste;
        }

        /// <summary>Kullanıcı değişkenleri (eski DegiskenleriKaydet).</summary>
        private static void DegiskenleriKaydet(Kullanici kullanan, int muhasebe, string birim, string kurumsal)
        {
            try
            {
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "MUHASEBE", muhasebe > 0 ? muhasebe.ToString() : "");
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "MUHASEBEBIRIM", birim ?? "");
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "MUHASEBEKURUMSAL", kurumsal ?? "");
            }
            catch (Exception) { }
        }

        private static string KodlarStr(Kullanici kullanan, int yil, ENUMHesapKodNitelik nitelik, int seviye)
        {
            string s = "";
            HesapPlaniSatir hK = new HesapPlaniSatir();
            hK.yil = yil;
            hK.seviye = seviye;
            hK.nitelik = ((int)nitelik).ToString();
            ObjectArray hV = Servis().HesapPlaniListele(kullanan, hK);
            if (hV != null && hV.sonuc.islemSonuc)
                foreach (HesapPlaniSatir h in hV.objeler)
                    s += "|" + h.hesapKod.Replace(".", "") + "|";
            return s;
        }

        // ------------------------------------------------------------------ tanım (eski Page_Load)

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                IMUHServis servisMUH = Servis();
                int yil = DateTime.Now.Year;
                string kurum = Kurum();
                bool yonetici = Yonetici(kullanan);

                // fiş türleri (eski ButceMuhasebeGenel.FisTurDoldur)
                List<object> fisTurler = new List<object>();
                foreach (ENUMFISTUR f in new[] { ENUMFISTUR.DONEMICI, ENUMFISTUR.ACILIS, ENUMFISTUR.KAPANIS, ENUMFISTUR.AMORTISMANKAPANISI, ENUMFISTUR.SONFIS })
                    fisTurler.Add(new { kod = (int)f, ad = TNS.MUH.Arac.FisTurAdBul(f) });

                // ihale türleri (eski IhaleKodDoldur)
                List<object> ihaleler = new List<object>();
                try
                {
                    ObjectArray oa = TNS.HRC.Arac.Tanimla().IhaleTurMaddeListele(kullanan, 0);
                    foreach (TNS.HRC.IhaleTurMadde item in oa.objeler)
                    {
                        string tut = item.aciklama ?? "";
                        if (tut.Length > 60) tut = tut.Substring(0, 60);
                        ihaleler.Add(new { kod = (item.adi ?? "").Replace(".", ""), ad = item.adi + " " + tut });
                    }
                }
                catch (Exception) { }

                // istatistik kodları
                object Istatistik(int kod)
                {
                    IstatistikKod k = servisMUH.OkuIstatistikKodGiris(kullanan, new IstatistikKod() { kod = kod }, true, false);
                    if (k == null) return null;
                    List<object> degerler = new List<object>();
                    if (k.istatistikKodDeger != null)
                        foreach (IstatistikKodDeger d in k.istatistikKodDeger)
                            degerler.Add(new { kod = d.degerKod, ad = d.aciklama ?? "" });
                    return new { ad = k.ad ?? "", degerler = degerler };
                }

                // döviz listesi (eski DovizDoldur: aktif para birimi hariç)
                List<object> dovizler = new List<object>();
                bool dovizVar = TNS.MUH.Arac.MuhasebeDovizVarMi();
                if (dovizVar)
                {
                    ObjectArray pl = TNS.HRC.Arac.Tanimla().ParaBirimiListele(new Kullanici(), new ParaBirimi());
                    foreach (ParaBirimi p in pl.objeler)
                    {
                        if (p.kod == 1) continue;
                        dovizler.Add(new { adi = p.adi ?? "", kisaltma = p.kisaltma ?? "" });
                    }
                }

                // KDV hesapları (eski KDVHesaplariBul: appSettings MuhasebeKDVHesabiGider/Gelir1..19)
                List<string> kdvler = new List<string>();
                for (int i = 1; i < 20; i++)
                {
                    string deger = Ayar.Oku("MuhasebeKDVHesabiGider" + i);
                    if (deger != "") kdvler.Add(deger.Replace(".", "") + "-" + i);
                    deger = Ayar.Oku("MuhasebeKDVHesabiGelir" + i);
                    if (deger != "") kdvler.Add(deger.Replace(".", "") + "-" + i);
                }

                // döviz hesapları (eski DovizHesaplariBul)
                string dovizHesabi = "";
                HesapPlaniSatir hD = new HesapPlaniSatir();
                hD.yil = yil;
                hD.dovizKurKod = "HEPSİ";
                ObjectArray hDV = servisMUH.HesapPlaniListele(kullanan, hD);
                if (hDV != null && hDV.sonuc.islemSonuc)
                    foreach (HesapPlaniSatir h in hDV.objeler)
                        dovizHesabi += "|" + h.hesapKod.Replace(".", "") + "|";

                // önceki muhasebe - birim (kullanıcı değişkenleri)
                int muhasebe = OrtakFonksiyonlar.ConvertToInt(GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBE"), 0);
                string birim = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBEBIRIM") ?? "";
                string kurumsal = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBEKURUMSAL") ?? "";
                if (kurumsal != "")
                    kurumsal = OrtakFonksiyonlar.KodNoktaKoy(TertipYapi.KodYapiBul(yil, 1, false), kurumsal.Replace(".", ""));
                if (birim.Trim() == "") kurumsal = "";

                return Results.Json(new
                {
                    yil = yil,
                    bugun = DateTime.Today.ToString("yyyy-MM-dd"),
                    kurum = kurum,
                    yonetici = yonetici,
                    tarim = TNS.DEG.Arac.VarsayilanKurumBul() == "0023",
                    prgBag = TNS.MUH.Arac.prgBag,
                    cumhurbaskanligi = TNS.HRC.Arac.CumhurbaskanligiMi(),
                    detayBirim = TNS.MUH.Arac.DetayBirimVarMi(),
                    kurumsalYapi = TertipYapi.KodYapiBul(yil, 1, false) ?? "",
                    altHesapGizle = TNS.MUH.Arac.AltHesapGosterme(),
                    isGizle = kurum.Replace(".", "") == "460604" || TNS.MUH.Arac.MuhasebeIsKolonuGizle(),
                    oybs = TNS.MUH.Arac.OfmEntegrasyonVarMi(),
                    tertipYok = TNS.MUH.Arac.MuhasebeTertipYokMu(),
                    satirAciklama = TNS.MUH.Arac.MuhasebeSatirAciklamaVarMi(),
                    mahsupGizle = TNS.MUH.Arac.MuhasebeMahsupKolonuGizle(),
                    doviz = dovizVar,
                    yazdirTercih = TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/ODEMIFYAZDIRTERCIH") == "1",
                    imzaKod = OrtakFonksiyonlar.ConvertToInt(GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBEIMZATANIMKOD"), 0),
                    fisTurler = fisTurler,
                    ihaleler = ihaleler,
                    ist1 = Istatistik(1),
                    ist2 = Istatistik(2),
                    dovizler = dovizler,
                    kasaHesabi = KodlarStr(kullanan, yil, ENUMHesapKodNitelik.KASAHESABI, 1),
                    avansHesabi = KodlarStr(kullanan, yil, ENUMHesapKodNitelik.AVANS, 1),
                    kdvHesabi = KodlarStr(kullanan, yil, ENUMHesapKodNitelik.KDV, 0),
                    yansitmaHesabi = KodlarStr(kullanan, yil, ENUMHesapKodNitelik.YANSITMAYAPILACAK, 1),
                    dovizHesabi = dovizHesabi,
                    kdvler = string.Join(";", kdvler),
                    muhasebe = muhasebe,
                    birim = birim,
                    kurumsal = kurumsal,
                    satirSayisi = BosSatirSayisi
                });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Sayfa bilgileri okunamadı."));
            }
        }

        // ------------------------------------------------------------------ belge (eski btnListele_Click + BelgeFormaYaz)

        private static object BelgeJson(OdemeEmriMIFForm form)
        {
            List<Dictionary<string, object>> liste = new List<Dictionary<string, object>>();
            foreach (OdemeEmriMIFDetay detay in form.detay.objeler)
            {
                string altHesapKod = detay.altHesapKod ?? "";
                if (altHesapKod != "") altHesapKod += " - " + detay.altHesapAd;
                liste.Add(Satir(detay.kesinti, detay.hesapKod + "-" + detay.aciklama, detay.aciklama, TNS.MUH.Arac.TertipStrOl(detay),
                    TNS.MUH.Arac.TertipStrOlPrg(detay), detay.borc, detay.alacak, altHesapKod, detay.satirAciklama, detay.doviz,
                    detay.dovizKurTur, detay.dovizBorc, detay.dovizAlacak, detay.dovizKur, detay.mahsupBelgeNo, detay.DetayBirim, detay.isKod, detay.isAdi));
            }

            ENUMFISTUR fisTur = form.fisTur != null && form.fisTur.Length > 0 ? form.fisTur[0] : ENUMFISTUR.DONEMICI;

            return new
            {
                yil = form.yil,
                belgeNo = form.belgeNo ?? "",
                belgeTur = (int)form.belgeTur,
                durum = form.formDurum,
                durumAd = DurumDonustur(form.formDurum),
                belgeTarih = TarihYaz(form.islemTarih),
                muhasebe = form.muhasebe,
                birim = form.birim ?? "",
                kurumsal = OrtakFonksiyonlar.KodNoktaKoy(TertipYapi.KodYapiBul(form.yil, 1, false), (form.kurKod ?? "").Replace(".", "")),
                birimAd = TNS.MUH.Arac.BirimAdOl(form.muhasebeAd, form.birimAd),
                ilgiliAd = form.ilgiliAd ?? "",
                ilgiliNo = form.ilgiliNo ?? "",
                ilgiliVD = form.ilgiliVD ?? "",
                ilgiliBankaAd = form.ilgiliBankaAd ?? "",
                ilgiliBankaNo = form.ilgiliBankaNo ?? "",
                aciklama = form.aciklama ?? "",
                ist1 = form.istKod1 != null ? form.istKod1.degerKod : 0,
                ist2 = form.istKod2 != null ? form.istKod2.degerKod : 0,
                OEBTur = form.OEBTur ?? "",
                OEBNo = form.OEBNo ?? "",
                OEBTarih = TarihYaz(form.OEBTarih),
                OEBTutar = form.OEBTutar,
                IOETarih = TarihYaz(form.IOETarih),
                IOEYevNo = form.IOEYevNo ?? "",
                IOEKim = form.IOEKim ?? "",
                AVKTarih = TarihYaz(form.AVKTarih),
                AVKYevNo = form.AVKYevNo ?? "",
                AVKTutar = form.AVKTutar,
                YEVYevTarih = TarihYaz(form.YEVYevTarih),
                YEVYevNo = form.YEVYevNo > 0 ? form.YEVYevNo.ToString() : "",
                YEVVerNo = form.YEVVerNo ?? "",
                YEVCekNo = form.YEVCekNo ?? "",
                YEVCekTarih = (form.YEVCekNo ?? "") != "" ? TarihYaz(form.YEVCekTarih) : "",
                projeNo = (form.projeNo ?? "").ToString(),
                ihaleKod = TNS.MUH.Arac.IhaleKodBul(form.ihale),
                alimTur = TNS.MUH.Arac.IhaleAlimturBul(form.ihale),
                fisTur = (int)fisTur,
                ilgiliBelgeNo = form.ilgiliBelgeNo ?? "",
                surecNo = form.surecYil > 0 && (form.surecNo ?? "") != "" ? form.surecYil + "/" + form.surecNo : "",
                yevmiyeAciklama = string.IsNullOrWhiteSpace(form.yevmiyeAciklama) ? (form.aciklama ?? "") : form.yevmiyeAciklama,
                satirlar = Numarala(liste)
            };
        }

        private static IResult Belge(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string belgeNo, string yevmiyeNo, string baslik)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            string hataStr = "";
            belgeNo = Temiz(belgeNo);
            string yevNo = belgeNo == "" ? Temiz(yevmiyeNo) : "";
            if (belgeNo == "" && yevNo == "")
                hataStr = "Belge listelemesi için, Belge No alanına görmek istediğiniz belgenin nosunu yazmanız gerekmektedir.";
            if ((yil ?? 0) == 0)
                hataStr += (hataStr != "" ? "\n" : "") + "Belge listelemesi için, Yıl alanına görmek istediğiniz belgenin yılını yazmanız gerekmektedir.";
            if (hataStr != "") return Hata(hataStr);

            belgeNo = BelgeNoDuzenle(belgeNo);
            try
            {
                ObjectArray fler = OdemeEmriMIFYazdirma.Listele(kullanan, Servis(), yil.Value, belgeNo, yevNo, baslik == "1");
                if (!fler.sonuc.islemSonuc) return Hata(HataMetni(fler.sonuc.hataStr, "Belge okunamadı."));
                if (fler.objeler.Count == 0 || ((OdemeEmriMIFForm)fler.objeler[0]).kurum == "")
                {
                    if (belgeNo != "") return Hata(string.Format("Görmek istediğiniz {0} nolu belge bulunamadı", belgeNo));
                    return Hata(string.Format("Görmek istediğiniz {0} yevmiye nolu belge bulunamadı.", yevNo));
                }
                return Results.Json(new { belge = BelgeJson((OdemeEmriMIFForm)fler.objeler[0]) });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Belge okunamadı."));
            }
        }

        // ------------------------------------------------------------------ kaydet (eski btnKaydet_Click)

        /// <summary>Eski KullanilmayacakHesaplar (kurum 12, 30, 5030; /MUHASEBE/KULLANILMAYACAKHESAPKONTROL=1 ise denetlenmez).</summary>
        private static string KullanilmayacakHesaplar(string hesapNo)
        {
            if (TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/KULLANILMAYACAKHESAPKONTROL") == "1") return "";

            string[] kurumlar = { "12", "30", "5030" };
            if (!kurumlar.Contains(Kurum())) return "";

            string[] hesaplar = { "030201", "030202",
                "03020301", "03020302", "03020303",
                "03020401", "03020402", "03020403",
                "03020501", "03020502", "03020503", "03020504", "03020505",
                "03020601", "03020602", "03020603", "03020604",
                "03020901",
                "03030101", "030302", "030303", "030304", "030305",
                "03050103", "03050104", "03050105", "03050108", "03050109",
                "03050401", "03050402", "03050403",
                "030505",
                "03060101",
                "03070101", "03070102", "03070103", "03070104",
                "03070201",
                "03070301", "03070302", "03070303", "03070304",
                "03080101", "03080201", "03080301",
                "060101", "060102", "060103", "060104",
                "06020101", "06020102", "06020103",
                "06030101", "060302", "060303", "060304",
                "060402",
                "060501", "060502", "060504", "060505", "060506", "060507",
                "060601", "060602", "060604", "060605", "060606",
                "060701", "060702", "060704", "060706", "060707",
                "060902",
                "0302",
                "03050202",
                "03050910",
                "03070101", "03070102", "03070190", "03070301", "03070302"};

            string hata = "";
            for (int i = 0; i < hesaplar.Length; i++)
            {
                if (hesapNo.IndexOf("830" + hesaplar[i]) > -1)
                    hata = "\nBu tertibe ait Ödeme Emri ve Diğer hazırlık belgelerini Harcama Yönetimi Modülünden Süreç oluşturarak gerçekleştirmeniz gerekmektedir.";
            }

            if (string.IsNullOrEmpty(hata) && hesapNo.StartsWith("161"))
            {
                if (!(hesapNo.StartsWith("161.18") || hesapNo.StartsWith("16118")))
                    hata = "\nBu tertibe ait Muhasebe İşlem Fişi ve Diğer hazırlık belgelerini Harcama Yönetimi Modülünden Süreç oluşturarak gerçekleştirmeniz gerekmektedir.";
            }
            return hata;
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, OemKaydetIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new OemKaydetIstegi();
            try
            {
                DegiskenleriKaydet(kullanan, istek.Muhasebe, Temiz(istek.Birim), Temiz(istek.Kurumsal));

                OdemeEmriMIFForm form = new OdemeEmriMIFForm();
                form.yil = istek.Yil;
                form.islemTarih = Tarih(istek.BelgeTarih);
                form.belgeNo = Temiz(istek.BelgeNo);
                form.kurum = Kurum();
                form.muhasebe = istek.Muhasebe;
                form.birim = Temiz(istek.Birim);
                form.kurKod = Temiz(istek.Kurumsal);

                string hata = "";
                double bakiye = 0;

                if (form.yil == 0)
                    hata = "Yıl bilgisi boş bırakılamaz.";
                if (form.muhasebe <= 0 || string.IsNullOrWhiteSpace(form.birim))
                    hata += "\nBirim bilgisi boş bırakılamaz";
                if (TNSDateTime.IsNull(form.islemTarih))
                    hata += "\nBelge Tarihi bilgisi boş bırakılamaz";
                else if (form.islemTarih.Yil != form.yil)
                    hata += "\n" + string.Format("İşlem yapılan Yıl ({0}) ile belge tarihi ({1}) arasında uyuşmazlık var.", form.yil, form.islemTarih.ToString());

                ObjectArray pler = DetayDoldur(form.yil, istek.Satirlar);
                // boş hesap kodlu ama tutarlı satır var mı (eski FRMOEM005; DetayDoldur boş hesap kodlu satırları atlar)
                int sira = 0;
                foreach (OemSatir r in istek.Satirlar ?? new List<OemSatir>())
                {
                    sira++;
                    if (S(r, "HESAPKOD").Trim() == "" && (D(r, "BORC") != 0 || D(r, "ALACAK") != 0))
                        return Basarisiz(string.Format("Belgenin detayında hesap kodu girilmediği halde Borç veya Alacak alanlarına rakam girilmiş. Satır : {0}", sira));
                }

                bool bulundu = false;
                foreach (OdemeEmriMIFDetay p in pler.objeler)
                {
                    string hesapNo = p.hesapKod.Replace(".", "").Trim();
                    if (form.belgeNo == "" && form.yil == DateTime.Now.Year)
                        hata += KullanilmayacakHesaplar(hesapNo);

                    if (hesapNo != "")
                    {
                        bulundu = true;
                        bakiye += p.borc - p.alacak;
                        bakiye = Math.Round(bakiye, 2);
                        if (p.borc > 0 && p.alacak > 0)
                            hata += "\n" + p.hesapKod + " koduna ait satırda, Borç: " + p.borc + " Alacak: " + p.alacak + " - Bir belge detayında aynı satırda hem borç hemde alacak alanına rakam yazılamaz.";
                    }
                }

                if (istek.Baslik) bulundu = true;

                if (!bulundu)
                    return Basarisiz("Belgenin detayında kayıt edilecek herhangi bir bilgi bulunamadı.");
                else if (bakiye > 0.0)
                    hata += "\nBakiye:" + bakiye + "-BORÇ tutarı ile ALACAK tutarı birbirine eşit değil.";

                if (hata != "") return Basarisiz(hata.Trim('\n'));

                form.ilgiliAd = istek.IlgiliAd ?? "";
                form.ilgiliNo = istek.IlgiliNo ?? "";
                form.ilgiliVD = istek.IlgiliVD ?? "";
                form.ilgiliBankaAd = istek.IlgiliBankaAd ?? "";
                form.ilgiliBankaNo = istek.IlgiliBankaNo ?? "";
                form.aciklama = istek.Aciklama ?? "";
                form.istKod1.degerKod = istek.Ist1;
                form.istKod2.degerKod = istek.Ist2;

                form.OEBTur = istek.OEBTur ?? "";
                form.OEBNo = istek.OEBNo ?? "";
                form.OEBTarih = Tarih(istek.OEBTarih);
                form.OEBTutar = OrtakFonksiyonlar.ConvertToDbl(istek.OEBTutar);
                form.IOETarih = Tarih(istek.IOETarih);
                form.IOEYevNo = istek.IOEYevNo ?? "";
                form.IOEKim = istek.IOEKim ?? "";
                form.AVKTarih = Tarih(istek.AVKTarih);
                form.AVKYevNo = istek.AVKYevNo ?? "";
                form.AVKTutar = OrtakFonksiyonlar.ConvertToDbl(istek.AVKTutar);
                form.YEVVerNo = istek.YEVVerNo ?? "";
                form.YEVCekNo = istek.YEVCekNo ?? "";
                if (form.YEVCekNo != "")
                    form.IOETarih = Tarih(istek.YEVCekTarih); // eski kodda çek tarihi IOETarih alanına yazılıyordu

                form.islemYapan = kullanan.kullaniciKodu;
                form.islemYapanAd = kullanan.ad + " " + kullanan.soyad;
                form.projeNo = istek.ProjeNo ?? "";
                string ihale = Temiz(istek.IhaleKod).Replace(".", "");
                if (ihale != "" && Temiz(istek.AlimTur) != "") ihale += Temiz(istek.AlimTur);
                form.ihale = ihale;
                form.fisTurYap((ENUMFISTUR)istek.FisTur);
                form.ilgiliBelgeNo = istek.IlgiliBelgeNo ?? "";
                SurecAta(form, istek.SurecNo);

                if (istek.Baslik) form.sadeceUstVeri = true;

                foreach (OdemeEmriMIFDetay p in pler.objeler)
                {
                    OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(form.yil);
                    string hesapNo = p.hesapKod;
                    if (string.IsNullOrEmpty(hesapNo)) continue;

                    string tertip = TNS.MUH.Arac.TertipStrOl(p);
                    if (!string.IsNullOrEmpty(tertip))
                        TNS.MUH.Arac.TertipAta(ref detay, tertip);

                    detay.kesinti = p.kesinti;
                    detay.hesapKod = hesapNo.Replace(".", "").Trim();
                    detay.borc = Math.Round(p.borc, 2);
                    detay.alacak = Math.Round(p.alacak, 2);
                    detay.satirNo = p.satirNo;
                    detay.satirAciklama = p.satirAciklama;
                    detay.altHesapKod = p.altHesapKod;

                    // Eski gibi birim + kurumsal kod birlikte gider (ör. 005.12345678); iş kuralı DETAYBIRIM'e 3 haneyi yazar,
                    // tertip yoksa KURKOD'u birimden alır, varsa tertibin kurumsal koduyla uyumunu denetler.
                    string dBirim = Temiz(p.detayBirim);
                    if (dBirim == ".") dBirim = "";
                    detay.detayBirim = dBirim;
                    detay.mahsupBelgeNo = p.mahsupBelgeNo;

                    if (!string.IsNullOrEmpty(p.doviz) && p.doviz != "TL")
                    {
                        detay.doviz = p.doviz;
                        detay.dovizKurTur = p.dovizKurTur;
                        detay.dovizBorc = Math.Round(p.dovizBorc, 2);
                        detay.dovizAlacak = Math.Round(p.dovizAlacak, 2);
                        detay.dovizKur = p.dovizKur;
                    }

                    detay.isKod = p.isKod;
                    form.detay.objeler.Add(detay);
                }

                form.mukerrerlikKontrolu = istek.MukerrerKontrol;
                form.altHesapZorunsuzsaHata = true;
                IMUHServis servisMUH = Servis();
                Sonuc sonuc = servisMUH.OdemeEmriMIFKaydet(kullanan, form);
                if (sonuc.islemSonuc)
                {
                    string bilgi = sonuc.bilgiStr ?? "";
                    if (TNS.MUH.Arac.OfmEntegrasyonVarMi())
                    {
                        Sonuc sonucOybs = servisMUH.OybsKaydet(kullanan, form.yil, sonuc.anahtar, istek.Oybs ?? "");
                        if (!sonucOybs.islemSonuc)
                            return Results.Json(new { basarili = true, belgeNo = sonuc.anahtar, uyari = Duz("Belge kaydı yapılamadı." + sonucOybs.hataStr + "\n" + bilgi) });
                    }
                    return Results.Json(new { basarili = true, belgeNo = sonuc.anahtar, bilgi = Duz(bilgi) });
                }

                // Kayıt edilmek istenilen belgenin benzeri bulundu (eski "!MKK!" uyarısı: onaylanırsa mükerrerlik denetimi yapılmadan yeniden kaydedilir)
                if ((sonuc.hataStr ?? "").StartsWith("!MKK!"))
                {
                    string[] belgeNolar = sonuc.hataStr.Replace("!MKK!", "").Split(';', StringSplitOptions.RemoveEmptyEntries);
                    return Results.Json(new
                    {
                        basarili = false,
                        mukerrer = true,
                        yil = form.yil,
                        belgeler = belgeNolar,
                        hata = "Belge kaydı yapılamadı.\nKayıt edilmek istenilen belgenin bir benzeri " + string.Join(",", belgeNolar) +
                               " numara ile daha önce kayıt edilmiştir. Evet düğmesine basarak bu belgeyi kayıt edebilir veya Hayır düğmesine basarak kayıt işleminden vazgeçebilirsiniz."
                    });
                }
                return Basarisiz("Belge kaydı yapılamadı." + (sonuc.hataStr ?? "") + "\n" + (sonuc.bilgiStr ?? ""));
            }
            catch (Exception ex)
            {
                return Basarisiz("Belge kaydı yapılamadı. " + ex.Message);
            }
        }

        // ------------------------------------------------------------------ durum (eski BelgeDurumDegistir)

        private static int DurumKodu(string islem)
        {
            switch (Temiz(islem))
            {
                case "onayla": return (int)ENUMBelgeDurumu.ONAYLI;
                case "onaykaldir": return (int)ENUMBelgeDurumu.ONAYKALDIR;
                case "iptal": return (int)ENUMBelgeDurumu.IPTAL;
                case "yevver": return (int)ENUMBelgeDurumu.ICMALYEVMIYENOVER;
                case "yevkaldir": return (int)ENUMBelgeDurumu.YEVMIYEKALDIR;
                default: return 0;
            }
        }

        /// <summary>Durum sonuç metinleri (eski FRMOEM / FRMOES kaynakları).</summary>
        internal static void DurumMetinleri(int durum, bool tekBelge, out string olumlu, out string olumsuz)
        {
            olumlu = ""; olumsuz = "";
            if (durum == (int)ENUMBelgeDurumu.ONAYLI)
            {
                olumlu = tekBelge ? "Belge başarıyla Onaylandı." : "Seçili belgeler başarıyla Onaylandı.";
                olumsuz = tekBelge ? "Belgenin Onaylama işlemi gerçekleştirilemedi." : "Seçili belgelerin Onaylama işlemleri gerçekleştirilemedi.";
            }
            else if (durum == (int)ENUMBelgeDurumu.IPTAL)
            {
                olumlu = "Seçili belgelerin İptal edilme işlemleri başarıyla gerçekleştirildi.";
                olumsuz = "Seçili belgelerin İptal edilme işlemleri gerçekleştirilemedi.";
            }
            else if (durum == (int)ENUMBelgeDurumu.ICMALYEVMIYENOVER)
            {
                olumlu = "Seçili belgelere Yevmiye Numarası başarıyla verildi.";
                olumsuz = "Seçili belgelere Yevmiye Numarası verilemedi.";
            }
            else if (durum == (int)ENUMBelgeDurumu.ONAYKALDIR)
            {
                olumlu = tekBelge ? "Belgenin Onayı başarıyla kaldırıldı." : "Seçili belgelerin Onayları başarıyla kaldırıldı.";
                olumsuz = tekBelge ? "Belgenin Onayı kaldırılamadı." : "Seçili belgelerin Onayları kaldırılamadı.";
            }
        }

        /// <summary>Eski OdemeEmriMIF2.OybsBelgeDurumDegistir: formda seçili OYBS kayıtlarının muhasebeleşme bilgisi güncellenir.</summary>
        private static string OybsBelgeDurumDegistir(Kullanici kullanan, int yil, string belgeNo, int durum, string oybs)
        {
            if (string.IsNullOrEmpty(oybs)) return "";
            try
            {
                foreach (string parca in oybs.Split(';'))
                {
                    if (string.IsNullOrWhiteSpace(parca) || !parca.Contains("-")) continue;
                    string[] tipId = parca.Split('-');
                    OybsIslem islem = new OybsIslem();
                    islem.islemTipi = OybsTip(OrtakFonksiyonlar.ConvertToInt(tipId[0], 0));
                    islem.MuhasebeIslemAciklamasi = yil + "/" + belgeNo + "nolu belge ile " + DateTime.Now.ToString() + " tarihinde muhasebeleşmiştir.";
                    islem.OybsId = long.Parse(tipId[1]);
                    if (durum == (int)ENUMBelgeDurumu.ONAYLI)
                        islem.MuhasebelestiMi = true;
                    else if (durum == (int)ENUMBelgeDurumu.ONAYKALDIR)
                    {
                        islem.MuhasebelestiMi = false;
                        islem.MuhasebeIslemAciklamasi = string.Empty;
                        Servis().OybsSil(kullanan, yil, belgeNo);
                    }
                    OdemeEmriWebServisleri.Kaydet(islem);
                }
                return "";
            }
            catch (Exception ex)
            {
                return "OYBS güncellenemedi: " + ex.Message;
            }
        }

        private static async Task<IResult> Durum(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, OemDurumIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new OemDurumIstegi();
            int durum = DurumKodu(istek.Islem);
            if (durum == 0) return Basarisiz("İşlem tanımsız.");
            try
            {
                TNSCollection fler = new TNSCollection();
                OdemeEmriMIFForm f = new OdemeEmriMIFForm();
                f.yil = istek.Yil;
                f.belgeNo = Temiz(istek.BelgeNo);
                f.kurum = Kurum();
                f.islemYapan = kullanan.kullaniciKodu;
                f.islemYapanAd = kullanan.ad + " " + kullanan.soyad;
                f.formDurum = durum;
                SurecAta(f, istek.SurecNo);

                if (f.belgeNo != "")
                    fler.Add(f);

                if (durum == (int)ENUMBelgeDurumu.ICMALYEVMIYENOVER)
                {
                    if (Temiz(istek.YevmiyeNo) != "")
                        f.YEVYevNo = OrtakFonksiyonlar.ConvertToInt(Temiz(istek.YevmiyeNo), 0);
                    f.yevmiyeAciklama = istek.YevmiyeAciklama ?? "";
                    if (!istek.BelgeTarihi)
                    {
                        f.YEVYevTarih = Tarih(istek.YevmiyeTarih);
                        if (f.YEVYevTarih.isNull)
                            return Basarisiz("Yevmiye tarihi hatalı. Lütfen geçerli bir tarih giriniz.");
                    }
                }

                Sonuc sonuc = Servis().BelgeDurumDegistirODEMIF(kullanan, fler, true);
                DurumMetinleri(durum, true, out string olumlu, out string olumsuz);
                if (!sonuc.islemSonuc) return Basarisiz(olumsuz + (sonuc.hataStr ?? ""));

                string uyari = "";
                if (TNS.MUH.Arac.OfmEntegrasyonVarMi())
                    uyari = OybsBelgeDurumDegistir(kullanan, f.yil, f.belgeNo, durum, istek.Oybs);
                return Results.Json(new { basarili = true, bilgi = Duz(olumlu + (sonuc.bilgiStr ?? "")), uyari = uyari });
            }
            catch (Exception ex)
            {
                return Basarisiz(ex.Message);
            }
        }

        // ------------------------------------------------------------------ yazdırma ve EBYS (eski Yazdir)

        /// <summary>Belge çıktısı. tur: 1 belgenin türüyle, 2 Ödeme Emri, 3 MİF (eski btnYazdir BelgeTur). prg: PEB'li şablon (eski chkPRGButceFormat).</summary>
        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string belgeNo, int? tur, string prg)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (Temiz(belgeNo) == "") return Hata("Yazdırmak için önce belgeyi kaydediniz ya da listeleyiniz.");
            ENUMBelgeTuru belgeTur = ENUMBelgeTuru.TANIMSIZ;
            if (tur == 2) belgeTur = ENUMBelgeTuru.ODEMEEMRI;
            else if (tur == 3) belgeTur = ENUMBelgeTuru.MIF;
            try
            {
                AsposeLisans.Yukle();
                string bNo = BelgeNoDuzenle(belgeNo);
                string hata = OdemeEmriMIFYazdirma.Yazdir(kullanan, yil ?? 0, bNo, "", belgeTur, out string dosya, out string uzanti, out string _, false, false, prg == "1");
                if (dosya == null) return Hata(HataMetni(hata, "Belge yazdırılamadı."));
                return DosyaIslem.DosyaGonder(dosya, bNo, true, uzanti ?? GenelIslemler.ExcelTur());
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Belge yazdırılamadı."));
            }
        }

        /// <summary>Hızlı rapor "KBS-SGB Fark" (eski ButceMuhasebeGenel.HizliRaporAl, kod 1: rapor 5000-13). Seyyar Görev sayfada açılır.</summary>
        private static IResult HizliRapor(HttpContext baglam, IWebHostEnvironment ortam, string kod, int? yil, int? muhasebe, string birim)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (kod != "1") return Hata("Hazır Değil");
            int iyil = yil ?? -1;
            Hashtable raporParam = new Hashtable();
            raporParam["ButceYil"] = iyil.ToString();
            int ay = DateTime.Now.Month;
            if (iyil < DateTime.Now.Year) ay = 12;
            raporParam["AyNo"] = ay.ToString();
            raporParam["Muhasebe"] = (muhasebe ?? -1).ToString();
            raporParam["Birim"] = Temiz(birim);
            raporParam["Kur"] = Kurum();
            raporParam["EkoKirilim"] = "4";
            raporParam["CiktiYeri"] = "EXCEL";
            try
            {
                AsposeLisans.Yukle();
                string dosya = RaporlamaServis.TekRaporAl(5000, 13, kullanan, null, raporParam, null);
                if (RaporlamaServis.MesajMi(dosya)) return Hata(dosya);
                if (string.IsNullOrEmpty(dosya) || !File.Exists(dosya)) return Hata("Rapor hazırlanamadı.");
                string uzanti = Path.GetExtension(dosya).TrimStart('.').ToLowerInvariant();
                return DosyaIslem.DosyaGonder(dosya, "KBS_SGB_Fark_" + DosyaIslem.DosyaAdUretSade(), true, uzanti);
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }

        // ------------------------------------------------------------------ seyyar görev kullanımı (eski HarcamaSurecleri/SeyyarGorevBilgi.aspx)

        /// <summary>Rapor Al "Seyyar Görev": istihdam şekline göre vize edilen / kullanılan / kalan gün ve gönderilen / harcanan tutar.</summary>
        private static IResult Seyyar(HttpContext baglam, IWebHostEnvironment ortam, int? yil, int? muhasebe, string birim)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                TNS.HRC.SeyyarGorevDurumAna sgda = TNS.HRC.Arac.Tanimla().SeyyarGorevDurumBul(kullanan, yil ?? 0, muhasebe ?? 0, Temiz(birim), "",
                    TNS.HRC.Arac.SEYYARGOREVISLEM, 0, "", "");
                if (sgda == null) return Hata("Seyyar görev bilgisi okunamadı.");
                if (sgda.sonuc != null && !sgda.sonuc.islemSonuc) return Hata(HataMetni(sgda.sonuc.hataStr, "Seyyar görev bilgisi okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (TNS.HRC.SeyyarGorevDurum d in sgda.istihdamSekilleri)
                {
                    string ad = "";
                    try { ad = TNS.PER.Arac.IstihdamSekliAd((TNS.PER.ENUMPersonelIstihdamSekli)d.istihdamSekli); } catch { }
                    satirlar.Add(new { istihdamSekli = d.istihdamSekli, ad = ad, vizeEdilen = d.vizeEdilen, kullanilan = d.kullanilan,
                        kalan = d.vizeEdilen - d.kullanilan, tutar = d.tutar });
                }
                return Results.Json(new { satirlar = satirlar, gonderilen = sgda.gonderilen, harcanan = sgda.harcanan, kalan = sgda.gonderilen - sgda.harcanan });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Seyyar görev bilgisi okunamadı."));
            }
        }

        // ------------------------------------------------------------------ küçük işlemler

        /// <summary>Süreç Bağı Kur (eski btnSurecBagKur_Click): dönen ihale bilgisi formdaki ihale kod / alım türüne yazılır.</summary>
        private static async Task<IResult> SurecBag(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, OemBelgeIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new OemBelgeIstegi();
            try
            {
                SurecAta(istek.SurecNo, out int surecYil, out string surecNo);
                Sonuc sonuc = Servis().OdemeEmriMIFSurecBagKur(kullanan, istek.Yil, Temiz(istek.BelgeNo), surecYil, surecNo, istek.Muhasebe, Temiz(istek.Birim));
                if (!sonuc.islemSonuc) return Basarisiz(sonuc.hataStr);
                return Results.Json(new
                {
                    basarili = true,
                    bilgi = "Süreç ilişkisi kuruldu",
                    ihaleKod = TNS.MUH.Arac.IhaleKodBul(sonuc.anahtar),
                    alimTur = TNS.MUH.Arac.IhaleAlimturBul(sonuc.anahtar)
                });
            }
            catch (Exception ex)
            {
                return Basarisiz(ex.Message);
            }
        }

        /// <summary>İlgili belge bağı (eski btnIlgiliBelgeYaz_Click; yalnız sistem yöneticisi).</summary>
        private static async Task<IResult> IlgiliBelge(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, OemBelgeIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!Yonetici(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            istek ??= new OemBelgeIstegi();
            try
            {
                Sonuc sonuc = Servis().OdemeEmriMIFIlgiliBelgeKaydet(kullanan, istek.Yil, Temiz(istek.BelgeNo), Temiz(istek.IlgiliBelgeNo));
                return sonuc.islemSonuc ? Basari("Bağ kuruldu") : Basarisiz("Bağ kurulamadı:" + sonuc.hataStr);
            }
            catch (Exception ex)
            {
                return Basarisiz("Bağ kurulamadı:" + ex.Message);
            }
        }

        /// <summary>Kayıtlı belgede satırın alt hesabını değiştir (eski AltHesapDegistir; tablo bağlam menüsü komutu AHD).</summary>
        private static async Task<IResult> AltHesap(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, OemBelgeIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new OemBelgeIstegi();
            try
            {
                string altHesap = Temiz(istek.AltHesapKod).Split('-')[0].Trim();
                Sonuc sonuc = Servis().AltHesapDegistir(kullanan, istek.Yil, Temiz(istek.BelgeNo), istek.SiraNo, altHesap);
                return sonuc.islemSonuc ? Basari("Değişiklik tamamlandı") : Basarisiz("Değişiklik yapılırken hata oludu:" + sonuc.hataStr);
            }
            catch (Exception ex)
            {
                return Basarisiz("Değişiklik yapılırken hata oludu:" + ex.Message);
            }
        }

        /// <summary>Döviz kuru (eski DovizKurGetir): aktif para birimi için 1, diğerleri için Merkez Bankası kuru.</summary>
        private static IResult DovizKur(HttpContext baglam, IWebHostEnvironment ortam, string doviz, string kurTur, string tarih)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici _);
            if (ret != null) return ret;
            try
            {
                TNS.HRC.IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();
                ObjectArray pl = servisHRC.ParaBirimiListele(new Kullanici(), new ParaBirimi());
                doviz = Temiz(doviz);
                string aktifKur = "";
                foreach (ParaBirimi p in pl.objeler)
                {
                    if (p.kod == 1) aktifKur = p.kisaltma;
                    if (p.adi == doviz)
                    {
                        doviz = p.kisaltma;
                        break;
                    }
                }
                if (doviz == aktifKur || doviz == "") return Results.Json(new { kur = 1m });

                TNS.HRC.MerkezBankasiKurSinif kriter = new TNS.HRC.MerkezBankasiKurSinif();
                kriter.kurTarih = Tarih(tarih);
                ObjectArray liste = servisHRC.MerkezBankKurDegerleri(kriter, new[] { doviz });
                decimal kur = 0;
                foreach (TNS.HRC.KurDegerleri k in liste.objeler)
                {
                    switch (Temiz(kurTur))
                    {
                        case "ALIS": kur = k.alis; break;
                        case "SATIS": kur = k.satis; break;
                        case "EALIS": kur = k.efektifAlis; break;
                        case "ESATIS": kur = k.efektifSatis; break;
                        case "SALIS": kur = k.serbestAlis; break;
                        case "SSATIS": kur = k.serbestSatis; break;
                    }
                    break;
                }
                return Results.Json(new { kur = kur });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Döviz kuru okunamadı."));
            }
        }

        /// <summary>Taahhüt kartı seçimi (eski TaahhutBilgiYaz): proje sıra no, ödenek tertibi ve tertibin ekonomik kodundan 920.01 hesabı.</summary>
        private static IResult Taahhut(HttpContext baglam, IWebHostEnvironment ortam, string kod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            string hesapKod = "", tertip = "", projeNo = "";
            try
            {
                if (!string.IsNullOrEmpty(kod))
                {
                    TaahhutKartiBilgi bilgi = new TaahhutKartiBilgi();
                    bilgi.kod = OrtakFonksiyonlar.ConvertToInt(kod, -1);
                    ObjectArray liste = Servis().TaahhutKartiListele(kullanan, bilgi, 0);
                    if (liste != null && liste.sonuc.islemSonuc && liste.objeler.Count == 1)
                    {
                        TaahhutKartiBilgi tk = (TaahhutKartiBilgi)liste.objeler[0];
                        projeNo = tk.projeSiraNo;
                        tertip = tk.odenekTertip ?? "";
                        string[] t = tertip.Split('-');
                        if (t.Length > 3)
                            hesapKod = TNS.MUH.Arac.EkonomikKoddanHesapkodBul(t[3].Replace(".", ""), "920.01", 4);
                    }
                }
                return Results.Json(new { hesapKod = hesapKod ?? "", tertip = tertip, projeNo = projeNo ?? "" });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Taahhüt kartı okunamadı."));
            }
        }

        /// <summary>Detay birim seçenekleri (eski SaymanlikBirimDoldur: muhasebenin birimleri ve boş seçenek).</summary>
        private static IResult DetayBirimler(HttpContext baglam, IWebHostEnvironment ortam, int? muhasebe)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici _);
            if (ret != null) return ret;
            List<object> liste = new List<object>();
            try
            {
                if ((muhasebe ?? 0) > 0)
                {
                    MuhasebeBirim mb = new MuhasebeBirim();
                    mb.muhasebe = muhasebe.Value;
                    ObjectArray oa = Servis().MuhasebeBirimListele(mb);
                    // Eski combo değeri BirimKurum: 3 haneli birim + "." + birimin kurumsal kodu (ör. 005.12345678). Kayıtta iş kuralı
                    // ikiye böler: DETAYBIRIM'e birim yazılır; tertip yoksa KURKOD birimin kurumsal kodu olur, varsa ikisi uyumlu olmalıdır.
                    foreach (MuhasebeBirim b in oa.objeler)
                    {
                        string bk = b.birimKod ?? "", kur = (b.kurum ?? "").Trim();
                        liste.Add(new { kod = kur == "" ? bk : bk + "." + kur, ad = b.birimAd ?? "" });
                    }
                    if (liste.Count > 0) liste.Add(new { kod = "", ad = "" });
                }
                return Results.Json(new { birimler = liste });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Birimler okunamadı."));
            }
        }

        // ------------------------------------------------------------------ hesap planı, alt hesap, firma (Sgb.hesapPlaniSec / Sgb.firmaSec uçları)
        // Makbuz uçlarının (MakbuzUclari.HesapAgac / HesapAra / HesapAd / FirmaAra) aynısı; yetki bu sayfadan gelir.

        private static int HesapYil(int? yil)
        {
            int y = yil ?? 0;
            return y <= 2000 ? DateTime.Now.Year : y;
        }

        private static object HesapDugum(HesapPlaniSatir h)
        {
            return new { kod = h.hesapKod, ad = h.hesapKod + " - " + h.aciklama, hesapAd = h.aciklama ?? "", altVar = !h.detay, detay = h.detay };
        }

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

        private static IResult HesapAra(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string metin, string sonKirilim)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            metin = Temiz(metin);
            if (metin.Length < 2) return Hata("En az iki karakter yazınız.");
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

        private static IResult HesapAd(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string kod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            kod = Temiz(kod).Split('-')[0].Trim();
            if (kod == "") return Results.Json(new { ad = "", detay = false });
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
                            return Results.Json(new { kod = d.hesapKod, ad = d.aciklama ?? "", detay = d.detay, dovizKurKod = d.dovizKurKod ?? "", dovizKurTurSatis = d.dovizKurTurSatis ?? "" });
                return Results.Json(new { kod = kod, ad = "", detay = false, dovizKurKod = "", dovizKurTurSatis = "" });
            }
            catch (Exception)
            {
                return Hata("Hesap kodu okunamadı.");
            }
        }

        /// <summary>Alt hesap arama (eski AltHesapStore_Refresh: AltHesapListeHazirla; "vergiNo - ad").</summary>
        private static IResult AltHesapAra(HttpContext baglam, IWebHostEnvironment ortam, int? muhasebe, string metin)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            metin = Temiz(metin);
            if (metin.Length < 2) return Results.Json(new { satirlar = new List<object>() });
            try
            {
                ObjectArray bilgi = Servis().AltHesapListeHazirla(kullanan, muhasebe ?? 0, metin);
                List<object> satirlar = new List<object>();
                foreach (TNS.HRC.FirmaBilgisi f in bilgi.objeler)
                {
                    satirlar.Add(new { kod = f.vergiNo + " - " + f.ad, vergiNo = f.vergiNo ?? "", ad = f.ad ?? "" });
                    if (satirlar.Count >= 100) break;
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Arama yapılamadı.");
            }
        }

        /// <summary>Kişi / firma arama (eski HarcamaSurecleri/ListeFirmaKisi.aspx).</summary>
        private static IResult FirmaAra(HttpContext baglam, IWebHostEnvironment ortam, int? muhasebe, string ad, string vergiNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            ad = Temiz(ad);
            vergiNo = Temiz(vergiNo);
            if (ad == "" && vergiNo == "") return Hata("Aramak için ad ya da kimlik / vergi no yazınız.");
            try
            {
                TNS.HRC.FirmaBilgisi h = new TNS.HRC.FirmaBilgisi();
                h.muhasebe = muhasebe ?? 0;
                h.ad = ad;
                h.vergiNo = vergiNo;
                h.aktifDurum = 1;
                ObjectArray bilgi = TNS.HRC.Arac.Tanimla().FirmaBilgisiListele(kullanan, h);
                if (bilgi == null || !bilgi.sonuc.islemSonuc) return Hata(HataMetni(bilgi?.sonuc.hataStr, "Arama yapılamadı."));
                List<object> satirlar = new List<object>();
                foreach (TNS.HRC.FirmaBilgisi f in bilgi.objeler)
                {
                    satirlar.Add(new
                    {
                        vergiNo = f.vergiNo ?? "",
                        ad = f.ad ?? "",
                        vergiDairesi = f.vergiDairesi ?? "",
                        adres = f.adres ?? "",
                        bankaAd = f.banka ?? "",
                        bankaHesapNo = f.hesapNo ?? ""
                    });
                    if (satirlar.Count >= 500) break;
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Arama yapılamadı.");
            }
        }
            // ------------------------------------------------------------------ şablonlar (eski wndSablonKaydet / wndSablonAc)

        /// <summary>Şablon listesi. tur=kaydet: kullanıcının kendi şablonları; tur=acma: benim=1 ise benimkiler, değilse muhasebe biriminin
        /// herkese açık şablonları (eski SablonListesiYukle).</summary>
        private static IResult SablonListe(HttpContext baglam, IWebHostEnvironment ortam, string tur, int? benim, int? muhasebe)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                Sablon sablon = new Sablon();
                if (tur == "kaydet" || benim != 0)
                    sablon.islemYapan = kullanan.kullaniciKodu;
                else
                {
                    sablon.gormeYetkisi = 2;
                    sablon.muhasebe = muhasebe ?? 0;
                }
                sablon.detayVer = false;
                ObjectArray sV = Servis().SablonListe(sablon);
                if (sV == null || !sV.sonuc.islemSonuc) return Hata(HataMetni(sV?.sonuc.hataStr, "Şablonlar okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (Sablon d in sV.objeler)
                    satirlar.Add(new { KOD = d.yil + ";" + d.kod, AD = d.ad ?? "" });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Şablonlar okunamadı."));
            }
        }

        private static bool SablonKodAyir(string kod, out int yil, out int sablonKod)
        {
            string[] dler = (kod ?? "").Split(';');
            yil = OrtakFonksiyonlar.ConvertToInt(dler[0], 0);
            sablonKod = dler.Length > 1 ? OrtakFonksiyonlar.ConvertToInt(dler[1], 0) : 0;
            return sablonKod > 0;
        }

        /// <summary>Şablonu forma açar (eski SablonAc): başlık bilgileri ve satırlar.</summary>
        private static IResult SablonAc(HttpContext baglam, IWebHostEnvironment ortam, string kod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!SablonKodAyir(kod, out int yil, out int sablonKod)) return Hata("Açmak için şablon seçiniz.");
            try
            {
                Sablon sablon = new Sablon();
                sablon.yil = yil;
                sablon.kod = sablonKod;
                sablon.detayVer = true;
                ObjectArray sV = Servis().SablonListe(sablon);
                if (sV == null || !sV.sonuc.islemSonuc) return Hata(HataMetni(sV?.sonuc.hataStr, "Şablon açılamadı."));
                foreach (Sablon s in sV.objeler)
                {
                    List<Dictionary<string, object>> liste = new List<Dictionary<string, object>>();
                    foreach (SablonDetay d in s.detay.objeler)
                        liste.Add(Satir(d.kesinti, d.hesapKod + "-" + d.aciklama, d.aciklama, d.tertip != null ? d.tertip.TertipStrOl() : "", "",
                            d.borc, d.alacak, d.altHesapKod, d.satirAciklama, d.doviz, d.dovizKurTur, d.dovizBorc, d.dovizAlacak, d.dovizKur,
                            "", d.detayBirim, 0, ""));
                    return Results.Json(new
                    {
                        muhasebe = s.muhasebe,
                        birim = s.birim ?? "",
                        kurumsal = OrtakFonksiyonlar.KodNoktaKoy(TertipYapi.KodYapiBul(yil, 1, false), s.kurKod ?? ""),
                        ilgiliAd = s.ilgiliAd ?? "",
                        ilgiliNo = s.ilgiliNo ?? "",
                        ilgiliVD = s.ilgiliVD ?? "",
                        ilgiliBankaAd = s.ilgiliBankaAd ?? "",
                        ilgiliBankaNo = s.ilgiliBankaNo ?? "",
                        aciklama = s.aciklama ?? "",
                        satirlar = Numarala(liste)
                    });
                }
                return Hata("Şablon bulunamadı.");
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Şablon açılamadı."));
            }
        }

        /// <summary>Şablon Kaydet (eski btnSablonKaydet_Click).</summary>
        private static async Task<IResult> SablonKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, OemSablonIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new OemSablonIstegi();
            if (Temiz(istek.Ad) == "") return Basarisiz("Şablon adını yazınız.");
            try
            {
                Sablon sablon = new Sablon();
                sablon.yil = istek.Yil;
                sablon.ad = Temiz(istek.Ad);
                sablon.gormeYetkisi = istek.Izin == 1 ? 1 : 2;
                if (istek.Uzerine)
                {
                    SablonKodAyir(istek.Kod, out int _, out int kod);
                    if (kod == 0) return Basarisiz("Varolan bir şablonu değiştirmek istediğiniz için, ilk önce listeden şablon seçiniz.");
                    sablon.kod = kod;
                }
                sablon.muhasebe = istek.Muhasebe;
                sablon.birim = istek.Birim ?? "";
                sablon.kurKod = istek.Kurumsal ?? "";
                sablon.ilgiliAd = istek.IlgiliAd ?? "";
                sablon.ilgiliNo = istek.IlgiliNo ?? "";
                sablon.ilgiliVD = istek.IlgiliVD ?? "";
                sablon.ilgiliBankaAd = istek.IlgiliBankaAd ?? "";
                sablon.ilgiliBankaNo = istek.IlgiliBankaNo ?? "";
                sablon.aciklama = istek.Aciklama ?? "";
                sablon.islemYapan = kullanan.kullaniciKodu;

                ObjectArray pler = DetayDoldur(sablon.yil, istek.Satirlar);
                foreach (OdemeEmriMIFDetay d in pler.objeler)
                {
                    SablonDetay sd = new SablonDetay();
                    sd.alacak = d.alacak;
                    sd.altHesapKod = d.altHesapKod;
                    sd.detayBirim = d.detayBirim;
                    sd.satirAciklama = d.satirAciklama;
                    sd.borc = d.borc;
                    sd.doviz = d.doviz;
                    sd.dovizAlacak = d.dovizAlacak;
                    sd.dovizBorc = d.dovizBorc;
                    sd.dovizKur = d.dovizKur;
                    sd.dovizKurTur = d.dovizKurTur;
                    sd.hesapKod = d.hesapKod;
                    sd.kesinti = d.kesinti;
                    sd.sira = d.satirNo;
                    sd.tertip.TertipAta(d.kur, d.fon, d.fin, d.eko, d.peb);
                    sablon.detay.objeler.Add(sd);
                }

                Sonuc sonuc = Servis().SablonKaydet(sablon);
                if (!sonuc.islemSonuc) return Basarisiz("Şablon kaydı yapılamadı. Hata : " + (sonuc.hataStr ?? ""));
                return Basari(string.IsNullOrEmpty(sonuc.bilgiStr) ? "Şablon kaydedildi." : sonuc.bilgiStr);
            }
            catch (Exception ex)
            {
                return Basarisiz("Şablon kaydı yapılamadı. Hata : " + ex.Message);
            }
        }

        /// <summary>Şablon Sil (eski btnSablonSil_Click / SablonSil).</summary>
        private static async Task<IResult> SablonSil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, OemSablonIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!SablonKodAyir(istek?.Kod, out int yil, out int kod)) return Basarisiz("Silmek için şablon seçiniz.");
            try
            {
                Sablon sablon = new Sablon();
                sablon.yil = yil;
                sablon.kod = kod;
                Sonuc sonuc = Servis().SablonSil(sablon);
                return sonuc.islemSonuc ? Basari("Şablon başarıyla silindi.") : Basarisiz("Şablon silinemedi." + (sonuc.hataStr ?? ""));
            }
            catch (Exception ex)
            {
                return Basarisiz("Şablon silinemedi." + ex.Message);
            }
        }

        // ------------------------------------------------------------------ kur farkı (eski wndKurFarki)

        private static async Task<IResult> KurFarki(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, OemKurFarkiIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new OemKurFarkiIstegi();
            if (string.IsNullOrWhiteSpace(istek.Aciklama)) return Basarisiz("Açıklama alanı boş bırakılamaz");
            try
            {
                OdemeEmriMIFForm belge = new OdemeEmriMIFForm();
                belge.yil = istek.Yil;
                belge.islemTarih = Tarih(istek.KurTarih);
                belge.belgeNo = Temiz(istek.BelgeNo);
                belge.aciklama = istek.Aciklama;
                belge.detay = new ObjectArray();
                OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(belge.yil);
                detay.hesapKod = Temiz(istek.HesapKod);
                if (detay.hesapKod != "")
                    detay.hesapKod = detay.hesapKod.Split('-')[0].Trim();
                belge.detay.objeler.Add(detay);

                Sonuc sonuc = Servis().KurFarkiBelgeUret(kullanan, belge, istek.MuhasebeBirimineIsle);
                if (!sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc.hataStr, "Kur farkı işlemi yapılamadı."));
                return Results.Json(new { basarili = true, bilgi = "Kur farkı işlemi başarıyla sonuçlandı. Belge No: " + sonuc.anahtar, belgeNo = sonuc.anahtar });
            }
            catch (Exception ex)
            {
                return Basarisiz(ex.Message);
            }
        }

        // ------------------------------------------------------------------ yansıtma (eski wndYansitma)

        /// <summary>Yansıtma Göster (eski btnYansitmaGoster_Click + YansitmaListesiOlustur).</summary>
        private static async Task<IResult> YansitmaListe(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, OemYansitmaIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new OemYansitmaIstegi();
            try
            {
                IMUHServis servisMUH = Servis();
                int yil = istek.Yil;
                string[] yler = KodlarStr(kullanan, yil, ENUMHesapKodNitelik.YANSITMAYAPILACAK, 1).Split('|');
                string[] klar = KodlarStr(kullanan, yil, ENUMHesapKodNitelik.KDV, 0).Split('|');
                ObjectArray pler = DetayDoldur(yil, istek.Satirlar);

                // KDV satırları üstteki hesaba eklenir (aynı hesap sonra birleştirildiği için KDV açıkta kalmasın)
                for (int i = 0; i < pler.objeler.Count; i++)
                {
                    OdemeEmriMIFDetay p = (OdemeEmriMIFDetay)pler.objeler[i];
                    if (p.hesapKod.Length < 3) continue;
                    for (int kk = 0; kk < klar.Length; kk++)
                    {
                        if (string.IsNullOrWhiteSpace(klar[kk]) || klar[kk].Length < 3) continue;
                        if (klar[kk].Substring(0, 3) == p.hesapKod.Substring(0, 3))
                        {
                            if (i == 0) break;
                            OdemeEmriMIFDetay oncekiP = (OdemeEmriMIFDetay)pler.objeler[i - 1];
                            if (oncekiP.hesapKod == "" && i > 1)
                                oncekiP = (OdemeEmriMIFDetay)pler.objeler[i - 2]; // bir hesabın iki KDV satırı olabilir
                            oncekiP.borc += p.borc;
                            oncekiP.alacak += p.alacak;
                            p.hesapKod = "";
                            p.borc = 0;
                            p.alacak = 0;
                            break;
                        }
                    }
                }

                List<string[]> hs = new List<string[]>();
                foreach (OdemeEmriMIFDetay p in pler.objeler)
                {
                    if (string.IsNullOrEmpty(p.hesapKod)) continue;
                    bool yansit = false;
                    string uc = p.hesapKod.Length >= 3 ? p.hesapKod.Substring(0, 3) : p.hesapKod;
                    for (int kk = 0; kk < yler.Length; kk++)
                        if (yler[kk] == uc) { yansit = true; break; }
                    if (yansit || yler.Length == 0)
                        hs.Add(new[] { p.hesapKod, TNS.MUH.Arac.TertipStrOl(p), p.borc.ToString(), p.alacak.ToString(), p.detayBirim ?? "", p.isKod.ToString() });
                }
                if (hs.Count == 0) return Hata("Yansıtma yapılacak bilgi bulunamadı");

                List<HesapPlaniSatir> hp = new List<HesapPlaniSatir>();
                bool yansitma323 = false;
                foreach (string[] hDeger in hs)
                {
                    bool bulundu = false;
                    foreach (HesapPlaniSatir hKontrol in hp)
                    {
                        if (hKontrol.hesapKod == hDeger[0] && hKontrol.grup.EndsWith(hDeger[4] + "|" + hDeger[5]))
                        {
                            string[] grup = hKontrol.grup.Split('|');
                            double borc = OrtakFonksiyonlar.ConvertToDbl(grup[0]) + OrtakFonksiyonlar.ConvertToDbl(hDeger[2]);
                            double alacak = OrtakFonksiyonlar.ConvertToDbl(grup[1]) + OrtakFonksiyonlar.ConvertToDbl(hDeger[3]);
                            // 120 hesabına hem borç hem alacak geliyorsa alacak alınır
                            if (hKontrol.hesapKod.StartsWith("120") && borc > 0 && alacak > 0) borc = 0;
                            hKontrol.grup = borc + "|" + alacak + "|" + grup[2] + "|" + grup[3] + "|" + grup[4];
                            bulundu = true;
                            break;
                        }
                    }
                    // 323 borçta yansıtılır, alacakta yansıtılmaz
                    if (hDeger[0].StartsWith("323") && OrtakFonksiyonlar.ConvertToDbl(hDeger[3]) > 0) bulundu = true;
                    if (!bulundu)
                    {
                        HesapPlaniSatir h = new HesapPlaniSatir();
                        h.hesapKod = hDeger[0];
                        h.yil = yil;
                        h.grup = hDeger[2] + "|" + hDeger[3] + "|" + hDeger[1] + "|" + hDeger[4] + "|" + hDeger[5];
                        hp.Add(h);
                        if (h.hesapKod.StartsWith("323")) yansitma323 = true; // 323 yansıtması varsa 630 yansıtılmaz
                    }
                }

                bool h630 = hp.Any(h => h.hesapKod.StartsWith("630"));
                bool h600 = hp.Any(h => h.hesapKod.StartsWith("600"));
                bool h120 = hp.Any(h => h.hesapKod.StartsWith("120"));

                ObjectArray hy = new ObjectArray();
                List<YansitmaYapilacakSatirlar> yapilacak = new List<YansitmaYapilacakSatirlar>();
                for (int hSatir = 0; hSatir < hp.Count; hSatir++)
                {
                    HesapPlaniSatir h = hp[hSatir];
                    if (h120 && h600 && h630 && h.hesapKod.StartsWith("600")) continue; // EGO
                    if (h120 && h600 && h.hesapKod.StartsWith("120")) continue;        // EGO
                    if (yansitma323 && h.hesapKod.StartsWith("630")) continue;

                    string grup = h.grup; // servis dönüşünde grup siliniyor
                    HesapPlaniSatir h1 = servisMUH.HesapKodOzellikAl(h);
                    if (h1 == null) continue;
                    bool zorunluBorc = TNS.MUH.Arac.NitelikVarmi(h1.nitelik, (int)ENUMHesapKodNitelik.YANSITMAZORUNLUBORC);
                    bool zorunluAlacak = TNS.MUH.Arac.NitelikVarmi(h1.nitelik, (int)ENUMHesapKodNitelik.YANSITMAZORUNLUALACAK);
                    if (!(TNS.MUH.Arac.NitelikVarmi(h1.nitelik, (int)ENUMHesapKodNitelik.YANSITMAYAPILACAK) || zorunluBorc || zorunluAlacak)) continue;

                    string[] dizi = grup.Split('|');
                    double borcT = dizi.Length > 0 ? OrtakFonksiyonlar.ConvertToDbl(dizi[0]) : 0;
                    double alacakT = dizi.Length > 1 ? OrtakFonksiyonlar.ConvertToDbl(dizi[1]) : 0;
                    if (!zorunluAlacak && zorunluBorc && borcT == 0) continue;
                    if (!zorunluBorc && zorunluAlacak && alacakT == 0) continue;

                    for (int i = hSatir + 1; i < hp.Count; i++)
                    {
                        HesapPlaniSatir h3 = hp[i];
                        string grupKDV = h3.grup;
                        HesapPlaniSatir h2 = servisMUH.HesapKodOzellikAl(h3);
                        if (h2 != null && TNS.MUH.Arac.NitelikVarmi(h2.nitelik, (int)ENUMHesapKodNitelik.KDV))
                        {
                            string[] dk = grupKDV.Split('|');
                            borcT += OrtakFonksiyonlar.ConvertToDbl(dk[0]);
                            alacakT += dk.Length > 1 ? OrtakFonksiyonlar.ConvertToDbl(dk[1]) : 0;
                        }
                        else
                            break;
                    }
                    hy.objeler.Add(h1);

                    YansitmaYapilacakSatirlar yys = new YansitmaYapilacakSatirlar();
                    yys.hesapKod = h.hesapKod.Replace(".", "").Trim();
                    yys.hesapAd = h.hesapKod + "-" + h1.aciklama;
                    yys.tertip = dizi.Length > 2 ? dizi[2] : "";
                    yys.detayBirim = dizi.Length > 3 ? dizi[3] : "";
                    yys.borc = borcT;
                    yys.alacak = alacakT;
                    yys.isKod = dizi.Length > 4 ? dizi[4] : "0";
                    yapilacak.Add(yys);
                }

                List<object> yansitmaHesaplari = new List<object>();
                ObjectArray hyv = servisMUH.HesapPlaniYansitmaListele(kullanan, hy);
                List<HesapPlaniYansitma> hpyler = new List<HesapPlaniYansitma>();
                if (hyv != null && hyv.sonuc.islemSonuc)
                    foreach (HesapPlaniYansitma hpy in hyv.objeler)
                    {
                        hpyler.Add(hpy);
                        yansitmaHesaplari.Add(new
                        {
                            HESAPKOD = hpy.hesapKod.Replace(".", "").Trim(),
                            HESAPAD = hpy.hesapKod + "-" + hpy.hKodAciklama,
                            YANSITMAHESAPKOD = hpy.yansitmaHesapKod,
                            YANSITMAHESAPAD = hpy.yansitmaHesapKod + "-" + hpy.yKodAciklama
                        });
                    }

                List<object> satirlar = new List<object>();
                foreach (YansitmaYapilacakSatirlar yys in yapilacak)
                {
                    HesapPlaniYansitma ilk = hpyler.FirstOrDefault(x => x.hesapKod.Replace(".", "").Trim() == yys.hesapKod);
                    if (ilk != null) yys.yansitma = ilk.yansitmaHesapKod + "-" + ilk.yKodAciklama;
                    satirlar.Add(new
                    {
                        HESAPKOD = yys.hesapKod,
                        HESAPAD = yys.hesapAd,
                        YANSITMA = yys.yansitma,
                        TERTIP = yys.tertip,
                        BORC = yys.borc,
                        ALACAK = yys.alacak,
                        DETAYBIRIM = yys.detayBirim,
                        ISKOD = yys.isKod
                    });
                }
                if (satirlar.Count == 0) return Hata("Yansıtma yapılacak bilgi bulunamadı");
                return Results.Json(new { satirlar = satirlar, yansitmaHesaplari = yansitmaHesaplari });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Yansıtma listesi hazırlanamadı."));
            }
        }

        private static void YansitmaToplamaEkle(List<OdemeEmriMIFDetay> toplamlar, string detayBirim, double borc, double alacak)
        {
            OdemeEmriMIFDetay od = toplamlar.FirstOrDefault(x => x.detayBirim == detayBirim);
            if (od == null)
            {
                od = new OdemeEmriMIFDetay(DateTime.Now.Year);
                od.detayBirim = detayBirim;
                toplamlar.Add(od);
            }
            od.borc += borc;
            od.alacak += alacak;
        }

        /// <summary>Yansıtma Yap (eski btnYansitmaYap_Click): eski yansıtma satırları atılır, seçilen yansıtmalar ve gider / gelir yansıtma
        /// karşılıkları eklenerek tablo yeniden kurulur.</summary>
        private static async Task<IResult> YansitmaYap(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, OemYansitmaIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new OemYansitmaIstegi();
            try
            {
                IMUHServis servisMUH = Servis();
                int yil = istek.Yil;
                string ekoKodYapi = TNS.MUH.Arac.DegiskenDegerBul(yil, "/KODYAPI/KOD4/KODYAPI") ?? "";
                Dictionary<int, string> isBilgisi = new Dictionary<int, string>();
                List<Dictionary<string, object>> liste = new List<Dictionary<string, object>>();

                foreach (OemSatir row in istek.Satirlar ?? new List<OemSatir>())
                {
                    string hesapKod = S(row, "HESAPKOD");
                    // önceki yansıtma satırları ve boş satırlar atlanır
                    if (string.IsNullOrEmpty(hesapKod) || hesapKod.StartsWith("800") || hesapKod.StartsWith("805") || hesapKod.StartsWith("830")
                        || hesapKod.StartsWith("835") || hesapKod.StartsWith("810"))
                        continue;
                    isBilgisi[OrtakFonksiyonlar.ConvertToInt(S(row, "ISKOD"), 0)] = S(row, "ISAD");
                    liste.Add(Satir(I(row, "KESINTI"), hesapKod, S(row, "HESAPAD"), S(row, "TERTIP"), S(row, "TERTIPPRG"), D(row, "BORC"), D(row, "ALACAK"),
                        S(row, "ALTHESAPKOD"), S(row, "SATIRACIKLAMA"), S(row, "DOVIZ"), S(row, "DOVIZKURTUR"), D(row, "DOVIZBORC"), D(row, "DOVIZALACAK"),
                        (decimal)D(row, "DOVIZKUR"), S(row, "MAHSUPBELGENO"), S(row, "DETAYBIRIM"), S(row, "ISKOD"), S(row, "ISAD")));
                }

                List<OdemeEmriMIFDetay> yansitmaHesaplari = new List<OdemeEmriMIFDetay>();
                List<OdemeEmriMIFDetay> toplamGider = new List<OdemeEmriMIFDetay>();
                List<OdemeEmriMIFDetay> toplamGelir = new List<OdemeEmriMIFDetay>();

                foreach (OemSatir row in istek.Yansitmalar ?? new List<OemSatir>())
                {
                    string yansitmaKod = S(row, "YANSITMA");
                    if (yansitmaKod == "") continue;
                    string detayBirim = S(row, "DETAYBIRIM");
                    double borc = D(row, "BORC");
                    double alacak = D(row, "ALACAK");
                    int isKod = OrtakFonksiyonlar.ConvertToInt(S(row, "ISKOD"), 0);

                    // tertibin ekonomik kodu yansıtma hesabından alınır
                    string tertip = S(row, "TERTIP");
                    if (tertip != "")
                    {
                        string[] t = tertip.Split('-');
                        int tire = yansitmaKod.IndexOf("-");
                        if (t.Length < 4 || tire < 4) return Basarisiz("Yansıtma hesabı hatalı: " + yansitmaKod);
                        string[] y = yansitmaKod.Substring(4, tire - 4).Split('.');
                        if (y.Length < 4) return Basarisiz("Yansıtma 4 kırılım değil, hesap hatalı:" + yansitmaKod);
                        string eko = y[0] + "." + y[1] + "." + y[2] + "." + y[3];
                        if (ekoKodYapi.Length == 9)
                            eko = y[0] + "." + OrtakFonksiyonlar.ConvertToInt(y[1], 0) + "." + OrtakFonksiyonlar.ConvertToInt(y[2], 0) + "." + y[3];
                        tertip = t[0] + "-" + t[1] + "-" + t[2] + "-" + eko;
                        if (t.Length > 4) tertip += "-" + t[4];
                    }

                    bool bulundu = false;
                    foreach (OdemeEmriMIFDetay od in yansitmaHesaplari)
                    {
                        if (od.detayBirim == detayBirim && od.hesapKod == yansitmaKod && TNS.MUH.Arac.TertipStrOl(od) == tertip && od.isKod == isKod)
                        {
                            od.borc += borc;
                            od.alacak += alacak;
                            bulundu = true;
                        }
                    }
                    if (!bulundu)
                    {
                        OdemeEmriMIFDetay yh = new OdemeEmriMIFDetay(DateTime.Now.Year);
                        yh.hesapKod = yansitmaKod;
                        yh.hesapAd = yansitmaKod;
                        TNS.MUH.Arac.TertipAta(ref yh, tertip);
                        yh.borc = borc;
                        yh.alacak = alacak;
                        yh.detayBirim = detayBirim;
                        yh.isKod = isKod;
                        yh.isAdi = isBilgisi.TryGetValue(isKod, out string isAd) ? isAd : "";
                        yansitmaHesaplari.Add(yh);
                    }

                    HesapPlaniSatir h = new HesapPlaniSatir();
                    h.yil = yil;
                    int tire2 = yansitmaKod.IndexOf("-");
                    h.hesapKod = (tire2 > 0 ? yansitmaKod.Substring(0, tire2) : yansitmaKod).Trim().Replace(".", "");
                    if (h.hesapKod == "") continue;
                    h = servisMUH.HesapKodOzellikAl(h);
                    if (h == null) continue;
                    if (TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.GIDERBUTCESI))
                        YansitmaToplamaEkle(toplamGider, detayBirim, borc, alacak);
                    if (TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.GELIRBUTCESI))
                        YansitmaToplamaEkle(toplamGelir, detayBirim, borc, alacak);
                }

                foreach (OdemeEmriMIFDetay od in yansitmaHesaplari)
                    liste.Add(Satir(0, od.hesapKod, od.hesapAd, TNS.MUH.Arac.TertipStrOl(od), "", od.borc, od.alacak, "", "", "", "", 0, 0, 0, "",
                        od.detayBirim, od.isKod, od.isAdi));

                // gider yansıtma karşılığı: borç alacağa, alacak borca
                KarsilikEkle(kullanan, servisMUH, yil, ENUMHesapKodNitelik.GIDERYANSITMA, toplamGider, liste, true);
                // gelir yansıtma karşılığı: alacak borca, borç alacağa (eski sırayla önce alacak)
                KarsilikEkle(kullanan, servisMUH, yil, ENUMHesapKodNitelik.GELIRYANSITMA, toplamGelir, liste, false);

                return Results.Json(new { basarili = true, satirlar = Numarala(liste) });
            }
            catch (Exception ex)
            {
                return Basarisiz(ex.Message);
            }
        }

        private static void KarsilikEkle(Kullanici kullanan, IMUHServis servisMUH, int yil, ENUMHesapKodNitelik nitelik, List<OdemeEmriMIFDetay> toplamlar,
            List<Dictionary<string, object>> liste, bool borcOnce)
        {
            if (toplamlar.Count == 0) return;
            HesapPlaniSatir hgy = new HesapPlaniSatir();
            hgy.yil = yil;
            hgy.nitelik = ((int)nitelik).ToString();
            ObjectArray hgyv = servisMUH.HesapPlaniListele(kullanan, hgy);
            if (hgyv == null || !hgyv.sonuc.islemSonuc) return;
            foreach (HesapPlaniSatir hh in hgyv.objeler)
            {
                foreach (OdemeEmriMIFDetay od in toplamlar)
                {
                    Dictionary<string, object> borcSatiri = od.borc > 0
                        ? Satir(0, hh.hesapKod + "-" + hh.aciklama, hh.aciklama, "", "", 0, od.borc, "", "", "", "", 0, 0, 0, "", od.detayBirim, od.isKod, od.isAdi) : null;
                    Dictionary<string, object> alacakSatiri = od.alacak > 0
                        ? Satir(0, hh.hesapKod + "-" + hh.aciklama, hh.aciklama, "", "", od.alacak, 0, "", "", "", "", 0, 0, 0, "", od.detayBirim, od.isKod, od.isAdi) : null;
                    if (borcOnce)
                    {
                        if (borcSatiri != null) liste.Add(borcSatiri);
                        if (alacakSatiri != null) liste.Add(alacakSatiri);
                    }
                    else
                    {
                        if (alacakSatiri != null) liste.Add(alacakSatiri);
                        if (borcSatiri != null) liste.Add(borcSatiri);
                    }
                }
            }
        }

        // ------------------------------------------------------------------ dosyadan yükle (eski wndDosyadanYukle)

        /// <summary>Excel'den satır yükler (eski DosyadanBelgeYukle). Sütunlar: hesap kodu, tertip, borç, alacak, alt hesap, açıklama,
        /// detay birim, mahsup belge no, döviz, kur türü, döviz borç, döviz alacak, kur. Arka arkaya 20'den çok boş satırda durur.</summary>
        private static async Task<IResult> DosyadanYukle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!baglam.Request.HasFormContentType) return Results.BadRequest();
            IFormCollection form = await baglam.Request.ReadFormAsync();
            IFormFile dosya = form.Files["dosya"];
            if (dosya == null) return Basarisiz("Lütfen ilk önce yüklemek istediğiniz excel dosyasını seçin.");
            if (dosya.Length <= 0) return Basarisiz("Lütfen ilk önce yüklemek istediğiniz excel dosyasını seçin.");
            if (dosya.Length > 20 * 1024 * 1024) return Basarisiz("Dosya çok büyük (en çok 20 MB).");

            string uzanti = Path.GetExtension(dosya.FileName ?? "");
            string geciciAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul() + (string.IsNullOrEmpty(uzanti) ? "." + GenelIslemler.ExcelTur() : uzanti);
            try
            {
                using (FileStream hedef = File.Create(geciciAd))
                    await dosya.CopyToAsync(hedef);

                AsposeLisans.Yukle();
                List<Dictionary<string, object>> liste = new List<Dictionary<string, object>>();
                Tablo XLS = GenelIslemler.NewTablo();
                XLS.DosyaOkuAc(geciciAd);
                try
                {
                    int satir = 0, bosSatir = 0;
                    while (bosSatir <= 20)
                    {
                        string hesapKod = (XLS.HucreDegerAl(satir, 0) ?? "").Trim();
                        if (hesapKod == "") { satir++; bosSatir++; continue; }
                        bosSatir = 0;
                        liste.Add(Satir(0, hesapKod, "", XLS.HucreDegerAl(satir, 1), "", XLS.HucreDegerAlDbl(satir, 2), XLS.HucreDegerAlDbl(satir, 3),
                            XLS.HucreDegerAl(satir, 4), XLS.HucreDegerAl(satir, 5), XLS.HucreDegerAl(satir, 8), XLS.HucreDegerAl(satir, 9),
                            XLS.HucreDegerAlDbl(satir, 10), XLS.HucreDegerAlDbl(satir, 11), OrtakFonksiyonlar.ConvertToDecimal(XLS.HucreDegerAlDbl(satir, 12)),
                            XLS.HucreDegerAl(satir, 7), XLS.HucreDegerAl(satir, 6), 0, ""));
                        satir++;
                    }
                }
                finally
                {
                    XLS.DosyaKapat();
                }

                int yuklenen = liste.Count;
                if (yuklenen == 0) return Basarisiz("Yüklemek istediğiniz dosyanın içi boş.");
                return Results.Json(new
                {
                    basarili = true,
                    bilgi = "Excel dosyası başarı ile yüklendi. Yüklenen satır sayısı : " + yuklenen,
                    satirlar = Numarala(liste)
                });
            }
            catch (Exception ex)
            {
                return Basarisiz(HataMetni(ex.Message, "Dosya okunamadı."));
            }
            finally
            {
                try { if (File.Exists(geciciAd)) File.Delete(geciciAd); } catch { }
            }
        }

        // ------------------------------------------------------------------ OYBS (eski OYBS sekmesi)

        internal static OybsIslemTipi OybsTip(int tip)
        {
            switch (tip)
            {
                case (int)OybsIslemTipi.FonCikisi: return OybsIslemTipi.FonCikisi;
                case (int)OybsIslemTipi.FonNemalandirmasi: return OybsIslemTipi.FonNemalandirmasi;
                default: return OybsIslemTipi.FonGirisi;
            }
        }

        private static string OybsPara(string tutar)
        {
            return OrtakFonksiyonlar.ConvertToDecimal(tutar).ToString("N2", new CultureInfo("tr-TR"));
        }

        private static string OybsTarih(DateTime t)
        {
            return t == DateTime.MinValue ? "" : t.ToString("dd.MM.yyyy HH:mm:ss");
        }

        private static object OybsSatir(OybsIslem islem)
        {
            string miktar = "", paraBirimi = "", tarih = "", kisiKurum = "";
            if (islem.islemTipi == OybsIslemTipi.FonGirisi)
            {
                miktar = OybsPara(islem.GirisMiktari); paraBirimi = islem.GirisParaBirimi; tarih = OybsTarih(islem.GirisTarihi); kisiKurum = islem.OdemeYapan;
            }
            else if (islem.islemTipi == OybsIslemTipi.FonCikisi)
            {
                miktar = OybsPara(islem.CikisMiktari); paraBirimi = islem.CikisBirimi; tarih = OybsTarih(islem.CikisTarihi); kisiKurum = islem.OdemeYapilan;
            }
            else if (islem.islemTipi == OybsIslemTipi.FonNemalandirmasi)
            {
                miktar = islem.NemalandirilanMiktar; paraBirimi = islem.NemaParaBirimi; tarih = OybsTarih(islem.VadeBaslangicTarihi);
            }
            return new
            {
                IDSET = ((int)islem.islemTipi) + "-" + islem.OybsId,
                ISLEMTIP = (int)islem.islemTipi,
                OYBSID = islem.OybsId.ToString(),
                MIKTAR = miktar ?? "",
                VADEMIKTAR = islem.VadeSonuToplamMiktar ?? "",
                PARABIRIMI = paraBirimi ?? "",
                TARIH = tarih,
                BITISTARIH = OybsTarih(islem.VadeBitisTarihi),
                KISIKURUM = kisiKurum ?? "",
                BANKAHESAP = islem.IlgiliBankaHesabi ?? "",
                AKTIFMI = islem.Aktif.ToString(),
                TANIM = islem.Tanim ?? ""
            };
        }

        /// <summary>OYBS kayıtları (eski ListeleOybs). Belgeye bağlı kayıt varsa onlar kilitli gelir; yoksa seçilen işlem tipinin
        /// muhasebeleşecek (belge yeni / değiştirildi) ya da muhasebeleşmiş kayıtları.</summary>
        private static IResult OybsListe(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string belgeNo, int? durum, int? islemTip)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                int belgeDurum = durum ?? (int)ENUMBelgeDurumu.YENI;
                bool muhasebelesecek = belgeDurum == (int)ENUMBelgeDurumu.YENI || belgeDurum == (int)ENUMBelgeDurumu.DEGISTIRILDI;
                List<object> satirlar = new List<object>();

                if (Temiz(belgeNo) != "")
                {
                    ObjectArray secili = Servis().OybsListele(kullanan, yil ?? 0, BelgeNoDuzenle(belgeNo));
                    if (secili != null && secili.ObjeSayisi > 0)
                    {
                        int tip = OrtakFonksiyonlar.ConvertToInt(secili.sonuc.anahtar, 0);
                        string oybs = "";
                        foreach (string item in secili.objeler)
                        {
                            oybs += tip + "-" + item + ";";
                            OybsIslem islem = OdemeEmriWebServisleri.Getir(OybsTip(tip), long.Parse(item));
                            if (islem != null) satirlar.Add(OybsSatir(islem));
                        }
                        return Results.Json(new { satirlar = satirlar, kilitli = true, islemTip = tip, oybs = oybs, listele = muhasebelesecek });
                    }
                }

                if (islemTip == null) return Results.Json(new { satirlar = satirlar, kilitli = false, listele = muhasebelesecek });
                List<OybsIslem> liste = OdemeEmriWebServisleri.KayitlariGetir(OybsTip(islemTip.Value),
                    muhasebelesecek ? OybsKayitTipi.MuhasebelesecekKayitlar : OybsKayitTipi.MuhasebelesmisKayitlar);
                foreach (OybsIslem islem in liste ?? new List<OybsIslem>())
                    satirlar.Add(OybsSatir(islem));
                return Results.Json(new { satirlar = satirlar, kilitli = false, islemTip = islemTip.Value, listele = muhasebelesecek });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "OYBS kayıtları okunamadı."));
            }
        }

        // ------------------------------------------------------------------ ekli dokümanlar (eski Ekli Dokümanlar sekmesi)

        private static string DokumanYol()
        {
            return Path.Combine(Ayar.Oku("MuhasebeDosyaYol") ?? "", "MuhasebeBelgesi");
        }

        private static IResult DokumanListe(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string belgeNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            List<object> satirlar = new List<object>();
            if (Temiz(belgeNo) == "") return Results.Json(new { satirlar = satirlar });
            try
            {
                Dokuman dokuman = new Dokuman();
                dokuman.yil = yil ?? 0;
                dokuman.belgeNo = BelgeNoDuzenle(belgeNo);
                ObjectArray belgeler = Servis().DokumanListele(kullanan, dokuman);
                if (belgeler == null || !belgeler.sonuc.islemSonuc) return Hata(HataMetni(belgeler?.sonuc.hataStr, "Dokümanlar okunamadı."));
                foreach (Dokuman d in belgeler.objeler)
                    satirlar.Add(new { YIL = d.yil, BELGENO = d.belgeNo ?? "", SIRANO = d.siraNo, DOKUMANADI = d.dokumanAdi ?? "" });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Dokümanlar okunamadı."));
            }
        }

        private static IResult DokumanIndir(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string belgeNo, int? siraNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil == null || Temiz(belgeNo) == "" || siraNo == null) return Hata("Doküman seçiniz.");
            try
            {
                Dokuman dokuman = new Dokuman();
                dokuman.yil = yil.Value;
                dokuman.belgeNo = belgeNo.Trim();
                dokuman.siraNo = siraNo.Value;
                ObjectArray dokumanlar = Servis().DokumanListele(kullanan, dokuman);
                if (dokumanlar == null || !dokumanlar.sonuc.islemSonuc) return Hata(HataMetni(dokumanlar?.sonuc.hataStr, "Doküman okunamadı."));
                foreach (Dokuman d in dokumanlar.objeler)
                {
                    string dosya = Path.Combine(DokumanYol(), dokuman.yil.ToString(), dokuman.yil + "_" + dokuman.belgeNo + "_" + dokuman.siraNo);
                    byte[] icerik = File.Exists(dosya) ? File.ReadAllBytes(dosya) : d.dokumanIcerik;
                    if (icerik == null) return Hata("Doküman bulunamadı.");
                    string ad = Path.GetFileName(d.dokumanAdi ?? "dokuman");
                    string uzanti = ad.Contains(".") ? ad.Substring(ad.LastIndexOf('.') + 1) : GenelIslemler.ExcelTur();
                    string gecici = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
                    File.WriteAllBytes(gecici, icerik);
                    string adUzantisiz = ad.Contains(".") ? ad.Substring(0, ad.LastIndexOf('.')) : ad;
                    return DosyaIslem.DosyaGonder(gecici, adUzantisiz, true, uzanti);
                }
                return Hata("Doküman bulunamadı.");
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Doküman okunamadı."));
            }
        }

        private static async Task<IResult> DokumanYukle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!baglam.Request.HasFormContentType) return Results.BadRequest();
            IFormCollection form = await baglam.Request.ReadFormAsync();
            string belgeNo = Temiz(form["belgeNo"]);
            int yil = OrtakFonksiyonlar.ConvertToInt(form["yil"].ToString(), 0);
            if (belgeNo == "") return Basarisiz("Belgeyi kayıt ettikten sonra dokuman ekleme işlemini yapın.");
            IFormFile dosya = form.Files["dosya"];
            if (dosya == null || dosya.Length == 0) return Basarisiz("Yüklemek için bir dosya seçiniz.");
            if (dosya.Length > 50 * 1024 * 1024) return Basarisiz("Dosya çok büyük (en çok 50 MB).");
            try
            {
                Dokuman dokuman = new Dokuman();
                dokuman.yil = yil;
                dokuman.belgeNo = BelgeNoDuzenle(belgeNo);
                dokuman.dokumanAdi = Path.GetFileName(dosya.FileName);
                using (MemoryStream bellek = new MemoryStream())
                {
                    await dosya.CopyToAsync(bellek);
                    dokuman.dokumanIcerik = bellek.ToArray();
                }
                dokuman.kayitEdilecekDosyaYol = DokumanYol();
                Sonuc sonuc = Servis().DokumanKaydet(kullanan, dokuman);
                return sonuc.islemSonuc ? Basari("Doküman eklendi.") : Basarisiz(HataMetni(sonuc.hataStr, "Doküman eklenemedi."));
            }
            catch (Exception ex)
            {
                return Basarisiz(HataMetni(ex.Message, "Doküman eklenemedi."));
            }
        }

        private static async Task<IResult> DokumanSil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, OemBelgeIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new OemBelgeIstegi();
            if (istek.Yil <= 0 || Temiz(istek.BelgeNo) == "" || istek.SiraNo <= 0) return Basarisiz("Belge seçiniz.");
            try
            {
                Dokuman dokuman = new Dokuman();
                dokuman.yil = istek.Yil;
                dokuman.belgeNo = istek.BelgeNo.Trim();
                dokuman.siraNo = istek.SiraNo;
                dokuman.kayitEdilecekDosyaYol = DokumanYol();
                Sonuc sonuc = Servis().DokumanSil(kullanan, dokuman);
                return sonuc.islemSonuc ? Basari("Doküman silindi.") : Basarisiz(HataMetni(sonuc.hataStr, "Doküman silinemedi."));
            }
            catch (Exception ex)
            {
                return Basarisiz(HataMetni(ex.Message, "Doküman silinemedi."));
            }
        }
    }
}
