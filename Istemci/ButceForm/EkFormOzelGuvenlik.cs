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
using TNS.BKD;
using TNS.KYM;

namespace Istemci.ButceForm
{
    /// <summary>Özel güvenlik / temizlik ihalesinin tertip tablosundaki bir satır (eski fpListe: KurKod, FonKod, FinKod, EkoKod).</summary>
    public class IhaleTertipSatiri
    {
        public string KurKod { get; set; }
        public string FonKod { get; set; }
        public string FinKod { get; set; }
        public string EkoKod { get; set; }
    }

    /// <summary>Maliyet tablosundaki bir satır (eski fpListe1); boş hücre null gelir.</summary>
    public class IhaleMaliyetSatiri
    {
        public double? Harcama { get; set; }
        public double? Tahmin { get; set; }
        public double? Teklif { get; set; }
    }

    /// <summary>
    /// Özel güvenlik ve temizlik formlarındaki ihale ayrıntısı (eski sayfalarda aynı alanlar). Yıllık alanlar üç öğelidir:
    /// [0] önceki (yıl - 2), [1] bu yıl (yıl - 1), [2] sonraki (bütçe yılı). Eski metin kutularında olduğu gibi metin gelir.
    /// </summary>
    public class IhaleDetayi
    {
        public string Aciklama { get; set; }
        public string[] HizmetAlani { get; set; }
        public string[] GorevliSayisi { get; set; }
        public string[] Baslama { get; set; }
        public string[] Bitis { get; set; }
        public List<IhaleTertipSatiri> Tertipler { get; set; }
        /// <summary>Maliyet kodu 1..10 sırasıyla (dizinde 0..9).</summary>
        public List<IhaleMaliyetSatiri> Maliyetler { get; set; }

        internal string Yillik(string[] d, int i) { return d != null && d.Length > i ? (d[i] ?? "") : ""; }
    }

    public class OzelGuvenlikIstegi
    {
        public int Yil { get; set; }
        public string Kur { get; set; }
        public IhaleDetayi Detay { get; set; }
    }

    /// <summary>
    /// Form 21 Özel güvenlik hizmetine dair bilgi formu (eski ButceForm/OzelGuvenlik.aspx?yil=..&amp;kurKod=..). Eski formda tek ihale
    /// (ihale no 1) vardı: açıklama, tertip tablosu (5 satır), üç yılın hizmet alanı / görevli sayısı / başlama / bitiş bilgileri
    /// ve maliyet tablosu (10 kalem: harcama, yıl sonu harcama tahmini, teklif).
    ///   GET  /api/ekform/ozelguvenlik/liste?yil=&amp;kur=    (eski OzelGuvenlikListele -> OzelGuvenlikMaliyetOku(.., 1))
    ///   POST /api/ekform/ozelguvenlik/kaydet              (eski btnKaydet_Click -> OzelGuvenlikKaydet(.., 1, bilgi))
    ///   GET  /api/ekform/ozelguvenlik/yazdir?yil=&amp;kur=   (eski btnYazdir_Click -> OzelGuvenlikHazirla)
    /// </summary>
    public static class EkFormOzelGuvenlik
    {
        /// <summary>Eski formun tek ihalesinin numarası (eski IhaleNo = 1; kayıtta da 1 sabit verilirdi).</summary>
        private const int IhaleNo = 1;

        /// <summary>Eski FRMOGU011.</summary>
        internal const string SecimHatasi = "Yıl veya Kurum alanlarından biri veya bir kaçı boş bırakılmış.  Lütfen ilgili alanları kontrol edip tekrar deneyin.";

        /// <summary>Eski FRMOGU012 / FRMTEM012.</summary>
        internal const string KayitTamam = "Bilgiler başarıyla saklandı.";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/ekform/ozelguvenlik/liste", Liste);
            app.MapPost("/api/ekform/ozelguvenlik/kaydet", Kaydet);
            app.MapGet("/api/ekform/ozelguvenlik/yazdir", Yazdir);
        }

