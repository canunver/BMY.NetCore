using System;
using System.Data;
using TNS.BKD;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// ödenek ihtiyaç işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHOdenekIhtiyac
    {
        static Tertip to = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdenek);

        /// <summary>
        /// Ödenek ihtiyaç kaydet.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="ihtiyac">ihtiyac</param>
        /// <returns></returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray ihtiyac)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            if (!(Yetki.SistemYoneticisimi(kullanan) ||
                Yetki.MuhasebeYoneticisimi(kullanan) ||
                kullanan.KullaniciTipiIceriyorMu(18)))//Ödeme Emri Yetkisi
            {
                sonuc.hataStr = ButceMuhasebeResI.MUH080;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            else
            {
                foreach (OdenekIhtiyac m in ihtiyac.objeler)
                {
                    if (!TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, m.muhasebe.ToString(), m.birim))
                    {
                        sonuc.hataStr = ButceMuhasebeResI.MUH081;
                        sonuc.islemSonuc = false;
                        return sonuc;
                    }
                    else
                        break;//İlk satırda gelen muhasebe kodu ile diğerleri aynı olduğu varsayılıyor.
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABOdenekIhtiyac tab = new TABOdenekIhtiyac(ve);

            OdenekIhtiyac s = new OdenekIhtiyac();
            s = (OdenekIhtiyac)ihtiyac.objeler[0];
            tab.Sil(s);

            foreach (OdenekIhtiyac m in ihtiyac.objeler)
            {
                string hata = string.Empty;
                if (m.yil <= 0)
                    hata = ButceMuhasebeResI.MUH082 + "<br>";
                if (m.muhasebe <= 0)
                    hata += ButceMuhasebeResI.MUH083 + "<br>";
                if (string.IsNullOrEmpty(m.birim))
                    hata += ButceMuhasebeResI.MUH184 + "<br>";

                if (!string.IsNullOrEmpty(hata))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = hata;
                    break;
                }

                sonuc = tab.Kaydet(m);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH135;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Ödenek ihtiyaç listele.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="ihtiyac">ihtiyac</param>
        /// <returns></returns>
        public ObjectArray Listele(Kullanici kullanan, OdenekIhtiyac ihtiyac)
        {
            ObjectArray mler = new ObjectArray();

            if (!(Yetki.SistemYoneticisimi(kullanan) ||
                Yetki.MuhasebeYoneticisimi(kullanan) ||
                kullanan.KullaniciTipiIceriyorMu(18)))//Ödeme Emri Yetkisi
            {
                mler.sonuc.hataStr = ButceMuhasebeResI.MUH080;
                mler.sonuc.islemSonuc = false;
                return mler;
            }
            else
            {
                if (!TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, ihtiyac.muhasebe.ToString(), ihtiyac.birim))
                {
                    mler.sonuc.hataStr = ButceMuhasebeResI.MUH081;
                    mler.sonuc.islemSonuc = false;
                    return mler;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdenekIhtiyac tab = new TABOdenekIhtiyac(ve);

            IDataReader dr = tab.Listele(ihtiyac);

            while (dr.Read())
            {
                OdenekIhtiyac m = new OdenekIhtiyac();
                m.yil = VeriErisim.DegerAl(dr, 0, 0);
                m.muhasebe = VeriErisim.DegerAl(dr, 1, 0);
                m.birim = VeriErisim.DegerAl(dr, 2, "");
                m.kurKod = VeriErisim.DegerAl(dr, 3, "");
                m.fonKod = VeriErisim.DegerAl(dr, 4, "");
                m.finKod = VeriErisim.DegerAl(dr, 5, "");
                m.ekoKod = VeriErisim.DegerAl(dr, 6, "");
                m.harcama1 = VeriErisim.DegerAl(dr, 7, (double)0);
                m.harcama2 = VeriErisim.DegerAl(dr, 8, (double)0);
                m.harcama3 = VeriErisim.DegerAl(dr, 9, (double)0);
                m.odenek1 = VeriErisim.DegerAl(dr, 10, (double)0);
                m.odenek2 = VeriErisim.DegerAl(dr, 11, (double)0);
                m.odenek3 = VeriErisim.DegerAl(dr, 12, (double)0);
                m.odenek4 = VeriErisim.DegerAl(dr, 13, (double)0);
                m.aciklama = VeriErisim.DegerAl(dr, 14, "");
                m.ekoAd = VeriErisim.DegerAl(dr, 15, "");

                m.kurKod = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(0), m.kurKod);
                m.fonKod = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(1), m.fonKod);
                m.finKod = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(2), m.finKod);
                m.ekoKod = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(3), m.ekoKod);

                mler.objeler.Add(m);
            }
            dr.Close();

            if (mler.objeler.Count == 0)
            {
                ihtiyac.ekoKod = "03";

                //Listelenecek kurumsal kodları bul
                TABMuhasebeBirim mbirim = new TABMuhasebeBirim(ve);
                MuhasebeBirim mb = new MuhasebeBirim();
                mb.kurum = TNS.MUH.Yetki.VarsayilanKurumBul();
                mb.muhasebe = ihtiyac.muhasebe;
                mb.birimKod = ihtiyac.birim;

                IDataReader dr2 = mbirim.Listele(mb);
                string kurumsallar = "";
                while (dr2.Read())
                {
                    if (kurumsallar != "") kurumsallar += ";";
                    kurumsallar += VeriErisim.DegerAl(dr2, 2, "");
                }
                dr2.Close();
                ////////////////////////////////////////////////

                //tertipleri doldurmak için
                dr2 = tab.ListeleIlk(ihtiyac, "EKOKODVER", kurumsallar);
                while (dr2.Read())
                {
                    OdenekIhtiyac m = new OdenekIhtiyac();
                    m.yil = ihtiyac.yil;
                    m.muhasebe = ihtiyac.muhasebe;
                    m.birim = ihtiyac.birim;
                    m.ilkKayit = true;
                    string kurKod = VeriErisim.DegerAl(dr2, 1, "");
                    string fonKod = VeriErisim.DegerAl(dr2, 2, "");
                    string finKod = VeriErisim.DegerAl(dr2, 3, "");
                    string ekoKod = VeriErisim.DegerAl(dr2, 4, "");
                    bool bulundu = false;

                    foreach (OdenekIhtiyac oi in mler.objeler)
                    {
                        if (oi.kurKod == kurKod && oi.fonKod == fonKod && oi.finKod == finKod && oi.ekoKod == ekoKod)
                        {
                            bulundu = true;
                            break;
                        }
                    }

                    if (!bulundu)
                    {
                        m.kurKod = kurKod;
                        m.fonKod = fonKod;
                        m.finKod = finKod;
                        m.ekoKod = ekoKod;
                        mler.objeler.Add(m);
                    }
                }

                dr2.Close();

                //KBO doldur
                dr2 = tab.ListeleKBO(ihtiyac, kurumsallar);

                while (dr2.Read())
                {
                    OdenekIhtiyac m = new OdenekIhtiyac();
                    m.yil = ihtiyac.yil;
                    m.muhasebe = ihtiyac.muhasebe;
                    m.birim = ihtiyac.birim;
                    m.ilkKayit = true;
                    string kurKod = VeriErisim.DegerAl(dr2, 1, "");
                    string fonKod = VeriErisim.DegerAl(dr2, 2, "");
                    string finKod = VeriErisim.DegerAl(dr2, 3, "");
                    string ekoKod = VeriErisim.DegerAl(dr2, 4, "");
                    bool bulundu = false;

                    foreach (OdenekIhtiyac oi in mler.objeler)
                    {
                        if (oi.kurKod == kurKod && oi.fonKod == fonKod && oi.finKod == finKod && oi.ekoKod == ekoKod)
                        {
                            oi.kbo = VeriErisim.DegerAl(dr2, 5, 0.0);
                            bulundu = true;
                            break;
                        }
                    }

                    if (!bulundu)
                    {
                        m.kurKod = kurKod;
                        m.fonKod = fonKod;
                        m.finKod = finKod;
                        m.ekoKod = ekoKod;
                        m.kbo = VeriErisim.DegerAl(dr2, 5, 0.0);
                        m.harcama1 = 0;
                        m.harcama2 = 0;
                        m.harcama3 = 0;
                        m.odenek1 = 0;
                        m.odenek2 = 0;
                        m.odenek3 = 0;
                        m.odenek4 = 0;
                        mler.objeler.Add(m);
                    }
                }

                dr2.Close();

                //Eski yıl sonu harcamalarını doldur
                dr2 = tab.ListeleHarcama(ihtiyac, kurumsallar);

                while (dr2.Read())
                {
                    int yil = VeriErisim.DegerAl(dr2, 0, 0);
                    int ay = VeriErisim.DegerAl(dr2, 1, 0);
                    int muh = VeriErisim.DegerAl(dr2, 2, 0);
                    string birim = VeriErisim.DegerAl(dr2, 3, "");
                    string kurKod = VeriErisim.DegerAl(dr2, 4, "");
                    string fonKod = VeriErisim.DegerAl(dr2, 5, "");
                    string finKod = VeriErisim.DegerAl(dr2, 6, "");
                    string ekoKod = VeriErisim.DegerAl(dr2, 7, "");
                    double harcama = VeriErisim.DegerAl(dr2, 8, 0.0);
                    double harcamaYil = VeriErisim.DegerAl(dr2, 9, 0.0);

                    foreach (OdenekIhtiyac oi in mler.objeler)
                    {
                        if (oi.kurKod == kurKod && oi.fonKod == fonKod && oi.finKod == finKod && oi.ekoKod == ekoKod)
                        {
                            if (ay == 1) oi.harcamaAy1 += harcama;
                            if (ay == 2) oi.harcamaAy2 += harcama;
                            if (ay == 3) oi.harcamaAy3 += harcama;
                            if (ay == 4) oi.harcamaAy4 += harcama;
                            if (ay == 5) oi.harcamaAy5 += harcama;
                            if (ay == 6) oi.harcamaAy6 += harcama;
                            if (ay == 7) oi.harcamaAy7 += harcama;
                            if (ay == 8) oi.harcamaAy8 += harcama;
                            if (ay == 9) oi.harcamaAy9 += harcama;
                            if (ay == 10) oi.harcamaAy10 += harcama;
                            if (ay == 11) oi.harcamaAy11 += harcama;
                            if (ay == 12) oi.harcamaAy12 += harcama;

                            if (ay == 12)
                            {
                                if (yil == oi.yil - 3)
                                {
                                    oi.harcama1 += harcama;
                                    oi.harcamaYil1 += harcamaYil;
                                }
                                if (yil == oi.yil - 2)
                                {
                                    oi.harcama2 += harcama;
                                    oi.harcamaYil2 += harcamaYil;
                                }
                                if (yil == oi.yil - 1)
                                {
                                    oi.harcama3 += harcama;
                                    oi.harcamaYil3 += harcamaYil;
                                }
                            }
                            break;
                        }
                    }
                }
                dr2.Close();

                foreach (OdenekIhtiyac oi in mler.objeler)
                {
                    //Aylık harcamaları bul
                    oi.harcamaAy12 = oi.harcamaAy12 - oi.harcamaAy11;
                    oi.harcamaAy11 = oi.harcamaAy11 - oi.harcamaAy10;
                    oi.harcamaAy10 = oi.harcamaAy10 - oi.harcamaAy9;
                    oi.harcamaAy9 = oi.harcamaAy9 - oi.harcamaAy8;
                    oi.harcamaAy8 = oi.harcamaAy8 - oi.harcamaAy7;
                    oi.harcamaAy7 = oi.harcamaAy7 - oi.harcamaAy6;
                    oi.harcamaAy6 = oi.harcamaAy6 - oi.harcamaAy5;
                    oi.harcamaAy5 = oi.harcamaAy5 - oi.harcamaAy4;
                    oi.harcamaAy4 = oi.harcamaAy4 - oi.harcamaAy3;
                    oi.harcamaAy3 = oi.harcamaAy3 - oi.harcamaAy2;
                    oi.harcamaAy2 = oi.harcamaAy2 - oi.harcamaAy1;

                    double harcamaYillikToplam = oi.harcamaYil1 + oi.harcamaYil2 + oi.harcamaYil3;
                    double oran1 = (oi.harcamaAy1 + oi.harcamaAy2 + oi.harcamaAy3) / harcamaYillikToplam;
                    double oran2 = (oi.harcamaAy4 + oi.harcamaAy5 + oi.harcamaAy6) / harcamaYillikToplam;
                    double oran3 = (oi.harcamaAy7 + oi.harcamaAy8 + oi.harcamaAy9) / harcamaYillikToplam;
                    double oran4 = (oi.harcamaAy10 + oi.harcamaAy11 + oi.harcamaAy12) / harcamaYillikToplam;

                    if (harcamaYillikToplam > 0)
                    {
                        oi.odenek1 = oi.kbo * oran1;
                        oi.odenek2 = oi.kbo * oran2;
                        oi.odenek3 = oi.kbo * oran3;
                        oi.odenek4 = oi.kbo * oran4;
                    }
                    else
                    {
                        oi.odenek1 = 0;
                        oi.odenek2 = 0;
                        oi.odenek3 = 0;
                        oi.odenek4 = 0;
                    }
                    oi.kurKod = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(0), oi.kurKod);
                    oi.fonKod = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(1), oi.fonKod);
                    oi.finKod = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(2), oi.finKod);
                    oi.ekoKod = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(3), oi.ekoKod);
                }

                mler.objeler.Sort(new Sirala());
            }

            ve.VeriErisimKapat();
            return mler;
        }

        class Sirala : System.Collections.IComparer
        {
            public int Compare(object o1, object o2)
            {
                if (o1 == o2) return 0;
                OdenekIhtiyac i_o1 = (OdenekIhtiyac)o1;
                OdenekIhtiyac i_o2 = (OdenekIhtiyac)o2;
                return (i_o1.kurKod + i_o1.fonKod + i_o1.finKod + i_o1.ekoKod).CompareTo((i_o2.kurKod + i_o2.fonKod + i_o2.finKod + i_o2.ekoKod));
            }
        }

        /// <summary>
        /// Ödenek ihtiyaç icmal olarak listele.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="ihtiyac">ihtiyac</param>
        /// <returns></returns>
        public ObjectArray ListeleIcmal(Kullanici kullanan, OdenekIhtiyac ihtiyac)
        {
            ObjectArray mler = new ObjectArray();

            if (!(Yetki.SistemYoneticisimi(kullanan) ||
                Yetki.MuhasebeYoneticisimi(kullanan) ||
                kullanan.KullaniciTipiIceriyorMu(18)))//Ödeme Emri Yetkisi
            {
                mler.sonuc.hataStr = ButceMuhasebeResI.MUH080;
                mler.sonuc.islemSonuc = false;
                return mler;
            }
            else
            {
                if (!TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, ihtiyac.muhasebe.ToString(), ihtiyac.birim))
                {
                    mler.sonuc.hataStr = ButceMuhasebeResI.MUH081;
                    mler.sonuc.islemSonuc = false;
                    return mler;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdenekIhtiyac tab = new TABOdenekIhtiyac(ve);

            IDataReader dr = tab.ListeleIcmal(ihtiyac);

            while (dr.Read())
            {
                OdenekIhtiyac m = new OdenekIhtiyac();
                m.yil = VeriErisim.DegerAl(dr, 0, 0);
                m.muhasebe = VeriErisim.DegerAl(dr, 1, 0);
                m.kurKod = VeriErisim.DegerAl(dr, 2, "");
                m.fonKod = VeriErisim.DegerAl(dr, 3, "");
                m.finKod = VeriErisim.DegerAl(dr, 4, "");
                m.ekoKod = VeriErisim.DegerAl(dr, 5, "");
                m.harcama1 = VeriErisim.DegerAl(dr, 6, (double)0);
                m.harcama2 = VeriErisim.DegerAl(dr, 7, (double)0);
                m.harcama3 = VeriErisim.DegerAl(dr, 8, (double)0);
                m.odenek1 = VeriErisim.DegerAl(dr, 9, (double)0);
                m.odenek2 = VeriErisim.DegerAl(dr, 10, (double)0);
                m.odenek3 = VeriErisim.DegerAl(dr, 11, (double)0);
                m.odenek4 = VeriErisim.DegerAl(dr, 12, (double)0);
                m.ekoAd = VeriErisim.DegerAl(dr, 13, "");

                m.kurKod = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(0), m.kurKod);
                m.fonKod = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(1), m.fonKod);
                m.finKod = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(2), m.finKod);
                m.ekoKod = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(3), m.ekoKod);

                mler.objeler.Add(m);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return mler;
        }
    }
}