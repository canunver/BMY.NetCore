using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// Performans göstergesi gerçekleşen değerleriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class PEBGostergeGercek
    {
        /// <summary>
        /// Performans göstergesi gerçekleşen değerlerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen PerformansGostergeGerceklesen nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="gerceklesenler">Performans göstergesi gerçekleşen değerlerini tutan PerformansGostergeGerceklesen yapısında nesneler tutan dizi.</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TNSCollection gerceklesenler)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string hata = string.Empty;

            foreach (PerformansGostergeGerceklesen pGerceklesen in gerceklesenler)
            {

                sonuc = Yetki.YazmaHakkiVarMi(kullanan, pGerceklesen.ggKurKod);
                if (!sonuc.islemSonuc)
                    hata += sonuc.hataStr;

                if (pGerceklesen.yil <= 0)
                    hata += PerformansButceResI.PEB039 + "<br>";
                if (string.IsNullOrEmpty(pGerceklesen.ggKurKod))
                    hata += PerformansButceResI.PEB005 + "<br>";
                if (pGerceklesen.perHedefKod <= 0)
                    hata += PerformansButceResI.PEB069 + "<br>";
                if (pGerceklesen.ay <= 0)
                    hata += PerformansButceResI.PEB070 + "<br>";
                if (pGerceklesen.degerler.Count <= 0)
                    hata += PerformansButceResI.PEB071 + "<br>";
            }
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.DegerAta(hata);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGostergeGercek tGercek = new TABGostergeGercek(ve);

            foreach (PerformansGostergeGerceklesen pGerceklesen in gerceklesenler)
            {
                if (!PEBGenel.PerformansHedefiVarMi(ve, pGerceklesen.yil, pGerceklesen.phKurKod, pGerceklesen.perHedefKod))
                {
                    hata += PerformansButceResI.PEB072 + "<br>";
                }
                if (PEBGostergeGercekOnay.OnayliMi(ve, kullanan, pGerceklesen))
                {
                    hata += PerformansButceResI.PEB117 + "<br>";
                }

                foreach (PerformansGostergeGerceklesenDeger gercekDeger in pGerceklesen.degerler)
                {
                    if (!PEBGenel.GostergeVarMi(ve, gercekDeger.gosterge.gostergeKod, (int)ENUMGostergeKullanimSekli.Hizmet))
                    {
                        hata += string.Format(PerformansButceResI.PEB073, gercekDeger.gosterge.gostergeKod.ToString());
                    }
                    if (gercekDeger.veriYok && gercekDeger.miktar != 0)
                    {
                        hata += "Veri yok işareti koymuş olmanıza rağmen, mikat girilmiş. Veri yok ise lütfen miktar alanına 0 giriniz!";
                    }
                }
            }

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.DegerAta(hata);
                ve.VeriErisimKapat();
                return sonuc;
            }

            ve.BeginTransaction();
            foreach (PerformansGostergeGerceklesen pGerceklesen in gerceklesenler)
            {
                foreach (PerformansGostergeGerceklesenDeger gercekDeger in pGerceklesen.degerler)
                {
                    tGercek.Sil(pGerceklesen, gercekDeger);
                    if (!(gercekDeger.miktar == 0
                        && string.IsNullOrEmpty(gercekDeger.kaynak)
                        && string.IsNullOrEmpty(gercekDeger.sonuc)
                        && string.IsNullOrEmpty(gercekDeger.sapma)
                        && string.IsNullOrEmpty(gercekDeger.onlem)
                        && gercekDeger.veriYok))
                        sonuc = tGercek.Kaydet(pGerceklesen, gercekDeger);

                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            if (sonuc.islemSonuc)
            {
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
        /// Verilen kriterlere uygun olan performans göstergesi gerçekleşen değerleri bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen PerformansGostergeGerceklesen nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pGerceklesen">Performans göstergesi gerçekleşen değerlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans göstergesi gerçekleşen değerler listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, PerformansGostergeGerceklesen pGerceklesen)
        {
            ObjectArray gerceklesenVAN = new ObjectArray();
            gerceklesenVAN.sonuc = Yetki.OkumaHakkiVarMi(kullanan, pGerceklesen.ggKurKod);
            if (!gerceklesenVAN.sonuc.islemSonuc)
                return gerceklesenVAN;

            string hata = string.Empty;
            if (pGerceklesen.yil <= 0)
                hata += PerformansButceResI.PEB039 + "<br>";
            if (string.IsNullOrEmpty(pGerceklesen.ggKurKod))
                hata += PerformansButceResI.PEB005 + "<br>";
            //if (pGerceklesen.perHedefKod <= 0)
            //    hata += PerformansButceResI.PEB069 + "<br>";
            //if (pGerceklesen.ay <= 0)
            //    hata += PerformansButceResI.PEB070 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                gerceklesenVAN.sonuc.islemSonuc = false;
                gerceklesenVAN.sonuc.hataStr = hata;
                return gerceklesenVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGostergeGercek tGercek = new TABGostergeGercek(ve);

            PerformansGostergeHedeflenen hedef = new PerformansGostergeHedeflenen();
            hedef.yil = pGerceklesen.yil;
            hedef.phKurKod = pGerceklesen.phKurKod;
            hedef.perHedefKod = pGerceklesen.perHedefKod;
            hedef.ghKurKod = pGerceklesen.ggKurKod;

            hedef.phKurAd = null;
            hedef.perHedefAd = null;
            hedef.ghKurAd = null;
            hedef.asama = 6;

            ObjectArray hedeflenenVAN = PEBGostergeHedef.Listele(ve, kullanan, hedef);
            if (!hedeflenenVAN.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return hedeflenenVAN;
            }
            if (hedeflenenVAN.objeler.Count <= 0)
            {
                ve.VeriErisimKapat();
                hedeflenenVAN.sonuc.bilgiStr = PerformansButceResI.PEB074;
                return hedeflenenVAN;
            }

            foreach (PerformansGostergeHedeflenen hedefDegerler in hedeflenenVAN.objeler)
            {
                PerformansGostergeGerceklesen gelen = new PerformansGostergeGerceklesen();
                gelen.yil = pGerceklesen.yil;
                gelen.phKurKod = hedefDegerler.phKurKod;
                gelen.phKurAd = hedefDegerler.phKurAd;
                gelen.ggKurKod = hedefDegerler.ghKurKod;
                gelen.ggKurAd = hedefDegerler.ghKurAd;
                gelen.perHedefKod = hedefDegerler.perHedefKod;
                gelen.perHedefAd = hedefDegerler.perHedefAd;
                //gelen.ay = pGerceklesen.ay;

                foreach (PerformansGostergeHedeflenenDegerler h in hedefDegerler.gostergeDegerler)
                {
                    PerformansGostergeGerceklesenDeger gDeger = new PerformansGostergeGerceklesenDeger();
                    gDeger.gosterge = h.gosterge;
                    gDeger.hedefMiktar = h.miktar;
                    gDeger.veriYok = true;
                    gelen.degerler.Add(gDeger);
                }
                gerceklesenVAN.objeler.Add(gelen);
            }

            IDataReader dr = tGercek.Listele(pGerceklesen);
            while (dr.Read())
            {
                string phKurKod = VeriErisim.DegerAl(dr, 1, "");
                int perHedefKod = VeriErisim.DegerAl(dr, 2, 0);
                string ggKurKod = VeriErisim.DegerAl(dr, 10, "");
                int gelenGosterge = VeriErisim.DegerAl(dr, 4, 0);

                PerformansGostergeGerceklesenDeger g = GostergeBul(gerceklesenVAN, phKurKod, perHedefKod, ggKurKod, gelenGosterge);
                if (g != null)
                {
                    if (pGerceklesen.ay > 0)
                    {
                        g.miktar = VeriErisim.DegerAl(dr, 5, (double)0);
                        g.veriYok = dr.IsDBNull(5);
                        g.kaynak = VeriErisim.DegerAl(dr, 6, string.Empty);
                        g.sonuc = VeriErisim.DegerAl(dr, 7, string.Empty);
                        g.sapma = VeriErisim.DegerAl(dr, 8, string.Empty);
                        g.onlem = VeriErisim.DegerAl(dr, 9, string.Empty);
                        break;
                    }
                    else
                    {
                        GGSade sade = new GGSade();
                        sade.ay = VeriErisim.DegerAl(dr, 3, 0);
                        sade.miktar = VeriErisim.DegerAl(dr, 5, (double)0);
                        sade.veriYok = dr.IsDBNull(5);
                        sade.kaynak = VeriErisim.DegerAl(dr, 6, string.Empty);
                        sade.sonuc = VeriErisim.DegerAl(dr, 7, string.Empty);
                        sade.sapma = VeriErisim.DegerAl(dr, 8, string.Empty);
                        sade.onlem = VeriErisim.DegerAl(dr, 9, string.Empty);
                        if (g.donemDegerler == null) g.donemDegerler = new TNSCollection();
                        g.donemDegerler.Add(sade);
                    }
                }
            }
            dr.Close();

              

            if (gerceklesenVAN.objeler.Count == 0)
                gerceklesenVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return gerceklesenVAN;
        }

        private static PerformansGostergeGerceklesenDeger GostergeBul(ObjectArray gerceklesenVAN, string phKurKod, int perHedefKod, string ggKurKod, int gelenGosterge)
        {
            foreach (PerformansGostergeGerceklesen pgg in gerceklesenVAN.objeler)
            {
                if (pgg.phKurKod == phKurKod && pgg.perHedefKod == perHedefKod && pgg.ggKurKod == ggKurKod)
                {
                    foreach (PerformansGostergeGerceklesenDeger g in pgg.degerler)
                    {
                        if (g.gosterge.gostergeKod == gelenGosterge)
                        {
                            return g;
                        }
                    }
                }
            }
            return null;
        }
    }
}