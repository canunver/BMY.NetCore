using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.KYM;

namespace Istemci.Kullanicilar
{
    /// <summary>Bir resmi tatil satırı. yil 0 ise her yıl aynı günde olan tatildir (eski "Tüm Yıllar"), değilse o yıla özeldir.</summary>
    public class ResmiTatilSatiri
    {
        public int yil { get; set; }
        public int ay { get; set; }
        public int gun { get; set; }
        public int sure { get; set; }
        public string aciklama { get; set; }
    }

    /// <summary>Kaydet isteği: eski boşsa yeni kayıt, doluysa o satır değiştirilir.</summary>
    public class ResmiTatilKaydetIstegi
    {
        public ResmiTatilSatiri eski { get; set; }
        public ResmiTatilSatiri yeni { get; set; }
    }

    /// <summary>
    /// Resmi Tatil Günleri Düzenleme (eski KYM/ResmiTatilXml.aspx; yeni sayfa KYM/ResmiTatilXml) uçları:
    ///   GET  /api/resmitatil/liste   dosyadaki bütün tatiller (eski XmlYukle)
    ///   POST /api/resmitatil/kaydet  tatil ekle / değiştir (eski record.add / update / move + EventStore_SubmitData)
    ///   POST /api/resmitatil/sil     tatil sil (eski record.remove + EventStore_SubmitData)
    /// Tatiller appSettings "ResmiTatil" anahtarındaki XML dosyasında tutulur. Bu dosyayı TNS.ResmiTatil (iş kuralları; ör. Alacak
    /// Takip'te takip / ödeme son tarihinin resmi tatile denk gelmemesi, izin ve bordro gün hesapları) de aynı anahtarla okur.
    /// Dosya eski sayfadaki biçimle (RESMITATIL / YIL DEGER / TATIL AY GUN SURE ACIKLAMA, XmlSerializer, UTF-8) yazılır ve
    /// ardından TNS.ResmiTatil.Yenile() ile önbellek boşaltılır; yeni günler hemen geçerli olur.
    /// Yalnızca sistem yöneticisi girer (eski Page_Load).
    /// </summary>
    public static class ResmiTatilUclari
    {
        private static readonly object kilit = new object();

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/resmitatil/liste", Liste);
            app.MapPost("/api/resmitatil/kaydet", Kaydet);
            app.MapPost("/api/resmitatil/sil", Sil);
        }

        public static bool YetkiVar(Kullanici kullanan)
        {
            return kullanan != null && !string.IsNullOrEmpty(kullanan.kullaniciKodu)
                && kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
        }

        private static IResult Denetle(HttpContext baglam)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Hata(string metin)
        {
            return Results.Json(new { basarili = false, hata = metin });
        }

        /// <summary>TNS.ResmiTatil'in okuduğu dosya (appSettings "ResmiTatil").</summary>
        private static string DosyaYolu()
        {
            return System.Configuration.ConfigurationManager.AppSettings.Get("ResmiTatil") + "";
        }

        // ------------------------------------------------------------------ okuma / yazma

        /// <summary>Dosyadaki bütün satırlar (eski XmlYukle / TarihEkle ile aynı öznitelikler).</summary>
        private static List<ResmiTatilSatiri> Oku(string dosya)
        {
            List<ResmiTatilSatiri> satirlar = new List<ResmiTatilSatiri>();
            XmlDocument xml = new XmlDocument();
            xml.Load(dosya);
            XmlElement kok = xml["RESMITATIL"];
            if (kok == null) return satirlar;
            foreach (XmlNode yilDugum in kok.ChildNodes)
            {
                if (yilDugum.NodeType != XmlNodeType.Element || yilDugum.Attributes?["DEGER"] == null) continue;
                int yil = OrtakFonksiyonlar.ConvertToInt(yilDugum.Attributes["DEGER"].InnerText, -1);
                if (yil < 0) continue;
                foreach (XmlNode t in yilDugum.ChildNodes)
                {
                    if (t.NodeType != XmlNodeType.Element || t.Attributes == null) continue;
                    satirlar.Add(new ResmiTatilSatiri
                    {
                        yil = yil,
                        ay = OrtakFonksiyonlar.ConvertToInt(t.Attributes["AY"]?.InnerText, 0),
                        gun = OrtakFonksiyonlar.ConvertToInt(t.Attributes["GUN"]?.InnerText, 0),
                        sure = OrtakFonksiyonlar.ConvertToInt(t.Attributes["SURE"]?.InnerText, 0),
                        aciklama = t.Attributes["ACIKLAMA"]?.InnerText ?? ""
                    });
                }
            }
            return satirlar;
        }

