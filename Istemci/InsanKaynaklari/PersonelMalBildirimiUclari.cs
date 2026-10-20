using System.Collections.Generic;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.IKY;
using TNS.KYM;

namespace Istemci.InsanKaynaklari
{
    /// <summary>Mal bildirimi kaydet / sil isteği: alan adları PersonelMalBildirimi sınıfındaki gibi; tarihler "yyyy-MM-dd".</summary>
    public class MalBildirimiIstegi
    {
        public int kod { get; set; }
        public int personelKod { get; set; }
        public string tcKimlikNo { get; set; }
        public string tarih { get; set; }
        public string aciklama { get; set; }
        public List<MalBilKimlikSatiri> kimlikListesi { get; set; }
        public List<MalBilTasinmazSatiri> tasinmazListesi { get; set; }
        public List<MalBilKooperatifSatiri> kooperatifListesi { get; set; }
        public List<MalBilTasitSatiri> tasitListesi { get; set; }
        public List<MalBilDigerTasinirSatiri> digerTasinirListesi { get; set; }
        public List<MalBilBankaMenkulSatiri> bankaMenkulListesi { get; set; }
        public List<MalBilAltinMucevherSatiri> altinMucevherListesi { get; set; }
        public List<MalBilBorcAlacakSatiri> borcAlacakListesi { get; set; }
        public List<MalBilDigerServetSatiri> digerServetListesi { get; set; }
    }

    public class MalBilKimlikSatiri
    {
        public int siraNo { get; set; }
        public string adiSoyadi { get; set; }
        public string dogumTarihi { get; set; }
        public string dogumYeri { get; set; }
        public string yakinligi { get; set; }
        public string tcKimlikNo { get; set; }
    }

    public class MalBilTasinmazSatiri
    {
        public int siraNo { get; set; }
        public double degeri { get; set; }
        public string tasinmazCinsi { get; set; }
        public string adres { get; set; }
        public double hisseMiktari { get; set; }
        public string edinmeTarihi { get; set; }
        public string malikTCKN { get; set; }
    }

    public class MalBilKooperatifSatiri
    {
        public int siraNo { get; set; }
        public string kooperatifAdiYeri { get; set; }
        public double hisseDegeri { get; set; }
        public string uyelikTarihi { get; set; }
        public string tcKimlikNo { get; set; }
    }

    public class MalBilTasitSatiri
    {
        public int siraNo { get; set; }
        public string plakaNo { get; set; }
        public string tasitinCinsi { get; set; }
        public string tasitinMarkasi { get; set; }
        public double edinmeDegeri { get; set; }
        public int modelYili { get; set; }
        public string edinmeTarihi { get; set; }
        public string tcKimlikNo { get; set; }
    }

    public class MalBilDigerTasinirSatiri
    {
        public int siraNo { get; set; }
        public string tasinirMalCinsi { get; set; }
        public double edinmeDegeri { get; set; }
        public int modelYili { get; set; }
        public string edinmeTarihi { get; set; }
        public string tcKimlikNo { get; set; }
    }

    public class MalBilBankaMenkulSatiri
    {
        public int siraNo { get; set; }
        public string nitelik { get; set; }
        public double miktari { get; set; }
        public string cinsi { get; set; }
        public string tcKimlikNo { get; set; }
    }

    public class MalBilAltinMucevherSatiri
    {
        public int siraNo { get; set; }
        public string cinsi { get; set; }
        public double degeri { get; set; }
        public double miktari { get; set; }
        public string tcKimlikNo { get; set; }
    }

    public class MalBilBorcAlacakSatiri
    {
        public int siraNo { get; set; }
        public string borcluAdi { get; set; }
        public string alacakliAdi { get; set; }
        public double tutar { get; set; }
    }

    public class MalBilDigerServetSatiri
    {
        public int siraNo { get; set; }
        public string servetUnsuru { get; set; }
        public string edinmeSekli { get; set; }
        public string tcKimlikNo { get; set; }
    }

