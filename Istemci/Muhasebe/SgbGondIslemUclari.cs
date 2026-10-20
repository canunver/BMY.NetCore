using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.KYM;
using TNS.MUH;

namespace Istemci.Muhasebe
{
    /// <summary>Bir satırın düzeltme isteği (eski IslemYap parametreleri). tur: 0 çift işlemli (bağı kopart), 1-2 tutarsızlık (B.İ. üret / yaz).</summary>
    public class SgbGondIslemIstegi
    {
        public int tur { get; set; }
        public int yil { get; set; }
        public int belgeTur { get; set; }
        public string belgeNo { get; set; }
        public int revizeYil { get; set; }
        public int revizeNo { get; set; }
    }

    /// <summary>
    /// Bütçe İşlemleri Düzeltme (eski ButceMuhasebe/SGBGondIslemDuzeltme.aspx: Gönderme/Tenkis - Bütçe İşlemi listeleme/düzeltme) uçları.
    ///   GET  /api/sgbgondislem/tanim  : açılış yılı ve yıllar
    ///   GET  /api/sgbgondislem/liste  : tur 0 çift işlemli olanlar, 1 tenkis - işlem tutarsızlığı, 2 gönderme - işlem tutarsızlığı
    ///                                   (sorgular eski sayfadaki KosulOl'un aynısı, IUZYServis.SorguDondur ile)
    ///   POST /api/sgbgondislem/islem  : bir satır için tur 0 IMUHServis.BIBagKopart, diğerleri IMUHServis.BIDuzelt
    /// Eski sayfada "İşlemYap" seçili satırları arka planda tek tek işleyip ilerlemeyi gösteriyordu; burada sayfa satırları sırayla
    /// bu uca gönderir ve ilerlemeyi kendisi yazar. Sayfa yetkisi menu.xml'den.
    /// </summary>
    public static class SgbGondIslemUclari
    {
        public const string SayfaAdres = "../ButceMuhasebe/SGBGondIslemDuzeltme.aspx";
        private const string Uc = "/api/sgbgondislem/";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "tanim", Tanim);
            app.MapGet(Uc + "liste", Liste);
            app.MapPost(Uc + "islem", Islem);
        }

        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, SayfaAdres);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult red = Denetle(baglam, ortam, out Kullanici kullanan);
            if (red != null) return red;
            int yil = DateTime.Now.Year;
            List<int> yillar = new List<int>();
            for (int i = yil; i >= 2006; i--) yillar.Add(i);
            return Results.Json(new { yil = yil, yillar = yillar });
        }

        /// <summary>Eski Listele: en çok 500 satır.</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int? tur, int? yil, int? muhasebe, string eko, string peb)
        {
            IResult red = Denetle(baglam, ortam, out Kullanici kullanan);
            if (red != null) return red;

            int t = tur ?? 0;
            if (t < 0 || t > 2) t = 0;
            int y = yil ?? 0;
            if (y < 2000) return Hata("Yıl giriniz.");

            List<object> satirlar = new List<object>();
            try
            {
                string ksl = KosulOl(t, y, muhasebe ?? 0, Suz(eko), Suz(peb));
                DataTable dt = TNS.UZY.Arac.Tanimla().SorguDondur(ksl, "BaglantiSatiriBUM", 0, 0);
                if (dt != null)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        decimal gondToplam = DecimalAl(item, 7);
                        decimal islToplam = DecimalAl(item, 13);
                        satirlar.Add(new
                        {
                            tur = t,
                            yil = IntAl(item, 0),
                            belgeTur = IntAl(item, 1),
                            belgeNo = StrAl(item, 2),
                            kurumKod = StrAl(item, 3),
                            muhasebeKod = IntAl(item, 4),
                            birimKod = StrAl(item, 5),
                            gondDurum = IntAl(item, 6),
                            gondToplam = gondToplam,
                            islYil = IntAl(item, 8),
                            islBelgeTur = IntAl(item, 9),
                            islBelgeNo = StrAl(item, 10),
                            islDurum = IntAl(item, 11),
                            revizeYil = y,
                            revizeNo = IntAl(item, 12),
                            islToplam = islToplam,
                            fark = gondToplam - islToplam
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
            return Results.Json(new { satirlar = satirlar });
        }

        /// <summary>Bir satırın düzeltmesi (eski IslemYapIc).</summary>
        private static async Task<IResult> Islem(HttpContext baglam, IWebHostEnvironment ortam, SgbGondIslemIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult red = Denetle(baglam, ortam, out Kullanici kullanan);
            if (red != null) return red;
            if (istek == null) return Hata("İşlem yapılamadı.");

            Sonuc s;
            try
            {
                IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
                if (istek.tur == 0)
                {
                    s = servisMUH.BIBagKopart(kullanan, istek.revizeYil, istek.revizeNo);
                }
                else
                {
                    TNS.BKD.MPRevizeForm f = new TNS.BKD.MPRevizeForm();
                    f.yil = istek.revizeYil;
                    f.revizeNo = istek.revizeNo;
                    f.odGonNo = istek.belgeNo;
                    f.odGonTur = istek.belgeTur;
                    f.odGonYil = istek.yil;
                    s = servisMUH.BIDuzelt(kullanan, f);
                }
            }
            catch (Exception)
            {
                return Hata("İşlem yapılamadı.");
            }
            if (!s.islemSonuc) return Hata(HataMetni(s.hataStr));
            return Results.Json(new { basarili = true, bilgi = "İşlem başarıyla tamamlandı, lütfen listeleme yapınız!" });
        }

        // ------------------------------------------------------------------ eski sayfanın sorguları

        private static string KosulOl(int tur, int yil, int saymanlikKod, string ekoKod, string pebKod)
        {
            string sayKosul, sayKosulIc, sayKosulMsrf;
            if (saymanlikKod > 0 || !string.IsNullOrWhiteSpace(ekoKod) || !string.IsNullOrWhiteSpace(pebKod))
            {
                sayKosulMsrf = $"AND 1 IN (SELECT 1 FROM MPGMASRAFPLANITERTIP IC where IC.YIL=MPGMASRAFPLANIREVIZEFORM.YIL AND IC.REVIZENO=MPGMASRAFPLANIREVIZEFORM.REVIZENO ";
                if (saymanlikKod > 0)
                {
                    sayKosul = $" AND MUHGONDERMETENKISFORM.MUHASEBEKOD ={saymanlikKod}";
                    sayKosulMsrf += $" AND CONVERT(INT, SUBSTRING(IC.KURUMSALYAPIKOD,2,5))={saymanlikKod}";
                }
                else
                    sayKosul = "";

                if (!string.IsNullOrWhiteSpace(ekoKod) || !string.IsNullOrWhiteSpace(pebKod))
                {
                    sayKosulIc = $"AND 1 IN (SELECT 1 FROM MUHGONDERMETENKIS IC where IC.YIL=MUHGONDERMETENKISFORM.YIL AND IC.BELGETUR=MUHGONDERMETENKISFORM.BELGETUR AND IC.BELGENO=MUHGONDERMETENKISFORM.BELGENO ";
                    if (!string.IsNullOrWhiteSpace(ekoKod))
                    {
                        ekoKod = "'" + ekoKod.Replace("'", "").Replace(".", "") + "%'";
                        sayKosulIc += $" AND IC.EKOKOD LIKE {ekoKod}";
                        sayKosulMsrf += $" AND IC.EKOKOD LIKE {ekoKod}";
                    }
                    if (!string.IsNullOrWhiteSpace(pebKod))
                    {
                        pebKod = "'" + pebKod.Replace("'", "").Replace(".", "") + "%'";
                        sayKosulIc += $" AND IC.PEBKOD LIKE {pebKod}";
                        sayKosulMsrf += $" AND IC.PEBKOD LIKE {pebKod}";
                    }
                    sayKosulIc += ")";
                }
                else
                    sayKosulIc = "";
                sayKosulMsrf += ")";
            }
            else
            {
                sayKosul = "";
                sayKosulMsrf = "";
                sayKosulIc = "";
            }
            if (tur == 0)
            {
                return $@"SELECT top 500 G.*, R.* FROM

                (SELECT MPGMASRAFPLANIREVIZEFORM.ODEGONYIL YIL, MPGMASRAFPLANIREVIZEFORM.ODEGONTUR BELGETUR, MPGMASRAFPLANIREVIZEFORM.ODEGONNO BELGENO, MPGMASRAFPLANIREVIZEFORM.FORMDURUM, MPGMASRAFPLANIREVIZEFORM.REVIZENO, SUM(MPGMASRAFPLANITERTIP.ODENEK * -1) TOPLAM
                from MPGMASRAFPLANIREVIZEFORM
                LEFT OUTER JOIN MPGMASRAFPLANITERTIP ON MPGMASRAFPLANITERTIP.YIL=MPGMASRAFPLANIREVIZEFORM.YIL AND MPGMASRAFPLANITERTIP.REVIZENO=MPGMASRAFPLANIREVIZEFORM.REVIZENO
                where MPGMASRAFPLANIREVIZEFORM.YIL={yil} AND MPGMASRAFPLANIREVIZEFORM.REVIZENO>0 AND MPGMASRAFPLANIREVIZEFORM.ODEGONTUR>0
                and 1 IN (SELECT 1 FROM MPGMASRAFPLANIREVIZEFORM  IC where IC.ODEGONYIL = MPGMASRAFPLANIREVIZEFORM.ODEGONYIL AND IC.ODEGONTUR = MPGMASRAFPLANIREVIZEFORM.ODEGONTUR AND IC.ODEGONNO = MPGMASRAFPLANIREVIZEFORM.ODEGONNO AND (IC.YIL != MPGMASRAFPLANIREVIZEFORM.YIL OR IC.REVIZENO != MPGMASRAFPLANIREVIZEFORM.REVIZENO))
                {sayKosulMsrf}
                group by MPGMASRAFPLANIREVIZEFORM.ODEGONYIL, MPGMASRAFPLANIREVIZEFORM.ODEGONTUR, MPGMASRAFPLANIREVIZEFORM.ODEGONNO, MPGMASRAFPLANIREVIZEFORM.FORMDURUM, MPGMASRAFPLANIREVIZEFORM.REVIZENO) R

                LEFT OUTER JOIN

                (SELECT MUHGONDERMETENKISFORM.YIL, MUHGONDERMETENKISFORM.BELGETUR, MUHGONDERMETENKISFORM.BELGENO, KURUMKOD, MUHASEBEKOD, BIRIMKOD, MUHGONDERMETENKISFORM.FORMDURUM, SUM(MUHGONDERMETENKIS.MIKTAR*-1) TOPLAM
                  FROM  MUHGONDERMETENKISFORM
                  LEFT OUTER JOIN MUHGONDERMETENKIS ON MUHGONDERMETENKIS.YIL=MUHGONDERMETENKISFORM.YIL AND MUHGONDERMETENKIS.BELGETUR=MUHGONDERMETENKISFORM.BELGETUR AND MUHGONDERMETENKIS.BELGENO=MUHGONDERMETENKISFORM.BELGENO
                  where  MUHGONDERMETENKISFORM.YIL={yil} {sayKosul} {sayKosulIc}
                  group by MUHGONDERMETENKISFORM.YIL, MUHGONDERMETENKISFORM.BELGETUR, MUHGONDERMETENKISFORM.BELGENO, KURUMKOD, MUHASEBEKOD, BIRIMKOD, MUHGONDERMETENKISFORM.FORMDURUM) G

                ON R.YIL=G.YIL and R.BELGETUR=G.BELGETUR and R.BELGENO=G.BELGENO";
            }
            else if (tur == 1)
            {
                return $@"SELECT top 500 G.*, R.* FROM
                (SELECT MUHGONDERMETENKISFORM.YIL, MUHGONDERMETENKISFORM.BELGETUR, MUHGONDERMETENKISFORM.BELGENO, KURUMKOD, MUHASEBEKOD, BIRIMKOD, MUHGONDERMETENKISFORM.FORMDURUM, SUM(MUHGONDERMETENKIS.MIKTAR*-1) TOPLAM
                  FROM  MUHGONDERMETENKISFORM
                  LEFT OUTER JOIN MUHGONDERMETENKIS ON MUHGONDERMETENKIS.YIL=MUHGONDERMETENKISFORM.YIL AND MUHGONDERMETENKIS.BELGETUR=MUHGONDERMETENKISFORM.BELGETUR AND MUHGONDERMETENKIS.BELGENO=MUHGONDERMETENKISFORM.BELGENO
                  where  MUHGONDERMETENKISFORM.YIL={yil} {sayKosul} {sayKosulIc} AND MUHGONDERMETENKISFORM.BELGETUR=4 AND
                (FORMDURUM=5 OR FORMDURUM=9)
                  group by MUHGONDERMETENKISFORM.YIL, MUHGONDERMETENKISFORM.BELGETUR, MUHGONDERMETENKISFORM.BELGENO, KURUMKOD, MUHASEBEKOD, BIRIMKOD, MUHGONDERMETENKISFORM.FORMDURUM) G
                LEFT OUTER JOIN

                (select MPGMASRAFPLANIREVIZEFORM.ODEGONYIL YIL, MPGMASRAFPLANIREVIZEFORM.ODEGONTUR BELGETUR, MPGMASRAFPLANIREVIZEFORM.ODEGONNO BELGENO, MPGMASRAFPLANIREVIZEFORM.FORMDURUM, MPGMASRAFPLANIREVIZEFORM.REVIZENO, SUM(MPGMASRAFPLANITERTIP.ODENEK * -1) TOPLAM
                from MPGMASRAFPLANIREVIZEFORM
                LEFT OUTER JOIN MPGMASRAFPLANITERTIP ON MPGMASRAFPLANITERTIP.YIL=MPGMASRAFPLANIREVIZEFORM.YIL AND MPGMASRAFPLANITERTIP.REVIZENO=MPGMASRAFPLANIREVIZEFORM.REVIZENO
                where MPGMASRAFPLANIREVIZEFORM.YIL={yil} AND MPGMASRAFPLANIREVIZEFORM.REVIZENO>0
                AND MPGMASRAFPLANITERTIP.ODENEK<0 AND MPGMASRAFPLANIREVIZEFORM.ODEGONTUR=4
                {sayKosulMsrf}
                group by MPGMASRAFPLANIREVIZEFORM.ODEGONYIL, MPGMASRAFPLANIREVIZEFORM.ODEGONTUR, MPGMASRAFPLANIREVIZEFORM.ODEGONNO, MPGMASRAFPLANIREVIZEFORM.FORMDURUM, MPGMASRAFPLANIREVIZEFORM.REVIZENO) R
                ON R.YIL=G.YIL and R.BELGETUR=G.BELGETUR and R.BELGENO=G.BELGENO
                where (G.FORMDURUM=9 AND (R.FORMDURUM !=9 AND R.FORMDURUM IS NOT NULL)) OR (G.FORMDURUM=5 AND (R.FORMDURUM !=5  OR R.FORMDURUM IS NULL)) or (G.FORMDURUM=5 AND R.TOPLAM!=G.TOPLAM)";
            }
            else
            {
                return $@"SELECT top 500 G.*, R.* FROM
                (select MPGMASRAFPLANIREVIZEFORM.ODEGONYIL YIL, MPGMASRAFPLANIREVIZEFORM.ODEGONTUR BELGETUR, MPGMASRAFPLANIREVIZEFORM.ODEGONNO BELGENO, MPGMASRAFPLANIREVIZEFORM.FORMDURUM, MPGMASRAFPLANIREVIZEFORM.REVIZENO, SUM(MPGMASRAFPLANITERTIP.ODENEK) TOPLAM
                from MPGMASRAFPLANIREVIZEFORM
                LEFT OUTER JOIN MPGMASRAFPLANITERTIP ON MPGMASRAFPLANITERTIP.YIL=MPGMASRAFPLANIREVIZEFORM.YIL AND MPGMASRAFPLANITERTIP.REVIZENO=MPGMASRAFPLANIREVIZEFORM.REVIZENO
                where MPGMASRAFPLANIREVIZEFORM.YIL={yil} AND MPGMASRAFPLANIREVIZEFORM.REVIZENO>0
                AND MPGMASRAFPLANIREVIZEFORM.ODEGONTUR=3
                AND (MPGMASRAFPLANIREVIZEFORM.FORMDURUM=5 OR MPGMASRAFPLANIREVIZEFORM.FORMDURUM=9)
                AND MPGMASRAFPLANITERTIP.ODENEK>0
                {sayKosulMsrf}
                group by MPGMASRAFPLANIREVIZEFORM.ODEGONYIL, MPGMASRAFPLANIREVIZEFORM.ODEGONTUR, MPGMASRAFPLANIREVIZEFORM.ODEGONNO, MPGMASRAFPLANIREVIZEFORM.FORMDURUM, MPGMASRAFPLANIREVIZEFORM.REVIZENO) R

                LEFT OUTER JOIN

                (SELECT MUHGONDERMETENKISFORM.YIL, MUHGONDERMETENKISFORM.BELGETUR, MUHGONDERMETENKISFORM.BELGENO, KURUMKOD, MUHASEBEKOD, BIRIMKOD, MUHGONDERMETENKISFORM.FORMDURUM, SUM(MUHGONDERMETENKIS.MIKTAR) TOPLAM
                  FROM  MUHGONDERMETENKISFORM
                  LEFT OUTER JOIN MUHGONDERMETENKIS ON MUHGONDERMETENKIS.YIL=MUHGONDERMETENKISFORM.YIL AND MUHGONDERMETENKIS.BELGETUR=MUHGONDERMETENKISFORM.BELGETUR AND MUHGONDERMETENKIS.BELGENO=MUHGONDERMETENKISFORM.BELGENO
                  where MUHGONDERMETENKISFORM.YIL={yil} {sayKosul} {sayKosulIc} AND  MUHGONDERMETENKISFORM.BELGETUR=3
                  group by MUHGONDERMETENKISFORM.YIL, MUHGONDERMETENKISFORM.BELGETUR, MUHGONDERMETENKISFORM.BELGENO, KURUMKOD, MUHASEBEKOD, BIRIMKOD, MUHGONDERMETENKISFORM.FORMDURUM) G
                ON R.YIL=G.YIL and R.BELGETUR=G.BELGETUR and R.BELGENO=G.BELGENO

                where (R.FORMDURUM=9 AND (G.FORMDURUM !=9 AND G.FORMDURUM IS NOT NULL)) OR (R.FORMDURUM=5 AND (G.FORMDURUM !=5  OR G.FORMDURUM IS NULL)) or (R.FORMDURUM=5 AND R.TOPLAM!=G.TOPLAM)";
            }
        }

        // ------------------------------------------------------------------ yardımcılar

        /// <summary>Eko / faaliyet kodunda yalnızca rakam ve nokta kalır (sorgu metnine elle yazılan değerler girmesin).</summary>
        private static string Suz(string deger)
        {
            if (string.IsNullOrWhiteSpace(deger)) return "";
            StringBuilder sb = new StringBuilder();
            foreach (char c in deger.Trim())
                if (char.IsDigit(c) || c == '.') sb.Append(c);
            return sb.ToString();
        }

        private static int IntAl(DataRow item, int i)
        {
            if (i >= item.Table.Columns.Count || item[i] is DBNull) return 0;
            return Convert.ToInt32(item[i]);
        }

        private static string StrAl(DataRow item, int i)
        {
            if (i >= item.Table.Columns.Count || item[i] is DBNull) return "";
            return Convert.ToString(item[i]);
        }

        private static decimal DecimalAl(DataRow item, int i)
        {
            if (i >= item.Table.Columns.Count || item[i] is DBNull) return 0;
            return Convert.ToDecimal(item[i]);
        }

        private static IResult Hata(string metin)
        {
            return Results.Json(new { basarili = false, hata = metin });
        }

        private static string HataMetni(string hata)
        {
            hata = hata ?? "";
            if (hata.Trim() == "" || hata.Contains("   at ")) return "İşlem yapılamadı.";
            return hata.Replace("<br />", "\n").Replace("<br/>", "\n").Replace("<br>", "\n").Trim();
        }
    }
}
