using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using ButceHazirlikv2;
using Istemci.OrtakSayfa;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.YAT;

namespace Istemci.ButceHazirlik
{
    /// <summary>Kredi / hibe planı satırı (eski grdKrediPlan).</summary>
    public class YatKrediPlanSatiri
    {
        public int Yil { get; set; }
        public double AnlasmaTutari { get; set; }
        public double AnlasmaTutariIc { get; set; }
        public double KullanilanTutar { get; set; }
        public double KullanilanTutarIc { get; set; }
        public double KullanilacakTutar { get; set; }
        public double KullanilacakTutarIc { get; set; }
    }

    /// <summary>Kredi geri ödeme satırı (eski grdKrediOdeme).</summary>
    public class YatKrediOdemeSatiri
    {
        public int Yil { get; set; }
        public double AnaParaGeriOdeme { get; set; }
        public double OdenecekFaiz { get; set; }
        public string ProjeninSonDurumu { get; set; }
        public string DarBogazCozum { get; set; }
    }

    /// <summary>Proje teklif rakamları satırı (eski grdYeniTeklif).</summary>
    public class YatYeniTeklifSatiri
    {
        public string EkoKod { get; set; }
        public string EkoAd { get; set; }
        public double ProjeTutariDis { get; set; }
        public double ProjeTutariIc { get; set; }
        public double DisKredi0 { get; set; }
        public double IcKaynak0 { get; set; }
        public double DisKredi1 { get; set; }
        public double IcKaynak1 { get; set; }
        public double DisKredi2 { get; set; }
        public double IcKaynak2 { get; set; }
    }

    /// <summary>Alt faaliyet satırı (eski grdAltFaaliyet).</summary>
    public class YatAltFaaliyetSatiri
    {
        public string PebKod { get; set; }
        public string PebAd { get; set; }
    }

    public class YatKaynakKaydetIstegi
    {
        public int Yil { get; set; }
        public string SenaryoKod { get; set; }
        public int AltProjeKodu { get; set; }
        public int KaynakNo { get; set; }
        public int KaynakTur { get; set; }
        public string KaynakAd { get; set; }
        public int ToplamaDahil { get; set; }
        public bool DptRapor { get; set; }
        public bool Yikob { get; set; }
        public int PbKod { get; set; }
        public string Dipnot { get; set; }
        public string KrediyiVeren { get; set; }
        /// <summary>yyyy-MM-dd ya da boş</summary>
        public string BaslamaTarih { get; set; }
        public string YururlukTarih { get; set; }
        public string Tur { get; set; }
        public string Tutar { get; set; }
        public string Vade { get; set; }
        public string OdemesizDonem { get; set; }
        public string FaizOran { get; set; }
        public string IcParaKrediOran { get; set; }
        /// <summary>Teklif tablosunun satırları: butceYil ve teklif alanları (alan adı -> değer).</summary>
        public List<Dictionary<string, double>> Teklif { get; set; }
        /// <summary>Harcama tablosunun satırları: butceYil ve harcama alanları.</summary>
        public List<Dictionary<string, double>> Harcama { get; set; }
        public List<YatYeniTeklifSatiri> YeniTeklif { get; set; }
        public List<YatAltFaaliyetSatiri> AltFaaliyet { get; set; }
        public List<YatKrediPlanSatiri> KrediDetay { get; set; }
        public List<YatKrediOdemeSatiri> KrediOdeme { get; set; }
    }

    public class YatKaynakIstegi
    {
        public int Yil { get; set; }
        public string SenaryoKod { get; set; }
        public int AltProjeKodu { get; set; }
        public int KaynakNo { get; set; }
        /// <summary>Taşımada hedef alt proje.</summary>
        public int HedefAltProje { get; set; }
    }