    /// <summary>
    /// Personel İşlemleri "Mal Bildirimi" sekmesi (eski InsanKaynaklari/PersonelMalBildirimi.aspx; yeni sayfa
    /// InsanKaynaklari/PersonelMalBildirimi) uçları:
    ///   GET  /api/personelbilgi/malbildirim/liste?personelKod=   (eski ListeleMalBildirimi: sağdaki bildirim listesi)
    ///   GET  /api/personelbilgi/malbildirim/oku?kod=             (eski OkuMalBildirimi: seçilen bildirimin 9 tablosu)
    ///   POST /api/personelbilgi/malbildirim/kaydet               (eski Kaydet düğmesi / kaydetMenkul / KaydetMealBildirimi)
    ///   POST /api/personelbilgi/malbildirim/sil                  (eski Sil düğmesi / btnSil_Click)
    /// İş kuralları eski sayfadaki gibi IIKYServis.PersonelMalBildirimiListele / Oku / Kaydet / Sil.
    /// </summary>
    public static class PersonelMalBildirimiUclari
    {
        private const string SayfaAd = "PersonelMalBildirimi";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/personelbilgi/malbildirim/liste", Liste);
            app.MapGet("/api/personelbilgi/malbildirim/oku", Oku);
            app.MapPost("/api/personelbilgi/malbildirim/kaydet", Kaydet);
            app.MapPost("/api/personelbilgi/malbildirim/sil", Sil);
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            List<object> satirlar = new List<object>();
            int pKod = personelKod ?? 0;
            if (pKod <= 0) return Results.Json(new { satirlar = satirlar });

            ObjectArray oa = TNS.IKY.Arac.Tanimla().PersonelMalBildirimiListele(kullanan, new PersonelMalBildirimi { personelKod = pKod });
            if (!oa.sonuc.islemSonuc)
                return Results.Json(new { satirlar = satirlar, hata = PersonelBilgiOrtak.HataMetni(oa.sonuc.hataStr, "Mal bildirimi bilgileri okunamadı.") });

            foreach (PersonelMalBildirimi d in oa.objeler)
                satirlar.Add(new { d.kod, tarih = PersonelBilgiOrtak.TarihYaz(d.tarih), d.aciklama });
            return Results.Json(new { satirlar = satirlar });
        }

        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int? kod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            if ((kod ?? 0) <= 0) return Results.Json(new { hata = "Lütfen mal bildirimini seçiniz." });

            ObjectVAN ov = TNS.IKY.Arac.Tanimla().PersonelMalBildirimiOku(kullanan, kod.Value);
            if (!ov.sonuc.islemSonuc)
                return Results.Json(new { hata = PersonelBilgiOrtak.HataMetni(ov.sonuc.hataStr, "Mal bildirimi okunamadı.") });
            if (!(ov.o is PersonelMalBildirimi d))
                return Results.Json(new { hata = "Mal bildirimi bulunamadı." });

            List<object> kimlik = new List<object>();
            foreach (MalBilKimlik x in d.kimlikListesi)
                kimlik.Add(new { x.siraNo, x.adiSoyadi, dogumTarihi = PersonelBilgiOrtak.TarihYaz(x.dogumTarihi), x.dogumYeri, x.yakinligi, x.tcKimlikNo });

            List<object> tasinmaz = new List<object>();
            foreach (MalBilTasinmaz x in d.tasinmazListesi)
                tasinmaz.Add(new { x.siraNo, x.degeri, x.tasinmazCinsi, x.adres, x.hisseMiktari, edinmeTarihi = PersonelBilgiOrtak.TarihYaz(x.edinmeTarihi), x.malikTCKN });

            List<object> kooperatif = new List<object>();
            foreach (MalBilKooperatif x in d.kooperatifListesi)
                kooperatif.Add(new { x.siraNo, x.kooperatifAdiYeri, x.hisseDegeri, uyelikTarihi = PersonelBilgiOrtak.TarihYaz(x.uyelikTarihi), x.tcKimlikNo });

            List<object> tasit = new List<object>();
            foreach (MalBilTasit x in d.tasitListesi)
                tasit.Add(new { x.siraNo, x.plakaNo, x.tasitinCinsi, x.tasitinMarkasi, x.edinmeDegeri, x.modelYili, edinmeTarihi = PersonelBilgiOrtak.TarihYaz(x.edinmeTarihi), x.tcKimlikNo });

