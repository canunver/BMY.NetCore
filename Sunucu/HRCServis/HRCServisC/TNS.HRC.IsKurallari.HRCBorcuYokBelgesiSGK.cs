using System;
using TNS.KYM;
using TNS.Veri;
using TNS.HRC.IsKurallari.Veri;
using System.Data;
using HRCServisC.tr.gov.sgb.uygulama.gib;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Borcu yok belge bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCBorcuYokBelgesiSGK
    {
        /// <summary>
        /// Arguman olarak verilen, borcu yok belge bilgisini veritabanına kaydeder
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="belge">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="icMi">if set to <c>true</c> [ic mi].</param>
        /// <returns>
        /// İşlem sonucunu döndüren, Sonuc sınıfında nesne
        /// </returns>
        public Sonuc Kaydet(Kullanici kullanan, BorcuYokBelgesiSGK belge)
        {
            Sonuc sonuc = new Sonuc();

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            if (!HRCYetkiKontrol.YoneticiMi(kullanan))
            {
                if (!HRCYetkiKontrol.MuhasebeBirimYetkiVarMi(kullanan, belge.muhasebe.ToString(), ""))
                {
                    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00058;
                    sonuc.islemSonuc = false;
                    return sonuc;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABBorcuYokBelgesiSGK tabBelge = new TABBorcuYokBelgesiSGK(ve);

            TABGenel tabGenel = new TABGenel(ve);
            ve.BeginTransaction();

            if (belge.kod == 0)
            {
                string sayacKod = "BORCUYOKTURSGKID";
                belge.kod = OrtakFonksiyonlar.ConvertToInt(tabGenel.BelgeNoAl(sayacKod, "0"), 0);
                sonuc = tabBelge.Kaydet(belge);
            }
            else
            {
                sonuc.hataStr = "Kod bilgisi boş gelmeli.";
                return sonuc;
            }
            //kaydet bitti--------------->

            if (sonuc.islemSonuc)
            {
                string kurumKod = System.Configuration.ConfigurationManager.AppSettings.Get("GelirlerKurumAd");
                sonuc = SGKLogKaydet(belge.vergiNo, belge.islemYapan, belge.sorguTip, belge.refNo, belge.hata, belge.hataMesaj, kurumKod);

                if (sonuc.islemSonuc)
                {
                    sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                    sonuc.anahtar = belge.kod.ToString();
                    ve.Commit();
                }
                else
                {
                    sonuc.hataStr = "Log kaydederken hata oluştu.";
                    ve.Rollback();
                }
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
        /// Arguman olarak verilen kriterlere bağlı olarak, borcu yok belge bilgisini veritabanından okur
        /// ve BorcuYokBelgesi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <param name="icMi">if set to <c>true</c> [ic mi].</param>
        /// <returns>
        /// İstenilen kriterdeki, borcu yok belge bilgisi listesini BorcuYokBelgesi nesne koleksiyonu olarak tutan nesne
        /// </returns>
        public ObjectArray Listele(Kullanici kullanan, BorcuYokBelgesiSGK kriter)
        {
            ObjectArray donenListe = new ObjectArray();

            if (!HRCYetkiKontrol.YoneticiMi(kullanan))
            {
                if (!HRCYetkiKontrol.MuhasebeBirimYetkiVarMi(kullanan, kriter.muhasebe.ToString(), ""))
                {
                    donenListe.sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00060;
                    donenListe.sonuc.islemSonuc = false;

                    return donenListe;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABBorcuYokBelgesiSGK tabBelge = new TABBorcuYokBelgesiSGK(ve);

            IDataReader dr = tabBelge.Listele(kriter);

            if (tabBelge.Hata() == "")
            {
                while (dr.Read())
                {
                    BorcuYokBelgesiSGK belge = new BorcuYokBelgesiSGK(0);

                    belge.kod = VeriErisim.DegerAl(dr, 0, 0);
                    belge.muhasebe = VeriErisim.DegerAl(dr, 1, 0);
                    belge.refNo = VeriErisim.DegerAl(dr, 2, string.Empty);
                    belge.vergiNo = VeriErisim.DegerAl(dr, 3, string.Empty);
                    belge.sorguTip = VeriErisim.DegerAl(dr, 4, 0);
                    belge.durum = VeriErisim.DegerAl(dr, 5, 0);
                    belge.tarih = VeriErisim.DegerAl(dr, 6);
                    belge.kayitTarih = VeriErisim.DegerAl(dr, 7);
                    belge.islemYapan = VeriErisim.DegerAl(dr, 8, string.Empty);
                    donenListe.Ekle(belge);
                }
            }
            dr.Close();

            ve.VeriErisimKapat();

            return donenListe;
        }

        /// <summary>
        /// Gelirler İdaresi Başkanlığından alınan servislerini kullanma bilgilerini veritabanına kaydeder
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="sorgulanan">İşlem yapılacak sorgulanan bilgisi.</param>
        /// <param name="islemYapan">İşlem yapılacak işlem yapan kullanıcı adı bilgisi.</param>
        /// <param name="sorguTipi">The sorgu tipi.</param>
        /// <param name="refNo">The ref no.</param>
        /// <param name="hataKodu">The hata kodu.</param>
        /// <param name="hataMesaji">The hata mesaji.</param>
        /// <param name="kurum">İşlem yapılacak kurum bilgisi.</param>
        /// <returns>
        /// İşlem sonucunu döndüren, Sonuc sınıfında nesne
        /// </returns>
        public Sonuc SGKLogKaydet(string sorgulanan, string islemYapan, int sorguTipi, string refNo, int hataKodu, string hataMesaji, string kurum)
        {
            Sonuc sonuc = new Sonuc();

            //VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            //TABBorcuYokBelgesiSGK tabBelge = new TABBorcuYokBelgesiSGK(ve);

            //ve.BeginTransaction();

            //sonuc = tabBelge.SGKLogKaydet(sorgulanan, islemYapan, sorguTipi, refNo, hataKodu, hataMesaji, kurum);

            //if (sonuc.islemSonuc)
            //{
            //    sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
            //    ve.Commit();
            //}
            //else
            //{
            //    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
            //    ve.Rollback();
            //}

            //ve.VeriErisimKapat();

            return sonuc;
        }


        /// <summary>
        /// Arguman olarak verilen, borcu belge durum bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="belge">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc BelgeDurumDegistir(Kullanici kullanan, BorcuYokBelgesiSGK belge)
        {
            Sonuc sonuc = new Sonuc();

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            if (!HRCYetkiKontrol.YoneticiMi(kullanan))
            {
                if (!HRCYetkiKontrol.MuhasebeBirimYetkiVarMi(kullanan, belge.muhasebe.ToString(), ""))
                {
                    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00058;
                    sonuc.islemSonuc = false;
                    return sonuc;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABBorcuYokBelgesiSGK tabBelge = new TABBorcuYokBelgesiSGK(ve);

            ve.BeginTransaction();

            belge.tarih = new TNSDateTime(DateTime.Now.ToString());

            sonuc = tabBelge.BelgeDurumDegistir(belge);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                sonuc.anahtar = belge.kod.ToString();
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


        private ObjectArray ListeleSonucBekleyen()
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", null);

            TABBorcuYokBelgesiSGK tabBelge = new TABBorcuYokBelgesiSGK(ve);


            ObjectArray liste = new ObjectArray();

            BorcuYokBelgesiSGK kriter = new BorcuYokBelgesiSGK(0);
            kriter.durum = (int)ENUMSGKBelgeDurum.YENI;
            IDataReader dr = tabBelge.Listele(kriter);

            if (tabBelge.Hata() == "")
            {
                while (dr.Read())
                {
                    BorcuYokBelgesiSGK belge = new BorcuYokBelgesiSGK(0);

                    belge.kod = VeriErisim.DegerAl(dr, 0, 0);
                    belge.muhasebe = VeriErisim.DegerAl(dr, 1, 0);
                    belge.refNo = VeriErisim.DegerAl(dr, 2, string.Empty);
                    belge.vergiNo = VeriErisim.DegerAl(dr, 3, string.Empty);
                    belge.sorguTip = VeriErisim.DegerAl(dr, 4, 0);
                    belge.durum = VeriErisim.DegerAl(dr, 5, 0);
                    belge.tarih = VeriErisim.DegerAl(dr, 6);
                    belge.kayitTarih = VeriErisim.DegerAl(dr, 7);
                    belge.islemYapan = VeriErisim.DegerAl(dr, 8, string.Empty);
                    liste.Ekle(belge);
                }
            }
            dr.Close();

            ve.VeriErisimKapat();

            return liste;
        }

        public Sonuc BorcuYokSGKBelgeSonucSorgula()
        {
            Sonuc sonuc = new Sonuc(false,"hata oluştu.","");

            ObjectArray belgeListe = ListeleSonucBekleyen();

            foreach (BorcuYokBelgesiSGK belge in belgeListe.objeler)
            {
                //1.servis ile sonucu sorgula
                //2.db ye sonuç kaydet
            }

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, borcu yok belge bilgisini veritabanına kaydeder
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="belge">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>
        /// İşlem sonucunu döndüren, Sonuc sınıfında nesne
        /// </returns>
        public Sonuc KaydetSonuc(Kullanici kullanan, BorcuYokBelgesiSGKSonuc belge)
        {
            Sonuc sonuc = new Sonuc();

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABBorcuYokBelgesiSGK tabBelge = new TABBorcuYokBelgesiSGK(ve);

            TABGenel tabGenel = new TABGenel(ve);
            ve.BeginTransaction();

            sonuc = tabBelge.KaydetSonuc(belge);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                sonuc.anahtar = belge.kod.ToString();
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
        /// Arguman olarak verilen kriterlere bağlı olarak, borcu yok belge bilgisini veritabanından okur
        /// ve BorcuYokBelgesi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>
        /// İstenilen kriterdeki, borcu yok belge bilgisi listesini BorcuYokBelgesi nesne koleksiyonu olarak tutan nesne
        /// </returns>
        public ObjectArray ListeleSonuc(Kullanici kullanan, BorcuYokBelgesiSGKSonuc kriter)
        {
            ObjectArray donenListe = new ObjectArray();

            if (!HRCYetkiKontrol.YoneticiMi(kullanan))
            {
                //if (!HRCYetkiKontrol.MuhasebeBirimYetkiVarMi(kullanan, kriter.muhasebe.ToString(), ""))
                //{
                //    donenListe.sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00060;
                //    donenListe.sonuc.islemSonuc = false;

                //    return donenListe;
                //}
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABBorcuYokBelgesiSGK tabBelge = new TABBorcuYokBelgesiSGK(ve);

            IDataReader dr = tabBelge.ListeleSonuc(kriter);

            if (tabBelge.Hata() == "")
            {
                while (dr.Read())
                {
                    BorcuYokBelgesiSGKSonuc belge = new BorcuYokBelgesiSGKSonuc();

                    belge.kod = VeriErisim.DegerAl(dr, 0, 0);
                    belge.refNo = VeriErisim.DegerAl(dr, 1, string.Empty);
                    belge.damgaBorc = VeriErisim.DegerAl(dr, 2, 0.0);
                    belge.ekpBorc = VeriErisim.DegerAl(dr, 3, 0.0);
                    belge.ipcBorc = VeriErisim.DegerAl(dr, 4, 0.0);
                    belge.issizlikBorc = VeriErisim.DegerAl(dr, 5, 0.0);
                    belge.oivBorc = VeriErisim.DegerAl(dr, 6, 0.0);
                    belge.primBorc = VeriErisim.DegerAl(dr, 7, 0.0);

                    donenListe.Ekle(belge);
                }
            }
            dr.Close();

            ve.VeriErisimKapat();

            return donenListe;
        }



        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sırada bekleyen borcu yok belgesini bir sonraki duruma taşır
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SiradaBekleyenleriGonder(BorcuYokBelgesi kriter)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            //ObjectArray oListe = Listele(null, kriter, true);
            //GIBServis servisGib = new GIBServis();

            //string kurumKod = System.Configuration.ConfigurationManager.AppSettings.Get("GelirlerKurumAd");

            //foreach (BorcuYokBelgesi dilekce in oListe.objeler)
            //{
            //    BorcuYokturKaydetOutput dko = new BorcuYokturKaydetOutput();
            //    BorcuYokturKaydetInput dki = new BorcuYokturKaydetInput();

            //    dki.dilekceBitisTarihi = dilekce.bitisTarihi.Oku().ToString("yyyyMMdd");
            //    dki.yaziTalepNedeni = dilekce.talepNedeni.ToString();

            //    if (dilekce.vergiKimlikNo.Length == 10)
            //        dki.vergiKimlikNumarasi = dilekce.vergiKimlikNo;
            //    else if (dilekce.vergiKimlikNo.Length == 11)
            //        dki.tcKimlikNumarasi = dilekce.vergiKimlikNo;

            //    try
            //    {
            //        dko = servisGib.BorcuYokDilekceKaydet(dki, dilekce.islemYapan, kurumKod);
            //    }
            //    catch (Exception ex)
            //    {
            //        throw ex;
            //    }

            //    dilekce.cevapKoduGeldi = dko.basarili;
            //    if (dko.basarili)
            //    {
            //        dilekce.durum = (int)ENUMBorcuYokDurum.GONDERILDI;
            //        dilekce.cevapId = dko.ID;
            //        Kullanici kul = new Kullanici();
            //        kul.mernis = dilekce.islemYapan;
            //        sonuc = Kaydet(kul, dilekce, true);
            //    }
            //    else
            //    {
            //        Kullanici kul = new Kullanici();
            //        kul.mernis = dilekce.islemYapan;
            //        dilekce.hataMesaji = dko.hataMesaji;
            //        sonuc = Kaydet(kul, dilekce, true);
            //    }
            //}

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, cevaplanan borcu yok belgesi durumuna göre borcu yok belgelerini bir sonraki duruma taşır
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc CevaplananlariGoster(BorcuYokBelgesi kriter)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            //ObjectArray oListe = Listele(null, kriter, true);
            //GIBServis servisGib = new GIBServis();

            //string kurumKod = System.Configuration.ConfigurationManager.AppSettings.Get("GelirlerKurumAd");

            //foreach (BorcuYokBelgesi dilekce in oListe.objeler)
            //{
            //    //*******************************************
            //    BorcuYokturSorguOutput dso = new BorcuYokturSorguOutput();
            //    DilekceSorguInput dsi = new DilekceSorguInput();
            //    dsi.dilekceID = dilekce.cevapId;
            //    try
            //    {
            //        dso = servisGib.BorcuYokDilekceSorgulama(dsi, "SISTEM", kurumKod);
            //    }
            //    catch { }
            //    // dilekçe sorgulama başarılı değilse, hata mesajına bakıyoruz..

            //    if (!dso.basarili)
            //    {

            //        dilekce.durum = (int)ENUMBorcuYokDurum.DILEKCESORGUHATASI;
            //        dilekce.hataMesaji = dso.hataMesaji;
            //    }
            //    else  // dilekçe sorgulama başarılı ise
            //    {
            //        if (!dso.cevaplandi)  // dilekçe henüz cevaplanmamış ise
            //        {
            //            dilekce.durum = (int)ENUMBorcuYokDurum.CEVAPLANMADI;
            //        }
            //        else  // dilekçe cevaplanmış ise
            //        {
            //            if (dso.borcuYoktur)  // borcu yok ise
            //                dilekce.durum = (int)ENUMBorcuYokDurum.CEVAPOLUMLU;
            //            else  // borcu var ise
            //                dilekce.durum = (int)ENUMBorcuYokDurum.CEVAPOLUMSUZ;
            //        }
            //    }
            //    Kullanici kul = new Kullanici();
            //    kul.mernis = dilekce.islemYapan;
            //    sonuc = Kaydet(kul, dilekce, true);
            //}

            return sonuc;
        }
    }
}
