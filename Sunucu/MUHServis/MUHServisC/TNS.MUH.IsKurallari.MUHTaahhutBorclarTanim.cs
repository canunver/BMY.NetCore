using System.Data;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// Taahhüt BorclarVeTemlikler işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHTaahhutBorclarTanim
    {
        /// <summary>
        /// BorclarVeTemlikler kaydet
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="BorclarVeTemliklerBilgileri">The borclar ve temlikler bilgileri.</param>
        /// <returns></returns>
        public Sonuc Kaydet(Kullanici kullanan, TaahhutBorclarTanim BorclarVeTemliklerBilgileri)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTaahhutBorclarTanim to = new TABTaahhutBorclarTanim(ve);


            BorclarVeTemliklerBilgileri.kod = to.SiraNoBul(BorclarVeTemliklerBilgileri.kod);
            sonuc = to.Kaydet(BorclarVeTemliklerBilgileri);
               
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
        /// TaahhutBorclarVeTemliklerBilgi
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public ObjectArray Listele(Kullanici kullanan, TaahhutBorclarTanim bilgi)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTaahhutBorclarTanim to = new TABTaahhutBorclarTanim(ve);

            IDataReader r = to.Listele(bilgi);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TaahhutBorclarTanim taahhut = new TaahhutBorclarTanim();

                    taahhut.kod = VeriErisim.DegerAl(r, 0, 0);
                    taahhut.konu = VeriErisim.DegerAl(r, 1, "");
                    taahhut.alacakliAdSoyad = VeriErisim.DegerAl(r, 2, "");
                    taahhut.temlikAcanVergiNo = VeriErisim.DegerAl(r, 3, "");
                    taahhut.temlikVerenVergiNo = VeriErisim.DegerAl(r, 4, "");
                    taahhut.temlikTutari = VeriErisim.DegerAl(r, 5, 0.0);
                    taahhut.aciklama = VeriErisim.DegerAl(r, 6, "");
                    taahhut.noterOnayTarihi = VeriErisim.DegerAl(r, 7);
                    taahhut.noterOnayNo = VeriErisim.DegerAl(r, 8, "");
                    taahhut.islemYapanKod = VeriErisim.DegerAl(r, 9, "");
                    taahhut.islemYapanAd = VeriErisim.DegerAl(r, 10,"");
                    taahhut.islemTarihi = VeriErisim.DegerAl(r, 11);
                    iVAN.objeler.Add(taahhut);
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
        /// <param name="kod">kod</param>
        /// <returns></returns>
        public Sonuc Sil(Kullanici kullanan, ObjectArray borcListe)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH068, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTaahhutBorclarTanim to = new TABTaahhutBorclarTanim(ve);

            foreach (TaahhutBorclarTanim borc in borcListe.objeler)
            {
                sonuc = to.Sil(borc.kod);
                if (sonuc.islemSonuc == true)
                    continue;
            }
      

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
