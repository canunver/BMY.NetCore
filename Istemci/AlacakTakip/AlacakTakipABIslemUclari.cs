using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.ATM;
using TNS.KYM;
using static Istemci.AlacakTakip.AlacakTakipListeABUclari;

namespace Istemci.AlacakTakip
{
    /// <summary>
    /// Borç hareketi (İşlem Girişi penceresi; eski wndOdemeGiris) isteği. Tarihler yyyy-MM-dd.
    ///   degisen (yalnızca hesapla): değişen alan; "tarih" (kur yeniden okunur), "miktarAB" / "miktarUlusal" (iade / düzeltmede yalnızca faiz
    ///   hesaplanır) ya da başka bir alan (eski BHDegisti). islemTur (yalnızca kaydet): 1 Kaydet, 5 Onayla, 9 İptal Et, 6 Yeni Yap.
    /// </summary>
    public class ABOdemeIstegi
    {
        public int iPARDDonemi { get; set; }
        public int dosyaAnahtar { get; set; }
        public int siraNo { get; set; }
        public int ilgiliIslemSiraNo { get; set; }
        public int tur { get; set; }
        public string tarih { get; set; }
        public string karar { get; set; }
        public string borcunIlgiliOlduguTaksit { get; set; }
        public string duzeltimTarihi { get; set; }
        public double dovizKuru { get; set; }
        public double miktarAB { get; set; }
        public double miktarUlusal { get; set; }
        public double faizAB { get; set; }
        public double faizUlusal { get; set; }
        public double yatirilanAB { get; set; }
        public double yatirilanUlusal { get; set; }
        public string aktarimTarihi { get; set; }
        public double aktarimTutari { get; set; }
        public string gerekce { get; set; }
        public bool hatadaKaydet { get; set; }
        public string degisen { get; set; }
        public int islemTur { get; set; }
    }

    public class ABOdemeSilIstegi
    {
        public int iPARDDonemi { get; set; }
        public int dosyaAnahtar { get; set; }
        public int siraNo { get; set; }
    }

    /// <summary>Borç Bilgisi Girişi (eski wndBorcluGiris). islemTur: 1 Kaydet, 5 Onayla, 9 İptal Et, 6 Yeni Yap, 2 Güncelle.</summary>
    public class ABBorcluIstegi
    {
        public int ipardDonemi { get; set; }
        public int dosyaAnahtar { get; set; }
        public int sira { get; set; }
        public string defterSirasi { get; set; }
        public string tedbir { get; set; }
        public string altTedbir { get; set; }
        public string butceKalemi { get; set; }
        public string saymanlikKod { get; set; }
        public string birimKod { get; set; }
        public int borcluKod { get; set; }
        public string basvuruKayitNumarasi { get; set; }
        public string sozlesmeNumarasi { get; set; }
        public string gerekce { get; set; }
        public int islemTur { get; set; }
    }

    /// <summary>Ön Borç Girişi (eski wndOnBorcGiris). borcunKodu: virgülle ayrılmış kodlar.</summary>
    public class ABOnBorcIstegi
    {
        public int ipardDonemi { get; set; }
        public int dosyaAnahtar { get; set; }
        public int sira { get; set; }
        public bool kaldirma { get; set; }
        public string onBorcTarihi { get; set; }
        public string onBorcSayi { get; set; }
        public string vakaNumarasi { get; set; }
        public bool avans { get; set; }
        public int borcTuru { get; set; }
        public string borcunKodu { get; set; }
        public double onBorcDovizKuru { get; set; }
        public double onBorcMiktarAB { get; set; }
        public double onBorcMiktarUlusal { get; set; }
        public string aktarimTarihi { get; set; }
        public string gerekce { get; set; }
        public int islemTur { get; set; }
    }

    /// <summary>Geri Alım Kararı Girişi (eski wndGAKGiris).</summary>
    public class ABGakIstegi
    {
        public int ipardDonemi { get; set; }
        public int dosyaAnahtar { get; set; }
        public int sira { get; set; }
        public bool kaldirma { get; set; }
        public string geriAlimKarariTarihi { get; set; }
        public string geriAlimSayi { get; set; }
        public string bildirimTarihi { get; set; }
        public string tesellumTarihi { get; set; }
        public string odemeSonTarihi { get; set; }
        public string temerrutTarihi { get; set; }
        public string faizinDurdurulmaTarihi { get; set; }
        public double gakDovizKuru { get; set; }
        public double gakMiktarAB { get; set; }
        public double gakMiktarUlusal { get; set; }
        public double silinmeyecekMiktar { get; set; }
        public string gerekce { get; set; }
        public int islemTur { get; set; }
    }

    /// <summary>Süreç Durumu Girişi (eski wndDurumGiris).</summary>
    public class ABDurumIstegi
    {
        public int ipardDonemi { get; set; }
        public int dosyaAnahtar { get; set; }
        public int sira { get; set; }
        public int surecAsamasi { get; set; }
        public string yorumlarTR { get; set; }
        public string gerekce { get; set; }
        public int islemTur { get; set; }
    }

    /// <summary>Taksit Girişi / İptali (eski wndTaksitGiris). tecilFaizNo: 0 Tecil Faizi, 1 Tüzel Tecil Faizi.</summary>
    public class ABTaksitIstegi
    {
        public int ipardDonemi { get; set; }
        public int dosyaAnahtar { get; set; }
        public int sira { get; set; }
        public bool kaldirma { get; set; }
        public string girisTarihi { get; set; }
        public string taksitTalepTarihi { get; set; }
        public string taksitTalepSayi { get; set; }
        public string taksitKararTarihi { get; set; }
        public string taksitKararSayi { get; set; }
        public string taksitIlkTarih { get; set; }
        public int taksitSayisi { get; set; }
        public int tecilFaizNo { get; set; }
        public string gerekce { get; set; }
        public int islemTur { get; set; }
    }