            List<object> digerTasinir = new List<object>();
            foreach (MalBilDigerTasinir x in d.digerTasinirListesi)
                digerTasinir.Add(new { x.siraNo, x.tasinirMalCinsi, x.edinmeDegeri, x.modelYili, edinmeTarihi = PersonelBilgiOrtak.TarihYaz(x.edinmeTarihi), x.tcKimlikNo });

            List<object> bankaMenkul = new List<object>();
            foreach (MalBilBankaMenkul x in d.bankaMenkulListesi)
                bankaMenkul.Add(new { x.siraNo, x.nitelik, x.miktari, x.cinsi, x.tcKimlikNo });

            List<object> altinMucevher = new List<object>();
            foreach (MalBilAltinMucevher x in d.altinMucevherListesi)
                altinMucevher.Add(new { x.siraNo, x.cinsi, x.degeri, x.miktari, x.tcKimlikNo });

            List<object> borcAlacak = new List<object>();
            foreach (MalBilBorcAlacak x in d.borcAlacakListesi)
                borcAlacak.Add(new { x.siraNo, x.borcluAdi, x.alacakliAdi, x.tutar });

            List<object> digerServet = new List<object>();
            foreach (MalBilDigerServet x in d.digerServetListesi)
                digerServet.Add(new { x.siraNo, x.servetUnsuru, x.edinmeSekli, x.tcKimlikNo });

            return Results.Json(new
            {
                d.kod,
                d.personelKod,
                tarih = PersonelBilgiOrtak.TarihYaz(d.tarih),
                d.aciklama,
                kimlikListesi = kimlik,
                tasinmazListesi = tasinmaz,
                kooperatifListesi = kooperatif,
                tasitListesi = tasit,
                digerTasinirListesi = digerTasinir,
                bankaMenkulListesi = bankaMenkul,
                altinMucevherListesi = altinMucevher,
                borcAlacakListesi = borcAlacak,
                digerServetListesi = digerServet
            });
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, MalBildirimiIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            // Eski btnKaydet_Click denetimleri (FRMPAB002, FRMPMB010, FRMPMB011).
            string hata = "";
            if (istek.personelKod <= 0)
                hata += "Personel Bilgisi boş bırakılamaz.\n";
            if (PersonelBilgiOrtak.TarihOku(istek.tarih).Year < 1900)
                hata += "Mal Bildirim Tarihi boş bırakılamaz.\n";
            if ((istek.aciklama ?? "").Trim() == "")
                hata += "Açıklama alanı boş bırakılamaz.\n";
            if (hata != "") return Results.Json(new { basarili = false, hata = hata.Trim() });

            PersonelMalBildirimi d = new PersonelMalBildirimi();
            d.kod = istek.kod;
            d.personelKod = istek.personelKod;
            d.tcKimlikNo = istek.tcKimlikNo ?? "";
            d.tarih = PersonelBilgiOrtak.TnsTarih(istek.tarih);
            d.aciklama = istek.aciklama ?? "";

