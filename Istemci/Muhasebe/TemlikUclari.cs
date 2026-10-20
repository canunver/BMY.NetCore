using System;
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
    /// <summary>Temlik kaydı (eski btnKaydet_Click). kod 0 ise yeni kayıt.</summary>
    public class TemlikIstegi
    {
        public int kod { get; set; }
        public string konu { get; set; }
        public string alacakliAdSoyad { get; set; }
        public string temlikAcanVergiNo { get; set; }
        public string temlikVerenVergiNo { get; set; }
        public double temlikTutari { get; set; }
        public string aciklama { get; set; }
        /// <summary>yyyy-MM-dd</summary>
        public string noterOnayTarihi { get; set; }
        public string noterOnayNo { get; set; }
    }

    public class TemlikSilIstegi
    {
        public int kod { get; set; }
    }

    /// <summary>
    /// Temlik Tanımlama / Borçlar ve Temlikler (eski ButceMuhasebe/TaahhutBorclarVeTemliklerKayit.aspx). İş kuralları servisi
    /// (IMUHServis.TaahhutBorclarTanim*) değişmeden çağrılır. Uçlar /api/temlik/*:
    ///   GET  liste?konu&amp;temlikAcanVergiNo&amp;temlikVerenVergiNo&amp;aciklama&amp;noterBas&amp;noterBit&amp;noterOnayNo  eski btnAra_Click
    ///   POST kaydet (TemlikIstegi)  eski btnKaydet_Click
    ///   POST sil ({ kod })          eski btnSil_Click
    /// Yetki: sistem / muhasebe yöneticisi ya da menüdeki sayfa yetkisi (eski Page_Load).
    /// </summary>
    public static class TemlikUclari
    {
        public const string Kok = "/api/temlik/";
        public const string SayfaAdresi = "../ButceMuhasebe/TaahhutBorclarVeTemliklerKayit.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Kok + "liste", Liste);
            app.MapPost(Kok + "kaydet", Kaydet);
            app.MapPost(Kok + "sil", Sil);
        }

        public static bool YetkiVar(Kullanici kullanan, string klasor)
        {
            return MuhTanimOrtak.Yetkili(kullanan, klasor, SayfaAdresi);
        }

        private static IMUHServis Servis() { return TNS.MUH.Arac.Tanimla(); }

        private static string Q(HttpContext baglam, string ad)
        {
            return Temiz(baglam.Request.Query[ad]);
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam)
        {
            return Oku(baglam, k => YetkiVar(k, ortam.ContentRootPath), kullanan =>
            {
                TaahhutBorclarTanim b = new TaahhutBorclarTanim();
                b.konu = Q(baglam, "konu");
                b.temlikAcanVergiNo = Q(baglam, "temlikAcanVergiNo");
                b.temlikVerenVergiNo = Q(baglam, "temlikVerenVergiNo");
                b.aciklama = Q(baglam, "aciklama");
                b.noterOnayTarihi = OdemeEmriMIFUclari.Tarih(Q(baglam, "noterBas"));
                b.noterOnayTarihiSorgu = OdemeEmriMIFUclari.Tarih(Q(baglam, "noterBit"));
                b.noterOnayNo = Q(baglam, "noterOnayNo");

                ObjectArray yler = Servis().TaahhutBorclarTanimListele(kullanan, b);
                if (yler == null || (yler.sonuc != null && !yler.sonuc.islemSonuc))
                    return Hata(HataMetni(yler?.sonuc?.hataStr, "Liste okunamadı."));

                List<object> satirlar = new List<object>();
                foreach (TaahhutBorclarTanim y in yler.objeler)
                {
                    satirlar.Add(new
                    {
                        KOD = y.kod,
                        KONU = y.konu ?? "",
                        ALACAKLIADSOYAD = y.alacakliAdSoyad ?? "",
                        TEMLIKACANVERGINO = y.temlikAcanVergiNo ?? "",
                        TEMLIKVERENVERGINO = y.temlikVerenVergiNo ?? "",
                        TEMLIKTUTARI = y.temlikTutari,
                        ACIKLAMA = y.aciklama ?? "",
                        NOTERONAYTARIHI = OdemeEmriMIFUclari.TarihYaz(y.noterOnayTarihi),
                        NOTERONAYNO = y.noterOnayNo ?? ""
                    });
                }
                return Results.Json(new { satirlar });
            }, "Liste okunamadı.");
        }

        /// <summary>Eski zorunluAlanKontrolu (istemcide de yapılır).</summary>
        private static string Eksik(TemlikIstegi i)
        {
            if (string.IsNullOrWhiteSpace(i.konu)) return "Lütfen konu alanını giriniz";
            if (string.IsNullOrWhiteSpace(i.alacakliAdSoyad)) return "Lütfen alacaklı alanını giriniz";
            if (string.IsNullOrWhiteSpace(i.temlikAcanVergiNo)) return "Lütfen temlik açan vergi numarası alanını giriniz";
            if (string.IsNullOrWhiteSpace(i.temlikVerenVergiNo)) return "Lütfen temlik veren vergi numarası alanını giriniz";
            if (string.IsNullOrWhiteSpace(i.noterOnayTarihi)) return "Lütfen noter onay tarihi alanını giriniz";
            if (string.IsNullOrWhiteSpace(i.noterOnayNo)) return "Lütfen noter onay numarası alanını giriniz";
            return "";
        }

        private static Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TemlikIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => YetkiVar(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null) return Hata("Bilgiler alınamadı.");
                string eksik = Eksik(istek);
                if (eksik != "") return Hata(eksik);

                TaahhutBorclarTanim b = new TaahhutBorclarTanim();
                b.kod = istek.kod;
                b.konu = Temiz(istek.konu);
                b.alacakliAdSoyad = Temiz(istek.alacakliAdSoyad);
                b.temlikAcanVergiNo = istek.temlikAcanVergiNo ?? "";
                b.temlikVerenVergiNo = istek.temlikVerenVergiNo ?? "";
                b.temlikTutari = istek.temlikTutari;
                b.aciklama = istek.aciklama ?? "";
                b.noterOnayTarihi = OdemeEmriMIFUclari.Tarih(istek.noterOnayTarihi);
                b.noterOnayNo = istek.noterOnayNo ?? "";
                b.islemTarihi = new TNSDateTime(DateTime.Now);
                b.islemYapanAd = kullanan.ADSOYAD;
                b.islemYapanKod = kullanan.TCKIMLIKNO;

                Sonuc sonuc = Servis().TaahhutBorclarTanimKaydet(kullanan, b);
                if (sonuc == null || !sonuc.islemSonuc) return Hata(HataMetni(sonuc?.hataStr, "Kaydedilemedi."));
                return Tamam("Kayıt başarıyla gerçekleşti.");
            });
        }

        private static Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TemlikSilIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => YetkiVar(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null || istek.kod <= 0) return Hata("Lütfen Silmek için kayıt seçiniz");
                ObjectArray liste = new ObjectArray();
                TaahhutBorclarTanim borc = new TaahhutBorclarTanim();
                borc.kod = istek.kod;
                liste.objeler.Add(borc);
                Sonuc sonuc = Servis().TaahhutBorclarTanimSil(kullanan, liste);
                if (sonuc == null || !sonuc.islemSonuc) return Hata(HataMetni(sonuc?.hataStr, "Silinemedi."));
                return Tamam("Silme işlemi başarıyla gerçekleşti.");
            });
        }
    }
}
