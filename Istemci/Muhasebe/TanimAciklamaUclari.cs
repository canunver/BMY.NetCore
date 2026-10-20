using System.Collections.Generic;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.KYM;
using TNS.MUH;
using static Istemci.Muhasebe.MuhTanimOrtak;

namespace Istemci.Muhasebe
{
    public class TanimAciklamaIstegi
    {
        public int kod { get; set; }
        public int muhasebeKod { get; set; }
        public string birimKod { get; set; }
        public int tipi { get; set; }
        public string adi { get; set; }
        public string aciklama { get; set; }
    }

    public class TanimAciklamaSilIstegi
    {
        public int kod { get; set; }
    }

    /// <summary>
    /// Açıklama Tanımları (eski ButceMuhasebe/TanimAciklama.aspx): Ödeme Emri / MİF formundan pencere olarak açılır
    /// (TanimAciklama?muhasebe=..&amp;birim=..&amp;tip=1). Uçlar /api/tanimaciklama/*:
    ///   liste  (GET, muhasebe, birim, tip) : { satirlar: [ { kod, adi, aciklama, muhasebeKod, birimKod, tipi } ] }
    ///   kaydet (POST TanimAciklamaIstegi)  : { basarili, bilgi } / { hata }
    ///   sil    (POST { kod })              : { basarili, bilgi } / { hata }
    /// Sayfa menüde yok; yetkisi Ödeme Emri / MİF Belgesi menü seçeneklerinden gelir.
    /// </summary>
    public static class TanimAciklamaUclari
    {
        public const string Kok = "/api/tanimaciklama/";
        public const string SayfaAdresi = "../ButceMuhasebe/TanimAciklama.aspx";

        /// <summary>Ödeme Emri / MİF çerçevesinin menüdeki adresleri (alt sayfaların yetkisi bunlardan gelir).</summary>
        public static readonly string[] UstSayfalar =
        {
            "../ButceMuhasebe/OdemeEmriMIFFrame.aspx",
            "../ButceMuhasebe/OdemeEmriMIFFrame.aspx?yeni=1",
            "../ButceMuhasebe/OdemeEmriMIFFrame.aspx?yeni=1&baslikSayfasi=1"
        };

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Kok + "liste", Liste);
            app.MapPost(Kok + "kaydet", Kaydet);
            app.MapPost(Kok + "sil", Sil);
        }

        public static bool YetkiVar(Kullanici kullanan, string klasor)
        {
            return SayfaYetki.Yetkili(kullanan, klasor, SayfaAdresi, UstSayfalar);
        }

        private static IMUHServis Servis() { return TNS.MUH.Arac.Tanimla(); }

        /// <summary>
        /// Eski btnListele_Click: /MUHASEBE/ACIKLAMALISTETUR değişkeni "Birim" ise birime göre, "Kisi" ise yalnızca kullanıcının
        /// kaydettikleri (muhasebe / birim aranmaz), değilse muhasebe ve tipe göre listelenir.
        /// </summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int? muhasebe, string birim, int? tip)
        {
            return Oku(baglam, k => YetkiVar(k, ortam.ContentRootPath), kullanan =>
            {
                string listelemeTur = TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/ACIKLAMALISTETUR") + "";

                AciklamaBilgi t = new AciklamaBilgi();
                t.muhasebeKod = muhasebe ?? 0;
                t.tipi = tip ?? 0;

                if (listelemeTur == "Birim")
                    t.birimKod = Temiz(birim);

                if (listelemeTur == "Kisi")
                {
                    t.muhasebeKod = 0;
                    t.birimKod = "";
                    t.kayitEden = kullanan.mernis;
                }

                ObjectArray bilgiler = Servis().AciklamaListele(kullanan, t);
                if (bilgiler == null || !bilgiler.sonuc.islemSonuc)
                    return Hata(HataMetni(bilgiler?.sonuc.hataStr, "Açıklamalar okunamadı."));

                List<object> satirlar = new List<object>();
                foreach (AciklamaBilgi tt in bilgiler.objeler)
                    satirlar.Add(new
                    {
                        kod = tt.kod,
                        adi = tt.adi ?? "",
                        aciklama = tt.aciklama ?? "",
                        muhasebeKod = tt.muhasebeKod,
                        birimKod = tt.birimKod ?? "",
                        tipi = tt.tipi
                    });
                return Results.Json(new { satirlar });
            }, "Açıklamalar okunamadı.");
        }

        /// <summary>Eski btnKaydet_Click: kod 0 ise yeni kayıt; kaydeden oturumdaki kullanıcı, paylaşılan 0.</summary>
        private static Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TanimAciklamaIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => YetkiVar(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null) return Hata("Bilgiler alınamadı.");

                AciklamaBilgi t = new AciklamaBilgi();
                t.kod = istek.kod;
                t.tipi = istek.tipi;
                t.muhasebeKod = istek.muhasebeKod;
                t.birimKod = Temiz(istek.birimKod);
                t.adi = istek.adi ?? "";
                t.aciklama = istek.aciklama ?? "";
                t.paylasilan = 0;
                t.kayitEden = kullanan.mernis;

                return SonucYaniti(Servis().AciklamaKaydet(kullanan, t), "Başarıyla kayıt edildi");
            });
        }

        private static Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TanimAciklamaSilIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => YetkiVar(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null || istek.kod <= 0) return Hata("Lütfen silmek istediğiniz açıklamayı listeden seçin.");
                AciklamaBilgi t = new AciklamaBilgi();
                t.kod = istek.kod;
                return SonucYaniti(Servis().AciklamaSil(kullanan, t), "Başarıyla silindi");
            });
        }
    }
}
