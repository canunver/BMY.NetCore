using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ButceHazirlikv2;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.BHF;
using TNS.BHMv2;
using TNS.BKD;
using TNS.KYM;

namespace Istemci.ButceHazirlik
{
    public class BumkoIstegi
    {
        public int Yil { get; set; }
        public string Senaryo { get; set; }
        /// <summary>SBB aşaması: 0 Birim Teklifi, 1 İdare Teklifi, 7 Yatırım Vize.</summary>
        public int Asama { get; set; }
        public string Kullanici { get; set; }
        public string Parola { get; set; }
        /// <summary>odenek | gerekce | faaliyet</summary>
        public string Secim { get; set; }
        public string Eko { get; set; }
    }

    /// <summary>
    /// BÜMKO Aktarım sayfasının (eski ButceHazirlikv2/BUMKOVeriGonder.aspx) uçları:
    ///   GET  /api/bumko/tanim            hazırlık yılı, senaryolar, kullanıcının senaryosu ve kurum
    ///   POST /api/bumko/aktar            seçilen veriyi SBB program bütçe sistemine gönderir (eski btnAktar_Click):
    ///        odenek   = Program Bütçe (Ödenek, YSHT, Tavan Üstü), GiderButcesiKaydet
    ///        gerekce  = Bütçe Fiş/Ek Öd. Ger., tertip tertip ButceFisiKaydet
    ///        faaliyet = Faaliyet Gerekçesi, FaaliyetGerekceKaydet
    ///   POST /api/bumko/odenek-kontrol   Ödenek Kontrol raporu: SBB ayrıntılı harcama programı ile SGB masraf planı farkları
    ///        (şablon BHMv2\SBBSGBOdenekKontrol.xltx)
    /// e-Bütçe kullanıcı kodu ve parolası eski sayfadaki gibi ekrandan girilir; saklanmaz, hata metinlerine ve kütüğe yazılmaz.
    /// Eski sayfadaki gizli form seçenekleri (Form 6/7, 10, 11 ...) ve hiç çağrılmayan BelgeleriKontrolEt alınmadı.
    /// </summary>
    public static class BumkoAktarimUclari
    {
        public const string SayfaAdresi = ButceTanimOrtak.Klasor + "BUMKOVeriGonder.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/bumko/tanim", Tanim);
            app.MapPost("/api/bumko/aktar", Aktar);
            app.MapPost("/api/bumko/odenek-kontrol", OdenekKontrol);
        }

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = ButceTanimOrtak.Denetle(baglam, ortam, out Kullanici kullanan, SayfaAdresi);
            if (ret != null) return ret;
            try
            {
                List<object> senaryolar = new List<object>();
                foreach (ButceSenaryo bs in TNS.BHMv2.Arac.Tanimla().ButceSenaryoListele(kullanan, null) ?? new ButceSenaryo[0])
                    senaryolar.Add(new { kod = bs.kod, ad = bs.kod + "- " + bs.ad });
                string senaryo = ButceRakamUclari.KullaniciSenaryoBul(kullanan);
                string kurum = TNS.DEG.Arac.VarsayilanKurumBul() ?? "";
                return Results.Json(new
                {
                    yil = OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBulCache(0, "/BUTCEHAZIRLIK"), DateTime.Now.Year + 1),
                    senaryo = string.IsNullOrWhiteSpace(senaryo) ? "00.00" : senaryo,
                    senaryolar = senaryolar,
                    kurum = kurum + "-" + BHGiderRaporlari.KurumAd()
                });
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Hata("Sayfa bilgileri okunamadı.");
            }
        }

        private static string Denetle(BumkoIstegi istek)
        {
            if (istek == null || istek.Yil < 2003) return "Yıl bilgisi hatalı!";
            if (string.IsNullOrWhiteSpace(istek.Kullanici) || string.IsNullOrEmpty(istek.Parola)) return "e-Bütçe kullanıcı kodunu ve şifresini yazınız.";
            return null;
        }

        /// <summary>Aktar (eski btnAktar_Click).</summary>
        private static async Task<IResult> Aktar(HttpContext baglam, IWebHostEnvironment ortam, BumkoIstegi istek, IAntiforgery sahtecilik)
        {
            var (ret, kullanan) = await ButceTanimOrtak.DenetleYaz(baglam, ortam, sahtecilik, SayfaAdresi);
            if (ret != null) return ret;
            string eksik = Denetle(istek);
            if (eksik != null) return ButceTanimOrtak.Basarisiz(eksik);

            Aktarim a = new Aktarim(kullanan, istek);
            try
            {
                switch (istek.Secim)
                {
                    case "odenek": return await a.ProgramButceAktar(ButceTanimOrtak.Temiz(istek.Eko));
                    case "gerekce": return await a.ProgramButceGerekceAktar(ButceTanimOrtak.Temiz(istek.Eko));
                    case "faaliyet": return await a.FaaliyetGerekcesiAktar();
                    default: return ButceTanimOrtak.Basarisiz("Aktarılacak veriyi seçiniz.");
                }
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Basarisiz("Aktarım işlemi başarısız.");
            }
        }

        /// <summary>Ödenek Kontrol (eski btnOdenekKontrol_Click).</summary>
        private static async Task<IResult> OdenekKontrol(HttpContext baglam, IWebHostEnvironment ortam, BumkoIstegi istek, IAntiforgery sahtecilik)
        {
            var (ret, kullanan) = await ButceTanimOrtak.DenetleYaz(baglam, ortam, sahtecilik, SayfaAdresi);
            if (ret != null) return ret;
            string eksik = Denetle(istek);
            if (eksik != null) return ButceTanimOrtak.Hata(eksik);
            try
            {
                return await new Aktarim(kullanan, istek).OdenekKontrol();
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Hata("Rapor hazırlanamadı.");
            }
        }

        /// <summary>Bir aktarım isteğinin işleri (eski sayfanın alanları ve yordamları).</summary>
        private class Aktarim
        {
            private readonly Kullanici kullanan;
            private readonly int yil;
            private readonly int bumkoAsama;
            private readonly string kurumKod, eButceKullanici, eButceParola, senaryoKod;
            private readonly IBHMv2Servis servisBHMv2 = TNS.BHMv2.Arac.Tanimla();
            private readonly IBHFServis servisBHF = TNS.BHF.Arac.Tanimla();

            public Aktarim(Kullanici kullanan, BumkoIstegi istek)
            {
                this.kullanan = kullanan;
                yil = istek.Yil;
                bumkoAsama = istek.Asama;
                kurumKod = TNS.DEG.Arac.VarsayilanKurumBul();
                eButceKullanici = istek.Kullanici.Trim();
                eButceParola = istek.Parola;
                senaryoKod = string.IsNullOrWhiteSpace(istek.Senaryo) ? "00.00" : istek.Senaryo.Trim();
            }

            private static IResult Sonuc(bool basarili, string metin, string ayrinti = null)
            {
                if (basarili) return Results.Json(new { basarili = true, bilgi = metin, ayrinti = ayrinti ?? "" });
                return Results.Json(new { basarili = false, hata = metin, ayrinti = ayrinti ?? "" });
            }

            private void KutukYaz(string metin, bool yeni = false)
            {
                OrtakFonksiyonlar.HataDosyaYaz("BumkoVG.txt", metin, yeni);
            }

            private string Cevir(int kodNo, string kod)
            {
                if (kodNo < 10) return kod;
                try
                {
                    return TNS.BHMv2.Arac.ProgramButceKodaCevir(yil, kodNo, kod.Replace(".", ""));
                }
                catch
                {
                    return null;
                }
            }

            // -------------------------------------------------------------- Program Bütçe (Ödenek, YSHT, Tavan Üstü)

            /// <summary>Eski VerileriHazirla: tertipler program bütçe kodlarına çevrilir; çözülemeyen ya da birleşen varsa hata döner.</summary>
            private bool VerileriHazirla(string ekoKod, StringBuilder sonucYazi, List<SbbButceServisi.OdenekSatiri> listData)
            {
                StringBuilder listeYazi = new StringBuilder();
                TNS.BHMv2.Arac.ProgramButceKodCevirSifirla(yil);
                Tertip kistas = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, kurumKod, "", "", ekoKod);
                UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
                kodBilgileri.Add(new UcgenKodBilgi(1, kistas.KodNoKirilimSay(0)));
                kodBilgileri.Add(new UcgenKodBilgi(3, 1));
                kodBilgileri.Add(new UcgenKodBilgi(4, 4));
                kodBilgileri.Add(new UcgenKodBilgi(14, kistas.KodNoKirilimSay(4)));
                EkDegerParam[] ekBilgiler = new EkDegerParam[5];
                ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, senaryoKod);
                ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 1, senaryoKod);
                ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 2, senaryoKod);
                ekBilgiler[3] = new EkDegerParam(ENUMTabloKod.TABHarcamaTahmin, -1, 0, senaryoKod);
                ekBilgiler[4] = new EkDegerParam(ENUMTabloKod.TABPEBTavanUstu, 0, 0, senaryoKod);
                OdenekBilgi[] odnler = servisBHMv2.ListeTertipOl(kullanan, kistas, kodBilgileri, ekBilgiler, false);
                bool hata = false;
                if (odnler != null && odnler.Length > 0)
                {
                    foreach (OdenekBilgi odn in odnler)
                    {
                        bool rakamVar = false;
                        for (int i = 0; i < 5; i++)
                            if ((double)odn.ekRakamlar[i] > 0) rakamVar = true;
                        string absKur = odn.tertip.kodlar[0];
                        string absFin = odn.tertip.kodlar[2];
                        string absEko = odn.tertip.kodlar[3];
                        string absPEB = odn.tertip.kodlar[4];
                        TNS.BHMv2.Arac.ProgramButceKodaCevir(yil, 140, absPEB.Replace(".", ""), out long p, out long ap, out long f, out long prgPEB);
                        if (prgPEB > 0)
                        {
                            SbbButceServisi.OdenekSatiri oht = new SbbButceServisi.OdenekSatiri();
                            oht.KurKod = absKur;
                            oht.FinKod = absFin;
                            oht.EkoKod = absEko;
                            oht.ProgramId = p;
                            oht.AltProgramId = ap;
                            oht.FaaliyetId = f;
                            oht.AltFaaliyetId = prgPEB;
                            oht.teklif = (double)odn.ekRakamlar[0];
                            oht.tahmin1 = (double)odn.ekRakamlar[1];
                            oht.tahmin2 = (double)odn.ekRakamlar[2];
                            oht.ysht = (double)odn.ekRakamlar[3];
                            oht.odenekUstu = (double)odn.ekRakamlar[4];
                            listeYazi.Append($"Aktarılan\t{absKur}-{absFin}-{absEko}-{absPEB}\t{absKur}-{absFin}-{absEko}-{p}.{ap}.{f}.{prgPEB}\t{oht.teklif}\t{oht.tahmin1}\t{oht.tahmin2}\t{oht.ysht}\t{oht.odenekUstu}\r\n");
                            // Eski VeriVarMi her zaman -1 döndürüyordu: her tertip ayrı satır olarak gönderilir.
                            listData.Add(oht);
                        }
                        else if (rakamVar)
                        {
                            sonucYazi.Append($"{absKur}-{absFin}-{absEko}-{absPEB}: kod karşılığı çözülemedi!\r\n");
                            hata = true;
                        }
                    }
                }
                else
                {
                    sonucYazi.Append("Aktarılacak veri bulunamadı!\r\n");
                    hata = true;
                }
                KutukYaz(listeYazi.ToString(), true);
                return hata;
            }

            public async Task<IResult> ProgramButceAktar(string ekoKod)
            {
                StringBuilder sonucYazi = new StringBuilder();
                List<SbbButceServisi.OdenekSatiri> listData = new List<SbbButceServisi.OdenekSatiri>();
                if (VerileriHazirla(ekoKod, sonucYazi, listData))
                    return Sonuc(false, "Aktarım yapılmadı.", sonucYazi.ToString());
                try
                {
                    SbbButceServisi.WsSonuc snc = await SbbButceServisi.GiderButcesiKaydet(eButceKullanici, eButceParola, yil, bumkoAsama, Cevir(1, kurumKod), listData);
                    if (snc.islemSonuc)
                        return Sonuc(true, "Aktarım işlemi başarılı (" + listData.Count + " satır).");
                    return Sonuc(false, "Aktarım işlemi başarısız.", snc.hataStr);
                }
                catch (Exception ex)
                {
                    return Sonuc(false, "Aktarım işlemi başarısız, kullanıcı kodu ve şifresini kontrol ediniz.", ex.Message);
                }
            }

            // -------------------------------------------------------------- Bütçe Fiş / Ek Ödenek Gerekçeleri

            private string TavanUstuGerekceBul(string trtp)
            {
                KistasYapi tertip = new KistasYapi();
                tertip.ekleTertipler = new Tertip[1];
                tertip.ekleTertipler[0] = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, trtp);
                TavanUstuTertipOdenek[] tUstler = servisBHMv2.TavanUstuOdenekListele(kullanan, new string[] { senaryoKod }, tertip, true);
                if (tUstler != null && tUstler.Length > 0)
                    return tUstler[0].gerekce;
                return "";
            }

            private static string TertipKoduBul(SbbButceServisi.FisGerekcesi oht)
            {
                return $"{oht.ProgramId}.{oht.AltProgramId}.{oht.FaaliyetId}.{oht.AltFaaliyetId}-{oht.KurKod}-{oht.FinKod}-{oht.EkoKod}";
            }

            /// <summary>Eski ProgramButceGerekceAktar / GerekceleriGonder: rakamı olan her tertibin fiş ve tavan üstü gerekçesi.</summary>
            public async Task<IResult> ProgramButceGerekceAktar(string ekoKod)
            {
                StringBuilder sonucYazi = new StringBuilder();
                List<SbbButceServisi.FisGerekcesi> listData = new List<SbbButceServisi.FisGerekcesi>();
                TNS.BHMv2.Arac.ProgramButceKodCevirSifirla(yil);
                Tertip kistas = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, kurumKod, "", "", ekoKod);
                KutukYaz($"Başladı: {kurumKod} - {ekoKod}", true);
                UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
                kodBilgileri.Add(new UcgenKodBilgi(1, kistas.KodNoKirilimSay(0)));
                kodBilgileri.Add(new UcgenKodBilgi(2, 4));
                kodBilgileri.Add(new UcgenKodBilgi(3, 1));
                kodBilgileri.Add(new UcgenKodBilgi(4, 4));
                kodBilgileri.Add(new UcgenKodBilgi(14, kistas.KodNoKirilimSay(4)));
                EkDegerParam[] ekBilgiler = new EkDegerParam[4];
                ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, senaryoKod);
                ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 1, senaryoKod);
                ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 2, senaryoKod);
                ekBilgiler[3] = new EkDegerParam(ENUMTabloKod.TABPEBTavanUstu, 0, 0, senaryoKod);
                OdenekBilgi[] odnler = servisBHMv2.ListeTertipOl(kullanan, kistas, kodBilgileri, ekBilgiler, false);
                if (odnler == null || odnler.Length == 0)
                    return Sonuc(false, "Aktarım işlemi başarısız.", "Aktarılacak veri bulunamadı!");

                foreach (OdenekBilgi odn in odnler)
                {
                    if (!((double)odn.ekRakamlar[0] > 0 || (double)odn.ekRakamlar[1] > 0 || (double)odn.ekRakamlar[2] > 0 || (double)odn.ekRakamlar[3] > 0))
                        continue;
                    SbbButceServisi.FisGerekcesi oht = new SbbButceServisi.FisGerekcesi();
                    oht.Yil = yil;
                    oht.Asama = bumkoAsama;
                    oht.KurKod = odn.tertip.kodlar[0];
                    oht.FinKod = odn.tertip.kodlar[2];
                    oht.EkoKod = odn.tertip.kodlar[3];
                    string[] pafa = (Cevir(140, odn.tertip.kodlar[4]) ?? "").Split('.');
                    oht.ProgramId = OrtakFonksiyonlar.ConvertToLong(OrtakFonksiyonlar.DiziElemanAl(pafa, 0, "0"));
                    oht.AltProgramId = OrtakFonksiyonlar.ConvertToLong(OrtakFonksiyonlar.DiziElemanAl(pafa, 1, "0"));
                    oht.FaaliyetId = OrtakFonksiyonlar.ConvertToLong(OrtakFonksiyonlar.DiziElemanAl(pafa, 2, "0"));
                    oht.AltFaaliyetId = OrtakFonksiyonlar.ConvertToLong(OrtakFonksiyonlar.DiziElemanAl(pafa, 3, "0"));
                    if (oht.AltFaaliyetId <= 0)
                        return Sonuc(false, "Aktarım işlemi başarısız.", $"{odn.tertip.TertipStrOl()} kod karşılığı çözülemedi!");

                    oht.FisAciklama = ButceFisiIcerik.GerekceGetir(kullanan, yil, odn.tertip.TertipStrOl(), false, out string baslangicFark, out string sonrakiFark);
                    oht.TavanUstuTalepSebep = "";
                    if ((double)odn.ekRakamlar[3] > 0)
                        oht.TavanUstuTalepSebep = TavanUstuGerekceBul(odn.tertip.TertipStrOl());
                    if (!string.IsNullOrEmpty(oht.FisAciklama) || !string.IsNullOrEmpty(oht.TavanUstuTalepSebep))
                        listData.Add(oht);
                }

                int gonderilen = 0;
                try
                {
                    foreach (SbbButceServisi.FisGerekcesi oht in listData)
                    {
                        SbbButceServisi.WsSonuc snc = await SbbButceServisi.ButceFisiKaydet(eButceKullanici, eButceParola, oht);
                        if (!snc.islemSonuc)
                        {
                            string mtn = $"Bütçe fişi veya gerekçe aktarılamadı ({TertipKoduBul(oht)}): {snc.hataStr}";
                            sonucYazi.Append(mtn + "\r\n");
                            KutukYaz(mtn);
                        }
                        else
                            gonderilen++;
                    }
                }
                catch (Exception ex)
                {
                    string mtn = "Aktarım işlemi başarısız, kullanıcı kodu ve şifresini kontrol ediniz: " + ex.Message;
                    sonucYazi.Append(mtn + "\r\n");
                    KutukYaz(mtn);
                }
                KutukYaz("Bitti");
                if (sonucYazi.Length == 0)
                    return Sonuc(true, "Aktarım işlemi başarılı (" + gonderilen + " gerekçe).");
                return Sonuc(false, "Aktarım işlemi başarısız (" + gonderilen + " / " + listData.Count + " gerekçe gönderildi).", sonucYazi.ToString());
            }

            // -------------------------------------------------------------- Faaliyet Gerekçesi

            /// <summary>Eski FaaliyetGerekcesiAktar (Hizmet Gerekçesi Aktarımı).</summary>
            public async Task<IResult> FaaliyetGerekcesiAktar()
            {
                StringBuilder sonucYazi = new StringBuilder();
                int eklenen = 0;
                ObjectArray liste = servisBHF.FaaliyetGerekceListele(kullanan, yil, TNS.DEG.Arac.VarsayilanKurumBul(), 0);
                if (liste == null || !liste.sonuc.islemSonuc)
                    return Sonuc(false, "Hizmet gerekçeleri okunamadı.", liste == null ? "" : YatProjeOrtak.HataMetni(liste.sonuc.hataStr, ""));

                foreach (FaaliyetGerekce fgGit in liste.objeler)
                {
                    try
                    {
                        SbbButceServisi.WsSonuc sonuc = await SbbButceServisi.FaaliyetGerekceKaydet(eButceKullanici, eButceParola, yil, bumkoAsama, fgGit.kurKod,
                            fgGit.faalKod.ToString(), fgGit.gerekce, fgGit.dayanak, fgGit.maliyet);
                        if (!sonuc.islemSonuc)
                            sonucYazi.Append($"Hata: {fgGit.faalKod} kodlu birime ait hizmet gerekçesi gönderilemedi: {sonuc.hataStr}\r\n");
                        else
                            eklenen++;
                    }
                    catch (Exception ex)
                    {
                        sonucYazi.Append($"Hata: {fgGit.faalKod} kodlu birime ait hizmet gerekçesi gönderilemedi: {ex.Message}\r\n");
                    }
                }
                string bilgi = $"{eklenen} adet hizmet gerekçesi gönderildi.";
                KutukYaz("Hizmet Gerekçesi Aktarımı: " + bilgi + "\r\n" + sonucYazi);
                return Sonuc(sonucYazi.Length == 0, bilgi, sonucYazi.ToString());
            }

            // -------------------------------------------------------------- Ödenek Kontrol raporu

            private int fsatir1 = 0, fsutun1 = 0, fsatir2 = 0, fsutun2 = 0, bsatir = 0, bsutun = 0;

            public async Task<IResult> OdenekKontrol()
            {
                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                string dosyaAd = DosyaIslem.DosyaAdUret("SBBSGBOdenekKontrol", "") + ".xlsx";
                XLS.DosyaAc(Ayar.Oku("RaporSablonYol") + "/BHMv2/SBBSGBOdenekKontrol.xltx", dosyaAd);
                int shNo = 0;
                XLS.HucreAdAdresCoz("FormatSatir", ref shNo, ref fsatir1, ref fsutun1, ref fsatir2, ref fsutun2);
                XLS.HucreAdAdresCoz("BaslaSatir", ref bsatir, ref bsutun);
                int yuks = XLS.SatirYukseklikAl(bsatir);
                int serbestAy;
                if (yil == DateTime.Now.Year) serbestAy = DateTime.Now.Month;
                else if (yil > DateTime.Now.Year) serbestAy = 0;
                else serbestAy = 13;

                int satir = bsatir;
                try
                {
                    OdenekBilgi[] sgbMPDurum = SGBMasrafPlaniSonDurum() ?? new OdenekBilgi[0];
                    List<SbbButceServisi.AhpSatiri> ahpSonDurum = await SbbButceServisi.AHPDetayListeleSonDurum(eButceKullanici, eButceParola, yil, kurumKod, "");
                    if (ahpSonDurum == null)
                        throw new Exception("SBB'den AHP son durum alınamadı!");
                    else if (ahpSonDurum.Count == 0)
                        throw new Exception("SBB'den AHP son durum listesi boş geldi!");

                    Tertip gelenTertip = new Tertip(yil, ENUMTabloKod.TABMasrafPlani);
                    foreach (SbbButceServisi.AhpSatiri item in ahpSonDurum)
                    {
                        string afaal_fon = TNS.BHMv2.Arac.ProgramButceKodaCevir(yil, 1200, item.AltFaaliyetId.ToString());
                        string[] a_f = string.IsNullOrWhiteSpace(afaal_fon) ? new string[0] : afaal_fon.Split('.');
                        if (a_f.Length != 2)
                        {
                            HataliTertipYaz(item, XLS, satir++);
                            continue;
                        }
                        gelenTertip.TertipAta(item.KurKod + "-" + a_f[1] + "-" + item.FinKod + "-" + item.EkoKod + "-" + a_f[0]);
                        gelenTertip.Noktala();
                        OdenekBilgi sgbBilgi = SGBBilgiBul(sgbMPDurum, gelenTertip.TertipStrOl());
                        SatirYazKiyasla(XLS, ref satir, gelenTertip, item, sgbBilgi, serbestAy);
                    }
                    for (int i = 0; i < sgbMPDurum.Length; i++)
                        if (sgbMPDurum[i] != null)
                            SatirYazKiyasla(XLS, ref satir, null, null, sgbMPDurum[i], serbestAy);
                }
                catch (Exception ex)
                {
                    XLS.HucreDegerYaz(satir++, 0, ex.Message);
                }
                XLS.HucreKopyala(fsatir1 + 2, fsutun1, fsatir2 + 2, fsutun2, satir, bsutun);
                XLS.SatirYukseklikAyarla(bsatir, satir, -1, 5, yuks);
                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(dosyaAd, System.IO.Path.GetFileNameWithoutExtension(dosyaAd), true, "xlsx");
            }

            private void SatirYazKiyasla(Tablo XLS, ref int satir, Tertip gelenTertip, SbbButceServisi.AhpSatiri sbbBilgi, OdenekBilgi sgbBilgi, int serbestAy)
            {
                bool blokeEkle;
                if (serbestAy >= 13)
                {
                    blokeEkle = true;
                    serbestAy = 12;
                }
                else
                    blokeEkle = false;
                double serbestSBB = 0;
                if (sbbBilgi != null)
                {
                    for (int i = 0; i < serbestAy && i < sbbBilgi.ayDagilim.Length; i++)
                        serbestSBB += sbbBilgi.ayDagilim[i];
                    if (blokeEkle)
                        serbestSBB += sbbBilgi.bloke;
                }
                double serbestSGB = 0;
                if (sgbBilgi != null)
                    for (int i = 0; i < serbestAy; i++)
                        serbestSGB += sgbBilgi.EkRakam(i);

                if (OrtakFonksiyonlar.EsitMi(serbestSGB, serbestSBB)) return;

                XLS.HucreKopyala(fsatir1, fsutun1, fsatir2, fsutun2, satir, bsutun);
                XLS.HucreDegerYaz(satir, bsutun, gelenTertip != null ? gelenTertip.TertipStrOl() : sgbBilgi.tertip.TertipStrOl());
                int sut = bsutun + 16;
                double tplm;
                if (sbbBilgi != null)
                {
                    tplm = 0;
                    foreach (double ayDeger in sbbBilgi.ayDagilim)
                    {
                        XLS.HucreDegerYaz(satir, sut++, ayDeger);
                        tplm += ayDeger;
                    }
                    XLS.HucreDegerYaz(satir, sut++, sbbBilgi.bloke);
                    tplm += sbbBilgi.bloke;
                    XLS.HucreDegerYaz(satir, sut++, tplm);
                    XLS.HucreDegerYaz(satir, sut++, serbestSBB);
                }
                else
                    sut += 15;
                if (sgbBilgi != null)
                {
                    tplm = 0;
                    foreach (var ayDeger in sgbBilgi.ekRakamlar)
                    {
                        XLS.HucreDegerYaz(satir, sut++, (double)ayDeger);
                        tplm += (double)ayDeger;
                    }
                    XLS.HucreDegerYaz(satir, sut++, 0);
                    XLS.HucreDegerYaz(satir, sut++, tplm);
                    XLS.HucreDegerYaz(satir, sut++, serbestSGB);
                }
                satir++;
            }

            private static OdenekBilgi SGBBilgiBul(OdenekBilgi[] sgbMPDurum, string arananTertip)
            {
                for (int i = 0; i < sgbMPDurum.Length; i++)
                {
                    OdenekBilgi sgbBilgi = sgbMPDurum[i];
                    if (sgbBilgi != null && sgbBilgi.tertip.TertipStrOl().Equals(arananTertip))
                    {
                        sgbMPDurum[i] = null;
                        return sgbBilgi;
                    }
                }
                return null;
            }

            private OdenekBilgi[] SGBMasrafPlaniSonDurum()
            {
                Tertip kistasTertip = new Tertip(yil, ENUMTabloKod.TABMasrafPlani);
                kistasTertip.TertibeKodEkle(0, kurumKod, "");
                UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(0), kistasTertip.KodNoKirilimSay(0)));
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(1), kistasTertip.KodNoKirilimSay(1)));
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(2), kistasTertip.KodNoKirilimSay(2)));
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), 2, true));
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(4), kistasTertip.KodNoKirilimSay(4)));
                string asama = "9";
                EkDegerParam[] ekBilgiler = new EkDegerParam[12];
                for (int ayIc = 1; ayIc <= 12; ayIc++)
                    ekBilgiler[ayIc - 1] = new EkDegerParam(ENUMTabloKod.TABMasrafPlani, 0, 0, asama + ";" + ayIc.ToString());
                return servisBHMv2.ListeTertipOl(kullanan, kistasTertip, kodBilgileri, ekBilgiler, false);
            }

            private void HataliTertipYaz(SbbButceServisi.AhpSatiri item, Tablo XLS, int satir)
            {
                XLS.HucreKopyala(fsatir1, fsutun1, fsatir2, fsutun2, satir, bsutun);
                XLS.HucreDegerYaz(satir, bsutun, item.ProgramId + "." + item.AltProgramId + "." + item.FaaliyetId + "." + item.AltFaaliyetId + "-" + item.KurKod + "-" + item.FinKod + "-" + item.EkoKod);
                XLS.HucreDegerYaz(satir, bsutun + 1, "Tertip dönüştürülemedi!");
            }
        }
    }
}
