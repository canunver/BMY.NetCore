using System;
using System.Data;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// Taahhüt işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHTaahhut
    {
        /// <summary>
        /// Taahhüt kaydet
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="taahhutBilgileri">Taahhüt bilgileri</param>
        /// <returns></returns>
        public static Sonuc Kaydet(Kullanici kullanan, int taahhutKod, string degerArtisiKod, TNSCollection taahhutBilgileri)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TaahhutKartiBilgi okunanKart = MUHTaahhutKarti.TaahhutKartOku(ve, kullanan, taahhutKod, null, null, TaahhutKartiBilgi.ENUMSozTur.Hepsi, sonuc, false);
            if (okunanKart == null)
            {
                sonuc.DegerAta("Lütfen tahahhkuk kartı seçiniz!");
            }
            else
            {
                if (okunanKart.kapandi == 1)
                    sonuc.DegerAta(ButceMuhasebeResI.MUH204);
            }

            if (sonuc.islemSonuc)
            {
                foreach (TaahhutBilgi bilgi in taahhutBilgileri)
                {
                    bilgi.taahhutKod = taahhutKod;
                    bilgi.degerArtisKod = degerArtisiKod;
                    if (!bilgi.bosMu)
                    {
                        if (string.IsNullOrWhiteSpace(bilgi.tertipKod))
                            sonuc.DegerAta("Tertip alanı boş olamaz!");
                        if (bilgi.yil <= 0)
                            sonuc.DegerAta("Yıl alanı dolu olmalı!");
                        if (bilgi.ay <= 0 || bilgi.ay > 12)
                            sonuc.DegerAta("Ay alanı 1-12 arasında olmalı!");
                        //if (bilgi.ilkTaahhutTutari < 0 || bilgi.fiyatFarki < 0 || bilgi.ilaveTaahhutler < 0 || bilgi.digerTaahhutler < 0)
                        //    sonuc.DegerAta("Girilen rakamlar negatif olamaz!");
                    }
                }
            }

            if (sonuc.islemSonuc)
            {
                TABTaahhut to = new TABTaahhut(ve);
                ve.BeginTransaction();
                foreach (TaahhutBilgi bilgi in taahhutBilgileri)
                {
                    if (!bilgi.bosMu)
                    {
                        bool ekle = bilgi.kod <= 0;
                        if (ekle)
                            bilgi.kod = to.SiraNoBul();
                        sonuc = to.Kaydet(bilgi, ekle);

                        if (!sonuc.islemSonuc)
                            break;
                    }
                }
                if (sonuc.islemSonuc && okunanKart.sozTur == TaahhutKartiBilgi.ENUMSozTur.Personel)
                    sonuc = to.SozlesmeTutarGuncelle(taahhutKod);

                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Imza Taahhüt
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, TaahhutBilgi bilgi)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTaahhut to = new TABTaahhut(ve);
            IDataReader r = to.Listele(bilgi);
            if (r != null)
            {
                while (r.Read())
                {
                    TaahhutBilgi taahhut = new TaahhutBilgi();

                    taahhut.kod = VeriErisim.DegerAl(r, 0, 0);
                    taahhut.taahhutKod = VeriErisim.DegerAl(r, 1, 0);
                    taahhut.yil = VeriErisim.DegerAl(r, (int)TABTaahhut.ENUMAlanlar.YIL, 0);
                    taahhut.ay = VeriErisim.DegerAl(r, (int)TABTaahhut.ENUMAlanlar.AY, 0);

                    taahhut.ilkTaahhutTutari = VeriErisim.DegerAl(r, (int)TABTaahhut.ENUMAlanlar.ILKTAAHHUTTUTARI, 0.0);
                    taahhut.fiyatFarki = VeriErisim.DegerAl(r, (int)TABTaahhut.ENUMAlanlar.FIYATFARKI, 0.0);
                    taahhut.ilaveTaahhutler = VeriErisim.DegerAl(r, (int)TABTaahhut.ENUMAlanlar.ILAVETAAHHUTLER, 0.0);
                    taahhut.digerTaahhutler = VeriErisim.DegerAl(r, (int)TABTaahhut.ENUMAlanlar.DIGERTAAHHUTLER, 0.0);
                    taahhut.islemTarihi = VeriErisim.DegerAl(r, (int)TABTaahhut.ENUMAlanlar.ISLEMTARIHI);
                    taahhut.islemYapanKod = VeriErisim.DegerAl(r, (int)TABTaahhut.ENUMAlanlar.ISLEMYAPANKOD, "");
                    taahhut.islemYapanAd = VeriErisim.DegerAl(r, (int)TABTaahhut.ENUMAlanlar.ISLEMYAPANAD, "");

                    taahhut.degerArtisKod = VeriErisim.DegerAl(r, (int)TABTaahhut.ENUMAlanlar.TAAHHUTDEGERARTISIKOD, "");
                    taahhut.tertipKod = VeriErisim.DegerAl(r, (int)TABTaahhut.ENUMAlanlar.TERTIPKOD, "");

                    iVAN.objeler.Add(taahhut);
                }
                r.Close();
            }
            ve.VeriErisimKapat();

            return iVAN;
        }

        public static ObjectArray TaahhutToplamListele(VeriErisim veriErisim, Kullanici kullanan, int taahhutKod)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTaahhut to = new TABTaahhut(ve);
            TertipToplamBul(to, iVAN, taahhutKod, false);
            TertipToplamBul(to, iVAN, taahhutKod, true);
            ve.VeriErisimKapat();
            return iVAN;
        }

        private static void TertipToplamBul(TABTaahhut to, ObjectArray oa, int taahhutKod, bool degerArtisiMi)
        {
            IDataReader r = to.MevcutOnayliToplamBul(taahhutKod, false);
            if (r != null)
            {
                while (r.Read())
                {
                    TaahhutBilgi taahhut = new TaahhutBilgi();
                    int col = 0;
                    taahhut.tertipKod = VeriErisim.DegerAl(r, col++, "");
                    taahhut.yil = VeriErisim.DegerAl(r, col++, 0);
                    taahhut.ay = VeriErisim.DegerAl(r, col++, 0);

                    taahhut.ilkTaahhutTutari = VeriErisim.DegerAl(r, col++, 0.0);
                    taahhut.fiyatFarki = VeriErisim.DegerAl(r, col++, 0.0);
                    taahhut.ilaveTaahhutler = VeriErisim.DegerAl(r, col++, 0.0);
                    taahhut.digerTaahhutler = VeriErisim.DegerAl(r, col++, 0.0);

                    YoksaEkleVarsaArtir(oa, taahhut);
                }
                r.Close();
            }
        }

        private static void YoksaEkleVarsaArtir(ObjectArray oa, TaahhutBilgi taahhut)
        {
            foreach (TaahhutBilgi item in oa.objeler)
            {
                if (item.tertipKod == taahhut.tertipKod && item.yil == taahhut.yil && item.ay == taahhut.ay)
                {
                    taahhut.ilkTaahhutTutari += taahhut.ilkTaahhutTutari;
                    taahhut.fiyatFarki += taahhut.ilkTaahhutTutari;
                    taahhut.ilaveTaahhutler += taahhut.ilkTaahhutTutari;
                    taahhut.digerTaahhutler += taahhut.ilkTaahhutTutari;
                    return;
                }
            }
            oa.objeler.Add(taahhut);
        }

        /// <summary>
        /// Taahhüt sil.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kod">kod</param>
        /// <returns></returns>
        public Sonuc Sil(Kullanici kullanan, int kod)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH068, "");

            //if (!Yetki.SistemYoneticisimi(kullanan))
            //{
            //    sonuc.hataStr = ButceMuhasebeResI.MUH080;
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}
            if (kod <= 0)
            {
                sonuc.DegerAta(ButceMuhasebeResI.MUH133);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTaahhut to = new TABTaahhut(ve);
            sonuc = to.Sil(kod, 0, null);

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

        internal static ObjectArray OEBekleyenListele(VeriErisim ve, Kullanici kullanan, int yil, int muhasebeKod, string birimKod)
        {
            ObjectArray iVAN = new ObjectArray();

            TABTaahhut to = new TABTaahhut(ve);
            IDataReader r = to.OEBekleyenListele(yil, muhasebeKod, birimKod, false);
            if (r != null)
            {
                while (r.Read())
                {
                    iVAN.objeler.Add(TaahhutOku(r));
                }
                r.Close();
            }
            else
                iVAN.sonuc.DegerAta("Veri tabanından kayıt okunamadı:" + ve.hata);
            if (iVAN.sonuc.islemSonuc)
            {
                r = to.OEBekleyenListele(yil, muhasebeKod, birimKod, true);
                if (r != null)
                {
                    while (r.Read())
                    {
                        iVAN.objeler.Add(TaahhutOku(r));
                    }
                    r.Close();
                }
                else
                    iVAN.sonuc.DegerAta("Veri tabanından kayıt okunamadı:" + ve.hata);
            }

            return iVAN;
        }

        private static TaahhutBilgi TaahhutOku(IDataReader r)
        {
            TaahhutBilgi taahhut = new TaahhutBilgi();

            taahhut.kod = VeriErisim.DegerAl(r, 0, 0);
            taahhut.taahhutKod = VeriErisim.DegerAl(r, 1, 0);
            taahhut.yil = VeriErisim.DegerAl(r, (int)TABTaahhut.ENUMAlanlar.YIL, 0);
            taahhut.ay = VeriErisim.DegerAl(r, (int)TABTaahhut.ENUMAlanlar.AY, 0);
            taahhut.ilkTaahhutTutari = VeriErisim.DegerAl(r, (int)TABTaahhut.ENUMAlanlar.ILKTAAHHUTTUTARI, 0.0);
            taahhut.fiyatFarki = VeriErisim.DegerAl(r, (int)TABTaahhut.ENUMAlanlar.FIYATFARKI, 0.0);
            taahhut.ilaveTaahhutler = VeriErisim.DegerAl(r, (int)TABTaahhut.ENUMAlanlar.ILAVETAAHHUTLER, 0.0);
            taahhut.digerTaahhutler = VeriErisim.DegerAl(r, (int)TABTaahhut.ENUMAlanlar.DIGERTAAHHUTLER, 0.0);
            taahhut.islemTarihi = VeriErisim.DegerAl(r, (int)TABTaahhut.ENUMAlanlar.ISLEMTARIHI);
            taahhut.islemYapanKod = VeriErisim.DegerAl(r, (int)TABTaahhut.ENUMAlanlar.ISLEMYAPANKOD, "");
            taahhut.islemYapanAd = VeriErisim.DegerAl(r, (int)TABTaahhut.ENUMAlanlar.ISLEMYAPANAD, "");

            taahhut.degerArtisKod = VeriErisim.DegerAl(r, (int)TABTaahhut.ENUMAlanlar.TAAHHUTDEGERARTISIKOD, "");
            taahhut.tertipKod = VeriErisim.DegerAl(r, (int)TABTaahhut.ENUMAlanlar.TERTIPKOD, "");

            int tut = (int)TABTaahhut.ENUMAlanlar.SONRAKIYILTUTAR;
            taahhut.muteahhitKimlikNo = VeriErisim.DegerAl(r, ++tut, "");
            taahhut.MIFYil = VeriErisim.DegerAl(r, ++tut, 0);
            taahhut.MIFNo = VeriErisim.DegerAl(r, ++tut, "");
            taahhut.MIFDurum = VeriErisim.DegerAl(r, ++tut, 0);

            return taahhut;
        }
    }
}
