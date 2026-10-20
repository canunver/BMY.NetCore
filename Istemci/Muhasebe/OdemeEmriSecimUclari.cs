using System;
using System.Collections;
using System.Collections.Generic;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.MUH;

namespace Istemci.Muhasebe
{
    /// <summary>
    /// Ödeme Emri / MİF formunun (eski ButceMuhasebe/OdemeEmriMIF2.aspx) açtığı küçük seçim / gösterim pencerelerinin uçları.
    /// Pencereleri wwwroot/js/odemeemrisecim.js (Sgb.oem) kurar. Hepsi GET'tir (yalnızca okuma):
    ///   GET /api/oemsecim/mahsup?yil=&amp;muhasebe=&amp;birim=&amp;hesapKod=&amp;ilgiliNo=&amp;ilgiliAd=&amp;yevmiye=1&amp;kapatilmis=1&amp;goster=0|1|2
    ///                                       mahsup edilecek belgeler (eski ListeMahsupBelge.aspx btnListele_Click)
    ///   GET /api/oemsecim/fatura?yil=&amp;muhasebe=&amp;birim=&amp;ilgiliNo=&amp;ilgiliAd=&amp;islendi=1
    ///                                       süreç faturaları (eski ListeSurecFatura.aspx btnListele_Click)
    ///   GET /api/oemsecim/is?yil=&amp;muh=&amp;muhBir=       iş listesi (eski ListeIs.aspx Listele)
    ///   GET /api/oemsecim/kdv                 kesinti hesabındaki KDV oranları (eski KesintiHesapla2.aspx, ButceMuhasebeGenel.KDVListesi)
    ///   GET /api/oemsecim/taahhut?yil=&amp;vergiNo=  devam eden taahhüt kartları (eski TaahhutKartiListe.aspx Listele)
    ///   GET /api/oemsecim/kapatma?yil=&amp;belgeNo=  kapatılan / kapatan belgeler (eski ListeKapatma.aspx Listele)
    ///   GET /api/oemsecim/tarihce?yil=&amp;belgeNo=  belge tarihçesi (eski BelgeTarihce.aspx; belge türü verilmez, eski gibi 0)
    ///   GET /api/oemsecim/muavin?yil=&amp;hesapKod=&amp;altHesapKod=&amp;projeNo=   alt hesap bazında muavin defteri
    ///                                       (eski MuavinDefteriAltHesapBazindaRapor.aspx)
    ///   GET /api/oemsecim/hesap/agac, hesap/ara, hesap/ad   mahsup penceresindeki hesap kodu (Sgb.hesapKodu / Sgb.hesapPlaniSec;
    ///                                       Makbuz uçlarının aynısı, yetki bu formdan)
    /// Yetki: pencereler menüde yoktur; yetki üst sayfadan gelir (Ödeme Emri/MİF çerçevesinin menüdeki üç adresinden biri).
    /// Muavin defteri ayrıca Alacak Takip "Takip İşlemleri" ekranından (alacaktakip.js) açıldığı için o sayfanın yetkisiyle de okunur.
    /// </summary>
    public static class OdemeEmriSecimUclari
    {
        private const string Uc = "/api/oemsecim/";

        public const string FrameAdresi = "../ButceMuhasebe/OdemeEmriMIFFrame.aspx";
        public const string MuavinAdresi = "../ButceMuhasebe/MuavinDefteriAltHesapBazindaRapor.aspx";

        /// <summary>Ödeme Emri/MİF formunun menüdeki adresleri (eski, yeni ve başlık bilgileri).</summary>
        public static readonly string[] UstSayfalar =
        {
            FrameAdresi,
            FrameAdresi + "?yeni=1",
            FrameAdresi + "?yeni=1&baslikSayfasi=1"
        };

