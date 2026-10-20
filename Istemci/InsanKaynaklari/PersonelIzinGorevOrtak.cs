using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.IKY;
using TNS.KYM;

namespace Istemci.InsanKaynaklari
{
    /// <summary>
    /// İzin/Görev (PersonelIzin) ve Görevlendirmeler (PersonelGorev) sekmelerinin ortak işleri. Uçlar kendi önekleriyle
    /// (/api/personelbilgi/izin/..., /api/personelbilgi/gorev/...) bu yordamları çağırır; yetki denetimini (PersonelBilgiOrtak.Denetle)
    /// çağıran uç yapar.
    ///   - Belge listesi / ekleme / gösterme / silme (eski BelgeListesiDoldur, BelgeKaydet, BelgeGoster, BelgeSil)
    ///   - Birim ağacı (eski ExtOrgSecim + ORGBirim.NodeLoad)
    ///   - Coğrafya (yer) kod ağacı (eski ../ButceHazirlikv2/KodAgac.aspx?kodTur=20)
    /// </summary>
    public static class PersonelIzinGorevOrtak
    {
        /// <summary>Yüklenebilecek belgenin en büyük boyutu (eski sayfada sınır IIS'in istek sınırıydı).</summary>
        public const int BelgeAzamiBoyut = 20 * 1024 * 1024;

        // ------------------------------------------------------------------ belgeler

        private static ObjectArray BelgeleriOku(Kullanici kullanan, int personelKod, ENUMBELGEFORMTIP tip, int kayitKod)
        {
            PersonelBelgeler belge = new PersonelBelgeler();
            belge.pKod = personelKod;
            belge.tip = (int)tip;
            belge.tipKayitKod = kayitKod;
            return TNS.IKY.Arac.Tanimla().PersonelBelgeListele(kullanan, belge);
        }

        /// <summary>Eski BelgeListesiDoldur: kaydın (tipKayitKod) belgeleri.</summary>
        public static IResult BelgeListesi(Kullanici kullanan, int personelKod, ENUMBELGEFORMTIP tip, int kayitKod)
        {
            List<object> satirlar = new List<object>();
            if (personelKod <= 0 || kayitKod <= 0) return Results.Json(new { satirlar = satirlar });

            ObjectArray belgeler = BelgeleriOku(kullanan, personelKod, tip, kayitKod);
            if (belgeler == null) return Results.Json(new { satirlar = satirlar });
            if (!belgeler.sonuc.islemSonuc && !string.IsNullOrWhiteSpace(belgeler.sonuc.hataStr))
                return Results.Json(new { satirlar = satirlar, hata = PersonelBilgiOrtak.HataMetni(belgeler.sonuc.hataStr, "Belgeler okunamadı.") });

            foreach (PersonelBelgeler detay in belgeler.objeler)
            {
                satirlar.Add(new
                {
                    KOD = detay.kod,
                    PKOD = detay.pKod,
                    TIPKAYITKOD = detay.tipKayitKod,
                    TIP = detay.tip,
                    BELGEADI = detay.belgeAdi
                });
            }
            return Results.Json(new { satirlar = satirlar });
        }

        /// <summary>
        /// Eski BelgeKaydet: çok parçalı formdaki personelKod, kayitKod ve dosya alanlarıyla belgeyi kaydeder.
        /// (Eski sayfa kayıt seçili değilse önce Kaydet'i çağırıyordu; yeni sayfada bunu JavaScript yapar, uç kayıt kodu ister.)
        /// </summary>
        public static async Task<IResult> BelgeYukle(HttpContext baglam, Kullanici kullanan, ENUMBELGEFORMTIP tip)
        {
            if (!baglam.Request.HasFormContentType) return Results.BadRequest();
            IFormCollection form = await baglam.Request.ReadFormAsync();
            int personelKod = OrtakFonksiyonlar.ConvertToInt(form["personelKod"].ToString(), 0);
            int kayitKod = OrtakFonksiyonlar.ConvertToInt(form["kayitKod"].ToString(), 0);
            if (personelKod <= 0) return Results.Json(new { basarili = false, hata = "Personel bilgisi boş bırakılamaz." });
            if (kayitKod <= 0) return Results.Json(new { basarili = false, hata = "Belge eklemek için önce kaydı seçiniz ya da kaydediniz." });

            IFormFile dosya = form.Files["dosya"];
            if (dosya == null || dosya.Length <= 0) return Results.Json(new { basarili = false, hata = "Yüklemek istediğiniz dosyanın içi boş." });
            if (dosya.Length > BelgeAzamiBoyut) return Results.Json(new { basarili = false, hata = "Dosya çok büyük (en çok 20 MB)." });

            int belgeBoyutu = (int)dosya.Length;
            PersonelBelgeler yeniBelge = new PersonelBelgeler(personelKod, kayitKod, (int)tip, dosya.FileName ?? "belge", belgeBoyutu);
            using (MemoryStream ms = new MemoryStream(belgeBoyutu))
            {
                await dosya.CopyToAsync(ms);
                byte[] icerik = ms.ToArray();
                Array.Copy(icerik, yeniBelge.belge, Math.Min(icerik.Length, belgeBoyutu));
            }

            Sonuc sonuc = TNS.IKY.Arac.Tanimla().PersonelBelgeKaydet(kullanan, yeniBelge);
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Belge kaydedilemedi.") });
            return Results.Json(new { basarili = true, bilgi = PersonelBilgiOrtak.Duz(sonuc.bilgiStr) });
        }

