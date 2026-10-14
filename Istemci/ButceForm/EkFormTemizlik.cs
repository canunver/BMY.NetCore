using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.BHF;
using TNS.KYM;

namespace Istemci.ButceForm
{
    /// <summary>Saklanacak bir temizlik ihalesi: sekmenin birimi ve ihale numarası (0: yeni ihale) ile ekrandaki bilgiler.</summary>
    public class TemizlikIhaleIstegi
    {
        public string KurKod { get; set; }
        public int IhaleNo { get; set; }
        public IhaleDetayi Detay { get; set; }
    }

    public class TemizlikIstegi
    {
        public int Yil { get; set; }
        public List<TemizlikIhaleIstegi> Ihaleler { get; set; }
    }

    /// <summary>
    /// Form 22 Temizlik hizmetine ilişkin bilgi formu. Eski ButceForm/TemizlikHizmeti.aspx her ihale için bir sekme açıp sekmede
    /// çerçeve içinde TemizlikHizmetiIhaleDetay.aspx?yil=..&amp;kurKod=..&amp;ihaleNo=.. yüklüyordu; burada sekmeler aynı sayfada çizilir.
    ///   GET  /api/ekform/temizlik/liste?yil=&amp;kur=   (eski TemizlikHizmeti.TemizlikListele -> TemizlikMaliyetOku(.., 0) ile ihaleler,
    ///                                                 her sekme için TemizlikHizmetiIhaleDetay.TemizlikListele -> TemizlikMaliyetOku(.., ihaleNo))
    ///   POST /api/ekform/temizlik/kaydet             (eski TemizlikHizmetiIhaleDetay.btnKaydet_Click -> TemizlikKaydet(.., kurKod, ihaleNo, bilgi))
    ///   GET  /api/ekform/temizlik/yazdir?yil=&amp;kur=  (eski TemizlikHizmeti.btnYazdir_Click -> TemizlikHazirla)
    /// </summary>
    public static class EkFormTemizlik
    {
        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/ekform/temizlik/liste", Liste);
            app.MapPost("/api/ekform/temizlik/kaydet", Kaydet);
            app.MapGet("/api/ekform/temizlik/yazdir", Yazdir);
        }

        /// <summary>
        /// Eski TemizlikListele: ihale no 0 ile birimin (ve altındakilerin) ihaleleri okunur; her biri "n. İhale" sekmesi olur ve
        /// ayrıntısı sekmenin birimi ve ihale numarasıyla ayrıca okunur (eski detay sayfası). Seçilen birim en alt düzeyse
        /// (noktasız 8 hane) boş bir "Yeni İhale" sekmesi (ihale no 0) eklenir.
        /// </summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur)
        {
            IResult ret = EkFormUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            // Eski ana sayfa denetlemeden okuyordu; detay sayfası yıl ve birim boşsa okumuyordu.
            if (!EkFormUclari.SecimGecerli(yil, kur)) return EkFormUclari.Hata(EkFormOzelGuvenlik.SecimHatasi);
            kur = kur.Trim();
            try
            {
                IBHFServis servis = TNS.BHF.Arac.Tanimla();
                TemizlikBilgi[] ihaleler = servis.TemizlikMaliyetOku(kullanan, yil, kur, 0) ?? new TemizlikBilgi[0];
                List<object> sekmeler = new List<object>();
                int sira = 1;
                foreach (TemizlikBilgi tb in ihaleler)
                {
                    sekmeler.Add(new
                    {
                        baslik = string.Format("{0}. İhale", sira),
                        kurKod = tb.kurKod,
                        ihaleNo = tb.ihaleNo,
                        detay = DetayOku(servis, kullanan, yil, tb.kurKod, tb.ihaleNo)
                    });
                    sira++;
                }
                if (kur.Replace(".", "").Length == 8)
                    sekmeler.Add(new { baslik = "Yeni İhale", kurKod = kur, ihaleNo = 0, detay = (object)null });
                return Results.Json(new { ihaleler = sekmeler });
            }
            catch (Exception ex)
            {
                return EkFormUclari.Hata(EkFormUclari.HataMetni(ex.Message, "Temizlik hizmeti bilgileri okunamadı."));
            }
        }

