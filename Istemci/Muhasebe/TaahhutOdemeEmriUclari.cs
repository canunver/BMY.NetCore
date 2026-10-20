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
    /// <summary>Diğer Gelirler satırı (eski StoreDigerGelirler alanları).</summary>
    public class TaahhutDigerGelirSatiri
    {
        public string HESAPKOD { get; set; }
        public double ALACAK { get; set; }
        public string HESAPKODKDV { get; set; }
        public double ALACAKKDV { get; set; }
    }

    /// <summary>Taahhüt ödeme emri kaydı (eski TaahhutOdemeEmriKayit.aspx formu).</summary>
    public class TaahhutOdemeEmriIstegi
    {
        public int kod { get; set; }
        public int taahhutKod { get; set; }
        public int yil { get; set; }
        public string belgeNo { get; set; }
        public string tarih { get; set; }
        public string aciklama { get; set; }
        public string hakedisHesap { get; set; }
        public double hakedisTutari { get; set; }
        public string odemeHesap { get; set; }
        public string yansitmaHesap { get; set; }
        public string kdvHesapKodu { get; set; }
        public double kdv { get; set; }
        public string kdvHesapKodu2 { get; set; }
        public double kdv2 { get; set; }
        public string kdvHesapKodu3 { get; set; }
        public double kdv3 { get; set; }
        public string damgaVergisiHesapKodu { get; set; }
        public double damgaVergisi { get; set; }
        public string damgaVergisiHesapKodu2 { get; set; }
        public double damgaVergisi2 { get; set; }
        public string kdvTevkifatiHesapKodu { get; set; }
        public double kdvTevkifati { get; set; }
        public string gelirVergisiHesapKodu { get; set; }
        public double gelirVergisiStopaji { get; set; }
        public string paraCezalariHesapKodu { get; set; }
        public double paraCezalari { get; set; }
        public string mahsupEdileAvansHesapKodu { get; set; }
        public double mahsupEdilenAvans { get; set; }
        public string mahsupTertip { get; set; }
        public string mahsupBelgeNo { get; set; }
        public string icraKesintisiHesapKodu { get; set; }
        public double icraKesintisi { get; set; }
        public string teminatKesintisiHesapKodu { get; set; }
        public double teminatKesintisi { get; set; }
        public string sskPrimiGecikmeZammiHesapKodu { get; set; }
        public double sskPrimiVeGecikmeZammi { get; set; }
        public string temlikHesapKodu { get; set; }
        public double temlik { get; set; }
        public string fonPayiHesapKodu { get; set; }
        public double fonPayi { get; set; }
        public string vergiBorcuHesapKodu { get; set; }
        public double vergiBorcu { get; set; }
        public string butceEmanetiHesapKodu { get; set; }
        public double butceEmaneti { get; set; }
        public List<TaahhutDigerGelirSatiri> digerGelirler { get; set; }
    }

    /// <summary>
    /// Taahhüt Kartı &gt; Ödeme Emri penceresinden açılan Ödeme Emri Kayıt Sayfası (eski ButceMuhasebe/TaahhutOdemeEmriKayit.aspx).
    /// Hakediş için ödeme emri/MİF belgesi üretir ve taahhüdün ödeme kaydını yazar. Yetki Taahhüt Kartı sayfasından ve ödeme emri kullanıcı tipinden gelir.
    /// </summary>
    public static class TaahhutOdemeEmriUclari
    {
        public const string Kok = "/api/taahhutodemeemri/";
        public const string SayfaAdresi = "../ButceMuhasebe/TaahhutOdemeEmriKayit.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Kok + "tanim", Tanim);
            app.MapGet(Kok + "hesaplar", Hesaplar);
            app.MapGet(Kok + "yansitma", Yansitma);
            app.MapPost(Kok + "kaydet", Kaydet);
        }

        private static IMUHServis Servis() { return TNS.MUH.Arac.Tanimla(); }

        private static Func<Kullanici, bool> Yetki(IWebHostEnvironment ortam)
        {
            return k => TaahhutKartiUclari.AltSayfaYetkili(k, ortam.ContentRootPath, SayfaAdresi) && TaahhutKartiUclari.OdemeYetkili(k);
        }

        private static string Kod(string s) { return Temiz(s).Split('-')[0].Trim(); }

        private static List<object> HesapListesi(Kullanici kullanan, int yil, string hesapKodHesapAd, string hesapKod = null)
        {
            HesapPlaniSatir h = new HesapPlaniSatir();
            h.yil = yil;
            if (hesapKod != null) h.hesapKod = hesapKod;
            else h.hesapKodHesapAd = hesapKodHesapAd;
            h.detay = true;
            List<object> l = new List<object>();
            ObjectArray hesap = Servis().HesapPlaniListele(kullanan, h);
            if (hesap != null && hesap.objeler != null)
                foreach (HesapPlaniSatir d in hesap.objeler)
                    l.Add(new { kod = d.hesapKod, ad = d.hesapKod + " - " + d.aciklama });
            return l;
        }

        /// <summary>Eski ...HesapListesiDoldur yordamları (Fon Payı, Vergi Borcu, Temlik, Bütçe Emaneti listeleri eskiden de boştu).</summary>
        private static object HesapListeleri(Kullanici kullanan, int yil)
        {
            bool cb = TNS.HRC.Arac.CumhurbaskanligiMi();
            return new
            {
                kdv = HesapListesi(kullanan, yil, "191"),
                damga = HesapListesi(kullanan, yil, cb ? "600.01.05.01.01" : "360.03"),
                gelir = HesapListesi(kullanan, yil, cb ? "600.01.01.01.03" : "360.01"),
                tevkifat = HesapListesi(kullanan, yil, cb ? "600.01.03.01.02" : "360.04.02"),
                paraCeza = HesapListesi(kullanan, yil, "600.05.03.02.99"),
                mahsupAvans = HesapListesi(kullanan, yil, null, "259.01;160"),
                icra = HesapListesi(kullanan, yil, "333"),
                teminat = HesapListesi(kullanan, yil, "330"),
                ssk = HesapListesi(kullanan, yil, "361")
            };
        }

        private static List<object> YansitmaListesi(Kullanici kullanan, int yil, string hesapKod)
        {
            List<object> l = new List<object>();
            hesapKod = Kod(hesapKod);
            if (hesapKod == "") return l;
            ObjectArray hy = new ObjectArray();
            hy.objeler.Add(new HesapPlaniSatir() { yil = yil, hesapKod = hesapKod });
            ObjectArray hyv = Servis().HesapPlaniYansitmaListele(kullanan, hy);
            if (hyv != null && hyv.sonuc.islemSonuc)
                foreach (HesapPlaniYansitma hpy in hyv.objeler)
                    l.Add(new { kod = hpy.yansitmaHesapKod, ad = hpy.yansitmaHesapKod + "-" + hpy.yKodAciklama });
            return l;
        }

        private static string YansitmaBul(Kullanici kullanan, int yil, string hesapKod)
        {
            ObjectArray hy = new ObjectArray();
            hy.objeler.Add(new HesapPlaniSatir() { yil = yil, hesapKod = hesapKod });
            ObjectArray hyv = Servis().HesapPlaniYansitmaListele(kullanan, hy);
            if (hyv != null && hyv.sonuc.islemSonuc)
                foreach (HesapPlaniYansitma hpy in hyv.objeler)
                    return hpy.yansitmaHesapKod;
            return "";
        }

        /// <summary>Eski Page_Load + OdemeEmriGetir.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam, int? taahhutKod, int? kod, int? yil)
        {
            return Oku(baglam, Yetki(ortam), kullanan =>
            {
                if ((taahhutKod ?? 0) <= 0) return Hata("Taahhüt kartı bilgisi alınamadı.");
                TaahhutKartiBilgi d = TaahhutKartiUclari.KartOku(kullanan, taahhutKod.Value, out string hata);
                if (d == null) return Hata(hata);

                int y = (yil ?? 0) > 2000 ? yil.Value : DateTime.Now.Year;
                string aciklama = d.yil + "/" + d.projeSiraNo + " numaralı taahhüt dosyası kapsamında oluşturulmuştur. Sözleşme Tarihi:" + d.sozlesmeTarihi.ToString() + "; Sözleşme Süresi:" + d.iseBaslamaTarihi.ToString() + "-" + d.isBitirmeTarihi.ToString() + " tarihleri arasıdır.";

                object kayit = null;
                List<object> digerGelirler = new List<object>();
                List<object> yansitmalar = new List<object>();
                if ((kod ?? 0) > 0)
                {
                    ObjectArray liste = Servis().TaahhutOdemeEmriListele(kullanan, new TaahhutOdeme() { kod = kod.Value });
                    if (liste != null && liste.objeler != null)
                        foreach (TaahhutOdeme o in liste.objeler)
                        {
                            kayit = new
                            {
                                kod = o.kod,
                                taahhutKod = o.taahhutKod,
                                belgeNo = o.odemeEmriKod ?? "",
                                tarih = OdemeEmriMIFUclari.TarihYaz(o.islemTarihi),
                                aciklama = o.aciklama ?? "",
                                hakedisHesap = o.hakedisHesap ?? "",
                                hakedisTutari = o.hakedisTutari,
                                odemeHesap = o.odemeHesap ?? "",
                                yansitmaHesap = o.yansitmaHesap ?? "",
                                kdvHesapKodu = o.kdvHesapKodu ?? "",
                                kdv = o.kdv,
                                kdvHesapKodu2 = o.kdvHesapKodu2 ?? "",
                                kdv2 = o.kdv2,
                                kdvHesapKodu3 = o.kdvHesapKodu3 ?? "",
                                kdv3 = o.kdv3,
                                damgaVergisiHesapKodu = o.damgaVergisiHesapKodu ?? "",
                                damgaVergisi = o.damgaVergisi,
                                damgaVergisiHesapKodu2 = o.damgaVergisiHesapKodu2 ?? "",
                                damgaVergisi2 = o.damgaVergisi2,
                                kdvTevkifatiHesapKodu = o.kdvTevkifatiHesapKodu ?? "",
                                kdvTevkifati = o.kdvTevkifati,
                                gelirVergisiHesapKodu = o.gelirVergisiHesapKodu ?? "",
                                gelirVergisiStopaji = o.gelirVergisiStopaji,
                                paraCezalariHesapKodu = o.paraCezalariHesapKodu ?? "",
                                paraCezalari = o.paraCezalari,
                                mahsupEdileAvansHesapKodu = o.mahsupEdileAvansHesapKodu ?? "",
                                mahsupEdilenAvans = o.mahsupEdilenAvans,
                                icraKesintisiHesapKodu = o.icraKesintisiHesapKodu ?? "",
                                icraKesintisi = o.icraKesintisi,
                                teminatKesintisiHesapKodu = o.teminatKesintisiHesapKodu ?? "",
                                teminatKesintisi = o.teminatKesintisi,
                                sskPrimiGecikmeZammiHesapKodu = o.sskPrimiGecikmeZammiHesapKodu ?? "",
                                sskPrimiVeGecikmeZammi = o.sskPrimiVeGecikmeZammi,
                                temlikHesapKodu = o.temlikHesapKodu ?? "",
                                temlik = o.temlik,
                                fonPayiHesapKodu = o.fonPayiHesapKodu ?? "",
                                fonPayi = o.fonPayi,
                                vergiBorcuHesapKodu = o.vergiBorcuHesapKodu ?? "",
                                vergiBorcu = o.vergiBorcu,
                                butceEmanetiHesapKodu = o.butceEmanetiHesapKodu ?? "",
                                butceEmaneti = o.butceEmaneti
                            };
                            if (o.digerGelirler != null && o.digerGelirler.objeler != null)
                                foreach (TaahhutOdemeDiger da in o.digerGelirler.objeler)
                                    digerGelirler.Add(new { KOD = da.kod, HESAPKOD = da.hesapKod ?? "", ALACAK = da.alacak, HESAPKODKDV = da.hesapKodKDV ?? "", ALACAKKDV = da.alacakKDV });
                            yansitmalar = YansitmaListesi(kullanan, y, o.hakedisHesap);
                            break;
                        }
                }

                return Results.Json(new
                {
                    yil = y,
                    tarih = DateTime.Now.ToString("yyyy-MM-dd"),
                    aciklama = aciklama,
                    muhasebeKod = d.muhasebeKod,
                    birimKod = d.birimKod ?? "",
                    tertip = d.odenekTertip ?? "",
                    hesaplar = HesapListeleri(kullanan, y),
                    yansitmalar = yansitmalar,
                    kayit = kayit,
                    digerGelirler = digerGelirler
                });
            }, "Sayfa bilgileri okunamadı.");
        }

        private static IResult Hesaplar(HttpContext baglam, IWebHostEnvironment ortam, int? yil)
        {
            return Oku(baglam, Yetki(ortam), kullanan =>
            {
                int y = (yil ?? 0) > 2000 ? yil.Value : DateTime.Now.Year;
                return Results.Json(new { hesaplar = HesapListeleri(kullanan, y) });
            }, "Hesap listeleri okunamadı.");
        }

        /// <summary>Eski YansitmaListesiOlustur: hakediş hesabının yansıtma hesapları.</summary>
        private static IResult Yansitma(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string hesapKod)
        {
            return Oku(baglam, Yetki(ortam), kullanan =>
            {
                int y = (yil ?? 0) > 2000 ? yil.Value : DateTime.Now.Year;
                return Results.Json(new { satirlar = YansitmaListesi(kullanan, y, hesapKod) });
            }, "Yansıtma hesapları okunamadı.");
        }

        /// <summary>Eski btnKaydetOdeme_Click.</summary>
        private static Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TaahhutOdemeEmriIstegi istek)
        {
            return Yaz(baglam, sahtecilik, Yetki(ortam), kullanan =>
            {
                if (istek == null || istek.taahhutKod <= 0) return Hata("Taahhüt kartı bilgisi alınamadı.");
                TaahhutKartiBilgi kart = TaahhutKartiUclari.KartOku(kullanan, istek.taahhutKod, out string hata);
                if (kart == null) return Hata(hata);
                if (Kod(istek.hakedisHesap) == "") return Hata("Hakediş hesap kodunu giriniz.");
                if (Kod(istek.odemeHesap) == "") return Hata("Ödeme hesap kodunu giriniz.");

                TaahhutOdeme t = new TaahhutOdeme();
                t.kod = istek.kod;
                t.taahhutKod = istek.taahhutKod;
                t.odemeEmriKod = Temiz(istek.belgeNo);
                t.odemeEmriTarihi = OdemeEmriMIFUclari.Tarih(istek.tarih);
                t.aciklama = Temiz(istek.aciklama);
                t.hakedisTutari = istek.hakedisTutari;
                t.damgaVergisi = istek.damgaVergisi;
                t.gelirVergisiStopaji = istek.gelirVergisiStopaji;
                t.fonPayi = istek.fonPayi;
                t.paraCezalari = istek.paraCezalari;
                t.mahsupEdilenAvans = istek.mahsupEdilenAvans;
                t.icraKesintisi = istek.icraKesintisi;
                t.teminatKesintisi = istek.teminatKesintisi;
                t.sskPrimiVeGecikmeZammi = istek.sskPrimiVeGecikmeZammi;
                t.vergiBorcu = istek.vergiBorcu;
                t.kdvTevkifati = istek.kdvTevkifati;
                t.temlik = istek.temlik;
                t.butceEmaneti = istek.butceEmaneti;
                t.kdv = istek.kdv;
                t.kdv2 = istek.kdv2;
                t.kdv3 = istek.kdv3;
                t.damgaVergisi2 = istek.damgaVergisi2;

                double digerGelirler = 0.0;
                if (istek.digerGelirler != null)
                    foreach (TaahhutDigerGelirSatiri item in istek.digerGelirler)
                    {
                        string hesapKod = Kod(item.HESAPKOD);
                        if (hesapKod == "") continue;
                        TaahhutOdemeDiger to = new TaahhutOdemeDiger();
                        to.taahhutKod = t.taahhutKod;
                        to.taahhutOdemeKod = t.kod;
                        to.hesapKod = hesapKod;
                        to.hesapKodKDV = Kod(item.HESAPKODKDV);
                        to.alacak = item.ALACAK;
                        to.alacakKDV = item.ALACAKKDV;
                        t.digerGelirler.objeler.Add(to);
                        digerGelirler += to.alacak + to.alacakKDV;
                    }

                t.kesintilerToplami = t.damgaVergisi + t.damgaVergisi2 + t.gelirVergisiStopaji + t.fonPayi + t.paraCezalari + t.mahsupEdilenAvans
                    + t.icraKesintisi + t.teminatKesintisi + t.sskPrimiVeGecikmeZammi + t.vergiBorcu + t.kdvTevkifati + t.temlik + t.butceEmaneti + digerGelirler;
                t.yapilanOdeme = t.hakedisTutari + t.kdv + t.kdv2 + t.kdv3 - t.kesintilerToplami;
                t.islemTarihi = OdemeEmriMIFUclari.Tarih(istek.tarih);
                t.islemYapanKod = kullanan.TCKIMLIKNO;
                t.islemYapanAd = kullanan.ADSOYAD;

                t.hakedisHesap = Kod(istek.hakedisHesap);
                t.odemeHesap = Kod(istek.odemeHesap);
                t.yansitmaHesap = Temiz(istek.yansitmaHesap);
                t.kdvHesapKodu = Kod(istek.kdvHesapKodu);
                t.kdvHesapKodu2 = Kod(istek.kdvHesapKodu2);
                t.kdvHesapKodu3 = Kod(istek.kdvHesapKodu3);
                t.damgaVergisiHesapKodu = Kod(istek.damgaVergisiHesapKodu);
                t.damgaVergisiHesapKodu2 = Kod(istek.damgaVergisiHesapKodu2);
                t.gelirVergisiHesapKodu = Kod(istek.gelirVergisiHesapKodu);
                t.fonPayiHesapKodu = Kod(istek.fonPayiHesapKodu);
                t.paraCezalariHesapKodu = Kod(istek.paraCezalariHesapKodu);
                t.mahsupEdileAvansHesapKodu = Kod(istek.mahsupEdileAvansHesapKodu);
                t.icraKesintisiHesapKodu = Kod(istek.icraKesintisiHesapKodu);
                t.teminatKesintisiHesapKodu = Kod(istek.teminatKesintisiHesapKodu);
                t.sskPrimiGecikmeZammiHesapKodu = Kod(istek.sskPrimiGecikmeZammiHesapKodu);
                t.vergiBorcuHesapKodu = Kod(istek.vergiBorcuHesapKodu);
                t.kdvTevkifatiHesapKodu = Kod(istek.kdvTevkifatiHesapKodu);
                t.temlikHesapKodu = Kod(istek.temlikHesapKodu);
                t.butceEmanetiHesapKodu = Kod(istek.butceEmanetiHesapKodu);

                string odemeEmriNo = OdemeEmriKaydet(kullanan, kart, istek, t, out bool belgeOlustu);
                if (!belgeOlustu) return Hata("Ödeme emri oluşturma işlemi hata verdi.\n" + odemeEmriNo);
                t.odemeEmriKod = odemeEmriNo;

                ObjectArray liste = new ObjectArray();
                liste.Ekle(t);
                Sonuc sonuc = Servis().TaahhutOdemeEmriKaydet(kullanan, liste);
                if (sonuc == null || !sonuc.islemSonuc)
                    return Results.Json(new { hata = "Ödeme emri oluşturma işlemi hata verdi.\n" + HataMetni(sonuc?.hataStr, ""), belgeNo = odemeEmriNo });

                return Results.Json(new
                {
                    basarili = true,
                    bilgi = "Ödeme Emri/MİF kaydı yapıldı. Belge Numarası :" + odemeEmriNo + "\nBilgiler kayıt edildi.",
                    kod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, t.kod),
                    belgeNo = odemeEmriNo,
                    yil = istek.yil
                });
            });
        }

        private static TNS.MUH.OdemeEmriMIFDetay Satir(int yil, string hesapKod, double borc, double alacak, int kesinti = 0)
        {
            TNS.MUH.OdemeEmriMIFDetay td = new TNS.MUH.OdemeEmriMIFDetay(yil);
            td.hesapKod = hesapKod;
            td.borc = borc;
            td.alacak = alacak;
            td.kesinti = kesinti;
            return td;
        }

        /// <summary>Eski OdemeEmriKaydet: ödeme emri/MİF belgesini oluşturur. Başarılıysa belge numarasını, değilse hata metnini döndürür.</summary>
        private static string OdemeEmriKaydet(Kullanici kullanan, TaahhutKartiBilgi kart, TaahhutOdemeEmriIstegi istek, TaahhutOdeme t, out bool belgeOlustu)
        {
            belgeOlustu = false;
            IMUHServis servisMUH = Servis();
            string tertip = kart.odenekTertip ?? "";
            double sakliKDV = 0;

            TNS.MUH.OdemeEmriMIFForm o = new TNS.MUH.OdemeEmriMIFForm();
            o.yil = istek.yil > 2000 ? istek.yil : DateTime.Now.Year;
            o.birim = kart.birimKod;
            o.muhasebe = kart.muhasebeKod;
            o.belgeNo = Temiz(istek.belgeNo);
            o.islemTarih = OdemeEmriMIFUclari.Tarih(istek.tarih);
            o.kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
            o.formDurumTarih = new TNSDateTime(DateTime.Now.ToString());
            o.islemYapan = kullanan.kullaniciKodu;
            o.islemYapanAd = kullanan.ad + " " + kullanan.soyad;
            o.projeNo = kart.projeSiraNo;
            o.aciklama = t.aciklama;

            int oeYil = kart.odemeEmriYil;
            string oeBelgeNo = kart.odemeEmriNo ?? "";
            double acilisOdemeEmriTutar = kart.ihaleBedeli;
            string paraBirimiKod = kart.parabirimiKod ?? "";
            if (kart.degerArtisi != null && kart.degerArtisi.objeler != null)
                foreach (TaahhutKartiDegerArtisi da in kart.degerArtisi.objeler)
                    if (da.sozlesmeTutari > 0) acilisOdemeEmriTutar += da.sozlesmeTutari;

            decimal kurDegeri = 1;
            if (paraBirimiKod != "") kurDegeri = TaahhutKartiUclari.KurDegeri(kullanan, paraBirimiKod, o.islemTarih);

            TNS.HRC.FirmaBilgisi f = TNS.HRC.Arac.Tanimla().FirmaBilgisiListele(kullanan, o.muhasebe, kart.muteahhitKimlikNo);
            if (f != null)
            {
                o.ilgiliAd = f.ad;
                o.ilgiliBankaAd = f.banka;
                o.ilgiliBankaNo = f.hesapNo;
                o.ilgiliVD = f.vergiDairesi;
                o.ilgiliNo = f.vergiNo;
            }

            // Cumhurbaşkanlığı ise KDV tutarı hakediş hesabına eklenir, KDV olarak işlem yapılmaz
            if (TNS.HRC.Arac.CumhurbaskanligiMi())
            {
                sakliKDV = t.kdv;
                t.kdv = 0;
            }

            TNS.MUH.OdemeEmriMIFDetay td = Satir(o.yil, t.hakedisHesap, t.hakedisTutari + sakliKDV, 0);
            TNS.MUH.Arac.TertipAta(ref td, tertip);
            o.detay.objeler.Add(td);

            if (t.kdv > 0 && t.kdvHesapKodu != "") o.detay.objeler.Add(Satir(o.yil, t.kdvHesapKodu, t.kdv, 0));
            if (t.kdv2 > 0) o.detay.objeler.Add(Satir(o.yil, t.kdvHesapKodu2, t.kdv2, 0));
            if (t.kdv3 > 0) o.detay.objeler.Add(Satir(o.yil, t.kdvHesapKodu3, t.kdv3, 0));

            double paraCezasiYansitma = 0.0;
            double damgaVergisi1Yansitma = 0.0;
            double damgaVergisi2Yansitma = 0.0;
            double gelirVergisiStopajiYansitma = 0.0;
            double kdvTevkifatiYansitma = 0.0;

            if (t.damgaVergisi > 0)
            {
                o.detay.objeler.Add(Satir(o.yil, t.damgaVergisiHesapKodu, 0, t.damgaVergisi, 1));
                if (t.damgaVergisiHesapKodu.StartsWith("600")) damgaVergisi1Yansitma = t.damgaVergisi;
            }
            if (t.damgaVergisi2 > 0)
            {
                o.detay.objeler.Add(Satir(o.yil, t.damgaVergisiHesapKodu2, 0, t.damgaVergisi2, 1));
                if (t.damgaVergisiHesapKodu2.StartsWith("600")) damgaVergisi2Yansitma = t.damgaVergisi2;
            }

            foreach (TaahhutOdemeDiger od in t.digerGelirler.objeler)
            {
                o.detay.objeler.Add(Satir(o.yil, od.hesapKod, 0, od.alacak, 0));
                if (od.alacakKDV > 0) o.detay.objeler.Add(Satir(o.yil, od.hesapKodKDV, 0, od.alacakKDV, 0));
            }

            if (t.gelirVergisiStopaji > 0)
            {
                o.detay.objeler.Add(Satir(o.yil, t.gelirVergisiHesapKodu, 0, t.gelirVergisiStopaji, 1));
                if (t.gelirVergisiHesapKodu.StartsWith("600")) gelirVergisiStopajiYansitma = t.gelirVergisiStopaji;
            }
            if (t.fonPayi > 0) o.detay.objeler.Add(Satir(o.yil, t.fonPayiHesapKodu, 0, t.fonPayi, 1));
            if (t.paraCezalari > 0)
            {
                o.detay.objeler.Add(Satir(o.yil, t.paraCezalariHesapKodu, 0, t.paraCezalari, 1));
                paraCezasiYansitma = t.paraCezalari;
            }
            if (t.mahsupEdilenAvans > 0)
            {
                td = Satir(o.yil, t.mahsupEdileAvansHesapKodu, 0, t.mahsupEdilenAvans, 0);
                td.TertipAta(Temiz(istek.mahsupTertip));
                td.mahsupBelgeNo = Temiz(istek.mahsupBelgeNo);
                o.detay.objeler.Add(td);
            }
            if (t.icraKesintisi > 0) o.detay.objeler.Add(Satir(o.yil, t.icraKesintisiHesapKodu, 0, t.icraKesintisi, 1));
            if (t.teminatKesintisi > 0) o.detay.objeler.Add(Satir(o.yil, t.teminatKesintisiHesapKodu, 0, t.teminatKesintisi, 1));
            if (t.sskPrimiVeGecikmeZammi > 0) o.detay.objeler.Add(Satir(o.yil, t.sskPrimiGecikmeZammiHesapKodu, 0, t.sskPrimiVeGecikmeZammi, 1));
            if (t.vergiBorcu > 0) o.detay.objeler.Add(Satir(o.yil, t.vergiBorcuHesapKodu, 0, t.vergiBorcu, 1));
            if (t.kdvTevkifati > 0)
            {
                o.detay.objeler.Add(Satir(o.yil, t.kdvTevkifatiHesapKodu, 0, t.kdvTevkifati, 1));
                if (t.kdvTevkifatiHesapKodu.StartsWith("600")) kdvTevkifatiYansitma = t.kdvTevkifati;
            }
            if (t.temlik > 0) o.detay.objeler.Add(Satir(o.yil, t.temlikHesapKodu, 0, t.temlik, 1));
            if (t.butceEmaneti > 0) o.detay.objeler.Add(Satir(o.yil, t.butceEmanetiHesapKodu, 0, t.butceEmaneti, 0));

            o.detay.objeler.Add(Satir(o.yil, t.odemeHesap, 0, t.yapilanOdeme));

            if (!string.IsNullOrEmpty(t.yansitmaHesap))
            {
                td = Satir(o.yil, t.yansitmaHesap, t.hakedisTutari + t.kdv + t.kdv2 + t.kdv3, 0);
                TNS.MUH.Arac.TertipAta(ref td, tertip);
                o.detay.objeler.Add(td);

                td = Satir(o.yil, "835", 0, t.hakedisTutari + t.kdv + t.kdv2 + t.kdv3);
                TNS.MUH.Arac.TertipAta(ref td, tertip);
                o.detay.objeler.Add(td);
            }

            double digerYansitma = 0.0;
            foreach (TaahhutOdemeDiger od in t.digerGelirler.objeler)
            {
                if (!od.hesapKod.StartsWith("600")) continue;
                td = Satir(o.yil, YansitmaBul(kullanan, o.yil, od.hesapKod), 0, od.alacak + od.alacakKDV);
                o.detay.objeler.Add(td);
                digerYansitma += td.alacak;
            }
            if (damgaVergisi1Yansitma > 0) o.detay.objeler.Add(Satir(o.yil, YansitmaBul(kullanan, o.yil, t.damgaVergisiHesapKodu), 0, damgaVergisi1Yansitma));
            if (damgaVergisi2Yansitma > 0) o.detay.objeler.Add(Satir(o.yil, YansitmaBul(kullanan, o.yil, t.damgaVergisiHesapKodu2), 0, damgaVergisi2Yansitma));
            if (gelirVergisiStopajiYansitma > 0) o.detay.objeler.Add(Satir(o.yil, YansitmaBul(kullanan, o.yil, t.gelirVergisiHesapKodu), 0, gelirVergisiStopajiYansitma));
            if (kdvTevkifatiYansitma > 0) o.detay.objeler.Add(Satir(o.yil, YansitmaBul(kullanan, o.yil, t.kdvTevkifatiHesapKodu), 0, kdvTevkifatiYansitma));
            if (paraCezasiYansitma > 0) o.detay.objeler.Add(Satir(o.yil, YansitmaBul(kullanan, o.yil, t.paraCezalariHesapKodu), 0, paraCezasiYansitma));

            double yansitmaToplam = paraCezasiYansitma + digerYansitma + damgaVergisi1Yansitma + damgaVergisi2Yansitma + kdvTevkifatiYansitma + gelirVergisiStopajiYansitma;
            if (yansitmaToplam > 0) o.detay.objeler.Add(Satir(o.yil, "805", yansitmaToplam, 0));

            // Açılış tahakkuku belgesindeki 920 / 921 hesapları
            if (string.IsNullOrEmpty(oeBelgeNo.Trim()))
            {
                if (sakliKDV > 0) t.kdv = sakliKDV;
                return "Taahhüt Dosyası'nın tahakkuku yapılmamıştır. Lütfen ilk önce bu belgeyi oluşturun";
            }

            string hesap920 = "";
            string hesap921 = "";
            if (oeYil > 0)
            {
                TNS.MUH.OdemeEmriMIFForm oeTahakkuk = new TNS.MUH.OdemeEmriMIFForm();
                oeTahakkuk.yil = oeYil;
                oeTahakkuk.belgeNo = oeBelgeNo;
                ObjectArray oeTahListe = servisMUH.OdemeEmriMIFListele(kullanan, oeTahakkuk, true);
                if (oeTahListe != null && oeTahListe.objeler != null)
                    foreach (TNS.MUH.OdemeEmriMIFForm oem in oeTahListe.objeler)
                        foreach (TNS.MUH.OdemeEmriMIFDetay oed in oem.detay.objeler)
                        {
                            if (oed.hesapKod.StartsWith("921")) hesap921 = oed.hesapKod;
                            if (oed.hesapKod.StartsWith("920")) hesap920 = oed.hesapKod;
                        }

                if (hesap920 != "" && hesap921 != "")
                {
                    o.detay.objeler.Add(Satir(o.yil, hesap921, t.hakedisTutari, 0));
                    td = Satir(o.yil, hesap920, 0, t.hakedisTutari);
                    td.altHesapKod = o.ilgiliNo;
                    TNS.MUH.Arac.TertipAta(ref td, tertip);
                    o.detay.objeler.Add(td);
                }
            }

            // Hazırlanacak ödeme tutarı taahhüt tutarından fazla mı?
            double toplamOdemeEmriTutari = servisMUH.TaahhutOdemeEmriHazirlananToplamTutar(kullanan, t.taahhutKod);
            if (o.belgeNo != "")
            {
                OdemeEmriMIFForm form = new OdemeEmriMIFForm();
                form.yil = o.yil;
                form.belgeNo = o.belgeNo;
                ObjectArray flerVAN = servisMUH.OdemeEmriMIFTekBelgeListele(kullanan, form);
                if (flerVAN != null && flerVAN.sonuc.islemSonuc && flerVAN.objeler.Count > 0)
                {
                    OdemeEmriMIFForm ff = (OdemeEmriMIFForm)flerVAN.objeler[0];
                    // Belge daha önce düzenlenmiş ve şimdi değiştiriliyorsa önceki tutar taahhüt toplamından düşülür
                    if (ff.belgeNo != "")
                        foreach (OdemeEmriMIFDetay od in ff.detay.objeler)
                            if (od.hesapKod.Replace(".", "").StartsWith("920"))
                                toplamOdemeEmriTutari -= paraBirimiKod != "" ? od.dovizAlacak : od.alacak;
                }
            }

            double kalanOdemeEmriTutari = acilisOdemeEmriTutar - toplamOdemeEmriTutari;
            string pb = paraBirimiKod != "" ? paraBirimiKod : "TL";
            if (Math.Round(kalanOdemeEmriTutari, 2) < Math.Round(t.hakedisTutari, 2))
            {
                if (sakliKDV > 0) t.kdv = sakliKDV;
                return "Taahhüt Tutarı:" + acilisOdemeEmriTutar.ToString("#,###.00") + pb + ",\nÖnceden hazırlanan Ödeme Emri Tutarı:" + toplamOdemeEmriTutari.ToString("#,###.00") + pb
                    + ",\nKalan Tutar:" + kalanOdemeEmriTutari.ToString("#,###.00") + pb + ",\nbu bilgiler doğrultusunda " + t.hakedisTutari.ToString("#,###.00") + pb + "'lik belge hazırlanamaz.";
            }

            if (paraBirimiKod != "") o = TaahhutKartiUclari.OdemeEmriDetayDovizDonustur(servisMUH, kullanan, o, kurDegeri);

            Sonuc sonuc = servisMUH.OdemeEmriMIFKaydet(kullanan, o);
            if (sakliKDV > 0) t.kdv = sakliKDV;
            if (sonuc == null || !sonuc.islemSonuc) return HataMetni(sonuc?.hataStr, "Ödeme emri belgesi kaydedilemedi.");
            belgeOlustu = OrtakFonksiyonlar.ConvertToDecimal(sonuc.anahtar) != 0;
            return belgeOlustu ? sonuc.anahtar : HataMetni(sonuc.hataStr, "Ödeme emri belge numarası alınamadı.");
        }
    }
}