        /// <summary>
        /// Belge bu personelin bu kaydına mı ait? Eski BelgeGoster / BelgeSil yalnızca belge koduyla çalışıyordu; uç, adres satırından
        /// başka personelin belgesine erişilmesin diye belgenin sayfadaki kayda ait olduğunu iş kuralının kendi listesinden doğrular.
        /// </summary>
        private static bool BelgeKaydaAit(Kullanici kullanan, int personelKod, ENUMBELGEFORMTIP tip, int kayitKod, int belgeKod)
        {
            if (personelKod <= 0 || kayitKod <= 0 || belgeKod <= 0) return false;
            ObjectArray belgeler = BelgeleriOku(kullanan, personelKod, tip, kayitKod);
            if (belgeler == null || !belgeler.sonuc.islemSonuc) return false;
            foreach (PersonelBelgeler b in belgeler.objeler)
                if (b.kod == belgeKod) return true;
            return false;
        }

        /// <summary>Eski BelgeGoster: belgeyi indirilecek dosya olarak döndürür.</summary>
        public static IResult BelgeGoster(Kullanici kullanan, int personelKod, ENUMBELGEFORMTIP tip, int kayitKod, int kod)
        {
            if (!BelgeKaydaAit(kullanan, personelKod, tip, kayitKod, kod)) return Results.Json(new { hata = "Belge bulunamadı." });

            Sonuc dosyaKonumuAdiUzantisi = TNS.IKY.Arac.Tanimla().PersonelBelgeListesiGetir(kullanan, new PersonelBelgeler { kod = kod });
            if (dosyaKonumuAdiUzantisi == null || !dosyaKonumuAdiUzantisi.islemSonuc || string.IsNullOrEmpty(dosyaKonumuAdiUzantisi.anahtar))
                return Results.Json(new { hata = PersonelBilgiOrtak.HataMetni(dosyaKonumuAdiUzantisi?.hataStr ?? dosyaKonumuAdiUzantisi?.bilgiStr, "Belge bulunamadı.") });

            string[] parametreler = dosyaKonumuAdiUzantisi.anahtar.Split('\n');
            if (parametreler.Length != 3 || !File.Exists(parametreler[0])) return Results.Json(new { hata = "Belge bulunamadı." });
            return DosyaIslem.DosyaGonder(parametreler[0], parametreler[1], true, parametreler[2]);
        }

        /// <summary>Eski BelgeSil.</summary>
        public static IResult BelgeSil(Kullanici kullanan, int personelKod, ENUMBELGEFORMTIP tip, int kayitKod, int kod)
        {
            if (kod == 0) return Results.Json(new { basarili = false, hata = "Belge seçiniz." });
            if (!BelgeKaydaAit(kullanan, personelKod, tip, kayitKod, kod)) return Results.Json(new { basarili = false, hata = "Belge bulunamadı." });

            Sonuc sonuc = TNS.IKY.Arac.Tanimla().PersonelBelgeSil(kullanan, new PersonelBelgeler { kod = kod });
            if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Belge silinemedi.") });
            return Results.Json(new { basarili = true, bilgi = PersonelBilgiOrtak.Duz(sonuc.bilgiStr) });
        }

