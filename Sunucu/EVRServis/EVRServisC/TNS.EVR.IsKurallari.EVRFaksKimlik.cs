
using TNS.Veri;
using TNS.KYM;
using TNS.EVR.IsKurallari.Veri;
using EVRServisI;
using System.Data;
using System;
namespace TNS.EVR.IsKurallari
{
    /// <summary>
    /// Faks kimlik bilgileriyle ilgili, tanımlama, silme, listeleme yordamlarını tutan sınıf.
    /// </summary>
    public class EVRFaksKimlik
    {
        /// <summary>
        /// Parametre olarak verilen evrakfakskimlik nesnesine göre, kimlik bilgilerini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">kimlik bilgilerini içeren nesne </param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, EvrakFaksKimlik form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABEvrakFaksKimlik to = new TABEvrakFaksKimlik(ve);

                sonuc = to.Kaydet(form);
                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{01088B5A-6674-44FB-B6B7-A456E2944B77} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static string KimlikGetir(VeriErisim veriErisim, string birimKod)
        {
            string kimlik = "";
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABEvrakFaksKimlik to = new TABEvrakFaksKimlik(ve);

                kimlik = to.Getir(birimKod);

                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{02841878-6CAB-41E6-90B8-BE8AA8DBBC32} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();

            return kimlik;
        }

        public static string BirimGetir(VeriErisim veriErisim, string kimlik)
        {
            string birim = "";
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABEvrakFaksKimlik to = new TABEvrakFaksKimlik(ve);

                birim = to.BirimBul(kimlik);

                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{7183FB36-1BA2-4D41-B00A-D8F81EC0900F} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();

            return birim;
        }

        public static ObjectArray FaksDosyaKoduGetir(VeriErisim veriErisim, string birim)
        {
            EvrakFaks evrFaks;
            ObjectArray hVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakFaksKimlik to = new TABEvrakFaksKimlik(ve);
            IDataReader r = to.DosyaKoduGetir(birim);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    evrFaks = new EvrakFaks();
                    evrFaks.kodu = VeriErisim.DegerAl(r, 0, "");
                    evrFaks.birimKod = VeriErisim.DegerAl(r, 1, "");
                    evrFaks.kimden = VeriErisim.DegerAl(r, 2, "");
                    evrFaks.kime = VeriErisim.DegerAl(r, 3, "");
                    evrFaks.aciklama = VeriErisim.DegerAl(r, 4, "");
                    evrFaks.faksNo = VeriErisim.DegerAl(r, 5, "");
                    evrFaks.oncelik = VeriErisim.DegerAl(r, 6, 0);
                    evrFaks.kayitEden = VeriErisim.DegerAl(r, 7, "");
                    evrFaks.kayitZamani = VeriErisim.DegerAl(r, 8);
                    evrFaks.teyit = VeriErisim.DegerAl(r, 9, 0);
                    evrFaks.gonderilmeZamani = VeriErisim.DegerAl(r, 10);
                    evrFaks.durumu = VeriErisim.DegerAl(r, 11, 0);
                    evrFaks.tekrarSayisi = VeriErisim.DegerAl(r, 12, 0);
                    evrFaks.kapakSayfasi = VeriErisim.DegerAl(r, 13, 0);
                    evrFaks.sayfaSayisi = VeriErisim.DegerAl(r, 14, 0);
                    evrFaks.konu = VeriErisim.DegerAl(r, 15, "");
                    hVAN.objeler.Add(evrFaks);
                }
            }

            r.Close();
            ve.VeriErisimKapat();
            return hVAN;
        }
    }
}
