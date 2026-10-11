using System.Data;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;
using TNS.BKD;
using System;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// Mutemet işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHMutemetTanim
    {
        /// <summary>
        /// Mutemet kaydet
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public Sonuc Kaydet(Kullanici kullanan, MutemetTanim bilgi)
        {
            Sonuc sonuc = new Sonuc(true, ButceMuhasebeResI.MUH001, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABMutemetTanim to = new TABMutemetTanim(ve);

            if (bilgi.kod == 0)
                bilgi.kod = to.SiraNoBul(bilgi.kod);

            sonuc = to.Kaydet(bilgi);

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
        /// mutemetTanim
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public ObjectArray Listele(Kullanici kullanan, MutemetTanim bilgi)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABMutemetTanim to = new TABMutemetTanim(ve);

            IDataReader r = to.Listele(bilgi);

            if (r != null)
            {
                Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdemeEmri);

                while (r.Read())
                {
                    MutemetTanim mutemet = new MutemetTanim();

                    mutemet.kod = VeriErisim.DegerAl(r, 0, 0);
                    mutemet.yil = VeriErisim.DegerAl(r, 1, 0);
                    mutemet.tcKimlikNo = VeriErisim.DegerAl(r, 2, "");
                    mutemet.muhasebeKod = VeriErisim.DegerAl(r, 3, 0);
                    mutemet.birimKod = VeriErisim.DegerAl(r, 4, "");
                    mutemet.hesapKod = VeriErisim.DegerAl(r, 5, "");
                    mutemet.tutar = VeriErisim.DegerAl(r, 6, 0.0);
                    mutemet.islemYapanAd = VeriErisim.DegerAl(r, 7, "");
                    mutemet.islemYapanKod = VeriErisim.DegerAl(r, 8, "");
                    mutemet.islemTarihi = VeriErisim.DegerAl(r, 9);
                    mutemet.kisiAdi = VeriErisim.DegerAl(r, 10, "");
                    mutemet.hesapAdi = VeriErisim.DegerAl(r, 11, "");

                    mutemet.hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), mutemet.hesapKod);

                    iVAN.objeler.Add(mutemet);
                }
                r.Close();
            }

            ve.VeriErisimKapat();
            return iVAN;
        }

        /// <summary>
        /// mutemet sil.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="mutemetListe">The mutemet liste.</param>
        /// <returns></returns>
        public Sonuc Sil(Kullanici kullanan, ObjectArray mutemetListe)
        {
            Sonuc sonuc = new Sonuc(true, ButceMuhasebeResI.MUH001, "");

            if (mutemetListe.objeler.Count == 0 || mutemetListe == null)
            {
                sonuc.DegerAta(ButceMuhasebeResI.MUH133);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABMutemetTanim to = new TABMutemetTanim(ve);

            foreach (MutemetTanim t in mutemetListe.objeler)
            {
                sonuc = to.Sil(t.kod);
                if (!sonuc.islemSonuc)
                    break;
            }


            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Kayıt silindi";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Mutemet listesi aktar
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kaynakYil">Kaynak yil</param>
        /// <param name="hedefYil">Hedef yil</param>
        /// <returns></returns>
        public Sonuc Aktar(Kullanici kullanan, int kaynakYil, int hedefYil)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            ObjectArray kaynakListe = Listele(kullanan, new MutemetTanim { yil = kaynakYil });
            if (kaynakListe.objeler.Count == 0)
            {
                sonuc.hataStr = "Kaynak gösterilen yılda kayıt yok";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABMutemetTanim to = new TABMutemetTanim(ve);
            sonuc = to.Aktar(kaynakYil, hedefYil);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                ve.BeginTransaction();
                foreach (MutemetTanim hk in kaynakListe.objeler)
                {
                    hk.yil = hedefYil;
                    hk.kod = 0;
                    sonuc = Kaydet(kullanan, hk);
                    if (!sonuc.islemSonuc) break;
                }
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = string.Format("Mutemet bilgileri {0} yılına aktarıldı", hedefYil);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}