        /// <summary>Eski TemizlikHizmetiIhaleDetay.TemizlikListele (yalnızca ihale no &gt; 0 ise okunur; kayıt yoksa null).</summary>
        private static object DetayOku(IBHFServis servis, Kullanici kullanan, int yil, string kurKod, int ihaleNo)
        {
            if (!(yil > 0 && !string.IsNullOrEmpty(kurKod) && ihaleNo > 0)) return null;
            TemizlikBilgi[] ogL = servis.TemizlikMaliyetOku(kullanan, yil, kurKod, ihaleNo);
            if (ogL == null || ogL.Length == 0) return null;
            TemizlikBilgi ogb = ogL[0];
            List<IhaleMaliyetSatiri> maliyetler = EkFormOzelGuvenlik.BosMaliyetler();
            foreach (TemizlikMaliyet m in ogb.maliyetler ?? new TemizlikMaliyet[0])
                EkFormOzelGuvenlik.MaliyetYaz(maliyetler, m.kod, m.Harcama, m.Tahmin, m.Teklif);
            return EkFormOzelGuvenlik.DetayOl(yil, ogb.aciklama,
                new[] { ogb.hzmAlanOnceki, ogb.hzmAlanBuYil, ogb.hzmAlanSonraki },
                new[] { ogb.gorSayisiOnceki, ogb.gorSayisiBuYil, ogb.gorSayisiSonraki },
                new[] { ogb.basOnceki, ogb.basBuYil, ogb.basSonraki },
                new[] { ogb.bitisOnceki, ogb.bitisBuYil, ogb.bitisSonraki },
                ogb.tertipler, maliyetler);
        }

        /// <summary>
        /// Eski TemizlikHizmetiIhaleDetay.btnKaydet_Click, istekteki her ihale için sırayla (ilk hatada durur). Eski Kaydet yalnızca
        /// açık sekmenin çerçevesindeki düğmeye basıyordu; burada değişen sekmelerin hepsi aynı çağrıyla saklanır.
        /// </summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TemizlikIstegi istek)
        {
            var (ret, kullanan) = await EkFormUclari.DenetleYaz(baglam, ortam, sahtecilik);
            if (ret != null) return ret;
            istek ??= new TemizlikIstegi();
            List<TemizlikIhaleIstegi> ihaleler = istek.Ihaleler ?? new List<TemizlikIhaleIstegi>();
            if (ihaleler.Count == 0) return EkFormUclari.Hata("Saklanacak ihale yok.");
            try
            {
                IBHFServis servis = TNS.BHF.Arac.Tanimla();
                List<string> anahtarlar = new List<string>();
                foreach (TemizlikIhaleIstegi ih in ihaleler)
                {
                    if (ih == null) continue;
                    if (!EkFormUclari.SecimGecerli(istek.Yil, ih.KurKod)) return EkFormUclari.Hata(EkFormOzelGuvenlik.SecimHatasi);
                    TemizlikBilgi bilgi = BilgiOl(ih.Detay ?? new IhaleDetayi());
                    Sonuc sonuc = servis.TemizlikKaydet(kullanan, istek.Yil, ih.KurKod, ih.IhaleNo, bilgi);
                    if (!sonuc.islemSonuc) return EkFormUclari.Hata(EkFormUclari.HataMetni(sonuc.hataStr, "Kayıt yapılamadı."));
                    anahtarlar.Add(sonuc.anahtar);   // eski: txtNo.Text = sonuc.anahtar (yeni ihalenin numarası)
                }
                return Results.Json(new { basarili = true, bilgi = EkFormOzelGuvenlik.KayitTamam, anahtarlar = anahtarlar });
            }
            catch (Exception ex)
            {
                return EkFormUclari.Hata(EkFormUclari.HataMetni(ex.Message, "Kayıt yapılamadı."));
            }
        }