    /// <summary>
    /// AB Takip İşlemleri ekranının işlem pencereleri (eski AlacakTakipListeAB.aspx pencereleri) uçları:
    ///   GET  /api/alacaktakip/ab/odeme/ac?iPARDDonemi=&amp;dosyaAnahtar=&amp;islemTur=&amp;siraNo=&amp;ilgiliSiraNo=   (eski OdemeEkranAc)
    ///   POST /api/alacaktakip/ab/odeme/hesapla    alan değişince tutarlar (eski BHDegisti)
    ///   POST /api/alacaktakip/ab/odeme/kaydet     Kaydet / Onayla / İptal Et / Yeni Yap (eski btnBHKaydet_Click)
    ///   POST /api/alacaktakip/ab/odeme/sil        borç hareketi siler (eski OdemeSil)
    ///   GET  /api/alacaktakip/ab/borclu/ac?iPARDDonemi=&amp;dosyaAnahtar=&amp;sira=,  POST borclu/kaydet   (eski BorcluAc / btnBorcluIslem_Click)
    ///   GET  /api/alacaktakip/ab/onborc/ac,  POST onborc/kaydet,  GET onborc/kodlar?borcTuru=   (eski OnBorcAc / btnOnBorcIslem_Click / cmbBorcTuru_Change)
    ///   GET  /api/alacaktakip/ab/gak/ac,  POST gak/kaydet,  GET gak/tarihler?tesellumTarihi=   (eski GeriAlimKarariAc / btnGAKIslem_Click / TesellumTarihi_Changed)
    ///   GET  /api/alacaktakip/ab/durum/ac,  POST durum/kaydet   (eski SurecDurumAc / btnDurumIslem_Click)
    ///   GET  /api/alacaktakip/ab/taksit/ac,  POST taksit/hesapla,  POST taksit/kaydet   (eski TaksitDurumAc / Taksitlendir / btnTaksitIslem_Click)
    /// </summary>
    public static class AlacakTakipABIslemUclari
    {
        public static void Ekle(WebApplication app)
        {
            const string u = AlacakTakipListeABUclari.U;
            app.MapGet(u + "odeme/ac", OdemeAc);
            app.MapPost(u + "odeme/hesapla", OdemeHesapla);
            app.MapPost(u + "odeme/kaydet", OdemeKaydet);
            app.MapPost(u + "odeme/sil", OdemeSil);
            app.MapGet(u + "borclu/ac", BorcluAc);
            app.MapPost(u + "borclu/kaydet", BorcluKaydet);
            app.MapGet(u + "onborc/ac", OnBorcAc);
            app.MapPost(u + "onborc/kaydet", OnBorcKaydet);
            app.MapGet(u + "onborc/kodlar", OnBorcKodlar);
            app.MapGet(u + "gak/ac", GakAc);
            app.MapPost(u + "gak/kaydet", GakKaydet);
            app.MapGet(u + "gak/tarihler", GakTarihler);
            app.MapGet(u + "durum/ac", DurumAc);
            app.MapPost(u + "durum/kaydet", DurumKaydet);
            app.MapGet(u + "taksit/ac", TaksitAc);
            app.MapPost(u + "taksit/hesapla", TaksitHesapla);
            app.MapPost(u + "taksit/kaydet", TaksitKaydet);
        }

        private const string DosyaYok = "Lütfen önce dosya kaydını yapınız!";

        private static bool IslemTurGecerli(int islemTur, bool guncelleVar)
        {
            return islemTur == (int)ENUMDosyaIslemTur.Kaydet || islemTur == (int)ENUMDosyaIslemTur.Onayla ||
                   islemTur == (int)ENUMDosyaIslemTur.IptalEt || islemTur == (int)ENUMDosyaIslemTur.YeniYap ||
                   (guncelleVar && islemTur == (int)ENUMDosyaIslemTur.OzelGuncelle);
        }

        private static IResult IslemSonucu(Sonuc s, int sira)
        {
            if (s != null && s.islemSonuc)
                return Results.Json(new { basarili = true, bilgi = "İşlem Başarılı", sira = OrtakFonksiyonlar.ConvertToInt(s.anahtar, sira) });
            string h = HataMetni(s?.hataStr, "");
            return Basarisiz(h == "" ? "İşlem Başarısız." : "İşlem Başarısız: " + h);
        }

        /// <summary>Pencerelerin ortak alt bölümü ve tuşları (eski Ekrana*Yaz: tusGizle, YeniYapilirMi).</summary>
        private static Dictionary<string, object> ParcaOrtak(DosyaABParca p)
        {
            return new Dictionary<string, object>
            {
                ["ipardDonemi"] = p.ipardDonemi,
                ["dosyaAnahtar"] = p.dosyaAnahtar,
                ["sira"] = p.sira,
                ["kayitDurum"] = (int)p.kayitDurum,
                ["yeni"] = p.kayitDurum == DosyaABParca.ENUMKayitDurum.YENI,
                ["yeniYapilir"] = G(() => p.YeniYapilirMi(), false),
                ["gerekce"] = p.gerekce ?? "",
                ["kaydedenAdSoyad"] = G(() => p.kaydedenAdSoyad, "").Trim(),
                ["kayitTarihi"] = TarihYaz(p.kayitTarihi),
                ["onaylayanAdSoyad"] = G(() => p.onaylayanAdSoyad, "").Trim(),
                ["onayTarihi"] = TarihYaz(p.onayTarihi)
            };
        }

        // ================================================================ borç hareketi (İşlem Girişi)

