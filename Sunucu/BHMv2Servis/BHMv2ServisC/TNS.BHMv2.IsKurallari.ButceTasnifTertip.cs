using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using TNS.BKD;

namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// Bütçe tasnif satırlarına girilen tertip şablonları ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class BHMv2ButceTasnifTertip
    {
        /// <summary>
        /// Tasnif satırına ait tertip sablonlarını verilen tasnif kodu, satır kodu ve yıl parametrelerini kullanarak veri tabanından okuyan yordam.
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tasnifNo">Okunacak şablonun ait olduğu tasnifi gösteren, tasnif sıra numarası</param>
        /// <param name="hiyerarsikSatirKodu">Okunacak şablonun ait olduğu satırın kodu</param>
        /// <param name="yil">Aynı satırın tasnifleri yıldan yıla değişebildiği için, okunacak şablonun yılı</param>
        /// <returns>Okunan tertip şablonlarının dizisi döndürülür.</returns>
        public static ButceTasnifTertipSablon[] Oku(VeriErisim veriErisim, Kullanici kullanan, int tasnifNo, string hiyerarsikSatirKodu, int yil)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBulSozluk(), kullanan.KimlikBul());
            TABBHMBUTCETASNIFTERTIPSABLON tabBHMBUTCETASNIFTERTIP = new TABBHMBUTCETASNIFTERTIPSABLON(ve);

            IDataReader dr = tabBHMBUTCETASNIFTERTIP.Listele(tasnifNo, hiyerarsikSatirKodu);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            ArrayList al = new ArrayList();
            while (dr.Read())
            {
                ButceTasnifTertipSablon detay = new ButceTasnifTertipSablon(VeriErisim.DegerAl(dr, (int)TABBHMBUTCETASNIFTERTIPSABLON.ENUMAlanlar.TERTIPSABLON, string.Empty), (TertipEkleCikar)VeriErisim.DegerAl(dr, (int)TABBHMBUTCETASNIFTERTIPSABLON.ENUMAlanlar.EKLECIKAR, 1));
                al.Add(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return (ButceTasnifTertipSablon[])al.ToArray(typeof(ButceTasnifTertipSablon));
        }
    }
}
