using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading.Tasks;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.BKD;
using TNS.IKY;
using TNS.KYM;

namespace Istemci.InsanKaynaklari
{
    /// <summary>Kaydet / Kontrol isteği: eski EkrandanAl'ın okuduğu ekran alanları (tarihler "yyyy-MM-dd").</summary>
    public class PersonelKisiselIstegi
    {
        public int kod { get; set; }
        public string tcKimlikNo { get; set; }
        public string sicilNo { get; set; }
        public string ad { get; set; }
        public string soyad { get; set; }
        public int sosyalGuvence { get; set; }
        public string emekliSicilNo { get; set; }
        public string sskSicilNo { get; set; }
        public string bagkurSicilNo { get; set; }
        public string ilkIseBaslamaTarihi { get; set; }
        public string kurumaBaslamaTarihi { get; set; }
        public string asaletTarihi { get; set; }
        public string arsivNo { get; set; }
        public bool aktifMi { get; set; }
        public string geldigiKurum { get; set; }
        public string geldigiKurumSicili { get; set; }
        public string vergiDairesi { get; set; }
        public string bankaIbanNo { get; set; }
        public string rumuz { get; set; }
        public string titr { get; set; }
        public int sakatlikDurumu { get; set; }
        public int sakatlikDerecesi { get; set; }
        public bool terorMagduru { get; set; }
        public bool hukumlu { get; set; }
        public bool korunmayaMuhtac { get; set; }
        public int cinsiyet { get; set; }
        public string ilkSoyad { get; set; }
        public string babaAd { get; set; }
        public string anaAd { get; set; }
        public string dogumTarihi { get; set; }
        public string dogumYeri { get; set; }
        public int medeniHali { get; set; }
        public string dini { get; set; }
        public string kanGrubu { get; set; }
        public string cuzdanSeriNo { get; set; }
        public string cuzdanKayitNo { get; set; }
        public string nufusIlKod { get; set; }
        public string nufusIlceKod { get; set; }
        public string nufusMahalleKoy { get; set; }
        public string nufusCiltNo { get; set; }
        public string nufusSiraNo { get; set; }
        public string nufusAileSiraNo { get; set; }
        public string cuzdanVerilisNedeni { get; set; }
        public string cuzdanVerildigiYer { get; set; }
        public string cuzdanVerilisTarihi { get; set; }
        public string pasaportNo { get; set; }

        // Yalnızca yeni personelde (eski "Yeni Personel Birim/Unvan/Tür Girişi" penceresi)
        public int unvanKod { get; set; }
        public int birimKod { get; set; }
        public int personelTip { get; set; }
        public string kadroDerece { get; set; }
        public string ekGosterge { get; set; }
        /// <summary>Atanacağı kadro: "unvanKod-kadroDerece" (KadroDoluBos.ID).</summary>
        public string atanacagiKadro { get; set; }
    }

    public class PersonelTcDegistirIstegi
    {
        public int personelKod { get; set; }
        public string yeniTcKimlikNo { get; set; }
    }

    public class PersonelKisiselBelgeSilIstegi
    {
        public int personelKod { get; set; }
        public int kod { get; set; }
    }

    /// <summary>
    /// Personel İşlemleri / Kişisel sekmesi (eski InsanKaynaklari/PersonelKisisel.aspx; yeni sayfa InsanKaynaklari/PersonelKisisel)
    /// uçları. personelKod 0 ise sayfa yeni personel girişi içindir (eski davranış). İş kuralları eski sayfadaki gibi IIKYServis
    /// (PersonelListele, PersonelKaydet, PersonelTCKimlikNoDegistir, PersonelResimKaydet, PersonelBelge*, PersonelBulOncekiSonraki,
    /// DoluBosKadroListele, SozlukListele, UnvanListele, MailImzaRaporu, NakilBildirimRaporu, HizmetBelgesiRaporu ...), kimlik
    /// sorgusu TNS.KPM (eski Genel.KimlikBilgisiGetir) ve TNS.KYM.MernisBilgiOku.
    ///   GET  /api/personelbilgi/kisisel/tanim                         açılır listeler ve ayarlar (eski Page_Load)
    ///   GET  /api/personelbilgi/kisisel/ilceler?ilKod=                (eski IlBilgisiDegisti / IlceYukle)
    ///   GET  /api/personelbilgi/kisisel/oku?personelKod=              (eski Listele)
    ///   GET  /api/personelbilgi/kisisel/kimlik?tcKimlikNo=            (eski KimlikBilgisiGetir: MERNİS / KPS)
    ///   POST /api/personelbilgi/kisisel/kontrol                       (eski AlanlariKontrolEt: yeni personelde pencereden önce)
    ///   POST /api/personelbilgi/kisisel/kaydet                        (eski btnKaydet_Click)
    ///   POST /api/personelbilgi/kisisel/tckimlikno-degistir           (eski btnTCKimlikNoDegistir_Click)
    ///   POST /api/personelbilgi/kisisel/resim        (multipart: personelKod, dosya)   (eski ResimKayit)
    ///   GET  /api/personelbilgi/kisisel/sicil?sicilNo=&amp;yon=onceki|sonraki   (eski txtSicilNo_TriggerClick)
    ///   GET  /api/personelbilgi/kisisel/orgagac?ust=&amp;ara=             birim seçimi (eski ExtOrgSecim ddlBirim)
    ///   GET  /api/personelbilgi/kisisel/acikkadro?personelTip=&amp;birim= (eski cmbPersonelTipDegisti / Genel.AcikKadroDoldur)
    ///   GET  /api/personelbilgi/kisisel/belgeler?personelKod=         (eski BelgeListesiDoldur)
    ///   POST /api/personelbilgi/kisisel/belge-yukle  (multipart: personelKod, dosya)   (eski BelgeKaydet)
    ///   GET  /api/personelbilgi/kisisel/belge-indir?kod=              (eski BelgeGoster)
    ///   POST /api/personelbilgi/kisisel/belge-sil                     (eski BelgeSil)
    ///   GET  /api/personelbilgi/kisisel/rapor/tanitma?personelKod=    Per. Tanıtma Kartı, PDF (eski TanitmaKartiYazdir)
    ///   GET  /api/personelbilgi/kisisel/rapor/mailimza?personelKod=   Mail İmza, PDF (eski MailImzaYazdir)
    ///   GET  /api/personelbilgi/kisisel/rapor/nakil?personelKod=      Nakil Bildirim, Excel (eski NakilBildirimiYazdir)
    /// Nüfus Cüzdanı Örneği / İkametgah İlmühaberi ve Kişi Borcu düğmeleri ortak uçları kullanır (PersonelOrtakRaporlar).
    /// </summary>
    public static class PersonelKisiselUclari
    {
        private const string SayfaAd = "PersonelKisisel";
        private const string KayitBasarili = "Kayıt işlemi başarıyla gerçekleşti."; // eski Resources.InsanKaynaklari.FRMPKS071

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/personelbilgi/kisisel/tanim", Tanim);
            app.MapGet("/api/personelbilgi/kisisel/ilceler", Ilceler);
            app.MapGet("/api/personelbilgi/kisisel/oku", Oku);
            app.MapGet("/api/personelbilgi/kisisel/kimlik", Kimlik);
            app.MapPost("/api/personelbilgi/kisisel/kontrol", Kontrol);
            app.MapPost("/api/personelbilgi/kisisel/kaydet", Kaydet);
            app.MapPost("/api/personelbilgi/kisisel/tckimlikno-degistir", TcKimlikNoDegistir);
            app.MapPost("/api/personelbilgi/kisisel/resim", ResimKayit).DisableAntiforgery();
            app.MapGet("/api/personelbilgi/kisisel/sicil", Sicil);
            app.MapGet("/api/personelbilgi/kisisel/orgagac", OrgAgac);
            app.MapGet("/api/personelbilgi/kisisel/acikkadro", AcikKadro);
            app.MapGet("/api/personelbilgi/kisisel/belgeler", Belgeler);
            app.MapPost("/api/personelbilgi/kisisel/belge-yukle", BelgeKaydet).DisableAntiforgery();
            app.MapGet("/api/personelbilgi/kisisel/belge-indir", BelgeGoster);
            app.MapPost("/api/personelbilgi/kisisel/belge-sil", BelgeSil);
            app.MapGet("/api/personelbilgi/kisisel/rapor/tanitma", TanitmaKarti);
            app.MapGet("/api/personelbilgi/kisisel/rapor/mailimza", MailImza);
            app.MapGet("/api/personelbilgi/kisisel/rapor/nakil", NakilBildirim);
        }

