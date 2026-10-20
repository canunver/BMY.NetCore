using System;
using System.Collections;
using System.IO;
using Istemci.Oturum;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.Raporlama;

namespace Istemci.InsanKaynaklari
{
    /// <summary>
    /// Personel İşlemleri ekranının birden çok sekmesinin kullandığı rapor uçları (yetki: ana sayfa PersonelBilgi).
    ///
    ///   GET /api/personelbilgi/rapor/nufus?tur=&amp;personelKod=&amp;tcKimlikNo=
    ///       Eski NufusYazdir düğmeleri (PersonelKisisel, PersonelAile): TekRaporAl(500, tur, ...) → Rapor.xml grup 500,
    ///       KimlikPaylasim.NufusYazdir. tur: 1 = İkametgah İlmühaberi, 2 = Nüfus Cüzdanı Örneği (3 = Vukuatlı Nüfus Kayıt Örneği).
    ///       Raporun T.C. kimlik numarası:
    ///         1. tcKimlikNo doluysa o (Aile sekmesi: aile bireyinin T.C.'si; Kişisel sekmesi: henüz kaydedilmemiş yeni personelde
    ///            formdaki T.C.),
    ///         2. değilse personelKod &gt; 0 ise: Manas kullanılıyorsa personelKod'un kendisi (eski PersonelKisisel: hdnKod), değilse
    ///            personelin kayıtlı T.C. kimlik numarası (eski PersonelKisisel: txtTCKimlikNo; kayıtlı personelde değiştirilemez).
    ///       Düzenleyen / onaylayan ad ve unvanı kullanıcı değişkenlerinden (KIMLIKDUZENLEYENAD ...) gelir. Çıktı Word (.docx).
    ///   GET /api/personelbilgi/rapor/kisiborcu?personelKod=
    ///       Eski btnKisiBorcuYazdir_Click (PersonelKisisel, PersonelKadro): PersonelRaporlari2.MaasIdasiHesaplamaCetveliYazdir,
    ///       IKY\MaasIadesiHesaplamaCetveli.xltx şablonundan Excel (.xlsx).
    /// Hata durumunda { hata } döner (Sgb.indir bildirim olarak gösterir).
    /// </summary>
    public static class PersonelOrtakRaporlar
    {
        private const string SayfaAd = "PersonelBilgi";
        private const int NufusRaporGrup = 500;

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/personelbilgi/rapor/nufus", Nufus);
            app.MapGet("/api/personelbilgi/rapor/kisiborcu", KisiBorcu);
        }

        private static IResult Hata(string metin)
        {
            return Results.Json(new { hata = metin });
        }

        // ------------------------------------------------------------------ nüfus raporları (grup 500)

        private static IResult Nufus(HttpContext baglam, IWebHostEnvironment ortam, int? tur, int? personelKod, string tcKimlikNo)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            int istekTur = tur ?? 1;
            if (istekTur < 1 || istekTur > 3) istekTur = 1;
            int pKod = personelKod ?? 0;

            string tc = (tcKimlikNo ?? "").Trim();
            if (tc == "" && pKod > 0)
            {
                if (TNS.DEG.Arac.ManasKullaniyor())
                    tc = pKod.ToString();
                else
                {
                    TNS.IKY.Personel kriter = new TNS.IKY.Personel();
                    kriter.kod = pKod;
                    ObjectArray oa = TNS.IKY.Arac.Tanimla().PersonelListele(kullanan, kriter, false, false);
                    if (!oa.sonuc.islemSonuc) return Hata(PersonelBilgiOrtak.HataMetni(oa.sonuc.hataStr, "Personel bilgisi okunamadı."));
                    if (oa.objeler.Count > 0) tc = (((TNS.IKY.Personel)oa.objeler[0]).tcKimlikNo ?? "").Trim();
                }
            }
            if (tc == "") return Hata("T.C. Kimlik No bilgisi boş. Rapor alınamaz.");

