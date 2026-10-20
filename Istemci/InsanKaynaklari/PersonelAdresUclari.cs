using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.IKY;
using TNS.KYM;

namespace Istemci.InsanKaynaklari
{
    /// <summary>Kaydet isteği: alan adları PersonelIletisim sınıfındaki gibi.</summary>
    public class PersonelAdresIstegi
    {
        public int personelKod { get; set; }
        public string adres { get; set; }
        public string isTelefon { get; set; }
        public string evTelefon { get; set; }
        public string cepTelefon { get; set; }
        public string faks { get; set; }
        public string eposta { get; set; }
        public string eposta2 { get; set; }
        public string url { get; set; }
        public string cepTelefon2 { get; set; }
        public string tarabyaTel { get; set; }
        public string lojmanTel { get; set; }
        public string telsiz { get; set; }
        public string goruntulu { get; set; }
        public bool rehberdeGozuksun { get; set; }
    }

    /// <summary>
    /// Personel İşlemleri "İletişim/Adres" sekmesi (eski InsanKaynaklari/PersonelAdres.aspx; yeni sayfa InsanKaynaklari/PersonelAdres) uçları:
    ///   GET  /api/personelbilgi/adres/oku?personelKod=     (eski Page_Load + Listele)
    ///   POST /api/personelbilgi/adres/kaydet               (eski btnKaydet_Click)
    ///   GET  /api/personelbilgi/adres/getir?personelKod=   Adres Bilgisini Getir: KPS ikametgah adresi (eski btnAdres_Click + Genel.AdresBilgisiGetir)
    /// </summary>
    public static class PersonelAdresUclari
    {
        private const string SayfaAd = "PersonelAdres";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/personelbilgi/adres/oku", Oku);
            app.MapPost("/api/personelbilgi/adres/kaydet", Kaydet);
            app.MapGet("/api/personelbilgi/adres/getir", AdresGetir);
        }

        private static IIKYServis Servis()
        {
            return TNS.IKY.Arac.Tanimla();
        }

        private static IResult Hata(string metin)
        {
            return Results.Json(new { basarili = false, hata = metin });
        }

        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            // Eski Page_Load: Manas kullanılıyorsa Telefon (Tarabya) ve Görüntülü alanları gizlenir.
            bool manas = false;
            try { manas = TNS.DEG.Arac.ManasKullaniyor(); } catch (Exception) { }

            int kod = personelKod ?? 0;
            if (kod == 0) return Results.Json(new { manas = manas, bilgi = (object)null });
            try
            {
                PersonelIletisim iletisim = Servis().PersonelAdresListele(kullanan, kod);
                if (iletisim == null) return Results.Json(new { manas = manas, hata = "İletişim bilgileri okunamadı." });
                if (iletisim.sonuc != null && !iletisim.sonuc.islemSonuc)
                    return Results.Json(new { manas = manas, hata = PersonelBilgiOrtak.HataMetni(iletisim.sonuc.hataStr, "İletişim bilgileri okunamadı.") });
                return Results.Json(new
                {
                    manas = manas,
                    bilgi = new
                    {
                        adres = iletisim.adres ?? "",
                        isTelefon = iletisim.isTelefon ?? "",
                        evTelefon = iletisim.evTelefon ?? "",
                        cepTelefon = iletisim.cepTelefon ?? "",
                        faks = iletisim.faks ?? "",
                        mobilImzaTelefon = iletisim.mobilImzaTelefon ?? "",
                        eposta = (iletisim.eposta ?? "").Trim(),
                        eposta2 = (iletisim.eposta2 ?? "").Trim(),
                        cepTelefon2 = iletisim.cepTelefon2 ?? "",
                        tarabyaTel = iletisim.tarabyaTel ?? "",
                        lojmanTel = iletisim.lojmanTel ?? "",
                        telsiz = iletisim.telsiz ?? "",
                        goruntulu = iletisim.goruntulu ?? "",
                        rehberdeGozuksun = iletisim.rehberdeGozuksun == 1,
                        url = iletisim.url ?? ""
                    }
                });
            }
            catch (Exception ex)
            {
                return Results.Json(new { manas = manas, hata = PersonelBilgiOrtak.HataMetni(ex.Message, "İletişim bilgileri okunamadı.") });
            }
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, PersonelAdresIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            if (istek.personelKod == 0) return Hata("Personel Bilgisi boş bırakılamaz.");

