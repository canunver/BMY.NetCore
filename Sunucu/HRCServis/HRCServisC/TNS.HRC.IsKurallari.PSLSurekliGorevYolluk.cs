using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Sürekli görev yolluk belge bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class PSLSurekliGorevYolluk
    {
        private ObjectArray gundelikTanimList;
        const int yurtIciUstSinir = 60;
        const int yurtDisiUstSinir = 52;
        const int yurtIciArtis = 10;
        const int yurtDisiArtis = 8;
        const int kisiSabiti = 20;
        const decimal degiskenUnsurYurtDisi = ((decimal)0.007);

        /// <summary>
        /// Arguman olarak verilen, sürekli görev yolluk belge bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="sb">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, SurekliGorevYollukBelge sb, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc();

            //sonuc.islemSonuc = OMKYetkiKontrol.YazmaYetkisiVarMi(kullanan, sb.muhasebeKod, sb.harcamaKod, sb.ambarKod);
            //if (!sonuc.islemSonuc)
            //{
            //    sonuc.hataStr = "Belirtilen muhasebe, harcama, ambar kodları için yazma yetkiniz yoktur.";
            //    return sonuc;
            //}

            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////
            bool tamamlandi = true;
            int yil = sb.yil;
            string surecNo = sb.surecNo;
            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////

            sonuc = Kontrol(sb);

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            decimal gundelikDeger = 0;
            if (sb.emeklilikMaasiMi == (int)ENUMEvetHayir.HAYIR)
            {
                YollukGundelikTanim gundelikTanimKriter = new YollukGundelikTanim();
                if (sb.belgeTur == 1) //yurtiçi
                    gundelikTanimKriter.tur = ENUMYollukGundelikTuru.SUREKLIGECICI;
                else if (sb.belgeTur == 2)//yurtdışı
                    gundelikTanimKriter.tur = ENUMYollukGundelikTuru.GECICIYURTDISI;
                //gundelikTanimKriter.tur = (int)ENUMYollukGundelikTuru.GECICIYURTDISI;
                gundelikTanimKriter.yil = sb.gundelikYil;
                gundelikTanimList = (new PSLSurekliGorevGundelikTanim()).Listele(kullanan, gundelikTanimKriter);
                gundelikDeger = GundelikTanimYevmiyeDegeri(sb.gundelikKod);
                sb.yerDegistirmeGideri = sb.yerDegisMesafe * gundelikDeger * sb.degiskenUnsur;
                sb.yerDegistirmeGideri = Math.Round(sb.yerDegistirmeGideri, 2);
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSurekliGorevYolluk TABSurekliGorevYolluk = new TABSurekliGorevYolluk(ve);
            TABGenel tabGenel = new TABGenel(ve);

            ve.BeginTransaction();

            sonuc = TABSurekliGorevYolluk.Kaydet(sb);

            if (!sonuc.islemSonuc)
            {
                ve.Rollback();
                ve.VeriErisimKapat();
                return sonuc;
            }


            if (sb.emeklilikMaasiMi == (int)ENUMEvetHayir.HAYIR)
            {
                //Bu belgeye ait satırları sil

                SurekliGorevYollukDetay gdSil = new SurekliGorevYollukDetay();

                gdSil.yil = sb.yil;
                gdSil.muhasebeKod = sb.muhasebeKod;
                gdSil.birimKod = sb.birimKod;
                gdSil.surecNo = sb.surecNo;
                gdSil.belgeTur = sb.belgeTur;
                gdSil.tcKimlikNoBelge = sb.tcKimlikNo;

                sonuc = SilDetay(ve, kullanan, gdSil);

                if (!sonuc.islemSonuc)
                {
                    ve.Rollback();
                    ve.VeriErisimKapat();
                    return sonuc;
                }

                int sabitUnsurKatsayiToplamı = 0;
                int sabitUnsurArtis = 0;
                int ustSinir = 0;
                if (sb.belgeTur == (int)ENUMSurekliGorevTuru.YURTICI)
                {
                    ustSinir = yurtIciUstSinir;
                    sabitUnsurArtis = yurtIciArtis;
                }
                else if (sb.belgeTur == (int)ENUMSurekliGorevTuru.YURTDISI)
                {
                    ustSinir = yurtDisiUstSinir;
                    sabitUnsurArtis = yurtDisiArtis;
                    sb.degiskenUnsur = degiskenUnsurYurtDisi;
                }

                foreach (SurekliGorevYollukDetay kisi in sb.detay.objeler)
                {
                    if (kisi.akrabalikKodu == (int)ENUMAkrabalikDerecesi.KENDISI)
                    {
                        sabitUnsurKatsayiToplamı += kisiSabiti;
                        kisi.toplamTutar += sb.yerDegistirmeGideri;
                        kisi.toplamTutar += sb.biletUcret;
                        kisi.toplamTutar += sb.gunSayi * gundelikDeger;
                        kisi.sabitUnsur = gundelikDeger * kisiSabiti;
                        kisi.toplamTutar += kisi.sabitUnsur;

                    }
                    else
                    {
                        kisi.toplamTutar += sb.biletUcret;
                        kisi.toplamTutar += sb.gunSayi * gundelikDeger;

                        sabitUnsurKatsayiToplamı += sabitUnsurArtis;

                        if (sabitUnsurKatsayiToplamı <= ustSinir)
                        {
                            kisi.sabitUnsur = gundelikDeger * sabitUnsurArtis;
                            kisi.toplamTutar += kisi.sabitUnsur;
                        }

                    }

                    if (sb.dovizKuru != 0)
                    {
                        kisi.toplamTutarDoviz = kisi.toplamTutar;
                        kisi.toplamTutar = kisi.toplamTutar * sb.dovizKuru;
                        kisi.toplamTutarDoviz = Math.Round(kisi.toplamTutarDoviz, 2);
                    }

                    kisi.toplamTutar = Math.Round(kisi.toplamTutar, 2);
                    kisi.sabitUnsur = Math.Round(kisi.sabitUnsur, 2);

                }

                //Kaydetme Kısmı
                int sira = 0;
                foreach (SurekliGorevYollukDetay sDetay in sb.detay.objeler)
                {
                    sira++;
                    sDetay.yil = sb.yil;
                    sDetay.muhasebeKod = sb.muhasebeKod;
                    sDetay.birimKod = sb.birimKod;
                    sDetay.surecNo = sb.surecNo;
                    sDetay.belgeTur = sb.belgeTur;
                    sDetay.tcKimlikNoBelge = sb.tcKimlikNo;
                    sDetay.siraNo = sira;
                    sonuc = KaydetDetay(ve, kullanan, sDetay);

                    if (!sonuc.islemSonuc)
                    {
                        ve.Rollback();
                        ve.VeriErisimKapat();
                        return sonuc;
                    }
                }
            }

            //Süreç devam ediyor olarak işaretle.
            ///////////////////////////////////////////////////////////////////////////
            if (sonuc.hataStr == "" && surecNo != "-1")
                sonuc = new HRCSurec().SurecDurumKaydet(null, kullanan, yil, surecNo, ENUMSurecDurum.DEVAMEDIYOR);
            ///////////////////////////////////////////////////////////////////////////

            //Parca Adım devam ediyor olarak işaretle.
            ///////////////////////////////////////////////////////////////////////////
            if (sonuc.hataStr == "" && surecNo != "-1")
                sonuc = new HRCSurec().ParcaDurumKaydet(null, kullanan, yil, surecNo, islemKod, parcaKod, ENUMSurecParcaDurum.DEVAMEDIYOR, kullanan.kullaniciKodu);
            ///////////////////////////////////////////////////////////////////////////

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = sb.surecNo;
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sürekli görev yolluk belge bilgisini veritabanından okur 
        /// ve SurekliGorevYollukBelge nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islem yapan.</param>
        /// <param name="sb">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sürekli görev yolluk belge bilgisini SurekliGorevYollukBelge nesnesi olarak tutan nesne</returns>
        public SurekliGorevYollukBelge Listele(Kullanici kullanan, SurekliGorevYollukBelge sb)
        {
            SurekliGorevYollukBelge bilgi = new SurekliGorevYollukBelge();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSurekliGorevYolluk tabSurekliGorevYolluk = new TABSurekliGorevYolluk(ve);

            IDataReader dr = tabSurekliGorevYolluk.Listele(sb);

            if (dr.Read())
            {
                bilgi.yil = VeriErisim.DegerAl(dr, 0, 0);
                bilgi.muhasebeKod = VeriErisim.DegerAl(dr, 1, 0);
                bilgi.birimKod = VeriErisim.DegerAl(dr, 2, "");
                bilgi.surecNo = VeriErisim.DegerAl(dr, 3, "");
                bilgi.belgeTur = VeriErisim.DegerAl(dr, 4, 0);
                bilgi.tcKimlikNo = VeriErisim.DegerAl(dr, 5, "");
                bilgi.kadroEk = VeriErisim.DegerAl(dr, 6, "");
                bilgi.gundelikKod = VeriErisim.DegerAl(dr, 7, "");
                bilgi.nereden = VeriErisim.DegerAl(dr, 8, "");
                bilgi.nereye = VeriErisim.DegerAl(dr, 9, "");
                bilgi.atamaTarih = VeriErisim.DegerAl(dr, 10);
                bilgi.tasitCesit = VeriErisim.DegerAl(dr, 11, "");
                bilgi.biletUcret = (decimal)VeriErisim.DegerAl(dr, 12, 0.0);
                bilgi.gunSayi = VeriErisim.DegerAl(dr, 13, 0);
                bilgi.gundelikTutar = (decimal)VeriErisim.DegerAl(dr, 14, 0.0);
                bilgi.yerDegisMesafe = VeriErisim.DegerAl(dr, 15, 0);
                bilgi.yerDegisTutari = (decimal)VeriErisim.DegerAl(dr, 16, 0.0);
                bilgi.dovizKodu = VeriErisim.DegerAl(dr, 17, 0);
                bilgi.dovizKuru = (decimal)VeriErisim.DegerAl(dr, 18, 0.0);
                bilgi.degiskenUnsur = (decimal)VeriErisim.DegerAl(dr, 19, 0.0);
                bilgi.yerDegistirmeGideri = (decimal)VeriErisim.DegerAl(dr, 20, 0.0);
                bilgi.unvanKodu = VeriErisim.DegerAl(dr, 21, 0);
                bilgi.bildirimTarihi = VeriErisim.DegerAl(dr, 22);
                bilgi.odemeEmriNo = VeriErisim.DegerAl(dr, 23, "");
                bilgi.emeklilikMaasiMi = VeriErisim.DegerAl(dr, 24, 0);
                bilgi.emeklilikMaasi = (decimal)VeriErisim.DegerAl(dr, 25, 0.0);
                bilgi.gundelikYil = VeriErisim.DegerAl(dr, 26, 0);
                bilgi.adSoyad = VeriErisim.DegerAl(dr, 27, "") + "  " + VeriErisim.DegerAl(dr, 28, "");
                bilgi.unvanAdi = VeriErisim.DegerAl(dr, 29, "");
                bilgi.gundelikDeger = (decimal)VeriErisim.DegerAl(dr, 30, 0.0);
            }
            dr.Close();
            bilgi.detay = DetayListele(ve, kullanan, sb);
            ve.VeriErisimKapat();
            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sürekli görev yolluk belge bilgisine ait 
        /// ödeme emri belge numarası bilgisini veritabanından okur 
        /// ve SurekliGorevYollukBelge nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak süreç yıl bilgisi.</param>
        /// <param name="surecNo">İşlem yapılacak süreç no bilgisi.</param>
        /// <returns>İstenilen kriterdeki, sürekli görev yolluk belge bilgisine ait 
        /// ödeme emri belge numarası bilgisi listesini SurekliGorevYollukBelge nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleOdemeEmriNo(int yil, string surecNo)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));
            TABSurekliGorevYolluk toSurekliGorevYolluk = new TABSurekliGorevYolluk(ve);

            IDataReader dr = toSurekliGorevYolluk.ListeleOdemeEmri(yil, surecNo);

            while (dr.Read())
            {
                SurekliGorevYollukBelge sd = new SurekliGorevYollukBelge();
                sd.odemeEmriNo = VeriErisim.DegerAl(dr, 0, "");
                sd.yil = VeriErisim.DegerAl(dr, 1, 0);

                bilgi.Ekle(sd);
            }
            dr.Close();

            if (bilgi.ObjeSayisi == 0)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00287;
            }
            else
                bilgi.sonuc.islemSonuc = true;

            ve.VeriErisimKapat();
            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen, sürekli görev yolluk bilgisine ait ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc OdemeEmriNoKaydet(Kullanici kullanan, SurekliGorevYollukBelge kriter)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABSurekliGorevYolluk to = new TABSurekliGorevYolluk(ve);
            ve.BeginTransaction();

            sonuc = to.OdemeEmriNoKaydet(kriter);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, sürekli görev yolluk belgesini, sürekli görev yolluk belge havuzundan siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemYapan">The islemi yapan.</param>
        /// <param name="sb">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HavuzSil(Kullanici islemYapan, SurekliGorevYollukBelge sb)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));

            TABSurekliGorevYolluk toGorevYolluk = new TABSurekliGorevYolluk(ve);
            TABGenel tabGenel = new TABGenel(ve);

            ve.BeginTransaction();

            sonuc = toGorevYolluk.HavuzSil(sb);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sürekli görev yolluk istatistik bilgisini veritabanından okur 
        /// ve PSLYollukIstatistikSinif nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="sb">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sürekli görev yolluk istatistik bilgisi listesini PSLYollukIstatistikSinif nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleIstatistikKurlu(Kullanici kullanan, PSLYollukIstatistikSinif sb)
        {
            ObjectArray donenListe = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSurekliGorevYolluk TABSurekliGorevYolluk = new TABSurekliGorevYolluk(ve);

            IDataReader dr = TABSurekliGorevYolluk.ListeleIstatistikKurlu(sb);

            PSLYollukIstatistikSinif yollukIst = new PSLYollukIstatistikSinif();

            while (dr.Read())
            {
                yollukIst = new PSLYollukIstatistikSinif();
                yollukIst.il = VeriErisim.DegerAl(dr, 0, 0);
                yollukIst.yollukTip = VeriErisim.DegerAl(dr, 1, 0);
                yollukIst.perSayi = VeriErisim.DegerAl(dr, 2, 0);
                yollukIst.yolDoviz = VeriErisim.DegerAl(dr, 3, 0.0);
                yollukIst.yol = VeriErisim.DegerAl(dr, 4, 0.0);
                yollukIst.islemYapanSayi = VeriErisim.DegerAl(dr, 5, 0);
                yollukIst.islemSayisi = VeriErisim.DegerAl(dr, 6, 0);
                donenListe.objeler.Add(yollukIst);
            }
            dr.Close();


            ve.VeriErisimKapat();
            return donenListe;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sürekli görev yolluk belge bilgisini veritabanından okur 
        /// ve SurekliGorevYollukBelge nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islem yapan.</param>
        /// <param name="sb">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sürekli görev yolluk belge bilgisini SurekliGorevYollukBelge nesnesi olarak tutan nesne</returns>
        public SurekliGorevYollukBelge OncedenKayitliMi(Kullanici kullanan, SurekliGorevYollukBelge sb)
        {
            SurekliGorevYollukBelge bilgi = null;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSurekliGorevYolluk TABSurekliGorevYolluk = new TABSurekliGorevYolluk(ve);

            IDataReader dr = TABSurekliGorevYolluk.OncedenKayitliMi(sb);

            while (dr.Read())
            {
                bilgi = new SurekliGorevYollukBelge();
                bilgi.yil = VeriErisim.DegerAl(dr, 0, 0);
                bilgi.muhasebeKod = VeriErisim.DegerAl(dr, 1, 0);
                bilgi.birimKod = VeriErisim.DegerAl(dr, 2, "");
                bilgi.surecNo = VeriErisim.DegerAl(dr, 3, "");
                bilgi.belgeTur = VeriErisim.DegerAl(dr, 4, 0);
                bilgi.tcKimlikNo = VeriErisim.DegerAl(dr, 5, "");
                bilgi.atamaTarih = VeriErisim.DegerAl(dr, 6);
                break;
            }
            dr.Close();
            ve.VeriErisimKapat();
            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sürekli görev yolluk toplam yevmiye tutar bilgisini veritabanından okur 
        /// ve double olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="belgeTur">İşlem yapılacak belge tur bilgisi (Yurt içi, Yurt dışı).</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İstenilen kriterdeki, sürekli görev yolluk toplam yevmiye tutar bilgisini double olarak döner</returns>
        public double ToplamYevmiyeTutarGetir(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMSurekliGorevTuru belgeTur, bool sadeceIl)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSurekliGorevYolluk to = new TABSurekliGorevYolluk(ve);
            double deger = to.ToplamYevmiyeTutar(yil, muhasebeKod, birimKod, belgeTur, sadeceIl);
            return deger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sürekli görev yolluk ortalama mesafe bilgisini veritabanından okur 
        /// ve double olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="belgeTur">İşlem yapılacak belge tur bilgisi (Yurt içi, Yurt dışı).</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İstenilen kriterdeki, sürekli görev yolluk ortalama mesafe bilgisini double olarak döner</returns>
        public double OrtalamaMesafeGetir(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMSurekliGorevTuru belgeTur, bool sadeceIl)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSurekliGorevYolluk to = new TABSurekliGorevYolluk(ve);
            double deger = to.OrtalamaMesafe(yil, muhasebeKod, birimKod, belgeTur, sadeceIl);
            return deger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sürekli görev yolluk toplam adam sayısı bilgisini veritabanından okur 
        /// ve double olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="belgeTur">İşlem yapılacak belge tur bilgisi (Yurt içi, Yurt dışı).</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İstenilen kriterdeki, sürekli görev yolluk toplam adam sayısı bilgisini double olarak döner</returns>
        public double ToplamAdamSayisiGetir(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMSurekliGorevTuru belgeTur, bool sadeceIl)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSurekliGorevYolluk to = new TABSurekliGorevYolluk(ve);
            double deger = to.ToplamAdamSayisi(yil, muhasebeKod, birimKod, belgeTur, sadeceIl);
            return deger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, tayin ve emeklilik için düzenlenen sürekli görev yolluk gerçekleşme bilgisini veritabanından okur 
        /// ve (tür adı, kişi sayısı, ödenen tutar) nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="belgeTur">İşlem yapılacak belge tur bilgisi (Yurt içi, Yurt dışı).</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İstenilen kriterdeki, tayin ve emeklilik için düzenlenen sürekli görev yolluk gerçekleşme bilgisini
        /// (tür adı, kişi sayısı, ödenen tutar) nesne koleksiyonu olarak döner</returns>
        public ObjectArray ToplamGerceklesmeBilgileriTayinEmekliListele(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMSurekliGorevTuru belgeTur, bool sadeceIl)
        {
            ObjectArray donenListe = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSurekliGorevYolluk to = new TABSurekliGorevYolluk(ve);
            IDataReader dr = to.ToplamTayinBilgileriListele(yil, muhasebeKod, birimKod, belgeTur, sadeceIl);
            ObjectArray liste;
            while (dr.Read())
            {
                liste = new ObjectArray();
                liste.objeler.Add("TAYIN");
                liste.objeler.Add(VeriErisim.DegerAl(dr, 0, 0));
                liste.objeler.Add(VeriErisim.DegerAl(dr, 1, 0.0));
                donenListe.objeler.Add(liste);
            }
            dr.Close();

            IDataReader dr2 = to.ToplamEmekliBilgileriListele(yil, muhasebeKod, birimKod, belgeTur, sadeceIl);
            while (dr2.Read())
            {
                liste = new ObjectArray();
                liste.objeler.Add("EMEKLI");
                liste.objeler.Add(VeriErisim.DegerAl(dr2, 0, 0));
                liste.objeler.Add(VeriErisim.DegerAl(dr2, 1, 0));
                donenListe.objeler.Add(liste);
            }
            dr2.Close();
            ve.VeriErisimKapat();

            return donenListe;
        }

        /// <summary>
        /// Belirtilen parametrelere ait Harcama Yönetimi sisteminde gerçekleşen verileri döndürür.
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Gerçekleşmenin yılı</param>
        /// <param name="muhasebeKod">Gerçekleşmenin yapıldığı muhasebeKod</param>
        /// <param name="birimKod">Gerçekleşmenin yapıldığı birimKod</param>
        /// <param name="belgeTur">Gerçekleşmenin türü (Yurt içi, Yurt dışı)</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İşlem sonucunu döndüren, ObjectArray sınıfında nesne içinde ObjectArrayler içerir ve her bir ObjectArray içerisinde sırasıyla Gerçekleşme türü (TAYIN,EMEKLI), Adam Sayısı ve Ödeme Tutarı bilgileri bulunur.</returns>
        public ObjectArray ToplamGerceklesmeBilgileriListele(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMSurekliGorevTuru belgeTur, bool sadeceIl)
        {
            ObjectArray donenListe = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSurekliGorevYolluk to = new TABSurekliGorevYolluk(ve);
            IDataReader dr = to.ToplamGerceklesmeListele(yil, muhasebeKod, birimKod, belgeTur, sadeceIl);
            ObjectArray liste;
            while (dr.Read())
            {
                liste = new ObjectArray();
                liste.objeler.Add(VeriErisim.DegerAl(dr, 0, ""));
                liste.objeler.Add(VeriErisim.DegerAl(dr, 1, 0));
                liste.objeler.Add(VeriErisim.DegerAl(dr, 2, 0.0));
                donenListe.objeler.Add(liste);
            }
            dr.Close();
            ve.VeriErisimKapat();

            return donenListe;
        }


        private decimal GundelikTanimYevmiyeDegeri(string kod)
        {

            foreach (YollukGundelikTanim yevmiye in gundelikTanimList.objeler)
            {
                if (yevmiye.kod == kod)
                    return (decimal)yevmiye.deger;
            }
            return decimal.Zero;

        }

        private Sonuc SilDetay(VeriErisim ve, Kullanici kullanan, SurekliGorevYollukDetay sDetay)
        {
            Sonuc sonuc = new Sonuc();

            TABSurekliGorevYolluk tabDetaySil = new TABSurekliGorevYolluk(ve);

            sonuc = tabDetaySil.SilDetay(sDetay);

            return sonuc;
        }

        private Sonuc KaydetDetay(VeriErisim ve, Kullanici kullanan, SurekliGorevYollukDetay sDetay)
        {
            Sonuc sonuc = new Sonuc();

            TABSurekliGorevYolluk tabDetay = new TABSurekliGorevYolluk(ve);

            sonuc = tabDetay.KaydetDetay(sDetay);

            return sonuc;
        }

        private ObjectArray DetayListele(VeriErisim veriErisim, Kullanici kullanan, SurekliGorevYollukBelge sb)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSurekliGorevYolluk tabSurekliGorevYollukDetay = new TABSurekliGorevYolluk(ve);

            IDataReader dr = tabSurekliGorevYollukDetay.ListeleDetay(sb);

            while (dr.Read())
            {
                SurekliGorevYollukDetay sDetay = new SurekliGorevYollukDetay();

                sDetay.yil = VeriErisim.DegerAl(dr, 0, 0);
                sDetay.muhasebeKod = VeriErisim.DegerAl(dr, 1, 0);
                sDetay.birimKod = VeriErisim.DegerAl(dr, 2, "");
                sDetay.surecNo = VeriErisim.DegerAl(dr, 3, "");
                sDetay.belgeTur = VeriErisim.DegerAl(dr, 4, 0);
                sDetay.tcKimlikNoBelge = VeriErisim.DegerAl(dr, 5, "");
                sDetay.siraNo = VeriErisim.DegerAl(dr, 6, 0);
                sDetay.tcKimlikNo = VeriErisim.DegerAl(dr, 7, "");
                sDetay.adSoyad = VeriErisim.DegerAl(dr, 8, "");
                sDetay.akrabalikKodu = VeriErisim.DegerAl(dr, 9, 0);
                sDetay.sabitUnsur = (decimal)VeriErisim.DegerAl(dr, 10, 0.0);
                sDetay.toplamTutar = (decimal)VeriErisim.DegerAl(dr, 11, 0.0);
                sDetay.toplamTutarDoviz = (decimal)VeriErisim.DegerAl(dr, 12, 0.0);

                bilgi.Ekle(sDetay);
            }
            dr.Close();

            if (bilgi.ObjeSayisi == 0)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00287;
            }
            else
                bilgi.sonuc.islemSonuc = true;

            ve.VeriErisimKapat();
            return bilgi;
        }

        private Sonuc Kontrol(SurekliGorevYollukBelge sb)
        {
            Sonuc sonuc = new Sonuc();

            //Belge Kontrol
            if (sb.yil == 0)
                sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00003 + "<br>";
            if (sb.muhasebeKod == 0)
                sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00004 + "<br>";
            if (sb.birimKod == "")
                sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00005 + "<br>";
            if (sb.tcKimlikNo == "")
                sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00236 + "<br>";
            if (sb.belgeTur == 0)
                sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00288 + "<br>";
            if (sb.kadroEk == "")
                sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00289 + "<br>";
            if (sb.emeklilikMaasiMi == 0)
            {
                if (sb.gundelikKod == "")
                    sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00290 + "<br>";
            }
            if (sb.bildirimTarihi.isNull)
                sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00291 + "<br>";

            return sonuc;
        }
    }
}
