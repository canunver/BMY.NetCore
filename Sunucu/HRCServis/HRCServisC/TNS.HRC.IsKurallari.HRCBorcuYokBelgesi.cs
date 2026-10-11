using HRCServisC.tr.gov.sgb.uygulama.gib;
using System;
using System.Configuration;
using System.Data;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Borcu yok belge bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCBorcuYokBelgesi
    {
        private Sonuc KullaniciKontrol(Kullanici kullanan, string muhasebe)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            if (!HRCYetkiKontrol.YoneticiMi(kullanan))
            {
                if (!HRCYetkiKontrol.MuhasebeBirimYetkiVarMi(kullanan, muhasebe, ""))
                {
                    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00058;
                    sonuc.islemSonuc = false;
                    return sonuc;
                }
            }
            return sonuc;
        }

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
        public Sonuc Kaydet(Kullanici kullanan, BorcuYokBelgesi belge, bool icMi)
        {
            Sonuc sonuc = new Sonuc();
            if (!icMi)
                sonuc = KullaniciKontrol(kullanan, belge.muhasebe.ToString()); // ht

            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABBorcuYokBelgesi tabBelge = new TABBorcuYokBelgesi(ve);

            TABGenel tabGenel = new TABGenel(ve);

            ve.BeginTransaction();

            //<----Kaydetmeye başlıyorum
            belge.islemYapan = kullanan.mernis;
            belge.sonIslemTarihi = new TNSDateTime(DateTime.Now.ToString());
            belge.formDurum = 1;//YENI
            if (belge.kod == 0)
            {
                //yeni kayıt olduğu için 
                belge.kaydedenTcKimlik = kullanan.mernis;
                belge.kayitTarihi = new TNSDateTime(DateTime.Now.ToString());

                if (!belge.cevapKoduGeldi)
                {
                    if (tabBelge.DilekceVerilmisMi(belge) > 0)
                    {
                        sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00059;
                        sonuc.islemSonuc = false;
                        return sonuc;
                    }
                }

                string sayacKod = "BORCUYOKTURID";
                belge.kod = OrtakFonksiyonlar.ConvertToInt(tabGenel.BelgeNoAl(sayacKod, "0"), 0);

                sonuc = tabBelge.Kaydet(belge);
            }
            else
                sonuc = tabBelge.Guncelle(belge);
            //kaydet bitti--------------->

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

        public Sonuc Sil(Kullanici kullanan, TNSCollection belgeler)
        {

            Sonuc sonuc = new Sonuc(true, "", "");
            if (belgeler.Count > 0)
                sonuc = KullaniciKontrol(kullanan, ((BorcuYokBelgesi)belgeler[0]).muhasebe.ToString());
            else
                sonuc.DegerAta("En az bir sırada bekleyen kayıt seçmelisiniz");


            if (!sonuc.islemSonuc) return sonuc;
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABBorcuYokBelgesi tabBelge = new TABBorcuYokBelgesi(ve);
            try
            {
                ve.BeginTransaction();
                foreach (BorcuYokBelgesi item in belgeler)
                {
                    if (!sonuc.islemSonuc) break;
                    sonuc = tabBelge.Sil(item);
                }
                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = "İşlem başarıyla gerçekleştirildi";
                }
                else
                {
                    ve.Rollback();
                }
            }
            catch (Exception ex)
            {
                sonuc.DegerAta(ex.ToString());
            }
            finally
            {
                ve.VeriErisimKapat();
            }
            return sonuc;
        }
        /// <summary>
        /// Gelirler İdaresi Başkanlığından alınan servislerini kullanma bilgilerini veritabanına kaydeder
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="sorgulanan">İşlem yapılacak sorgulanan bilgisi.</param>
        /// <param name="islemYapan">İşlem yapılacak işlem yapan kullanıcı adı bilgisi.</param>
        /// <param name="aciklama">İşlem yapılacak aciklama bilgisi.</param>
        /// <param name="kurum">İşlem yapılacak kurum bilgisi.</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc GIBLogKaydet(string sorgulanan, string islemYapan, string aciklama, string kurum)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", null);
            TABBorcuYokBelgesi tabBelge = new TABBorcuYokBelgesi(ve);

            ve.BeginTransaction();

            sonuc = tabBelge.GIBLogKaydet(sorgulanan, islemYapan, aciklama, kurum);

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
        /// Arguman olarak verilen, borcu belge durum bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="belge">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc BelgeDurumDegistir(Kullanici kullanan, BorcuYokBelgesi belge)
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
            TABBorcuYokBelgesi tabBelge = new TABBorcuYokBelgesi(ve);

            ve.BeginTransaction();

            belge.islemYapan = kullanan.mernis;
            belge.sonIslemTarihi = new TNSDateTime(DateTime.Now.ToString());
            string[] belgeler = belge.cevapId.Split(';');//kod int olduğu için bilgiler cevapID içinde taşınıyor

            for (int i = 0; i < belgeler.Length; i++)
            {
                belge.kod = OrtakFonksiyonlar.ConvertToInt(belgeler[i], 0);
                sonuc = tabBelge.BelgeDurumDegistir(belge);
                if (!sonuc.islemSonuc)
                    break;
            }

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
        /// <returns>İstenilen kriterdeki, borcu yok belge bilgisini BorcuYokBelgesi nesnesi olarak tutan nesne</returns>
        public BorcuYokBelgesi BorcuYokAc(Kullanici kullanan, BorcuYokBelgesi kriter)
        {
            BorcuYokBelgesi belge = new BorcuYokBelgesi(0);

            if (kriter.kod > 0 || kriter.cevapId != "")
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

                TABBorcuYokBelgesi tabBelge = new TABBorcuYokBelgesi(ve);

                IDataReader dr = tabBelge.Listele(kriter, true);

                if (tabBelge.Hata() == "")
                {
                    while (dr.Read())
                    {
                        belge.kod = VeriErisim.DegerAl(dr, 0, 0);
                        belge.muhasebe = VeriErisim.DegerAl(dr, 1, 0);
                        belge.cevapId = VeriErisim.DegerAl(dr, 2, string.Empty);
                        belge.vergiKimlikNo = VeriErisim.DegerAl(dr, 3, string.Empty);
                        belge.talepNedeni = VeriErisim.DegerAl(dr, 4, 0);
                        belge.durum = VeriErisim.DegerAl(dr, 5, 0);
                        belge.baslangicTarihi = VeriErisim.DegerAl(dr, 6);
                        belge.bitisTarihi = VeriErisim.DegerAl(dr, 7);
                        belge.kayitTarihi = VeriErisim.DegerAl(dr, 8);
                        belge.kaydedenTcKimlik = VeriErisim.DegerAl(dr, 9, string.Empty);
                        belge.sonIslemTarihi = VeriErisim.DegerAl(dr, 10);
                        belge.islemYapan = VeriErisim.DegerAl(dr, 11, string.Empty);
                        belge.hataMesaji = VeriErisim.DegerAl(dr, 12, string.Empty);
                        belge.vergiDaire = VeriErisim.DegerAl(dr, 13, string.Empty);
                        belge.unvan = VeriErisim.DegerAl(dr, 14, string.Empty);
                        belge.adres = VeriErisim.DegerAl(dr, 15, string.Empty);
                        belge.ilgiliMakam = VeriErisim.DegerAl(dr, 16, string.Empty);
                    }
                }
                dr.Close();

                ve.VeriErisimKapat();
            }
            return belge;
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
        public ObjectArray Listele(Kullanici kullanan, BorcuYokBelgesi kriter, bool icMi)
        {
            ObjectArray donenListe = new ObjectArray();
            if (!icMi)
            {
                if (!HRCYetkiKontrol.YoneticiMi(kullanan))
                {
                    if (!HRCYetkiKontrol.MuhasebeBirimYetkiVarMi(kullanan, kriter.muhasebe.ToString(), ""))
                    {
                        donenListe.sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00060;
                        donenListe.sonuc.islemSonuc = false;

                        return donenListe;
                    }
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABBorcuYokBelgesi tabBelge = new TABBorcuYokBelgesi(ve);

            if (!kriter.bitisTarihi.isNull)
                kriter.bitisTarihi.AddDays(1);

            IDataReader dr = tabBelge.Listele(kriter, !icMi);

            if (dr != null)
            {
                while (dr.Read())
                {
                    BorcuYokBelgesi belge = new BorcuYokBelgesi(0);

                    belge.kod = VeriErisim.DegerAl(dr, 0, 0);
                    belge.muhasebe = VeriErisim.DegerAl(dr, 1, 0);
                    belge.cevapId = VeriErisim.DegerAl(dr, 2, string.Empty);
                    belge.vergiKimlikNo = VeriErisim.DegerAl(dr, 3, string.Empty);
                    belge.talepNedeni = VeriErisim.DegerAl(dr, 4, 0);
                    belge.durum = VeriErisim.DegerAl(dr, 5, 0);
                    belge.baslangicTarihi = VeriErisim.DegerAl(dr, 6);
                    belge.bitisTarihi = VeriErisim.DegerAl(dr, 7);
                    belge.kayitTarihi = VeriErisim.DegerAl(dr, 8);
                    belge.kaydedenTcKimlik = VeriErisim.DegerAl(dr, 9, string.Empty);
                    belge.sonIslemTarihi = VeriErisim.DegerAl(dr, 10);
                    belge.islemYapan = VeriErisim.DegerAl(dr, 11, string.Empty);
                    belge.hataMesaji = VeriErisim.DegerAl(dr, 12, string.Empty);
                    belge.vergiDaire = VeriErisim.DegerAl(dr, 13, string.Empty);
                    belge.unvan = VeriErisim.DegerAl(dr, 14, string.Empty);
                    belge.adres = VeriErisim.DegerAl(dr, 15, string.Empty);
                    belge.ilgiliMakam = VeriErisim.DegerAl(dr, 16, string.Empty);
                    belge.formDurum = VeriErisim.DegerAl(dr, 17, 0);
                    belge.birim = VeriErisim.DegerAl(dr, 18, string.Empty);
                    belge.ilgiliBelgeYil = VeriErisim.DegerAl(dr, 19, 0);
                    belge.ilgiliBelgeNo = VeriErisim.DegerAl(dr, 20, string.Empty);

                    donenListe.Ekle(belge);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return donenListe;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sırada bekleyen borcu yok belgesini bir sonraki duruma taşır
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SiradaBekleyenleriGonder(Kullanici kullanan, BorcuYokBelgesi kriter)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            ObjectArray oListe = Listele(kullanan, kriter, true);
            GIBServis servisGib = new GIBServis();
            servisGib.Url = ConfigurationManager.AppSettings.Get("GIBServisURL");
            servisGib.Proxy = OrtakFonksiyonlar.ProxyAyarla();

            string kurumKod = ConfigurationManager.AppSettings.Get("GelirlerKurumAd");

            foreach (BorcuYokBelgesi dilekce in oListe.objeler)
            {
                borcuYokturKaydetOutput dko = new borcuYokturKaydetOutput();
                borcuYokturKaydetInput dki = new borcuYokturKaydetInput();

                dki.dilekceBitisTarihi = dilekce.bitisTarihi.Oku().ToString("yyyyMMdd");
                dki.yaziTalepNedeni = dilekce.talepNedeni.ToString();

                if (dilekce.vergiKimlikNo.Length == 10)
                    dki.vergiKimlikNumarasi = dilekce.vergiKimlikNo;
                else if (dilekce.vergiKimlikNo.Length == 11)
                    dki.tcKimlikNumarasi = dilekce.vergiKimlikNo;

                try
                {
                    dko = servisGib.BorcuYokDilekceKaydet(dki, dilekce.islemYapan, kurumKod);
                }
                catch (Exception ex)
                {
                    OrtakFonksiyonlar.HataStrYaz("Borcu Yok Belgesi Hata. " + ex.Message + "*" + ex.StackTrace);
                    throw ex;
                }

                dilekce.cevapKoduGeldi = dko.durum.sonuc; // dko.basarili;
                if (dko.durum.durumKodu == "1000")
                {
                    dilekce.durum = (int)ENUMBorcuYokDurum.GONDERILDI;
                    dilekce.cevapId = dko.ID;
                    Kullanici kul = new Kullanici();
                    kul.mernis = dilekce.islemYapan;
                    sonuc = Kaydet(kul, dilekce, true);
                    if (!sonuc.islemSonuc)
                        OrtakFonksiyonlar.HataStrYaz("Borcu Yok Hata. Kaydetmedi." + sonuc.hataStr);
                }
                else
                {
                    Kullanici kul = new Kullanici();
                    kul.mernis = dilekce.islemYapan;
                    dilekce.hataMesaji = dko.durum.durumKodAciklamasi;
                    sonuc = Kaydet(kul, dilekce, true);
                    if (!sonuc.islemSonuc)
                        OrtakFonksiyonlar.HataStrYaz("Borcu Yok Gönderilmedi Hata . Kaydetmedi." + sonuc.hataStr + "*" + dko.durum.durumKodAciklamasi);
                }
            }

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, cevaplanan borcu yok belgesi durumuna göre borcu yok belgelerini bir sonraki duruma taşır
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc CevaplananlariGoster(Kullanici kullanan, BorcuYokBelgesi kriter)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            ObjectArray oListe = Listele(kullanan, kriter, true);
            GIBServis servisGib = new GIBServis();

            string kurumKod = System.Configuration.ConfigurationManager.AppSettings.Get("GelirlerKurumAd");

            foreach (BorcuYokBelgesi dilekce in oListe.objeler)
            {
                //*******************************************
                borcuYokturSorguOutput dso = new borcuYokturSorguOutput();
                dilekceSorguInput dsi = new dilekceSorguInput();
                dsi.dilekceID = dilekce.cevapId;
                try
                {
                    dso = servisGib.BorcuYokDilekceSorgulama(dsi, "SISTEM", kurumKod);
                }
                catch (Exception ex)
                {
                    OrtakFonksiyonlar.HataStrYaz("Borcu Yok Belgesi Cevap Hata. " + ex.Message + "*" + ex.StackTrace);
                    throw ex;
                }

                // dilekçe sorgulama başarılı değilse, hata mesajına bakıyoruz..

                if (dso.durum.durumKodu != "1000")
                {
                    dilekce.durum = (int)ENUMBorcuYokDurum.DILEKCESORGUHATASI;
                    dilekce.hataMesaji = dso.durum.durumKodAciklamasi;
                }
                else  // dilekçe sorgulama başarılı ise
                {
                    if (!dso.cevaplandi)  // dilekçe henüz cevaplanmamış ise
                    {
                        dilekce.durum = (int)ENUMBorcuYokDurum.CEVAPLANMADI;
                    }
                    else  // dilekçe cevaplanmış ise
                    {
                        if (dso.borcuYoktur)  // borcu yok ise
                            dilekce.durum = (int)ENUMBorcuYokDurum.CEVAPOLUMLU;
                        else  // borcu var ise
                            dilekce.durum = (int)ENUMBorcuYokDurum.CEVAPOLUMSUZ;
                    }
                }
                Kullanici kul = new Kullanici();
                kul.mernis = dilekce.islemYapan;
                sonuc = Kaydet(kul, dilekce, true);
                if (!sonuc.islemSonuc)
                    OrtakFonksiyonlar.HataStrYaz("Borcu Yok Cevap geldi Hata . Kaydetmedi." + sonuc.hataStr);
            }

            return sonuc;
        }
    }
}
