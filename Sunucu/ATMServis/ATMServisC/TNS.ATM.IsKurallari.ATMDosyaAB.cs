using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.KYM;
using TNS.Veri;
using TNS.ATM.IsKurallari.Veri;
using System.Collections;

namespace TNS.ATM.IsKurallari
{
    /// <summary>
    /// ATMDosyaAB formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class ATMDosyaAB
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriATM";

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        private static Sonuc AlanlarGecerliMiBorclu(Kullanici kullanan, ABBorclu form, DosyaAB dosyaAB, Sonuc s)
        {
            if (form.ipardDonemi <= 0) s.DegerAta("Dönem seçiniz!");
            if (string.IsNullOrWhiteSpace(form.defterSirasi)) s.DegerAta("Defter sırası giriniz!");
            if (string.IsNullOrWhiteSpace(form.tedbir)) s.DegerAta("Tedbir giriniz!");
            if (string.IsNullOrWhiteSpace(form.altTedbir)) s.DegerAta("Alttedbir giriniz!");
            if (string.IsNullOrWhiteSpace(form.butceKalemi)) s.DegerAta("Bütçe kalemi giriniz!");
            if (form.saymanlikKod <= 0) s.DegerAta("Saymanlık seçiniz!");
            if (string.IsNullOrWhiteSpace(form.birimKod)) s.DegerAta("Birim giriniz!");
            if (form.borcluKod <= 0) s.DegerAta("Borçluyu seçiniz!");
            if (string.IsNullOrWhiteSpace(form.basvuruKayitNumarasi)) s.DegerAta("Kayıt numarasını giriniz!");
            if (string.IsNullOrWhiteSpace(form.sozlesmeNumarasi)) s.DegerAta("Sözleşme numarasını giriniz!");
            return s;
        }

        private static Sonuc AlanlarGecerliMiDurum(Kullanici kullanan, DosyaABDurum form, DosyaAB dosyaAB, Sonuc s)
        {
            if (form.surecAsamasi == DosyaAB.ENUMDosyaDurum.TANIMSIZ)
                s.DegerAta("Durum aşaması boş bırakılamaz");
            if (string.IsNullOrWhiteSpace(form.gerekce))
                s.DegerAta("Lütfen işlem gerekçesi giriniz!");
            return s;
        }

        private static Sonuc AlanlarGecerliMiTaksit(Kullanici kullanan, DosyaABTaksit form, DosyaAB dosyaAB, Sonuc s)
        {
            if (!form.taksitKaldirma)
            {
                if (TNSDateTime.IsNull(dosyaAB.geriAlimKarariTarihi))
                {
                    s.DegerAta("Geri alım kararı tarihi olmayan dosyalar taksitlendirilemez!");
                }
                else if (TNSDateTime.IsNull(form.taksitTalepTarihi))
                    s.DegerAta("Taksit talep tarihi boş bırakılamaz!");
                else
                {
                    if (dosyaAB.geriAlimKarariTarihi.Oku() >= form.taksitTalepTarihi.Oku())
                        s.DegerAta("Taksit talep tarihi geri alım kararı tarihinden sonra olmalıdır!");

                    if (form.girisTarihi.Oku() < form.taksitTalepTarihi.Oku())
                        s.DegerAta("Sisteme giriş tarihi, taksit talep tarihinden küçük olamaz!");

                    if (TNSDateTime.IsNull(form.taksitKararTarihi))
                        s.DegerAta("Taksit karar tarihi boş bırakılamaz!");
                    else
                    {
                        if (form.taksitKararTarihi.Oku() < form.taksitTalepTarihi.Oku())
                            s.DegerAta("Karar tarihi, talep tarihinden küçük olamaz!");

                        if (TNSDateTime.IsNull(form.taksitIlkTarih))
                            s.DegerAta("Taksit ilk tarihi boş bırakılamaz!");
                        else if (form.taksitIlkTarih.Oku() < form.taksitKararTarihi.Oku())
                            s.DegerAta("Taksit ilk tarihi, karar tarihinden küçük olamaz!");
                    }
                }

                if (String.IsNullOrWhiteSpace(form.taksitTalepSayi))
                    s.DegerAta("Taksit talep sayısı boş bırakılamaz!");

                if (String.IsNullOrWhiteSpace(form.taksitKararSayi))
                    s.DegerAta("Taksit karar sayısı boş bırakılamaz!");

                if (form.taksitSayisi <= 0)
                    s.DegerAta("Taksit sayısı dolu olmalı!");
                if (form.tecilFaizNo < 0)
                    s.DegerAta("Tecil faiz türü seçilmelidir!");
            }
            if (string.IsNullOrWhiteSpace(form.gerekce))
                s.DegerAta("Lütfen işlem gerekçesi giriniz!");
            return s;
        }

        private static Sonuc AlanlarGecerliMiOnBorc(Kullanici kullanan, OnBorc form, DosyaAB dosyaAB, Sonuc s)
        {
            if (!form.OBKaldirma)
            {
                if (TNSDateTime.IsNull(form.onBorcTarihi)) s.DegerAta("Ön borç tarihini giriniz!");
                if (string.IsNullOrWhiteSpace(form.onBorcSayi)) s.DegerAta("Ön borç sayısını giriniz!");
                if (string.IsNullOrWhiteSpace(form.vakaNumarasi) && form.borcTuru != DosyaAB.BorcTurleri.FazlaOdeme) s.DegerAta("Vaka numarasını giriniz!");
                if (form.borcTuru == DosyaAB.BorcTurleri.Tanimsiz) s.DegerAta("Borç Türünü Giriniz!");
                if (string.IsNullOrWhiteSpace(form.borcunKodu)) s.DegerAta("Borç kodunu seçiniz!");
                if (form.onBorcMiktarAB < 0) s.DegerAta("Ön borç miktarı AB payı hatalı!");
                if (form.onBorcMiktarUlusal < 0) s.DegerAta("Ön borç miktarı Ulusal Payı hatalı!");
                if (form.onBorcMiktarUlusal == 0 && form.onBorcMiktarAB == 0) s.DegerAta("Ön borç için Ulusal Pay veya AB Payı girilmelidir!");
            }
            else
            {
                if (!TNSDateTime.IsNull(dosyaAB.geriAlimKarariTarihi)) s.DegerAta("Geri alım kararı olan dosyaların, ön borcu iptal edilemez!");
            }
            if (string.IsNullOrWhiteSpace(form.gerekce)) s.DegerAta("Lütfen işlem gerekçesi giriniz!");
            return s;
        }

        private static Sonuc AlanlarGecerliMiGAK(Kullanici kullanan, GeriAlimKarari form, DosyaAB dosyaAB, Sonuc s)
        {
            if (!form.gakKaldirma)
            {
                if (TNSDateTime.IsNull(dosyaAB.onBorcTarihi))
                    s.DegerAta("Ön borç bilgileri girilmeli ve onaylanmalıdır!");
                else
                {
                    if (TNSDateTime.IsNull(form.geriAlimKarariTarihi))
                        s.DegerAta("Geri alım kararı tarihi dolu olmalıdır!");
                    else
                    {
                        if (!TNSDateTime.IsNull(dosyaAB.onBorcTarihi) && form.geriAlimKarariTarihi.Oku() < dosyaAB.onBorcTarihiDT)
                            s.DegerAta("Geri alım tarihi ön borç tarihinden büyük eşit olmalıdır!");

                        if (!TNSDateTime.IsNull(form.bildirimTarihi) && form.bildirimTarihi.Oku() < form.geriAlimKarariTarihi.Oku())
                            s.DegerAta("Bildirim tarihi geri alım tarihinden büyük eşit olmalıdır!");

                        if (!TNSDateTime.IsNull(form.bildirimTarihi) && !TNSDateTime.IsNull(form.tesellumTarihi) && form.tesellumTarihi.Oku() < form.bildirimTarihi.Oku())
                            s.DegerAta("Mektup alım tarihi, bildirim tarihinden büyük eşit olmalıdır!");
                        else if (TNSDateTime.IsNull(form.bildirimTarihi) && !TNSDateTime.IsNull(form.tesellumTarihi))
                            s.DegerAta("Mektup alım tarihini girmeyiniz!");

                        if (!TNSDateTime.IsNull(form.tesellumTarihi) && !TNSDateTime.IsNull(form.odemeSonTarihi) && form.odemeSonTarihi.Oku() < form.tesellumTarihi.Oku())
                            s.DegerAta("Ödeme son tarihi, Mektup alım tarihinden büyük eşit olmalıdır!");
                        else if (TNSDateTime.IsNull(form.tesellumTarihi) && !TNSDateTime.IsNull(form.odemeSonTarihi))
                            s.DegerAta("Ödeme son tarihini girmeyiniz!");

                        if (!TNSDateTime.IsNull(form.odemeSonTarihi) && TNS.ResmiTatil.ResmiTatilMi(form.odemeSonTarihi, 0))
                            s.DegerAta("Girdiğiniz tarih resmi tatil olduğundan Ödeme son tarihi olamaz!");

                        if (!TNSDateTime.IsNull(form.odemeSonTarihi) && !TNSDateTime.IsNull(form.temerrutTarihi) && form.temerrutTarihi.Oku() < form.odemeSonTarihi.Oku())
                            s.DegerAta("Temerrüt tarihini, Ödeme son tarihinden büyük eşit olmalıdır!");
                        else if (TNSDateTime.IsNull(form.odemeSonTarihi) && !TNSDateTime.IsNull(form.temerrutTarihi))
                            s.DegerAta("Temerrüt tarihini girmeyiniz!");
                    }
                    if (string.IsNullOrWhiteSpace(form.geriAlimSayi)) s.DegerAta("Geri alım sayısını giriniz!");
                    if (form.gakMiktarAB < 0) s.DegerAta("Geri alım miktarı AB payı hatalı!");
                    if (form.gakMiktarUlusal < 0) s.DegerAta("Geri alım miktarı ulusal payı hatalı!");
                }
            }
            else
            {
                if (dosyaAB.taksitSayisi > 0) s.DegerAta("Taksitlendirilmiş dosyaların, geri alım kararı iptal edilemez!");
                //if (dosyaAB.islemYapilanMiktarToplam > 0) s.DegerAta("Geri ödeme yapılmış dosyaların, geri alım kararı iptal edilemez!");
            }
            if (string.IsNullOrWhiteSpace(form.gerekce)) s.DegerAta("Lütfen işlem gerekçesi giriniz!");

            return s;
        }