            PersonelIletisim iletisim = new PersonelIletisim();
            iletisim.kod = istek.personelKod;
            iletisim.adres = istek.adres ?? "";
            iletisim.isTelefon = istek.isTelefon ?? "";
            iletisim.evTelefon = istek.evTelefon ?? "";
            iletisim.cepTelefon = istek.cepTelefon ?? "";
            iletisim.faks = istek.faks ?? "";
            iletisim.eposta = istek.eposta ?? "";
            iletisim.eposta2 = istek.eposta2 ?? "";
            iletisim.url = istek.url ?? "";
            iletisim.cepTelefon2 = istek.cepTelefon2 ?? "";
            iletisim.tarabyaTel = istek.tarabyaTel ?? "";
            iletisim.lojmanTel = istek.lojmanTel ?? "";
            iletisim.telsiz = istek.telsiz ?? "";
            iletisim.goruntulu = istek.goruntulu ?? "";
            if (istek.rehberdeGozuksun)
                iletisim.rehberdeGozuksun = 1;

            Sonuc sonuc = Servis().PersonelAdresKaydet(kullanan, iletisim);
            if (!sonuc.islemSonuc) return Hata(PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "İletişim bilgileri kaydedilemedi."));
            return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
        }

        /// <summary>Eski TcKimlikGetir: personelin T.C. kimlik numarası.</summary>
        private static string TcKimlikGetir(Kullanici kullanan, int kod)
        {
            TNS.IKY.Personel kriter = new TNS.IKY.Personel();
            kriter.kod = kod;
            string kimlikNo = string.Empty;
            ObjectArray pler = Servis().PersonelListele(kullanan, kriter, false, true);
            if (pler != null && pler.objeler != null)
                foreach (TNS.IKY.Personel p in pler.objeler)
                    kimlikNo = p.tcKimlikNo;
            return kimlikNo ?? "";
        }

        private static IResult AdresGetir(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            string tcKimlikNo = TcKimlikGetir(kullanan, personelKod ?? 0);
            if (tcKimlikNo == "") return Hata("Personel T.C.Kimlik No bilgisi boş bırakılamaz.");

            try
            {
                // Eski Genel.AdresBilgisiGetir: KPS ikametgah sorgusu.
                TNS.KPM.Istek istek = new TNS.KPM.Istek();
                istek.istekTur = (int)TNS.KPM.ENUMIstekTur.IKAMETGAH;
                istek.tcKimlikNo = tcKimlikNo;
                istek.adSoyad = kullanan.ADSOYAD;
                istek.islemYapan = kullanan.kullaniciKodu;
                istek.islemTarih = new TNSDateTime(DateTime.Now);

                ObjectArray ikametgahList = TNS.KPM.Arac.Tanimla().IkametgahRaporla(kullanan, istek);
                if (!ikametgahList.sonuc.islemSonuc) throw new Exception(ikametgahList.sonuc.hataStr);
                if (ikametgahList.ObjeSayisi == 0) throw new Exception(ikametgahList.sonuc.bilgiStr);
                TNS.KPM.Kisi ikametgah = (TNS.KPM.Kisi)ikametgahList.objeler[0];

                string adres = "";
                if ((ikametgah.adres.koy ?? "").Trim() != "")
                    adres += ikametgah.adres.koy + " ";
                if ((ikametgah.adres.bucak ?? "").Trim() != "")
                    adres += ikametgah.adres.bucak + " ";
                if ((ikametgah.adres.mahalle ?? "").Trim() != "")
                    adres += ikametgah.adres.mahalle + " ";
                if ((ikametgah.adres.sokak ?? "").Trim() != "")
                    adres += ikametgah.adres.sokak + " ";
                if ((ikametgah.adres.numara ?? "").Trim() != "")
                    adres += "No: " + ikametgah.adres.numara + " ";
                if ((ikametgah.adres.ilce ?? "").Trim() != "")
                    adres += ikametgah.adres.ilce + " / ";
                if ((ikametgah.adres.il ?? "").Trim() != "")
                    adres += ikametgah.adres.il;

                return Results.Json(new { basarili = true, adres = adres });
            }
            catch (Exception ex)
            {
                return Hata(PersonelBilgiOrtak.HataMetni(ex.Message, "Adres bilgisi okunamadı."));
            }
        }
    }
}
