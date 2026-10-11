using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// Performans hedef bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class PEBPerHedef
    {
        /// <summary>
        /// Performans hedef varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen PerformansHedef nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// Performans hedefin stratejik hedeflerle bağının ve ağırlık bilgilerinin de ilgili yordamı çağırarak kaydedilmesini sağlar.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="hedef">Performans hedef bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, PerformansHedef hedef)
        {
            Sonuc sonuc = Yetki.YazmaHakkiVarMi(kullanan, hedef.kurKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = string.Empty;
            if (string.IsNullOrEmpty(hedef.kurKod))
                hata += PerformansButceResI.PEB005 + "<br>";
            else if (!PEBGenel.KurKodVarMi(hedef.yil, hedef.kurKod))
                hata += PerformansButceResI.PEB006 + "<br>";
            if (string.IsNullOrEmpty(hedef.perHedef))
                hata += PerformansButceResI.PEB089 + "<br>";
            if (string.IsNullOrEmpty(hedef.perHedefGorKod))
                hata += "Hedef kodunu Giriniz" + "<br>";
            if (hedef.perAgirlik.Count <= 0)
                hata += PerformansButceResI.PEB090 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPerHedef tHedef = new TABPerHedef(ve);
            ve.BeginTransaction();

            if (tHedef.HedefVarMi(hedef.yil, hedef.kurKod, hedef.perHedefKod))
                sonuc = tHedef.Guncelle(hedef);
            else if (hedef.perHedefKod > 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.PEB091 + "<br>";
            }
            else
            {
                sonuc = tHedef.Kaydet(hedef);
                hedef.perHedefKod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar.Trim(), 0);
            }

            if (sonuc.islemSonuc)
            {
                foreach (PerformansHedefAgirlik phs in hedef.perAgirlik)
                {
                    phs.yil = hedef.yil;
                    phs.kurKod = hedef.kurKod;
                    phs.perHedefKod = hedef.perHedefKod;
                }
                sonuc = PEBPerHedefAgirlik.Kaydet(ve, kullanan, hedef.perAgirlik);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = hedef.perHedefKod.ToString();
                sonuc.bilgiStr = PerformansButceResI.PEB015;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = PerformansButceResI.PEB016 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Performans hedefi, faaliyet/projelerde, performans göstergesi hedeflenen/gerçekleşen değerlerinde
        /// ve sabit maliyetlerde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pHedef">PerformansHedef nesnelerinden oluşan listeyi tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, TNSCollection pHedef)
        {
            Sonuc sonuc = new Sonuc(false, PerformansButceResI.PEB040, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPerHedef tPerHedef = new TABPerHedef(ve);
            TABPerHedefAgirlik tPerHedefAgirlik = new TABPerHedefAgirlik(ve);
            TABGostergeGercek tGostergeGercek = new TABGostergeGercek(ve);
            TABGostergeHedef tGostergeHedef = new TABGostergeHedef(ve);
            ve.BeginTransaction();

            foreach (PerformansHedef ph in pHedef)
            {
                sonuc = Yetki.YazmaHakkiVarMi(kullanan, ph.kurKod);
                if (!sonuc.islemSonuc)
                    break;

                if (tPerHedef.HedefKullanilmisMi(ph.yil, ph.kurKod, ph.perHedefKod))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = string.Format(PerformansButceResI.PEB092, ph.kurKod, ph.perHedefKod.ToString());
                    break;
                }

                //Metrik hedeflenen değerler siliniyor.
                PerformansGostergeHedeflenen gHedef = new PerformansGostergeHedeflenen();
                gHedef.yil = ph.yil;
                gHedef.phKurKod = ph.kurKod;
                gHedef.perHedefKod = ph.perHedefKod;
                sonuc = tGostergeHedef.Sil(gHedef);
                if (!sonuc.islemSonuc)
                    break;

                //Metrik gerçekleşen değerler siliniyor.
                PerformansGostergeGerceklesen gGercek = new PerformansGostergeGerceklesen();
                gGercek.yil = ph.yil;
                gGercek.phKurKod = ph.kurKod;
                gGercek.perHedefKod = ph.perHedefKod;
                sonuc = tGostergeGercek.Sil(gGercek);
                if (!sonuc.islemSonuc)
                    break;

                //Performans Hedefin ağırlıkları siliniyor.
                PerformansHedefAgirlik agirlik = new PerformansHedefAgirlik();
                agirlik.yil = ph.yil;
                agirlik.kurKod = ph.kurKod;
                agirlik.perHedefKod = ph.perHedefKod;
                sonuc = tPerHedefAgirlik.Sil(agirlik);
                if (!sonuc.islemSonuc)
                    break;

                //Performans Hedefin kendisi siliniyor.
                sonuc = tPerHedef.Sil(ph);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PerformansButceResI.PEB020;
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = PerformansButceResI.PEB021 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan performans hedef bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="hedef">Performans hedef bilgisini tutan nesne</param>
        /// <param name="detayAgirlik">Performans hedefin stratejik hedeflerle bağının ve ağırlık bilgilerinin istenip istenmediği bilgisi</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="strHedefKod">Stratejik hedef programatik kodu</param>
        /// <param name="kurumDahil">Kurumun performans hedeflerinin listeye dahil edilip edilmeyeceği bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans hedef listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, PerformansHedef hedef, bool detayAgirlik, int donemBYil, int strHedefKod, bool kurumDahil)
        {
            return Listele(veriErisim, kullanan, hedef, detayAgirlik, donemBYil, strHedefKod, false, kurumDahil, -1);
        }

        /// <summary>
        /// Verilen kriterlere uygun olan performans hedef bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="hedef">Performans hedef bilgisini tutan nesne</param>
        /// <param name="detayAgirlik">Performans hedefin stratejik hedeflerle bağının ve ağırlık bilgilerinin istenip istenmediği bilgisi</param>
        /// <param name="detayGosterge">Performans hedefe ait performans gösterge bilgilerinin istenip istenmediği bilgisi</param>
        /// <param name="kurumDahil">Kurumun performans hedeflerinin listeye dahil edilip edilmeyeceği bilgisi</param>
        /// <param name="asama">Performans göstergesi hedeflenen değerlerinin istendiği aşama</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans hedef listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, PerformansHedef hedef, bool detayAgirlik, bool detayGosterge, bool kurumDahil, int asama)
        {
            return Listele(veriErisim, kullanan, hedef, detayAgirlik, 0, 0, detayGosterge, kurumDahil, asama);
        }

        /// <summary>
        /// Verilen kriterlere uygun olan performans hedef bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="hedef">Performans hedef bilgisini tutan nesne</param>
        /// <param name="detayAgirlik">Performans hedefin stratejik hedeflerle bağının ve ağırlık bilgilerinin istenip istenmediği bilgisi</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="strHedefKod">Stratejik hedef programatik kodu</param>
        /// <param name="detayGosterge">Performans hedefe ait performans gösterge bilgilerinin istenip istenmediği bilgisi</param>
        /// <param name="kurumDahil">Kurumun performans hedeflerinin listeye dahil edilip edilmeyeceği bilgisi</param>
        /// <param name="asama">Performans göstergesi hedeflenen değerlerinin istendiği aşama</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans hedef listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, PerformansHedef hedef, bool detayAgirlik, int donemBYil, int strHedefKod, bool detayGosterge, bool kurumDahil, int asama)
        {
            ObjectArray hedefVAN = new ObjectArray();
            hedefVAN.sonuc = Yetki.OkumaHakkiVarMi(kullanan, hedef.kurKod);
            if (!hedefVAN.sonuc.islemSonuc)
                return hedefVAN;

            //if (detayAgirlik && !detayGosterge)
            //{
            //    string hata = string.Empty;
            //    if (string.IsNullOrEmpty(hedef.kurKod))
            //        hata += PerformansButceResI.PEB005 + "<br>";
            //    if (hedef.perHedefKod <= 0)
            //        hata += PerformansButceResI.PEB093 + "<br>";
            //    if (!string.IsNullOrEmpty(hata))
            //    {
            //        hedefVAN.sonuc.islemSonuc = false;
            //        hedefVAN.sonuc.hataStr = hata;
            //        return hedefVAN;
            //    }
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve3 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBHM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPerHedef tPerHedef = new TABPerHedef(ve);
            BKD.Tertip t = new BKD.Tertip(hedef.yil, BKD.ENUMTabloKod.TABPEBOdenek);
            IDataReader dr = tPerHedef.Listele(hedef, donemBYil, strHedefKod, kurumDahil, t.Kirilimlar(0));
            while (dr.Read())
            {
                PerformansHedef gelen = new PerformansHedef();
                gelen.yil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.kurKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.kurKod = PEBGenel.KurKodNoktala(gelen.kurKod);
                gelen.kurAd = PEBServis.UzayYaziDegerGetir(kullanan, ve3, "ABSKUR", gelen.yil.ToString() + "-" + gelen.kurKod, true, "");
                gelen.perHedefKod = VeriErisim.DegerAl(dr, 2, 0);
                gelen.perHedef = VeriErisim.DegerAl(dr, 3, string.Empty);
                gelen.aciklama = VeriErisim.DegerAl(dr, 4, string.Empty);
                gelen.ebutceKod = VeriErisim.DegerAl(dr, 5, string.Empty);
                gelen.perHedefGorKod = VeriErisim.DegerAl(dr, 6, string.Empty);

                gelen.kalkinmaAmac = VeriErisim.DegerAl(dr, 7, string.Empty);
                gelen.kalkinmaHedef = VeriErisim.DegerAl(dr, 8, string.Empty);
                gelen.kalkinmaPolitika = VeriErisim.DegerAl(dr, 9, string.Empty);

                if (detayAgirlik)
                {
                    ObjectArray pAgirlik = PEBPerHedefAgirlik.Listele(ve2, kullanan, gelen.yil, gelen.kurKod, gelen.perHedefKod);
                    if (!pAgirlik.sonuc.islemSonuc)
                    {
                        hedefVAN.sonuc = pAgirlik.sonuc;
                        break;
                    }
                    else
                        gelen.perAgirlik = pAgirlik.objeler;
                }

                if (detayGosterge)
                {
                    PerformansGostergeHedeflenen pgh = new PerformansGostergeHedeflenen();
                    pgh.yil = gelen.yil;
                    pgh.phKurKod = gelen.kurKod;
                    pgh.asama = asama;
                    pgh.perHedefKod = gelen.perHedefKod;
                    ObjectArray pGosterge = PEBGostergeHedef.Listele(ve2, kullanan, pgh);
                    if (!pGosterge.sonuc.islemSonuc)
                    {
                        hedefVAN.sonuc = pGosterge.sonuc;
                        break;
                    }
                    else
                        gelen.perGostergeHedeflenen = pGosterge.objeler;
                }

                hedefVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (hedefVAN.objeler.Count == 0)
                hedefVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();
            ve3.VeriErisimKapat();
            return hedefVAN;
        }

        ///// <summary>
        ///// Verilen kriterlere uygun olan performans hedef bilgilerini döndüren yordam
        ///// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="hedef">Performans hedef bilgisini tutan nesne</param>
        ///// <param name="detayAgirlik">Performans hedefin stratejik hedeflerle bağının ve ağırlık bilgilerinin istenip istenmediği bilgisi</param>
        ///// <param name="donemBYil">Dönem başlangıç yılı</param>
        ///// <param name="strHedefKod">Stratejik hedef programatik kodu</param>
        ///// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans hedef listesi döndürülür.</returns>
        //public static ObjectArray ListeleEskiDB(VeriErisim veriErisim, Kullanici kullanan, PerformansHedef hedef, bool detayAgirlik, int donemBYil, int strHedefKod)
        //{
        //    return ListeleEskiDB(veriErisim, kullanan, hedef, detayAgirlik, donemBYil, strHedefKod, false);
        //}

        ///// <summary>
        ///// Verilen kriterlere uygun olan performans hedef bilgilerini döndüren yordam
        ///// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="hedef">Performans hedef bilgisini tutan nesne</param>
        ///// <param name="detayAgirlik">Performans hedefin stratejik hedeflerle bağının ve ağırlık bilgilerinin istenip istenmediği bilgisi</param>
        ///// <param name="donemBYil">Dönem başlangıç yılı</param>
        ///// <param name="strHedefKod">Stratejik hedef programatik kodu</param>
        ///// <param name="detayGosterge">Performans hedefe ait performans gösterge bilgilerinin istenip istenmediği bilgisi</param>
        ///// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans hedef listesi döndürülür.</returns>
        //private static ObjectArray ListeleEskiDB(VeriErisim veriErisim, Kullanici kullanan, PerformansHedef hedef, bool detayAgirlik, int donemBYil, int strHedefKod, bool detayGosterge)
        //{
        //    ObjectArray hedefVAN = new ObjectArray();
        //    hedefVAN.sonuc = Yetki.OkumaHakkiVarMi(kullanan, hedef.kurKod);
        //    if (!hedefVAN.sonuc.islemSonuc)
        //        return hedefVAN;

        //    if (detayAgirlik && !detayGosterge)
        //    {
        //        string hata = string.Empty;
        //        if (string.IsNullOrEmpty(hedef.kurKod))
        //            hata += PerformansButceResI.PEB005 + "<br>";
        //        if (hedef.perHedefKod <= 0)
        //            hata += PerformansButceResI.PEB093 + "<br>";
        //        if (!string.IsNullOrEmpty(hata))
        //        {
        //            hedefVAN.sonuc.islemSonuc = false;
        //            hedefVAN.sonuc.hataStr = hata;
        //            return hedefVAN;
        //        }
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEBY");
        //    VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPEBY");
        //    TABPerHedef tPerHedef = new TABPerHedef(ve);

        //    IDataReader dr = tPerHedef.Listele(hedef, donemBYil, strHedefKod, false);
        //    while (dr.Read())
        //    {
        //        PerformansHedef gelen = new PerformansHedef();
        //        gelen.yil = VeriErisim.DegerAl(dr, 0, 0);
        //        gelen.kurKod = VeriErisim.DegerAl(dr, 1, string.Empty);
        //        gelen.kurKod = PEBGenel.KurKodNoktala(gelen.kurKod);
        //        gelen.kurAd = PEBServis.UzayYaziDegerGetir("ABSKUR", gelen.yil.ToString() + "-" + gelen.kurKod, true, "");
        //        gelen.perHedefKod = VeriErisim.DegerAl(dr, 2, 0);
        //        gelen.perHedef = VeriErisim.DegerAl(dr, 3, string.Empty);
        //        gelen.aciklama = VeriErisim.DegerAl(dr, 4, string.Empty);
        //        gelen.ebutceKod = VeriErisim.DegerAl(dr, 5, string.Empty);

        //        if (detayAgirlik)
        //        {
        //            ObjectArray pAgirlik = PEBPerHedefAgirlik.Listele(ve2, kullanan, gelen.yil, gelen.kurKod, gelen.perHedefKod);
        //            if (!pAgirlik.sonuc.islemSonuc)
        //            {
        //                hedefVAN.sonuc = pAgirlik.sonuc;
        //                break;
        //            }
        //            else
        //                gelen.perAgirlik = pAgirlik.objeler;
        //        }

        //        if (detayGosterge)
        //        {
        //            PerformansGostergeHedeflenen pgh = new PerformansGostergeHedeflenen();
        //            pgh.yil = gelen.yil;
        //            pgh.phKurKod = gelen.kurKod;
        //            pgh.perHedefKod = gelen.perHedefKod;
        //            ObjectArray pGosterge = PEBGostergeHedef.Listele(ve2, kullanan, pgh);
        //            if (!pGosterge.sonuc.islemSonuc)
        //            {
        //                hedefVAN.sonuc = pGosterge.sonuc;
        //                break;
        //            }
        //            else
        //                gelen.perGostergeHedeflenen = pGosterge.objeler;
        //        }

        //        hedefVAN.objeler.Add(gelen);
        //    }
        //    dr.Close();

        //    if (hedefVAN.objeler.Count == 0)
        //        hedefVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

        //    ve.VeriErisimKapat();
        //    ve2.VeriErisimKapat();
        //    return hedefVAN;
        //}

        internal static Sonuc PerformansKopyala(VeriErisim veriErisim, Kullanici kullanan, int kaynakYil, int hedefYil)
        {
            Sonuc s = new Sonuc();
            if (!kullanan.KullaniciTipiIceriyorMu((int)TNS.KYM.ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                s.DegerAta("Bu işlem yapmaya yetkiniz yok!");
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.ExecuteNonquery(string.Format("INSERT INTO PEBPERHEDEF SELECT {0} YIL, KURKOD, HEDEFKOD, HEDEF, ACIKLAMA, EBUTCEKOD, HEDEFGORKOD, KPAMAC, KPHEDEF, KPPOLITIKA FROM PEBPERHEDEF WHERE YIL={1}", hedefYil, kaynakYil));
            HataEkle(s, "Performans Hedefleri", ve.hata);

            ve.hata = "";
            ve.ExecuteNonquery(string.Format("INSERT INTO PEBPERHEDEFAGIRLIK SELECT {0} YIL, KURKOD, PERHEDEFKOD, DONEMBYIL, STRHEDEFKOD, AGIRLIK FROM PEBPERHEDEFAGIRLIK WHERE YIL={1}", hedefYil, kaynakYil));
            HataEkle(s, "Performans Hedefi, Strajij hedef bağı", ve.hata);

            ve.hata = "";
            ve.ExecuteNonquery(string.Format("INSERT INTO PEBFAALIYETPROJE SELECT {0} YIL, KURKOD, FPKOD, FPAD, PROJEMI, SORUMLU, FAALIYETSEVIYESI, TUR, EBUTCEKOD FROM PEBFAALIYETPROJE WHERE YIL={1}", hedefYil, kaynakYil));
            HataEkle(s, "Faaliyet/projeler", ve.hata);

            ve.hata = "";
            ve.ExecuteNonquery(string.Format("INSERT INTO PEBSABITMALIYET SELECT {0} YIL, SMKURKOD, SMKOD, SMAD, TUR, PHKURKOD, PERHEDEFKOD FROM PEBSABITMALIYET WHERE YIL={1}", hedefYil, kaynakYil));
            HataEkle(s, "Genel yönetim giderleri", ve.hata);

            ve.hata = "";
            ve.ExecuteNonquery(string.Format("INSERT INTO PEBFAALIYETPROJEAGIRLIK SELECT {0} YIL, FPKURKOD, FPKOD, PHKURKOD, PERHEDEFKOD, AGIRLIK FROM PEBFAALIYETPROJEAGIRLIK WHERE YIL={1}", hedefYil, kaynakYil));
            HataEkle(s, "Faaliyet/proje, Performans hedefi bağı", ve.hata);

            ve.hata = "";
            ve.ExecuteNonquery(string.Format("INSERT INTO PEBSABITMALIYETAGIRLIK SELECT {0} YIL, SMKURKOD, SMKOD, FPKURKOD, FPKOD, SENARYOKOD, AGIRLIK, ODENEK FROM PEBSABITMALIYETAGIRLIK WHERE YIL={1}", hedefYil, kaynakYil));
            HataEkle(s, "Genel yönetim giderleri, Faaliyet bağı", ve.hata);

            ve.hata = "";
            ve.ExecuteNonquery(string.Format("INSERT INTO PEBFAALIYETPROJEPEBFON SELECT {0} YIL, PEBKOD, MALIYETKURKOD, SABITMI, MALIYETKOD FROM PEBFAALIYETPROJEPEBFON WHERE YIL={1}", hedefYil, kaynakYil));
            HataEkle(s, "Faaliyet/proje bütçe bağı", ve.hata);

            ve.VeriErisimKapat();
            return s;
        }

        private static void HataEkle(Sonuc s, string veri, string ve_hata)
        {
            string bilgi;
            if (string.IsNullOrWhiteSpace(ve_hata))
                bilgi = veri + " kopyalandı!";
            else
                bilgi = veri + " kopyalanamadı:" + ve_hata;
            if (!string.IsNullOrWhiteSpace(s.bilgiStr)) s.bilgiStr += "<br>";
            s.bilgiStr += bilgi;
        }
    }
}