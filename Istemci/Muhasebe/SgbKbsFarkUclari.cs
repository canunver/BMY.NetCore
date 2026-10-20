using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Istemci.ButceHazirlik;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.KYM;
using TNS.MUH;

namespace Istemci.Muhasebe
{
    /// <summary>Listede seçilen bir fark satırı (alan adları SGBKBSFark'taki gibi).</summary>
    public class SgbKbsFarkSatiri
    {
        public int SayKod { get; set; }
        public string BirimKod { get; set; }
        public string Tertip { get; set; }
        public double SGBHarcama { get; set; }
        public double KBSHarcama { get; set; }
    }

    /// <summary>Seçili satırlar için ters kayıt isteği. Mode: son listelemenin türü (1 harcama farkı, 2 gönderme - harcama farkı).</summary>
    public class SgbKbsFarkKaydetIstegi
    {
        public int Yil { get; set; }
        public int Mode { get; set; }
        public string Ihale { get; set; }
        public List<SgbKbsFarkSatiri> Satirlar { get; set; }
    }

    /// <summary>
    /// SGB - KBS Fark (Faal.) (eski ButceMuhasebe/SGBKBSDuzeltmePEB.aspx) uçları.
    ///   GET  /api/sgbkbsfark/tanim   : açılış yılı / ayı ve yıllar
    ///   GET  /api/sgbkbsfark/liste   : mode 1 SGB - KBS harcama farkı, mode 2 gönderilen - KBS harcama farkı (IMUHServis.IstisnaRaporlari)
    ///   GET  /api/sgbkbsfark/kodlar  : kod ağacı (1 kurumsal, 2 fonksiyonel, 3 finans, 4 ekonomik, 14 faaliyet)
    ///   POST /api/sgbkbsfark/kaydet  : seçili satırlar için ters kayıt (mode 1: ödeme emri / MİF, mode 2: tenkis belgesi)
    /// Kaydetme mantığı eski sayfanın btnKaydet_Click kodunun aynısıdır (iş kuralı servisleri değişmedi). Sayfa yetkisi menu.xml'den.
    /// </summary>
    public static class SgbKbsFarkUclari
    {
        public const string SayfaAdres = "../ButceMuhasebe/SGBKBSDuzeltmePEB.aspx";
        private const string Uc = "/api/sgbkbsfark/";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "tanim", Tanim);
            app.MapGet(Uc + "liste", Liste);
            app.MapGet(Uc + "kodlar", Kodlar);
            app.MapPost(Uc + "kaydet", Kaydet);
        }

        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, SayfaAdres);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        /// <summary>Açılış (eski Page_Load): şubattan önce geçen yılın 12. ayı, sonra bu yılın bu ayı.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult red = Denetle(baglam, ortam, out Kullanici kullanan);
            if (red != null) return red;

            int yilFark = DateTime.Now.Month >= 2 ? 0 : 1;
            int yil = DateTime.Now.Year - yilFark;
            int ay = yilFark == 1 ? 12 : DateTime.Now.Month;
            List<int> yillar = new List<int>();
            for (int i = DateTime.Now.Year; i >= 2006; i--) yillar.Add(i);
            return Results.Json(new { yil = yil, ay = ay, yillar = yillar });
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int? yil, int? ay, string muhasebe, string birim,
            string kur, string fon, string fin, string eko, string peb, int? mode, int? durum, string minFark, int? farkTur)
        {
            IResult red = Denetle(baglam, ortam, out Kullanici kullanan);
            if (red != null) return red;

            int y = yil ?? 0;
            if (y < 2000) return Hata("Yıl giriniz.");
            int m = mode == 2 ? 2 : 1;

            SGBKBSFark f = new SGBKBSFark();
            f.yil = y;
            f.ay = ay ?? 12;
            f.SaySorgu = Suz(muhasebe, "0123456789,-/");
            f.BirimKod = Suz(birim, "0123456789");
            f.kurKod = Suz(kur, "0123456789.");
            f.fonKod = Suz(fon, "0123456789.");
            f.finKod = Suz(fin, "0123456789.");
            f.ekoKod = Suz(eko, "0123456789.");
            f.pebKod = Suz(peb, "0123456789.");
            f.pebli = true;
            f.mode = m;
            int d = durum ?? 0;
            f.sgbKsbDurum = d >= 0 && d <= 2 ? d : 0;
            f.minFark = SayiCoz(minFark);
            f.minFarkTur = farkTur == 1 ? 1 : 0;

            ObjectArray oa;
            try
            {
                oa = TNS.MUH.Arac.Tanimla().IstisnaRaporlari(kullanan, f);
            }
            catch (Exception)
            {
                return Hata("İşlem yapılamadı.");
            }
            if (!oa.sonuc.islemSonuc) return Hata(HataMetni(oa.sonuc.hataStr));

            List<object> satirlar = new List<object>();
            foreach (object o in oa.objeler)
            {
                if (!(o is SGBKBSFark k)) continue;
                satirlar.Add(new
                {
                    SayKod = k.SayKod,
                    BirimKod = k.BirimKod,
                    HesapKod = k.HesapKod,
                    Tertip = k.Tertip,
                    SGBHarcama = k.SGBHarcama,
                    KBSHarcama = k.KBSHarcama,
                    fark = k.fark
                });
            }
            return Results.Json(new { mode = m, satirlar = satirlar });
        }

        private static IResult Kodlar(HttpContext baglam, IWebHostEnvironment ortam, int yil, string tur, string ust)
        {
            IResult red = Denetle(baglam, ortam, out Kullanici kullanan);
            if (red != null) return red;
            tur = (tur ?? "").Trim();
            if (tur != "1" && tur != "2" && tur != "3" && tur != "4" && tur != "14") return Hata("Kod türü hatalı.");
            return GorusmeCariUclari.KodDuzeyi(kullanan, yil, tur, ust, false);
        }

        /// <summary>Seçili kayıtlar için ters kayıt (eski btnKaydet_Click).</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, SgbKbsFarkKaydetIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult red = Denetle(baglam, ortam, out Kullanici kullanan);
            if (red != null) return red;

            int mode = istek?.Mode ?? 0;
            if (mode != 1 && mode != 2) return Hata("Hazır değil");
            if (istek.Satirlar == null || istek.Satirlar.Count == 0) return Hata("ters kayıt yapılacak işlemleri seçiniz");
            int icYil = istek.Yil;
            if (icYil < 2000) return Hata("Yıl giriniz.");

            IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
            string hatalar = "";
            int yapilan = 0;
            try
            {
                TNS.BKD.Tertip tUz = new TNS.BKD.Tertip(icYil, TNS.BKD.ENUMTabloKod.TABPEBOdenek);
                int eko4Uz = tUz.KodUzunlukBul(3, 4, true);
                string ihale = (istek.Ihale ?? "").Trim();

                foreach (SgbKbsFarkSatiri satir in istek.Satirlar)
                {
                    if (satir == null) continue;
                    SGBKBSFark item = new SGBKBSFark();
                    item.SayKod = satir.SayKod;
                    item.BirimKod = (satir.BirimKod ?? "").Trim();
                    item.Tertip = (satir.Tertip ?? "").Trim();
                    item.SGBHarcama = satir.SGBHarcama;
                    item.KBSHarcama = satir.KBSHarcama;

                    if (mode == 1)
                    {
                        OdemeEmriMIFForm oeForm = new OdemeEmriMIFForm();
                        oeForm.yil = icYil;
                        oeForm.muhasebe = item.SayKod;
                        oeForm.birim = item.BirimKod;
                        oeForm.ihale = ihale;
                        DateTime simdi = DateTime.Now;
                        if (simdi.Year == oeForm.yil)
                            oeForm.islemTarih = new TNSDateTime(simdi);
                        else
                            oeForm.islemTarih = new TNSDateTime(new DateTime(oeForm.yil, 12, 31));

                        if (item.fark < 0)
                        {
                            oeForm.aciklama = "SGB'de fazla yapılan harcamaların mahsup işlemi";
                            double dusulecekTutar = 0 - item.fark;

                            OdemeEmriMIFDetay oeDetay1 = new OdemeEmriMIFDetay(oeForm.yil);
                            oeDetay1.hesapKod = TNS.MUH.Arac.EkonomikKoddanHesapkodBul(item.ekoKod, "830", eko4Uz);
                            oeDetay1.borc = 0;
                            oeDetay1.alacak = dusulecekTutar;
                            oeDetay1.TertipAta(item.Tertip);
                            oeForm.detay.Ekle(oeDetay1);

                            oeDetay1 = new OdemeEmriMIFDetay(oeForm.yil);
                            oeDetay1.hesapKod = "835";
                            oeDetay1.borc = dusulecekTutar;
                            oeDetay1.alacak = 0;
                            oeForm.detay.Ekle(oeDetay1);
                        }
                        else if (item.fark > 0)
                        {
                            oeForm.aciklama = "KBS'de fazla yapılan harcamaların SGB'ye işlenmesi";
                            double harcanacakTutar = item.fark;

                            OdemeEmriMIFDetay oeDetay1 = new OdemeEmriMIFDetay(oeForm.yil);
                            oeDetay1.hesapKod = TNS.MUH.Arac.EkonomikKoddanHesapkodBul(item.ekoKod, "830", eko4Uz);
                            oeDetay1.TertipAta(item.Tertip);
                            oeDetay1.borc = harcanacakTutar;
                            oeDetay1.alacak = 0;
                            oeForm.detay.Ekle(oeDetay1);

                            oeDetay1 = new OdemeEmriMIFDetay(oeForm.yil);
                            oeDetay1.hesapKod = "835";
                            oeDetay1.borc = 0;
                            oeDetay1.alacak = harcanacakTutar;
                            oeForm.detay.Ekle(oeDetay1);
                        }
                        if (oeForm.detay.ObjeSayisi > 0)
                        {
                            Sonuc s = servisMUH.OdemeEmriMIFKaydet(kullanan, oeForm);
                            if (!s.islemSonuc)
                                hatalar += $"{oeForm.muhasebe}-{oeForm.birim}-{item.ekoKod}-{item.pebKod}: " + HataMetni(s.hataStr) + "\n";
                            else
                                yapilan++;
                        }
                    }
                    else
                    {
                        GondermeTenkisForm gonForm = new GondermeTenkisForm();
                        gonForm.belgeTur = ENUMBelgeTuru.TENKIS;
                        gonForm.yil = icYil;
                        gonForm.muhasebe = item.SayKod;
                        gonForm.birim = item.BirimKod;
                        gonForm.kurum = TNS.DEG.Arac.VarsayilanKurumBul();
                        gonForm.islemYapan = kullanan.kullaniciKodu;

                        DateTime simdi = DateTime.Now;
                        if (simdi.Year == gonForm.yil)
                            gonForm.islemTarih = new TNSDateTime(simdi);
                        else
                            gonForm.islemTarih = new TNSDateTime(new DateTime(gonForm.yil, 12, 31));
                        gonForm.aciklama = "Fazla ödeneklerin tenkis edilmesi";
                        if (item.fark < 0)
                        {
                            GondermeTenkisDetay oeDetay1 = new GondermeTenkisDetay(gonForm.yil);
                            oeDetay1.tertip.TertipAta(item.Tertip);
                            oeDetay1.miktar = 0 - item.fark;
                            gonForm.detay.Ekle(oeDetay1);
                        }

                        if (gonForm.detay.ObjeSayisi > 0)
                        {
                            Sonuc s = servisMUH.GondermeTenkisKaydet(kullanan, gonForm, true, false, true);
                            if (!s.islemSonuc)
                                hatalar += $"{gonForm.muhasebe}-{gonForm.birim}-{item.ekoKod}-{item.pebKod}: " + HataMetni(s.hataStr) + "\n";
                            else
                                yapilan++;
                        }
                    }
                }
            }
            catch (Exception)
            {
                hatalar += "İşlem yapılamadı.\n";
            }

            return Results.Json(new
            {
                basarili = hatalar == "",
                yapilan = yapilan,
                hata = hatalar.Trim(),
                bilgi = yapilan + " kayıt için ters kayıt yapıldı."
            });
        }

        // ------------------------------------------------------------------ yardımcılar

        /// <summary>Metinden yalnızca izin verilen karakterleri bırakır (sorgu metnine elle yazılan değerler girmesin).</summary>
        private static string Suz(string deger, string izinli)
        {
            if (string.IsNullOrWhiteSpace(deger)) return "";
            StringBuilder sb = new StringBuilder();
            foreach (char c in deger.Trim())
                if (izinli.IndexOf(c) >= 0) sb.Append(c);
            return sb.ToString();
        }

        private static double SayiCoz(string metin)
        {
            metin = (metin ?? "").Trim();
            if (metin == "") return 0;
            if (metin.Contains(",")) metin = metin.Replace(".", "").Replace(",", ".");
            return double.TryParse(metin, NumberStyles.Float, CultureInfo.InvariantCulture, out double d) ? d : 0;
        }

        private static IResult Hata(string metin)
        {
            return Results.Json(new { basarili = false, hata = metin });
        }

        /// <summary>Servisin hata metni; boşsa ya da bir istisna dökümüyse genel hata metni.</summary>
        private static string HataMetni(string hata)
        {
            hata = hata ?? "";
            if (hata.Trim() == "" || hata.Contains("   at ")) return "İşlem yapılamadı.";
            return hata.Replace("<br />", "\n").Replace("<br/>", "\n").Replace("<br>", "\n").Trim();
        }
    }
}
