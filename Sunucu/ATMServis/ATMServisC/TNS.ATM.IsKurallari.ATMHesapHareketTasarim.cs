using System;
using System.Data;
using TNS.ATM.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.ATM.IsKurallari
{
    /// <summary>
    /// hesap Hareket tasarım Sınıfı
    /// </summary>
    public class DosyaHesapHareketTasarimClass
    {

        /// <summary>
        /// Guncelles the specified veri erisim.
        /// </summary>
        /// <param name="veriErisim">Veri Erişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="hesapHareketTasarim">The hesap hareket tasarim.</param>
        /// <returns>
        /// Sonuc tipinde sınıf döndürür
        /// </returns>
        public static Sonuc Guncelle(VeriErisim veriErisim, Kullanici kullanan, DosyaHesapHareketTasarim hesapHareketTasarim)
        {
            Sonuc donusDeger = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMHesapHareketTasarim tabHesapHareketTasarim = new TABATMHesapHareketTasarim(ve);
            ve.BeginTransaction();

            try
            {
                donusDeger = tabHesapHareketTasarim.Guncelle(hesapHareketTasarim);
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
        /// DosyaHesapHareketTasarim bilgilerini kaydeden fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="listeHesapHareketTasarim">The liste hesap hareket tasarim.</param>
        /// <returns>
        /// Sonuc tipinde sınıf döndürür
        /// </returns>
        public Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ObjectArray listeHesapHareketTasarim)
        {
            Sonuc donusDeger = new Sonuc();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMHesapHareketTasarim tabHesapHareketTasarim = new TABATMHesapHareketTasarim(ve);
            ve.BeginTransaction();

            donusDeger = tabHesapHareketTasarim.Sil((DosyaHesapHareketTasarim)listeHesapHareketTasarim.objeler[0]);

            if (donusDeger.islemSonuc)
            {
                foreach (DosyaHesapHareketTasarim bilgi in listeHesapHareketTasarim.objeler)
                {
                    bilgi.kod = tabHesapHareketTasarim.KodBul();
                    donusDeger = tabHesapHareketTasarim.Kaydet(bilgi);

                    if (!donusDeger.islemSonuc)
                        break;
                }

                ve.Commit();
            }

            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return donusDeger;
        }

        /// <summary>
        /// DosyaHesapHareketTasarim  bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="hesapHareketTasarim">The hesap hareket tasarim.</param>
        /// <returns>
        /// ObjectArray tipinde sınıf döndürür
        /// </returns>
        public ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, DosyaHesapHareketTasarim hesapHareketTasarim)
        {
            ObjectArray donusDeger = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMHesapHareketTasarim tabHesapHareketTasarim = new TABATMHesapHareketTasarim(ve);
            IDataReader dr = null;

            try
            {
                dr = tabHesapHareketTasarim.Listele(hesapHareketTasarim);
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
        /// DosyaHesapHareketTasarim bilgilerini DataReader sınıfından alan fonksiyon
        /// </summary>
        /// <param name="dr">Data Reader, IDataReader tipinde sınıf</param>
        /// <returns>DosyaHesapHareketTasarim tipinde sınıf döndürür</returns>
        private DosyaHesapHareketTasarim DosyaAl(IDataReader dr)
        {
            DosyaHesapHareketTasarim hareket = new DosyaHesapHareketTasarim();
            hareket.kod = VeriErisim.DegerAl(dr, 0, 0);
            hareket.siraNo = VeriErisim.DegerAl(dr, 1, 0);
            hareket.hesapKodu = VeriErisim.DegerAl(dr, 2, "");
            hareket.yansitmaHesapKodu = VeriErisim.DegerAl(dr, 3, "");
            hareket.kdvHesapKodu = VeriErisim.DegerAl(dr, 4, "");
            hareket.altHesap = VeriErisim.DegerAl(dr, 5, 0);
            hareket.belgeSatirNo = VeriErisim.DegerAl(dr, 6, 0);
            hareket.oran = VeriErisim.DegerAl(dr, 7, (decimal)0);
            hareket.adi = VeriErisim.DegerAl(dr, 8, "");
            hareket.borcAlacak = VeriErisim.DegerAl(dr, 9, -1);
            hareket.islemYapanAd = VeriErisim.DegerAl(dr, 10, "");
            hareket.islemYapanKod = VeriErisim.DegerAl(dr, 11, "");
            hareket.islemTarihi = VeriErisim.DegerAl(dr, 12);
            hareket.anaKod = VeriErisim.DegerAl(dr, 13, 0);
            hareket.alanAdi = VeriErisim.DegerAl(dr, 14, "");
            hareket.formul = VeriErisim.DegerAl(dr, 15, "");
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
        public Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, DosyaHesapHareketTasarim kriter)
        {
            Sonuc donusDeger = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(null));
            TABATMHesapHareketTasarim tabHesapHareketTasarim = new TABATMHesapHareketTasarim(ve);
            ve.BeginTransaction();

            //if (donusDeger.islemSonuc)
            //    donusDeger = tabHesapHareketTasarim.Sil(kriter);

            if (donusDeger.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return donusDeger;
        }

    }
}