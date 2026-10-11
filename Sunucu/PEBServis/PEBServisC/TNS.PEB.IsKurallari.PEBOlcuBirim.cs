using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// Ölçü Birim tanım bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class PEBOlcuBirim
    {
        /// <summary>
        /// Ölçü birim varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen OlcuBirim nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="olcu">Ölçü birim bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, OlcuBirim olcu)
        {
            Sonuc sonuc = Yetki.TanimlamaYazmaHakkiVarMi(kullanan);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = string.Empty;
            if (string.IsNullOrEmpty(olcu.ad))
                hata = PerformansButceResI.PEB084 + "<br>";
            if (string.IsNullOrEmpty(olcu.ebutceKod))
                hata = PerformansButceResI.PEB085 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.DegerAta(hata);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOlcuBirim tOlcu = new TABOlcuBirim(ve);
            ve.BeginTransaction();

            if (tOlcu.OlcuBirimVarMi(olcu.kod))
                sonuc = tOlcu.Guncelle(olcu);
            else if (olcu.kod > 0)
            {
                sonuc.DegerAta(PerformansButceResI.PEB086 + "<br>");
            }
            else
            {
                sonuc = tOlcu.Kaydet(olcu);
                olcu.kod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar.Trim(), 0);
            }

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
        /// Ölçü birim, performans göstergede kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen OlcuBirim nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="olcu">Ölçü birim bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, OlcuBirim olcu)
        {
            Sonuc sonuc = Yetki.TanimlamaYazmaHakkiVarMi(kullanan);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = string.Empty;
            if (olcu.kod <= 0)
                hata = PerformansButceResI.PEB087 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.DegerAta(hata);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOlcuBirim tOlcu = new TABOlcuBirim(ve);

            if (tOlcu.OlcuBirimKullanilmisMi(olcu.kod))
            {
                ve.VeriErisimKapat();
                sonuc.DegerAta(PerformansButceResI.PEB088);
                return sonuc;
            }

            ve.BeginTransaction();
            sonuc = tOlcu.Sil(olcu);
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
        /// Verilen kriterlere uygun olan ölçü birim bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="olcu">Ölçü birim bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ölçü birim listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, OlcuBirim olcu)
        {
            ObjectArray olcuVAN = new ObjectArray();
            olcuVAN.sonuc = Yetki.OkumaHakkiVarMi(kullanan);
            if (!olcuVAN.sonuc.islemSonuc)
                return olcuVAN;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOlcuBirim tOlcu = new TABOlcuBirim(ve);

            IDataReader dr = tOlcu.Listele(olcu);
            while (dr.Read())
            {
                OlcuBirim gelen = new OlcuBirim();
                gelen.kod = VeriErisim.DegerAl(dr, 0, 0);
                gelen.ad = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.ebutceKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                olcuVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (olcuVAN.objeler.Count == 0)
                olcuVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return olcuVAN;
        }

        ///// <summary>
        ///// Verilen kriterlere uygun olan ölçü birim bilgilerini döndüren yordam
        ///// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="olcu">Ölçü birim bilgisini tutan nesne</param>
        ///// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ölçü birim listesi döndürülür.</returns>
        //public static ObjectArray ListeleEskiDB(VeriErisim veriErisim, Kullanici kullanan, OlcuBirim olcu)
        //{
        //    ObjectArray olcuVAN = new ObjectArray();
        //    olcuVAN.sonuc = Yetki.OkumaHakkiVarMi(kullanan);
        //    if (!olcuVAN.sonuc.islemSonuc)
        //        return olcuVAN;

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEBY");
        //    TABOlcuBirim tOlcu = new TABOlcuBirim(ve);

        //    IDataReader dr = tOlcu.Listele(olcu);
        //    while (dr.Read())
        //    {
        //        OlcuBirim gelen = new OlcuBirim();
        //        gelen.kod = VeriErisim.DegerAl(dr, 0, 0);
        //        gelen.ad = VeriErisim.DegerAl(dr, 1, string.Empty);
        //        gelen.ebutceKod = VeriErisim.DegerAl(dr, 2, string.Empty);
        //        olcuVAN.objeler.Add(gelen);
        //    }
        //    dr.Close();

        //    if (olcuVAN.objeler.Count == 0)
        //        olcuVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

        //    ve.VeriErisimKapat();
        //    return olcuVAN;
        //}
    }
}