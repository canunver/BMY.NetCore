using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using TNS.ORG;
using System;

namespace TNS.EVR.IsKurallari
{
    /// <summary>
    /// Birim bilgileriyle ilgili, tanımlama, silme, listeleme, kök evrak birimi bulma, evrak birim personellerini listeleme ve yetki kontrolü yordamlarını tutan sınıf.
    /// </summary>
    public class EVRBirim
    {
        public static bool orgSemaPersonel = OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBul(0, "/OrgSemaPersonel"), 0) > 0 ? true : false;

        /// <summary>
        /// Parametre olarak verilen evrak birimlerini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">>Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Evrak birimlerini içeren nesne koleksiyonu</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            sonuc = KontrolEt(ve, kullanan, liste, "Yazma");
            if (sonuc.islemSonuc)
            {
                ve.BeginTransaction();
                try
                {
                    TABBirim to = new TABBirim(ve);

                    sonuc = to.Kaydet(liste);

                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();

                        sonuc.bilgiStr += EvrakI.EVRI011;
                    }
                    else
                        ve.Rollback();

                }
                catch (Exception ex)
                {
                    ve.Rollback();
                    OrtakFonksiyonlar.HataStrYaz("{919B5ACA-BBA1-4593-BDA9-68C8C5E40693} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
                }
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen evrak birimlerini veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Evrak birimlerini içeren nesne koleksiyonu</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            sonuc = KontrolEt(ve, kullanan, liste, "Silme");

            if (sonuc.islemSonuc)
            {
                ve.BeginTransaction();
                try
                {
                    TABBirim to = new TABBirim(ve);
                    foreach (Birim bilgi in liste.objeler)
                    {
                        if (to.KullanilmisMi(bilgi.kod))
                            sonuc.hataStr += "<br>" + string.Format(EvrakI.EVRI012, bilgi.kod);
                    }

                    if (sonuc.hataStr == "")
                        sonuc = to.Sil(liste);
                    else
                        sonuc.islemSonuc = false;

                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.bilgiStr = EvrakI.EVRI013;
                    }
                    else
                        ve.Rollback();

                }
                catch (Exception ex)
                {
                    ve.Rollback();
                    OrtakFonksiyonlar.HataStrYaz("{3FBFCD64-3C4A-4B9C-B370-F841286A82CD} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
                }
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Birim kaydetme veya silme işlemi için yetki kontrolü yapan yordam.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgilerini içeren nesne</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Evrak birimlerini içeren nesne koleksiyonu</param>
        /// <param name="yetkiTipi">"Yazma" veya "Silme" bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc KontrolEt(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste, string yetkiTipi)
        {
            string kodYapisi = TNS.DEG.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD18/KODYAPI");
            string hata = "";
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI014, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            if (liste != null && yetkiTipi == "Yazma")
            {
                foreach (Birim bilgi in liste.objeler)
                {
                    if (bilgi.kod.Trim() == "")
                        hata += EvrakI.EVRI015 + "<br>";
                    if (bilgi.ad.Trim() == "")
                        hata += bilgi.kod + " - " + EvrakI.EVRI016 + "<br>";
                    if (bilgi.ustBirim.Trim() == "")
                        hata += bilgi.kod + " - " + EvrakI.EVRI017 + "<br>";
                    if (bilgi.genelEvrakMi == 0)
                        hata += bilgi.kod + " - " + EvrakI.EVRI018 + "<br>";
                    if (bilgi.kullanilmiyor == 0)
                        hata += bilgi.kod + " - " + EvrakI.EVRI019 + "<br>";


                    if (!OrtakFonksiyonlar.KodYapiKontrol(kodYapisi, bilgi.kod.Replace(".", ""), false).islemSonuc)
                        hata += string.Format(EvrakI.EVRI020, kodYapisi) + "<br>";
                }
            }
            else if (liste != null && yetkiTipi == "Silme")
            {
                TABBirim th = new TABBirim(ve);
                foreach (Birim bilgi in liste.objeler)
                {
                    if (th.AnasiCocuguVarMi(bilgi.kod, true))
                        hata += "<br>" + string.Format(EvrakI.EVRI021, bilgi.kod);
                }
            }

            if (hata == "")
                sonuc.islemSonuc = true;

            sonuc.hataStr = hata;

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Evrak birimlerinin seviye değerlerini düzenleyen yordam.
        /// </summary>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc DetaySayilariDuzenle(VeriErisim veriErisim)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABBirim to = new TABBirim(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = to.DetaySayilariDuzenle("");
                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr += EvrakI.EVRI022;
                }
                else
                    ve.Rollback();

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{BD69B745-FD29-4523-8324-1FC87D2A52EB} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Birim nesnesine göre evrak birimlerini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="bilgi">Birim bilgisini içeren nesne</param>
        /// <returns>Birim nesne koleksiyonunu olarak döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Birim bilgi, bool anaBirim)
        {
            ObjectArray hVAN = new ObjectArray();

            //bool orgSemaEvrakKullaniyorMu = EVRServis.OrgSemaEvrakKullaniyorMu();

            bool orgSemaEvrakKullaniyorMu = false;


            if (orgSemaEvrakKullaniyorMu)
            {
                IORGServis servisORG = TNS.ORG.Arac.Tanimla();

                OrganizasyonBirim kriter = new OrganizasyonBirim()
                {
                    evrakMi = 1,
                    evrakbirim = bilgi.kod,
                    adi = bilgi.ad,
                    evrGenelEvrakMi = bilgi.genelEvrakMi,
                    evrUstBirimKodu = bilgi.ustBirim,
                    evrKisiyeHavale = bilgi.kisiyeHavale,
                    evrSeviye = bilgi.seviye,
                    eposta = bilgi.ePosta
                };

                ObjectArray liste = servisORG.OrganizasyonBirimListele(kriter, false, false);
                if (liste.sonuc.islemSonuc)
                {
                    foreach (OrganizasyonBirim orgBirim in liste.objeler)
                    {
                        bilgi = new Birim();

                        bilgi.kod = orgBirim.evrakbirim;
                        bilgi.ad = orgBirim.adi;
                        bilgi.genelEvrakMi = orgBirim.evrGenelEvrakMi;
                        bilgi.ustBirim = orgBirim.evrUstBirimKodu;
                        bilgi.kisiyeHavale = orgBirim.evrKisiyeHavale;
                        bilgi.ePosta = orgBirim.eposta;
                        bilgi.seviye = orgBirim.evrSeviye;
                        bilgi.kisaAd = orgBirim.kisaadi;
                        //bilgi.detay = false;
                        //bilgi.kullanilmiyor = 0;
                        //bilgi.ustBirimAd = "";

                        hVAN.objeler.Add(bilgi);
                    }
                }
            }
            else
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
                TABBirim to = new TABBirim(ve);
                IDataReader r = to.Listele(bilgi, anaBirim);

                if (to.Hata() == "")
                {
                    while (r.Read())
                    {
                        bilgi = new Birim();

                        bilgi.kod = VeriErisim.DegerAl(r, 0, "");
                        bilgi.idareKimlikKodu = VeriErisim.DegerAl(r, 1, "");
                        bilgi.ad = VeriErisim.DegerAl(r, 2, "");
                        bilgi.genelEvrakMi = VeriErisim.DegerAl(r, 3, 0);
                        bilgi.ustBirim = VeriErisim.DegerAl(r, 4, "");
                        bilgi.seviye = VeriErisim.DegerAl(r, 5, 0);
                        bilgi.detay = VeriErisim.DegerAl(r, 6, 0) == 0;
                        bilgi.kullanilmiyor = VeriErisim.DegerAl(r, 7, 0);
                        bilgi.ePosta = VeriErisim.DegerAl(r, 8, "");
                        bilgi.kisiyeHavale = VeriErisim.DegerAl(r, 9, 0);
                        bilgi.kisaAd = VeriErisim.DegerAl(r, 10, "");
                        bilgi.detayDagitim = VeriErisim.DegerAl(r, 11, 0);
                        bilgi.dagitimBirimKod = VeriErisim.DegerAl(r, 12, "");
                        bilgi.ustBirimAd = VeriErisim.DegerAl(r, 13, "");

                        hVAN.objeler.Add(bilgi);
                    }
                    r.Close();
                }
                ve.VeriErisimKapat();
            }

            return hVAN;
        }

        /// <summary>
        /// Parametre olarak verilen Birim nesnesine göre evrak birimini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="bilgi">Birim bilgisini içeren nesne</param>
        /// <returns>Evrak birimi Birim nesnesi olarak döndürülür.</returns>
        public static Birim Oku(VeriErisim veriErisim, Birim bilgi)
        {
            Birim donenDeger = new Birim();

            ObjectArray hVAN = Listele(veriErisim, bilgi, false);

            if (hVAN.objeler.Count > 0)
                donenDeger = (Birim)hVAN.objeler[0];

            return donenDeger;
        }

        /// <summary>
        /// Parametre olarak verilen evrak birim kodunun evrak kök kodunu getiren yordam.
        /// </summary>
        /// <param name="evrakBirim">Evrek birim kodunu içeren nesne</param>
        /// <returns>Evrak kök kodu string nesesi olarak döndürülür.</returns>
        public static string EvrakKokBirimGetir(VeriErisim veriErisim, string evrakBirim)
        {
            Birim donenDeger = EvrakKokBirimGetir2(veriErisim, evrakBirim);

            if (donenDeger == null)
                return "";
            return donenDeger.kod;
        }

        public static Birim EvrakKokBirimGetir2(VeriErisim veriErisim, string evrakBirim)
        {
            if (string.IsNullOrEmpty(evrakBirim))
                return null;

            Birim donenDeger = Oku(veriErisim, new Birim() { kod = evrakBirim });

            //Genel evrak değilse genel evrak'a kadar çık ve kök birimi bul. (10.10.2012-Hüseyin)
            int index = 0;
            while (donenDeger.genelEvrakMi != (int)ENUMGenelEvrakBirimi.EVET && donenDeger.ad.Length > 1 && index <= 11)
            {
                donenDeger = Oku(veriErisim, new Birim() { kod = donenDeger.ustBirim });
                index++;
            }

            return donenDeger;

            //Esiden böyleydi
            /*
             * if (donenDeger.genelEvrakMi == (int)ENUMGenelEvrakBirimi.EVET)
             *   return donenDeger.kod;
             * else
             *    return donenDeger.ustBirim;
             */
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, personel bilgilerini personel nesne koleksiyonunu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="bilgi">Personel bilgilerini içeren nesne</param>
        /// <returns>Personel nesne koleksiyonunu olarak döndürülür.</returns>
        public static ObjectArray PersonelListele(VeriErisim veriErisim, Personel bilgi)
        {
            ObjectArray hVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            TABBirim to = new TABBirim(ve);
            IDataReader r = to.PersonelListele(bilgi, EVRBirim.orgSemaPersonel);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    bilgi = new Personel();

                    bilgi.tcKimlikNo = VeriErisim.DegerAl(r, 0, "");
                    bilgi.sicilNo = VeriErisim.DegerAl(r, 1, "");
                    bilgi.ad = VeriErisim.DegerAl(r, 2, "");
                    bilgi.soyad = VeriErisim.DegerAl(r, 3, "");
                    bilgi.eposta = VeriErisim.DegerAl(r, 4, "");
                    bilgi.durum = VeriErisim.DegerAl(r, 5, 0);
                    bilgi.muhasebeKodu = VeriErisim.DegerAl(r, 6, "");
                    bilgi.unvan = VeriErisim.DegerAl(r, 7, "");
                    bilgi.evrakBirimKodu = VeriErisim.DegerAl(r, 8, "");
                    bilgi.akademikUnvan = VeriErisim.DegerAl(r, 9, "");

                    if (!string.IsNullOrEmpty(bilgi.unvan) && bilgi.unvan.IndexOf("FİRMA") > -1)
                        continue;

                    bilgi.unvan = OrtakFonksiyonlar.IlkHarflerBuyuk(bilgi.unvan);
                    bilgi.unvan = bilgi.unvan.Replace(" Ve ", " ve ").Replace(" İle ", " ile ");

                    hVAN.objeler.Add(bilgi);
                }
                r.Close();
            }

            ve.VeriErisimKapat();
            return hVAN;
        }

