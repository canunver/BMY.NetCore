using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.KYM;

namespace Istemci.Guncelleme
{
    /// <summary>
    /// Yazılım Güncelle sayfasının (eski YazilimGuncelle/YazilimGuncelle.aspx) uçları:
    ///   POST /api/guncelleme/yazilim-zip  : (multipart: dosya, zamanFark) yüklenen Zip dosyasındaki değişen dosyaları kopyalar
    /// Eski sayfanın bilgi alanına yazdığı satırları { satirlar: [{ metin, tur }] } olarak döndürür. Eski sayfadaki güncelleme
    /// adresinden (ListeVer.aspx / DosyaVer.aspx) indirme kullanıcı isteğiyle alınmadı. Zip .NET'in kendi kitaplığıyla
    /// (System.IO.Compression) açılır; şifreli Zip ve Rar desteklenmez (eski SharpZipLib kullanıcı isteğiyle çıkarıldı).
    /// </summary>
    public static class YazilimGuncelleUclari
    {
        public static void Ekle(WebApplication app)
        {
            app.MapPost("/api/guncelleme/yazilim-zip", ZiptenGuncelle).DisableAntiforgery();
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            return GuncellemeOrtak.Denetle(baglam, k => GuncellemeOrtak.YazilimGuncelleYetkili(k, ortam.ContentRootPath), out kullanan);
        }

        private static int ZamanFarki(string deger)
        {
            // Eski kod: sayı değilse 60 dakika.
            return int.TryParse((deger ?? "").Trim(), out int dakika) ? dakika : 60;
        }

        /// <summary>Eski btnZiptanYukle_Click: Zip dosyası geçici klasöre açılır, değişen dosyalar kopyalanır.</summary>
        private static async Task<IResult> ZiptenGuncelle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            GuncellemeOrtak.BuyukGovdeyeIzinVer(baglam);
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!baglam.Request.HasFormContentType) return Results.BadRequest();

            IFormCollection form = await baglam.Request.ReadFormAsync(GuncellemeOrtak.BuyukForm());
            IFormFile dosya = form.Files["dosya"];
            YazilimGuncelleIslem islem = new YazilimGuncelleIslem(GuncellemeOrtak.KokBul(ortam.ContentRootPath), ZamanFarki(form["zamanFark"]));

            if (dosya == null || dosya.Length == 0)
            {
                islem.Yaz("Yüklenecek dosyayı seçiniz!", "hata");
                return Results.Json(new { satirlar = islem.Satirlar });
            }
            string uzanti = Path.GetExtension(dosya.FileName).ToLowerInvariant();
            if (uzanti != ".zip")
            {
                islem.Yaz("Yüklenecek dosya Zip olmalı!", "hata");
                return Results.Json(new { satirlar = islem.Satirlar });
            }