        /// <summary>
        /// DosyaAB nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DosyaAB yapısında DosyaAB: Borç Dosyası bilgilerini tutan değişken</param>
        public static ObjectVAN Oku(VeriErisim veriErisim, Kullanici kullanan, DosyaAB form, bool yetkiKontroluYap)
        {
            ObjectVAN ov = new ObjectVAN();
            if (form.iPARDDonemi > 0 && form.dosyaAnahtar > 0)
            {
                ObjectArray s = Listele(veriErisim, kullanan, form, yetkiKontroluYap);
                if (s == null)
                {
                    s = new ObjectArray();
                    s.sonuc.DegerAta("Listeleme metodu çalışmadı!");
                }
                ov.sonuc = s.sonuc;
                if (s.ObjeSayisi > 0) ov.o = s[0];
            }
            else
                ov.sonuc.DegerAta("IPARD Dönemi ve dosya kodu dolu olmalıdır!");
            return ov;
        }

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, DosyaAB form, ENUMDosyaIslemTur islemTur)
        {
            if (kullanan == null) return false;
            return Yetki.DosyaYazmaOnaylamaHakkiVarMi(kullanan, form.saymanlikKod, form.birimKod, islemTur).islemSonuc;
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, DosyaAB form)
        {
            if (kullanan == null) return false;
            return Yetki.DosyaOkumaHakkiVarMi(kullanan, form.saymanlikKod, form.birimKod).islemSonuc;
        }

        /// <summary>
        /// DosyaAB listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DosyaAB yapısında DosyaAB: Borç Dosyası bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, DosyaAB form, bool yetkiKontroluYap)
        {
            ObjectArray oa = new ObjectArray();
            bool sonraKontrolYap = false;
            if (yetkiKontroluYap)
            {
                if (form.iPARDDonemi > 0 && form.dosyaAnahtar > 0)
                    sonraKontrolYap = true;
                else
                {
                    if (!GormeYetkisiVarMi(kullanan, form))
                    {
                        oa.sonuc.DegerAta(AlacakTakipResI.DosyaAB002);
                        return oa;
                    }
                }
            }
            IslemKiyasla islemKiyasla = new IslemKiyasla();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMDOSYAAB tabATMDOSYAAB = new TABATMDOSYAAB(ve);
            TABATMBORCHAREKET tabATMBORCHAREKET = new TABATMBORCHAREKET(ve);

            IDataReader dr = tabATMDOSYAAB.Listele(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }

            while (dr.Read())
            {
                DosyaAB detay = new DosyaAB();

                int sonAlan = 0;
                detay.iPARDDonemi = VeriErisim.DegerAl(dr, sonAlan++, 0);
                detay.dosyaAnahtar = VeriErisim.DegerAl(dr, sonAlan++, 0);
                try
                {
                    ListeleBorclu(detay, VeriErisim.DegerAl(dr, sonAlan++, "")); //alanlarBorclu
                    ListeleDurum(detay, VeriErisim.DegerAl(dr, sonAlan++, "")); //alanlarDurum 
                    ListeleOB(detay, VeriErisim.DegerAl(dr, sonAlan++, "")); //alanlarOB 
                    ListeleGAK(detay, VeriErisim.DegerAl(dr, sonAlan++, "")); //alanlarGAK 
                    ListeleTaksit(detay, VeriErisim.DegerAl(dr, sonAlan++, "")); //alanlarTAKSIT

                    ListeleDetaylar(detay, VeriErisim.DegerAl(dr, sonAlan++, ""), detay.geriAlimDovizKuru); //hareketSorgu

                    detay.IslemToplamBul(form.islemTarihi, -1, false, null, form.onayTarihi, -1);

                    bool ekle = false;
                    if (form.borcDurumu == 0) ekle = true;
                    else
                    {
                        if (form.borcDurumu == 4)
                        {
                            if (detay.kalanToplamToplam == 0) ekle = true;
                        }
                        else if (detay.kalanToplamToplam > 0)
                        {
                            if (form.borcDurumu == 1) ekle = true;
                            else if (form.borcDurumu == 2)
                            {
                                if (detay.islemYapilanMiktarToplam > 0) ekle = true;
                            }
                            else
                                if (detay.islemYapilanMiktarToplam == 0) ekle = true;
                        }
                    }
                    if (ekle)
                    {
                        if (sonraKontrolYap)
                        {
                            if (!GormeYetkisiVarMi(kullanan, detay))
                            {
                                oa.sonuc.DegerAta(AlacakTakipResI.DosyaAB002);
                                return oa;
                            }
                        }
                        IslemEkle(detay, detay.borcuBilgileri);
                        IslemEkle(detay, detay.durumBilgileri);
                        IslemEkle(detay, detay.TaksitBilgileri);
                        IslemEkle(detay, detay.OnBorcBilgileri);
                        IslemEkle(detay, detay.GAKBilgileri);
                        detay.islemler.Sort(islemKiyasla);
                        if (detay.taksitSayisi > 0 && !TNSDateTime.IsNull(detay.taksitIlkTarih))
                        {
                            TNSDateTime tut = new TNSDateTime(detay.islemTarihi);
                            DosyaABTaksit onayliTaksit = (DosyaABTaksit)detay.KayitBul(detay.TaksitBilgileri, false, detay.islemTarihi);
                            detay.YeniTaksitYap(detay.taksitSayisi, onayliTaksit);
                            detay.IslemToplamBul(tut, -1, true, null, form.onayTarihi, -1);
                        }
                        else
                            detay.taksitSayisi = 0;
                        oa.Ekle(detay);
                    }
                }
                catch (Exception ex)
                {
                    OrtakFonksiyonlar.HataDosyaYaz("DosyaAB.Text", $"{detay.iPARDDonemi} - {detay.dosyaAnahtar} okunurken hata:" + ex.Message, false);
                }
            }
            dr.Close();

            ve.VeriErisimKapat();
            oa.objeler.Sort(new DosyaABSirala());
            return oa;
        }

        private static void IslemEkle(DosyaAB okunan, TNSCollection liste)
        {
            foreach (DosyaABParca item in liste)
            {
                okunan.IslemEkle(item);
            }
        }

        //private static DosyaABParca IslemBilgiOl(IDataReader dr)
        //{
        //    DosyaABParca islemBilgi = new DosyaABParca();
        //    islemBilgi.ipardDonemi = VeriErisim.DegerAl(dr, 0, 0);
        //    islemBilgi.dosyaAnahtar = VeriErisim.DegerAl(dr, 1, 0);
        //    islemBilgi.sira = VeriErisim.DegerAl(dr, 2, 0);
        //    islemBilgi.basTarih = VeriErisim.DegerAl(dr, 3);
        //    islemBilgi.bitTarih = VeriErisim.DegerAl(dr, 4);
        //    islemBilgi.kayitDurum = (DosyaABParca.ENUMKayitDurum)VeriErisim.DegerAl(dr, 5, 0);
        //    islemBilgi.kaydeden = VeriErisim.DegerAl(dr, 6, "");
        //    islemBilgi.kayitTarihi.Yaz(VeriErisim.DegerAl(dr, 7));
        //    islemBilgi.onaylayan = VeriErisim.DegerAl(dr, 8, "");
        //    islemBilgi.onayTarihi.Yaz(VeriErisim.DegerAl(dr, 9));
        //    islemBilgi.gerekce = VeriErisim.DegerAl(dr, 10, "");

        //    islemBilgi.tabloKod = (DosyaABParca.ENUMTabloKod)VeriErisim.DegerAl(dr, 11, 0);
        //    islemBilgi.kaydedenAd = VeriErisim.DegerAl(dr, 12, "");
        //    islemBilgi.kaydedenSoyad = VeriErisim.DegerAl(dr, 13, "");
        //    islemBilgi.kaydedenUnvan = VeriErisim.DegerAl(dr, 14, "");
        //    islemBilgi.onaylayanAd = VeriErisim.DegerAl(dr, 15, "");
        //    islemBilgi.onaylayanSoyad = VeriErisim.DegerAl(dr, 16, "");
        //    islemBilgi.onaylayanUnvan = VeriErisim.DegerAl(dr, 17, "");
        //    return islemBilgi;
        //}

        private static void ListeleTaksit(DosyaAB dosya, string okunanXML)
        {
            if (string.IsNullOrWhiteSpace(okunanXML)) return;

            XMLStringDataReader r = new XMLStringDataReader(okunanXML);
            if (r != null)
            {
                while (r.Read())
                {
                    dosya.TaksitBilgileri.Add(BorcluTaksitOku(r));
                }
                r.Close();
            }
        }

        private static void ListeleGAK(DosyaAB dosya, string okunanXML)
        {
            if (string.IsNullOrWhiteSpace(okunanXML)) return;

            XMLStringDataReader r = new XMLStringDataReader(okunanXML);
            if (r != null)
            {
                while (r.Read())
                {
                    dosya.GAKBilgileri.Add(BorcluGAKOku(r));
                }
                r.Close();
            }
        }

        private static void ListeleOB(DosyaAB dosya, string okunanXML)
        {
            if (string.IsNullOrWhiteSpace(okunanXML)) return;

            XMLStringDataReader r = new XMLStringDataReader(okunanXML);
            if (r != null)
            {
                while (r.Read())
                {
                    dosya.OnBorcBilgileri.Add(BorcluOBOku(r));
                }
                r.Close();
            }
        }

        private static void ListeleDurum(DosyaAB dosya, string okunanXML)
        {
            if (string.IsNullOrWhiteSpace(okunanXML)) return;

            XMLStringDataReader r = new XMLStringDataReader(okunanXML);
            if (r != null)
            {
                while (r.Read())
                {
                    dosya.durumBilgileri.Add(BorcluDurumOku(r));
                }
                r.Close();
            }
        }

        private static void ListeleBorclu(DosyaAB dosya, string okunanXML)
        {
            if (string.IsNullOrWhiteSpace(okunanXML)) return;

            XMLStringDataReader r = new XMLStringDataReader(okunanXML);
            if (r != null)
            {
                while (r.Read())
                {
                    dosya.borcuBilgileri.Add(BorcluDetayOku(r));
                }
                r.Close();
            }
        }

        private static void ListeleDetaylar(DosyaAB anaDosya, string okunanXML, double anaDovizKuru)
        {
            anaDosya.borcHareket = new TNSCollection();
            if (string.IsNullOrWhiteSpace(okunanXML)) return;

            XMLStringDataReader r = new XMLStringDataReader(okunanXML);
            if (r != null)
            {
                int yerNo = 1;
                while (r.Read())
                {
                    anaDosya.borcHareket.Add(DetayOku(anaDosya, r, anaDovizKuru, yerNo++));
                }
                r.Close();
                IlgiliKayitlariDegistir(anaDosya);
            }
        }

        private static BorcHareket DetayOku(DosyaAB anaDosya, IDataReader dr, double anaDovizKuru, int yerNo)
        {
            BorcHareket detay = new BorcHareket(anaDosya);
            detay.yerNo = yerNo;
            detay.siraNo = VeriErisim.DegerAl(dr, (int)TABATMBORCHAREKET.ENUMAlanlar.SIRANO, 0);
            detay.tur = (BorcHareket.ENUMHareketTur)VeriErisim.DegerAl(dr, (int)TABATMBORCHAREKET.ENUMAlanlar.TUR, 0);
            detay.tarih = VeriErisim.DegerAl(dr, (int)TABATMBORCHAREKET.ENUMAlanlar.TARIH);
            detay.karar = VeriErisim.DegerAl(dr, (int)TABATMBORCHAREKET.ENUMAlanlar.KARAR, "");
            //detay.ongorulenSilinmeTarihi = VeriErisim.DegerAl(dr, (int)TABATMBORCHAREKET.ENUMAlanlar.ONGORULENSILINMETARIHI);
            detay.miktarAB = OrtakFonksiyonlar.KurusYap(VeriErisim.DegerAl(dr, (int)TABATMBORCHAREKET.ENUMAlanlar.MIKTARAB, 0.0));
            detay.miktarUlusal = OrtakFonksiyonlar.KurusYap(VeriErisim.DegerAl(dr, (int)TABATMBORCHAREKET.ENUMAlanlar.MIKTARULUSAL, 0.0));
            detay.faizAB = OrtakFonksiyonlar.KurusYap(VeriErisim.DegerAl(dr, (int)TABATMBORCHAREKET.ENUMAlanlar.FAIZAB, 0.0));
            detay.faizUlusal = OrtakFonksiyonlar.KurusYap(VeriErisim.DegerAl(dr, (int)TABATMBORCHAREKET.ENUMAlanlar.FAIZULUSAL, 0.0));

            detay.yatirilanAB = OrtakFonksiyonlar.KurusYap(VeriErisim.DegerAl(dr, (int)TABATMBORCHAREKET.ENUMAlanlar.YATIRILANAB, 0.0));
            detay.yatirilanUlusal = OrtakFonksiyonlar.KurusYap(VeriErisim.DegerAl(dr, (int)TABATMBORCHAREKET.ENUMAlanlar.YATIRILANULUSAL, 0.0));
            detay.duzeltimTarihi = VeriErisim.DegerAl(dr, (int)TABATMBORCHAREKET.ENUMAlanlar.DUZELTIMTARIHI);
            detay.kayitDurum = (DosyaABParca.ENUMKayitDurum)VeriErisim.DegerAl(dr, (int)TABATMBORCHAREKET.ENUMAlanlar.DURUM, 0);

            detay.aktarimTarihi = VeriErisim.DegerAl(dr, (int)TABATMBORCHAREKET.ENUMAlanlar.AKTARIMTARIHI);
            detay.aktarimTutari = OrtakFonksiyonlar.KurusYap(VeriErisim.DegerAl(dr, (int)TABATMBORCHAREKET.ENUMAlanlar.AKTARIMTUTARI, 0.0));

            detay.borcunIlgiliOlduguTaksit = VeriErisim.DegerAl(dr, (int)TABATMBORCHAREKET.ENUMAlanlar.ILGILIOLDUGUTAKSIT, "");

            detay.ertelemeAB = VeriErisim.DegerAl(dr, (int)TABATMBORCHAREKET.ENUMAlanlar.ERTELEMEAB, 0.0);
            detay.ertelemeUl = VeriErisim.DegerAl(dr, (int)TABATMBORCHAREKET.ENUMAlanlar.ERTELEMEUL, 0.0);

            detay.kaydeden = VeriErisim.DegerAl(dr, (int)TABATMBORCHAREKET.ENUMAlanlar.KAYDEDEN, "");
            detay.kayitTarihi.Yaz(VeriErisim.DegerAl(dr, (int)TABATMBORCHAREKET.ENUMAlanlar.KAYITTARIHI));
            detay.onaylayan = VeriErisim.DegerAl(dr, (int)TABATMBORCHAREKET.ENUMAlanlar.ONAYLAYAN, "");
            detay.onayTarihi.Yaz(VeriErisim.DegerAl(dr, (int)TABATMBORCHAREKET.ENUMAlanlar.ONAYTARIHI));
            detay.gerekce = VeriErisim.DegerAl(dr, (int)TABATMBORCHAREKET.ENUMAlanlar.GEREKCE, "");
            detay.ilgiliIslemSiraNo = VeriErisim.DegerAl(dr, (int)TABATMBORCHAREKET.ENUMAlanlar.ILGILIISLEMSIRANO, 0);

            int yer = (int)TABATMBORCHAREKET.ENUMAlanlar.ILGILIISLEMSIRANO;
            detay.dovizKuru = VeriErisim.DegerAl(dr, ++yer, 0.0);
            detay.kaydedenAd = VeriErisim.DegerAl(dr, ++yer, "");
            detay.kaydedenSoyad = VeriErisim.DegerAl(dr, ++yer, "");
            detay.kaydedenUnvan = VeriErisim.DegerAl(dr, ++yer, "");
            detay.onaylayanAd = VeriErisim.DegerAl(dr, ++yer, "");
            detay.onaylayanSoyad = VeriErisim.DegerAl(dr, ++yer, "");
            detay.onaylayanUnvan = VeriErisim.DegerAl(dr, ++yer, "");
            return detay;
        }

        internal static Sonuc KaydetBorcHareket(VeriErisim veriErisim, Kullanici kullanan, int iPARDDonemi, int dosyaAnahtar, BorcHareket bh, ENUMDosyaIslemTur islemTur)
        {
            Sonuc s = new Sonuc();
            DosyaAB dosyaOkunan = new DosyaAB();
            dosyaOkunan.iPARDDonemi = iPARDDonemi;
            dosyaOkunan.dosyaAnahtar = dosyaAnahtar;
            if (dosyaOkunan.iPARDDonemi <= 0 || dosyaOkunan.dosyaAnahtar <= 0)
                s.DegerAta("Açılmamış dosyaya işlem yapamazsınız!");

            if (TNSDateTime.IsNull(bh.tarih))
                s.DegerAta("İşlemin tarihini giriniz!");

            if (bh.tur == BorcHareket.ENUMHareketTur.Tanimsiz)
                s.DegerAta("İşlemin türünü giriniz!");

            bool ilgiliOlmali = bh.tur == BorcHareket.ENUMHareketTur.OdeneniAktarma || bh.tur == BorcHareket.ENUMHareketTur.Iade;
            if (islemTur != ENUMDosyaIslemTur.IptalEt)
            {
                if (bh.OdemeTuruIslem)
                {
                    if (bh.tur != BorcHareket.ENUMHareketTur.Iade)
                    {
                        if (bh.yatirilanAB < 0 || bh.yatirilanUlusal < 0)
                            s.DegerAta("Tutarlar negatif olamaz!");
                    }
                    else
                    {
                        if (bh.yatirilanAB > 0 || bh.yatirilanUlusal > 0)
                            s.DegerAta("İade işlemlerinde Tutarlar negatif olmalı!");
                    }
                    if (bh.yatirilanAB == 0 && bh.yatirilanUlusal == 0)
                        s.DegerAta("Lütfen işlem yapılan miktarı giriniz!");
                }

                if (ilgiliOlmali && bh.ilgiliIslemSiraNo <= 0)
                {
                    s.DegerAta("Yapılan ödemenin aktarılması işleminde, ödenen işlemin numarası da olmalıdır!");
                }
                else if (!ilgiliOlmali && bh.ilgiliIslemSiraNo > 0)
                {
                    s.DegerAta("Yalnızca yapılan ödemelerin aktarılması işlemlerinde, ilgili işlem numarası olabilir!");
                }
            }
            if (!s.islemSonuc)
                return s;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectVAN dosyaOkunanVan = Oku(ve, kullanan, dosyaOkunan, false);
            if (!dosyaOkunanVan.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return dosyaOkunanVan.sonuc;
            }
            dosyaOkunan = (DosyaAB)dosyaOkunanVan.o;
            if (dosyaOkunan == null)
            {
                ve.VeriErisimKapat();
                s.DegerAta("Verilen kayıt bulunamadı!");
                return s;
            }
            s = Yetki.DosyaYazmaOnaylamaHakkiVarMi(kullanan, dosyaOkunan.saymanlikKod, dosyaOkunan.birimKod, ENUMDosyaIslemTur.Kaydet);
            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }
            BorcHareket bhOkunan;
            TABATMBORCHAREKET tab = new TABATMBORCHAREKET(ve);
            TABATMDOSYAAB tabDosya = new TABATMDOSYAAB(ve);
            if (bh.siraNo > 0)
            {
                bhOkunan = dosyaOkunan.HareketBul(bh.siraNo);
                if (bhOkunan == null)
                    s.DegerAta("Güncellenmek istenilen kayıt bulunamadı!");
                else
                {
                    bool hataVar = false;

                    if (ilgiliOlmali && islemTur == ENUMDosyaIslemTur.IptalEt)
                    {
                        if (bh.ilgiliIslemSiraNo == 0)
                            hataVar = true;
                        else
                        {
                            BorcHareket bhIlgili = dosyaOkunan.HareketBul(bh.ilgiliIslemSiraNo);
                            if (bhIlgili == null)
                                hataVar = true;
                            else
                            {
                                if (bhIlgili.tur == BorcHareket.ENUMHareketTur.Silme || bhIlgili.tur == BorcHareket.ENUMHareketTur.Duzeltme || bhIlgili.tur == BorcHareket.ENUMHareketTur.Terkin || bhIlgili.tur == BorcHareket.ENUMHareketTur.FaizHesabi || bhIlgili.tur == BorcHareket.ENUMHareketTur.FaizHesabiIslem)
                                    hataVar = true;
                            }
                        }
                    }

                    if (!hataVar)
                        bhOkunan.IslemKontrol(s, islemTur, kullanan.kullaniciKodu);
                }
            }
            else
            {
                bhOkunan = null;
                if (islemTur != ENUMDosyaIslemTur.Kaydet)
                {
                    s.DegerAta("Daha önce kaydedilmemiş kayıtların durumu değiştirilemez!");
                }
            }

            BorcHareket bhFaizIslem = null;
            if ((bh.tur == BorcHareket.ENUMHareketTur.Odeme || bh.tur == BorcHareket.ENUMHareketTur.Mahsup || bh.tur == BorcHareket.ENUMHareketTur.Silme) && islemTur != ENUMDosyaIslemTur.Kaydet)
            {
                bhFaizIslem = dosyaOkunan.HareketBul(BorcHareket.ENUMHareketTur.FaizHesabiIslem, bhOkunan.tarihDT);
                //if (bhFaizIslem == null)
                //{
                //    s.DegerAta("İlgili işlemin faiz hesabı bulunamadığından işlem iptal edildi, lütfen önce kaydedeniz!");
                //}
            }

            if (s.islemSonuc)
            {
                ve.BeginTransaction();
                if (islemTur == ENUMDosyaIslemTur.Kaydet)
                {
                    if (bh.tur == BorcHareket.ENUMHareketTur.Odeme || bh.tur == BorcHareket.ENUMHareketTur.Mahsup || bh.tur == BorcHareket.ENUMHareketTur.Silme)
                    {
                        s = DosyaFaiziHesapla(ve, kullanan, dosyaOkunan, bh.tarih, false);
                    }

                    if (s.islemSonuc)
                    {
                        if (bh.siraNo == 0)
                        {
                            bh.siraNo = tab.AnahtarBul(dosyaOkunan);
                            if (!string.IsNullOrWhiteSpace(ve.hata))
                                s.DegerAta(ve.hata);
                            else
                                s = tab.Ekle(dosyaOkunan, bh, kullanan.kullaniciKodu);
                        }
                        else
                        {
                            s = tab.Guncelle(dosyaOkunan, bh, kullanan.kullaniciKodu);
                        }
                        s.anahtar = bh.siraNo.ToString();
                    }
                }
                else
                {
                    TNSDateTime islemZamani = new TNSDateTime(DateTime.Now);
                    if (islemTur == ENUMDosyaIslemTur.YeniYap) islemTur = ENUMDosyaIslemTur.Kaydet;
                    s = tab.DurumDegistir(islemZamani, dosyaOkunan.iPARDDonemi, dosyaOkunan.dosyaAnahtar, bhOkunan.siraNo, (int)islemTur, kullanan.kullaniciKodu);
                    if (s.islemSonuc && bhFaizIslem != null)
                        s = tab.DurumDegistir(islemZamani, dosyaOkunan.iPARDDonemi, dosyaOkunan.dosyaAnahtar, bhFaizIslem.siraNo, (int)islemTur, kullanan.kullaniciKodu);

                    s.anahtar = bhOkunan.siraNo.ToString();
                }
            }
            if (s.islemSonuc)
            {
                ve.Commit();
                TarihceYaz(ve, kullanan, dosyaOkunan, bh, TarihceAB.TarihceIslemKodlar.KaydetHareket);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        internal static ObjectArray ListeleBorcHareket(VeriErisim veriErisim, Kullanici kullanan, int iPARDDonemi, int dosyaAnahtar, BorcHareket bh, ENUMDosyaIslemTur islemTur, out DosyaAB dosyaOkunan)
        {
            ObjectArray oa = new ObjectArray();
            dosyaOkunan = new DosyaAB();
            dosyaOkunan.iPARDDonemi = iPARDDonemi;
            dosyaOkunan.dosyaAnahtar = dosyaAnahtar;
            if (dosyaOkunan.iPARDDonemi <= 0 || dosyaOkunan.dosyaAnahtar <= 0)
            {
                oa.sonuc.DegerAta("Açılmamış dosyaya işlem yapamazsınız!");
                return oa;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectVAN dosyaOkunanVan = Oku(ve, kullanan, dosyaOkunan, false);
            if (!dosyaOkunanVan.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                oa.sonuc = dosyaOkunanVan.sonuc;
                return oa;
            }
            dosyaOkunan = (DosyaAB)dosyaOkunanVan.o;
            if (dosyaOkunan == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta("Verilen kayıt bulunamadı!");
                return oa;
            }
            if (islemTur == ENUMDosyaIslemTur.Listeleme)
                oa.sonuc = Yetki.DosyaOkumaHakkiVarMi(kullanan, dosyaOkunan.saymanlikKod, dosyaOkunan.birimKod);
            else
                oa.sonuc = Yetki.DosyaYazmaOnaylamaHakkiVarMi(kullanan, dosyaOkunan.saymanlikKod, dosyaOkunan.birimKod, islemTur);
            if (!oa.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return oa;
            }

            foreach (BorcHareket item in dosyaOkunan.borcHareket)
            {
                if (item.siraNo == bh.siraNo)
                    oa.objeler.Add(item);
            }
            ve.VeriErisimKapat();
            return oa;
        }

        private static void IlgiliKayitlariDegistir(DosyaAB anaDosya)
        {
            foreach (BorcHareket item in anaDosya.borcHareket)
            {
                if (item.ilgiliIslemSiraNo > 0)
                {
                    BorcHareket ilgili = anaDosya.HareketBul(item.ilgiliIslemSiraNo);
                    if (ilgili != null)
                    {
                        if (item.tur == BorcHareket.ENUMHareketTur.OdeneniAktarma)
                        {
                            ilgili.aktarimTarihi.Yaz(item.aktarimTarihi);
                            if (item.aktarimTutari > 0)
                                ilgili.aktarimTutari = item.aktarimTutari;
                            else
                                ilgili.aktarimTutari = item.miktarAB + item.faizAB;
                            item.dovizKuru = ilgili.dovizKuru;
                        }
                        else if (item.tur == BorcHareket.ENUMHareketTur.Iade)
                        {
                            item.dovizKuru = ilgili.dovizKuru;
                        }
                    }
                }
            }
        }

        internal static Sonuc SilBorcHareket(VeriErisim veriErisim, Kullanici kullanan, int iPARDDonemi, int dosyaAnahtar, int siraNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            BorcHareket bh = new BorcHareket(null);
            bh.siraNo = siraNo;
            DosyaAB dosyaOkunan;
            ObjectArray oa = ListeleBorcHareket(ve, kullanan, iPARDDonemi, dosyaAnahtar, bh, ENUMDosyaIslemTur.Kaydet, out dosyaOkunan);
            if (oa.sonuc.islemSonuc)
            {
                if (oa.ObjeSayisi < 1)
                    oa.sonuc.DegerAta("Aranan kayıt bulunamadı!");
                else if (oa.ObjeSayisi > 1)
                    oa.sonuc.DegerAta("Birden fazla kayıt bulundu!");
                else
                {
                    bh = (BorcHareket)oa.objeler[0];
                    if (bh.kayitDurum == DosyaABParca.ENUMKayitDurum.ONAYLI)
                    {
                        oa.sonuc.DegerAta("Onaylanmış kayıtları silemezsiniz!");
                    }
                }
            }
            if (!oa.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return oa.sonuc;
            }

            TABATMBORCHAREKET tab = new TABATMBORCHAREKET(ve);
            ve.BeginTransaction();
            Sonuc s = tab.Sil(iPARDDonemi, dosyaAnahtar, siraNo);
            if (s.islemSonuc)
            {
                ve.Commit();
                TarihceYaz(ve, kullanan, dosyaOkunan, bh, TarihceAB.TarihceIslemKodlar.DetaySil);
            }
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        private static void TarihceYaz(VeriErisim veriErisim, Kullanici kullanan, DosyaAB dosyaOkunan, BorcHareket bh, TarihceAB.TarihceIslemKodlar islemKod)
        {
            TarihceYaz(veriErisim, kullanan, dosyaOkunan.iPARDDonemi, dosyaOkunan.dosyaAnahtar, bh.siraNo, bh.ToString(), islemKod);
        }

        //private static void TarihceYaz(VeriErisim veriErisim, Kullanici kullanan, DosyaAB form, DosyaAB dosyaOkunan, TarihceAB.TarihceIslemKodlar islemKod)
        //{
        //    TarihceYaz(veriErisim, kullanan, form.iPARDDonemi, form.dosyaAnahtar, 0, form.ToString(), islemKod);
        //}

        private static void TarihceYaz(VeriErisim veriErisim, Kullanici kullanan, int iPARDDonemi, int dosyaAnahtar, int hareketNo, string metin, TarihceAB.TarihceIslemKodlar islemKod)
        {
            TABATMDOSYAAB tabTar = new TABATMDOSYAAB(veriErisim);
            tabTar.TarihceEkle(kullanan.kullaniciKodu, iPARDDonemi, dosyaAnahtar, hareketNo, metin, (int)islemKod);
        }

        internal static ObjectArray TarihceListele(VeriErisim veriErisim, Kullanici kullanan, int iPARDDonemi, int dosyaAnahtar)
        {
            ObjectArray oa = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMDOSYAAB tabTar = new TABATMDOSYAAB(ve);

            IDataReader dr = tabTar.TarihceListele(iPARDDonemi, dosyaAnahtar);
            if (dr != null)
            {
                while (dr.Read())
                {
                    TarihceAB form = new TarihceAB();
                    form.zaman = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarTARIHCE.ZAMAN);
                    form.islemiYapan = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarTARIHCE.ISLEMIYAPAN, "");
                    form.islemKod = (TarihceAB.TarihceIslemKodlar)VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarTARIHCE.ISLEMKOD, 0);
                    form.metin = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarTARIHCE.METIN, "");
                    form.mernis = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarTARIHCE.ISLEMIYAPAN + 1, "");
                    form.ad = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarTARIHCE.ISLEMIYAPAN + 2, "");
                    form.soyad = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarTARIHCE.ISLEMIYAPAN + 3, "");
                    oa.Ekle(form);
                }
                dr.Close();
            }
            else
                oa.sonuc.DegerAta(ve.hata);
            ve.VeriErisimKapat();
            return oa;
        }

        internal static Sonuc DosyaFaiziHesapla(VeriErisim veriErisim, Kullanici kullanan, int iPARDDonemi, int dosyaAnahtar, TNSDateTime hesapTarihi)
        {
            Sonuc s = new Sonuc();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            DosyaAB form = new DosyaAB();
            form.iPARDDonemi = iPARDDonemi;
            form.dosyaAnahtar = dosyaAnahtar;
            ObjectVAN ov = Oku(veriErisim, kullanan, form, false);
            if (ov.sonuc.islemSonuc && ov.o != null)
            {
                s = DosyaFaiziHesapla(veriErisim, kullanan, (DosyaAB)ov.o, hesapTarihi, true);
            }
            else
                s = ov.sonuc;
            ve.VeriErisimKapat();
            return s;
        }

        private static Sonuc DosyaFaiziHesapla(VeriErisim ve, Kullanici kullanan, DosyaAB form, TNSDateTime hesapTarihi, bool AySonu)
        {
            Sonuc s = new Sonuc();
            if (hesapTarihi.Oku() > DateTime.Now) return s;
            if (TNSDateTime.IsNull(form.odemeSonTarihi)) return s;
            if (hesapTarihi.Oku() <= form.odemeSonTarihiDT) return s;
            if (AySonu && !TNSDateTime.IsNull(form.faizinDurdurulmaTarihi) && hesapTarihi.Oku() >= form.faizinDurdurulmaTarihi.Oku()) return s;

            TNSDateTime dtHesapTarihi = new TNSDateTime(hesapTarihi);
            BorcHareket detay = new BorcHareket(form);
            detay.tarih = hesapTarihi;
            detay.tur = AySonu ? BorcHareket.ENUMHareketTur.FaizHesabi : BorcHareket.ENUMHareketTur.FaizHesabiIslem;
            //detay.dovizKuru = dovizKuru;
            TABATMBORCHAREKET tabATMBORCHAREKET = new TABATMBORCHAREKET(ve);

            int eskiAnahtar = 0;
            IDataReader dr = tabATMBORCHAREKET.EskiAnahtarBul(form, detay);
            if (dr != null && string.IsNullOrWhiteSpace(ve.hata))
            {
                while (dr.Read())
                {
                    int anahtar = VeriErisim.DegerAl(dr, 0, 0);
                    int durum = VeriErisim.DegerAl(dr, 1, 0);
                    if (durum == 5)
                    {
                        dr.Close();
                        return s;
                    }
                    else
                        eskiAnahtar = anahtar;
                }
            }
            else
                s.DegerAta("Veri tabanı hatası:" + ve.hata);

            if (dr != null)
                dr.Close();

            if (!s.islemSonuc)
            {
                return s;
            }

            form.IslemToplamBul(dtHesapTarihi, -1, false, null, new TNSDateTime(DateTime.Now), -1);
            form.FaizHesapla(hesapTarihi);
            if (form.kalanMiktarToplam > 0)
            {
                detay.faizAB = form.faizMiktarAB;
                detay.faizUlusal = form.faizMiktarUlusal;
                TABATMDOSYAAB tabATMDOSYAAB = new TABATMDOSYAAB(ve);
                ve.BeginTransaction();

                if (eskiAnahtar > 0)
                {
                    detay.siraNo = eskiAnahtar;
                    s = tabATMBORCHAREKET.Guncelle(form, detay, kullanan.kullaniciKodu);
                }
                else if (eskiAnahtar == 0)
                {
                    detay.siraNo = tabATMBORCHAREKET.AnahtarBul(form);
                    if (!string.IsNullOrWhiteSpace(ve.hata)) s.DegerAta(ve.hata);
                    if (s.islemSonuc)
                        s = tabATMBORCHAREKET.Ekle(form, detay, kullanan.kullaniciKodu);
                }
                if (s.islemSonuc)
                    s.anahtar = detay.siraNo.ToString();
                if (s.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }

            return s;
        }

        internal static Sonuc DosyalarinFaiziniHesapla(VeriErisim veriErisim, Kullanici kullanan, TNSDateTime hesapTarihi, DosyaAB form, int hesaplamaTur)
        {
            Sonuc s = Yetki.DosyaYazmaOnaylamaHakkiVarMi(kullanan, 1, "000", hesaplamaTur == 1 ? ENUMDosyaIslemTur.Kaydet : ENUMDosyaIslemTur.Onayla);
            if (!s.islemSonuc)
                return s;

            double dovizKuru = DovizKuruBulOncAy(kullanan, hesapTarihi);
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            if (hesaplamaTur == 1)
            {
                ObjectArray oa = Listele(ve, kullanan, form, true);
                if (!oa.sonuc.islemSonuc)
                {
                    ve.VeriErisimKapat();
                    return oa.sonuc;
                }
                ve.BeginTransaction();
                foreach (DosyaAB item in oa.objeler)
                {
                    s = DosyaFaiziHesapla(ve, kullanan, item, hesapTarihi, true);
                    if (!s.islemSonuc) break;
                }
                if (s.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }
            else if (hesaplamaTur == 2 || hesaplamaTur == 3)
            {
                TABATMBORCHAREKET tab = new TABATMBORCHAREKET(ve);

                ve.BeginTransaction();
                s = tab.DurumDegistirToplu(hesapTarihi, BorcHareket.ENUMHareketTur.FaizHesabi, hesaplamaTur == 2 ? ENUMDosyaIslemTur.Onayla : ENUMDosyaIslemTur.IptalEt, kullanan.kullaniciKodu, new TNSDateTime(DateTime.Now));
                if (s.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }
            ve.VeriErisimKapat();
            return s;
        }

        internal static double DovizKuruBulOncAy(Kullanici kullanan, TNSDateTime kurTarihi)
        {
            int ay = kurTarihi.Ay;
            int yil = kurTarihi.Yil;
            ay--;
            if (ay == 0)
            {
                ay = 12;
                yil--;
            }
            double sonuc = 0;

            TNS.HRC.IHRCServis servisHrc = TNS.HRC.Arac.Tanimla();
            HRC.MerkezBankasiKurSinif kriter = new HRC.MerkezBankasiKurSinif();
            kriter.kurTarih = new TNSDateTime(new DateTime(yil, ay, 1));
            kriter.yakinGunBul = false;
            kriter.satirlar = new HRC.MerkezBankasiKurDetaySinif[1];
            kriter.satirlar[0] = new HRC.MerkezBankasiKurDetaySinif();
            kriter.satirlar[0].paraBirim.kisaltma = "EUR";
            kriter = servisHrc.MerkezBankKurOku(kullanan, kriter);
            if (kriter != null)
            {
                sonuc = Convert.ToDouble(kriter.KurBul("EUR", "ALIS", -1));
            }
            else
                sonuc = 0;
            return sonuc;
        }

        internal static ObjectArray DuyuruListesiAl(VeriErisim veriErisim, Kullanici kullanan, string kullaniciTipleri)
        {
            ObjectArray oa = new ObjectArray();
            if (!TNS.DEG.Arac.TKDKMi())
            {
                return oa;
            }
            TNSDateTime dt = new TNSDateTime(DateTime.Now);
            dt.AddDays(30);
            dt.AddYears(-2);

            DosyaAB dosyaKriter = new DosyaAB();
            dosyaKriter.geriAlimSurecAsamalari = "1,2,8,10,12";
            dosyaKriter.borcTurleri = "1,3,4";
            dosyaKriter.geriAlimKarariTarihAralik = "-" + dt.ToString();
            dt.AddYears(2);
            ObjectArray dosyalar = Listele(null, kullanan, dosyaKriter, true);
            Duyuru duy = new Duyuru();
            duy.kod = -1;
            duy.onem = 1;
            duy.tarih = new TNSDateTime(DateTime.Now);
            duy.gizle = ENUMGizle.GORUNUR;
            //duy.KullaniciTipleriOl(VeriErisim.DegerAl(r, 5, ""));
            duy.pencerede = ENUMPencerede.GORUNUR;

            if (dosyalar.sonuc.islemSonuc && dosyalar.ObjeSayisi > 0)
            {
                duy.konu = dt.ToString() + " tarihinden önce silinmesi gereken dosyaların listesi:";
                StringBuilder html = new StringBuilder();
                html.Append(duy.konu);
                html.Append("</p><table border='1' cellspacing='0' cellpadding='0' class='yaziAlan'><tr>");
                html.Append(HucreOl("Firma", true));
                html.Append(HucreOl("G.A.K. Tarihi", true));
                html.Append(HucreOl("Kalan AB.", true));
                html.Append("</tr>");
                int say = 0;
                foreach (DosyaAB item in dosyalar.objeler)
                {
                    if (!item.silmeVar && item.kalanMiktarAB > 0 && (
                        item.geriAlimSurecAsamasi == DosyaAB.ENUMDosyaDurum.GERIALIM ||
                        item.geriAlimSurecAsamasi == DosyaAB.ENUMDosyaDurum.YASALTAKIP ||
                        item.geriAlimSurecAsamasi == DosyaAB.ENUMDosyaDurum.SILINMISBORCLARDEVAMEDENADIMLAR ||
                        item.geriAlimSurecAsamasi == DosyaAB.ENUMDosyaDurum.TAKSITLENDIRILMIS ||
                        item.geriAlimSurecAsamasi == DosyaAB.ENUMDosyaDurum.GERIODEMEKISMEN)
                       )
                    {
                        html.Append("<tr>");
                        html.Append(HucreOl(item.borcluAd));
                        html.Append(HucreOl(item.geriAlimKarariTarihi.ToString(), false, true));
                        html.Append(HucreOl(item.kalanMiktarAB.ToString("#,##0"), false, true));
                        html.Append("</tr>");
                        say++;
                    }
                }
                html.Append("</table>");
                duy.duyuru = html.ToString();
                if (say > 0)
                    oa.Ekle(duy);
            }
            else if (!dosyalar.sonuc.islemSonuc)
            {
                //OrtakFonksiyonlar.HataDosyaYaz("ATMDuyuru.txt", dosyalar.sonuc.hataStr, false);
            }
            return oa;
        }

        private static string HucreOl(string veri, bool th = false, bool saga = false)
        {
            string ek;
            if (th) ek = "th";
            else ek = "td";
            if (saga) ek += " style='text-align:right'";
            return "<" + ek + ">" + veri + "</" + ek + ">";
        }

        private static void DurumIslemler(Sonuc s, TABATMDOSYAAB tab, ENUMDosyaIslemTur islemTur, DosyaABParca form, string tabloAd, string kullaniciAd)
        {
            TNSDateTime islemZamani = new TNSDateTime(DateTime.Now);
            if (islemTur == ENUMDosyaIslemTur.YeniYap)
                s = tab.YeniYap(islemZamani, form.ipardDonemi, form.dosyaAnahtar, form.sira, tabloAd);
            else
            {
                if (islemTur == ENUMDosyaIslemTur.Onayla)
                    s = tab.PasifYap(islemZamani, form.ipardDonemi, form.dosyaAnahtar, form.sira, tabloAd);
                if (s.islemSonuc)
                    s = tab.DurumDegistir(islemZamani, form.ipardDonemi, form.dosyaAnahtar, form.sira, (int)islemTur, tabloAd, kullaniciAd);
            }
        }

        #region "borçlu"
        public static ObjectVAN ABBorcluOku(VeriErisim veriErisim, Kullanici kullanan, ABBorclu form)
        {
            ObjectVAN ov = new ObjectVAN();
            ObjectArray s = ABBorcluListele(veriErisim, kullanan, form);
            ov.sonuc = s.sonuc;
            if (s != null && s.ObjeSayisi > 0) ov.o = s[0];
            return ov;
        }

        private static ABBorclu BorcluDetayOku(IDataReader dr)
        {
            ABBorclu detay = new ABBorclu();
            detay.tabloKod = DosyaABParca.ENUMTabloKod.BORCLU;
            detay.ipardDonemi = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarBORCLU.IPARDDONEMI, 0);
            detay.dosyaAnahtar = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarBORCLU.DOSYAANAHTAR, 0);
            detay.sira = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarBORCLU.SIRA, 0);
            detay.basTarih.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarBORCLU.BASTARIH));
            detay.bitTarih.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarBORCLU.BITTARIH));
            detay.kayitDurum = (DosyaABParca.ENUMKayitDurum)VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarBORCLU.DURUM, 0);

            detay.defterSirasi = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarBORCLU.DEFTERSIRASI, "");
            detay.tedbir = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarBORCLU.TEDBIR, "");
            detay.altTedbir = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarBORCLU.ALTTEDBIR, "");
            detay.butceKalemi = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarBORCLU.BUTCEKALEMI, "");
            detay.saymanlikKod = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarBORCLU.SAYMANLIKKOD, 0);
            detay.birimKod = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarBORCLU.BIRIMKOD, "");
            detay.borcluKod = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarBORCLU.BORCLUKOD, 0);
            detay.basvuruKayitNumarasi = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarBORCLU.BASVURUKAYITNUMARASI, "");
            detay.sozlesmeNumarasi = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarBORCLU.SOZLESMENUMARASI, "");

            detay.kaydeden = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarBORCLU.KAYDEDEN, "");
            detay.kayitTarihi.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarBORCLU.KAYITTARIHI));
            detay.onaylayan = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarBORCLU.ONAYLAYAN, "");
            detay.onayTarihi.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarBORCLU.ONAYTARIHI));
            detay.gerekce = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarBORCLU.GEREKCE, "");

            int alanSira = (int)TABATMDOSYAAB.ENUMAlanlarBORCLU.GEREKCE + 1;

            detay.kimlikNo = VeriErisim.DegerAl(dr, alanSira++, "");
            detay.borcluAd = VeriErisim.DegerAl(dr, alanSira++, "");
            detay.saymanlikAd = VeriErisim.DegerAl(dr, alanSira++, "");
            detay.birimAd = VeriErisim.DegerAl(dr, alanSira++, "");

            detay.kaydedenAd = VeriErisim.DegerAl(dr, alanSira++, "");
            detay.kaydedenSoyad = VeriErisim.DegerAl(dr, alanSira++, "");
            detay.kaydedenUnvan = VeriErisim.DegerAl(dr, alanSira++, "");

            detay.onaylayanAd = VeriErisim.DegerAl(dr, alanSira++, "");
            detay.onaylayanSoyad = VeriErisim.DegerAl(dr, alanSira++, "");
            detay.onaylayanUnvan = VeriErisim.DegerAl(dr, alanSira++, "");
            return detay;
        }

        /// <summary>
        /// OnBorc listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">OnBorc yapısında OnBorc:  bilgilerini tutan değişken</param>
        public static ObjectArray ABBorcluListele(VeriErisim veriErisim, Kullanici kullanan, ABBorclu form)
        {
            ObjectArray oa = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMDOSYAAB tabATMONBORC = new TABATMDOSYAAB(ve);

            IDataReader dr = tabATMONBORC.BorcluListele(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {

                oa.Ekle(BorcluDetayOku(dr));
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        /*
        /// <summary>
        /// DosyaAB nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DosyaAB yapısında DosyaAB: Borç Dosyası bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, DosyaAB form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form, 1))
            {
                s.DegerAta(AlacakTakipResI.DosyaAB001);
                return s;
            }

            s = AlanlarGecerliMi(kullanan, form, s);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMDOSYAAB tab = new TABATMDOSYAAB(ve);
            DosyaAB dosyaOkunan = null;
            if (form.dosyaAnahtar > 0)
            {
                ObjectVAN dosyaOkunanVan = Oku(ve, kullanan, form, false);
                if (!dosyaOkunanVan.sonuc.islemSonuc)
                {
                    ve.VeriErisimKapat();
                    return dosyaOkunanVan.sonuc;
                }
                dosyaOkunan = (DosyaAB)dosyaOkunanVan.o;
                if (dosyaOkunan == null)
                {
                    ve.VeriErisimKapat();
                    s.DegerAta("Verilen kayıt bulunamadı!");
                    return s;
                }
            }

            ve.BeginTransaction();

            if (dosyaOkunan == null)
            {
                form.dosyaAnahtar = tab.AnahtarBul(form);
                if (!string.IsNullOrWhiteSpace(ve.hata))
                    s.DegerAta(ve.hata);
                else
                    s = tab.Ekle(form);
            }
            else
            {
                s = tab.Guncelle(form);
            }

            if (s.islemSonuc)
            {
                ve.Commit();
                s.anahtar = form.dosyaAnahtar.ToString();
                TarihceYaz(ve, kullanan, form, dosyaOkunan, TarihceAB.TarihceIslemKodlar.Kaydet);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }
        */

        public static Sonuc ABBorcluKaydet(VeriErisim veriErisim, Kullanici kullanan, ABBorclu form, ENUMDosyaIslemTur islemTur)
        {
            Sonuc s = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            DosyaAB dosyaAB = null;

            bool yeniBorclu = false;
            if (form.dosyaAnahtar <= 0)
            {
                yeniBorclu = true;
                dosyaAB = new DosyaAB(form);
            }
            else
            {
                ObjectVAN ovDosyaAB = Oku(ve, kullanan, new DosyaAB(form.ipardDonemi, form.dosyaAnahtar), true);
                if (ovDosyaAB.sonuc.islemSonuc && ovDosyaAB.o != null)
                {
                    dosyaAB = (DosyaAB)ovDosyaAB.o;
                }
                else
                {
                    if (ovDosyaAB.sonuc.islemSonuc)
                        s.DegerAta("Dosya bulunamadı!");
                    else
                        s.DegerAta(ovDosyaAB.sonuc.hataStr);
                    ve.VeriErisimKapat();
                    return s;
                }
            }

            if (!YazmaYetkisiVarMi(kullanan, dosyaAB, ENUMDosyaIslemTur.Kaydet))
            {
                s.DegerAta(AlacakTakipResI.DosyaAB001);
                ve.VeriErisimKapat();
                return s;
            }

            s = AlanlarGecerliMiBorclu(kullanan, form, dosyaAB, s);
            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            TABATMDOSYAAB tabATMABBorclu = new TABATMDOSYAAB(ve);

            ABBorclu okunan = null;

            if (form.sira > 0)
            {
                ObjectVAN ov = ABBorcluOku(ve, kullanan, form);
                if (ov.sonuc.islemSonuc && ov.o != null)
                {
                    okunan = (ABBorclu)ov.o;
                }
                else
                {
                    if (ov.sonuc.islemSonuc)
                        ov.sonuc.DegerAta("Kayıt bulunamadı!");
                    s = ov.sonuc;
                }
            }
            if (okunan == null && islemTur != ENUMDosyaIslemTur.Kaydet)
            {
                s.DegerAta("Onaylama, iptal etme ve güncelleme işlemleri mevcut kayıtlar üzerinde yapılabilir!");
            }
            if (okunan != null)
            {
                okunan.IslemKontrol(s, islemTur, kullanan.kullaniciKodu);
            }

            if (s.islemSonuc)
            {
                ve.BeginTransaction();
                if (islemTur == ENUMDosyaIslemTur.Kaydet)
                {
                    if (yeniBorclu)
                    {
                        form.dosyaAnahtar = tabATMABBorclu.AnahtarBul(dosyaAB);
                        dosyaAB.dosyaAnahtar = tabATMABBorclu.AnahtarBul(dosyaAB);
                        if (!string.IsNullOrWhiteSpace(ve.hata))
                            s.DegerAta(ve.hata);
                        else
                            s = tabATMABBorclu.Ekle(dosyaAB);
                    }

                    if (form.sira == 0)
                    {
                        int say = tabATMABBorclu.YeniKayitSayisiBul(form.ipardDonemi, form.dosyaAnahtar, TABATMDOSYAAB.TABLOADBORCLU);
                        if (say > 0)
                            s.DegerAta("Zaten yeni durumda olan bir kayıt olduğundan, bu işlemi kaydedemezsiniz!");
                        form.sira = tabATMABBorclu.SiraNoBul(form.ipardDonemi, form.dosyaAnahtar, TABATMDOSYAAB.TABLOADBORCLU);
                        if (!string.IsNullOrWhiteSpace(ve.hata))
                            s.DegerAta("Veri tabanından sıra no alınamadı!");
                        if (s.islemSonuc)
                            s = tabATMABBorclu.EkleBorclu(form, kullanan.KullaniciKodu);
                    }
                    else
                        s = tabATMABBorclu.GuncelleBorclu(form, kullanan.kullaniciKodu);
                }
                else if (islemTur == ENUMDosyaIslemTur.OzelGuncelle)
                {
                    s = tabATMABBorclu.GuncelleBorcluOzel(form, kullanan.kullaniciKodu);
                }
                else
                {
                    DurumIslemler(s, tabATMABBorclu, islemTur, form, TABATMDOSYAAB.TABLOADBORCLU, kullanan.kullaniciKodu);
                }

                if (s.islemSonuc)
                {
                    s.anahtar = form.ipardDonemi + "-" + form.dosyaAnahtar + "-" + form.sira;
                    ve.Commit();
                }
                else
                    ve.Rollback();
            }
            ve.VeriErisimKapat();
            return s;
        }
        #endregion

        #region "Ön Borç"
        /// <summary>
        /// OnBorc nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">OnBorc yapısında OnBorc:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc OnBorcKaydet(VeriErisim veriErisim, Kullanici kullanan, OnBorc form, ENUMDosyaIslemTur islemTur)
        {
            Sonuc s = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectVAN ovDosyaAB = Oku(ve, kullanan, new DosyaAB(form.ipardDonemi, form.dosyaAnahtar), true);
            DosyaAB dosyaAB = null;
            if (ovDosyaAB.sonuc.islemSonuc && ovDosyaAB.o != null)
            {
                dosyaAB = (DosyaAB)ovDosyaAB.o;
            }
            else
            {
                if (ovDosyaAB.sonuc.islemSonuc)
                    s.DegerAta("Dosya bulunamadı!");
                else
                    s.DegerAta(ovDosyaAB.sonuc.hataStr);
                ve.VeriErisimKapat();
                return s;
            }

            if (!YazmaYetkisiVarMi(kullanan, dosyaAB, islemTur))
            {
                s.DegerAta("Yazma Yetkisi Yok");
                return s;
            }

            s = AlanlarGecerliMiOnBorc(kullanan, form, dosyaAB, s);
            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            TABATMDOSYAAB tabATMONBORC = new TABATMDOSYAAB(ve);
            OnBorc okunan = null;
            if (form.sira > 0)
            {
                ObjectVAN ov = OnBorcOku(ve, kullanan, form);
                if (ov.sonuc.islemSonuc && ov.o != null)
                {
                    okunan = (OnBorc)ov.o;
                }
                else
                {
                    if (ov.sonuc.islemSonuc)
                        ov.sonuc.DegerAta("Kayıt bulunamadı!");
                    s = ov.sonuc;
                }
            }
            if (okunan == null && islemTur != ENUMDosyaIslemTur.Kaydet)
            {
                s.DegerAta("Onaylama, iptal etme ve güncelleme işlemleri mevcut kayıtlar üzerinde yapılabilir!");
            }
            if (okunan != null)
            {
                okunan.IslemKontrol(s, islemTur, kullanan.kullaniciKodu);
            }

            if (s.islemSonuc)
            {
                ve.BeginTransaction();
                if (islemTur == ENUMDosyaIslemTur.Kaydet)
                {
                    if (form.OBKaldirma)
                    {
                        form.onBorcTarihi.isNull = true;
                        form.onBorcSayi = "";
                        form.vakaNumarasi = "";
                        form.avans = false;
                        form.borcTuru = DosyaAB.BorcTurleri.Tanimsiz;
                        form.borcunKodu = "";
                        form.onBorcDovizKuru = 0;
                        form.onBorcMiktarAB = 0;
                        form.onBorcMiktarUlusal = 0;
                        form.aktarimTarihi.isNull = true;
                    }

                    if (form.sira == 0)
                    {
                        int say = tabATMONBORC.YeniKayitSayisiBul(form.ipardDonemi, form.dosyaAnahtar, TABATMDOSYAAB.TABLOADOB);
                        if (say > 0)
                            s.DegerAta("Zaten yeni durumda olan bir kayıt olduğundan, bu işlemi kaydedemezsiniz!");
                        form.sira = tabATMONBORC.SiraNoBul(form.ipardDonemi, form.dosyaAnahtar, TABATMDOSYAAB.TABLOADOB);
                        if (!string.IsNullOrWhiteSpace(ve.hata))
                            s.DegerAta("Veri tabanından sıra no alınamadı!");
                        if (s.islemSonuc)
                            s = tabATMONBORC.EkleOB(form, kullanan.KullaniciKodu);
                    }
                    else
                        s = tabATMONBORC.GuncelleOB(form, kullanan.kullaniciKodu);
                }
                else if (islemTur == ENUMDosyaIslemTur.OzelGuncelle)
                {
                    s = tabATMONBORC.GuncelleOBOzel(form, kullanan.kullaniciKodu);
                }
                else
                {
                    DurumIslemler(s, tabATMONBORC, islemTur, form, TABATMDOSYAAB.TABLOADOB, kullanan.kullaniciKodu);
                }

                if (s.islemSonuc)
                {
                    ve.Commit();
                    s.anahtar = form.sira.ToString();
                }
                else
                    ve.Rollback();
            }
            ve.VeriErisimKapat();
            return s;
        }
        /// <summary>
        /// OnBorc nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">OnBorc yapısında OnBorc:  bilgilerini tutan değişken</param>
        public static ObjectVAN OnBorcOku(VeriErisim veriErisim, Kullanici kullanan, OnBorc form)
        {
            ObjectVAN ov = new ObjectVAN();
            ObjectArray s = OnBorcListele(veriErisim, kullanan, form);
            ov.sonuc = s.sonuc;
            if (s != null && s.ObjeSayisi > 0) ov.o = s[0];
            return ov;
        }

        private static OnBorc BorcluOBOku(IDataReader dr)
        {
            OnBorc detay = new OnBorc();
            detay.tabloKod = DosyaABParca.ENUMTabloKod.ONBORC;
            detay.ipardDonemi = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarOB.IPARDDONEMI, 0);
            detay.dosyaAnahtar = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarOB.DOSYAANAHTAR, 0);
            detay.sira = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarOB.SIRA, 0);
            detay.basTarih.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarOB.BASTARIH));
            detay.bitTarih.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarOB.BITTARIH));
            detay.kayitDurum = (DosyaABParca.ENUMKayitDurum)VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarOB.DURUM, 0);
            detay.borcTuru = (DosyaAB.BorcTurleri)VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarOB.BORCTURU, 0);
            detay.borcunKodu = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarOB.BORCUNKODU, "");
            detay.onBorcTarihi.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarOB.ONBORCTARIHI));
            detay.onBorcSayi = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarOB.ONBORCSAYI, "");
            detay.avans = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarOB.AVANS, 0) == 1;
            detay.vakaNumarasi = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarOB.VAKANUMARASI, "");
            detay.onBorcMiktarAB = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarOB.ONBORCMIKTARAB, 0.0);
            detay.onBorcMiktarUlusal = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarOB.ONBORCMIKTARULUSAL, 0.0);
            detay.aktarimTarihi.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarOB.IPARDAKTARIMTAR));
            detay.kaydeden = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarOB.KAYDEDEN, "");
            detay.kayitTarihi.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarOB.KAYITTARIHI));
            detay.onaylayan = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarOB.ONAYLAYAN, "");
            detay.onayTarihi.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarOB.ONAYTARIHI));
            detay.gerekce = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarOB.GEREKCE, "");

            int alanSira = (int)TABATMDOSYAAB.ENUMAlanlarOB.GEREKCE + 1;
            detay.kaydedenAd = VeriErisim.DegerAl(dr, alanSira++, "");
            detay.kaydedenSoyad = VeriErisim.DegerAl(dr, alanSira++, "");
            detay.kaydedenUnvan = VeriErisim.DegerAl(dr, alanSira++, "");

            detay.onaylayanAd = VeriErisim.DegerAl(dr, alanSira++, "");
            detay.onaylayanSoyad = VeriErisim.DegerAl(dr, alanSira++, "");
            detay.onaylayanUnvan = VeriErisim.DegerAl(dr, alanSira++, "");
            detay.onBorcDovizKuru = VeriErisim.DegerAl(dr, alanSira++, 0.0);
            detay.OBKaldirma = TNSDateTime.IsNull(detay.onBorcTarihi);

            return detay;
        }

        /// <summary>
        /// OnBorc listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">OnBorc yapısında OnBorc:  bilgilerini tutan değişken</param>
        public static ObjectArray OnBorcListele(VeriErisim veriErisim, Kullanici kullanan, OnBorc form)
        {
            ObjectArray oa = new ObjectArray();
            //if (!GormeYetkisiVarMi(kullanan, form))
            //{
            //    oa.sonuc.DegerAta("");
            //    return oa;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMDOSYAAB tabATMONBORC = new TABATMDOSYAAB(ve);

            IDataReader dr = tabATMONBORC.OBListele(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                oa.Ekle(BorcluOBOku(dr));
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }
        #endregion

        #region "GAK"
        public static ObjectVAN GAKOku(VeriErisim veriErisim, Kullanici kullanan, GeriAlimKarari form)
        {
            ObjectVAN ov = new ObjectVAN();
            ObjectArray s = GAKListele(veriErisim, kullanan, form);
            ov.sonuc = s.sonuc;
            if (s != null && s.ObjeSayisi > 0) ov.o = s[0];
            return ov;
        }

        private static GeriAlimKarari BorcluGAKOku(IDataReader dr)
        {
            GeriAlimKarari detay = new GeriAlimKarari();
            detay.tabloKod = DosyaABParca.ENUMTabloKod.GERIALIMKARARI;
            detay.ipardDonemi = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarGAK.IPARDDONEMI, 0);
            detay.dosyaAnahtar = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarGAK.DOSYAANAHTAR, 0);
            detay.sira = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarGAK.SIRA, 0);
            detay.basTarih.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarGAK.BASTARIH));
            detay.bitTarih.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarGAK.BITTARIH));
            detay.kayitDurum = (DosyaABParca.ENUMKayitDurum)VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarGAK.DURUM, 0);
            detay.geriAlimSayi = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarGAK.GERIALIMSAYI, "");

            detay.geriAlimKarariTarihi.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarGAK.GERIALIMKARARITARIHI));
            detay.bildirimTarihi.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarGAK.BILDIRIMTARIHI));
            detay.tesellumTarihi.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarGAK.TESELLUMTARIHI));
            detay.odemeSonTarihi.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarGAK.ODEMESONTARIHI));
            detay.temerrutTarihi.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarGAK.TEMERRUTTARIHI));
            detay.faizinDurdurulmaTarihi.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarGAK.FAIZINDURDURULMATARIHI));

            detay.gakMiktarAB = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarGAK.GERIALIMMIKTARAB, 0.0);
            detay.gakMiktarUlusal = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarGAK.GERIALIMMIKTARULUSAL, 0.0);
            detay.silinmeyecekMiktar = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarGAK.SILINMEYECEKMIKTAR, 0.0);

            detay.kaydeden = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarGAK.KAYDEDEN, "");
            detay.kayitTarihi.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarGAK.KAYITTARIHI));
            detay.onaylayan = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarGAK.ONAYLAYAN, "");
            detay.onayTarihi.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarGAK.ONAYTARIHI));
            detay.gerekce = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarGAK.GEREKCE, "");

            int alanSira = (int)TABATMDOSYAAB.ENUMAlanlarGAK.GEREKCE + 1;
            detay.kaydedenAd = VeriErisim.DegerAl(dr, alanSira++, "");
            detay.kaydedenSoyad = VeriErisim.DegerAl(dr, alanSira++, "");
            detay.kaydedenUnvan = VeriErisim.DegerAl(dr, alanSira++, "");

            detay.onaylayanAd = VeriErisim.DegerAl(dr, alanSira++, "");
            detay.onaylayanSoyad = VeriErisim.DegerAl(dr, alanSira++, "");
            detay.onaylayanUnvan = VeriErisim.DegerAl(dr, alanSira++, "");
            detay.gakDovizKuru = VeriErisim.DegerAl(dr, alanSira++, 0.0);
            detay.gakKaldirma = TNSDateTime.IsNull(detay.geriAlimKarariTarihi);

            return detay;
        }

        /// <summary>
        /// OnBorc listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">OnBorc yapısında OnBorc:  bilgilerini tutan değişken</param>
        public static ObjectArray GAKListele(VeriErisim veriErisim, Kullanici kullanan, GeriAlimKarari form)
        {
            ObjectArray oa = new ObjectArray();
            //if (!GormeYetkisiVarMi(kullanan, form))
            //{
            //    oa.sonuc.DegerAta("");
            //    return oa;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMDOSYAAB tabATMONBORC = new TABATMDOSYAAB(ve);

            IDataReader dr = tabATMONBORC.GAKListele(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                oa.Ekle(BorcluGAKOku(dr));
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }
        public static Sonuc GAKKaydet(VeriErisim veriErisim, Kullanici kullanan, GeriAlimKarari form, ENUMDosyaIslemTur islemTur)
        {
            Sonuc s = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectVAN ovDosyaAB = Oku(ve, kullanan, new DosyaAB(form.ipardDonemi, form.dosyaAnahtar), true);
            DosyaAB dosyaAB = null;
            if (ovDosyaAB.sonuc.islemSonuc && ovDosyaAB.o != null)
            {
                dosyaAB = (DosyaAB)ovDosyaAB.o;
            }
            else
            {
                if (ovDosyaAB.sonuc.islemSonuc)
                    s.DegerAta("Dosya bulunamadı!");
                else
                    s.DegerAta(ovDosyaAB.sonuc.hataStr);
                ve.VeriErisimKapat();
                return s;
            }

            if (!YazmaYetkisiVarMi(kullanan, dosyaAB, islemTur))
            {
                s.DegerAta("Yazma Yetkisi Yok");
                return s;
            }

            s = AlanlarGecerliMiGAK(kullanan, form, dosyaAB, s);
            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            TABATMDOSYAAB tabATMGAK = new TABATMDOSYAAB(ve);

            GeriAlimKarari okunan = null;

            if (form.sira > 0)
            {
                ObjectVAN ov = GAKOku(ve, kullanan, form);
                if (ov.sonuc.islemSonuc && ov.o != null)
                {
                    okunan = (GeriAlimKarari)ov.o;
                }
                else
                {
                    if (ov.sonuc.islemSonuc)
                        ov.sonuc.DegerAta("Kayıt bulunamadı!");
                    s = ov.sonuc;
                }
            }
            if (okunan == null && islemTur != ENUMDosyaIslemTur.Kaydet)
            {
                s.DegerAta("Onaylama, iptal etme ve güncelleme işlemleri mevcut kayıtlar üzerinde yapılabilir!");
            }
            if (okunan != null)
            {
                okunan.IslemKontrol(s, islemTur, kullanan.kullaniciKodu);
            }

            if (s.islemSonuc)
            {
                ve.BeginTransaction();
                if (islemTur == ENUMDosyaIslemTur.Kaydet)
                {
                    if (form.gakKaldirma)
                    {
                        form.geriAlimKarariTarihi.isNull = true;
                        form.gakDovizKuru = 0;
                        form.geriAlimSayi = "";
                        form.bildirimTarihi.isNull = true;
                        form.tesellumTarihi.isNull = true;
                        form.odemeSonTarihi.isNull = true;
                        form.temerrutTarihi.isNull = true;
                        form.faizinDurdurulmaTarihi.isNull = true;
                        form.gakMiktarAB = 0;
                        form.gakMiktarUlusal = 0;
                        form.silinmeyecekMiktar = 0;
                    }

                    if (form.sira == 0)
                    {
                        int say = tabATMGAK.YeniKayitSayisiBul(form.ipardDonemi, form.dosyaAnahtar, TABATMDOSYAAB.TABLOADGAK);
                        if (say > 0)
                            s.DegerAta("Zaten yeni durumda olan bir kayıt olduğundan, bu işlemi kaydedemezsiniz!");
                        form.sira = tabATMGAK.SiraNoBul(form.ipardDonemi, form.dosyaAnahtar, TABATMDOSYAAB.TABLOADGAK);
                        if (!string.IsNullOrWhiteSpace(ve.hata))
                            s.DegerAta("Veri tabanından sıra no alınamadı!");
                        if (s.islemSonuc)
                            s = tabATMGAK.EkleGAK(form, kullanan.KullaniciKodu);
                    }
                    else
                        s = tabATMGAK.GuncelleGAK(form, kullanan.kullaniciKodu);
                }
                else if (islemTur == ENUMDosyaIslemTur.OzelGuncelle)
                {
                    s = tabATMGAK.GuncelleGAKOzel(form, kullanan.kullaniciKodu);
                }
                else
                {
                    DurumIslemler(s, tabATMGAK, islemTur, form, TABATMDOSYAAB.TABLOADGAK, kullanan.kullaniciKodu);
                }

                if (s.islemSonuc)
                {
                    s.anahtar = form.sira.ToString();
                    ve.Commit();
                }
                else
                    ve.Rollback();
            }
            ve.VeriErisimKapat();
            return s;
        }
        #endregion

        #region "Durum"
        public static ObjectVAN DurumOku(VeriErisim veriErisim, Kullanici kullanan, DosyaABDurum form)
        {
            ObjectVAN ov = new ObjectVAN();
            ObjectArray s = DurumListele(veriErisim, kullanan, form);
            ov.sonuc = s.sonuc;
            if (s != null && s.ObjeSayisi > 0) ov.o = s[0];
            return ov;
        }

        private static DosyaABDurum BorcluDurumOku(IDataReader dr)
        {
            DosyaABDurum detay = new DosyaABDurum();
            detay.tabloKod = DosyaABParca.ENUMTabloKod.DURUM;
            detay.ipardDonemi = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarDURUM.IPARDDONEMI, 0);
            detay.dosyaAnahtar = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarDURUM.DOSYAANAHTAR, 0);
            detay.sira = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarDURUM.SIRA, 0);
            detay.basTarih.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarDURUM.BASTARIH));
            detay.bitTarih.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarDURUM.BITTARIH));
            detay.kayitDurum = (DosyaABParca.ENUMKayitDurum)VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarDURUM.DURUM, 0);

            detay.surecAsamasi = (DosyaAB.ENUMDosyaDurum)VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarDURUM.SURECASAMASI, 0);
            detay.yorumlarTR = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarDURUM.YORUMLARTR, "");
            detay.yorumlarEN = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarDURUM.YORUMLAREN, "");

            detay.kaydeden = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarDURUM.KAYDEDEN, "");
            detay.kayitTarihi.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarDURUM.KAYITTARIHI));
            detay.onaylayan = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarDURUM.ONAYLAYAN, "");
            detay.onayTarihi.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarDURUM.ONAYTARIHI));
            detay.gerekce = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarDURUM.GEREKCE, "");

            int alanSira = (int)TABATMDOSYAAB.ENUMAlanlarDURUM.GEREKCE + 1;
            detay.kaydedenAd = VeriErisim.DegerAl(dr, alanSira++, "");
            detay.kaydedenSoyad = VeriErisim.DegerAl(dr, alanSira++, "");
            detay.kaydedenUnvan = VeriErisim.DegerAl(dr, alanSira++, "");

            detay.onaylayanAd = VeriErisim.DegerAl(dr, alanSira++, "");
            detay.onaylayanSoyad = VeriErisim.DegerAl(dr, alanSira++, "");
            detay.onaylayanUnvan = VeriErisim.DegerAl(dr, alanSira++, "");
            return detay;
        }

        /// <summary>
        /// OnBorc listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">OnBorc yapısında OnBorc:  bilgilerini tutan değişken</param>
        public static ObjectArray DurumListele(VeriErisim veriErisim, Kullanici kullanan, DosyaABDurum form)
        {
            ObjectArray oa = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMDOSYAAB tabATMONBORC = new TABATMDOSYAAB(ve);

            IDataReader dr = tabATMONBORC.DURUMListele(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                oa.Ekle(BorcluDurumOku(dr));
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        public static Sonuc DurumKaydet(VeriErisim veriErisim, Kullanici kullanan, DosyaABDurum form, ENUMDosyaIslemTur islemTur)
        {
            Sonuc s = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectVAN ovDosyaAB = Oku(ve, kullanan, new DosyaAB(form.ipardDonemi, form.dosyaAnahtar), true);
            DosyaAB dosyaAB = null;
            if (ovDosyaAB.sonuc.islemSonuc && ovDosyaAB.o != null)
            {
                dosyaAB = (DosyaAB)ovDosyaAB.o;
            }
            else
            {
                if (ovDosyaAB.sonuc.islemSonuc)
                    s.DegerAta("Dosya bulunamadı!");
                else
                    s.DegerAta(ovDosyaAB.sonuc.hataStr);
                ve.VeriErisimKapat();
                return s;
            }

            if (!YazmaYetkisiVarMi(kullanan, dosyaAB, islemTur))
            {
                s.DegerAta("Yazma Yetkisi Yok");
                return s;
            }

            s = AlanlarGecerliMiDurum(kullanan, form, dosyaAB, s);
            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            TABATMDOSYAAB tabATMDurum = new TABATMDOSYAAB(ve);

            DosyaABDurum okunan = null;

            if (form.sira > 0)
            {
                ObjectVAN ov = DurumOku(ve, kullanan, form);
                if (ov.sonuc.islemSonuc && ov.o != null)
                {
                    okunan = (DosyaABDurum)ov.o;
                }
                else
                {
                    if (ov.sonuc.islemSonuc)
                        ov.sonuc.DegerAta("Kayıt bulunamadı!");
                    s = ov.sonuc;
                }
            }
            if (okunan == null && islemTur != ENUMDosyaIslemTur.Kaydet)
            {
                s.DegerAta("Onaylama, iptal etme ve güncelleme işlemleri mevcut kayıtlar üzerinde yapılabilir!");
            }
            if (okunan != null)
            {
                okunan.IslemKontrol(s, islemTur, kullanan.kullaniciKodu);
            }

            if (s.islemSonuc)
            {
                ve.BeginTransaction();
                if (islemTur == ENUMDosyaIslemTur.Kaydet)
                {
                    if (form.sira == 0)
                    {
                        int say = tabATMDurum.YeniKayitSayisiBul(form.ipardDonemi, form.dosyaAnahtar, TABATMDOSYAAB.TABLOADDURUM);
                        if (say > 0)
                            s.DegerAta("Zaten yeni durumda olan bir kayıt olduğundan, bu işlemi kaydedemezsiniz!");
                        form.sira = tabATMDurum.SiraNoBul(form.ipardDonemi, form.dosyaAnahtar, TABATMDOSYAAB.TABLOADDURUM);
                        if (!string.IsNullOrWhiteSpace(ve.hata))
                            s.DegerAta("Veri tabanından sıra no alınamadı!");
                        if (s.islemSonuc)
                            s = tabATMDurum.EkleDurum(form, kullanan.KullaniciKodu);
                    }
                    else
                        s = tabATMDurum.GuncelleDurum(form, kullanan.kullaniciKodu);
                }
                else
                {
                    DurumIslemler(s, tabATMDurum, islemTur, form, TABATMDOSYAAB.TABLOADDURUM, kullanan.kullaniciKodu);
                }

                if (s.islemSonuc)
                {
                    s.anahtar = form.sira.ToString();
                    ve.Commit();
                }
                else
                    ve.Rollback();
            }
            ve.VeriErisimKapat();
            return s;
        }
        #endregion

        #region "Taksit"
        public static ObjectVAN ABTaksitOku(VeriErisim veriErisim, Kullanici kullanan, DosyaABTaksit form)
        {
            ObjectVAN ov = new ObjectVAN();
            ObjectArray s = ABTaksitListele(veriErisim, kullanan, form);
            ov.sonuc = s.sonuc;
            if (s != null && s.ObjeSayisi > 0) ov.o = s[0];
            return ov;
        }

        private static DosyaABTaksit BorcluTaksitOku(IDataReader dr)
        {
            DosyaABTaksit detay = new DosyaABTaksit();
            detay.tabloKod = DosyaABParca.ENUMTabloKod.TAKSIT;
            detay.ipardDonemi = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarTAKSIT.IPARDDONEMI, 0);
            detay.dosyaAnahtar = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarTAKSIT.DOSYAANAHTAR, 0);
            detay.sira = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarTAKSIT.SIRA, 0);
            detay.basTarih.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarTAKSIT.BASTARIH));
            detay.bitTarih.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarTAKSIT.BITTARIH));
            detay.kayitDurum = (DosyaABParca.ENUMKayitDurum)VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarTAKSIT.DURUM, 0);

            detay.taksitTalepTarihi.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarTAKSIT.TAKSITTALEPTARIHI));
            detay.girisTarihi.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarTAKSIT.GIRISTARIHI));
            detay.taksitTalepSayi = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarTAKSIT.TAKSITTALEPSAYI, "");
            detay.taksitKararTarihi.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarTAKSIT.TAKSITKARARTARIHI));
            detay.taksitKararSayi = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarTAKSIT.TAKSITKARARSAYI, "");
            detay.taksitIlkTarih.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarTAKSIT.TAKSITILKTARIH));
            detay.taksitSayisi = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarTAKSIT.TAKSITSAYISI, 0);
            detay.taksitKaldirma = detay.taksitSayisi == 0;

            detay.kaydeden = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarTAKSIT.KAYDEDEN, "");
            detay.kayitTarihi.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarTAKSIT.KAYITTARIHI));
            detay.onaylayan = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarTAKSIT.ONAYLAYAN, "");
            detay.onayTarihi.Yaz(VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarTAKSIT.ONAYTARIHI));
            detay.gerekce = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarTAKSIT.GEREKCE, "");
            detay.tecilFaizNo = VeriErisim.DegerAl(dr, (int)TABATMDOSYAAB.ENUMAlanlarTAKSIT.TECILTUR, 0);

            int alanSira = (int)TABATMDOSYAAB.ENUMAlanlarTAKSIT.TECILTUR + 1;
            detay.kaydedenAd = VeriErisim.DegerAl(dr, alanSira++, "");
            detay.kaydedenSoyad = VeriErisim.DegerAl(dr, alanSira++, "");
            detay.kaydedenUnvan = VeriErisim.DegerAl(dr, alanSira++, "");

            detay.onaylayanAd = VeriErisim.DegerAl(dr, alanSira++, "");
            detay.onaylayanSoyad = VeriErisim.DegerAl(dr, alanSira++, "");
            detay.onaylayanUnvan = VeriErisim.DegerAl(dr, alanSira++, "");
            return detay;
        }

        /// <summary>
        /// OnBorc listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">OnBorc yapısında OnBorc:  bilgilerini tutan değişken</param>
        public static ObjectArray ABTaksitListele(VeriErisim veriErisim, Kullanici kullanan, DosyaABTaksit form)
        {
            ObjectArray oa = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMDOSYAAB tabATMONBORC = new TABATMDOSYAAB(ve);

            IDataReader dr = tabATMONBORC.TaksitListele(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                oa.Ekle(BorcluTaksitOku(dr));
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }
        public static Sonuc TaksitKaydet(VeriErisim veriErisim, Kullanici kullanan, DosyaABTaksit form, ENUMDosyaIslemTur islemTur)
        {
            Sonuc s = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectVAN ovDosyaAB = Oku(ve, kullanan, new DosyaAB(form.ipardDonemi, form.dosyaAnahtar), true);
            DosyaAB dosyaAB = null;
            if (ovDosyaAB.sonuc.islemSonuc && ovDosyaAB.o != null)
            {
                dosyaAB = (DosyaAB)ovDosyaAB.o;
            }
            else
            {
                if (ovDosyaAB.sonuc.islemSonuc)
                    s.DegerAta("Dosya bulunamadı!");
                else
                    s.DegerAta(ovDosyaAB.sonuc.hataStr);
                ve.VeriErisimKapat();
                return s;
            }

            if (!YazmaYetkisiVarMi(kullanan, dosyaAB, islemTur))
            {
                s.DegerAta("Yazma Yetkisi Yok");
                return s;
            }

            s = AlanlarGecerliMiTaksit(kullanan, form, dosyaAB, s);
            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            TABATMDOSYAAB tabATMTaksit = new TABATMDOSYAAB(ve);

            DosyaABTaksit okunan = null;

            if (form.sira > 0)
            {
                ObjectVAN ov = ABTaksitOku(ve, kullanan, form);
                if (ov.sonuc.islemSonuc && ov.o != null)
                {
                    okunan = (DosyaABTaksit)ov.o;
                }
                else
                {
                    if (ov.sonuc.islemSonuc)
                        ov.sonuc.DegerAta("Kayıt bulunamadı!");
                    s = ov.sonuc;
                }
            }
            if (okunan == null && islemTur != ENUMDosyaIslemTur.Kaydet)
            {
                s.DegerAta("Onaylama, iptal etme ve güncelleme işlemleri mevcut kayıtlar üzerinde yapılabilir!");
            }
            if (okunan != null)
            {
                okunan.IslemKontrol(s, islemTur, kullanan.kullaniciKodu);
            }

            if (s.islemSonuc)
            {
                ve.BeginTransaction();
                if (islemTur == ENUMDosyaIslemTur.Kaydet)
                {
                    if (form.taksitKaldirma)
                    {
                        form.girisTarihi.isNull = true;
                        form.taksitTalepTarihi.isNull = true;
                        form.taksitTalepSayi = "";
                        form.taksitKararTarihi.isNull = true;
                        form.taksitKararSayi = "";
                        form.taksitIlkTarih.isNull = true;
                        form.taksitSayisi = 0;
                        form.tecilFaizNo = 0;
                    }

                    if (form.sira == 0)
                    {
                        int say = tabATMTaksit.YeniKayitSayisiBul(form.ipardDonemi, form.dosyaAnahtar, TABATMDOSYAAB.TABLOADTAKSIT);
                        if (say > 0)
                            s.DegerAta("Zaten yeni durumda olan bir kayıt olduğundan, bu işlemi kaydedemezsiniz!");
                        form.sira = tabATMTaksit.SiraNoBul(form.ipardDonemi, form.dosyaAnahtar, TABATMDOSYAAB.TABLOADTAKSIT);
                        if (!string.IsNullOrWhiteSpace(ve.hata))
                            s.DegerAta("Veri tabanından sıra no alınamadı!");
                        if (s.islemSonuc)
                            s = tabATMTaksit.EkleTaksit(form, kullanan.KullaniciKodu);
                    }
                    else
                        s = tabATMTaksit.GuncelleTaksit(form, kullanan.kullaniciKodu);
                }
                else
                {
                    DurumIslemler(s, tabATMTaksit, islemTur, form, TABATMDOSYAAB.TABLOADTAKSIT, kullanan.kullaniciKodu);
                }

                if (s.islemSonuc)
                {
                    s.anahtar = form.sira.ToString();
                    ve.Commit();
                }
                else
                    ve.Rollback();
            }
            ve.VeriErisimKapat();
            return s;
        }

        private class IslemKiyasla : IComparer
        {
            public int Compare(object x, object y)
            {
                if (x is DosyaABParca && y is DosyaABParca)
                {
                    DosyaABParca ilk = (DosyaABParca)x;
                    DosyaABParca iki = (DosyaABParca)y;
                    if (ilk.kayitDurum == iki.kayitDurum)
                    {
                        return iki.kayitTarihiDT.CompareTo(ilk.kayitTarihiDT);
                    }
                    return ((int)(ilk.kayitDurum)).CompareTo((int)(iki.kayitDurum));
                }
                return 0;
            }
        }

        private class DosyaABSirala : IComparer
        {
            public int Compare(object x, object y)
            {
                if (x == null || y == null)
                    return 0;
                if (x is DosyaAB && y is DosyaAB)
                {
                    DosyaAB ilk = (DosyaAB)x;
                    DosyaAB iki = (DosyaAB)y;

                    if (ilk.defterSirasi == null || iki.defterSirasi == null)
                        return 0;
                    return ilk.defterSirasi.CompareTo(iki.defterSirasi);
                }
                return 0;
            }
        }
        #endregion
    }
}


