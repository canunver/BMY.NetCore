using System.Collections.Generic;
using System.Threading.Tasks;
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
    public class MudurlukIstegi
    {
        public string kod { get; set; }
        public string ad { get; set; }
        public bool aktif { get; set; }
    }

    /// <summary>
    /// Müdürlük Bilgileri (eski ButceMuhasebe/TanimMudurluk.aspx): Fatura Hazırlama'daki müdürlük kodlarının tanımı.
    ///   GET /api/muhtanim/mudurluk/liste   POST /api/muhtanim/mudurluk/kaydet, mudurluk/sil
    /// İşlemler js/muhtanim-mudurluk.js ile yapılır. Eski sayfa yalnızca menü yetkisiyle girilirdi.
    /// </summary>
    public static class MuhTanimMudurlukUclari
    {
        public const string MudurlukAdresi = "../ButceMuhasebe/TanimMudurluk.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Kok + "mudurluk/liste", Liste);
            app.MapPost(Kok + "mudurluk/kaydet", Kaydet);
            app.MapPost(Kok + "mudurluk/sil", Sil);
        }

        public static bool MudurlukYetkili(Kullanici kullanan, string klasor)
        {
            return Yetkili(kullanan, klasor, MudurlukAdresi, false);
        }

        private static IMUHServis Servis() { return TNS.MUH.Arac.Tanimla(); }

        /// <summary>Eski Listele: bütün müdürlükler (eski Bul tuşu da kriter vermeden listeliyordu).</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam)
        {
            return Oku(baglam, k => MudurlukYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                ObjectArray yler = Servis().MudurlukListele(kullanan, new Mudurluk { aktif = -1 }) /* -1: aktif/pasif hepsi (0 verilirse yalnız pasifler gelir) */;
                if (yler == null) return Results.Json(new { satirlar = new object[0] });
                if (!yler.sonuc.islemSonuc) return Hata(HataMetni(yler.sonuc.hataStr, "Liste okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (Mudurluk d in yler.objeler)
                    satirlar.Add(new { kod = d.kod ?? "", ad = d.ad ?? "", aktif = d.aktif });
                return Results.Json(new { satirlar = satirlar });
            });
        }

        /// <summary>Eski btnKaydet_Click: kod iki karakter, ad boş olamaz.</summary>
        private static Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, MudurlukIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => MudurlukYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null) return Hata("İstek okunamadı.");
                string kod = istek.kod ?? "";
                List<string> hatalar = new List<string>();
                if (kod.Trim() == "")
                    hatalar.Add("Müdürlük kod bilgisi boş bırakılamaz");
                else if (kod.Trim().Length != 2)
                    hatalar.Add("Müdürlük kod bilgisi iki karakterden oluşmalıdır");
                if (Temiz(istek.ad) == "")
                    hatalar.Add("Müdürlük Adı bilgisi boş bırakılamaz");
                if (hatalar.Count > 0) return Hata(string.Join("\n", hatalar) + "\nKayıt işlemi yapılamadı.");

                Mudurluk mudurluk = new Mudurluk();
                mudurluk.kod = kod;
                mudurluk.ad = Temiz(istek.ad);
                mudurluk.aktif = istek.aktif ? 1 : 0;
                return SonucYaniti(Servis().MudurlukKaydet(kullanan, mudurluk), "Kayıt işlemi başarıyla tamamlandı.");
            });
        }

        private static Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, MudurlukIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => MudurlukYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null || (istek.kod ?? "").Trim() == "") return Hata("Lütfen silmek istediğiniz bilgiyi seçin.");
                Mudurluk mudurluk = new Mudurluk();
                mudurluk.kod = istek.kod;
                return SonucYaniti(Servis().MudurlukSil(kullanan, mudurluk), "Silme işlemi başarıyla gerçekleşti.");
            });
        }
    }
}
