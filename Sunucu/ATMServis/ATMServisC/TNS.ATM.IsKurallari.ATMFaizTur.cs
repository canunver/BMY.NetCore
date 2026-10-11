using System;
using System.Data;
using TNS.ATM.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.ATM.IsKurallari
{
    /// <summary>
    /// Alacak Takip Faiz Tür Sınıfı
    /// </summary>
    public class FaizTurClass
    {
        /// <summary>
        /// Faiz tür bilgilerini kaydeden fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="faizTur">Faiz Tür bilgisi, FaizTur tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        public Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, FaizTur faizTur)
        {
            Sonuc donusDeger = Yetki.FaizTurYazmaHakkiVarMi(kullanan);
            if (!donusDeger.islemSonuc)
                return donusDeger;

            if (faizTur.yil != Genel.AktifYilDondur())
            {
                donusDeger.hataStr = AlacakTakipResI.ATA032;
                donusDeger.islemSonuc = false;
                return donusDeger;
            }

            if (faizTur.ad == "")
            {
                donusDeger.hataStr = AlacakTakipResI.ATA033;
                donusDeger.islemSonuc = false;
                return donusDeger;
            }

            //if (faizTur.oran <= 0)
            //{
            //    donusDeger.hataStr = AlacakTakipResI.ATA034;
            //    donusDeger.islemSonuc = false;
            //    return donusDeger;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMFaizTur tabFaizTur = new TABATMFaizTur(ve);
            ve.BeginTransaction();

            try
            {
                if (faizTur.kod > 0)
                    donusDeger = tabFaizTur.Guncelle(faizTur);
                else
                    donusDeger = tabFaizTur.Kaydet(faizTur);
            }
            catch (Exception e)
            {
                donusDeger.DegerAta(e.Message);
            }

            if (donusDeger.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Yıl ve ada göre Faiz tür bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="yil">Yıl bilgisi, Int tipinde sınıf</param>
        /// <param name="ad">Ad bilgisi, String tipinde sınıf</param>
        /// <returns>ObjectArray  tipinde sınıf döndürür</returns>
        public ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, int yil, string ad)
        {
            ObjectArray donusDeger = new ObjectArray();
            donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan);
            if (!donusDeger.sonuc.islemSonuc)
                return donusDeger;

            if (yil < 0)
            {
                donusDeger.sonuc.hataStr = AlacakTakipResI.ATA035;
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMFaizTur tabFaizTur = new TABATMFaizTur(ve);
            IDataReader dr = null;

            try
            {
                dr = tabFaizTur.Listele(yil, ad);
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
                FaizTur gelen = new FaizTur();
                gelen.yil = VeriErisim.DegerAl(dr, 0, -1);
                gelen.kod = VeriErisim.DegerAl(dr, 1, -1);
                gelen.ad = VeriErisim.DegerAl(dr, 2, "");
                gelen.oran = VeriErisim.DegerAl(dr, 3, -1.0);

                donusDeger.objeler.Add(gelen);
            }

            if (donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = AlacakTakipResI.ATA036;

            donusDeger.sonuc.islemSonuc = true;

            dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }
    }
}
