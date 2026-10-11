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
    /// Dosya planı birim bilgileriyle ilgili, kaydetme ve listeleme yordamlarını tutan sınıf. 
    /// </summary>
    public class EVRDosyaPlaniBirim
    {
        /// <summary>
        /// Parametre olarak verilen DosyaPlaniBirim nesne koleksiyonunu veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">DosyaPlaniBirim nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = null;
            VeriErisim ve = null;

            try
            {
                string birim = ((DosyaPlaniBirim)liste[0]).evrakBirimKodu;
                string dosyaKodu = ((DosyaPlaniBirim)liste[0]).dosyaPlani.kod;

                if (!TNS.EVR.Yetki.KullaniciBirimeYetkiliMi(kullanan, birim, ENUMOkumaYazma.YAZMA))
                    return new Sonuc(false, string.Format(EvrakI.EVRI038, birim), "");

                ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
                TABBirimDosyaPlani to = new TABBirimDosyaPlani(ve);

                ve.BeginTransaction();

                sonuc = to.Sil(birim, "");
                if (sonuc.islemSonuc && dosyaKodu != "")
                    sonuc = to.Kaydet(liste);

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr += EvrakI.EVRI039;
                }
                else
                    ve.Rollback();

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{800EB145-84B1-4DC6-8600-5EC589178F07} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            if (ve != null)
                ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen DosyaPlaniBirim nesnesine göre, DosyaPlaniBirim nesne koleksiyonunu veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">DosyaPlaniBirim bilgisini içeren nesne</param>
        /// <returns>DosyaPlaniBirim nesne koleksiyonunu olarak döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, DosyaPlaniBirim bilgi)
        {
            ObjectArray hVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            TABBirimDosyaPlani to = new TABBirimDosyaPlani(ve);
            IDataReader r = to.Listele(bilgi);

            if (r == null)
            {
                ve.VeriErisimKapat();
                hVAN.sonuc.hataStr = EvrakI.EVRI040;
                hVAN.sonuc.islemSonuc = false;
                return hVAN;
            }

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    bilgi = new DosyaPlaniBirim();

                    bilgi.evrakBirimKodu = VeriErisim.DegerAl(r, 0, "");
                    bilgi.dosyaPlani.kod = VeriErisim.DegerAl(r, 1, "");

                    bilgi.dosyaPlani.ad = VeriErisim.DegerAl(r, 2, "");
                    bilgi.dosyaPlani.seviye = VeriErisim.DegerAl(r, 3, 0);
                    bilgi.dosyaPlani.detay = VeriErisim.DegerAl(r, 4, 0) == 0;
                    //bilgi.dosyaPlani.bolumTanim.bolumAdi = VeriErisim.DegerAl(r, 5, "");
                    hVAN.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return hVAN;
        }
    }

    /// <summary>
    /// Dosya planı favori bilgileriyle ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf. 
    /// </summary>
    public class EVRDosyaPlaniFavori
    {
        /// <summary>
        /// Parametre olarak verilen DosyaPlani nesne koleksiyonunu veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">DosyaPlani nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABBirimDosyaPlani to = new TABBirimDosyaPlani(ve);

                sonuc = to.SilFavori(kullanan.mernis, liste);
                if (sonuc.islemSonuc)
                    sonuc = to.KaydetFavori(kullanan.mernis, liste);
                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{5A33C41D-7E8B-45B7-8507-8F19F06EB312} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Dosya planı favori bilgilerini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="ad">Dosya plan kodunun ad bilgisini içeren nesne</param>
        /// <param name="kod">Dosya plan kodunun kod bilgisini içeren nesne</param>
        /// <returns>DosyaPlani nesne koleksiyonunu olarak döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, string ad, string kod)
        {
            ObjectArray hVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            TABBirimDosyaPlani to = new TABBirimDosyaPlani(ve);
            IDataReader r = to.ListeleFavori(kullanan.mernis, ad, kod);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    DosyaPlani bilgi = new DosyaPlani();

                    bilgi.kod = VeriErisim.DegerAl(r, 1, "");
                    bilgi.ad = VeriErisim.DegerAl(r, 2, "");

                    hVAN.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return hVAN;
        }

        /// <summary>
        /// Parametre olarak verilen DosyaPlani nesne koleksiyonunu içeren dosya planı favori bilgilerini veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">DosyaPlani nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>        
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABBirimDosyaPlani to = new TABBirimDosyaPlani(ve);
                sonuc = to.SilFavori(kullanan.mernis, liste);
                ve.Commit();
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{97164732-3B21-4781-B45E-8E15D91E925C} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }
    }
}