    /// <summary>
    /// Alt proje bütçe kaynağı giriş sayfasının (eski ButceHazirlikv2/AltProjeButceKaynakGiris.aspx; AltProjeDetay'ın kaynak
    /// sekmelerinde çerçeve olarak açılır) JavaScript'ten çağırdığı uçlar:
    ///   GET  /api/yatkaynak/oku?yil=&amp;senaryoKod=&amp;kaynakNo=  kaynak bilgileri ve tabloları (eski ButceKaynakListele)
    ///   GET  /api/yatkaynak/parabirimleri, /ekokodlar?yil=&amp;ust= (ekonomik kod ağacının bir düzeyi), /altprojeler?yil=&amp;senaryoKod=
    ///   GET  /api/yatkaynak/yazdir, /izleme (raporlar 2004-12 ve 2002-15)
    ///   POST /api/yatkaynak/kaydet, /sil, /tasi
    /// İşlemler eskisi gibi TNS.YAT.Arac.Tanimla() üzerinden yapılır; yetki üst sayfa ProjeGiris'ten gelir.
    /// </summary>
    public static class AltProjeButceKaynakUclari
    {
        private const string SayfaAd = "AltProjeButceKaynakGiris";

        // Teklif ve harcama tablolarının alanları (ButceKaynakTeklif özellikleri; eski stoTeklif / stoHarcama alanları).
        private static readonly string[] TeklifAlanlari =
        {
            "ProjeTutarDis", "ProjeTutarToplam",
            "Odenek0Dis", "Odenek0DisaGiden", "Odenek0Toplam",
            "Odenek1Dis", "Odenek1DisaGiden", "Odenek1Toplam",
            "Odenek2Dis", "Odenek2DisaGiden", "Odenek2Toplam",
            "OdenekSDis", "OdenekSDisaGiden", "OdenekSToplam"
        };

        private static readonly string[] HarcamaAlanlari =
        {
            "KumulatifHarcamaDis", "KumulatifHarcamaDisaGiden", "KumulatifHarcamaToplam",
            "Harcama6ayDis", "Harcama6ayDisaGiden", "Harcama6ayToplam",
            "HarcamaYilSonuDis", "HarcamaYilSonuDisaGiden", "HarcamaYilSonuToplam"
        };

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/yatkaynak/oku", Oku);
            app.MapGet("/api/yatkaynak/parabirimleri", ParaBirimleri);
            app.MapGet("/api/yatkaynak/ekokodlar", EkoKodlar);
            app.MapGet("/api/yatkaynak/altprojeler", AltProjeler);
            app.MapGet("/api/yatkaynak/yazdir", Yazdir);
            app.MapGet("/api/yatkaynak/izleme", IzlemeYazdir);
            app.MapPost("/api/yatkaynak/kaydet", Kaydet);
            app.MapPost("/api/yatkaynak/sil", Sil);
            app.MapPost("/api/yatkaynak/tasi", Tasi);
        }

        private static IResult Hata(string hata)
        {
            return YatProjeOrtak.Hata(hata);
        }

        private static string Temiz(string deger)
        {
            return YatProjeOrtak.Temiz(deger);
        }

        private static string IlkKucuk(string ad)
        {
            return char.ToLowerInvariant(ad[0]) + ad.Substring(1);
        }

        /// <summary>Teklif nesnesinin verilen alanlarını { butceYil, alan... } sözlüğüne çevirir (alan adları küçük harfle başlar).</summary>
        private static Dictionary<string, object> TeklifSatiri(ButceKaynakTeklif t, string[] alanlar)
        {
            Dictionary<string, object> satir = new Dictionary<string, object>();
            satir["butceYil"] = t.butceYil;
            foreach (string ad in alanlar)
                satir[IlkKucuk(ad)] = typeof(ButceKaynakTeklif).GetProperty(ad).GetValue(t);
            return satir;
        }

