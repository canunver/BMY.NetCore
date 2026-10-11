using System.Data;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;
using System;
using System.Collections.Generic;
using TNS.IKY;
using TNS.HRC;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// Taahhüt işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHTaahhutKarti
    {
        /// <summary>
        /// Taahhüt kaydet
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="taahhutBilgileri">Taahhüt bilgileri</param>
        /// <returns></returns>
        public static Sonuc Kaydet(Kullanici kullanan, TaahhutKartiBilgi bilgi, TNSCollection taahhutBilgiler)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            if (bilgi == null)
            {
                sonuc.DegerAta("Kaydedilecek kart bilgisi gönderilmiş olması");
                return sonuc;
            }

            if (!bilgi.VerilerTutarliMi(sonuc))
            {
                return sonuc;
            }

            if (!TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, bilgi.muhasebeKod.ToString(), bilgi.birimKod))
            {
                sonuc.DegerAta(ButceMuhasebeResI.MUH081);
                return sonuc;
            }

            if (sonuc.islemSonuc)
            {
                FirmaBilgisi fSorgu = new FirmaBilgisi();

                if (bilgi.sozTur == TaahhutKartiBilgi.ENUMSozTur.Personel)
                    fSorgu.personeldeDeAra = true;

                fSorgu.muhasebe = bilgi.muhasebeKod;
                fSorgu.vergiNo = bilgi.muteahhitKimlikNo;
                ObjectArray firmalar = TNS.HRC.Arac.Tanimla().FirmaBilgisiListele(kullanan, fSorgu);

                if (firmalar.objeler.Count == 0)
                    sonuc.DegerAta("Girilen kimlik numarasına ait firma bilgisi bulunmamaktadır. Lütfen firma kaydı yaptıktan sonra Taahhüt Kartı kayıt işlemine devam ediniz!<br>");
            }
            if (!sonuc.islemSonuc)
            {
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTaahhutKarti to = new TABTaahhutKarti(ve);
            if (sonuc.islemSonuc && bilgi.sozTur == TaahhutKartiBilgi.ENUMSozTur.Personel) //Melih 14.12.2022 Normal taahhütde sorun oluyor
            {
                string sozlesmeNo = to.KesisenSozlesmeBul(bilgi);

                if (!string.IsNullOrEmpty(sozlesmeNo))
                    sonuc.DegerAta($"Girilen sözleşme ile {sozlesmeNo} nolu sözleşmenin tarihleri kesişmektedir. Lütfen tarihleri düzeltip devam ediniz!<br>");
            }

            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }

            //TaahhutKartiBilgi okunan = TaahhutKartOku(ve, kullanan, bilgi.kod, bilgi.muteahhitKimlikNo, bilgi.iseBaslamaTarihi, TaahhutKartiBilgi.ENUMSozTur.Personel, sonuc);
            //TaahhutKartiBilgi.ENUMSozTur.Personel-->bilgi.sozTur yapıldı Melih 18.11.2022 Manas normal taahhüt kaydında sorun oluyor
            TaahhutKartiBilgi okunan = TaahhutKartOku(ve, kullanan, bilgi.kod, bilgi.muteahhitKimlikNo, bilgi.iseBaslamaTarihi, bilgi.sozTur, sonuc, false);

            if (okunan != null)
            {
                if (okunan.OdemeEmriOnayliMi())
                {
                    sonuc.DegerAta($"Taahhütle ilgili ödeme emri {okunan.odemeEmriNo} onaylı olduğundan, kayıt üzerinde işlem yapılamaz.");
                }
            }

            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }

            if (bilgi.sozTur == TaahhutKartiBilgi.ENUMSozTur.Personel && okunan != null)
            {
                bilgi.kod = okunan.kod;
                bilgi.projeSiraNo = okunan.projeSiraNo;
                bilgi.odemeEmriDurum = okunan.odemeEmriDurum;
            }

            ve.BeginTransaction();

            string anahtar = "";
            try
            {
                TABTaahhutKartiTarihce t = new TABTaahhutKartiTarihce(ve);

                if (!bilgi.OdemeEmriOnayliMi())
                {
                    if (bilgi.projeSiraNo.Trim() == "")
                    {
                        //projeSiraNo alınıyor
                        //**********************************************************
                        TABGenel tg = new TABGenel(ve);
                        string sayacKod = "TAAHHUTPROJESIRANO" + bilgi.yil.ToString();
                        bilgi.projeSiraNo = bilgi.yil.ToString() + tg.BelgeNoAl(sayacKod, "0000");
                        //**********************************************************
                    }
                    bool ekle = false;
                    if (bilgi.kod <= 0)
                    {
                        //kod alınıyor
                        //**********************************************************
                        TABGenel tg = new TABGenel(ve);
                        string sayacKod = "TAAHHUTKOD";
                        bilgi.kod = OrtakFonksiyonlar.ConvertToInt(tg.BelgeNoAl(sayacKod, "0000"), 0);
                        ekle = true;
                        //**********************************************************
                    }

                    bilgi.islemYapanKod = kullanan.kullaniciKodu;
                    bilgi.islemYapanAd = kullanan.ADSOYAD;
                    bilgi.islemTarihi = new TNSDateTime(DateTime.Now);

                    sonuc = to.Kaydet(bilgi, ekle);
                    if (sonuc.islemSonuc && taahhutBilgiler != null)
                    {
                        sonuc = TaahhutBilgiKaydet(ve, bilgi.kod, null, taahhutBilgiler, sonuc);
                    }

                    //Taahhüt ekranında değer artışından gelen bölümün kayıt edilmesi için buraya konuldu. Can kapatmıştı. 21.10.2020 Melih
                    if (sonuc.islemSonuc && bilgi.sozTur != TaahhutKartiBilgi.ENUMSozTur.Personel)
                        sonuc = KaydetDegerArtisi(ve, kullanan, bilgi.kod, bilgi.degerArtisi);
                }
                else
                {
                    //Burada personel için gelen veriden hareketle, yeni değer artışı verisi üretilir. Onaysız kayıt varsa üzerine yazılır
                    //Hepsi onaylıysa, yeni kayıt eklenir
                    //if (okunan != null)
                    //    DegerArtisiDurumAta(okunan, bilgi);

                    if (bilgi.degerArtisi.objeler.Count == 0)//Bunu melih ekledi. Değerartışı nesnesi geldi ise hata oluşmasın Melih 21.10.2020
                        sonuc.DegerAta("Personel için değer artışı kısmı henüz yazılmadı!");
                }

                anahtar = bilgi.kod.ToString();
                if (sonuc.islemSonuc)
                {
                    TaahhutKartiTarihce tarihce = new TaahhutKartiTarihce();
                    tarihce.taahhutKod = OrtakFonksiyonlar.ConvertToInt(bilgi.kod, 0);
                    tarihce.islemYapanAd = kullanan.ADSOYAD;
                    tarihce.islemYapanKod = kullanan.TCKIMLIKNO;
                    tarihce.islemTarihi = new TNSDateTime(DateTime.Now);

                    if (sonuc.bilgiStr == "Yeni")
                    {
                        tarihce.islemAdi = ENUMTaahhutTarihceDurum.YENI.ToString();
                        tarihce.islemKod = (int)ENUMTaahhutTarihceDurum.YENI;
                        tarihce.aciklama = "Yeni kayıt";
                    }
                    else
                    {
                        tarihce.islemAdi = ENUMTaahhutTarihceDurum.GUNCELLEME.ToString();
                        tarihce.islemKod = (int)ENUMTaahhutTarihceDurum.GUNCELLEME;
                        tarihce.aciklama = "Güncelleme";
                    }

                    MUHTaahhutKartiTarihce muh = new MUHTaahhutKartiTarihce();
                    sonuc = muh.Kaydet(ve, kullanan, tarihce);
                }
            }
            catch (Exception ex)
            {
                sonuc.DegerAta(ex.Message);
            }
            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.anahtar = anahtar;
                sonuc.bilgiStr = ButceMuhasebeResI.MUH201;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static TaahhutKartiBilgi TaahhutKartOku(VeriErisim ve, Kullanici kullanan, int kod, string muteahhitKimlikNo, TNSDateTime arananTarih, TaahhutKartiBilgi.ENUMSozTur sozTur, Sonuc s, bool bulamazsaHataVer)
        {
            TaahhutKartiBilgi kriter = new TaahhutKartiBilgi();
            if (kod > 0)
                kriter.kod = kod;
            else if (!string.IsNullOrWhiteSpace(muteahhitKimlikNo) && !TNSDateTime.IsNull(arananTarih) && sozTur == TaahhutKartiBilgi.ENUMSozTur.Personel)
            {
                kriter.muteahhitKimlikNo = muteahhitKimlikNo;
                //kriter.yil = yil;
                kriter.arananTarih = arananTarih;
                kriter.sozTur = TaahhutKartiBilgi.ENUMSozTur.Personel;
            }
            else
                return null;

            ObjectArray oa = Listele(ve, kullanan, kriter, 0);
            if (!oa.sonuc.islemSonuc)
            {
                s.DegerAta(oa.sonuc.hataStr);
                return null;
            }
            else if (oa.ObjeSayisi == 1)
            {
                return (TaahhutKartiBilgi)oa.objeler[0];
            }
            else
            {
                if (oa.ObjeSayisi == 0)
                {
                    if (bulamazsaHataVer)
                        s.DegerAta("Aranan kart bulunamadı!");
                }
                else
                    s.DegerAta("Birden fazla kart bulundu!");
                return null;
            }
        }

        private static void DegerArtisiDurumAta(TaahhutKartiBilgi okunan, TaahhutKartiBilgi bilgi)
        {
            foreach (TaahhutKartiDegerArtisi item in okunan.degerArtisi.objeler)
            {
                TaahhutKartiDegerArtisi gelen = ListedeDegerArtisiBul(bilgi, item.kod);
                if (gelen != null)
                {
                    gelen.odemeEmriFormDurum = okunan.odemeEmriDurum;
                }
            }
        }

        private static TaahhutKartiDegerArtisi ListedeDegerArtisiBul(TaahhutKartiBilgi bilgi, string kod)
        {
            foreach (TaahhutKartiDegerArtisi item in bilgi.degerArtisi.objeler)
            {
                if (item.kod == kod) return item;
            }
            return null;
        }

        private static Sonuc TaahhutBilgiKaydet(VeriErisim ve, int taahhutKartiKod, string degerArtisKod, TNSCollection taahhutBilgiler, Sonuc sonuc)
        {
            TABTaahhut tab = new TABTaahhut(ve);
            tab.Sil(-1, taahhutKartiKod, degerArtisKod);
            foreach (TaahhutBilgi item in taahhutBilgiler)
            {
                item.kod = tab.SiraNoBul();
                item.taahhutKod = taahhutKartiKod;
                item.degerArtisKod = degerArtisKod;
                tab.Kaydet(item, true);
            }
            return sonuc;
        }

        public static Sonuc KaydetDegerArtisi(VeriErisim ve, Kullanici kullanan, int taahhutKod, ObjectArray taahhutDegerArtisiBilgileri)
        {
            Sonuc sonuc = new Sonuc();

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            try
            {
                TABTaahhutKarti to = new TABTaahhutKarti(ve);

                //Taahhüt ekranında silme yapıldı ise ilk önce silinsin sonra listedekiler kayıt edilsin Melih 20.10.2020
                sonuc = to.SilDegerArtisi(taahhutKod);

                foreach (TaahhutKartiDegerArtisi bilgi in taahhutDegerArtisiBilgileri.objeler)
                {
                    if (bilgi.odemeEmriFormDurum > 2) continue;
                    bilgi.taahhutKod = taahhutKod;
                    bool ekle;

                    ekle = true;//yukarıda önceki kayıtları sildiğimiz için her zaman ekleme yapılmalı
                    if (string.IsNullOrWhiteSpace(bilgi.kod))
                        bilgi.kod = Guid.NewGuid().ToString().Replace("-", "");

                    //if (string.IsNullOrWhiteSpace(bilgi.kod))
                    //{
                    //    bilgi.kod = Guid.NewGuid().ToString().Replace("-", "");
                    //    ekle = true;
                    //}
                    //else
                    //{
                    //    ekle = false;
                    //}

                    bilgi.islemYapanKod = kullanan.mernis;
                    bilgi.islemYapanAd = kullanan.ADSOYAD;
                    bilgi.islemTarihi = new TNSDateTime(DateTime.Now);

                    sonuc = to.KaydetDegerArtisi(bilgi, ekle);

                    if (!sonuc.islemSonuc)
                        break;
                }
            }
            catch (Exception ex)
            {
                sonuc.DegerAta("Değer artışı kaydetme sırasında beklenmedik bir hata oluştu:" + ex.Message);
            }
            if (sonuc.islemSonuc)
            {
                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc KaydetDegerArtisiOdemeEmri(VeriErisim ve, Kullanici kullanan, List<string> kodlar, List<int> degerArtisitaahhutKartlari, int odemeEmriYil, string odemeEmriNo)
        {
            Sonuc sonuc = new Sonuc();
            if (odemeEmriYil <= 0)
                sonuc.hataStr += "Ödeme Emri yıl bilgisi boş bırakılamaz";
            if (odemeEmriNo == "")
                sonuc.hataStr += "Ödeme Emri belge no bilgisi boş bırakılamaz";

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            TABTaahhutKarti to = new TABTaahhutKarti(ve);

            sonuc = to.KaydetDegerArtisiOdemeEmri(kodlar, odemeEmriYil, odemeEmriNo);

            if (sonuc.islemSonuc)
            {
                foreach (int item in degerArtisitaahhutKartlari)
                {
                    TaahhutKartiTarihce tarihce = new TaahhutKartiTarihce();
                    tarihce.taahhutKod = item;
                    tarihce.islemAdi = ENUMTaahhutTarihceDurum.TAHAKKUKOLUSTURULDU.ToString();
                    tarihce.islemKod = (int)ENUMTaahhutTarihceDurum.TAHAKKUKOLUSTURULDU;
                    tarihce.islemYapanAd = kullanan.ADSOYAD;
                    tarihce.islemYapanKod = kullanan.TCKIMLIKNO;
                    tarihce.islemTarihi = new TNSDateTime(DateTime.Now);
                    tarihce.aciklama = "Değer artışı Tahahhuk oluşturuldu";
                    sonuc = new MUHTaahhutKartiTarihce().Kaydet(ve, kullanan, tarihce);
                    if (!sonuc.islemSonuc)
                        return sonuc;
                }
            }

            return sonuc;
        }

        public static ObjectVAN DegerArtisiAcilabilirMi(VeriErisim veriErisim, Kullanici kullanan, TaahhutKartiDegerArtisi bilgi)
        {
            ObjectVAN ov = new ObjectVAN();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            if (bilgi.taahhutKod <= 0)
            {
                ov.sonuc.DegerAta("Lütfen tahahhkuk kartı seçiniz!");
            }
            else
            {
                TaahhutKartiBilgi okunanKart = TaahhutKartOku(ve, kullanan, bilgi.taahhutKod, null, null, TaahhutKartiBilgi.ENUMSozTur.Hepsi, ov.sonuc, false);
                if (okunanKart == null)
                {
                    ov.sonuc.DegerAta("Lütfen tahahhkuk kartı seçiniz!");
                }
                else if (okunanKart.odemeEmriDurum != 5 && okunanKart.odemeEmriDurum != 6)
                {
                    ov.sonuc.DegerAta("Taahhüt kartı ile ilgili açılış ödeme emri onaylı olmadığından, değer artışı kaydı yapamazsınız!");
                }
                else if (string.IsNullOrEmpty(bilgi.kod))
                {
                    foreach (TaahhutKartiDegerArtisi item in okunanKart.degerArtisi.objeler)
                    {
                        if (item.odemeEmriFormDurum < 5)
                        {
                            ov.sonuc.DegerAta("Daha önce açılmış ve onaylanmamış bir değer artışı kaydı var. Lütfen o kaydı düzenleyiniz!");
                        }
                    }
                }
                ov.o = okunanKart;
            }
            ve.VeriErisimKapat();
            return ov;
        }

        public static Sonuc KaydetDegerArtisiTek(VeriErisim veriErisim, Kullanici kullanan, TaahhutKartiDegerArtisi bilgi, TNSCollection taahhutBilgiler)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ObjectVAN ov = DegerArtisiAcilabilirMi(ve, kullanan, bilgi);
            Sonuc sonuc = ov.sonuc;

            if (sonuc.islemSonuc)
            {
                if (TNSDateTime.IsNull(bilgi.sozlesmeTarihi))
                    sonuc.DegerAta("Sözleşme tarihi boş olamaz!");
                if (TNSDateTime.IsNull(bilgi.onayTarihi))
                    sonuc.DegerAta("Onay tarihi boş olamaz!");
                if (bilgi.sozlesmeTutari == 0)
                    sonuc.DegerAta("Sözleşme tutarı girilmelidir!");
            }

            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }

            ve.BeginTransaction();
            try
            {
                TABTaahhutKarti to = new TABTaahhutKarti(ve);

                bool ekle;
                if (string.IsNullOrWhiteSpace(bilgi.kod))
                {
                    bilgi.kod = Guid.NewGuid().ToString().Replace("-", "");
                    ekle = true;
                }
                else
                    ekle = false;
                bilgi.islemYapanKod = kullanan.mernis;
                bilgi.islemYapanAd = kullanan.ADSOYAD;
                bilgi.islemTarihi = new TNSDateTime(DateTime.Now);

                sonuc = to.KaydetDegerArtisi(bilgi, ekle);
                if (sonuc.islemSonuc && taahhutBilgiler != null && taahhutBilgiler.Count > 0)
                {
                    sonuc = TaahhutBilgiKaydet(ve, bilgi.taahhutKod, bilgi.kod, taahhutBilgiler, sonuc);
                }
                sonuc.anahtar = bilgi.kod;
            }
            catch (Exception ex)
            {
                sonuc.DegerAta("Değer artışı kaydetme sırasında beklenmedik bir hata oluştu:" + ex.Message);
            }
            if (sonuc.islemSonuc)
            {
                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc KaydetDegerArtisiOdemeEmri(Kullanici kullanan, string kod, int taahhukKod, int odemeEmriYil, string odemeEmriNo)
        {
            Sonuc sonuc = new Sonuc();
            if (kod == "")
                sonuc.hataStr += "Deger Artışı kod bilgisi boş bırakılamaz";
            if (odemeEmriYil <= 0)
                sonuc.hataStr += "Ödeme Emri yıl bilgisi boş bırakılamaz";
            if (odemeEmriNo == "")
                sonuc.hataStr += "Ödeme Emri belge no bilgisi boş bırakılamaz";

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTaahhutKarti to = new TABTaahhutKarti(ve);

            sonuc = to.KaydetDegerArtisiOdemeEmri(kod, odemeEmriYil, odemeEmriNo);

            if (sonuc.islemSonuc)
            {
                TaahhutKartiTarihce tarihce = new TaahhutKartiTarihce();
                tarihce.taahhutKod = OrtakFonksiyonlar.ConvertToInt(taahhukKod, 0);
                tarihce.islemAdi = ENUMTaahhutTarihceDurum.TAHAKKUKOLUSTURULDU.ToString();
                tarihce.islemKod = (int)ENUMTaahhutTarihceDurum.TAHAKKUKOLUSTURULDU;
                tarihce.islemYapanAd = kullanan.ADSOYAD;
                tarihce.islemYapanKod = kullanan.TCKIMLIKNO;
                tarihce.islemTarihi = new TNSDateTime(DateTime.Now);
                tarihce.aciklama = "Değer artışı Tahahhuk oluşturuldu";
                sonuc = new MUHTaahhutKartiTarihce().Kaydet(ve, kullanan, tarihce);
                if (!sonuc.islemSonuc)
                    return sonuc;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH201;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc KaydetOdemeEmri(VeriErisim ve, Kullanici kullanan, List<int> kartKodlar, int odemeEmriYil, string odemeEmriNo)
        {
            Sonuc sonuc = new Sonuc();
            if (odemeEmriYil <= 0)
                sonuc.hataStr += "Ödeme Emri yıl bilgisi boş bırakılamaz";
            if (odemeEmriNo == "")
                sonuc.hataStr += "Ödeme Emri belge no bilgisi boş bırakılamaz";

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            TABTaahhutKarti to = new TABTaahhutKarti(ve);

            sonuc = to.KaydetOdemeEmri(kartKodlar, odemeEmriYil, odemeEmriNo);

            if (sonuc.islemSonuc)
            {
                foreach (int item in kartKodlar)
                {
                    TaahhutKartiTarihce tarihce = new TaahhutKartiTarihce();
                    tarihce.taahhutKod = item;
                    tarihce.islemAdi = ENUMTaahhutTarihceDurum.TAHAKKUKOLUSTURULDU.ToString();
                    tarihce.islemKod = (int)ENUMTaahhutTarihceDurum.TAHAKKUKOLUSTURULDU;
                    tarihce.islemYapanAd = kullanan.ADSOYAD;
                    tarihce.islemYapanKod = kullanan.TCKIMLIKNO;
                    tarihce.islemTarihi = new TNSDateTime(DateTime.Now);
                    tarihce.aciklama = "Tahahhuk oluşturuldu";
                    sonuc = new MUHTaahhutKartiTarihce().Kaydet(ve, kullanan, tarihce);
                    if (!sonuc.islemSonuc)
                        return sonuc;
                }
            }
            return sonuc;
        }

        public static Sonuc KaydetOdemeEmri(Kullanici kullanan, int kartYil, int kartKod, int odemeEmriYil, string odemeEmriNo)
        {
            Sonuc sonuc = new Sonuc();
            if (kartYil <= 0)
                sonuc.hataStr += "Kart yıl bilgisi boş bırakılamaz";
            if (kartKod <= 0)
                sonuc.hataStr += "Kart kod bilgisi boş bırakılamaz";
            if (odemeEmriYil <= 0)
                sonuc.hataStr += "Ödeme Emri yıl bilgisi boş bırakılamaz";
            if (odemeEmriNo == "")
                sonuc.hataStr += "Ödeme Emri belge no bilgisi boş bırakılamaz";

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTaahhutKarti to = new TABTaahhutKarti(ve);

            sonuc = to.KaydetOdemeEmri(kartYil, kartKod, odemeEmriYil, odemeEmriNo);

            if (sonuc.islemSonuc)
            {
                TaahhutKartiTarihce tarihce = new TaahhutKartiTarihce();
                tarihce.taahhutKod = OrtakFonksiyonlar.ConvertToInt(kartKod, 0);
                tarihce.islemAdi = ENUMTaahhutTarihceDurum.TAHAKKUKOLUSTURULDU.ToString();
                tarihce.islemKod = (int)ENUMTaahhutTarihceDurum.TAHAKKUKOLUSTURULDU;
                tarihce.islemYapanAd = kullanan.ADSOYAD;
                tarihce.islemYapanKod = kullanan.TCKIMLIKNO;
                tarihce.islemTarihi = new TNSDateTime(DateTime.Now);
                tarihce.aciklama = "Tahahhuk oluşturuldu";
                sonuc = new MUHTaahhutKartiTarihce().Kaydet(ve, kullanan, tarihce);
                if (!sonuc.islemSonuc)
                    return sonuc;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH201;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Imza Taahhüt
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, TaahhutKartiBilgi bilgi, int personel1Nrml0Yok2Hepsi)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTaahhutKarti to = new TABTaahhutKarti(ve);

            //if (!TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, bilgi.muhasebeKod.ToString(), bilgi.birimKod))
            //{
            //    iVAN.sonuc.hataStr = ButceMuhasebeResI.MUH081;
            //    iVAN.sonuc.islemSonuc = false;
            //    return iVAN;
            //}

            if (bilgi.birimKod != "") bilgi.birimKod += ";";
            bilgi.birimKod += TNS.MUH.Yetki.KullaniciMuhasebeBirimiGetir(kullanan);

            IDataReader r = to.Listele(bilgi);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TaahhutKartiBilgi taahhut = new TaahhutKartiBilgi();

                    taahhut.kod = VeriErisim.DegerAl(r, 0, 0);
                    taahhut.muhasebeKod = VeriErisim.DegerAl(r, 1, 0);
                    taahhut.projeSiraNo = VeriErisim.DegerAl(r, 2, "");
                    taahhut.DptProjeSiraNo = VeriErisim.DegerAl(r, 3, "");
                    taahhut.muteahhitKimlikNo = VeriErisim.DegerAl(r, 4, "");
                    taahhut.muteahhitUnvan = VeriErisim.DegerAl(r, 5, "");
                    taahhut.muteahhitAdres = VeriErisim.DegerAl(r, 6, "");
                    taahhut.konu = VeriErisim.DegerAl(r, 7, "");
                    taahhut.kesifBeldeli = VeriErisim.DegerAl(r, 8, 0.0);
                    taahhut.indirimOrani = VeriErisim.DegerAl(r, 9, 0.0);
                    taahhut.ihaleBedeli = VeriErisim.DegerAl(r, 10, 0.0);
                    taahhut.gerceklesenTaahhutTutar = VeriErisim.DegerAl(r, 11, 0.0);
                    taahhut.damgaVergisiTarihi = VeriErisim.DegerAl(r, 12);
                    taahhut.damgaVergisiNo = VeriErisim.DegerAl(r, 13, "");
                    taahhut.damgaVergisiTutar = VeriErisim.DegerAl(r, 14, 0.0);
                    taahhut.sozlesmeDVTarihi = VeriErisim.DegerAl(r, 15);
                    taahhut.sozlesmeDVNo = VeriErisim.DegerAl(r, 16, "");
                    taahhut.sozlesmeDVTutar = VeriErisim.DegerAl(r, 17, 0.0);
                    taahhut.sozlesmeTarihi = VeriErisim.DegerAl(r, 18);
                    taahhut.iseBaslamaTarihi = VeriErisim.DegerAl(r, 19);
                    taahhut.geciciKabulTarihi = VeriErisim.DegerAl(r, 20);
                    taahhut.isBitirmeTarihi = VeriErisim.DegerAl(r, 21);
                    taahhut.sureUzatimi1 = VeriErisim.DegerAl(r, 22, "");
                    taahhut.sureUzatimi2 = VeriErisim.DegerAl(r, 23, "");
                    taahhut.sureUzatimi3 = VeriErisim.DegerAl(r, 24, "");
                    taahhut.odenekTertip = VeriErisim.DegerAl(r, 25, "");
                    taahhut.tertipHesapKod = VeriErisim.DegerAl(r, 26, "");
                    taahhut.duranTertip = VeriErisim.DegerAl(r, 27, "");
                    taahhut.islemYapanKod = VeriErisim.DegerAl(r, 28, "");
                    taahhut.islemYapanAd = VeriErisim.DegerAl(r, 29, "");
                    taahhut.islemTarihi = VeriErisim.DegerAl(r, 30);
                    taahhut.birimKod = VeriErisim.DegerAl(r, 31, "");
                    taahhut.yil = VeriErisim.DegerAl(r, 32, 0);
                    taahhut.aciklama = VeriErisim.DegerAl(r, 33, "");
                    taahhut.odemeEmriYil = VeriErisim.DegerAl(r, 34, 0);
                    taahhut.odemeEmriNo = VeriErisim.DegerAl(r, 35, "");
                    taahhut.kapandi = VeriErisim.DegerAl(r, 36, 0);
                    taahhut.parabirimiKod = VeriErisim.DegerAl(r, 37, "");
                    taahhut.garantiBaslamaTarihi = VeriErisim.DegerAl(r, 38);
                    taahhut.garantiBitisTarihi = VeriErisim.DegerAl(r, 39);
                    taahhut.isKod = VeriErisim.DegerAl(r, 40, 0);
                    taahhut.sozTur = (TaahhutKartiBilgi.ENUMSozTur)VeriErisim.DegerAl(r, 41, 0);
                    taahhut.alimUsuluKod = VeriErisim.DegerAl(r, 42, "");
                    taahhut.sgkBildirimi = VeriErisim.DegerAl(r, 43, 0);
                    taahhut.odemeEmriDurum = VeriErisim.DegerAl(r, 44, -1);
                    taahhut.sorguTaahhutTutar = VeriErisim.DegerAl(r, 45, 0);
                    taahhut.sorguOdenenTutar = VeriErisim.DegerAl(r, 46, 0);
                    iVAN.objeler.Add(taahhut);
                }
                r.Close();

                List<string> KontrolEdilen = new List<string>();
                foreach (TaahhutKartiBilgi item in iVAN.objeler)
                {
                    if (item.kod > 0 || item.muteahhitKimlikNo != "")
                    {
                        string mKod = item.muhasebeKod.ToString() + "_" + item.birimKod;
                        if (!KontrolEdilen.Contains(mKod))
                        {
                            if (!TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, item.muhasebeKod.ToString(), item.birimKod))
                            {
                                iVAN.sonuc.hataStr = ButceMuhasebeResI.MUH081;
                                iVAN.sonuc.islemSonuc = false;
                                return iVAN;
                            }
                            KontrolEdilen.Add(mKod);
                        }

                        //Taahut deger artışı bilgilerini al
                        item.degerArtisi = ListeleDegerArtisi(ve, kullanan, item.kod);
                    }
                }
            }

            if (personel1Nrml0Yok2Hepsi != 0 && iVAN.sonuc.islemSonuc)
            {
                ObjectArray pVAN = new ObjectArray();
                TNS.IKY.IIKYServis pServis = TNS.IKY.Arac.Tanimla();
                IKY.PersonelTipi pTipKistas = new IKY.PersonelTipi();
                pTipKistas.aktif = 1;

                IKY.Personel p = new IKY.Personel();
                p.genelSorgu = bilgi.muteahhitUnvan;
                p.tcKimlikNo = bilgi.muteahhitKimlikNo;
                p.kadro.muhasebeKodu = bilgi.muhasebeKod;
                p.kadro.muhasebeBirimi = bilgi.birimKod;
                p.aktifTarih = bilgi.arananTarih;
                p.aktifTarihTur = 10;
                p.aktifDurum = -1;
                //p.aktifTarihSon = bordroDonem.sonGun;
                //p.aktifTarihSon.AddDays(1);
                //p.aktifTarihSonHareket = ptForm.pesinMi == 0;

                p.personelTipleri = TipleriVirgulle(pServis.ListelePersonelTip(kullanan, pTipKistas));
                p.listeSiralama = IKY.ENUMPersonelSira.Unvan;
                p.maasParamlarOlsun = true;
                p.bordroDisiDahil = false;
                ObjectArray pListe = pServis.PersonelListele(kullanan, p, false, true);
                if (pListe.sonuc.islemSonuc)
                {
                    foreach (TNS.IKY.Personel item in pListe.objeler)
                    {
                        //if (item.aktifMi == 0) continue;
                        if (ListedeBul(item.tcKimlikNo, iVAN.objeler) == null)
                        {
                            TaahhutKartiBilgi taahhut = new TaahhutKartiBilgi();

                            taahhut.muhasebeKod = item.kadro.muhasebeKodu;
                            taahhut.muteahhitKimlikNo = item.tcKimlikNo;
                            taahhut.muteahhitUnvan = item.ad + " " + item.soyad;
                            taahhut.muteahhitAdres = item.iletisim.adres;
                            taahhut.konu = item.personelTipAd + "(" + item.unvanAd + ")";
                            taahhut.birimKod = item.kadro.muhasebeBirimi;
                            taahhut.sozTur = TaahhutKartiBilgi.ENUMSozTur.Personel;
                            taahhut.kapandi = 99;

                            pVAN.Ekle(taahhut);
                        }
                    }
                    if (personel1Nrml0Yok2Hepsi == 2)
                    {
                        pVAN.objeler.AddRange(iVAN.objeler);
                    }
                    iVAN = pVAN;
                }
                else
                {
                    pVAN.sonuc = pListe.sonuc;
                    iVAN = pVAN;
                }

                /*
                SELECT  * FROM [InsanKaynaklari].[dbo].[IKYPERSONELTARIHCE]
                  where (DEGISIKLIK LIKE '%IslemTipi="GUNCELLE"%'
                  or DEGISIKLIK LIKE '%IslemTipi="KAYDET"%'
                  or DEGISIKLIK LIKE '%IslemTipi="SIL"%')
                  and DEGISIKLIK LIKE '<personel><kadro %'

                SELECT MUTEAHHITKIMLIKNO, ISLEMTARIHI 
                FROM MUHTAAHHUTKARTI where SOZTUR=2
                UNION
                select MUHTAAHHUTKARTI.MUTEAHHITKIMLIKNO, MUHTAAHHUTKARTIDEGERARTISI.ISLEMTARIHI  from 
                MUHTAAHHUTKARTIDEGERARTISI INNER JOIN MUHTAAHHUTKARTI ON MUHTAAHHUTKARTIDEGERARTISI.TAAHHUTKOD = MUHTAAHHUTKARTI.KOD 

                */
            }

            ve.VeriErisimKapat();

            return iVAN;
        }

        private static string TipleriVirgulle(ObjectArray tipler)
        {
            string donusDeger = "";
            if (tipler != null && tipler.sonuc != null && tipler.sonuc.islemSonuc)
            {
                foreach (PersonelTipi pt in tipler.objeler)
                {
                    if (pt.HITAP != ENUMHizmetTablolari.Bagkur)
                    {
                        if (donusDeger != "") donusDeger += ",";
                        donusDeger += pt.kod;
                    }
                }
            }
            return donusDeger;
        }

        private static object ListedeBul(string tcKimlikNo, TNSCollection objeler)
        {
            foreach (TaahhutKartiBilgi item in objeler)
            {
                if (item.muteahhitKimlikNo == tcKimlikNo)
                    return item;

            }
            return null;
        }

        public static ObjectArray ListeleDegerArtisi(VeriErisim veriErisim, Kullanici kullanan, int taahhutKod)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTaahhutKarti to = new TABTaahhutKarti(ve);

            IDataReader r = to.ListeleDegerArtisi(taahhutKod);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TaahhutKartiDegerArtisi da = new TaahhutKartiDegerArtisi();

                    da.kod = VeriErisim.DegerAl(r, 0, "");
                    da.taahhutKod = VeriErisim.DegerAl(r, 1, 0);
                    da.onayTarihi.Yaz(VeriErisim.DegerAl(r, 2));
                    da.sozlesmeTarihi.Yaz(VeriErisim.DegerAl(r, 3));
                    da.sozlesmeTutari = VeriErisim.DegerAl(r, 4, 0.0);
                    da.damgaVergisiTarihi.Yaz(VeriErisim.DegerAl(r, 5));
                    da.damgaVergisiNo = VeriErisim.DegerAl(r, 6, "");
                    da.damgaVergisiTutari = VeriErisim.DegerAl(r, 7, 0.0);
                    da.odemeEmriYil = VeriErisim.DegerAl(r, 8, 0);
                    da.odemeEmriNo = VeriErisim.DegerAl(r, 9, "");
                    da.islemYapanKod = VeriErisim.DegerAl(r, 10, "");
                    da.islemYapanAd = VeriErisim.DegerAl(r, 11, "");
                    da.islemTarihi = VeriErisim.DegerAl(r, 12);
                    da.odemeEmriFormDurum = VeriErisim.DegerAl(r, 13, -1);

                    iVAN.objeler.Add(da);
                }
                r.Close();
            }

            ve.VeriErisimKapat();

            return iVAN;
        }

        /// <summary>
        /// Taahhüt sil.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kod">kod</param>
        /// <returns></returns>
        public Sonuc Sil(Kullanici kullanan, int kod)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH068, "");

            if (kod <= 0)
            {
                sonuc.DegerAta(ButceMuhasebeResI.MUH133);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            string hata = "";
            OrtakBelge obelgeForm = new OrtakBelge();
            obelgeForm.ilgiKod = kod.ToString();
            obelgeForm.tip = ENUMOrtakBelgeTip.TAAHHUTKARTI;
            ObjectArray oListe = MUHOratakBelge.Listele(kullanan, obelgeForm);
            if (oListe.ObjeSayisi > 0)
            {
                hata = "<li>Bu dosyaya ekli belgeler bulunmaktadır.</li>";
            }

            TaahhutKartiBilgi tForm = new TaahhutKartiBilgi();
            tForm.kod = kod;
            ObjectArray oa = Listele(ve, kullanan, tForm, 0);
            foreach (TaahhutKartiBilgi item in oa.objeler)
            {
                if (item.odemeEmriNo != "")
                {
                    hata = "<li>Bu dosyaya açılış taahakkuku yapılmıştır.</li>";
                }
            }

            if (hata != "")
            {
                sonuc.hataStr = "Silme işlemi kontrolleri sırasında sorun bulundu<br>" + hata;
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            ve.BeginTransaction();
            TABTaahhutKarti to = new TABTaahhutKarti(ve);
            sonuc = to.Sil(kod);

            if (sonuc.islemSonuc)
            {
                TABTaahhut t = new TABTaahhut(ve);
                sonuc = t.Sil(-1, kod, null);
            }
            else
                ve.Rollback();

            if (sonuc.islemSonuc)
            {
                TABTaahhutTeminatlar tt = new TABTaahhutTeminatlar(ve);
                TaahhutTeminatlar teminat = new TaahhutTeminatlar();
                teminat.taahhutKod = kod;
                sonuc = tt.Sil(teminat);
            }
            else
                ve.Rollback();

            if (sonuc.islemSonuc)
            {
                TABTaahhutAvanslar ta = new TABTaahhutAvanslar(ve);
                TaahhutAvanslar avans = new TaahhutAvanslar();
                avans.taahhutKod = kod;
                sonuc = ta.Sil(avans);
            }
            else
                ve.Rollback();

            if (sonuc.islemSonuc)
            {
                TABTaahhutBorclar tb = new TABTaahhutBorclar(ve);
                TaahhutBorclar borclar = new TaahhutBorclar();
                borclar.taahhutKod = kod;
                sonuc = tb.Sil(borclar);
            }
            else
                ve.Rollback();

            if (sonuc.islemSonuc)
            {
                TABTaahhutOdemeEmri tto = new TABTaahhutOdemeEmri(ve);
                sonuc = tto.SilDiger(0, kod);
                sonuc = tto.Sil(kod);
            }
            else
                ve.Rollback();

            if (sonuc.islemSonuc)
            {
                TaahhutKartiTarihce trh = new TaahhutKartiTarihce();
                trh.taahhutKod = kod;
                TABTaahhutKartiTarihce tkt = new TABTaahhutKartiTarihce(ve);
                sonuc = tkt.Sil(trh);
            }
            else
                ve.Rollback();

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH134;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;

        }

        /// <summary>
        /// Taahhüt kapat.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kod">kod</param>
        /// <returns></returns>
        public static Sonuc Kapat(Kullanici kullanan, TNSCollection kodlar, int durum)
        {
            Sonuc sonuc = new Sonuc(true, "Dosya kapatma sırasında denetim dışı bir hata oluştu", "");// false olursa aşağıdaki kontrollerden geçip if (!sonuc.islemSonuc) denetimde çıkıyor Melih Manas 08.04.2021
            if (kodlar == null || kodlar.Count == 0)
            {
                sonuc.DegerAta("Kapatacağınız dosyayı seçin");
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTaahhutKarti to = new TABTaahhutKarti(ve);

            foreach (TaahhutKartiBilgi item in kodlar)
            {
                TaahhutKartiBilgi tkb = new TaahhutKartiBilgi();
                tkb.kod = item.kod;
                ve.hata = "";
                ObjectArray oa = Listele(ve, kullanan, tkb, 0);// 2 olursa Sami Balcı yetkiden dolayı (Personel listeleme yapamıyor) Melih 08.04.2021 Manas
                if (!oa.sonuc.islemSonuc)
                {
                    sonuc.DegerAta(oa.sonuc.hataStr);
                    break;
                }
                else if (oa.ObjeSayisi != 1)
                {

                    string msg;
                    if (oa.ObjeSayisi == 0)
                        msg = $"{item.kod} kod için taahhüt kartı bulunamadı!";
                    else
                        msg = $"{item.kod} kod için birden fazla kartı taahhüt kartı bulundu!";
                    sonuc.DegerAta(msg);
                    break;
                }
                TaahhutKartiBilgi okunan = (TaahhutKartiBilgi)oa.objeler[0];
                item.iseBaslamaTarihi = okunan.iseBaslamaTarihi;

                if (okunan.OdemeEmriOnayliMi() && durum == 9)
                {
                    sonuc.DegerAta(($"{item.kod} kodlu Taahhütle ilgili ödeme emri {okunan.odemeEmriNo} onaylı olduğundan, kayıt iptal edilemez."));
                    break;
                }
            }

            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }

            ve.BeginTransaction();
            foreach (TaahhutKartiBilgi item in kodlar)
            {
                if (sonuc.islemSonuc)
                    sonuc = to.Kapat(item.kod, durum, item.iseBaslamaTarihi);
                else
                    break;

                if (sonuc.islemSonuc)
                {

                    TaahhutKartiTarihce tarihce = new TaahhutKartiTarihce();
                    tarihce.taahhutKod = item.kod;
                    tarihce.islemAdi = ENUMTaahhutTarihceDurum.KAPANDI.ToString();
                    tarihce.islemKod = (int)ENUMTaahhutTarihceDurum.KAPANDI;
                    tarihce.islemYapanAd = kullanan.ADSOYAD;
                    tarihce.islemYapanKod = kullanan.TCKIMLIKNO;
                    tarihce.islemTarihi = new TNSDateTime(DateTime.Now);
                    tarihce.aciklama = "Dosya kapandı";
                    MUHTaahhutKartiTarihce muh = new MUHTaahhutKartiTarihce();
                    sonuc = muh.Kaydet(ve, kullanan, tarihce);
                }
                else
                    break;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
            }
            else
            {
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        internal static Sonuc OdemeEmriUretDegerArtisi(VeriErisim veriErisim, Kullanici kullanan, int taahhutKod, string degerArtisiKod)
        {
            Sonuc s = new Sonuc();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TaahhutKartiBilgi taahhutKarti = TaahhutKartOku(ve, kullanan, taahhutKod, null, null, TaahhutKartiBilgi.ENUMSozTur.Hepsi, s, false);
            if (taahhutKarti == null)
            {
                ve.VeriErisimKapat();
                s.DegerAta("İstenilen taahhüt kartı bulunamadı!");
                return s;
            }
            TaahhutKartiDegerArtisi degerArtisi = null;
            foreach (TaahhutKartiDegerArtisi item in taahhutKarti.degerArtisi.objeler)
            {
                if (item.kod == degerArtisiKod)
                {
                    degerArtisi = item;
                    break;
                }
            }
            if (degerArtisi == null)
            {
                ve.VeriErisimKapat();
                s.DegerAta("Değer artışı kaydı bulunamadı!");
                return s;
            }

            if (taahhutKarti.odemeEmriDurum != 6)
            {
                ve.VeriErisimKapat();
                s.DegerAta("Önce taahhüt kartıyla ilgili MİF yevmiyelendirilmeli!");
                return s;
            }

            if (degerArtisi.odemeEmriFormDurum == 5 || degerArtisi.odemeEmriFormDurum == 6)
            {
                ve.VeriErisimKapat();
                s.DegerAta("Değer artışı kaydıyla ilgili MİF onaylı olduğundan!");
                return s;
            }

            TaahhutBilgi taahhutBilgi = new TaahhutBilgi();
            taahhutBilgi.taahhutKod = taahhutKod;
            taahhutBilgi.degerArtisKod = degerArtisiKod;
            ObjectArray odemePlani = MUHTaahhut.Listele(ve, kullanan, taahhutBilgi);

            if (!odemePlani.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                s.DegerAta("Değer artışı kaydıyla ilgili ödeme planı bulunamadığından işleme devam edilemedi!");
                return s;
            }

            int aktifYil = DateTime.Now.Year;

            if (degerArtisi.odemeEmriYil > 0 && degerArtisi.odemeEmriYil != aktifYil && degerArtisi.odemeEmriFormDurum != 9)
            {
                ve.VeriErisimKapat();
                s.DegerAta("Değer artışı kaydıyla ilgili MİF farklı bir yıla ait olduğundan, ilgili kayıt iptal edildikten sonra devam edebilirsiniz!");
                return s;
            }

            OdemeEmriMIFForm f = new OdemeEmriMIFForm();
            f.yil = aktifYil;
            if (degerArtisi.odemeEmriYil == aktifYil)
                f.belgeNo = degerArtisi.odemeEmriNo;
            else
                f.belgeNo = "";
            f.muhasebe = taahhutKarti.muhasebeKod;
            f.birim = taahhutKarti.birimKod;
            MuhasebeBirim muhBirim = new MuhasebeBirim();
            muhBirim.muhasebe = taahhutKarti.muhasebeKod;
            muhBirim.birimKod = taahhutKarti.birimKod;
            muhBirim = MUHMuhasebeBirimServis.ListeleMuhBirim(ve, muhBirim);

            f.ilgiliNo = string.IsNullOrWhiteSpace(muhBirim.vno) ? "Muhtelif" : muhBirim.vno;
            f.ilgiliAd = muhBirim.birimAd;
            f.islemTarih.Yaz(DateTime.Now);

            foreach (TaahhutBilgi tb in odemePlani.objeler)
            {
                string hesap920 = "";
                if (tb.yil == f.yil)
                    hesap920 = "920.01";
                else if (tb.yil > f.yil)
                    hesap920 = "920.02";
                else
                    hesap920 = "";

                if (hesap920 != "")
                {
                    BulyadaEkle(f, hesap920, 4, taahhutKarti.muteahhitKimlikNo, tb.tertipKod, tb.ilkTaahhutTutari, 0, tb.ay);
                    //if (item.sonrakiYillarTutar > 0)
                    //    d = BulyadaEkle(f, "920.02", 4, item.muteahhitKimlikNo, item.tertipKod, item.sonrakiYillarTutar, 0);
                    BulyadaEkle(f, "921", 0, "", "", 0, tb.ilkTaahhutTutari, -1);
                }
            }

            f.aciklama = $"{muhBirim.birimAd} 01.09.{f.yil}-31.08.{f.yil + 1} dönemi {taahhutKarti.muteahhitUnvan} adlı personele ait değer artışının taahhüt kaydı.";

            if (f.detay.ObjeSayisi > 0)
            {
                f.detay.objeler.Sort(new DetayKiyasla("1"));
                s = new IsKurallari.MUHOdemeEmriMIFServis().Kaydet(kullanan, f);

                string uretilenBelgeNo = s.anahtar;
                ve.BeginTransaction();

                if (s.islemSonuc)
                {
                    List<string> degerArtislari = new List<string>();
                    degerArtislari.Add(degerArtisiKod);
                    List<int> degerArtisitaahhutKartlari = new List<int>();
                    degerArtisitaahhutKartlari.Add(taahhutKod);

                    s = KaydetDegerArtisiOdemeEmri(ve, kullanan, degerArtislari, degerArtisitaahhutKartlari, f.yil, uretilenBelgeNo);
                }

                if (s.islemSonuc)
                {
                    s.anahtar = uretilenBelgeNo;
                    ve.Commit();
                }
                else
                    ve.Rollback();
            }
            else
                s.DegerAta("İşlem yapılacak kayıt bulunamadı!");
            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc OdemeEmriUret(VeriErisim veriErisim, Kullanici kullanan, TaahhutKartiBilgi taahhut)
        {
            if (taahhut.muhasebeKod <= 0 || string.IsNullOrWhiteSpace(taahhut.birimKod))
            {
                return new Sonuc("Lütfen muhasebe ve birim seçiniz!");
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ObjectArray oa = MUHTaahhut.OEBekleyenListele(ve, kullanan, taahhut.yil, taahhut.muhasebeKod, taahhut.birimKod);

            if (oa.sonuc.islemSonuc && oa.ObjeSayisi == 0)
            {
                oa.sonuc.DegerAta("Sözleşmeye ait, tertip bazlı taahhütler bulunamadı!");
            }

            if (!oa.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return oa.sonuc;
            }

            int aktifYil = DateTime.Now.Year;

            OdemeEmriMIFForm f = new OdemeEmriMIFForm();
            f.yil = aktifYil;
            f.belgeNo = "";
            f.muhasebe = taahhut.muhasebeKod;
            f.birim = taahhut.birimKod;
            MuhasebeBirim muhBirim = new MuhasebeBirim();
            muhBirim.muhasebe = taahhut.muhasebeKod;
            muhBirim.birimKod = taahhut.birimKod;
            muhBirim = MUHMuhasebeBirimServis.ListeleMuhBirim(ve, muhBirim);

            f.ilgiliNo = string.IsNullOrWhiteSpace(muhBirim.vno) ? "Muhtelif" : muhBirim.vno;
            f.ilgiliAd = muhBirim.birimAd;
            f.islemTarih.Yaz(DateTime.Now);
            OdemeEmriMIFDetay d;
            List<string> degerArtislari = new List<string>();
            List<int> taahhutKartlari = new List<int>();
            List<int> degerArtisitaahhutKartlari = new List<int>();
            List<string> kisiListe = new List<string>();
            foreach (TaahhutBilgi item in oa.objeler)
            {
                if (item.MIFDurum > 2 && item.MIFDurum <= 6) continue;
                if (f.belgeNo == "" && !string.IsNullOrWhiteSpace(item.MIFNo))
                    f.belgeNo = item.MIFNo;
                if (!string.IsNullOrWhiteSpace(item.degerArtisKod))
                {
                    DegerArtisiEkle(degerArtislari, item.degerArtisKod);
                    TaahhutKartiEkle(degerArtisitaahhutKartlari, item.taahhutKod);
                }
                else
                    TaahhutKartiEkle(taahhutKartlari, item.taahhutKod);
                string hesap920 = "";
                if (item.yil == taahhut.yil && aktifYil == taahhut.yil)
                    hesap920 = "920.01";
                else if (item.yil > taahhut.yil && aktifYil == taahhut.yil)
                    hesap920 = "920.02";
                else if (item.yil == taahhut.yil + 1 && aktifYil == taahhut.yil + 1)
                    hesap920 = "920.01";

                if (hesap920 != "")
                {
                    d = BulyadaEkle(f, hesap920, 4, item.muteahhitKimlikNo, item.tertipKod, item.ilkTaahhutTutari, 0, item.ay);
                    KisiEkle(kisiListe, item.muteahhitKimlikNo);
                    //if (item.sonrakiYillarTutar > 0)
                    //    d = BulyadaEkle(f, "920.02", 4, item.muteahhitKimlikNo, item.tertipKod, item.sonrakiYillarTutar, 0);
                    d = BulyadaEkle(f, "921", 0, "", "", 0, item.ilkTaahhutTutari, -1);
                }
            }

            f.aciklama = $"{muhBirim.birimAd} 01.09.{f.yil}-31.08.{f.yil + 1} dönemi {kisiListe.Count} personele ait sözleşmelerin taahhüt kaydı.";

            if (f.detay.ObjeSayisi > 0)
            {
                f.detay.objeler.Sort(new DetayKiyasla("1"));
                oa.sonuc = new IsKurallari.MUHOdemeEmriMIFServis().Kaydet(kullanan, f);

                string uretilenBelgeNo = oa.sonuc.anahtar;
                ve.BeginTransaction();

                if (oa.sonuc.islemSonuc && taahhutKartlari.Count > 0)
                {
                    oa.sonuc = KaydetOdemeEmri(ve, kullanan, taahhutKartlari, f.yil, uretilenBelgeNo);
                }

                if (oa.sonuc.islemSonuc && degerArtislari.Count > 0)
                {
                    oa.sonuc = KaydetDegerArtisiOdemeEmri(ve, kullanan, degerArtislari, degerArtisitaahhutKartlari, f.yil, uretilenBelgeNo);
                }

                if (oa.sonuc.islemSonuc)
                {
                    oa.sonuc.anahtar = uretilenBelgeNo;
                    ve.Commit();
                }
                else
                    ve.Rollback();
            }
            else
                oa.sonuc.DegerAta("İşlem yapılacak kayıt bulunamadı!");
            ve.VeriErisimKapat();
            return oa.sonuc;
        }

        private static void KisiEkle(List<string> kisiListe, string muteahhitKimlikNo)
        {
            if (!kisiListe.Contains(muteahhitKimlikNo))
                kisiListe.Add(muteahhitKimlikNo);
        }

        private static void TaahhutKartiEkle(List<int> liste, int deger)
        {
            if (!liste.Contains(deger))
                liste.Add(deger);
        }

        private static void DegerArtisiEkle(List<string> liste, string deger)
        {
            if (!liste.Contains(deger))
                liste.Add(deger);
        }

        private static OdemeEmriMIFDetay BulyadaEkle(OdemeEmriMIFForm f, string hesapKodBasla, int ekoEkleUz, string muteahhitKimlikNo, string tertipKod, double borc, double alacak, int ay)
        {
            TNS.BKD.Tertip tUz = new TNS.BKD.Tertip(f.yil, TNS.BKD.ENUMTabloKod.TABPEBOdenek);
            int eko4Uz = tUz.KodUzunlukBul(3, 4, true);

            OdemeEmriMIFDetay d = new OdemeEmriMIFDetay(f.yil);
            d.TertipAta(tertipKod);
            d.borc = borc;
            d.alacak = alacak;
            d.BorcAlacakAyarla();
            if (ekoEkleUz > 0)
            {
                if (string.IsNullOrEmpty(f.kurKod))
                {
                    f.kurKod = d.kur;
                }
                d.hesapKod = HesapKodOl(hesapKodBasla, d.eko, ekoEkleUz, eko4Uz);
            }
            else
            {
                d.kur = f.kurKod;
                d.hesapKod = hesapKodBasla;
            }
            if (ay > 0)
            {
                d.hesapKod += ay.ToString("00");
            }
            d.altHesapKod = muteahhitKimlikNo;
            return BulyadaEkle(f, d, ekoEkleUz);
        }

        private static OdemeEmriMIFDetay BulyadaEkle(OdemeEmriMIFForm f, OdemeEmriMIFDetay d, int ekoEkleUz)
        {
            foreach (OdemeEmriMIFDetay item in f.detay.objeler)
            {
                if (item.Equals(d))
                {
                    item.borc += d.borc;
                    item.alacak += d.alacak;
                    item.BorcAlacakAyarla();
                    return item;
                }
            }
            f.detay.Ekle(d);
            return d;
        }

        private static string HesapKodOl(string hesapKodBasla, string eko, int ekoEkleUz, int eko4Uz)
        {
            string yeniKod = TNS.MUH.Arac.EkonomikKoddanHesapkodBul(eko, hesapKodBasla, eko4Uz);
            //if (yeniKod.Length > hesapKodBasla.Length + ekoEkleUz * 3)
            //    yeniKod = yeniKod.Substring(0, 15);
            return yeniKod;
        }
    }
}
