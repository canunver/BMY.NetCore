using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.MUH;
using static Istemci.Muhasebe.MuhTanimOrtak;

namespace Istemci.Muhasebe
{
    public class MakbuzTipiIstegi
    {
        public int kod { get; set; }
        public string aciklama { get; set; }
        public string hesapKod { get; set; }
        public string krediHesapKod { get; set; }
        public string kdv { get; set; }
        public string kdvHesapKod { get; set; }
        public string kasaHesapKod { get; set; }
        public string yansitmaHesapKod { get; set; }
        public int durum { get; set; }
    }

    public class TeminatNeviIstegi
    {
        public string kod { get; set; }
        public string adi { get; set; }
        public string hesapKodu { get; set; }
    }

    public class VDAktarimIstegi
    {
        public int tur { get; set; }
        public int kod { get; set; }
        public string turKod { get; set; }
        public string adi { get; set; }
        public string hesapKod { get; set; }
        public string orani { get; set; }
    }

    public class YansitmaIstegi
    {
        public int yil { get; set; }
        public string hesapKod { get; set; }
        public string yansitmaHesapKod { get; set; }
        public string siraNo { get; set; }
    }

    public class YilIstegi
    {
        public int yil { get; set; }
    }

    public class KapaliBirimIstegi
    {
        public string muhasebe { get; set; }
        public string birimKod { get; set; }
        public string kurum { get; set; }
        public bool kapat { get; set; }
        public string yeniMuhasebe { get; set; }
        public string yeniBirim { get; set; }
    }

    /// <summary>
    /// Form + liste biçimindeki Muhasebe Tanımları sayfalarının uçları:
    ///   makbuztipi   : Makbuz Tipi Bilgileri (eski TanimMakbuzTipiYeni.aspx; Aktif / Pasif durumlu. Eski TanimMakbuzTipi.aspx Core'a alınmadı)
    ///   teminatnevi  : Teminat Mektubu Nevi Tanımlama (eski HarcamaSurecleri/TanimTeminatNevi.aspx)
    ///   vdaktarim    : Damga Vergisi / KDV Tevkifatı / KDV / Muhtasar Aktarım Tür Tanımlama (eski TanimVergiDairesiAktarimTur.aspx?tur=1..4)
    ///   kapalibirim  : Kapalı Birim Bilgileri (eski KapaliMuhasebeBirimForm.aspx)
    /// </summary>
    public static class MuhTanimBasitUclari
    {
        public const string MakbuzTipiYeniAdresi = "../ButceMuhasebe/TanimMakbuzTipiYeni.aspx";
        public const string TeminatNeviAdresi = "../HarcamaSurecleri/TanimTeminatNevi.aspx";
        public const string VDAktarimAdresi = "../ButceMuhasebe/TanimVergiDairesiAktarimTur.aspx";
        public const string KapaliBirimAdresi = "../ButceMuhasebe/KapaliMuhasebeBirimForm.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Kok + "makbuztipi/liste", MakbuzTipiListe);
            app.MapPost(Kok + "makbuztipi/kaydet", MakbuzTipiKaydet);
            app.MapPost(Kok + "makbuztipi/sil", MakbuzTipiSil);

            app.MapGet(Kok + "teminatnevi/liste", TeminatNeviListe);
            app.MapPost(Kok + "teminatnevi/kaydet", TeminatNeviKaydet);
            app.MapPost(Kok + "teminatnevi/sil", TeminatNeviSil);

            app.MapGet(Kok + "vdaktarim/liste", VDAktarimListe);
            app.MapPost(Kok + "vdaktarim/kaydet", VDAktarimKaydet);
            app.MapPost(Kok + "vdaktarim/sil", VDAktarimSil);


            app.MapGet(Kok + "kapalibirim/liste", KapaliBirimListe);
            app.MapPost(Kok + "kapalibirim/ackapat", KapaliBirimAcKapat);
            app.MapPost(Kok + "kapalibirim/personel-tasi", KapaliBirimPersonelTasi);
        }

        private static IMUHServis Servis() { return TNS.MUH.Arac.Tanimla(); }

        // ------------------------------------------------------------------ makbuz tipi

        /// <summary>Makbuz Tipi Bilgileri sayfasına yetkisi olan kullanır.</summary>
        public static bool MakbuzTipiYetkili(Kullanici kullanan, string klasor)
        {
            return Yetkili(kullanan, klasor, MakbuzTipiYeniAdresi);
        }

