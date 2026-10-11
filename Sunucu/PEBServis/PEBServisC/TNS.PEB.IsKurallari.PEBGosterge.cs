using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// Performans gösterge tanım bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class PEBGosterge
    {
        /// <summary>
        /// Performans gösterge varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen PerformansGosterge nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="gosterge">Performans gösterge bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, PerformansGosterge gosterge)
        {
            Sonuc sonuc = Yetki.YazmaHakkiVarMi(kullanan, gosterge.kurumKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = string.Empty;
            if (string.IsNullOrEmpty(gosterge.gostergeAd))
                hata = PerformansButceResI.PEB060 + "<br>";
            if (gosterge.olcuBirim.kod <= 0)
                hata += PerformansButceResI.PEB061 + "<br>";
            if (gosterge.tur <= 0)
                hata += PerformansButceResI.PEB062 + "<br>";
            if (gosterge.seviye <= 0)
                hata += PerformansButceResI.PEB064 + "<br>";
            if (gosterge.ilerlemeYonu <= 0)
                hata += PerformansButceResI.PEB065 + "<br>";
            if (string.IsNullOrWhiteSpace(gosterge.kurumKod))
                hata += "Kurumkod boş olamaz!<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.DegerAta(hata);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGosterge tGosterge = new TABGosterge(ve);
            ve.BeginTransaction();

            if (tGosterge.GostergeVarMi(gosterge.gostergeKod))
                sonuc = tGosterge.Guncelle(gosterge);
            else if (gosterge.gostergeKod > 0)
            {
                sonuc.DegerAta(PerformansButceResI.PEB066 + "<br>");
            }
            else
                sonuc = tGosterge.Kaydet(gosterge);
            sonuc.anahtar = gosterge.gostergeKod.ToString();
            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = PerformansButceResI.PEB015;
                ve.Commit();
            }
            else
            {
                sonuc.DegerAta(PerformansButceResI.PEB016 + "<br>" + sonuc.hataStr);
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Performans gösterge, hedeflenen ve gerçekleşen değerlerde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen PerformansGosterge nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="gosterge">Performans gösterge bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, PerformansGosterge gosterge)
        {
            Sonuc sonuc = Yetki.TanimlamaYazmaHakkiVarMi(kullanan);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = string.Empty;
            if (gosterge.gostergeKod <= 0)
                hata = PerformansButceResI.PEB067 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.DegerAta(hata);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGosterge tGosterge = new TABGosterge(ve);

            if (tGosterge.GostergeKullanilmisMi(gosterge.gostergeKod))
            {
                ve.VeriErisimKapat();
                sonuc.DegerAta(PerformansButceResI.PEB068);
                return sonuc;
            }

            ve.BeginTransaction();
            sonuc = tGosterge.Sil(gosterge);
            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PerformansButceResI.PEB020;
            }
            else
            {
                ve.Rollback();
                sonuc.DegerAta(PerformansButceResI.PEB021 + "<br>" + sonuc.hataStr);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan performans gösterge bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="gosterge">Performans gösterge bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans gösterge listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, PerformansGosterge gosterge)
        {
            ObjectArray gostergeVAN = new ObjectArray();
            gostergeVAN.sonuc = Yetki.OkumaHakkiVarMi(kullanan);
            if (!gostergeVAN.sonuc.islemSonuc)
                return gostergeVAN;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGosterge tGosterge = new TABGosterge(ve);

            IDataReader dr = tGosterge.Listele(gosterge);
            while (dr.Read())
            {
                PerformansGosterge gelen = new PerformansGosterge();
                gelen.gostergeKod = VeriErisim.DegerAl(dr, 0, 0);
                gelen.gostergeAd = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.olcuBirim.kod = VeriErisim.DegerAl(dr, 2, 0);
                gelen.tur = (ENUMGostergeTur)VeriErisim.DegerAl(dr, 3, 0);
                gelen.ondalikSayisi = VeriErisim.DegerAl(dr, 4, 0);
                gelen.seviye = VeriErisim.DegerAl(dr, 5, 0);
                gelen.ilerlemeYonu = (ENUMGostergeIlerlemeYonu)VeriErisim.DegerAl(dr, 6, 0);
                gelen.ebutceKod = VeriErisim.DegerAl(dr, 7, string.Empty);
                gelen.kurumKod = VeriErisim.DegerAl(dr, 8, string.Empty);
                gelen.olcuBirim.ad = VeriErisim.DegerAl(dr, 9, string.Empty);
                gelen.olcuBirim.ebutceKod = VeriErisim.DegerAl(dr, 10, string.Empty);

                gostergeVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (gostergeVAN.objeler.Count == 0)
                gostergeVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return gostergeVAN;
        }

        ///// <summary>
        ///// Verilen kriterlere uygun olan performans gösterge bilgilerini döndüren yordam
        ///// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="gosterge">Performans gösterge bilgisini tutan nesne</param>
        ///// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans gösterge listesi döndürülür.</returns>
        //public static ObjectArray ListeleEskiDB(VeriErisim veriErisim, Kullanici kullanan, PerformansGosterge gosterge)
        //{
        //    ObjectArray gostergeVAN = new ObjectArray();
        //    gostergeVAN.sonuc = Yetki.OkumaHakkiVarMi(kullanan);
        //    if (!gostergeVAN.sonuc.islemSonuc)
        //        return gostergeVAN;

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEBY", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABGosterge tGosterge = new TABGosterge(ve);

        //    IDataReader dr = tGosterge.Listele(gosterge);
        //    while (dr.Read())
        //    {
        //        PerformansGosterge gelen = new PerformansGosterge();
        //        gelen.gostergeKod = VeriErisim.DegerAl(dr, 0, 0);
        //        gelen.gostergeAd = VeriErisim.DegerAl(dr, 1, string.Empty);
        //        gelen.olcuBirim.kod = VeriErisim.DegerAl(dr, 2, 0);
        //        gelen.olcuBirim.ad = PEBServis.UzayYaziDegerGetir("PEBOLCUBIRIMY", gelen.olcuBirim.kod.ToString(), true, "");
        //        gelen.olcuBirim.ebutceKod = PEBServis.UzayYaziDegerGetir("PEBOLCUBIRIMY", gelen.olcuBirim.kod.ToString(), true, "EBUTCEKOD");
        //        gelen.tur = VeriErisim.DegerAl(dr, 3, 0);
        //        gelen.kullanimSekli = VeriErisim.DegerAl(dr, 4, 0);
        //        gelen.seviye = VeriErisim.DegerAl(dr, 5, 0);
        //        gelen.ilerlemeYonu = VeriErisim.DegerAl(dr, 6, 0);
        //        gelen.ebutceKod = VeriErisim.DegerAl(dr, 7, string.Empty);

        //        gostergeVAN.objeler.Add(gelen);
        //    }
        //    dr.Close();

        //    if (gostergeVAN.objeler.Count == 0)
        //        gostergeVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

        //    ve.VeriErisimKapat();
        //    return gostergeVAN;
        //}
    }
}