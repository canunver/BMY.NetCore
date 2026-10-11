using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using TNS.BKD;
using TNS.UZY;

namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// Bütçe girilirken bütçe gelir fişine girilen açıklamalar ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class BHMv2ButceGelirFisi
    {
        /// <summary>
        /// Bütçe gelir fişleri girilirken her bir tertip için girilen gerekçe yazısını veri tabanına kaydeden yordam.
        /// Yetki kontrolü yapılır. Veri tabanı bağlantısı açılır. Kayıtlı gerekçe silinir. Gelen gerekçe 2500 karakterlik parçalar haline getirilerek veri tabanına kaydedilir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tertip">Bütçe fişinin tertibi</param>
        /// <param name="gerekce">Bütçe tertibi için istenen ödeneğin gerekçesi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc KaydetGerekce(Kullanici kullanan, Tertip tertip, string gerekce)
        {
            Sonuc sonuc = new Sonuc(false, string.Empty, string.Empty);

            string kurkod = tertip.kodlar[0];

            bool yetkikontrol = BHMv2Genel.YazmaYetkisiVarMi(kullanan, kurkod, "00.00");
            if (!yetkikontrol)
            {
                sonuc.DegerAta(string.Format(ButceHazirlikResI.BHMBF001, tertip.TertipStrOl()));
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(tertip.yil), kullanan.KimlikBul());
            TABBHMBUTCEGELIRFISI tabButceFisi = new TABBHMBUTCEGELIRFISI(ve);
            ve.BeginTransaction();

            sonuc = tabButceFisi.SilGerekce(tertip);
            if (sonuc.islemSonuc)
            {
                int yazilan = 0;
                string parca = string.Empty;
                int satir = 0;
                int kalan = gerekce.Length;
                int parcaBoy;
                while (kalan > 0)
                {
                    if (kalan > 2500)
                        parcaBoy = 2500;
                    else
                        parcaBoy = kalan;

                    parca = gerekce.Substring(yazilan, parcaBoy);
                    yazilan = yazilan + parcaBoy;
                    kalan = kalan - parcaBoy;

                    sonuc = tabButceFisi.KaydetGerekce(tertip, ++satir, parca);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }
            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            return sonuc;
        }

        /// <summary>
        /// Verilen yıl ve tertip için istenilen gelir için girilmiş olan gerekçe döndürülür.
        /// Yetki kontrolü yapılır. Veri tabanı bağlantısı açılır. Kayıtlı gerekçe okunur parçalar birleştirilir ve döndürülür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tertip">Bütçe fişinin tertibi</param>
        /// <returns>Bütçe tertibi için istenen ödeneğin gerekçesi döndürülür</returns>
        public static string ListeleGerekce(Kullanici kullanan, Tertip tertip)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(tertip.yil), kullanan.KimlikBul());
            TABBHMBUTCEGELIRFISI tabButceFisi = new TABBHMBUTCEGELIRFISI(ve);

            int satir;
            string yazi = string.Empty;

            IDataReader r = tabButceFisi.ListeleGerekce(tertip);
            if (tabButceFisi.Hata() == string.Empty)
            {
                while (r.Read())
                {
                    satir = r.GetInt32(0);
                    yazi += r.GetString(1);
                }
            }
            r.Close();
            ve.VeriErisimKapat();
            return yazi;
        }
    }
}
