using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System.Collections;
using System;

namespace TNS.EVR.IsKurallari
{
    /// <summary>
    /// Evrak dağıtım bilgileriyle ilgili, kaydetme ve listeleme yordamlarını tutan sınıf. 
    /// </summary>
    public class EVREvrakDagitim
    {
        /// <summary>
        /// Parametre olarak verilen evrak bilgisi ve EvrakDagitim nesne koleksiyonunu ilişkilendirilerek veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="prEvrakNo">Evrak prEvrakNo bilgisi içeren nesne</param>
        /// <param name="liste">EvrakDagitim nesne koleksiyonunu içreren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, string prEvrakNo, ArrayList liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            if (!string.IsNullOrEmpty(prEvrakNo))
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

                ve.BeginTransaction();
                try
                {
                    TABEvrakDagitim to = new TABEvrakDagitim(ve);

                    sonuc = to.Sil(prEvrakNo);

                    if (sonuc.islemSonuc)
                    {
                        if (liste.Count > 0)
                            sonuc = to.Kaydet(prEvrakNo, liste);

                        if (sonuc.islemSonuc)
                        {
                            ve.Commit();
                            sonuc.bilgiStr += EvrakI.EVRI081;
                        }
                        else
                            ve.Rollback();
                    }
                    else
                        ve.Rollback();
                }
                catch (Exception ex)
                {
                    ve.Rollback();
                    OrtakFonksiyonlar.HataStrYaz("{4C8A855C-B9AD-4566-8BBC-72B8BC619DBC} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
                }
                ve.VeriErisimKapat();
            }
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen prEvrakNo bilgisine göre, evrak'ın dağıtım bilgisini EvrakDagitim nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="prEvrakNo">prEvrakNo bilgisini içeren nesne</param>
        /// <returns>EvrakDagitim nesne koleksiyonunu olarak döndürülür.</returns>
        public static ArrayList Listele(VeriErisim veriErisim, string prEvrakNo)
        {
            ArrayList hVAN = new ArrayList();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            TABEvrakDagitim to = new TABEvrakDagitim(ve);
            IDataReader r = to.Listele(prEvrakNo);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    EvrakDagitim bilgi = new EvrakDagitim();
                    bilgi.geregiBilgi = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.dagitimKod = VeriErisim.DegerAl(r, 1, "");
                    bilgi.dagitimAd = VeriErisim.DegerAl(r, 2, "");

                    hVAN.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return hVAN;
        }
    }
}
