using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System;

namespace TNS.EVR.IsKurallari
{
    /// <summary>
    /// Posta tür bilgileriyle ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf. 
    /// </summary>
    public class EVRPostaIslemTur
    {
        /// <summary>        
        /// Parametre olarak verilen Tanimlama nesne koleksiyonunu, posta tür bilgisi olarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="liste">Tanimlama nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI142, "");

            if (sonuc.islemSonuc)
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

                ve.BeginTransaction();
                try
                {
                    TABPostaIslemTur to = new TABPostaIslemTur(ve);

                    sonuc = to.Kaydet(liste);

                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();

                        sonuc.bilgiStr += EvrakI.EVRI143;
                    }
                    else
                        ve.Rollback();
                }
                catch (Exception ex)
                {
                    ve.Rollback();
                    OrtakFonksiyonlar.HataStrYaz("{CC9D0617-DD28-4DB8-A2AB-AE41CC340797} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
                }

                ve.VeriErisimKapat();
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, posta tür bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="liste">Tanimlama nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI144, "");

            if (sonuc.islemSonuc)
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

                ve.BeginTransaction();
                try
                {
                    TABPostaIslemTur to = new TABPostaIslemTur(ve);
                    sonuc = to.Sil(liste);

                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.bilgiStr = EvrakI.EVRI145;
                    }
                    else
                        ve.Rollback();

                }
                catch (Exception ex)
                {
                    ve.Rollback();
                    OrtakFonksiyonlar.HataStrYaz("{DD7F45B4-CA29-4E17-9CEE-C5D10AC33C06} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
                }
                ve.VeriErisimKapat();
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Tanimlama bilgisine göre, posta tür listesini Tanimlama nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="bilgi">Tanimlama bilgisini içeren nesne</param></param>
        /// <returns>Tanimlama nesne koleksiyonunu olarak döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Tanimlama bilgi)
        {
            ObjectArray hVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            TABPostaIslemTur to = new TABPostaIslemTur(ve);
            IDataReader r = to.Listele(bilgi);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    bilgi = new Tanimlama();

                    bilgi.kod = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.ad = VeriErisim.DegerAl(r, 1, "");

                    hVAN.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return hVAN;
        }
    }
}
