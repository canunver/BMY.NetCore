using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;
using TNS.BKD;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// Birim bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class PEBKurSinif
    {
        /// <summary>
        /// Verilen kriterlere uygun olan birim bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Kurum kodu</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında birim listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurKod)
        {
            ObjectArray kurVAN = new ObjectArray();
            //kurVAN.sonuc = Yetki.OkumaHakkiVarMi(kullanan, kurKod);
            //if (!kurVAN.sonuc.islemSonuc)
            //    return kurVAN;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBKD", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKur tKur = new TABKur(ve);
            IDataReader dr = tKur.Listele(yil, kurKod);
            while (dr.Read())
            {
                ABSKod gelen = new ABSKod();
                //gelen.yil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.kod = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.kod = PEBGenel.KurKodNoktala(gelen.kod);
                gelen.AdAta(VeriErisim.DegerAl(dr, 2, string.Empty));
                //gelen.kurAd2 = VeriErisim.DegerAl(dr, 3, string.Empty);

                kurVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (kurVAN.objeler.Count == 0)
                kurVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return kurVAN;
        }
    }
}