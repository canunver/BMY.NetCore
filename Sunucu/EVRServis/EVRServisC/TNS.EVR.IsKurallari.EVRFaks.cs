
using TNS.Veri;
using TNS.KYM;
using TNS.EVR.IsKurallari.Veri;
using EVRServisI;
using System.Data;
using System;
namespace TNS.EVR.IsKurallari
{
    /// <summary>
    /// Faks bilgileriyle ilgili, tanımlama, silme, listeleme yordamlarını tutan sınıf.
    /// </summary>
    public class EVRFaks
    {
        /// <summary>
        /// Parametre olarak verilen EvrakFaks nesnesine göre, Faks bilgilerini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Faks bilgilerini içeren nesne </param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABEvrakFaks to = new TABEvrakFaks(ve);

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
                OrtakFonksiyonlar.HataStrYaz("{CEBEA47C-013C-4A57-931E-BBCA63F9A578} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen EvrakFaks nesnesine göre, Faks bilgilerini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Faks bilgilerini içeren nesne </param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Guncelle(VeriErisim veriErisim, ObjectArray liste, string islem)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABEvrakFaks to = new TABEvrakFaks(ve);

                if (islem == "durum")
                    sonuc = to.GuncelleDurum(liste, islem);
                else if (islem == "oncelik")
                    sonuc = to.GuncelleOncelik(liste, islem);
                else if (islem == "webServis")
                    sonuc = to.GuncelleDurumWebServis(liste, islem);

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
                OrtakFonksiyonlar.HataStrYaz("{5ED30C7D-5471-40BA-94FE-D41907294DA5} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Faks birimlerini veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Faks bilgilerini içeren nesne koleksiyonu</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        /// 

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
                    OrtakFonksiyonlar.HataStrYaz("{46FBCB76-071D-4100-B01A-30ABED0A67E5} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
                }

            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, EvrakFaks bilgi)
        {
            ObjectArray hVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            TABEvrakFaks to = new TABEvrakFaks(ve);
            IDataReader r = to.Listele(bilgi);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    bilgi = new EvrakFaks();
                    bilgi.kodu = VeriErisim.DegerAl(r, 0, "");
                    bilgi.birimKod = VeriErisim.DegerAl(r, 1, "");
                    bilgi.kimden = VeriErisim.DegerAl(r, 2, "");
                    bilgi.kime = VeriErisim.DegerAl(r, 3, "");
                    bilgi.aciklama = VeriErisim.DegerAl(r, 4, "");
                    bilgi.faksNo = VeriErisim.DegerAl(r, 5, "");
                    bilgi.oncelik = VeriErisim.DegerAl(r, 6, 0);
                    bilgi.kayitEden = VeriErisim.DegerAl(r, 7, "");
                    bilgi.kayitZamani = VeriErisim.DegerAl(r, 8);
                    bilgi.teyit = VeriErisim.DegerAl(r, 9, 0);
                    bilgi.gonderilmeZamani = VeriErisim.DegerAl(r, 10);
                    bilgi.durumu = VeriErisim.DegerAl(r, 11, 0);
                    bilgi.tekrarSayisi = VeriErisim.DegerAl(r, 12, 0);
                    bilgi.kapakSayfasi = VeriErisim.DegerAl(r, 13, 0);
                    bilgi.sayfaSayisi = VeriErisim.DegerAl(r, 14, 0);
                    bilgi.konu = VeriErisim.DegerAl(r, 15, "");
                    hVAN.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return hVAN;
        }
    }
}
