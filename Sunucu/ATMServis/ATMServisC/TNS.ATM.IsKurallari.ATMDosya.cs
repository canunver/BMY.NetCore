using System;
using System.Data;
using TNS.ATM.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using System.Collections;
using TNS.DEG;

namespace TNS.ATM.IsKurallari
{
    /// <summary>
    /// Alacak Takip Dosya Sınıfı
    /// </summary>
    public class DosyaClass
    {
        private static string kurumKod = null;
        private static string kurKod = null;


        /// <summary>
        /// Yıl, Dosya Numarası, Durum Kodu, Durum Tarihine göre Alacak Takip durumunu güncelleyen fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="yil">Yıl bilgisi, Int tipinde sınıf</param>
        /// <param name="dosyaNo">Dosya numarası bilgisi, Int tipinde sınıf</param>
        /// <param name="durumKodu">Durum kodu bilgisi, Int tipinde sınıf</param>
        /// <param name="durumTarihi">Durum Tarihi bilgisi, TNSDateTime tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        public static Sonuc DurumGuncelle(VeriErisim veriErisim, int yil, int dosyaNo, int durumKodu, TNSDateTime durumTarihi)
        {
            Sonuc donusDeger = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(null));
            TABATMDosya tabAlacakTakip = new TABATMDosya(ve);
            ve.BeginTransaction();

