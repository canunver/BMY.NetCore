using System.Data;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// Belgelerin kayıt edilmesi sırasında oluşturulan tarihçe bilgi işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHTarihceServis
    {
        /// <summary>
        /// Tarihce listele
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="belgeTur">Belge tür</param>
        /// <param name="belgeNo">Belge numarası</param>
        /// <returns></returns>
        public ObjectArray Listele(Kullanici kullanan, int yil, ENUMBelgeTuru belgeTur, string belgeNo)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTarihce to = new TABTarihce(ve);
            IDataReader r = to.Listele(yil, belgeTur, belgeNo);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TarihceBilgisi tarih = new TarihceBilgisi();

                    tarih.yil = VeriErisim.DegerAl(r, 0, 0);
                    tarih.belgeTur = (ENUMBelgeTuru)VeriErisim.DegerAl(r, 1, 0);
                    tarih.belgeNo = VeriErisim.DegerAl(r, 2, "");
                    tarih.islemYapan = VeriErisim.DegerAl(r, 3, "");
                    tarih.islemTarih = VeriErisim.DegerAl(r, 4);
                    tarih.islem = VeriErisim.DegerAl(r, 5, "");
                    tarih.islemYapanAd = VeriErisim.DegerAl(r, 6, "");

                    if (string.IsNullOrEmpty(tarih.islemYapanAd))
                        tarih.islemYapanAd = tarih.islemYapan;

                    iVAN.objeler.Add(tarih);
                }
                r.Close();
            }

            ve.VeriErisimKapat();
            return iVAN;
        }
    }
}