        private static IIKYServis Servis() { return TNS.IKY.Arac.Tanimla(); }

        private static IResult Hata(string metin) { return Results.Json(new { hata = metin }); }

        private static IResult Basarisiz(string metin) { return Results.Json(new { basarili = false, hata = metin }); }

        private static string HataMetni(string hata, string varsayilan) { return PersonelBilgiOrtak.HataMetni(hata, varsayilan); }

        private static string T(string s) { return (s ?? "").Trim(); }

        /// <summary>Eski Page_Load: Mernis bağlantısı varsa nüfus alanları kilitlenir (Manas'ta kilit yok).</summary>
        private static bool MernisKilit()
        {
            if (TNS.DEG.Arac.ManasKullaniyor()) return false;
            return !TNS.KYM.Arac.Tanimla().MernisKontroluYapma();
        }

        // ------------------------------------------------------------------ açılır listeler (eski XxxDoldur)

        /// <summary>Eski Genel.SozlukYukle: ilk seçenek (0, ""); alt seviyeler "- " ile girintili.</summary>
        private static List<object> SozlukYukle(ENUMSozlukUnsurlari unsur)
        {
            Sozluk kriter = new Sozluk { kayitKod = ((int)unsur).ToString().PadLeft(2, '0'), seviye = 2 };
            ObjectArray oa = Servis().SozlukListele(kriter);
            List<object> liste = new List<object>();
            liste.Add(new { kod = 0, ad = "" });
            if (oa != null && oa.objeler != null)
                foreach (Sozluk s in oa.objeler)
                {
                    string ad = "";
                    for (int i = 2; i < s.seviye; i++) ad += "- ";
                    liste.Add(new { kod = s.kod, ad = ad + s.ad });
                }
            return liste;
        }

        /// <summary>Eski Genel.SozlukKoduGetir.</summary>
        private static int SozlukKoduGetir(string kayitKod)
        {
            ObjectArray list = Servis().SozlukListele(new Sozluk { kayitKod = kayitKod, birebir = true });
            foreach (Sozluk s in list.objeler)
                return s.kod;
            return 0;
        }

        private static List<object> iller = null;

        /// <summary>Eski GenelIslemlerIstemci.IlYukle: kod türü 20, seviye 3, yalnızca ad (sıralı); kod noktasız.</summary>
        private static List<object> IlYukle(Kullanici kullanan)
        {
            if (iller != null) return iller;
            List<object> liste = KodListesi(kullanan, "", 3);
            if (liste.Count > 0) iller = liste;
            return liste;
        }

        /// <summary>Eski GenelIslemlerIstemci.IlceYukle: kod türü 20, seviye 4, ilin altı.</summary>
        private static List<object> IlceYukle(Kullanici kullanan, string ilKod)
        {
            if (string.IsNullOrWhiteSpace(ilKod)) return new List<object>();
            return KodListesi(kullanan, ilKod.Trim(), 4);
        }

        /// <summary>Eski GenelIslemlerIstemci.DDLKodDoldur(ddl, 0, "20", ustKod, seviye, true, true).</summary>
        private static List<object> KodListesi(Kullanici kullanan, string ustKod, int seviye)
        {
            List<object> liste = new List<object>();
            ABSKodlarVAN asamav = TNS.BKD.Arac.Tanimla().KodListeleSeviye(0, "20", ustKod, seviye, true);
            if (asamav == null || asamav.sonuc == null || !asamav.sonuc.islemSonuc || asamav.absKodlar == null) return liste;
            string kultur = string.IsNullOrEmpty(kullanan.aktifKultur) ? "tr" : kullanan.aktifKultur;
            asamav.absKodlar.Sort();
            foreach (ABSKod kod in asamav.absKodlar)
                liste.Add(new { kod = (kod.kod ?? "").Replace(".", "").Trim(), ad = (kod.Ad(kultur) ?? "").Trim() });
            return liste;
        }

        /// <summary>Eski Genel.CografyaKodunaCevir: MERNİS il / ilçe kodunu coğrafya koduna çevirir.</summary>
        private static string CografyaKodunaCevir(string mernisKod)
        {
            return TNS.UZY.Arac.Tanimla().UzayDegeriStr(new Kullanici(), "COGRAFYAMERNIS", mernisKod, true);
        }

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                // Eski Genel.UnvanDoldur(servisIKY, ...): bütün unvanlar, başta boş; görünen "ad (sınıf kısa adı)".
                List<object> unvanlar = new List<object>();
                unvanlar.Add(new { kod = 0, ad = "" });
                ObjectArray ua = Servis().UnvanListele(new Unvan());
                if (ua.sonuc.islemSonuc)
                    foreach (Unvan u in ua.objeler)
                        unvanlar.Add(new { kod = u.UnvanKod, ad = u.UnvanAd });

                List<object> cinsiyetler = new List<object>();
                foreach (Sozluk s in TNS.IKY.Arac.CinsiyetListe)
                    cinsiyetler.Add(new { kod = s.kod, ad = s.ad });

                // Eski Genel.PersonelTurDoldur(cmbPersonelTip, personelTipStore, null)
                List<object> personelTipleri = new List<object>();
                foreach (PersonelTipi p in TNS.IKY.Arac.PersonelTipListe)
                    personelTipleri.Add(new { kod = p.kod, ad = p.ad });

