
using TNS.Veri;
using TNS.KYM;
using TNS.EVR.IsKurallari.Veri;
using EVRServisI;
using System.Data;
using System;
namespace TNS.EVR.IsKurallari
{
    /// <summary>
    /// Faks Tarihce bilgileriyle ilgili, tanımlama, silme, listeleme yordamlarını tutan sınıf.
    /// </summary>
    public class EVRFaksTarihce
    {
        /// <summary>
        /// Parametre olarak verilen evrakfaks nesnesine göre, tarihce bilgilerini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">tarihce bilgilerini içeren nesne </param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABEvrakFaksTarihce to = new TABEvrakFaksTarihce(ve);

                sonuc = to.Kaydet(liste);

                if (sonuc.islemSonuc)
                {
                    ve.Commit();

                    sonuc.bilgiStr += EvrakI.EVRI011;
                }
                else
                    ve.Rollback();

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{DE75EB31-EC1D-4A38-8723-1BDF38B2D50A} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen faks birimlerini veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">faks bilgilerini içeren nesne koleksiyonu</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, string kodu)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            if (sonuc.islemSonuc)
            {
                ve.BeginTransaction();
                try
                {
                    TABTasfiye to = new TABTasfiye(ve);

                    if (sonuc.hataStr == "")
                        sonuc = to.Sil(kodu);
                    else
                        sonuc.islemSonuc = false;

                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.bilgiStr = EvrakI.EVRI175;
                    }
                    else
                        ve.Rollback();

                }
                catch (Exception ex)
                {
                    ve.Rollback();
                    OrtakFonksiyonlar.HataStrYaz("{5AEF4F09-9CE2-4FEC-B4E5-0088CF589979} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
                }

            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, EvrakFaksTarihce bilgi)
        {
            ObjectArray hVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            TABEvrakFaksTarihce to = new TABEvrakFaksTarihce(ve);
            IDataReader r = to.Listele(bilgi);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    bilgi = new EvrakFaksTarihce();
                    bilgi.tarihceKodu = VeriErisim.DegerAl(r, 0, "");
                    bilgi.kodu = VeriErisim.DegerAl(r, 1, "");
                    bilgi.kayitEden = VeriErisim.DegerAl(r, 2, "");
                    bilgi.kayitZamani = VeriErisim.DegerAl(r, 3);
                    bilgi.kayitAciklama = VeriErisim.DegerAl(r, 4, "");
                    hVAN.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return hVAN;
        }

        public static Sonuc KontrolEt(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste, string yetkiTipi)
        {
            string hata = "";
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            if (liste != null && yetkiTipi == "Yazma")
            {
                foreach (EvrakTasfiye bilgi in liste.objeler)
                {
                    if (bilgi.prEvrakNo == "")
                    {
                        hata = EvrakI.EVRI171 + "<br>";
                        break;
                    }
                    if (bilgi.tasfiyeKod == 0)
                    {
                        hata = EvrakI.EVRI170 + "<br>";
                        break;
                    }
                }
            }

            if (hata == "")
                sonuc.islemSonuc = true;

            sonuc.hataStr = hata;

            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}
