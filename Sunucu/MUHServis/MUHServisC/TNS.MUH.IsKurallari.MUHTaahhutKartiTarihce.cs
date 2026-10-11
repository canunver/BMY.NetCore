using System.Data;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// Taahhüt kartı tarihce işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHTaahhutKartiTarihce
    {
        /// <summary>
        /// Taahhüt kartı tarihce kaydet
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="taahhutTarihceBilgileri">Taahhüt tarihce bilgileri</param>
        /// <returns></returns>
        public Sonuc Kaydet(VeriErisim ve, Kullanici kullanan, TaahhutKartiTarihce taahhutTarihce)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTaahhutKartiTarihce to = new TABTaahhutKartiTarihce(ve);

            //kod alınıyor
            //**********************************************************
            TABGenel tg = new TABGenel(ve);
            string sayacKod = "TAAHHUTKODTARIHCE";
            taahhutTarihce.kod = OrtakFonksiyonlar.ConvertToInt(tg.BelgeNoAl(sayacKod, "0000"), 0);
            //**********************************************************

            sonuc = to.Kaydet(taahhutTarihce);


            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH201;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Taahhüt tarihce
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public ObjectArray Listele(Kullanici kullanan, TaahhutKartiTarihce bilgi)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTaahhutKartiTarihce to = new TABTaahhutKartiTarihce(ve);

            IDataReader r = to.Listele(bilgi);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TaahhutKartiTarihce taahhut = new TaahhutKartiTarihce();

                    taahhut.kod = VeriErisim.DegerAl(r, 0, 0);
                    taahhut.taahhutKod = VeriErisim.DegerAl(r, 1, 0);
                    taahhut.islemAdi = VeriErisim.DegerAl(r, 2, "");
                    taahhut.islemKod = VeriErisim.DegerAl(r, 3, 0);
                    taahhut.islemYapanAd = VeriErisim.DegerAl(r, 4, "");
                    taahhut.islemYapanKod = VeriErisim.DegerAl(r, 5, "");
                    taahhut.islemTarihi = VeriErisim.DegerAl(r, 6);
                    taahhut.aciklama = VeriErisim.DegerAl(r, 7, "");

                    iVAN.objeler.Add(taahhut);
                }
                r.Close();
            }

            ve.VeriErisimKapat();

            return iVAN;
        }

        /// <summary>
        /// Taahhüt tarihce sil.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kod">kod</param>
        /// <returns></returns>
        public Sonuc Sil(Kullanici kullanan, TaahhutKartiTarihce taahhutTarihceBilgileri)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH068, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTaahhutKartiTarihce to = new TABTaahhutKartiTarihce(ve);
            sonuc = to.Sil(taahhutTarihceBilgileri);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH134;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;

        }
    }
}