        private static Dictionary<string, object> OdemeJson(BorcHareket bh, DosyaAB dab)
        {
            BorcHareket.ENUMHareketTur tur = bh.tur;
            bool odemeTuru = G(() => bh.OdemeTuruIslem, BorcHareket.OdemeTuruIslemMi(tur));
            bool silme = tur == BorcHareket.ENUMHareketTur.Silme;
            bool terkin = tur == BorcHareket.ENUMHareketTur.Terkin;
            Dictionary<string, object> h = new Dictionary<string, object>
            {
                ["siraNo"] = bh.siraNo,
                ["ilgiliIslemSiraNo"] = bh.ilgiliIslemSiraNo,
                ["tur"] = (int)tur,
                ["tarih"] = TarihYaz(bh.tarih),
                ["karar"] = bh.karar ?? "",
                ["borcunIlgiliOlduguTaksit"] = bh.borcunIlgiliOlduguTaksit ?? "",
                ["duzeltimTarihi"] = TarihYaz(bh.duzeltimTarihi),
                ["aktarimTarihi"] = TarihYaz(bh.aktarimTarihi),
                ["dovizKuru"] = N(G(() => bh.dovizKuruIslem, bh.dovizKuru)),
                ["miktarAB"] = N(bh.miktarAB),
                ["miktarUlusal"] = N(bh.miktarUlusal),
                ["miktarToplam"] = N(G(() => bh.miktarToplam, 0d)),
                ["faizAB"] = N(bh.faizAB),
                ["faizUlusal"] = N(bh.faizUlusal),
                ["faizToplam"] = N(G(() => bh.faizToplam, 0d)),
                ["toplamAB"] = N(G(() => bh.toplamAB, 0d)),
                ["toplamUlusal"] = N(G(() => bh.toplamUlusal, 0d)),
                ["toplamToplam"] = N(G(() => bh.toplamToplam, 0d)),
                ["miktarABEUR"] = N(G(() => bh.miktarABEUR, 0d)),
                ["miktarUlusalEUR"] = N(G(() => bh.miktarUlusalEUR, 0d)),
                ["miktarToplamEUR"] = N(G(() => bh.miktarToplamEUR, 0d)),
                ["faizABEUR"] = N(G(() => bh.faizABEUR, 0d)),
                ["faizUlusalEUR"] = N(G(() => bh.faizUlusalEUR, 0d)),
                ["faizToplamEUR"] = N(G(() => bh.faizToplamEUR, 0d)),
                ["toplamABEUR"] = N(G(() => bh.toplamABEUR, 0d)),
                ["toplamUlusalEUR"] = N(G(() => bh.toplamUlusalEUR, 0d)),
                ["toplamToplamEUR"] = N(G(() => bh.toplamToplamEUR, 0d)),
                ["yatirilanAB"] = N(bh.yatirilanAB),
                ["yatirilanUlusal"] = N(bh.yatirilanUlusal),
                ["yatirilanToplam"] = N(G(() => bh.yatirilanToplam, 0d)),
                ["yatirilanABEUR"] = N(G(() => bh.yatirilanABEUR, 0d)),
                ["yatirilanUlusalEUR"] = N(G(() => bh.yatirilanUlusalEUR, 0d)),
                ["yatirilanToplamEUR"] = N(G(() => bh.yatirilanToplamEUR, 0d)),
                ["durum"] = G(() => bh.DurumKodAd, ""),
                ["aktarAB"] = N(G(() => bh.aktarAB, 0d)),
                ["aktarUlusal"] = N(G(() => bh.aktarUlusal, 0d)),
                ["aktarKisiye"] = N(G(() => bh.aktarKisiye, 0d)),
                ["aktarimTutari"] = N(bh.aktarimTutari),
                ["aktarimTutariEUR"] = N(G(() => bh.aktarimTutariEUR, 0d)),
                ["gerekce"] = bh.gerekce ?? "",
                ["kaydedenAdSoyad"] = G(() => bh.kaydedenAdSoyad, "").Trim(),
                ["kayitTarihi"] = TarihYaz(bh.kayitTarihi),
                ["onaylayanAdSoyad"] = G(() => bh.onaylayanAdSoyad, "").Trim(),
                ["onayTarihi"] = TarihYaz(bh.onayTarihi)
            };
            Dictionary<string, object> kalan = new Dictionary<string, object>
            {
                ["kalanMiktarAB"] = N(G(() => dab.kalanMiktarAB, 0d)),
                ["kalanMiktarUlusal"] = N(G(() => dab.kalanMiktarUlusal, 0d)),
                ["kalanMiktarToplam"] = N(G(() => dab.kalanMiktarToplam, 0d)),
                ["kalanFaizMiktarAB"] = N(G(() => dab.kalanFaizMiktarAB, 0d)),
                ["kalanFaizMiktarUlusal"] = N(G(() => dab.kalanFaizMiktarUlusal, 0d)),
                ["kalanFaizMiktarToplam"] = N(G(() => dab.kalanFaizMiktarToplam, 0d)),
                ["kalanToplamAB"] = N(G(() => dab.kalanToplamAB, 0d)),
                ["kalanToplamUlusal"] = N(G(() => dab.kalanToplamUlusal, 0d)),
                ["kalanToplamToplam"] = N(G(() => dab.kalanToplamToplam, 0d)),
                ["kalanMiktarABEUR"] = N(G(() => dab.kalanMiktarABEUR, 0d)),
                ["kalanMiktarUlusalEUR"] = N(G(() => dab.kalanMiktarUlusalEUR, 0d)),
                ["kalanMiktarToplamEUR"] = N(G(() => dab.kalanMiktarToplamEUR, 0d)),
                ["kalanFaizMiktarABEUR"] = N(G(() => dab.kalanFaizMiktarABEUR, 0d)),
                ["kalanFaizMiktarUlusalEUR"] = N(G(() => dab.kalanFaizMiktarUlusalEUR, 0d)),
                ["kalanFaizMiktarToplamEUR"] = N(G(() => dab.kalanFaizMiktarToplamEUR, 0d)),
                ["kalanToplamABEUR"] = N(G(() => dab.kalanToplamABEUR, 0d)),
                ["kalanToplamUlusalEUR"] = N(G(() => dab.kalanToplamUlusalEUR, 0d)),
                ["kalanToplamToplamEUR"] = N(G(() => dab.kalanToplamToplamEUR, 0d))
            };
            // Pencere görünümü (eski OdemeEkranAc: frmOdeme, cfAktarim, etiketler, Ulusal alanlarının salt okunurluğu)
            Dictionary<string, object> ekran = new Dictionary<string, object>
            {
                ["baslik"] = "İşlem Girişi (" + G(() => BorcHareket.TurAdBul(tur), "") + ")",
                ["odemeTuru"] = odemeTuru,
                ["aktarimGoster"] = silme || tur == BorcHareket.ENUMHareketTur.FazlaOdemeAktarim || tur == BorcHareket.ENUMHareketTur.OdeneniAktarma,
                ["ulusalSaltOkunur"] = silme || tur == BorcHareket.ENUMHareketTur.OdeneniAktarma,
                ["islemEtiket"] = silme && !odemeTuru ? "Silinen Tutar (TL)" : "Geri Alım Miktarı (TL)",
                ["islemBolumEtiket"] = terkin ? "Terkin Tarih/Karar/Taksit" : "İşlem Tarih/Karar/Taksit",
                ["duzeltmeEtiket"] = silme ? "Silme Tarihi" : (terkin ? "Yazı Tarihi" : "Düzeltme Tarihi")
            };
            return new Dictionary<string, object>
            {
                ["iPARDDonemi"] = dab.iPARDDonemi,
                ["dosyaAnahtar"] = dab.dosyaAnahtar,
                ["hareket"] = h,
                ["kalan"] = kalan,
                ["ekran"] = ekran
            };
        }

        /// <summary>Eski EkrandanOkuBH: pencere değerleri; kalanlar işlemden önceki duruma göre hesaplanır.</summary>
        private static BorcHareket EkrandanOkuBH(DosyaAB formAna, ABOdemeIstegi i)
        {
            BorcHareket bh = new BorcHareket(formAna);
            bh.siraNo = i.siraNo;
            bh.ilgiliIslemSiraNo = i.ilgiliIslemSiraNo;
            BorcHareket bhAna = formAna.HareketBul(bh.siraNo);
            if (bhAna != null) bh.yerNo = bhAna.yerNo;
            bh.tarihDT = TarihOku(i.tarih);
            bh.tur = (BorcHareket.ENUMHareketTur)i.tur;
            bh.karar = i.karar ?? "";
            bh.miktarAB = i.miktarAB;
            bh.miktarUlusal = i.miktarUlusal;
            bh.faizAB = i.faizAB;
            bh.faizUlusal = i.faizUlusal;
            bh.yatirilanAB = i.yatirilanAB;
            bh.yatirilanUlusal = i.yatirilanUlusal;
            bh.duzeltimTarihi = new TNSDateTime(TarihOku(i.duzeltimTarihi));
            bh.borcunIlgiliOlduguTaksit = i.borcunIlgiliOlduguTaksit ?? "";
            formAna.IslemToplamBul(bh.tarih, bh.yerNo - 1, true, null, new TNSDateTime(DateTime.Now), bh.ilgiliIslemSiraNo);
            bh.dovizKuru = i.dovizKuru;
            bh.aktarimTarihi = new TNSDateTime(TarihOku(i.aktarimTarihi));
            bh.aktarimTutari = i.aktarimTutari;
            bh.gerekce = i.gerekce ?? "";
            return bh;
        }