        /// <summary>Satırları eski sayfadaki ResmiTatilX.XmlKaydet ile aynı biçimde yazar ve TNS.ResmiTatil önbelleğini boşaltır.</summary>
        private static void Yaz(string dosya, List<ResmiTatilSatiri> satirlar)
        {
            ResmiTatilX r = new ResmiTatilX();
            foreach (ResmiTatilSatiri s in satirlar)
            {
                YilX yilX = r.YilBul(s.yil);
                if (yilX == null)
                {
                    yilX = new YilX { DEGER = s.yil };
                    r.yillar.Add(yilX);
                }
                yilX.tatiller.Add(new TatilX { AY = s.ay, GUN = s.gun, SURE = s.sure, ACIKLAMA = s.aciklama });
            }
            File.WriteAllText(dosya, r.XmlOlustur());
            TNS.ResmiTatil.Yenile();
        }

        private static bool Ayni(ResmiTatilSatiri a, ResmiTatilSatiri b)
        {
            return a.yil == b.yil && a.ay == b.ay && a.gun == b.gun && (a.aciklama ?? "") == (b.aciklama ?? "");
        }

        private static IResult ListeSonucu(List<ResmiTatilSatiri> satirlar, string bilgi)
        {
            return Results.Json(new
            {
                basarili = true,
                bilgi,
                buYil = DateTime.Now.Year,
                tatiller = satirlar.OrderBy(o => o.yil).ThenBy(o => o.ay).ThenBy(o => o.gun).ToList()
            });
        }

        // ------------------------------------------------------------------ uçlar

        private static IResult Liste(HttpContext baglam)
        {
            IResult ret = Denetle(baglam);
            if (ret != null) return ret;
            string dosya = DosyaYolu();
            if (dosya == "" || !File.Exists(dosya)) return Hata("Resmi tatil dosyası bulunamadı (App.config ResmiTatil: " + dosya + ").");
            try
            {
                lock (kilit)
                    return ListeSonucu(Oku(dosya), null);
            }
            catch (Exception ex)
            {
                return Hata("Resmi tatil dosyası okunamadı: " + ex.Message);
            }
        }

        private static string Denetle(ResmiTatilSatiri s)
        {
            if (s == null) return "Tatil bilgisi boş.";
            s.aciklama = (s.aciklama ?? "").Trim();
            if (s.aciklama == "") return "Açıklama giriniz.";
            if (s.yil != 0 && (s.yil < 1900 || s.yil > 2999)) return "Yıl hatalı.";
            if (s.sure < 1 || s.sure > 30) return "Süre 1 ile 30 gün arasında olmalı.";
            if (s.ay < 1 || s.ay > 12) return "Tarih hatalı.";
            // Her yıl tekrarlanan tatil artık yıl olmayan yıllarda da geçerli olmalı (29 Şubat olamaz).
            int denemeYil = s.yil == 0 ? 2001 : s.yil;
            if (s.gun < 1 || s.gun > DateTime.DaysInMonth(denemeYil, s.ay))
                return s.yil == 0 && s.ay == 2 && s.gun == 29 ? "Her yıl tekrarlanan tatil 29 Şubat olamaz." : "Tarih hatalı.";
            return null;
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IAntiforgery sahtecilik, ResmiTatilKaydetIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();
            string hata = Denetle(istek.yeni);
            if (hata != null) return Hata(hata);

            string dosya = DosyaYolu();
            if (dosya == "" || !File.Exists(dosya)) return Hata("Resmi tatil dosyası bulunamadı.");
            try
            {
                lock (kilit)
                {
                    List<ResmiTatilSatiri> satirlar = Oku(dosya);
                    if (istek.eski != null)
                    {
                        int i = satirlar.FindIndex(o => Ayni(o, istek.eski));
                        if (i < 0) return Hata("Değiştirilecek tatil bulunamadı; liste yenilendi, tekrar deneyiniz.");
                        satirlar.RemoveAt(i);
                    }
                    if (satirlar.Any(o => o.yil == istek.yeni.yil && o.ay == istek.yeni.ay && o.gun == istek.yeni.gun))
                        return Hata("Bu tarihte aynı türde bir tatil zaten var.");
                    satirlar.Add(istek.yeni);
                    Yaz(dosya, satirlar);
                    return ListeSonucu(satirlar, istek.yeni.aciklama + (istek.eski == null ? " eklendi" : " güncellendi"));
                }
            }
            catch (Exception ex)
            {
                return Hata("Resmi tatil dosyası yazılamadı: " + ex.Message);
            }
        }

