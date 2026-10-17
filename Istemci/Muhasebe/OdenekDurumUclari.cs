using System;
using System.Collections.Generic;
using System.IO;
using Istemci.ButceHazirlik;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.BKD;
using TNS.BUM;
using TNS.KYM;
using TNS.MUH;

namespace Istemci.Muhasebe
{
    /// <summary>
    /// Ödenek Durum Bilgisi sayfalarının (eski ButceMuhasebe/OdenekDurumTertip.aspx ve OdenekDurumHarcamaBirim.aspx) uçları:
    ///   GET /api/odenekdurum/tanim                         : yıllar (2006 .. bu yıl), bu yıl ve varsayılan kurumsal kod
    ///   GET /api/odenekdurum/kodlar?yil=&amp;tur=&amp;ust=          : kod ağacının bir düzeyi (eski nodeLoadPar: 1 kurumsal, 2 fonksiyonel, 3 finans, 4 ekonomik, 14 faaliyet)
    ///   GET /api/odenekdurum/tertip?yil=&amp;kur=&amp;fon=&amp;fin=&amp;eko=&amp;peb=            (eski OdenekDurumTertip btnListele_Click)
    ///   GET /api/odenekdurum/tertip/yazdir?...                                  (eski OdenekDurumTertip btnYazdir_Click; MUH-OdenekDurum.xlt)
    ///   GET /api/odenekdurum/birim?yil=&amp;muhasebe=&amp;birim=&amp;kur=...              (eski OdenekDurumHarcamaBirim btnListele_Click + MasrafCetveliMuhasebe)
    ///   GET /api/odenekdurum/birim/yazdir?...                                   (eski OdenekDurumHarcamaBirim btnYazdir_Click)
    /// Hesaplar eski sayfalardaki gibi IMUHServis.OdenekDurum, IBUMServis.AHPSonDurumGoster, IMUHServis.HarcamaDurumListesi,
    /// talep toplamı (GondermeTenkisListele) ve avans (MUHODEMEEMRIMIF uzayı) ile yapılır. Değerler eski etiket adlarıyla döner.
    /// </summary>
    public static class OdenekDurumUclari
    {
        public const string TertipAdres = "../ButceMuhasebe/OdenekDurumTertip.aspx";
        public const string BirimAdres = "../ButceMuhasebe/OdenekDurumHarcamaBirim.aspx";

        /// <summary>Eski sayfada "#,##0" ile (kuruşsuz) yazılan değerler; geri kalanlar "#,##0.00".</summary>
        public static readonly string[] TamSayiAlanlar =
        {
            "Ocak", "Subat", "Mart", "Toplam1_3", "Nisan", "Mayis", "Haziran", "Toplam4_6", "Temmuz", "Agustos", "Eylul", "Toplam7_9",
            "Ekim", "Kasim", "Aralik", "Toplam10_12", "Toplam1_6", "Toplam7_12", "ToplamAHP",
            "KBO", "Eklenen", "Dusulen", "TOdenek", "Serbest", "Bloke", "Kullanilabilir"
        };

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/odenekdurum/tanim", Tanim);
            app.MapGet("/api/odenekdurum/kodlar", Kodlar);
            app.MapGet("/api/odenekdurum/tertip", TertipListe);
            app.MapGet("/api/odenekdurum/tertip/yazdir", TertipYazdir);
            app.MapGet("/api/odenekdurum/birim", BirimListe);
            app.MapGet("/api/odenekdurum/birim/yazdir", BirimYazdir);
        }

