using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Istemci.Muhasebe;
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
using TNS.BUM;
using TNS.KYM;
using TNS.Raporlama;
using TNS.UZY;

namespace Istemci.ButceHazirlik
{
    /// <summary>Bütçe işlemi formunun bir satırı (eski MasrafPlaniRevize.GelenBilgi). Alan adları JSON'da da böyledir.</summary>
    public class MPRevizeBilgi
    {
        public string tertip { get; set; } = "";
        public string tertipPrg { get; set; } = "";
        public string kurYapiKod { get; set; } = "";
        public double eklenen { get; set; }
        public double dusulen { get; set; }
        public double ay1 { get; set; }
        public double ay2 { get; set; }
        public double ay3 { get; set; }
        public double ay4 { get; set; }
        public double ay5 { get; set; }
        public double ay6 { get; set; }
        public double ay7 { get; set; }
        public double ay8 { get; set; }
        public double ay9 { get; set; }
        public double ay10 { get; set; }
        public double ay11 { get; set; }
        public double ay12 { get; set; }
        public string altProjeNo { get; set; } = "";
        public string altProjeAd { get; set; } = "";
        public string sektorAd { get; set; } = "";
        public string bolgeler { get; set; } = "";
        public int ioiIsKod { get; set; }
        public string ioiIsAd { get; set; } = "";

        public double toplam
        {
            get
            {
                double t = 0;
                for (int i = 1; i <= 12; i++) t += Ay(i);
                return t;
            }
        }

        public string ioiIsKodAd { get { return ioiIsKod == 0 ? "" : ioiIsKod + "-" + ioiIsAd; } }

        public MPRevizeBilgi() { }

        public MPRevizeBilgi(MPRevizeBilgi b)
        {
            tertip = b.tertip;
            kurYapiKod = b.kurYapiKod;
            eklenen = b.eklenen;
            dusulen = b.dusulen;
            for (int i = 1; i <= 12; i++) AyAta(i, b.Ay(i));
        }

        public double Ay(int ayNo)
        {
            switch (ayNo)
            {
                case 1: return ay1;
                case 2: return ay2;
                case 3: return ay3;
                case 4: return ay4;
                case 5: return ay5;
                case 6: return ay6;
                case 7: return ay7;
                case 8: return ay8;
                case 9: return ay9;
                case 10: return ay10;
                case 11: return ay11;
                case 12: return ay12;
            }
            return 0;
        }

        public void AyAta(int ayNo, double deger)
        {
            deger = Math.Round(deger, 2);
            switch (ayNo)
            {
                case 1: ay1 = deger; break;
                case 2: ay2 = deger; break;
                case 3: ay3 = deger; break;
                case 4: ay4 = deger; break;
                case 5: ay5 = deger; break;
                case 6: ay6 = deger; break;
                case 7: ay7 = deger; break;
                case 8: ay8 = deger; break;
                case 9: ay9 = deger; break;
                case 10: ay10 = deger; break;
                case 11: ay11 = deger; break;
                case 12: ay12 = deger; break;
            }
        }

        internal bool RakamVarMi()
        {
            for (int ay = 1; ay <= 12; ay++)
                if (Ay(ay) != 0) return true;
            return false;
        }
    }

    /// <summary>Bütçe işlemi formu (eski MasrafPlaniRevize.aspx'in alanları).</summary>
    public class MPRevizeIstegi
    {
        public int Yil { get; set; }
        public int No { get; set; }
        public string Birim { get; set; }
        public int IslemTur { get; set; }
        public string AltIslem { get; set; }
        public string EvrakTarih { get; set; }
        public string EvrakNo { get; set; }
        public string EvrakIlgi { get; set; }
        public string Aciklama { get; set; }
        public string BumkoNo { get; set; }
        public string OdGonNo { get; set; }
        public int OdGonTur { get; set; }
        public int OdGonYil { get; set; }
        public bool ProjelerArasi { get; set; }
        public bool SektorlerArasi { get; set; }
        public bool BolgelerArasi { get; set; }
        public string OnayYaziNo { get; set; }
        public string OnayYaziTarih { get; set; }
        public int IlgiliSiraNo { get; set; } = -1;
        public string Dayanaklar { get; set; }
        public bool EkDus { get; set; }
        public List<MPRevizeBilgi> Satirlar { get; set; }
    }

    /// <summary>Belge Arama'da seçilen belgelerin durumu (9 iptal, 5 onaylı, 1 taslak).</summary>
    public class MPRevizeDurumIstegi
    {
        public int Yil { get; set; }
        public int Durum { get; set; }
        public List<int> Nolar { get; set; }
    }

