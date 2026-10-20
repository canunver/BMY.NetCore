using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.IKY;
using TNS.KYM;

namespace Istemci.InsanKaynaklari
{
    /// <summary>Kaydet isteği (eski PersonelAtama.VeriTopla'nın okuduğu alanlar). Sayısal alanlar metin: eskideki ConvertToInt(Text, 0) gibi çevrilir.</summary>
    public class PersonelAtamaIstegi
    {
        // eski gizli alanlar: hdnYeniKadroDurum, hdnYeniPKadroKod, hdnEskiKadroKod, hdnPersonelKod, hdnIliskiliKadroIslemNo
        public int kadroDurum { get; set; }
        public int kod { get; set; }
        public int eskiKod { get; set; }
        public int personelKod { get; set; }
        public string iliskiliKadroIslemNo { get; set; }

        public string personelTip { get; set; }
        /// <summary>Başlama sebebi "tip-kod" (HizmetSebebi.tipKod).</summary>
        public string baslamaGerekcesi { get; set; }
        /// <summary>yyyy-MM-dd</summary>
        public string atamaTarihi { get; set; }
        public string atamaNedeni { get; set; }

        public int organizasyonKodu { get; set; }
        public int gorevOrganizasyonKodu { get; set; }
        public int tayinOlacagiBirim { get; set; }
        public string gorevYeri { get; set; }

        public string unvan { get; set; }
        public string vekilUnvanKod { get; set; }
        public string evrakNo { get; set; }
        public string yasalDayanak { get; set; }
        public string onayTarih { get; set; }
        public string meslekKodu { get; set; }
        public string aciklama { get; set; }

        public string kadroDerece { get; set; }
        public string kadroTarih { get; set; }

        public string kazanilmisHakDerece { get; set; }
        public string kazanilmisHakKademe { get; set; }
        public string kazanilmisHakTarih { get; set; }
        public string kazanilmisGosterge { get; set; }
        public string kazanilmisEkGosterge { get; set; }

        public string odemeyeEsasDerece { get; set; }
        public string odemeyeEsasKademe { get; set; }
        public string odemeyeEsasTarih { get; set; }
        public string odemeyeEsasGosterge { get; set; }
        public string odemeyeEsasEkGosterge { get; set; }

        public string emekliyeEsasDerece { get; set; }
        public string emekliyeEsasKademe { get; set; }
        public string emekliyeEsasTarih { get; set; }
        public string emekliyeEsasGosterge { get; set; }
        public string emekliyeEsasEkGosterge { get; set; }

        /// <summary>Ayrılma sebebi "tip-kod".</summary>
        public string ayrilmaGerekcesi { get; set; }
        public string ayrilmaNedeni { get; set; }
        public string ayrilmaTarihi { get; set; }
        public string hizmetGunSayisi { get; set; }

        /// <summary>Atanacağı kadro "unvanKod-derece" (KadroDoluBos.ID).</summary>
        public string atanacagiKadro { get; set; }
    }