        /// <summary>Eski btnKaydet_Click'teki TemizlikBilgi kurulumu.</summary>
        private static TemizlikBilgi BilgiOl(IhaleDetayi d)
        {
            TemizlikBilgi bilgi = new TemizlikBilgi();
            bilgi.aciklama = d.Aciklama ?? "";
            bilgi.hzmAlanOnceki = OrtakFonksiyonlar.ConvertToInt(d.Yillik(d.HizmetAlani, 0), 0);
            bilgi.hzmAlanBuYil = OrtakFonksiyonlar.ConvertToInt(d.Yillik(d.HizmetAlani, 1), 0);
            bilgi.hzmAlanSonraki = OrtakFonksiyonlar.ConvertToInt(d.Yillik(d.HizmetAlani, 2), 0);
            bilgi.gorSayisiOnceki = OrtakFonksiyonlar.ConvertToInt(d.Yillik(d.GorevliSayisi, 0), 0);
            bilgi.gorSayisiBuYil = OrtakFonksiyonlar.ConvertToInt(d.Yillik(d.GorevliSayisi, 1), 0);
            bilgi.gorSayisiSonraki = OrtakFonksiyonlar.ConvertToInt(d.Yillik(d.GorevliSayisi, 2), 0);
            bilgi.basOnceki = d.Yillik(d.Baslama, 0);
            bilgi.basBuYil = d.Yillik(d.Baslama, 1);
            bilgi.basSonraki = d.Yillik(d.Baslama, 2);
            bilgi.bitisOnceki = d.Yillik(d.Bitis, 0);
            bilgi.bitisBuYil = d.Yillik(d.Bitis, 1);
            bilgi.bitisSonraki = d.Yillik(d.Bitis, 2);

            ArrayList al = new ArrayList();
            List<IhaleMaliyetSatiri> ml = d.Maliyetler ?? new List<IhaleMaliyetSatiri>();
            for (int i = 0; i < ml.Count && i < EkFormOzelGuvenlik.MaliyetSayisi; i++)
            {
                IhaleMaliyetSatiri s = ml[i];
                // Eski kod: üç hücreden en az biri dolu olan kalem saklanır (boş hücre 0).
                if (s == null || (s.Harcama == null && s.Tahmin == null && s.Teklif == null)) continue;
                TemizlikMaliyet maliyet = new TemizlikMaliyet();
                maliyet.kod = Convert.ToString(i + 1);
                maliyet.Harcama = s.Harcama ?? 0;
                maliyet.Tahmin = s.Tahmin ?? 0;
                maliyet.Teklif = s.Teklif ?? 0;
                al.Add(maliyet);
            }
            bilgi.maliyetler = (TemizlikMaliyet[])al.ToArray(typeof(TemizlikMaliyet));
            bilgi.tertipler = EkFormOzelGuvenlik.TertipleriAl(d.Tertipler);
            return bilgi;
        }

        /// <summary>Eski btnYazdir_Click: TemizlikHazirla, dosya adı "form{no}_{kurKod}".</summary>
        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur)
        {
            IResult ret = EkFormUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!EkFormUclari.SecimGecerli(yil, kur)) return EkFormUclari.Hata(EkFormOzelGuvenlik.SecimHatasi);
            kur = kur.Trim();
            return EkFormUclari.RaporGonder(() => new global::ButceForm.ButceFormRaporlari().TemizlikHazirla(
                kullanan, TNS.BHF.Arac.Tanimla(), null, DosyaIslem.DosyaAdUret(), EkFormUclari.ExcelTur(),
                EkFormUclari.RaporParam(yil, kur, ""), ""),
                "form" + global::ButceForm.BHFFormlar.FormNoBul(yil, "Temizlik") + "_" + kur);
        }
    }
}
