using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
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
    /// <summary>XML oluşturma, Excel'e yazdırma ve sürüm saklama isteği. satirlar: tablodaki bütün satırlar (eski AllGridRows).</summary>
    public class VergiAktarimIstegi
    {
        public int tur { get; set; }
        public int yil { get; set; }
        public int ay { get; set; }
        public bool yevmiyeli { get; set; }
        public string surum { get; set; }
        public List<Dictionary<string, JsonElement>> satirlar { get; set; }
    }

    /// <summary>Beyannamedeki mükellef bilgileri (kurum değişkenleri /VERGIBEYAN/*).</summary>
    public class VergiMukellef
    {
        public string vergiNo { get; set; }
        public string vdKodu { get; set; }
        public string soyadi { get; set; }
        public string adi { get; set; }
        public string alanKodu { get; set; }
        public string telNo { get; set; }
    }

    public class VergiMukellefIstegi
    {
        public int tur { get; set; }
        public VergiMukellef mukellef { get; set; }
    }

    /// <summary>
    /// Vergi dairesi aktarım sayfalarının ortak uçları. Eski dört sayfa (ButceMuhasebe/DamgaVergisiAktarimi, KDVTevkifatiAktarimi,
    /// KDVAktarimi, MuhtasarAktarimi .aspx) aynı ekrandı: Yıl / Ay / Yevmiyeli kıstaslarıyla büyük defterden satırlar listelenir,
    /// tablodan beyanname XML'i (Kaydet) ve Excel listesi (Yazdır) üretilir, XML sürümü "stratek" kullanıcı değişkeninde saklanır.
    /// Türe göre değişen yerler (hesap kodları, satır hesabı, XML gövdesi, Excel sütunları) eski sayfaların kodu birebir alınarak
    /// tur ile ayrılır. tur = ENUMVergiDairesiAktarim: 1 Damga Vergisi, 2 KDV Tevkifatı, 3 KDV, 4 Muhtasar.
    ///   GET  /api/vergiaktarim/tanim?tur                        eski Page_Load (Ay/Yıl, sürüm)
    ///   GET  /api/vergiaktarim/liste?tur&amp;yil&amp;ay&amp;yevmiyeli     eski btnListe_Click (Listele)
    ///   POST /api/vergiaktarim/xml                              eski btnKaydet_Click (beyanname XML dosyası)
    ///   POST /api/vergiaktarim/yazdir                           eski btnListeYazdir_Click (MUH-*Aktarimi.xlt, Excel)
    ///   POST /api/vergiaktarim/surum                            eski btnXMLKaydet_Click (sürümü sakla)
    ///   POST /api/vergiaktarim/mukellef                         mükellef bilgilerini kurum değişkenlerine saklar
    /// Mükellef bilgileri (vergi no, vergi dairesi kodu, unvan, telefon) eski sayfalarda EGO için koda yazılıydı; artık kurum
    /// değişkenlerinden (TNS.DEG, yıl 0, /VERGIBEYAN/VERGINO, VDKODU, SOYADI, ADI, ALANKODU, TELNO) okunur ve sayfadan saklanır.
    /// Muhtasar'daki EGO'ya ait sabit SGK işyeri bilgileri (ekler) kaldırıldı.
    /// İş kuralları servisi (IMUHServis.BuyukDefterListele, DamgaVergisiAktarimTipiListele, KDVBilgisiBul, OdemeEmriMIFTekSatirOku;
    /// IHRCServis.FirmaBilgisiListele) değişmeden çağrılır. Yetki: türün menüdeki sayfa adresi.
    /// </summary>
    public static class VergiAktarimUclari
    {
        public const int DAMGA = (int)ENUMVergiDairesiAktarim.DAMGAVERGISI;
        public const int TEVKIFAT = (int)ENUMVergiDairesiAktarim.KDVTEFKIFATI;
        public const int KDV = (int)ENUMVergiDairesiAktarim.KDV;
        public const int MUHTASAR = (int)ENUMVergiDairesiAktarim.MUHTASAR;

        private const string Uc = "/api/vergiaktarim/";
        private const string SurumKullanici = "stratek"; // eski sayfalar sürümü bu kullanıcının değişkeninde saklıyordu

        /// <summary>Türe göre değişen sabitler (eski sayfalardaki değerler).</summary>
        private class TurBilgi
        {
            public string SayfaAdres;
            public string Baslik;
            public string Sablon;
            public string SurumDegisken;
            public string HesapKod;
            public string KodVer;
            public string DosyaEk;
        }

        private static readonly Dictionary<int, TurBilgi> Turler = new Dictionary<int, TurBilgi>
        {
            { DAMGA, new TurBilgi { SayfaAdres = "../ButceMuhasebe/DamgaVergisiAktarimi.aspx", Baslik = "Damga Vergisi Aktarımı", Sablon = "MUH-DamgaVergisiAktarimi.xlt",
                SurumDegisken = "DamgaVergisiVersion", HesapKod = "360.03.02;360.03.03;360.03.09", KodVer = "DAMGA_", DosyaEk = "_DAMGAVERGISI_" } },
            { TEVKIFAT, new TurBilgi { SayfaAdres = "../ButceMuhasebe/KDVTevkifatiAktarimi.aspx", Baslik = "KDV Tevkifatı Aktarımı", Sablon = "MUH-KDVTevkifatiAktarimi.xlt",
                SurumDegisken = "KDVTevkifatiVersion", HesapKod = "360.04.02.06;360.04.02.07;360.04.02.08;360.04.02.09;360.04.02.11", KodVer = "KDV2_", DosyaEk = "_KDV2_" } },
            { KDV, new TurBilgi { SayfaAdres = "../ButceMuhasebe/KDVAktarimi.aspx", Baslik = "KDV Aktarımı", Sablon = "MUH-KDVAktarimi.xlt",
                SurumDegisken = "KDVVersion", HesapKod = "391.01;391.02;391.03", KodVer = "KDV1_", DosyaEk = "_KDV1_" } },
            { MUHTASAR, new TurBilgi { SayfaAdres = "../ButceMuhasebe/MuhtasarAktarimi.aspx", Baslik = "Muhtasar Aktarımı", Sablon = "MUH-MuhtasarAktarimi.xlt",
                SurumDegisken = "MuhtasarVersion", HesapKod = "360.01.02.02;360.01.03", KodVer = "MUH_", DosyaEk = "_MUH_" } }
        };

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "tanim", Tanim);
            app.MapGet(Uc + "liste", Liste);
            app.MapPost(Uc + "xml", Xml);
            app.MapPost(Uc + "yazdir", Yazdir);
            app.MapPost(Uc + "surum", Surum);
            app.MapPost(Uc + "mukellef", MukellefSakla);
        }

        public static string Baslik(int tur)
        {
            return Turler.TryGetValue(tur, out TurBilgi b) ? b.Baslik : "Vergi Dairesi Aktarımı";
        }

        public static bool SayfaYetkili(Kullanici kullanan, string uygulamaKlasoru, int tur)
        {
            if (kullanan == null || !Turler.TryGetValue(tur, out TurBilgi b)) return false;
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, b.SayfaAdres);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, int tur, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!Turler.ContainsKey(tur)) return Results.BadRequest();
            if (!SayfaYetkili(kullanan, ortam.ContentRootPath, tur)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IMUHServis Servis() { return TNS.MUH.Arac.Tanimla(); }

        private static IResult Hata(string hata) { return Results.Json(new { hata = Duz(hata) }); }

        private static string Duz(string s)
        {
            return (s ?? "").Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<br />", "\n").Trim();
        }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Trim();
            if (hata == "" || hata.Contains("   at ")) return varsayilan;
            return Duz(hata);
        }

        private static string Q(HttpContext baglam, string ad)
        {
            return ((string)baglam.Request.Query[ad] ?? "").Trim();
        }

        private static int QSayi(HttpContext baglam, string ad)
        {
            int.TryParse(Q(baglam, ad), out int d);
            return d;
        }

        private static bool Evet(HttpContext baglam, string ad)
        {
            string d = Q(baglam, ad);
            return d == "1" || d.Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        // ------------------------------------------------------------------ tablodan gelen satır değerleri (eski JObject.Value<T>)

        /// <summary>Satırdaki değerin metni; sayılar tarayıcının yazdığı gibi (ör. 1234.5) gelir, eski Value&lt;string&gt; ile aynı.</summary>
        private static string M(Dictionary<string, JsonElement> r, string ad)
        {
            if (r == null || !r.TryGetValue(ad, out JsonElement e)) return null;
            switch (e.ValueKind)
            {
                case JsonValueKind.String: return e.GetString();
                case JsonValueKind.Null:
                case JsonValueKind.Undefined: return null;
                case JsonValueKind.True: return "True";
                case JsonValueKind.False: return "False";
                default: return e.GetRawText();
            }
        }

        /// <summary>Sayı (eski ConvertToDbl(Value&lt;string&gt;.Replace(".", ","))); yoksa 0.</summary>
        private static double D(Dictionary<string, JsonElement> r, string ad)
        {
            string s = M(r, ad);
            if (string.IsNullOrWhiteSpace(s)) return 0.0;
            double.TryParse(s.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double d);
            return d;
        }

        private static int I(Dictionary<string, JsonElement> r, string ad)
        {
            string s = M(r, ad);
            if (string.IsNullOrWhiteSpace(s)) return 0;
            if (int.TryParse(s.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int i)) return i;
            double.TryParse(s.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double d);
            return (int)d;
        }

        /// <summary>ISLEMTARIH (eski Value&lt;DateTime&gt;).</summary>
        private static DateTime T(Dictionary<string, JsonElement> r, string ad)
        {
            string s = M(r, ad);
            if (!string.IsNullOrWhiteSpace(s) && DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime t)) return t;
            return DateTime.MinValue;
        }

        // ------------------------------------------------------------------ açılış (eski Page_Load: AyYilDoldur, sürüm)

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            int tur = QSayi(baglam, "tur");
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici _);
            if (ret != null) return ret;
            List<int> yillar = new List<int>();
            for (int i = 2006; i <= DateTime.Now.Year; i++) yillar.Add(i);
            return Results.Json(new
            {
                yil = DateTime.Now.Year,
                ay = DateTime.Now.Month,
                yillar = yillar,
                surum = GenelIslemler.KullaniciDegiskenGetir(SurumKullanici, Turler[tur].SurumDegisken),
                mukellef = MukellefOku()
            });
        }

        // ------------------------------------------------------------------ büyük defter okuma (eski sayfalarda tekrarlanan blok)

        /// <summary>Ayın ilk ve son günü (eski: "01." + ay + "." + yil; t2.AddMonths(1); t2.AddDays(-1)).</summary>
        private static void AyAraligi(int yil, int ay, out TNSDateTime t1, out TNSDateTime t2)
        {
            t1 = new TNSDateTime("01." + ay + "." + yil);
            t2 = new TNSDateTime("01." + ay + "." + yil);
            t2.AddMonths(1);
            t2.AddDays(-1);
        }

        private static List<BuyukDefter> BuyukDefterOku(Kullanici kullanan, int yil, TNSDateTime t1, TNSDateTime t2, bool yevmiyeli, string hesapKod)
        {
            OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            oEmri.yil = yil;
            if (yevmiyeli)
            {
                oEmri.formDurum = 24;
                oEmri.sorguYEVYevTarih = t1.Oku().ToString() + ";" + t2.Oku().ToString();
            }
            else
            {
                oEmri.formDurum = 22;
                oEmri.sorguIslemTarih = t1.Oku().ToString() + ";" + t2.Oku().ToString();
            }
            oEmri.sorguEkoSiraliMi = true;

            OdemeEmriMIFDetay oEmriDetay = new OdemeEmriMIFDetay(oEmri.yil);
            oEmriDetay.hesapKod = hesapKod.Replace(".", "").Trim();
            oEmri.detay.objeler.Add(oEmriDetay);

            List<BuyukDefter> mbler = new List<BuyukDefter>();
            BuyukDefter[] mb = Servis().BuyukDefterListele(kullanan, oEmri, 0, 3000, "10");
            if (mb != null)
                for (int ii = 0; ii < mb.Length; ii++)
                    mbler.Add(mb[ii]);
            return mbler;
        }

        private static List<BuyukDefter> AyinBuyukDefteri(Kullanici kullanan, int yil, int ay, bool yevmiyeli, string hesapKod)
        {
            AyAraligi(yil, ay, out TNSDateTime t1, out TNSDateTime t2);
            return BuyukDefterOku(kullanan, yil, t1, t2, yevmiyeli, hesapKod);
        }

        private static ObjectArray AktarimTipleri(Kullanici kullanan, int tur, string hesapKod)
        {
            VergiDairesiAktarimTurTanim tanimAktarim = new VergiDairesiAktarimTurTanim();
            tanimAktarim.aktarimTurKod = tur;
            tanimAktarim.hesapKod = hesapKod.Replace(".", "").Trim();
            return Servis().DamgaVergisiAktarimTipiListele(kullanan, tanimAktarim);
        }

        /// <summary>İlgili adını son boşluktan ad / soyad olarak ayırır (eski sayfalardaki aynı kod).</summary>
        private static void AdSoyadAyir(string ilgiliAd, out string adi, out string soyadi)
        {
            adi = ilgiliAd ?? "";
            soyadi = "";
            int boslukYer = adi.Trim().LastIndexOf(" ");
            if (boslukYer > -1)
            {
                soyadi = adi.Substring(boslukYer).Trim();
                adi = adi.Substring(0, boslukYer).Trim();
            }
        }

        // ------------------------------------------------------------------ listele (eski Listele)

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam)
        {
            int tur = QSayi(baglam, "tur");
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;
            int yil = QSayi(baglam, "yil");
            int ay = QSayi(baglam, "ay");
            if (yil <= 0 || ay < 1 || ay > 12) return Hata("Yıl ve ay seçiniz.");
            bool yevmiyeli = Evet(baglam, "yevmiyeli");
            try
            {
                List<object> al;
                if (tur == DAMGA) al = DamgaListele(kullanan, yil, ay, yevmiyeli);
                else if (tur == TEVKIFAT) al = TevkifatListele(kullanan, yil, ay, yevmiyeli);
                else if (tur == KDV) al = KDVListele(kullanan, yil, ay, yevmiyeli);
                else al = MuhtasarListele(kullanan, yil, ay, yevmiyeli);
                return Results.Json(new { satirlar = al });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Liste okunamadı."));
            }
        }

        /// <summary>Eski DamgaVergisiAktarimi.Listele.</summary>
        private static List<object> DamgaListele(Kullanici kullanan, int yil, int ay, bool yevmiyeli)
        {
            string hesapKod = Turler[DAMGA].HesapKod;
            List<BuyukDefter> mbler = AyinBuyukDefteri(kullanan, yil, ay, yevmiyeli, hesapKod);
            ObjectArray dvler = AktarimTipleri(kullanan, DAMGA, hesapKod);

            List<object> al = new List<object>();
            foreach (BuyukDefter item in mbler)
            {
                foreach (VergiDairesiAktarimTurTanim dv in dvler.objeler)
                {
                    if (item.alacak == 0) continue;

                    if (dv.hesapKod == item.hesapKod.Replace(".", ""))
                    {
                        al.Add(new
                        {
                            ISLEMTARIH = item.islemTarih.Oku(),
                            BELGENO = item.belgeNo,
                            TURKOD = dv.turKod,
                            ALTHESAPKOD = item.altHesapAd,
                            ALACAK = item.alacak,
                            ICERDIGIPARA = IcerdigiParaBol(item.alacak, dv.orani)
                        });
                        break;
                    }
                }
            }
            return al;
        }

        /// <summary>Eski KDVTevkifatiAktarimi.Listele.</summary>
        private static List<object> TevkifatListele(Kullanici kullanan, int yil, int ay, bool yevmiyeli)
        {
            string hesapKod = Turler[TEVKIFAT].HesapKod;
            List<BuyukDefter> mbler = AyinBuyukDefteri(kullanan, yil, ay, yevmiyeli, hesapKod);
            ObjectArray dvler = AktarimTipleri(kullanan, TEVKIFAT, hesapKod);

            List<object> al = new List<object>();
            double matrah = 0.0;
            foreach (BuyukDefter item in mbler)
            {
                foreach (VergiDairesiAktarimTurTanim dv in dvler.objeler)
                {
                    if (item.alacak == 0) continue;
                    double kdvTutar = 0.0;
                    int kdvOran = 0;
                    matrah = TevkifatMatrahBul(kullanan, item.yil, item.belgeNo, item.satirno, out kdvOran, out kdvTutar);

                    AdSoyadAyir(item.ilgiliAd, out string adi, out string soyadi);

                    string vNo = "";
                    string tcNo = "";
                    if (item.ilgiliNo.Length == 10)
                        vNo = item.ilgiliNo;
                    else
                        tcNo = item.ilgiliNo;

                    if (dv.hesapKod == item.hesapKod.Replace(".", ""))
                    {
                        al.Add(new
                        {
                            ISLEMTARIH = item.islemTarih.Oku(),
                            BELGENO = item.belgeNo,
                            TURKOD = dv.turKod,
                            VERGIORAN = dv.adi,
                            ADI = adi,
                            SOYADI = soyadi,
                            TCNO = tcNo,
                            VNO = vNo,
                            ALTHESAPKOD = item.altHesapAd,
                            ALACAK = item.alacak,
                            MATRAH = matrah,
                            KDVORAN = kdvOran,
                            KDVTUTAR = kdvTutar,
                            ICERDIGIPARA = IcerdigiParaCarp(item.alacak, dv.orani)
                        });
                        break;
                    }
                }
            }
            return al;
        }

        /// <summary>Eski KDVTevkifatiAktarimi.MatrahBul: belgenin 191 satırından KDV oranı ve tutarı, bir önceki satırdan matrah.</summary>
        private static double TevkifatMatrahBul(Kullanici kullanan, int yil, string belgeNo, int satirNo, out int kdvOran, out double kdvTutar)
        {
            OdemeEmriMIFDetay detayKDV = Servis().KDVBilgisiBul(kullanan, yil, belgeNo, satirNo, "191");
            OdemeEmriMIFDetay matrahBilgisi = Servis().OdemeEmriMIFTekSatirOku(kullanan, yil, belgeNo, detayKDV.satirNo - 1);
            double matrah = 0.0;
            kdvOran = 0;
            kdvTutar = detayKDV.borc;
            if (detayKDV.hesapKod != "")
            {
                if (detayKDV.hesapKod.Replace(".", "") == "19101")
                    kdvOran = 18;
                else if (detayKDV.hesapKod.Replace(".", "") == "19102")
                    kdvOran = 8;
                else if (detayKDV.hesapKod.Replace(".", "") == "19103")
                    kdvOran = 1;

                matrah = matrahBilgisi.borc;
            }
            return matrah;
        }

        /// <summary>Eski KDVAktarimi.Listele.</summary>
        private static List<object> KDVListele(Kullanici kullanan, int yil, int ay, bool yevmiyeli)
        {
            string hesapKod = Turler[KDV].HesapKod;
            List<BuyukDefter> mbler = AyinBuyukDefteri(kullanan, yil, ay, yevmiyeli, hesapKod);
            ObjectArray dvler = AktarimTipleri(kullanan, KDV, hesapKod);

            List<object> al = new List<object>();
            double matrah = 0.0;
            foreach (BuyukDefter item in mbler)
            {
                foreach (VergiDairesiAktarimTurTanim dv in dvler.objeler)
                {
                    //mahsup kaydı alınmamalı. Düzeltme kayıtları alınmalı. Burası için bir kontrol konulmalı.
                    //Kaydın mahsup veya düzeltme olduğu nasıl anlaşılacak
                    if (item.borc > 0 && item.ilgiliAd.IndexOf("VERGİ") > -1) continue;
                    double kdvTutar = 0.0;
                    int kdvOran = 0;
                    matrah = KDVMatrahBul(out kdvOran, out kdvTutar, item.hesapKod, item.borc, item.alacak);

                    AdSoyadAyir(item.ilgiliAd, out string adi, out string soyadi);

                    string vNo = "";
                    string tcNo = "";
                    if (item.ilgiliNo.Length == 10)
                        vNo = item.ilgiliNo;
                    else
                        tcNo = item.ilgiliNo;

                    if (dv.hesapKod == item.hesapKod.Replace(".", ""))
                    {
                        al.Add(new
                        {
                            ISLEMTARIH = item.islemTarih.Oku(),
                            BELGENO = item.belgeNo,
                            TURKOD = dv.turKod,
                            VERGIORAN = dv.adi,
                            ADI = adi,
                            SOYADI = soyadi,
                            TCNO = tcNo,
                            VNO = vNo,
                            ALTHESAPKOD = item.altHesapAd,
                            ALACAK = item.alacak - item.borc,
                            MATRAH = matrah,
                            KDVORAN = kdvOran,
                            KDVTUTAR = kdvTutar,
                            ICERDIGIPARA = IcerdigiParaCarp(item.alacak, dv.orani)
                        });
                        break;
                    }
                }
            }
            return al;
        }

        /// <summary>Eski KDVAktarimi.MatrahBul: 191 borç / 391 alacak; oran hesap kodundan (01: 18, 02: 8, 03: 1).</summary>
        private static double KDVMatrahBul(out int kdvOran, out double kdvTutar, string hesapKod, double borc, double alacak)
        {
            double matrah = 0.0;
            kdvOran = 0;
            if (hesapKod.StartsWith("191"))//191 Borç--391 Alacak
                kdvTutar = borc - alacak;
            else if (hesapKod.StartsWith("391"))//191 Borç--391 Alacak
                kdvTutar = alacak - borc;
            else
                kdvTutar = alacak;

            if (hesapKod.Replace(".", "") == "39101" || hesapKod.Replace(".", "") == "19101")
                kdvOran = 18;
            else if (hesapKod.Replace(".", "") == "39102" || hesapKod.Replace(".", "") == "19102")
                kdvOran = 8;
            else if (hesapKod.Replace(".", "") == "39103" || hesapKod.Replace(".", "") == "19103")
                kdvOran = 1;

            matrah = kdvTutar * (100.00 / kdvOran);

            return matrah;
        }

        /// <summary>Eski MuhtasarAktarimi.Listele (adres firma bilgisinden).</summary>
        private static List<object> MuhtasarListele(Kullanici kullanan, int yil, int ay, bool yevmiyeli)
        {
            string hesapKod = Turler[MUHTASAR].HesapKod;
            List<BuyukDefter> mbler = AyinBuyukDefteri(kullanan, yil, ay, yevmiyeli, hesapKod);
            ObjectArray dvler = AktarimTipleri(kullanan, MUHTASAR, hesapKod);
            TNS.HRC.IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();

            List<object> al = new List<object>();
            double matrah = 0.0;
            foreach (BuyukDefter item in mbler)
            {
                foreach (VergiDairesiAktarimTurTanim dv in dvler.objeler)
                {
                    //mahsup kaydı alınmamalı. Düzeltme kayıtları alınmalı. Burası için bir kontrol konulmalı.
                    //Kaydın mahsup veya düzeltme olduğu nasıl anlaşılacak
                    if (item.borc > 0 && item.ilgiliAd.IndexOf("VERGİ") > -1) continue;
                    double tutar = 0.0;
                    matrah = MuhtasarMatrahBul(out tutar, item.hesapKod, item.borc, item.alacak);

                    AdSoyadAyir(item.ilgiliAd, out string adi, out string soyadi);
                    string adres = "";

                    TNS.HRC.FirmaBilgisi b = new TNS.HRC.FirmaBilgisi();
                    b.muhasebe = item.muhasebe;
                    b.vergiNo = item.ilgiliNo;
                    if (b.vergiNo.Trim() != "")
                    {
                        ObjectArray listeFirma = servisHRC.FirmaBilgisiListele(kullanan, b);
                        if (listeFirma.ObjeSayisi > 0)
                        {
                            TNS.HRC.FirmaBilgisi firma = (TNS.HRC.FirmaBilgisi)listeFirma.objeler[0];
                            adres = firma.adres;
                        }
                    }

                    if (dv.hesapKod == item.hesapKod.Replace(".", ""))
                    {
                        al.Add(new
                        {
                            ISLEMTARIH = item.islemTarih.Oku(),
                            BELGENO = item.belgeNo,
                            TURKOD = dv.turKod.ToString("000"),
                            HESAPKOD = item.hesapKod,
                            VERGIORAN = dv.adi,
                            ADI = adi,
                            SOYADI = soyadi,
                            TCNO = item.ilgiliNo,
                            ADRES = adres,
                            ODEMETUR = "0",
                            ODEMETARIH = "",
                            ODEMENO = "",
                            ALACAK = item.alacak - item.borc,
                            MATRAH = matrah,
                            TUTAR = tutar,
                        });
                        break;
                    }
                }
            }
            return al;
        }

        /// <summary>Eski MuhtasarAktarimi.MatrahBul: 360.01.02.02 için %20, 360.01.03 için %3.</summary>
        private static double MuhtasarMatrahBul(out double tutar, string hesapKod, double borc, double alacak)
        {
            double matrah = 0.0;
            double oran = 0.0;

            hesapKod = hesapKod.Replace(".", "");

            if (hesapKod.StartsWith("360010202"))
                tutar = alacak - borc;
            else if (hesapKod.StartsWith("3600103"))
                tutar = alacak - borc;
            else
                tutar = alacak;

            if (hesapKod.Replace(".", "") == "360010202")
                oran = 20;
            else if (hesapKod.Replace(".", "") == "3600103")
                oran = 3;

            matrah = tutar * (100.00 / oran);

            return matrah;
        }

        /// <summary>Eski KDV / KDV Tevkifatı IcerdigiParaHesapla (alacak * oran).</summary>
        private static decimal IcerdigiParaCarp(double alacak, decimal oran)
        {
            decimal b = 0;
            decimal a = Convert.ToDecimal(alacak);
            if (a != 0 && oran != 0)
                b = Math.Round(a * oran, 2);
            return b;
        }

        /// <summary>Eski Damga Vergisi IcerdigiParaHesapla (alacak / oran).</summary>
        private static decimal IcerdigiParaBol(double alacak, decimal oran)
        {
            decimal b = 0;
            decimal a = Convert.ToDecimal(alacak);
            if (a != 0 && oran != 0)
                b = Math.Round(a / oran, 2);
            return b;
        }

        /// <summary>
        /// Muhtasar ve KDV beyanında hesap toplamı (eski TutarVer). borcAlacak: 0 borç-alacak, 1 borç, 2 alacak, 4 alacak-borç.
        /// KDV'de 190 hesabı yılbaşından okunur; Muhtasar'da 360.03.01 ve 360.01.01.02 için VERGİ adlı borç satırları alınmaz.
        /// </summary>
        private static double TutarVer(Kullanici kullanan, int tur, int yil, int ay, bool yevmiyeli, string hesapKod, int borcAlacak)
        {
            AyAraligi(yil, ay, out TNSDateTime t1, out TNSDateTime t2);
            if (tur == KDV && hesapKod.StartsWith("190"))
            {
                t1 = new TNSDateTime("01.01." + yil);
                t2.AddDays(-1);
            }

            List<BuyukDefter> mbler = BuyukDefterOku(kullanan, yil, t1, t2, yevmiyeli, hesapKod);

            double islemTutar = 0.0;
            foreach (BuyukDefter item in mbler)
            {
                if (tur == MUHTASAR)
                {
                    if (hesapKod == "360.03.01" && item.borc > 0 && item.ilgiliAd.IndexOf("VERGİ") > -1) continue;
                    if (hesapKod == "360.01.01.02" && item.borc > 0 && item.ilgiliAd.IndexOf("VERGİ") > -1) continue;
                }

                if (borcAlacak == 0)
                    islemTutar += item.borc - item.alacak;
                else if (borcAlacak == 1)
                    islemTutar += item.borc;
                else if (borcAlacak == 2)
                    islemTutar += item.alacak;
                else if (borcAlacak == 4 && tur == MUHTASAR)
                    islemTutar += item.alacak - item.borc;
            }

            return islemTutar;
        }

        /// <summary>Eski KDVAktarimi.ListeleAktarim: 191 (indirilecek) ya da 391 (hesaplanan) KDV'yi orana göre toplar.</summary>
        private static List<GIBAktarim> KDVListeleAktarim(Kullanici kullanan, int yil, int ay, bool yevmiyeli, string tur, bool yilbasindan)
        {
            string hesapKod = tur == "191" ? "191.01;191.02;191.03" : "391.01;391.02;391.03";

            AyAraligi(yil, ay, out TNSDateTime t1, out TNSDateTime t2);
            if (yilbasindan)
                t1 = new TNSDateTime("01.01." + yil);

            List<BuyukDefter> mbler = BuyukDefterOku(kullanan, yil, t1, t2, yevmiyeli, hesapKod);

            List<GIBAktarim> liste = new List<GIBAktarim>();
            double matrah = 0.0;
            foreach (BuyukDefter item in mbler)
            {
                //mahsup kaydı alınmamalı. Düzeltme kayıtları alınmalı. Burası için bir kontrol konulmalı.
                //Kaydın mahsup veya düzeltme olduğu nasıl anlaşılacak
                if (tur == "191")
                {
                    if (item.alacak > 0 && item.ilgiliAd.IndexOf("VERGİ") > -1) continue;
                }
                else
                {
                    if (item.borc > 0 && item.ilgiliAd.IndexOf("VERGİ") > -1) continue;
                }
                double kdvTutar = 0.0;
                int kdvOran = 0;
                matrah = KDVMatrahBul(out kdvOran, out kdvTutar, item.hesapKod, item.borc, item.alacak);

                bool bulundu = false;
                foreach (GIBAktarim aktarim in liste)
                {
                    if (aktarim.oran == kdvOran)
                    {
                        aktarim.matrah += matrah;
                        aktarim.alacak += kdvTutar;
                        aktarim.oran = kdvOran;
                        bulundu = true;
                    }
                }

                if (!bulundu)
                {
                    GIBAktarim aktarimDetay = new GIBAktarim();
                    aktarimDetay.matrah = matrah;
                    aktarimDetay.alacak = kdvTutar;
                    aktarimDetay.oran = kdvOran;
                    liste.Add(aktarimDetay);
                }
            }

            return liste;
        }

        // ------------------------------------------------------------------ beyanname XML (eski btnKaydet_Click)

        private static void XMLSatirEkle(XmlDocument doc, XmlElement ust, string element, double tutar)
        {
            XmlElement satir = doc.CreateElement(element);
            satir.InnerText = tutar.ToString("0.00", CultureInfo.InvariantCulture);
            ust.AppendChild(satir);
        }

        private static void XMLSatirEkleText(XmlDocument doc, XmlElement ust, string element, string deger)
        {
            XmlElement satir = doc.CreateElement(element);
            satir.InnerText = deger ?? "";
            ust.AppendChild(satir);
        }

        /// <summary>
        /// Beyanname kökü ve genel bölümü (idari, mukellef, hsv, duzenleyen). Mükellef bilgileri kurum değişkenlerinden gelir;
        /// Damga Vergisi beyanında duzenleyen bölümü yoktur.
        /// </summary>
        private static XmlDocument BeyannameBasla(string kodVer, string surum, int yil, int ay, bool duzenleyenVar, VergiMukellef m)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<?xml version=\"1.0\" encoding=\"iso-8859-9\"?><beyanname kodVer=\"" + kodVer + surum
                + "\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"" + kodVer + surum + ".xsd\" />");

            XmlElement XmlElementimGenel = doc.CreateElement("genel");
            XmlElement idari = doc.CreateElement("idari");
            XmlElementimGenel.AppendChild(idari);

            XMLSatirEkleText(doc, idari, "vdKodu", m.vdKodu);

            XmlElement donem = doc.CreateElement("donem");
            idari.AppendChild(donem);
            XMLSatirEkleText(doc, donem, "tip", "aylik");
            XMLSatirEkleText(doc, donem, "yil", yil.ToString());
            XMLSatirEkleText(doc, donem, "ay", ay.ToString());

            XmlElement mukellef = doc.CreateElement("mukellef");
            XmlElementimGenel.AppendChild(mukellef);
            MukellefYaz(doc, mukellef, m);

            XmlElement hsv = doc.CreateElement("hsv");
            hsv.SetAttribute("sifat", "kendisi");
            XmlElementimGenel.AppendChild(hsv);
            MukellefYaz(doc, hsv, m);

            if (duzenleyenVar)
            {
                XmlElement duzenleyen = doc.CreateElement("duzenleyen");
                XmlElementimGenel.AppendChild(duzenleyen);
                MukellefYaz(doc, duzenleyen, m);
            }

            doc.DocumentElement.AppendChild(XmlElementimGenel);
            return doc;
        }

        private static void MukellefYaz(XmlDocument doc, XmlElement ust, VergiMukellef m)
        {
            XMLSatirEkleText(doc, ust, "vergiNo", m.vergiNo);
            XMLSatirEkleText(doc, ust, "soyadi", m.soyadi);
            XMLSatirEkleText(doc, ust, "adi", m.adi);
            XMLSatirEkleText(doc, ust, "alanKodu", m.alanKodu);
            XMLSatirEkleText(doc, ust, "telNo", m.telNo);
        }

        // ------------------------------------------------------------------ mükellef bilgileri (kurum değişkenleri)

        private const string MukellefYol = "/VERGIBEYAN/";

        private static string DegOku(string ad)
        {
            return (TNS.DEG.Arac.DegiskenDegerBul(0, MukellefYol + ad) ?? "").Trim();
        }

        private static VergiMukellef MukellefOku()
        {
            return new VergiMukellef
            {
                vergiNo = DegOku("VERGINO"),
                vdKodu = DegOku("VDKODU"),
                soyadi = DegOku("SOYADI"),
                adi = DegOku("ADI"),
                alanKodu = DegOku("ALANKODU"),
                telNo = DegOku("TELNO")
            };
        }

        private static async Task<IResult> MukellefSakla(HttpContext baglam, IWebHostEnvironment ortam, VergiMukellefIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null || istek.mukellef == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, istek.tur, out Kullanici _);
            if (ret != null) return ret;
            VergiMukellef m = istek.mukellef;
            try
            {
                TNS.DEG.Arac.DegiskenDegerAta(0, MukellefYol + "VERGINO", (m.vergiNo ?? "").Trim());
                TNS.DEG.Arac.DegiskenDegerAta(0, MukellefYol + "VDKODU", (m.vdKodu ?? "").Trim());
                TNS.DEG.Arac.DegiskenDegerAta(0, MukellefYol + "SOYADI", (m.soyadi ?? "").Trim());
                TNS.DEG.Arac.DegiskenDegerAta(0, MukellefYol + "ADI", (m.adi ?? "").Trim());
                TNS.DEG.Arac.DegiskenDegerAta(0, MukellefYol + "ALANKODU", (m.alanKodu ?? "").Trim());
                TNS.DEG.Arac.DegiskenDegerAta(0, MukellefYol + "TELNO", (m.telNo ?? "").Trim());
                return Results.Json(new { basarili = true, bilgi = "Mükellef bilgileri saklandı.", mukellef = MukellefOku() });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Mükellef bilgileri saklanamadı."));
            }
        }

        private static async Task<IResult> Xml(HttpContext baglam, IWebHostEnvironment ortam, VergiAktarimIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, istek.tur, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek.satirlar == null || istek.satirlar.Count == 0) return Hata("Listede aktarılacak satır yok.");
            if (istek.yil <= 0 || istek.ay < 1 || istek.ay > 12) return Hata("Yıl ve ay seçiniz.");

            TurBilgi bilgi = Turler[istek.tur];
            string surum = (istek.surum ?? "").Trim();
            try
            {
                VergiMukellef mukellef = MukellefOku();
                if (mukellef.vergiNo == "" || mukellef.vdKodu == "")
                    return Hata("Mükellef bilgileri girilmemiş. \"Mükellef Bilgileri\" ile vergi no ve vergi dairesi kodunu giriniz.");
                XmlDocument doc = BeyannameBasla(bilgi.KodVer, surum, istek.yil, istek.ay, istek.tur != DAMGA, mukellef);
                if (istek.tur == DAMGA) DamgaXml(doc, istek);
                else if (istek.tur == TEVKIFAT) TevkifatXml(doc, istek);
                else if (istek.tur == KDV) KDVXml(doc, istek, kullanan);
                else MuhtasarXml(doc, istek, kullanan);

                string tempDosya = DosyaIslem.DosyaAdUret("VergiAktarim", "") + ".xml";
                using (XmlTextWriter xmleEkle = new XmlTextWriter(tempDosya, Encoding.GetEncoding("ISO-8859-9")))
                {
                    xmleEkle.Formatting = Formatting.Indented;
                    doc.WriteContentTo(xmleEkle);
                }
                return DosyaIslem.DosyaGonder(tempDosya, istek.yil + "_" + istek.ay + bilgi.DosyaEk + surum + ".xml", true, "xml");
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "XML dosyası oluşturulamadı."));
            }
        }

        /// <summary>Eski DamgaVergisiAktarimi.btnKaydet_Click gövdesi (duzenlenenKagitlar).</summary>
        private static void DamgaXml(XmlDocument doc, VergiAktarimIstegi istek)
        {
            XmlElement XmlElementimOzel = doc.CreateElement("ozel");
            XmlElement duzenlenenKagitlar = doc.CreateElement("duzenlenenKagitlar");
            XmlElementimOzel.AppendChild(duzenlenenKagitlar);
            int sNo = 0;
            foreach (Dictionary<string, JsonElement> item in istek.satirlar)
            {
                sNo++;
                XmlElement duzenlenenKagit = doc.CreateElement("duzenlenenKagit");
                duzenlenenKagitlar.AppendChild(duzenlenenKagit);

                XMLSatirEkleText(doc, duzenlenenKagit, "siraNo", sNo.ToString());

                TNSDateTime duzenlemeTarihiStr = new TNSDateTime(T(item, "ISLEMTARIH"));
                XMLSatirEkleText(doc, duzenlenenKagit, "duzenlemeTarihi",
                    !duzenlemeTarihiStr.isNull ? duzenlemeTarihiStr.ToString().Replace(".", "").Replace("/", "") : "");

                XMLSatirEkleText(doc, duzenlenenKagit, "no", M(item, "BELGENO"));
                XMLSatirEkleText(doc, duzenlenenKagit, "turu", M(item, "TURKOD"));
                XMLSatirEkleText(doc, duzenlenenKagit, "aciklama", M(item, "ALTHESAPKOD"));

                string paraStr = M(item, "ICERDIGIPARA") ?? "";
                if (paraStr.IndexOf(".") < 0) paraStr += ".00";
                XMLSatirEkleText(doc, duzenlenenKagit, "icerdigiBelliPara", paraStr);

                paraStr = M(item, "ALACAK") ?? "";
                if (paraStr.IndexOf(".") < 0) paraStr += ".00";
                XMLSatirEkleText(doc, duzenlenenKagit, "damgaVergisi", paraStr);

                doc.DocumentElement.AppendChild(XmlElementimOzel);
            }
        }

        /// <summary>Eski KDVTevkifatiAktarimi.btnKaydet_Click gövdesi (kesintiler, tevkifatUygulananlar).</summary>
        private static void TevkifatXml(XmlDocument doc, VergiAktarimIstegi istek)
        {
            XmlElement XmlElementimOzel = doc.CreateElement("ozel");
            XmlElement kesintiler = doc.CreateElement("kesintiler");
            XmlElementimOzel.AppendChild(kesintiler);

            double toplamMatrah = 0.0;
            double toplamAlacak = 0.0;

            //İşlem türüne göre gruplandır
            List<GIBAktarim> liste = new List<GIBAktarim>();
            foreach (Dictionary<string, JsonElement> item in istek.satirlar)
            {
                XmlElement kesinti = doc.CreateElement("kesinti");
                kesintiler.AppendChild(kesinti);

                string soyadi = M(item, "SOYADI") ?? "";
                if (soyadi.Length > 30) soyadi = soyadi.Substring(0, 30);
                XMLSatirEkleText(doc, kesinti, "soyadi", soyadi);
                string adi = M(item, "ADI") ?? "";
                if (adi.Length > 30) adi = adi.Substring(0, 30);
                XMLSatirEkleText(doc, kesinti, "adi", adi);

                string kimlikNo = M(item, "VNO");
                if (kimlikNo != "")
                    XMLSatirEkleText(doc, kesinti, "vergiNo", M(item, "VNO"));
                else
                    XMLSatirEkleText(doc, kesinti, "tcKimlikNo", M(item, "TCNO"));

                XMLSatirEkleText(doc, kesinti, "vergiyeTabiMatrah", M(item, "MATRAH"));
                XMLSatirEkleText(doc, kesinti, "tutar", M(item, "ALACAK"));
                XMLSatirEkleText(doc, kesinti, "odemeTuru", M(item, "TURKOD"));
                doc.DocumentElement.AppendChild(XmlElementimOzel);

                bool bulundu = false;
                string islemTuru = M(item, "TURKOD");
                double matrah = D(item, "MATRAH");
                double alacak = D(item, "ALACAK");
                int oran = I(item, "KDVORAN");
                string vergiOran = M(item, "VERGIORAN");

                toplamMatrah += matrah;
                toplamAlacak += alacak;

                foreach (GIBAktarim aktarim in liste)
                {
                    if (aktarim.islemTuru == islemTuru && aktarim.oran == oran)
                    {
                        aktarim.matrah += matrah;
                        aktarim.alacak += alacak;
                        aktarim.oran = oran;
                        aktarim.vergiOran = vergiOran;
                        bulundu = true;
                    }
                }

                if (!bulundu)
                {
                    GIBAktarim aktarimDetay = new GIBAktarim();
                    aktarimDetay.islemTuru = islemTuru;
                    aktarimDetay.matrah = matrah;
                    aktarimDetay.alacak = alacak;
                    aktarimDetay.oran = oran;
                    aktarimDetay.vergiOran = vergiOran;
                    liste.Add(aktarimDetay);
                }
            }

            XMLSatirEkle(doc, XmlElementimOzel, "toplamMatrah", toplamMatrah);
            XMLSatirEkle(doc, XmlElementimOzel, "hesaplanan", toplamAlacak);
            XMLSatirEkle(doc, XmlElementimOzel, "odenmesiGereken", toplamAlacak);

            XmlElement tevkifatUygulananlar = doc.CreateElement("tevkifatUygulananlar");
            XmlElementimOzel.AppendChild(tevkifatUygulananlar);
            foreach (GIBAktarim aktarim in liste)
            {
                XmlElement tevkifatUygulanan = doc.CreateElement("tevkifatUygulanan");
                tevkifatUygulananlar.AppendChild(tevkifatUygulanan);

                XMLSatirEkleText(doc, tevkifatUygulanan, "islemTuru", aktarim.islemTuru);
                XMLSatirEkle(doc, tevkifatUygulanan, "matrah", aktarim.matrah);
                XMLSatirEkleText(doc, tevkifatUygulanan, "oran", aktarim.oran.ToString());
                XMLSatirEkleText(doc, tevkifatUygulanan, "tevkifatOrani", aktarim.vergiOran);
                XMLSatirEkle(doc, tevkifatUygulanan, "vergi", aktarim.alacak);

                doc.DocumentElement.AppendChild(XmlElementimOzel);
            }
        }

        /// <summary>Eski KDVAktarimi.btnKaydet_Click gövdesi (tevkifatUygulanmayanlar, indirilecek KDV, indirimler, devreden KDV).</summary>
        private static void KDVXml(XmlDocument doc, VergiAktarimIstegi istek, Kullanici kullanan)
        {
            int yilT = istek.yil;
            int ayT = istek.ay;
            bool yevmiyeli = istek.yevmiyeli;

            XmlElement XmlElementimOzel = doc.CreateElement("ozel");

            double toplamMatrah = 0.0;
            double toplamAlacak = 0.0;

            //İşlem türüne göre gruplandır
            List<GIBAktarim> liste = new List<GIBAktarim>();
            foreach (Dictionary<string, JsonElement> item in istek.satirlar)
            {
                bool bulundu = false;
                string islemTuru = M(item, "TURKOD");
                double matrah = D(item, "MATRAH");
                double alacak = D(item, "ALACAK");
                int oran = I(item, "KDVORAN");
                string vergiOran = M(item, "VERGIORAN");

                foreach (GIBAktarim aktarim in liste)
                {
                    if (aktarim.islemTuru == islemTuru && aktarim.oran == oran)
                    {
                        aktarim.matrah += matrah;
                        aktarim.alacak += alacak;
                        aktarim.oran = oran;
                        aktarim.vergiOran = vergiOran;
                        bulundu = true;
                    }
                }

                if (!bulundu)
                {
                    GIBAktarim aktarimDetay = new GIBAktarim();
                    aktarimDetay.islemTuru = islemTuru;
                    aktarimDetay.matrah = matrah;
                    aktarimDetay.alacak = alacak;
                    aktarimDetay.oran = oran;
                    aktarimDetay.vergiOran = vergiOran;
                    liste.Add(aktarimDetay);
                }
            }

            XmlElement tevkifatUygulanmayanlar = doc.CreateElement("tevkifatUygulanmayanlar");
            XmlElementimOzel.AppendChild(tevkifatUygulanmayanlar);
            foreach (GIBAktarim aktarim in liste)
            {
                XmlElement tevkifatUygulanmayan = doc.CreateElement("tevkifatUygulanmayan");
                tevkifatUygulanmayanlar.AppendChild(tevkifatUygulanmayan);

                aktarim.matrah = Math.Round(aktarim.matrah, 2);
                toplamMatrah += aktarim.matrah;
                XMLSatirEkle(doc, tevkifatUygulanmayan, "matrah", aktarim.matrah);

                XMLSatirEkleText(doc, tevkifatUygulanmayan, "oran", aktarim.oran.ToString());

                aktarim.alacak = Math.Round(aktarim.alacak, 2);
                XMLSatirEkle(doc, tevkifatUygulanmayan, "vergi", aktarim.alacak);
                toplamAlacak += aktarim.alacak;

                doc.DocumentElement.AppendChild(XmlElementimOzel);
            }

            XMLSatirEkle(doc, XmlElementimOzel, "vergiToplami", toplamAlacak);
            XMLSatirEkle(doc, XmlElementimOzel, "toplamMatrah", toplamMatrah);//doğru
            XMLSatirEkle(doc, XmlElementimOzel, "hesaplananKDV", toplamAlacak);
            XMLSatirEkle(doc, XmlElementimOzel, "toplamKDV", toplamAlacak);

            XmlElement indirilecekKDVODlar = doc.CreateElement("indirilecekKDVODler");
            XmlElementimOzel.AppendChild(indirilecekKDVODlar);
            double indirilecekKDVODToplamKDV = 0.0;
            liste = KDVListeleAktarim(kullanan, yilT, ayT, yevmiyeli, "191", false);
            foreach (GIBAktarim aktarim in liste)
            {
                XmlElement indirilecekKDVOD = doc.CreateElement("indirilecekKDVOD");
                indirilecekKDVODlar.AppendChild(indirilecekKDVOD);

                XMLSatirEkle(doc, indirilecekKDVOD, "bedel", aktarim.matrah);
                XMLSatirEkleText(doc, indirilecekKDVOD, "oran", aktarim.oran.ToString());
                XMLSatirEkle(doc, indirilecekKDVOD, "KDVTutari", aktarim.alacak);
                indirilecekKDVODToplamKDV += aktarim.alacak;

                doc.DocumentElement.AppendChild(XmlElementimOzel);
            }

            XMLSatirEkle(doc, XmlElementimOzel, "indirilecekKDVODToplamKDV", indirilecekKDVODToplamKDV);

            XmlElement indirimler = doc.CreateElement("indirimler");
            XmlElementimOzel.AppendChild(indirimler);
            double indirimT = TutarVer(kullanan, KDV, yilT, ayT, yevmiyeli, "190", 0);

            XmlElement indirim = doc.CreateElement("indirim");
            indirimler.AppendChild(indirim);
            XMLSatirEkleText(doc, indirim, "indirimTuru", "101");
            XMLSatirEkle(doc, indirim, "vergi", indirimT);

            indirim = doc.CreateElement("indirim");
            indirimler.AppendChild(indirim);
            XMLSatirEkleText(doc, indirim, "indirimTuru", "102");
            XMLSatirEkle(doc, indirim, "vergi", indirilecekKDVODToplamKDV);

            double indirimlerToplami = indirimT + indirilecekKDVODToplamKDV;

            XMLSatirEkle(doc, XmlElementimOzel, "indirimlerToplami", indirimlerToplami);

            XMLSatirEkle(doc, XmlElementimOzel, "ihracGerceklesmeyenTecilEdilemeyenKDV", 0);
            XMLSatirEkle(doc, XmlElementimOzel, "kdvsizTeminEdilenlerIcinOdenmeyenKDV", 0);
            XMLSatirEkle(doc, XmlElementimOzel, "ihracatDonemindeIadeEdilecekKDV", 0);
            XMLSatirEkle(doc, XmlElementimOzel, "yuklenilenKDV", 0);
            XMLSatirEkle(doc, XmlElementimOzel, "ihracVergiFark", 0);
            XMLSatirEkle(doc, XmlElementimOzel, "tecilEdilecekKDV", 0);
            XMLSatirEkle(doc, XmlElementimOzel, "odenmesiGerekenKDV", 0);
            XMLSatirEkle(doc, XmlElementimOzel, "kanun7143OdenmesiGerekenKDV", 0);
            XMLSatirEkle(doc, XmlElementimOzel, "buDonemdeOdenmesiGerekenKDV", 0);
            XMLSatirEkle(doc, XmlElementimOzel, "tahakkukEdecek7143", 0);
            XMLSatirEkle(doc, XmlElementimOzel, "mahsupEdecek7143", 0);
            XMLSatirEkle(doc, XmlElementimOzel, "iadeEdilmesiGerekenKDV", 0);

            double sonrakiDonemeDevredenKDV = indirimT + (indirilecekKDVODToplamKDV - toplamAlacak);
            XMLSatirEkle(doc, XmlElementimOzel, "sonrakiDonemeDevredenKDV", sonrakiDonemeDevredenKDV);

            XMLSatirEkle(doc, XmlElementimOzel, "teslimVeHizmetleriTeskilEdenBedelAylik", toplamMatrah);

            double teslimVeHizmetleriTeskilEdenBedelKumulatif = 0;
            liste = KDVListeleAktarim(kullanan, yilT, ayT, yevmiyeli, "391", true);
            foreach (GIBAktarim aktarim in liste)
                teslimVeHizmetleriTeskilEdenBedelKumulatif += aktarim.matrah;

            XMLSatirEkle(doc, XmlElementimOzel, "teslimVeHizmetleriTeskilEdenBedelKumulatif", teslimVeHizmetleriTeskilEdenBedelKumulatif);

            double krediKarti = TutarVer(kullanan, KDV, yilT, ayT, yevmiyeli, "109", 1);
            XMLSatirEkle(doc, XmlElementimOzel, "krediKarti", krediKarti);
        }

        /// <summary>Eski MuhtasarAktarimi.btnKaydet_Click gövdesi (matrah bildirimleri, mahsuplar, ödeme bildirimleri).</summary>
        private static void MuhtasarXml(XmlDocument doc, VergiAktarimIstegi istek, Kullanici kullanan)
        {
            int yilT = istek.yil;
            int ayT = istek.ay;
            bool yevmiyeli = istek.yevmiyeli;

            XmlElement XmlElementimOzel = doc.CreateElement("ozel");
            MuhtasarText(doc, XmlElementimOzel, "beyanimVarYok", "1");

            double toplamGayrisafiTutar = 0.0;
            double toplamKesintiTutari = 0.0;
            double terkinSonrasiTutar = 0.0;
            double odenecek = 0.0;
            double iadeEdilecek = 0.0;
            double odenecekDamgaVergisi = 0.0;
            double gelirVergisi = 0.0;

            XmlElement matrahBildirimleri = doc.CreateElement("matrahBildirimleri");
            XmlElementimOzel.AppendChild(matrahBildirimleri);

            XmlElement matrahBildirimi = doc.CreateElement("matrahBildirimi");
            matrahBildirimleri.AppendChild(matrahBildirimi);
            MuhtasarText(doc, matrahBildirimi, "turKodu", "012");//Gelir vergisi
            double kesintiTutari = TutarVer(kullanan, MUHTASAR, yilT, ayT, yevmiyeli, "360.01.01.02", 2);//360.01.01.02 alacak
            double gayrisafiTutar = 0.0;//Hesaplanamaz
            XMLSatirEkle(doc, matrahBildirimi, "gayrisafiTutar", gayrisafiTutar);
            XMLSatirEkle(doc, matrahBildirimi, "kesintiTutari", kesintiTutari);
            toplamGayrisafiTutar += gayrisafiTutar;
            toplamKesintiTutari += kesintiTutari;
            terkinSonrasiTutar += kesintiTutari;

            matrahBildirimi = doc.CreateElement("matrahBildirimi");
            matrahBildirimleri.AppendChild(matrahBildirimi);
            MuhtasarText(doc, matrahBildirimi, "turKodu", "022");//Avukat ödemeleri
            kesintiTutari = TutarVer(kullanan, MUHTASAR, yilT, ayT, yevmiyeli, "360.01.02.02", 2);//360.01.02.02 alacak
            gayrisafiTutar = kesintiTutari * 100 / 20;//(360.01.02.02 * 100 / 20) alacak
            XMLSatirEkle(doc, matrahBildirimi, "gayrisafiTutar", gayrisafiTutar);
            XMLSatirEkle(doc, matrahBildirimi, "kesintiTutari", kesintiTutari);
            toplamGayrisafiTutar += gayrisafiTutar;
            toplamKesintiTutari += kesintiTutari;
            terkinSonrasiTutar += kesintiTutari;

            matrahBildirimi = doc.CreateElement("matrahBildirimi");
            matrahBildirimleri.AppendChild(matrahBildirimi);
            MuhtasarText(doc, matrahBildirimi, "turKodu", "031");//Yapım işi
            kesintiTutari = TutarVer(kullanan, MUHTASAR, yilT, ayT, yevmiyeli, "360.01.03", 2);//360.01.03 alacak
            gayrisafiTutar = kesintiTutari * 100 / 3;//(360.01.03*100/3) alacak
            XMLSatirEkle(doc, matrahBildirimi, "gayrisafiTutar", gayrisafiTutar);
            XMLSatirEkle(doc, matrahBildirimi, "kesintiTutari", kesintiTutari);
            toplamGayrisafiTutar += gayrisafiTutar;
            toplamKesintiTutari += kesintiTutari;
            terkinSonrasiTutar += kesintiTutari;

            matrahBildirimi = doc.CreateElement("matrahBildirimi");
            matrahBildirimleri.AppendChild(matrahBildirimi);
            MuhtasarText(doc, matrahBildirimi, "turKodu", "302");//Damga vergisi
            kesintiTutari = TutarVer(kullanan, MUHTASAR, yilT, ayT, yevmiyeli, "360.03.01", 4);//360.03.01 bakiye
            gayrisafiTutar = kesintiTutari * 1000 / 7.59;//360.03.01*(1000/7,59) bakiye
            XMLSatirEkle(doc, matrahBildirimi, "gayrisafiTutar", gayrisafiTutar);
            XMLSatirEkle(doc, matrahBildirimi, "kesintiTutari", kesintiTutari);
            odenecekDamgaVergisi = kesintiTutari;

            XMLSatirEkle(doc, XmlElementimOzel, "toplamGayrisafiTutar", toplamGayrisafiTutar);
            XMLSatirEkle(doc, XmlElementimOzel, "toplamKesintiTutari", toplamKesintiTutari);
            XMLSatirEkle(doc, XmlElementimOzel, "terkinSonrasiTutar", terkinSonrasiTutar);

            //<mahsuplar>
            XmlElement mahsuplar = doc.CreateElement("mahsuplar");
            XmlElementimOzel.AppendChild(mahsuplar);
            gelirVergisi = TutarVer(kullanan, MUHTASAR, yilT, ayT, yevmiyeli, "360.01.01.02", 1);//360.01.01.02 borc
            XMLSatirEkle(doc, mahsuplar, "gelirVergisi", gelirVergisi);
            XMLSatirEkle(doc, mahsuplar, "mahsupToplami", gelirVergisi);

            //<damgaVergileri>
            XmlElement damgaVergileri = doc.CreateElement("damgaVergileri");
            XmlElementimOzel.AppendChild(damgaVergileri);
            XMLSatirEkle(doc, damgaVergileri, "tevkifatDamgaVergisi", odenecekDamgaVergisi);

            //<odemeBildirimleri>
            XmlElement odemeBildirimleri = doc.CreateElement("odemeBildirimleri");
            XmlElementimOzel.AppendChild(odemeBildirimleri);
            foreach (Dictionary<string, JsonElement> item in istek.satirlar)
            {
                XmlElement odemeBildirimi = doc.CreateElement("odemeBildirimi");
                odemeBildirimleri.AppendChild(odemeBildirimi);

                MuhtasarText(doc, odemeBildirimi, "soyadi", M(item, "SOYADI"));
                MuhtasarText(doc, odemeBildirimi, "adi", M(item, "ADI"));
                MuhtasarText(doc, odemeBildirimi, "adres", M(item, "ADRES"));
                MuhtasarText(doc, odemeBildirimi, "tcKimlikNo", M(item, "TCNO"));
                MuhtasarText(doc, odemeBildirimi, "turKodu", M(item, "TURKOD"));
                XMLSatirEkle(doc, odemeBildirimi, "gayrisafiTutar", D(item, "MATRAH"));
                MuhtasarText(doc, odemeBildirimi, "odemeTur", M(item, "ODEMETUR"));
                MuhtasarText(doc, odemeBildirimi, "odemeTarih", M(item, "ODEMETARIH"));
                MuhtasarText(doc, odemeBildirimi, "odemeSeriSiraNo", M(item, "ODEMENO"));
                XMLSatirEkle(doc, odemeBildirimi, "kesintiTutari", D(item, "ALACAK"));
            }

            //<calisanIsciler>
            XmlElement calisanIsciler = doc.CreateElement("calisanIsciler");
            XmlElementimOzel.AppendChild(calisanIsciler);
            XMLSatirEkle(doc, calisanIsciler, "digerUcretli", 0.0);
            XMLSatirEkle(doc, calisanIsciler, "toplam", 0.0);

            XMLSatirEkle(doc, XmlElementimOzel, "matrah", toplamGayrisafiTutar);
            XMLSatirEkle(doc, XmlElementimOzel, "tahakkukEden", toplamKesintiTutari);
            XMLSatirEkle(doc, XmlElementimOzel, "mahsup", gelirVergisi);
            XMLSatirEkle(doc, XmlElementimOzel, "odenecek", odenecek);
            XMLSatirEkle(doc, XmlElementimOzel, "iadeEdilecek", iadeEdilecek);
            XMLSatirEkle(doc, XmlElementimOzel, "odenecekDamgaVergisi", odenecekDamgaVergisi);

            // Eski sayfadaki <ekler> (EGO'ya ait sabit SGK işyeri bilgileri) kaldırıldı.

            doc.DocumentElement.AppendChild(XmlElementimOzel);
        }

        /// <summary>Muhtasar sayfasının XMLSatirEkleText'i değeri kırparak yazıyordu.</summary>
        private static void MuhtasarText(XmlDocument doc, XmlElement ust, string element, string deger)
        {
            XMLSatirEkleText(doc, ust, element, (deger ?? "").Trim());
        }

        // ------------------------------------------------------------------ Excel'e yazdır (eski btnListeYazdir_Click)

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

        private static async Task<IResult> Yazdir(HttpContext baglam, IWebHostEnvironment ortam, VergiAktarimIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, istek.tur, out Kullanici kullanan);
            if (ret != null) return ret;
            // Damga Vergisi listesi tablodan değil, defterden yeniden okunur (eski sayfadaki gibi).
            if (istek.tur != DAMGA && (istek.satirlar == null || istek.satirlar.Count == 0)) return Hata("Listede yazdırılacak satır yok.");
            if (istek.tur == DAMGA && (istek.yil <= 0 || istek.ay < 1 || istek.ay > 12)) return Hata("Yıl ve ay seçiniz.");

            try
            {
                string sablonAd = Turler[istek.tur].Sablon;
                string sablon = SablonYol(sablonAd);
                if (!File.Exists(sablon)) return Hata("Şablon (MUH/" + sablonAd + ") bulunamadı.");

                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                string sonucDosyaAd = DosyaIslem.DosyaAdUret("VergiAktarim", "") + ".xlsx";
                XLS.DosyaAc(sablon, sonucDosyaAd);

                int satir = 0;
                int sutun = 0;
                XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
                int yazSatir = satir + 1;

                if (istek.tur == DAMGA)
                    yazSatir = DamgaYazdir(XLS, kullanan, istek, satir, sutun, yazSatir);
                else
                {
                    foreach (Dictionary<string, JsonElement> item in istek.satirlar)
                    {
                        // Muhtasar: TCNO; KDV ve KDV Tevkifatı: vergi no, yoksa TC kimlik no
                        string vergiNoSatir = istek.tur == MUHTASAR ? M(item, "TCNO") : M(item, "VNO");
                        if (istek.tur != MUHTASAR && string.IsNullOrWhiteSpace(vergiNoSatir)) vergiNoSatir = M(item, "TCNO");
                        vergiNoSatir += " - " + M(item, "ADI") + " " + M(item, "SOYADI");

                        XLS.HucreKopyala(satir, sutun, satir, sutun + 9, yazSatir, sutun);
                        XLS.HucreDegerYaz(yazSatir, sutun + 0, new TNSDateTime(T(item, "ISLEMTARIH")).ToString());
                        XLS.HucreDegerYaz(yazSatir, sutun + 1, M(item, "BELGENO") ?? "");
                        XLS.HucreDegerYaz(yazSatir, sutun + 2, M(item, "TURKOD") ?? "");
                        XLS.HucreDegerYaz(yazSatir, sutun + 3, vergiNoSatir);
                        XLS.HucreDegerYaz(yazSatir, sutun + 4, M(item, "KDVORAN") ?? "");
                        XLS.HucreDegerYaz(yazSatir, sutun + 5, D(item, "MATRAH"));
                        XLS.HucreDegerYaz(yazSatir, sutun + 6, D(item, "KDVTUTAR"));
                        XLS.HucreDegerYaz(yazSatir, sutun + 7, D(item, "ALACAK"));
                        yazSatir++;
                    }
                }

                XLS.SatirYukseklikAyarla(satir + 1, yazSatir, -1);
                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(sonucDosyaAd, "Liste_" + DateTime.Now.Ticks, true, GenelIslemler.ExcelTur());
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }

        /// <summary>Eski DamgaVergisiAktarimi.btnListeYazdir_Click: satırlar defterden okunur (360.03.04 dahil).</summary>
        private static int DamgaYazdir(Tablo XLS, Kullanici kullanan, VergiAktarimIstegi istek, int satir, int sutun, int yazSatir)
        {
            string hesapKod = "360.03.02;360.03.03;360.03.04;360.03.09";
            List<BuyukDefter> mbler = AyinBuyukDefteri(kullanan, istek.yil, istek.ay, istek.yevmiyeli, hesapKod);
            ObjectArray dvler = AktarimTipleri(kullanan, DAMGA, hesapKod);

            foreach (BuyukDefter item in mbler)
            {
                foreach (VergiDairesiAktarimTurTanim dv in dvler.objeler)
                {
                    if (dv.hesapKod == item.hesapKod.Replace(".", ""))
                    {
                        XLS.HucreKopyala(satir, sutun, satir, sutun + 6, yazSatir, sutun);
                        XLS.HucreDegerYaz(yazSatir, sutun + 0, item.islemTarih.Oku());
                        XLS.HucreDegerYaz(yazSatir, sutun + 1, item.belgeNo);
                        XLS.HucreDegerYaz(yazSatir, sutun + 2, dv.turKod);
                        XLS.HucreDegerYaz(yazSatir, sutun + 3, item.altHesapAd);
                        XLS.HucreDegerYaz(yazSatir, sutun + 4, item.alacak);
                        XLS.HucreDegerYaz(yazSatir, sutun + 5, IcerdigiParaBol(item.alacak, dv.orani));
                        yazSatir++;
                        break;
                    }
                }
            }
            return yazSatir;
        }

        // ------------------------------------------------------------------ sürümü sakla (eski btnXMLKaydet_Click)

        private static async Task<IResult> Surum(HttpContext baglam, IWebHostEnvironment ortam, VergiAktarimIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, istek.tur, out Kullanici _);
            if (ret != null) return ret;
            try
            {
                Kullanici kaydetKul = new Kullanici();
                kaydetKul.kullaniciKodu = SurumKullanici;
                Sonuc s = GenelIslemler.KullaniciDegiskenSakla(kaydetKul, Turler[istek.tur].SurumDegisken, (istek.surum ?? "").Trim());
                if (s != null && !s.islemSonuc) return Hata(HataMetni(s.hataStr, "Sürüm saklanamadı."));
                return Results.Json(new { basarili = true, bilgi = "Sürüm saklandı." });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Sürüm saklanamadı."));
            }
        }
    }
}
