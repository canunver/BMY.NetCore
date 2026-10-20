using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.MUH;

namespace Istemci.Muhasebe
{
    public class YilSonuBelgeIstegi
    {
        /// <summary>Sayfadaki yıl (eski spnYil); belge bu yıla üretilir.</summary>
        public int Yil { get; set; }
        public int Muhasebe { get; set; }
        public string Birim { get; set; }
        /// <summary>Satırdaki belge no (tabloda elle yazılabilir; boşsa yeni numara alınır).</summary>
        public string BelgeNo { get; set; }
        /// <summary>Yıl sonu işlem türü (MUHOdemeEmriMIFServis.YilSonuBelgeUret: 1..24).</summary>
        public int Tur { get; set; }
        /// <summary>Belge tarihi (yyyy-MM-dd).</summary>
        public string BelgeTarih { get; set; }
    }

    /// <summary>
    /// Yıl Sonu Kapatma İşlemleri (eski ButceMuhasebe/YilSonuIslemleri.aspx) uçları.
    ///   GET  /api/yilsonu/tanim    yıllar, varsayılan yıl (geçen yıl, eski sayfadaki gibi)
    ///   GET  /api/yilsonu/liste    ?yil&amp;muhasebe&amp;birim : eski Listele (servisMUH.YilSonuIslemleriListele) → satirlar {YIL, AD, BELGENO, TUR}
    ///   POST /api/yilsonu/olustur  {Yil, Muhasebe, Birim, BelgeNo, Tur, BelgeTarih}: eski BelgeOlustur islem "1" (belge kaydı + yevmiye no)
    ///   GET  /api/yilsonu/rapor    aynı bilgiler sorgu olarak: eski BelgeOlustur islem "2" (üretilecek belgenin satırları Excel'e, DosyaYaz)
    /// Yetki: eski sayfa gibi TNS.MUH.Yetki.YetkisiVarMi(Gorme, ODEMEEMRI).
    /// </summary>
    public static class YilSonuIslemleriUclari
    {
        public const string SayfaAdresi = "../ButceMuhasebe/YilSonuIslemleri.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/yilsonu/tanim", Tanim);
            app.MapGet("/api/yilsonu/liste", Liste);
            app.MapPost("/api/yilsonu/olustur", Olustur);
            app.MapGet("/api/yilsonu/rapor", Rapor);
        }

        public static bool YetkiVar(Kullanici kullanan)
        {
            if (kullanan == null) return false;
            try
            {
                return TNS.MUH.Yetki.YetkisiVarMi(kullanan, (int)TNS.MUH.ENUMOGY.Gorme, (int)ENUMBelgeTuru.ODEMEEMRI);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static IResult Denetle(HttpContext baglam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Hata(string hata)
        {
            return Results.Json(new { basarili = false, hata = hata });
        }

        private static IResult Tanim(HttpContext baglam)
        {
            IResult ret = Denetle(baglam, out Kullanici kullanan);
            if (ret != null) return ret;
            List<int> yillar = new List<int>();
            for (int i = 2005; i <= DateTime.Now.Year + 1; i++) yillar.Add(i);
            return Results.Json(new { yil = DateTime.Now.Year - 1, yillar = yillar });
        }

        /// <summary>Eski Listele().</summary>
        private static IResult Liste(HttpContext baglam, int? yil, int? muhasebe, string birim)
        {
            IResult ret = Denetle(baglam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                YilSonuIslem yilSonuIslem = new YilSonuIslem();
                yilSonuIslem.yil = yil > 0 ? yil.Value : DateTime.Now.Year - 1;
                yilSonuIslem.muhasebe = muhasebe ?? 0;
                yilSonuIslem.birim = (birim ?? "").Trim();
                ObjectArray liste = TNS.MUH.Arac.Tanimla().YilSonuIslemleriListele(kullanan, yilSonuIslem);

                List<object> satirlar = new List<object>();
                if (liste != null)
                {
                    foreach (YilSonuIslem y in liste.objeler)
                        satirlar.Add(new { YIL = y.yil, AD = y.ad, BELGENO = y.belgeNo, TUR = y.tur });
                    if (!liste.sonuc.islemSonuc && satirlar.Count == 0 && !string.IsNullOrWhiteSpace(liste.sonuc.hataStr))
                        return Hata(liste.sonuc.hataStr);
                }
                return Results.Json(new { basarili = true, satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
        }

        /// <summary>Eski BelgeOlustur içinde servise giden form.</summary>
        private static OdemeEmriMIFForm FormHazirla(YilSonuBelgeIstegi istek)
        {
            OdemeEmriMIFForm form = new OdemeEmriMIFForm();
            form.yil = istek.Yil;
            form.belgeNo = (istek.BelgeNo ?? "").Trim();
            form.aciklama = "";
            form.muhasebe = istek.Muhasebe;
            form.birim = (istek.Birim ?? "").Trim();
            form.kurKod = TNS.DEG.Arac.VarsayilanKurumBul();
            form.organizasyonKodu = 0;
            form.islemTarih = new TNSDateTime(TarihMetni(istek.BelgeTarih));
            form.fisTurYap(ENUMFISTUR.KAPANIS);
            return form;
        }

        /// <summary>yyyy-MM-dd → eski DateField metni (dd.MM.yyyy); boşsa boş (eski sayfada tarih boş bırakılınca olduğu gibi).</summary>
        private static string TarihMetni(string s)
        {
            s = (s ?? "").Trim();
            if (s == "") return "";
            if (DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d))
                return d.ToString("dd.MM.yyyy");
            return s;
        }

        private static string Denetim(YilSonuBelgeIstegi istek)
        {
            if (istek == null) return "Bilgi yok.";
            if (istek.Yil <= 0) return "Yıl bilgisi boş bırakılmış.";
            if (istek.Tur <= 0) return "İşlem türü okunamadı.";
            return null;
        }

        /// <summary>Eski BelgeOlustur(..., islem "1"): belgeyi kaydeder, gerekiyorsa yevmiye numarası verir.</summary>
        private static async Task<IResult> Olustur(HttpContext baglam, YilSonuBelgeIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, out Kullanici kullanan);
            if (ret != null) return ret;
            string eksik = Denetim(istek);
            if (eksik != null) return Hata(eksik);

            try
            {
                IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
                OdemeEmriMIFForm form = FormHazirla(istek);
                int tur = istek.Tur;
                ObjectArray sonuc = servisMUH.YilSonuBelgeUret(kullanan, form, tur, "1");
                if (!sonuc.sonuc.islemSonuc)
                    return Hata(string.IsNullOrWhiteSpace(sonuc.sonuc.hataStr) ? "Belge oluşturulamadı." : sonuc.sonuc.hataStr);

                string yevmiyeHata = null;
                if (tur != 18 && tur != 19 && !TNS.HRC.Arac.CumhurbaskanligiMi()) // Cumhurbaşkanlığı yevmiyeli olmasını istemiyor
                {
                    // YEVMİYE NO VER
                    form.belgeNo = sonuc.sonuc.anahtar;
                    form.formDurum = (int)ENUMBelgeDurumu.ICMALYEVMIYENOVER;
                    form.kurum = GenelIslemlerIstemci.VarsayilanKurumBul();

                    if (form.belgeNo != "")
                    {
                        TNSCollection fler = new TNSCollection();
                        fler.Add(form);
                        Sonuc ys = servisMUH.BelgeDurumDegistirODEMIF(kullanan, fler, true);
                        // Eski sayfa bu sonuca bakmıyordu; yalnızca bilgi olarak gösterilir.
                        if (ys != null && !ys.islemSonuc && !string.IsNullOrWhiteSpace(ys.hataStr)) yevmiyeHata = ys.hataStr;
                    }
                }
                return Results.Json(new { basarili = true, bilgi = sonuc.sonuc.bilgiStr, belgeNo = sonuc.sonuc.anahtar, yevmiyeHata = yevmiyeHata });
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
        }

        /// <summary>Eski BelgeOlustur(..., islem "2"): üretilecek belgenin satırları Excel'e yazılır (kayıt yapılmaz).</summary>
        private static IResult Rapor(HttpContext baglam, int? yil, int? muhasebe, string birim, string belgeNo, int? tur, string belgeTarih)
        {
            IResult ret = Denetle(baglam, out Kullanici kullanan);
            if (ret != null) return ret;
            YilSonuBelgeIstegi istek = new YilSonuBelgeIstegi
            {
                Yil = yil ?? 0, Muhasebe = muhasebe ?? 0, Birim = birim, BelgeNo = belgeNo, Tur = tur ?? 0, BelgeTarih = belgeTarih
            };
            string eksik = Denetim(istek);
            if (eksik != null) return Hata(eksik);

            try
            {
                ObjectArray sonuc = TNS.MUH.Arac.Tanimla().YilSonuBelgeUret(kullanan, FormHazirla(istek), istek.Tur, "2");
                if (sonuc.ObjeSayisi == 1 && sonuc.objeler[0].GetType() == typeof(OdemeEmriMIFForm))
                {
                    AsposeLisans.Yukle();
                    string dosya = DosyayaYaz((OdemeEmriMIFForm)sonuc.objeler[0]);
                    return DosyaIslem.DosyaGonder(dosya, Path.GetFileName(dosya), true, ".xlsx");
                }
                return Hata(string.IsNullOrWhiteSpace(sonuc.sonuc.hataStr) ? "Rapor hazırlanamadı." : sonuc.sonuc.hataStr);
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
        }

        /// <summary>Eski DosyayaYaz: belge satırları boş bir Excel dosyasına.</summary>
        private static string DosyayaYaz(OdemeEmriMIFForm belge)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            XLS.BosDosyaAc(DosyaIslem.DosyaAdUret() + ".xlsx");

            int sat = 0;
            XLS.HucreDegerYaz(sat, 0, "Hesap Kod");
            XLS.HucreDegerYaz(sat, 1, "Alt HesapKod");
            XLS.HucreDegerYaz(sat, 2, "Tertip");
            XLS.HucreDegerYaz(sat, 3, "Borç");
            XLS.HucreDegerYaz(sat, 4, "Alacak");
            XLS.HucreDegerYaz(sat, 5, "Döviz Borç");
            XLS.HucreDegerYaz(sat, 6, "Döviz Alacak");
            XLS.HucreDegerYaz(sat, 7, "Birim");
            XLS.HucreDegerYaz(sat++, 8, "KurKod");

            for (int i = 0; i < belge.detay.ObjeSayisi; i++)
            {
                OdemeEmriMIFDetay d = (OdemeEmriMIFDetay)belge.detay[i];

                XLS.HucreDegerYaz(sat, 0, d.hesapKod);
                XLS.HucreDegerYaz(sat, 1, d.altHesapKod);
                XLS.HucreDegerYaz(sat, 2, d.TertipStr());
                XLS.HucreDegerYaz(sat, 3, d.borc);
                XLS.HucreDegerYaz(sat, 4, d.alacak);
                XLS.HucreDegerYaz(sat, 5, d.dovizBorc);
                XLS.HucreDegerYaz(sat, 6, d.dovizAlacak);
                XLS.HucreDegerYaz(sat, 7, d.detayBirim);
                XLS.HucreDegerYaz(sat++, 8, d.kur);
            }
            XLS.DosyaSaklaTamYol();
            XLS.DosyaKapat();
            return XLS.SonucDosyaAd();
        }
    }
}