        private static IResult OdemeAc(HttpContext baglam, IWebHostEnvironment ortam, int? iPARDDonemi, int? dosyaAnahtar, int? islemTur, int? siraNo, int? ilgiliSiraNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                IATMServis servis = Servis();
                DosyaAB form = DosyaOku(kullanan, iPARDDonemi ?? 0, dosyaAnahtar ?? 0, out string hata);
                if (form == null) return Hata(hata);
                if (form.iPARDDonemi <= 0 || form.dosyaAnahtar <= 0) return Hata(DosyaYok);
                int sira = siraNo ?? 0;
                BorcHareket bh = new BorcHareket(form);
                if (sira > 0)
                {
                    bh.siraNo = sira;
                    ObjectArray oa = servis.ListeleDosyaABBorcHareket(kullanan, form.iPARDDonemi, form.dosyaAnahtar, bh);
                    if (oa == null || !oa.sonuc.islemSonuc) return Hata(HataMetni(oa?.sonuc?.hataStr, "Kayıt okunamadı."));
                    if (oa.ObjeSayisi == 0) return Hata("Aranan kayıt bulunamadı!");
                    bh = (BorcHareket)oa.objeler[0];
                    bh.anaDosya = form;
                    BorcHareket bhAna = form.HareketBul(sira);
                    if (bhAna != null) bh.yerNo = bhAna.yerNo;
                }
                else
                {
                    int tur = islemTur ?? 0;
                    if (!Enum.IsDefined(typeof(BorcHareket.ENUMHareketTur), tur)) return Hata("İşlem türü seçiniz.");
                    bh.tur = (BorcHareket.ENUMHareketTur)tur;
                    bh.tarihDT = DateTime.Now.Date;
                    bh.dovizKuru = servis.DovizKuruBulOncAy(kullanan, bh.tarih);
                    bh.ilgiliIslemSiraNo = ilgiliSiraNo ?? 0;
                }

                if (bh.tur == BorcHareket.ENUMHareketTur.OdeneniAktarma || bh.tur == BorcHareket.ENUMHareketTur.Iade)
                {
                    if (bh.ilgiliIslemSiraNo > 0)
                    {
                        BorcHareket bhIlgili = form.HareketBul(bh.ilgiliIslemSiraNo);
                        if (bhIlgili == null) return Hata("İlgili ödeme kaydı bulunamadı!");
                        if (bhIlgili.tur == BorcHareket.ENUMHareketTur.Silme || bhIlgili.tur == BorcHareket.ENUMHareketTur.Duzeltme ||
                            bhIlgili.tur == BorcHareket.ENUMHareketTur.Terkin || bhIlgili.tur == BorcHareket.ENUMHareketTur.FaizHesabi ||
                            bhIlgili.tur == BorcHareket.ENUMHareketTur.FaizHesabiIslem)
                        {
                            if (sira == 0) return Hata("Silme, Faiz Hesabı, Terkin işlemlerinin aktarması veya iadesi yapılamaz!");
                        }
                        bh.dovizKuru = bhIlgili.dovizKuru;
                        if (sira == 0 && bh.tur == BorcHareket.ENUMHareketTur.OdeneniAktarma)
                            bh.aktarimTutari = bhIlgili.miktarAB + bhIlgili.faizAB;
                    }
                    else if (sira == 0)
                        return Hata("Yatırılan ödemelerin aktarılmasında veya iadesinde, ödemenin bilgisi de girilmelidir!");
                }
                form.IslemToplamBul(bh.tarih, bh.yerNo - 1, false, null, bh.onayTarihi, bh.ilgiliIslemSiraNo);
                return Results.Json(OdemeJson(bh, form));
            }
            catch (Exception)
            {
                return Hata("İşlem penceresi açılamadı.");
            }
        }

        private static async Task<IResult> OdemeHesapla(HttpContext baglam, IWebHostEnvironment ortam, ABOdemeIstegi i, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (i == null) return Results.BadRequest();
            try
            {
                DosyaAB dab = DosyaOku(kullanan, i.iPARDDonemi, i.dosyaAnahtar, out string hata);
                if (dab == null) return Hata(hata);
                BorcHareket hareketSatir = EkrandanOkuBH(dab, i);
                BorcHareket.ENUMHareketTur bhTur = (BorcHareket.ENUMHareketTur)i.tur;
                string degisen = Temiz(i.degisen);
                bool sadeceFaizHesapla = BorcHareket.IadeDuzeltmeTuruIslemMi(bhTur) && (degisen == "miktarAB" || degisen == "miktarUlusal");
                if (degisen == "tarih" && !(hareketSatir.tur == BorcHareket.ENUMHareketTur.OdeneniAktarma || hareketSatir.tur == BorcHareket.ENUMHareketTur.Iade))
                    hareketSatir.dovizKuru = Servis().DovizKuruBulOncAy(kullanan, hareketSatir.tarih);

                if (sadeceFaizHesapla || !BorcHareket.OdemeTuruIslemMi(bhTur))
                    hareketSatir.OdenendenFaizHesapla(dab.tecilFaizNo, null);
                else
                    hareketSatir.OdenendenMiktarHesapla(i.hatadaKaydet);
                return Results.Json(OdemeJson(hareketSatir, dab));
            }
            catch (Exception)
            {
                return Hata("Tutarlar hesaplanamadı.");
            }
        }

