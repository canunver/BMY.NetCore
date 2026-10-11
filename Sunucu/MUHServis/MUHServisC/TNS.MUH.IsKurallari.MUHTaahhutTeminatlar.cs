using System.Data;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// AlinanTeminatlar işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHTaahhutTeminatlar
    {
        /// <summary>
        /// AlinanTeminatlar kaydet
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="teminatBilgileri">The teminat bilgileri.</param>
        /// <returns></returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray teminatBilgileri)
        {
            Sonuc sonuc = new Sonuc(true, ButceMuhasebeResI.MUH001, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTaahhutTeminatlar to = new TABTaahhutTeminatlar(ve);
            TABTaahhutKarti tk = new TABTaahhutKarti(ve);
            bool kapali = false;

            foreach (TaahhutTeminatlar bilgi in teminatBilgileri.objeler)
            {
                kapali = tk.TaahhutKapaliMi(bilgi.taahhutKod);

                if (kapali == true)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = ButceMuhasebeResI.MUH204;
                }
                else
                {
                    bool veri = to.KayitVarMi(bilgi);
                    if (veri == false)
                    {
                        sonuc = to.Kaydet(bilgi);
                        if (!sonuc.islemSonuc)
                            break;
                    }
                }
            }

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
        /// AlinanTeminatlar
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public ObjectArray Listele(Kullanici kullanan, TaahhutTeminatlar bilgi)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTaahhutTeminatlar to = new TABTaahhutTeminatlar(ve);

            IDataReader r = to.Listele(bilgi);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TaahhutTeminatlar teminat = new TaahhutTeminatlar();

                    //teminat.kod = VeriErisim.DegerAl(r, 0, 0);
                    teminat.taahhutKod = VeriErisim.DegerAl(r, 0, 0);
                    teminat.yil = VeriErisim.DegerAl(r, 1, 0);
                    teminat.dosyaNo = VeriErisim.DegerAl(r, 2, "");
                    teminat.tarih = VeriErisim.DegerAl(r, 3);
                    teminat.numara = VeriErisim.DegerAl(r, 4, "");
                    teminat.aciklama = VeriErisim.DegerAl(r, 5, "");
                    teminat.tutar = VeriErisim.DegerAl(r, 6, 0.0);
                    teminat.islemTarihi = VeriErisim.DegerAl(r, 7);
                    teminat.islemYapanKod = VeriErisim.DegerAl(r, 8, "");
                    teminat.islemYapanAd = VeriErisim.DegerAl(r, 9, "");
                    iVAN.objeler.Add(teminat);
                }
                r.Close();
            }

            ve.VeriErisimKapat();

            return iVAN;
        }

        /// <summary>
        /// Taahhüt sil.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">The liste.</param>
        /// <returns></returns>
        public Sonuc Sil(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(true, ButceMuhasebeResI.MUH001, "");

            if (liste.objeler.Count == 0 || liste == null)
            {
                sonuc.DegerAta(ButceMuhasebeResI.MUH133);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTaahhutTeminatlar to = new TABTaahhutTeminatlar(ve);

            foreach (TaahhutTeminatlar bilgi in liste.objeler)
            {
                sonuc = to.Sil(bilgi);
                if (!sonuc.islemSonuc)
                    break;
            }

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
    }
}
