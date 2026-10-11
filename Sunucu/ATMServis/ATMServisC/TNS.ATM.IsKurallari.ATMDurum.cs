using System;
using System.Data;
using TNS.ATM.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.ATM.IsKurallari
{
    /// <summary>
    /// Alacak Takip Durum Sınıfı
    /// </summary>
    public class DurumClass
    {
        /// <summary>
        /// Faiz türüne göre Durum bilgisi kaydeden fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="faizTur">Faiz Tür bilgisi, FaizTur tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        public Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, FaizTur faizTur)
        {
            Sonuc donusDeger = Yetki.YazmaHakkiVarMi(kullanan);
            if (!donusDeger.islemSonuc)
                return donusDeger;

            if (faizTur.ad == "")
            {
                donusDeger.hataStr = AlacakTakipResI.ATA029;
                donusDeger.islemSonuc = false;
                return donusDeger;
            }

            if (faizTur.oran <= 0)
            {
                donusDeger.hataStr = AlacakTakipResI.ATA030;
                donusDeger.islemSonuc = false;
                return donusDeger;
            }

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
        /// HepsiMi alanına göre Durum bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="hepsiMi">Hepsi Mi bilgisi, Bool tipinde sınıf</param>
        /// <returns>ObjectArray tipinde sınıf döndürür</returns>
        public ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, bool hepsiMi)
        {
            ObjectArray donusDeger = new ObjectArray();
            donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan);
            if (!donusDeger.sonuc.islemSonuc)
                return donusDeger;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMDurum tabDurum = new TABATMDurum(ve);
            IDataReader dr = tabDurum.Listele(hepsiMi);

            if(dr==null)
            {
                donusDeger.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return donusDeger;
            }

            while (dr.Read())
            {
                Durum gelen = new Durum();
                gelen.kod = VeriErisim.DegerAl(dr, 0, -1);
                gelen.ad = VeriErisim.DegerAl(dr, 1, "");

                donusDeger.objeler.Add(gelen);
            }

            if (donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = AlacakTakipResI.ATA031;

            donusDeger.sonuc.islemSonuc = true;

            dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }
    }
}
