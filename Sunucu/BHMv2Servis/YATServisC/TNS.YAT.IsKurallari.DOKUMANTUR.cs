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
    /// Doküman türleriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class DokumanTur
    {
        /// <summary>
        /// Doküman tür varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="dokumanTur">Doküman tür sözlük bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(Kullanici kullanan, Sozluk dokumanTur)
        {
            return IslemYap(null, (int)ENUMSozlukIslemTur.KAYDET, kullanan, dokumanTur);
        }

        /// <summary>
        /// Doküman tür, alt projelere doküman eklenirken kullanılmamışsa silen,
        /// kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="dokumanTur">Doküman tür sözlük bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(Kullanici kullanan, Sozluk dokumanTur)
        {
            return IslemYap(null, (int)ENUMSozlukIslemTur.SIL, kullanan, dokumanTur);
        }

        /// <summary>
        /// Gelen işlem türüne göre doküman türünü kaydeden veya silen yordam
        /// islemTur parametresinin değeri 0 ise kayıt, 1 ise silme işlemi yapar.
        /// Kullanıcı işleme yetkili değilse veya gönderilen sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="islemTur">İşlem kayıt mı, silme mi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="dokumanTur">Doküman tür sözlük bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private static Sonuc IslemYap(VeriErisim veriErisim, int islemTur, Kullanici kullanan, Sozluk dokumanTur)
        {
            Sonuc donusDeger = Yetki.SozlukYazmaHakkiVarMi(kullanan);
            if (!donusDeger.islemSonuc)
                return donusDeger;

            if (islemTur == (int)ENUMSozlukIslemTur.KAYDET)
                donusDeger = Genel.SozlukKaydetKontrol(dokumanTur);
            else
                donusDeger = Genel.SozlukSilKontrol(dokumanTur.kod);
            if (!donusDeger.islemSonuc)
                return donusDeger;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATDokumanTur tabDokumanTur = new TABYATDokumanTur(ve);
            ve.BeginTransaction();

            try
            {
                if (islemTur == (int)ENUMSozlukIslemTur.KAYDET)
                {
                    donusDeger = tabDokumanTur.Guncelle(dokumanTur);
                    if (!donusDeger.islemSonuc)
                        donusDeger = tabDokumanTur.Kaydet(dokumanTur);
                }
                else
                {
                    if (Genel.DokumanTurKullanilmisMi(ve, dokumanTur.kod))
                    {
                        donusDeger.islemSonuc = false;
                        donusDeger.hataStr = YatirimResI.YAT037;
                    }
                    else
                        donusDeger = tabDokumanTur.Sil(dokumanTur);
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
        /// Verilen kriterlere uygun olan doküman tür bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="dokumanTur">Doküman tür kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında doküman türleri listesi döndürülür.</returns>
        public static BilgiVAN Listele(VeriErisim veriErisim, Kullanici kullanan, Sozluk dokumanTur)
        {
            BilgiVAN donusDeger = new BilgiVAN();

            donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan);
            if (!donusDeger.sonuc.islemSonuc)
                return donusDeger;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATDokumanTur tabDokumanTur = new TABYATDokumanTur(ve);
            IDataReader dr = null;

            try
            {
                dr = tabDokumanTur.Listele(dokumanTur);
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
                Sozluk gelen = new Sozluk();
                gelen.kod = VeriErisim.DegerAl(dr, 0, -1);
                gelen.ad = VeriErisim.DegerAl(dr, 1, "");

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