using System;
using System.Data;
using TNS.ATM.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.ATM.IsKurallari
{
    /// <summary>
    /// hesap Hareket Sınıfı
    /// </summary>
    public class DosyaHesapHareketAnaClass
    {
        /// <summary>
        /// Guncelles the specified veri erisim.
        /// </summary>
        /// <param name="veriErisim">Veri Erişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="hesapHareketAna">The hesap hareket ana.</param>
        /// <returns>
        /// Sonuc tipinde sınıf döndürür
        /// </returns>
        public static Sonuc Guncelle(VeriErisim veriErisim, Kullanici kullanan, DosyaHesapHareketAna hesapHareketAna)
        {
            Sonuc donusDeger = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMHesapHareketAna tabHesapHareketAna = new TABATMHesapHareketAna(ve);
            ve.BeginTransaction();

            try
            {
                donusDeger = tabHesapHareketAna.Guncelle(hesapHareketAna);
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
        /// DosyaHesapHareketAna dosya bilgilerini kaydeden fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="hesapHareketAna">The hesap hareket ana.</param>
        /// <returns>
        /// Sonuc tipinde sınıf döndürür
        /// </returns>
        public Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, DosyaHesapHareketAna hesapHareketAna)
        {
            Sonuc donusDeger = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMHesapHareketAna tabHesapHareketAna = new TABATMHesapHareketAna(ve);
            ve.BeginTransaction();

            try
            {
                if (hesapHareketAna.kod == 0)
                    donusDeger = tabHesapHareketAna.Kaydet(hesapHareketAna);
                else
                    donusDeger = tabHesapHareketAna.Guncelle(hesapHareketAna);
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
        /// Alacak Takip dosya bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="hesapHareketAna">The hesap hareket ana.</param>
        /// <returns>
        /// ObjectArray tipinde sınıf döndürür
        /// </returns>
        public ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, DosyaHesapHareketAna hesapHareketAna)
        {
            ObjectArray donusDeger = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMHesapHareketAna tabHesapHareketAna = new TABATMHesapHareketAna(ve);
            IDataReader dr = null;

            try
            {
                dr = tabHesapHareketAna.Listele(hesapHareketAna);
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
        /// Alacak Takip dosya bilgilerini DataReader sınıfından alan fonksiyon
        /// </summary>
        /// <param name="dr">Data Reader, IDataReader tipinde sınıf</param>
        /// <returns>Dosya tipinde sınıf döndürür</returns>
        private DosyaHesapHareketAna DosyaAl(IDataReader dr)
        {
            DosyaHesapHareketAna hareket = new DosyaHesapHareketAna();
            hareket.kod = VeriErisim.DegerAl(dr, 0, 0);
            hareket.yil = VeriErisim.DegerAl(dr, 1, 0);
            hareket.dosyaNo = VeriErisim.DegerAl(dr, 2, 0);
            hareket.tasarimKod = VeriErisim.DegerAl(dr, 3, 0);
            hareket.tarih = VeriErisim.DegerAl(dr, 4);
            hareket.odemeEmriNo = VeriErisim.DegerAl(dr, 5, "");
            hareket.toplamTutar = VeriErisim.DegerAl(dr, 6, 0.0);
            hareket.islemYapanAd = VeriErisim.DegerAl(dr, 7, "");
            hareket.islemYapanKod = VeriErisim.DegerAl(dr, 8, "");
            hareket.islemTarihi = VeriErisim.DegerAl(dr, 9);
            hareket.tasarimAdi = VeriErisim.DegerAl(dr, 10, "");

            return hareket;
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
        public Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, DosyaHesapHareketAna kriter)
        {
            Sonuc donusDeger = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(null));
            TABATMHesapHareketAna tabHesapHareketAna = new TABATMHesapHareketAna(ve);
            TABATMHesapHareketCocuk tabHesapHareketCocuk = new TABATMHesapHareketCocuk(ve);
            DosyaHesapHareketCocuk cocuk = new DosyaHesapHareketCocuk();
            cocuk.anaKod = kriter.kod;
            ve.BeginTransaction();


            donusDeger = tabHesapHareketAna.Sil(kriter);
            if (donusDeger.islemSonuc)
            {
                donusDeger = tabHesapHareketCocuk.Sil(cocuk);
                if (donusDeger.islemSonuc)
                    ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return donusDeger;
        }

        /// <summary>
        /// Alacak Takip dosya bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="hesapHareketAna">The hesap hareket ana.</param>
        /// <returns>
        /// ObjectArray tipinde sınıf döndürür
        /// </returns>
        public ObjectArray ListeleDosya(VeriErisim veriErisim, Kullanici kullanan, DosyaHesapHareketAna hesapHareketAna)
        {
            ObjectArray donusDeger = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMHesapHareketAna tabHesapHareketAna = new TABATMHesapHareketAna(ve);
            IDataReader dr = null;

            try
            {
                dr = tabHesapHareketAna.ListeleDosya(hesapHareketAna);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                ve.VeriErisimKapat();
                return donusDeger;
            }


            while (dr.Read())
                donusDeger.objeler.Add(DosyaAlDosya(dr));

            if (donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = AlacakTakipResI.ATA018;

            donusDeger.sonuc.islemSonuc = true;

            dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Alacak Takip dosya bilgilerini DataReader sınıfından alan fonksiyon
        /// </summary>
        /// <param name="dr">Data Reader, IDataReader tipinde sınıf</param>
        /// <returns>Dosya tipinde sınıf döndürür</returns>
        private DosyaHesapHareketAna DosyaAlDosya(IDataReader dr)
        {
            DosyaHesapHareketAna hareket = new DosyaHesapHareketAna();
            hareket.kod = VeriErisim.DegerAl(dr, 0, 0);
            hareket.yil = VeriErisim.DegerAl(dr, 1, 0);
            hareket.dosyaNo = VeriErisim.DegerAl(dr, 2, 0);
            string birim = VeriErisim.DegerAl(dr, 3, "");
            hareket.borcluAdi = VeriErisim.DegerAl(dr, 4, "");
            int saymanlik = hareket.saymanlikKodu = VeriErisim.DegerAl(dr, 5, 0);
            hareket.toplamTutar = VeriErisim.DegerAl(dr, 6, 0.0);
            hareket.islemTarihi = VeriErisim.DegerAl(dr, 7);
            hareket.odemeEmriNo = VeriErisim.DegerAl(dr, 8, "");
            hareket.birimKodu = Genel.BirimAdiDondur(saymanlik, birim);
            hareket.tasarimKod = VeriErisim.DegerAl(dr, 9, 0);
            hareket.tasarimAdi = VeriErisim.DegerAl(dr, 10, "");
            return hareket;
        }

        /// <summary>
        /// DosyaHesapHareketAna ödeme emri no bilgilerini kaydeden fonksiyon
        /// </summary>
        public Sonuc KaydetOdemeEmriNo(VeriErisim veriErisim, Kullanici kullanan, ObjectArray hesapHareketAnaList)
        {
            Sonuc donusDeger = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMHesapHareketAna tabHesapHareketAna = new TABATMHesapHareketAna(ve);
            ve.BeginTransaction();

            try
            {
                if (hesapHareketAnaList.objeler.Count > 0)
                {
                    foreach (DosyaHesapHareketAna ana in hesapHareketAnaList.objeler)
                    {
                        donusDeger = tabHesapHareketAna.KaydetOdemeEmriNo(ana);
                        if (!donusDeger.islemSonuc)
                            break;
                    }
                }
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

    }
}