                return Results.Json(new
                {
                    unvanlar = unvanlar,
                    iller = IlYukle(kullanan),
                    cinsiyetler = cinsiyetler,
                    medeniHaller = SozlukYukle(ENUMSozlukUnsurlari.MEDENIDURUMU),
                    sakatlikDurumlari = SozlukYukle(ENUMSozlukUnsurlari.SAKATLIKDURUMU),
                    sosyalGuvenceler = SozlukYukle(ENUMSozlukUnsurlari.SOSYALGUVENCEKOD),
                    personelTipleri = personelTipleri,
                    varsayilanPersonelTip = 100,
                    kvkk = TNS.IKY.Arac.KVKK(),          // eski: Nüfus Bilgileri sekmesi gizlenir
                    mernisKilit = MernisKilit(),          // eski: nüfus alanları salt okunur
                    manas = TNS.DEG.Arac.ManasKullaniyor() // eski: kimlik sorgusu yok, bazı alanlar gizli
                });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Ekran bilgileri okunamadı."));
            }
        }

        private static IResult Ilceler(HttpContext baglam, IWebHostEnvironment ortam, string ilKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                return Results.Json(new { ilceler = IlceYukle(kullanan, ilKod) });
            }
            catch (Exception)
            {
                return Hata("İlçeler okunamadı.");
            }
        }

        // ------------------------------------------------------------------ oku (eski Listele)

        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            int kod = personelKod ?? 0;
            if (kod <= 0) return Results.Json(new { personel = (object)null });

            TNS.IKY.Personel kriter = new TNS.IKY.Personel();
            kriter.kod = kod;
            kriter.tarihce = true;
            ObjectArray dler = Servis().PersonelListele(kullanan, kriter, true, false);
            if (!dler.sonuc.islemSonuc) return Hata(HataMetni(dler.sonuc.hataStr, "Personel bilgisi okunamadı."));
            if (dler.objeler.Count == 0) return Results.Json(new { personel = (object)null });

            TNS.IKY.Personel d = (TNS.IKY.Personel)dler.objeler[0];
            bool manas = TNS.DEG.Arac.ManasKullaniyor();

            return Results.Json(new
            {
                manasGizle = manas, // eski: Manas'ta sosyal güvence, emekli / bağkur sicil, asalet, arşiv no ... gizlenir ve okunmaz
                ilceler = IlceYukle(kullanan, d.nufus.nufusIlKod),
                personel = new
                {
                    kod = d.kod,
                    tcKimlikNo = d.tcKimlikNo,
                    sicilNo = d.sicilNo,
                    ad = d.ad,
                    soyad = d.soyad,
                    unvanAd = d.unvanAd,
                    unvanVekaletAd = d.unvanVekaletAd,
                    birimAd = d.birimAd,
                    gorevBirimAd = d.kadro.gorevBirimAd,
                    personelTipAd = d.personelTipAd,
                    kadroBaslangicTarihi = d.kadroBaslangicTarihi.Year != 1 ? PersonelBilgiOrtak.TarihYaz(d.kadroBaslangicTarihi) : "",
                    dereceEkGosterge = d.kadro.kadroDerece + "/" + d.kadro.emekliyeEsasEkGosterge,

                    sosyalGuvence = manas ? 0 : d.sosyalGuvence,
                    emekliSicilNo = manas ? "" : d.emekliSicilNo,
                    bagkurSicilNo = manas ? "" : d.bagkurSicilNo,
                    asaletTarihi = manas ? "" : PersonelBilgiOrtak.TarihYaz(d.asaletTarihi),
                    arsivNo = manas ? "" : d.arsivNo,
                    geldigiKurumSicili = manas ? "" : d.geldigiKurumSicili,
                    vergiDairesi = manas ? "" : d.vergiDairesi,
                    rumuz = manas ? "" : d.rumuz,
                    terorMagduru = !manas && d.terorMagduru > 0,
                    hukumlu = !manas && d.hukumlu > 0,
                    korunmayaMuhtac = !manas && d.korunmayaMuhtac > 0,

                    sskSicilNo = d.sskSicilNo,
                    ilkIseBaslamaTarihi = PersonelBilgiOrtak.TarihYaz(d.ilkIseBaslamaTarihi),
                    kurumaBaslamaTarihi = PersonelBilgiOrtak.TarihYaz(d.kurumaBaslamaTarihi),
                    aktifMi = d.aktifMi == 1,
                    eposta = d.iletisim.eposta,
                    bankaIbanNo = d.bankaIbanNo,
                    geldigiKurum = d.geldigiKurum,
                    titr = d.titr,
                    sakatlikDurumu = d.sakatlikDurumu,
                    sakatlikDerecesi = d.sakatlikDerecesi,

                    cinsiyet = (int)d.cinsiyet,
                    ilkSoyad = d.ilkSoyad,
                    babaAd = d.nufus.babaAd,
                    anaAd = d.nufus.anaAd,
                    dogumTarihi = PersonelBilgiOrtak.TarihYaz(d.dogumTarihi),
                    dogumYeri = d.nufus.dogumYeri,
                    medeniHali = d.nufus.medeniHali,
                    dini = d.nufus.dini,
                    kanGrubu = d.nufus.kanGrubu,
                    cuzdanSeriNo = d.nufus.cuzdanSeriNo,
                    cuzdanKayitNo = d.nufus.cuzdanKayitNo,
                    nufusIlKod = d.nufus.nufusIlKod,
                    nufusIlceKod = d.nufus.nufusIlceKod,
                    nufusMahalleKoy = d.nufus.nufusMahalleKoy,
                    nufusCiltNo = d.nufus.nufusCiltNo,
                    nufusSiraNo = d.nufus.nufusSiraNo,
                    nufusAileSiraNo = d.nufus.nufusAileSiraNo,
                    cuzdanVerilisNedeni = d.nufus.cuzdanVerilisNedeni,
                    cuzdanVerildigiYer = d.nufus.cuzdanVerildigiYer,
                    cuzdanVerilisTarihi = PersonelBilgiOrtak.TarihYaz(d.nufus.cuzdanVerilisTarihi),
                    pasaportNo = d.nufus.pasaportNo
                }
            });
        }

        // ------------------------------------------------------------------ kimlik sorgusu (eski KimlikBilgisiGetir)

        private static IResult Kimlik(HttpContext baglam, IWebHostEnvironment ortam, string tcKimlikNo)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            string tc = T(tcKimlikNo);
            if (tc == "") return Hata("Personel T.C.Kimlik No bilgisi boş bırakılamaz."); // eski FRMPKS072

            TNS.IKY.Personel p;
            try
            {
                p = KimlikBilgisiGetir(kullanan, tc);
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Kimlik bilgisi alınamadı."));
            }

            try
            {
                if (!string.IsNullOrEmpty(p.ad))
                {
                    string ilKod = CografyaKodunaCevir(p.nufus.nufusIlKod);
                    int medeniHali = SozlukKoduGetir(((int)ENUMSozlukUnsurlari.MEDENIDURUMU).ToString().PadLeft(2, '0') + "." + p.nufus.medeniHali.ToString().PadLeft(5, '0'));
                    return Results.Json(new
                    {
                        nufus = new
                        {
                            tcKimlikNo = p.tcKimlikNo,
                            ad = p.ad,
                            soyad = p.soyad,
                            cuzdanSeriNo = p.nufus.cuzdanSeriNo,
                            cuzdanKayitNo = p.nufus.cuzdanKayitNo,
                            cuzdanVerilisTarihi = PersonelBilgiOrtak.TarihYaz(p.nufus.cuzdanVerilisTarihi),
                            cuzdanVerilisNedeni = p.nufus.cuzdanVerilisNedeni,
                            cuzdanVerildigiYer = p.nufus.cuzdanVerildigiYer,
                            nufusMahalleKoy = p.nufus.nufusMahalleKoy,
                            nufusCiltNo = p.nufus.nufusCiltNo,
                            nufusAileSiraNo = p.nufus.nufusAileSiraNo,
                            nufusSiraNo = p.nufus.nufusSiraNo,
                            dogumYeri = p.nufus.dogumYeri,
                            kanGrubu = p.nufus.kanGrubu,
                            medeniHali = medeniHali,
                            pasaportNo = p.nufus.pasaportNo,
                            pasaportVerilisTarihi = PersonelBilgiOrtak.TarihYaz(p.nufus.pasaportVerilisTarihi),
                            pasaportVerildigiYer = p.nufus.pasaportVerildigiYer,
                            pasaportVerenMakam = p.nufus.pasaportVerenMakam,
                            pasaportSuresi = p.nufus.pasaportSuresi,
                            anaAd = p.nufus.anaAd,
                            babaAd = p.nufus.babaAd,
                            dini = p.nufus.dini,
                            ilkSoyad = p.ilkSoyad,
                            dogumTarihi = PersonelBilgiOrtak.TarihYaz(p.dogumTarihi),
                            cinsiyet = (int)p.cinsiyet,
                            nufusIlKod = ilKod,
                            nufusIlceKod = CografyaKodunaCevir(p.nufus.nufusIlceKod)
                        },
                        ilceler = IlceYukle(kullanan, ilKod)
                    });
                }

                // Kurumda nüfus cüzdanı kontrolü için yetki yoksa
                KullaniciVAN kVAN = TNS.KYM.Arac.Tanimla().MernisBilgiOku(kullanan, tc, p.dogumTarihi);
                if (kVAN.sonuc.islemSonuc)
                {
                    // Eski: "Erkek" ise cinsiyet listesinin 2. öğesi (Erkek); değilse 3. öğe (listede yok: seçim boş kalıyordu).
                    int cinsiyet = kVAN.kullanici.cinsiyet == "Erkek" ? (int)ENUMCINSIYET.ERKEK
                        : (kVAN.kullanici.cinsiyet ?? "").StartsWith("Kad", StringComparison.OrdinalIgnoreCase) ? (int)ENUMCINSIYET.KADIN : 0;
                    return Results.Json(new
                    {
                        mernis = new
                        {
                            ad = kVAN.kullanici.ad,
                            soyad = kVAN.kullanici.soyad,
                            dogumTarihi = PersonelBilgiOrtak.TarihYaz(kVAN.kullanici.dogumTarihi),
                            anaAd = kVAN.kullanici.anaAdi,
                            babaAd = kVAN.kullanici.babaAdi,
                            cinsiyet = cinsiyet
                        }
                    });
                }
                return Results.Json(new { mernis = new { ad = "", soyad = "", bulunamadi = true } });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Kimlik bilgisi alınamadı."));
            }
        }

        /// <summary>Eski Genel.KimlikBilgisiGetir: KPS nüfus kayıt örneği sorgusu (iş kuralı TNS.KPM).</summary>
        private static TNS.IKY.Personel KimlikBilgisiGetir(Kullanici kullanan, string tcKimlikNo)
        {
            TNS.KPM.IKPMServis servisKPM = TNS.KPM.Arac.Tanimla();

            TNS.KPM.Istek istek = new TNS.KPM.Istek();
            istek.istekTur = ((int)TNS.KPM.ENUMIstekTur.NUFUSCUZDANORNEK);
            istek.tcKimlikNo = tcKimlikNo;
            istek.adSoyad = kullanan.ADSOYAD; //Nufus kaydı için isim göndermek gerekiyor.
            istek.islemYapan = kullanan.kullaniciKodu;
            istek.islemTarih.Yaz(DateTime.Now);
            istek.entegrasyon = true;

            ObjectArray nufusList = servisKPM.NufusKayitOrnekRaporla(kullanan, istek);

            TNS.KPM.Kisi nufusOrnek = new TNS.KPM.Kisi();
            TNS.IKY.Personel p = new TNS.IKY.Personel();

            if (!nufusList.sonuc.islemSonuc)
                throw new Exception("Nüfus sorgusu hata ile sonuçlandı: " + nufusList.sonuc.hataStr);

            if (nufusList.ObjeSayisi > 0)
                nufusOrnek = (TNS.KPM.Kisi)nufusList.objeler[0];
            else
                throw new Exception("Nüfus Sorgusu Boş Kayıt Döndürdü");

            p.tcKimlikNo = nufusOrnek.tcKimlikNo;
            p.nufus.cuzdanSeriNo = nufusOrnek.nufusCuzdani.nufusCuzdanSeri;
            p.nufus.cuzdanKayitNo = nufusOrnek.nufusCuzdani.kayitNo;
            p.nufus.cuzdanVerilisTarihi = new TNSDateTime(nufusOrnek.nufusCuzdani.verilisTarihi.ToString());
            p.nufus.cuzdanVerilisNedeni = nufusOrnek.nufusCuzdani.verilisNedeniAciklama;
            p.nufus.cuzdanVerildigiYer = nufusOrnek.nufusCuzdani.verildigiYer;
            p.nufus.nufusMahalleKoy = nufusOrnek.kayitliOlduguYer.mahalleKoy;
            p.nufus.nufusCiltNo = nufusOrnek.kayitliOlduguYer.ciltNo.ToString();
            p.nufus.nufusAileSiraNo = nufusOrnek.kayitliOlduguYer.aileSiraNo.ToString();
            p.nufus.nufusSiraNo = nufusOrnek.kayitliOlduguYer.bireySiraNo.ToString();

            p.ad = nufusOrnek.ad;
            p.soyad = nufusOrnek.soyad;
            p.nufus.anaAd = nufusOrnek.anaAd;
            p.nufus.babaAd = nufusOrnek.babaAd;
            p.nufus.dogumYeri = nufusOrnek.dogumYeri;
            p.nufus.dogumTarihi = new TNSDateTime(nufusOrnek.dogumTarihi.ToString());
            p.nufus.dini = nufusOrnek.din.ToString();
            p.nufus.ilkSoyad = nufusOrnek.kizlikSoyad;

            p.nufus.kanGrubu = nufusOrnek.kanGrubu;

            p.nufus.cinsiyeti = nufusOrnek.cinsiyet == 2 ? ENUMCINSIYET.KADIN : ENUMCINSIYET.ERKEK;
            p.nufus.medeniHali = nufusOrnek.medeniHal;

            p.nufus.nufusIlKod = nufusOrnek.kayitliOlduguYer.ilce.il.kod.ToString();
            p.nufus.nufusIlceKod = nufusOrnek.kayitliOlduguYer.ilce.kod.ToString();

            return p;
        }

        // ------------------------------------------------------------------ kaydet (eski EkrandanAl / AlanlariKontrolEt / btnKaydet_Click)

        private static TNS.IKY.Personel EkrandanAl(PersonelKisiselIstegi i)
        {
            TNS.IKY.Personel d = new TNS.IKY.Personel();

            d.kod = i.kod;

            d.tcKimlikNo = T(i.tcKimlikNo);
            d.sicilNo = T(i.sicilNo);
            d.ad = T(i.ad);
            d.soyad = T(i.soyad);
            d.sosyalGuvence = i.sosyalGuvence;
            d.emekliSicilNo = T(i.emekliSicilNo);
            d.sskSicilNo = T(i.sskSicilNo);
            d.bagkurSicilNo = T(i.bagkurSicilNo);
            d.ilkIseBaslamaTarihi = PersonelBilgiOrtak.TnsTarih(i.ilkIseBaslamaTarihi);
            d.kurumaBaslamaTarihi = PersonelBilgiOrtak.TnsTarih(i.kurumaBaslamaTarihi);
            d.asaletTarihi = PersonelBilgiOrtak.TnsTarih(i.asaletTarihi);
            d.arsivNo = i.arsivNo ?? "";
            d.aktifMi = i.aktifMi ? 1 : 0;
            d.geldigiKurum = i.geldigiKurum ?? "";
            d.geldigiKurumSicili = i.geldigiKurumSicili ?? "";
            d.vergiDairesi = i.vergiDairesi ?? "";
            d.bankaIbanNo = i.bankaIbanNo ?? "";

            d.rumuz = i.rumuz ?? "";
            d.titr = i.titr ?? "";

            d.sakatlikDurumu = i.sakatlikDurumu;
            d.sakatlikDerecesi = i.sakatlikDerecesi;
            d.terorMagduru = i.terorMagduru ? 1 : 0;
            d.hukumlu = i.hukumlu ? 1 : 0;
            d.korunmayaMuhtac = i.korunmayaMuhtac ? 1 : 0;

            d.nufus.kod = i.kod;
            d.nufus.cinsiyeti = (ENUMCINSIYET)i.cinsiyet;
            d.nufus.ilkSoyad = T(i.ilkSoyad);
            d.nufus.babaAd = T(i.babaAd);
            d.nufus.anaAd = T(i.anaAd);
            d.nufus.dogumTarihi = PersonelBilgiOrtak.TnsTarih(i.dogumTarihi);
            d.nufus.dogumYeri = i.dogumYeri ?? "";
            d.nufus.medeniHali = i.medeniHali;
            d.nufus.dini = i.dini ?? "";
            d.nufus.kanGrubu = i.kanGrubu ?? "";
            d.nufus.cuzdanSeriNo = T(i.cuzdanSeriNo);
            d.nufus.cuzdanKayitNo = T(i.cuzdanKayitNo);
            d.nufus.nufusIlKod = i.nufusIlKod ?? "";
            d.nufus.nufusIlceKod = i.nufusIlceKod ?? "";
            d.nufus.nufusMahalleKoy = T(i.nufusMahalleKoy);
            d.nufus.nufusCiltNo = T(i.nufusCiltNo);
            d.nufus.nufusSiraNo = T(i.nufusSiraNo);
            d.nufus.nufusAileSiraNo = T(i.nufusAileSiraNo);
            d.nufus.cuzdanVerilisNedeni = T(i.cuzdanVerilisNedeni);
            d.nufus.cuzdanVerildigiYer = T(i.cuzdanVerildigiYer);
            d.nufus.cuzdanVerilisTarihi = PersonelBilgiOrtak.TnsTarih(i.cuzdanVerilisTarihi);
            d.nufus.pasaportNo = T(i.pasaportNo);

            if (d.kod == 0)
            {
                d.UNVANKOD = i.unvanKod;
                d.kadro.organizasyonKodu = i.birimKod;
                d.kadro.personelTip = i.personelTip;
                d.kadro.kadroDerece = OrtakFonksiyonlar.ConvertToInt(T(i.kadroDerece), 0);
                d.kadro.kadroTarih = new TNSDateTime(DateTime.Now);

                d.kadro.kazanilmisHakDerece = d.kadro.kadroDerece;
                d.kadro.kazanilmisEkGosterge = OrtakFonksiyonlar.ConvertToInt(T(i.ekGosterge), 0);
                d.kadro.kazanilmisHakKademe = 1;
                d.kadro.kazanilmisHakTarih = d.kadro.kadroTarih;

                d.kadro.emekliyeEsasDerece = d.kadro.kadroDerece;
                d.kadro.emekliyeEsasEkGosterge = d.kadro.kazanilmisEkGosterge;
                d.kadro.emekliyeEsasKademe = 1;
                d.kadro.emekliyeEsasTarih = d.kadro.kadroTarih;

                d.kadro.odemeyeEsasDerece = d.kadro.kadroDerece;
                d.kadro.odemeyeEsasEkGosterge = d.kadro.kazanilmisEkGosterge;
                d.kadro.odemeyeEsasKademe = 1;
                d.kadro.odemeyeEsasTarih = d.kadro.kadroTarih;

                string[] secilenKadro = (i.atanacagiKadro ?? "").Split('-');
                d.kadro.atananacagiKadroUnvanKod = OrtakFonksiyonlar.ConvertToInt(secilenKadro.Length > 0 ? secilenKadro[0] : "", 0);
                d.kadro.atananacagiKadroDerece = OrtakFonksiyonlar.ConvertToInt(secilenKadro.Length > 1 ? secilenKadro[1] : "", 0);
            }
            return d;
        }

        private static async Task<IResult> Kontrol(HttpContext baglam, IWebHostEnvironment ortam, PersonelKisiselIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                string hata = EkrandanAl(istek).Kontrol(kullanan);
                if (!string.IsNullOrEmpty(hata)) return Basarisiz(PersonelBilgiOrtak.Duz(hata));
                return Results.Json(new { basarili = true });
            }
            catch (Exception ex)
            {
                return Basarisiz(HataMetni(ex.Message, "Bilgiler denetlenemedi."));
            }
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, PersonelKisiselIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            TNS.IKY.Personel d = EkrandanAl(istek);
            Sonuc sonuc = Servis().PersonelKaydet(kullanan, d);
            if (!sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc.hataStr, "Personel bilgisi kaydedilemedi."));

            // Eski: yeni kayıtta KayittanSonraGoster(anahtar)
            int yeniKod = d.kod == 0 ? OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, 0) : 0;
            return Results.Json(new { basarili = true, bilgi = KayitBasarili, yeniKod = yeniKod });
        }

        private static async Task<IResult> TcKimlikNoDegistir(HttpContext baglam, IWebHostEnvironment ortam, PersonelTcDegistirIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            string hata = "";
            string yeniTcKimlikNo = T(istek.yeniTcKimlikNo);

            TNS.IKY.Personel d = new TNS.IKY.Personel();
            d.kod = istek.personelKod;
            if (d.kod > 0)
            {
                d.tcKimlikNo = yeniTcKimlikNo;
                Sonuc sonuc = Servis().PersonelTCKimlikNoDegistir(kullanan, d);

                if (!sonuc.islemSonuc)
                    hata = HataMetni(sonuc.hataStr, "TC Kimlik No değiştirilemedi.");
            }
            else
                hata = "Personel bilgisi bulunamadı! Lütfen öncelikle personel seçiniz.";

            if (!string.IsNullOrWhiteSpace(hata)) return Basarisiz(hata);
            return Results.Json(new { basarili = true, bilgi = "TC Kimlik No bilgisi değiştirildi.", tcKimlikNo = yeniTcKimlikNo });
        }

        // ------------------------------------------------------------------ resim (eski ResimKayit)

        private static async Task<IResult> ResimKayit(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!baglam.Request.HasFormContentType) return Results.BadRequest();

            IFormCollection form = await baglam.Request.ReadFormAsync();
            int kod = OrtakFonksiyonlar.ConvertToInt((string)form["personelKod"], 0);
            if (kod == 0)
                return Basarisiz("Personel bilgisini kayıt ettikten sonra resim ekleme işlemini yapınız.."); // eski FRMGNL004
            IFormFile dosya = form.Files["dosya"];
            if (dosya == null || dosya.Length == 0) return Basarisiz("Resim dosyası seçiniz.");

            PersonelResim resim = new PersonelResim();
            using (MemoryStream bellek = new MemoryStream())
            {
                await dosya.CopyToAsync(bellek);
                resim.resim = bellek.ToArray();
            }

            System.Drawing.Image orjinalResim;
            try
            {
                orjinalResim = System.Drawing.Image.FromStream(new MemoryStream(resim.resim));
            }
            catch (Exception)
            {
                return Basarisiz("Seçilen dosya resim olarak okunamadı.");
            }

            using (orjinalResim)
            {
                //Küçültülecek boyutlar
                int dosyaGenislik = 114;
                int dosyaYukseklik = 146;

                //Ratio'yu koru
                double rW = dosyaGenislik * 1.0 / orjinalResim.Width;
                double rH = dosyaYukseklik * 1.0 / orjinalResim.Height;
                double ra = Math.Min(rW, rH);
                if (ra > 0)
                {
                    dosyaGenislik = (int)(orjinalResim.Width * ra);
                    dosyaYukseklik = (int)(orjinalResim.Height * ra);
                }

                using (Bitmap kucukResim = new Bitmap(dosyaGenislik, dosyaYukseklik))
                {
                    using (Graphics graphics = Graphics.FromImage(kucukResim))
                    {
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        graphics.CompositingQuality = CompositingQuality.HighSpeed;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.CompositingMode = CompositingMode.SourceCopy;
                        graphics.DrawImage(orjinalResim, 0, 0, dosyaGenislik, dosyaYukseklik);
                    }
                    try
                    {
                        //Küçültülen resim byte[] dönüştürülüyor
                        MemoryStream ms = new MemoryStream();
                        kucukResim.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        resim.resim = ms.ToArray();
                    }
                    catch { }
                }
            }

            resim.kod = kod;
            Sonuc sonuc = Servis().PersonelResimKaydet(resim);
            if (sonuc != null && !sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc.hataStr, "Resim kaydedilemedi."));
            return Results.Json(new { basarili = true, bilgi = "Resim kaydedildi." });
        }

        // ------------------------------------------------------------------ sicil önceki / sonraki (eski txtSicilNo_TriggerClick)

        private static IResult Sicil(HttpContext baglam, IWebHostEnvironment ortam, string sicilNo, string yon)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            string s = sicilNo ?? "";
            // Eski: aşağı ok (tusNo 0) önceki, yukarı ok (tusNo 1) sonraki
            bool onceki = !string.Equals(yon, "sonraki", StringComparison.OrdinalIgnoreCase);
            ObjectArray oa = Servis().PersonelBulOncekiSonraki(kullanan, s, onceki);
            if (!oa.sonuc.islemSonuc) return Hata(HataMetni(oa.sonuc.hataStr, "Personel aranamadı."));
            if (oa.ObjeSayisi == 0)
                return Hata(s + " sicil nolu personelden önceki/sonraki sicil numaralı personel bulunamadı!");
            TNS.IKY.Personel p = (TNS.IKY.Personel)oa.objeler[0];
            return Results.Json(new { kod = p.kod });
        }

        // ------------------------------------------------------------------ yeni personel penceresi: birim ve açık kadro

        /// <summary>
        /// Eski ExtOrgSecim + ORGBirim.NodeLoad: kökte kullanıcının "Personel Saymanlık-Birim Yetkisi"ne göre birimler (yetkisi yoksa
        /// bütün kök birimler); ara yazılınca adında geçen birimler düz liste olarak (il / ilçe adıyla) gelir.
        /// </summary>
        private static IResult OrgAgac(HttpContext baglam, IWebHostEnvironment ortam, int? ust, string ara)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                ara = T(ara);
                TNS.ORG.OrganizasyonBirim kriter = new TNS.ORG.OrganizasyonBirim();
                kriter.anaBirimKod = Math.Max(0, ust ?? 0);
                kriter.sablonMu = false;
                if (ara != "")
                {
                    kriter.anaBirimKod = 0;
                    kriter.adi = ara;
                }
                else if (kriter.anaBirimKod == 0)
                {
                    string yetkili = kullanan.YetkiTipiDegeriBul("Personel Saymanlık-Birim Yetkisi") ?? "";
                    bool birimYetkisiVar = false;
                    foreach (string y in yetkili.Split(';'))
                    {
                        string[] m = y.Split('-');
                        if (m.Length == 2 && (OrtakFonksiyonlar.ConvertToInt(m[0], 0) > 0 || OrtakFonksiyonlar.ConvertToInt(m[1], 0) > 0))
                            birimYetkisiVar = true;
                    }
                    if (birimYetkisiVar) kriter.yetkiliOlduguMuhasebeBirim = yetkili;
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

        /// <summary>
        /// Eski cmbPersonelTipDegisti: personel tipi kadro işlemi gerektiriyorsa açık (boş) kadrolar listelenir (Genel.AcikKadroDoldur;
        /// eski kodda birim kodu verilir ama sorguda kullanılmaz), gerektirmiyorsa alan gizlenir.
        /// </summary>
        private static IResult AcikKadro(HttpContext baglam, IWebHostEnvironment ortam, int? personelTip, int? birim)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                PersonelTipi personelTipiYeni = TNS.IKY.Arac.PersonelTipBul(personelTip ?? -1);
                if (personelTipiYeni == null && TNS.IKY.Arac.PersonelTipListe.Count > 0)
                    personelTipiYeni = (PersonelTipi)TNS.IKY.Arac.PersonelTipListe[0];
                if (personelTipiYeni == null || !personelTipiYeni.KadroIslemiYapilacakMi())
                    return Results.Json(new { goster = false, kadrolar = new List<object>() });

                TNS.IKY.PersonelKadro pk = new TNS.IKY.PersonelKadro();
                pk.bosKadrolar = true;
                pk.kisiHaric = 0;
                ObjectArray oa = Servis().DoluBosKadroListele(pk);
                List<object> kadrolar = new List<object>();
                if (oa != null && oa.objeler != null)
                    foreach (KadroDoluBos k in oa.objeler)
                        kadrolar.Add(new { k.ID, k.Aciklama });
                KadroDoluBos bos = new KadroDoluBos(); // eski bosEkle: sona boş seçenek
                kadrolar.Add(new { bos.ID, bos.Aciklama });
                return Results.Json(new { goster = true, kadrolar = kadrolar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Açık kadrolar okunamadı."));
            }
        }

        // ------------------------------------------------------------------ belgeler (eski BelgeListesiDoldur / BelgeKaydet / BelgeGoster / BelgeSil)

        private static IResult Belgeler(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            int kod = personelKod ?? 0;
            List<object> liste = new List<object>();
            if (kod <= 0) return Results.Json(new { satirlar = liste });

            PersonelBelgeler belge = new PersonelBelgeler();
            belge.pKod = kod;
            belge.tip = (int)ENUMBELGEFORMTIP.PERSONELKISISEL;
            belge.tipKayitKod = kod;

            ObjectArray belgeler = Servis().PersonelBelgeListele(kullanan, belge);
            if (belgeler != null && belgeler.sonuc != null && !belgeler.sonuc.islemSonuc)
                return Results.Json(new { satirlar = liste, hata = HataMetni(belgeler.sonuc.hataStr, "Belgeler okunamadı.") });
            if (belgeler != null && belgeler.objeler != null)
                foreach (PersonelBelgeler detay in belgeler.objeler)
                {
                    liste.Add(new
                    {
                        KOD = detay.kod,
                        PKOD = detay.pKod,
                        TIPKAYITKOD = detay.tipKayitKod,
                        TIP = detay.tip,
                        BELGEADI = detay.belgeAdi
                    });
                }
            return Results.Json(new { satirlar = liste });
        }

        private static async Task<IResult> BelgeKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!baglam.Request.HasFormContentType) return Results.BadRequest();

            IFormCollection form = await baglam.Request.ReadFormAsync();
            int kod = OrtakFonksiyonlar.ConvertToInt((string)form["personelKod"], 0);
            if (kod == 0) return Basarisiz("Personel bilgisini kayıt ettikten sonra belge ekleyiniz.");
            IFormFile dosya = form.Files["dosya"];
            int belgeBoyutu = dosya == null ? 0 : (int)dosya.Length;
            if (belgeBoyutu <= 0) return Basarisiz("Belge dosyası seçiniz.");

            PersonelBelgeler yeniBelge = new PersonelBelgeler(kod, kod, (int)ENUMBELGEFORMTIP.PERSONELKISISEL, Path.GetFileName(dosya.FileName), belgeBoyutu);
            using (MemoryStream bellek = new MemoryStream())
            {
                await dosya.CopyToAsync(bellek);
                yeniBelge.belge = bellek.ToArray();
            }

            Sonuc sonuc = Servis().PersonelBelgeKaydet(kullanan, yeniBelge);
            if (!sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc.hataStr, "Belge kaydedilemedi."));
            return Results.Json(new { basarili = true, bilgi = string.IsNullOrEmpty(sonuc.bilgiStr) ? "Belge kaydedildi." : PersonelBilgiOrtak.Duz(sonuc.bilgiStr) });
        }

        /// <summary>
        /// İş kuralının belge getirme / silme yordamları yalnızca belge koduna bakar (yetki denetimi yorum satırı); belgenin
        /// sayfadaki personelin kişisel belgesi olduğu iş kuralının kendi listesinden (PersonelBelgeListele) doğrulanır.
        /// </summary>
        private static bool BelgePersoneleAit(Kullanici kullanan, int personelKod, int belgeKod)
        {
            if (personelKod <= 0 || belgeKod <= 0) return false;
            PersonelBelgeler kriter = new PersonelBelgeler();
            kriter.pKod = personelKod;
            kriter.tip = (int)ENUMBELGEFORMTIP.PERSONELKISISEL;
            kriter.tipKayitKod = personelKod;
            ObjectArray belgeler = Servis().PersonelBelgeListele(kullanan, kriter);
            if (belgeler == null || belgeler.objeler == null || (belgeler.sonuc != null && !belgeler.sonuc.islemSonuc)) return false;
            foreach (PersonelBelgeler b in belgeler.objeler)
                if (b.kod == belgeKod) return true;
            return false;
        }

        private static IResult BelgeGoster(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod, int? kod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            if ((kod ?? 0) <= 0) return Hata("Belge seçiniz.");
            try
            {
                if (!BelgePersoneleAit(kullanan, personelKod ?? 0, kod.Value)) return Hata("Belge bulunamadı.");
                Sonuc dosyaKonumuAdiUzantisi = Servis().PersonelBelgeListesiGetir(kullanan, new PersonelBelgeler { kod = kod.Value });
                if (!dosyaKonumuAdiUzantisi.islemSonuc) return Hata(HataMetni(dosyaKonumuAdiUzantisi.hataStr, "Belge okunamadı."));
                string[] parametreler = (dosyaKonumuAdiUzantisi.anahtar ?? "").Split('\n');
                if (parametreler.Length != 3 || !File.Exists(parametreler[0])) return Hata("Belge bulunamadı.");
                baglam.Response.Headers["X-Content-Type-Options"] = "nosniff";
                return DosyaIslem.DosyaGonder(parametreler[0], parametreler[1], true, parametreler[2]);
            }
            catch (Exception)
            {
                return Hata("Belge okunamadı.");
            }
        }

        private static async Task<IResult> BelgeSil(HttpContext baglam, IWebHostEnvironment ortam, PersonelKisiselBelgeSilIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            if (istek.kod == 0) return Basarisiz("Belge seçiniz.");
            if (!BelgePersoneleAit(kullanan, istek.personelKod, istek.kod)) return Basarisiz("Belge bulunamadı.");
            Sonuc sonuc = Servis().PersonelBelgeSil(kullanan, new PersonelBelgeler { kod = istek.kod });
            if (!sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc.hataStr, "Belge silinemedi."));
            return Results.Json(new { basarili = true, bilgi = "Belge silindi." });
        }

        // ------------------------------------------------------------------ yazdırma düğmeleri

        /// <summary>Per. Tanıtma Kartı (eski TanitmaKartiYazdir → PersonelRaporlari2.TanitmaKartiYazdir, PDF).</summary>
        private static IResult TanitmaKarti(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            int kod = personelKod ?? 0;
            if (kod == 0) return Hata("Lütfen kişi seçiniz");
            string sablon = PersonelOrtakRaporlar.SablonBul(ortam, "IKY", "PersonelTanitmaKarti.docx");
            if (sablon == null) return Hata("Rapor şablonu bulunamadı (IKY\\PersonelTanitmaKarti.docx).");
            try
            {
                AsposeLisans.Yukle();
                string sonucDosyaad = TanitmaKartiYazdir(kullanan, kod, sablon, "pdf");
                return DosyaIslem.DosyaGonder(sonucDosyaad, "PersonelTanitmaKarti.pdf", true, "pdf");
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }

        /// <summary>Eski PersonelRaporlari2.TanitmaKartiYazdir (kod aynen; şablon yolu çağıran tarafta bulunur).</summary>
        private static string TanitmaKartiYazdir(Kullanici kullanan, int kod, string dosyaAd, string ciktiTur)
        {
            IIKYServis servisIKY = TNS.IKY.Arac.Tanimla();
            if (kod == 0)
            {
                throw new Exception("Lütfen kişi seçiniz");
            }
            TNS.IKY.Personel personel = servisIKY.HizmetBelgesiRaporu(kullanan, kod);
            string sonucDosyaad = Path.Combine(TNS.OrtakFonksiyonlar.GeciciYolBul(), Path.GetRandomFileName()) + "." + ciktiTur;

            Aspose.Words.Document doc = new Aspose.Words.Document(dosyaAd);
            Aspose.Words.DocumentBuilder docb = new Aspose.Words.DocumentBuilder(doc);

            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "kurumAd", GenelIslemlerIstemci.VarsayilanKurumBulAd());
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "AD", personel.ad + " " + personel.soyad);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "TC", personel.tcKimlikNo);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "Sicil", personel.sicilNo);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "DogumTarihi", personel.dogumTarihi.ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "DogumYeri", personel.nufus.dogumYeri);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "MedeniHali", personel.nufus.medeniHaliAd);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "nufusil", personel.nufus.ilAd + " - " + personel.nufus.ilceAd);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "AnneAdi", personel.nufus.anaAd);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "BabaAdi", personel.nufus.babaAd);

            DataTable tab = new DataTable();
            tab.TableName = "Ogrenimler";
            tab.Columns.Add("ogrenimi");
            tab.Columns.Add("okulu");
            tab.Columns.Add("tarihi");

            if (personel.ogrenim != null && personel.ogrenim.Length > 0)
            {
                foreach (TNS.IKY.PersonelOgrenim a in personel.ogrenim)
                {
                    DataRow row = tab.NewRow();
                    row["ogrenimi"] = a.ogrenimAdi;
                    row["okulu"] = a.okulAdi;
                    row["tarihi"] = a.belgeTarihi == null ? "" : a.belgeTarihi.ToString();
                    tab.Rows.Add(row);
                }
            }
            else
            {
                DataRow row = tab.NewRow();
                row["ogrenimi"] = "";
                row["okulu"] = "";
                row["tarihi"] = "";
                tab.Rows.Add(row);
            }

            string esAd = "", kurslar = "", esMeslek = "", anneMeslek = "", babaMeslek = "";
            if (personel.aile != null)
                foreach (TNS.IKY.PersonelAile a in personel.aile)
                {
                    if (a.yakinlik == ENUMYakinlikDerecesi.Esi)
                    {
                        esAd = a.ad + " " + a.soyad;
                        esMeslek = a.meslek;
                    }
                    else if (a.yakinlik == ENUMYakinlikDerecesi.Annesi)
                        anneMeslek = a.meslek;
                    if (a.yakinlik == ENUMYakinlikDerecesi.Babasi)
                        babaMeslek = a.meslek;
                }

            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "EsAd", esAd);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "EsMeslek", esMeslek);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "BabaMeslek", babaMeslek);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "AnneMeslek", anneMeslek);

            if (personel.kurs != null)
                foreach (TNS.IKY.PersonelKurs k in personel.kurs)
                    kurslar += k.kursAdi + " / " + k.tarih + "\r\n";

            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "kurs", kurslar);

            string diller = "";
            if (personel.dil != null)
                foreach (TNS.IKY.PersonelDil d in personel.dil)
                    diller += d.dilAdi + " / " + d.belgeAdi + " / " + d.duzey + " / " + d.puan + "\r\n";

            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "dil", diller);

            Hizmet hizmetSuresi = new Hizmet();
            if (personel.hizmetSureleri != null)
                foreach (Hizmet h in personel.hizmetSureleri) //1 kişi 1 hizmet süresi bilgisi dönecek
                    hizmetSuresi = h;

            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "THizmetSuresi", hizmetSuresi.hizmetYili.ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "IseBaslama", personel.ilkIseBaslamaTarihi.ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "CIseBaslama", personel.kurumaBaslamaTarihi.ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "CUnvan", hizmetSuresi.guncelHizmet.unvan);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "CStatu", personel.personelTipAd);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "GeldigiKurum", personel.geldigiKurum);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "Birim", hizmetSuresi.guncelHizmet.birim);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "KurumdakiSure", Hizmet.YilDondur(hizmetSuresi.kurumdakiSure).ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "KadrodakiSure", Hizmet.YilDondur(hizmetSuresi.kadrodakiSure).ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "kadroyaGecmeTarihi", hizmetSuresi.guncelHizmet.kadroyaGectigiTarih.ToShortDateString());

            ObjectArray unvanlar = servisIKY.UnvanaGoreHizmetGrupla(personel.kod);
            string isler = "";
            int uCnt = 0;
            string yilAraligi = "";
            foreach (Unvan u in unvanlar.objeler)
            {
                if (isler != "") isler += " ";
                if (u.minYil > 0)
                {
                    if (u.maxYil > 0)
                        yilAraligi = " " + u.minYil + "-" + u.maxYil;
                    else
                        yilAraligi = " " + u.minYil + "-...";
                }
                else
                    yilAraligi = " " + u.hizmetGunu + " GÜN";
                isler += KucukHarf(uCnt) + ") " + u.ad + yilAraligi;
                uCnt++;
            }
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "GecmisHizmetler", isler);

            byte[] resim = servisIKY.PersonelResimGetir(personel.kod, 0, 0, 0);

            if (resim != null)
            {
                AsposeAlanaResimEkle(docb, "resim", resim, Aspose.Words.Drawing.RelativeHorizontalPosition.Default, 0, Aspose.Words.Drawing.RelativeVerticalPosition.Paragraph, 0, Aspose.Words.Drawing.WrapType.None, 150, 150);
            }
            doc.MailMerge.ExecuteWithRegions(tab);

            if (ciktiTur == "pdf")
                doc.Save(sonucDosyaad, Aspose.Words.SaveFormat.Pdf);
            else
                doc.Save(sonucDosyaad);
            return sonucDosyaad;
        }

        static char[] alfabeTurkce = { 'a', 'b', 'c', 'ç', 'd', 'e', 'f', 'g', 'ğ', 'h', 'ı', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'ö', 'p', 'r', 's', 'ş', 't', 'u', 'ü', 'v', 'y', 'z' };
        private static string KucukHarf(int harfNo)
        {
            if (harfNo < alfabeTurkce.Length)
                return alfabeTurkce[harfNo].ToString();

            return harfNo.ToString();
        }

        /// <summary>Eski GenelIslemlerIstemci.AsposeAlanaResimEkle (Core OrtakClass'ta yok): alana en-boy oranı korunarak resim konur.</summary>
        private static void AsposeAlanaResimEkle(Aspose.Words.DocumentBuilder builder, string alanAd, byte[] resim, Aspose.Words.Drawing.RelativeHorizontalPosition horPos, double sol, Aspose.Words.Drawing.RelativeVerticalPosition verPos, double ust, Aspose.Words.Drawing.WrapType bolunme, double w, double h)
        {
            using (MemoryStream ms = new MemoryStream(resim))
            using (System.Drawing.Image image = System.Drawing.Image.FromStream(ms))
            {
                double aspectRatioW = w / image.Width;
                double aspectRatioH = h / image.Height;
                double aspectRatio = Math.Min(aspectRatioW, aspectRatioH);
                if (aspectRatio > 0)
                {
                    w = image.Width * aspectRatio;
                    h = image.Height * aspectRatio;
                }

                while (builder.MoveToMergeField(alanAd))
                {
                    builder.InsertImage(image, horPos, sol, verPos, ust, w, h, bolunme);
                }
            }
        }

        /// <summary>Mail İmza (eski MailImzaYazdir: IKY\MailImza.docx, PDF).</summary>
        private static IResult MailImza(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            int kod = personelKod ?? 0;
            if (kod == 0) return Hata("Lütfen kişi seçiniz");
            string dosyaAd = PersonelOrtakRaporlar.SablonBul(ortam, "IKY", "MailImza.docx");
            if (dosyaAd == null) return Hata("Rapor şablonu bulunamadı (IKY\\MailImza.docx).");
            try
            {
                AsposeLisans.Yukle();
                MailImzaRapor mail = Servis().MailImzaRaporu(kod);
                string sonucDosyaad = Path.Combine(TNS.OrtakFonksiyonlar.GeciciYolBul(), Path.GetRandomFileName()) + ".pdf";

                Aspose.Words.Document doc = new Aspose.Words.Document(dosyaAd);
                Aspose.Words.DocumentBuilder docb = new Aspose.Words.DocumentBuilder(doc);

                GenelIslemlerIstemci.AsposeAlanaYaz(docb, "isim", mail.isim);
                GenelIslemlerIstemci.AsposeAlanaYaz(docb, "unvan", mail.unvan);
                GenelIslemlerIstemci.AsposeAlanaYaz(docb, "birim", mail.birim);
                GenelIslemlerIstemci.AsposeAlanaYaz(docb, "istel", mail.isTel);
                GenelIslemlerIstemci.AsposeAlanaYaz(docb, "ceptel", mail.cepTel);
                GenelIslemlerIstemci.AsposeAlanaYaz(docb, "fax", mail.fax);
                GenelIslemlerIstemci.AsposeAlanaYaz(docb, "adres", ""); //T.C. Cumhurbaşkanlığı Genel Sekreterliği 06689 Çankaya-Ankara

                // Eski: kurum logosu ../KYM/LogoGetir.aspx?tur=2 adresinden indiriliyordu; burada aynı kayıt doğrudan okunur.
                byte[] resim = null;
                try
                {
                    ObjectArray logolar = TNS.KYM.Arac.Tanimla().LogoListele(kullanan, new Logo((int)ENUMLogoTuru.KURUM));
                    if (logolar != null && logolar.sonuc.islemSonuc && logolar.ObjeSayisi > 0)
                        resim = ((Logo)logolar.objeler[0]).data;
                }
                catch (Exception)
                {
                }

                if (resim != null && resim.Length > 0)
                    AsposeAlanaResimEkle(docb, "resim", resim, Aspose.Words.Drawing.RelativeHorizontalPosition.Default, 0, Aspose.Words.Drawing.RelativeVerticalPosition.Paragraph, 0, Aspose.Words.Drawing.WrapType.None, 80, 80);
                else
                    GenelIslemlerIstemci.AsposeAlanaYaz(docb, "resim", "");

                doc.Save(sonucDosyaad, Aspose.Words.SaveFormat.Pdf);
                return DosyaIslem.DosyaGonder(sonucDosyaad, "MailImza.pdf", true, "pdf");
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }

        /// <summary>Nakil Bildirim (eski NakilBildirimiYazdir: IKY\PersonelNakilBildirimi2.xltx, Excel).</summary>
        private static IResult NakilBildirim(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            int kod = personelKod ?? 0;
            if (kod == 0) return Hata("Lütfen kişi seçiniz");
            string sablonAd = PersonelOrtakRaporlar.SablonBul(ortam, "IKY", "PersonelNakilBildirimi2.xltx");
            if (sablonAd == null) return Hata("Rapor şablonu bulunamadı (IKY\\PersonelNakilBildirimi2.xltx).");

            TNS.IKY.Personel nakil;
            PersonelIzinGunleri izin;
            try
            {
                nakil = Servis().NakilBildirimRaporu(kod);
                izin = Servis().PersonelYilBazliYillikIzinBilgileri(kullanan, kod, DateTime.Now);
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Nakil bildirimi bilgileri okunamadı."));
            }
            if (nakil == null) return Hata("Nakil bildirimi bilgileri okunamadı.");
            if (izin == null) izin = new PersonelIzinGunleri();

            try
            {
                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                string sonucDosyaAd = DosyaIslem.DosyaAdUret("", ".xlsx");
                XLS.DosyaAc(sablonAd, sonucDosyaAd);

                TNS.IKY.PersonelOgrenim ogrenim = nakil.ogrenim != null && nakil.ogrenim.Length > 0 ? nakil.ogrenim[0] : new TNS.IKY.PersonelOgrenim();
                TNS.IKY.PersonelAile aile = nakil.aile != null && nakil.aile.Length > 0 ? nakil.aile[0] : new TNS.IKY.PersonelAile();

                string esDurum = "";
                XLS.HucreAdBulYaz("baslik", GenelIslemlerIstemci.VarsayilanKurumBulAd());
                XLS.HucreAdBulYaz("isim", nakil.ad);
                XLS.HucreAdBulYaz("kurumSicili", nakil.sicilNo);
                XLS.HucreAdBulYaz("emekliSicili", nakil.emekliSicilNo);
                XLS.HucreAdBulYaz("TC", nakil.tcKimlikNo);
                XLS.HucreAdBulYaz("eskiUnvan", nakil.kadro.unvanAd);
                XLS.HucreAdBulYaz("eskiGorevYeri", nakil.kadro.birimAd);
                XLS.HucreAdBulYaz("EskiDerece", nakil.kadro.kazanilmisHakDerece + "/" + nakil.kadro.kazanilmisHakKademe + " " + nakil.kadro.emekliyeEsasDerece + "/" + nakil.kadro.emekliyeEsasKademe);
                XLS.HucreAdBulYaz("atamaTarihi", nakil.kadro.atamaTarihi.ToString());
                XLS.HucreAdBulYaz("ogrenim", ogrenim.okulAdi);
                XLS.HucreAdBulYaz("izin", izin.kalan + " Gün izni bulunmaktadır.");
                XLS.HucreAdBulYaz("kidemGun", Hizmet.YilAyGunDondur(nakil.kadro.kidemGunu));
                XLS.HucreAdBulYaz("ayrilisTarihi", nakil.kadro.ayrilmaTarihi.ToString() + " (Mesai Bitimi)");
                XLS.HucreAdBulYaz("saglikIzinSayisi", nakil.kullanilanIzinGun + " Gün");
                if (!string.IsNullOrWhiteSpace(aile.durumAd))
                    esDurum = "Eşi: " + aile.durumAd;
                XLS.HucreAdBulYaz("aile", esDurum + aile.cocukSayisi + " cocuk");

                XLS.SatirSil(8, 8);

                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(sonucDosyaAd, "PersonelNakilBildirimi_" + DateTime.Now.Ticks.ToString(), true, "xlsx");
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }
    }
}
