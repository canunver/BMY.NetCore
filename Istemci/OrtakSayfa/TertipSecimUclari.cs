using System;
using System.Collections.Generic;
using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.BHMv2;
using TNS.BKD;
using TNS.KYM;
using TNS.MUH;

namespace Istemci.OrtakSayfa
{
    /// <summary>
    /// Ortak tertip seçimi bileşeninin (wwwroot/js/tertipsec.js, Sgb.tertipSec) uçları. Eski sayfaların yerine geçer:
    ///   tur=bhm : ButceHazirlikv2/ListeTertip.aspx  (ödenek - gönderilen; muhasebe - birim verilirse birim kalan / gönderilen / kullanılan)
    ///   tur=muh : ButceMuhasebe/ListeTertip.aspx    (ödenek - harcanan - bloke; ekonomik kod iki düzeyliyse alt kodlardan seçilir)
    ///   GET /api/tertipsec/liste   tertip listesi (Listele, "Aradığım tertip listede yok" = hepsi=1)
    ///   GET /api/tertipsec/altkod  satırın ekonomik kodunun alt kodları (eski BeforeExpand)
    ///   GET /api/tertipsec/prg     program bütçe kodu (eski TertipPrgBul)
    ///   GET /api/tertipsec/kodlar  kıstas kutularının kod ağacı (eski KodAgac.aspx?kodTur= 1, 3, 4, 14)
    ///   GET /api/tertipsec/kodad   kıstas kutusuna yazılan kodun adı (eski KodAdGetir)
    /// Oturum açmış her kullanıcı kullanır; tertipleri iş kuralları kullanıcıya göre süzer. Sayfaların kendi işlemleri (Kaydet...) yetkiyi
    /// ayrıca denetler.
    /// </summary>
    public static class TertipSecimUclari
    {
        private const string Uc = "/api/tertipsec/";
        private const int VarsayilanSayi = 30;

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "liste", Liste);
            app.MapGet(Uc + "altkod", AltKod);
            app.MapGet(Uc + "prg", Prg);
            app.MapGet(Uc + "kodlar", Kodlar);
            app.MapGet(Uc + "kodad", KodAdi);
        }

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        private static string HataMetni(string hata, string varsayilan)
        {
            if (string.IsNullOrWhiteSpace(hata)) return varsayilan;
            int i = hata.IndexOf("HATA:", StringComparison.Ordinal);
            return i >= 0 ? hata.Substring(i + 5).Trim() : varsayilan;
        }

        /// <summary>Yıl: verilmemişse ya da ileri bir yılsa bu yıl (eski ListeTertip).</summary>
        private static int Yil(int? yil)
        {
            int y = yil ?? 0;
            return y <= 2000 || y > DateTime.Now.Year ? DateTime.Now.Year : y;
        }

        // ------------------------------------------------------------------ ortak iş kuralları (Ödenek Gönderme de kullanır)

        /// <summary>Eski TertipSayiKontrol: çok satır gelirse ilk 30'u gösterilir, bir sonraki listede sınır 30 artar.</summary>
        public static int TertipSayiKontrol(int gelenSay, ref int maxTertipSay, ref string uyari)
        {
            int satSay = gelenSay;
            if (satSay > maxTertipSay * 1.5)
            {
                uyari = string.Format("Verdiğiniz arama sonucunda {0} adet tertip listelenmiştir. Ekranda bu tertiplerden {1} adedi görüntülenmektedir. Lütfen kıstas alanlarına bilgi girerek gelecek listenin boyunu azaltınız!", satSay, maxTertipSay);
                satSay = VarsayilanSayi;
                maxTertipSay += VarsayilanSayi;
            }
            return satSay;
        }

        /// <summary>Program bütçe kodu (faaliyet - kurumsal - finans - ekonomik).</summary>
        public static string PrgKod(int yil, string[] kodlar)
        {
            string peb = OrtakFonksiyonlar.DiziElemanAl(kodlar, 4, "") ?? "";
            return TNS.BHMv2.Arac.ProgramButceKodaCevir(yil, 140, peb.Replace(".", "")) + "-" + OrtakFonksiyonlar.DiziElemanAl(kodlar, 0, "")
                + "-" + OrtakFonksiyonlar.DiziElemanAl(kodlar, 2, "") + "-" + OrtakFonksiyonlar.DiziElemanAl(kodlar, 3, "");
        }

        /// <summary>Eski ListeTertip PrgKodYap: faaliyet kodu yoksa boş.</summary>
        public static string PrgKodYap(int yil, string tertip)
        {
            string[] kodlar = (tertip ?? "").Split('-');
            if (string.IsNullOrWhiteSpace(TNS.MUH.Arac.DiziElemanAl(kodlar, 4, ""))) return "";
            return PrgKod(yil, kodlar);
        }

        /// <summary>Tertip metni: kur-fon-fin-eko[-peb].</summary>
        private static string TertipYaz(string[] kodlar)
        {
            string peb = TNS.MUH.Arac.DiziElemanAl(kodlar, 4, "");
            return TNS.MUH.Arac.DiziElemanAl(kodlar, 0, "") + "-" + TNS.MUH.Arac.DiziElemanAl(kodlar, 1, "") + "-" + TNS.MUH.Arac.DiziElemanAl(kodlar, 2, "")
                + "-" + TNS.MUH.Arac.DiziElemanAl(kodlar, 3, "") + (peb != "" ? "-" + peb : "");
        }

        /// <summary>Eski ButceHazirlikv2/ListeTertip TertipleriListele: ödenek ve gönderilen (ListeTertipOl).</summary>
        public static OdenekBilgi[] BhmTertipler(Kullanici kullanan, IBHMv2Servis servisBHMv2, int yil, ENUMOdenekKontrolTur okt,
            string kur, string eko, string peb, bool tumu, out bool mpOnayli, out string baslik, string fin = "")
        {
            string ekoKistas = eko;
            if (okt == ENUMOdenekKontrolTur.E2OG_E4OE_E2K || okt == ENUMOdenekKontrolTur.E2POG_E4POE_E2PK_E4PTALEP)
                ekoKistas = TNS.BHMv2.Arac.IkileEko(yil, ekoKistas, TNS.BKD.Arac.Eko2_3Bul(kullanan));

            string pebKistas = peb;
            if (okt == ENUMOdenekKontrolTur.E2OG_E4OE_E2K)
                pebKistas = "";
            Tertip kistasTertip = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, kur, "", fin ?? "", ekoKistas, pebKistas);

            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(0), kistasTertip.KodNoKirilimSay(0)));
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(1), kistasTertip.KodNoKirilimSay(1)));
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(2), kistasTertip.KodNoKirilimSay(2)));

            if (okt == ENUMOdenekKontrolTur.E2OG_E4OE_E2K || okt == ENUMOdenekKontrolTur.E2POG_E4POE_E2PK_E4PTALEP)
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), 2, TNS.BKD.Arac.Eko2_3Bul(kullanan)));
            else
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), 4));

            if (okt != ENUMOdenekKontrolTur.E2OG_E4OE_E2K)
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(4), kistasTertip.KodNoKirilimSay(4)));

            EkDegerParam[] ekBilgiler = new EkDegerParam[2];
            mpOnayli = servisBHMv2.MasrafPlaniGenelOnayDurumBul(kullanan, yil, "", ENUMMasrafPlanAsama.AHP) == 5;

            if (mpOnayli)
            {
                int ay = TNS.BHMv2.Arac.SerbestAyBul(yil) + 1;
                ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABMasrafPlaniBirim, 0, 0, "11;0/" + ay.ToString());
                baslik = "Masraf Pl. (1-" + (ay - 1).ToString() + ". aylar)";
            }
            else if (okt == ENUMOdenekKontrolTur.E2OG_E4OE_E2K || okt == ENUMOdenekKontrolTur.E4POG_E4POE_E4PK)
            {
                ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABButce, 0, 0, "06.00");
                baslik = "Ödenek (Son)";
            }
            else
            {
                ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, "06.00");
                baslik = "Baş. Ödeneği";
            }
            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABOdenekGonderme, 0, 0, "");
            return servisBHMv2.ListeTertipOl(kullanan, kistasTertip, kodBilgileri, ekBilgiler, !tumu, tumu) ?? new OdenekBilgi[0];
        }

        /// <summary>Eski ButceHazirlikv2/ListeTertip TertipleriListeleBirim: birime gönderilen, harcanan, bloke (ve masraf planı).</summary>
        public static OdenekBilgi[] BhmBirimTertipler(Kullanici kullanan, IBHMv2Servis servisBHMv2, int yil, int belgeTur, ENUMOdenekKontrolTur okt,
            int muhasebe, string birim, string kur, string eko, string peb, bool tumu)
        {
            bool mpOnayli = servisBHMv2.MasrafPlaniGenelOnayDurumBul(kullanan, yil, "", ENUMMasrafPlanAsama.AHP) == 5;

            string ortak000 = "";
            try { ortak000 = TNS.DEG.Arac.DegiskenDegerBul(yil, "/Ortak000"); } catch { }
            if (ortak000 == "1")
                birim = "";

            Tertip kistasTertip = new Tertip(yil, ENUMTabloKod.TABOdenekGonderme, kur, "", "", "", "", "@" + muhasebe + "." + birim);

            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(0), kistasTertip.KodNoKirilimSay(0)));
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(1), kistasTertip.KodNoKirilimSay(1)));
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(2), kistasTertip.KodNoKirilimSay(2)));

            string ekoKistas = eko;
            if ((belgeTur != 4 && okt != ENUMOdenekKontrolTur.E4POG_E4POE_E4PK) ||
                (belgeTur == 4 && (okt == ENUMOdenekKontrolTur.E2OG_E4OE_E2K || okt == ENUMOdenekKontrolTur.E2POG_E4POE_E2PK_E4PTALEP)))
            {
                ekoKistas = TNS.BHMv2.Arac.IkileEko(yil, ekoKistas, TNS.BKD.Arac.Eko2_3Bul(kullanan));
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), 2, TNS.BKD.Arac.Eko2_3Bul(kullanan)));
            }
            else
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), kistasTertip.KodNoKirilimSay(3)));

            string pebKistas = peb;
            if ((belgeTur != 4 && (okt == ENUMOdenekKontrolTur.E2OG_E4OE_E2K || okt == ENUMOdenekKontrolTur.E4POG_E4POE_E2K)) ||
                (belgeTur == 4 && okt == ENUMOdenekKontrolTur.E2OG_E4OE_E2K))
                pebKistas = "";
            else
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(4), kistasTertip.KodNoKirilimSay(4)));

            kistasTertip.kodlar[3] = ekoKistas;
            kistasTertip.kodlar[4] = pebKistas;

            EkDegerParam[] ekBilgiler;
            if (mpOnayli)
            {
                ekBilgiler = new EkDegerParam[4];
                ekBilgiler[3] = new EkDegerParam(ENUMTabloKod.TABMasrafPlaniBirim, 0, 0, "11");
            }
            else
                ekBilgiler = new EkDegerParam[3];

            if (TNS.MUH.Arac.OdenekUstuKontrolBul() == ENUMOdenekUstuKontrol.ODENEK)
                ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABButce, 0, 0, "");
            else
                ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABOdenekGonderme, 0, 0, ";");

            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABOdemeEmri, 0, 0, "");
            ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABHYBloke, 0, 0, "");

            return servisBHMv2.ListeTertipOl(kullanan, kistasTertip, kodBilgileri, ekBilgiler, !tumu, tumu) ?? new OdenekBilgi[0];
        }

        public static OdenekBilgi TertipBul(OdenekBilgi[] liste, string aranan)
        {
            if (liste == null) return null;
            foreach (OdenekBilgi ob in liste)
                if (ob != null && aranan == ob.tertip.TertipStrOl())
                    return ob;
            return null;
        }

        /// <summary>Eski HesapkoddanEkoBul: hesap kodunun ekonomik kodu.</summary>
        public static string HesapkoddanEkoBul(int yil, string hesapKod, IMUHServis servis)
        {
            string hesYapi = TNS.MUH.Arac.DegiskenDegerBul(yil, "/KODYAPI/KOD8/KODYAPI");
            hesapKod = OrtakFonksiyonlar.KodNoktaKoy(hesYapi, hesapKod.Replace(".", ""));

            HesapPlaniSatir h = new HesapPlaniSatir();
            h.yil = yil;
            h.hesapKod = hesapKod;
            try
            {
                h = servis.HesapKodOzellikAl(h);
            }
            catch
            {
                return "";
            }
            string ekoYapi = TNS.MUH.Arac.DegiskenDegerBul(yil, "/KODYAPI/KOD4/KODYAPI");
            return TNS.MUH.Arac.HesapkoddanEkonomikKodBul(yil, h, ekoYapi) ?? "";
        }

        /// <summary>Eski YilBelirle: yıllara sari süreçte bu yıl, değilse sürecin yılı.</summary>
        private static int SurecYili(Kullanici kullanan, TNS.HRC.IHRCServis servisHRC, int yil, string surecNo)
        {
            TNS.HRC.Surec surec = new TNS.HRC.Surec();
            surec.yil = yil;
            surec.surecNo = surecNo;
            ObjectArray surecListe = servisHRC.SurecListele(kullanan, surec);
            if (surecListe != null && surecListe.ObjeSayisi > 0 && ((TNS.HRC.Surec)surecListe.objeler[0]).yillaraSariIslemMi == 1)
                return DateTime.Now.Year;
            return yil;
        }

        // ------------------------------------------------------------------ uçlar

        /// <summary>
        /// Tertip listesi. İlk açılışta (ilk=1) kıstaslar eski sayfaların Page_Load'u gibi tamamlanır; boş kalanlara son olarak
        /// tarayıcının hatırladığı son kıstas (oturumKur, oturumEko: eski Session["Kurumsal"], Session["Ekonomik"]) yazılır.
        /// Kurumsal, finans, ekonomik ve faaliyetten en az ikisi dolu olmalıdır.
        /// </summary>
        private static IResult Liste(HttpContext baglam, string tur, int? yil, int? muhasebe, string birim, string kur, string fin, string eko, string peb,
            string hesapKod, int? belgeTur, string onay, string surecNo, int? surecYil, string hepsi, int? sayi, string ilk, string oturumKur, string oturumEko)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            try
            {
                bool bhm = Temiz(tur) == "bhm";
                bool ilkAcilis = ilk == "1";
                int iyil = Yil(yil);
                int muh = muhasebe ?? 0;
                int bt = belgeTur ?? 1;
                birim = Temiz(birim);
                kur = Temiz(kur);
                fin = Temiz(fin);
                eko = Temiz(eko);
                peb = Temiz(peb);
                hesapKod = Temiz(hesapKod);
                bool tumu = hepsi == "1";
                int maxTertipSay = sayi ?? VarsayilanSayi;
                if (maxTertipSay <= 0) maxTertipSay = VarsayilanSayi;

                ENUMOdenekUstuKontrol ouKontrol = TNS.MUH.Arac.OdenekUstuKontrolBul();
                ENUMOdenekKontrolTur okt = TNS.MUH.Arac.OdenekKontrolTurBul(iyil);
                bool anaGoster = true;

                if (!bhm)
                {
                    // Eski ButceMuhasebe/ListeTertip Page_Load: ana satırlar mı seçilir, ekonomik kodun alt düzeyi mi?
                    bool ego = false;
                    anaGoster = false;
                    if (bt == 1 || bt == 2)
                    {
                        if ((TNS.DEG.Arac.VarsayilanKurumBul() ?? "").Replace(".", "") == "460604") ego = true;
                        if (TNS.MUH.Arac.EkonomikIkiDuzeyKontrol(ego, hesapKod) == 2) anaGoster = true;
                    }
                    if (okt != ENUMOdenekKontrolTur.E4POG_E4POE_E2K)
                    {
                        if (bt == 4 || okt == ENUMOdenekKontrolTur.E4POG_E4POE_E4PK || (onay == "1" && ouKontrol == ENUMOdenekUstuKontrol.ODENEK))
                            anaGoster = true;
                        else if (bt == 3 && okt != ENUMOdenekKontrolTur.E2POG_E4POE_E2PK_E4PTALEP)
                            anaGoster = true;
                    }
                    else if (bt == 4)
                        anaGoster = true;

                    if (ilkAcilis)
                    {
                        IMUHServis muhServis = TNS.MUH.Arac.Tanimla();
                        // Muhasebe - birim verilmişse kurumsal kodları ondan bulunur.
                        if (muh > 0 && birim != "")
                        {
                            TNS.MUH.MuhasebeBirim mb = new TNS.MUH.MuhasebeBirim();
                            mb.muhasebe = muh;
                            mb.birimKod = birim;
                            ObjectArray mbler = muhServis.MuhasebeBirimListele(mb);
                            string kurKod = "";
                            if (mbler != null && mbler.objeler != null)
                                foreach (TNS.MUH.MuhasebeBirim mm in mbler.objeler)
                                {
                                    if (kurKod != "") kurKod += ",";
                                    kurKod += mm.kurum;
                                }
                            if (kurKod != "") kur = kurKod;
                        }
                        if (hesapKod != "") eko = HesapkoddanEkoBul(iyil, hesapKod, muhServis);

                        TNS.HRC.IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();
                        surecNo = Temiz(surecNo);
                        if (surecNo != "")
                        {
                            iyil = SurecYili(kullanan, servisHRC, surecYil > 2000 ? surecYil.Value : iyil, surecNo);
                            okt = TNS.MUH.Arac.OdenekKontrolTurBul(iyil);
                            if (eko == "") eko = Temiz(servisHRC.SurecEkonomikKodAl(kullanan, iyil, surecNo));
                        }
                        // EGO: ekonomik kod boşsa 0 (kurumsal kodun bütün tertipleri).
                        if (ego && eko == "") eko = "0";
                        if (kur == "" && muh > 0 && birim != "")
                            kur = Temiz(servisHRC.MuhasebeBirimKurumsalKodAl(muh, birim));
                    }
                }

                if (ilkAcilis)
                {
                    if (kur == "") kur = Temiz(oturumKur);
                    if (eko == "") eko = Temiz(oturumEko);
                }

                kur = kur.Replace(";", ",");
                fin = fin.Replace(";", ",");
                eko = eko.Replace(";", ",");
                peb = peb.Replace(";", ",");

                int doluAlan = (kur != "" ? 1 : 0) + (fin != "" ? 1 : 0) + (eko != "" ? 1 : 0) + (peb != "" ? 1 : 0);
                if (doluAlan < 2)
                {
                    return Results.Json(new
                    {
                        yil = iyil, anaGoster = anaGoster, mpOnayli = false, odenekBaslik = "", kur = kur, eko = eko,
                        uyari = ilkAcilis ? "" : "Lütfen kriter bölümünde, birden fazla alana değer giriniz.",
                        satirlar = new List<object>(), sayi = maxTertipSay
                    });
                }

                string uyari = "";
                List<object> liste = new List<object>();
                bool mpOnayli;
                string odenekBaslik = "Ödenek";
                IBHMv2Servis servisBHMv2 = TNS.BHMv2.Arac.Tanimla();

                if (bhm)
                {
                    OdenekBilgi[] obler = BhmTertipler(kullanan, servisBHMv2, iyil, okt, kur, eko, peb, tumu, out mpOnayli, out odenekBaslik, fin);
                    OdenekBilgi[] birimTertipleri = BhmBirimTertipler(kullanan, servisBHMv2, iyil, bt, okt, muh, birim, kur, eko, peb, tumu);
                    int satSay = TertipSayiKontrol(obler.Length, ref maxTertipSay, ref uyari);
                    foreach (OdenekBilgi ob in obler)
                    {
                        if (satSay != 0 && liste.Count >= satSay) break;
                        string tertip = ob.tertip.TertipStrOl();
                        string[] kodlar = tertip.Split('-');
                        double odenek = Convert.ToDouble(ob.ekRakamlar[0]);
                        double gonderilen = Convert.ToDouble(ob.ekRakamlar[1]);
                        double birimHarcanan = 0, birimGonderilen = 0, birimBloke = 0, mp = 0;
                        OdenekBilgi bob = TertipBul(birimTertipleri, tertip);
                        if (bob != null)
                        {
                            birimHarcanan = Convert.ToDouble(bob.ekRakamlar[1]);
                            birimGonderilen = Convert.ToDouble(bob.ekRakamlar[0]);
                            birimBloke = Convert.ToDouble(bob.ekRakamlar[2]);
                            if (mpOnayli) mp = Convert.ToDouble(bob.ekRakamlar[3]);
                        }
                        liste.Add(new
                        {
                            TERTIP = TertipYaz(kodlar),
                            TERTIPPRG = TNS.BHMv2.Arac.ProgramTertipOl(iyil, TNS.MUH.Arac.DiziElemanAl(kodlar, 0, ""), TNS.MUH.Arac.DiziElemanAl(kodlar, 1, ""),
                                TNS.MUH.Arac.DiziElemanAl(kodlar, 2, ""), TNS.MUH.Arac.DiziElemanAl(kodlar, 3, ""), TNS.MUH.Arac.DiziElemanAl(kodlar, 4, ""), false),
                            KALAN = odenek - gonderilen,
                            ODENEK = odenek,
                            GONDERILEN = gonderilen,
                            BIRIMKALAN = birimGonderilen - birimHarcanan - birimBloke,
                            BIRIMGONDERILEN = birimGonderilen,
                            BIRIMKULLANILAN = birimHarcanan + birimBloke,
                            MP = mp,
                            ACIKLAMA = ob.tertip.adlar.Length > 3 ? ob.tertip.adlar[3] : "",
                            FAALIYETACIKLAMA = ob.tertip.adlar.Length >= 5 ? ob.tertip.adlar[4] : ""
                        });
                    }
                }
                else
                {
                    OdenekBilgi[] obler = TNS.BHMv2.Arac.TertipListele(kullanan, servisBHMv2, bt, iyil, muh, birim, kur, "", fin, eko, peb, okt, ouKontrol, tumu, out mpOnayli)
                        ?? new OdenekBilgi[0];
                    int satSay = TertipSayiKontrol(obler.Length, ref maxTertipSay, ref uyari);
                    foreach (OdenekBilgi ob in obler)
                    {
                        if (satSay != 0 && liste.Count >= satSay) break;
                        string[] kodlar = ob.tertip.TertipStrOl().Split('-');
                        double odenek = Convert.ToDouble(ob.ekRakamlar[0]);
                        double harcanan = Convert.ToDouble(ob.ekRakamlar[1]);
                        double bloke = Convert.ToDouble(ob.ekRakamlar[2]);
                        liste.Add(new
                        {
                            TERTIP = TertipYaz(kodlar),
                            TERTIPPRG = PrgKod(iyil, kodlar),
                            KALAN = odenek - harcanan - bloke,
                            ODENEK = odenek,
                            HARCANAN = harcanan,
                            BLOKE = bloke,
                            MP = mpOnayli ? Convert.ToDouble(ob.ekRakamlar[3]) : 0,
                            ACIKLAMA = ob.tertip.adlar.Length > 3 ? ob.tertip.adlar[3] : "",
                            FAALIYETACIKLAMA = ob.tertip.adlar.Length >= 5 ? ob.tertip.adlar[4] : ""
                        });
                    }
                }

                return Results.Json(new
                {
                    yil = iyil,
                    anaGoster = anaGoster,
                    mpOnayli = mpOnayli,
                    odenekBaslik = odenekBaslik,
                    kur = kur,
                    eko = eko,
                    uyari = uyari,
                    satirlar = liste,
                    sayi = maxTertipSay
                });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Tertipler okunamadı."));
            }
        }

        /// <summary>
        /// Satırın ekonomik kodunun alt kodları (eski ButceMuhasebe/ListeTertip BeforeExpand). Ödenek kontrol türü E4POG_E4POE_E2K ise
        /// faaliyet kodu tertip tanımlarından seçilir (pebler).
        /// </summary>
        private static IResult AltKod(HttpContext baglam, int? yil, string tertip, string eko)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            try
            {
                int iyil = Yil(yil);
                string[] parca = Temiz(tertip).Split('-');
                if (parca.Length < 4) return Hata("Tertip hatalı.");
                string tertipOn = parca[0] + "-" + parca[1] + "-" + parca[2] + "-";
                string pebKod = TNS.MUH.Arac.DiziElemanAl(parca, 4, "");
                string tertipArka = pebKod != "" ? "-" + pebKod : "";
                ENUMOdenekKontrolTur okt = TNS.MUH.Arac.OdenekKontrolTurBul(iyil);

                ABSKodlarVAN ekoKodlar = TNS.BKD.Arac.Tanimla().KodListeleSeviye(iyil, "4", parca[3], 4, true, false, "", Temiz(eko).Replace(";", ","), null);

                List<object> pebler = new List<object>();
                string varsayilanPeb = "";
                if (okt == ENUMOdenekKontrolTur.E4POG_E4POE_E2K)
                {
                    Tertip kistasTertip = new Tertip(iyil, ENUMTabloKod.TABPEBOdenek, parca[0], parca[1], parca[2], parca[3]);
                    Tertip[] tler = TNS.BHMv2.Arac.Tanimla().TertipTanimlariListele(kullanan, kistasTertip);
                    List<string> eklenen = new List<string>();
                    if (tler != null && tler.Length > 0)
                    {
                        foreach (Tertip t in tler)
                        {
                            string icPeb = t.kodlar[4];
                            if (eklenen.Contains(icPeb)) continue;
                            eklenen.Add(icPeb);
                            pebler.Add(new { kod = icPeb, ad = GenelIslemler.KodAd(58, iyil + "-" + icPeb, true) });
                        }
                    }
                    else
                    {
                        eklenen.Add("01.01");
                        pebler.Add(new { kod = "01.01", ad = "Genel İşlemler" });
                    }
                    varsayilanPeb = eklenen[0];
                }

                List<object> satirlar = new List<object>();
                if (ekoKodlar != null && ekoKodlar.absKodlar != null)
                    foreach (ABSKod kod in ekoKodlar.absKodlar)
                    {
                        string t = tertipOn + kod.kod + tertipArka;
                        satirlar.Add(new { TERTIP = t, TERTIPPRG = PrgKodYap(iyil, t), EKO = kod.kod, PEB = varsayilanPeb, ACIKLAMA = kod.ad });
                    }
                return Results.Json(new { satirlar = satirlar, pebler = pebler });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Alt kodlar okunamadı."));
            }
        }

        private static IResult Prg(HttpContext baglam, int? yil, string tertip)
        {
            if (OturumBilgisi.KullaniciOku(baglam) == null) return Results.Unauthorized();
            try { return Results.Json(new { prg = PrgKodYap(Yil(yil), Temiz(tertip)) }); }
            catch (Exception) { return Results.Json(new { prg = "" }); }
        }

        /// <summary>Kod ağacı: 1 kurumsal, 3 finans, 4 ekonomik, 14 faaliyet. Düğüm adı "kod- Ad" biçimindedir.</summary>
        private static IResult Kodlar(HttpContext baglam, int? yil, string tur, string ust)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            tur = Temiz(tur);
            if (tur != "1" && tur != "3" && tur != "4" && tur != "14") return Hata("Kod türü hatalı.");
            return MPTavanUclari.KodAgaci(kullanan, Yil(yil), tur, ust);
        }

        /// <summary>Kodun adı (eski KodAdGetir): kur 1, fin 3, eko 4, peb 58. Birden çok kod (virgüllü) yazılmışsa ad boş.</summary>
        private static IResult KodAdi(HttpContext baglam, int? yil, string tur, string kod)
        {
            if (OturumBilgisi.KullaniciOku(baglam) == null) return Results.Unauthorized();
            kod = Temiz(kod);
            if (kod == "" || kod.IndexOf(',') >= 0 || kod.IndexOf(';') >= 0) return Results.Json(new { ad = "" });
            int kodTur = tur == "kur" ? 1 : tur == "fin" ? 3 : tur == "eko" ? 4 : tur == "peb" ? 58 : 0;
            if (kodTur == 0) return Hata("Kod türü hatalı.");
            string ad = MPTavanUclari.KodAd(Yil(yil), kodTur, kod);
            return Results.Json(new { ad = string.IsNullOrWhiteSpace(ad) ? "Hatalı kod girilmiş." : ad });
        }
    }
}
