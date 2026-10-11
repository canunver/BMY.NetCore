using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.KYM;
using TNS.BHMv2;
using TNS.Veri;
using TNS.YAT.IsKurallari.Veri;
using TNS.DEG;

namespace TNS.YAT.IsKurallari
{
    /// <summary>
    /// Bütçe kaynak bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class ButceKaynakClass
    {
        /// <summary>
        /// Bütçe kaynak varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen bütçe kaynak nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kaynak">Bütçe kaynak bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, ButceKaynak kaynak, bool yeni, bool kontrolEt)
        {
            Sonuc donusDeger;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));

            if (kontrolEt)
            {
                donusDeger = Yetki.YazmaHakkiVarMi(kullanan);
                if (string.IsNullOrEmpty(kaynak.kaynakAd))
                {
                    donusDeger.DegerAta(YatirimResI.YAT021);
                }

                if (!Genel.AltProjeVarMi(ve, yil, senaryoKod, kaynak.altProjeKodu))
                {
                    donusDeger.DegerAta(YatirimResI.YAT022);
                }

                if (!donusDeger.islemSonuc)
                {
                    ve.VeriErisimKapat();
                    return donusDeger;
                }
            }
            else
                donusDeger = new Sonuc();

            TABYATButceKaynak tabBK = new TABYATButceKaynak(ve);
            ve.BeginTransaction();

