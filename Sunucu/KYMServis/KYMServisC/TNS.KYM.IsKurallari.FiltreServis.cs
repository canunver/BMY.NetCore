using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.KYM.IsKurallari.Veri;
using System.Threading;
using System.Collections;

namespace TNS.KYM.IsKurallari
{
    /// <summary>
    /// Ekranlarda girilmiş olan ve kullanıcı tarafından saklanmak istenen filtrelerle ilgili bilgilerin veri tabanına yazılması ve okunması ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public static class FiltreServis
    {
        /// <summary>
        /// Filtre nesnesi ile geçirilen bilgileri veri tabanına kaydeden yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="filtre">Kaydedilecek filtre nesnesi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Filtre filtre)
        {
            Sonuc donusDeger = new Sonuc();

            string hata = filtre.KontrolEt(false);
            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.DegerAta(hata);
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", null);
            TABFILTRE tabBS = new TABFILTRE(ve);

            try
            {
                ve.BeginTransaction();
                if (filtre.siraNo > 0)
                {
                    donusDeger = tabBS.Guncelle(filtre);
                }
                else
                {
                    filtre.siraNo = tabBS.AnahtarBul(filtre.kullanan.kullaniciKodu, filtre.grup);
                    donusDeger = tabBS.Kaydet(filtre);
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
        /// Verilen filtre bilgilesi veri tabanından silinir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="filtre">Veri tabanından silinecek filtre nesnesi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Filtre filtre)
        {
            Sonuc donusDeger = new Sonuc();
            string hata = filtre.KontrolEt(true);

            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = hata;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", null);
            TABFILTRE tabBS = new TABFILTRE(ve);
            ve.BeginTransaction();

            try
            {
                donusDeger = tabBS.Sil(filtre);
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
        /// Verilen kıstasa uyan filtrelerin listesini döndüren yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="grupAd">Listelenecek filtrelerin filtre grubu.</param>
        /// <param name="siraNo">0 ise grupdaki tüm kayıtlı filtreler, 0'dan farklı ise verilen sıradaki filtre kaydı okunur.</param>
        /// <returns>Verilen filtre kıstasına uyan filtrelerin listesi</returns>
        public static Filtre[] Listele(VeriErisim veriErisim, Kullanici kullanan, string grupAd, int siraNo)
        {
            ArrayList donusDeger = new ArrayList();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", kullanan.KimlikBul());
            TABFILTRE tabBS = new TABFILTRE(ve);
            IDataReader dr = null;

            try
            {
                dr = tabBS.Listele(kullanan.kullaniciKodu, grupAd, siraNo);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            while (dr.Read())
            {
                Filtre filtre = new Filtre();
                //filtre.kullaniciKodu = VeriErisim.DegerAl(dr, (int)TABBHMFILTRE.ENUMAlanlar.KULLANICIKODU, string.Empty);
                filtre.siraNo = VeriErisim.DegerAl(dr, (int)TABFILTRE.ENUMAlanlar.SIRANO, 0);
                //filtre.grup = VeriErisim.DegerAl(dr, (int)TABBHMFILTRE.ENUMAlanlar.GRUP, string.Empty);
                filtre.ad = VeriErisim.DegerAl(dr, (int)TABFILTRE.ENUMAlanlar.AD, string.Empty);
                filtre.kriter = VeriErisim.DegerAl(dr, (int)TABFILTRE.ENUMAlanlar.KRITER, string.Empty);

                donusDeger.Add(filtre);
            }

            if (donusDeger.Count == 0)
                return null;

            dr.Close();
            ve.VeriErisimKapat();
            return (Filtre[])donusDeger.ToArray(typeof(Filtre));
        }
    }
}
