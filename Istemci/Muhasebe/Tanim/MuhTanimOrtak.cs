using System;
using System.Collections.Generic;
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
using TNS.KYM;
using TNS.MUH;

namespace Istemci.Muhasebe
{
    /// <summary>
    /// Muhasebe Tanımları sayfalarının (menüde Kaynak Yönetimi > Muhasebe Tanımları) ortak sunucu yardımcıları:
    ///   - yetki denetimi (sistem / muhasebe yöneticisi ya da menüdeki sayfa yetkisi),
    ///   - { hata } / { basarili, bilgi } yanıtları,
    ///   - hesap planı ve kişi / firma seçim pencerelerinin uçları (Sgb.hesapPlaniSec / Sgb.firmaSec, js/makbuzsecim.js),
    ///   - MUH-Bos.xlt ile Excel'e aktarma ve Excel'den aktarma (eski ExcelAktar.aspx).
    /// Uçlar /api/muhtanim/ altındadır; her sayfanın kendi uçları ayrı dosyadadır (MuhTanim*Uclari.cs).
    /// </summary>
    public static class MuhTanimOrtak
    {
        public const string Kok = "/api/muhtanim/";

        public const string HesapPlaniAdresi = "../ButceMuhasebe/TanimHesapPlani.aspx";
        public const string MutemetAdresi = "../ButceMuhasebe/TanimMutemetLimit.aspx";
        public const string HesapKisitAdresi = "../ButceMuhasebe/TanimHesapPlaniKisit.aspx";
        public const string TKDKAdresi = "../ButceMuhasebe/TanimTKDKTalepForm.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Kok + "hesap/agac", HesapAgac);
            app.MapGet(Kok + "hesap/ara", HesapAra);
            app.MapGet(Kok + "hesap/ad", HesapAd);
            app.MapGet(Kok + "firma/ara", FirmaAra);
            app.MapPost(Kok + "excel-aktar", ExceldenAktar);

            MuhTanimBasitUclari.Ekle(app);
            MuhTanimHesapPlaniUclari.Ekle(app);
            MuhTanimListeUclari.Ekle(app);
            MuhTanimDigerUclari.Ekle(app);
        }

        // ------------------------------------------------------------------ yetki

        /// <summary>
        /// Sayfa yetkisi. muhYonetici: eski sayfadaki gibi sistem yöneticisi ve muhasebe yöneticisi menü yetkisine bakılmadan girer
        /// (eski "if (!(SISTEMYONETICISI || MUHASEBEYONETICISI)) SayfayaGirisYapabilirMi"). Diğer kullanıcılar menüdeki yetkiyle girer.
        /// </summary>
        public static bool Yetkili(Kullanici kullanan, string klasor, string adres, bool muhYonetici = true)
        {
            if (kullanan == null) return false;
            if (muhYonetici && MuhasebeUclari.TanimYetkisiVar(kullanan)) return true;
            return SayfaYetki.Yetkili(kullanan, klasor, adres);
        }

        /// <summary>Sistem yöneticisi ya da muhasebe yöneticisi mi (eski sayfalarda bazı tuşlar yalnızca bunlara görünür).</summary>
        public static bool Yonetici(Kullanici kullanan)
        {
            return MuhasebeUclari.TanimYetkisiVar(kullanan);
        }

        public static IResult Denetle(HttpContext baglam, Func<Kullanici, bool> yetki, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!yetki(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        /// <summary>Okuma ucu: oturum ve yetki denetlenir, iş yapılır; beklenmeyen hata { hata } olarak döner.</summary>
        public static IResult Oku(HttpContext baglam, Func<Kullanici, bool> yetki, Func<Kullanici, IResult> is_, string hataMetni = "Bilgiler okunamadı.")
        {
            IResult ret = Denetle(baglam, yetki, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                return is_(kullanan);
            }
            catch (Exception)
            {
                return Hata(hataMetni);
            }
        }

        /// <summary>Yazma ucu: istek jetonu, oturum ve yetki denetlenir, iş yapılır.</summary>
        public static async Task<IResult> Yaz(HttpContext baglam, IAntiforgery sahtecilik, Func<Kullanici, bool> yetki, Func<Kullanici, IResult> is_)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, yetki, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                return is_(kullanan);
            }
            catch (Exception)
            {
                return Hata("İşlem yapılamadı.");
            }
        }

        // ------------------------------------------------------------------ yanıtlar

        public static IResult Hata(string hata)
        {
            return Results.Json(new { hata = hata ?? "" });
        }

        public static IResult Tamam(string bilgi)
        {
            return Results.Json(new { basarili = true, bilgi = bilgi ?? "" });
        }