//internal static Sonuc TaksitKaldir(VeriErisim veriErisim, Kullanici kullanan, DosyaAB alacakTakip)
//{
//    Sonuc s = new Sonuc();
//    if (alacakTakip.iPARDDonemi <= 0 || alacakTakip.dosyaAnahtar <= 0)
//        s.DegerAta("Lütfen geçerli bir dosya seçiniz!");
//    if (!s.islemSonuc) return s;
//    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
//    DosyaAB form = new DosyaAB();
//    form.iPARDDonemi = alacakTakip.iPARDDonemi;
//    form.dosyaAnahtar = alacakTakip.dosyaAnahtar;
//    ObjectVAN ov = Oku(ve, kullanan, form, false);
//    if (!ov.sonuc.islemSonuc)
//        s = ov.sonuc;
//    else if (ov.o == null)
//    {
//        s.DegerAta("Verdiğini dosya bulunamadı!");
//    }
//    if (!s.islemSonuc)
//    {
//        ve.VeriErisimKapat();
//        return s;
//    }
//    form = (DosyaAB)ov.o;
//    if (!YazmaYetkisiVarMi(kullanan, form, 1))
//    {
//        s.DegerAta(AlacakTakipResI.DosyaAB001);
//    }

//    ve.BeginTransaction();
//    TABATMBORCHAREKET tab = new TABATMBORCHAREKET(ve);
//    TABATMDOSYAAB tabAna = new TABATMDOSYAAB(ve);

