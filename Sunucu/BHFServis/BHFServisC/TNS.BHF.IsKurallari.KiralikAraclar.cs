using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHF;
using TNS.BHF.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.BHF.IsKurallari
{
    /// <summary>
    /// BHFKiralikAraclar formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class BHFKiralikAraclar
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriBHF";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, KiralikAracSozlesmesi form, Sonuc s)
        {
            TNS.MUH.IMUHServis muhServis = TNS.MUH.Arac.Tanimla();
            if (!muhServis.MuhasebeBirimYetkiVarMi(kullanan, form.saymanlikNo.ToString(), form.birimKod, MUH.ENUMOGY.Gorme))
            {
                s.DegerAta("Bu birimle (muhasebe:" + form.saymanlikNo + ", birim: " + form.birimKod + ") ilgili verileri görme yetkiniz bulunmamaktadır.");
            }
            return s.islemSonuc;
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, KiralikAracSozlesmesi form, Sonuc s)
        {
            TNS.MUH.IMUHServis muhServis = TNS.MUH.Arac.Tanimla();
            if (!muhServis.MuhasebeBirimYetkiVarMi(kullanan, form.saymanlikNo.ToString(), form.birimKod, MUH.ENUMOGY.Gorme))
            {
                s.DegerAta("Bu birimle (muhasebe:" + form.saymanlikNo + ", birim: " + form.birimKod + ") ilgili verileri görme yetkiniz bulunmamaktadır.");
            }
            return s.islemSonuc;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, KiralikAracSozlesmesi form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (form.saymanlikNo == 0) s.DegerAta("Lütfen saymanlık seçiniz!");
            if (string.IsNullOrWhiteSpace(form.birimKod)) s.DegerAta("Lütfen birim seçiniz!");
            if (TNSDateTime.IsNull(form.imzaTarihi)) s.DegerAta("Lütfen imza tarihini giriniz!");
            if (TNSDateTime.IsNull(form.isBaslangicTarihi)) s.DegerAta("Lütfen iş başlangıç tarihini giriniz!");
            else if (TNSDateTime.IsNull(form.isBitisTarihi)) s.DegerAta("Lütfen iş bitiş tarihini giriniz!");
            else if (form.isBitisTarihiDT < form.isBaslangicTarihiDT) s.DegerAta("Lütfen iş bitiş tarihini, başlangıç tarihinden büyük giriniz!");
            if (string.IsNullOrWhiteSpace(form.sozlesmeNo)) s.DegerAta("Lütfen sözleşme numarasını giriniz!");
            if (form.firmaKod <= 0) s.DegerAta("Lütfen firma seçiniz!");
            //if (form.sozlesmeBedeliDoner < 0) s.DegerAta("Hatalı Döner Sermaye sözleşme bedeli giriniz!");
            //if (form.sozlesmeBedeliButce < 0) s.DegerAta("Hatalı Bütçe sözleşme bedeli giriniz!");
            //if (form.sozlesmeBedeliTop <= 0) s.DegerAta("Lütfen sözleşme bedeli giriniz!");
            //if (form.butceTuru < 1 || form.butceTuru > 3) s.DegerAta("Lütfen bütçe türü giriniz!");
            //if (form.sozlesmeBedeliButce > 0 && (form.surecYil <= 0 || string.IsNullOrWhiteSpace(form.surecNo))) s.DegerAta("Lütfen süreç numarası giriniz!");
            //if (form.sozlesmeBedeliButce == 0 && (form.surecYil > 0 || !string.IsNullOrWhiteSpace(form.surecNo))) s.DegerAta("Lütfen süreç numarası girmeyiniz!");

            return s;
        }

        /// <summary>
        /// KiralikAracSozlesmesi nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KiralikAracSozlesmesi yapısında KiralikAracSozlesmesi: Araç kiralamak için yapılan sözleşmelerin girildiği ekran bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc KaydetSozlesme(VeriErisim veriErisim, Kullanici kullanan, KiralikAracSozlesmesi form)
        {
            Sonuc s = new Sonuc(true, "", "");
            KiralikAracSozlesmesi okunan = null;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            if (form.yil > 0 && form.sozlesmeKod > 0)
            {
                ObjectVAN ov = OkuSozlesme(veriErisim, kullanan, form, false);
                if (!ov.sonuc.islemSonuc)
                {
                    s = ov.sonuc;
                }
                else if (ov.o == null)
                {
                    s.DegerAta("Verdiğiniz koda ait sözleşme bulunamadı!");
                }
                if (!s.islemSonuc)
                {
                    ve.VeriErisimKapat();
                    return s;
                }
                okunan = (KiralikAracSozlesmesi)ov.o;
            }
            else
            {
                if (form.yil != 0 || form.sozlesmeKod != 0)
                    s.DegerAta("Lütfen yeni kayıtlar için yıl ve sözleşme kod alanlarını girmeyiniz!");
                MUH.MuhasebeBirim muhasebeBirim = new MUH.MuhasebeBirim();
                muhasebeBirim.muhasebe = form.saymanlikNo;
                muhasebeBirim.birimKod = form.birimKod;
                muhasebeBirim.pasif = 1;
                ObjectArray oaMuhlar = MUH.Arac.Tanimla().MuhasebeBirimListele(muhasebeBirim);
                if (oaMuhlar.sonuc.islemSonuc)
                {
                    if (oaMuhlar.ObjeSayisi == 1)
                    {
                        muhasebeBirim = (MUH.MuhasebeBirim)oaMuhlar[0];
                        if (muhasebeBirim.pasif > 0)
                            s.DegerAta("Verilen birim (" + form.saymanlikNoBirim + ") pasif olduğundan işlem yapılamaz!");
                    }
                    else if (oaMuhlar.ObjeSayisi == 0)
                        s.DegerAta("Verilen birim (" + form.saymanlikNoBirim + ") bulunamadı!");
                    else
                        s.DegerAta("Verilen birim (" + form.saymanlikNoBirim + ") için birden fazla kayıt bulundu!");
                }
                else
                    s.DegerAta("Muhasebe birim okunurken hata:" + oaMuhlar.sonuc.hataStr);
            }

            if (s.islemSonuc && okunan != null)
                YazmaYetkisiVarMi(kullanan, okunan, s);

            if (s.islemSonuc)
                YazmaYetkisiVarMi(kullanan, form, s);

            if (s.islemSonuc)
                s = AlanlarGecerliMi(kullanan, form);

            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            TABBHFKIRALIKARACSOZLESMESI tabBHFKIRALIKARACSOZLESMESI = new TABBHFKIRALIKARACSOZLESMESI(ve);

            ve.BeginTransaction();

            if (form.yil == 0 && form.sozlesmeKod == 0)
            {
                form.yil = form.imzaTarihi.Yil;
                form.sozlesmeKod = tabBHFKIRALIKARACSOZLESMESI.NoBul(form);
                if (ve.hata != "")
                    s.DegerAta(ve.hata);
            }

            if (s.islemSonuc)
            {
                if (okunan == null)
                    s = tabBHFKIRALIKARACSOZLESMESI.Ekle(form);
                else
                    s = tabBHFKIRALIKARACSOZLESMESI.Guncelle(form);
            }
            if (s.islemSonuc)
            {
                s.anahtar = form.yil.ToString() + "-" + form.sozlesmeKod.ToString();
                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// KiralikAracSozlesmesi nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KiralikAracSozlesmesi yapısında KiralikAracSozlesmesi: Araç kiralamak için yapılan sözleşmelerin girildiği ekran bilgilerini tutan değişken</param>
        public static ObjectVAN OkuSozlesme(VeriErisim veriErisim, Kullanici kullanan, KiralikAracSozlesmesi form, bool detayVar)
        {
            ObjectVAN ov = new ObjectVAN();
            if (form.yil > 0 && form.sozlesmeKod > 0)
            {
                ObjectArray s = ListeleSozlesme(veriErisim, kullanan, form, detayVar);
                ov.sonuc = s.sonuc;
                if (s != null && s.ObjeSayisi > 0) ov.o = s[0];
            }
            else
                ov.sonuc.DegerAta("Lütfen sözleşme kodunu seçiniz!");
            return ov;
        }

        /// <summary>
        /// KiralikAracSozlesmesi listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KiralikAracSozlesmesi yapısında KiralikAracSozlesmesi: Araç kiralamak için yapılan sözleşmelerin girildiği ekran bilgilerini tutan değişken</param>
        public static ObjectArray ListeleSozlesme(VeriErisim veriErisim, Kullanici kullanan, KiralikAracSozlesmesi form, bool detayVar)
        {
            ObjectArray oa = new ObjectArray();
            if (!(form.yil != 0 && form.sozlesmeKod != 0))
            {
                if (!GormeYetkisiVarMi(kullanan, form, oa.sonuc))
                {
                    return oa;
                }
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHFKIRALIKARACSOZLESMESI tabBHFKIRALIKARACSOZLESMESI = new TABBHFKIRALIKARACSOZLESMESI(ve);

            IDataReader dr;
            dr = tabBHFKIRALIKARACSOZLESMESI.Listele(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            BKD.ButceKodYapi bky = TNS.BKD.Arac.Tanimla().ButceKodBilgiBul(form.yil, 14, "");
            while (dr.Read())
            {
                KiralikAracSozlesmesi detay = new KiralikAracSozlesmesi();
                detay.yil = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARACSOZLESMESI.ENUMAlanlar.YIL, 0);
                detay.sozlesmeKod = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARACSOZLESMESI.ENUMAlanlar.SOZLESMEKOD, 0);
                detay.saymanlikNo = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARACSOZLESMESI.ENUMAlanlar.SAYMANLIKNO, 0);
                detay.birimKod = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARACSOZLESMESI.ENUMAlanlar.BIRIMKOD, "");
                detay.imzaTarihi = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARACSOZLESMESI.ENUMAlanlar.IMZATARIHI);
                detay.sozlesmeNo = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARACSOZLESMESI.ENUMAlanlar.SOZLESMENO, "");
                detay.isBaslangicTarihi = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARACSOZLESMESI.ENUMAlanlar.ISBASLANGICTARIHI);
                detay.isBitisTarihi = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARACSOZLESMESI.ENUMAlanlar.ISBITISTARIHI);
                detay.firmaKod = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARACSOZLESMESI.ENUMAlanlar.FIRMAKOD, 0);
                //detay.sozlesmeBedeliButce = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARACSOZLESMESI.ENUMAlanlar.SOZLESMEBEDELIBUTCE, 0.0);
                //detay.sozlesmeBedeliDoner = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARACSOZLESMESI.ENUMAlanlar.SOZLESMEBEDELIDONER, 0.0);
                detay.ilcelerDahil = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARACSOZLESMESI.ENUMAlanlar.ILCELERDAHIL, 0);
                detay.surecYil = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARACSOZLESMESI.ENUMAlanlar.SURECYIL, 0);
                detay.surecNo = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARACSOZLESMESI.ENUMAlanlar.SURECNO, "");
                detay.firmaAd = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARACSOZLESMESI.ENUMAlanlar.SURECNO + 1, "");
                detay.muhasebeAd = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARACSOZLESMESI.ENUMAlanlar.SURECNO + 2, "");
                detay.muhBirimAd = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARACSOZLESMESI.ENUMAlanlar.SURECNO + 3, "");
                detay.kiralikArac = ListeleDetaylar(VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARACSOZLESMESI.ENUMAlanlar.SURECNO + 4, ""), bky.kodYapi);
                bool ekle = true;
                if (form.yil != 0 && form.sozlesmeKod != 0)
                {
                    if (!GormeYetkisiVarMi(kullanan, detay, oa.sonuc))
                    {
                        ekle = false;
                    }
                }
                if (ekle)
                    oa.Ekle(detay);

            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        private static TNSCollection ListeleDetaylar(string okunanXML, string kodYapi)
        {
            TNSCollection pVAN = new TNSCollection();
            if (string.IsNullOrWhiteSpace(okunanXML)) return pVAN;

            XMLStringDataReader r = new XMLStringDataReader(okunanXML);
            if (r != null)
            {
                while (r.Read())
                {
                    pVAN.Add(DetayOku(r, kodYapi));
                }
                r.Close();
            }
            return pVAN;
        }

        private static ObjectArray SozlesmeAracOku(TABBHFKIRALIKARAC tabBHFKIRALIKARAC, Kullanici kullanan, KiralikAracSozlesmesi formIc, int tasitSiraNo)
        {
            ObjectArray oa = new ObjectArray();
            BKD.ButceKodYapi bky = TNS.BKD.Arac.Tanimla().ButceKodBilgiBul(formIc.yil, 14, "");
            IDataReader dr = tabBHFKIRALIKARAC.Listele(formIc.yil, formIc.sozlesmeKod, tasitSiraNo);
            if (dr == null)
            {
                oa.sonuc.DegerAta(tabBHFKIRALIKARAC.Hata());
                return oa;
            }
            while (dr.Read())
            {
                oa.objeler.Add(DetayOku(dr, bky.kodYapi));
            }
            dr.Close();
            return oa;
        }

        private static KiralikArac DetayOku(IDataReader dr, string kodYapi)
        {
            KiralikArac detay = new KiralikArac();
            detay.aracSiraNo = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARAC.ENUMAlanlar.ARACSIRANO, 0);
            detay.tasitCinsi = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARAC.ENUMAlanlar.TASITCINSI, 0);
            detay.enBuyukYas = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARAC.ENUMAlanlar.ENBUYUKYAS, 0);
            detay.koltukSayisi = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARAC.ENUMAlanlar.KOLTUKSAYISI, 0);
            detay.yakitTuru = (ENUMYakitTuru)VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARAC.ENUMAlanlar.YAKITTURU, 0);
            detay.soforDahil = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARAC.ENUMAlanlar.SOFORDAHIL, 0);
            detay.yakitDahil = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARAC.ENUMAlanlar.YAKITDAHIL, 0);
            detay.adet = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARAC.ENUMAlanlar.ADET, 0);
            detay.sure = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARAC.ENUMAlanlar.SURE, 0);
            detay.sureBirimi = (ENUMSureBirim)VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARAC.ENUMAlanlar.SUREBIRIMI, 0);
            detay.birimFiyat = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARAC.ENUMAlanlar.BIRIMFIYAT, 0.0);
            detay.pebKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapi, VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARAC.ENUMAlanlar.PEBKOD, ""));
            detay.dsMi = string.IsNullOrWhiteSpace(detay.pebKod);
            detay.tasitCinsiAd = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARAC.ENUMAlanlar.PEBKOD + 1, "");
            detay.tasitTKod = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARAC.ENUMAlanlar.PEBKOD + 2, "");
            detay.tasitTipKodu = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARAC.ENUMAlanlar.PEBKOD + 3, 0);
            detay.tasitTipAd = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARAC.ENUMAlanlar.PEBKOD + 4, "");
            detay.pebAd = VeriErisim.DegerAl(dr, (int)TABBHFKIRALIKARAC.ENUMAlanlar.PEBKOD + 5, "");
            return detay;
        }

        internal static ObjectVAN OkuArac(VeriErisim veriErisim, Kullanici kullanan, int yil, int sozlesmeKod, int aracSiraNo)
        {
            ObjectVAN ovAna = new ObjectVAN();
            if (yil <= 0 || sozlesmeKod <= 0)
            {
                ovAna.sonuc.DegerAta("Lütfen yıl ve sözleşme kod alanlarını giriniz!");
                return ovAna;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            KiralikAracSozlesmesi soz = new KiralikAracSozlesmesi();
            soz.yil = yil;
            soz.sozlesmeKod = sozlesmeKod;
            ObjectVAN ovDetay = OkuSozlesme(veriErisim, kullanan, soz, false);
            if (!ovDetay.sonuc.islemSonuc)
            {
                ovAna.sonuc = ovDetay.sonuc;
            }
            else if (ovDetay.o == null)
            {
                ovAna.sonuc.DegerAta("Verdiğiniz koda ait sözleşme bulunamadı!");
            }
            soz = (KiralikAracSozlesmesi)ovDetay.o;

            if (ovAna.sonuc.islemSonuc && soz != null)
                YazmaYetkisiVarMi(kullanan, soz, ovAna.sonuc);

            if (!ovAna.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return ovAna;
            }

            TABBHFKIRALIKARAC tabBHFKIRALIKARAC = new TABBHFKIRALIKARAC(ve);
            ObjectArray oa = SozlesmeAracOku(tabBHFKIRALIKARAC, kullanan, soz, aracSiraNo);
            if (!oa.sonuc.islemSonuc)
                ovAna.sonuc = oa.sonuc;
            else if (oa.ObjeSayisi != 1)
                ovAna.sonuc.DegerAta("Aranan kayıt bulunamadı!");
            else
                ovAna.o = oa.objeler[0];

            ve.VeriErisimKapat();
            return ovAna;
        }

        private static Sonuc AlanlarGecerliMiArac(Kullanici kullanan, KiralikArac form, int kodBoy)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (form.tasitCinsi == 0) s.DegerAta("Lütfen taşıt cinsi seçiniz!");
            if (form.enBuyukYas <= 0) s.DegerAta("Lütfen kiralanacak araç için olası en büyük yaşı giriniz!");
            if (form.koltukSayisi <= 0) s.DegerAta("Lütfen koltuk sayısını giriniz!");
            if (form.yakitTuru == ENUMYakitTuru.Bos) s.DegerAta("Lütfen yakıt türü giriniz!");
            if (form.adet < 1) s.DegerAta("Lütfen kiralanacak araç sayısını giriniz!");
            if (form.sure < 1) s.DegerAta("Lütfen kiralanacak süreyi giriniz!");
            if (form.sureBirimi == ENUMSureBirim.Bos) s.DegerAta("Lütfen kiralama süresinin birimini giriniz!");
            if (form.birimFiyat < 1) s.DegerAta("Lütfen birim fiyatı giriniz!");
            if (form.dsMi)
            {
                if (!string.IsNullOrWhiteSpace(form.pebKod)) s.DegerAta("Lütfen Döner Sermayeler için faaliyet girmeyiniz!");
            }
            else
            {
                //if (string.IsNullOrWhiteSpace(form.pebKod)) s.DegerAta("Lütfen ilişkili faaliyet seçiniz!");
                //else if (form.pebKod.Replace(".", "").Length != kodBoy) s.DegerAta("Lütfen en alt seviyedeki faaliyeti seçiniz!");
            }
            return s;
        }

        public static Sonuc KaydetArac(VeriErisim veriErisim, Kullanici kullanan, int yil, int sozlesmeKod, KiralikArac form)
        {
            Sonuc s = new Sonuc(true, "", "");
            KiralikAracSozlesmesi okunan = null;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            if (yil > 0 && sozlesmeKod > 0)
            {
                KiralikAracSozlesmesi soz = new KiralikAracSozlesmesi();
                soz.yil = yil;
                soz.sozlesmeKod = sozlesmeKod;
                ObjectVAN ov = OkuSozlesme(veriErisim, kullanan, soz, false);
                if (!ov.sonuc.islemSonuc)
                {
                    s = ov.sonuc;
                }
                else if (ov.o == null)
                {
                    s.DegerAta("Verdiğiniz koda ait sözleşme bulunamadı!");
                }
                if (!s.islemSonuc)
                {
                    ve.VeriErisimKapat();
                    return s;
                }
                okunan = (KiralikAracSozlesmesi)ov.o;
            }
            else
                s.DegerAta("Lütfen yıl ve sözleşme kod alanlarını giriniz!");

            if (s.islemSonuc && okunan != null)
                YazmaYetkisiVarMi(kullanan, okunan, s);

            if (s.islemSonuc)
            {
                BKD.ButceKodYapi bky = TNS.BKD.Arac.Tanimla().ButceKodBilgiBul(okunan.yil, 14, "");
                s = AlanlarGecerliMiArac(kullanan, form, bky.KirilimToplamUzunluk(10));
            }
            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            TABBHFKIRALIKARAC tabBHFKIRALIKARAC = new TABBHFKIRALIKARAC(ve);

            ve.BeginTransaction();

            bool yeni = false;
            if (form.aracSiraNo == 0)
            {
                form.aracSiraNo = tabBHFKIRALIKARAC.NoBul(yil, sozlesmeKod);
                if (ve.hata != "")
                    s.DegerAta(ve.hata);
                yeni = true;
            }

            if (s.islemSonuc)
            {
                if (yeni)
                    s = tabBHFKIRALIKARAC.Ekle(yil, sozlesmeKod, form);
                else
                    s = tabBHFKIRALIKARAC.Guncelle(yil, sozlesmeKod, form);
            }
            if (s.islemSonuc)
            {
                s.anahtar = form.aracSiraNo.ToString();
                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        public static Sonuc SilArac(VeriErisim veriErisim, Kullanici kullanan, int yil, int sozlesmeKod, int aracSiraNo)
        {
            Sonuc s = new Sonuc(true, "", "");
            KiralikAracSozlesmesi okunan = null;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            if (yil > 0 && sozlesmeKod > 0)
            {
                KiralikAracSozlesmesi soz = new KiralikAracSozlesmesi();
                soz.yil = yil;
                soz.sozlesmeKod = sozlesmeKod;
                ObjectVAN ov = OkuSozlesme(veriErisim, kullanan, soz, false);
                if (!ov.sonuc.islemSonuc)
                {
                    s = ov.sonuc;
                }
                else if (ov.o == null)
                {
                    s.DegerAta("Verdiğiniz koda ait sözleşme bulunamadı!");
                }
                if (!s.islemSonuc)
                {
                    ve.VeriErisimKapat();
                    return s;
                }
                okunan = (KiralikAracSozlesmesi)ov.o;
            }
            else
                s.DegerAta("Lütfen yıl ve sözleşme kod alanlarını giriniz!");

            if (s.islemSonuc && okunan != null)
                YazmaYetkisiVarMi(kullanan, okunan, s);

            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            TABBHFKIRALIKARAC tabBHFKIRALIKARAC = new TABBHFKIRALIKARAC(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabBHFKIRALIKARAC.Sil(yil, sozlesmeKod, aracSiraNo);
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc SilSozlesme(VeriErisim veriErisim, Kullanici kullanan, KiralikAracSozlesmesi form)
        {
            Sonuc sonuc = new Sonuc();
            if (form.yil <= 0 || form.sozlesmeKod <= 0)
            {
                sonuc.DegerAta("Lütfen yıl ve sözleşme kod alanlarını giriniz!");
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            ObjectVAN ovDetay = OkuSozlesme(veriErisim, kullanan, form, true);
            if (!ovDetay.sonuc.islemSonuc)
            {
                sonuc = ovDetay.sonuc;
            }
            else if (ovDetay.o == null)
            {
                sonuc.DegerAta("Verdiğiniz koda ait sözleşme bulunamadı!");
            }
            form = (KiralikAracSozlesmesi)ovDetay.o;

            if (sonuc.islemSonuc)
            {
                if (form == null)
                    sonuc.DegerAta("Kayıt bulunamadı!");
                else if (form.kiralikArac.Count > 0)
                    sonuc.DegerAta("Araç kaydı olan sözleşmeler silinemez");
                else
                    YazmaYetkisiVarMi(kullanan, form, sonuc);
            }

            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }

            TABBHFKIRALIKARACSOZLESMESI tab = new TABBHFKIRALIKARACSOZLESMESI(ve);

            ve.BeginTransaction();
            sonuc = tab.Sil(form);
            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return sonuc;
        }

        internal static ObjectArray KiralikAracRaporla(VeriErisim veriErisim, Kullanici kullanan, int yil, int saymanlikNo, string birimKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            donusDeger.sonuc = new Sonuc(true, "", "");
            if (yil <= 0)
                donusDeger.sonuc.DegerAta(ButceFormResI.BHFETS019);

            if (!donusDeger.sonuc.islemSonuc)
                return donusDeger;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHFKIRALIKARACSOZLESMESI tabHAT = new TABBHFKIRALIKARACSOZLESMESI(ve);

            IDataReader dr = tabHAT.Raporla(yil, saymanlikNo, birimKod);
            if (dr != null)
            {
                string eskiSiraNo = "";
                HizmetAlimiTasitBilgisiDetay hat = null;
                while (dr.Read())
                {
                    string siraNo = VeriErisim.DegerAl(dr, 0, "");
                    if (siraNo != eskiSiraNo)
                    {
                        hat = new HizmetAlimiTasitBilgisiDetay();
                        hat.tasitSozluk.siraNo = siraNo;
                        eskiSiraNo = siraNo;
                        donusDeger.Ekle(hat);
                    }

                    int SOFORDAHIL = VeriErisim.DegerAl(dr, 1, 0);
                    int YAKITDAHIL = VeriErisim.DegerAl(dr, 2, 0);
                    int ADET = VeriErisim.DegerAl(dr, 3, 0);
                    int SURE = VeriErisim.DegerAl(dr, 4, 0);
                    ENUMSureBirim SUREBIRIMI = (ENUMSureBirim)VeriErisim.DegerAl(dr, 5, 0);
                    double BIRIMFIYAT = VeriErisim.DegerAl(dr, 6, 0.0);
                    string PEBKOD = VeriErisim.DegerAl(dr, 7, "");
                    HizmetAlimiTasitMaliyet tut = null;
                    int j = 0;
                    if (string.IsNullOrWhiteSpace(PEBKOD))
                        j = 1;
                    if (SOFORDAHIL == 1 && YAKITDAHIL == 1) tut = hat.soforluYakitGiderDahil[j];
                    else if (SOFORDAHIL == 1 && YAKITDAHIL != 1) tut = hat.soforluYakitGiderHaric[j];
                    else if (SOFORDAHIL != 1 && YAKITDAHIL == 1) tut = hat.soforsuzYakitGiderDahil[j];
                    else if (SOFORDAHIL != 1 && YAKITDAHIL != 1) tut = hat.soforsuzYakitGiderHaric[j];

                    int tasitAy = KiralikArac.TasitAyBul( SURE,  SUREBIRIMI,  ADET);
                     
                    double maliyet = tasitAy * KiralikArac.BirimFiyatAyBul(SUREBIRIMI, BIRIMFIYAT);
                    tut.adet += tasitAy;
                    tut.sure = 1;
                    tut.maliyet += maliyet;
                }
            }
            else
                donusDeger.sonuc.DegerAta(ve.hata);

            foreach (HizmetAlimiTasitBilgisiDetay hat in donusDeger.objeler)
            {
                for (int j = 0; j < 2; j++)
                {
                    BirimFiyatHesapla(hat.soforluYakitGiderDahil[j].adet, ref hat.soforluYakitGiderDahil[j].sure, ref hat.soforluYakitGiderDahil[j].maliyet);
                    BirimFiyatHesapla(hat.soforluYakitGiderHaric[j].adet, ref hat.soforluYakitGiderHaric[j].sure, ref hat.soforluYakitGiderHaric[j].maliyet);
                    BirimFiyatHesapla(hat.soforsuzYakitGiderDahil[j].adet, ref hat.soforsuzYakitGiderDahil[j].sure, ref hat.soforsuzYakitGiderDahil[j].maliyet);
                    BirimFiyatHesapla(hat.soforsuzYakitGiderHaric[j].adet, ref hat.soforsuzYakitGiderHaric[j].sure, ref hat.soforsuzYakitGiderHaric[j].maliyet);
                }
            }

            return donusDeger;
        }

        private static void BirimFiyatHesapla(int adet, ref double sure, ref double maliyet)
        {
            if (adet <= 0) return;
            maliyet = maliyet / adet;
            //sure = 12;
            //int gadet = adet;
            //adet = adet / 12;
            //if (adet > 0)
            //    maliyet = maliyet / (sure * adet);
            //else
            //    maliyet = maliyet / gadet;
        }
    }
}
