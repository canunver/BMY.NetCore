using System;
using System.Data;
using TNS.ATM.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.ATM.IsKurallari
{
    /// <summary>
    /// hesap Hareket cocuk Sınıfı
    /// </summary>
    public class DosyaHesapHareketCocukClass
    {
        /// <summary>
        /// Guncelles the specified veri erisim.
        /// </summary>
        /// <param name="veriErisim">Veri Erişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="hesapHareketCocuk">The hesap hareket cocuk.</param>
        /// <returns>
        /// Sonuc tipinde sınıf döndürür
        /// </returns>
        public static Sonuc Guncelle(VeriErisim veriErisim, Kullanici kullanan, DosyaHesapHareketCocuk hesapHareketCocuk)
        {
            Sonuc donusDeger = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMHesapHareketCocuk tabHesapHareketCocuk = new TABATMHesapHareketCocuk(ve);
            ve.BeginTransaction();

            try
            {
                donusDeger = tabHesapHareketCocuk.Guncelle(hesapHareketCocuk);
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
        /// DosyahesapHareketCocuk bilgilerini kaydeden fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="listeHesapHareketCocuk">The liste hesap hareket cocuk.</param>
        /// <returns>
        /// Sonuc tipinde sınıf döndürür
        /// </returns>
        public Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ObjectArray listeHesapHareketCocuk)
        {
            Sonuc donusDeger = new Sonuc();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMHesapHareketCocuk tabHesapHareketCocuk = new TABATMHesapHareketCocuk(ve);
            ve.BeginTransaction();

            DosyaHesapHareketCocuk c = (DosyaHesapHareketCocuk)listeHesapHareketCocuk.objeler[0];
            DosyaHesapHareketCocuk newC = new DosyaHesapHareketCocuk();
            newC.anaKod = c.anaKod;
            donusDeger = tabHesapHareketCocuk.Sil(newC);
            if (donusDeger.islemSonuc)
            {
                foreach (DosyaHesapHareketCocuk hesapHareketCocuk in listeHesapHareketCocuk.objeler)
                {
                    donusDeger = tabHesapHareketCocuk.Kaydet(hesapHareketCocuk);
                    if (donusDeger.islemSonuc)
                        ve.Commit();
                    else
                        ve.Rollback();
                }
            }

            ve.VeriErisimKapat();

            return donusDeger;
        }

        /// <summary>
        /// DosyahesapHareketCocuk  bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="hesapHareketCocuk">The hesap hareket cocuk.</param>
        /// <returns>
        /// ObjectArray tipinde sınıf döndürür
        /// </returns>
        public ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, DosyaHesapHareketCocuk hesapHareketCocuk)
        {
            ObjectArray donusDeger = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMHesapHareketCocuk tabHesapHareketCocuk = new TABATMHesapHareketCocuk(ve);
            IDataReader dr = null;

            try
            {
                dr = tabHesapHareketCocuk.Listele(hesapHareketCocuk);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                ve.VeriErisimKapat();
                return donusDeger;
            }


            while (dr.Read())
                donusDeger.objeler.Add(DosyaAl(dr));

            if (donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = AlacakTakipResI.ATA018;

            donusDeger.sonuc.islemSonuc = true;

            dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// DosyahesapHareketCocuk bilgilerini DataReader sınıfından alan fonksiyon
        /// </summary>
        /// <param name="dr">Data Reader, IDataReader tipinde sınıf</param>
        /// <returns>DosyahesapHareketCocuk tipinde sınıf döndürür</returns>
        private DosyaHesapHareketCocuk DosyaAl(IDataReader dr)
        {
            DosyaHesapHareketCocuk cocuk = new DosyaHesapHareketCocuk();
            cocuk.kod = VeriErisim.DegerAl(dr, 0, 0);
            cocuk.anaKod = VeriErisim.DegerAl(dr, 1, 0);
            cocuk.siraNo = VeriErisim.DegerAl(dr, 2, 0);
            cocuk.hesapKodu = VeriErisim.DegerAl(dr, 3, "");
            cocuk.adi = VeriErisim.DegerAl(dr, 4, "");
            cocuk.borc = VeriErisim.DegerAl(dr, 5, 0.0);
            cocuk.alacak = VeriErisim.DegerAl(dr, 6, 0.0);
            cocuk.islemYapanAd = VeriErisim.DegerAl(dr, 7, "");
            cocuk.islemYapanKod = VeriErisim.DegerAl(dr, 8, "");
            cocuk.islemTarihi = VeriErisim.DegerAl(dr, 9);
            cocuk.uretilenSatir = VeriErisim.DegerAl(dr, 10, 0);
            cocuk.altHesapKod = VeriErisim.DegerAl(dr, 11, "");
            cocuk.dosyaYil = VeriErisim.DegerAl(dr, 12, 0);
            cocuk.dosyaMuhasebe = VeriErisim.DegerAl(dr, 13, 0);
            cocuk.dosyaBirim = VeriErisim.DegerAl(dr, 14, "");
            cocuk.dosyaNo = VeriErisim.DegerAl(dr, 15, 0);
            cocuk.dosyaBorcluAd = VeriErisim.DegerAl(dr, 16, "");
            cocuk.tasarimKod = VeriErisim.DegerAl(dr, 17, 0);
            return cocuk;
        }

        /// <summary>
        /// Arguman olarak verilen, taşınır form bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Silinecek bilgiyi tutan nesne</param>
        /// <returns>
        /// İşlem sonucunu döndüren, Sonuc sınıfında nesne
        /// </returns>
        public Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, DosyaHesapHareketCocuk kriter)
        {
            Sonuc donusDeger = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(null));
            TABATMHesapHareketCocuk tabHesapHareketCocuk = new TABATMHesapHareketCocuk(ve);
            ve.BeginTransaction();

            if (donusDeger.islemSonuc)
                donusDeger = tabHesapHareketCocuk.Sil(kriter);

            if (donusDeger.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return donusDeger;
        }

    }
}