            if (istek.kimlikListesi != null)
                foreach (MalBilKimlikSatiri x in istek.kimlikListesi)
                    if (x != null)
                        d.kimlikListesi.Add(new MalBilKimlik { siraNo = x.siraNo, adiSoyadi = x.adiSoyadi ?? "", dogumTarihi = PersonelBilgiOrtak.TnsTarih(x.dogumTarihi), dogumYeri = x.dogumYeri ?? "", yakinligi = x.yakinligi ?? "", tcKimlikNo = x.tcKimlikNo ?? "" });
            if (istek.tasinmazListesi != null)
                foreach (MalBilTasinmazSatiri x in istek.tasinmazListesi)
                    if (x != null)
                        d.tasinmazListesi.Add(new MalBilTasinmaz { siraNo = x.siraNo, degeri = x.degeri, tasinmazCinsi = x.tasinmazCinsi ?? "", adres = x.adres ?? "", hisseMiktari = x.hisseMiktari, edinmeTarihi = PersonelBilgiOrtak.TnsTarih(x.edinmeTarihi), malikTCKN = x.malikTCKN ?? "" });
            if (istek.kooperatifListesi != null)
                foreach (MalBilKooperatifSatiri x in istek.kooperatifListesi)
                    if (x != null)
                        d.kooperatifListesi.Add(new MalBilKooperatif { siraNo = x.siraNo, kooperatifAdiYeri = x.kooperatifAdiYeri ?? "", hisseDegeri = x.hisseDegeri, uyelikTarihi = PersonelBilgiOrtak.TnsTarih(x.uyelikTarihi), tcKimlikNo = x.tcKimlikNo ?? "" });
            if (istek.tasitListesi != null)
                foreach (MalBilTasitSatiri x in istek.tasitListesi)
                    if (x != null)
                        d.tasitListesi.Add(new MalBilTasit { siraNo = x.siraNo, plakaNo = x.plakaNo ?? "", tasitinCinsi = x.tasitinCinsi ?? "", tasitinMarkasi = x.tasitinMarkasi ?? "", edinmeDegeri = x.edinmeDegeri, modelYili = x.modelYili, edinmeTarihi = PersonelBilgiOrtak.TnsTarih(x.edinmeTarihi), tcKimlikNo = x.tcKimlikNo ?? "" });
            if (istek.digerTasinirListesi != null)
                foreach (MalBilDigerTasinirSatiri x in istek.digerTasinirListesi)
                    if (x != null)
                        d.digerTasinirListesi.Add(new MalBilDigerTasinir { siraNo = x.siraNo, tasinirMalCinsi = x.tasinirMalCinsi ?? "", edinmeDegeri = x.edinmeDegeri, modelYili = x.modelYili, edinmeTarihi = PersonelBilgiOrtak.TnsTarih(x.edinmeTarihi), tcKimlikNo = x.tcKimlikNo ?? "" });
            if (istek.bankaMenkulListesi != null)
                foreach (MalBilBankaMenkulSatiri x in istek.bankaMenkulListesi)
                    if (x != null)
                        d.bankaMenkulListesi.Add(new MalBilBankaMenkul { siraNo = x.siraNo, nitelik = x.nitelik ?? "", miktari = x.miktari, cinsi = x.cinsi ?? "", tcKimlikNo = x.tcKimlikNo ?? "" });
            if (istek.altinMucevherListesi != null)
                foreach (MalBilAltinMucevherSatiri x in istek.altinMucevherListesi)
                    if (x != null)
                        d.altinMucevherListesi.Add(new MalBilAltinMucevher { siraNo = x.siraNo, cinsi = x.cinsi ?? "", degeri = x.degeri, miktari = x.miktari, tcKimlikNo = x.tcKimlikNo ?? "" });
            if (istek.borcAlacakListesi != null)
                foreach (MalBilBorcAlacakSatiri x in istek.borcAlacakListesi)
                    if (x != null)
                        d.borcAlacakListesi.Add(new MalBilBorcAlacak { siraNo = x.siraNo, borcluAdi = x.borcluAdi ?? "", alacakliAdi = x.alacakliAdi ?? "", tutar = x.tutar });
            if (istek.digerServetListesi != null)
                foreach (MalBilDigerServetSatiri x in istek.digerServetListesi)
                    if (x != null)
                        d.digerServetListesi.Add(new MalBilDigerServet { siraNo = x.siraNo, servetUnsuru = x.servetUnsuru ?? "", edinmeSekli = x.edinmeSekli ?? "", tcKimlikNo = x.tcKimlikNo ?? "" });

            Sonuc sonuc = TNS.IKY.Arac.Tanimla().PersonelMalBildirimiKaydet(kullanan, d);
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Mal bildirimi kaydedilemedi.") });
            return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
        }

        private static async Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, MalBildirimiIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            // Eski btnSil_Click (FRMPMB014 / FRMPMB013).
            if (istek.kod <= 0) return Results.Json(new { basarili = false, hata = "Lütfen silmek istediğiniz kaydı seçiniz." });

            PersonelMalBildirimi d = new PersonelMalBildirimi();
            d.kod = istek.kod;
            d.personelKod = istek.personelKod;
            d.tcKimlikNo = istek.tcKimlikNo ?? ""; // iş kuralı (IKYPersonel.SilMalBildirimi) T.C. kimlik no'yu boş kabul etmez
            Sonuc sonuc = TNS.IKY.Arac.Tanimla().PersonelMalBildirimiSil(kullanan, d);
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Mal bildirimi silinemedi.") });
            return Results.Json(new { basarili = true, bilgi = "Silme işlemi başarıyla gerçekleşti." });
        }
    }
}