        /// <summary>
        /// Eski OzelGuvenlikListele. Kayıt yoksa detay null döner (eski formda alanlar ve tablolar boş kalırdı).
        /// Not: eski sayfada IhaleNo örnek alanıydı; ilk açılışta 1 verilip okunuyordu (çerçeve sayfası Listele'de sayfayı yeniden yüklüyordu).
        /// </summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur)
        {
            IResult ret = EkFormUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!EkFormUclari.SecimGecerli(yil, kur)) return EkFormUclari.Hata(SecimHatasi);
            try
            {
                OzelGuvenlikBilgi[] ogL = TNS.BHF.Arac.Tanimla().OzelGuvenlikMaliyetOku(kullanan, yil, kur.Trim(), IhaleNo);
                object detay = null;
                if (ogL != null && ogL.Length > 0)
                {
                    OzelGuvenlikBilgi ogb = ogL[0];
                    List<IhaleMaliyetSatiri> maliyetler = BosMaliyetler();
                    foreach (OzelGuvenlikMaliyet m in ogb.maliyetler ?? new OzelGuvenlikMaliyet[0])
                        MaliyetYaz(maliyetler, m.kod, m.Harcama, m.Tahmin, m.Teklif);
                    detay = DetayOl(yil, ogb.ihaleTanim != null ? ogb.ihaleTanim.aciklama : "",
                        new[] { ogb.hzmAlanOnceki, ogb.hzmAlanBuYil, ogb.hzmAlanSonraki },
                        new[] { ogb.gorSayisiOnceki, ogb.gorSayisiBuYil, ogb.gorSayisiSonraki },
                        new[] { ogb.basOnceki, ogb.basBuYil, ogb.basSonraki },
                        new[] { ogb.bitisOnceki, ogb.bitisBuYil, ogb.bitisSonraki },
                        ogb.tertipler, maliyetler);
                }
                return Results.Json(new { detay = detay });
            }
            catch (Exception ex)
            {
                return EkFormUclari.Hata(EkFormUclari.HataMetni(ex.Message, "Özel güvenlik bilgileri okunamadı."));
            }
        }

        /// <summary>Eski btnKaydet_Click: ekrandaki bilgilerle OzelGuvenlikBilgi kurulur, ihale no 1 ile saklanır.</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, OzelGuvenlikIstegi istek)
        {
            var (ret, kullanan) = await EkFormUclari.DenetleYaz(baglam, ortam, sahtecilik);
            if (ret != null) return ret;
            istek ??= new OzelGuvenlikIstegi();
            if (!EkFormUclari.SecimGecerli(istek.Yil, istek.Kur)) return EkFormUclari.Hata(SecimHatasi);
            IhaleDetayi d = istek.Detay ?? new IhaleDetayi();
            try
            {
                OzelGuvenlikBilgi bilgi = new OzelGuvenlikBilgi();
                bilgi.ihaleTanim.aciklama = d.Aciklama ?? "";
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
                for (int i = 0; i < ml.Count && i < MaliyetSayisi; i++)
                {
                    IhaleMaliyetSatiri s = ml[i];
                    // Eski kod: üç hücreden en az biri dolu olan kalem saklanır (boş hücre 0).
                    if (s == null || (s.Harcama == null && s.Tahmin == null && s.Teklif == null)) continue;
                    OzelGuvenlikMaliyet maliyet = new OzelGuvenlikMaliyet();
                    maliyet.kod = Convert.ToString(i + 1);
                    maliyet.Harcama = s.Harcama ?? 0;
                    maliyet.Tahmin = s.Tahmin ?? 0;
                    maliyet.Teklif = s.Teklif ?? 0;
                    al.Add(maliyet);
                }
                bilgi.maliyetler = (OzelGuvenlikMaliyet[])al.ToArray(typeof(OzelGuvenlikMaliyet));
                bilgi.tertipler = TertipleriAl(d.Tertipler);

                Sonuc sonuc = TNS.BHF.Arac.Tanimla().OzelGuvenlikKaydet(kullanan, istek.Yil, istek.Kur.Trim(), IhaleNo, bilgi);
                if (!sonuc.islemSonuc) return EkFormUclari.Hata(EkFormUclari.HataMetni(sonuc.hataStr, "Kayıt yapılamadı."));
                return EkFormUclari.Tamam(KayitTamam);
            }
            catch (Exception ex)
            {
                return EkFormUclari.Hata(EkFormUclari.HataMetni(ex.Message, "Kayıt yapılamadı."));
            }
        }

        /// <summary>Eski btnYazdir_Click: OzelGuvenlikHazirla, dosya adı "form{no}_{kurKod}".</summary>
        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur)
        {
            IResult ret = EkFormUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!EkFormUclari.SecimGecerli(yil, kur)) return EkFormUclari.Hata(SecimHatasi);
            kur = kur.Trim();
            return EkFormUclari.RaporGonder(() => new global::ButceForm.ButceFormRaporlari().OzelGuvenlikHazirla(
                kullanan, TNS.BHF.Arac.Tanimla(), null, DosyaIslem.DosyaAdUret(), EkFormUclari.ExcelTur(),
                EkFormUclari.RaporParam(yil, kur, ""), ""),
                "form" + global::ButceForm.BHFFormlar.FormNoBul(yil, "OzelGuvenlik") + "_" + kur);
        }

        // ------------------------------------------------------------------ özel güvenlik ve temizlik formlarının ortak parçaları

        /// <summary>Eski maliyet tablosunun satır sayısı (eski GridInit: RowHeader.Rows.Count = 10).</summary>
        internal const int MaliyetSayisi = 10;

        /// <summary>Eski tertip tablosunun satır sayısı (eski GridTertip: RowCount = 5).</summary>
        internal const int TertipSayisi = 5;

        internal static List<IhaleMaliyetSatiri> BosMaliyetler()
        {
            List<IhaleMaliyetSatiri> l = new List<IhaleMaliyetSatiri>();
            for (int i = 0; i < MaliyetSayisi; i++) l.Add(new IhaleMaliyetSatiri());
            return l;
        }

        /// <summary>Eski listeleme: maliyet kodu (1..10) satırına harcama, tahmin ve teklif yazılır.</summary>
        internal static void MaliyetYaz(List<IhaleMaliyetSatiri> l, string kod, double harcama, double tahmin, double teklif)
        {
            int i = Convert.ToInt16(kod) - 1;
            if (i < 0 || i >= l.Count) return;
            l[i] = new IhaleMaliyetSatiri { Harcama = harcama, Tahmin = tahmin, Teklif = teklif };
        }

        /// <summary>Eski listeleme: tertip kodları bütçe tertip yapısına göre noktalanarak (KodNoktaKoy) gösterilir; tablo 5 satırdır.</summary>
        internal static object DetayOl(int yil, string aciklama, int[] hizmetAlani, int[] gorevliSayisi, string[] baslama, string[] bitis,
            GuvenlikTemizlikIhaleTertip[] tertipler, List<IhaleMaliyetSatiri> maliyetler)
        {
            Tertip t = new Tertip(yil, ENUMTabloKod.TABButce);
            List<object> tl = new List<object>();
            foreach (GuvenlikTemizlikIhaleTertip tr in tertipler ?? new GuvenlikTemizlikIhaleTertip[0])
            {
                tl.Add(new
                {
                    kurKod = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(0), tr.kurKod),
                    fonKod = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(1), tr.fonKod),
                    finKod = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(2), tr.finKod),
                    ekoKod = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(3), tr.ekoKod)
                });
            }
            return new
            {
                aciklama = aciklama ?? "",
                hizmetAlani = Array.ConvertAll(hizmetAlani, x => x.ToString()),
                gorevliSayisi = Array.ConvertAll(gorevliSayisi, x => x.ToString()),
                baslama = Array.ConvertAll(baslama, x => x ?? ""),
                bitis = Array.ConvertAll(bitis, x => x ?? ""),
                tertipler = tl,
                maliyetler = maliyetler
            };
        }

        /// <summary>Eski kayıt: dört kodu da dolu olan tertip satırları, ekranda yazıldığı gibi saklanır.</summary>
        internal static GuvenlikTemizlikIhaleTertip[] TertipleriAl(List<IhaleTertipSatiri> satirlar)
        {
            ArrayList al = new ArrayList();
            foreach (IhaleTertipSatiri s in satirlar ?? new List<IhaleTertipSatiri>())
            {
                if (s == null) continue;
                GuvenlikTemizlikIhaleTertip tertip = new GuvenlikTemizlikIhaleTertip();
                tertip.kurKod = s.KurKod ?? "";
                tertip.fonKod = s.FonKod ?? "";
                tertip.finKod = s.FinKod ?? "";
                tertip.ekoKod = s.EkoKod ?? "";
                if (!string.IsNullOrWhiteSpace(tertip.kurKod) && !string.IsNullOrWhiteSpace(tertip.fonKod)
                    && !string.IsNullOrWhiteSpace(tertip.finKod) && !string.IsNullOrWhiteSpace(tertip.ekoKod))
                    al.Add(tertip);
            }
            return (GuvenlikTemizlikIhaleTertip[])al.ToArray(typeof(GuvenlikTemizlikIhaleTertip));
        }
    }
}