        /// <summary>Servis sonucunu yanıta çevirir: başarılıysa servisin bilgi metni (yoksa varsayılan), değilse hata metni.</summary>
        public static IResult SonucYaniti(Sonuc sonuc, string varsayilanBilgi = "İşlem başarıyla gerçekleşti.")
        {
            if (sonuc == null) return Hata("İşlem yapılamadı.");
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc.hataStr, "İşlem yapılamadı."));
            string bilgi = SatirMetni(sonuc.bilgiStr);
            return Results.Json(new { basarili = true, bilgi = bilgi != "" ? bilgi : varsayilanBilgi, anahtar = sonuc.anahtar ?? "" });
        }

        /// <summary>Servisin hata metni; boşsa ya da sistem hatası ayrıntısı içeriyorsa varsayılan metin.</summary>
        public static string HataMetni(string hata, string varsayilan)
        {
            hata = SatirMetni(hata);
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        /// <summary>Servis metinlerindeki &lt;br&gt; satır sonları düz satır sonuna çevrilir.</summary>
        public static string SatirMetni(string metin)
        {
            return (metin ?? "").Replace("<br />", "\n").Replace("<br/>", "\n").Replace("<br>", "\n").Trim();
        }

        public static string Temiz(string s)
        {
            return (s ?? "").Trim();
        }

        // ------------------------------------------------------------------ Excel

        /// <summary>MUH şablonu: önce kurumun RaporSablonYol\MUH klasöründe, yoksa uygulamayla gelen RaporSablon\MUH klasöründe.</summary>
        public static string SablonBul(string uygulamaKlasoru, string sablonAd)
        {
            return MakbuzRaporlari.SablonBul(uygulamaKlasoru, sablonAd);
        }

        /// <summary>
        /// Satırları boş Excel şablonuna (MUH\MUH-Bos.xlt) yazar ve indirilecek dosya olarak döndürür; eski sayfaların
        /// btnExceleAktar_Click kodunun aynısıdır (satır 1'den başlar, 0. satırın biçimi kopyalanır).
        /// genislikler: (sütun, Jexcel birimi) çiftleri.
        /// </summary>
        public static IResult BosExceleYaz(string uygulamaKlasoru, List<object[]> satirlar, string gidenAd, params (int sutun, int genislik)[] genislikler)
        {
            string sablon = SablonBul(uygulamaKlasoru, "MUH-Bos.xlt");
            if (sablon == null) return Hata("Rapor şablonu bulunamadı (MUH\\MUH-Bos.xlt).");
            AsposeLisans.Yukle();
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;
            string sonucDosyaAd = DosyaIslem.DosyaAdUret(gidenAd + "_", "." + GenelIslemler.ExcelTur());
            XLS.DosyaAc(sablon, sonucDosyaAd);
            foreach (object[] degerler in satirlar)
            {
                satir++;
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 12, satir, sutun);
                for (int i = 0; i < degerler.Length; i++)
                    XLS.HucreDegerYaz(satir, i, degerler[i] == null ? "" : degerler[i].ToString());
            }
            foreach ((int s, int g) in genislikler)
                XLS.SutunGenislikAyarla(s, s, GenelIslemler.JexcelBirimtoExcelBirimCol(g));
            if (satir > 0) XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(300));
            XLS.DosyaSaklaTamYol();
            return DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), gidenAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Excel'den aktar (eski ButceMuhasebe/ExcelAktar.aspx): tur=HesapPlani (yıl, hesap kodu, açıklama, tertip, kesinti, nitelik)
        /// ya da tur=Yansitma (yıl, hesap kodu, yansıtma hesap kodu, sıra no). İlk sütunu boş olan satırda durulur; 5000 satırda bir kaydedilir.
        /// </summary>
        private static async Task<IResult> ExceldenAktar(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IFormCollection form = await baglam.Request.ReadFormAsync();
            string tur = Temiz(form["tur"]);
            if (tur != "HesapPlani" && tur != "Yansitma") return Hata("Aktarım türü alınamadı.");
            // Yansıtmalar artık Hesap Planı sayfasında yönetiliyor; ikisi de Hesap Planı yetkisiyle.
            IResult ret = Denetle(baglam, k => Yetkili(k, ortam.ContentRootPath, HesapPlaniAdresi), out Kullanici kullanan);
            if (ret != null) return ret;

            IFormFile dosya = form.Files["dosya"];
            if (dosya == null || dosya.Length == 0) return Hata("Lütfen aktarım yapılacak dosyayı seçin.");
            if (dosya.Length > 20 * 1024 * 1024) return Hata("Dosya çok büyük (en çok 20 MB).");

            string uzanti = Path.GetExtension(dosya.FileName ?? "").ToLowerInvariant();
            if (uzanti != ".xls" && uzanti != ".xlsx") return Hata("Excel dosyası (.xls, .xlsx) seçiniz.");
            string yol = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul() + uzanti;
            try
            {
                using (FileStream hedef = File.Create(yol))
                    await dosya.CopyToAsync(hedef);
                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                XLS.DosyaOkuAc(yol);
                IMUHServis servis = TNS.MUH.Arac.Tanimla();

                ObjectArray liste = new ObjectArray();
                Sonuc sonuc = new Sonuc(true, "", "");
                int satir = 0;
                int sayac = 0;
                while (true)
                {
                    string yil = XLS.HucreDegerAl(satir, 0);
                    if (yil.Trim() == "") break;
                    if (tur == "Yansitma")
                    {
                        HesapPlaniYansitma y = new HesapPlaniYansitma();
                        y.yil = OrtakFonksiyonlar.ConvertToInt(yil, 0);
                        y.hesapKod = XLS.HucreDegerAl(satir, 1);
                        y.yansitmaHesapKod = XLS.HucreDegerAl(satir, 2);
                        y.satir = OrtakFonksiyonlar.ConvertToInt(XLS.HucreDegerAl(satir, 3), 0);
                        liste.objeler.Add(y);
                    }
                    else
                    {
                        HesapPlaniSatir h = new HesapPlaniSatir();
                        h.yil = OrtakFonksiyonlar.ConvertToInt(yil, 0);
                        h.hesapKod = XLS.HucreDegerAl(satir, 1).Trim();
                        h.aciklama = XLS.HucreDegerAl(satir, 2).Replace("'", " ").Trim();
                        h.tertip = OrtakFonksiyonlar.ConvertToInt(XLS.HucreDegerAl(satir, 3), 0);
                        h.kesinti = OrtakFonksiyonlar.ConvertToInt(XLS.HucreDegerAl(satir, 4), 0);
                        h.nitelik = XLS.HucreDegerAl(satir, 5);
                        liste.objeler.Add(h);
                    }
                    if (sayac > 5000)
                    {
                        sayac = 0;
                        sonuc = tur == "Yansitma" ? servis.YansitmaKaydet(liste) : servis.HesapPlaniKaydet(kullanan, liste);
                        liste = new ObjectArray();
                        if (!sonuc.islemSonuc) break;
                    }
                    satir++;
                    sayac++;
                }
                if (sonuc.islemSonuc && liste.objeler.Count > 0)
                    sonuc = tur == "Yansitma" ? servis.YansitmaKaydet(liste) : servis.HesapPlaniKaydet(kullanan, liste);

                if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc.hataStr, "Aktarım yapılamadı."));
                return Tamam("Kayıt işlemi başarıyla gerçekleşti. " + satir + " adet bilgi aktarıldı.");
            }
            catch (Exception)
            {
                return Hata("Excel dosyası okunamadı.");
            }
            finally
            {
                try { File.Delete(yol); } catch { }
            }
        }

        // ------------------------------------------------------------------ hesap planı ve kişi / firma seçimi

        /// <summary>Hesap planı / kişi seçimini bu sayfalardan birini kullanabilen herkes yapabilir.</summary>
        private static bool SecimYetkili(Kullanici kullanan, string klasor)
        {
            if (Yonetici(kullanan)) return true;
            foreach (string adres in new[] { HesapPlaniAdresi, MutemetAdresi, HesapKisitAdresi, TKDKAdresi, MuhTanimBasitUclari.TeminatNeviAdresi })
                if (SayfaYetki.Yetkili(kullanan, klasor, adres)) return true;
            for (int tur = 1; tur <= 4; tur++)
                if (MuhTanimBasitUclari.VDAktarimYetkili(kullanan, klasor, tur)) return true;
            return false;
        }

        private static int HesapYil(int? yil)
        {
            int y = yil ?? 0;
            return y <= 2000 ? DateTime.Now.Year : y;
        }

        private static object HesapDugum(HesapPlaniSatir h)
        {
            return new { kod = h.hesapKod, ad = h.hesapKod + " - " + h.aciklama, hesapAd = h.aciklama ?? "", altVar = !h.detay, detay = h.detay };
        }

        /// <summary>Hesap planı ağacı (eski ListeHesapPlani.aspx): kökte 1-9 ana gruplar, sonra düzey düzey hesaplar.</summary>
        private static IResult HesapAgac(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string ust)
        {
            return Oku(baglam, k => SecimYetkili(k, ortam.ContentRootPath), kullanan =>
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

                string[] kirilimlar = ust.Split('.');
                HesapPlaniSatir hs = new HesapPlaniSatir();
                hs.yil = y;
                hs.hesapKod = ust;
                hs.seviye = ust.Length < 3 ? 1 : kirilimlar.Length + 1;

                IMUHServis servis = TNS.MUH.Arac.Tanimla();
                ObjectArray hv = new ObjectArray();
                for (int i = 0; i < 10; i++)
                {
                    hv = servis.HesapPlaniListele(new Kullanici(), hs);
                    if (hv.objeler.Count == 0) hs.seviye++;
                    else break;
                }
                foreach (HesapPlaniSatir h in hv.objeler) dugumler.Add(HesapDugum(h));
                return Results.Json(new { dugumler = dugumler });
            }, "Hesap planı okunamadı.");
        }

        /// <summary>Hesap kodu / adı ile arama (eski HesapKod.ascx).</summary>
        private static IResult HesapAra(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string metin, string sonKirilim)
        {
            return Oku(baglam, k => SecimYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                metin = Temiz(metin);
                if (metin.Length < 2) return Hata("En az iki karakter yazınız.");
                HesapPlaniSatir h = new HesapPlaniSatir();
                h.yil = HesapYil(yil);
                h.hesapKodHesapAd = metin;
                h.detay = sonKirilim == "1";
                ObjectArray hesap = TNS.MUH.Arac.Tanimla().HesapPlaniListele(kullanan, h);
                if (hesap == null || !hesap.sonuc.islemSonuc) return Hata(HataMetni(hesap?.sonuc.hataStr, "Arama yapılamadı."));
                List<object> satirlar = new List<object>();
                foreach (HesapPlaniSatir d in hesap.objeler)
                {
                    satirlar.Add(HesapDugum(d));
                    if (satirlar.Count >= 500) break;
                }
                return Results.Json(new { satirlar = satirlar });
            }, "Arama yapılamadı.");
        }

        /// <summary>
        /// Elle yazılan hesap kodunun adı (Sgb.hesapKodu bileşeni, js/hesapkodu.js): kod yılın kod yapısına göre noktalanır;
        /// hesap planında yoksa ad boş döner. detay: son seviye mi.
        /// </summary>
        private static IResult HesapAd(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string kod)
        {
            return Oku(baglam, k => SecimYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                kod = Temiz(kod);
                if (kod == "") return Results.Json(new { kod = "", ad = "", detay = false });
                int y = HesapYil(yil);
                string hesYapi = TNS.MUH.Arac.DegiskenDegerBul(y, "/KODYAPI/KOD8/KODYAPI");
                if (!string.IsNullOrEmpty(hesYapi)) kod = OrtakFonksiyonlar.KodNoktaKoy(hesYapi, kod.Replace(".", ""));
                ObjectArray hv = TNS.MUH.Arac.Tanimla().HesapPlaniListele(kullanan, new HesapPlaniSatir { yil = y, hesapKod = kod });
                if (hv != null && hv.objeler != null)
                    foreach (HesapPlaniSatir d in hv.objeler)
                        if (d.hesapKod == kod) return Results.Json(new { kod = d.hesapKod, ad = d.aciklama ?? "", detay = d.detay });
                return Results.Json(new { kod = kod, ad = "", detay = false });
            }, "Hesap kodu okunamadı.");
        }

        /// <summary>Kişi / firma arama (eski HarcamaSurecleri/ListeFirmaKisi.aspx, Mutemet Tanımlama'nın kişi kodu).</summary>
        private static IResult FirmaAra(HttpContext baglam, IWebHostEnvironment ortam, int? muhasebe, string ad, string vergiNo)
        {
            return Oku(baglam, k => SecimYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                ad = Temiz(ad);
                vergiNo = Temiz(vergiNo);
                if (ad == "" && vergiNo == "") return Hata("Aramak için ad ya da kimlik / vergi no yazınız.");
                TNS.HRC.FirmaBilgisi f = new TNS.HRC.FirmaBilgisi();
                f.muhasebe = muhasebe ?? 0;
                f.ad = ad;
                f.vergiNo = vergiNo;
                f.aktifDurum = 1;
                ObjectArray bilgi = TNS.HRC.Arac.Tanimla().FirmaBilgisiListele(kullanan, f);
                if (bilgi == null || !bilgi.sonuc.islemSonuc) return Hata(HataMetni(bilgi?.sonuc.hataStr, "Arama yapılamadı."));
                List<object> satirlar = new List<object>();
                foreach (TNS.HRC.FirmaBilgisi x in bilgi.objeler)
                {
                    satirlar.Add(new { vergiNo = x.vergiNo ?? "", ad = x.ad ?? "", vergiDairesi = x.vergiDairesi ?? "", adres = x.adres ?? "" });
                    if (satirlar.Count >= 500) break;
                }
                return Results.Json(new { satirlar = satirlar });
            }, "Arama yapılamadı.");
        }
    }
}
