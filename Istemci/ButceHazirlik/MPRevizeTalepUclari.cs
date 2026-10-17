using System;
using System.Collections;
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
using TNS.BHMv2;
using TNS.BKD;
using TNS.KYM;
using TNS.Raporlama;
using TNS.YAT;

namespace Istemci.ButceHazirlik
{
    /// <summary>Masraf Planı Durumu seçimleri (yıl, başlangıç hali, muhasebe - birim, kodlar).</summary>
    public class MPRevizeTalepSecim
    {
        public int Yil { get; set; }
        public bool IlkHali { get; set; }
        public string Muhasebe { get; set; }
        public string Birim { get; set; }
        public string Kur { get; set; }
        public string Fon { get; set; }
        public string Fin { get; set; }
        public string Eko { get; set; }
        public string Peb { get; set; }
    }

    /// <summary>
    /// Masraf Planı Durumu sayfasının (eski ButceHazirlikv2/MasrafPlaniRevizeTalep.aspx, menüde "Revize Talep") uçları:
    ///   GET  /api/mprevizetalep/tanim                 yıl, kullanıcının yetkili muhasebe - birimi
    ///   GET  /api/mprevizetalep/kodlar, /kodad         kod ağacı (1, 2, 3, 4, 14) ve kod adı (1, 2, 3, 4, 58)
    ///   GET  /api/mprevizetalep/liste?...              (eski btnListele_Click: ListeleOdenek ekrana)
    ///   GET  /api/mprevizetalep/yazdir?...             (eski btnRaporYazdir2_Click "Liste Yazdır": MasrafPlaniDurum.xltx)
    ///   GET  /api/mprevizetalep/fark?...               (eski btnRaporYazdir_Click "KBS-SGB Fark": Rapor.xml 5000-13)
    ///   GET  /api/mprevizetalep/merkezeal?...          (eski btnFazlaCek_Click "Merkeze Al": Excel)
    ///   GET  /api/mprevizetalep/aylik?...              (eski AylikGuncelle: tertibin aylık harcamaları, 830 hesap)
    /// İş kuralları eskisi gibi TNS.BHMv2, TNS.MUH ve TNS.YAT servislerinden çağrılır.
    /// </summary>
    public static class MPRevizeTalepUclari
    {
        public const string Adres = "../ButceHazirlikv2/MasrafPlaniRevizeTalep.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/mprevizetalep/tanim", Tanim);
            app.MapGet("/api/mprevizetalep/kodlar", Kodlar);
            app.MapGet("/api/mprevizetalep/kodad", KodAdi);
            app.MapGet("/api/mprevizetalep/liste", Liste);
            app.MapGet("/api/mprevizetalep/yazdir", Yazdir);
            app.MapGet("/api/mprevizetalep/fark", Fark);
            app.MapGet("/api/mprevizetalep/merkezeal", MerkezeAl);
            app.MapGet("/api/mprevizetalep/aylik", Aylik);
        }

        // ------------------------------------------------------------------ yetki

        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, Adres);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        private static MPRevizeTalepSecim Secim(HttpRequest r)
        {
            return new MPRevizeTalepSecim
            {
                Yil = OrtakFonksiyonlar.ConvertToInt((string)r.Query["yil"], 0),
                IlkHali = (string)r.Query["ilkHali"] == "1" || (string)r.Query["ilkHali"] == "true",
                Muhasebe = Temiz(r.Query["muhasebe"]),
                Birim = Temiz(r.Query["birim"]),
                Kur = Temiz(r.Query["kur"]),
                Fon = Temiz(r.Query["fon"]),
                Fin = Temiz(r.Query["fin"]),
                Eko = Temiz(r.Query["eko"]),
                Peb = Temiz(r.Query["peb"])
            };
        }

        // ------------------------------------------------------------------ açılış, kod ağacı, kod adı

        /// <summary>Açılış (eski Page_Load): bu yıl ve kullanıcının yetkili muhasebe - birimi (TNS.MUH.Arac.YetkiliBirimBul).</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            string muhasebe = "", birim = "";
            try { TNS.MUH.Arac.YetkiliBirimBul(kullanan, out muhasebe, out birim); } catch { }
            return Results.Json(new { yil = DateTime.Now.Year, muhasebe = muhasebe ?? "", birim = birim ?? "" });
        }

        private static IResult Kodlar(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kod, string ust)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return MPTavanUclari.KodAgaci(kullanan, yil, kod, ust);
        }

        private static IResult KodAdi(HttpContext baglam, IWebHostEnvironment ortam, int yil, int tur, string kod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return MPTavanUclari.KodAdiBul(yil, tur, kod);
        }

        // ------------------------------------------------------------------ ödenek listesi (eski ListeleOdenek)

        /// <summary>
        /// Eski ListeleOdenek: muhasebe - birimin tertiplerinde gönderilen (TABOdenekGonderme ;5), harcanan (TABOdemeEmri) ve masraf
        /// planının 12 ayı (TABMasrafPlaniBirim; ekrana: başlangıç hali işaretliyse aşama 5, değilse 9; Excel'e: önce 5, sonra 9).
        /// Ekonomik kod kırılımı ve alt faaliyet ödenek kontrol türüne göre (OdenekKontrolTurBul) eskisi gibidir.
        /// </summary>
        private static OdenekBilgi[] OdenekListele(Kullanici kullanan, MPRevizeTalepSecim s, bool ekrana)
        {
            if (s.Yil <= 0) throw new Exception("Yıl giriniz.");
            if (s.Muhasebe == "" || s.Birim == "") throw new Exception("Saymanlık ve birim seçiniz.");
            TNS.MUH.IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
            if (!servisMUH.MuhasebeBirimYetkiVarMi(kullanan, s.Muhasebe, s.Birim, TNS.MUH.ENUMOGY.Gorme))
                throw new Exception("Birimi görmeye yetkiniz yok!");

            TNS.MUH.ENUMOdenekKontrolTur okTur = TNS.MUH.Arac.OdenekKontrolTurBul(s.Yil);
            Tertip kistasTertip = new Tertip(s.Yil, ENUMTabloKod.TABTasraOdenek);
            kistasTertip.TertibeKodEkle(0, s.Kur, "");
            kistasTertip.TertibeKodEkle(1, s.Fon, "");
            kistasTertip.TertibeKodEkle(2, s.Fin, "");
            kistasTertip.TertibeKodEkle(3, s.Eko, "");
            kistasTertip.TertibeKodEkle(4, s.Peb, "");
            kistasTertip.TertibeKodEkle(5, "@" + s.Muhasebe + "." + s.Birim, "");

            UcgenKodBilgileri ukb = new UcgenKodBilgileri();
            ukb.Add(new UcgenKodBilgi(kistasTertip.KodNolar(0), kistasTertip.KodNoKirilimSay(0)));
            ukb.Add(new UcgenKodBilgi(kistasTertip.KodNolar(1), kistasTertip.KodNoKirilimSay(1)));
            ukb.Add(new UcgenKodBilgi(kistasTertip.KodNolar(2), kistasTertip.KodNoKirilimSay(2)));
            if (okTur == TNS.MUH.ENUMOdenekKontrolTur.E2OG_E4OE_E2K || okTur == TNS.MUH.ENUMOdenekKontrolTur.E2POG_E4POE_E2PK_E4PTALEP)
                ukb.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), 2, TNS.BKD.Arac.Eko2_3Bul(kullanan)));
            else
                ukb.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), kistasTertip.KodNoKirilimSay(3)));
            if (okTur != TNS.MUH.ENUMOdenekKontrolTur.E2OG_E4OE_E2K)
                ukb.Add(new UcgenKodBilgi(kistasTertip.KodNolar(4), kistasTertip.KodNoKirilimSay(4)));

            EkDegerParam[] ekBilgiler = new EkDegerParam[14 + (ekrana ? 0 : 12)];
            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABOdenekGonderme, 0, 0, ";5");
            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABOdemeEmri, 0, 0, "");
            if (ekrana)
                AylariEkle(ekBilgiler, 2, s.IlkHali ? "5" : "9");
            else
            {
                AylariEkle(ekBilgiler, 2, "5");
                AylariEkle(ekBilgiler, 14, "9");
            }
            return TNS.BHMv2.Arac.Tanimla().ListeTertipOl(kullanan, kistasTertip, ukb, ekBilgiler, false, false, true) ?? new OdenekBilgi[0];
        }

        private static void AylariEkle(EkDegerParam[] ekBilgiler, int basla, string asama)
        {
            for (int ay = 0; ay < 12; ay++)
                ekBilgiler[basla + ay] = new EkDegerParam(ENUMTabloKod.TABMasrafPlaniBirim, 0, 0, asama + ";" + (ay + 1));
        }

        /// <summary>Eski FaaliyetBilgiBul: alt faaliyetin sektör ve proje bilgisi (aynı alt faaliyet bir kez okunur).</summary>
        private static AltFaalBilgi FaaliyetBilgiBul(Kullanici kullanan, IYATServis servisYat, int yil, string faalKod, Hashtable ht)
        {
            faalKod = faalKod ?? "";
            AltFaalBilgi afb = ht[faalKod] as AltFaalBilgi;
            if (afb == null && !ht.ContainsKey(faalKod))
            {
                try { afb = servisYat.AltFaaliyetBilgiBul(kullanan, yil, faalKod, "", "0600", false); } catch { afb = null; }
                ht[faalKod] = afb;
            }
            return afb;
        }

        private static double Ek(OdenekBilgi ob, int i)
        {
            if (ob.ekRakamlar == null || i >= ob.ekRakamlar.Count || ob.ekRakamlar[i] == null) return 0;
            return Convert.ToDouble(ob.ekRakamlar[i]);
        }

        /// <summary>
        /// Listele (eski btnListele_Click / MPTertipGoster): tertip, program tertibi, ekonomik ve alt faaliyet adı, gönderilen, harcanan,
        /// 12 ayın masraf planı ve toplamı, sektör ve proje. Çeyrek toplamları eski tabloda ayrı sütunlardı; sayfada ay grubu olarak gösterilir.
        /// Toplam satırı sayfada hesaplanır.
        /// </summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            MPRevizeTalepSecim s = Secim(baglam.Request);
            try
            {
                OdenekBilgi[] obler = OdenekListele(kullanan, s, true);
                IYATServis servisYat = TNS.YAT.Arac.Tanimla();
                Hashtable ht = new Hashtable();
                List<object> satirlar = new List<object>();
                foreach (OdenekBilgi ob in obler)
                {
                    AltFaalBilgi afb = FaaliyetBilgiBul(kullanan, servisYat, s.Yil, ob.tertip.kodlar[4], ht);
                    double[] aylar = new double[12];
                    double toplam = 0;
                    for (int ay = 0; ay < 12; ay++)
                    {
                        aylar[ay] = Ek(ob, 2 + ay);
                        toplam += aylar[ay];
                    }
                    double gonderilen = Ek(ob, 0), harcanan = Ek(ob, 1);
                    satirlar.Add(new
                    {
                        tertip = ob.tertip.TertipStrOl(),
                        tertipPrg = TNS.BHMv2.Arac.ProgramTertipOl(ob.tertip, false),
                        ekoAd = ob.tertip.adlar[3] ?? "",
                        pebAd = ob.tertip.adlar[4] ?? "",
                        gonderilen = gonderilen,
                        harcanan = harcanan,
                        harcanabilir = gonderilen - harcanan,
                        a1 = aylar[0], a2 = aylar[1], a3 = aylar[2], a4 = aylar[3], a5 = aylar[4], a6 = aylar[5],
                        a7 = aylar[6], a8 = aylar[7], a9 = aylar[8], a10 = aylar[9], a11 = aylar[10], a12 = aylar[11],
                        toplam = toplam,
                        gonderilebilir = toplam - gonderilen,
                        sektorKod = afb?.sektor?.kod ?? "",
                        sektorAd = afb?.sektor?.ad ?? "",
                        projeNo = afb?.proje?.DPTProjeNo ?? "",
                        projeAd = afb?.proje?.ad ?? ""
                    });
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(string.IsNullOrWhiteSpace(ex.Message) ? "Listeleme yapılamadı." : ex.Message);
            }
        }

        // ------------------------------------------------------------------ Liste Yazdır (eski ExcelGonder)

        /// <summary>BHMv2 şablonu: önce kurumun RaporSablonYol\BHMv2 klasörü, yoksa uygulamayla gelen SGB.Net\RaporSablon\BHMv2.</summary>
        internal static string SablonBul(string uygulamaKlasoru, string sablonAd)
        {
            string kurumYol = Ayar.Oku("RaporSablonYol");
            if (kurumYol != "")
            {
                string s = Path.Combine(kurumYol, "BHMv2", sablonAd);
                if (File.Exists(s)) return s;
            }
            string u = Path.Combine(uygulamaKlasoru, "RaporSablon", "BHMv2", sablonAd);
            if (File.Exists(u)) return u;
            throw new Exception("Rapor şablonu bulunamadı (BHMv2\\" + sablonAd + ").");
        }

        /// <summary>
        /// Liste Yazdır (eski btnRaporYazdir2_Click / ExcelGonder): MasrafPlaniDurum.xltx şablonuna gönderilen, harcanan, masraf planının
        /// başlangıç (aşama 5) ve son (aşama 9) halinin aylık dağılımı, sektör; en altta toplam.
        /// </summary>
        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            MPRevizeTalepSecim s = Secim(baglam.Request);
            try
            {
                OdenekBilgi[] obler = OdenekListele(kullanan, s, false);
                AsposeLisans.Yukle();
                string sablon = SablonBul(ortam.ContentRootPath, "MasrafPlaniDurum.xltx");
                ToplamAl toplamAl = new ToplamAl(2, 26);
                toplamAl.HepsiniSifirla();
                Tablo XLS = GenelIslemler.NewTablo();
                string sonucDosyaAd = DosyaIslem.DosyaAdUret() + ".xlsx";
                XLS.DosyaAc(sablon, sonucDosyaAd);
                int bSat = 0, bSut = 0;
                int fshNo = 0, fSat = 0, fSut = 0, fSat2 = 0, fSut2 = 0;
                XLS.HucreAdAdresCoz("BaslaSatir", ref bSat, ref bSut);
                XLS.HucreAdAdresCoz("FormatSatir", ref fshNo, ref fSat, ref fSut, ref fSat2, ref fSut2);

                int satir = bSat;
                int satYuk = XLS.SatirYukseklikAl(satir);
                IYATServis servisYat = TNS.YAT.Arac.Tanimla();
                Hashtable ht = new Hashtable();
                foreach (OdenekBilgi ob in obler)
                {
                    AltFaalBilgi afb = FaaliyetBilgiBul(kullanan, servisYat, s.Yil, ob.tertip.kodlar[4], ht);
                    toplamAl.SatirSifirla(1);
                    for (int ind = 0; ind < 26; ind++)
                        toplamAl.DegerAta(1, ind, Ek(ob, ind));
                    toplamAl.ToplamEkle(0, 1);
                    XLS.HucreKopyala(fSat, fSut, fSat2, fSut2, satir, 0);
                    ExceleSatirYaz(ob.tertip, XLS, satir, toplamAl, 1, afb);
                    satir++;
                }
                XLS.HucreKopyala(fSat, fSut, fSat2, fSut2, satir, 0);
                ExceleSatirYaz(null, XLS, satir, toplamAl, 0, null);
                XLS.SatirYukseklikAyarla(bSat, satir, -1, 3, satYuk);
                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(sonucDosyaAd, "MasrafPlaniDurum" + DateTime.Now.Ticks + ".xlsx", true, "xlsx");
            }
            catch (Exception ex)
            {
                return Hata(string.IsNullOrWhiteSpace(ex.Message) ? "Rapor hazırlanamadı." : ex.Message);
            }
        }

        private static void ExceleSatirYaz(Tertip tertip, Tablo XLS, int satir, ToplamAl toplamAl, int topSat, AltFaalBilgi afb)
        {
            if (tertip != null)
            {
                XLS.HucreDegerYaz(satir, 0, tertip.TertipStrOl());
                XLS.HucreDegerYaz(satir, 1, tertip.adlar[3] ?? "");
                XLS.HucreDegerYaz(satir, 2, tertip.adlar[4] ?? "");
            }
            else
                XLS.HucreDegerYaz(satir, 0, "T O P L A M");

            XLS.HucreDegerYaz(satir, 3, toplamAl.toplam[topSat, 0]);
            XLS.HucreDegerYaz(satir, 4, toplamAl.toplam[topSat, 1]);

            int ayYer = 0;
            for (int ay = 0; ay < 12; ay++)
            {
                XLS.HucreDegerYaz(satir, 6 + ayYer, toplamAl.toplam[topSat, 2 + ay]);
                XLS.HucreDegerYaz(satir, 23 + ayYer, toplamAl.toplam[topSat, 14 + ay]);
                ayYer++;
                if (ay == 2 || ay == 5 || ay == 8 || ay == 11)
                    ayYer++;
            }
            if (afb != null && afb.sektor != null)
                XLS.HucreDegerYaz(satir, 42, afb.sektor.kod + "-" + afb.sektor.ad);
        }

        // ------------------------------------------------------------------ KBS-SGB Fark (eski btnRaporYazdir_Click)

        /// <summary>KBS-SGB Fark: Rapor.xml 5000-13 (bu ay; geçmiş yıl için 12. ay), muhasebe - birim, varsayılan kurum, Excel.</summary>
        private static IResult Fark(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            MPRevizeTalepSecim s = Secim(baglam.Request);
            if (s.Yil <= 0) return Hata("Yıl giriniz.");
            if (s.Muhasebe == "" || s.Birim == "") return Hata("Saymanlık ve birim seçiniz.");
            try
            {
                AsposeLisans.Yukle();
                Hashtable raporParam = new Hashtable();
                raporParam["ButceYil"] = s.Yil.ToString();
                int ay = DateTime.Now.Month;
                if (s.Yil < DateTime.Now.Year) ay = 12;
                raporParam["AyNo"] = ay.ToString();
                raporParam["Muhasebe"] = s.Muhasebe;
                raporParam["Birim"] = s.Birim;
                raporParam["Kur"] = GenelIslemlerIstemci.VarsayilanKurumBul();
                raporParam["EkoKirilim"] = "4";
                raporParam["CiktiYeri"] = "EXCEL";
                string dosya = RaporlamaServis.TekRaporAl(5000, 13, kullanan, TNS.BHMv2.Arac.Tanimla(), raporParam, null);
                if (RaporlamaServis.MesajMi(dosya)) return Hata(dosya);
                string uzanti = Path.GetExtension(dosya).TrimStart('.').ToLowerInvariant();
                return DosyaIslem.DosyaGonder(dosya, "KBS_SGB_Fark_" + DateTime.Now.Ticks + "." + uzanti, true, uzanti);
            }
            catch (Exception ex)
            {
                return Hata(string.IsNullOrWhiteSpace(ex.Message) ? "Rapor hazırlanamadı." : ex.Message);
            }
        }

        // ------------------------------------------------------------------ Merkeze Al (eski btnFazlaCek_Click)

        /// <summary>
        /// Merkeze Al (eski btnFazlaCek_Click): seçilen kodlarda, masraf planı (aşama 9, serbest aya kadar) gönderilenden fazla olan taşra
        /// tertiplerinde fark masraf planından aylara düşülür (MPRevizeUclari.DusIslem) ve aynı tutar merkezin (birim 9) tertibine eksi
        /// olarak yazılır. Eskisi gibi kayıt yapılmaz; satırlar (tertip, birim, 12 ay) Excel olarak indirilir; Bütçe İşlemi formuna
        /// Yükle ile alınabilir. (Eski kod tek bir satır nesnesini bütün tertiplerde yeniden kullanıyordu; burada her tertip ayrıdır.)
        /// </summary>
        private static IResult MerkezeAl(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            MPRevizeTalepSecim s = Secim(baglam.Request);
            if (s.Yil <= 0) return Hata("Yıl giriniz.");
            try
            {
                int yil = s.Yil;
                IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                Tertip kistasTertip = new Tertip(yil, ENUMTabloKod.TABMasrafPlani);
                kistasTertip.TertibeKodEkle(0, s.Kur, "");
                kistasTertip.TertibeKodEkle(1, s.Fon, "");
                kistasTertip.TertibeKodEkle(2, s.Fin, "");
                kistasTertip.TertibeKodEkle(3, s.Eko, "");
                kistasTertip.TertibeKodEkle(4, s.Peb, "");

                UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(0), kistasTertip.KodNoKirilimSay(0)));
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(1), kistasTertip.KodNoKirilimSay(1)));
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(2), kistasTertip.KodNoKirilimSay(2)));
                TNS.MUH.ENUMOdenekKontrolTur okt = TNS.MUH.Arac.OdenekKontrolTurBul(yil);
                if (okt == TNS.MUH.ENUMOdenekKontrolTur.E2OG_E4OE_E2K || okt == TNS.MUH.ENUMOdenekKontrolTur.E2POG_E4POE_E2PK_E4PTALEP)
                    kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), 2, TNS.BKD.Arac.Eko2_3Bul(kullanan)));
                else
                    kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), kistasTertip.KodNoKirilimSay(3)));
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(4), kistasTertip.KodNoKirilimSay(4)));
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(5), kistasTertip.KodNoKirilimSay(5)));

                EkDegerParam[] ekBilgiler = new EkDegerParam[2];
                int ay = TNS.BHMv2.Arac.SerbestAyBul(yil) + 1;
                ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABMasrafPlani, 0, 0, "9;0/" + ay);
                ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABOdenekGondermeKY, 0, 0, "5");
                OdenekBilgi[] obler = servis.ListeTertipOl(kullanan, kistasTertip, kodBilgileri, ekBilgiler, false) ?? new OdenekBilgi[0];

                List<MPRevizeSatir> tasra = new List<MPRevizeSatir>();
                List<MPRevizeSatir> merkez = new List<MPRevizeSatir>();
                int tSay = 0;
                foreach (OdenekBilgi ob in obler)
                {
                    if (tSay > 2000) break;
                    double mp = Ek(ob, 0);
                    double gond = Ek(ob, 1);
                    string ky = ob.tertip.kodlar[5] ?? "";
                    if (!(mp > gond) || ky.StartsWith("9")) continue;
                    ob.tertip.kodlar[5] = "";
                    MPRevizeBilgi satir = new MPRevizeBilgi { tertip = ob.tertip.TertipStrOl(), kurYapiKod = ky, dusulen = mp - gond };
                    try
                    {
                        List<MPRevizeBilgi> tek = new List<MPRevizeBilgi> { satir };
                        MPRevizeUclari.DusIslem(yil, servis, 0, new List<MPRevizeBilgi>(), tek, 0);
                        tasra.Add(MPRevizeUclari.SatirYap(yil, satir, satir.tertip + "-" + satir.kurYapiKod));
                        MerkezeEkle(yil, satir, merkez);
                        tSay++;
                    }
                    catch (Exception)
                    {
                        // Eskisi gibi dağıtılamayan tertip atlanır.
                    }
                }
                tasra.AddRange(merkez);

                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                string sonucDosyaAd = DosyaIslem.DosyaAdUret("MPRevize", "") + "." + XLS.UzantiBul();
                XLS.BosDosyaAc(sonucDosyaAd);
                int sat = 0;
                foreach (MPRevizeSatir r in tasra)
                {
                    string kyKod = r.tertip.kodlar[5];
                    r.tertip.kodlar[5] = "";
                    XLS.HucreDegerYaz(sat, 0, r.tertip.TertipStrOl());
                    XLS.HucreDegerYaz(sat, 1, kyKod);
                    for (int ayi = 0; ayi < 12; ayi++)
                        XLS.HucreDegerYaz(sat, 2 + ayi, r.odenek[ayi]);
                    sat++;
                }
                XLS.DosyaSaklaTamYol();
                string uzanti = Path.GetExtension(sonucDosyaAd).TrimStart('.');
                return DosyaIslem.DosyaGonder(sonucDosyaAd, Path.GetFileName(sonucDosyaAd), true, uzanti);
            }
            catch (Exception ex)
            {
                return Hata(string.IsNullOrWhiteSpace(ex.Message) ? "İşlem yapılamadı." : ex.Message);
            }
        }

        private static void MerkezeEkle(int yil, MPRevizeBilgi satir, List<MPRevizeSatir> merkez)
        {
            string anahtar = satir.tertip + "-9";
            MPRevizeSatir m = merkez.Find(x => x.tertip.TertipStrOl() == anahtar);
            if (m == null)
            {
                m = new MPRevizeSatir();
                m.tertip = new Tertip(yil, ENUMTabloKod.TABMasrafPlani);
                m.tertip.TertipAta(anahtar);
                merkez.Add(m);
            }
            for (int ay = 1; ay <= 12; ay++)
                m.odenek[ay - 1] -= satir.Ay(ay);
        }

        // ------------------------------------------------------------------ aylık harcama (eski AylikGuncelle)

        /// <summary>Eski AylikGuncelle: tertibin muhasebe - birimdeki ödeme emri / MİF borçları (830 hesap, iptaller hariç), ay ay ve toplam.</summary>
        private static IResult Aylik(HttpContext baglam, IWebHostEnvironment ortam, int yil, string muhasebe, string birim, string tertip)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil <= 0) return Hata("Yıl giriniz.");
            try
            {
                TNS.MUH.IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
                TNS.MUH.OdemeEmriMIFForm form = new TNS.MUH.OdemeEmriMIFForm
                {
                    yil = yil,
                    muhasebe = OrtakFonksiyonlar.ConvertToInt(Temiz(muhasebe), 0),
                    birim = Temiz(birim)
                };
                tertip = Temiz(tertip);
                if (tertip != "")
                {
                    string[] kodlar = tertip.Replace(".", "").Split('-');
                    TNS.MUH.OdemeEmriMIFDetay detay = new TNS.MUH.OdemeEmriMIFDetay(yil)
                    {
                        hesapKod = "830",
                        kur = TNS.MUH.Arac.DiziElemanAl(kodlar, 0, string.Empty),
                        fon = TNS.MUH.Arac.DiziElemanAl(kodlar, 1, string.Empty),
                        fin = TNS.MUH.Arac.DiziElemanAl(kodlar, 2, string.Empty),
                        eko = TNS.MUH.Arac.DiziElemanAl(kodlar, 3, string.Empty),
                        peb = TNS.MUH.Arac.DiziElemanAl(kodlar, 4, string.Empty)
                    };
                    form.detay = new ObjectArray();
                    form.detay.Ekle(detay);
                }
                ObjectArray liste = servisMUH.OdemeEmriMIFListele(kullanan, form, true);
                double[] aylar = new double[12];
                if (liste != null && liste.objeler != null)
                    foreach (TNS.MUH.OdemeEmriMIFForm f in liste.objeler)
                    {
                        if (f.formDurum == 9 || f.islemTarih == null || f.islemTarih.Ay < 1 || f.islemTarih.Ay > 12 || f.detay == null) continue;
                        foreach (TNS.MUH.OdemeEmriMIFDetay d in f.detay.objeler)
                            aylar[f.islemTarih.Ay - 1] += d.borc;
                    }
                List<object> satirlar = new List<object>();
                double toplam = 0;
                for (int i = 0; i < 12; i++)
                {
                    satirlar.Add(new { ay = GenelIslemler.AyAdDondur(i + 1), tutar = aylar[i] });
                    toplam += aylar[i];
                }
                return Results.Json(new { satirlar = satirlar, toplam = toplam });
            }
            catch (Exception ex)
            {
                return Hata(string.IsNullOrWhiteSpace(ex.Message) ? "Harcamalar okunamadı." : ex.Message);
            }
        }
    }
}
