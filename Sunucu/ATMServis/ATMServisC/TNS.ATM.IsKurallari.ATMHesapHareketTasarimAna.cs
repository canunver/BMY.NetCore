using System;
using System.Data;
using TNS.ATM.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.ATM.IsKurallari
{
    /// <summary>
    /// hesap Hareket tasarım Ana Sınıfı
    /// </summary>
    public class DosyaHesapHareketTasarimAnaClass
    {

        /// <param name="veriErisim">Veri Erişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="DosyaHesapHareketTasarimAna">DosyaHesapHareketTasarimAna tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        public static Sonuc Guncelle(VeriErisim veriErisim, Kullanici kullanan, DosyaHesapHareketTasarimAna hesapHareketTasarimAna)
        {
            Sonuc donusDeger = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMHesapHareketTasarimAna tabHesapHareketTasarimAna = new TABATMHesapHareketTasarimAna(ve);
            ve.BeginTransaction();

            try
            {
                donusDeger = tabHesapHareketTasarimAna.Guncelle(hesapHareketTasarimAna);
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
        /// DosyaHesapHareketTasarimAna bilgilerini kaydeden fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="DosyaHesapHareketTasarimAna">DosyaHesapHareketTasarimAna dosya bilgisi tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        public Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, DosyaHesapHareketTasarimAna hesapHareketTasarimAna)
        {
            Sonuc donusDeger = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMHesapHareketTasarimAna tabHesapHareketTasarimAna = new TABATMHesapHareketTasarimAna(ve);
            ve.BeginTransaction();

            if (donusDeger.islemSonuc)
            {
                if (hesapHareketTasarimAna.kod == 0)
                {
                    hesapHareketTasarimAna.kod = tabHesapHareketTasarimAna.KodBul();
                    donusDeger = tabHesapHareketTasarimAna.Kaydet(hesapHareketTasarimAna);
                }
                else
                    donusDeger = tabHesapHareketTasarimAna.Guncelle(hesapHareketTasarimAna);

                if (donusDeger.islemSonuc)
                      ve.Commit();
            }

            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return donusDeger;
        }

        /// <summary>
        /// DosyaHesapHareketTasarimAna  bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="DosyaHesapHareketTasarimAna ">DosyaHesapHareketTasarimAna tipinde sınıf</param>
        /// <returns>ObjectArray tipinde sınıf döndürür</returns>
        public ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, DosyaHesapHareketTasarimAna hesapHareketTasarimAna)
        {
            ObjectArray donusDeger = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMHesapHareketTasarimAna tabHesapHareketTasarimAna = new TABATMHesapHareketTasarimAna(ve);
            IDataReader dr = null;

            try
            {
                dr = tabHesapHareketTasarimAna.Listele(hesapHareketTasarimAna);
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
        /// DosyaHesapHareketTasarimAna bilgilerini DataReader sınıfından alan fonksiyon
        /// </summary>
        /// <param name="dr">Data Reader, IDataReader tipinde sınıf</param>
        /// <returns>DosyaHesapHareketTasarimAna tipinde sınıf döndürür</returns>
        private DosyaHesapHareketTasarimAna DosyaAl(IDataReader dr)
        {
            DosyaHesapHareketTasarimAna hareketAna = new DosyaHesapHareketTasarimAna();
            hareketAna.kod = VeriErisim.DegerAl(dr, 0, 0);
            hareketAna.ad = VeriErisim.DegerAl(dr, 1, "");
            hareketAna.aktif = VeriErisim.DegerAl(dr, 2, 0);
            hareketAna.islemYapanAd = VeriErisim.DegerAl(dr, 3, "");
            hareketAna.islemYapanKod = VeriErisim.DegerAl(dr, 4, "");
            hareketAna.islemTarihi = VeriErisim.DegerAl(dr, 5);
            return hareketAna;
        }

        /// <summary>
        /// Arguman olarak verilen, taşınır form bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, DosyaHesapHareketTasarimAna kriter)
        {
            Sonuc donusDeger = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(null));
            TABATMHesapHareketTasarimAna tabHesapHareketTasarimAna = new TABATMHesapHareketTasarimAna(ve);
            ve.BeginTransaction();

            if (donusDeger.islemSonuc)
                donusDeger = tabHesapHareketTasarimAna.Sil(kriter);

            if (donusDeger.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return donusDeger;
        }

    }
}