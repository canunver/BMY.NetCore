using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Istemci.Organizasyon;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.BKD;
using TNS.KYM;
using TNS.MUH;

namespace Istemci.Muhasebe
{
    public class ButceAcilisFisiIstegi
    {
        public int Yil { get; set; }
        public int Muhasebe { get; set; }
        public string Birim { get; set; }
        /// <summary>Kurumsal kod (bütçe tertibi kıstası)</summary>
        public string Kur { get; set; }
        /// <summary>Daha önce ayrılmış belge numarası (boş olabilir)</summary>
        public string BelgeNo { get; set; }
    }

    /// <summary>
    /// Gider Bütçesi Açılış Fişi (eski ButceMuhasebe/ButceAcilisFisi.aspx) uçları. Yalnızca sistem yöneticisi kullanır (eski sayfa gibi).
    ///   GET  /api/butceacilis/tanim     yıllar (2005 - gelecek yıl), varsayılan yıl gelecek yıl
    ///   POST /api/butceacilis/olustur   {Yil, Muhasebe, Birim, Kur, BelgeNo}: eski "Bütçeden Faaliyet Bazlı Oluştur"
    ///                                   (btnOlusturFaaliyet_Click). Eski "Fiş Oluştur" ve "Excel Dosyasından Oluştur" Core'da yok (kullanıcı isteği).
    /// </summary>
    public static class ButceAcilisFisiUclari
    {
        public const string SayfaAdresi = "../ButceMuhasebe/ButceAcilisFisi.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/butceacilis/tanim", Tanim);
            app.MapPost("/api/butceacilis/olustur", Olustur);
        }

        /// <summary>Eski sayfa: kullanan.KullaniciTipiIceriyorMu(SISTEMYONETICISI).</summary>
        public static bool YetkiVar(Kullanici kullanan)
        {
            return GenelGorevUclari.YetkiVar(kullanan);
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
            return Results.Json(new { yil = DateTime.Now.Year + 1, yillar = yillar });
        }

        private static async Task<IResult> Olustur(HttpContext baglam, ButceAcilisFisiIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Hata("Bilgi yok.");

            string birim = (istek.Birim ?? "").Trim();
            string kur = (istek.Kur ?? "").Trim();
            List<string> hatalar = new List<string>();
            if (istek.Yil <= 0) hatalar.Add("Yıl bilgisi boş bırakılmış.");
            if (istek.Muhasebe <= 0) hatalar.Add("Muhasebe Kod bilgisi boş bırakılmış.");
            if (birim == "") hatalar.Add("Birim Kod bilgisi boş bırakılmış.");
            if (kur == "") hatalar.Add("Kurumsal Kod bilgisi boş bırakılmış.");
            if (hatalar.Count > 0) return Hata(string.Join("\n", hatalar));

            IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
            TNS.BHMv2.IBHMv2Servis bhmServis = TNS.BHMv2.Arac.Tanimla();
            Tertip kistasTertip = new Tertip(istek.Yil, ENUMTabloKod.TABPEBOdenek, kur, "", "", "", "");
            try
            {
                TNS.BHMv2.UcgenKodBilgileri ukb = new TNS.BHMv2.UcgenKodBilgileri();
                for (int i = 0; i < kistasTertip.kodlar.Length; i++)
                    ukb.Add(new TNS.BHMv2.UcgenKodBilgi(kistasTertip.KodNolar(i), kistasTertip.KodNoKirilimSay(0)));

                TNS.BHMv2.EkDegerParam[] ekDegerler = new TNS.BHMv2.EkDegerParam[1];
                ekDegerler[0] = new TNS.BHMv2.EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, "06.00");
                TNS.BHMv2.OdenekBilgi[] tertipOdenekler = bhmServis.ListeTertipOl(kullanan, kistasTertip, ukb, ekDegerler, true, false);

                OdemeEmriMIFForm tf = new OdemeEmriMIFForm();
                tf.aciklama = "AÇILIŞ FİŞİ (BÜTÇE)";
                tf.belgeNo = (istek.BelgeNo ?? "").Trim();
                tf.fisTurYap(ENUMFISTUR.ACILIS);
                tf.birim = birim;
                tf.muhasebe = istek.Muhasebe;
                tf.yil = istek.Yil;
                tf.islemTarih = new TNSDateTime(DateTime.Now.ToString());
                tf.formDurumTarih = new TNSDateTime(DateTime.Now.ToString());
                tf.kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
                tf.islemYapan = kullanan.kullaniciKodu;
                tf.islemYapanAd = kullanan.ad + " " + kullanan.soyad;
                Hashtable ht = new Hashtable();
                foreach (TNS.BHMv2.OdenekBilgi o in tertipOdenekler)
                {
                    string icBirim = DetayBirimBul(servisMUH, ht, tf.muhasebe, o.tertip.kodlar[0], tf.birim);
                    OdemeEmriMIFDetay td = new OdemeEmriMIFDetay(tf.yil);
                    string yardimciHesapKod = o.tertip.kodlar[3].Substring(0, 2) + "0" + o.tertip.kodlar[3].Substring(3, 1);
                    td.hesapKod = "900." + yardimciHesapKod;
                    TNS.MUH.Arac.TertipAta(ref td, o.tertip.TertipStrOl());
                    td.borc = (double)o.ekRakamlar[0];
                    td.alacak = 0;
                    tf.detay.objeler.Add(td);
                    td.detayBirim = icBirim;

                    td = new OdemeEmriMIFDetay(tf.yil);
                    td.hesapKod = "901.01." + yardimciHesapKod;
                    TNS.MUH.Arac.TertipAta(ref td, o.tertip.TertipStrOl());
                    td.borc = 0;
                    td.alacak = (double)o.ekRakamlar[0];
                    td.detayBirim = icBirim;
                    tf.detay.objeler.Add(td);
                }

                Sonuc sonuc = servisMUH.OdemeEmriMIFKaydet(kullanan, tf);
                if (!sonuc.islemSonuc) return Hata(string.IsNullOrWhiteSpace(sonuc.hataStr) ? "Fiş oluşturulamadı." : sonuc.hataStr);
                return Results.Json(new { basarili = true, yil = tf.yil, belgeNo = sonuc.anahtar, satirSayisi = tf.detay.objeler.Count });
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
        }

        /// <summary>Eski DetayBirimBul: kurumsal kodun bu muhasebedeki birimi; yoksa fişin birimi.</summary>
        private static string DetayBirimBul(IMUHServis servisMUH, Hashtable ht, int muhasebe, string kurKod, string defBirimKod)
        {
            string birimKod = null;
            try
            {
                birimKod = (string)ht[kurKod];
            }
            catch
            {
                birimKod = null;
            }
            if (birimKod == null)
            {
                MuhasebeBirim muhasebeBirim = new MuhasebeBirim();
                muhasebeBirim.birimKod = "!000";
                muhasebeBirim.kurum = kurKod;
                muhasebeBirim.muhasebe = muhasebe;

                ObjectArray oa = servisMUH.MuhasebeBirimListele(muhasebeBirim);
                if (oa.sonuc.islemSonuc && oa.ObjeSayisi > 0)
                    birimKod = ((MuhasebeBirim)oa.objeler[0]).birimKod;
                else birimKod = defBirimKod;
                ht[kurKod] = birimKod;
            }
            return birimKod;
        }
    }
}