//    s = tab.Sil(form, BorcHareket.ENUMHareketTur.Taksit);
//    if (s.islemSonuc)
//        s = tabAna.TaksitlendiSil(alacakTakip);
//    if (s.islemSonuc)
//    {
//        ve.Commit();
//        TarihceYaz(ve, kullanan, form, form, TarihceAB.TarihceIslemKodlar.TaksitKaldirma);
//    }
//    else
//        ve.Rollback();
//    ve.VeriErisimKapat();
//    return s;
//}

//internal static Sonuc Taksitlendir(VeriErisim veriErisim, Kullanici kullanan, DosyaAB alacakTakip, int taksitSay)
//{
//    Sonuc s = new Sonuc();
//    if (alacakTakip.iPARDDonemi <= 0 || alacakTakip.dosyaAnahtar <= 0)
//        s.DegerAta("Lütfen geçerli bir dosya seçiniz!");
//    if (alacakTakip.taksitIlkTarihDT.Equals(DateTime.MinValue))
//        s.DegerAta("Lütfen bir tarih seçiniz");
//    if (taksitSay <= 1)
//        s.DegerAta("Lütfen taksit sayısı giriniz!");
//    if (alacakTakip.taksitTalepTarihDT == DateTime.MinValue)
//        s.DegerAta("Lütfen talep tarihini giriniz!");
//    if (string.IsNullOrWhiteSpace(alacakTakip.taksitTalepSayi))
//        s.DegerAta("Lütfen talep sayısını giriniz!");

