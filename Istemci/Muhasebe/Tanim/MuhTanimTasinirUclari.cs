using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.KYM;
using TNS.MUH;
using static Istemci.Muhasebe.MuhTanimOrtak;

namespace Istemci.Muhasebe
{
    /// <summary>
    /// Taşınır Ekonomik Kod (eski ButceMuhasebe/TasinirEkonomikKod.aspx) uçları:
    ///   tasinir/liste    : yılın 150 ve 25 ile başlayan, yansıtması olan hesapları (eski GridListeDoldur, YansitmaEkoKodListele)
    ///   tasinir/kodlar   : seçilen hesabın 830 ile başlayan yansıtma hesapları (eski SatirSecildi, YansitmaListele)
    /// </summary>
    public static class MuhTanimTasinirUclari
    {
        public const string TasinirAdresi = "../ButceMuhasebe/TasinirEkonomikKod.aspx";

        /// <summary>Eski sayfadaki sabit hesap kodları.</summary>
        private const string HesapKodlari = "150,25";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Kok + "tasinir/liste", Liste);
            app.MapGet(Kok + "tasinir/kodlar", Kodlar);
        }

        private static IMUHServis Servis() { return TNS.MUH.Arac.Tanimla(); }

        /// <summary>Eski sayfa gibi: sistem / muhasebe yöneticisi menü yetkisine bakılmadan girer, diğerleri menü yetkisiyle.</summary>
        public static bool TasinirYetkili(Kullanici kullanan, string klasor) { return Yetkili(kullanan, klasor, TasinirAdresi); }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int? yil)
        {
            return Oku(baglam, k => TasinirYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                HesapPlaniYansitma yansitma = new HesapPlaniYansitma();
                yansitma.hesapKod = HesapKodlari;
                if ((yil ?? 0) != 0) yansitma.yil = yil.Value;

                ObjectArray yler = Servis().YansitmaEkoKodListele(yansitma);
                if (yler == null || !yler.sonuc.islemSonuc) return Hata(HataMetni(yler?.sonuc.hataStr, "Liste okunamadı."));

                List<object> satirlar = new List<object>();
                foreach (HesapPlaniYansitma y in yler.objeler)
                    satirlar.Add(new
                    {
                        YIL = y.yil,
                        HKOD = y.hesapKod ?? "",
                        ACIKLAMA = y.hKodAciklama ?? ""
                    });
                return Results.Json(new { satirlar });
            });
        }

        private static IResult Kodlar(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string hesapKod)
        {
            return Oku(baglam, k => TasinirYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (Temiz(hesapKod) == "") return Results.Json(new { satirlar = new List<object>() });

                HesapPlaniYansitma yansitma = new HesapPlaniYansitma();
                yansitma.hesapKod = Temiz(hesapKod);
                yansitma.yansitmaHesapKod = "830%";
                if ((yil ?? 0) != 0) yansitma.yil = yil.Value;

                ObjectArray yler = Servis().YansitmaListele(yansitma);
                if (yler == null || !yler.sonuc.islemSonuc) return Hata(HataMetni(yler?.sonuc.hataStr, "Hesap kodları okunamadı."));

                List<object> satirlar = new List<object>();
                foreach (HesapPlaniYansitma y in yler.objeler)
                    satirlar.Add(new { YKOD = y.yansitmaHesapKod ?? "" });
                return Results.Json(new { satirlar });
            });
        }
    }
}
