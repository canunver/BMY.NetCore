using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.ATM;
using TNS.KYM;
using TNS.MUH;

namespace Istemci.AlacakTakip
{
    public class OdemeEmriOlusturIstegi
    {
        /// <summary>Seçilen hesap hareketlerinin kodları (eski getSelectedNoList: KOD,KOD,...).</summary>
        public List<int> kodlar { get; set; }
        public string aciklama { get; set; }
    }

    /// <summary>
    /// Toplu Ödeme Emri (eski AlacakTakip/AlacakTakipOdemeEmri.aspx) uçları. Sayfa Takip İşlemleri (AlacakTakipListe) ekranından
    /// Sgb.pencere('../AlacakTakip/AlacakTakipOdemeEmri') ile açılır:
    ///   GET  /api/alacaktakip/odemeemri/liste                    ödeme emri bağlanmamış hesap hareketleri ve son açıklama (eski Page_Load / Listele)
    ///   POST /api/alacaktakip/odemeemri/olustur                  seçilenlerden tek ödeme emri (eski OdemeEmriOlustur)
    ///   GET  /api/alacaktakip/odemeemri/yazdir?kodlar=1,2          oluşturulacak belgenin satırları, Excel
    ///                                                            (eski OdemeEmriOlustur Tur=Yaz, ATM\ATM-TopluOdemeEmriListe.xlt)
    /// Sayfa menüde yok; yetkisi üst sayfa Takip İşlemleri'nden (AlacakTakipListe / AlacakTakipListeAB) gelir ve eskisi gibi
    /// TNS.ATM.Yetki.SayfayaGirisYapabilirMi de aranır. Ödeme emri kayıt yetkisi iş kuralında (TNS.MUH) denetlenir.
    /// </summary>
    public static class OdemeEmriUclari
    {
        public const string SayfaAdresi = "../AlacakTakip/AlacakTakipOdemeEmri.aspx";
        private const string Uc = "/api/alacaktakip/odemeemri/";
        private const string AciklamaDegisken = "ALACAKTAKIPODEMEEMRIACIKLAMA";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "liste", Liste);
            app.MapPost(Uc + "olustur", Olustur);
            app.MapGet(Uc + "yazdir", Yazdir);
        }

        // ------------------------------------------------------------------ yetki ve ortak

        public static bool Yetkili(Kullanici kullanan, string klasor)
        {
            if (kullanan == null) return false;
            return SayfaYetki.Yetkili(kullanan, klasor, SayfaAdresi, OdemeUclari.UstSayfalar) && TNS.ATM.Yetki.SayfayaGirisYapabilirMi(kullanan);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!Yetkili(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        private static IResult Basarisiz(string hata) { return Results.Json(new { basarili = false, hata = hata ?? "" }); }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Replace("<br>", " ").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        private static string TarihYaz(TNSDateTime t)
        {
            try
            {
                if (t == null || t.isNull) return "";
                DateTime d = t.Oku();
                return d <= DateTime.MinValue.AddDays(1) ? "" : d.ToString("yyyy-MM-dd");
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>Kod listesini eski biçime ("1,2,3,") çevirir; geçersiz ya da boşsa null.</summary>
        private static string KodListesi(IEnumerable<int> kodlar)
        {
            if (kodlar == null) return null;
            string liste = "";
            foreach (int k in kodlar)
                if (k > 0) liste += k.ToString(CultureInfo.InvariantCulture) + ",";
            return liste == "" ? null : liste;
        }

        // ------------------------------------------------------------------ liste

        /// <summary>Açılış (eski Page_Load + Listele): ödeme emri numarası olmayan hesap hareketleri ve kullanıcının son açıklaması.</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                string aciklama = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, AciklamaDegisken);

                DosyaHesapHareketAna hesapHareket = new DosyaHesapHareketAna();
                hesapHareket.odemeEmriNo = "!!!";
                ObjectArray bilgi = TNS.ATM.Arac.Tanimla().DosyaHesapHareketAnaDosyaListele(kullanan, hesapHareket);
                if (bilgi == null || bilgi.sonuc == null || !bilgi.sonuc.islemSonuc)
                    return Results.Json(new { aciklama = aciklama, satirlar = new List<object>(), hata = HataMetni(bilgi?.sonuc?.hataStr, "Hesap hareketleri okunamadı.") });

                List<object> list = new List<object>();
                foreach (DosyaHesapHareketAna t in bilgi.objeler)
                {
                    list.Add(new
                    {
                        KOD = t.kod,
                        YIL = t.yil,
                        DOSYANO = t.dosyaNo.ToString("0000"),
                        BIRIMKODU = t.birimKodu,
                        BORCLUAD = t.borcluAdi,
                        TOPLAMTUTAR = t.toplamTutar,
                        ISLEMTARIHI = TarihYaz(t.islemTarihi),
                        ODEMEEMRINO = t.odemeEmriNo,
                        TASARIMAD = t.tasarimAdi,
                        TASARIMKOD = t.tasarimKod
                    });
                }
                return Results.Json(new { aciklama = aciklama, satirlar = list, bilgi = list.Count == 0 ? "Kayıt bulunmamaktadır." : "" });
            }
            catch (Exception)
            {
                return Hata("Hesap hareketleri okunamadı.");
            }
        }

        // ------------------------------------------------------------------ ödeme emri

        /// <summary>Ödeme Emri Oluştur (eski OdemeEmriOlustur): seçilen hareketlerden tek ödeme emri; hareketlere belge numarası yazılır.</summary>
        private static async Task<IResult> Olustur(HttpContext baglam, IWebHostEnvironment ortam, OdemeEmriOlusturIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            string secilenHareketler = KodListesi(istek?.kodlar);
            if (secilenHareketler == null) return Basarisiz("Lütfen kayıt seçiniz.");
            string aciklama = (istek.aciklama ?? "").Trim();
            try
            {
                GenelIslemler.KullaniciDegiskenSakla(kullanan, AciklamaDegisken, aciklama);

                IATMServis servisATM = TNS.ATM.Arac.Tanimla();
                IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
                OdemeEmriMIFForm odemeEmri = BelgeHazirla(kullanan, servisATM, servisMUH, secilenHareketler, aciklama, out Hashtable htBirimKurumsal,
                    out bool tekBirim, out string tekBirimKod, out bool tekKisi, out string tekKisiVNo, out string tekKisiAdi);
                if (odemeEmri == null) return Basarisiz("Seçilen kayıtların hesap hareketi bulunamadı.");

                if (tekBirim)
                {
                    odemeEmri.birim = tekBirimKod;
                    odemeEmri.kurKod = OrtakFonksiyonlar.ConvertToStr(htBirimKurumsal[odemeEmri.muhasebe + "_" + odemeEmri.birim]);
                }

                if (tekKisi)
                {
                    odemeEmri.ilgiliAd = tekKisiAdi;
                    odemeEmri.ilgiliNo = tekKisiVNo;
                }

                Sonuc sonuc = servisMUH.OdemeEmriMIFKaydet(kullanan, odemeEmri);
                if (sonuc == null || !sonuc.islemSonuc)
                    return Basarisiz("Ödeme emri oluşturulamadı. " + HataMetni(sonuc?.hataStr, ""));

                string odemeEmriNo = sonuc.anahtar;
                string[] kodlar = secilenHareketler.Trim(',').Split(',');
                ObjectArray listAna = new ObjectArray();

                foreach (var kod in kodlar)
                {
                    int k = OrtakFonksiyonlar.ConvertToInt(kod, 0);
                    DosyaHesapHareketAna h = new DosyaHesapHareketAna();
                    h.kod = k;
                    h.odemeEmriNo = odemeEmriNo;
                    listAna.objeler.Add(h);
                }

                sonuc = servisATM.DosyaHesapHareketAnaOdemeEmriNoKaydet(kullanan, listAna);
                if (sonuc == null || !sonuc.islemSonuc)
                    return Results.Json(new
                    {
                        basarili = false,
                        hata = "Ödeme emri oluşturuldu ancak hareketlere bağlanamadı. " + HataMetni(sonuc?.hataStr, ""),
                        yil = odemeEmri.yil,
                        belgeNo = odemeEmriNo
                    });

                return Results.Json(new { basarili = true, bilgi = "Ödeme emri oluşturuldu.", yil = odemeEmri.yil, belgeNo = odemeEmriNo });
            }
            catch (Exception)
            {
                return Basarisiz("Ödeme emri oluşturulamadı.");
            }
        }

        /// <summary>Ödeme Emri Yazdır (eski OdemeEmriOlustur, Tur=Yaz): oluşturulacak belgenin satırları kaydedilmeden Excel'e yazılır.</summary>
        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, string kodlar)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            List<int> liste = new List<int>();
            foreach (string s in (kodlar ?? "").Split(','))
                if (int.TryParse(s.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int k)) liste.Add(k);
            string secilenHareketler = KodListesi(liste);
            if (secilenHareketler == null) return Hata("Lütfen kayıt seçiniz.");
            string sablon = OdemeRaporlari.SablonBul(ortam.ContentRootPath, OdemeRaporlari.TopluOdemeEmriSablon);
            if (sablon == null) return Hata("Rapor şablonu bulunamadı (ATM\\" + OdemeRaporlari.TopluOdemeEmriSablon + ").");
            try
            {
                OdemeEmriMIFForm odemeEmri = BelgeHazirla(kullanan, TNS.ATM.Arac.Tanimla(), TNS.MUH.Arac.Tanimla(), secilenHareketler, "",
                    out Hashtable _, out bool _, out string _, out bool _, out string _, out string _);
                if (odemeEmri == null) return Hata("Seçilen kayıtların hesap hareketi bulunamadı.");

                AsposeLisans.Yukle();
                string dosya = OdemeRaporlari.TopluOdemeEmri(odemeEmri, sablon);
                string gidenAd = "ATM-TopluOdemeEmriListe_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                return DosyaIslem.DosyaGonder(dosya, gidenAd, true, GenelIslemler.ExcelTur());
            }
            catch (Exception)
            {
                return Hata("Rapor hazırlanamadı.");
            }
        }

        /// <summary>
        /// Eski OdemeEmriOlustur'un belge hazırlayan bölümü (kod aynen): seçilen hareketlerin satırları hesap kodu ve birime göre
        /// birleştirilir, tasarımdaki alt hesap / satır numarası / yansıtma hesabı uygulanır, 805 yansıtma satırları eklenir ve sıralanır.
        /// Hareket yoksa null.
        /// </summary>
        private static OdemeEmriMIFForm BelgeHazirla(Kullanici kullanan, IATMServis servisATM, IMUHServis servisMUH, string secilenHareketler, string aciklama,
            out Hashtable htBirimKurumsal, out bool tekBirim, out string tekBirimKod, out bool tekKisi, out string tekKisiVNo, out string tekKisiAdi)
        {
            htBirimKurumsal = new Hashtable();
            tekBirim = true;
            tekBirimKod = "";
            tekKisi = true;
            tekKisiVNo = "";
            tekKisiAdi = "";

            DosyaHesapHareketTasarim hesapHareket = new DosyaHesapHareketTasarim();
            ObjectArray tasarim = servisATM.DosyaHesapHareketTasarimListele(kullanan, hesapHareket);

            DosyaHesapHareketCocuk cocuk = new DosyaHesapHareketCocuk();
            cocuk.anaKodList = secilenHareketler;
            ObjectArray cocukList = servisATM.DosyaHesapHareketCocukListele(kullanan, cocuk);
            if (cocukList == null || cocukList.objeler == null || cocukList.objeler.Count == 0)
                return null;

            OdemeEmriMIFForm odemeEmri = new OdemeEmriMIFForm();
            odemeEmri.yil = DateTime.Now.Year;
            odemeEmri.islemTarih = new TNSDateTime(DateTime.Now);
            odemeEmri.kurum = TNS.MUH.Arac.VarsayilanKurumKod();
            odemeEmri.aciklama = aciklama;

            odemeEmri.birim = "000";
            if (cocukList.objeler.Count > 0)
            {
                DosyaHesapHareketCocuk hc = (DosyaHesapHareketCocuk)cocukList.objeler[0];
                odemeEmri.muhasebe = hc.dosyaMuhasebe;
            }

            OdemeEmriMIFDetay d;
            foreach (DosyaHesapHareketCocuk c in cocukList.objeler)
            {
                if (c.borc == 0 && c.alacak == 0) continue;

                if (tekBirimKod != c.dosyaBirim && tekBirimKod != "")
                    tekBirim = false;
                tekBirimKod = c.dosyaBirim;

                if (tekKisiVNo != c.altHesapKod && tekKisiVNo != "")
                    tekKisi = false;
                tekKisiVNo = c.altHesapKod;
                tekKisiAdi = c.dosyaBorcluAd;

                //Birime ait kurumsal kodu bulmak için.
                string kurKod = "";
                if (htBirimKurumsal[c.dosyaMuhasebe + "_" + c.dosyaBirim] == null)
                {
                    kurKod = servisMUH.SaymanlikBirimKurkodBul(c.dosyaMuhasebe, c.dosyaBirim);
                    htBirimKurumsal[c.dosyaMuhasebe + "_" + c.dosyaBirim] = kurKod;
                }
                else
                {
                    kurKod = OrtakFonksiyonlar.ConvertToStr(htBirimKurumsal[c.dosyaMuhasebe + "_" + c.dosyaBirim]);
                }

                bool bulundu = false;
                foreach (OdemeEmriMIFDetay oed in odemeEmri.detay.objeler)
                {
                    if (oed.hesapKod == c.hesapKodu.Replace(".", "") && oed.detayBirim == c.dosyaBirim + "." + kurKod)
                    {
                        bool islemYap = false;
                        if (AltHesapKullanilacakMi(tasarim, c.hesapKodu, c.tasarimKod, c.borc > 0 ? 0 : 1))
                        {
                            if (oed.altHesapKod == c.altHesapKod)
                            {
                                islemYap = false;//Eğer alt hesap bazında kayıt tutuluyorsa aynı hesap kodlar farklı satırda olmalıki borçlar ödeme emrinde net görülsün
                                break;
                            }
                        }
                        else
                            islemYap = true;

                        if (islemYap)
                        {
                            //Eğer bulunan hesap borç kaydı ve şu anda yapılacak alacak ise bu değildir. Bir satıra hem borç hem de alacak yazılamaz
                            if (c.borc > 0 && oed.alacak > 0) continue;
                            if (c.alacak > 0 && oed.borc > 0) continue;

                            oed.borc += c.borc;
                            oed.alacak += c.alacak;
                            bulundu = true;
                        }
                    }
                }

                if (!bulundu)
                {
                    d = new OdemeEmriMIFDetay(DateTime.Now.Year);
                    d.satirNo = SatirNoVer(tasarim, c.hesapKodu, c.tasarimKod, c.borc > 0 ? 0 : 1);
                    d.hesapKod = c.hesapKodu.Replace(".", "");

                    d.detayBirim = c.dosyaBirim + "." + kurKod;
                    d.borc = c.borc;
                    d.alacak = c.alacak;

                    if (AltHesapKullanilacakMi(tasarim, c.hesapKodu, c.tasarimKod, c.borc > 0 ? 0 : 1))
                        d.altHesapKod = c.altHesapKod;

                    odemeEmri.detay.objeler.Add(d);
                }

                YansitmaSatiriEkle(tasarim, c, htBirimKurumsal, odemeEmri.detay);
            }

            YansitmaSatiri805Ekle(odemeEmri.detay);
            odemeEmri.detay.objeler.Sort(new Sirala());
            return odemeEmri;
        }

        private static bool AltHesapKullanilacakMi(ObjectArray tasarim, string hesapKod, int tasarimAnaKod, int borcAlacak)
        {
            foreach (DosyaHesapHareketTasarim t in tasarim.objeler)
            {
                if (t.anaKod == tasarimAnaKod && t.hesapKodu.Replace(".", "") == hesapKod.Replace(".", "") && t.borcAlacak == borcAlacak)
                    return t.altHesap > 0;
            }

            return false;
        }

        private static int SatirNoVer(ObjectArray tasarim, string hesapKod, int tasarimAnaKod, int borcAlacak)
        {
            foreach (DosyaHesapHareketTasarim t in tasarim.objeler)
            {
                if (t.anaKod == tasarimAnaKod && t.hesapKodu.Replace(".", "") == hesapKod.Replace(".", "") && t.borcAlacak == borcAlacak)
                    return t.belgeSatirNo;
            }

            return 0;
        }

        private static string YansitmaHesabiVer(ObjectArray tasarim, string hesapKod, int tasarimAnaKod, int borcAlacak)
        {
            foreach (DosyaHesapHareketTasarim t in tasarim.objeler)
            {
                if (t.anaKod == tasarimAnaKod && t.hesapKodu.Replace(".", "") == hesapKod.Replace(".", "") && t.borcAlacak == borcAlacak)
                    return t.yansitmaHesapKodu ?? "";
            }

            return "";
        }

        private static void YansitmaSatiriEkle(ObjectArray tasarim, DosyaHesapHareketCocuk cocuk, Hashtable htBirimKurumsal, ObjectArray odemeEmridetay)
        {
            string yansitmaHesabi = YansitmaHesabiVer(tasarim, cocuk.hesapKodu, cocuk.tasarimKod, cocuk.borc > 0 ? 0 : 1);
            if (yansitmaHesabi.Trim() == "") return;

            double alacak = Math.Round(cocuk.alacak, 2);

            bool bulundu = false;

            string kurKod = OrtakFonksiyonlar.ConvertToStr(htBirimKurumsal[cocuk.dosyaMuhasebe + "_" + cocuk.dosyaBirim]);

            foreach (OdemeEmriMIFDetay oed in odemeEmridetay.objeler)
            {
                if (oed.hesapKod == yansitmaHesabi.Replace(".", "") && oed.detayBirim == cocuk.dosyaBirim + "." + kurKod)
                {
                    oed.borc += 0;
                    oed.alacak += alacak;
                    bulundu = true;
                }
            }

            if (!bulundu)
            {
                OdemeEmriMIFDetay d = new OdemeEmriMIFDetay(DateTime.Now.Year);
                d.satirNo = 800000 + SatirNoVer(tasarim, cocuk.hesapKodu, cocuk.tasarimKod, cocuk.borc > 0 ? 0 : 1);
                d.hesapKod = yansitmaHesabi.Replace(".", "");
                d.detayBirim = cocuk.dosyaBirim + "." + kurKod;
                d.borc = 0;
                d.alacak = alacak;

                odemeEmridetay.objeler.Add(d);
            }
        }

        private static void YansitmaSatiri805Ekle(ObjectArray odemeEmriDetay)
        {
            ObjectArray yansitma805 = new ObjectArray();

            foreach (OdemeEmriMIFDetay oed in odemeEmriDetay.objeler)
            {
                if (oed.hesapKod.StartsWith("800"))
                {
                    bool bulundu = false;
                    foreach (OdemeEmriMIFDetay od in yansitma805.objeler)
                    {
                        if (od.hesapKod == "805" && od.detayBirim == oed.detayBirim)
                        {
                            od.borc += oed.alacak;
                            bulundu = true;
                        }
                    }

                    if (!bulundu)
                    {
                        OdemeEmriMIFDetay d = new OdemeEmriMIFDetay(DateTime.Now.Year);
                        d.satirNo = 999999;
                        d.hesapKod = "805";
                        d.detayBirim = oed.detayBirim;
                        d.borc = oed.alacak;
                        d.alacak = 0;

                        yansitma805.objeler.Add(d);
                    }
                }
            }

            foreach (OdemeEmriMIFDetay od in yansitma805.objeler)
            {
                odemeEmriDetay.objeler.Add(od);
            }
        }

        private class Sirala : IComparer
        {
            public int Compare(object o1, object o2)
            {
                if (o1 == o2) return 0;
                OdemeEmriMIFDetay i_o1 = (OdemeEmriMIFDetay)o1;
                OdemeEmriMIFDetay i_o2 = (OdemeEmriMIFDetay)o2;
                return (i_o1.satirNo.ToString("000000") + i_o1.hesapKod).CompareTo(i_o2.satirNo.ToString("000000") + i_o2.hesapKod);
            }
        }
    }
}