        private static object MakbuzTipiSatir(MakbuzTipi mt)
        {
            return new
            {
                kod = mt.kod,
                aciklama = mt.aciklama ?? "",
                hesapKod = mt.hesapKod ?? "",
                krediHesapKod = mt.krediHesapKod ?? "",
                kdv = mt.kdv,
                kdvHesapKod = mt.kdvHesapKod ?? "",
                kasaHesapKod = mt.kasaHesapKod ?? "",
                yansitmaHesapKod = mt.yansitmaHesapKod ?? "",
                durum = mt.durum,
                durumAd = mt.durum == 2 ? "Pasif" : "Aktif"
            };
        }

        private static MakbuzTipi MakbuzTipiOku(MakbuzTipiIstegi i, int bosKdv)
        {
            MakbuzTipi mt = new MakbuzTipi();
            mt.kod = i.kod;
            mt.aciklama = Temiz(i.aciklama);
            mt.hesapKod = Temiz(i.hesapKod);
            mt.krediHesapKod = Temiz(i.krediHesapKod);
            mt.kdv = OrtakFonksiyonlar.ConvertToInt(Temiz(i.kdv), bosKdv);
            mt.kdvHesapKod = Temiz(i.kdvHesapKod);
            mt.kasaHesapKod = Temiz(i.kasaHesapKod);
            mt.yansitmaHesapKod = Temiz(i.yansitmaHesapKod);
            mt.durum = i.durum;
            return mt;
        }

