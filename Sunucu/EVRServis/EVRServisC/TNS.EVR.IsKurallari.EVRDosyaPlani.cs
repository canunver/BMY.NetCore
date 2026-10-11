using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System;

namespace TNS.EVR.IsKurallari
{
    /// <summary>
    /// Dosya planı bilgileriyle ilgili, tanımlama, silme, listeleme, kullanım durumu değiştirme ve yetki kontrolü yordamlarını tutan sınıf. 
    /// </summary>
    public class EVRDosyaPlani
    {
        /// <summary>
        /// Parametre olarak verilen DosyaPlani nesne koleksiyonunu veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">DosyaPlani nesne koleksiyonunu içeren nesne</param>
        /// <param name="aktarim">DosyaPlani nesne koleksiyonunun aktarım ile gelip gelmediğini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste, bool aktarim, string seciliKod)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string anahtar = "";
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            sonuc = KontrolEt(ve, kullanan, liste, "Yazma", seciliKod);

            if (sonuc.islemSonuc)
            {
                ve.BeginTransaction();
                try
                {
                    TABDosyaPlani to = new TABDosyaPlani(ve);

                    //Yeni dosya kodları liste olarak aktarım yapılmak istendiğinde eskileri pasif yapılmasın.Melih.19.12.2012
                    //if (aktarim)//Aktarım yapılırken eski kodların hepsi kullanılmıyor olarak işaretleniyor
                    //    to.KullanilmiyorYap("");

                    ObjectArray eskiListe = new ObjectArray();
                    DosyaPlani eski = null;
                    foreach (DosyaPlani bilgi in liste.objeler)
                    {
                        eskiListe = Listele(ve, new DosyaPlani { kod = bilgi.kod, tur = bilgi.tur });
                        eski = eskiListe.ObjeSayisi > 0 ? (DosyaPlani)eskiListe.objeler[0] : new DosyaPlani();
                        sonuc = DosyaPlaniTarihceYaz(kullanan, to, bilgi, eski, 1);
                        if (sonuc.islemSonuc)
                        {
                            if (bilgi.acilisTarihi.isNull)
                                bilgi.acilisTarihi = new TNSDateTime(DateTime.Now);

                            //*************************************************************
                            //Client tarafında listeden bir alan seçilip dosya kodu ve adı değiştirilip gönderilirse. Yeni kayıt olacak fakar islemKodu aynı olacağı için hr zaman islemkod yeniden alıyor
                            anahtar = new TABGenel(ve).BelgeNoAl("DOSYAPLAN", "0");
                            bilgi.islemKod = OrtakFonksiyonlar.ConvertToInt(anahtar, 0);
                            //*************************************************************

                            sonuc = to.Kaydet(bilgi);
                            if (!sonuc.islemSonuc)
                                break;
                        }
                        else
                            break;
                    }

                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();

                        if (liste.objeler.Count == 1) sonuc.anahtar = anahtar;

                        sonuc.bilgiStr += EvrakI.EVRI026;
                    }
                    else
                        ve.Rollback();

                }
                catch (Exception ex)
                {
                    ve.Rollback();
                    OrtakFonksiyonlar.HataStrYaz("{5E83E1CB-24EF-40FD-A8CA-F1B943265B43} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
                }

            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        private static Sonuc DosyaPlaniTarihceYaz(Kullanici kullanan, TABDosyaPlani to, DosyaPlani yeni, DosyaPlani eski, int islem)
        {
            // islem 1 kaydet
            // islem 2 sil 
            string kodFormat = "P{0}{1}";
            if (yeni.tur > 0 && !yeni.kod.StartsWith("P"))
            {
                yeni.kod = string.Format(kodFormat, yeni.tur.ToString().PadLeft(2, '0'), yeni.kod);
            }
            if (eski.tur > 0 && !eski.kod.StartsWith("P"))
            {
                eski.kod = string.Format(kodFormat, eski.tur.ToString().PadLeft(2, '0'), eski.kod);
            }

            Sonuc sonuc = new Sonuc(true, "", "");
            ENUMDosyaPlaniTarihceIslem tarihceIslem = ENUMDosyaPlaniTarihceIslem.TANIMSIZ;
            if (islem == 1)
            {
                tarihceIslem = ENUMDosyaPlaniTarihceIslem.DEGISTIRILDI;
                if (eski.kod != yeni.kod)
                    tarihceIslem = ENUMDosyaPlaniTarihceIslem.YENIKAYIT;
            }
            else if (islem == 2)
            {
                tarihceIslem = ENUMDosyaPlaniTarihceIslem.SIL;
                sonuc = to.KaydetTarihce(eski.kod, TABDosyaPlani.ENUMAlanlar.KOD, eski.kod, yeni.kod, tarihceIslem, kullanan.kullaniciKodu);
                return sonuc;
            }

            if (sonuc.islemSonuc && yeni.kod != eski.kod)
                sonuc = to.KaydetTarihce(yeni.kod, TABDosyaPlani.ENUMAlanlar.KOD, eski.kod, yeni.kod, tarihceIslem, kullanan.kullaniciKodu);
            if (sonuc.islemSonuc && yeni.ad != eski.ad)
                sonuc = to.KaydetTarihce(yeni.kod, TABDosyaPlani.ENUMAlanlar.AD, eski.ad, yeni.ad, tarihceIslem, kullanan.kullaniciKodu);
            //if (sonuc.islemSonuc && yeni.detay != eski.detay)
            //    sonuc = to.KaydetTarihce(yeni.kod, TABDosyaPlani.ENUMAlanlar.DETAY, eski.detay.ToString(), yeni.detay.ToString(), tarihceIslem, kullanan.kullaniciKodu);
            if (sonuc.islemSonuc && yeni.kullanilmiyor != eski.kullanilmiyor)
                sonuc = to.KaydetTarihce(yeni.kod, TABDosyaPlani.ENUMAlanlar.KULLANILMIYOR, eski.kullanilmiyor.ToString(), yeni.kullanilmiyor.ToString(), tarihceIslem, kullanan.kullaniciKodu);
            if (sonuc.islemSonuc && yeni.saklamaSuresi != eski.saklamaSuresi)
                sonuc = to.KaydetTarihce(yeni.kod, TABDosyaPlani.ENUMAlanlar.SAKLAMASURESI, eski.saklamaSuresi.ToString(), yeni.saklamaSuresi.ToString(), tarihceIslem, kullanan.kullaniciKodu);
            if (sonuc.islemSonuc && yeni.saklamaKriteri != eski.saklamaKriteri)
                sonuc = to.KaydetTarihce(yeni.kod, TABDosyaPlani.ENUMAlanlar.SAKLAMAKRITERI, eski.saklamaKriteri.ToString(), yeni.saklamaKriteri.ToString(), tarihceIslem, kullanan.kullaniciKodu);

            return sonuc;
        }

        public static Sonuc DosyaPlaniBolumTanimKaydet(VeriErisim veriErisim, Kullanici kullanan, DosyaPlaniBolumTanim bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            if (bilgi.periyod == 1)
            {
                if (bilgi.baslangicAy > 12 || bilgi.baslangicAy < 1 || bilgi.bitisAy > 12 || bilgi.bitisAy < 1 || bilgi.baslangicGun < 1 || bilgi.bitisGun < 1)
                {
                    sonuc.DegerAta("Başlangıç ve/veya Bitiş Tarih tanımlarında hata var.");
                    return sonuc;
                }
                int basSonGun = DateTime.DaysInMonth(DateTime.Now.Year, bilgi.baslangicAy);
                int bitSonGun = DateTime.DaysInMonth(DateTime.Now.Year, bilgi.bitisAy);
                if (bilgi.baslangicGun > basSonGun || bilgi.bitisGun > bitSonGun)
                {
                    sonuc.DegerAta("Başlangıç ve/veya Bitiş Tarih tanımlarında hata var.");
                    return sonuc;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABDosyaPlani to = new TABDosyaPlani(ve);
            ve.BeginTransaction();
            try
            {
                sonuc = to.KaydetBolumTanim(bilgi);

                if (sonuc.islemSonuc)
                {
                    ve.Commit();

                    sonuc.bilgiStr += EvrakI.EVRI004;
                }
                else
                    ve.Rollback();

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{D8505A41-A770-4526-BE63-DFCA43D9E3BE} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc DosyaPlaniBolumTanimSonIslemKaydet(VeriErisim veriErisim, Kullanici kullanan, DosyaPlaniBolumTanim bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABDosyaPlani to = new TABDosyaPlani(ve);
            ve.BeginTransaction();
            try
            {
                sonuc = to.KaydetBolumTanimSonIslem(bilgi);

                if (sonuc.islemSonuc)
                {
                    ve.Commit();

                    sonuc.bilgiStr += EvrakI.EVRI004;
                }
                else
                    ve.Rollback();

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{D8505A41-A770-4526-BE63-DFCA43D9E3XE} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static ObjectArray DosyaPlaniBolumTanimListele(VeriErisim veriErisim, DosyaPlaniBolumTanim bilgi)
        {
            ObjectArray hVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            TABDosyaPlani to = new TABDosyaPlani(ve);
            IDataReader r = to.ListeleBolumTanim(bilgi);

            if (r == null)
            {
                ve.VeriErisimKapat();
                hVAN.sonuc.hataStr = EvrakI.EVRI029;
                hVAN.sonuc.islemSonuc = false;
                return hVAN;
            }

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    bilgi = new DosyaPlaniBolumTanim();

                    bilgi.hesapKodu = VeriErisim.DegerAl(r, (int)TABDosyaPlani.ENUMAlanlarBolumTanim.HESAPKODU, "");
                    bilgi.bolumAdi = VeriErisim.DegerAl(r, (int)TABDosyaPlani.ENUMAlanlarBolumTanim.BOLUMADI, "");
                    bilgi.durumu = VeriErisim.DegerAl(r, (int)TABDosyaPlani.ENUMAlanlarBolumTanim.DURUMU, 0);
                    bilgi.periyod = VeriErisim.DegerAl(r, (int)TABDosyaPlani.ENUMAlanlarBolumTanim.PERIYOD, 0);
                    bilgi.baslangicGun = VeriErisim.DegerAl(r, (int)TABDosyaPlani.ENUMAlanlarBolumTanim.BASLANGICGUN, 0);
                    bilgi.bitisGun = VeriErisim.DegerAl(r, (int)TABDosyaPlani.ENUMAlanlarBolumTanim.BITISGUN, 0);
                    bilgi.baslangicAy = VeriErisim.DegerAl(r, (int)TABDosyaPlani.ENUMAlanlarBolumTanim.BASLANGICAY, 0);
                    bilgi.bitisAy = VeriErisim.DegerAl(r, (int)TABDosyaPlani.ENUMAlanlarBolumTanim.BITISAY, 0);
                    bilgi.sonIslem = VeriErisim.DegerAl(r, (int)TABDosyaPlani.ENUMAlanlarBolumTanim.SONISLEM, 0);
                    bilgi.saklamaOrtami = VeriErisim.DegerAl(r, (int)TABDosyaPlani.ENUMAlanlarBolumTanim.SAKLAMAORTAMI, 0);
                    bilgi.fizikiSaklamaYeri = VeriErisim.DegerAl(r, (int)TABDosyaPlani.ENUMAlanlarBolumTanim.FIZIKISAKLAMAYERI, "");
                    hVAN.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return hVAN;
        }

        public static ObjectArray DosyaPlaniBolumListele(VeriErisim veriErisim, DosyaPlaniBolum bilgi)
        {
            ObjectArray hVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            TABDosyaPlani to = new TABDosyaPlani(ve);
            IDataReader r = to.ListeleBolum(bilgi);

            if (r == null)
            {
                ve.VeriErisimKapat();
                hVAN.sonuc.hataStr = EvrakI.EVRI029;
                hVAN.sonuc.islemSonuc = false;
                return hVAN;
            }

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    bilgi = new DosyaPlaniBolum();

                    bilgi.kod = VeriErisim.DegerAl(r, (int)TABDosyaPlani.ENUMAlanlarBolum.KOD, 0);
                    bilgi.hesapKodu = VeriErisim.DegerAl(r, (int)TABDosyaPlani.ENUMAlanlarBolum.HESAPKODU, "");
                    bilgi.bolumAdi = VeriErisim.DegerAl(r, (int)TABDosyaPlani.ENUMAlanlarBolum.BOLUMADI, "");
                    bilgi.durumu = VeriErisim.DegerAl(r, (int)TABDosyaPlani.ENUMAlanlarBolum.DURUMU, 0);
                    bilgi.baslangic = VeriErisim.DegerAl(r, (int)TABDosyaPlani.ENUMAlanlarBolum.ACILISTARIHI);
                    bilgi.bitis = VeriErisim.DegerAl(r, (int)TABDosyaPlani.ENUMAlanlarBolum.KAPANISTARIHI);
                    bilgi.dosyaPlani.ad = VeriErisim.DegerAl(r, (int)TABDosyaPlani.ENUMAlanlarBolum.BOLUMADI, "");
                    bilgi.dosyaPlani.kod = VeriErisim.DegerAl(r, (int)TABDosyaPlani.ENUMAlanlarBolum.HESAPKODU, "");
                    bilgi.tur = VeriErisim.DegerAl(r, (int)TABDosyaPlani.ENUMAlanlarBolum.TUR, 0);
                    bilgi.klasorKod = VeriErisim.DegerAl(r, (int)TABDosyaPlani.ENUMAlanlarBolum.KLASORKODU, 0);
                    bilgi.saklamaOrtami = VeriErisim.DegerAl(r, (int)TABDosyaPlani.ENUMAlanlarBolum.SAKLAMAORTAMI, 0);
                    bilgi.fizikiSaklamaYeri = VeriErisim.DegerAl(r, (int)TABDosyaPlani.ENUMAlanlarBolum.FIZIKISAKLAMAYERI, "");
                    hVAN.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return hVAN;
        }

        public static Sonuc DosyaPlaniBolumKaydet(VeriErisim veriErisim, Kullanici kullanan, DosyaPlaniBolum bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            ObjectArray klasor = new ObjectArray();
            if (bilgi.klasorKod > 0)
            {
                DosyaPlaniBolum kbolum = new DosyaPlaniBolum();
                kbolum.kod = bilgi.klasorKod;
                klasor = DosyaPlaniBolumListele(null, kbolum);
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABDosyaPlani to = new TABDosyaPlani(ve);
            ve.BeginTransaction();
            try
            {
                //Eğer klasör yaratılıyorsa diğer klasör tarihleri ile çakışmaması gerekir
                int sayi = to.TarihAraligiKontrolEt(bilgi);
                if (sayi > 0)
                {
                    sonuc.hataStr = bilgi.baslangic.ToString() + " - " + bilgi.bitis.ToString() + " tarihleri arası klasör oluşturmak için uygun değildir.";
                    sonuc.islemSonuc = false;
                }

                if (klasor.objeler.Count > 0)
                {
                    foreach (DosyaPlaniBolum d in klasor.objeler)
                    {
                        if (d.durumu == 0)
                        {
                            sonuc.hataStr = "Kapalı olan klasör altına bölüm açılamaz.";
                            sonuc.islemSonuc = false;
                        }
                        //Eğer bölüm yaratılıyorsa mensup olduğu klasör tarihleri içinde yer alması gerekir
                        if (!(d.baslangic.CompareTo(bilgi.baslangic) <= 0 && d.bitis.CompareTo(bilgi.baslangic) >= 0 && d.baslangic.CompareTo(bilgi.bitis) <= 0 && d.bitis.CompareTo(bilgi.bitis) >= 0))
                        {
                            sonuc.hataStr = "Verilen tarihler uygun değildir. " + d.baslangic.ToString() + " - " + d.bitis.ToString() + " tarihleri arasında olması gerekir.";
                            sonuc.islemSonuc = false;
                        }
                    }
                }

                if (sonuc.islemSonuc)
                {
                    to.AktiflikGuncelle(bilgi);
                    sonuc = to.KaydetBolum(bilgi);
                }

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.anahtar = bilgi.kod.ToString();
                    sonuc.bilgiStr += EvrakI.EVRI004;
                }
                else
                    ve.Rollback();

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{D8505A41-A770-4526-BE63-DFCA43D9E3BE} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc DosyaPlaniKlasorBolumOlustur(VeriErisim veriErisim, Kullanici kullanan)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            int aktifYil = DateTime.Now.Year;
            int aktifAy = DateTime.Now.Month;

            //İlk önce klasörler şablonlarını listele
            DosyaPlaniBolumTanim sablonKriter = new DosyaPlaniBolumTanim();
            sablonKriter.tur = 1;
            ObjectArray sablon = DosyaPlaniBolumTanimListele(ve, sablonKriter);

            //Klasör şablonlarını dolaş Periyot ile son işlemine bakarak oluşturulması gerekiyorsa oluştur
            foreach (DosyaPlaniBolumTanim dt in sablon.objeler)
            {
                int sonIslemDeger = 0;
                if ((dt.periyod == 1 && dt.sonIslem != aktifYil) || (dt.periyod == 2 && dt.sonIslem != aktifAy))//YILLIK ve Sonişlem Bu yıl değilse VEYA AYLIK ve Sonişlem bu ay değilse
                {
                    DosyaPlaniBolum db = new DosyaPlaniBolum();

                    if (dt.periyod == 1)//YILLIK
                    {
                        db.baslangic = new TNSDateTime(dt.baslangicGun + "." + dt.baslangicAy + "." + aktifYil);
                        db.bitis = new TNSDateTime(dt.bitisGun + "." + dt.bitisAy + "." + aktifYil);
                        sonIslemDeger = aktifYil;
                    }
                    else if (dt.periyod == 2)//AYLIK
                    {
                        int sonGun = DateTime.DaysInMonth(aktifYil, aktifAy);
                        if (dt.bitisGun > sonGun) dt.bitisGun = sonGun;

                        db.baslangic = new TNSDateTime(dt.baslangicGun + "." + aktifAy.ToString("00") + "." + aktifYil);
                        db.bitis = new TNSDateTime(dt.bitisGun + "." + aktifAy.ToString("00") + "." + aktifYil);
                        sonIslemDeger = aktifAy;
                    }

                    db.bolumAdi = dt.bolumAdi.Replace("{YIL}", aktifYil.ToString()).Replace("{AY}", OrtakFonksiyonlar.AyAdiGetir(aktifAy));
                    db.durumu = 1;
                    db.hesapKodu = dt.hesapKodu;
                    db.klasorKod = 0;
                    db.kod = 0;
                    db.tur = 1;
                    db.saklamaOrtami = dt.saklamaOrtami;
                    db.fizikiSaklamaYeri = dt.fizikiSaklamaYeri;

                    sonuc = DosyaPlaniBolumKaydet(ve, kullanan, db);
                    if (sonuc.islemSonuc)
                    {
                        DosyaPlaniBolumTanim sonIslem = new DosyaPlaniBolumTanim();
                        sonIslem.hesapKodu = dt.hesapKodu;
                        sonIslem.tur = 1;
                        sonIslem.sonIslem = sonIslemDeger;
                        sonuc = DosyaPlaniBolumTanimSonIslemKaydet(ve, kullanan, sonIslem);
                    }
                }
            }


            //Bölüm şablonlarını dolaş. 
            //Bölüm kayıtlı olduğu hesapkoduna ait aktif durumdaki klasörü bul
            //Bu klasörün altında Periyot ile son işlemine bakarak oluşturulması gerekiyorsa oluştur
            sablonKriter.tur = 2;
            sablon = DosyaPlaniBolumTanimListele(ve, sablonKriter);

            foreach (DosyaPlaniBolumTanim dt in sablon.objeler)
            {
                //Bu şablonun yaratılacağı klasörü bul.
                DosyaPlaniBolum bolum = new DosyaPlaniBolum();
                bolum.hesapKodu = dt.hesapKodu;
                bolum.tur = 1;
                ObjectArray klasor = DosyaPlaniBolumListele(ve, bolum);

                foreach (DosyaPlaniBolum dbb in klasor.objeler)
                {
                    if (dbb.durumu == 0) continue;

                    int sonIslemDeger = 0;
                    if ((dt.periyod == 1 && dt.sonIslem != aktifYil) || (dt.periyod == 2 && dt.sonIslem != aktifAy))//YILLIK ve Sonişlem Bu yıl değilse VEYA AYLIK ve Sonişlem bu ay değilse
                    {
                        DosyaPlaniBolum db = new DosyaPlaniBolum();

                        if (dt.periyod == 1)//YILLIK
                        {
                            db.baslangic = new TNSDateTime(dt.baslangicGun + "." + dt.baslangicAy + "." + aktifYil);
                            db.bitis = new TNSDateTime(dt.bitisGun + "." + dt.bitisAy + "." + aktifYil);
                            sonIslemDeger = aktifYil;
                        }
                        else if (dt.periyod == 2)//AYLIK
                        {
                            int sonGun = DateTime.DaysInMonth(aktifYil, aktifAy);
                            if (dt.bitisGun > sonGun) dt.bitisGun = sonGun;

                            db.baslangic = new TNSDateTime(dt.baslangicGun + "." + aktifAy.ToString("00") + "." + aktifYil);
                            db.bitis = new TNSDateTime(dt.bitisGun + "." + aktifAy.ToString("00") + "." + aktifYil);
                            sonIslemDeger = aktifAy;
                        }

                        db.bolumAdi = dt.bolumAdi.Replace("{YIL}", aktifYil.ToString()).Replace("{AY}", OrtakFonksiyonlar.AyAdiGetir(aktifAy));
                        db.durumu = 1;
                        db.hesapKodu = dt.hesapKodu;
                        db.klasorKod = dbb.kod;//KLASÖR Kodu
                        db.kod = 0;
                        db.tur = 2;
                        db.saklamaOrtami = dt.saklamaOrtami;
                        db.fizikiSaklamaYeri = dt.fizikiSaklamaYeri;

                        sonuc = DosyaPlaniBolumKaydet(ve, kullanan, db);
                        if (sonuc.islemSonuc)
                        {
                            DosyaPlaniBolumTanim sonIslem = new DosyaPlaniBolumTanim();
                            sonIslem.hesapKodu = dt.hesapKodu;
                            sonIslem.tur = 2;
                            sonIslem.sonIslem = sonIslemDeger;
                            sonuc = DosyaPlaniBolumTanimSonIslemKaydet(ve, kullanan, sonIslem);
                        }
                    }
                }
            }


            ve.VeriErisimKapat();

            return sonuc;

        }

        /// <summary>
        /// Parametre olarak verilen DosyaPlani nesne koleksiyonunu içeren dosya planı bilgilerini veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">DosyaPlani nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI027, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            sonuc = KontrolEt(ve, kullanan, liste, "Silme", null);
            if (sonuc.islemSonuc)
            {
                ve.BeginTransaction();
                try
                {
                    TABBirimDosyaPlani tob = new TABBirimDosyaPlani(ve);
                    foreach (DosyaPlani bilgi in liste.objeler)
                        sonuc = tob.Sil("", bilgi.kod);

                    if (sonuc.islemSonuc)
                    {
                        TABDosyaPlani to = new TABDosyaPlani(ve);
                        ObjectArray eskiListe = new ObjectArray();
                        DosyaPlani eski = null;
                        foreach (DosyaPlani bilgi in liste.objeler)
                        {
                            eskiListe = Listele(ve, new DosyaPlani { kod = bilgi.kod, tur = bilgi.tur });
                            eski = eskiListe.ObjeSayisi > 0 ? (DosyaPlani)eskiListe.objeler[0] : new DosyaPlani();
                            sonuc = DosyaPlaniTarihceYaz(kullanan, to, new DosyaPlani(), eski, 2);
                            if (sonuc.islemSonuc)
                            {
                                sonuc = to.Sil(bilgi);
                                if (!sonuc.islemSonuc)
                                    break;
                            }
                            else
                                break;
                        }
                    }

                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.bilgiStr = EvrakI.EVRI028;
                    }
                    else
                        ve.Rollback();
                }
                catch (Exception ex)
                {
                    ve.Rollback();
                    OrtakFonksiyonlar.HataStrYaz("{A0CB8629-98F5-41EC-A47B-7502FB25960E} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
                }
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen DosyaPlani nesnesine göre, DosyaPlani nesne koleksiyonunu veritabanından okuyan yordam.
        /// </summary>
        /// <param name="bilgi">DosyaPlani bilgisini içeren nesne</param>
        /// <returns>DosyaPlani nesne koleksiyonunu olarak döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, DosyaPlani bilgi)
        {
            ObjectArray hVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            if (bilgi.tur > 0 && !bilgi.kod.StartsWith("P"))
            {
                string kodFormat = "P{0}{1}";
                bilgi.kod = string.Format(kodFormat, bilgi.tur.ToString().PadLeft(2, '0'), bilgi.kod);
            }

            TABDosyaPlani to = new TABDosyaPlani(ve);
            IDataReader r = to.Listele(bilgi);

            if (r == null)
            {
                ve.VeriErisimKapat();
                hVAN.sonuc.hataStr = EvrakI.EVRI029;
                hVAN.sonuc.islemSonuc = false;
                return hVAN;
            }

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    bilgi = new DosyaPlani();

                    bilgi.kod = VeriErisim.DegerAl(r, 0, "");
                    bilgi.ad = VeriErisim.DegerAl(r, 1, "");
                    bilgi.seviye = VeriErisim.DegerAl(r, 2, 0);
                    bilgi.detay = VeriErisim.DegerAl(r, 3, 0) == 0;
                    bilgi.kullanilmiyor = VeriErisim.DegerAl(r, 4, 0);
                    bilgi.saklamaSuresi = VeriErisim.DegerAl(r, 5, 0);
                    bilgi.saklamaKriteri = VeriErisim.DegerAl(r, 6, 0);
                    bilgi.tur = BilgiTurBul(bilgi.kod);
                    bilgi.islemKod = VeriErisim.DegerAl(r, 7, 0);
                    bilgi.sureBaslangicTipi = VeriErisim.DegerAl(r, 8, 0);
                    bilgi.tasfiyePlanTipi = VeriErisim.DegerAl(r, 9, 0);
                    bilgi.gerekce = VeriErisim.DegerAl(r, 10, "");
                    bilgi.aciklama = VeriErisim.DegerAl(r, 11, "");
                    bilgi.gizlilik = VeriErisim.DegerAl(r, 12, 0);
                    bilgi.anahtarKelime = VeriErisim.DegerAl(r, 13, "");
                    bilgi.acilisTarihi = VeriErisim.DegerAl(r, 14);
                    bilgi.kapanisTarihi = VeriErisim.DegerAl(r, 15);
                    bilgi.adTipi = VeriErisim.DegerAl(r, 16, 0);
                    bilgi.saklamaPlanTipi = VeriErisim.DegerAl(r, 17, 0);
                    //bilgi.bolumTanim.bolumAdi = VeriErisim.DegerAl(r, 11, "");

                    if (bilgi.tur > 0)
                        bilgi.kod = bilgi.kod.Substring(3);

                    hVAN.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return hVAN;
        }

        public static DosyaPlani Oku(VeriErisim veriErisim, DosyaPlani bilgi)
        {
            ObjectArray liste = Listele(veriErisim, bilgi);

            DosyaPlani dp = new DosyaPlani();

            if (liste.objeler.Count > 0)
                return (DosyaPlani)liste.objeler[0];
            else
                return dp;
        }

        /// <summary>
        /// Parametre olarak verilen DosyaPlani nesnesine göre, DosyaPlani nesne koleksiyonunu veritabanından okuyan yordam.
        /// </summary>
        /// <param name="bilgi">DosyaPlani bilgisini içeren nesne</param>
        /// <returns>DosyaPlani nesne koleksiyonunu olarak döndürülür.</returns>
        public static ObjectArray ListeleTarihce(VeriErisim veriErisim, DosyaPlaniTarihce bilgi)
        {
            ObjectArray hVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            if (bilgi.kodTur > 0 && !bilgi.kod.StartsWith("P"))
            {
                string kodFormat = "P{0}{1}";
                bilgi.kod = string.Format(kodFormat, bilgi.kodTur.ToString().PadLeft(2, '0'), bilgi.kod);
            }

            TABDosyaPlani to = new TABDosyaPlani(ve);
            IDataReader r = to.ListeleTarihce(bilgi);

            if (r == null)
            {
                ve.VeriErisimKapat();
                hVAN.sonuc.hataStr = EvrakI.EVRI029;
                hVAN.sonuc.islemSonuc = false;
                return hVAN;
            }

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    bilgi = new DosyaPlaniTarihce();

                    bilgi.islemKod = VeriErisim.DegerAl(r, 0, "");
                    bilgi.kod = VeriErisim.DegerAl(r, 1, "");
                    bilgi.tarih = VeriErisim.DegerAl(r, 2);
                    bilgi.kolon = VeriErisim.DegerAl(r, 3, "");

                    bilgi.eskiDeger = VeriErisim.DegerAl(r, 4, "");
                    bilgi.yeniDeger = VeriErisim.DegerAl(r, 5, "");
                    bilgi.islem = TarihceIslemDondur(VeriErisim.DegerAl(r, 6, ""));
                    bilgi.kullaniciKodu = VeriErisim.DegerAl(r, 7, "");
                    bilgi.kodTur = BilgiTurBul(bilgi.kod);

                    bilgi.kolonAciklama = KolonAciklamaGetir(bilgi);
                    bilgi.islemAciklama = IslemAciklamaGetir(bilgi);
                    bilgi.eskiDeger = EskiDegerAciklamaGetir(bilgi);
                    bilgi.yeniDeger = YeniDegerAciklamaGetir(bilgi);

                    hVAN.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return hVAN;
        }

        public static ObjectArray ListeleDosyaPlaniEvrak(VeriErisim veriErisim, EvrakSinif bilgi)
        {
            ObjectArray hVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABDosyaPlani to = new TABDosyaPlani(ve);
            IDataReader r = to.ListeleDosyaPlaniEvrak(bilgi);

            if (r == null)
            {
                ve.VeriErisimKapat();
                hVAN.sonuc.hataStr = EvrakI.EVRI029;
                hVAN.sonuc.islemSonuc = false;
                return hVAN;
            }

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    bilgi = new EvrakSinif();

                    //dosya kodlarını atama işlemi
                    //*********************************************
                    string dosyaKodlari = VeriErisim.DegerAl(r, 0, "");
                    string[] dler = dosyaKodlari.Split(';');
                    foreach (string d in dler)
                    {
                        DosyaPlani dosyaPlani = new DosyaPlani();
                        dosyaPlani.kod = d;
                        if (dosyaPlani.kod.Trim() != "")
                            bilgi.dosyaKodlari.Add(dosyaPlani);
                    }

                    bilgi.evrakNo = VeriErisim.DegerAl(r, 2, 0);
                    bilgi.evrakBirimKodu = VeriErisim.DegerAl(r, 3, "");
                    bilgi.konu = VeriErisim.DegerAl(r, 4, "");
                    bilgi.evrakTipi = VeriErisim.DegerAl(r, 5, 0);

                    bilgi.geldigiYer = VeriErisim.DegerAl(r, 6, "");
                    bilgi.gelenEvrakTarih = VeriErisim.DegerAl(r, 7);
                    bilgi.hareket.islem = VeriErisim.DegerAl(r, 8, 0);

                    hVAN.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return hVAN;
        }

        public static ObjectArray ListeleKlasorBolum(VeriErisim veriErisim, DosyaPlaniBolum bilgi)
        {
            ObjectArray hVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            if (bilgi.tur > 0 && !bilgi.dosyaPlani.kod.StartsWith("P"))
            {
                string kodFormat = "P{0}{1}";
                bilgi.dosyaPlani.kod = string.Format(kodFormat, bilgi.tur.ToString().PadLeft(2, '0'), bilgi.kod);
            }

            TABDosyaPlani to = new TABDosyaPlani(ve);
            IDataReader r = to.ListeleKlasorBolum(bilgi);

            if (r == null)
            {
                ve.VeriErisimKapat();
                hVAN.sonuc.hataStr = EvrakI.EVRI029;
                hVAN.sonuc.islemSonuc = false;
                return hVAN;
            }

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    bilgi = new DosyaPlaniBolum();

                    bilgi.hesapKodu = VeriErisim.DegerAl(r, 0, "");
                    bilgi.dosyaPlani.ad = VeriErisim.DegerAl(r, 1, "");
                    bilgi.tur = VeriErisim.DegerAl(r, 2, 0);
                    bilgi.klasorKod = VeriErisim.DegerAl(r, 3, 0);

                    bilgi.bolumAdi = VeriErisim.DegerAl(r, 4, "");
                    bilgi.baslangic = VeriErisim.DegerAl(r, 5);
                    bilgi.bitis = VeriErisim.DegerAl(r, 6);
                    bilgi.durumu = VeriErisim.DegerAl(r, 7, 0);
                    bilgi.klasorKod = VeriErisim.DegerAl(r, 8, 0); ;

                    hVAN.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return hVAN;
        }

        //public static ObjectArray ListeleDosyaTarihceSayisi(VeriErisim veriErisim, DosyaPlaniTarihce bilgi)
        //{
        //    ObjectArray hVAN = new ObjectArray();

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

        //    if (bilgi.kodTur > 0 && !bilgi.kod.StartsWith("P"))
        //    {
        //        string kodFormat = "P{0}{1}";
        //        bilgi.kod = string.Format(kodFormat, bilgi.kodTur.ToString().PadLeft(2, '0'), bilgi.kod);
        //    }

        //    TABDosyaPlani to = new TABDosyaPlani(ve);
        //    IDataReader r = to.ListeleDosyaTarihceSayisi(bilgi);

        //    if (r == null)
        //    {
        //        ve.VeriErisimKapat();
        //        hVAN.sonuc.hataStr = EvrakI.EVRI029;
        //        hVAN.sonuc.islemSonuc = false;
        //        return hVAN;
        //    }

        //    if (to.Hata() == "")
        //    {
        //        while (r.Read())
        //        {
        //            bilgi = new DosyaPlaniTarihce();

        //            //bilgi.islemKod = VeriErisim.DegerAl(r, 0, "");
        //            bilgi.kod = VeriErisim.DegerAl(r, 0, "");
        //            bilgi.acilanDosya = VeriErisim.DegerAl(r, 1, 0);
        //            bilgi.kapananDosya = VeriErisim.DegerAl(r, 2, 0);
        //            //bilgi.tarih = VeriErisim.DegerAl(r, 2);
        //            //bilgi.kolon = VeriErisim.DegerAl(r, 3, "");

        //            //bilgi.eskiDeger = VeriErisim.DegerAl(r, 4, "");
        //            //bilgi.yeniDeger = VeriErisim.DegerAl(r, 5, "");
        //            //bilgi.islem = TarihceIslemDondur(VeriErisim.DegerAl(r, 6, ""));
        //            //bilgi.kullaniciKodu = VeriErisim.DegerAl(r, 7, "");
        //            //bilgi.acilanDosya = VeriErisim.DegerAl(r, 8, "");
        //            //bilgi.kapananDosya = VeriErisim.DegerAl(r, 9, "");

        //            //bilgi.kodTur = BilgiTurBul(bilgi.kod);

        //            //bilgi.kolonAciklama = KolonAciklamaGetir(bilgi);
        //            //bilgi.islemAciklama = IslemAciklamaGetir(bilgi);
        //            //bilgi.eskiDeger = EskiDegerAciklamaGetir(bilgi);
        //            //bilgi.yeniDeger = YeniDegerAciklamaGetir(bilgi);

        //            hVAN.objeler.Add(bilgi);
        //        }
        //    }

        //    r.Close();

        //    ve.VeriErisimKapat();

        //    return hVAN;
        //}

        private static string YeniDegerAciklamaGetir(DosyaPlaniTarihce bilgi)
        {
            string donusDeger = string.Empty;
            switch (bilgi.kolon)
            {
                case "KULLANILMIYOR":
                    if (bilgi.yeniDeger == "1")
                        donusDeger = "AÇIK";
                    else if (bilgi.yeniDeger == "2")
                        donusDeger = "KAPALI";
                    break;
                default:
                    donusDeger = bilgi.yeniDeger;
                    break;
            }
            return donusDeger;
        }

        private static string EskiDegerAciklamaGetir(DosyaPlaniTarihce bilgi)
        {
            string donusDeger = string.Empty;
            switch (bilgi.kolon)
            {
                case "KULLANILMIYOR":
                    if (bilgi.eskiDeger == "1")
                        donusDeger = "AÇIK";
                    else if (bilgi.eskiDeger == "2")
                        donusDeger = "KAPALI";
                    break;
                default:
                    donusDeger = bilgi.eskiDeger;
                    break;
            }
            return donusDeger;
        }

        private static string IslemAciklamaGetir(DosyaPlaniTarihce bilgi)
        {
            string donusDeger = string.Empty;
            switch (bilgi.islem)
            {
                case ENUMDosyaPlaniTarihceIslem.TANIMSIZ:
                    donusDeger = "TANIMSIZ";
                    break;
                case ENUMDosyaPlaniTarihceIslem.YENIKAYIT:
                    donusDeger = "YENİ KAYIT";
                    break;
                case ENUMDosyaPlaniTarihceIslem.KAYDET:
                    donusDeger = "KAYDET";
                    break;
                case ENUMDosyaPlaniTarihceIslem.DEGISTIRILDI:
                    donusDeger = "DEĞİŞTİRİLDİ";
                    break;
                case ENUMDosyaPlaniTarihceIslem.SIL:
                    donusDeger = "SİLİNDİ";
                    break;
                default:
                    donusDeger = "TANIMSIZ";
                    break;
            }
            return donusDeger;
        }

        private static string KolonAciklamaGetir(DosyaPlaniTarihce bilgi)
        {
            string donusDeger = string.Empty;
            switch (bilgi.kolon)
            {
                case "KULLANILMIYOR":
                    if (bilgi.yeniDeger == "1")
                        donusDeger = "AÇILIŞ";
                    else if (bilgi.yeniDeger == "2")
                        donusDeger = "KAPANIŞ";
                    break;
                default:
                    donusDeger = bilgi.kolon;
                    break;
            }
            return donusDeger;
        }

        private static ENUMDosyaPlaniTarihceIslem TarihceIslemDondur(string islem)
        {
            if (islem == ENUMDosyaPlaniTarihceIslem.DEGISTIRILDI.ToString())
                return ENUMDosyaPlaniTarihceIslem.DEGISTIRILDI;
            if (islem == ENUMDosyaPlaniTarihceIslem.KAYDET.ToString())
                return ENUMDosyaPlaniTarihceIslem.KAYDET;
            if (islem == ENUMDosyaPlaniTarihceIslem.SIL.ToString())
                return ENUMDosyaPlaniTarihceIslem.SIL;
            if (islem == ENUMDosyaPlaniTarihceIslem.TANIMSIZ.ToString())
                return ENUMDosyaPlaniTarihceIslem.TANIMSIZ;
            if (islem == ENUMDosyaPlaniTarihceIslem.YENIKAYIT.ToString())
                return ENUMDosyaPlaniTarihceIslem.YENIKAYIT;
            return ENUMDosyaPlaniTarihceIslem.TANIMSIZ;
        }

        private static int BilgiTurBul(string kod)
        {
            int tur = 0;
            if (kod.StartsWith("P"))
            {
                tur = OrtakFonksiyonlar.ConvertToInt(kod.Substring(1, 2), 0);
            }
            return tur;
        }

        /// <summary>
        /// Birim kaydetme veya silme işlemi için yetki kontrolü yapan yordam.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgilerini içeren nesne</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">DosyaPlani nesne koleksiyonunu içeren nesne</param>
        /// <param name="yetkiTipi">"Yazma" veya "Silme" bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc KontrolEt(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste, string yetkiTipi, string seciliKod)
        {
            string hata = "";
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI030, "");
            string turFormat = "P{0}{1}";

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            if (liste != null && yetkiTipi == "Yazma")
            {
                foreach (DosyaPlani bilgi in liste.objeler)
                {
                    if (bilgi.kod == "")
                        hata += EvrakI.EVRI031 + "<br>";
                    if (bilgi.ad == "")
                        hata += bilgi.kod + " - " + EvrakI.EVRI032 + "<br>";

                    //if (!OrtakFonksiyonlar.KodYapiKontrol(kodYapisi, bilgi.kod.Replace(".", ""), false).islemSonuc)
                    //    hata += string.Format(EvrakI.EVRI033, kodYapisi) + "<br>";

                    //if (VarMi(null, bilgi, seciliKod).islemSonuc)         //Dosya kodu aynı olabileceğinden kapatildı.
                    //    hata += EvrakI.EVRI169 + "<br>";

                    if (bilgi.tur > 0)
                        bilgi.kod = string.Format(turFormat, bilgi.tur.ToString().PadLeft(2, '0'), bilgi.kod);
                }
            }
            else if (liste != null && yetkiTipi == "Silme")
            {
                TABDosyaPlani th = new TABDosyaPlani(ve);
                foreach (DosyaPlani bilgi in liste.objeler)
                {
                    if (bilgi.tur > 0)
                        bilgi.kod = string.Format(turFormat, bilgi.tur.ToString().PadLeft(2, '0'), bilgi.kod);

                    if (th.AnasiCocuguVarMi(bilgi.kod, true))
                        hata += "<br>" + string.Format(EvrakI.EVRI034, bilgi.kod);
                    else
                    {
                        if (th.KullanilmisMi(bilgi.kod))
                            hata += "<br>" + string.Format(EvrakI.EVRI035, bilgi.kod);
                    }

                    //if (bilgi.tur > 0)
                    //    bilgi.kod = bilgi.kod.Length > 2 ? bilgi.kod.Substring(3) : bilgi.kod;
                }
            }

            if (hata == "")
                sonuc.islemSonuc = true;
            else
                sonuc.hataStr = hata;

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Dosya planlarının seviye değerlerini düzenleyen yordam.
        /// </summary>
        /// <param name="veriErisim">>Veri erişim bilgilerini içeren nesne</param>       
        public static void DetaySayilariDuzenle(VeriErisim veriErisim)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            TABDosyaPlani to = new TABDosyaPlani(ve);
            to.DetaySayilariDuzenle("");
            ve.VeriErisimKapat();
        }

        /// <summary>
        /// Dosya planı bilgilerini kullanılmıyor olarak güncelleyen yordam.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgilerini içeren nesne</param>
        public static void KullanilmiyorYap(VeriErisim veriErisim)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            TABDosyaPlani to = new TABDosyaPlani(ve);
            to.KullanilmiyorYap("");
            ve.VeriErisimKapat();
        }

        /// <summary>
        /// Parametre olarak verilen dosya planının veritabanında mevcut olup olmadığını kontrol eden yordam.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgilerini içeren nesne</param>
        /// <param name="dosya">Dosya planı bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc VarMi(VeriErisim veriErisim, DosyaPlani dosya, string seciliKod)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI036, "");

            //Eğer ad diğer ile başlıyor ise kontrol etme
            string tmpAd = dosya.ad.Trim();
            string ad = tmpAd.ToUpper(new System.Globalization.CultureInfo("tr-TR"));
            if (ad.StartsWith("DİĞER")) dosya.ad = "";

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABDosyaPlani tab = new TABDosyaPlani(ve);
            if (tab.VarMi(dosya, seciliKod))
                sonuc.islemSonuc = true;

            if (dosya.ad == "") dosya.ad = tmpAd;
            ve.VeriErisimKapat();

            return sonuc;
        }

    }
}
