using System;
using System.Collections.Generic;
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
    /// <summary>Takip durumu değiştirme (eski wndTakipDurumDegistir / btnTakipDurumDegistir_Click).</summary>
    public class FaturaTakipDurumIstegi
    {
        public int Yil { get; set; }
        public string BelgeNo { get; set; }
        public int TakipDurum { get; set; }
    }

    /// <summary>
    /// Fatura Takip (eski ButceMuhasebe/FaturaTakipSorgu.aspx). Ödeme emri / MİF belgeleri hesap kodu (varsayılan 320.01) ve alt hesaba
    /// göre listelenir, her belgenin takip durumu değiştirilir, liste Excel'e yazdırılır (MUH/MUH-FaturaTakip.xlt).
    ///   GET  /api/faturatakip/tanim, liste, yazdir, tarihce?yil=&amp;belgeNo=, hesap/ara?yil=&amp;metin=, althesap/ara?metin=
    ///   POST /api/faturatakip/takip-durum
    /// Yetki: menüdeki Fatura Takip ve eski sayfadaki gibi ödeme emri görme yetkisi (TNS.MUH.Yetki.YetkisiVarMi).
    /// </summary>
    public static class FaturaTakipUclari
    {
        public const string SayfaAdresi = "../ButceMuhasebe/FaturaTakipSorgu.aspx";
        private const string Uc = "/api/faturatakip/";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "tanim", Tanim);
            app.MapGet(Uc + "liste", Liste);
            app.MapGet(Uc + "yazdir", Yazdir);
            app.MapGet(Uc + "tarihce", Tarihce);
            app.MapGet(Uc + "hesap/ara", HesapAra);
            app.MapGet(Uc + "althesap/ara", AltHesapAra);
            app.MapPost(Uc + "takip-durum", TakipDurumKaydet);
        }

        // ------------------------------------------------------------------ yetki ve ortak

        /// <summary>Menü yetkisi ve eski Page_Load'daki ödeme emri görme yetkisi.</summary>
        public static bool SayfaYetkili(Kullanici kullanan, string klasor)
        {
            if (kullanan == null) return false;
            if (!SayfaYetki.Yetkili(kullanan, klasor, SayfaAdresi)) return false;
            try { return TNS.MUH.Yetki.YetkisiVarMi(kullanan, (int)ENUMOGY.Gorme, (int)ENUMBelgeTuru.ODEMEEMRI); }
            catch (Exception) { return false; }
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!SayfaYetkili(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IMUHServis Servis() { return TNS.MUH.Arac.Tanimla(); }

        private static IResult Hata(string hata) { return Results.Json(new { hata = Duz(hata) }); }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        private static string Duz(string s)
        {
            return (s ?? "").Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<br />", "\n").Trim();
        }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return Duz(hata);
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

        /// <summary>Eski DurumDonustur.</summary>
        public static string DurumDonustur(int durum)
        {
            if (durum == (int)ENUMBelgeDurumu.YENI)
                return "Yeni";
            else if (durum == (int)ENUMBelgeDurumu.DEGISTIRILDI)
                return "Değiştirildi";
            else if (durum == (int)ENUMBelgeDurumu.ONAYLI)
                return "Onaylı";
            else if (durum == (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER)
                return "Yevmiyeli";
            else if (durum == (int)ENUMBelgeDurumu.IPTAL)
                return "İptal";
            else
                return "Belirsiz - " + durum;
        }

        /// <summary>Eski TakipDurumAdGetir.</summary>
        public static string TakipDurumAdGetir(int takipDurum)
        {
            if (takipDurum == 0)
                return "Gelmedi";
            else if (takipDurum == 1)
                return "Kontrole Geldi";
            else if (takipDurum == 2)
                return "İşleme Alındı";
            else if (takipDurum == 3)
                return "Geri gönderildi";
            else if (takipDurum == 4)
                return "Muhasebeye gönderildi";
            else if (takipDurum == 10)
                return "Ödendi";

            return "";
        }

        private static readonly int[] TakipDurumlari = { 0, 1, 2, 3, 4, 10 };

        // ------------------------------------------------------------------ açılış

        /// <summary>Eski Page_Load: yıl bu yıl, belge durum Yeni Belgeler (1), takip durum Hepsi (100), hesap kodu 320.01.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici _);
            if (ret != null) return ret;

            List<object> takip = new List<object>();
            foreach (int t in TakipDurumlari) takip.Add(new { kod = t, ad = TakipDurumAdGetir(t) });

            return Results.Json(new
            {
                yil = DateTime.Now.Year,
                durum = 1,
                takipDurum = 100,
                hesapNo = "320.01",
                belgeDurumlari = new object[]
                {
                    new { kod = 1, ad = "Yeni Belgeler" },
                    new { kod = 5, ad = "Onaylı" },
                    new { kod = 23, ad = "Yevmiyesiz" },
                    new { kod = 6, ad = "Yevmiyeli" },
                    new { kod = 9, ad = "İptal Edilenler" },
                    new { kod = 22, ad = "Bütün Belgeler (İptaller Hariç)" }
                },
                takipDurumlari = takip
            });
        }

        // ------------------------------------------------------------------ liste (eski Sorgula / btnListe_Click)

        /// <summary>Eski Sorgula(): sunucuya giden FaturaTakip süzgeci. Yıl ya da belge durumu boşsa hata metni döner.</summary>
        private static FaturaTakip Filtre(int? yil, int? durum, int? takipDurum, int? muhasebe, string birim, string yevmiyeNo1, string yevmiyeNo2,
            string belgeNo1, string belgeNo2, string belgeTarihi1, string belgeTarihi2, string hesapNo, string althesapNo, out string hata)
        {
            hata = null;
            FaturaTakip f = new FaturaTakip();
            f.yil = yil ?? 0;
            f.formDurum = durum ?? 0;
            f.takipDurum = takipDurum ?? 0;

            if (f.yil == 0 || f.formDurum == 0)
            {
                hata = "Sorgulama için yıl alanını boş bırakamazsınız.";
                return null;
            }

            f.kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
            f.muhasebe = muhasebe ?? 0;
            f.birim = Temiz(birim);

            string b1 = Temiz(belgeNo1);
            string b2 = Temiz(belgeNo2);

            string[] belgeNoparca = b1.Split('-', ' ', '_');
            if (belgeNoparca.Length > 0) b1 = belgeNoparca[0];

            belgeNoparca = b2.Split('-', ' ', '_');
            if (belgeNoparca.Length > 0) b2 = belgeNoparca[0];

            if (b1 != "" || b2 != "")
            {
                if (b1 != "" && b1.Length < 9)
                    b1 = (f.kurum ?? "").Replace(".", "").PadRight(4, '0') + b1.PadLeft(6, '0');
                if (b2 != "" && b2.Length < 9)
                    b2 = (f.kurum ?? "").Replace(".", "").PadRight(4, '0') + b2.PadLeft(6, '0');

                f.belgeNo = b1 + ";" + b2;
            }

            yevmiyeNo1 = Temiz(yevmiyeNo1);
            yevmiyeNo2 = Temiz(yevmiyeNo2);
            if (yevmiyeNo1 != "" || yevmiyeNo2 != "")
                f.sorguYEVYevNo = yevmiyeNo1 + ";" + yevmiyeNo2;

            if (Temiz(belgeTarihi1) != "")
                f.sorguIslemTarih = OdemeEmriMIFUclari.Tarih(Temiz(belgeTarihi1)).ToString() + ";";
            if (Temiz(belgeTarihi2) != "")
                f.sorguIslemTarih += OdemeEmriMIFUclari.Tarih(Temiz(belgeTarihi2)).ToString();

            OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(f.yil);
            detay.hesapKod = Temiz(hesapNo);
            detay.hesapKod = detay.hesapKod.Split('-')[0];

            detay.altHesapKod = althesapNo ?? "";
            if (!string.IsNullOrEmpty(detay.altHesapKod.Trim()))
            {
                string[] ahk = detay.altHesapKod.Split('-');
                detay.altHesapKod = ahk[0].Trim();
            }

            f.detay.objeler.Add(detay);
            return f;
        }

        private static object Satir(FaturaTakip ff)
        {
            return new
            {
                yil = ff.yil,
                belgeNo = ff.belgeNo,
                yevmiyeNo = ff.YEVYevNo,
                belgeTur = ff.belgeTur == ENUMBelgeTuru.ODEMEEMRI ? "ÖDE" : "MİF",
                belgeTarih = TarihYaz(ff.islemTarih),
                muhasebe = ff.muhasebe.ToString().PadLeft(5, '0') + " - " + ff.muhasebeAd,
                birim = ff.birim + " - " + ff.birimAd,
                tutar = ff.borcToplam > 0 ? ff.borcToplam : ff.alacakToplam,
                ilgili = ff.ilgiliAd,
                aciklama = ff.aciklama,
                durum = ff.formDurum,
                durumAd = DurumDonustur(ff.formDurum),
                islemYapan = ff.islemYapan,
                takipDurum = ff.takipDurum,
                takipDurumAd = TakipDurumAdGetir(ff.takipDurum),
                ekliDosya = ff.ekliDosyaVarmi > 0 ? "Var" : ""
            };
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int? yil, int? durum, int? takipDurum, int? muhasebe, string birim,
            string yevmiyeNo1, string yevmiyeNo2, string belgeNo1, string belgeNo2, string belgeTarihi1, string belgeTarihi2, string hesapNo, string althesapNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                FaturaTakip f = Filtre(yil, durum, takipDurum, muhasebe, birim, yevmiyeNo1, yevmiyeNo2, belgeNo1, belgeNo2, belgeTarihi1, belgeTarihi2,
                    hesapNo, althesapNo, out string hata);
                if (f == null) return Hata(hata);

                ObjectArray fler = Servis().FaturaTakipListele(kullanan, f);
                if (fler == null || !fler.sonuc.islemSonuc) return Hata(HataMetni(fler?.sonuc.hataStr, "Liste okunamadı."));

                List<object> satirlar = new List<object>();
                foreach (FaturaTakip ff in fler.objeler) satirlar.Add(Satir(ff));
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Liste okunamadı.");
            }
        }

        // ------------------------------------------------------------------ Liste Yazdır (eski btnListeYazdir_Click)

        private static string SablonBul(string sablonAd)
        {
            string kurumYol = Ayar.Oku("RaporSablonYol") ?? "";
            if (kurumYol != "")
            {
                string kurum = System.IO.Path.Combine(kurumYol, "MUH", sablonAd);
                if (System.IO.File.Exists(kurum)) return kurum;
            }
            foreach (string kok in new string[] { System.IO.Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
            {
                string uygulama = System.IO.Path.Combine(kok, "RaporSablon", "MUH", sablonAd);
                if (System.IO.File.Exists(uygulama)) return uygulama;
            }
            return null;
        }

        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, int? yil, int? durum, int? takipDurum, int? muhasebe, string birim,
            string yevmiyeNo1, string yevmiyeNo2, string belgeNo1, string belgeNo2, string belgeTarihi1, string belgeTarihi2, string hesapNo, string althesapNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            string sablonAd = "MUH-FaturaTakip.xlt";
            string sablon = SablonBul(sablonAd);
            if (sablon == null) return Hata("Rapor şablonu bulunamadı (MUH\\" + sablonAd + ").");
            try
            {
                FaturaTakip f = Filtre(yil, durum, takipDurum, muhasebe, birim, yevmiyeNo1, yevmiyeNo2, belgeNo1, belgeNo2, belgeTarihi1, belgeTarihi2,
                    hesapNo, althesapNo, out string hata);
                if (f == null) return Hata(hata);

                ObjectArray fler = Servis().FaturaTakipListele(kullanan, f);
                if (fler == null || !fler.sonuc.islemSonuc) return Hata(HataMetni(fler?.sonuc.hataStr, "Rapor hazırlanamadı."));

                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                int satir = 0;
                int satirD = 0;
                int sutunD = 0;
                string baslik = "FATURA TAKİP LİSTESİ";

                string sonucDosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();

                XLS.DosyaAc(sablon, sonucDosyaAd);
                XLS.HucreAdAdresCoz("txtDetaySatir", ref satirD, ref sutunD);

                satir = satirD + 1;

                XLS.HucreAdBulYaz("txtBaslik", baslik);
                string kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
                string kurumAd = GenelIslemlerIstemci.VarsayilanKurumBulAd();

                XLS.HucreAdBulYaz("txtYil", (yil ?? 0).ToString());
                XLS.HucreAdBulYaz("txtKurum", kurum + " - " + kurumAd);

                int i = 0;
                foreach (FaturaTakip ff in fler.objeler)
                {
                    i++;
                    XLS.HucreKopyala(satirD, 0, satirD, 28, satir, 0);

                    XLS.HucreDegerYaz(satir, 1, i.ToString());
                    XLS.HucreDegerYaz(satir, 2, ff.belgeNo);
                    XLS.HucreDegerYaz(satir, 3, ff.islemTarih.ToString());
                    XLS.HucreDegerYaz(satir, 4, ff.muhasebe.ToString().PadLeft(5, '0') + " - " + ff.muhasebeAd);
                    XLS.HucreDegerYaz(satir, 5, ff.birim + " - " + ff.birimAd);
                    XLS.HucreDegerYaz(satir, 6, ff.borcToplam > 0 ? ff.borcToplam : ff.alacakToplam);
                    XLS.HucreDegerYaz(satir, 7, ff.ilgiliAd);
                    XLS.HucreDegerYaz(satir, 8, DurumDonustur(ff.formDurum));
                    XLS.HucreDegerYaz(satir, 9, TakipDurumAdGetir(ff.takipDurum));
                    XLS.HucreDegerYaz(satir, 10, ff.ekliDosyaVarmi > 0 ? "Var" : "Yok");
                    XLS.HucreDegerYaz(satir, 11, ff.aciklama);

                    satir++;
                }

                XLS.SatirSil(satirD, satirD);

                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(sonucDosyaAd, "FaturaTakip_" + DosyaIslem.DosyaAdUretSade(), true, GenelIslemler.ExcelTur());
            }
            catch (Exception)
            {
                return Hata("Rapor hazırlanamadı.");
            }
        }

        // ------------------------------------------------------------------ takip durumu (eski btnTakipDurumDegistir_Click)

        private static async Task<IResult> TakipDurumKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, FaturaTakipDurumIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || Temiz(istek.BelgeNo) == "") return Results.Json(new { basarili = false, hata = "Belge seçilmemiş." });
            try
            {
                FaturaTakip form = new FaturaTakip();
                form.belgeNo = Temiz(istek.BelgeNo);
                form.yil = istek.Yil;
                form.takipDurum = istek.TakipDurum;

                Sonuc sonuc = Servis().FaturaTakipKaydet(kullanan, form);
                if (sonuc.islemSonuc)
                    return Results.Json(new { basarili = true, bilgi = Duz(sonuc.bilgiStr), takipDurum = form.takipDurum, takipDurumAd = TakipDurumAdGetir(form.takipDurum) });
                return Results.Json(new { basarili = false, hata = HataMetni(sonuc.hataStr, "Takip durumu kaydedilemedi.") });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Takip durumu kaydedilemedi." });
            }
        }

        // ------------------------------------------------------------------ belge tarihçesi (eski BelgeTarihce.aspx; OdemeEmriSecimUclari.Tarihce ile aynı)

        private static IResult Tarihce(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string belgeNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            belgeNo = Temiz(belgeNo);
            if ((yil ?? 0) == 0 || belgeNo == "") return Hata("Yıl ve/ya Belge No bilgisi alınamadı.");
            try
            {
                ObjectArray tVAN = Servis().TarihceListele(kullanan, yil.Value, (ENUMBelgeTuru)0, belgeNo);
                if (!tVAN.sonuc.islemSonuc) return Hata(HataMetni(tVAN.sonuc.hataStr, "Tarihçe okunamadı."));
                List<object> satirlar = new List<object>();
                int sira = 0;
                foreach (TarihceBilgisi t in tVAN.objeler)
                {
                    sira++;
                    satirlar.Add(new { sira = sira, islemTarih = t.islemTarih.ToLongString(), islem = t.islem, islemYapan = t.islemYapan, islemYapanAd = t.islemYapanAd });
                }
                return Results.Json(new { belge = yil + " / " + belgeNo, satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Tarihçe okunamadı."));
            }
        }

        // ------------------------------------------------------------------ hesap / alt hesap arama (eski HesapStore / AltHesapStore)

        /// <summary>Eski HesapListesiDoldur: yazılan metne göre hesap planı (detay = false), "kod - ad".</summary>
        private static IResult HesapAra(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string metin)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            metin = Temiz(metin);
            if (metin == "") return Results.Json(new { satirlar = new object[0] });
            try
            {
                HesapPlaniSatir h = new HesapPlaniSatir();
                h.yil = yil ?? 0;
                if (h.yil == 0) h.yil = DateTime.Now.Year;
                h.hesapKodHesapAd = metin;
                h.detay = false;
                ObjectArray hesap = Servis().HesapPlaniListele(kullanan, h);
                if (hesap == null || !hesap.sonuc.islemSonuc) return Hata(HataMetni(hesap?.sonuc.hataStr, "Arama yapılamadı."));

                List<object> satirlar = new List<object>();
                foreach (HesapPlaniSatir detay in hesap.objeler)
                {
                    satirlar.Add(new { KOD = detay.hesapKod + " - " + detay.aciklama });
                    if (satirlar.Count >= 200) break;
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Arama yapılamadı.");
            }
        }

        /// <summary>Eski AltHesapListesiDoldur: AltHesapListeHazirla(kullanan, 0, kriter), "vergiNo - ad".</summary>
        private static IResult AltHesapAra(HttpContext baglam, IWebHostEnvironment ortam, string metin)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            metin = Temiz(metin);
            if (metin == "") return Results.Json(new { satirlar = new object[0] });
            try
            {
                ObjectArray bilgi = Servis().AltHesapListeHazirla(kullanan, 0, metin);
                if (bilgi == null || !bilgi.sonuc.islemSonuc) return Hata(HataMetni(bilgi?.sonuc.hataStr, "Arama yapılamadı."));

                List<object> satirlar = new List<object>();
                foreach (TNS.HRC.FirmaBilgisi f in bilgi.objeler)
                {
                    satirlar.Add(new { KOD = f.vergiNo + " - " + f.ad });
                    if (satirlar.Count >= 200) break;
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Arama yapılamadı.");
            }
        }
    }
}