        /// <summary>Tablodan gelen satırı teklif nesnesine çevirir; yalnızca o tablonun alanları dolar (eski JSON.Deserialize gibi).</summary>
        private static ButceKaynakTeklif TeklifOl(Dictionary<string, double> satir, string[] alanlar)
        {
            ButceKaynakTeklif t = new ButceKaynakTeklif();
            if (satir.TryGetValue("butceYil", out double yil)) t.butceYil = (int)yil;
            foreach (string ad in alanlar)
                if (satir.TryGetValue(IlkKucuk(ad), out double deger))
                    typeof(ButceKaynakTeklif).GetProperty(ad).SetValue(t, deger);
            return t;
        }

        private static ButceKaynakTeklif YilBul(List<ButceKaynakTeklif> liste, int butceYil)
        {
            return liste.Find(x => x.butceYil == butceYil);
        }

        private static bool ButceYiliVarMi(TNSCollection teklifler, int yil)
        {
            if (teklifler == null) return false;
            foreach (ButceKaynakTeklif bkt in teklifler)
                if (bkt.butceYil == yil) return true;
            return false;
        }

        private static string TarihYaz(TNSDateTime t)
        {
            if (TNSDateTime.IsNull(t) || t.Oku() == DateTime.MinValue) return "";
            return t.Oku().ToString("yyyy-MM-dd");
        }

        private static TNSDateTime TarihOku(string metin)
        {
            metin = Temiz(metin);
            if (metin != "" && DateTime.TryParseExact(metin, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime t))
                return new TNSDateTime(t);
            return new TNSDateTime(DateTime.MinValue);
        }

        /// <summary>Nesnenin adı verilen özelliğinin metin değeri (alt faaliyet listesi farklı türde gelebilir).</summary>
        private static string OzellikMetni(object o, string ad)
        {
            if (o == null) return "";
            PropertyInfo p = o.GetType().GetProperty(ad);
            if (p != null) return Convert.ToString(p.GetValue(o)) ?? "";
            FieldInfo f = o.GetType().GetField(ad);
            return f != null ? Convert.ToString(f.GetValue(o)) ?? "" : "";
        }

        private static List<object> ParaBirimiListesi()
        {
            List<object> liste = new List<object>();
            ObjectArray oa = TNS.HRC.Arac.ParaBirimleri;
            if (oa?.objeler != null)
                foreach (object o in oa.objeler)
                    if (o is TNS.BKD.ParaBirimi pb)
                        liste.Add(new { kod = pb.kod, kisaltma = pb.kisaltma });
            return liste;
        }

        // ------------------------------------------------------------------ okuma