    /// <summary>
    /// Personel Atama penceresi (eski InsanKaynaklari/PersonelAtama.aspx; yeni sayfa InsanKaynaklari/PersonelAtama):
    /// Hizmet Belgesi (Kadro) sekmesinden açılır; personelin bir hizmet/kadro kaydını önceki (eski) kaydıyla yan yana gösterir,
    /// yeni kaydı ekler / düzenler.
    ///   GET  /api/personelbilgi/atama/tanim?personelKod          seçenekler (eski Page_Load: UnvanDoldur, HizmetSebebiDoldur x2,
    ///                                                            PersonelTurDoldur, AcikKadroDoldur)
    ///   GET  /api/personelbilgi/atama/oku?personelKod&amp;kod&amp;eskiKod  yeni ve eski kadro kaydı (eski Listele / PersonelHizmetSatiriBul / GizleAc)
    ///   GET  /api/personelbilgi/atama/personeltip?tur            personel türü değişince (eski cmbPersonelTipDegisti)
    ///   GET  /api/personelbilgi/atama/gosterge?derece&amp;kademe     gösterge (eski GostergeHesapla: Arac.GostergeBul)
    ///   GET  /api/personelbilgi/atama/orgagac?ust&amp;ara&amp;tum        birim seçimi (eski ExtOrgSecim; tum=1: yetki süzmesiz, Gid. Birim)
    ///   POST /api/personelbilgi/atama/kaydet                     (eski btnKaydet_Click: VeriTopla + PersonelKadroKaydet)
    /// </summary>
    public static class PersonelAtamaUclari
    {
        private const string Uc = "/api/personelbilgi/atama/";
        private const string SayfaAd = "PersonelAtama";
        private const string BirimYetkisi = "Personel Saymanlık-Birim Yetkisi";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "tanim", Tanim);
            app.MapGet(Uc + "oku", Oku);
            app.MapGet(Uc + "personeltip", PersonelTip);
            app.MapGet(Uc + "gosterge", Gosterge);
            app.MapGet(Uc + "orgagac", OrgAgac);
            app.MapPost(Uc + "kaydet", Kaydet);
        }

        private static IIKYServis Servis() { return TNS.IKY.Arac.Tanimla(); }

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        private static int Sayi(string s) { return OrtakFonksiyonlar.ConvertToInt(Temiz(s), 0); }

        // ------------------------------------------------------------------ seçenekler

        /// <summary>Eski Page_Load'daki açılır liste doldurmaları.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                IIKYServis servis = Servis();
                List<string> hatalar = new List<string>();

                // Genel.UnvanDoldur(ddlUnvan, unvanStore): aktif unvanlar; ddlUnvan ve ddlVekaletUnvani aynı listeyi kullanır.
                List<object> unvanlar = new List<object>();
                ObjectArray oaUnvan = servis.UnvanListele(new Unvan { aktif = true });
                if (oaUnvan.sonuc.islemSonuc)
                {
                    foreach (Unvan u in oaUnvan.objeler)
                        unvanlar.Add(new { UnvanKod = u.UnvanKod, UnvanAd = u.UnvanAd });
                }
                else hatalar.Add(PersonelBilgiOrtak.HataMetni(oaUnvan.sonuc.hataStr, "Unvanlar okunamadı."));

                // Genel.HizmetSebebiDoldur(..., BASLAMAGRUBU / AYRILMAGRUBU, tipKod: true): ilk seçenek boş.
                List<object> baslamaSebepleri = HizmetSebepleri(servis, ENUMHizmetSebebiNevi.BASLAMAGRUBU, hatalar);
                List<object> ayrilmaSebepleri = HizmetSebepleri(servis, ENUMHizmetSebebiNevi.AYRILMAGRUBU, hatalar);

                // Genel.PersonelTurDoldur(cmbPersonelTip, personelTipStore, null): Arac.PersonelTipListe.
                List<object> personelTipleri = new List<object>();
                foreach (PersonelTipi t in TNS.IKY.Arac.PersonelTipListe)
                    personelTipleri.Add(new { kod = t.kod, ad = t.ad });

                // Genel.AcikKadroDoldur(stoAtanacagiKadro, 0, true, pKod): boş kadrolar (kişinin kendisi hariç) + sonda boş satır.
                List<object> acikKadrolar = new List<object>();
                TNS.IKY.PersonelKadro pk = new TNS.IKY.PersonelKadro();
                pk.bosKadrolar = true;
                pk.kisiHaric = personelKod ?? 0;
                ObjectArray oaKadro = servis.DoluBosKadroListele(pk);
                if (oaKadro != null && oaKadro.objeler != null)
                {
                    oaKadro.objeler.Add(new KadroDoluBos());
                    foreach (KadroDoluBos k in oaKadro.objeler)
                        acikKadrolar.Add(new { ID = k.ID, Aciklama = k.Aciklama });
                }

                return Results.Json(new
                {
                    unvanlar = unvanlar,
                    baslamaSebepleri = baslamaSebepleri,
                    ayrilmaSebepleri = ayrilmaSebepleri,
                    personelTipleri = personelTipleri,
                    acikKadrolar = acikKadrolar,
                    hata = string.Join("\n", hatalar)
                });
            }
            catch (Exception)
            {
                return Hata("Seçenek listeleri okunamadı.");
            }
        }

        private static List<object> HizmetSebepleri(IIKYServis servis, ENUMHizmetSebebiNevi sebepler, List<string> hatalar)
        {
            List<object> liste = new List<object>();
            ObjectArray dler = servis.HizmetSebebiListele(sebepler);
            if (dler.sonuc.islemSonuc)
            {
                if (dler.objeler.Count > 0)
                {
                    liste.Add(new { kod = "", ad = "" });
                    foreach (HizmetSebebi d in dler.objeler)
                        liste.Add(new { kod = d.tipKod, ad = d.sebepAdi });
                }
            }
            else hatalar.Add(PersonelBilgiOrtak.HataMetni(dler.sonuc.hataStr, "Hizmet sebepleri okunamadı."));
            return liste;
        }

        // ------------------------------------------------------------------ kayıt okuma

        /// <summary>Eski Listele(kadroKod, eskiKod): yeni kayıt ve önceki (eski) kayıt, alanların görünürlük / salt okunurluk durumu.</summary>
        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod, int? kod, int? eskiKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                int pKod = personelKod ?? 0;
                if (pKod <= 0) return Hata("Personel bilgisi bulunamadı.");

                string hata = PersonelHizmetSatiriBul(kullanan, pKod, kod ?? 0, out TNS.IKY.PersonelKadro yeniKadro, out TNS.IKY.PersonelKadro eskiKadro, eskiKod ?? -1);
                if (hata != "") return Hata(hata);

                // Eski kaydın birim alanları eski kaydın personel türüne göre (eski Listele).
                PersonelTipi personelTipiEski = TNS.IKY.Arac.PersonelTipBul(eskiKadro.personelTip);
                bool eskiKurumDisi = !(personelTipiEski == null || personelTipiEski.kurumDisi == 0);

                return Results.Json(new
                {
                    yeni = KadroYaz(yeniKadro),
                    eski = KadroYaz(eskiKadro),
                    eskiKurumDisi = eskiKurumDisi,
                    tip = PersonelTipBilgisi(yeniKadro.personelTip),
                    duzen = GizleAc(yeniKadro.kadroDurum)
                });
            }
            catch (Exception)
            {
                return Hata("Hizmet bilgisi okunamadı.");
            }
        }

        /// <summary>Eski PersonelHizmetSatiriBul (aynen). Hata yoksa "" döner.</summary>
        private static string PersonelHizmetSatiriBul(Kullanici kullanan, int personelKod, int kadroKod, out TNS.IKY.PersonelKadro okunan, out TNS.IKY.PersonelKadro eski, int onceki)
        {
            okunan = null;
            eski = null;
            ObjectArray list2 = null;
            IIKYServis servis = Servis();

            if (kadroKod > 0)
            {
                list2 = servis.PersonelKadroListele(kullanan, new TNS.IKY.PersonelKadro { kod = kadroKod, personelKod = personelKod }, false);
                if (list2 != null && !list2.sonuc.islemSonuc) return PersonelBilgiOrtak.HataMetni(list2.sonuc.hataStr, "Hizmet bilgisi okunamadı.");
                if (list2 != null && list2.ObjeSayisi > 0)
                {
                    okunan = (TNS.IKY.PersonelKadro)list2.objeler[0];
                    if (okunan.eskiKod > 0 && okunan.eskiKod != kadroKod) onceki = okunan.eskiKod;
                }
            }

            if (onceki > 0)
                list2 = servis.PersonelKadroListele(kullanan, new TNS.IKY.PersonelKadro { kod = onceki, personelKod = personelKod }, false);
            else if (kadroKod == 0)
                list2 = servis.PersonelKadroListele(kullanan, new TNS.IKY.PersonelKadro { personelKod = personelKod, kadroDurum = ENUMKadroDurumu.AKTIFKAYIT }, false);
            else
                list2 = null;

            if (list2 != null && !list2.sonuc.islemSonuc) return PersonelBilgiOrtak.HataMetni(list2.sonuc.hataStr, "Önceki hizmet bilgisi okunamadı.");
            if (list2 != null && list2.ObjeSayisi > 0)
                eski = (TNS.IKY.PersonelKadro)list2.objeler[0];

            if (eski == null) eski = new TNS.IKY.PersonelKadro();
            if (okunan == null)
            {
                okunan = (TNS.IKY.PersonelKadro)eski.Clone();
                okunan.kod = 0;
                okunan.kadroDurum = ENUMKadroDurumu.TANIMSIZ;
                okunan.atananacagiKadroDerece = 0;
                okunan.atananacagiKadroUnvanKod = 0;
                okunan.eskiKod = eski.kod;
                okunan.atamaTarihi = new TNSDateTime(DateTime.MinValue);
                okunan.ayrilmaTarihi = new TNSDateTime(DateTime.MinValue);
                okunan.ayrilmaGerekcesi = 0;
                okunan.ayrilmaGerekcesiAd = "";
                okunan.gitOrganizasyonKopruKodu = "";
                okunan.tayinOlacagiBirim = 0;
                okunan.tayinOlacagiBirimAd = "";
                okunan.iliskiliKadroIslemNo = 0;
            }
            return "";
        }

        /// <summary>Kadro kaydını ekrana gidecek biçimde yazar (eski Listele'deki alan doldurmalar).</summary>
        private static object KadroYaz(TNS.IKY.PersonelKadro k)
        {
            return new
            {
                k.kod,
                k.eskiKod,
                kadroDurum = (int)k.kadroDurum,
                k.kadroDurumAd,
                k.personelTip,
                personelTipAd = k.personelTipAd ?? "",
                hizmetHesabi = k.hizmetHesabi.ToString(),
                baslamaGerekcesi = HizmetSebebi.TipKodOl(k.hizmetSebebiTip, k.hizmetSebebiKod),
                hizmetSebebiAd = k.hizmetSebebiAd ?? "",
                atamaTarihi = PersonelBilgiOrtak.TarihYaz(k.atamaTarihi),
                atamaNedeni = k.hizmetSebebi ?? "",
                k.organizasyonKodu,
                birimAd = k.birimAd ?? "",
                k.gorevOrganizasyonKodu,
                gorevBirimAd = k.gorevBirimAd ?? "",
                gorevYeri = k.gorevYeri ?? "",
                k.unvan,
                unvanAd = k.unvanAd ?? "",
                k.vekilUnvanKod,
                vekilUnvanAd = k.vekilUnvanAd ?? "",
                evrakNo = k.evrakNo ?? "",
                yasalDayanak = k.yasalDayanak ?? "",
                onayTarih = PersonelBilgiOrtak.TarihYaz(k.onayTarih),
                meslekKodu = k.meslekKodu ?? "",
                aciklama = k.aciklama ?? "",
                k.kadroDerece,
                kadroTarih = PersonelBilgiOrtak.TarihYaz(k.kadroTarih),
                k.kazanilmisHakDerece,
                k.kazanilmisHakKademe,
                kazanilmisHakTarih = PersonelBilgiOrtak.TarihYaz(k.kazanilmisHakTarih),
                k.kazanilmisGosterge,
                k.kazanilmisEkGosterge,
                k.odemeyeEsasDerece,
                k.odemeyeEsasKademe,
                odemeyeEsasTarih = PersonelBilgiOrtak.TarihYaz(k.odemeyeEsasTarih),
                k.odemeyeEsasGosterge,
                k.odemeyeEsasEkGosterge,
                k.emekliyeEsasDerece,
                k.emekliyeEsasKademe,
                emekliyeEsasTarih = PersonelBilgiOrtak.TarihYaz(k.emekliyeEsasTarih),
                k.emekliyeEsasGosterge,
                k.emekliyeEsasEkGosterge,
                ayrilmaGerekcesi = HizmetSebebi.TipKodOl(k.ayrilmaGerekcesiTip, k.ayrilmaGerekcesi),
                ayrilmaGerekcesiAd = k.ayrilmaGerekcesiAd ?? "",
                ayrilmaNedeni = k.ayrilmaNedeni ?? "",
                ayrilmaTarihi = PersonelBilgiOrtak.TarihYaz(k.ayrilmaTarihi),
                k.hizmetGunSayisi,
                k.tayinOlacagiBirim,
                tayinOlacagiBirimAd = k.tayinOlacagiBirimAd ?? "",
                k.iliskiliKadroIslemNo,
                atanacagiKadro = k.atananacagiKadroUnvanKod.ToString() + '-' + k.atananacagiKadroDerece.ToString()
            };
        }

        /// <summary>Eski GizleAc(kayitDurumu): Kaydet düğmesi ve alanların salt okunurluğu.</summary>
        private static object GizleAc(ENUMKadroDurumu kayitDurumu)
        {
            bool onayDurum = kayitDurumu == ENUMKadroDurumu.AKTIFKAPANIYOR || kayitDurumu == ENUMKadroDurumu.AKTIFKAPANDI || kayitDurumu == ENUMKadroDurumu.AKTIFKAYIT || kayitDurumu == ENUMKadroDurumu.AYRILDI || kayitDurumu == ENUMKadroDurumu.REDDEDILDI;
            bool bitmis = kayitDurumu == ENUMKadroDurumu.AYRILDI || kayitDurumu == ENUMKadroDurumu.REDDEDILDI || kayitDurumu == ENUMKadroDurumu.AKTIFKAPANDI;
            bool ayrilabilir = kayitDurumu == ENUMKadroDurumu.TANIMSIZ || kayitDurumu == ENUMKadroDurumu.YENI || kayitDurumu == ENUMKadroDurumu.AKTIFYENI || kayitDurumu == ENUMKadroDurumu.AKTIFKAYIT || kayitDurumu == ENUMKadroDurumu.AKTIFKAPANIYOR;
            return new { kaydetKapali = bitmis, onayDurum = onayDurum, ayrilabilir = ayrilabilir };
        }

        /// <summary>Eski cmbPersonelTipDegisti + BirimGoster: tür bulunamazsa listedeki ilk tür.</summary>
        private static object PersonelTipBilgisi(int personelTur)
        {
            PersonelTipi personelTipiYeni = TNS.IKY.Arac.PersonelTipBul(personelTur);
            if (personelTipiYeni == null && TNS.IKY.Arac.PersonelTipListe.Count > 0)
                personelTipiYeni = (PersonelTipi)TNS.IKY.Arac.PersonelTipListe[0];
            bool kurumDisi = !(personelTipiYeni == null || personelTipiYeni.kurumDisi == 0);
            return new
            {
                hizmetGrubu = personelTipiYeni != null ? personelTipiYeni.HITAP.ToString() : "",
                kurumDisi = kurumDisi,
                kadroIslemi = personelTipiYeni != null && personelTipiYeni.KadroIslemiYapilacakMi()
            };
        }

        private static IResult PersonelTip(HttpContext baglam, IWebHostEnvironment ortam, int? tur)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                return Results.Json(new { tip = PersonelTipBilgisi(tur ?? 0) });
            }
            catch (Exception)
            {
                return Hata("Personel türü okunamadı.");
            }
        }

        /// <summary>Eski GostergeHesapla(yer): derece / kademe değişince göstergeyi Arac.GostergeBul ile yazar.</summary>
        private static IResult Gosterge(HttpContext baglam, IWebHostEnvironment ortam, string derece, string kademe)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            return Results.Json(new { gosterge = TNS.IKY.Arac.GostergeBul(Sayi(derece), Sayi(kademe)) });
        }

        // ------------------------------------------------------------------ birim seçimi

        /// <summary>
        /// Birim ağacı (eski ExtOrgSecim). Kadro / görev birimi: kullanıcının "Personel Saymanlık-Birim Yetkisi"ne göre
        /// (eski ddlBirim.KullanicininBirimleri). tum=1: yetki süzmesi yok (eski ddlTayinOlacagiBirim). ara: adda arama, düz liste.
        /// </summary>
        private static IResult OrgAgac(HttpContext baglam, IWebHostEnvironment ortam, int? ust, string ara, int? tum)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                ara = Temiz(ara);
                TNS.ORG.OrganizasyonBirim kriter = new TNS.ORG.OrganizasyonBirim();
                kriter.anaBirimKod = Math.Max(0, ust ?? 0);
                kriter.sablonMu = false;
                if (ara != "")
                {
                    kriter.anaBirimKod = 0;
                    kriter.adi = ara;
                    if ((tum ?? 0) != 1)
                    {
                        string yetkili = BirimYetkisiVer(kullanan);
                        if (yetkili != "") kriter.yetkiliOlduguMuhasebeBirim = yetkili;
                    }
                }
                else if (kriter.anaBirimKod == 0)
                {
                    string yetkili = (tum ?? 0) == 1 ? "" : BirimYetkisiVer(kullanan);
                    if (yetkili != "") kriter.yetkiliOlduguMuhasebeBirim = yetkili;
                    else kriter.kopruKod = "999";
                }

                ObjectArray<TNS.ORG.OrganizasyonBirim> sonuc = TNS.ORG.Arac.Tanimla().OrganizasyonBirimListele(kriter, false, false);
                if (sonuc == null || !sonuc.sonuc.islemSonuc) return Hata("Birimler okunamadı.");
                List<object> satirlar = new List<object>();
                foreach (TNS.ORG.OrganizasyonBirim b in sonuc.objeler)
                {
                    string ad = b.adi ?? "";
                    if (ara != "")
                    {
                        string ilIlce = TNS.ORG.Arac.OrgBirimIlIlceAdBul(b.kod);
                        if (!string.IsNullOrEmpty(ilIlce)) ad += " (" + ilIlce + ")";
                    }
                    satirlar.Add(new { kod = b.kod, ad = ad, birimAd = b.adi ?? "", altVar = ara == "" && b.altDalVar });
                    if (satirlar.Count >= 300) break;
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Birimler okunamadı.");
            }
        }

        /// <summary>Kullanıcının saymanlık-birim yetkisi ("muh-bir;muh-bir"); hiçbir değer dolu değilse "".</summary>
        private static string BirimYetkisiVer(Kullanici kullanan)
        {
            string yetkili = kullanan.YetkiTipiDegeriBul(BirimYetkisi) ?? "";
            foreach (string y in yetkili.Split(';'))
            {
                string[] m = y.Split('-');
                if (m.Length == 2 && (OrtakFonksiyonlar.ConvertToInt(m[0], 0) > 0 || OrtakFonksiyonlar.ConvertToInt(m[1], 0) > 0))
                    return yetkili;
            }
            return "";
        }

        // ------------------------------------------------------------------ kaydet

        /// <summary>Eski btnKaydet_Click: VeriTopla + PersonelKadroKaydet. Başarıda yeniden okunacak kod / eskiKod döner (eski Listele(d.kod, d.eskiKod)).</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, PersonelAtamaIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                TNS.IKY.PersonelKadro d = VeriTopla(istek);
                Sonuc sonuc = Servis().PersonelKadroKaydet(kullanan, d);
                if (!sonuc.islemSonuc)
                    return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Hizmet bilgisi kaydedilemedi.") });

                // İş kuralı yeni kaydın kodunu sonuc.anahtar'da döndürür (servis uzakta olduğundan d.kod güncellenmez).
                int kod = d.kod > 0 ? d.kod : OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, 0);
                string bilgi = PersonelBilgiOrtak.Duz(sonuc.bilgiStr);
                if (bilgi == "") bilgi = "Kayıt işlemi başarıyla gerçekleşti.";
                return Results.Json(new { basarili = true, bilgi = bilgi, kod = kod, eskiKod = d.eskiKod });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Hizmet bilgisi kaydedilemedi." });
            }
        }

        /// <summary>Eski VeriTopla (aynen).</summary>
        private static TNS.IKY.PersonelKadro VeriTopla(PersonelAtamaIstegi i)
        {
            TNS.IKY.PersonelKadro d = new TNS.IKY.PersonelKadro();
            d.kadroDurum = (ENUMKadroDurumu)i.kadroDurum;
            d.kod = i.kod;
            d.eskiKod = i.eskiKod;
            d.personelKod = i.personelKod;
            d.personelTip = Sayi(i.personelTip);
            HizmetGerekcesiAta(i.baslamaGerekcesi, d, 1);
            d.atamaTarihi = PersonelBilgiOrtak.TnsTarih(i.atamaTarihi);
            d.hizmetSebebi = i.atamaNedeni ?? "";

            d.organizasyonKodu = i.organizasyonKodu;
            d.gorevOrganizasyonKodu = i.gorevOrganizasyonKodu;
            if (d.gorevOrganizasyonKodu == 0)
                d.gorevOrganizasyonKodu = d.organizasyonKodu;
            d.tayinOlacagiBirim = i.tayinOlacagiBirim;
            d.gorevYeri = i.gorevYeri ?? "";

            //Sınıf kaydedilmiyor, unvan kodundan bulunuyor.
            d.unvan = Sayi(i.unvan);
            d.vekilUnvanKod = Sayi(i.vekilUnvanKod);
            d.evrakNo = i.evrakNo ?? "";
            d.yasalDayanak = i.yasalDayanak ?? "";
            d.onayTarih = PersonelBilgiOrtak.TnsTarih(i.onayTarih);
            d.meslekKodu = i.meslekKodu ?? "";

            d.aciklama = i.aciklama ?? "";

            d.kadroDerece = Sayi(i.kadroDerece);
            d.kadroTarih = PersonelBilgiOrtak.TnsTarih(i.kadroTarih);

            d.kazanilmisHakDerece = Sayi(i.kazanilmisHakDerece);
            d.kazanilmisHakKademe = Sayi(i.kazanilmisHakKademe);
            d.kazanilmisHakTarih = PersonelBilgiOrtak.TnsTarih(i.kazanilmisHakTarih);
            d.kazanilmisGosterge = Sayi(i.kazanilmisGosterge);
            d.kazanilmisEkGosterge = Sayi(i.kazanilmisEkGosterge);

            d.odemeyeEsasDerece = Sayi(i.odemeyeEsasDerece);
            d.odemeyeEsasKademe = Sayi(i.odemeyeEsasKademe);
            d.odemeyeEsasTarih = PersonelBilgiOrtak.TnsTarih(i.odemeyeEsasTarih);
            d.odemeyeEsasGosterge = Sayi(i.odemeyeEsasGosterge);
            d.odemeyeEsasEkGosterge = Sayi(i.odemeyeEsasEkGosterge);

            d.emekliyeEsasDerece = Sayi(i.emekliyeEsasDerece);
            d.emekliyeEsasKademe = Sayi(i.emekliyeEsasKademe);
            d.emekliyeEsasTarih = PersonelBilgiOrtak.TnsTarih(i.emekliyeEsasTarih);
            d.emekliyeEsasGosterge = Sayi(i.emekliyeEsasGosterge);
            d.emekliyeEsasEkGosterge = Sayi(i.emekliyeEsasEkGosterge);

            HizmetGerekcesiAta(i.ayrilmaGerekcesi, d, 2);
            d.ayrilmaNedeni = i.ayrilmaNedeni ?? "";
            d.ayrilmaTarihi = PersonelBilgiOrtak.TnsTarih(i.ayrilmaTarihi);
            d.hizmetGunSayisi = Sayi(i.hizmetGunSayisi);

            d.iliskiliKadroIslemNo = Sayi(i.iliskiliKadroIslemNo);

            string[] secilenKadro = (i.atanacagiKadro ?? "").Split('-');
            d.atananacagiKadroUnvanKod = OrtakFonksiyonlar.ConvertToInt(OrtakFonksiyonlar.DiziElemanAl(secilenKadro, 0, ""), 0);
            d.atananacagiKadroDerece = OrtakFonksiyonlar.ConvertToInt(OrtakFonksiyonlar.DiziElemanAl(secilenKadro, 1, ""), 0);
            return d;
        }

        /// <summary>Eski HizmetGerekcesiAta: "tip-kod" -> başlama (gelGit 1) ya da ayrılma (2) gerekçesi.</summary>
        private static void HizmetGerekcesiAta(string tipKodMetin, TNS.IKY.PersonelKadro d, int gelGit)
        {
            int tip = 0;
            int kod = 0;

            if (!string.IsNullOrEmpty(tipKodMetin))
            {
                string[] tipKod = tipKodMetin.Split('-');
                tip = OrtakFonksiyonlar.ConvertToInt(OrtakFonksiyonlar.DiziElemanAl(tipKod, 0, ""), 0);
                kod = OrtakFonksiyonlar.ConvertToInt(OrtakFonksiyonlar.DiziElemanAl(tipKod, 1, ""), 0);
            }

            if (gelGit == 1)
            {
                d.hizmetSebebiTip = (ENUMHizmetSebebiNevi)tip;
                d.hizmetSebebiKod = kod;
            }
            else
            {
                d.ayrilmaGerekcesiTip = (ENUMHizmetSebebiNevi)tip;
                d.ayrilmaGerekcesi = kod;
            }
        }
    }
}