    /// <summary>
    /// Bütçe İşlemi çerçevesi (eski ButceHazirlikv2/MasrafPlaniButceIslemFrame.aspx) ve alt sayfalarının uçları. Alt sayfalar
    /// (MasrafPlaniRevize "Form", MasrafPlaniRevizeSorgu "Belge Arama", MasrafPlaniTertipBilgi) menüde olmadığından yetki çerçeveden gelir.
    ///   GET  /api/mprevize/tanim?yil                  işlem türleri, birimler, masraf planı onaylı mı (MasrafPlaniGenelOnayDurumBul == 5)
    ///   GET  /api/mprevize/tur?yil&amp;tur              dayanaklar (BIDayanakListesi), alt işlemler (ButceIslemListesi), Ekle/Düş varsayılanı
    ///   GET  /api/mprevize/oku?yil&amp;no&amp;birim&amp;tur    (eski btnListele_Click: ListeleMP / ListeleBI)
    ///   POST /api/mprevize/kaydet, /dagit, /onayla    (eski btnKaydet_Click, btnRakamDagit_click, btnOnayla_Click)
    ///   GET  /api/mprevize/yazdir?yil&amp;no             (eski FormYazdir: MPRevize1/2.docx, Word)
    ///   GET  /api/mprevize/rapor?yil&amp;birim&amp;tur       (eski btnRapor1/2_Click: Rapor(Top.) / Rapor(Dağ.), MPRaporDetay)
    ///   POST /api/mprevize/odgonduret                 (eski btnOdenekGondermeUret_Click)
    ///   POST /api/mprevize/yukle, /islemuret          (eski btnDosyaYukle_Click, btnDosyadanIslemYap_click; Excel)
    ///   GET  /api/mprevize/odenekdevri?yil            (eski btnOdenekDevri_Click)
    ///   GET  /api/mprevize/pebbilgi, /tertipler, /ioi, /kodlar, /kodad
    ///   GET  /api/mprevize/tertipbilgi/tanim, /tertipbilgi, /tertipbilgi/detay   (eski MasrafPlaniTertipBilgi.aspx)
    ///   GET  /api/mprevize/sorgu/tanim, /sorgu/liste, /sorgu/rapor; POST /sorgu/durum   (eski MasrafPlaniRevizeSorgu.aspx)
    /// İş kuralları eskisi gibi TNS.BHMv2, TNS.BUM, TNS.MUH ve TNS.UZY servislerinden çağrılır.
    /// </summary>
    public static class MPRevizeUclari
    {
        public const string Adres = "../ButceHazirlikv2/MasrafPlaniButceIslemFrame.aspx";
        public const string FormAdres = "../ButceHazirlikv2/MasrafPlaniRevize.aspx";
        public const string SorguAdres = "../ButceHazirlikv2/MasrafPlaniRevizeSorgu.aspx";
        public const string TertipBilgiAdres = "../ButceHazirlikv2/MasrafPlaniTertipBilgi.aspx";
        private const char DAYANAKAYIRAC = ',';
        private const string Uc = "/api/mprevize/";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "tanim", Tanim);
            app.MapGet(Uc + "tur", Tur);
            app.MapGet(Uc + "oku", Oku);
            app.MapPost(Uc + "kaydet", Kaydet);
            app.MapPost(Uc + "dagit", Dagit);
            app.MapPost(Uc + "onayla", Onayla);
            app.MapGet(Uc + "yazdir", Yazdir);
            app.MapGet(Uc + "rapor", Rapor);
            app.MapPost(Uc + "odgonduret", OdGondUret);
            app.MapPost(Uc + "yukle", Yukle).DisableAntiforgery();
            app.MapPost(Uc + "islemuret", IslemUret).DisableAntiforgery();
            app.MapGet(Uc + "odenekdevri", OdenekDevri);
            app.MapGet(Uc + "pebbilgi", PebBilgi);
            app.MapGet(Uc + "tertipler", Tertipler);
            app.MapGet(Uc + "ioi", Ioi);
            app.MapGet(Uc + "kodlar", Kodlar);
            app.MapGet(Uc + "kodad", KodAdi);
            app.MapGet(Uc + "tertipbilgi/tanim", TertipBilgiTanim);
            app.MapGet(Uc + "tertipbilgi", TertipBilgi);
            app.MapGet(Uc + "tertipbilgi/detay", TertipBilgiDetay);
            app.MapGet(Uc + "sorgu/tanim", SorguTanim);
            app.MapGet(Uc + "sorgu/liste", SorguListe);
            app.MapPost(Uc + "sorgu/durum", SorguDurum);
            app.MapGet(Uc + "sorgu/rapor", SorguRapor);
        }

        // ------------------------------------------------------------------ yetki ve ortak

        /// <summary>Çerçeve menüdedir; alt sayfalar çerçevenin yetkisiyle açılır.</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru, string sayfaAdresi = Adres)
        {
            if (sayfaAdresi == Adres) return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, Adres);
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, sayfaAdresi, Adres);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static bool Yonetici(Kullanici kullanan)
        {
            return kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
        }

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        private static IResult Bilgi(string bilgi) { return Results.Json(new { bilgi = bilgi }); }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = Temiz(hata);
            return hata == "" ? varsayilan : hata;
        }

        private static TNSDateTime Tarih(string s)
        {
            s = Temiz(s);
            if (s != "" && DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime t))
                return new TNSDateTime(t);
            return new TNSDateTime(DateTime.MinValue);
        }

        private static string TarihYaz(TNSDateTime t)
        {
            return t == null || t.isNull ? "" : t.Oku().ToString("yyyy-MM-dd");
        }

        private static string TarihYaz(DateTime t)
        {
            return t <= DateTime.MinValue.AddDays(1) ? "" : t.ToString("yyyy-MM-dd");
        }

        private static bool MPOnayli(Kullanici kullanan, int yil)
        {
            return TNS.BHMv2.Arac.Tanimla().MasrafPlaniGenelOnayDurumBul(kullanan, yil, "", ENUMMasrafPlanAsama.AHP) == 5;
        }

        private static object KodAd(int kod, string ad) { return new { kod = kod, ad = ad }; }

        /// <summary>
        /// Eski GenelIslemlerIstemci.DDLBirimDoldur ("Ödenek Gönderme Hazırlayacağı Birimler"): yönetici bütün birimleri, diğerleri
        /// yetki değerleriyle başlayan birimleri görür; kurumEkle ise yöneticiye varsayılan kurum en başta eklenir. Ad "kod / ad".
        /// </summary>
        private static List<object> Birimler(Kullanici kullanan, int yil, bool kurumEkle)
        {
            List<object> liste = new List<object>();
            string kurum = GenelIslemlerIstemci.VarsayilanKurumBul() ?? "";
            ABSKodlarVAN akv = TNS.BKD.Arac.Tanimla().BirimListele(kullanan, yil, kurum, true);
            bool yonetici = Yonetici(kullanan);
            if (yonetici && kurumEkle && kurum != "")
                liste.Add(new { kod = kurum, ad = kurum + " / " + (GenelIslemlerIstemci.VarsayilanKurumBulAd() ?? "") });
            if (akv == null || !akv.sonuc.islemSonuc || akv.absKodlar == null) return liste;
            string yetkiStr = kullanan.YetkiTipiDegeriBul("Ödenek Gönderme Hazırlayacağı Birimler") ?? "";
            string[] yetkili = yetkiStr.Split(';');
            foreach (ABSKod ak in akv.absKodlar)
            {
                if (yonetici && kurumEkle && ak.kod == kurum) continue;
                bool ekle = yonetici;
                for (int i = 0; i < yetkili.Length && !ekle; i++)
                    if (!string.IsNullOrEmpty(yetkili[i]) && ak.kod.StartsWith(yetkili[i]))
                        ekle = true;
                if (ekle)
                    liste.Add(new { kod = ak.kod, ad = ak.kod + " / " + ak.Ad(kullanan.aktifKultur) });
            }
            return liste;
        }

        private static List<object> IslemTurleri(Kullanici kullanan)
        {
            List<object> l = new List<object>();
            l.Add(KodAd((int)ENUMRevizeFormTip.MasrafPlaniRevize, TNS.BKD.Arac.RevizeFormTipAl(ENUMRevizeFormTip.MasrafPlaniRevize)));
            if (Yonetici(kullanan))
            {
                l.Add(KodAd((int)ENUMRevizeFormTip.MasrafPlaniAktarma, TNS.BKD.Arac.RevizeFormTipAl(ENUMRevizeFormTip.MasrafPlaniAktarma)));
                l.Add(KodAd((int)ENUMRevizeFormTip.MasrafPlaniAHP, TNS.BKD.Arac.RevizeFormTipAl(ENUMRevizeFormTip.MasrafPlaniAHP)));
            }
            l.Add(KodAd((int)ENUMRevizeFormTip.AHPRevize, TNS.BKD.Arac.RevizeFormTipAl(ENUMRevizeFormTip.AHPRevize)));
            l.Add(KodAd((int)ENUMRevizeFormTip.Ekleme, TNS.BKD.Arac.RevizeFormTipAl(ENUMRevizeFormTip.Ekleme)));
            l.Add(KodAd((int)ENUMRevizeFormTip.KurumAktarma, TNS.BKD.Arac.RevizeFormTipAl(ENUMRevizeFormTip.KurumAktarma)));
            l.Add(KodAd((int)ENUMRevizeFormTip.Aktarma, TNS.BKD.Arac.RevizeFormTipAl(ENUMRevizeFormTip.Aktarma)));
            return l;
        }

        private static string UstKodBul(ENUMRevizeFormTip belgeTur)
        {
            if (belgeTur == ENUMRevizeFormTip.AHPRevize) return "01.02";
            if (belgeTur == ENUMRevizeFormTip.Ekleme) return "02.01";
            if (belgeTur == ENUMRevizeFormTip.Aktarma || belgeTur == ENUMRevizeFormTip.KurumAktarma) return "02.02";
            return "";
        }

        private static ENUMRevizeFormTip TurAl(int tur)
        {
            return tur <= 0 ? ENUMRevizeFormTip.MasrafPlaniRevize : (ENUMRevizeFormTip)tur;
        }

        // ------------------------------------------------------------------ açılış ve işlem türü

        /// <summary>
        /// Açılış ve yıl değişimi (eski Page_Load, ddlYil_Degisti): işlem türleri (yöneticiye Masraf Planı Aktarma ve MP-AHP de), Birim
        /// listesi; masraf planı onaylı değilse Raporlar, Yükle ve Birim / Tertip Prg. / İş Kod sütunları kapalıdır (kayıt BUM'a yapılır).
        /// </summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam, int? yil)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            int y = yil ?? DateTime.Now.Year;
            try
            {
                return Results.Json(new
                {
                    yil = y,
                    bugun = DateTime.Today.ToString("yyyy-MM-dd"),
                    yonetici = Yonetici(kullanan),
                    mpOnayli = MPOnayli(kullanan, y),
                    islemTurleri = IslemTurleri(kullanan),
                    islemTur = (int)ENUMRevizeFormTip.MasrafPlaniRevize,
                    birimler = Birimler(kullanan, y, false)
                });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Sayfa bilgileri okunamadı."));
            }
        }

        /// <summary>
        /// İşlem türü değişince (eski SayfaInit): dayanaklar (BIDayanakListesi), alt işlemler (ButceIslemListesi; AHP Revize 01.02, Ekleme
        /// 02.01, Aktarma / Kurum Aktarma 02.02). AHP Revize, Masraf Planı Revize ve MP-AHP'de Ekle/Düş Göster kapalı gelir.
        /// </summary>
        private static IResult Tur(HttpContext baglam, IWebHostEnvironment ortam, int yil, int tur)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            ENUMRevizeFormTip belgeTur = TurAl(tur);
            try
            {
                IBUMServis bumServis = TNS.BUM.Arac.Tanimla();
                List<object> dayanaklar = new List<object>();
                ObjectArray oa = bumServis.BIDayanakListesi(yil, (int)belgeTur);
                if (oa != null && oa.sonuc.islemSonuc && oa.objeler != null)
                    foreach (TNS.BUM.BIDayanak d in oa.objeler)
                        dayanaklar.Add(new { kod = d.dayanakKod.ToString(), ad = d.dayanakAd ?? "", aciklama = d.aciklama ?? "" });

                List<object> altIslemler = new List<object> { new { kod = "", ad = "" } };
                string ustKod = UstKodBul(belgeTur);
                if (ustKod != "")
                {
                    ObjectArray ai = bumServis.ButceIslemListesi(ustKod, false, false);
                    if (ai != null && ai.objeler != null)
                        foreach (ListeSatir ls in ai.objeler)
                            altIslemler.Add(new { kod = (ls.kod ?? "").Replace(".", ""), ad = ls.ad ?? "" });
                }
                bool ekDus = !(belgeTur == ENUMRevizeFormTip.AHPRevize || belgeTur == ENUMRevizeFormTip.MasrafPlaniRevize || belgeTur == ENUMRevizeFormTip.MasrafPlaniAHP);
                return Results.Json(new { baslik = TNS.BKD.Arac.RevizeFormTipAl(belgeTur), dayanaklar = dayanaklar, altIslemler = altIslemler, ekDus = ekDus });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "İşlem türü bilgileri okunamadı."));
            }
        }

        // ------------------------------------------------------------------ satırlar

        /// <summary>Eski KayitEkle: MPRevizeSatir -> ekran satırı (kurumsal yapı ayrılır, toplamın işaretine göre eklenen / düşülen).</summary>
        internal static MPRevizeBilgi BilgiYap(MPRevizeSatir s)
        {
            MPRevizeBilgi gb = new MPRevizeBilgi();
            if (s == null || s.tertip == null) return gb;
            gb.kurYapiKod = OrtakFonksiyonlar.DiziElemanAl(s.tertip.kodlar, 5, "");
            s.tertip.kodlar[5] = "";
            gb.tertip = s.tertip.TertipStrOl();
            gb.tertipPrg = TNS.BHMv2.Arac.ProgramTertipOl(s.tertip, false);
            gb.altProjeNo = s.DPTPROJEALTPROJENO ?? "";
            gb.altProjeAd = s.ALTPROJEAD ?? "";
            gb.sektorAd = s.sektorAd ?? "";
            gb.bolgeler = s.bolgeler ?? "";
            gb.ioiIsKod = s.IOIISKOD;
            gb.ioiIsAd = s.IOIISAD ?? "";
            if (s.odenek != null && s.odenek.Length == 12)
            {
                double satToplam = 0;
                for (int i = 0; i < 12; i++)
                {
                    satToplam += s.odenek[i];
                    gb.AyAta(i + 1, s.odenek[i]);
                }
                if (satToplam < 0) gb.dusulen = 0 - satToplam;
                else gb.eklenen = satToplam;
            }
            return gb;
        }

        private static MPRevizeBilgi BilgiYap(int yil, string tertipOn, string kurYapi, double[] odenek)
        {
            MPRevizeSatir s = new MPRevizeSatir();
            s.odenek = odenek;
            s.tertip = new Tertip(yil, ENUMTabloKod.TABMasrafPlani);
            s.tertip.TertipAta(tertipOn + "-" + kurYapi);
            return BilgiYap(s);
        }

        /// <summary>Ekran satırı -> MPRevizeSatir (tertipStr: tertip ve kurumsal yapı birleşik).</summary>
        internal static MPRevizeSatir SatirYap(int yil, MPRevizeBilgi gb, string tertipStr)
        {
            MPRevizeSatir s = new MPRevizeSatir();
            s.tertip = new Tertip(yil, ENUMTabloKod.TABMasrafPlani);
            s.tertip.TertipAta(tertipStr);
            for (int ay = 1; ay <= 12; ay++) s.odenek[ay - 1] = gb.Ay(ay);
            s.IOIISKOD = gb.ioiIsKod;
            return s;
        }

        private static List<MPRevizeBilgi> Satirlar(List<MPRevizeBilgi> l)
        {
            List<MPRevizeBilgi> r = new List<MPRevizeBilgi>();
            if (l != null)
                foreach (MPRevizeBilgi b in l)
                    if (b != null && !string.IsNullOrWhiteSpace(b.tertip))
                    {
                        b.tertip = b.tertip.Trim();
                        b.kurYapiKod = Temiz(b.kurYapiKod);
                        r.Add(b);
                    }
            return r;
        }

        // ------------------------------------------------------------------ oku

        private static object FormJson(MPRevizeForm form)
        {
            return new
            {
                yil = form.yil,
                no = form.revizeNo,
                islemTur = (int)form.revizeFormTip,
                altIslem = (form.altIslemKod ?? "").Replace(".", ""),
                evrakTarih = TarihYaz(form.evrakTarih),
                evrakNo = form.evrakNo ?? "",
                evrakIlgi = form.evrakIlgi ?? "",
                aciklama = form.aciklama ?? "",
                bumkoNo = string.IsNullOrEmpty(form.bumkoIslemNo) ? "" : form.bumkoKurumKod + "-" + form.bumkoIslemNo,
                odGonNo = form.odGonNo ?? "",
                odGonTur = form.odGonTur,
                odGonYil = form.odGonYil,
                projelerArasi = form.projelerArasi,
                sektorlerArasi = form.sektorlerArasi,
                bolgelerArasi = form.bolgelerArasi,
                onayYaziNo = form.onayYaziNo ?? "",
                onayYaziTarih = TarihYaz(form.onayYaziTarihDT),
                ilgiliSiraNo = form.ilgiliRevizeNo,
                dayanaklar = form.dayanakKodlar ?? "",
                durumAd = form.durumAd,
                onayli = form.formDurum == ENUMRevizeFormDurum.Onayli
            };
        }

        /// <summary>
        /// Oku (eski btnListele_Click): masraf planı onaylıysa MPRevizeFormOku (birim ilk satırın kurumsal kodundan), değilse BUM bütçe
        /// işlemi (BIOku: seçilen birim ve işlem türünün üst kodu). Toplamlar sayfada alt satırda hesaplanır.
        /// </summary>
        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int yil, int no, string birim, int? tur)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil <= 0) return Hata("Yıl giriniz.");
            if (no <= 0) return Hata("Belge numarası giriniz.");
            try
            {
                List<MPRevizeBilgi> satirlar = new List<MPRevizeBilgi>();
                if (MPOnayli(kullanan, yil))
                {
                    MPRevizeForm form = TNS.BHMv2.Arac.Tanimla().MPRevizeFormOku(kullanan, yil, no);
                    if (form == null) return Hata("Kayıt bulunamadı");
                    string formBirim = "";
                    if (form.satirlar != null && form.satirlar.Length > 0)
                    {
                        formBirim = form.satirlar[0].tertip.kodlar[0];
                        foreach (MPRevizeSatir s in form.satirlar)
                            satirlar.Add(BilgiYap(s));
                    }
                    return Results.Json(new { form = FormJson(form), birim = formBirim, satirlar = satirlar });
                }

                object o = TNS.BUM.Arac.Tanimla().BIOku(kullanan, yil, Temiz(birim), UstKodBul(TurAl(tur ?? 0)), no);
                if (o is Sonuc sn) return Hata(HataMetni(sn.hataStr, "Kayıt bulunamadı"));
                BIForm bf = o as BIForm;
                if (bf == null) return Hata("Kayıt bulunamadı");
                if (bf.giderler == null || bf.giderler.Count <= 0) return Hata("Kayıt bulunamadı");
                string bBirim = ((BISatir)bf.giderler[0]).tertip.kodlar[0];
                foreach (BISatir s in bf.giderler)
                    satirlar.Add(BilgiYap(yil, s.tertip.TertipStrOl(), "", s.aylikProgram));
                string dayanak = "";
                if (bf.dayanaklar != null)
                    foreach (TNS.BUM.BIDayanak d in bf.dayanaklar)
                    {
                        if (dayanak != "") dayanak += DAYANAKAYIRAC;
                        dayanak += d.dayanakKod;
                    }
                return Results.Json(new
                {
                    form = new
                    {
                        yil = bf.yil,
                        no = bf.siraNo,
                        islemTur = tur ?? 0,
                        altIslem = (bf.islemKod ?? "").Replace(".", ""),
                        evrakTarih = TarihYaz(bf.evrakTarih),
                        evrakNo = bf.evrakNo ?? "",
                        evrakIlgi = bf.ilgi ?? "",
                        aciklama = bf.notlar ?? "",
                        bumkoNo = bf.bumkoSiraNo > 0 ? bf.bumkoKurum + "-" + bf.bumkoSiraNo : "",
                        odGonNo = "",
                        odGonTur = 0,
                        odGonYil = 0,
                        projelerArasi = false,
                        sektorlerArasi = false,
                        bolgelerArasi = false,
                        onayYaziNo = "",
                        onayYaziTarih = "",
                        ilgiliSiraNo = OrtakFonksiyonlar.ConvertToInt(bf.ilgiliIslemKod, -1),
                        dayanaklar = dayanak,
                        durumAd = bf.formDurum?.durumAd ?? "",
                        onayli = false
                    },
                    birim = bBirim,
                    satirlar = satirlar
                });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Belge okunamadı."));
            }
        }

        // ------------------------------------------------------------------ kaydet, rakam dağıt, onayla

        private static void BumkoNoBul(string birlesik, out string kurum, out string siraNo)
        {
            kurum = "";
            siraNo = "";
            string[] kodlar = Temiz(birlesik).Split('-');
            if (kodlar.Length == 2)
            {
                kurum = kodlar[0];
                siraNo = kodlar[1];
            }
        }

        /// <summary>
        /// Kaydet (eski btnKaydet_Click): Ekle/Düş Göster işaretliyse önce rakamlar aylara dağıtılır (RakamDagit). Masraf planı onaylıysa
        /// MPRevizeFormKaydetEklDus (ödenek gönderme ile ilişkili kayıtlı form yalnızca yöneticiyle), değilse BUM bütçe işlemi (BIKaydet).
        /// </summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, MPRevizeIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new MPRevizeIstegi();
            if (istek.Yil <= 0) return Hata("Yıl giriniz.");
            try
            {
                IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                List<MPRevizeBilgi> gbler = Satirlar(istek.Satirlar);
                if (istek.EkDus)
                    RakamDagit(istek.Yil, servis, istek.No, gbler, Tarih(istek.EvrakTarih));

                Sonuc s;
                if (MPOnayli(kullanan, istek.Yil))
                {
                    MPRevizeForm form = new MPRevizeForm();
                    form.yil = istek.Yil;
                    form.revizeNo = istek.No;
                    form.revizeFormTip = TurAl(istek.IslemTur);
                    form.formDurum = ENUMRevizeFormDurum.Musvette;
                    form.evrakTarih = Tarih(istek.EvrakTarih);
                    form.evrakNo = Temiz(istek.EvrakNo);
                    form.evrakIlgi = Temiz(istek.EvrakIlgi);
                    form.aciklama = Temiz(istek.Aciklama);
                    form.odGonNo = Temiz(istek.OdGonNo);
                    form.odGonTur = istek.OdGonTur;
                    form.odGonYil = istek.OdGonYil;
                    form.projelerArasi = istek.ProjelerArasi;
                    form.sektorlerArasi = istek.SektorlerArasi;
                    form.bolgelerArasi = istek.BolgelerArasi;
                    form.onayYaziNo = Temiz(istek.OnayYaziNo);
                    TNSDateTime oyt = Tarih(istek.OnayYaziTarih);
                    form.onayYaziTarihDT = oyt.isNull ? DateTime.MinValue : oyt.Oku();
                    BumkoNoBul(istek.BumkoNo, out form.bumkoKurumKod, out form.bumkoIslemNo);
                    form.altIslemKod = Temiz(istek.AltIslem);
                    form.yedekTertipStr = "";
                    form.ilgiliRevizeNo = istek.IlgiliSiraNo;
                    form.dayanakKodlar = Temiz(istek.Dayanaklar);

                    List<MPRevizeSatir> al = new List<MPRevizeSatir>();
                    foreach (MPRevizeBilgi gb in gbler)
                        al.Add(SatirYap(istek.Yil, gb, gb.tertip + "-" + gb.kurYapiKod));
                    form.satirlar = al.ToArray();

                    s = new Sonuc();
                    if (form.revizeNo > 0)
                    {
                        if (!string.IsNullOrEmpty(form.odGonNo) && !Yonetici(kullanan))
                            s.DegerAta("Bu form ödenek gönderme ile ilişkili olduğundan bu ekrandan kaydetme işlemi yapamazsınız!");
                    }
                    else
                    {
                        form.odGonNo = "";
                        form.odGonTur = 0;
                        form.odGonYil = 0;
                    }
                    if (s.islemSonuc)
                        s = servis.MPRevizeFormKaydetEklDus(kullanan, form, false, istek.EkDus);
                }
                else
                {
                    BIForm form = new BIForm(istek.Yil, Temiz(istek.Birim), Temiz(istek.AltIslem), istek.No);
                    form.formDurum = new BIFormDurum((int)ENUMFormDurum.YENIKAYIT);
                    form.evrakTarih = Tarih(istek.EvrakTarih);
                    form.evrakNo = Temiz(istek.EvrakNo);
                    form.ilgi = Temiz(istek.EvrakIlgi);
                    form.notlar = Temiz(istek.Aciklama);
                    BumkoNoBul(istek.BumkoNo, out form.bumkoKurum, out string bumkoSira);
                    form.bumkoSiraNo = OrtakFonksiyonlar.ConvertToInt(bumkoSira, 0);
                    form.ilgiliIslemKod = "";
                    form.ilgiliSiraNo = 0;
                    TNSCollection dc = new TNSCollection();
                    foreach (string d in Temiz(istek.Dayanaklar).Split(DAYANAKAYIRAC, StringSplitOptions.RemoveEmptyEntries))
                    {
                        TNS.BUM.BIDayanak bi = new TNS.BUM.BIDayanak();
                        bi.dayanakKod = OrtakFonksiyonlar.ConvertToInt(d, -1);
                        dc.Add(bi);
                    }
                    form.dayanaklar = dc;
                    TNSCollection giderler = new TNSCollection();
                    foreach (MPRevizeBilgi gb in gbler)
                    {
                        BISatir satir = new BISatir();
                        satir.tertip = new Tertip(istek.Yil, ENUMTabloKod.TABPEBOdenek);
                        satir.tertip.TertipAta(gb.tertip);
                        satir.aylikProgram = new double[12];
                        double satToplam = 0;
                        for (int i = 0; i < 12; i++)
                        {
                            satir.aylikProgram[i] = gb.Ay(i + 1);
                            satToplam += satir.aylikProgram[i];
                        }
                        satir.miktar = satToplam;
                        giderler.Add(satir);
                    }
                    form.giderler = giderler;
                    s = TNS.BUM.Arac.Tanimla().BIKaydet(kullanan, form, false);
                }

                if (s.islemSonuc)
                    return Results.Json(new { bilgi = "Kayıt işlemi başarıyla tamamlandı", no = OrtakFonksiyonlar.ConvertToInt(s.anahtar, istek.No), satirlar = gbler });
                return Results.Json(new { hata = "Kayıt işlemi tamamlanamadı:" + s.hataStr, satirlar = gbler });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Kayıt yapılamadı."));
            }
        }

        /// <summary>Rakam Dağıt (eski btnRakamDagit_click): eklenen / düşülen tutarlar aylara dağıtılır, satırlar geri döner.</summary>
        private static async Task<IResult> Dagit(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, MPRevizeIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new MPRevizeIstegi();
            if (istek.Yil <= 0) return Hata("Yıl giriniz.");
            try
            {
                List<MPRevizeBilgi> gbler = Satirlar(istek.Satirlar);
                RakamDagit(istek.Yil, TNS.BHMv2.Arac.Tanimla(), istek.No, gbler, Tarih(istek.EvrakTarih));
                return Results.Json(new { satirlar = gbler });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Rakamlar dağıtılamadı."));
            }
        }

        /// <summary>
        /// Eski RakamDagit: her satırda eklenen ya da düşülenden yalnızca biri pozitif olmalıdır. Düşülenler tertibin masraf planından ilk
        /// aylardan başlayarak düşülür (DusIslem); eklenenler düşülen aylara, düşülen kalmadıysa evrak tarihinin ayına eklenir (EkleIslem).
        /// </summary>
        internal static void RakamDagit(int yil, IBHMv2Servis servis, int revizeNo, List<MPRevizeBilgi> tertipler, TNSDateTime evrakTarih)
        {
            List<MPRevizeBilgi> dusulenler = new List<MPRevizeBilgi>();
            for (int i = 0; i < tertipler.Count; i++)
            {
                if (string.IsNullOrEmpty(tertipler[i].tertip)) continue;
                if (tertipler[i].eklenen < 0 || tertipler[i].dusulen < 0)
                    throw new Exception("Eklenen ve dusulen rakamları pozitif olmalı!");
                if (tertipler[i].eklenen == 0 && tertipler[i].dusulen == 0)
                    throw new Exception("Eklenen veya dusulene rakam giriniz!");
                if (tertipler[i].eklenen > 0 && tertipler[i].dusulen > 0)
                    throw new Exception("Eklenen veya dusulenden birine rakam giriniz!");
                double topla = Math.Round(tertipler[i].eklenen - tertipler[i].dusulen, 2);
                if (topla < 0) DusIslem(yil, servis, revizeNo, dusulenler, tertipler, i);
            }
            int ay = evrakTarih == null || evrakTarih.isNull ? DateTime.Now.Month : evrakTarih.Ay;
            for (int i = 0; i < tertipler.Count; i++)
            {
                if (string.IsNullOrEmpty(tertipler[i].tertip)) continue;
                double topla = Math.Round(tertipler[i].eklenen - tertipler[i].dusulen, 2);
                if (topla > 0) EkleIslem(dusulenler, tertipler, i, ay);
            }
        }

        /// <summary>Eski DusIslem: düşülen tutar tertibin (kurumsal yapı boşsa merkez 9) masraf planından ilk aylardan başlayarak düşülür.</summary>
        public static void DusIslem(int yil, IBHMv2Servis servis, int revizeNo, List<MPRevizeBilgi> dusulenler, List<MPRevizeBilgi> tertipler, int i)
        {
            double topla = Math.Round(tertipler[i].eklenen - tertipler[i].dusulen, 2);
            Tertip t = new Tertip(yil, ENUMTabloKod.TABMasrafPlani);
            t.TertipAta(tertipler[i].tertip);
            t.kodlar[5] = tertipler[i].kurYapiKod;
            if (string.IsNullOrEmpty(t.kodlar[5])) t.kodlar[5] = "9";
            double[] durum = servis.MasrafPlaniOdenekBilgiBul(t, true, revizeNo);
            int durumBasla = 5;
            int ay = 1;
            for (; ay <= 12;)
            {
                double ayDeger = Math.Round(durum[durumBasla + ay], 2);
                if (ayDeger >= 0)
                {
                    double ayToplam = Math.Round(topla + ayDeger, 2);
                    if (ayToplam <= 0)
                    {
                        tertipler[i].AyAta(ay, 0 - ayDeger);
                        topla = Math.Round(topla + ayDeger, 2);
                    }
                    else
                    {
                        tertipler[i].AyAta(ay, topla);
                        topla = 0;
                    }
                    ay++;
                    if (topla == 0) break;
                }
                else
                    throw new Exception(tertipler[i].tertip + " ay durumu negatif olmuş.");
            }
            if (topla < 0) throw new Exception(tertipler[i].tertip + " Dağıtılamadı");
            for (; ay <= 12; ay++)
                tertipler[i].AyAta(ay, 0);
            MPRevizeBilgi dusulenDagit = new MPRevizeBilgi(tertipler[i]);
            dusulenDagit.eklenen = 0;
            dusulenDagit.dusulen = 0;
            dusulenler.Add(dusulenDagit);
        }

        private static void EkleIslem(List<MPRevizeBilgi> dusulenler, List<MPRevizeBilgi> tertipler, int i, int islemAy)
        {
            MPRevizeBilgi islenen = tertipler[i];
            double topla = Math.Round(islenen.eklenen - islenen.dusulen, 2);
            for (int a = 1; a <= 12; a++)
                islenen.AyAta(a, 0);
            while (true)
            {
                int gbSatir = dusulenler.FindIndex(x => x.RakamVarMi());
                if (gbSatir < 0)
                {
                    if (topla > 0)
                    {
                        islenen.AyAta(islemAy, islenen.Ay(islemAy) + topla);
                        topla = 0;
                    }
                    break;
                }
                MPRevizeBilgi gb = dusulenler[gbSatir];
                int ay = 1;
                for (; ay <= 12;)
                {
                    double ayDeger = gb.Ay(ay);
                    if (ayDeger >= 0) { ay++; continue; }
                    double fark = topla + ayDeger;
                    if (fark >= 0)
                    {
                        islenen.AyAta(ay, islenen.Ay(ay) + (0 - ayDeger));
                        gb.AyAta(ay, 0);
                        topla = fark;
                    }
                    else
                    {
                        islenen.AyAta(ay, islenen.Ay(ay) + topla);
                        gb.AyAta(ay, fark);
                        topla = 0;
                    }
                    if (topla == 0) break;
                    else ay++;
                }
                if (ay > 12)
                    dusulenler.RemoveAt(gbSatir);
                if (topla == 0) break;
            }
        }

        /// <summary>Onayla (eski btnOnayla_Click; sistem yöneticisi): MPRevizeFormDurumAta Onaylı.</summary>
        private static async Task<IResult> Onayla(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, MPRevizeIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!Yonetici(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            istek ??= new MPRevizeIstegi();
            if (istek.Yil <= 0 || istek.No <= 0) return Hata("Önce formu kaydediniz ya da okuyunuz.");
            try
            {
                Sonuc s = TNS.BHMv2.Arac.Tanimla().MPRevizeFormDurumAta(kullanan, istek.Yil, istek.No, ENUMRevizeFormDurum.Onayli);
                if (s.islemSonuc) return Bilgi("Form onaylandı");
                return Hata("Form onaylanamadı:" + s.hataStr);
            }
            catch (Exception ex)
            {
                return Hata("Form onaylanamadı:" + ex.Message);
            }
        }

        // ------------------------------------------------------------------ Yazdır (eski FormYazdir, Word)

        private static string SayiGosterim(double sayi, bool tlEkle, bool sifirsaBosluk = true)
        {
            if (sifirsaBosluk && sayi == 0) return "";
            string retVal = sayi.ToString("#,##0.00", CultureInfo.GetCultureInfo("tr-TR"));
            if (tlEkle) retVal += " TL";
            return retVal;
        }

        private static string KurumAntetBul(Kullanici kullanan)
        {
            TNS.HRC.IdareBilgi ib = TNS.HRC.Arac.Tanimla().KisiIdareBilgiBul(kullanan);
            if (ib != null && !string.IsNullOrWhiteSpace(ib.antet)) return ib.antet;
            return "Lütfen idare bilgisi ekranınında antet tanımlayınız!";
        }

        /// <summary>
        /// Yazdır (eski FormYazdir): AHP Revize, Masraf Planı Aktarma ve Masraf Planı Revize'de aylı şablon (MPRevize2.docx: her tertibin
        /// başlangıç, eklenen, düşülen ve son durumu ay ay), diğerlerinde MPRevize1.docx (tertip, düşülen, eklenen ve toplamlar).
        /// </summary>
        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, int yil, int no)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil <= 0 || no <= 0) return Hata("Önce formu kaydediniz ya da okuyunuz.");
            try
            {
                IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                MPRevizeForm form = servis.MPRevizeFormOku(kullanan, yil, no);
                if (form == null) return Hata("Kayıt bulunamadı");
                IBKDServis bkdServis = TNS.BKD.Arac.Tanimla();
                IBUMServis bumServis = TNS.BUM.Arac.Tanimla();

                bool aylarVar = form.revizeFormTip == ENUMRevizeFormTip.AHPRevize || form.revizeFormTip == ENUMRevizeFormTip.MasrafPlaniAktarma
                    || form.revizeFormTip == ENUMRevizeFormTip.MasrafPlaniRevize;
                AsposeLisans.Yukle();
                string sablon = MPRevizeTalepUclari.SablonBul(ortam.ContentRootPath, "MPRevize" + (aylarVar ? "2" : "1") + ".docx");
                Aspose.Words.Document doc = new Aspose.Words.Document(sablon);

                DataTable tabHeader = new DataTable { TableName = "Header" };
                tabHeader.Columns.Add("Kurum");
                tabHeader.Rows.Add(KurumAntetBul(kullanan));
                doc.MailMerge.ExecuteWithRegions(tabHeader);

                DataTable tabBaslik = new DataTable { TableName = "Baslik" };
                foreach (string k in new[] { "Sayi", "Konu", "Gerekce", "DusProjeler", "EkleProjeler", "TopDus" }) tabBaslik.Columns.Add(k);
                form.AdlariBul(bkdServis, 3);
                form.AdlariBul(bkdServis, 4);
                tabBaslik.Rows.Add(form.evrakNo, TNS.BKD.Arac.RevizeFormTipAl(form.revizeFormTip), form.aciklama, form.FaaliyetAdlariBulDusulen,
                    form.FaaliyetAdlariBulEklenen, SayiGosterim(form.ToplamEklenen, true));
                doc.MailMerge.ExecuteWithRegions(tabBaslik);

                DataTable tabDayanaklar = new DataTable { TableName = "Dayanaklar" };
                tabDayanaklar.Columns.Add("Dayanak");
                if (!string.IsNullOrWhiteSpace(form.dayanakKodlar))
                {
                    foreach (string d in form.dayanakKodlar.Split(DAYANAKAYIRAC))
                    {
                        TNS.BUM.BIDayanak bid = new TNS.BUM.BIDayanak();
                        bid.yil = form.yil;
                        bid.dayanakKod = OrtakFonksiyonlar.ConvertToInt(d, -1);
                        bid = bumServis.DayanakOku(bid);
                        tabDayanaklar.Rows.Add(bid?.dayanakAd ?? "");
                    }
                }
                else
                    tabDayanaklar.Rows.Add("");
                doc.MailMerge.ExecuteWithRegions(tabDayanaklar);

                DataTable tabIslemNo = new DataTable { TableName = "IslemNo" };
                tabIslemNo.Columns.Add("IslemNo");
                tabIslemNo.Rows.Add(form.revizeNo);
                doc.MailMerge.ExecuteWithRegions(tabIslemNo);

                DataTable tab = new DataTable { TableName = "Tertipler" };
                if (aylarVar)
                {
                    foreach (string k in new[] { "Tertip", "TertipAd", "KBO", "TEklenen", "TDusulen", "SonDurum" }) tab.Columns.Add(k);
                    for (int i = 1; i <= 14; i++)
                    {
                        tab.Columns.Add("A" + i);
                        tab.Columns.Add("E" + i);
                        tab.Columns.Add("D" + i);
                        tab.Columns.Add("S" + i);
                    }
                }
                else
                    foreach (string k in new[] { "Kur", "Fon", "Fin", "Eko", "Peb", "PebAd", "Dusulen", "Eklenen" }) tab.Columns.Add(k);

                double topDusulen = 0, topEklenen = 0;
                if (form.satirlar != null)
                {
                    foreach (MPRevizeSatir satir in form.satirlar)
                    {
                        if (aylarVar)
                        {
                            double[] baslangic = servis.MPMasrafPlaniOku(kullanan, ENUMMasrafPlanAsama.SONDURUMONAYLI, satir.tertip, 0, -1, false);
                            double[] ayDagilim = servis.MPMasrafPlaniOku(kullanan, ENUMMasrafPlanAsama.SONDURUMONAYLI, satir.tertip, -1, form.revizeNo, true);
                            DataRow row = tab.NewRow();
                            row["Tertip"] = satir.tertip.TertipStrOl();
                            row["TertipAd"] = satir.tertip.adlar[3];
                            double tEklenen = 0, tDusulen = 0, tAHP = 0, kbo = 0;
                            string alanEk;
                            for (int ay = 0; ay < 12; ay++)
                            {
                                double eklenen = 0, dusulen = 0;
                                if (satir.odenek[ay] > 0) eklenen = satir.odenek[ay];
                                else dusulen = 0 - satir.odenek[ay];
                                tEklenen += eklenen;
                                tDusulen += dusulen;
                                double ahpSon = ayDagilim[ay];
                                alanEk = (ay + 1).ToString();
                                row["A" + alanEk] = SayiGosterim(ahpSon - eklenen + dusulen, false, false);
                                row["E" + alanEk] = SayiGosterim(eklenen, false);
                                row["D" + alanEk] = SayiGosterim(dusulen, false);
                                row["S" + alanEk] = SayiGosterim(ahpSon, false, false);
                                tAHP += ahpSon;
                                kbo += baslangic[ay];
                            }
                            double sonDurum = tAHP - tEklenen + tDusulen;
                            double fark = sonDurum - kbo;
                            row["KBO"] = SayiGosterim(kbo, false, false);
                            row["TEklenen"] = fark > 0 ? SayiGosterim(fark, false, false) : "";
                            row["TDusulen"] = fark < 0 ? SayiGosterim(0 - fark, false, false) : "";
                            row["SonDurum"] = SayiGosterim(sonDurum, false);
                            if (form.revizeFormTip == ENUMRevizeFormTip.AHPRevize)
                            {
                                alanEk = "13";
                                row["A" + alanEk] = "";
                                row["D" + alanEk] = SayiGosterim(tEklenen, false);
                                row["E" + alanEk] = SayiGosterim(tDusulen, false);
                                tEklenen = 0;
                                tDusulen = 0;
                                row["S" + alanEk] = "";
                            }
                            alanEk = "14";
                            row["A" + alanEk] = SayiGosterim(sonDurum, false, false);
                            row["E" + alanEk] = SayiGosterim(tEklenen, false);
                            row["D" + alanEk] = SayiGosterim(tDusulen, false);
                            row["S" + alanEk] = SayiGosterim(tAHP, false, false);
                            tab.Rows.Add(row);
                        }
                        else
                        {
                            double eklenen = satir.Eklenen;
                            double dusulen = satir.Dusulen;
                            tab.Rows.Add(satir.tertip.kodlar[0], satir.tertip.kodlar[1], satir.tertip.kodlar[2], satir.tertip.kodlar[3], satir.tertip.kodlar[4],
                                satir.tertip.adlar[4], SayiGosterim(dusulen, false), SayiGosterim(eklenen, false));
                            topEklenen += eklenen;
                            topDusulen += dusulen;
                        }
                    }
                }
                doc.MailMerge.ExecuteWithRegions(tab);
                if (!aylarVar)
                {
                    DataTable tabToplam = new DataTable { TableName = "Toplamlar" };
                    tabToplam.Columns.Add("TopDus");
                    tabToplam.Columns.Add("TopEkle");
                    tabToplam.Rows.Add(SayiGosterim(topDusulen, false), SayiGosterim(topEklenen, false));
                    doc.MailMerge.ExecuteWithRegions(tabToplam);
                }
                string ciktiAd = DosyaIslem.DosyaAdUret("", form.revizeNo.ToString()) + ".doc";
                doc.Save(ciktiAd);
                return DosyaIslem.DosyaGonder(ciktiAd, "MPRevize" + (aylarVar ? "2" : "1") + "_" + form.revizeNo + "_" + DateTime.Now.Ticks + ".doc", true, "doc");
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Form yazdırılamadı."));
            }
        }

        // ------------------------------------------------------------------ Raporlar (eski btnRapor1/2_Click)

        /// <summary>
        /// Rapor(Top.) (tur 0: Birim "") ve Rapor(Dağ.) (tur 9: Birim "9"): seçilen birimin masraf planı tertiplerinin aylık dağılımı
        /// (eski Rapor.xml 998-20: MPRaporDetay, MPRaporDetay.xlt). Rapor sınıfı Core'da Istemci.ButceHazirlik'te olduğundan doğrudan çağrılır.
        /// </summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, int yil, string birim, string birimAd, int? tur)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil <= 0) return Hata("Yıl giriniz.");
            birim = Temiz(birim);
            if (birim == "") return Hata("Birim seçiniz.");
            try
            {
                AsposeLisans.Yukle();
                Hashtable raporParam = new Hashtable();
                raporParam["ButceYil"] = yil.ToString();
                raporParam["KurKod"] = birim;
                raporParam["KurAd"] = Temiz(birimAd);
                raporParam["Birim"] = (tur ?? 0) == 9 ? "9" : "";
                string dosya = new MPRaporDetay(ortam.ContentRootPath).RaporAl(kullanan, TNS.BHMv2.Arac.Tanimla(), "", DosyaIslem.DosyaAdUret(), "EXCEL", raporParam, "");
                string uzanti = Path.GetExtension(dosya).TrimStart('.').ToLowerInvariant();
                return DosyaIslem.DosyaGonder(dosya, "MPRapor" + ((tur ?? 0) == 9 ? "Dag" : "Top") + "_" + DateTime.Now.Ticks + "." + uzanti, true, uzanti);
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }

        // ------------------------------------------------------------------ Öd. Gönd. Üret (eski btnOdenekGondermeUret_Click)

        /// <summary>
        /// Öd.Gönd.Üret: onaylı formdaki her taşra birimi için serbest aya kadar ayların toplamı tertip tertip alınır; masraf planı
        /// gönderilenden fazlaysa ödenek gönderme, azsa tenkis belgesi oluşturulur (GondermeTenkisKaydet).
        /// </summary>
        private static async Task<IResult> OdGondUret(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, MPRevizeIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new MPRevizeIstegi();
            int yil = istek.Yil;
            if (yil <= 0 || istek.No <= 0) return Hata("Önce formu kaydediniz ya da okuyunuz.");
            try
            {
                IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                TNS.MUH.IMUHServis muhServis = TNS.MUH.Arac.Tanimla();
                MPRevizeForm form = servis.MPRevizeFormOku(kullanan, yil, istek.No);
                if (form == null || form.satirlar == null || form.satirlar.Length <= 0) return Hata("Kayıt bulunamadı");
                if (form.formDurum != ENUMRevizeFormDurum.Onayli) return Hata("Öncelikle formu onaylamanız gerekmektedir.");

                int sonAy = TNS.BHMv2.Arac.SerbestAyBul(yil);
                string aciklama = Temiz(istek.Aciklama);
                List<string> durum = new List<string>();
                for (int kaynakSat = 0; kaynakSat < form.satirlar.Length; kaynakSat++)
                {
                    string birim = form.satirlar[kaynakSat].tertip.kodlar[5];
                    if (string.IsNullOrEmpty(birim) || birim.StartsWith("9")) continue;
                    Hashtable ht = new Hashtable();
                    for (int aktifSat = kaynakSat; aktifSat < form.satirlar.Length; aktifSat++)
                    {
                        if (birim != form.satirlar[aktifSat].tertip.kodlar[5]) continue;
                        form.satirlar[aktifSat].tertip.kodlar[5] = "";
                        string anahtar = form.satirlar[aktifSat].tertip.TertipStrOl();
                        for (int ay = 0; ay < sonAy; ay++)
                        {
                            double onc = ht[anahtar] is double d ? d : 0;
                            ht[anahtar] = onc + form.satirlar[aktifSat].odenek[ay];
                        }
                    }
                    durum.Add(OdenekGondermeTenkisKaydet(kullanan, servis, muhServis, yil, sonAy, birim, ht, aciklama));
                }
                return Results.Json(new { bilgi = durum.Count == 0 ? "İşlem yapılacak taşra birimi bulunamadı." : string.Join("\n", durum), uyariVar = true });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "İşlem yapılamadı."));
            }
        }

        private static string OdenekGondermeTenkisKaydet(Kullanici kullanan, IBHMv2Servis servis, TNS.MUH.IMUHServis muhServis, int yil, int ay, string birim, Hashtable ht, string aciklama)
        {
            IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
            KurumsalYapi ky = servis.KurumsalYapiDondur(kullanan.aktifKultur, birim);
            int saymanlikKod = OrtakFonksiyonlar.ConvertToInt(ky?.saymanlikKod, -1);
            int birimKod = OrtakFonksiyonlar.ConvertToInt(ky?.birimKod, -1);
            if (saymanlikKod <= 0 || birimKod <= 0) return birim + " kodlu yapı çözümlenemedi!";

            string durum = "";
            TNS.MUH.GondermeTenkisForm gonForm = new TNS.MUH.GondermeTenkisForm();
            TNS.MUH.GondermeTenkisForm tenForm = new TNS.MUH.GondermeTenkisForm();
            gonForm.yil = yil;
            gonForm.belgeTur = TNS.MUH.ENUMBelgeTuru.ODENEKGONDERME;
            gonForm.kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
            gonForm.yaziTarih = new TNSDateTime(DateTime.Now);
            gonForm.istekYaziTarih = gonForm.yaziTarih;
            gonForm.islemTarih = gonForm.yaziTarih;
            gonForm.muhasebe = saymanlikKod;
            gonForm.birim = ky.birimKod;
            gonForm.talep = "";
            gonForm.formDurum = 1;
            gonForm.aciklama = aciklama;
            tenForm.yil = yil;
            tenForm.belgeTur = TNS.MUH.ENUMBelgeTuru.TENKIS;
            tenForm.kurum = gonForm.kurum;
            tenForm.yaziTarih = gonForm.yaziTarih;
            tenForm.istekYaziTarih = gonForm.yaziTarih;
            tenForm.islemTarih = gonForm.yaziTarih;
            tenForm.muhasebe = gonForm.muhasebe;
            tenForm.birim = gonForm.birim;
            tenForm.talep = "";
            tenForm.formDurum = 1;
            tenForm.aciklama = aciklama;
            string kimlik = gonForm.muhasebe + "-" + gonForm.birim;

            foreach (DictionaryEntry de in ht)
            {
                string tertipStr = (string)de.Key;
                double odenek = (double)de.Value;
                TNS.MUH.GondermeTenkisDetay detay = new TNS.MUH.GondermeTenkisDetay();
                detay.tertip = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
                detay.tertip.TertipAta(tertipStr);
                if (odenek == 0)
                {
                    durum = kimlik + $" için, {tertipStr} ödenek miktarı 0";
                    continue;
                }
                string ayKosul = odenek < 0 ? "0/13" : "0/" + (ay + 1);
                string uzaySorgu = TNS.UZY.Arac.UzayKosulOl(yil.ToString(), "5", tertipStr, birim, ayKosul, "5");
                double masrafPlani = uzyServis.UzayDegeriDbl(kullanan, "MPGMASRAFPLANITERTIP", uzaySorgu, false);
                uzaySorgu = TNS.UZY.Arac.UzayKosulOl(yil.ToString(), "", saymanlikKod.ToString(), ky.birimKod, "0/6", tertipStr);
                double gonderilen = uzyServis.UzayDegeriDbl(kullanan, "MUHGONDERMEBELGESITENKISBIRIM", uzaySorgu, false);
                durum = kimlik + $" için, öd:{odenek}, mp:{masrafPlani}, gönd:{gonderilen}";
                if (odenek > 0)
                {
                    if (masrafPlani > gonderilen)
                    {
                        detay.miktar = masrafPlani - gonderilen;
                        gonForm.detay.objeler.Add(detay);
                    }
                }
                else if (gonderilen > masrafPlani)
                {
                    detay.miktar = gonderilen - masrafPlani;
                    tenForm.detay.objeler.Add(detay);
                }
            }

            if (gonForm.detay.objeler.Count > 0)
            {
                Sonuc s = muhServis.GondermeTenkisKaydet(kullanan, gonForm, true, true, true);
                durum = s.islemSonuc ? kimlik + " için " + s.anahtar + " numaraları ödenek gönderme oluşturuldu"
                    : kimlik + " için ödenek gönderme oluşturulamadı (" + s.hataStr + ")";
            }
            if (tenForm.detay.objeler.Count > 0)
            {
                if (!string.IsNullOrEmpty(durum)) durum += "\n";
                Sonuc s = muhServis.GondermeTenkisKaydet(kullanan, tenForm, true, true, false);
                durum += s.islemSonuc ? kimlik + " için " + s.anahtar + " numaraları tenkis oluşturuldu"
                    : kimlik + " için tenkis oluşturulamadı (" + s.hataStr + ")";
            }
            if (gonForm.detay.objeler.Count == 0 && tenForm.detay.objeler.Count == 0)
                durum += kimlik + " için işlem yapılacak kayıt bulunamadı!";
            return durum;
        }

        // ------------------------------------------------------------------ dosya yükleme

        private static async Task<(IFormCollection form, string yol, string hata)> DosyaAl(HttpContext baglam)
        {
            IFormCollection form = await baglam.Request.ReadFormAsync();
            IFormFile dosya = form.Files["dosya"];
            if (dosya == null || dosya.Length == 0) return (form, null, "Yükleme için bir dosya seçiniz");
            if (dosya.Length > 20 * 1024 * 1024) return (form, null, "Dosya çok büyük (en çok 20 MB).");
            string uzanti = Path.GetExtension(dosya.FileName ?? "").ToLowerInvariant();
            if (uzanti != ".xls" && uzanti != ".xlsx") uzanti = "." + GenelIslemler.ExcelTur();
            string yol = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul() + uzanti;
            try
            {
                using (FileStream hedef = File.Create(yol))
                    await dosya.CopyToAsync(hedef);
            }
            catch (Exception)
            {
                return (form, null, string.Format("Dosya sunucuda saklanamadı ({0})", Path.GetFileName(yol)));
            }
            return (form, yol, null);
        }

        /// <summary>
        /// Yükle (eski btnDosyaYukle_Click): ilk satır başlıktır; "tertip" sütunu, "kurumsalyapı" ya da "saymanlık" + "birim" sütunları
        /// ve ay adlarıyla (Ocak ... Aralık) adlandırılmış sütunlar okunur. Saymanlık - birim verilmişse kurumsal yapı tertibin kurumsal
        /// koduyla bulunur (KurumsalYapiListele). Satırlar kaydedilmeden tabloya yazılır.
        /// </summary>
        private static async Task<IResult> Yukle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            var (form, yol, dHata) = await DosyaAl(baglam);
            if (dHata != null) return Hata(dHata);
            int yil = OrtakFonksiyonlar.ConvertToInt((string)form["yil"], 0);
            try
            {
                if (yil <= 0) return Hata("Yıl giriniz.");
                AsposeLisans.Yukle();
                IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                Tablo XLS = GenelIslemler.NewTablo();
                XLS.DosyaOkuAc(yol);
                int sat = 0, tertipSutun = -1, kurumsalYapiSutun = -1, saymanlikSutun = -1, birimSutun = -1;
                int[] aySutun = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
                bool ayVar = false;
                CultureInfo tr = CultureInfo.GetCultureInfo("tr-TR");
                for (int sutun = 0; ; sutun++)
                {
                    string baslik = (XLS.HucreDegerAl(sat, sutun) ?? "").Trim().ToLower(tr);
                    if (string.IsNullOrEmpty(baslik)) break;
                    if (baslik == "tertip" || baslik == "tertıp") tertipSutun = sutun;
                    else if (baslik == "kurumsalyapi" || baslik == "kurumsalyapı") kurumsalYapiSutun = sutun;
                    else if (baslik == "saymanlık" || baslik == "saymanlik") saymanlikSutun = sutun;
                    else if (baslik == "birim" || baslik == "bırım") birimSutun = sutun;
                    else
                        for (int ay = 0; ay < 12; ay++)
                            if (GenelIslemler.AyAdDondur(ay + 1).ToLower(tr).Equals(baslik))
                            {
                                ayVar = true;
                                aySutun[ay] = sutun;
                            }
                }
                string hataStr = "";
                if (tertipSutun < 0) hataStr = "Tertip sütunu belirtilmemiş!";
                if (!ayVar) hataStr += "Ay isimleri olan sütunlar bulunamadı!";
                if (kurumsalYapiSutun < 0 && (saymanlikSutun < 0 || birimSutun < 0))
                    hataStr += "Saymanlık-Birim sütunları veya KurumsalYapı sütunu belirtilmemiş";

                List<MPRevizeBilgi> satirlar = new List<MPRevizeBilgi>();
                if (hataStr == "")
                {
                    while (true)
                    {
                        sat++;
                        string tertipStr = (XLS.HucreDegerAl(sat, tertipSutun) ?? "").Trim();
                        if (string.IsNullOrEmpty(tertipStr)) break;
                        string kurumsalYapiKod = "";
                        if (kurumsalYapiSutun > -1)
                            kurumsalYapiKod = (XLS.HucreDegerAl(sat, kurumsalYapiSutun) ?? "").Trim();
                        else
                        {
                            string kurKod = tertipStr.Replace(".", "");
                            kurKod = kurKod.Length >= 8 ? kurKod.Substring(0, 8) : kurKod;
                            string[] kyDizi = servis.KurumsalYapiListele(XLS.HucreDegerAl(sat, saymanlikSutun), XLS.HucreDegerAl(sat, birimSutun), kurKod);
                            if (kyDizi != null && kyDizi.Length > 0) kurumsalYapiKod = kyDizi[0];
                        }
                        double[] aylik = new double[12];
                        for (int ay = 0; ay < 12; ay++)
                            if (aySutun[ay] >= 0)
                                aylik[ay] = Math.Round(XLS.HucreDegerAlDbl(0, sat, aySutun[ay]), 2);
                        satirlar.Add(BilgiYap(yil, tertipStr, kurumsalYapiKod, aylik));
                    }
                }
                XLS.DosyaKapat();
                if (hataStr != "") return Hata(hataStr);
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Dosya okunamadı."));
            }
            finally
            {
                try { File.Delete(yol); } catch { }
            }
        }

        /// <summary>
        /// İşlem Üret (eski btnDosyadanIslemYap_click): başlıksız Excel, 1. sütun tertip, sonra 12 ay. tur 1 Ödenek Bloke, 10 Ödenek Kes,
        /// 2 AHP Düzelt: her tertip için ButceIslemUret (en çok 5 paralel). tur 3 Tenkis: 2. sütun tenkis tutarı; tertibi harcanandan fazla
        /// ödenek gönderilmiş birimlere oranla paylaştırılan hızlı tenkis dosyası (Excel) indirilir.
        /// </summary>
        private static async Task<IResult> IslemUret(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            var (form, yol, dHata) = await DosyaAl(baglam);
            if (dHata != null) return Hata(dHata);
            int yil = OrtakFonksiyonlar.ConvertToInt((string)form["yil"], 0);
            int islemTur = OrtakFonksiyonlar.ConvertToInt((string)form["tur"], 0);
            try
            {
                if (yil <= 0) return Hata("Yıl giriniz.");
                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                XLS.DosyaOkuAc(yol);
                List<MPRevizeSatir> tc = new List<MPRevizeSatir>();
                for (int sat = 0; ; sat++)
                {
                    string tertipStr = (XLS.HucreDegerAl(sat, 0) ?? "").Trim();
                    if (string.IsNullOrEmpty(tertipStr)) break;
                    MPRevizeSatir islemBilgi = new MPRevizeSatir();
                    islemBilgi.tertip = new Tertip(yil, ENUMTabloKod.TABMasrafPlani);
                    islemBilgi.tertip.TertipAta(tertipStr);
                    for (int ay = 0; ay < 12; ay++)
                        islemBilgi.odenek[ay] = XLS.HucreDegerAlDbl(0, sat, ay + 1);
                    tc.Add(islemBilgi);
                }
                XLS.DosyaKapat();
                if (tc.Count == 0) return Hata("Dosyada tertip bulunamadı!");

                if (islemTur == 1 || islemTur == 10 || islemTur == 2)
                    return Results.Json(new { bilgi = ButceIslemUret(kullanan, yil, tc, islemTur), uyariVar = true });
                if (islemTur != 3) return Hata("İşlem tür henüz hazır değil");

                List<TNS.MUH.OdenekDurum> od = TenkisBelgesiUret(kullanan, yil, tc);
                Tablo XLSUret = GenelIslemler.NewTablo();
                string dosyaAd = DosyaIslem.DosyaAdUret("Tenkis", ".xlsx");
                XLSUret.BosDosyaAc(dosyaAd);
                int satIc = 0;
                if (od.Count > 0)
                    foreach (TNS.MUH.OdenekDurum item in od)
                    {
                        XLSUret.HucreDegerYaz(satIc, 0, item.muhasebe);
                        XLSUret.HucreDegerYaz(satIc, 1, item.birim);
                        XLSUret.HucreDegerYaz(satIc, 2, item.tertipButce.TertipStrOl());
                        XLSUret.HucreDegerYaz(satIc, 3, item.onayliTenkis);
                        satIc++;
                    }
                else
                    XLSUret.HucreDegerYaz(satIc, 0, "Bulunamadı");
                XLSUret.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(dosyaAd, Path.GetFileName(dosyaAd), true, "xlsx");
            }
            catch (Exception ex)
            {
                return Hata("İşlem döngüsünde hata:" + ex.Message);
            }
            finally
            {
                try { File.Delete(yol); } catch { }
            }
        }

        private static string ButceIslemUret(Kullanici kullanan, int yil, List<MPRevizeSatir> tc, int islemTur)
        {
            IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
            int basarili = 0;
            string[] hatalar = new string[tc.Count];
            Parallel.For(0, tc.Count, new ParallelOptions { MaxDegreeOfParallelism = 5 }, i =>
            {
                try
                {
                    Sonuc snc = servis.ButceIslemUret(kullanan, yil, tc[i], islemTur);
                    if (!snc.islemSonuc) hatalar[i] = tc[i].tertip.TertipStrOl() + " işlem başarısız: (" + snc.hataStr + ");";
                    else Interlocked.Increment(ref basarili);
                }
                catch (Exception ex)
                {
                    hatalar[i] = tc[i].tertip.TertipStrOl() + " " + ex.Message;
                }
            });
            string cvp = basarili > 0 ? basarili + " adet işlem üretildi!" : "";
            if (basarili < tc.Count)
            {
                cvp += " Hatalar:";
                foreach (string h in hatalar)
                    if (!string.IsNullOrEmpty(h)) cvp += "\n" + h;
            }
            return cvp.Trim();
        }

        private static double KurusAsagi(double d) { return Math.Floor(Math.Round(d * 100, 6)) / 100; }

        private static List<TNS.MUH.OdenekDurum> TenkisBelgesiUret(Kullanici kullanan, int yil, List<MPRevizeSatir> tc)
        {
            List<TNS.MUH.OdenekDurum> tenkisler = new List<TNS.MUH.OdenekDurum>();
            TNS.MUH.IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
            object kilit = new object();
            Parallel.ForEach(tc, new ParallelOptions { MaxDegreeOfParallelism = 5 }, islemSatir =>
            {
                TNS.MUH.OdenekDurumKriter kr = new TNS.MUH.OdenekDurumKriter();
                kr.yeniMetot = true;
                kr.ekoKodKirilim = 2;
                kr.eko23 = true;
                kr.yil = yil;
                kr.ay = yil < DateTime.Now.Year ? 12 : DateTime.Now.Month;
                kr.odenekGoster = false;
                kr.harcamaGoster = true;
                kr.odenekGondermeGoster = true;
                kr.detay = TNS.MUH.ENUMOdenekDurumDetay.BIRIM;
                kr.faaliyetGoster = true;
                kr.harcamaPebli = kr.faaliyetGoster;
                kr.durum = 5;
                kr.tertip = islemSatir.tertip;
                ObjectArray oVAN = servisMUH.HarcamaDurumListesi(kullanan, kr);
                if (!oVAN.sonuc.islemSonuc) throw new Exception(oVAN.sonuc.hataStr);
                List<TNS.MUH.OdenekDurum> ic = new List<TNS.MUH.OdenekDurum>();
                double toplam = 0;
                foreach (TNS.MUH.OdenekDurum item in oVAN.objeler)
                {
                    double tut = (item.onayliOdenekGonderme + item.onayliTenkis) - item.harcama;
                    if (tut > 0)
                    {
                        TNS.MUH.OdenekDurum sat = new TNS.MUH.OdenekDurum();
                        sat.tertipButce = item.tertipButce;
                        sat.muhasebe = item.muhasebe;
                        sat.birim = item.birim;
                        sat.onayliTenkis = tut;
                        toplam += tut;
                        ic.Add(sat);
                    }
                }
                // Eski TenkisHesapla: tenkis tutarı birimlere kalanlarıyla orantılı (kuruş aşağı yuvarlanarak) paylaştırılır.
                double tenkisYapilacak = Math.Abs(islemSatir.odenek[0]);
                if (toplam > tenkisYapilacak)
                    foreach (TNS.MUH.OdenekDurum item in ic)
                    {
                        double birimTutar = item.onayliTenkis;
                        double tenkisEdilebilir = 0;
                        if (birimTutar > 0)
                        {
                            tenkisEdilebilir = KurusAsagi(birimTutar * tenkisYapilacak / toplam);
                            tenkisYapilacak -= tenkisEdilebilir;
                            toplam -= birimTutar;
                        }
                        item.onayliTenkis = tenkisEdilebilir;
                    }
                lock (kilit) tenkisler.AddRange(ic);
            });
            return tenkisler;
        }

        // ------------------------------------------------------------------ Ödenek Devri (eski btnOdenekDevri_Click)

        /// <summary>
        /// Ödenek Devri: geçen yılın İÖİ işli (042134 fonksiyonu) onaylı masraf planından onaylı ödenek gönderilenler düşülür; kalanlar
        /// bu yılın tertiplerine Ocak ayına yazılır. Alt işlem 020113 seçilir ve Ekle/Düş Göster işaretlenir (sayfada).
        /// </summary>
        private static IResult OdenekDevri(HttpContext baglam, IWebHostEnvironment ortam, int yil)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil <= 0) return Hata("Yıl giriniz.");
            try
            {
                IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
                int onceki = yil - 1;
                string[] sqller = new string[2];
                sqller[0] = $@"SELECT  KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD, KURUMSALYAPIKOD, IOIISKOD, SUM(ODENEK)
  FROM MPGMASRAFPLANITERTIP
  where YIL={onceki} AND ASAMA=5 AND DURUM=5 AND IOIISKOD>0 AND FONKOD='042134'
  group by KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD, KURUMSALYAPIKOD, IOIISKOD
  order by KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD, KURUMSALYAPIKOD, IOIISKOD";
                sqller[1] = $@"SELECT MUHGONDERMETENKIS.KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD,  BHMKURUMSALYAPI.KOD, MUHGONDERMETENKISFORM.IOIISKOD, SUM(MUHGONDERMETENKIS.MIKTAR)
FROM  MUHGONDERMETENKISFORM INNER JOIN
             MUHGONDERMETENKIS ON MUHGONDERMETENKISFORM.YIL = MUHGONDERMETENKIS.YIL AND MUHGONDERMETENKISFORM.BELGETUR = MUHGONDERMETENKIS.BELGETUR AND
             MUHGONDERMETENKISFORM.BELGENO = MUHGONDERMETENKIS.BELGENO INNER JOIN
             BHMKURUMSALYAPI ON MUHGONDERMETENKIS.KURKOD = BHMKURUMSALYAPI.KURKOD AND CONVERT(VARCHAR(20), MUHGONDERMETENKISFORM.MUHASEBEKOD) = BHMKURUMSALYAPI.SAYMANLIKKOD AND
             MUHGONDERMETENKISFORM.BIRIMKOD = BHMKURUMSALYAPI.BIRIMKOD
where MUHGONDERMETENKIS.YIL={onceki} AND  MUHGONDERMETENKISFORM.FORMDURUM=5 AND MUHGONDERMETENKISFORM.IOIISKOD>0 AND FONKOD='042134'
GROUP BY MUHGONDERMETENKIS.KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD,  BHMKURUMSALYAPI.KOD, MUHGONDERMETENKISFORM.IOIISKOD
ORDER BY MUHGONDERMETENKIS.KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD,  BHMKURUMSALYAPI.KOD, MUHGONDERMETENKISFORM.IOIISKOD";
                string[] baglantilar = { "BaglantiSatiriBUM", "BaglantiSatiriBUM" };
                SortedList sl = uzyServis.SorguBirlestir(sqller, baglantilar, new int[] { 0, 1, 2, 3, 4, 5, 6 });

                List<MPRevizeBilgi> satirlar = new List<MPRevizeBilgi>();
                if (sl != null)
                    foreach (DictionaryEntry de in sl)
                    {
                        HTArray alAnahtar = (HTArray)de.Key;
                        HTArray alVeri = (HTArray)de.Value;
                        MPRevizeSatir s = new MPRevizeSatir();
                        s.tertip = new Tertip(yil, ENUMTabloKod.TABMasrafPlani, alAnahtar.StrAl(0), alAnahtar.StrAl(1), alAnahtar.StrAl(2),
                            alAnahtar.StrAl(3), alAnahtar.StrAl(4), alAnahtar.StrAl(5));
                        s.IOIISKOD = alAnahtar.IntAl(6);
                        s.odenek[0] = alVeri.DblAl(0) - alVeri.DblAl(1);
                        if (Math.Abs(s.odenek[0]) < 0.005) continue;
                        satirlar.Add(BilgiYap(s));
                    }
                return Results.Json(new { satirlar = satirlar, altIslem = "020113" });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Ödenek devri okunamadı."));
            }
        }

        // ------------------------------------------------------------------ yardımcı uçlar

        /// <summary>Eski PEBBilgiYaz: tertip girilince alt proje no, sektör, bölgeler ve program tertibi.</summary>
        private static IResult PebBilgi(HttpContext baglam, IWebHostEnvironment ortam, int yil, string tertip)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            tertip = Temiz(tertip);
            try
            {
                PEBProjeBilgi tut = TNS.BHMv2.Arac.Tanimla().PEBBilgiBul(kullanan, yil, TNS.OrtakFonksiyonlar.DiziElemanAl(tertip.Split('-'), 4, ""));
                string sektor = tut?.sektorAd ?? "", bolge = tut?.bolgeler ?? "";
                if (sektor.IndexOf("HATA:") > -1) return Hata(sektor);
                if (bolge.IndexOf("HATA:") > -1) return Hata(bolge);
                return Results.Json(new
                {
                    altProjeNo = tut?.DPTPROJEALTPROJENO ?? "",
                    sektorAd = sektor,
                    bolgeler = bolge,
                    tertipPrg = TNS.BHMv2.Arac.ProgramTertipOl(yil, tertip, false)
                });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Tertip bilgisi okunamadı."));
            }
        }

        /// <summary>
        /// Tertip seçimi (eski ButceHazirlikv2/ListeTertip.aspx?yil&amp;kur): ödenek - gönderilen ve masraf planı; kurumsal, finans, ekonomik ve alt faaliyetten en az ikisi dolu olmalıdır.
        /// Ödenek Gönderme'nin tertip listesiyle aynı iş kuralları (OdenekGondermeUclari) kullanılır; muhasebe - birim verilmez.
        /// </summary>
        private static IResult Tertipler(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string kur, string fin, string eko, string peb, string hepsi, int? sayi, string ilk)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                int iyil = yil ?? DateTime.Now.Year;
                if (iyil > DateTime.Now.Year) iyil = DateTime.Now.Year;
                kur = Temiz(kur).Replace(";", ",");
                fin = Temiz(fin).Replace(";", ",");
                eko = Temiz(eko).Replace(";", ",");
                peb = Temiz(peb).Replace(";", ",");
                bool tumu = hepsi == "1";
                int maxTertipSay = sayi ?? 30;
                if (maxTertipSay <= 0) maxTertipSay = 30;
                int doluAlan = (kur != "" ? 1 : 0) + (fin != "" ? 1 : 0) + (eko != "" ? 1 : 0) + (peb != "" ? 1 : 0);
                if (doluAlan < 2)
                    return Results.Json(new { uyari = ilk == "1" ? "" : "Lütfen kriter bölümünde, birden fazla alana değer giriniz.", satirlar = new List<object>(), sayi = maxTertipSay });

                TNS.MUH.ENUMOdenekKontrolTur okt = TNS.MUH.Arac.OdenekKontrolTurBul(iyil);
                IBHMv2Servis servisBHMv2 = TNS.BHMv2.Arac.Tanimla();
                OdenekBilgi[] obler = OdenekGondermeUclari.BhmTertipler(kullanan, servisBHMv2, iyil, okt, kur, eko, peb, tumu, out bool mpOnayli, out string odenekBaslik, fin);
                OdenekBilgi[] birimTertipleri = OdenekGondermeUclari.BhmBirimTertipler(kullanan, servisBHMv2, iyil, (int)TNS.MUH.ENUMBelgeTuru.ODENEKGONDERME, okt, 0, "", kur, eko, peb, tumu);
                string uyari = "";
                int satSay = OdenekGondermeUclari.TertipSayiKontrol(obler.Length, ref maxTertipSay, ref uyari);
                List<object> liste = new List<object>();
                foreach (OdenekBilgi ob in obler)
                {
                    if (satSay != 0 && liste.Count >= satSay) break;
                    string tertip = ob.tertip.TertipStrOl();
                    string[] kodlar = tertip.Split('-');
                    string pebKod = TNS.MUH.Arac.DiziElemanAl(kodlar, 4, "");
                    double odenek = Convert.ToDouble(ob.ekRakamlar[0]);
                    double gonderilen = Convert.ToDouble(ob.ekRakamlar[1]);
                    double mp = 0;
                    OdenekBilgi bob = OdenekGondermeUclari.TertipBul(birimTertipleri, tertip);
                    if (bob != null && mpOnayli) mp = Convert.ToDouble(bob.ekRakamlar[3]);
                    liste.Add(new
                    {
                        TERTIP = TNS.MUH.Arac.DiziElemanAl(kodlar, 0, "") + "-" + TNS.MUH.Arac.DiziElemanAl(kodlar, 1, "") + "-" + TNS.MUH.Arac.DiziElemanAl(kodlar, 2, "")
                            + "-" + TNS.MUH.Arac.DiziElemanAl(kodlar, 3, "") + (pebKod != "" ? "-" + pebKod : ""),
                        KALAN = odenek - gonderilen,
                        ODENEK = odenek,
                        GONDERILEN = gonderilen,
                        MP = mp,
                        ACIKLAMA = ob.tertip.adlar.Length > 3 ? ob.tertip.adlar[3] : "",
                        FAALIYETACIKLAMA = ob.tertip.adlar.Length >= 5 ? ob.tertip.adlar[4] : ""
                    });
                }
                return Results.Json(new { mpOnayli = mpOnayli, odenekBaslik = odenekBaslik, uyari = uyari, satirlar = liste, sayi = maxTertipSay });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Tertipler okunamadı."));
            }
        }

        /// <summary>
        /// İş seçimi (eski IOIProjeSecim.aspx?yil&amp;kurumsalYapiKod): saymanlık ve birim kurumsal yapı kodundan (noktasız 2-6 ve 8-10.
        /// haneler) ya da verilen muhasebe - birimden; isTur 1 İÖİ, 2 YİKOB, 0 hepsi.
        /// </summary>
        private static IResult Ioi(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string ky, string muhasebe, string birim, string ad, int? isTur, string ilk)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                string kyKod = Temiz(ky).Replace(".", "");
                string sayKod = Temiz(muhasebe), birKod = Temiz(birim);
                if (ilk == "1" && kyKod.Length > 9)
                {
                    sayKod = kyKod.Substring(1, 5);
                    birKod = kyKod.Substring(7, 3);
                }
                string muhAd = sayKod == "" ? "" : GenelIslemler.KodAd(11, sayKod, true);
                string birAd = sayKod == "" || birKod == "" ? "" : GenelIslemler.KodAd(12, birKod + "-" + sayKod, false);
                List<object> satirlar = new List<object>();
                if (ilk != "1")
                {
                    IOIProje p = new IOIProje();
                    p.ioiProjeAdi = Temiz(ad);
                    p.muhasebeKod = OrtakFonksiyonlar.ConvertToInt(sayKod, 0);
                    p.birimKod = birKod;
                    p.isTur = isTur ?? 0;
                    ObjectArray oa = TNS.BHMv2.Arac.Tanimla().ListeleIOIProjeGiris(kullanan, p, false, yil ?? DateTime.Now.Year);
                    if (!oa.sonuc.islemSonuc) return Hata(HataMetni(oa.sonuc.hataStr, "İşler okunamadı."));
                    foreach (IOIProje i in oa.objeler)
                        satirlar.Add(new
                        {
                            siraNo = i.siraNo,
                            ioiProjeAdi = i.ioiProjeAdi,
                            ilAdi = i.ilAdi,
                            aktarilanTutar = i.aktarilanTutar,
                            islemleEklenen = i.islemleEklenen,
                            iOIHarc = i.iOIHarc,
                            gonderilen = i.gonderilen
                        });
                }
                return Results.Json(new { muhasebe = sayKod, birim = birKod, muhasebeAd = muhAd, birimAd = birAd, satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "İşler okunamadı."));
            }
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

        // ------------------------------------------------------------------ Tertip Bilgi (eski MasrafPlaniTertipBilgi.aspx)

        private static IResult TertipBilgiTanim(HttpContext baglam, IWebHostEnvironment ortam, int? yil)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            int y = yil ?? DateTime.Now.Year;
            List<int> yillar = new List<int>();
            for (int i = y - 5; i <= DateTime.Now.Year + 1; i++) yillar.Add(i);
            try
            {
                return Results.Json(new { yil = y, yillar = yillar, birimler = Birimler(kullanan, y, true) });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Birimler okunamadı."));
            }
        }

        /// <summary>
        /// Listele (eski Listele): 12 ay için onaylı (aşama 5) masraf planı: seçilen birim (kurumsal yapı verilmişse), dağıtılmayan (merkez 9)
        /// ve dağıtılan (0/8). Toplam satırı sayfada hesaplanır.
        /// </summary>
        private static IResult TertipBilgi(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur, string fon, string fin, string eko, string peb, string ky)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil <= 0) return Hata("Yıl giriniz.");
            try
            {
                IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
                string y = yil.ToString();
                kur = Temiz(kur); fon = Temiz(fon); fin = Temiz(fin); eko = Temiz(eko); peb = Temiz(peb); ky = Temiz(ky);
                List<object> satirlar = new List<object>();
                for (int ay = 1; ay <= 12; ay++)
                {
                    double d1 = ky == "" ? 0 : servisUZY.UzayDegeriDbl(kullanan, "MPGMASRAFPLANIBIRIM", TNS.UZY.Arac.UzayKosulOl(y, "5", "", "", kur, fon, fin, eko, peb, ky, ay.ToString(), ""), false);
                    double d2 = servisUZY.UzayDegeriDbl(kullanan, "MPGMASRAFPLANIBIRIM", TNS.UZY.Arac.UzayKosulOl(y, "5", "", "", kur, fon, fin, eko, peb, "9", ay.ToString(), ""), false);
                    double d3 = servisUZY.UzayDegeriDbl(kullanan, "MPGMASRAFPLANIBIRIM", TNS.UZY.Arac.UzayKosulOl(y, "5", "", "", kur, fon, fin, eko, peb, "0/8", ay.ToString(), ""), false);
                    satirlar.Add(new { ay = ay + ". " + GenelIslemler.AyAdDondur(ay), birim = d1, dagitilmayan = d2, dagitilan = d3 });
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Listeleme yapılamadı."));
            }
        }

        /// <summary>
        /// Detay Yaz (eski FormUret): tur 1 "Tertip Detay" (MPBirimListele: tertibin birimleri), tur 2 "Birim Detay" (MPTertipListele:
        /// harcama biriminin tertipleri). Excel: tertip, birim, 12 ay, MP toplam, MP dönem (serbest aya kadar), gönderilen, fark.
        /// </summary>
        private static IResult TertipBilgiDetay(HttpContext baglam, IWebHostEnvironment ortam, int tur, int yil, string kur, string fon, string fin, string eko, string peb, string ky)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil <= 0) return Hata("Yıl giriniz.");
            ky = Temiz(ky);
            if (tur == 2 && ky == "") return Hata("Birim (harcama birimi) giriniz.");
            try
            {
                IBHMv2Servis servisBHMv2 = TNS.BHMv2.Arac.Tanimla();
                Tertip kistas = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
                kistas.TertipAta(Temiz(kur), Temiz(fon), Temiz(fin), Temiz(eko), Temiz(peb));
                TertipOdenek[] to = (tur == 1 ? servisBHMv2.MPBirimListele(kullanan, kistas) : servisBHMv2.MPTertipListele(kullanan, ky, kistas)) ?? new TertipOdenek[0];

                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                string dosyaAdxls = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul() + "." + XLS.UzantiBul();
                XLS.BosDosyaAc(dosyaAdxls);
                int sat = 0;
                XLS.HucreDegerYaz(sat, 0, "Tertip");
                XLS.SutunGenislikAyarla(0, 0, GenelIslemler.JexcelBirimtoExcelBirimCol(38));
                XLS.HucreDegerYaz(sat, 1, "Birim");
                XLS.SutunGenislikAyarla(1, 1, GenelIslemler.JexcelBirimtoExcelBirimCol(14));
                for (int ay = 0; ay < 12; ay++)
                    XLS.HucreDegerYaz(sat, 2 + ay, GenelIslemler.AyAdDondur(ay + 1));
                XLS.HucreDegerYaz(sat, 14, "MP Toplam");
                XLS.HucreDegerYaz(sat, 15, "MP Dönem");
                XLS.HucreDegerYaz(sat, 16, "Gönderilen");
                XLS.HucreDegerYaz(sat, 17, "Fark");
                sat++;
                int maxAy = TNS.BHMv2.Arac.SerbestAyBul(yil);
                foreach (TertipOdenek t in to)
                {
                    if (tur == 1)
                    {
                        XLS.HucreDegerYaz(sat, 0, kistas.TertipStrOl());
                        XLS.HucreDegerYaz(sat, 1, t.tertip.kodlar[0]);
                    }
                    else
                    {
                        XLS.HucreDegerYaz(sat, 0, t.tertip.TertipStrOl());
                        XLS.HucreDegerYaz(sat, 1, ky);
                    }
                    double satirToplam = 0, donemToplam = 0;
                    for (int ay = 0; ay < 12; ay++)
                    {
                        double o = t.odenek[ay];
                        satirToplam += o;
                        if (ay < maxAy) donemToplam += o;
                        XLS.HucreDegerYaz(sat, 2 + ay, o);
                    }
                    XLS.HucreDegerYaz(sat, 14, satirToplam);
                    XLS.HucreDegerYaz(sat, 15, donemToplam);
                    XLS.HucreDegerYaz(sat, 16, t.odenek[12]);
                    XLS.HucreDegerYaz(sat, 17, donemToplam - t.odenek[12]);
                    sat++;
                }
                XLS.DosyaSaklaTamYol();
                string uzanti = Path.GetExtension(dosyaAdxls).TrimStart('.');
                return DosyaIslem.DosyaGonder(dosyaAdxls, "MPTertipBilgi" + (tur == 1 ? "Tertip" : "Birim") + "_" + DateTime.Now.Ticks + "." + uzanti, true, uzanti);
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Dosya hazırlanamadı."));
            }
        }

        // ------------------------------------------------------------------ Belge Arama (eski MasrafPlaniRevizeSorgu.aspx)

        private static IResult SorguTanim(HttpContext baglam, IWebHostEnvironment ortam, int? yil)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            int y = yil ?? DateTime.Now.Year;
            try
            {
                List<object> durumlar = new List<object>
                {
                    new { kod = "1;6", ad = "İşlemdekiler" },
                    new { kod = "6", ad = "Müsvetteler" },
                    new { kod = "1", ad = "Taslaklar" },
                    new { kod = "5", ad = "Onaylılar" },
                    new { kod = "8;9", ad = "İptal Edilenler" },
                    new { kod = "1;5;6", ad = "Bütün Belgeler (İptaller Hariç)" }
                };
                List<object> turler = new List<object>();
                foreach (ENUMRevizeFormTip t in new[] { ENUMRevizeFormTip.MasrafPlaniRevize, ENUMRevizeFormTip.MasrafPlaniAktarma, ENUMRevizeFormTip.AHPRevize,
                    ENUMRevizeFormTip.Ekleme, ENUMRevizeFormTip.Aktarma, ENUMRevizeFormTip.Hepsi, ENUMRevizeFormTip.AktarmaEklemeler,
                    ENUMRevizeFormTip.MasrafPlaniIslemleri, ENUMRevizeFormTip.BumkoIslemleri })
                    turler.Add(KodAd((int)t, TNS.BKD.Arac.RevizeFormTipAl(t)));
                return Results.Json(new { yil = y, durumlar = durumlar, islemTurleri = turler, birimler = Birimler(kullanan, y, true), yonetici = Yonetici(kullanan) });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Sayfa bilgileri okunamadı."));
            }
        }

        private static string Aralik(string a, string b)
        {
            a = Temiz(a);
            b = Temiz(b);
            return a == "" && b == "" ? null : a + ";" + b;
        }

        /// <summary>yyyy-MM-dd -> eski tarih kutusunun metni (gg.aa.yyyy).</summary>
        private static string TarihMetin(string s)
        {
            s = Temiz(s);
            if (s != "" && DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime t))
                return t.ToString("dd.MM.yyyy");
            return "";
        }

        /// <summary>Eski KistasAl: yıl (2012 ve sonrası) ve birim zorunlu.</summary>
        private static MPRevizeFormSorgu KistasAl(HttpRequest r, out string hata)
        {
            int yil = OrtakFonksiyonlar.ConvertToInt((string)r.Query["yil"], 0);
            string birim = Temiz(r.Query["birim"]);
            hata = "";
            if (yil < 2012) hata = "Sorgulama için yıl alanını boş bırakamazsınız.";
            if (birim == "") hata += (hata == "" ? "" : " ") + "Sorgulama için Birim alanını boş bırakamazsınız.";
            if (hata != "") return null;
            MPRevizeFormSorgu f = new MPRevizeFormSorgu();
            f.yil = yil;
            f.formDurumAralik = Temiz(r.Query["durum"]);
            f.revizeFormTip = (ENUMRevizeFormTip)OrtakFonksiyonlar.ConvertToInt((string)r.Query["tur"], 7);
            f.revizeNoAralik = Aralik(r.Query["no1"], r.Query["no2"]);
            f.kaydedenKullaniciKodu = Temiz(r.Query["islemYapan"]);
            string t1 = TarihMetin(r.Query["tarih1"]), t2 = TarihMetin(r.Query["tarih2"]);
            if (t1 != "" || t2 != "") f.kayitTarihiAralik = t1 + ";" + t2;
            string d1 = TarihMetin(r.Query["durumTarih1"]), d2 = TarihMetin(r.Query["durumTarih2"]);
            if (d1 != "" || d2 != "") f.onaylamaTarihiAralik = d1 + ";" + d2;
            f.tertip = new Tertip(yil, ENUMTabloKod.TABMasrafPlani);
            f.tertip.kodlar[0] = birim;
            f.tertip.kodlar[1] = Temiz(r.Query["fon"]);
            f.tertip.kodlar[2] = Temiz(r.Query["fin"]);
            f.tertip.kodlar[3] = Temiz(r.Query["eko"]);
            f.tertip.kodlar[4] = Temiz(r.Query["peb"]);
            f.tertip.kodlar[5] = Temiz(r.Query["ky"]);
            f.detayGoster = false;
            return f;
        }

        /// <summary>Listele (eski btnListe_Click): masraf planı onaylıysa MPRevizeFormListele, değilse BUM bütçe işlemleri (BIListele).</summary>
        private static IResult SorguListe(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            MPRevizeFormSorgu f = KistasAl(baglam.Request, out string hata);
            if (f == null) return Hata(hata);
            try
            {
                List<object> satirlar = new List<object>();
                bool mpOnayli = MPOnayli(kullanan, f.yil);
                if (mpOnayli)
                {
                    MPRevizeForm[] fler = TNS.BHMv2.Arac.Tanimla().MPRevizeFormListele(kullanan, f) ?? new MPRevizeForm[0];
                    foreach (MPRevizeForm m in fler)
                        satirlar.Add(new
                        {
                            revizeNo = m.revizeNo,
                            odenekToplam = m.odenekToplam,
                            durumAd = m.durumAd,
                            evrakTarih = TarihYaz(m.evrakTarihDT),
                            kaydedenKullaniciKodu = m.kaydedenKullaniciKodu ?? "",
                            onayTarih = TarihYaz(m.onayTarihDT),
                            onaylayanKullaniciKodu = m.onaylayanKullaniciKodu ?? "",
                            formTipAd = m.formTipAd,
                            aciklama = m.aciklama ?? "",
                            BUMKONo = string.IsNullOrEmpty(m.bumkoIslemNo) ? "" : m.BUMKONo
                        });
                }
                else
                {
                    ObjectArray oa = TNS.BUM.Arac.Tanimla().BIListele(kullanan, new BIForm(f.yil, f.tertip.kodlar[0], "", 0), true);
                    if (oa == null || !oa.sonuc.islemSonuc) return Hata(HataMetni(oa?.sonuc?.hataStr, "Listeleme yapılamadı."));
                    foreach (BIForm b in oa.objeler)
                        satirlar.Add(new
                        {
                            revizeNo = b.siraNo,
                            odenekToplam = b.miktar,
                            durumAd = b.formDurum?.durumAd ?? "",
                            evrakTarih = TarihYaz(b.evrakTarih),
                            kaydedenKullaniciKodu = b.kullaniciKod ?? "",
                            onayTarih = TarihYaz(b.onayTarih),
                            onaylayanKullaniciKodu = "",
                            formTipAd = b.islemKod ?? "",
                            aciklama = b.notlar ?? "",
                            BUMKONo = b.bumkoSiraNo > 0 ? b.bumkoKurum + "-" + b.bumkoSiraNo : ""
                        });
                }
                return Results.Json(new { yil = f.yil, mpOnayli = mpOnayli, satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Listeleme yapılamadı."));
            }
        }

        /// <summary>İptal Et (9) / Onayla (5) / Yeni Yap (1, taslak) (eski btnDurumDegistir_Click; sistem yöneticisi, masraf planı onaylı yıllar).</summary>
        private static async Task<IResult> SorguDurum(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, MPRevizeDurumIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!Yonetici(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            istek ??= new MPRevizeDurumIstegi();
            if (istek.Nolar == null || istek.Nolar.Count == 0) return Hata("Belge seçiniz.");
            ENUMRevizeFormDurum durum;
            if (istek.Durum == 9) durum = ENUMRevizeFormDurum.Iptal;
            else if (istek.Durum == 5) durum = ENUMRevizeFormDurum.Onayli;
            else if (istek.Durum == 1) durum = ENUMRevizeFormDurum.Taslak;
            else return Hata("Durum hatalı.");
            try
            {
                IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                bool mpOnayli = MPOnayli(kullanan, istek.Yil);
                string hataStr = "";
                foreach (int no in istek.Nolar)
                {
                    Sonuc s = mpOnayli ? servis.MPRevizeFormDurumAta(kullanan, istek.Yil, no, durum) : new Sonuc(false, "Hazır değil", "");
                    if (!s.islemSonuc) hataStr += (hataStr == "" ? "" : "\n") + no + ": " + s.hataStr;
                }
                if (hataStr == "") return Bilgi("İşlem başarıyla tamamlandı!");
                return Results.Json(new { hata = "İşlem Tamamlanamadı!\n" + hataStr });
            }
            catch (Exception ex)
            {
                return Hata("İşlem Tamamlanamadı!" + ex.Message);
            }
        }

        /// <summary>
        /// Raporlar (eski btnRapor_Click; Rapor.xml 2003): rapor 1 "Tertip durum" (2003-12, birim ve kodlar), 2 "İşlem Tertip Toplam" ve
        /// 3 "İşlem Detay Liste" (2003-13, aramadaki kıstaslarla; 3'te detay).
        /// </summary>
        private static IResult SorguRapor(HttpContext baglam, IWebHostEnvironment ortam, int rapor)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            HttpRequest r = baglam.Request;
            int yil = OrtakFonksiyonlar.ConvertToInt((string)r.Query["yil"], 0);
            if (yil <= 0) return Hata("Yıl giriniz.");
            try
            {
                IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                Hashtable raporParam = new Hashtable();
                raporParam["ButceYil"] = yil.ToString();
                raporParam["Senaryo"] = "0600";
                raporParam["SenaryoAd"] = servis.ButceSenaryoAdBul(kullanan.aktifKultur, "0600");
                raporParam["KurKod"] = TNS.DEG.Arac.VarsayilanKurumBul();
                raporParam["KurAd"] = "";
                int raporNo = 12;
                if (rapor == 1)
                {
                    raporParam["kur"] = Temiz(r.Query["birim"]);
                    raporParam["fon"] = Temiz(r.Query["fon"]);
                    raporParam["fin"] = Temiz(r.Query["fin"]);
                    raporParam["eko"] = Temiz(r.Query["eko"]);
                    raporParam["peb"] = Temiz(r.Query["peb"]);
                }
                else
                {
                    raporNo = 13;
                    MPRevizeFormSorgu f = KistasAl(r, out string hata);
                    if (f == null) return Hata(hata);
                    f.detayGoster = rapor == 3;
                    raporParam["form"] = f;
                }
                AsposeLisans.Yukle();
                string dosya = RaporlamaServis.TekRaporAl(2003, raporNo, kullanan, servis, raporParam, null);
                if (RaporlamaServis.MesajMi(dosya)) return Hata(dosya);
                string uzanti = Path.GetExtension(dosya).TrimStart('.').ToLowerInvariant();
                return DosyaIslem.DosyaGonder(dosya, "ButceIslem_2003_" + raporNo + "_" + DateTime.Now.Ticks + "." + uzanti, true, uzanti);
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }
    }
}