        public static Sonuc DTVTBirimBilgisiKaydet(VeriErisim veriErisim, Birim form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABBirim to = new TABBirim(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = to.DTVTBirimBilgisiKaydet(form);
                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr += EvrakI.EVRI011;
                }
                else
                    ve.Rollback();
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{706886DC-8292-4AEC-A8CD-A130CE072796} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static string IdareKimlikKodGetir(VeriErisim veriErisim, string evrakBirimKod)
        {
            if (string.IsNullOrEmpty(evrakBirimKod)) return "";

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABBirim tab = new TABBirim(ve);

            string idareKimlikKod = tab.IdareKimlikKodGetir(evrakBirimKod);

            ve.VeriErisimKapat();
            return idareKimlikKod;
        }

        public static string BirimKodGetir(VeriErisim veriErisim, string idareKimlikKod)
        {
            if (string.IsNullOrEmpty(idareKimlikKod) || idareKimlikKod.Length != 8) return "";

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABBirim tab = new TABBirim(ve);

            string birimKod = tab.BirimKodGetir(idareKimlikKod);

            ve.VeriErisimKapat();
            return birimKod;
        }

        public static string UstBirimKodGetir(VeriErisim veriErisim, string evrakBirimKod)
        {
            if (string.IsNullOrEmpty(evrakBirimKod)) return "";

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABBirim tab = new TABBirim(ve);

            string ustBirimKod = tab.UstBirimKodGetir(evrakBirimKod);

            ve.VeriErisimKapat();
            return ustBirimKod;
        }

        public static string DagitimBirimKodGetir(VeriErisim veriErisim, string evrakBirimKod)
        {
            if (string.IsNullOrEmpty(evrakBirimKod)) return "";

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABBirim tab = new TABBirim(ve);

            string dagitimBirimKod = tab.DagitimBirimKodGetir(evrakBirimKod);

            ve.VeriErisimKapat();
            return dagitimBirimKod;
        }

        public static bool BirimAktifMi(VeriErisim veriErisim, string kod)
        {
            if (string.IsNullOrEmpty(kod)) return false;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABBirim tab = new TABBirim(ve);

            bool durum = tab.BirimKullanimDurumu(kod) == 1 ? true : false;

            ve.VeriErisimKapat();
            return durum;
        }

        public static Birim EvrakDagitimBirimGetir(VeriErisim veriErisim, string hazirlayanEvrakBirimKod)
        {
            Birim birim = new Birim() { kod = hazirlayanEvrakBirimKod };
            if (!string.IsNullOrEmpty(hazirlayanEvrakBirimKod))
            {
                birim = Oku(veriErisim, birim);

                //Dağıtım birim kodu seçilmemiş ise üst birime bakılsın
                if (string.IsNullOrEmpty(birim.dagitimBirimKod))
                    birim = Oku(veriErisim, new Birim() { kod = birim.ustBirim });
                else
                    birim = Oku(veriErisim, new Birim() { kod = birim.dagitimBirimKod });
            }
            return birim;
        }

        public static ObjectArray SayacListele(VeriErisim ve, string kod)
        {
            ObjectArray liste = new ObjectArray();

            if (string.IsNullOrEmpty(kod)) return liste;

            ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABGenel to = new TABGenel(ve);
            IDataReader r = to.SayacListele(kod);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    SayacBilgi bilgi = new SayacBilgi();

                    bilgi.kod = VeriErisim.DegerAl(r, 0, "");
                    bilgi.deger = VeriErisim.DegerAl(r, 1, "");

                    liste.objeler.Add(bilgi);
                }
                r.Close();
            }
            ve.VeriErisimKapat();