            Hashtable raporParam = new Hashtable();
            raporParam["TCKimlikNo"] = tc;
            raporParam["DUZENLEYENAD"] = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "KIMLIKDUZENLEYENAD");
            raporParam["DUZENLEYENUNVAN"] = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "KIMLIKDUZENLEYENUNVAN");
            raporParam["ONAYLAYANAD"] = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "KIMLIKONAYLAYANAD");
            raporParam["ONAYLAYANUNVAN"] = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "KIMLIKONAYLAYANUNVAN");

            string gidenAd = istekTur == 1 ? "IkametgahIlmuhaberi" : istekTur == 2 ? "NufusCuzdanOrnegi" : "VukuatliNufusKayitOrnegi";
            try
            {
                AsposeLisans.Yukle();
                string dosya = RaporlamaServis.TekRaporAl(NufusRaporGrup, istekTur, kullanan, null, raporParam, null);
                if (string.IsNullOrEmpty(dosya)) return Hata("Rapor hazırlanamadı.");
                if (RaporlamaServis.MesajMi(dosya)) return Hata(PersonelBilgiOrtak.HataMetni(dosya, "Rapor hazırlanamadı."));
                if (!File.Exists(dosya)) return Hata("Rapor hazırlanamadı.");
                string uzanti = Path.GetExtension(dosya).TrimStart('.').ToLowerInvariant();
                return DosyaIslem.DosyaGonder(dosya, gidenAd, true, uzanti);
            }
            catch (RaporException ex)
            {
                return Hata(PersonelBilgiOrtak.HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
            catch (Exception ex)
            {
                // Eski NufusYazdir iş kuralı mesajını istisna olarak atar (ör. nüfus kaydı bulunamadı).
                return Hata(PersonelBilgiOrtak.HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }

        // ------------------------------------------------------------------ kişi borcu (maaş iadesi hesaplama cetveli)

        private static IResult KisiBorcu(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            int pKod = personelKod ?? 0;
            if (pKod <= 0) return Hata("Lütfen kişi seçiniz");
            string sablon = SablonBul(ortam, "IKY", "MaasIadesiHesaplamaCetveli.xltx");
            if (sablon == null) return Hata("Rapor şablonu bulunamadı (IKY\\MaasIadesiHesaplamaCetveli.xltx).");
            try
            {
                AsposeLisans.Yukle();
                string dosya = MaasIdasiHesaplamaCetveliYazdir(kullanan, pKod, sablon, "xlsx");
                return DosyaIslem.DosyaGonder(dosya, "MaasIadesiHesaplamaCetveli.xlsx", true, "xlsx");
            }
            catch (Exception ex)
            {
                return Hata(PersonelBilgiOrtak.HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }

        /// <summary>
        /// Şablon önce kurumun rapor şablonu klasöründe (Ayar RaporSablonYol\klasor\ad), yoksa uygulamayla gelen
        /// SGB.Net\RaporSablon\klasor\ad yolunda aranır; yoksa null.
        /// </summary>
        internal static string SablonBul(IWebHostEnvironment ortam, string klasor, string ad)
        {
            string kurumYol = Ayar.Oku("RaporSablonYol");
            if (kurumYol != "")
            {
                string s = Path.Combine(kurumYol, klasor, ad);
                if (File.Exists(s)) return s;
            }
            string u = Path.Combine(ortam.ContentRootPath, "RaporSablon", klasor, ad);
            return File.Exists(u) ? u : null;
        }

        // ---- Eski PersonelRaporlari2.MaasIdasiHesaplamaCetveliYazdir ve yardımcıları (kod aynen; şablon yolu çağıran tarafta bulunur)

        internal static string MaasIdasiHesaplamaCetveliYazdir(Kullanici kullanan, int pKod, string sablonAd, string ciktiTur)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            string sonucDosyaAd = DosyaIslem.DosyaAdUret(System.IO.Path.GetFileNameWithoutExtension(sablonAd), "") + "." + ciktiTur;
            XLS.DosyaAc(sablonAd, sonucDosyaAd);


            TNS.IKY.Personel per = new TNS.IKY.Personel();
            per.kod = pKod;

            TNS.IKY.IIKYServis servisIKY = TNS.IKY.Arac.Tanimla();
            if (pKod == 0)
            {
                XLS.HucreDegerYaz(0, 0, "Lütfen kişi seçiniz");
            }
            else
            {
                ObjectArray olar = servisIKY.PersonelListele(kullanan, per, false, false);
                if (olar.ObjeSayisi == 1)
                {
                    MaasIadesiBilgiYaz(kullanan, XLS, (TNS.IKY.Personel)olar.objeler[0]);
                }
                else if (!olar.sonuc.islemSonuc)
                {
                    XLS.HucreDegerYaz(0, 0, olar.sonuc.hataStr);
                }
                else
                    XLS.HucreDegerYaz(0, 0, "Kişi bulunamadı!");
            }
            XLS.DosyaSaklamaFormatAta(ciktiTur);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private static void MaasIadesiBilgiYaz(Kullanici kullanan, Tablo XLS, TNS.IKY.Personel personel)
        {
            int baslaSatir = 0, baslaSutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);

            if (TNSDateTime.IsNull(personel.kadro.ayrilmaTarihi))
            {
                XLS.HucreDegerYaz(5, 2, "Kişi ayrılmadığından hesap yapılamaz!");
                return;
            }

            DateTime bTar = personel.kadro.ayrilmaTarihi.Oku();
            if (bTar.Day < 15)
            {
                bTar = bTar.AddMonths(-1);
            }
            bTar = bTar.AddDays(15 - bTar.Day);

            TNS.HRC.IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();
            TNS.HRC.BordroKistas bordroKistas = new TNS.HRC.BordroKistas(0, null, new TNSCollection(personel.tcKimlikNo));
            bordroKistas.verilenTarih = new TNSDateTime(bTar);
            ObjectArray oa = servisHRC.CalisanDinamikBordroListele(kullanan, bordroKistas);
            TNSCollection tc = new TNSCollection();
            TNS.HRC.PSLMaasTemelBilgisi kriter = new TNS.HRC.PSLMaasTemelBilgisi();
            kriter.yil = 0;
            kriter.ay = 0;
            if (oa.objeler.Count == 0)
            {
                XLS.HucreDegerYaz(5, 2, $"{personel.tcKimlikNo} kimlik numaralı kişiye ait {bordroKistas.verilenTarih} tarihinde bordro bulunamadı!");
                return;
            }
            foreach (TNS.HRC.CalisanDinamikBordroDonem item in oa.objeler)
            {
                TNS.HRC.BordroYapi gelBy = item.bordroYapi;
                OrtakFonksiyonlar.HataDosyaYaz("PersonelRaporlar2.txt", $"Kişinin personel tipi: {gelBy.bordroTur}", true);
                KesintiBilgiBul(kullanan, tc, gelBy.bordroTur);
                OrtakFonksiyonlar.HataDosyaYaz("PersonelRaporlar2.txt", $"Personel tipine ait {tc.Count} adet satır bulundu", true);
                kriter.yil = item.yil;
                kriter.ay = item.islemAy;
                foreach (TNS.HRC.KisiSatir kisiSatir in item.calisanDinamikBordro.kisiDegerler)
                {
                    foreach (TNS.HRC.BordroBilgi bb in tc)
                    {
                        if (bb.personelTur == gelBy.bordroTur)
                        {
                            object o = TNS.HRC.CalisanDinamikBordro.DegerBul(gelBy, kisiSatir, bb.alanAd);
                            if (o is double)
                            {
                                bb.toplam += (double)o;
                            }
                        }
                    }
                }
            }

            double damgaVergisiOran = 0;
            if (kriter.yil > 0)
            {
                ObjectArray maasTemelBilgiList = TNS.HRC.Arac.Tanimla().PSLMaasTemelBilgisiListele(kullanan, kriter);
                if (maasTemelBilgiList.ObjeSayisi > 0)
                {
                    TNS.HRC.PSLMaasTemelBilgisi maasTemelBilgisi = (TNS.HRC.PSLMaasTemelBilgisi)maasTemelBilgiList[0];
                    damgaVergisiOran = (double)maasTemelBilgisi.damgaVergisiOran;
                }
            }
            int bilgiSatir = 5;
            XLS.HucreDegerYaz(bilgiSatir++, 2, personel.birimAd);
            XLS.HucreDegerYaz(bilgiSatir++, 2, personel.tcKimlikNo);
            XLS.HucreDegerYaz(bilgiSatir++, 2, personel.adSoyad);
            XLS.HucreDegerYaz(bilgiSatir++, 2, personel.personelTipAd);
            XLS.HucreDegerYaz(bilgiSatir++, 2, personel.unvanAd);
            XLS.HucreDegerYaz(bilgiSatir++, 2, personel.kadro.ayrilmaTarihiDT.AddDays(-1));
            XLS.HucreDegerYaz(bilgiSatir++, 2, bTar);
            TimeSpan a = personel.kadro.ayrilmaTarihiDT - bTar;
            int gun = a.Days;
            XLS.HucreDegerYaz(bilgiSatir, 2, a.Days);
            int ayGun = DateTime.DaysInMonth(bTar.Year, bTar.Month);
            XLS.HucreDegerYaz(bilgiSatir++, 4, ayGun);
            XLS.HucreDegerYaz(bilgiSatir++, 2, personel.kadro.ayrilmaNedeni);

            int sat = baslaSatir;
            double hakEdis = 0;
            double hakEdisKes = 0;
            int kesSay = 0;
            int satYuk = XLS.SatirYukseklikAl(sat);
            XLS.HucreKopyala(0, 0, 0, 5, sat, 0);
            XLS.HucreDegerYaz(sat++, 0, "HAKEDİŞLER");
            foreach (TNS.HRC.BordroBilgi bb in tc)
            {
                if (bb.eklentiMi)
                {
                    double ek;
                    double kes;
                    if (bb.kesintiEklentiTur == TNS.HRC.BordroBilgi.KesintiTur.Eklenti)
                        ek = bb.toplam;
                    else
                        ek = bb.toplam - OrtakFonksiyonlar.KurusYap(bb.toplam * damgaVergisiOran / 100);
                    kes = OrtakFonksiyonlar.KurusYap(gun * ek / ayGun);

                    XLS.HucreKopyala(0, 0, 0, 5, sat, 0);
                    XLS.HucreDegerYaz(sat, 0, bb.kesintiAd);
                    XLS.HucreDegerYaz(sat, 2, ek);
                    XLS.HucreDegerYaz(sat, 3, kes);
                    XLS.HucreDegerYaz(sat++, 4, ek - kes);

                    hakEdis += ek;
                    hakEdisKes += kes;
                }
                else
                    kesSay++;
            }
            XLS.HucreKopyala(0, 0, 0, 5, sat, 0);
            XLS.HucreDegerYaz(sat, 0, "TOPLAM HAKEDİŞ");
            XLS.HucreDegerYaz(sat, 2, hakEdis);
            XLS.HucreDegerYaz(sat, 3, hakEdisKes);
            XLS.HucreDegerYaz(sat++, 4, hakEdis - hakEdisKes);
            XLS.HucreKopyala(0, 0, 0, 5, sat, 0);
            sat++;
            XLS.HucreKopyala(0, 0, 0, 5, sat, 0);
            XLS.HucreDegerYaz(sat++, 0, "YASAL KESİNTİLER");
            double kesTop = 0;
            double kesTopOran = 0;
            foreach (TNS.HRC.BordroBilgi bb in tc)
            {
                if (!bb.eklentiMi)
                {
                    XLS.HucreKopyala(0, 0, 0, 5, sat, 0);
                    XLS.HucreDegerYaz(sat, 0, bb.kesintiAd);
                    double carp = 1;
                    double oran = 1;
                    if (bb.kesintiEklentiTur == TNS.HRC.BordroBilgi.KesintiTur.KesintiIadeTam)
                    {
                        carp = -1;
                        if (bb.kesintiEklentiTur == TNS.HRC.BordroBilgi.KesintiTur.KesintiIadeTam)
                            oran = 0;
                    }
                    else if (bb.kesintiEklentiTur == TNS.HRC.BordroBilgi.KesintiTur.KesintiOran)
                    {
                        oran = gun * 1.0 / AyGunBul(ayGun, bb.hep30);
                    }
                    double y1 = carp * bb.toplam;
                    double y2 = OrtakFonksiyonlar.KurusYap(carp * oran * bb.toplam);
                    XLS.HucreDegerYaz(sat, 2, y1);
                    XLS.HucreDegerYaz(sat, 3, y2);
                    XLS.HucreDegerYaz(sat, 4, y1 - y2);
                    kesTop += y1;
                    kesTopOran += y2;
                    sat++;
                }
            }

            XLS.HucreKopyala(0, 0, 0, 5, sat, 0);
            XLS.HucreDegerYaz(sat, 0, "TOPLAM KESİNTİLER");
            XLS.HucreDegerYaz(sat, 2, kesTop);
            XLS.HucreDegerYaz(sat, 3, kesTopOran);
            XLS.HucreDegerYaz(sat++, 4, kesTop - kesTopOran);
            XLS.HucreKopyala(0, 0, 0, 5, sat, 0);
            sat++;
            XLS.HucreKopyala(2, 0, 2, 5, sat, 0);
            XLS.HucreDegerYaz(sat, 0, "BORÇ TUTARI");
            XLS.HucreDegerYaz(sat, 2, hakEdis - kesTop);
            XLS.HucreDegerYaz(sat, 3, hakEdisKes - kesTopOran);
            XLS.HucreDegerYaz(sat++, 4, hakEdis - kesTop - (hakEdisKes - kesTopOran));

            XLS.SatirYukseklikAyarla(baslaSatir, sat, satYuk);
        }

        private static int AyGunBul(int ayGun, bool hep30)
        {
            if (hep30) return 30;
            return ayGun;
        }

        private static void KesintiBilgiBul(Kullanici kullanan, TNSCollection tc, int personelTip)
        {
            ObjectArray oa = TNS.HRC.Arac.Tanimla().BordroKisiBorcuBilgiBul(kullanan, personelTip);
            tc.AddRange(oa.objeler);
        }
    }
}
