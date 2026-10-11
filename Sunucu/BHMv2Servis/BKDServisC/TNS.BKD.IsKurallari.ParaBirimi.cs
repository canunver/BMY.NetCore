using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.DEG;
using TNS.KYM;
using TNS.Veri;
using TNS.BKD.IsKurallari.Veri;
using System.Collections;

namespace TNS.BKD.IsKurallari
{
    /// <summary>
    /// Para birimi tanım bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class BKDParaBirimi
    {
        /// <summary>
        /// Verilen para birimi tanım bilgisini veri tabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="paraBirimi">Kaydedilecek para birimi tanım bilgilerini tutan nesne.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(Kullanici kullanan, ParaBirimi paraBirimi)
        {
            Sonuc sonuc = KodServisi.KodDegistirmeYetkisiVarMi(kullanan);
            if(!sonuc.islemSonuc)
            {
                sonuc.hataStr = ButceKodResI.BKDPB001;
                return sonuc;
            }

            string hata = "";

            if (paraBirimi.kod <= 0)
                hata = ButceKodResI.BKDPB002 + "<br>";

            if (paraBirimi.adi == string.Empty)
                hata += ButceKodResI.BKDPB003 + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBKD", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPARABIRIMI to = new TABPARABIRIMI(ve);
            ve.BeginTransaction();

            sonuc = to.GuncelleYadaEkle(paraBirimi);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = ButceKodResI.BKDPB004;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = ButceKodResI.BKDPB005 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Verilen kritere uyan, para birimleri veri tabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="kriterParaBirimi">Silinecek para birimlerinin uyacağı kriter bilgilerini tutan yapı.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(Kullanici kullanan, ParaBirimi kriterParaBirimi)
        {
            Sonuc sonuc = KodServisi.KodDegistirmeYetkisiVarMi(kullanan);
            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = ButceKodResI.BKDPB006;
                return sonuc;
            }

            if (kriterParaBirimi.kod == 0)
                sonuc.hataStr = ButceKodResI.BKDPB002;

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBKD", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPARABIRIMI to = new TABPARABIRIMI(ve);

            ve.BeginTransaction();

            sonuc = to.Sil(kriterParaBirimi);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = string.Format(ButceKodResI.BKDPB007,  kriterParaBirimi.kod) + "<br>";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = ButceKodResI.BKDPB008 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Verilen kritere uyan, para birimlerinin listesini döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="kriterParaBirimi">Döndürülecek para birimlerinin uyacağı kriter bilgilerini tutan yapı.</param>
        /// <returns>Verilen kritere uyan, para birimlerinin dizisi döndürülür.</returns>
        public static ParaBirimi[] Listele(Kullanici kullanan, ParaBirimi kriterParaBirimi)
        {
            ArrayList donusDeger = new ArrayList();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBKD", TNS.KYM.Arac.KimlikBul(kullanan));

            TABPARABIRIMI to = new TABPARABIRIMI(ve);
            IDataReader r = to.Listele(kriterParaBirimi);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    ParaBirimi f = new ParaBirimi();

                    f.kod = VeriErisim.DegerAl(r, 0, 0);
                    f.adi = VeriErisim.DegerAl(r, 1, "");
                    f.kisaltma = VeriErisim.DegerAl(r, 2, "");

                    donusDeger.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return (ParaBirimi[])donusDeger.ToArray(typeof(ParaBirimi));
        }

        //public DataTable Listele(Kullanici kullanan)
        //{
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("kod");
        //    dt.Columns.Add("adi");
        //    dt.Columns.Add("kisaltma");

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBKD");

        //    TABPARABIRIMI to = new TABPARABIRIMI(ve);
        //    ParaBirimi kriter = new ParaBirimi();
        //    IDataReader r = to.Listele(kriter);

        //    if (to.Hata() == "")
        //    {
        //        int kod = 0;
        //        string ad = string.Empty;
        //        string kisaltma = string.Empty;
        //        while (r.Read())
        //        {
        //            ParaBirimi f = new ParaBirimi();

        //            kod = VeriErisim.DegerAl(r, 0, 0);
        //            ad = VeriErisim.DegerAl(r, 1, "");
        //            kisaltma = VeriErisim.DegerAl(r, 2, "");

        //            dt.Rows.Add(kod, ad, kisaltma);
        //        }
        //    }

        //    r.Close();

        //    ve.VeriErisimKapat();

        //    return dt;
        //}
    }
}