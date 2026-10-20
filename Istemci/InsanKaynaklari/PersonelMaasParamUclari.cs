using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.IKY;
using TNS.KYM;
using TNS.Raporlama;

namespace Istemci.InsanKaynaklari
{
    /// <summary>Sendika Bilgisi Kaydet isteği.</summary>
    public class PersonelSendikaIstegi
    {
        public string tcKimlikNo { get; set; }
        public int sendikaNo { get; set; }
        public string kesintiOrani { get; set; }
    }

    /// <summary>Gelirler (değişken değerleri) satırı: alan adları MaasParam sınıfındaki gibi.</summary>
    public class PersonelMaasParamSatiri
    {
        public string degiskenAd { get; set; }
        public string aciklama { get; set; }
        public string degerGenel { get; set; }
    }

    /// <summary>Değ. Değerleri Kaydet isteği.</summary>
    public class PersonelMaasParamIstegi
    {
        public string tcKimlikNo { get; set; }
        public List<PersonelMaasParamSatiri> satirlar { get; set; }
    }

    /// <summary>Matrah satırı: alan adları PersonelMaasMatrah sınıfındaki gibi (tarihDT "yyyy-MM-dd").</summary>
    public class PersonelMaasMatrahSatiri
    {
        public string surecNo { get; set; }
        public string matrahTurAd { get; set; }
        public string tarihDT { get; set; }
        public double deger { get; set; }
    }

    /// <summary>Matrahlar Kaydet isteği.</summary>
    public class PersonelMaasMatrahIstegi
    {
        public string tcKimlikNo { get; set; }
        public int yil { get; set; }
        public List<PersonelMaasMatrahSatiri> satirlar { get; set; }
    }

    /// <summary>
    /// Personel İşlemleri "Maaş Parametreleri" sekmesi (eski InsanKaynaklari/PersonelMaasParam.aspx; yeni sayfa InsanKaynaklari/PersonelMaasParam) uçları.
    /// Eski sayfa gibi personel, adres satırındaki tcKimlikNo ile belirlenir.
    ///   GET  /api/personelbilgi/maasparam/tanim?tcKimlikNo=          Matrah türleri, sendikalar, kişinin sendika bilgisi (eski Page_Load, SendikaBilgiOku)
    ///   GET  /api/personelbilgi/maasparam/liste?tcKimlikNo=&amp;yil=     Gelirler ve matrahlar (eski Listele / btnListele_Click)
    ///   POST /api/personelbilgi/maasparam/sendika-kaydet             (eski btnSendikaKaydet_Click)
    ///   POST /api/personelbilgi/maasparam/gelir-kaydet               (eski btnKaydetGelirler_Click)
    ///   POST /api/personelbilgi/maasparam/matrah-kaydet              (eski btnKaydetMatrah_Click)
    ///   GET  /api/personelbilgi/maasparam/bordro?tcKimlikNo=          Bordro Yazdır (eski btnYazdir_Click: TekRaporAl(20000, 1))
    /// </summary>
    public static class PersonelMaasParamUclari
    {
        private const string SayfaAd = "PersonelMaasParam";
        private const string BosSurec = "000000000";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/personelbilgi/maasparam/tanim", Tanim);
            app.MapGet("/api/personelbilgi/maasparam/liste", Liste);
            app.MapPost("/api/personelbilgi/maasparam/sendika-kaydet", SendikaKaydet);
            app.MapPost("/api/personelbilgi/maasparam/gelir-kaydet", GelirKaydet);
            app.MapPost("/api/personelbilgi/maasparam/matrah-kaydet", MatrahKaydet);
            app.MapGet("/api/personelbilgi/maasparam/bordro", Bordro);
        }

        private static IIKYServis Servis()
        {
            return TNS.IKY.Arac.Tanimla();
        }

