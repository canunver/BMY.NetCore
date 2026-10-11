using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// İşlem tipi bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMIslemTip
    {
        /// <summary>
        /// İşlem tipi varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen IslemTip nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="islemTip">İşlem tipi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, IslemTip islemTip)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (!(islemTip.tur == (int)ENUMIslemTipi.ZFVERME || islemTip.tur == (int)ENUMIslemTipi.ZFDUSME || islemTip.tur == (int)ENUMIslemTipi.DTLVERME || islemTip.tur == (int)ENUMIslemTipi.DTLDUSME) && (islemTip.kod == 0))
                hata = TasinirMalResI.TMM074;

            if (islemTip.ad == "")
            {
                if (hata != "") hata += "<br>";
                hata += TasinirMalResI.TMM075;
            }

            if (islemTip.tur==0)
            {
                if (hata != "") hata += "<br>";
                hata += TasinirMalResI.TMM415;
            }

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            //Zimmet işlem tipi
            if (islemTip.tur == (int)ENUMIslemTipi.ZFVERME || islemTip.tur == (int)ENUMIslemTipi.ZFDUSME || islemTip.tur == (int)ENUMIslemTipi.DTLVERME || islemTip.tur == (int)ENUMIslemTipi.DTLDUSME)
                islemTip.kod = islemTip.tur == (int)ENUMIslemTipi.ZFVERME ? (int)ENUMZimmetIslemTipi.ZFVERME : islemTip.tur == (int)ENUMIslemTipi.ZFDUSME ? (int)ENUMZimmetIslemTipi.ZFDUSME : islemTip.tur == (int)ENUMIslemTipi.DTLVERME ? (int)ENUMZimmetIslemTipi.DTLVERME : islemTip.tur == (int)ENUMIslemTipi.DTLDUSME ? (int)ENUMZimmetIslemTipi.DTLDUSME : islemTip.kod;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIslemTip tabIslemTip = new TABIslemTip(ve);
            ve.BeginTransaction();

            try
            {
                sonuc = tabIslemTip.Guncelle(islemTip);
                if (!sonuc.islemSonuc)
                    sonuc = tabIslemTip.Kaydet(islemTip);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.Message;
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// İşlem tipi, taşınır işlem fişlerinde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen IslemTip nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="islemTip">İşlem tipi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, IslemTip islemTip)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM071, "");

            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (islemTip.kod == 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM074;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIslemTip tabIslemTip = new TABIslemTip(ve);

            sonuc = KullanilmisMi(islemTip, ve);
            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }

            ve.BeginTransaction();

            try
            {
                sonuc = tabIslemTip.Sil(islemTip);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.Message;
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan işlem tipi bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="islemTip">İşlem tipi bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında işlem tipi listesi döndürülür.</returns>
        public ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, IslemTip islemTip)
        {
            ObjectArray listeVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIslemTip tabIslemTip = new TABIslemTip(ve);
            IDataReader dr = null;

            try
            {
                dr = tabIslemTip.Listele(islemTip);
            }
            catch (Exception e)
            {
                listeVAN.sonuc.islemSonuc = false;
                listeVAN.sonuc.hataStr = e.Message;
                dr.Close();
                ve.VeriErisimKapat();
                return listeVAN;
            }

            while (dr.Read())
            {
                IslemTip gelen = new IslemTip();
                gelen.kod = VeriErisim.DegerAl(dr, 0, 0);
                gelen.ad = VeriErisim.DegerAl(dr, 1, "");
                gelen.tur = VeriErisim.DegerAl(dr, 2, 0);

                if (gelen.tur == (int)ENUMIslemTipi.ZFVERME || gelen.tur == (int)ENUMIslemTipi.ZFDUSME || gelen.tur == (int)ENUMIslemTipi.DTLVERME || gelen.tur == (int)ENUMIslemTipi.DTLDUSME)
                    continue;

                listeVAN.objeler.Add(gelen);
            }

            if (listeVAN.objeler.Count == 0)
                listeVAN.sonuc.bilgiStr = TasinirMalResI.TMM029;

            listeVAN.sonuc.islemSonuc = true;

            dr.Close();
            ve.VeriErisimKapat();
            return listeVAN;
        }

        /// <summary>
        /// Verilen işlem tipinin taşınır işlem fişlerinde kullanılıp kullanılmadığını kontrol eden yordam
        /// </summary>
        /// <param name="tip">İşlem tipi bilgilerini tutan nesne</param>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private Sonuc KullanilmisMi(IslemTip tip, VeriErisim ve)
        {
            TABIslemTip tabIslemTip = new TABIslemTip(ve);
            Sonuc sonuc = new Sonuc();

            IDataReader drVarMi = tabIslemTip.KullanilmisMi(tip);

            if (drVarMi.Read())
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM140;
            }

            drVarMi.Close();

            return sonuc;
        }
    }
}