        private static async Task<IResult> Sil(HttpContext baglam, IAntiforgery sahtecilik, ResmiTatilSatiri istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();

            string dosya = DosyaYolu();
            if (dosya == "" || !File.Exists(dosya)) return Hata("Resmi tatil dosyası bulunamadı.");
            try
            {
                lock (kilit)
                {
                    List<ResmiTatilSatiri> satirlar = Oku(dosya);
                    int i = satirlar.FindIndex(o => Ayni(o, istek));
                    if (i < 0) return Hata("Silinecek tatil bulunamadı; liste yenilendi.");
                    satirlar.RemoveAt(i);
                    Yaz(dosya, satirlar);
                    return ListeSonucu(satirlar, (istek.aciklama ?? "") + " silindi");
                }
            }
            catch (Exception ex)
            {
                return Hata("Resmi tatil dosyası yazılamadı: " + ex.Message);
            }
        }

        // ------------------------------------------------------------------ XML sınıfları (eski ResmiTatilXml.aspx.cs ile aynı)

        [XmlRoot("RESMITATIL", Namespace = "", IsNullable = false)]
        public class ResmiTatilX
        {
            public ResmiTatilX()
            {
                this.yillar = new List<YilX>();
            }

            [XmlElement(ElementName = "YIL")]
            public List<YilX> yillar { get; set; }

            public YilX YilBul(int yil)
            {
                foreach (var y in this.yillar)
                {
                    if (y.DEGER == yil)
                        return y;
                }
                return null;
            }

            public string XmlOlustur()
            {
                this.yillar = this.yillar.OrderBy(o => o.DEGER).ToList();

                foreach (var yil in this.yillar)
                    yil.tatiller = yil.tatiller.OrderBy(o => o.AY).ThenBy(o => o.GUN).ToList();

                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                XmlSerializer sr = new XmlSerializer(typeof(ResmiTatilX));

                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                var streamWriter = new System.IO.StreamWriter(ms, System.Text.Encoding.UTF8);
                sr.Serialize(streamWriter, this, ns);

                byte[] arr = ms.ToArray();

                streamWriter.Close();
                ms.Close();

                return System.Text.Encoding.UTF8.GetString(arr);
            }
        }

        public class YilX
        {
            public YilX()
            {
                this.tatiller = new List<TatilX>();
            }

            [XmlElement(ElementName = "TATIL")]
            public List<TatilX> tatiller { get; set; }

            [XmlAttribute]
            public int DEGER { get; set; }
        }

        public class TatilX
        {
            [XmlAttribute]
            public int AY { get; set; }

            [XmlAttribute]
            public int GUN { get; set; }

            [XmlAttribute]
            public int SURE { get; set; }

            [XmlAttribute]
            public string ACIKLAMA { get; set; }
        }
    }
}