        private static async Task<IResult> OdemeKaydet(HttpContext baglam, IWebHostEnvironment ortam, ABOdemeIstegi i, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (i == null) return Results.BadRequest();
            if (!IslemTurGecerli(i.islemTur, false)) return Basarisiz("İşlem türü seçiniz.");
            try
            {
                DosyaAB dosyaAB = DosyaOku(kullanan, i.iPARDDonemi, i.dosyaAnahtar, out string hata);
                if (dosyaAB == null) return Basarisiz(hata);
                if (dosyaAB.iPARDDonemi <= 0 || dosyaAB.dosyaAnahtar <= 0) return Basarisiz(DosyaYok);
                BorcHareket borcHareket = EkrandanOkuBH(dosyaAB, i);
                BorcHareket.ENUMHareketTur bhTur = (BorcHareket.ENUMHareketTur)i.tur;

                Sonuc s = new Sonuc();
                if (!borcHareket.IadeDuzeltmeTuruIslem)
                {
                    if (borcHareket.OdemeTuruIslem)
                        borcHareket.OdenendenMiktarHesapla(i.hatadaKaydet);
                    else
                    {
                        if (borcHareket.tur != BorcHareket.ENUMHareketTur.Duzeltme)
                        {
                            borcHareket.miktarAB = dosyaAB.kalanMiktarAB;
                            borcHareket.miktarUlusal = dosyaAB.kalanMiktarUlusal;
                            borcHareket.OdenendenFaizHesapla(dosyaAB.tecilFaizNo, null);
                        }
                        if (borcHareket.FaizIslemi)
                        {
                            borcHareket.miktarAB = 0;
                            borcHareket.miktarUlusal = 0;
                        }
                    }
                    if (borcHareket.OdemeTuruIslem && borcHareket.HataliOdeme)
                    {
                        if (bhTur != BorcHareket.ENUMHareketTur.Terkin && bhTur != BorcHareket.ENUMHareketTur.Iade)
                        {
                            if (i.hatadaKaydet)
                            {
                                if (borcHareket.HataliOdemeLimitli)
                                    s.DegerAta("Aktarılması gereken tutarlar 20 liranın üstünde olduğundan kayıt işlemi yapamazsınız!");
                            }
                            else
                                s.DegerAta("Yatırılan tutarlar uyumsuz olduğundan kayıt işlemi yapamazsınız!");
                        }
                    }
                }

                if (s.islemSonuc)
                    s = Servis().KaydetDosyaABBorcHareket(kullanan, dosyaAB.iPARDDonemi, dosyaAB.dosyaAnahtar, borcHareket, (ENUMDosyaIslemTur)i.islemTur);
                if (s != null && s.islemSonuc)
                    return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi gerçekleştirildi!", siraNo = OrtakFonksiyonlar.ConvertToInt(s.anahtar, i.siraNo) });
                string h = HataMetni(s?.hataStr, "");
                return Basarisiz(h == "" ? "Kayıt işlemi gerçekleştirilemedi." : "Kayıt işlemi gerçekleştirilemedi: " + h);
            }
            catch (Exception)
            {
                return Basarisiz("Kayıt işlemi gerçekleştirilemedi.");
            }
        }

