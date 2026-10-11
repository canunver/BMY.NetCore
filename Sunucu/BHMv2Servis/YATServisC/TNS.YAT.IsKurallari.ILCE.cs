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
    /// İlçe bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class Ilce
    {
        /// <summary>
        /// İlçe varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ilçe sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ilce">İlçe sözlük bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(Kullanici kullanan, IlceSozluk ilce)
        {
            return IslemYap(null, (int)ENUMSozlukIslemTur.KAYDET, kullanan, ilce);
        }

        /// <summary>
        /// İlçe, yatırım projelerinde ve/veya alt projelerinde kullanılmamışsa silen,
        /// kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ilce">İlçe sözlük bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(Kullanici kullanan, IlceSozluk ilce)
        {
            return IslemYap(null, (int)ENUMSozlukIslemTur.SIL, kullanan, ilce);
        }

        /// <summary>
        /// Listeleme esnasında gelen ilçe sözlük bilgisinin eksiksiz olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ilce">Kontrol edilecek ilçe sözlük bilgisi</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private static Sonuc IlceSozlukListeleKontrol(IlceSozluk ilce)
        {
            Sonuc donusDeger = new Sonuc(true, "", "");

            if (ilce.il.kod == 0)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = YatirimResI.YAT040;
            }

            return donusDeger;
        }

        /// <summary>
        /// Silme esnasında gelen ilçe sözlük bilgisinin eksiksiz olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ilce">Kontrol edilecek ilçe sözlük bilgisi</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private static Sonuc IlceSozlukSilKontrol(IlceSozluk ilce)
        {
            Sonuc donusDeger = IlceSozlukListeleKontrol(ilce);

            if (!donusDeger.islemSonuc)
                return donusDeger;

            if (ilce.ilceKodu == 0)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = YatirimResI.YAT041;
            }

            return donusDeger;
        }

        /// <summary>
        /// Kaydetme esnasında gelen ilçe sözlük bilgisinin eksiksiz olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ilce">Kontrol edilecek ilçe sözlük bilgisi</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private static Sonuc IlceSozlukKaydetKontrol(IlceSozluk ilce)
        {
            Sonuc donusDeger = IlceSozlukSilKontrol(ilce);

            if (!donusDeger.islemSonuc)
                return donusDeger;

            if (ilce.ilceAdi.Trim() == "")
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = YatirimResI.YAT042;
            }

            return donusDeger;
        }

        /// <summary>
        /// Gönderilen ilin sistemde kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="ilKodu">İl kodu kriteri</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private static Sonuc IlVarMi(VeriErisim ve, int ilKodu)
        {
            Sonuc donusDeger = new Sonuc(true, "", "");
            TABYATIl tabIl = new TABYATIl(ve);

            if (!tabIl.IlVarMi(ilKodu))
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = YatirimResI.YAT043;
            }

            return donusDeger;
        }

        /// <summary>
        /// Gelen işlem türüne göre ilçe bilgisini kaydeden veya silen yordam
        /// islemTur parametresinin değeri 0 ise kayıt, 1 ise silme işlemi yapar.
        /// Kullanıcı işleme yetkili değilse veya gönderilen ilçe sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="islemTur">İşlem kayıt mı, silme mi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ilce">İlçe sözlük bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private static Sonuc IslemYap(VeriErisim veriErisim, int islemTur, Kullanici kullanan, IlceSozluk ilce)
        {
            Sonuc donusDeger = Yetki.SozlukYazmaHakkiVarMi(kullanan);
            if (!donusDeger.islemSonuc)
                return donusDeger;

            if (islemTur == (int)ENUMSozlukIslemTur.KAYDET)
                donusDeger = IlceSozlukKaydetKontrol(ilce);
            else
                donusDeger = IlceSozlukSilKontrol(ilce);
            if (!donusDeger.islemSonuc)
                return donusDeger;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATIlce tabIlce = new TABYATIlce(ve);
            ve.BeginTransaction();

            donusDeger = IlVarMi(ve, ilce.il.kod);
            if (!donusDeger.islemSonuc)
            {
                ve.VeriErisimKapat();
                return donusDeger;
            }

            try
            {
                if (islemTur == (int)ENUMSozlukIslemTur.KAYDET)
                {
                    donusDeger = tabIlce.Guncelle(ilce);
                    if (!donusDeger.islemSonuc)
                        donusDeger = tabIlce.Kaydet(ilce);
                }
                else
                {
                    if (Genel.IlceKullanilmisMi(ve, ilce))
                    {
                        donusDeger.islemSonuc = false;
                        donusDeger.hataStr = YatirimResI.YAT044;
                    }
                    else
                        donusDeger = tabIlce.Sil(ilce);
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
        /// Verilen kriterlere uygun olan ilçe bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ilce">İlçe kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ilçe listesi döndürülür.</returns>
        public static BilgiVAN ListeleOneri(VeriErisim veriErisim, Kullanici kullanan, IlceSozluk ilce)
        {
            BilgiVAN donusDeger = new BilgiVAN();

            //donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan);
            //if (!donusDeger.sonuc.islemSonuc)
            //    return donusDeger;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATIlce tabIlce = new TABYATIlce(ve);
            IDataReader dr = null;

            try
            {
                dr = tabIlce.ListeleOneri(ilce);
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
                IlceSozluk gelen = new IlceSozluk();
                gelen.il.kod = VeriErisim.DegerAl(dr, 0, 0);
                gelen.ilceKodu = VeriErisim.DegerAl(dr, 1, 1);
                gelen.ilceAdi = VeriErisim.DegerAl(dr, 2, "") + " - " + VeriErisim.DegerAl(dr, 3, "");

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
        /// Verilen kriterlere uygun olan ilçe bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ilçe sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ilce">İlçe kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ilçe listesi döndürülür.</returns>
        public static BilgiVAN Listele(VeriErisim veriErisim, Kullanici kullanan, IlceSozluk ilce)
        {
            BilgiVAN donusDeger = new BilgiVAN();

            //donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan);
            //if (!donusDeger.sonuc.islemSonuc)
            //    return donusDeger;

            donusDeger.sonuc = IlceSozlukListeleKontrol(ilce);
            if (!donusDeger.sonuc.islemSonuc)
                return donusDeger;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATIlce tabIlce = new TABYATIlce(ve);
            IDataReader dr = null;

            try
            {
                dr = tabIlce.Listele(ilce);
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
                IlceSozluk gelen = new IlceSozluk();
                gelen.il.kod = VeriErisim.DegerAl(dr, 0, -1);
                gelen.ilceKodu = VeriErisim.DegerAl(dr, 1, -1);
                gelen.ilceAdi = VeriErisim.DegerAl(dr, 2, "");

                donusDeger.liste.Add(gelen);
            }

            if (donusDeger.liste.Count == 0)
                donusDeger.sonuc.bilgiStr = YatirimResI.YAT017;

            donusDeger.sonuc.islemSonuc = true;

            dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }
    }
}