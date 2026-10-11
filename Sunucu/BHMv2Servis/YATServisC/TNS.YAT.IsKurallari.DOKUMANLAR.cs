using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.KYM;
using TNS.Veri;
using TNS.YAT.IsKurallari.Veri;

namespace TNS.YAT.IsKurallari
{
    /// <summary>
    /// Yatırım alt projesinin doküman bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class DokumanClass
    {
        /// <summary>
        /// Doküman varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen doküman nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="dokuman">Doküman bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, Dokuman dokuman)
        {
            Sonuc donusDeger = Yetki.YazmaHakkiVarMi(kullanan);
            if (!donusDeger.islemSonuc)
                return donusDeger;

            if (dokuman.ad == "")
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = YatirimResI.YAT010;
                return donusDeger;
            }

            if (dokuman.tur.kod == 0)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = YatirimResI.YAT033;
                return donusDeger;
            }

            if (dokuman.dosyaAd == "")
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = YatirimResI.YAT034;
                return donusDeger;
            }

            if (dokuman.altProjeKodu == 0)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = YatirimResI.YAT035;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATDokuman tabDokuman = new TABYATDokuman(ve);
            ve.BeginTransaction();

            if (!Genel.AltProjeVarMi(ve, yil, senaryoKod, dokuman.altProjeKodu))
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = YatirimResI.YAT022;
            }

            if (dokuman.siraNo != "")
            {
                if (!Genel.DokumanVarMi(ve, dokuman.siraNo))
                {
                    donusDeger.islemSonuc = false;
                    donusDeger.hataStr = YatirimResI.YAT036;
                }
            }

            if (!donusDeger.islemSonuc)
            {
                ve.VeriErisimKapat();
                return donusDeger;
            }

            try
            {
                if (dokuman.siraNo == "")
                    donusDeger = tabDokuman.Kaydet(dokuman);
                else
                    donusDeger = tabDokuman.Guncelle(dokuman);
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
        /// Verilen kriterlere uygun olan dokümanları döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="altProjeKodu">Yatırım alt projesinin programatik kodu</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında dokümanlar döndürülür.</returns>
        public static BilgiVAN Listele(VeriErisim veriErisim, Kullanici kullanan, int altProjeKodu)
        {
            BilgiVAN donusDeger = new BilgiVAN();

            donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan);
            if (!donusDeger.sonuc.islemSonuc)
                return donusDeger;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATDokuman tabDokuman = new TABYATDokuman(ve);
            IDataReader dr = null;

            try
            {
                dr = tabDokuman.Listele(altProjeKodu);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                ve.VeriErisimKapat();
                return donusDeger;
            }

            while (dr.Read())
            {
                Dokuman gelen = new Dokuman();
                gelen.altProjeKodu = VeriErisim.DegerAl(dr, 0, -1);
                gelen.siraNo = VeriErisim.DegerAl(dr, 1, "");
                gelen.tur.kod = VeriErisim.DegerAl(dr, 2, -1);
                if (gelen.tur.kod != -1)
                    gelen.tur.ad = YATServis.UzayYaziDegerGetir("YATDOKUMANTUR", gelen.tur.kod.ToString(), true, "");
                gelen.ad = VeriErisim.DegerAl(dr, 3, "");
                gelen.dosyaAd = VeriErisim.DegerAl(dr, 4, "");

                donusDeger.liste.Add(gelen);
            }

            if (donusDeger.liste.Count == 0)
                donusDeger.sonuc.bilgiStr = YatirimResI.YAT017;

            donusDeger.sonuc.islemSonuc = true;

            dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Doküman programatik kodu verilen dokümanı silen yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="siraNo">Doküman programatik kodu</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, string siraNo)
        {
            Sonuc donusDeger = Yetki.YazmaHakkiVarMi(kullanan);
            if (!donusDeger.islemSonuc)
                return donusDeger;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATDokuman tabDokuman = new TABYATDokuman(ve);
            ve.BeginTransaction();

            try
            {
                donusDeger = tabDokuman.Sil(siraNo);
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
    }
}