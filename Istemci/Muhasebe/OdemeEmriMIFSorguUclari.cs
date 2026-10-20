using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Istemci.ButceHazirlik;
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
    /// <summary>Belge arama toplu işlem isteği (eski BelgeDurumDegisikligi ve düğmeleri).</summary>
    public class OemsDurumIstegi
    {
        public int Yil { get; set; }
        /// <summary>onayla, onaykaldir, iptal, yevver, yevkaldir, cekver, cekkaldir, istata</summary>
        public string Islem { get; set; }
        public List<string> Belgeler { get; set; }
        public string YevmiyeNo { get; set; }
        /// <summary>true: yevmiye tarihi belge tarihiyle aynı olsun (eski chkIslemYevmiye).</summary>
        public bool BelgeTarihi { get; set; } = true;
        public string YevmiyeTarih { get; set; }
        public string CekNo { get; set; }
        public string Ist1 { get; set; }
        public string Ist2 { get; set; }
    }

    /// <summary>
    /// Ödeme Emri / MİF belge arama (eski ButceMuhasebe/OdemeEmriMIFSorgu.aspx; Ödeme Emri / MİF çerçevesinin "Belge Arama" sekmesi).
    /// Yetki çerçeveden (OdemeEmriMIFUclari.Yetkili: menüdeki üç çerçeve adresi + ödeme emri görme yetkisi).
    ///   GET  liste/tanim     : durumlar, belge türleri, fiş türleri, istatistik kodları (eski Page_Load)
    ///   GET  liste           : kıstaslara göre belgeler (eski btnListe_Click / Sorgula)
    ///   GET  liste/detay     : belgenin satırları (eski BeforeExpand)
    ///   GET  liste/yazdir    : Liste Yazdır (MUH-OdemeEmriMIFSorguListe.xlt; eski btnListeYazdir_Click)
    ///   GET  liste/tarihce   : belge tarihçesi (eski BelgeTarihce.aspx)
    ///   GET  cek-yazdir      : Çek İşlemleri / Yazdır (MUH-OdemeEmriMIFCekListe.XLT; eski btnCekNoYazdir_Click, Sorgula("103"))
    ///   GET  belgeler-yazdir : seçili belgelerin çıktıları tek zip (eski btnBelgeYazdir_Click)
    ///   GET  kodlar          : kurumsal / fonksiyonel / finans / ekonomik / faaliyet kod ağacı (eski nodeLoadPar)
    ///   POST liste/durum     : seçili belgelerde onayla, onay kaldır, iptal, yevmiye no ver / kaldır, çek no ver / kaldır, istatistik ata
    /// Kıstas adları eski PropertyGrid (pgFiltre) adlarıdır; tarihler yyyy-MM-dd.
    /// </summary>
    public static class OdemeEmriMIFSorguUclari
    {
        private const string Uc = "/api/odemeemrimif/";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "liste/tanim", Tanim);
            app.MapGet(Uc + "liste", Liste);
            app.MapGet(Uc + "liste/detay", ListeDetay);
            app.MapGet(Uc + "liste/yazdir", ListeYazdir);
            app.MapGet(Uc + "liste/tarihce", Tarihce);
            app.MapGet(Uc + "cek-yazdir", CekYazdir);
            app.MapGet(Uc + "belgeler-yazdir", BelgelerYazdir);
            app.MapGet(Uc + "kodlar", Kodlar);
            app.MapPost(Uc + "liste/durum", Durum);
        }

        // ------------------------------------------------------------------ ortak

        private static IMUHServis Servis() { return TNS.MUH.Arac.Tanimla(); }

        private static string Temiz(string s) { return OdemeEmriMIFUclari.Temiz(s); }

        private static IResult Hata(string hata) { return OdemeEmriMIFUclari.Hata(hata); }

        private static string HataMetni(string hata, string varsayilan) { return OdemeEmriMIFUclari.HataMetni(hata, varsayilan); }

        /// <summary>"1.234,56" ya da "1234.56" -> sayı (eski ConvertToDbl, tr-TR).</summary>
        private static double Sayi(string s)
        {
            s = Temiz(s);
            if (s == "") return 0;
            if (s.Contains(",")) s = s.Replace(".", "").Replace(",", ".");
            return double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out double d) ? d : 0;
        }

        /// <summary>Eski MizanTertip.IntDiziYap.</summary>
        private static ENUMFISTUR[] IntDiziYap(string p)
        {
            List<ENUMFISTUR> list = new List<ENUMFISTUR>();
            foreach (string item in (p ?? "").Split(','))
            {
                if (string.IsNullOrWhiteSpace(item)) continue;
                try { list.Add((ENUMFISTUR)Convert.ToInt32(item.Trim())); }
                catch { }
            }
            return list.ToArray();
        }

        private static IstatistikKod IstatistikOku(Kullanici kullanan, int kod)
        {
            try
            {
                IstatistikKod kistas = new IstatistikKod();
                kistas.kod = kod;
                return Servis().OkuIstatistikKodGiris(kullanan, kistas, true, true);
            }
            catch (Exception)
            {
                return null;
            }
        }

        // ------------------------------------------------------------------ açılış (eski Page_Load)

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = OdemeEmriMIFUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                List<int> yillar = new List<int>();
                for (int i = 2006; i <= DateTime.Now.Year + 1; i++) yillar.Add(i);

                List<object> durumlar = new List<object>
                {
                    new { kod = 1, ad = "Yeni Belgeler" },
                    new { kod = 5, ad = "Onaylı" },
                    new { kod = 23, ad = "Yevmiyesiz" },
                    new { kod = 6, ad = "Yevmiyeli" },
                    new { kod = 9, ad = "İptal Edilenler" },
                    new { kod = 22, ad = "Bütün Belgeler (İptaller Hariç)" }
                };
                List<object> belgeTurleri = new List<object>
                {
                    new { kod = 0, ad = "Hepsi" },
                    new { kod = 1, ad = "Ödeme Emri" },
                    new { kod = 2, ad = "MİF" }
                };

                // Eski ButceMuhasebeGenel.FisTurDoldur
                List<object> fisTurleri = new List<object>();
                foreach (ENUMFISTUR ft in new[] { ENUMFISTUR.DONEMICI, ENUMFISTUR.ACILIS, ENUMFISTUR.KAPANIS, ENUMFISTUR.AMORTISMANKAPANISI, ENUMFISTUR.SONFIS })
                    fisTurleri.Add(new { kod = (int)ft, ad = TNS.MUH.Arac.FisTurAdBul(ft) });

                object Istatistik(int kod)
                {
                    IstatistikKod k = IstatistikOku(kullanan, kod);
                    if (k == null) return null;
                    List<object> degerler = new List<object>();
                    if (k.istatistikKodDeger != null)
                        foreach (IstatistikKodDeger d in k.istatistikKodDeger)
                            degerler.Add(new { kod = d.degerKod, ad = d.aciklama ?? "" });
                    return new { ad = k.ad ?? "", degerler = degerler };
                }

                return Results.Json(new
                {
                    yillar = yillar,
                    yil = DateTime.Now.Year,
                    durumlar = durumlar,
                    varsayilanDurum = 1,
                    belgeTurleri = belgeTurleri,
                    varsayilanBelgeTur = 0,
                    fisTurleri = fisTurleri,
                    ist1 = Istatistik(1),
                    ist2 = Istatistik(2)
                });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Sayfa bilgileri okunamadı."));
            }
        }

        // ------------------------------------------------------------------ sorgu (eski Sorgula)

        /// <summary>Eski Sorgula(hesapKod). hesapKod "103": çek listesi (çek no verilmişse ona göre, yoksa 103 alacaklı yevmiyeli ödeme emirleri).</summary>
        private static ObjectArray Sorgula(Kullanici kullanan, IQueryCollection q, string hesapKod, string cekNo, out string hata)
        {
            hata = null;
            string Al(string ad) { return Temiz(q[ad]); }

            OdemeEmriMIFForm f = new OdemeEmriMIFForm();
            f.yil = OrtakFonksiyonlar.ConvertToInt(Al("yil"), 0);
            f.formDurum = OrtakFonksiyonlar.ConvertToInt(Al("durum"), 0);

            if (f.yil == 0 || f.formDurum == 0)
            {
                hata = "Sorgulama için yıl alanını boş bırakamazsınız.";
                return null;
            }

            f.kurum = OdemeEmriMIFUclari.Kurum();
            f.muhasebe = OrtakFonksiyonlar.ConvertToInt(Al("muhasebe"), 0);
            f.birim = Al("birim");

            if (Al("belgeTur") != "")
                f.sorguBelgeTur = OrtakFonksiyonlar.ConvertToInt(Al("belgeTur"), -1);

            string b1 = Al("belgeNo1");
            string b2 = Al("belgeNo2");

            string[] belgeNoparca = b1.Split('-', ' ', '_');
            if (belgeNoparca.Length > 0) b1 = belgeNoparca[0];

            belgeNoparca = b2.Split('-', ' ', '_');
            if (belgeNoparca.Length > 0) b2 = belgeNoparca[0];

            if (b1 != "" || b2 != "")
            {
                if (b1 != "" && b1.Length < 9)
                    b1 = f.kurum.Replace(".", "").PadRight(4, '0') + b1.PadLeft(6, '0');
                if (b2 != "" && b2.Length < 9)
                    b2 = f.kurum.Replace(".", "").PadRight(4, '0') + b2.PadLeft(6, '0');

                f.belgeNo = b1 + ";" + b2;
            }

            if (Al("yevmiyeNo1") != "" || Al("yevmiyeNo2") != "")
                f.sorguYEVYevNo = Al("yevmiyeNo1") + ";" + Al("yevmiyeNo2");

            if (Al("yevmiyeTarihi1") != "")
                f.sorguYEVYevTarih = OdemeEmriMIFUclari.Tarih(Al("yevmiyeTarihi1")).ToString() + ";";
            if (Al("yevmiyeTarihi2") != "")
                f.sorguYEVYevTarih += OdemeEmriMIFUclari.Tarih(Al("yevmiyeTarihi2")).ToString();

            if (Al("belgeTarihi1") != "")
                f.sorguIslemTarih = OdemeEmriMIFUclari.Tarih(Al("belgeTarihi1")).ToString() + ";";
            if (Al("belgeTarihi2") != "")
                f.sorguIslemTarih += OdemeEmriMIFUclari.Tarih(Al("belgeTarihi2")).ToString();

            if (Al("durumTarihi1") != "")
                f.sorguFormDurumTarih = OdemeEmriMIFUclari.Tarih(Al("durumTarihi1")).ToString() + ";";
            if (Al("durumTarihi2") != "")
                f.sorguFormDurumTarih += OdemeEmriMIFUclari.Tarih(Al("durumTarihi2")).ToString();

            f.ilgiliAd = Al("ilgili");
            f.ilgiliNo = Al("ilgiliNo");
            f.islemYapan = Al("islemYapan");
            f.islemYapanAd = kullanan.ad + " " + kullanan.soyad;
            f.projeNo = Al("yatirimProjeNo");
            f.aciklama = Al("aciklama");

            f.OEBTur = Al("OdEsTur");
            f.OEBNo = Al("OdEsNo");
            if (Al("OdEsTarihi") != "")
                f.OEBTarih = OdemeEmriMIFUclari.Tarih(Al("OdEsTarihi"));

            f.YEVCekNo = Al("cekNoSorgu");
            f.fisTurYap(IntDiziYap(Al("fisTur")));
            f.istKod1.degerKod = OrtakFonksiyonlar.ConvertToInt(Al("ist1"), 0);
            f.istKod2.degerKod = OrtakFonksiyonlar.ConvertToInt(Al("ist2"), 0);

            OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(f.yil);
            detay.hesapKod = Al("hesapNo");
            detay.hesapKod = detay.hesapKod.Split('-')[0].Trim();

            detay.altHesapKod = Al("althesapNo");
            if (!string.IsNullOrEmpty(detay.altHesapKod.Trim()))
            {
                string[] ahk = detay.altHesapKod.Split('-');
                detay.altHesapKod = ahk[0].Trim();
            }

            detay.kur = Al("kur");
            detay.fon = Al("fon");
            detay.fin = Al("fin");
            detay.eko = Al("eko");
            detay.peb = Al("peb");

            detay.borc = Sayi(Al("borc"));
            detay.alacak = Sayi(Al("alacak"));

            if (hesapKod == "103")
            {
                if (Temiz(cekNo) != "")
                    f.YEVCekNo = Temiz(cekNo);
                else
                {
                    detay.hesapKod = "103";
                    f.belgeTur = ENUMBelgeTuru.ODEMEEMRI;
                    f.formDurum = (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
                    detay.alacak = 0.00001;
                }
            }

            f.ihale = Al("ihaleMadde");
            OdemeEmriMIFUclari.SurecAta(f, Al("surecNo"));

            f.detay.objeler.Add(detay);

            ObjectArray fler = Servis().OdemeEmriMIFListele(kullanan, f, false);
            if (fler == null || !fler.sonuc.islemSonuc)
            {
                hata = HataMetni(fler?.sonuc.hataStr, "Belgeler listelenemedi.");
                return null;
            }
            return fler;
        }

        private static string BelgeTurKisa(ENUMBelgeTuru t) { return t == ENUMBelgeTuru.ODEMEEMRI ? "ÖDE" : "MİF"; }

        /// <summary>Listele (eski btnListe_Click: StoreListe alanları).</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = OdemeEmriMIFUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                ObjectArray fler = Sorgula(kullanan, baglam.Request.Query, "", "", out string hata);
                if (fler == null) return Hata(hata);
                int yil = OrtakFonksiyonlar.ConvertToInt(Temiz(baglam.Request.Query["yil"]), 0);
                List<object> satirlar = new List<object>();
                foreach (OdemeEmriMIFForm ff in fler.objeler)
                {
                    satirlar.Add(new
                    {
                        yil = ff.yil > 0 ? ff.yil : yil,
                        belgeNo = ff.belgeNo,
                        yevmiyeNo = ff.YEVYevNo,
                        belgeTur = BelgeTurKisa(ff.belgeTur),
                        belgeTarih = OdemeEmriMIFUclari.TarihYaz(ff.islemTarih),
                        muhasebe = ff.muhasebe.ToString().PadLeft(5, '0') + " - " + ff.muhasebeAd,
                        birim = ff.birim + " - " + ff.birimAd,
                        borc = ff.borcToplam,
                        alacak = ff.alacakToplam,
                        ilgili = ff.ilgiliAd,
                        yevmiyeTarih = OdemeEmriMIFUclari.TarihYaz(ff.YEVYevTarih),
                        formDurum = ff.formDurum,
                        durum = OdemeEmriMIFUclari.DurumDonustur(ff.formDurum),
                        durumTarih = OdemeEmriMIFUclari.TarihYaz(ff.formDurumTarih),
                        islemYapan = ff.islemYapan,
                        OEBNo = ff.OEBNo,
                        OEBTarih = OdemeEmriMIFUclari.TarihYaz(ff.OEBTarih),
                        cekNo = ff.YEVCekNo,
                        cekTarih = OdemeEmriMIFUclari.TarihYaz(ff.YEVCekTarih),
                        surecNo = ff.surecYil + "/" + ff.surecNo,
                        ihaleMadde = ff.ihale,
                        aciklama = ff.aciklama,
                        ist1Ad = ff.istKod1 != null ? ff.istKod1.aciklama : "",
                        ist2Ad = ff.istKod2 != null ? ff.istKod2.aciklama : "",
                        ekliDosya = ff.ekliDosyaVarmi > 0 ? "Var" : "",
                        kimlikno = ff.ilgiliNo
                    });
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Belgeler listelenemedi."));
            }
        }

        /// <summary>Satır ayrıntısı: belgenin hesap / tertip / borç / alacak satırları (eski BeforeExpand).</summary>
        private static IResult ListeDetay(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string belgeNo)
        {
            IResult ret = OdemeEmriMIFUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (Temiz(belgeNo) == "") return Hata("Belge No bilgisi alınamadı.");
            try
            {
                ObjectArray fler = OdemeEmriMIFYazdirma.Listele(kullanan, Servis(), yil ?? 0, Temiz(belgeNo));
                if (fler == null || !fler.sonuc.islemSonuc) return Hata(HataMetni(fler?.sonuc.hataStr, "Belge okunamadı."));
                List<object> satirlar = new List<object>();
                if (fler.objeler.Count > 0)
                {
                    OdemeEmriMIFForm form = (OdemeEmriMIFForm)fler.objeler[0];
                    foreach (OdemeEmriMIFDetay detay in form.detay.objeler)
                        satirlar.Add(new
                        {
                            HesapKod = detay.hesapKod,
                            Tertip = TNS.MUH.Arac.TertipStrOl(detay.kur, detay.fon, detay.fin, detay.eko, detay.peb),
                            Borc = detay.borc,
                            Alacak = detay.alacak
                        });
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Belge okunamadı."));
            }
        }

        /// <summary>Belge tarihçesi (eski BelgeTarihce.aspx; eski TarihceGoster belge türü vermezdi).</summary>
        private static IResult Tarihce(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string belgeNo, int? belgeTur)
        {
            IResult ret = OdemeEmriMIFUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if ((yil ?? 0) == 0 || Temiz(belgeNo) == "") return Hata("Yıl ve/ya Belge No bilgisi alınamadı.");
            try
            {
                ObjectArray tVAN = Servis().TarihceListele(kullanan, yil.Value, (ENUMBelgeTuru)(belgeTur ?? 0), Temiz(belgeNo));
                if (!tVAN.sonuc.islemSonuc) return Hata(HataMetni(tVAN.sonuc.hataStr, "Tarihçe okunamadı."));
                List<object> satirlar = new List<object>();
                int sira = 0;
                foreach (TarihceBilgisi t in tVAN.objeler)
                {
                    sira++;
                    satirlar.Add(new { sira = sira, islemTarih = t.islemTarih.ToLongString(), islem = t.islem, islemYapan = t.islemYapan, islemYapanAd = t.islemYapanAd });
                }
                return Results.Json(new { belge = yil + " / " + Temiz(belgeNo), satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Tarihçe okunamadı."));
            }
        }

        // ------------------------------------------------------------------ yazdırma

        /// <summary>Liste Yazdır (eski btnListeYazdir_Click; hücre düzeni aynen).</summary>
        private static IResult ListeYazdir(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = OdemeEmriMIFUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                ObjectArray fler = Sorgula(kullanan, baglam.Request.Query, "", "", out string hata);
                if (fler == null) return Hata(hata);

                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                int satir = 0;

                int satirD = 0;
                int sutunD = 0;
                string baslik = "";
                string sablonAd = "MUH-OdemeEmriMIFSorguListe.xlt";

                baslik = "ÖDEME EMRİ/MUHASEBE İŞLEM FİŞİ BELGELERİ";

                string sonucDosyaAd = DosyaIslem.DosyaAdUret("MIFListe_", "") + "." + GenelIslemler.ExcelTur();

                XLS.DosyaAc(OdenekGondermeYazdirma.SablonYol(sablonAd), sonucDosyaAd);

                XLS.HucreAdAdresCoz("txtDetaySatir", ref satirD, ref sutunD);

                satir = satirD + 1;

                XLS.HucreAdBulYaz("txtBaslik", baslik);
                string yil = Temiz(baglam.Request.Query["yil"]);
                string kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
                string kurumAd = GenelIslemlerIstemci.VarsayilanKurumBulAd();

                XLS.HucreAdBulYaz("txtYil", yil);
                XLS.HucreAdBulYaz("txtKurum", kurum + " - " + kurumAd);

                IstatistikKod istatistikKod1 = IstatistikOku(kullanan, 1);
                IstatistikKod istatistikKod2 = IstatistikOku(kullanan, 2);

                if (istatistikKod1 != null)
                {
                    XLS.HucreDegerYaz(satirD - 1, 24, istatistikKod1.ad);
                    XLS.SutunGenislikAyarla(24, 24, 30);
                }

                if (istatistikKod2 != null)
                {
                    XLS.HucreDegerYaz(satirD - 1, 25, istatistikKod2.ad);
                    XLS.SutunGenislikAyarla(25, 25, 30);
                }

                int i = 0;
                foreach (OdemeEmriMIFForm ff in fler.objeler)
                {
                    i++;
                    XLS.HucreKopyala(satirD, 0, satirD, 28, satir, 0);

                    XLS.HucreDegerYaz(satir, 1, i.ToString());
                    XLS.HucreDegerYaz(satir, 2, ff.belgeNo);
                    if (ff.YEVYevNo > 0)
                        XLS.HucreDegerYaz(satir, 3, ff.YEVYevNo);
                    XLS.HucreDegerYaz(satir, 4, ff.belgeTur.ToString());
                    XLS.HucreDegerYaz(satir, 5, ff.islemTarih.ToString());
                    XLS.HucreDegerYaz(satir, 6, ff.muhasebe.ToString().PadLeft(5, '0') + " - " + ff.muhasebeAd);
                    XLS.HucreDegerYaz(satir, 7, ff.birim + " - " + ff.birimAd);
                    XLS.HucreDegerYaz(satir, 8, ff.borcToplam);
                    XLS.HucreDegerYaz(satir, 9, ff.alacakToplam);
                    XLS.HucreDegerYaz(satir, 10, ff.ilgiliNo);
                    XLS.HucreDegerYaz(satir, 11, ff.ilgiliAd);
                    XLS.HucreDegerYaz(satir, 12, ff.YEVYevTarih.ToString());
                    XLS.HucreDegerYaz(satir, 13, OdemeEmriMIFUclari.DurumDonustur(ff.formDurum));
                    XLS.HucreDegerYaz(satir, 14, ff.formDurumTarih.ToString());
                    XLS.HucreDegerYaz(satir, 15, ff.islemYapan);

                    XLS.HucreDegerYaz(satir, 16, ff.OEBNo);
                    XLS.HucreDegerYaz(satir, 17, ff.OEBTarih.ToString());

                    XLS.HucreDegerYaz(satir, 18, ff.YEVCekNo);
                    XLS.HucreDegerYaz(satir, 19, ff.YEVCekTarih.ToString());

                    XLS.HucreDegerYaz(satir, 20, ff.surecYil.ToString() + "/" + ff.surecNo);
                    XLS.HucreDegerYaz(satir, 21, ff.ihale);

                    XLS.HucreDegerYaz(satir, 22, ff.ekliDosyaVarmi > 0 ? "Var" : "Yok");

                    XLS.HucreDegerYaz(satir, 23, ff.aciklama);
                    XLS.HucreDegerYaz(satir, 24, ff.istKod1.aciklama);
                    XLS.HucreDegerYaz(satir, 25, ff.istKod2.aciklama);

                    satir++;
                }

                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(sonucDosyaAd, "MIFListe_" + DosyaIslem.DosyaAdUretSade(), true, GenelIslemler.ExcelTur());
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }

        /// <summary>Çek İşlemleri / Yazdır (eski btnCekNoYazdir_Click: Sorgula("103") ve çek listesi şablonu).</summary>
        private static IResult CekYazdir(HttpContext baglam, IWebHostEnvironment ortam, string cekNo)
        {
            IResult ret = OdemeEmriMIFUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                ObjectArray fler = Sorgula(kullanan, baglam.Request.Query, "103", cekNo, out string hata);
                if (fler == null) return Hata(hata);

                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                int satir = 0;
                int satirD = 0;
                int sutunD = 0;
                string sablonAd = "MUH-OdemeEmriMIFCekListe.XLT";

                string sonucDosyaAd = DosyaIslem.DosyaAdUret("MIFCekListe_", "") + "." + GenelIslemler.ExcelTur();

                XLS.DosyaAc(OdenekGondermeYazdirma.SablonYol(sablonAd), sonucDosyaAd);

                XLS.HucreAdAdresCoz("txtDetaySatir", ref satirD, ref sutunD);

                satir = satirD + 1;

                string yil = Temiz(baglam.Request.Query["yil"]);
                string kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
                string kurumAd = GenelIslemlerIstemci.VarsayilanKurumBulAd();

                XLS.HucreAdBulYaz("txtYil", yil);
                XLS.HucreAdBulYaz("txtKurum", kurum + " - " + kurumAd);

                int i = 0;
                foreach (OdemeEmriMIFForm ff in fler.objeler)
                {
                    i++;
                    XLS.HucreKopyala(satirD, 0, satirD, 20, satir, 0);

                    XLS.HucreDegerYaz(satir, 1, i.ToString());
                    XLS.HucreDegerYaz(satir, 2, ff.YEVYevNo);
                    XLS.HucreDegerYaz(satir, 3, ff.YEVYevTarih.ToString());
                    XLS.HucreDegerYaz(satir, 4, ff.muhasebe.ToString().PadLeft(5, '0') + " - " + ff.muhasebeAd);
                    XLS.HucreDegerYaz(satir, 5, ff.birim + " - " + ff.birimAd);
                    XLS.HucreDegerYaz(satir, 6, ff.alacakToplam);
                    if (ff.ilgiliNo != "")
                        ff.ilgiliAd += " (" + ff.ilgiliNo + ")";
                    XLS.HucreDegerYaz(satir, 7, ff.ilgiliAd);
                    XLS.HucreDegerYaz(satir, 8, ff.YEVCekNo);

                    satir++;
                }

                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(sonucDosyaAd, "Liste", true, GenelIslemler.ExcelTur());
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }

        private static List<string> BelgeListesi(IEnumerable<string> belgeler)
        {
            List<string> liste = new List<string>();
            if (belgeler == null) return liste;
            foreach (string b in belgeler)
                if (Temiz(b) != "" && !liste.Contains(Temiz(b))) liste.Add(Temiz(b));
            return liste;
        }

        /// <summary>Liste İşlemleri / Yazdır: seçili belgelerin çıktıları tek zip dosyasında (eski btnBelgeYazdir_Click; System.IO.Compression).</summary>
        private static IResult BelgelerYazdir(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string belgeler)
        {
            IResult ret = OdemeEmriMIFUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            List<string> liste = BelgeListesi((belgeler ?? "").Split(';', ','));
            if (liste.Count == 0) return Hata("Listeden herhangi bir belge seçilmediği için işlem yapılamadı.");
            string klasor = DosyaIslem.DosyaAdUret("MIFBelgeler_", ".dir");
            try
            {
                AsposeLisans.Yukle();
                Directory.CreateDirectory(klasor);
                string sonHata = "";
                foreach (string belgeNo in liste)
                {
                    string ad = string.Join("_", belgeNo.Split(Path.GetInvalidFileNameChars()));
                    string excelYazYer = Path.Combine(klasor, ad + "." + GenelIslemler.ExcelTur());
                    string hata = OdemeEmriMIFYazdirma.Yazdir(kullanan, yil ?? 0, belgeNo, excelYazYer, ENUMBelgeTuru.TANIMSIZ,
                        out string _, out string _, out string _);
                    if (!File.Exists(excelYazYer) && Temiz(hata) != "") sonHata = hata;
                }
                if (Directory.GetFiles(klasor).Length == 0) return Hata(HataMetni(sonHata, "Belgeler yazdırılamadı."));

                string zip = DosyaIslem.DosyaAdUret("MIFBelgeler_", ".zip");
                ZipFile.CreateFromDirectory(klasor, zip);
                return DosyaIslem.DosyaGonder(zip, "Belgeler.zip", true, "zip");
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Belgeler yazdırılamadı."));
            }
            finally
            {
                try { if (Directory.Exists(klasor)) Directory.Delete(klasor, true); } catch { }
            }
        }

        // ------------------------------------------------------------------ kod ağaçları (eski nodeLoadPar: 1, 2, 3, 4, 14)

        private static IResult Kodlar(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string tur, string ust)
        {
            IResult ret = OdemeEmriMIFUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            tur = Temiz(tur);
            if (tur != "1" && tur != "2" && tur != "3" && tur != "4" && tur != "14") return Hata("Kod türü hatalı.");
            int y = yil ?? 0;
            if (y <= 2000) y = DateTime.Now.Year;
            return GorusmeCariUclari.KodDuzeyi(kullanan, y, tur, ust, false);
        }

        // ------------------------------------------------------------------ toplu durum (eski BelgeDurumDegisikligi)

        private static bool OfmEntegrasyonMu()
        {
            return TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/OFMENTEGRASYON") == "1";
        }

        /// <summary>Eski OdemeEmriMIFSorgu.OybsBelgeDurumDegistir: her belgenin OYBS kayıtları (OybsListele) muhasebeleşti / kaldırıldı yapılır.</summary>
        private static string OybsBelgeDurumDegistir(Kullanici kullanan, TNSCollection fler, int durum)
        {
            try
            {
                IMUHServis servisMUH = Servis();
                foreach (OdemeEmriMIFForm form in fler)
                {
                    int yil = form.yil;
                    string belgeNo = form.belgeNo;

                    ObjectArray liste = servisMUH.OybsListele(kullanan, yil, belgeNo);

                    Sonuc sonuc = new Sonuc();
                    foreach (string oybsId in liste.objeler)
                    {
                        sonuc = new Sonuc();
                        OybsIslem islem = new OybsIslem();
                        islem.OybsId = long.Parse(oybsId);
                        islem.islemTipi = OdemeEmriMIFUclari.OybsTip(OrtakFonksiyonlar.ConvertToInt(liste.sonuc.anahtar, 0));
                        islem.MuhasebeIslemAciklamasi = yil + "/" + belgeNo + "nolu belge ile " + DateTime.Now.ToString() + " tarihinde muhasebeleşmiştir.";

                        if (durum == (int)ENUMBelgeDurumu.ONAYLI)
                        {
                            islem.MuhasebelestiMi = true;
                        }
                        else if (durum == (int)ENUMBelgeDurumu.ONAYKALDIR || durum == (int)ENUMBelgeDurumu.IPTAL)
                        {
                            islem.MuhasebelestiMi = false;
                            islem.MuhasebeIslemAciklamasi = string.Empty;

                            sonuc = servisMUH.OybsSil(kullanan, yil, belgeNo);

                            if (!sonuc.islemSonuc)
                                break;
                        }

                        OdemeEmriWebServisleri.Kaydet(islem);
                    }

                    if (!sonuc.islemSonuc)
                        return "OYBS güncellenemedi: " + Temiz(sonuc.hataStr);
                }
                return "";
            }
            catch (Exception ex)
            {
                return "OYBS güncellenemedi: " + ex.Message;
            }
        }

        private static async Task<IResult> Durum(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, OemsDurumIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = OdemeEmriMIFUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new OemsDurumIstegi();

            string islem = Temiz(istek.Islem).ToLowerInvariant();
            bool istAta = false;
            int durum;
            string cekNo = "";
            switch (islem)
            {
                case "onayla": durum = (int)ENUMBelgeDurumu.ONAYLI; break;
                case "onaykaldir": durum = (int)ENUMBelgeDurumu.ONAYKALDIR; break;
                case "iptal": durum = (int)ENUMBelgeDurumu.IPTAL; break;
                case "yevver": durum = (int)ENUMBelgeDurumu.ICMALYEVMIYENOVER; break;
                case "yevkaldir": durum = (int)ENUMBelgeDurumu.YEVMIYEKALDIR; break;
                case "cekver": durum = (int)ENUMBelgeDurumu.CEKNOVER; cekNo = Temiz(istek.CekNo); break;
                case "cekkaldir": durum = (int)ENUMBelgeDurumu.CEKNOVER; cekNo = ""; break;
                case "istata": durum = (int)ENUMBelgeDurumu.TANIMSIZ; istAta = true; break;
                default: return OdemeEmriMIFUclari.Basarisiz("İşlem tanımsız.");
            }

            List<string> belgeler = BelgeListesi(istek.Belgeler);
            if (belgeler.Count == 0) return OdemeEmriMIFUclari.Basarisiz("Listeden herhangi bir belge seçilmediği için işlem yapılamadı.");

            try
            {
                TNSCollection fler = new TNSCollection();
                foreach (string belgeNo in belgeler)
                {
                    OdemeEmriMIFForm f = new OdemeEmriMIFForm();
                    f.yil = istek.Yil;
                    f.kurum = OdemeEmriMIFUclari.Kurum();

                    f.belgeNo = belgeNo;
                    f.islemYapan = kullanan.kullaniciKodu;
                    f.islemYapanAd = kullanan.ad + " " + kullanan.soyad;
                    f.formDurum = durum;

                    if (durum == (int)ENUMBelgeDurumu.ICMALYEVMIYENOVER)
                    {
                        if (Temiz(istek.YevmiyeNo) != "")
                            f.YEVYevNo = OrtakFonksiyonlar.ConvertToInt(Temiz(istek.YevmiyeNo), 0);
                        if (!istek.BelgeTarihi)
                        {
                            f.YEVYevTarih = OdemeEmriMIFUclari.Tarih(istek.YevmiyeTarih);
                            if (f.YEVYevTarih.isNull)
                                return OdemeEmriMIFUclari.Basarisiz("Yevmiye tarihi hatalı. Lütfen geçerli bir tarih giriniz.");
                        }
                    }
                    if (durum == (int)ENUMBelgeDurumu.CEKNOVER)
                        f.YEVCekNo = cekNo;

                    if (OrtakFonksiyonlar.ConvertToDbl(f.belgeNo) > 0)
                        fler.Add(f);
                }

                Sonuc sonuc;
                if (istAta)
                    sonuc = Servis().BelgeIstatistikKodAtaODEMIF(kullanan, fler,
                        OrtakFonksiyonlar.ConvertToInt(Temiz(istek.Ist1), 0), OrtakFonksiyonlar.ConvertToInt(Temiz(istek.Ist2), 0));
                else
                    sonuc = Servis().BelgeDurumDegistirODEMIF(kullanan, fler, true);

                string olumluBilgi;
                string olumsuzBilgi;
                if (istAta)
                {
                    olumluBilgi = "Seçilen belgelere istatistik kodu atandı.";
                    olumsuzBilgi = "İstatistik kodu atama işlemi başarısız oldu!";
                }
                else
                    OdemeEmriMIFUclari.DurumMetinleri(durum, false, out olumluBilgi, out olumsuzBilgi);

                if (!sonuc.islemSonuc)
                    return OdemeEmriMIFUclari.Basarisiz(HataMetni(olumsuzBilgi + (sonuc.hataStr ?? ""), "İşlem yapılamadı."));

                string uyari = "";
                if (!istAta && OfmEntegrasyonMu() &&
                    (durum == (int)ENUMBelgeDurumu.ONAYLI || durum == (int)ENUMBelgeDurumu.ONAYKALDIR || durum == (int)ENUMBelgeDurumu.IPTAL))
                    uyari = OybsBelgeDurumDegistir(kullanan, fler, durum);

                return Results.Json(new
                {
                    basarili = true,
                    bilgi = HataMetni(OdemeEmriMIFUclari.Duz(olumluBilgi + (sonuc.bilgiStr ?? "")), "İşlem tamamlandı."),
                    uyari = uyari
                });
            }
            catch (Exception ex)
            {
                return OdemeEmriMIFUclari.Basarisiz(ex.Message);
            }
        }
    }
}