        public static bool TertipYetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, TertipAdres);
        }

        public static bool BirimYetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, BirimAdres);
        }

        /// <summary>birimSayfasi null ise iki sayfadan birine yetki yeter (ortak uçlar).</summary>
        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, bool? birimSayfasi, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            string kok = ortam.ContentRootPath;
            bool yetkili = birimSayfasi == null
                ? TertipYetkiVar(kullanan, kok) || BirimYetkiVar(kullanan, kok)
                : birimSayfasi.Value ? BirimYetkiVar(kullanan, kok) : TertipYetkiVar(kullanan, kok);
            if (!yetkili) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = Temiz(hata);
            return hata == "" ? varsayilan : hata;
        }

        // ------------------------------------------------------------------ açılış ve kod ağacı

        /// <summary>Açılış (eski Page_Load): yıl listesi 2006'dan bu yıla, varsayılan yıl bu yıl, varsayılan kurumsal kod.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, null, out Kullanici kullanan);
            if (ret != null) return ret;
            List<int> yillar = new List<int>();
            for (int i = DateTime.Now.Year; i >= 2006; i--) yillar.Add(i);
            string kurum = "";
            try { kurum = GenelIslemlerIstemci.VarsayilanKurumBul() ?? ""; } catch (Exception) { }
            return Results.Json(new { yil = DateTime.Now.Year, yillar = yillar, kurum = kurum, tamSayilar = TamSayiAlanlar });
        }

        /// <summary>Kod ağacı (eski nodeLoadPar(kodNo, yil, ...): yetki süzmesi olmadan).</summary>
        private static IResult Kodlar(HttpContext baglam, IWebHostEnvironment ortam, int yil, string tur, string ust)
        {
            IResult ret = Denetle(baglam, ortam, null, out Kullanici kullanan);
            if (ret != null) return ret;
            tur = Temiz(tur);
            if (tur != "1" && tur != "2" && tur != "3" && tur != "4" && tur != "14") return Hata("Kod türü hatalı.");
            return GorusmeCariUclari.KodDuzeyi(kullanan, yil, tur, ust, false);
        }

        // ------------------------------------------------------------------ hesap

        /// <summary>
        /// Eski Listele: ödenek, ödenek gönderme / tenkis, harcama, avans, talep; tertip sayfasında AHP ve KBÖ / serbest / bloke da.
        /// Hata olursa null döner ve hata doldurulur.
        /// </summary>
        private static Dictionary<string, double> Hesapla(Kullanici kullanan, int yil, int muhasebe, string birim,
            string kur, string fon, string fin, string eko, string peb, bool birimSayfasi, out string hata)
        {
            hata = null;
            kur = Temiz(kur).Replace(".", "");
            fon = Temiz(fon).Replace(".", "");
            fin = Temiz(fin).Replace(".", "");
            eko = Temiz(eko).Replace(".", "");
            peb = Temiz(peb).Replace(".", "");

            if (birimSayfasi && muhasebe == 0)
            {
                hata = "Lütfen Muhasebe kodunu boş bırakmayınız";
                return null;
            }

            OdenekDurumKriter kriter = new OdenekDurumKriter();

            if (eko != "")
            {
                Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABPEBOdenek);
                string kirilim = (string)t.Kirilimlar(3);

                if (eko.Length > kirilim.Replace(".", "").Length)
                {
                    eko = eko.Substring(0, kirilim.Replace(".", "").Length);
                    eko = OrtakFonksiyonlar.KodNoktaKoy(kirilim, eko);
                }
            }

            kriter.yil = yil;
            if (birimSayfasi)
            {
                kriter.muhasebe = muhasebe;
                kriter.birim = Temiz(birim);
            }
            kriter.tertip.kodlar[0] = kur;
            kriter.tertip.kodlar[1] = fon;
            kriter.tertip.kodlar[2] = fin;
            kriter.tertip.kodlar[3] = eko;
            kriter.tertip.kodlar[4] = peb;
            kriter.konsolide = true;
            kriter.harcamaGoster = true;
            kriter.odenekGondermeGoster = true;
            kriter.odemeEmriGoster = true;
            kriter.detay = ENUMOdenekDurumDetay.KONSOLIDE;
            kriter.yeniMetot = true;

            IMUHServis servis = TNS.MUH.Arac.Tanimla();
            ObjectArray oVAN = servis.OdenekDurum(kriter);
            if (oVAN == null || !oVAN.sonuc.islemSonuc)
            {
                hata = HataMetni(oVAN?.sonuc.hataStr, "Ödenek durumu okunamadı.");
                return null;
            }
            if (oVAN.objeler.Count == 0)
            {
                hata = HataMetni(oVAN.sonuc.hataStr, "Kayıt bulunamadı.");
                return null;
            }

            TNS.MUH.OdenekDurum o = (TNS.MUH.OdenekDurum)oVAN.objeler[0];
            Dictionary<string, double> d = new Dictionary<string, double>();

            if (!birimSayfasi)
            {
                ObjectArray ahpListe = TNS.BUM.Arac.Tanimla().AHPSonDurumGoster(kriter.yil, kriter.tertip);
                if (ahpListe != null && ahpListe.sonuc.islemSonuc == true && ahpListe.objeler.Count > 0)
                {
                    TNS.BUM.TertipBilgi ahp = (TertipBilgi)ahpListe.objeler[0];
                    string[] aylar = { "Ocak", "Subat", "Mart", "Nisan", "Mayis", "Haziran", "Temmuz", "Agustos", "Eylul", "Ekim", "Kasim", "Aralik" };
                    for (int i = 0; i < 12; i++) d[aylar[i]] = (double)ahp.ayDegerler[i];
                    double ahpToplam1_3 = (double)ahp.ayDegerler[0] + (double)ahp.ayDegerler[1] + (double)ahp.ayDegerler[2];
                    double ahpToplam4_6 = (double)ahp.ayDegerler[3] + (double)ahp.ayDegerler[4] + (double)ahp.ayDegerler[5];
                    double ahpToplam7_9 = (double)ahp.ayDegerler[6] + (double)ahp.ayDegerler[7] + (double)ahp.ayDegerler[8];
                    double ahpToplam10_12 = (double)ahp.ayDegerler[9] + (double)ahp.ayDegerler[10] + (double)ahp.ayDegerler[11];
                    d["Toplam1_3"] = ahpToplam1_3;
                    d["Toplam4_6"] = ahpToplam4_6;
                    d["Toplam7_9"] = ahpToplam7_9;
                    d["Toplam10_12"] = ahpToplam10_12;
                    d["Toplam1_6"] = ahpToplam1_3 + ahpToplam4_6;
                    d["Toplam7_12"] = ahpToplam7_9 + ahpToplam10_12;
                    d["ToplamAHP"] = ahpToplam1_3 + ahpToplam4_6 + ahpToplam7_9 + ahpToplam10_12;
                }

                d["KBO"] = o.kbo;
                d["Eklenen"] = o.eklenen;
                d["Dusulen"] = o.dusulen;
                d["TOdenek"] = o.toplamOdenek;
                d["Serbest"] = o.serbest;
                d["Bloke"] = o.bloke;
            }

            d["OnayliOdenekGonderme"] = o.onayliOdenekGonderme;
            d["OnaysizOdenekGonderme"] = o.onaysizOdenekGonderme;
            d["OnayliTenkis"] = o.onayliTenkis;
            d["OnaysizTenkis"] = o.onaysizTenkis;
            d["OnayliGONTEN"] = o.onayliOdenekGonderme - o.onayliTenkis;
            d["OnaysizGONTEN"] = o.onaysizOdenekGonderme - o.onaysizTenkis;

            d["OnayliOdemeEmri"] = o.onayliOdemeEmri;
            d["OnaysizOdemeEmri"] = o.onaysizOdemeEmri;
            d["ToplamOdemeEmri"] = o.onayliOdemeEmri + o.onaysizOdemeEmri;
            d["OnayliMIF"] = o.onayliMIF;
            d["OnaysizMIF"] = o.onaysizMIF;
            d["ToplamMIF"] = o.onayliMIF + o.onaysizMIF;

            d["OnayliODEMIF"] = o.onayliOdemeEmri + o.onayliMIF;
            d["OnaysizODEMIF"] = o.onaysizOdemeEmri + o.onaysizMIF;
            double toplamSgbHarcama = o.onayliOdemeEmri + o.onaysizOdemeEmri + o.onayliMIF + o.onaysizMIF + o.blokeEdilen + o.avans;
            d["ToplamODEMIF"] = toplamSgbHarcama;
            d["ToplamODEMIFDetay"] = o.onayliOdemeEmri + o.onaysizOdemeEmri + o.onayliMIF + o.onaysizMIF;

            d["ToplamOdenekGonderme"] = o.onayliOdenekGonderme + o.onaysizOdenekGonderme;
            d["ToplamTenkis"] = o.onayliTenkis + o.onaysizTenkis;
            double toplamGONTEN = (o.onaysizOdenekGonderme - o.onaysizTenkis) + (o.onayliOdenekGonderme - o.onayliTenkis);
            d["ToplamGONTEN"] = toplamGONTEN;

            d["Harcama"] = o.harcama;
            d["Avans"] = AvansToplamGetir(kullanan, kriter.yil, kriter.muhasebe, kriter.birim, kriter.tertip.kodlar[0], kriter.tertip.kodlar[1], kriter.tertip.kodlar[2], kriter.tertip.kodlar[3]);
            d["BlokeEdilen"] = o.blokeEdilen;
            d["OdenekTalep"] = TalepToplamGetir(kullanan, servis, kriter.yil, kriter.muhasebe, kriter.birim, kriter.tertip.kodlar[0], kriter.tertip.kodlar[1], kriter.tertip.kodlar[2], kriter.tertip.kodlar[3]);

            if (!birimSayfasi)
                d["Kullanilabilir"] = o.serbest - (o.onaysizOdenekGonderme + (o.onayliOdenekGonderme - o.onayliTenkis));

            if (toplamGONTEN > 0)
                d["SerbestKalan"] = toplamGONTEN - toplamSgbHarcama;
            else if (!birimSayfasi && o.serbest > 0)
                d["SerbestKalan"] = o.serbest - toplamSgbHarcama;
            else
                d["SerbestKalan"] = 0;

            return d;
        }

        /// <summary>Talep edilen ödenek gönderme belgelerinin toplamı (eski ButceMuhasebeGenel.TalepToplamGetir).</summary>
        private static double TalepToplamGetir(Kullanici kullanan, IMUHServis servis, int yil, int muhasebe, string birim, string kur, string fon, string fin, string eko)
        {
            GondermeTenkisForm f = new GondermeTenkisForm();
            f.belgeTur = ENUMBelgeTuru.ODENEKGONDERME;
            f.formDurum = 99; //Tüm talepler
            f.yil = yil;
            f.kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
            f.muhasebe = muhasebe;
            f.birim = birim;
            f.DortDuzeyDegisim = 0;

            GondermeTenkisDetay detay = new GondermeTenkisDetay();
            detay.tertip.kodlar[0] = kur;
            detay.tertip.kodlar[1] = fon;
            detay.tertip.kodlar[2] = fin;
            detay.tertip.kodlar[3] = eko;
            f.detay.objeler.Add(detay);

            double toplam = 0;
            ObjectArray liste = servis.GondermeTenkisListele(kullanan, f, 0);
            if (liste != null && liste.objeler != null)
                foreach (GondermeTenkisForm form in liste.objeler)
                    toplam += form.belgeToplami;
            return toplam;
        }

        /// <summary>Avans tutarı (eski ButceMuhasebeGenel.AvansToplamGetir: MUHODEMEEMRIMIF uzayı, 160-167 hesaplar).</summary>
        private static double AvansToplamGetir(Kullanici kullanan, int yil, int muhasebe, string birim, string kur, string fon, string fin, string eko)
        {
            string muhasebeStr = "";
            if (muhasebe > 0)
                muhasebeStr = muhasebe.ToString();
            return TNS.UZY.Arac.Tanimla().UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIF", yil.ToString() + "-" + muhasebeStr + "-" + birim + "-160,161,164,165,166,167-" + kur + "-" + fon + "-" + fin + "-" + eko + "-1/8--", false, "");
        }

        /// <summary>
        /// Birimlere göre ödenek gönderme / harcama listesi (eski MasrafCetveliMuhasebe: HarcamaDurumListesi, ekonomik kod 2. düzey;
        /// gönderme, harcama ve avansı olmayan satırlar atlanır). Kodlar eskisi gibi alandaki biçimiyle gönderilir.
        /// </summary>
        private static List<object> BirimSatirlari(Kullanici kullanan, int yil, int muhasebe, string birim, string kur, string fon, string fin, string eko, string peb)
        {
            OdenekDurumKriter kr = new OdenekDurumKriter();
            kr.yil = yil;
            kr.muhasebe = muhasebe;
            kr.birim = Temiz(birim);
            kr.ekoKodKirilim = 2;
            kr.harcamaGoster = true;
            kr.odenekGondermeGoster = true;
            kr.odenekGoster = false;
            kr.odemeEmriGoster = true;
            kr.avansGoster = true;
            kr.detay = ENUMOdenekDurumDetay.BIRIM;

            kr.tertip.kodlar[0] = Temiz(kur);
            kr.tertip.kodlar[1] = Temiz(fon);
            kr.tertip.kodlar[2] = Temiz(fin);
            kr.tertip.kodlar[3] = Temiz(eko);
            kr.tertip.kodlar[4] = Temiz(peb);

            List<object> satirlar = new List<object>();
            ObjectArray oVAN = TNS.MUH.Arac.Tanimla().HarcamaDurumListesi(kullanan, kr);
            if (oVAN == null || oVAN.objeler == null) return satirlar;
            foreach (TNS.MUH.OdenekDurum o in oVAN.objeler)
            {
                double toplamOdenekGonderme = (o.onaysizOdenekGonderme + o.onayliOdenekGonderme) + (o.onaysizTenkis + o.onayliTenkis);
                double sgbNetHarcama = o.onayliMIF + o.onayliOdemeEmri + o.onaysizMIF + o.onaysizOdemeEmri;

                if (toplamOdenekGonderme != 0 || sgbNetHarcama != 0 || o.harcama != 0 || o.avans != 0)
                {
                    satirlar.Add(new
                    {
                        muhasebe = o.muhasebe,
                        birim = o.birim,
                        tertip = o.tertipButce != null ? o.tertipButce.TertipStrOl() : "",
                        odenekGonderme = toplamOdenekGonderme,
                        yevmiyeli = o.harcama,
                        harcama = sgbNetHarcama,
                        avans = o.avans,
                        kalan = Math.Round(toplamOdenekGonderme - sgbNetHarcama - o.avans, 2)
                    });
                }
            }
            return satirlar;
        }

        // ------------------------------------------------------------------ listele

        private static IResult TertipListe(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur, string fon, string fin, string eko, string peb)
        {
            IResult ret = Denetle(baglam, ortam, false, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil <= 0) return Hata("Yıl seçiniz.");
            try
            {
                Dictionary<string, double> d = Hesapla(kullanan, yil, 0, "", kur, fon, fin, eko, peb, false, out string hata);
                if (d == null) return Hata(hata);
                return Results.Json(new { degerler = d });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Ödenek durumu okunamadı."));
            }
        }

        private static IResult BirimListe(HttpContext baglam, IWebHostEnvironment ortam, int yil, int? muhasebe, string birim, string kur, string fon, string fin, string eko, string peb)
        {
            IResult ret = Denetle(baglam, ortam, true, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil <= 0) return Hata("Yıl seçiniz.");
            try
            {
                Dictionary<string, double> d = Hesapla(kullanan, yil, muhasebe ?? 0, birim, kur, fon, fin, eko, peb, true, out string hata);
                if (d == null) return Hata(hata);
                return Results.Json(new { degerler = d, satirlar = BirimSatirlari(kullanan, yil, muhasebe ?? 0, birim, kur, fon, fin, eko, peb) });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Ödenek durumu okunamadı."));
            }
        }

        // ------------------------------------------------------------------ yazdır (MUH-OdenekDurum.xlt)

        /// <summary>Eski sayfa değerleri ekrandaki biçimli metinden okuduğu için kuruşsuz alanlar yuvarlanır.</summary>
        private static double Deger(Dictionary<string, double> d, string ad)
        {
            if (!d.TryGetValue(ad, out double v)) return 0;
            int hane = Array.IndexOf(TamSayiAlanlar, ad) >= 0 ? 0 : 2;
            return Math.Round(v, hane, MidpointRounding.AwayFromZero);
        }

        /// <summary>Şablonu açar, yılı ve tertip kodlarını (adlarıyla) yazar (eski btnYazdir_Click'in baş kısmı).</summary>
        private static Tablo SablonAc(int yil, string kur, string fon, string fin, string eko, out string sonucDosyaAd)
        {
            AsposeLisans.Yukle();
            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = DosyaIslem.DosyaAdUret("OdenekDurum_", "") + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(Ayar.Oku("RaporSablonYol") + "/MUH/MUH-OdenekDurum.xlt", sonucDosyaAd);

            string y = yil.ToString();
            kur = Temiz(kur); fon = Temiz(fon); fin = Temiz(fin); eko = Temiz(eko);
            XLS.HucreAdBulYaz("txtYil", y);
            XLS.HucreAdBulYaz("txtKur", kur);
            XLS.HucreAdBulYaz("txtFon", fon);
            XLS.HucreAdBulYaz("txtFin", fin);
            XLS.HucreAdBulYaz("txtEko", eko);

            if (kur != "")
                XLS.HucreAdBulYaz("txtKurAciklama", GenelIslemler.KodAd(1, y + "-" + kur, true));
            if (fon != "")
                XLS.HucreAdBulYaz("txtFonAciklama", GenelIslemler.KodAd(2, y + "-" + fon, true));
            if (fin != "")
                XLS.HucreAdBulYaz("txtFinAciklama", GenelIslemler.KodAd(3, y + "-" + fin, true));
            if (eko != "")
                XLS.HucreAdBulYaz("txtEkoAciklama", GenelIslemler.KodAd(4, y + "-" + eko, true));
            return XLS;
        }

        private static IResult TertipYazdir(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur, string fon, string fin, string eko, string peb)
        {
            IResult ret = Denetle(baglam, ortam, false, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil <= 0) return Hata("Yıl seçiniz.");
            try
            {
                Dictionary<string, double> d = Hesapla(kullanan, yil, 0, "", kur, fon, fin, eko, peb, false, out string hata);
                if (d == null) return Hata(hata);

                Tablo XLS = SablonAc(yil, kur, fon, fin, eko, out string sonucDosyaAd);
                string[,] hucreler =
                {
                    { "txtOcak", "Ocak" }, { "txtSubat", "Subat" }, { "txtMart", "Mart" }, { "txtToplam1_3", "Toplam1_3" },
                    { "txtNisan", "Nisan" }, { "txtMayis", "Mayis" }, { "txtHaziran", "Haziran" }, { "txtToplam4_6", "Toplam4_6" },
                    { "txtTemmuz", "Temmuz" }, { "txtAgustos", "Agustos" }, { "txtEylul", "Eylul" }, { "txtToplam7_9", "Toplam7_9" },
                    { "txtEkim", "Ekim" }, { "txtKasim", "Kasim" }, { "txtAralik", "Aralik" }, { "txtToplam10_12", "Toplam10_12" },
                    { "txtToplam1_6", "Toplam1_6" }, { "txtToplam7_12", "Toplam7_12" }, { "txtToplamAHP", "ToplamAHP" },
                    { "txtKBO", "KBO" }, { "txtEklenen", "Eklenen" }, { "txtDusulen", "Dusulen" }, { "txtOdenek", "TOdenek" },
                    { "txtSerbest", "Serbest" }, { "txtSerbestGer", "Serbest" }, { "txtBloke", "Bloke" },
                    { "txtOGonderme2", "OnayliOdenekGonderme" }, { "txtOGonderme1", "OnaysizOdenekGonderme" },
                    { "txtTenkis2", "OnayliTenkis" }, { "txtTenkis1", "OnaysizTenkis" },
                    { "txtGONTEN2", "OnayliGONTEN" }, { "txtGONTEN1", "OnaysizGONTEN" },
                    { "txtOdemeEmri2", "OnayliOdemeEmri" }, { "txtOdemeEmri1", "OnaysizOdemeEmri" }, { "txtOdemeEmri3", "ToplamOdemeEmri" },
                    { "txtOGonderme3", "ToplamOdenekGonderme" }, { "txtTenkis3", "ToplamTenkis" }, { "txtGONTEN3", "ToplamGONTEN" },
                    { "txtOGondermeGer", "ToplamOdenekGonderme" }, { "txtHarcamaGer", "Harcama" }, { "txtKullanGer", "Kullanilabilir" },
                    { "txtSerbestKalan", "SerbestKalan" }, { "txtBlokeGer", "BlokeEdilen" }, { "txtToplamHarcama", "ToplamODEMIF" }
                };
                for (int i = 0; i < hucreler.GetLength(0); i++)
                    XLS.HucreAdBulYaz(hucreler[i, 0], Deger(d, hucreler[i, 1]));

                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(sonucDosyaAd, "OdenekDurum", true, GenelIslemler.ExcelTur());
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }

        private static IResult BirimYazdir(HttpContext baglam, IWebHostEnvironment ortam, int yil, int? muhasebe, string birim, string kur, string fon, string fin, string eko, string peb)
        {
            IResult ret = Denetle(baglam, ortam, true, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil <= 0) return Hata("Yıl seçiniz.");
            try
            {
                Dictionary<string, double> d = Hesapla(kullanan, yil, muhasebe ?? 0, birim, kur, fon, fin, eko, peb, true, out string hata);
                if (d == null) return Hata(hata);

                Tablo XLS = SablonAc(yil, kur, fon, fin, eko, out string sonucDosyaAd);
                XLS.HucreAdBulYaz("txtOGonderme3", Deger(d, "ToplamOdenekGonderme"));
                XLS.HucreAdBulYaz("txtTenkis3", Deger(d, "ToplamTenkis"));
                XLS.HucreAdBulYaz("txtGONTEN3", Deger(d, "ToplamGONTEN"));
                XLS.HucreAdBulYaz("txtHarcamaGer", Deger(d, "Harcama"));
                XLS.HucreAdBulYaz("txtSerbestKalan", Deger(d, "SerbestKalan"));

                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(sonucDosyaAd, "OdenekDurum", true, GenelIslemler.ExcelTur());
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }
    }
}