        /// <summary>Bütçe kaynağının bilgileri ve tabloları (eski EkranAyarla / ButceKaynakListele).</summary>
        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int yil, string senaryoKod, int kaynakNo)
        {
            IResult ret = YatProjeOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            if (kaynakNo <= 0) return Hata("Bütçe Kaynak No boş geliyor.");
            if (yil < 2000) return Hata("Yıl hatalı!");
            senaryoKod = Temiz(senaryoKod);

            try
            {
                BilgiVAN bilgi = TNS.YAT.Arac.Tanimla().ButceKaynakBilgiOku(kullanan, kaynakNo, yil, senaryoKod, string.Empty, true, true, false, null);
                if (bilgi == null || bilgi.sonuc == null || !bilgi.sonuc.islemSonuc)
                    return Hata(YatProjeOrtak.HataMetni(bilgi?.sonuc?.hataStr, "Kaynak bilgileri okunamadı."));
                if (bilgi.liste == null || bilgi.liste.Count == 0)
                    return Hata(YatProjeOrtak.HataMetni(bilgi.sonuc.bilgiStr, "Kaynak bulunamadı."));

                ButceKaynak kaynak = (ButceKaynak)bilgi.liste[0];
                // Eski sayfa türü adından geri buluyordu (TurBul): tanımsız tür Bütçe sayılır.
                int tur = (int)kaynak.kaynakTur;
                if (tur < 1 || tur > 6) tur = (int)ENUMButceKaynakTur.BUTCE;

                // Teklif ve harcama tablolarında yıl-2, yıl-1 ve yıl satırları hep bulunur.
                if (kaynak.teklifler == null) kaynak.teklifler = new TNSCollection();
                for (int y = yil - 2; y <= yil; y++)
                {
                    if (ButceYiliVarMi(kaynak.teklifler, y)) continue;
                    ButceKaynakTeklif bkt = new ButceKaynakTeklif();
                    bkt.butceYil = y;
                    kaynak.teklifler.Add(bkt);
                }
                kaynak.teklifler.Sort();

                List<object> teklif = new List<object>();
                List<object> harcama = new List<object>();
                foreach (ButceKaynakTeklif t in kaynak.teklifler)
                {
                    teklif.Add(TeklifSatiri(t, TeklifAlanlari));
                    harcama.Add(TeklifSatiri(t, HarcamaAlanlari));
                }

                List<object> altFaaliyet = new List<object>();
                if (kaynak.pebKodlar != null)
                    foreach (object o in kaynak.pebKodlar)
                        altFaaliyet.Add(new { pebKod = OzellikMetni(o, "pebKod"), pebAd = OzellikMetni(o, "pebAd") });

                List<object> krediPlan = new List<object>();
                if (kaynak.krediPlan != null)
                    foreach (KrediDetay k in kaynak.krediPlan)
                        krediPlan.Add(new
                        {
                            yil = k.yil,
                            anlasmaTutari = k.anlasmaTutari, anlasmaTutariIc = k.anlasmaTutariIc,
                            kullanilanTutar = k.kullanilanTutar, kullanilanTutarIc = k.kullanilanTutarIc,
                            kullanilacakTutar = k.kullanilacakTutar, kullanilacakTutarIc = k.kullanilacakTutarIc
                        });

                List<object> krediOdeme = new List<object>();
                if (kaynak.geriOdeme != null)
                    foreach (ButceKaynakGeriOdeme g in kaynak.geriOdeme)
                        krediOdeme.Add(new
                        {
                            yil = g.yil, anaParaGeriOdeme = g.anaParaGeriOdeme, odenecekFaiz = g.odenecekFaiz,
                            projeninSonDurumu = g.projeninSonDurumu ?? "", darBogazCozum = g.darBogazCozum ?? ""
                        });

                List<object> yeniTeklif = new List<object>();
                if (kaynak.yeniTeklifler != null)
                    foreach (YeniProjeButce p in kaynak.yeniTeklifler)
                        yeniTeklif.Add(new
                        {
                            ekoKod = p.ekoKod ?? "", ekoAd = p.ekoAd ?? "",
                            projeTutariDis = p.projeTutariDis, projeTutariIc = p.projeTutariIc,
                            disKredi0 = p.disKredi0, icKaynak0 = p.icKaynak0,
                            disKredi1 = p.disKredi1, icKaynak1 = p.icKaynak1,
                            disKredi2 = p.disKredi2, icKaynak2 = p.icKaynak2
                        });

                return Results.Json(new
                {
                    yonetici = YatProjeOrtak.SistemYoneticisi(kullanan),
                    paraBirimleri = ParaBirimiListesi(),
                    kaynak = new
                    {
                        kaynakNo = kaynak.kaynakNo,
                        kaynakAd = kaynak.kaynakAd ?? "",
                        kaynakTur = tur,
                        turAd = ButceKaynak.TurAdGetir((ENUMButceKaynakTur)tur),
                        toplamaDahil = (int)kaynak.toplamaDahil,
                        dptRapor = kaynak.DPTRapordaGor,
                        yikob = kaynak.yikobMu == 1,
                        pbKod = kaynak.pbKod,
                        dipnot = kaynak.dipnot ?? "",
                        krediyiVeren = kaynak.krediyiVeren ?? "",
                        baslamaTarih = TarihYaz(kaynak.baslamaTarih),
                        yururlukTarih = TarihYaz(kaynak.yururlukTarih),
                        tur = kaynak.tur ?? "",
                        tutar = kaynak.tutar ?? "",
                        vade = kaynak.vade ?? "",
                        odemesizDonem = kaynak.odemesizDonem ?? "",
                        faizOran = kaynak.faizOran ?? "",
                        icParaKrediOran = kaynak.IcParaKrediOran ?? ""
                    },
                    teklif = teklif,
                    harcama = harcama,
                    altFaaliyet = altFaaliyet,
                    krediPlan = krediPlan,
                    krediOdeme = krediOdeme,
                    yeniTeklif = yeniTeklif
                });
            }
            catch (Exception)
            {
                return Hata("Kaynak bilgileri okunamadı.");
            }
        }

        /// <summary>Para birimleri (eski stoParaBirimi: TNS.HRC.Arac.ParaBirimleri).</summary>
        private static IResult ParaBirimleri(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = YatProjeOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                return Results.Json(new { paraBirimleri = ParaBirimiListesi() });
            }
            catch (Exception)
            {
                return Hata("Para birimleri okunamadı.");
            }
        }

        /// <summary>Ekonomik kod ağacının bir düzeyi (eski ddEko ağacı: nodeLoadPar('4', yil, ...)): { dugumler: [{ kod, ad, altVar }] }.</summary>
        private static IResult EkoKodlar(HttpContext baglam, IWebHostEnvironment ortam, int yil, string ust)
        {
            IResult ret = YatProjeOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            return ButceRakamUclari.KodDuzeyi(kullanan, yil, "4", ust);
        }

        /// <summary>Kaynağın taşınabileceği alt projeler (eski wndAltProjeler_BeforeShow / AltProjeCevir).</summary>
        private static IResult AltProjeler(HttpContext baglam, IWebHostEnvironment ortam, int yil, string senaryoKod)
        {
            IResult ret = YatProjeOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!YatProjeOrtak.SistemYoneticisi(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (yil < 2000) return Hata("Yıl hatalı!");

            try
            {
                BilgiVAN projeler = TNS.YAT.Arac.Tanimla().ProjeListele(kullanan, yil, Temiz(senaryoKod), new Proje(), null, true);
                if (projeler == null || projeler.sonuc == null || !projeler.sonuc.islemSonuc)
                    return Hata(YatProjeOrtak.HataMetni(projeler?.sonuc?.hataStr, "Alt projeler okunamadı."));
                List<object> satirlar = new List<object>();
                if (projeler.liste != null)
                    foreach (Proje proje in projeler.liste)
                    {
                        if (proje.altProjeler == null) continue;
                        foreach (AltProje ap in proje.altProjeler)
                            if (ap.kod > 0)
                                satirlar.Add(new { kod = ap.kod, ad = ap.ad ?? "", projeAd = proje.ad ?? "", projeAciklama = proje.DPTProjeNo ?? "" });
                    }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Alt projeler okunamadı.");
            }
        }

        // ------------------------------------------------------------------ yazma

        /// <summary>
        /// Kaydet (eski btnKaydet_Click / ButceKaynakKaydet): önce kaynak kimliği (ButceKaynakKaydet), sonra detaylar
        /// (ButceKaynakDetayKaydet). Tekliflerden yalnızca yılın satırı; harcamaların yıl-1 ve yıl-2 satırları yalnızca sistem
        /// yöneticisinde eklenir.
        /// </summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, YatKaynakKaydetIstegi istek, IAntiforgery sahtecilik)
        {
            var (ret, kullanan) = await YatProjeOrtak.DenetleYaz(baglam, ortam, sahtecilik, SayfaAd);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();
            if (istek.KaynakNo <= 0) return Hata("Bütçe Kaynak No boş geliyor.");
            if (istek.Yil < 2000) return Hata("Yıl hatalı!");
            string senaryoKod = Temiz(istek.SenaryoKod);
            int yil = istek.Yil;

            ButceKaynak kaynak = new ButceKaynak();
            kaynak.kaynakNo = istek.KaynakNo;
            kaynak.kaynakTur = istek.KaynakTur >= 1 && istek.KaynakTur <= 6 ? (ENUMButceKaynakTur)istek.KaynakTur : ENUMButceKaynakTur.BUTCE;
            kaynak.altProjeKodu = istek.AltProjeKodu;
            kaynak.kaynakAd = istek.KaynakAd ?? "";
            kaynak.toplamaDahil = istek.ToplamaDahil >= 1 && istek.ToplamaDahil <= 4 ? (ButceKaynak.ENUMKaynakToplam)istek.ToplamaDahil : ButceKaynak.ENUMKaynakToplam.ProjeyeGeneleDahil;
            kaynak.DPTRapordaGor = istek.DptRapor;
            kaynak.yikobMu = istek.Yikob ? 1 : 0;
            kaynak.pbKod = istek.PbKod > 0 ? istek.PbKod : 1;
            kaynak.dipnot = istek.Dipnot ?? "";

            try
            {
                IYATServis servis = TNS.YAT.Arac.Tanimla();
                Sonuc sonuc = servis.ButceKaynakKaydet(kullanan, yil, senaryoKod, kaynak, false);
                if (!sonuc.islemSonuc) return Hata(YatProjeOrtak.HataMetni(sonuc.hataStr, "Kayıt işlemi başarısız."));

                kaynak.krediyiVeren = Temiz(istek.KrediyiVeren);
                kaynak.baslamaTarih = TarihOku(istek.BaslamaTarih);
                kaynak.yururlukTarih = TarihOku(istek.YururlukTarih);
                kaynak.tur = Temiz(istek.Tur);
                kaynak.tutar = Temiz(istek.Tutar);
                kaynak.vade = Temiz(istek.Vade);
                kaynak.odemesizDonem = Temiz(istek.OdemesizDonem);
                kaynak.faizOran = Temiz(istek.FaizOran);
                kaynak.IcParaKrediOran = Temiz(istek.IcParaKrediOran);

                List<ButceKaynakTeklif> teklifler = (istek.Teklif ?? new List<Dictionary<string, double>>()).ConvertAll(s => TeklifOl(s, TeklifAlanlari));
                List<ButceKaynakTeklif> harcamalar = (istek.Harcama ?? new List<Dictionary<string, double>>()).ConvertAll(s => TeklifOl(s, HarcamaAlanlari));
                ButceKaynakTeklif teklifYil = YilBul(teklifler, yil);
                if (teklifYil != null) kaynak.teklifler.Add(teklifYil);
                if (YatProjeOrtak.SistemYoneticisi(kullanan))
                {
                    ButceKaynakTeklif teklifHarcama1 = YilBul(harcamalar, yil - 1);
                    ButceKaynakTeklif teklifHarcama2 = YilBul(harcamalar, yil - 2);
                    if (teklifHarcama1 != null) kaynak.teklifler.Add(teklifHarcama1);
                    if (teklifHarcama2 != null) kaynak.teklifler.Add(teklifHarcama2);
                }

                foreach (YatAltFaaliyetSatiri s in istek.AltFaaliyet ?? new List<YatAltFaaliyetSatiri>())
                {
                    string pebKod = Temiz(s.PebKod);
                    if (pebKod == "") continue;
                    TNS.BHMv2.AltFaaliyet af = new TNS.BHMv2.AltFaaliyet();
                    af.pebKod = pebKod;
                    af.pebAd = s.PebAd ?? "";
                    kaynak.pebKodlar.Add(af);
                }

                foreach (YatKrediPlanSatiri s in istek.KrediDetay ?? new List<YatKrediPlanSatiri>())
                {
                    KrediDetay kd = new KrediDetay();
                    kd.yil = s.Yil;
                    kd.anlasmaTutari = s.AnlasmaTutari;
                    kd.anlasmaTutariIc = s.AnlasmaTutariIc;
                    kd.kullanilanTutar = s.KullanilanTutar;
                    kd.kullanilanTutarIc = s.KullanilanTutarIc;
                    kd.kullanilacakTutar = s.KullanilacakTutar;
                    kd.kullanilacakTutarIc = s.KullanilacakTutarIc;
                    kaynak.krediPlan.Add(kd);
                }

                foreach (YatKrediOdemeSatiri s in istek.KrediOdeme ?? new List<YatKrediOdemeSatiri>())
                {
                    ButceKaynakGeriOdeme go = new ButceKaynakGeriOdeme();
                    go.yil = s.Yil;
                    go.anaParaGeriOdeme = s.AnaParaGeriOdeme;
                    go.odenecekFaiz = s.OdenecekFaiz;
                    go.projeninSonDurumu = s.ProjeninSonDurumu ?? "";
                    go.darBogazCozum = s.DarBogazCozum ?? "";
                    kaynak.geriOdeme.Add(go);
                }

                List<YeniProjeButce> yeniTeklifler = new List<YeniProjeButce>();
                foreach (YatYeniTeklifSatiri s in istek.YeniTeklif ?? new List<YatYeniTeklifSatiri>())
                {
                    yeniTeklifler.Add(new YeniProjeButce
                    {
                        ekoKod = Temiz(s.EkoKod), ekoAd = s.EkoAd ?? "",
                        projeTutariDis = s.ProjeTutariDis, projeTutariIc = s.ProjeTutariIc,
                        disKredi0 = s.DisKredi0, icKaynak0 = s.IcKaynak0,
                        disKredi1 = s.DisKredi1, icKaynak1 = s.IcKaynak1,
                        disKredi2 = s.DisKredi2, icKaynak2 = s.IcKaynak2
                    });
                }
                kaynak.yeniTeklifler = yeniTeklifler.ToArray();

                sonuc = servis.ButceKaynakDetayKaydet(kullanan, kaynak, yil, senaryoKod);
                if (!sonuc.islemSonuc) return Hata(YatProjeOrtak.HataMetni(sonuc.hataStr, "Kayıt işlemi başarısız."));
            }
            catch (Exception)
            {
                return Hata("Kayıt işlemi başarısız.");
            }
            return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla tamamlandı.", kaynakAd = kaynak.kaynakAd });
        }

        /// <summary>Sil (eski btnSil_Click / ButceKaynakSil): önce kaynağın detayları, sonra kaynağın kendisi silinir.</summary>
        private static async Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, YatKaynakIstegi istek, IAntiforgery sahtecilik)
        {
            var (ret, kullanan) = await YatProjeOrtak.DenetleYaz(baglam, ortam, sahtecilik, SayfaAd);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();
            if (istek.KaynakNo <= 0) return Hata("Bütçe Kaynak No boş geliyor.");
            string senaryoKod = Temiz(istek.SenaryoKod);

            try
            {
                IYATServis servis = TNS.YAT.Arac.Tanimla();
                Sonuc donen = servis.ButceKaynakDetaySil(kullanan, istek.Yil, senaryoKod, istek.AltProjeKodu, istek.KaynakNo);
                if (!donen.islemSonuc) return Hata(YatProjeOrtak.HataMetni(donen.hataStr, "Silme işlemi başarısız."));
                donen = servis.ButceKaynakSil(kullanan, istek.Yil, senaryoKod, istek.AltProjeKodu, istek.KaynakNo);
                if (!donen.islemSonuc) return Hata(YatProjeOrtak.HataMetni(donen.hataStr, "Silme işlemi başarısız."));
            }
            catch (Exception)
            {
                return Hata("Silme işlemi başarısız.");
            }
            return Results.Json(new { basarili = true, bilgi = "Silme işlemi başarıyla tamamlandı." });
        }

        /// <summary>Kaynağı başka bir alt projeye taşır (eski btnTasiKomut_Click); yalnızca sistem yöneticisi.</summary>
        private static async Task<IResult> Tasi(HttpContext baglam, IWebHostEnvironment ortam, YatKaynakIstegi istek, IAntiforgery sahtecilik)
        {
            var (ret, kullanan) = await YatProjeOrtak.DenetleYaz(baglam, ortam, sahtecilik, SayfaAd);
            if (ret != null) return ret;
            if (!YatProjeOrtak.SistemYoneticisi(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null) return Results.BadRequest();
            if (istek.KaynakNo <= 0) return Hata("Bütçe Kaynak No boş geliyor.");
            if (istek.HedefAltProje <= 0) return Hata("Lütfen taşınacak alt projeyi seçiniz.");

            try
            {
                Sonuc s = TNS.YAT.Arac.Tanimla().KaynakTasi(kullanan, istek.Yil, Temiz(istek.SenaryoKod), istek.KaynakNo, istek.HedefAltProje);
                if (!s.islemSonuc) return Hata(YatProjeOrtak.HataMetni(s.hataStr, "Taşıma işlemi başarısız."));
            }
            catch (Exception)
            {
                return Hata("Taşıma işlemi başarısız.");
            }
            return Results.Json(new { basarili = true, bilgi = "Kaynak taşındı, sonucu görmek için yeniden listeleme yapınız!" });
        }

        // ------------------------------------------------------------------ raporlar

        private static Hashtable RaporParam(int yil, string senaryoKod, int kaynakNo)
        {
            Hashtable raporParam = new Hashtable();
            raporParam["Senaryo"] = Temiz(senaryoKod);
            raporParam["ButceYil"] = yil.ToString();
            raporParam["KaynakNo"] = kaynakNo.ToString();
            raporParam["ProjeNo"] = "-";
            raporParam["AltProjeNo"] = "-";
            return raporParam;
        }

        private static IResult DosyaVer(string dosya, string ad)
        {
            string uzanti = Path.GetExtension(dosya).TrimStart('.');
            return DosyaIslem.DosyaGonder(dosya, ad, true, uzanti);
        }

        /// <summary>Kredi/Hibe planı Yazdır (eski btnYazdir_Click: TekRaporAl(2004, 12, ...)).</summary>
        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, int yil, string senaryoKod, int kaynakNo)
        {
            IResult ret = YatProjeOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            if (kaynakNo <= 0) return Hata("Bütçe Kaynak No boş geliyor.");
            try
            {
                AsposeLisans.Yukle();
                string dosya = YatProjeRaporlari.Rapor2004(kullanan, 12, RaporParam(yil, senaryoKod, kaynakNo), Ayar.Oku("RaporSablonYol"));
                return DosyaVer(dosya, "KrediPlani_" + kaynakNo);
            }
            catch (RaporHatasi ex)
            {
                return Hata(YatProjeOrtak.HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
            catch (Exception)
            {
                return Hata("Rapor hazırlanamadı.");
            }
        }

        /// <summary>Kredi izleme raporu (eski btnYazdirIzleme_Click: TekRaporAl(2002, 15, ...)).</summary>
        private static IResult IzlemeYazdir(HttpContext baglam, IWebHostEnvironment ortam, int yil, string senaryoKod, int kaynakNo)
        {
            IResult ret = YatProjeOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            if (kaynakNo <= 0) return Hata("Bütçe Kaynak No boş geliyor.");
            try
            {
                Hashtable raporParam = RaporParam(yil, senaryoKod, kaynakNo);
                raporParam["KurKod"] = TNS.DEG.Arac.VarsayilanKurumBul();
                AsposeLisans.Yukle();
                string dosya = YatProjeRaporlari.Rapor2002(kullanan, 15, raporParam, Ayar.Oku("RaporSablonYol"));
                return DosyaVer(dosya, "KrediIzleme_" + kaynakNo);
            }
            catch (RaporHatasi ex)
            {
                return Hata(YatProjeOrtak.HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
            catch (Exception)
            {
                return Hata("Rapor hazırlanamadı.");
            }
        }
    }
}