//    if (alacakTakip.taksitKararTarihDT == DateTime.MinValue)
//        s.DegerAta("Lütfen karar tarihini giriniz!");
//    if (string.IsNullOrWhiteSpace(alacakTakip.taksitKararSayi))
//        s.DegerAta("Lütfen karar sayısını giriniz!");
//    if (s.islemSonuc && alacakTakip.taksitKararTarihDT < alacakTakip.taksitTalepTarihDT)
//        s.DegerAta("Karar tarihi, talep tarihinden küçük olamaz!");
//    if (s.islemSonuc && alacakTakip.taksitIlkTarihDT < alacakTakip.taksitTalepTarihDT)
//        s.DegerAta("Tecil tarihi, karar tarihinden küçük olamaz!");

//    if (!s.islemSonuc) return s;
//    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
//    DosyaAB form = new DosyaAB();
//    form.iPARDDonemi = alacakTakip.iPARDDonemi;
//    form.dosyaAnahtar = alacakTakip.dosyaAnahtar;
//    ObjectVAN ov = Oku(ve, kullanan, form, false);
//    if (!ov.sonuc.islemSonuc)
//        s = ov.sonuc;
//    else if (ov.o == null)
//    {
//        s.DegerAta("Verdiğini dosya bulunamadı!");
//    }
//    if (!s.islemSonuc)
//    {
//        ve.VeriErisimKapat();
//        return s;
//    }
//    form = (DosyaAB)ov.o;