        private static IResult Hata(string metin)
        {
            return Results.Json(new { basarili = false, hata = metin });
        }

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam, string tcKimlikNo)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            string tc = (tcKimlikNo ?? "").Trim();

            // Matrah türleri (eski stoMatrahTur: PersonelMaasMatrah.MatrahTurAd(1..n)).
            List<string> matrahTurleri = new List<string>();
            for (int i = 1; i <= PersonelMaasMatrah.matrahTurAdlar.Length; i++)
                matrahTurleri.Add(PersonelMaasMatrah.MatrahTurAd(i));

            // Sendikalar: önbellek boşaltılıp yeniden okunur; sendika yoksa sendika bölümü gizlenir.
            List<IntKodStrAd> l = new List<IntKodStrAd>();
            string hata = "";
            int sendikaNo = 0;
            string kesintiOrani = "";
            try
            {
                TNS.IKY.Arac.SendikaListesi = null;
                Hashtable ht = TNS.IKY.Arac.SendikaListesi;
                foreach (SendikaBilgi item in ht.Values)
                    l.Add(new IntKodStrAd(item.sendikaNo, item.sendikaAd));
                if (l.Count > 0)
                {
                    l.Add(new IntKodStrAd(0, "-"));
                    l.Sort();
                    // Eski SendikaBilgiOku
                    ObjectVAN ov = Servis().OkuKisiSendikaBilgi(kullanan, tc);
                    if (ov.o == null) ov.o = new SendikaBilgi();
                    SendikaBilgi sb = (SendikaBilgi)ov.o;
                    sendikaNo = sb.sendikaNo;
                    kesintiOrani = sb.kesintiOrani.ToString("0.000");
                }
            }
            catch (Exception ex)
            {
                hata = PersonelBilgiOrtak.HataMetni(ex.Message, "Sendika bilgileri okunamadı.");
            }

            List<object> sendikalar = new List<object>();
            foreach (IntKodStrAd s in l) sendikalar.Add(new { s.kod, s.ad });
            return Results.Json(new
            {
                yil = DateTime.Now.Year,
                matrahTurleri = matrahTurleri,
                sendikaVar = l.Count > 0,
                sendikalar = sendikalar,
                sendikaNo = sendikaNo,
                kesintiOrani = kesintiOrani,
                hata = hata
            });
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, string tcKimlikNo, int? yil)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            List<object> paramlar = new List<object>();
            List<object> matrahlar = new List<object>();
            try
            {
                TNS.IKY.PersonelMaasParam maasParam = Servis().MaasParamListele(kullanan, (tcKimlikNo ?? "").Trim(), yil ?? DateTime.Now.Year);
                if (maasParam != null && maasParam.paramlar != null)
                    foreach (MaasParam p in maasParam.paramlar)
                        paramlar.Add(new { degiskenAd = p.degiskenAd ?? "", aciklama = p.aciklama ?? "", degerGenel = p.degerGenel ?? "" });
                if (maasParam != null && maasParam.matrahlar != null)
                    foreach (PersonelMaasMatrah m in maasParam.matrahlar)
                        matrahlar.Add(new { surecNo = m.surecNo ?? "", matrahTurAd = m.matrahTurAd ?? "", tarihDT = PersonelBilgiOrtak.TarihYaz(m.tarihDT), deger = m.deger });
            }
            catch (Exception ex)
            {
                return Results.Json(new { paramlar = paramlar, matrahlar = matrahlar, hata = PersonelBilgiOrtak.HataMetni(ex.Message, "Maaş parametreleri okunamadı.") });
            }
            return Results.Json(new { paramlar = paramlar, matrahlar = matrahlar });
        }

        private static async Task<IResult> SendikaKaydet(HttpContext baglam, IWebHostEnvironment ortam, PersonelSendikaIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            TNSCollection kisiler = new TNSCollection();
            ObjectArray oa = Servis().PersonelListele(kullanan, new TNS.IKY.Personel() { tcKimlikNo = (istek.tcKimlikNo ?? "").Trim() }, false, true);
            Sonuc s;
            if (oa.sonuc.islemSonuc && oa.ObjeSayisi > 0)
            {
                kisiler.Add(((TNS.IKY.Personel)oa.objeler[0]).kod);
                s = Servis().PersonelSendikayaEkle(kullanan, istek.sendikaNo, kisiler, OrtakFonksiyonlar.ConvertToDouble(istek.kesintiOrani ?? ""), false);
            }
            else
            {
                if (oa.sonuc.islemSonuc)
                    oa.sonuc.DegerAta("Aranan personel bulunamadı!");
                s = oa.sonuc;
            }
            if (s.islemSonuc) return Results.Json(new { basarili = true, bilgi = "Sendika bilgisi kaydedildi" });
            return Hata("Sendika bilgisi kaydedilenedi: " + PersonelBilgiOrtak.Duz(s.hataStr));
        }

        private static async Task<IResult> GelirKaydet(HttpContext baglam, IWebHostEnvironment ortam, PersonelMaasParamIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            TNSCollection kisiler = new TNSCollection();
            TNS.IKY.Personel per = new TNS.IKY.Personel();
            per.tcKimlikNo = (istek.tcKimlikNo ?? "").Trim();
            per.personelMaasParam = new TNS.IKY.PersonelMaasParam();
            per.personelMaasParam.paramlar = new TNSCollection();
            // Eski: tablodaki bütün satırlar MaasParam dizisine çevrilir (degerGenel atanınca degerDbl / degerStr dolar).
            if (istek.satirlar != null)
                foreach (PersonelMaasParamSatiri satir in istek.satirlar)
                {
                    MaasParam mp = new MaasParam();
                    mp.degiskenAd = satir.degiskenAd ?? "";
                    mp.aciklama = satir.aciklama ?? "";
                    mp.degerGenel = satir.degerGenel ?? "";
                    per.personelMaasParam.paramlar.Add(mp);
                }
            kisiler.Add(per);

            Sonuc s = Servis().MaasParamKaydet(kullanan, kisiler, null);
            if (s.islemSonuc) return Results.Json(new { basarili = true, bilgi = "Kayıt başarılı" });
            return Hata(PersonelBilgiOrtak.HataMetni(s.hataStr, "Kayıt yapılamadı."));
        }

        private static async Task<IResult> MatrahKaydet(HttpContext baglam, IWebHostEnvironment ortam, PersonelMaasMatrahIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            string tc = (istek.tcKimlikNo ?? "").Trim();
            // Eski: yalnızca süreci olmayan (elle girilen, "000000000") satırlar kaydedilir.
            TNSCollection matrahlar = new TNSCollection();
            if (istek.satirlar != null)
                foreach (PersonelMaasMatrahSatiri satir in istek.satirlar)
                {
                    string surecNo = satir.surecNo ?? "";
                    if (surecNo != "" && surecNo != BosSurec) continue;
                    PersonelMaasMatrah item = new PersonelMaasMatrah();
                    item.matrahTurAd = satir.matrahTurAd ?? "";
                    item.tarihDT = PersonelBilgiOrtak.TarihOku(satir.tarihDT);
                    item.deger = satir.deger;
                    item.yil = istek.yil;
                    item.surecNo = BosSurec;
                    item.tcKimlikNo = tc;
                    matrahlar.Add(item);
                }

            Sonuc s = Servis().MaasMatrahKaydet(kullanan, istek.yil, BosSurec, "", matrahlar);
            if (s.islemSonuc) return Results.Json(new { basarili = true, bilgi = "Kayıt başarılı" });
            return Hata(PersonelBilgiOrtak.HataMetni(s.hataStr, "Kayıt yapılamadı."));
        }

        /// <summary>
        /// Bordro Yazdır: eski TekRaporAl(20000, 1, kullanan, null, {tcKimlikNo}) ile aynı (Rapor.xml grup 20000, rapor 1:
        /// HarcamaSurecleri.BordroRaporlari, EKBILGI "Liste", şablon HRC\BordroKisiListe.xltx). Sınıf Core'a taşınmadıysa
        /// RaporlamaServis "henüz taşınmadı" iletisini verir; o ileti gösterilir.
        /// </summary>
        private static IResult Bordro(HttpContext baglam, IWebHostEnvironment ortam, string tcKimlikNo)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                AsposeLisans.Yukle();
                Hashtable raporParam = new Hashtable();
                raporParam["tcKimlikNo"] = (tcKimlikNo ?? "").Trim();
                string dosya = RaporlamaServis.TekRaporAl(20000, 1, kullanan, null, raporParam, null);
                if (string.IsNullOrEmpty(dosya)) return Results.Json(new { hata = "Rapor hazırlanamadı." });
                if (RaporlamaServis.MesajMi(dosya)) return Results.Json(new { hata = PersonelBilgiOrtak.HataMetni(dosya, "Rapor hazırlanamadı.") });
                if (!File.Exists(dosya)) return Results.Json(new { hata = "Rapor hazırlanamadı." });
                string uzanti = Path.GetExtension(dosya).TrimStart('.').ToLowerInvariant();
                return DosyaIslem.DosyaGonder(dosya, "Bordro_" + DosyaIslem.DosyaAdUretSade(), true, uzanti);
            }
            catch (RaporException ex)
            {
                return Results.Json(new { hata = PersonelBilgiOrtak.HataMetni(ex.Message, "Rapor hazırlanamadı.") });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Rapor hazırlanamadı." });
            }
        }
    }
}
