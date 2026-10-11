using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// Performans göstergesi hedeflenen değerleriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class PEBGostergeHedef
    {
        /// <summary>
        /// Performans göstergesi hedeflenen değerlerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen PerformansGostergeHedeflenen nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pHedeflenen">Performans göstergesi hedeflenen değerlerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, PerformansGostergeHedeflenen pHedeflenen)
        {
            Sonuc sonuc = new Sonuc();
            string hata = string.Empty;
            if (pHedeflenen.yil <= 0)
                sonuc.DegerAta(PerformansButceResI.PEB039 + "<br>");
            if (string.IsNullOrEmpty(pHedeflenen.ghKurKod))
                sonuc.DegerAta(PerformansButceResI.PEB005 + "<br>");
            else
            {
                int uz = pHedeflenen.ghKurKod.Replace(".", "").Length;
                if (uz != 8) sonuc.DegerAta("Lütfen kayıt için birim seçiniz!");
            }
            if (pHedeflenen.asama < 0)
                sonuc.DegerAta(PerformansButceResI.PEB075 + "<br>");

            if (pHedeflenen.gostergeDegerler.Count <= 0)
                sonuc.DegerAta(PerformansButceResI.PEB071 + "<br>");

            string ephKurkod = "";
            int ephKod = 0;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            for (int i = 0; i < pHedeflenen.gostergeDegerler.Count; i++)
            {
                PerformansGostergeHedeflenenDegerler pghd = (PerformansGostergeHedeflenenDegerler)pHedeflenen.gostergeDegerler[i];
                if (pghd.perHedefKod != ephKod || pghd.phKurKod != ephKurkod)
                {
                    ephKod = pghd.perHedefKod;
                    ephKurkod = pghd.phKurKod;
                    if (pghd.perHedefKod <= 0)
                        hata += PerformansButceResI.PEB069 + "<br>";
                    if (!PEBGenel.PerformansHedefiVarMi(ve, pHedeflenen.yil, pghd.phKurKod, pghd.perHedefKod))
                    {
                        sonuc.DegerAta(PerformansButceResI.PEB072 + "<br>");
                    }
                }
                if (!PEBGenel.GostergeVarMi(ve, pghd.gosterge.gostergeKod, (int)ENUMGostergeKullanimSekli.Hizmet))
                {
                    sonuc.DegerAta(string.Format(PerformansButceResI.PEB073, pghd.gosterge.gostergeKod.ToString()));
                }
            }

            Sonuc yzsonuc = Yetki.YazmaHakkiVarMi(kullanan, pHedeflenen.ghKurKod);
            if (!yzsonuc.islemSonuc)
                sonuc.DegerAta(yzsonuc.hataStr);

            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }

            TABGostergeHedef tGHedef = new TABGostergeHedef(ve);
            ve.BeginTransaction();

            ephKurkod = "";
            ephKod = 0;
            for (int i = 0; i < pHedeflenen.gostergeDegerler.Count; i++)
            {
                PerformansGostergeHedeflenenDegerler pghd = (PerformansGostergeHedeflenenDegerler)pHedeflenen.gostergeDegerler[i];
                if (pghd.perHedefKod != ephKod || pghd.phKurKod != ephKurkod)
                {
                    ephKod = pghd.perHedefKod;
                    ephKurkod = pghd.phKurKod;
                    pHedeflenen.phKurKod = pghd.phKurKod;
                    pHedeflenen.perHedefKod = pghd.perHedefKod;
                    sonuc = tGHedef.Sil(pHedeflenen);
                    if (!sonuc.islemSonuc) break;
                }
            }

            int ilkAsama = pHedeflenen.asama;
            foreach (PerformansGostergeHedeflenenDegerler gHDeger in pHedeflenen.gostergeDegerler)
            {
                if (!sonuc.islemSonuc)
                    break;
                pHedeflenen.phKurKod = gHDeger.phKurKod;
                pHedeflenen.perHedefKod = gHDeger.perHedefKod;
                for (int i = 0; i < gHDeger.miktar.Length; i++)
                {
                    if (!(gHDeger.miktar[i] == 0 && string.IsNullOrEmpty(gHDeger.aciklama[i])))
                    {
                        pHedeflenen.asama = ilkAsama + (i * 10);
                        sonuc = tGHedef.Kaydet(pHedeflenen, gHDeger, i);
                    }

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
        /// Verilen kriterlere uygun olan performans göstergesi hedeflenen değerleri bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen PerformansGostergeHedeflenen nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pHedeflenen">Performans göstergesi hedeflenen değerlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans göstergesi hedeflenen değerler listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, PerformansGostergeHedeflenen pHedeflenen)
        {
            ObjectArray hedeflenenVAN = new ObjectArray();
            hedeflenenVAN.sonuc = Yetki.OkumaHakkiVarMi(kullanan, pHedeflenen.ghKurKod);
            if (!hedeflenenVAN.sonuc.islemSonuc)
                return hedeflenenVAN;

            string hata = string.Empty;
            if (pHedeflenen.yil <= 0)
                hata += PerformansButceResI.PEB039 + "<br>";
            //if (string.IsNullOrEmpty(pHedeflenen.ghKurKod))
            //    hata += PerformansButceResI.PEB005 + "<br>";
            if (pHedeflenen.asama < 0)
                hata += PerformansButceResI.PEB075 + "<br>";
            //if (pHedeflenen.perHedefKod <= 0)
            //    hata += PerformansButceResI.PEB069 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                hedeflenenVAN.sonuc.islemSonuc = false;
                hedeflenenVAN.sonuc.hataStr = hata;
                return hedeflenenVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGostergeHedef tGHedef = new TABGostergeHedef(ve);

            PerformansGostergeHedeflenen gelen = null;

            PerformansGostergeHedeflenenDegerler hDeger = null;
            int eskiGosterge = 0;
            IDataReader dr = tGHedef.Listele(pHedeflenen);
            string eskiphKurKod = "";
            int eskiperHedefKod = 0;
            string eskighKurKod = "";
            int perHedefKolon;
            if (!string.IsNullOrEmpty(pHedeflenen.ghKurKod))
                perHedefKolon = 11;
            else
                perHedefKolon = 9;
            while (dr.Read())
            {
                string phKurKod = VeriErisim.DegerAl(dr, 0, "");
                int perHedefKod = VeriErisim.DegerAl(dr, 1, 0);
                string ghKurKod = VeriErisim.DegerAl(dr, 2, "");
                int asama = VeriErisim.DegerAl(dr, 3, 0);
                int gostergeKod = VeriErisim.DegerAl(dr, 4, 0);

                if (gelen == null || eskiphKurKod != phKurKod || eskiperHedefKod != perHedefKod || eskighKurKod != ghKurKod)
                {
                    //if (gelen != null)
                    //    GerceklesmeDoldur(ve2, gelen, gercekYil);
                    gelen = new PerformansGostergeHedeflenen();
                    gelen.yil = pHedeflenen.yil;
                    gelen.phKurKod = phKurKod;
                    gelen.ghKurKod = ghKurKod;
                    gelen.asama = pHedeflenen.asama;
                    gelen.perHedefKod = perHedefKod;
                    gelen.perHedefAd = VeriErisim.DegerAl(dr, perHedefKolon, "");
                    gelen.perHedefGorKod = VeriErisim.DegerAl(dr, perHedefKolon + 1, "");
                    eskiGosterge = 0;
                    hedeflenenVAN.objeler.Add(gelen);

                    eskiphKurKod = phKurKod;
                    eskiperHedefKod = perHedefKod;
                    eskighKurKod = ghKurKod;
                }

                if (eskiGosterge == 0 || eskiGosterge != gostergeKod)
                {
                    hDeger = new PerformansGostergeHedeflenenDegerler();
                    hDeger.gosterge.gostergeKod = gostergeKod;
                    hDeger.gosterge.gostergeAd = VeriErisim.DegerAl(dr, 5, string.Empty);
                    hDeger.gosterge.olcuBirim.ad = VeriErisim.DegerAl(dr, 6, string.Empty);
                    hDeger.gosterge.ebutceKod = VeriErisim.DegerAl(dr, 7, string.Empty);

                    gelen.gostergeDegerler.Add(hDeger);

                    eskiGosterge = gostergeKod;
                }

                hDeger.miktar[(int)(asama / 10)] = VeriErisim.DegerAl(dr, 8, (double)0);

                if (!string.IsNullOrEmpty(pHedeflenen.ghKurKod))
                {
                    hDeger.aciklama[(int)(asama / 10)] = VeriErisim.DegerAl(dr, 9, string.Empty);
                    hDeger.siraNo = VeriErisim.DegerAl(dr, 10, 0);
                }
            }
            //if (gelen != null)
            //    GerceklesmeDoldur(ve2, gelen, gercekYil);

            dr.Close();
            if (hedeflenenVAN.objeler.Count == 0)
                hedeflenenVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();
            return hedeflenenVAN;
        }

        //private static void GerceklesmeDoldur(VeriErisim ve2, PerformansGostergeHedeflenen gelen, int gercekYil)
        //{
        //    if (gercekYil > 0)
        //    {
        //        TABGostergeGercek tabGG = new TABGostergeGercek(ve2);
        //        PerformansGostergeGerceklesen gGercek = new PerformansGostergeGerceklesen();
        //        gGercek.yil = gercekYil;
        //        gGercek.phKurKod = gelen.phKurKod;
        //        gGercek.perHedefKod = gelen.perHedefKod;
        //        gGercek.ay = 12;
        //        gGercek.ggKurKod = gelen.ghKurKod;

        //        IDataReader dr = tabGG.Listele(gGercek);
        //        if (dr != null)
        //        {
        //            while (dr.Read())
        //            {
        //                string phKurKod = VeriErisim.DegerAl(dr, 1, "");
        //                int perHedefKod = VeriErisim.DegerAl(dr, 2, 0);
        //                int gelenGosterge = VeriErisim.DegerAl(dr, 4, 0);
        //                PerformansGostergeHedeflenenDegerler g = GostergeBul(gelen.gostergeDegerler, gelenGosterge);
        //                if (g == null)
        //                {
        //                    g = new PerformansGostergeHedeflenenDegerler();
        //                    g.gosterge = new PerformansGosterge();
        //                    g.gosterge.gostergeKod = gelenGosterge;
        //                }
        //                g.miktar[4] = VeriErisim.DegerAl(dr, 5, (double)0);
        //            }
        //            dr.Close();
        //        }
        //    }
        //}

        private static PerformansGostergeHedeflenenDegerler GostergeBul(TNSCollection gostergeDegerler, int gelenGostergeKod)
        {
            foreach (PerformansGostergeHedeflenenDegerler hDeger in gostergeDegerler)
            {
                if (hDeger.gosterge.gostergeKod == gelenGostergeKod)
                    return hDeger;
            }
            return null;
        }

        internal static Sonuc Kopyala(VeriErisim veriErisim, Kullanici kullanan, int yil, string ghKurKod, int kaynakAsama, int hedefAsama)
        {
            ObjectArray hedeflenenVAN = new ObjectArray();
            Sonuc sonuc = Yetki.OkumaHakkiVarMi(kullanan, ghKurKod);
            if (!hedeflenenVAN.sonuc.islemSonuc)
                return sonuc;

            string hata = string.Empty;
            if (yil <= 0)
                hata += PerformansButceResI.PEB039 + "<br>";

            if (kaynakAsama < 0)
                hata += PerformansButceResI.PEB075 + "<br>";
            if (hedefAsama < 0)
                hata += PerformansButceResI.PEB075 + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.DegerAta(hata);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGostergeHedef tGHedef = new TABGostergeHedef(ve);

            ve.BeginTransaction();

            sonuc = tGHedef.Kopyala(yil, ghKurKod, kaynakAsama, hedefAsama);
            if (sonuc.islemSonuc)
                sonuc = tGHedef.Kopyala(yil, ghKurKod, kaynakAsama + 10, hedefAsama + 10);
            if (sonuc.islemSonuc)
                sonuc = tGHedef.Kopyala(yil, ghKurKod, kaynakAsama + 20, hedefAsama + 20);
            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }
    }
}