        private static async Task<IResult> OdemeSil(HttpContext baglam, IWebHostEnvironment ortam, ABOdemeSilIstegi i, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (i == null) return Results.BadRequest();
            if (i.iPARDDonemi <= 0 || i.dosyaAnahtar <= 0 || i.siraNo <= 0) return Basarisiz(DosyaYok);
            try
            {
                Sonuc sonuc = Servis().SilDosyaABBorcHareket(kullanan, i.iPARDDonemi, i.dosyaAnahtar, i.siraNo);
                if (sonuc == null || !sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc?.hataStr, "Silme işlemi yapılamadı."));
                return Results.Json(new { basarili = true, bilgi = "Silme işlemi yapıldı." });
            }
            catch (Exception)
            {
                return Basarisiz("Silme işlemi yapılamadı.");
            }
        }

        // ================================================================ borçlu

        private static IResult BorcluAc(HttpContext baglam, IWebHostEnvironment ortam, int? iPARDDonemi, int? dosyaAnahtar, int? sira)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                // eski IslemEkranAc(BORCLU): dosya okunmaz, yalnızca anahtarlar kullanılır
                ABBorclu f = new ABBorclu();
                f.ipardDonemi = iPARDDonemi ?? 0;
                f.dosyaAnahtar = dosyaAnahtar ?? 0;
                f.sira = sira ?? 0;
                if (f.sira > 0)
                {
                    ObjectVAN ov = Servis().OkuBorcluForm(kullanan, f);
                    if (ov != null && ov.sonuc.islemSonuc) f = (ABBorclu)ov.o;
                }
                else
                {
                    f.sira = 0;
                    f.kayitDurum = DosyaABParca.ENUMKayitDurum.YENI;
                }
                Dictionary<string, object> s = ParcaOrtak(f);
                s["mevcutDosya"] = f.dosyaAnahtar > 0;
                s["defterSirasi"] = f.defterSirasi ?? "";
                s["tedbir"] = f.tedbir ?? "";
                s["altTedbir"] = f.altTedbir ?? "";
                s["butceKalemi"] = f.butceKalemi ?? "";
                s["saymanlikKod"] = f.saymanlikKod > 0 ? f.saymanlikKod.ToString() : "";
                s["birimKod"] = f.birimKod ?? "";
                s["birimAd"] = f.birimAd ?? "";
                s["borcluKod"] = f.borcluKod;
                s["borcluKodAd"] = string.IsNullOrEmpty(f.kimlikNo) && string.IsNullOrEmpty(f.borcluAd) ? "" : G(() => f.borcluKodAd, "");
                s["basvuruKayitNumarasi"] = f.basvuruKayitNumarasi ?? "";
                s["sozlesmeNumarasi"] = f.sozlesmeNumarasi ?? "";
                return Results.Json(s);
            }
            catch (Exception)
            {
                return Hata("Borçlu bilgileri okunamadı.");
            }
        }

        private static async Task<IResult> BorcluKaydet(HttpContext baglam, IWebHostEnvironment ortam, ABBorcluIstegi i, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (i == null) return Results.BadRequest();
            if (!IslemTurGecerli(i.islemTur, true)) return Basarisiz("İşlem türü seçiniz.");
            try
            {
                ABBorclu form = new ABBorclu();
                form.ipardDonemi = i.ipardDonemi;
                form.dosyaAnahtar = i.dosyaAnahtar;
                form.sira = i.sira;
                form.defterSirasi = i.defterSirasi ?? "";
                form.tedbir = i.tedbir ?? "";
                form.altTedbir = i.altTedbir ?? "";
                form.butceKalemi = i.butceKalemi ?? "";
                form.saymanlikKod = OrtakFonksiyonlar.ConvertToInt(Temiz(i.saymanlikKod), 0);
                form.birimKod = Temiz(i.birimKod);
                form.borcluKod = i.borcluKod;
                form.basvuruKayitNumarasi = i.basvuruKayitNumarasi ?? "";
                form.sozlesmeNumarasi = i.sozlesmeNumarasi ?? "";
                form.gerekce = i.gerekce ?? "";
                Sonuc s = Servis().KaydetBorcluForm(kullanan, form, (ENUMDosyaIslemTur)i.islemTur);
                if (s != null && s.islemSonuc)
                {
                    // anahtar: "iPARDDonemi-dosyaAnahtar-sira"
                    string[] p = (s.anahtar ?? "").Split('-');
                    return Results.Json(new
                    {
                        basarili = true,
                        bilgi = "İşlem Başarılı",
                        iPARDDonemi = OrtakFonksiyonlar.ConvertToInt(p.Length > 0 ? p[0] : "", form.ipardDonemi),
                        dosyaAnahtar = OrtakFonksiyonlar.ConvertToInt(p.Length > 1 ? p[1] : "", form.dosyaAnahtar),
                        sira = OrtakFonksiyonlar.ConvertToInt(p.Length > 2 ? p[2] : "", form.sira)
                    });
                }
                return IslemSonucu(s, i.sira);
            }
            catch (Exception)
            {
                return Basarisiz("İşlem Başarısız.");
            }
        }

        // ================================================================ ön borç

        private static List<object> BorcKodlari(int borcTuru)
        {
            TNSCollection liste = new TNSCollection();
            BorcTurleriSinif.BorcKoduDoldur(borcTuru, liste);
            List<object> l = new List<object>();
            foreach (TNS.BKD.ABSKod k in liste) l.Add(new { kod = Convert.ToString(k.kod), ad = k.kodAd ?? "" });
            return l;
        }

        private static IResult OnBorcKodlar(HttpContext baglam, IWebHostEnvironment ortam, int? borcTuru)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                return Results.Json(new { kodlar = BorcKodlari(borcTuru ?? 0) });
            }
            catch (Exception)
            {
                return Hata("Borç kodları okunamadı.");
            }
        }

        private static IResult OnBorcAc(HttpContext baglam, IWebHostEnvironment ortam, int? iPARDDonemi, int? dosyaAnahtar, int? sira)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                DosyaAB form = DosyaOku(kullanan, iPARDDonemi ?? 0, dosyaAnahtar ?? 0, out string hata);
                if (form == null) return Hata(hata);
                OnBorc ob = new OnBorc();
                ob.ipardDonemi = form.iPARDDonemi;
                ob.dosyaAnahtar = form.dosyaAnahtar;
                ob.sira = sira ?? 0;
                if (ob.sira > 0)
                {
                    ObjectVAN ov = Servis().OkuOnBorcForm(kullanan, ob);
                    if (ov != null && ov.sonuc.islemSonuc) ob = (OnBorc)ov.o;
                }
                else
                {
                    ob.kayitDurum = DosyaABParca.ENUMKayitDurum.YENI;
                    ob.sira = 0;
                    ob.onBorcTarihi.Yaz(Dt(form.onBorcTarihi));
                    ob.onBorcSayi = form.onBorcSayi;
                    ob.vakaNumarasi = form.vakaNumarasi;
                    ob.avans = form.avans;
                    ob.borcTuru = form.borcTuru;
                    ob.borcunKodu = form.borcunKodu;
                    ob.onBorcDovizKuru = form.onBorcDovizKuru;
                    ob.onBorcMiktarAB = form.onBorcMiktarAB;
                    ob.onBorcMiktarUlusal = form.onBorcMiktarUlusal;
                    ob.aktarimTarihi.Yaz(Dt(form.ipardAktarimTar));
                    ob.OBKaldirma = false;
                }
                Dictionary<string, object> s = ParcaOrtak(ob);
                s["kaldirma"] = ob.OBKaldirma;
                s["onBorcTarihi"] = TarihYaz(ob.onBorcTarihi);
                s["onBorcSayi"] = ob.onBorcSayi ?? "";
                s["vakaNumarasi"] = ob.vakaNumarasi ?? "";
                s["avans"] = ob.avans;
                s["borcTuru"] = (int)ob.borcTuru;
                s["borcunKodu"] = ob.borcunKodu ?? "";
                s["kodlar"] = BorcKodlari((int)ob.borcTuru);
                s["onBorcDovizKuru"] = N(ob.onBorcDovizKuru);
                s["onBorcMiktarAB"] = N(ob.onBorcMiktarAB);
                s["onBorcMiktarUlusal"] = N(ob.onBorcMiktarUlusal);
                s["onBorcToplam"] = N(G(() => ob.onBorcToplam, 0d));
                s["onBorcMiktarABEUR"] = N(G(() => ob.onBorcMiktarABEUR, 0d));
                s["onBorcMiktarUlusalEUR"] = N(G(() => ob.onBorcMiktarUlusalEUR, 0d));
                s["onBorcToplamEUR"] = N(G(() => ob.onBorcToplamEUR, 0d));
                s["aktarimTarihi"] = TarihYaz(ob.aktarimTarihi);
                return Results.Json(s);
            }
            catch (Exception)
            {
                return Hata("Ön borç bilgileri okunamadı.");
            }
        }

        private static async Task<IResult> OnBorcKaydet(HttpContext baglam, IWebHostEnvironment ortam, ABOnBorcIstegi i, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (i == null) return Results.BadRequest();
            if (!IslemTurGecerli(i.islemTur, true)) return Basarisiz("İşlem türü seçiniz.");
            try
            {
                OnBorc form = new OnBorc();
                form.OBKaldirma = i.kaldirma;
                form.onBorcTarihi = TnsTarih(i.onBorcTarihi);
                form.onBorcSayi = i.onBorcSayi ?? "";
                form.vakaNumarasi = i.vakaNumarasi ?? "";
                form.avans = i.avans;
                form.borcTuru = (DosyaAB.BorcTurleri)i.borcTuru;
                form.borcunKodu = Temiz(i.borcunKodu);
                form.onBorcDovizKuru = i.onBorcDovizKuru;
                form.onBorcMiktarAB = i.onBorcMiktarAB;
                form.onBorcMiktarUlusal = i.onBorcMiktarUlusal;
                form.aktarimTarihi = TnsTarih(i.aktarimTarihi);
                form.gerekce = i.gerekce ?? "";
                form.ipardDonemi = i.ipardDonemi;
                form.dosyaAnahtar = i.dosyaAnahtar;
                form.sira = i.sira;
                return IslemSonucu(Servis().KaydetOnBorcForm(kullanan, form, (ENUMDosyaIslemTur)i.islemTur), i.sira);
            }
            catch (Exception)
            {
                return Basarisiz("İşlem Başarısız.");
            }
        }

        // ================================================================ geri alım kararı

        private static IResult GakAc(HttpContext baglam, IWebHostEnvironment ortam, int? iPARDDonemi, int? dosyaAnahtar, int? sira)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                DosyaAB form = DosyaOku(kullanan, iPARDDonemi ?? 0, dosyaAnahtar ?? 0, out string hata);
                if (form == null) return Hata(hata);
                GeriAlimKarari g = new GeriAlimKarari();
                g.ipardDonemi = form.iPARDDonemi;
                g.dosyaAnahtar = form.dosyaAnahtar;
                g.sira = sira ?? 0;
                if (g.sira > 0)
                {
                    ObjectVAN ov = Servis().OkuGAKForm(kullanan, g);
                    if (ov != null && ov.sonuc.islemSonuc) g = (GeriAlimKarari)ov.o;
                }
                else
                {
                    g.kayitDurum = DosyaABParca.ENUMKayitDurum.YENI;
                    g.sira = 0;
                    g.geriAlimKarariTarihi.Yaz(G(() => form.geriAlimKarariTarihiDT, DateTime.MinValue));
                    g.geriAlimSayi = form.geriAlimSayi;
                    g.bildirimTarihi.Yaz(Dt(form.bildirimTarihi));
                    g.tesellumTarihi.Yaz(Dt(form.tesellumTarihi));
                    g.odemeSonTarihi.Yaz(Dt(form.odemeSonTarihi));
                    g.temerrutTarihi.Yaz(Dt(form.temerrutTarihi));
                    g.faizinDurdurulmaTarihi.Yaz(Dt(form.faizinDurdurulmaTarihi));
                    g.gakDovizKuru = form.geriAlimDovizKuru;
                    g.gakMiktarAB = form.geriAlimMiktarAB;
                    g.gakMiktarUlusal = form.geriAlimMiktarUlusal;
                    g.gakKaldirma = false;
                }
                Dictionary<string, object> s = ParcaOrtak(g);
                s["kaldirma"] = g.gakKaldirma;
                s["geriAlimKarariTarihi"] = TarihYaz(g.geriAlimKarariTarihi);
                s["geriAlimSayi"] = g.geriAlimSayi ?? "";
                s["bildirimTarihi"] = TarihYaz(g.bildirimTarihi);
                s["tesellumTarihi"] = TarihYaz(g.tesellumTarihi);
                s["odemeSonTarihi"] = TarihYaz(g.odemeSonTarihi);
                s["temerrutTarihi"] = TarihYaz(g.temerrutTarihi);
                s["faizinDurdurulmaTarihi"] = TarihYaz(g.faizinDurdurulmaTarihi);
                s["gakDovizKuru"] = N(g.gakDovizKuru);
                s["gakMiktarAB"] = N(g.gakMiktarAB);
                s["gakMiktarUlusal"] = N(g.gakMiktarUlusal);
                s["gakToplam"] = N(G(() => g.gakToplam, 0d));
                s["gakMiktarABEUR"] = N(G(() => g.gakMiktarABEUR, 0d));
                s["gakMiktarUlusalEUR"] = N(G(() => g.gakMiktarUlusalEUR, 0d));
                s["gakToplamEUR"] = N(G(() => g.gakToplamEUR, 0d));
                s["silinmeyecekMiktar"] = N(g.silinmeyecekMiktar);
                s["silinmeyecekMiktarEUR"] = N(G(() => g.silinmeyecekMiktarEUR, 0d));
                return Results.Json(s);
            }
            catch (Exception)
            {
                return Hata("Geri alım kararı bilgileri okunamadı.");
            }
        }

        /// <summary>Mektup alınma (tebellüğ) tarihinden ödeme son tarihi (1 ay sonrası, tatil değilse) ve temerrüt tarihi (eski TesellumTarihi_Changed).</summary>
        private static IResult GakTarihler(HttpContext baglam, IWebHostEnvironment ortam, string tesellumTarihi)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            DateTime t = TarihOku(tesellumTarihi);
            if (t == DateTime.MinValue) return Results.Json(new { odemeSonTarihi = "", temerrutTarihi = "" });
            try
            {
                GeriAlimKarari g = new GeriAlimKarari();
                g.tesellumTarihi.Yaz(t);
                g.TarihleriBul();
                return Results.Json(new { odemeSonTarihi = TarihYaz(g.odemeSonTarihi), temerrutTarihi = TarihYaz(g.temerrutTarihi) });
            }
            catch (Exception)
            {
                return Hata("Tarihler hesaplanamadı.");
            }
        }

        private static async Task<IResult> GakKaydet(HttpContext baglam, IWebHostEnvironment ortam, ABGakIstegi i, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (i == null) return Results.BadRequest();
            if (!IslemTurGecerli(i.islemTur, true)) return Basarisiz("İşlem türü seçiniz.");
            try
            {
                GeriAlimKarari form = new GeriAlimKarari();
                form.gakKaldirma = i.kaldirma;
                form.ipardDonemi = i.ipardDonemi;
                form.dosyaAnahtar = i.dosyaAnahtar;
                form.sira = i.sira;
                form.geriAlimKarariTarihi.Yaz(TarihOku(i.geriAlimKarariTarihi));
                form.geriAlimSayi = i.geriAlimSayi ?? "";
                form.bildirimTarihi.Yaz(TarihOku(i.bildirimTarihi));
                form.tesellumTarihi.Yaz(TarihOku(i.tesellumTarihi));
                form.odemeSonTarihi.Yaz(TarihOku(i.odemeSonTarihi));
                form.temerrutTarihi.Yaz(TarihOku(i.temerrutTarihi));
                form.faizinDurdurulmaTarihi.Yaz(TarihOku(i.faizinDurdurulmaTarihi));
                form.gakDovizKuru = i.gakDovizKuru;
                form.gakMiktarAB = i.gakMiktarAB;
                form.gakMiktarUlusal = i.gakMiktarUlusal;
                form.silinmeyecekMiktar = i.silinmeyecekMiktar;
                form.gerekce = i.gerekce ?? "";
                return IslemSonucu(Servis().KaydetGAKForm(kullanan, form, (ENUMDosyaIslemTur)i.islemTur), i.sira);
            }
            catch (Exception)
            {
                return Basarisiz("İşlem Başarısız.");
            }
        }

        // ================================================================ süreç durumu

        private static IResult DurumAc(HttpContext baglam, IWebHostEnvironment ortam, int? iPARDDonemi, int? dosyaAnahtar, int? sira)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                DosyaAB form = DosyaOku(kullanan, iPARDDonemi ?? 0, dosyaAnahtar ?? 0, out string hata);
                if (form == null) return Hata(hata);
                DosyaABDurum d = new DosyaABDurum();
                d.ipardDonemi = form.iPARDDonemi;
                d.dosyaAnahtar = form.dosyaAnahtar;
                d.sira = sira ?? 0;
                if (d.sira > 0)
                {
                    ObjectVAN ov = Servis().OkuDurumForm(kullanan, d);
                    if (ov != null && ov.sonuc.islemSonuc) d = (DosyaABDurum)ov.o;
                }
                else
                {
                    d.kayitDurum = DosyaABParca.ENUMKayitDurum.YENI;
                    d.sira = 0;
                    d.surecAsamasi = form.geriAlimSurecAsamasi;
                    d.yorumlarTR = form.yorumlarTR;
                    d.yorumlarEN = form.yorumlarEN;
                }
                Dictionary<string, object> s = ParcaOrtak(d);
                s["surecAsamasi"] = (int)d.surecAsamasi;
                s["yorumlarTR"] = d.yorumlarTR ?? "";
                return Results.Json(s);
            }
            catch (Exception)
            {
                return Hata("Süreç durum bilgileri okunamadı.");
            }
        }

        private static async Task<IResult> DurumKaydet(HttpContext baglam, IWebHostEnvironment ortam, ABDurumIstegi i, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (i == null) return Results.BadRequest();
            if (!IslemTurGecerli(i.islemTur, false)) return Basarisiz("İşlem türü seçiniz.");
            try
            {
                DosyaABDurum form = new DosyaABDurum();
                form.ipardDonemi = i.ipardDonemi;
                form.dosyaAnahtar = i.dosyaAnahtar;
                form.sira = i.sira;
                form.surecAsamasi = (DosyaAB.ENUMDosyaDurum)i.surecAsamasi;
                form.yorumlarTR = i.yorumlarTR ?? "";
                form.yorumlarEN = "";
                form.gerekce = i.gerekce ?? "";
                return IslemSonucu(Servis().KaydetDurumForm(kullanan, form, (ENUMDosyaIslemTur)i.islemTur), i.sira);
            }
            catch (Exception)
            {
                return Basarisiz("İşlem Başarısız.");
            }
        }

        // ================================================================ taksit

        private static DosyaABTaksit EkrandanTaksitOku(ABTaksitIstegi i)
        {
            DosyaABTaksit form = new DosyaABTaksit();
            form.ipardDonemi = i.ipardDonemi;
            form.dosyaAnahtar = i.dosyaAnahtar;
            form.sira = i.sira;
            form.girisTarihi.Yaz(TarihOku(i.girisTarihi));
            form.taksitTalepTarihi.Yaz(TarihOku(i.taksitTalepTarihi));
            form.taksitTalepSayi = i.taksitTalepSayi ?? "";
            form.taksitKararTarihi.Yaz(TarihOku(i.taksitKararTarihi));
            form.taksitKararSayi = i.taksitKararSayi ?? "";
            form.taksitIlkTarih.Yaz(TarihOku(i.taksitIlkTarih));
            form.taksitSayisi = i.taksitSayisi;
            form.tecilFaizNo = i.tecilFaizNo;
            form.taksitKaldirma = i.kaldirma;
            form.gerekce = i.gerekce ?? "";
            return form;
        }

        /// <summary>Eski TaksitlendirIc + TaksitBorcuYaz: girilen bilgilerle taksitler (kaydedilmeden) hesaplanır.</summary>
        private static object Taksitlendir(DosyaAB form, DosyaABTaksit t)
        {
            form.tecilFaizNo = t.tecilFaizNo;
            if (form.borcHareket == null) form.borcHareket = new TNSCollection();
            DateTime ilk = Dt(t.taksitIlkTarih), talep = Dt(t.taksitTalepTarihi);
            if (ilk != DateTime.MinValue && talep != DateTime.MinValue && talep >= G(() => form.geriAlimKarariTarihiDT, DateTime.MinValue) && t.taksitSayisi > 0)
            {
                form.taksitGirisTarih = new TNSDateTime(Dt(t.girisTarihi));
                form.taksitTalepTarih = new TNSDateTime(talep);
                form.taksitIlkTarih = new TNSDateTime(ilk);
                form.YeniTaksitYap(t.taksitSayisi, t);
            }
            else
                form.TaksitleriSil();
            return new
            {
                toplamAnaPara = N(G(() => form.kalanMiktarToplam, 0d)),
                toplamFaiz = N(G(() => form.kalanFaizMiktarToplam, 0d)),
                satirlar = Hareketler(form.borcHareket, true)
            };
        }

        private static IResult TaksitAc(HttpContext baglam, IWebHostEnvironment ortam, int? iPARDDonemi, int? dosyaAnahtar, int? sira)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                DosyaAB form = DosyaOku(kullanan, iPARDDonemi ?? 0, dosyaAnahtar ?? 0, out string hata);
                if (form == null) return Hata(hata);
                DosyaABTaksit t = new DosyaABTaksit();
                t.ipardDonemi = form.iPARDDonemi;
                t.dosyaAnahtar = form.dosyaAnahtar;
                t.sira = sira ?? 0;
                if (t.sira > 0)
                {
                    ObjectVAN ov = Servis().OkuTaksitForm(kullanan, t);
                    if (ov != null && ov.sonuc.islemSonuc) t = (DosyaABTaksit)ov.o;
                }
                else
                {
                    t.kayitDurum = DosyaABParca.ENUMKayitDurum.YENI;
                    t.sira = 0;
                    t.girisTarihi.Yaz(Dt(form.taksitGirisTarih));
                    t.taksitTalepTarihi.Yaz(G(() => form.taksitTalepTarihDT, DateTime.MinValue));
                    t.taksitTalepSayi = form.taksitTalepSayi;
                    t.taksitKararTarihi.Yaz(G(() => form.taksitKararTarihDT, DateTime.MinValue));
                    t.taksitKararSayi = form.taksitKararSayi;
                    t.taksitIlkTarih.Yaz(G(() => form.taksitIlkTarihDT, DateTime.MinValue));
                    t.taksitSayisi = form.taksitSayisi;
                    t.tecilFaizNo = form.tecilFaizNo;
                    t.taksitKaldirma = false;
                }
                Dictionary<string, object> s = ParcaOrtak(t);
                s["kaldirma"] = t.taksitKaldirma;
                s["girisTarihi"] = TarihYaz(t.girisTarihi);
                s["taksitTalepTarihi"] = TarihYaz(t.taksitTalepTarihi);
                s["taksitTalepSayi"] = t.taksitTalepSayi ?? "";
                s["taksitKararTarihi"] = TarihYaz(t.taksitKararTarihi);
                s["taksitKararSayi"] = t.taksitKararSayi ?? "";
                s["taksitIlkTarih"] = TarihYaz(t.taksitIlkTarih);
                s["taksitSayisi"] = t.taksitSayisi;
                s["tecilFaizNo"] = t.tecilFaizNo;
                s["taksitler"] = Taksitlendir(form, t);
                return Results.Json(s);
            }
            catch (Exception)
            {
                return Hata("Taksit bilgileri okunamadı.");
            }
        }

        private static async Task<IResult> TaksitHesapla(HttpContext baglam, IWebHostEnvironment ortam, ABTaksitIstegi i, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (i == null) return Results.BadRequest();
            try
            {
                DosyaAB form = DosyaOku(kullanan, i.ipardDonemi, i.dosyaAnahtar, out string hata);
                if (form == null) return Hata(hata);
                return Results.Json(Taksitlendir(form, EkrandanTaksitOku(i)));
            }
            catch (Exception)
            {
                return Hata("Taksitler hesaplanamadı.");
            }
        }

        private static async Task<IResult> TaksitKaydet(HttpContext baglam, IWebHostEnvironment ortam, ABTaksitIstegi i, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (i == null) return Results.BadRequest();
            if (!IslemTurGecerli(i.islemTur, false)) return Basarisiz("İşlem türü seçiniz.");
            try
            {
                return IslemSonucu(Servis().KaydetTaksitForm(kullanan, EkrandanTaksitOku(i), (ENUMDosyaIslemTur)i.islemTur), i.sira);
            }
            catch (Exception)
            {
                return Basarisiz("İşlem Başarısız.");
            }
        }
    }
}