            return liste;
        }

        public static Sonuc SayacKaydet(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            if (!(kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) ||
                kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.EVRAKBELGEYONETICISI) ||
                kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.EVRAKKURUMBELGEYONETICISI)))
            {
                sonuc.DegerAta("İşlem yapma yetkiniz yok");
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABGenel to = new TABGenel(ve);

            ve.BeginTransaction();
            try
            {
                foreach (SayacBilgi s in liste.objeler)
                {
                    sonuc = to.SayacKaydet(s.kod, s.deger);
                    if (!sonuc.islemSonuc)
                        break;
                }

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr += EvrakI.EVRI011;
                }
                else
                    ve.Rollback();
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{706886DC-8292-4AEC-A8CD-A130CE07279Sayac} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Birim[] Listele(VeriErisim veriErisim, string birimKodListesi)
        {
            ObjectArray liste = new ObjectArray();

            if (birimKodListesi != null && !string.IsNullOrWhiteSpace(birimKodListesi.TrimEnd(';')))
            {

                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
                TABBirim tab = new TABBirim(ve);

                IDataReader dr = tab.Listele(birimKodListesi);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        Birim form = new Birim();
                        form.kod = VeriErisim.DegerAl(dr, (int)TABBirim.ENUMAlanlar.KOD, "");
                        form.ad = VeriErisim.DegerAl(dr, (int)TABBirim.ENUMAlanlar.AD, "");
                        form.genelEvrakMi = VeriErisim.DegerAl(dr, (int)TABBirim.ENUMAlanlar.GENELEVRAK, 0);
                        form.ustBirim = VeriErisim.DegerAl(dr, (int)TABBirim.ENUMAlanlar.USTBIRIMKODU, "");
                        form.seviye = VeriErisim.DegerAl(dr, (int)TABBirim.ENUMAlanlar.SEVIYE, 0);
                        form.detay = VeriErisim.DegerAl(dr, (int)TABBirim.ENUMAlanlar.DETAY, 0) == 0;
                        form.kullanilmiyor = VeriErisim.DegerAl(dr, (int)TABBirim.ENUMAlanlar.KULLANILMIYOR, 0);
                        form.ePosta = VeriErisim.DegerAl(dr, (int)TABBirim.ENUMAlanlar.EPOSTA, "");
                        form.kisiyeHavale = VeriErisim.DegerAl(dr, (int)TABBirim.ENUMAlanlar.KISIYEHAVALE, 0);
                        form.idareKimlikKodu = VeriErisim.DegerAl(dr, (int)TABBirim.ENUMAlanlar.IDAREKIMLIKKODU, "");
                        form.kisaAd = VeriErisim.DegerAl(dr, (int)TABBirim.ENUMAlanlar.KISAAD, "");
                        form.detayDagitim = VeriErisim.DegerAl(dr, (int)TABBirim.ENUMAlanlar.DETAYDAGITIM, 0);
                        form.dagitimBirimKod = VeriErisim.DegerAl(dr, (int)TABBirim.ENUMAlanlar.DAGITIMBIRIMKOD, "");

                        liste.objeler.Add(form);
                    }
                    dr.Close();
                }

                ve.VeriErisimKapat();
            }
            return (Birim[])liste.objeler.ToArray(typeof(Birim));
        }
    }
}
