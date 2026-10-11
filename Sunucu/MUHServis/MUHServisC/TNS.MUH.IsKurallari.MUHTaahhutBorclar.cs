using System.Data;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// Taahhüt borclar işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHTaahhutBorclar
    {
        /// <summary>
        /// TaahhutBorclar kaydet
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="teminatBilgileri">The borç bilgileri.</param>
        /// <returns></returns>
        public static Sonuc Kaydet(Kullanici kullanan, ObjectArray borcBilgileri)
        {
            Sonuc sonuc = new Sonuc(true, ButceMuhasebeResI.MUH001, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTaahhutBorclar to = new TABTaahhutBorclar(ve);
            TABTaahhutKarti tk = new TABTaahhutKarti(ve);
            bool kapali = false;

            foreach (TaahhutBorclar bilgi in borcBilgileri.objeler)
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
        /// TaahhutBorclar
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public static ObjectArray Listele(Kullanici kullanan, TaahhutBorclar bilgi)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTaahhutBorclar to = new TABTaahhutBorclar(ve);

            IDataReader r = to.Listele(bilgi);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TaahhutBorclar borclar = new TaahhutBorclar();

                    borclar.kod = VeriErisim.DegerAl(r, 0, 0);
                    borclar.taahhutKod = VeriErisim.DegerAl(r, 1, 0);
                    borclar.dosyaNo = VeriErisim.DegerAl(r, 2, 0);
                    borclar.alacakliAdSoyad = VeriErisim.DegerAl(r, 3, "");
                    borclar.temlikAcanVergiNo = VeriErisim.DegerAl(r, 4, "");
                    borclar.temlikVerenVergiNo = VeriErisim.DegerAl(r, 5, "");
                    borclar.temlikTutari = VeriErisim.DegerAl(r, 6, 0.0);
                    borclar.islemYapanAd = VeriErisim.DegerAl(r, 7, "");
                    borclar.islemYapanKod = VeriErisim.DegerAl(r, 8, "");
                    borclar.islemTarihi = VeriErisim.DegerAl(r, 9);
                    iVAN.objeler.Add(borclar);
                }
                r.Close();
            }

            ve.VeriErisimKapat();

            return iVAN;
        }

        /// <summary>
        /// Borc sil
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">The liste.</param>
        /// <returns></returns>
        public static Sonuc Sil(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(true, ButceMuhasebeResI.MUH001, "");

            if (liste.objeler.Count == 0 || liste == null)
            {
                sonuc.DegerAta(ButceMuhasebeResI.MUH133);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTaahhutBorclar to = new TABTaahhutBorclar(ve);

            foreach (TaahhutBorclar bilgi in liste.objeler)
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
