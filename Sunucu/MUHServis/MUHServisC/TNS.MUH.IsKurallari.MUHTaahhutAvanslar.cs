using System.Data;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// MUHVerilenAvanslarVeMahsuplar işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHTaahhutAvanslar
    {
        /// <summary>
        /// VerilenAvanslarVeMahsuplar kaydet
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgiler">The bilgiler.</param>
        /// <returns></returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray bilgiler)
        {
            Sonuc sonuc = new Sonuc(true, ButceMuhasebeResI.MUH001, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTaahhutAvanslar to = new TABTaahhutAvanslar(ve);
            TABTaahhutKarti tk = new TABTaahhutKarti(ve);
            bool kapali = false;

            foreach (TaahhutAvanslar bilgi in bilgiler.objeler)
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
        /// VerilenAvanslarVeMahsuplarBilgi
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public ObjectArray Listele(Kullanici kullanan, TaahhutAvanslar bilgi)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTaahhutAvanslar to = new TABTaahhutAvanslar(ve);

            IDataReader r = to.Listele(bilgi);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TaahhutAvanslar teminat = new TaahhutAvanslar();

                    teminat.taahhutKod = VeriErisim.DegerAl(r, 0, 0);
                    teminat.yil = VeriErisim.DegerAl(r, 1, 0);
                    teminat.belgeNo = VeriErisim.DegerAl(r, 2, "");
                    teminat.tarih = VeriErisim.DegerAl(r, 3);
                    teminat.numara = VeriErisim.DegerAl(r, 4, "");
                    teminat.verilenAvans = VeriErisim.DegerAl(r, 5, 0.0);
                    teminat.mahsupEdilenAvans = VeriErisim.DegerAl(r, 6, 0.0);
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
        /// VerilenAvanslarVeMahsuplarBilgi sil.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="avansListesi">The avans listesi.</param>
        /// <returns></returns>
        public Sonuc Sil(Kullanici kullanan, ObjectArray avansListesi)
        {
            Sonuc sonuc = new Sonuc(true, ButceMuhasebeResI.MUH001, "");

            if (avansListesi.objeler.Count == 0 || avansListesi == null)
            {
                sonuc.DegerAta(ButceMuhasebeResI.MUH133);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTaahhutAvanslar to = new TABTaahhutAvanslar(ve);

            foreach (TaahhutAvanslar bilgi in avansListesi.objeler)
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
