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
    /// Proje türleriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class ProjeTur
    {
        /// <summary>
        /// Proje tür varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="projeTur">Proje tür sözlük bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(Kullanici kullanan, Sozluk projeTur)
        {
            return IslemYap(null, (int)ENUMSozlukIslemTur.KAYDET, kullanan, projeTur);
        }

        /// <summary>
        /// Proje tür, yatırım projelerinde ve/veya alt projelerinde kullanılmamışsa silen,
        /// kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="projeTur">Proje tür sözlük bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(Kullanici kullanan, Sozluk projeTur)
        {
            return IslemYap(null, (int)ENUMSozlukIslemTur.SIL, kullanan, projeTur);
        }

        /// <summary>
        /// Gelen işlem türüne göre proje türünü kaydeden veya silen yordam
        /// islemTur parametresinin değeri 0 ise kayıt, 1 ise silme işlemi yapar.
        /// Kullanıcı işleme yetkili değilse veya gönderilen sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="islemTur">İşlem kayıt mı, silme mi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="projeTur">Proje tür sözlük bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private static Sonuc IslemYap(VeriErisim veriErisim, int islemTur, Kullanici kullanan, Sozluk projeTur)
        {
            Sonuc donusDeger = Yetki.SozlukYazmaHakkiVarMi(kullanan);
            if (!donusDeger.islemSonuc)
                return donusDeger;

            if (islemTur == (int)ENUMSozlukIslemTur.KAYDET)
                donusDeger = Genel.SozlukKaydetKontrol(projeTur);
            else
                donusDeger = Genel.SozlukSilKontrol(projeTur.kod);
            if (!donusDeger.islemSonuc)
                return donusDeger;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATProjeTur tabProjeTur = new TABYATProjeTur(ve);
            ve.BeginTransaction();

            try
            {
                if (islemTur == (int)ENUMSozlukIslemTur.KAYDET)
                {
                    donusDeger = tabProjeTur.Guncelle(projeTur);
                    if (!donusDeger.islemSonuc)
                        donusDeger = tabProjeTur.Kaydet(projeTur);
                }
                else
                {
                    if (Genel.ProjeTurProjedeKullanilmisMi(ve, projeTur.kod) || Genel.ProjeTurAltProjedeKullanilmisMi(ve, projeTur.kod))
                    {
                        donusDeger.islemSonuc = false;
                        donusDeger.hataStr = YatirimResI.YAT051;
                    }
                    else
                        donusDeger = tabProjeTur.Sil(projeTur);
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
        /// Verilen kriterlere uygun olan proje tür bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="projeTur">Proje tür kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında proje türleri listesi döndürülür.</returns>
        public static BilgiVAN Listele(VeriErisim veriErisim, Kullanici kullanan, Sozluk projeTur)
        {
            BilgiVAN donusDeger = new BilgiVAN();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATProjeTur tabProjeTur = new TABYATProjeTur(ve);
            IDataReader dr = null;
            if (projeTur == null) projeTur = new Sozluk();
            try
            {
                dr = tabProjeTur.Listele(projeTur);
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