            islem.Yaz("Zip dosyasından güncellenecek!", "baslik");
            string gecici = OrtakFonksiyonlar.GeciciYolBul();
            string ad = OrtakClass.DosyaIslem.DosyaAdUretSade() + "_" + Path.GetFileNameWithoutExtension(Path.GetFileName(dosya.FileName));
            string zipDosya = Path.Combine(gecici, ad + uzanti);
            string zipKlasor = Path.Combine(gecici, ad);
            try
            {
                using (FileStream yaz = File.Create(zipDosya))
                    await dosya.CopyToAsync(yaz);

                // Dosyaların tarihi Zip'teki tarihleriyle açılır (karşılaştırma buna göre yapılır); klasör dışına yazan girişler reddedilir.
                System.IO.Compression.ZipFile.ExtractToDirectory(zipDosya, zipKlasor, true);

                islem.KlasordenGuncelle(zipKlasor);
            }
            catch (Exception ex)
            {
                islem.Yaz(ex.Message, "hata");
            }
            finally
            {
                try { File.Delete(zipDosya); } catch { }
                try { Directory.Delete(zipKlasor, true); } catch { }
            }
            return Results.Json(new { satirlar = islem.Satirlar });
        }
    }

    /// <summary>
    /// Eski YazilimGuncelle.aspx.cs'deki güncelleme işlemi (GuncellemeBasla, KlasordenDosyaAl, DosyaAl). Uygulama klasöründeki dosya
    /// yoksa ya da açılan Zip'teki dosya (zaman farkı düşüldükten sonra) daha yeniyse dosya kopyalanır.
    /// Değişenler: dosyalar uygulama klasörünün dışına yazılmaz; aynı tarihli dosyalarda uzunluk gerçekten karşılaştırılır
    /// (eski kodda hedef uzunluk hep 0'dı).
    /// </summary>
    internal class YazilimGuncelleIslem
    {
        private readonly string kok;
        private readonly int zamanFarki;
        public readonly List<object> Satirlar = new List<object>();

        public YazilimGuncelleIslem(string kok, int zamanFarki)
        {
            this.kok = kok;
            this.zamanFarki = zamanFarki;
        }

        /// <param name="tur">bilgi | baslik | hata</param>
        public void Yaz(string metin, string tur = "bilgi")
        {
            Satirlar.Add(new { metin = metin, tur = tur });
        }

        private void HataYaz(string adres, Exception e)
        {
            Yaz(string.Format("HATA - {0} adresine ulaşılırken hata oluştu. Hata: {1} {2} {3}", adres, e.Source, e.Message, e.InnerException?.Message), "hata");
        }

        private class Kaynak
        {
            public string Ad;
            public DateTime Tarih;
            public long Uzunluk;
        }

        public void KlasordenGuncelle(string klasor)
        {
            List<Kaynak> kaynaklar = new List<Kaynak>();
            KlasorListesi(new DirectoryInfo(klasor), "", kaynaklar);
            List<Kaynak> inecekler = Inecekler(kaynaklar);
            if (inecekler == null) return;
            foreach (Kaynak k in inecekler)
            {
                string kaynakDosya = Path.Combine(klasor, k.Ad);
                DosyaYaz(k.Ad, kaynakDosya, hedef => { File.Copy(kaynakDosya, hedef, true); return Task.CompletedTask; }).GetAwaiter().GetResult();
            }
        }

        private static void KlasorListesi(DirectoryInfo klasor, string ust, List<Kaynak> liste)
        {
            foreach (FileInfo f in klasor.GetFiles())
                // Eski kod tarihi metne çevirip geri okuyordu; saniyeden küçük kısım atılır.
                liste.Add(new Kaynak { Ad = ust + f.Name, Tarih = new DateTime(f.LastWriteTime.Ticks - f.LastWriteTime.Ticks % TimeSpan.TicksPerSecond), Uzunluk = f.Length });
            foreach (DirectoryInfo d in klasor.GetDirectories())
                KlasorListesi(d, ust + d.Name + Path.DirectorySeparatorChar, liste);
        }

        private string HedefYol(string ad)
        {
            string goreli = (ad ?? "").Replace('/', '\\').TrimStart('\\').Replace('\\', Path.DirectorySeparatorChar);
            if (goreli == "") return null;
            string tam = Path.GetFullPath(Path.Combine(kok, goreli));
            return tam.StartsWith(kok + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ? tam : null;
        }

        private List<Kaynak> Inecekler(List<Kaynak> kaynaklar)
        {
            Yaz("Güncellenecek dosyalar belirleniyor.");
            List<Kaynak> inecekler = new List<Kaynak>();
            try
            {
                foreach (Kaynak k in kaynaklar)
                {
                    string hedef = HedefYol(k.Ad);
                    if (hedef == null)
                    {
                        Yaz("Uygulama klasörünün dışındaki dosya atlandı: " + k.Ad, "hata");
                        continue;
                    }
                    bool indir = false;
                    if (!File.Exists(hedef))
                        indir = true;
                    else
                    {
                        FileInfo hedefBilgi = new FileInfo(hedef);
                        DateTime hedefTarih = hedefBilgi.LastWriteTime.AddMinutes(zamanFarki * -1);
                        if (k.Tarih > hedefTarih)
                            indir = true;
                        else if (k.Tarih == hedefTarih && k.Uzunluk != hedefBilgi.Length)
                            indir = true;
                    }
                    if (indir) inecekler.Add(k);
                }
            }
            catch (Exception e)
            {
                HataYaz("liste", e);
                return null;
            }

            if (inecekler.Count == 0)
            {
                Yaz("Güncellenecek dosya yok. Sonraki güncelleme zamanı bekleniyor.");
                return inecekler;
            }

            List<string> adlar = new List<string>();
            for (int i = 0; i < inecekler.Count && i <= 41; i++) adlar.Add(inecekler[i].Ad);
            Yaz(string.Format("{0} adet güncellenecek dosya bulundu. Farklı olanlar: {1}{2}", inecekler.Count, string.Join("; ", adlar), inecekler.Count > 42 ? "..........." : ""), "baslik");
            return inecekler;
        }

        private async Task DosyaYaz(string ad, string kaynakDosya, Func<string, Task> yaz)
        {
            string hedef = HedefYol(ad);
            try
            {
                if (File.Exists(hedef))
                    File.SetAttributes(hedef, FileAttributes.Normal);
                else
                    Directory.CreateDirectory(Path.GetDirectoryName(hedef));

                try
                {
                    await yaz(hedef);
                }
                catch (Exception e)
                {
                    Yaz("HATA " + kaynakDosya, "hata");
                    Yaz(string.Format("HATA - {0} {1} {2}", e.Source, e.Message, e.InnerException?.Message), "hata");
                }
            }
            catch (Exception e)
            {
                Yaz(string.Format("HATA - Güncellenecek dosya indirilirken hata oluştu. {0} Hata: {1} {2} {3}", kaynakDosya, e.Source, e.Message, e.InnerException?.Message), "hata");
            }
        }
    }
}
