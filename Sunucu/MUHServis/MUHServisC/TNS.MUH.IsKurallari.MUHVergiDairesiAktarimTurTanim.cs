using System.Data;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// Tanım damga vergisi işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHVergiDairesiAktarimTurTanimServis
    {
        /// <summary>
        /// kaydet.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="damgaVergisi">damga Vergisi</param>
        /// <returns></returns>
        public static Sonuc Kaydet(Kullanici kullanan, ObjectArray damgaVergisi)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            if (!Yetki.SistemYoneticisimi(kullanan))
            {
                sonuc.hataStr = ButceMuhasebeResI.MUH080;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABVergiDairesiAktarimTurTanim tDamga = new TABVergiDairesiAktarimTurTanim(ve);

            foreach (VergiDairesiAktarimTurTanim dv in damgaVergisi.objeler)
            {
                string hata = string.Empty;
                if (dv.turKod == 0)
                    hata = ButceMuhasebeResI.MUH202 + "<br>";
                if (string.IsNullOrEmpty(dv.adi))
                    hata += "Adı boş bırakılamaz.<br>";
                if (dv.aktarimTurKod == 0)
                    hata += "Aktarım Tür Kodu boş bırakılamaz.<br>";
                //if (dv.orani == 0)
                //    hata += "Oranı boş bırakılamaz.<br>";
                if (string.IsNullOrEmpty(dv.hesapKod))
                    hata += "Hesap Kodu alanı boş bırakılamaz.<br>";
                if (!string.IsNullOrEmpty(hata))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = hata;
                    break;
                }
                //bool hesapKoduVarmi = tDamga.HesapKoduVarmi(dv.hesapKod);
                dv.islemYapanAd = kullanan.ADSOYAD;
                dv.islemYapanKod = kullanan.TCKIMLIKNO;
                dv.islemTarihi = new TNSDateTime(System.DateTime.Now);

                if (dv.kod == 0)
                    sonuc = tDamga.Kaydet(dv);
                else
                    sonuc = tDamga.Guncelle(dv);

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

        /// <summary>
        /// Damga Vergisi Aktarımı sil.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="damgaVergisi ">Damga Vergisi </param>
        /// <returns></returns>
        public static Sonuc Sil(Kullanici kullanan, ObjectArray damgaVergisi)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH068, "");

            if (!Yetki.SistemYoneticisimi(kullanan))
            {
                sonuc.hataStr = ButceMuhasebeResI.MUH080;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABVergiDairesiAktarimTurTanim tDamga = new TABVergiDairesiAktarimTurTanim(ve);

            foreach (VergiDairesiAktarimTurTanim dv in damgaVergisi.objeler)
            {
                if (dv.kod != 0)
                    sonuc = tDamga.Sil(dv);
                else
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = ButceMuhasebeResI.MUH133;
                }

                if (!sonuc.islemSonuc)
                    break;
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

        /// <summary>
        /// listele.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="damgaVergisi">damga vergisi</param>
        /// <returns></returns>
        public static ObjectArray Listele(Kullanici kullanan, VergiDairesiAktarimTurTanim damgaVergisi)
        {
            ObjectArray dvler = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABVergiDairesiAktarimTurTanim tDamga = new TABVergiDairesiAktarimTurTanim(ve);

            IDataReader dr = tDamga.Listele(damgaVergisi);
            if (string.IsNullOrEmpty(tDamga.Hata()))
            {
                while (dr.Read())
                {
                    VergiDairesiAktarimTurTanim dv = new VergiDairesiAktarimTurTanim();
                    dv.kod = VeriErisim.DegerAl(dr, 0, 0);
                    dv.turKod = VeriErisim.DegerAl(dr, 1, 0);
                    dv.adi = VeriErisim.DegerAl(dr, 2, string.Empty);
                    dv.hesapKod = VeriErisim.DegerAl(dr, 3, "");
                    dv.orani = VeriErisim.DegerAl(dr, 4, (decimal)0.0);
                    dv.islemYapanAd = VeriErisim.DegerAl(dr, 5, string.Empty);
                    dv.islemYapanKod = VeriErisim.DegerAl(dr, 6, string.Empty);
                    dv.islemTarihi = VeriErisim.DegerAl(dr, 7);
                    dv.aktarimTurKod = VeriErisim.DegerAl(dr, 8, 0);
                    dvler.objeler.Add(dv);
                }
            }
            else
            {
                dvler.sonuc.hataStr = tDamga.Hata();
                dvler.sonuc.islemSonuc = false;
            }
            dr.Close();

            ve.VeriErisimKapat();
            return dvler;
        }
    }
}