        /// <summary>Muavin defteri penceresinin üst sayfaları: Ödeme Emri/MİF formu ve Alacak Takip "Takip İşlemleri" (alacaktakip.js).</summary>
        public static readonly string[] MuavinUstSayfalar =
        {
            FrameAdresi,
            FrameAdresi + "?yeni=1",
            FrameAdresi + "?yeni=1&baslikSayfasi=1",
            "../AlacakTakip/AlacakTakipListe.aspx",
            "../ButceMuhasebe/TaahhutKarti.aspx",
            "../ButceMuhasebe/TaahhutKarti.aspx?tur=2"
        };

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "mahsup", Mahsup);
            app.MapGet(Uc + "fatura", Fatura);
            app.MapGet(Uc + "is", IsListe);
            app.MapGet(Uc + "kdv", Kdv);
            app.MapGet(Uc + "taahhut", Taahhut);
            app.MapGet(Uc + "kapatma", Kapatma);
            app.MapGet(Uc + "tarihce", Tarihce);
            app.MapGet(Uc + "muavin", Muavin);
            app.MapGet(Uc + "hesap/agac", HesapAgac);
            app.MapGet(Uc + "hesap/ara", HesapAra);
            app.MapGet(Uc + "hesap/ad", HesapAd);
        }

        // ------------------------------------------------------------------ yetki ve ortak

        /// <summary>Ödeme Emri/MİF formunun pencereleri: üç form adresinden birinde yetkisi olan kullanır.</summary>
        public static bool YetkiVar(Kullanici kullanan, string klasor)
        {
            return SayfaYetki.Yetkili(kullanan, klasor, "", UstSayfalar);
        }

        /// <summary>Muavin defteri (pencere ve Razor sayfası): Ödeme Emri/MİF formu ya da Alacak Takip yetkisi.</summary>
        public static bool MuavinYetkiVar(Kullanici kullanan, string klasor)
        {
            return SayfaYetki.Yetkili(kullanan, klasor, MuavinAdresi, MuavinUstSayfalar);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = Temiz(hata).Replace("<br>", " ").Replace("<br/>", " ");
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        private static bool Isaretli(string deger)
        {
            deger = Temiz(deger).ToLowerInvariant();
            return deger == "1" || deger == "true" || deger == "evet";
        }

        /// <summary>Eski TNSDateTime.ToString() (kısa tarih, gg.aa.yyyy): eski sayfa üst sayfaya bu biçimi veriyordu.</summary>
        private static string KisaTarih(TNSDateTime t)
        {
            if (t == null || t.isNull) return "";
            return t.Oku().ToString("dd.MM.yyyy");
        }

        /// <summary>Tablo için tarih (yyyy-MM-dd); boşsa "".</summary>
        private static string IsoTarih(TNSDateTime t)
        {
            if (t == null || t.isNull || t.Oku() == DateTime.MinValue) return "";
            return t.Oku().ToString("yyyy-MM-dd");
        }

        private static int YilVeyaBuYil(int? yil)
        {
            int y = yil ?? 0;
            return y > 2000 ? y : DateTime.Now.Year;
        }

        // ------------------------------------------------------------------ mahsup belgesi (eski ListeMahsupBelge.aspx)

        private static IResult Mahsup(HttpContext baglam, IWebHostEnvironment ortam, int? yil, int? muhasebe, string birim, string hesapKod,
            string ilgiliNo, string ilgiliAd, string yevmiye, string kapatilmis, int? goster)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici _);
            if (ret != null) return ret;
            try
            {
                OdemeEmriMIFForm oeForm = new OdemeEmriMIFForm();
                bool kapatilmisGosterme = Isaretli(kapatilmis);

                oeForm.yil = yil ?? 0;
                oeForm.muhasebe = muhasebe ?? 0;
                oeForm.birim = Temiz(birim);
                oeForm.ilgiliVD = Temiz(hesapKod);

                oeForm.ilgiliNo = Temiz(ilgiliNo);
                if (string.IsNullOrEmpty(oeForm.ilgiliNo))
                    oeForm.ilgiliAd = Temiz(ilgiliAd);

                if (Isaretli(yevmiye))
                    oeForm.formDurum = (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;

                ObjectArray belgeler = TNS.MUH.Arac.Tanimla().MahsupEdilecekBelgeListele(oeForm, true);
                if (belgeler == null) return Hata("Belgeler okunamadı.");
                if (belgeler.sonuc != null && !belgeler.sonuc.islemSonuc) return Hata(HataMetni(belgeler.sonuc.hataStr, "Belgeler okunamadı."));

                int gosterSecim = goster ?? 0;
                List<object> liste = new List<object>();
                foreach (OdemeEmriMIFForm of in belgeler.objeler)
                {
                    if (kapatilmisGosterme)
                    {
                        double tutar = of.borcToplam + of.alacakToplam;
                        if (of.AVKTutar >= tutar) continue;
                    }

                    if (gosterSecim == 1 && of.alacakToplam > 0) continue;
                    if (gosterSecim == 2 && of.borcToplam > 0) continue;

                    liste.Add(new
                    {
                        MUHASEBE = of.muhasebe,
                        BIRIM = of.birim,
                        BELGENO = of.belgeNo,
                        BELGETARIH = KisaTarih(of.islemTarih),
                        ILGILI = of.ilgiliAd,
                        ILGILIKOD = of.ilgiliNo,
                        YEVMIYENO = of.YEVYevNo,
                        YEVMIYETARIH = KisaTarih(of.YEVYevTarih),
                        HESAPKOD = of.kurum,
                        BORC = of.borcToplam,
                        ALACAK = of.alacakToplam,
                        DOVIZ = of.AVKYevNo,
                        SATIRACIKLAMA = of.aciklama
                    });
                }
                return Results.Json(new { satirlar = liste });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Belgeler okunamadı."));
            }
        }

        // ------------------------------------------------------------------ süreç faturası (eski ListeSurecFatura.aspx)

        private static IResult Fatura(HttpContext baglam, IWebHostEnvironment ortam, int? yil, int? muhasebe, string birim,
            string ilgiliNo, string ilgiliAd, string islendi)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                TNS.HRC.FaturaTum form = new TNS.HRC.FaturaTum();
                form.yil = yil ?? 0;
                form.muhasebe = muhasebe ?? 0;
                form.birim = Temiz(birim);
                form.firmaKodu = Temiz(ilgiliNo);
                form.firmaAdi = "";
                if (form.firmaKodu.Length < 5)
                    form.firmaAdi = Temiz(ilgiliAd);
                if (form.birim == "000")
                    form.birim = "";
                form.tip = -1;

                if (form.firmaKodu == "" && form.firmaAdi == "")
                    return Hata("İlgilinin adı veya TCKN/VKN bilgilerini doldurun");

                TNS.HRC.FaturaTum[] faturaListe = TNS.HRC.Arac.Tanimla().ListeleFaturaTum(kullanan, form) ?? new TNS.HRC.FaturaTum[0];
                bool islendiGosterme = Isaretli(islendi);
                Hashtable fListe = new Hashtable();
                List<object> liste = new List<object>();
                foreach (TNS.HRC.FaturaTum of in faturaListe)
                {
                    if (islendiGosterme)
                    {
                        if (of.mIFYil > 0 && of.mIFNo != "" && of.firmaKodu == form.firmaKodu) continue;
                    }

                    // Bir süreçte başka firmaların da faturası olabiliyor: bu firmanın dışındakiler gösterilmez.
                    if (of.firmaKodu != form.firmaKodu) continue;

                    // Aynı faturadan ikinci kayıt gelirse gösterilmez.
                    string anahtar = of.surecYil + "-" + of.surecNo + "-" + of.firmaKodu + "-" + of.faturaNo;
                    if (OrtakFonksiyonlar.ConvertToInt(fListe[anahtar], 0) > 0) continue;

                    liste.Add(new
                    {
                        MUHASEBE = of.muhasebe,
                        BIRIM = of.birim,
                        SURECYIL = of.surecYil,
                        SURECNO = of.surecNo,
                        SURECYILNO = of.surecYil + "/" + of.surecNo,
                        FATURANO = of.faturaNo,
                        FATURATARIH = KisaTarih(of.faturaTarihi),
                        FATURATUTAR = of.faturaTutari,
                        FATURATUTARKDVLI = of.faturaTutariKDVLI
                    });

                    fListe[anahtar] = 1;
                }
                return Results.Json(new { satirlar = liste });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Faturalar okunamadı."));
            }
        }

        // ------------------------------------------------------------------ iş seçimi (eski ListeIs.aspx)

        private static IResult IsListe(HttpContext baglam, IWebHostEnvironment ortam, int? yil, int? muh, string muhBir)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                IsTanim form = new IsTanim();
                form.sorguYil = yil ?? 0;
                // Eski kodda muhasebeBirimKod önce muhasebe koduyla, hemen ardından birim koduyla yazılıyordu; sonuç birim kodu.
                form.muhasebeBirimKod = (muh ?? 0).ToString();
                form.muhasebeBirimKod = muhBir ?? "";

                ObjectArray liste = TNS.MUH.Arac.Tanimla().IsListele(kullanan, form);
                if (liste == null) return Hata("İş listesi okunamadı.");
                if (!liste.sonuc.islemSonuc) return Hata(HataMetni(liste.sonuc.hataStr, "İş listesi okunamadı."));

                List<object> satirlar = new List<object>();
                foreach (IsTanim t in liste.objeler)
                    satirlar.Add(new { kod = t.kod, adi = t.adi });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "İş listesi okunamadı."));
            }
        }

        // ------------------------------------------------------------------ kesinti hesabı (eski KesintiHesapla2.aspx)

        /// <summary>
        /// Eski ButceMuhasebeGenel.KDVListesi: /KDVLER değişkeni (boşsa "0,1,8,18,26"), virgül ya da noktalı virgülle ayrılmış.
        /// Sayıya çevrilemeyen değer -1 olur (eski sayfada o seçenek gizlenirdi).
        /// </summary>
        private static IResult Kdv(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici _);
            if (ret != null) return ret;
            try
            {
                string kdvLer = TNS.DEG.Arac.DegiskenDegerBul(0, "/KDVLER") + "";
                if (string.IsNullOrWhiteSpace(kdvLer))
                    kdvLer = "0,1,8,18,26";
                List<int> oranlar = new List<int>();
                foreach (string item in kdvLer.Split(',', ';'))
                    oranlar.Add(OrtakFonksiyonlar.ConvertToInt(item, -1));
                return Results.Json(new { oranlar = oranlar });
            }
            catch (Exception)
            {
                return Results.Json(new { oranlar = new[] { 0, 1, 8, 18, 26 } });
            }
        }

        // ------------------------------------------------------------------ taahhüt kartları (eski TaahhutKartiListe.aspx)

        private static IResult Taahhut(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string vergiNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                TaahhutKartiBilgi bilgi = new TaahhutKartiBilgi();
                bilgi.muhasebeKod = 0;
                bilgi.birimKod = "";
                // Eski kodda yıl süzgeci kapalıydı (bilgi.yil atanmıyordu); yalnızca müteahhit kimlik / vergi no ile devam edenler.
                bilgi.muteahhitKimlikNo = Temiz(vergiNo);
                bilgi.kapandi = 0;
                bilgi.sozTur = TaahhutKartiBilgi.ENUMSozTur.Normal;

                ObjectArray liste = TNS.MUH.Arac.Tanimla().TaahhutKartiListele(kullanan, bilgi, 0);
                if (liste == null) return Results.Json(new { satirlar = new List<object>() });
                if (!liste.sonuc.islemSonuc) return Hata(HataMetni(liste.sonuc.hataStr, "Taahhüt kartları okunamadı."));

                Dictionary<string, string> birimAdlari = new Dictionary<string, string>();
                Dictionary<int, string> muhasebeAdlari = new Dictionary<int, string>();
                List<object> satirlar = new List<object>();
                foreach (TaahhutKartiBilgi t in liste.objeler)
                {
                    if (!muhasebeAdlari.TryGetValue(t.muhasebeKod, out string muhasebeAd))
                    {
                        muhasebeAd = MuhasebeAd(t.muhasebeKod);
                        muhasebeAdlari[t.muhasebeKod] = muhasebeAd;
                    }
                    string anahtar = Temiz(t.birimKod) + "-" + t.muhasebeKod;
                    if (!birimAdlari.TryGetValue(anahtar, out string birimAd))
                    {
                        birimAd = BirimAd(t.muhasebeKod, t.birimKod);
                        birimAdlari[anahtar] = birimAd;
                    }

                    string odemeEmriYilNo = "";
                    if (t.odemeEmriYil > 0) odemeEmriYilNo = t.odemeEmriYil + " / " + t.odemeEmriNo;

                    string durumAdi;
                    if (t.kapandi == 1) durumAdi = "KAPANDI";
                    else if (t.kapandi == 9) durumAdi = "İPTAL";
                    else durumAdi = "DEVAM";

                    satirlar.Add(new
                    {
                        KOD = t.kod,
                        YIL = t.yil,
                        DURUMADI = durumAdi,
                        KAPANDI = t.kapandi,
                        PROJESIRANO = t.projeSiraNo,
                        MUHASEBEKOD = t.muhasebeKod,
                        MUHASEBEAD = muhasebeAd,
                        BIRIMKOD = t.birimKod,
                        BIRIMAD = birimAd,
                        KONU = t.konu,
                        MUTEAHHITKIMLIKNO = t.muteahhitKimlikNo,
                        MUTEAHHITUNVAN = t.muteahhitUnvan,
                        SOZLESMETARIHI = IsoTarih(t.sozlesmeTarihi),
                        IHALEBEDELI = t.ihaleBedeli,
                        ODEMEEMRIYIL = t.odemeEmriYil,
                        ODEMEEMRINO = t.odemeEmriNo,
                        ODEMEEMRIYILNO = odemeEmriYilNo
                    });
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Taahhüt kartları okunamadı."));
            }
        }

        /// <summary>Eski GenelIslemler.KodAd(11, muhasebe): MUHMUHASEBE.MUHASEBEAD.</summary>
        private static string MuhasebeAd(int muhasebeKod)
        {
            if (muhasebeKod <= 0) return "";
            try
            {
                return TNS.UZY.Arac.Tanimla().UzayDegeriStr(new Kullanici(), "MUHMUHASEBE", muhasebeKod.ToString(), true, "MUHASEBEAD") ?? "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>Eski GenelIslemler.KodAd(12, "birim-muhasebe"): muhasebe biriminin adı.</summary>
        private static string BirimAd(int muhasebeKod, string birimKod)
        {
            birimKod = Temiz(birimKod);
            if (muhasebeKod <= 0 || birimKod == "") return "";
            try
            {
                MuhasebeBirim bir = new MuhasebeBirim();
                bir.muhasebe = muhasebeKod;
                bir.birimKod = birimKod;
                ObjectArray oa = TNS.MUH.Arac.Tanimla().MuhasebeBirimListele(bir);
                if (oa != null && oa.ObjeSayisi >= 1) return ((MuhasebeBirim)oa.objeler[0]).birimAd ?? "";
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        // ------------------------------------------------------------------ kapatma listesi (eski ListeKapatma.aspx)

        private static IResult Kapatma(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string belgeNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            // Kısa yazılan numara (ör. 123) kurum koduyla tam belge numarasına tamamlanır; kapatılan / kapatan ayrımı tam numarayla yapılır.
            belgeNo = OdemeEmriMIFUclari.BelgeNoDuzenle(belgeNo);
            if ((yil ?? 0) == 0 || belgeNo == "") return Hata("Yıl ve/ya Belge No bilgisi alınamadı.");
            try
            {
                ObjectArray bilgiler = TNS.MUH.Arac.Tanimla().KapatmaListele(kullanan, yil.Value, belgeNo);
                List<object> kapatilan = new List<object>();
                List<object> kapatan = new List<object>();
                if (bilgiler != null && bilgiler.sonuc.islemSonuc && bilgiler.objeler.Count > 0)
                {
                    foreach (KapanisBilgileri bilgi in bilgiler.objeler)
                    {
                        object satir = new
                        {
                            YIL = bilgi.yil,
                            BELGENO = bilgi.belgeNo,
                            KYIL = bilgi.kYil,
                            KBELGENO = bilgi.kBelgeNo,
                            KHESAPKOD = bilgi.kHesapKod,
                            KILGILIVNO = bilgi.kIlgiliVno,
                            KTERTIP = bilgi.kTertip,
                            KTUTAR = bilgi.kTutar,
                            SIRANO = bilgi.siraNo,
                            BORCALACAK = bilgi.borcAlacak,
                            KBIRIMKOD = bilgi.kBirimKod
                        };
                        if (bilgi.belgeNo == belgeNo) kapatilan.Add(satir);
                        else kapatan.Add(satir);
                    }
                }
                return Results.Json(new { kapatilan = kapatilan, kapatan = kapatan });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Kapatma listesi okunamadı."));
            }
        }

        // ------------------------------------------------------------------ belge tarihçesi (eski BelgeTarihce.aspx)

        /// <summary>
        /// Ödeme emri / MİF formu belge türü vermeden açıyordu (belgeTur = 0); TABTarihce.Listele 0, ödeme emri ve MİF için belge türü
        /// süzmez, yılın o numaralı belgesinin bütün kayıtlarını getirir. Burada da aynısı yapılır.
        /// </summary>
        private static IResult Tarihce(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string belgeNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            belgeNo = Temiz(belgeNo);
            if ((yil ?? 0) == 0 || belgeNo == "") return Hata("Yıl ve/ya Belge No bilgisi alınamadı.");
            try
            {
                ObjectArray tVAN = TNS.MUH.Arac.Tanimla().TarihceListele(kullanan, yil.Value, (ENUMBelgeTuru)0, belgeNo);
                if (!tVAN.sonuc.islemSonuc) return Hata(HataMetni(tVAN.sonuc.hataStr, "Tarihçe okunamadı."));
                List<object> satirlar = new List<object>();
                int sira = 0;
                foreach (TarihceBilgisi t in tVAN.objeler)
                {
                    sira++;
                    satirlar.Add(new { sira = sira, islemTarih = t.islemTarih.ToLongString(), islem = t.islem, islemYapan = t.islemYapan, islemYapanAd = t.islemYapanAd });
                }
                return Results.Json(new { belge = yil + " / " + belgeNo, satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Tarihçe okunamadı."));
            }
        }

        // ------------------------------------------------------------------ alt hesap bazında muavin defteri (eski MuavinDefteriAltHesapBazindaRapor.aspx)

        private static IResult Muavin(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string hesapKod, string altHesapKod, string projeNo)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!MuavinYetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);

            hesapKod = Temiz(hesapKod);
            altHesapKod = Temiz(altHesapKod);
            if (hesapKod != "") hesapKod = hesapKod.Split('-')[0];
            if (altHesapKod != "") altHesapKod = altHesapKod.Split('-')[0];
            int y = yil ?? 0;
            try
            {
                TNSDateTime tarih1;
                TNSDateTime tarih2;
                if (y != 0)
                {
                    tarih1 = new TNSDateTime(new DateTime(y, 1, 1));
                    tarih2 = new TNSDateTime(new DateTime(y, 12, 31));
                }
                else
                {
                    tarih1 = new TNSDateTime(new DateTime(2008, 1, 1));
                    tarih2 = new TNSDateTime(new DateTime(DateTime.Now.Year, 12, 31));
                }

                OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
                oEmri.yil = y;
                oEmri.sorguIslemTarih = tarih1.ToString() + ";" + tarih2.ToString();
                oEmri.formDurum = 22;
                oEmri.sorguEkoSiraliMi = true;
                oEmri.projeNo = projeNo ?? "";

                OdemeEmriMIFDetay oEmriDetay = new OdemeEmriMIFDetay(y);
                oEmriDetay.hesapKod = hesapKod.Replace(".", "").Trim();
                oEmriDetay.altHesapKod = altHesapKod;
                oEmri.detay.objeler.Add(oEmriDetay);

                BuyukDefter[] mb = TNS.MUH.Arac.Tanimla().BuyukDefterListele(kullanan, oEmri, 0, 3000, "10") ?? new BuyukDefter[0];

                double tBorc = 0;
                double tAlacak = 0;
                List<object> satirlar = new List<object>();
                foreach (BuyukDefter item in mb)
                {
                    tBorc += item.borc;
                    tAlacak += item.alacak;
                    satirlar.Add(new
                    {
                        YEVMIYETARIH = IsoTarih(item.islemTarih),
                        ODEMEEMRIYIL = item.yil,
                        ODEMEEMRIKOD = item.belgeNo,
                        KODLAR = item.kur + " - " + item.fon + " - " + item.fin + " - " + item.eko,
                        ACIKLAMA = item.aciklama,
                        BORC = item.borc,
                        ALACAK = item.alacak,
                        BAKIYE = BakiyeStringUret(tBorc - tAlacak)
                    });
                }
                return Results.Json(new { hesapKod = hesapKod, altHesapKod = altHesapKod, satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Muavin defteri okunamadı."));
            }
        }

        private static string BakiyeStringUret(double tmpBakiye)
        {
            System.Globalization.CultureInfo tr = new System.Globalization.CultureInfo("tr-TR");
            if (tmpBakiye < 0) return Math.Abs(tmpBakiye).ToString("#,##0.00", tr) + " (A)";
            if (tmpBakiye > 0) return Math.Abs(tmpBakiye).ToString("#,##0.00", tr) + " (B)";
            return Math.Abs(tmpBakiye).ToString("0.00", tr);
        }

        // ------------------------------------------------------------------ hesap kodu (eski HesapKod.ascx; Sgb.hesapKodu / Sgb.hesapPlaniSec)
        // Makbuz uçlarının (MakbuzUclari.HesapAgac / HesapAra / HesapAd) aynısı; yetki Ödeme Emri/MİF formundan.

        private static object HesapDugum(HesapPlaniSatir h)
        {
            return new { kod = h.hesapKod, ad = h.hesapKod + " - " + h.aciklama, hesapAd = h.aciklama ?? "", altVar = !h.detay, detay = h.detay };
        }

        private static IResult HesapAgac(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string ust)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici _);
            if (ret != null) return ret;
            try
            {
                int y = YilVeyaBuYil(yil);
                ust = Temiz(ust);
                List<object> dugumler = new List<object>();
                if (ust == "")
                {
                    TNS.UZY.IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
                    for (int i = 1; i < 10; i++)
                    {
                        string ad = servisUZY.UzayDegeriStr(new Kullanici(), "MUHHESAPPLANI", y.ToString() + "-" + i.ToString(), true);
                        dugumler.Add(new { kod = i.ToString(), ad = i.ToString() + "-" + ad, hesapAd = ad ?? "", altVar = true, detay = false });
                    }
                    return Results.Json(new { dugumler = dugumler });
                }

                string[] hesapKirilimlar = ust.Split('.');
                HesapPlaniSatir hs = new HesapPlaniSatir();
                hs.yil = y;
                hs.hesapKod = ust;
                if (ust.Length < 3) hs.seviye = 1;
                else hs.seviye = hesapKirilimlar.Length + 1;

                IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
                ObjectArray hv = new ObjectArray();
                for (int i = 0; i < 10; i++)
                {
                    hv = servisMUH.HesapPlaniListele(new Kullanici(), hs);
                    if (hv.objeler.Count == 0) hs.seviye++;
                    else break;
                }
                foreach (HesapPlaniSatir h in hv.objeler) dugumler.Add(HesapDugum(h));
                return Results.Json(new { dugumler = dugumler });
            }
            catch (Exception)
            {
                return Hata("Hesap planı okunamadı.");
            }
        }

        private static IResult HesapAra(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string metin, string sonKirilim)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            metin = Temiz(metin);
            if (metin.Length < 2) return Hata("En az iki karakter yazınız.");
            try
            {
                HesapPlaniSatir h = new HesapPlaniSatir();
                h.yil = YilVeyaBuYil(yil);
                h.hesapKodHesapAd = metin;
                h.detay = sonKirilim != "0";
                ObjectArray hesap = TNS.MUH.Arac.Tanimla().HesapPlaniListele(kullanan, h);
                if (hesap == null || !hesap.sonuc.islemSonuc) return Hata(HataMetni(hesap?.sonuc.hataStr, "Arama yapılamadı."));
                List<object> satirlar = new List<object>();
                foreach (HesapPlaniSatir d in hesap.objeler)
                {
                    satirlar.Add(HesapDugum(d));
                    if (satirlar.Count >= 500) break;
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Arama yapılamadı.");
            }
        }

        private static IResult HesapAd(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string kod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            kod = Temiz(kod);
            if (kod == "") return Results.Json(new { ad = "", detay = false });
            try
            {
                int y = YilVeyaBuYil(yil);
                string hesYapi = TNS.MUH.Arac.DegiskenDegerBul(y, "/KODYAPI/KOD8/KODYAPI");
                if (!string.IsNullOrEmpty(hesYapi)) kod = OrtakFonksiyonlar.KodNoktaKoy(hesYapi, kod.Replace(".", ""));
                HesapPlaniSatir h = new HesapPlaniSatir();
                h.yil = y;
                h.hesapKod = kod;
                ObjectArray hv = TNS.MUH.Arac.Tanimla().HesapPlaniListele(kullanan, h);
                if (hv != null && hv.objeler != null)
                    foreach (HesapPlaniSatir d in hv.objeler)
                        if (d.hesapKod == kod) return Results.Json(new { kod = d.hesapKod, ad = d.aciklama ?? "", detay = d.detay });
                return Results.Json(new { kod = kod, ad = "", detay = false });
            }
            catch (Exception)
            {
                return Hata("Hesap kodu okunamadı.");
            }
        }
    }
}