//    if (!YazmaYetkisiVarMi(kullanan, form, 1))
//    {
//        s.DegerAta(AlacakTakipResI.DosyaAB001);
//    }

//    if (TNSDateTime.IsNull(form.geriAlimKarariTarihi))
//    {
//        s.DegerAta("Geri alım bilgileri girilmemiş");
//    }

//    form.IslemToplamBul(new TNSDateTime(alacakTakip.taksitTalepTarihDT.AddDays(-1)), -1, true);
//    if (OrtakFonksiyonlar.EsitMi(form.kalanMiktarToplam, 0))
//    {
//        s.DegerAta("Tasitlendirilecek borç bulunamadı!");
//    }

//    if (alacakTakip.taksitTalepTarihDT < form.geriAlimKarariTarihiDT)
//    {
//        s.DegerAta("Tasit talep tarihi geri alım kararı tarinden büyük olmalıdır!");
//    }
//    if (!s.islemSonuc)
//    {
//        ve.VeriErisimKapat();
//        return s;
//    }

//    ve.BeginTransaction();
//    TABATMBORCHAREKET tab = new TABATMBORCHAREKET(ve);
//    TABATMDOSYAAB tabAna = new TABATMDOSYAAB(ve);

//    s = tab.Sil(form, BorcHareket.ENUMHareketTur.Taksit);
//    form.taksitTalepTarihDT = alacakTakip.taksitTalepTarihDT;
//    form.taksitIlkTarihDT = alacakTakip.taksitIlkTarihDT;

//    form.YeniTaksitYap(taksitSay);

//    for (int i = 0; i < form.borcHareket.Count && s.islemSonuc; i++)
//    {
//        BorcHareket detay = (BorcHareket)form.borcHareket[i];
//        if (detay.tur == BorcHareket.ENUMHareketTur.Taksit)
//        {
//            detay.siraNo = tab.AnahtarBul(form);
//            if (!string.IsNullOrWhiteSpace(ve.hata))
//                s.DegerAta(ve.hata);
//            if (s.islemSonuc)
//                s = tab.Ekle(form, detay);
//        }
//    }
//    if (s.islemSonuc)
//        s = tabAna.TaksitlendiYaz(alacakTakip);
//    if (s.islemSonuc)
//    {
//        ve.Commit();
//        TarihceYaz(ve, kullanan, form, form, TarihceAB.TarihceIslemKodlar.Taksitlendirme);
//    }
//    else
//        ve.Rollback();
//    ve.VeriErisimKapat();
//    return s;
//}
