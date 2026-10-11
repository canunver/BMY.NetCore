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
    /// Evrak şablon dağıtım bilgileriyle ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf. 
    /// </summary>
    public class EVRSablonDagitim
    {
        /// <summary>
        /// Parametre olarak verilen SablonDagitim nesne koleksiyonunu sablon kodu ile ilişkili olarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="sablonKod">Şablon kod bilgisini içeren nense</param>
        /// <param name="liste">SablonDagitim nesne koleksiyonunu içreren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int sablonKod, ArrayList liste)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI150, "");
            if (sablonKod > 0 && liste.Count > 0)
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

                ve.BeginTransaction();
                try
                {
                    TABSablonDagitim to = new TABSablonDagitim(ve);

                    sonuc = to.Sil(sablonKod);

                    if (sonuc.islemSonuc)
                    {
                        sonuc = to.Kaydet(sablonKod, liste);

                        if (sonuc.islemSonuc)
                        {
                            ve.Commit();
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
                    OrtakFonksiyonlar.HataStrYaz("{7E8F703B-9182-494F-9EBB-EFE84BD6BE04} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
                }
                ve.VeriErisimKapat();
            }
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen sablonKod bilgisine göre, dağıtım şablonunu sablonDagitim nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="sablonKod">Şablon kod bilgisini içeren nense</param>
        /// <returns>SablonDagitim nesne koleksiyonunu olarak döndürülür.</returns>
        public static ArrayList Listele(VeriErisim veriErisim, Kullanici kullanan, int sablonKod)
        {
            ArrayList hVAN = new ArrayList();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            TABSablonDagitim to = new TABSablonDagitim(ve);
            IDataReader r = to.Listele(sablonKod);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    SablonDagitim bilgi = new SablonDagitim();
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

        /// <summary>
        /// Parametre olarak verilen sablonKod bilgisine göre, dağıtım şablonunu veritabanından silen yordam.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="sablonKod">Şablon kod bilgisini içeren nense</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, int sablonKod)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI151, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABSablonDagitim to = new TABSablonDagitim(ve);

                sonuc = to.Sil(sablonKod);

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = EvrakI.EVRI152;
                }
                else
                    ve.Rollback();

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{22E1497C-68B2-4AE9-B7FD-8997DFC68B34} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

    }
}