        /// <summary>Liste / Bul (eski Listele, btnAra_Click): boş kıstasla hepsi; Bul'da formdaki değerler kıstastır (KDV boşsa -1).</summary>
        private static IResult MakbuzTipiListe(HttpContext baglam, IWebHostEnvironment ortam, string aciklama, string hesapKod, string krediHesapKod,
            string kdv, string kdvHesapKod, string kasaHesapKod, string yansitmaHesapKod, int? durum)
        {
            return Oku(baglam, k => MakbuzTipiYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                MakbuzTipi kriter = MakbuzTipiOku(new MakbuzTipiIstegi
                {
                    aciklama = aciklama, hesapKod = hesapKod, krediHesapKod = krediHesapKod, kdv = kdv, kdvHesapKod = kdvHesapKod,
                    kasaHesapKod = kasaHesapKod, yansitmaHesapKod = yansitmaHesapKod, durum = durum ?? 0
                }, -1);
                ObjectArray mtler = Servis().MakbuzTipiListele(kullanan, kriter);
                if (!mtler.sonuc.islemSonuc) return Hata(HataMetni(mtler.sonuc.hataStr, "Liste okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (MakbuzTipi mt in mtler.objeler) satirlar.Add(MakbuzTipiSatir(mt));
                return Results.Json(new { satirlar = satirlar });
            });
        }

        private static Task<IResult> MakbuzTipiKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, MakbuzTipiIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => MakbuzTipiYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null) return Hata("İstek okunamadı.");
                ObjectArray mtler = new ObjectArray();
                mtler.objeler.Add(MakbuzTipiOku(istek, 0));
                return SonucYaniti(Servis().MakbuzTipiKaydet(kullanan, mtler), "Kayıt işlemi başarıyla tamamlandı.");
            });
        }

        private static Task<IResult> MakbuzTipiSil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, MakbuzTipiIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => MakbuzTipiYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null || istek.kod <= 0) return Hata("Lütfen silmek istediğiniz bilgiyi seçin.");
                ObjectArray mtler = new ObjectArray();
                mtler.objeler.Add(new MakbuzTipi { kod = istek.kod });
                return SonucYaniti(Servis().MakbuzTipiSil(kullanan, mtler), "Silme işlemi başarıyla gerçekleşti.");
            });
        }

        // ------------------------------------------------------------------ teminat mektubu nevi (HRC)

        private static bool TeminatYetkili(Kullanici kullanan, string klasor)
        {
            // Eski sayfa yalnızca SayfayaGirisYapabilirMi ile denetliyordu.
            return Yetkili(kullanan, klasor, TeminatNeviAdresi, false);
        }

        private static TNS.HRC.TeminatMektubuNevi TeminatOku(string kod, string adi, string hesapKodu)
        {
            TNS.HRC.TeminatMektubuNevi n = new TNS.HRC.TeminatMektubuNevi();
            n.kod = OrtakFonksiyonlar.ConvertToInt(Temiz(kod), 0);
            n.adi = Temiz(adi);
            n.hesapKodu = Temiz(hesapKodu);
            return n;
        }

        private static IResult TeminatNeviListe(HttpContext baglam, IWebHostEnvironment ortam, string kod, string adi, string hesapKodu)
        {
            return Oku(baglam, k => TeminatYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                ObjectArray yler = TNS.HRC.Arac.Tanimla().TeminatMektubuNeviListele(kullanan, TeminatOku(kod, adi, hesapKodu));
                if (yler == null) return Results.Json(new { satirlar = new object[0] });
                if (!yler.sonuc.islemSonuc) return Hata(HataMetni(yler.sonuc.hataStr, "Liste okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (TNS.HRC.TeminatMektubuNevi p in yler.objeler)
                    satirlar.Add(new { KOD = p.kod, ADI = p.adi ?? "", HESAPKODU = p.hesapKodu ?? "" });
                return Results.Json(new { satirlar = satirlar });
            });
        }

        private static Task<IResult> TeminatNeviKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TeminatNeviIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => TeminatYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null) return Hata("İstek okunamadı.");
                List<string> hatalar = new List<string>();
                if (Temiz(istek.kod) == "") hatalar.Add("Kod bilgisi boş bırakılmış.");
                if (Temiz(istek.adi) == "") hatalar.Add("Ad bilgisi boş bırakılmış.");
                if (hatalar.Count > 0) return Hata(string.Join("\n", hatalar) + "\nKayıt işlemi yapılamadı.");
                return SonucYaniti(TNS.HRC.Arac.Tanimla().TeminatMektubuNeviKaydet(kullanan, TeminatOku(istek.kod, istek.adi, istek.hesapKodu)),
                    "Kayıt işlemi başarıyla tamamlandı.");
            });
        }

        private static Task<IResult> TeminatNeviSil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TeminatNeviIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => TeminatYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null || Temiz(istek.kod) == "") return Hata("Lütfen silmek istediğiniz bilgiyi seçin.");
                return SonucYaniti(TNS.HRC.Arac.Tanimla().TeminatMektubuNeviSil(kullanan, TeminatOku(istek.kod, "", "")),
                    "Silme işlemi başarıyla gerçekleşti.");
            });
        }

        // ------------------------------------------------------------------ vergi dairesi aktarım türleri

        /// <summary>Menüdeki adres ?tur=N ile tanımlıdır; yetki o adresle denetlenir.</summary>
        public static bool VDAktarimYetkili(Kullanici kullanan, string klasor, int tur)
        {
            if (tur < 1 || tur > 4) return false;
            return Yetkili(kullanan, klasor, VDAktarimAdresi + "?tur=" + tur);
        }

        public static string VDAktarimBaslik(int tur)
        {
            switch (tur)
            {
                case (int)ENUMVergiDairesiAktarim.DAMGAVERGISI: return "Damga Vergisi Aktarımı Tanımlama Sayfası";
                case (int)ENUMVergiDairesiAktarim.KDVTEFKIFATI: return "KDV Tevkifatı Aktarımı Tanımlama Sayfası";
                case (int)ENUMVergiDairesiAktarim.KDV: return "KDV Aktarımı Tanımlama Sayfası";
                case (int)ENUMVergiDairesiAktarim.MUHTASAR: return "Muhtasar Aktarımı Tanımlama Sayfası";
                default: return "Aktarım Tür Tanımlama";
            }
        }

        // Eski sayfadaki olasiKodlar5: "Olmayanları Ekle" işaretliyse listede olmayan bu tür kod / adlar boş satır olarak eklenir.
        private static readonly (int kod, string ad)[] OlasiKodlar =
        {
            (1000, "gelirVergisi"), (1000, "tevkifatDamgaVergisi"), (1002, "012"), (1004, "302"),
            (2000, "041"), (2000, "021"), (2000, "284"),
            (10000, "beyanimVarYok"), (10000, "duzenleyen_adi"), (10000, "duzenleyen_alanKodu"), (10000, "duzenleyen_soyadi"),
            (10000, "duzenleyen_telNo"), (10000, "duzenleyen_vergiNo"), (10000, "hsv_adi"), (10000, "hsv_alanKodu"), (10000, "hsv_sifat"),
            (10000, "hsv_soyadi"), (10000, "hsv_telNo"), (10000, "hsv_vergiNo"), (10000, "mukellef_adi"), (10000, "mukellef_alanKodu"),
            (10000, "mukellef_soyadi"), (10000, "mukellef_telNo"), (10000, "mukellef_vergiNo"), (10000, "subeNo"), (10000, "vdKodu"),
            (11000, "belgeMahiyet"), (11000, "belgeTuru"), (11000, "isyeriAraciSiraNo"), (11000, "isyeriEskiSubeKod"), (11000, "isyeriIlKod"),
            (11000, "isyeriSiraNo"), (11000, "isyeriYeniSubeKod"), (11000, "kanun"), (11000, "tahakkukNeden"),
            (12000, "ad"), (12000, "agi"), (12000, "gun"), (12000, "gvEngellilikOran"), (12000, "gvKesinti"), (12000, "hesaplananGV"),
            (12000, "GVIstisnaAsgUcr"), (12000, "DVIstisnaAsgUcr"), (12000, "damgaVKesinti"), (12000, "hakEdilenUcret"), (12000, "iadeEdilecek"),
            (12000, "ilgiliDonemGvMatrah"), (12000, "mahsup"), (12000, "matrah"), (12000, "odenecek"), (12000, "odenecekDamgaVergisi"),
            (12000, "personelTip"), (12000, "personelTipES"), (12000, "primIkramiye"), (12000, "rapCalisti"), (12000, "sigortaliSicil"),
            (12000, "soyad"), (12000, "tahakkukEden"), (12000, "sgkTavan"), (12000, "eksikGunSayisi"), (12000, "eksikGunNedeni"),
            (12000, "iseBaslaTar"), (12000, "istenCikisTar"), (12000, "istenCikisNeden"), (12000, "meslekKod"),
            (12001, "matrah"), (12001, "tahakkukEden"), (12001, "mahsup"), (12001, "odenecek"), (12001, "iadeEdilecek"), (12001, "odenecekDamgaVergisi"),
            (13000, "isyeriTur"), (13000, "isyeriKod"), (13000, "ticaretSicilNo"), (13000, "ticaretSicilMudurluk"), (13000, "isyeriFaaliyetKod"),
            (13000, "isyeriAdi"), (13000, "isyeriAdresNo"), (13000, "isyeriMulkiyetDurum")
        };

        private static decimal Ondalik(string metin)
        {
            metin = Temiz(metin).Replace(',', '.');
            return decimal.TryParse(metin, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out decimal d) ? d : 0;
        }

        /// <summary>Liste / Bul (eski Listele, btnAra_Click). bul=1 ise formdaki değerler kıstastır (hesap kodundaki noktalar atılır).</summary>
        private static IResult VDAktarimListe(HttpContext baglam, IWebHostEnvironment ortam, int tur, int? bul, string turKod, string adi,
            string hesapKod, string orani, int? olmayanEkle)
        {
            return Oku(baglam, k => VDAktarimYetkili(k, ortam.ContentRootPath, tur), kullanan =>
            {
                VergiDairesiAktarimTurTanim kriter = new VergiDairesiAktarimTurTanim();
                if (bul == 1)
                {
                    kriter.turKod = OrtakFonksiyonlar.ConvertToInt(Temiz(turKod), 0);
                    kriter.hesapKod = Temiz(hesapKod).Replace(".", "");
                    kriter.adi = Temiz(adi);
                    if (Temiz(orani) != "") kriter.orani = Ondalik(orani);
                }
                kriter.aktarimTurKod = tur;
                ObjectArray dvler = Servis().DamgaVergisiAktarimTipiListele(kullanan, kriter);
                if (!dvler.sonuc.islemSonuc) return Hata(HataMetni(dvler.sonuc.hataStr, "Liste okunamadı."));
                if (olmayanEkle == 1)
                    foreach ((int kod, string ad) in OlasiKodlar)
                        if (string.IsNullOrWhiteSpace(VergiDairesiAktarimTurTanim.HesapKoduBul(dvler, kod, ad, true)))
                            dvler.objeler.Add(new VergiDairesiAktarimTurTanim { kod = 0, turKod = kod, adi = ad, aktarimTurKod = tur });

                List<object> satirlar = new List<object>();
                foreach (VergiDairesiAktarimTurTanim dv in dvler.objeler)
                    satirlar.Add(new { KOD = dv.kod, TURKOD = dv.turKod, ADI = dv.adi ?? "", HESAPKOD = dv.hesapKod ?? "", ORANI = dv.orani });
                return Results.Json(new { satirlar = satirlar });
            });
        }

        private static Task<IResult> VDAktarimKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, VDAktarimIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => istek != null && VDAktarimYetkili(k, ortam.ContentRootPath, istek.tur), kullanan =>
            {
                VergiDairesiAktarimTurTanim dv = new VergiDairesiAktarimTurTanim();
                dv.kod = istek.kod;
                dv.aktarimTurKod = istek.tur;
                dv.turKod = OrtakFonksiyonlar.ConvertToInt(Temiz(istek.turKod), 0);
                dv.hesapKod = Temiz(istek.hesapKod);
                dv.adi = Temiz(istek.adi);
                dv.orani = Ondalik(istek.orani);
                ObjectArray dvler = new ObjectArray();
                dvler.objeler.Add(dv);
                return SonucYaniti(Servis().DamgaVergisiAktarimTipiKaydet(kullanan, dvler), "Kayıt işlemi başarıyla tamamlandı.");
            });
        }

        private static Task<IResult> VDAktarimSil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, VDAktarimIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => istek != null && VDAktarimYetkili(k, ortam.ContentRootPath, istek.tur), kullanan =>
            {
                if (istek.kod <= 0) return Hata("Lütfen silmek istediğiniz bilgiyi seçin.");
                ObjectArray dvler = new ObjectArray();
                dvler.objeler.Add(new VergiDairesiAktarimTurTanim { kod = istek.kod });
                return SonucYaniti(Servis().DamgaVergisiAktarimTipiSil(kullanan, dvler), "Silme işlemi başarıyla gerçekleşti.");
            });
        }

        // ------------------------------------------------------------------ kapalı birimler

        private static bool KapaliBirimYetkili(Kullanici kullanan, string klasor)
        {
            // Eski sayfa yalnızca SayfayaGirisYapabilirMi ile denetliyordu.
            return Yetkili(kullanan, klasor, KapaliBirimAdresi, false);
        }

        private static MuhasebeBirim BirimOku(string muhasebe, string birimKod, string kurum)
        {
            MuhasebeBirim mb = new MuhasebeBirim();
            mb.muhasebe = OrtakFonksiyonlar.ConvertToInt(Temiz(muhasebe), 0);
            mb.birimKod = Temiz(birimKod);
            mb.kurum = Temiz(kurum);
            return mb;
        }

        private static IResult KapaliBirimListe(HttpContext baglam, IWebHostEnvironment ortam, string muhasebe, string birimKod, string kurum)
        {
            return Oku(baglam, k => KapaliBirimYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                ObjectArray oa = Servis().KapaliBirimListele(kullanan, BirimOku(muhasebe, birimKod, kurum));
                if (!oa.sonuc.islemSonuc) return Hata(HataMetni(oa.sonuc.hataStr, "Liste okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (MuhasebeBirim mb in oa.objeler)
                    satirlar.Add(new { muhasebe = mb.muhasebe, birimKod = mb.birimKod ?? "", kurum = mb.kurum ?? "", MuhasebeBirimAd = mb.MuhasebeBirimAd ?? "" });
                return Results.Json(new { satirlar = satirlar });
            });
        }

        /// <summary>Birim Kapat / Birim Aç (eski Kaydet(e, kapat)).</summary>
        private static Task<IResult> KapaliBirimAcKapat(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, KapaliBirimIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => KapaliBirimYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null) return Hata("İstek okunamadı.");
                Sonuc s = Servis().BirimAcKapat(kullanan, BirimOku(istek.muhasebe, istek.birimKod, istek.kurum), istek.kapat);
                return s.islemSonuc ? Tamam("İşlem başarıyla gerçekleşti.") : Hata(HataMetni(s.hataStr, "İşlem yapılamadı."));
            });
        }

        /// <summary>Veri Aktar > Personel Taşı (eski btnTasiPersonel_Click): birimdeki personel başka saymanlık / birime taşınır.</summary>
        private static Task<IResult> KapaliBirimPersonelTasi(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, KapaliBirimIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => KapaliBirimYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null) return Hata("İstek okunamadı.");
                int muhasebe = OrtakFonksiyonlar.ConvertToInt(Temiz(istek.muhasebe), 0);
                int yeniMuhasebe = OrtakFonksiyonlar.ConvertToInt(Temiz(istek.yeniMuhasebe), 0);
                if (muhasebe <= 0 || Temiz(istek.birimKod) == "") return Hata("Taşınacak muhasebe ve birim kodunu giriniz.");
                if (yeniMuhasebe <= 0 || Temiz(istek.yeniBirim) == "") return Hata("Aktarılacağı saymanlık ve birimi giriniz.");
                Sonuc s = TNS.PER.Arac.Tanimla().BirimKoduDegistir(kullanan, muhasebe, Temiz(istek.birimKod), yeniMuhasebe, Temiz(istek.yeniBirim));
                return s.islemSonuc ? Tamam("İşlem başarıyla gerçekleşti.") : Hata(HataMetni(s.hataStr, "İşlem yapılamadı."));
            });
        }
    }
}
