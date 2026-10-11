using System;
using System.Data;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;

namespace TNS.EVR.IsKurallari
{
    /// <summary>
    /// Evrak tarihçe bilgileriyle ilgili, kaydetme ve silme yordamlarını tutan sınıf. 
    /// </summary>
    public class EVREvrakTarihce
    {
        /// <summary>
        /// Parametre olark verilen evrak tarihçe bilgisine göre, evrak tarihçesini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgilerini içeren nesne</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Evrak'ın tarihçe bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, EvrakTarihce form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI086, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            try
            {
                ve.BeginTransaction();
                TABEvrakTarihce to = new TABEvrakTarihce(ve);

                form.islemTarih = new TNSDateTime(DateTime.Now);
                form.islemYapan = kullanan.mernis;
                form.islemYapanAd = kullanan.ad + " " + kullanan.soyad;

                sonuc = to.Kaydet(form);
            }
            catch (Exception ex)
            {
                OrtakFonksiyonlar.HataStrYaz("{7B9DE82A-2889-4FA6-B71C-B8C62BB380CC} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
                //OrtakFonksiyonlar.HataStrYaz(ex.Message + "*" + ex.StackTrace);
                sonuc.islemSonuc = false;
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Paramete olarak verilen evrEvrakNo nesnesine göre, evrak'ın tarihçesini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="evrEvrakNo">evrEvrakNo bilgisini içeren nesne</param>
        /// <returns>EvrakTarihce nesne koleksiyonunu olarak döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, string evrEvrakNo)
        {
            ObjectArray hVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            TABEvrakTarihce to = new TABEvrakTarihce(ve);
            IDataReader dr = to.Listele(evrEvrakNo);

            if (to.Hata() == "")
            {
                while (dr.Read())
                {
                    EvrakTarihce bilgi = new EvrakTarihce();
                    bilgi.evrEvrakNo = VeriErisim.DegerAl(dr, (int)TABEvrakTarihce.ENUMAlanlar.EVREVRAKNO, "");
                    bilgi.islemTarih = VeriErisim.DegerAl(dr, (int)TABEvrakTarihce.ENUMAlanlar.ISLEMTARIH);
                    bilgi.islemYapan = VeriErisim.DegerAl(dr, (int)TABEvrakTarihce.ENUMAlanlar.ISLEMYAPAN, "");
                    bilgi.islem = VeriErisim.DegerAl(dr, (int)TABEvrakTarihce.ENUMAlanlar.ISLEM, "");
                    bilgi.islemYapanAd = VeriErisim.DegerAl(dr, (int)TABEvrakTarihce.ENUMAlanlar.ISLEMYAPANAD, "");
                    bilgi.vekil = VeriErisim.DegerAl(dr, (int)TABEvrakTarihce.ENUMAlanlar.VEKIL, "");
                    bilgi.vekilAd = VeriErisim.DegerAl(dr, (int)TABEvrakTarihce.ENUMAlanlar.VEKILAD, "");
                    hVAN.objeler.Add(bilgi);
                }
            }
            dr.Close();

            ve.VeriErisimKapat();

            return hVAN;
        }
    }
}