            try
            {
                donusDeger = tabAlacakTakip.DurumGuncelle(yil, dosyaNo, durumKodu, durumTarihi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.Message;
            }

            if (donusDeger.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Alacak Takip dosyası tarihlerini kontrol eden fonksiyon
        /// </summary>
        /// <param name="alacakTakip">Alacak Takip dosya bilgisi, Dosya tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        private void TarihleriKontrolEt(Sonuc sonuc, Dosya alacakTakip)
        {

            //1. 2. den büyükse 1 dönüyor.
            //1. 2. den küçükse -1 dönüyor.
            //1. 2. ye eşitse 0 dönüyor.
            int kontrol = 0;
            if (!alacakTakip.borcDogusTarihi.isNull)
            {
                if (!alacakTakip.borcTespitTarihi.isNull)
                {
                    kontrol = alacakTakip.borcDogusTarihi.Oku().CompareTo(alacakTakip.borcTespitTarihi.Oku());
                    if (kontrol == 1)

                        sonuc.hataStr += AlacakTakipResI.ATA001;
                }

                if (!alacakTakip.borcDegerlendirmeTarihi.isNull)
                {
                    kontrol = alacakTakip.borcDogusTarihi.Oku().CompareTo(alacakTakip.borcDegerlendirmeTarihi.Oku());
                    if (kontrol == 1)
                        sonuc.hataStr += AlacakTakipResI.ATA002;
                }

                if (!alacakTakip.tebligTarihi.isNull)
                {
                    kontrol = alacakTakip.borcDogusTarihi.Oku().CompareTo(alacakTakip.tebligTarihi.Oku());
                    if (kontrol == 1)
                        sonuc.hataStr += AlacakTakipResI.ATA003;
                }

                if (!alacakTakip.faizBaslangicTarihi.isNull)
                {
                    kontrol = alacakTakip.borcDogusTarihi.Oku().CompareTo(alacakTakip.faizBaslangicTarihi.Oku());
                    if (kontrol == 1)
                        sonuc.hataStr += AlacakTakipResI.ATA004;
                }
            }

            if (!alacakTakip.borcTespitTarihi.isNull)
            {
                if (!alacakTakip.borcDegerlendirmeTarihi.isNull)
                {
                    kontrol = alacakTakip.borcTespitTarihi.Oku().CompareTo(alacakTakip.borcDegerlendirmeTarihi.Oku());
                    if (kontrol == 1)
                        sonuc.hataStr += AlacakTakipResI.ATA005;
                }

                if (!alacakTakip.tebligTarihi.isNull)
                {
                    kontrol = alacakTakip.borcTespitTarihi.Oku().CompareTo(alacakTakip.tebligTarihi.Oku());
                    if (kontrol == 1)
                        sonuc.hataStr += AlacakTakipResI.ATA006;
                }
            }

            if (!alacakTakip.borcDegerlendirmeTarihi.isNull)
            {
                if (!alacakTakip.tebligTarihi.isNull)
                {
                    kontrol = alacakTakip.borcDegerlendirmeTarihi.Oku().CompareTo(alacakTakip.tebligTarihi.Oku());
                    if (kontrol == 1)
                        sonuc.hataStr += AlacakTakipResI.ATA007;
                }
            }

            if (!alacakTakip.faizBaslangicTarihi.isNull)
            {
                if (!alacakTakip.tebligTarihi.isNull)
                {
                    kontrol = alacakTakip.faizBaslangicTarihi.Oku().CompareTo(alacakTakip.tebligTarihi.Oku());
                    if (kontrol == 1)
                        sonuc.hataStr += AlacakTakipResI.ATA008;
                }
            }

            if (string.IsNullOrEmpty(sonuc.hataStr))
                sonuc.islemSonuc = true;
        }

        /// <summary>
        /// Alacak Takip dosya bilgilerini kaydeden fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="alacakTakip">Alacak Takip dosya bilgisi, Dosya tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        public Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Dosya alacakTakip)
        {
            Sonuc donusDeger = Yetki.DosyaYazmaOnaylamaHakkiVarMi(kullanan, alacakTakip.saymanlikKodu, alacakTakip.birimKodu, ENUMDosyaIslemTur.Kaydet);
            if (!donusDeger.islemSonuc)
                return donusDeger;

            if (alacakTakip.yil <= 0)
            {
                donusDeger.DegerAta("Lütfen yıl giriniz!");
            }

            if (alacakTakip.borcluTipi <= 0)
            {
                donusDeger.DegerAta(AlacakTakipResI.ATA009);
            }

            if (string.IsNullOrWhiteSpace(alacakTakip.borcluAd))
            {
                donusDeger.DegerAta(AlacakTakipResI.ATA012);
            }

            //mernis kontrolü yapılsın mı?
            string mernis = System.Configuration.ConfigurationManager.AppSettings.Get("AlacakTakipMernisKontrolEtme") + "";
            if (string.IsNullOrWhiteSpace(mernis))
            {
                if (string.IsNullOrWhiteSpace(alacakTakip.kimlikNo))
                {
                    if (alacakTakip.borcluTipi == 1)
                        donusDeger.DegerAta(AlacakTakipResI.ATA010);
                    else
                        donusDeger.DegerAta(AlacakTakipResI.ATA011);
                }

                if (alacakTakip.borcluTipi == 2)
                {
                    //IKYMServis servisKYM = TNS.KYM.Arac.Tanimla();

                    //KullaniciVAN kVAN = servisKYM.MernisBilgiOku(kullanan, alacakTakip.kimlikNo);
                    //if (kVAN.sonuc.islemSonuc)
                    //{
                    //    if (alacakTakip.borcluAd != kVAN.kullanici.ad + " " + kVAN.kullanici.soyad)
                    //    {
                    //        donusDeger.DegerAta(AlacakTakipResI.ATA013);
                    //    }
                    //}
                }
            }

            if (alacakTakip.borc <= 0)
            {
                donusDeger.DegerAta(AlacakTakipResI.ATA014);
            }

            if (alacakTakip.paraBirimi <= 0)
            {
                donusDeger.DegerAta(AlacakTakipResI.ATA015);
            }

            TarihleriKontrolEt(donusDeger, alacakTakip);

            if (!donusDeger.islemSonuc)
                return donusDeger;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMDosya tabAlacakTakip = new TABATMDosya(ve);

            int sonDurum = 0;
            if (alacakTakip.dosyaNo > 0)
            {
                sonDurum = Genel.SonDurumDondur(ve, alacakTakip.yil, alacakTakip.dosyaNo);
            }

            donusDeger = Genel.SaymanlikDogruMu(alacakTakip.saymanlikKodu);
            if (!donusDeger.islemSonuc)
            {
                ve.VeriErisimKapat();
                return donusDeger;
            }

            donusDeger = Genel.BirimDogruMu(alacakTakip.saymanlikKodu, alacakTakip.birimKodu);
            if (!donusDeger.islemSonuc)
            {
                ve.VeriErisimKapat();
                return donusDeger;
            }

            if (alacakTakip.faizTur.yil > 0 && alacakTakip.faizTur.kod > 0)
            {
                if (!Genel.FaizTurVarMi(ve, alacakTakip.faizTur.yil, alacakTakip.faizTur.kod))
                {
                    donusDeger.DegerAta(AlacakTakipResI.ATA016);
                    ve.VeriErisimKapat();
                    return donusDeger;
                }
            }

            ve.BeginTransaction();
            try
            {
                DosyaDurumTarihceClass durumTarihce = new DosyaDurumTarihceClass();
                DosyaDurumTarihce durum = new DosyaDurumTarihce();
                durum.yil = alacakTakip.yil;

                if (alacakTakip.dosyaNo > 0)
                {
                    if (sonDurum > (int)ENUMDurum.TakipBiriminde)
                        alacakTakip.sonDurum.kod = sonDurum;
                    else
                        alacakTakip.sonDurum.kod = (int)ENUMDurum.TakipBiriminde;

                    donusDeger = tabAlacakTakip.Guncelle(alacakTakip);
                    if (donusDeger.islemSonuc)
                        durum.durum.kod = alacakTakip.sonDurum.kod;
                }
                else
                {
                    int sonDurumDeger = OrtakFonksiyonlar.ConvertToInt(System.Configuration.ConfigurationManager.AppSettings.Get("AlacakTakipDurumBasla") + "", 0);
                    if (sonDurumDeger > 0)
                        alacakTakip.sonDurum.kod = sonDurumDeger;
                    else
                        alacakTakip.sonDurum.kod = (int)ENUMDurum.TakipBiriminde;

                    donusDeger = tabAlacakTakip.Kaydet(alacakTakip);
                    if (donusDeger.islemSonuc)
                        durum.durum.kod = alacakTakip.sonDurum.kod;
                }

                if (donusDeger.islemSonuc && durum.durum.kod != sonDurum)
                {
                    durum.dosyaNo = OrtakFonksiyonlar.ConvertToInt(donusDeger.anahtar, 0);
                    durum.tarih = new TNSDateTime(DateTime.Today.Date.ToString());
                    durum.kullaniciKodu = kullanan.kullaniciKodu;
                    TNSCollection tc = new TNSCollection();
                    tc.Add(durum);
                    donusDeger = durumTarihce.Kaydet(ve, kullanan, tc);
                    if (donusDeger.islemSonuc)
                        donusDeger.anahtar = durum.dosyaNo.ToString();
                }
            }
            catch (Exception e)
            {
                donusDeger.DegerAta(e.Message);
            }

            if (donusDeger.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return donusDeger;
        }

        public static Dosya Oku(VeriErisim veriErisim, Kullanici kullanan, int yil, int dosyaNo)
        {
            Dosya okunacak = new Dosya();
            okunacak.yil = yil;
            okunacak.dosyaNo = dosyaNo;
            okunacak.muhasebeBorcDurumuAl = TNS.ATM.Arac.MuhasebedenGelsin(TNS.MUH.Arac.VarsayilanKurumKod().Replace(".", ""));
            ObjectArray liste = Listele(veriErisim, kullanan, okunacak, true);
            if (liste.objeler.Count > 0)
                return (Dosya)liste.objeler[0];
            else
                return null;
        }

        /// <summary>
        /// Alacak Takip dosya bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="alacakTakip">Alacak Takip Dosya bilgisi, Dosya tipinde sınıf</param>
        /// <returns>ObjectArray tipinde sınıf döndürür</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Dosya alacakTakip, bool yetkiKontroluYapma = false)
        {
            ObjectArray donusDeger = new ObjectArray();
            if (!yetkiKontroluYapma)
            {
                if (alacakTakip.saymanlikKodu > 0 && alacakTakip.birimKodu != "")
                    donusDeger.sonuc = Yetki.DosyaOkumaHakkiVarMi(kullanan, alacakTakip.saymanlikKodu, alacakTakip.birimKodu);
                else
                    donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan);

                if (!donusDeger.sonuc.islemSonuc)
                    return donusDeger;
            }
            else if (alacakTakip.yil <= 0 && alacakTakip.dosyaNo <= 0)
            {
                donusDeger.sonuc.hataStr = AlacakTakipResI.ATA017;
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMDosya tabDosya = new TABATMDosya(ve);
            IDataReader dr = null;

            if (alacakTakip.saymanlikKodu > 0)
            {
                donusDeger.sonuc = Genel.SaymanlikDogruMu(alacakTakip.saymanlikKodu);
                if (!donusDeger.sonuc.islemSonuc && alacakTakip.dosyaNo <= 0)
                {
                    ve.VeriErisimKapat();
                    return donusDeger;
                }
            }

            if (alacakTakip.saymanlikKodu > 0 && alacakTakip.birimKodu != "")
            {
                donusDeger.sonuc = Genel.BirimDogruMu(alacakTakip.saymanlikKodu, alacakTakip.birimKodu);
                if (!donusDeger.sonuc.islemSonuc && alacakTakip.dosyaNo <= 0)
                {
                    ve.VeriErisimKapat();
                    return donusDeger;
                }
            }

            if (kurumKod == null)
                kurumKod = TNS.MUH.Arac.VarsayilanKurumKod();
            if (kurumKod == "02")
            {
                if (kurKod == null)
                {
                    //965 birimi 2 tane tanımlı bu nedenle çoklama oluyor. Kurumsal göndermek için
                    kurKod = TNS.DEG.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEBIRIMIKURUMSALKOD") + "";
                }
                alacakTakip.kurKod = kurKod.Replace(".", "");
            }

            dr = tabDosya.Listele(alacakTakip, Yetki.SaymanlikVeBirimKodlariArrayDondur(kullanan));
            if (dr == null)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = ve.hata;
                ve.VeriErisimKapat();
                return donusDeger;
            }

            Dosya dosya = null;
            int taksitBasla = 0;
            while (dr.Read())
            {
                int gyil = VeriErisim.DegerAl(dr, 0, -1);
                int gdosyaNo = VeriErisim.DegerAl(dr, 1, -1);

                if (dosya == null || (dosya.yil != gyil || dosya.dosyaNo != gdosyaNo))
                {
                    dosya = new Dosya();
                    dosya.yil = gyil;
                    dosya.dosyaNo = gdosyaNo;
                    dosya.saymanlikKodu = VeriErisim.DegerAl(dr, 2, -1);
                    dosya.birimKodu = VeriErisim.DegerAl(dr, 3, "");

                    dosya.kimlikNo = VeriErisim.DegerAl(dr, 4, "");
                    dosya.borcluTipi = VeriErisim.DegerAl(dr, 5, -1);
                    dosya.borcTespitTarihi = VeriErisim.DegerAl(dr, 6);
                    dosya.borcDogusTarihi = VeriErisim.DegerAl(dr, 7);
                    dosya.borcluAd = VeriErisim.DegerAl(dr, 8, "");
                    dosya.borcluAdres = VeriErisim.DegerAl(dr, 9, "");
                    dosya.borc = VeriErisim.DegerAl(dr, 10, -1.0);
                    dosya.borcSebep = VeriErisim.DegerAl(dr, 11, "");
                    dosya.faizBaslangicTarihi = VeriErisim.DegerAl(dr, 12);
                    dosya.faizTur.yil = VeriErisim.DegerAl(dr, 13, -1);
                    dosya.faizTur.kod = VeriErisim.DegerAl(dr, 14, -1);
                    dosya.zamanAsimiBaslangicYili = VeriErisim.DegerAl(dr, 15, -1);
                    dosya.tebligTarihi = VeriErisim.DegerAl(dr, 16);
                    dosya.sonDurum.kod = VeriErisim.DegerAl(dr, 17, -1);
                    dosya.borcDegerlendirmeTarihi = VeriErisim.DegerAl(dr, 18);
                    dosya.paraBirimi = VeriErisim.DegerAl(dr, 19, 0);
                    dosya.sonDurumTarihi = VeriErisim.DegerAl(dr, 20);
                    dosya.saymanlikDosyaNo = VeriErisim.DegerAl(dr, 21, string.Empty);
                    dosya.odemeEmriNo = VeriErisim.DegerAl(dr, 22, string.Empty);

                    dosya.gelenYaziYer = VeriErisim.DegerAl(dr, 23, string.Empty);
                    dosya.gelenYaziTarih = VeriErisim.DegerAl(dr, 24);
                    dosya.gelenYaziSayi = VeriErisim.DegerAl(dr, 25, string.Empty);

                    dosya.saymanligaGidenYaziTarih = VeriErisim.DegerAl(dr, 26);
                    dosya.saymanligaGidenYaziSayi = VeriErisim.DegerAl(dr, 27, string.Empty);

                    dosya.kisiyiGidenYaziTarih = VeriErisim.DegerAl(dr, 28);
                    dosya.kisiyeGidenYaziSayi = VeriErisim.DegerAl(dr, 29, string.Empty);
                    dosya.taksitBaslangicTarihi = VeriErisim.DegerAl(dr, 30);

                    int son = 30;
                    dosya.sonDurum.ad = VeriErisim.DegerAl(dr, ++son, string.Empty);
                    dosya.faizTur.ad = VeriErisim.DegerAl(dr, ++son, string.Empty);
                    dosya.faizTur.oran = VeriErisim.DegerAl(dr, ++son, 0.0);
                    dosya.saymanlikAdi = VeriErisim.DegerAl(dr, ++son, "");
                    dosya.birimAdi = VeriErisim.DegerAl(dr, ++son, "");
                    dosya.muhasebeBorcDurumu = VeriErisim.DegerAl(dr, ++son, 0.0);
                    dosya.islemYapan = VeriErisim.DegerAl(dr, ++son, "");
                    taksitBasla = son + 1;
                    donusDeger.objeler.Add(dosya);
                }
                DosyaTaksit gelen = DosyaTaksitClass.TaksitOku(dr, taksitBasla);
                dosya.taksitEkle(gelen);
            }

            if (donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = AlacakTakipResI.ATA018;

            donusDeger.sonuc.islemSonuc = true;

            dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        //public double MuhasebeBorcDurumu(Kullanici kullanan, string kimlikNo)
        //{
        //    double borc = 0.0;

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABATMDosya tabDosya = new TABATMDosya(ve);

        //    borc = tabDosya.MuhasebeBorcDurumu(kimlikNo);

        //    ve.VeriErisimKapat();
        //    return borc;

        //}

        /// <summary>
        /// Yıla göre Tebliğ Tarihi Geçmiş Dosyaları listeleyen fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="yil">Yıl bilgisi, Int tipinde sınıf</param>
        /// <returns>ObjectArray tipinde sınıf döndürür</returns>
        public ObjectArray TebligTarihiGecmisDosyaListele(VeriErisim veriErisim, Kullanici kullanan, int yil)
        {
            ObjectArray donusDeger = new ObjectArray();
            donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan);
            if (!donusDeger.sonuc.islemSonuc)
                return donusDeger;

            if (yil <= 0)
            {
                donusDeger.sonuc.hataStr = AlacakTakipResI.ATA019;
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMDosya tabDosya = new TABATMDosya(ve);
            IDataReader dr = null;

            try
            {
                dr = tabDosya.TebligTarihiGecmisDosyaListele(yil, Yetki.SaymanlikVeBirimKodlariArrayDondur(kullanan));
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                ve.VeriErisimKapat();
                return donusDeger;
            }

            Hashtable birimAd = new Hashtable();
            Hashtable muhasebeAd = new Hashtable();
            string muhasebe = "";
            string birim = "";

            while (dr.Read())
            {
                Dosya dosya = new Dosya();
                dosya.yil = VeriErisim.DegerAl(dr, 0, -1);
                dosya.dosyaNo = VeriErisim.DegerAl(dr, 1, -1);
                dosya.saymanlikKodu = VeriErisim.DegerAl(dr, 2, -1);
                dosya.birimKodu = VeriErisim.DegerAl(dr, 3, "");


                if (muhasebeAd[dosya.saymanlikKodu.ToString()] == null)
                {
                    muhasebe = Genel.SaymanlikAdiDondur(dosya.saymanlikKodu);
                    muhasebeAd[dosya.saymanlikKodu.ToString()] = muhasebe;
                }
                else
                    muhasebe = (string)muhasebeAd[dosya.saymanlikKodu.ToString()];

                if (birimAd[dosya.birimKodu + "-" + dosya.saymanlikKodu.ToString().Trim()] == null)
                {
                    birim = Genel.BirimAdiDondur(dosya.saymanlikKodu, dosya.birimKodu);
                    birimAd[dosya.birimKodu + "-" + dosya.saymanlikKodu.ToString().Trim()] = birim;
                }
                else
                    birim = (string)birimAd[dosya.birimKodu + "-" + dosya.saymanlikKodu.ToString().Trim()];


                dosya.saymanlikAdi = muhasebe;
                dosya.birimAdi = birim;
                dosya.kimlikNo = VeriErisim.DegerAl(dr, 4, "");
                dosya.borcluTipi = VeriErisim.DegerAl(dr, 5, -1);
                dosya.borcTespitTarihi = VeriErisim.DegerAl(dr, 6);
                dosya.borcDogusTarihi = VeriErisim.DegerAl(dr, 7);
                dosya.borcluAd = VeriErisim.DegerAl(dr, 8, "");
                dosya.borcluAdres = VeriErisim.DegerAl(dr, 9, "");
                dosya.borc = VeriErisim.DegerAl(dr, 10, -1.0);
                dosya.borcSebep = VeriErisim.DegerAl(dr, 11, "");
                dosya.faizBaslangicTarihi = VeriErisim.DegerAl(dr, 12);
                dosya.faizTur.yil = VeriErisim.DegerAl(dr, 13, -1);
                dosya.faizTur.kod = VeriErisim.DegerAl(dr, 14, -1);
                dosya.zamanAsimiBaslangicYili = VeriErisim.DegerAl(dr, 15, -1);
                dosya.tebligTarihi = VeriErisim.DegerAl(dr, 16);
                dosya.sonDurum.kod = VeriErisim.DegerAl(dr, 17, -1);
                dosya.borcDegerlendirmeTarihi = VeriErisim.DegerAl(dr, 18);
                dosya.paraBirimi = VeriErisim.DegerAl(dr, 19, 0);
                dosya.sonDurumTarihi = VeriErisim.DegerAl(dr, 20);
                dosya.saymanlikDosyaNo = VeriErisim.DegerAl(dr, 21, string.Empty);
                dosya.odemeEmriNo = VeriErisim.DegerAl(dr, 22, string.Empty);

                dosya.sonDurum.ad = VeriErisim.DegerAl(dr, 23, string.Empty);
                dosya.faizTur.ad = VeriErisim.DegerAl(dr, 24, string.Empty);
                dosya.faizTur.oran = VeriErisim.DegerAl(dr, 25, 0.0);

                donusDeger.objeler.Add(dosya);
            }

            if (donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = AlacakTakipResI.ATA020;

            donusDeger.sonuc.islemSonuc = true;

            dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Odemes the emri kaydet dosya.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="dosyaNo">The dosya no.</param>
        /// <param name="belgeYil">The belge yil.</param>
        /// <param name="belgeNo">The belge no.</param>
        /// <returns></returns>
        public Sonuc OdemeEmriKaydetDosya(VeriErisim veriErisim, int yil, int dosyaNo, int belgeYil, string belgeNo)
        {
            Sonuc donusDeger = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(null));
            TABATMDosya tabAlacakTakip = new TABATMDosya(ve);
            ve.BeginTransaction();

            try
            {
                donusDeger = tabAlacakTakip.OdemeEmriKaydetDosya(yil, dosyaNo, belgeYil, belgeNo);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.Message;
            }

            if (donusDeger.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return donusDeger;
        }
    }
}