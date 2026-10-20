using System.Collections.Generic;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.IKY;
using TNS.KYM;

namespace Istemci.InsanKaynaklari
{
    /// <summary>Birime Al isteği: Tayin Bekleyenler'de seçili satır (eski gpTayinBekleyenler.getRowsValues) ve Tarih alanı.</summary>
    public class KadroBirimeAlIstegi
    {
        public int kod { get; set; }
        public int personelKod { get; set; }
        public string personelTCKimlikNo { get; set; }
        public string tarih { get; set; }
    }

    /// <summary>
    /// Bekleyen İşlemler penceresi (eski InsanKaynaklari/PersonelKadroOnaysiz.aspx) uçları:
    ///   GET  /api/personelbilgi/kadroonaysiz/liste?orgKod=    Onaysızlar ve Tayin Bekleyenler (eski Listele: OnaysizKayitlar)
    ///   POST /api/personelbilgi/kadroonaysiz/birimeal         Birime Al (eski btnBirimeAl_Click)
    /// </summary>
    public static class PersonelKadroOnaysizUclari
    {
        private const string Sayfa = "PersonelKadroOnaysiz";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/personelbilgi/kadroonaysiz/liste", Liste);
            app.MapPost("/api/personelbilgi/kadroonaysiz/birimeal", BirimeAl);
        }

        private static List<object> Satirlar(TNSCollection liste, bool tayin)
        {
            List<object> satirlar = new List<object>();
            if (liste == null) return satirlar;
            foreach (TNS.IKY.PersonelKadro k in liste)
            {
                if (tayin)
                    satirlar.Add(new { k.kod, k.personelKod, k.kadroDurumAd, k.unvanAd, k.personelTCKimlikNo, k.personelAdSoyad, k.birimAd, k.tayinOlacagiBirimAd });
                else
                    satirlar.Add(new { k.kod, k.personelKod, k.kadroDurumAd, k.unvanAd, k.personelTCKimlikNo, k.personelAdSoyad, k.birimAd });
            }
            return satirlar;
        }

        /// <summary>Eski Listele: OnaysizKayitlar(orgKod, sadeceSayilar=false).</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int? orgKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, Sayfa, out Kullanici kullanan);
            if (ret != null) return ret;

            ObjectVAN ov = TNS.IKY.Arac.Tanimla().OnaysizKayitlar(kullanan, orgKod ?? 0, false);
            if (!ov.sonuc.islemSonuc)
                return Results.Json(new { hata = PersonelBilgiOrtak.HataMetni(ov.sonuc.hataStr, "Bekleyen işlemler okunamadı.") });
            OnaysizKadroBilgi kb = ov.o as OnaysizKadroBilgi;
            if (kb == null) return Results.Json(new { onaysizlar = new object[0], tayinBekleyenler = new object[0] });
            return Results.Json(new { onaysizlar = Satirlar(kb.onaysizlar, false), tayinBekleyenler = Satirlar(kb.tayinBekleyenler, true) });
        }

        /// <summary>
        /// Eski btnBirimeAl_Click: seçili tayin kaydı okunur, yeni birimde NAKİLBAŞLAMA sebepli yeni hizmet kaydı oluşturulur ve onaylanır.
        /// </summary>
        private static async Task<IResult> BirimeAl(HttpContext baglam, IWebHostEnvironment ortam, KadroBirimeAlIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, Sayfa, out Kullanici kullanan);
            if (ret != null) return ret;

            if (istek.kod == 0 && istek.personelKod == 0)
                return Results.Json(new { basarili = false, hata = "Lütfen Satır Seçiniz!" });

            IIKYServis servisIKY = TNS.IKY.Arac.Tanimla();
            // Eski kod seçili satırın değerlerini PersonelKadro'ya çeviriyordu; süzmede kullanılan alanlar kod, personelKod, personelTCKimlikNo.
            TNS.IKY.PersonelKadro kadro = new TNS.IKY.PersonelKadro();
            kadro.kod = istek.kod;
            kadro.personelKod = istek.personelKod;
            kadro.personelTCKimlikNo = istek.personelTCKimlikNo ?? "";
            ObjectArray oa = servisIKY.PersonelKadroListele(kullanan, kadro, false);
            if (!oa.sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(oa.sonuc.hataStr, "Hizmet kaydı okunamadı.") });
            if (oa.ObjeSayisi == 0)
                return Results.Json(new { basarili = false, hata = "Kapatılacak kayıt bulunamadı!" });

            kadro = (TNS.IKY.PersonelKadro)oa.objeler[0];
            kadro.eskiKod = kadro.kod;
            kadro.kod = 0;
            kadro.organizasyonKodu = kadro.tayinOlacagiBirim;
            kadro.tayinOlacagiBirim = 0;
            kadro.hizmetSebebiKod = servisIKY.SebepKodBul(ENUMHizmetSebebiNevi.NAKILBASLAMA);
            kadro.ayrilmaGerekcesi = 0;
            kadro.atamaTarihi = PersonelBilgiOrtak.TnsTarih(istek.tarih);
            kadro.kadroDurum = ENUMKadroDurumu.YENI;
            kadro.ayrilmaNedeni = "";
            Sonuc s = servisIKY.PersonelKadroKaydet(kullanan, kadro);
            if (!s.islemSonuc)
                return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(s.hataStr, "Hizmet kaydı oluşturulamadı.") });

            TNS.IKY.PersonelKadro pk = new TNS.IKY.PersonelKadro();
            pk.kod = OrtakFonksiyonlar.ConvertToInt(s.anahtar, 0);
            pk.personelKod = kadro.personelKod;
            s = servisIKY.AtamaOnayBilgileriniGuncelle(kullanan, new TNSCollection(pk), 5);
            if (!s.islemSonuc)
                return Results.Json(new
                {
                    basarili = false,
                    listele = true,
                    hata = "Personel için hizmet kaydı oluşturuldu fakat onaylama işlemi yapılamadığından birime alma işlemi tamamlanamadı. Kişiyi bulup ilgili hizmet kaydını yetkili bir kullanıcıya onaylatmanız gerekmektedir. " + PersonelBilgiOrtak.HataMetni(s.hataStr, "")
                });
            return Results.Json(new { basarili = true, bilgi = "Personel birime alındı." });
        }
    }
}