        // ------------------------------------------------------------------ birim ağacı

        /// <summary>
        /// Eski ExtOrgSecim + ORGBirim.NodeLoad: ust düğümünün alt birimleri. Kökte, yetkiliBirimler true ise kullanıcının
        /// "Personel Saymanlık-Birim Yetkisi"ne göre birimler (yetkisi yoksa bütün kök birimler; eski KullanicininBirimleri),
        /// false ise bütün kök birimler gelir. ara yazılınca adında geçen birimler düz liste olarak (il / ilçe adıyla) gelir.
        /// </summary>
        public static IResult BirimAgaci(Kullanici kullanan, int? ust, string ara, bool yetkiliBirimler)
        {
            try
            {
                ara = (ara ?? "").Trim();
                TNS.ORG.OrganizasyonBirim kriter = new TNS.ORG.OrganizasyonBirim();
                kriter.anaBirimKod = Math.Max(0, ust ?? 0);
                kriter.sablonMu = false;
                if (ara != "")
                {
                    kriter.anaBirimKod = 0;
                    kriter.adi = ara;
                }
                else if (kriter.anaBirimKod == 0)
                {
                    string yetkili = yetkiliBirimler ? (kullanan.YetkiTipiDegeriBul("Personel Saymanlık-Birim Yetkisi") ?? "") : "";
                    bool birimYetkisiVar = false;
                    foreach (string y in yetkili.Split(';'))
                    {
                        string[] m = y.Split('-');
                        if (m.Length == 2 && (OrtakFonksiyonlar.ConvertToInt(m[0], 0) > 0 || OrtakFonksiyonlar.ConvertToInt(m[1], 0) > 0))
                            birimYetkisiVar = true;
                    }
                    if (birimYetkisiVar) kriter.yetkiliOlduguMuhasebeBirim = yetkili;
                    else kriter.kopruKod = "999";
                }

                ObjectArray<TNS.ORG.OrganizasyonBirim> sonuc = TNS.ORG.Arac.Tanimla().OrganizasyonBirimListele(kriter, false, false);
                if (sonuc == null || !sonuc.sonuc.islemSonuc) return Results.Json(new { hata = "Birimler okunamadı." });
                List<object> dugumler = new List<object>();
                foreach (TNS.ORG.OrganizasyonBirim b in sonuc.objeler)
                {
                    string ad = b.adi ?? "";
                    if (ara != "")
                    {
                        string ilIlce = TNS.ORG.Arac.OrgBirimIlIlceAdBul(b.kod);
                        if (!string.IsNullOrEmpty(ilIlce)) ad += " (" + ilIlce + ")";
                    }
                    dugumler.Add(new { kod = b.kod, ad = ad, birimAd = b.adi ?? "", altVar = ara == "" && b.altDalVar });
                    if (dugumler.Count >= 300) break;
                }
                return Results.Json(new { dugumler = dugumler });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Birimler okunamadı." });
            }
        }

        // ------------------------------------------------------------------ coğrafya (yer) kod ağacı

        /// <summary>
        /// Eski ../ButceHazirlikv2/KodAgac.aspx?kodTur=20 (coğrafya kodları: ülke / bölge / il / ilçe): ust kodunun altındaki,
        /// verilen seviyedeki (1-4) kodlar. Kökte seviye 1, ust boştur.
        /// </summary>
        public static IResult YerAgaci(Kullanici kullanan, string ust, int seviye)
        {
            if (seviye < 1 || seviye > 4) return Results.BadRequest();
            try
            {
                string kultur = string.IsNullOrEmpty(kullanan.aktifKultur) ? "tr-TR" : kullanan.aktifKultur;
                var kodlar = TNS.BKD.Arac.Tanimla().KodListeleSeviye(0, "20", (ust ?? "").Trim(), seviye, true, true, "", "", kultur);
                List<object> dugumler = new List<object>();
                if (kodlar != null && kodlar.sonuc.islemSonuc)
                    foreach (TNS.BKD.ABSKod k in kodlar.absKodlar)
                        dugumler.Add(new { kod = Convert.ToString(k.kod), ad = k.AdKultur(kultur), seviye = seviye, altVar = seviye < 4 });
                return Results.Json(new { dugumler = dugumler });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Yer kodları okunamadı." });
            }
        }
    }
}