            try
            {
                if (donusDeger.islemSonuc)
                {
                    if (yeni)
                    {
                        Sozluk s = new Sozluk(0, kaynak.kaynakAd);
                        donusDeger = IsKurallari.Genel.TanimSozlukEkle(ve, kullanan, 3, s, false);
                        if (donusDeger.islemSonuc)
                        {
                            kaynak.kaynakNo = OrtakFonksiyonlar.ConvertToInt(donusDeger.anahtar, 0);
                            donusDeger = tabBK.Kaydet(yil, senaryoKod, kaynak);
                        }
                    }
                    else
                        donusDeger = tabBK.Guncelle(yil, senaryoKod, kaynak);
                }
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
        /// Bütçe kaynağın detay bilgilerini, ödenek teklif  bilgilerini,
        /// harcama  bilgilerini ve geri ödeme bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen bütçe kaynak nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kaynak">Bütçe kaynak bilgisini tutan nesne</param>
        /// <param name="yil">Bütçe kaynağının yılı</param>
        /// <param name="senaryoKod">Gelen verilerin kaydedileceği senaryo</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc KaydetDetay(VeriErisim veriErisim, Kullanici kullanan, ButceKaynak kaynak, int yil, string senaryoKod)
        {
            Sonuc donusDeger = new Sonuc();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            ButceKaynak eskiKaynak = null;

            if (kaynak.kaynakNo <= 0)
            {
                donusDeger.DegerAta(YatirimResI.YAT023);
            }
            else
            {
                eskiKaynak = KaynakOku(ve, yil, senaryoKod, kaynak.kaynakNo, null);
                if (eskiKaynak == null)
                {
                    donusDeger.DegerAta(YatirimResI.YAT024);
                }
            }

            if (!donusDeger.islemSonuc)
            {
                ve.VeriErisimKapat();
                return donusDeger;
            }

            Sonuc hak = ProjeClass.ProjeAltProjeYazmaHakkiVarMi(ve, kullanan, 0, eskiKaynak.altProjeKodu, yil, senaryoKod);
            if (hak != null)
            {
                ve.VeriErisimKapat();
                return hak;
            }

            TABYATButceKaynak tabBK = new TABYATButceKaynak(ve);

            ve.BeginTransaction();

            try
            {
                if (donusDeger.islemSonuc)
                    donusDeger = tabBK.GuncelleTanim(yil, senaryoKod, kaynak);

                if (donusDeger.islemSonuc && kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                {
                    tabBK.SilPebKod(yil, senaryoKod, kaynak.kaynakNo);
                    foreach (AltFaaliyet peb in kaynak.pebKodlar)
                    {
                        if (!string.IsNullOrWhiteSpace(peb.pebKod))
                        {
                            peb.yil = yil;
                            peb.altProje = kaynak.altProjeKodu;
                            peb.butceKaynakNo = kaynak.kaynakNo;
                            donusDeger = tabBK.KaydetPebKod(peb, senaryoKod);
                            if (!donusDeger.islemSonuc)
                                break;
                        }
                    }
                }

                TABYATYENIPROJEBUTCE tabyp = new TABYATYENIPROJEBUTCE(ve);
                donusDeger = tabyp.Sil(yil, senaryoKod, kaynak.kaynakNo);

                if (kaynak.kaynakTur == ENUMButceKaynakTur.YENIPROJE)
                {
                    ButceKaynakTeklif teklifYil = new ButceKaynakTeklif();
                    teklifYil.butceYil = yil;
                    foreach (YeniProjeButce item in kaynak.yeniTeklifler)
                    {
                        teklifYil.Odenek0Dis += item.disKredi0;
                        teklifYil.Odenek0Toplam += (item.disKredi0 + item.icKaynak0);

                        teklifYil.Odenek1Dis += item.disKredi1;
                        teklifYil.Odenek1Toplam += (item.disKredi1 + item.icKaynak1);

                        teklifYil.Odenek2Dis += item.disKredi2;
                        teklifYil.Odenek2Toplam += (item.disKredi2 + item.icKaynak2);

                        teklifYil.ProjeTutarDis += item.projeTutariDis;
                        teklifYil.ProjeTutarToplam += (item.projeTutariDis + item.projeTutariIc);

                        donusDeger = tabyp.Ekle(yil, senaryoKod, kaynak.kaynakNo, item);
                        if (!donusDeger.islemSonuc) break;
                    }
                    if (kaynak.teklifler == null) kaynak.teklifler = new TNSCollection();
                    else kaynak.teklifler.Clear();
                    kaynak.teklifler.Add(teklifYil);
                }

                if (donusDeger.islemSonuc && kaynak.teklifler.Count > 0)
                    donusDeger = ButceKaynakTeklifClass.Kaydet(ve, kullanan, kaynak.kaynakNo, yil, senaryoKod, kaynak.teklifler);

                if (donusDeger.islemSonuc)
                    donusDeger = ButceKrediDetayClass.Kaydet(ve, yil, senaryoKod, kaynak.kaynakNo, kaynak.krediPlan);

                if (donusDeger.islemSonuc)
                    donusDeger = ButceKaynakGeriOdemeClass.Kaydet(ve, yil, senaryoKod, kaynak.kaynakNo, kaynak.geriOdeme);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.Message;
            }

            if (donusDeger.islemSonuc)
            {
                donusDeger.anahtar = kaynak.kaynakNo.ToString();
                ve.Commit();
            }
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            ButceKaynakTeklifClass.OdenekAktar(kullanan, yil, senaryoKod, kaynak.kaynakNo);
            return donusDeger;
        }

        ///// <summary>
        ///// Verilen kriterlere uygun olan bütçe kaynak bilgilerini döndüren yordam
        ///// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="yil">Yıl kriteri</param>
        ///// <param name="senaryoKod">Senaryo kriteri</param>
        ///// <param name="altProjeKodu">Yatırım alt projesinin programatik kodu</param>
        ///// <param name="detayVer">Bütçe kaynağın detay bilgileri listelensin mi listelenmesin mi</param>
        ///// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında bütçe kaynaklar döndürülür.</returns>
        //public static BilgiVAN Listele(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, int altProjeKodu, bool detayVer, bool odenekDegisimVar, string birimKod, bool eskiYillarKBO)
        //{
        //    return BilgiOku(veriErisim, kullanan, 0, altProjeKodu, 0, yil, senaryoKod, string.Empty, null, detayVer, false, birimKod, false, null, eskiYillarKBO);
        //}

        ///// <summary>
        ///// Verilen kriterlere uygun olan bütçe kaynak bilgilerini döndüren yordam
        ///// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="kaynakNo">Listelenmek istenen bütçe kaynağın programatik kodu</param>
        ///// <param name="yil">Yıl kriteri</param>
        ///// <param name="senaryoKod">Senaryo kriteri</param>
        ///// <param name="detayVer">Bütçe kaynağın detay bilgileri listelensin mi listelenmesin mi</param>
        ///// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında bütçe kaynaklar döndürülür.</returns>
        //public static BilgiVAN Listele(VeriErisim veriErisim, Kullanici kullanan, int kaynakNo, int yil, string senaryoKod, bool detayVer, string birimKod)
        //{
        //    return BilgiOku(veriErisim, kullanan, 0, kaynakNo, yil, senaryoKod, string.Empty, null, detayVer, false, birimKod, false, null, false);
        //}

        /// <summary>
        /// Verilen kriterlere uygun olan bütçe kaynak bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="altProjeKodu">Yatırım alt projesinin programatik kodu</param>
        /// <param name="kaynakNo">Listelenmek istenen bütçe kaynağın programatik kodu</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="tur">Listelenmek istenen bütçe kaynak türleri</param>
        /// <param name="detayVer">Bütçe kaynağın detay bilgileri listelensin mi listelenmesin mi</param>
        /// <param name="tumYillar">Projenin geçmiş yıl rakamlarını da getirir.</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında bütçe kaynaklar döndürülür.</returns>
        public static ButceKaynak KaynakOku(VeriErisim ve, int yil, string senaryoKod, int kaynakNo, string projeAd)
        {
            TABYATButceKaynak tabBK = new TABYATButceKaynak(ve);
            IDataReader dr = tabBK.Listele(yil, senaryoKod, 0, 0, kaynakNo, null, null, projeAd);
            ButceKaynak gelen = null;
            if (dr != null)
            {
                int sonAlan;
                if (dr.Read())
                {
                    gelen = ButceKaynakOl(dr, out sonAlan);
                }
                dr.Close();
            }
            return gelen;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan bütçe kaynak bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="altProjeKodu">Yatırım alt projesinin programatik kodu</param>
        /// <param name="kaynakNo">Listelenmek istenen bütçe kaynağın programatik kodu</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="tur">Listelenmek istenen bütçe kaynak türleri</param>
        /// <param name="detayVer">Bütçe kaynağın detay bilgileri listelensin mi listelenmesin mi</param>
        /// <param name="tekYil">Verilen yılın rakamlarını getirir.</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında bütçe kaynaklar döndürülür.</returns>
        public static BilgiVAN BilgiOku(VeriErisim veriErisim, Kullanici kullanan, int projeKodu, int altProjeKodu, int kaynakNo, int yil, string senaryoKod, string sektorKod, ENUMButceKaynakTur[] tur, bool detayVer, bool odenekDegisimVar, string birimKodu, bool tekYil, string projeAd, bool eskiYillarKBO)
        {
            BilgiVAN donusDeger = new BilgiVAN();
            if (string.IsNullOrEmpty(senaryoKod))
                donusDeger.sonuc.DegerAta("Senaryo boş olamaz");

            if (yil == 0)
                donusDeger.sonuc.DegerAta("Yıl boş olamaz");

            if (!donusDeger.sonuc.islemSonuc)
                return donusDeger;

            donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan, birimKodu);
            if (!donusDeger.sonuc.islemSonuc)
                return donusDeger;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATButceKaynak tabBK = new TABYATButceKaynak(ve);
            IDataReader dr = tabBK.Listele(yil, senaryoKod, projeKodu, altProjeKodu, kaynakNo, tur, sektorKod, projeAd);

            if (dr != null)
            {
                int sonAlan;
                while (dr.Read())
                {
                    donusDeger.liste.Add(ButceKaynakOl(dr, out sonAlan));
                }
                dr.Close();
            }
            else
            {
                donusDeger.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return donusDeger;
            }

            if (detayVer)
            {
                BilgiVAN detayGelen = new BilgiVAN();
                foreach (ButceKaynak bk in donusDeger.liste)
                {
                    detayGelen = ListelePebKod(ve, kullanan, bk.kaynakNo, yil, birimKodu, senaryoKod);
                    if (!detayGelen.sonuc.islemSonuc)
                        break;
                    bk.pebKodlar.AddRange(detayGelen.liste);

                    detayGelen = ButceKaynakTeklifClass.Listele(ve, kullanan, bk.kaynakNo, yil, senaryoKod, 0, tekYil, birimKodu, eskiYillarKBO);
                    if (!detayGelen.sonuc.islemSonuc)
                        break;
                    bk.teklifler.AddRange(detayGelen.liste);

                    bk.yeniTeklifler = ButceKaynakTeklifClass.YeniProjeListele(ve, kullanan, bk.kaynakNo, yil, senaryoKod);

                    detayGelen = ButceKaynakGeriOdemeClass.Listele(ve, yil, senaryoKod, bk.kaynakNo);
                    if (!detayGelen.sonuc.islemSonuc)
                        break;
                    bk.geriOdeme.AddRange(detayGelen.liste);

                    ObjectArray krediDetay = ButceKrediDetayClass.Listele(ve, yil, senaryoKod, bk.kaynakNo);
                    if (!krediDetay.sonuc.islemSonuc)
                        break;
                    bk.krediPlan.AddRange(krediDetay.objeler);

                }
                donusDeger.sonuc = detayGelen.sonuc;
            }

            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan bütçe kaynak bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="altProjeKodu">Yatırım alt projesinin programatik kodu</param>
        /// <param name="kaynakNo">Listelenmek istenen bütçe kaynağın programatik kodu</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="tur">Listelenmek istenen bütçe kaynak türleri</param>
        /// <param name="detayVer">Bütçe kaynağın detay bilgileri listelensin mi listelenmesin mi</param>
        /// <param name="tekYil">Verilen yılın rakamlarını getirir.</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında bütçe kaynaklar döndürülür.</returns>
        public static TNSCollection BilgiOku(int yil, string okunanXML)
        {
            IDataReader dr = new XMLStringDataReader(okunanXML);
            TNSCollection donusDeger = new TNSCollection();
            if (dr != null)
            {
                while (dr.Read())
                {
                    int sonAlan;
                    ButceKaynak bk = ButceKaynakOl(dr, out sonAlan);
                    BilgiVAN detayGelen = ListelePebKod(yil, bk.kaynakNo, VeriErisim.DegerAl(dr, sonAlan++, ""));
                    bk.pebKodlar.AddRange(detayGelen.liste);
                    detayGelen = ButceKaynakTeklifClass.Listele(VeriErisim.DegerAl(dr, sonAlan++, ""));
                    bk.teklifler.AddRange(detayGelen.liste);
                    bk.yeniTeklifler = ButceKaynakTeklifClass.YeniProjeListele(yil, VeriErisim.DegerAl(dr, sonAlan++, ""));

                    bk.odenekDegisimler = OdenekDegisimOl(VeriErisim.DegerAl(dr, sonAlan++, ""));
                    bk.harcamalar = OdenekDegisimOl(VeriErisim.DegerAl(dr, sonAlan++, ""));
                    bk.odenekler = OdenekDegisimOl(VeriErisim.DegerAl(dr, sonAlan++, ""));
                    /*
                     * 

                                        detayGelen = ButceKaynakGeriOdemeClass.Listele(ve, yil, senaryoKod, bk.kaynakNo);
                                        if (!detayGelen.sonuc.islemSonuc)
                                            break;
                                        bk.geriOdeme.AddRange(detayGelen.liste);

                                        ObjectArray krediDetay = ButceKrediDetayClass.Listele(ve, yil, senaryoKod, bk.kaynakNo);
                                        if (!krediDetay.sonuc.islemSonuc)
                                            break;
                                        bk.krediPlan.AddRange(krediDetay.objeler);
                     * */
                    donusDeger.Add(bk);
                    //DebugYaz(bk);
                }
                dr.Close();
            }
            return donusDeger;
        }

        //private static void DebugYaz(ButceKaynak bk)
        //{
        //    foreach (ButceKaynakTeklif g in bk.teklifler)
        //    {
        //        OrtakFonksiyonlar.HataDosyaYaz("Kaynak.txt", string.Format("g.kaynakNo:{0}, g.ButceYil:{1}, g.DusulenYilSonuToplam:{2}, g.EklenenYilSonuToplam:{3}, g.Harcama6ayToplam:{4}, g.HarcamaTahminToplam:{5}, g.HarcamaYilSonuToplam:{6}, g.KumulatifHarcamaToplam:{7}, g.Odenek0Toplam:{8}, g.ProjeTutarToplam:{9}", g.kaynakNo, g.ButceYil, g.DusulenYilSonuToplam, g.EklenenYilSonuToplam, g.Harcama6ayToplam, g.HarcamaTahminToplam, g.HarcamaYilSonuToplam, g.KumulatifHarcamaToplam, g.Odenek0Toplam, g.ProjeTutarToplam), false);
        //    }
        //    OrtakFonksiyonlar.HataDosyaYaz("Kaynak.txt", string.Format("bk.kaynakNo:{0}, bk.kaynakAd:{1}, bk.harcama_Toplam:{2}, bk.kumulatifHarcama_Toplam:{3}, bk.odenek_Toplam:{4}, bk.projeToplam_Toplam:{5}, bk.revizeOdenek_Toplam:{6}", bk.kaynakNo, bk.kaynakAd, bk.harcama_Toplam, bk.kumulatifHarcama_Toplam, bk.odenek_Toplam, bk.projeToplam_Toplam, bk.revizeOdenek_Toplam), false);
        //}

        private static TNSCollection OdenekDegisimOl(string okunanXML)
        {
            TNSCollection donusDeger = new TNSCollection();
            IDataReader dr = new XMLStringDataReader(okunanXML);
            if (dr != null)
            {
                while (dr.Read())
                {
                    KaynakOdenekBilgi bkt = new KaynakOdenekBilgi();
                    bkt.ay = VeriErisim.DegerAl(dr, 0, 0);
                    bkt.finKod = VeriErisim.DegerAl(dr, 1, "");
                    bkt.ekoKod = VeriErisim.DegerAl(dr, 2, "");
                    bkt.odenek = VeriErisim.DegerAl(dr, 3, 0.0);
                    donusDeger.Add(bkt);
                }
                dr.Close();
            }
            else
            {
            }
            return donusDeger;
        }

        private static ButceKaynak ButceKaynakOl(IDataReader dr, out int sonAlanDonus)
        {
            ButceKaynak gelen = new ButceKaynak();
            gelen.kaynakNo = VeriErisim.DegerAl(dr, 0, -1);
            gelen.altProjeKodu = VeriErisim.DegerAl(dr, 1, -1);
            gelen.krediyiVeren = VeriErisim.DegerAl(dr, 2, string.Empty);
            gelen.baslamaTarih = VeriErisim.DegerAl(dr, 3);
            gelen.yururlukTarih = VeriErisim.DegerAl(dr, 4);
            gelen.tur = VeriErisim.DegerAl(dr, 5, string.Empty);
            gelen.tutar = VeriErisim.DegerAl(dr, 6, string.Empty);
            gelen.vade = VeriErisim.DegerAl(dr, 7, string.Empty);
            gelen.odemesizDonem = VeriErisim.DegerAl(dr, 8, string.Empty);
            gelen.faizOran = VeriErisim.DegerAl(dr, 9, string.Empty);
            gelen.IcParaKrediOran = VeriErisim.DegerAl(dr, 10, string.Empty);
            gelen.kaynakTur = (ENUMButceKaynakTur)VeriErisim.DegerAl(dr, 11, 0);
            gelen.toplamaDahil = (ButceKaynak.ENUMKaynakToplam)VeriErisim.DegerAl(dr, 12, 0);
            gelen.kaynakAd = VeriErisim.DegerAl(dr, 13, string.Empty);
            gelen.DPTRapordaGor = VeriErisim.DegerAl(dr, 14, 0) == 1;
            //15 yıl
            //16 senaryokod
            gelen.pbKod = VeriErisim.DegerAl(dr, 17, 1);
            gelen.dipnot = VeriErisim.DegerAl(dr, 18, string.Empty);
            gelen.yikobMu = VeriErisim.DegerAl(dr, 19, 0);

            int sonAlan = 19;
            gelen.altProjeAdi = VeriErisim.DegerAl(dr, sonAlan + 1, string.Empty);
            gelen.dptProjeNo = VeriErisim.DegerAl(dr, sonAlan + 2, string.Empty);
            gelen.dptProjeAd = VeriErisim.DegerAl(dr, sonAlan + 3, string.Empty);
            gelen.birim = VeriErisim.DegerAl(dr, sonAlan + 4, string.Empty);
            gelen.pbAdi = VeriErisim.DegerAl(dr, sonAlan + 5, string.Empty);
            gelen.pbKisaltma = VeriErisim.DegerAl(dr, sonAlan + 6, string.Empty);

            gelen.projeNo = VeriErisim.DegerAl(dr, sonAlan + 7, 0);
            gelen.sektorNo = VeriErisim.DegerAl(dr, sonAlan + 8, "");
            gelen.sektorAd = VeriErisim.DegerAl(dr, sonAlan + 9, "");
            sonAlanDonus = sonAlan + 10;
            return gelen;
        }

        private static BilgiVAN ListelePebKod(VeriErisim veriErisim, Kullanici kullanan, int kaynakNo, int yil, string birimKodu, string senaryoKod)
        {
            BilgiVAN donusDeger = new BilgiVAN();

            donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan, birimKodu);
            if (!donusDeger.sonuc.islemSonuc)
                return donusDeger;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATButceKaynak tabBK = new TABYATButceKaynak(ve);
            IDataReader dr = tabBK.ListelePebKod(kaynakNo, yil, senaryoKod);

            if (dr == null)
            {
                donusDeger.sonuc.DegerAta(ve.hata);
            }
            else
            {
                PebKodListeOl(yil, kaynakNo, dr, donusDeger);
            }

            ve.VeriErisimKapat();
            return donusDeger;
        }

        private static void PebKodListeOl(int yil, int kaynakNo, IDataReader dr, BilgiVAN donusDeger)
        {
            string kodYapi = TNS.DEG.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD14/KODYAPI");
            while (dr.Read())
            {
                AltFaaliyet gelen = new AltFaaliyet();
                gelen.yil = yil;
                gelen.butceKaynakNo = kaynakNo;
                gelen.pebKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapi, VeriErisim.DegerAl(dr, 0, string.Empty));
                gelen.altProje = VeriErisim.DegerAl(dr, 1, -1);
                gelen.pebAd = VeriErisim.DegerAl(dr, 2, string.Empty);

                donusDeger.liste.Add(gelen);
            }
            dr.Close();
        }

        private static BilgiVAN ListelePebKod(int yil, int kaynakNo, string okunanXML)
        {
            BilgiVAN donusDeger = new BilgiVAN();
            IDataReader dr = new XMLStringDataReader(okunanXML);
            if (dr != null)
            {
                PebKodListeOl(yil, kaynakNo, dr, donusDeger);
            }
            else
            {
                donusDeger.sonuc.DegerAta("Okunan XML çözümlenemedi:" + okunanXML);
            }
            return donusDeger;
        }

        /// <summary>
        /// Bütçe kaynak, herhangi bir detay bilgiye sahip değilse silen,
        /// detay bilgileri varsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        /// bütçe kaynak nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kaynakNo">silinecek kaynağın numarası</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, int altProjeNo, int kaynakNo)
        {
            Sonuc donusDeger = Yetki.YazmaHakkiVarMi(kullanan);
            if (!donusDeger.islemSonuc)
                return donusDeger;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATButceKaynak tabBK = new TABYATButceKaynak(ve);
            ve.BeginTransaction();

            if (kaynakNo <= 0)
            {
                donusDeger.DegerAta(YatirimResI.YAT023);
            }

            if (tabBK.KaynakKullanilmisMi(yil, senaryoKod, kaynakNo))
            {
                string kaynakAd = YATServis.UzayYaziDegerGetir("YATBUTCEKAYNAK", yil + "-" + senaryoKod + "-" + kaynakNo.ToString(), true, string.Empty);
                donusDeger.DegerAta(string.Format(YatirimResI.YAT025, kaynakAd));
            }

            if (donusDeger.islemSonuc)
                donusDeger = tabBK.Sil(yil, senaryoKod, kaynakNo);

            if (donusDeger.islemSonuc)
            {
                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Bütçe kaynağın detay bilgilerini, ödenek teklif  bilgilerini,
        /// harcama  bilgilerini ve geri ödeme bilgilerini silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        /// bütçe kaynak nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="bklar">Bütçe kaynak nesnelerinden oluşan listeyi tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc SilDetay(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, int altProjeKodu, int kaynakNo)
        {
            Sonuc donusDeger = new Sonuc();
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                donusDeger.DegerAta("Projeden kaynak silme yetkiniz bulunmamaktadır!");
                return donusDeger;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATButceKaynak tabBK = new TABYATButceKaynak(ve);
            ve.BeginTransaction();

            if (kaynakNo <= 0)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = YatirimResI.YAT023;
            }
            if (!Genel.ButceKaynakVarMi(ve, yil, senaryoKod, kaynakNo))
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = YatirimResI.YAT024;
            }

            if (donusDeger.islemSonuc)
                donusDeger = tabBK.SilPebKod(yil, senaryoKod, kaynakNo);

            if (donusDeger.islemSonuc)
                donusDeger = ButceKaynakTeklifClass.Sil(ve, kullanan, yil, senaryoKod, altProjeKodu, kaynakNo);

            if (donusDeger.islemSonuc)
                donusDeger = ButceKaynakGeriOdemeClass.Sil(ve, yil, senaryoKod, kaynakNo);

            if (donusDeger.islemSonuc)
                donusDeger = ButceKrediDetayClass.Sil(ve, yil, senaryoKod, kaynakNo);

            if (donusDeger.islemSonuc)
            {
                ve.Commit();
                donusDeger.bilgiStr = YatirimResI.YAT020;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return donusDeger;
        }

        internal static Sonuc YATKaynakKaydetTopluVeri(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, ButceKaynak[] bklar, ENUMProjeMaliYapiKod[] teklifYillar)
        {
            Sonuc donusDeger = Yetki.YazmaHakkiVarMi(kullanan);
            if (!donusDeger.islemSonuc)
                return donusDeger;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATButceKaynakTeklif tabKT = new TABYATButceKaynakTeklif(ve);
            ve.BeginTransaction();

            foreach (ButceKaynak item in bklar)
            {
                for (int i = 0; i < teklifYillar.Length; i++)
                {
                    ProjeMaliYapi pmy = item.ProjeMaliYapiBul(teklifYillar[i]);
                    if (pmy != null)
                    {
                        donusDeger = tabKT.Sil(yil, item.kaynakNo, senaryoKod, -1, teklifYillar[i]);
                        if (donusDeger.islemSonuc)
                            donusDeger = tabKT.Kaydet(yil, senaryoKod, item.kaynakNo, teklifYillar[i], pmy);
                        if (!donusDeger.islemSonuc) break;
                    }
                }
                if (!donusDeger.islemSonuc) break;
            }

            if (donusDeger.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return donusDeger;
        }

        internal static Sonuc KaynakTasi(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, int kaynakNo, int apKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATButceKaynak tabKaynak = new TABYATButceKaynak(ve);
            ve.BeginTransaction();
            Sonuc donusDeger = tabKaynak.Tasi(yil, senaryoKod, kaynakNo, apKod);
            if (donusDeger.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return donusDeger;
        }
    }
}