using System;
using System.Data;
using TNS.BKD;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// Şablon işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHSablonServis
    {
        /// <summary>
        /// Sablon kaydet.
        /// </summary>
        /// <param name="sablon">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public Sonuc Kaydet(Sablon sablon)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            //sonuc=KontrolEt(kullanan, form);

            //if (sonuc.islemSonuc)
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));

                if (sablon.kod == 0)
                {
                    sablon.durum = 1;

                    //Belgeye numara alınıyor
                    //**********************************************************
                    TABGenel tg = new TABGenel(ve);

                    string sayacKod = sablon.yil.ToString();
                    sayacKod = "SABLON" + sayacKod;

                    sablon.kod = OrtakFonksiyonlar.ConvertToInt(tg.BelgeNoAl(sayacKod, "0"), 0);
                    //**********************************************************

                    //Çıkışta Sonuç Anahtar alanına konuluyor ki client tarafı yeni kayıt edilen belge numarasını alabilsin
                    //**********************************************************
                    if (sablon.kod == 0)
                    {
                        ve.VeriErisimKapat();
                        sonuc.hataStr = ButceMuhasebeResI.MUH007;
                        sonuc.islemSonuc = false;
                        return sonuc;
                    }
                    //**********************************************************
                }
                else
                    sablon.durum = 2;

                if (sablon.kod > 0)
                {
                    ve.BeginTransaction();
                    TABSablon to = new TABSablon(ve);

                    if (sablon.durum != 1)
                        sonuc = to.Sil(sablon.yil, sablon.kod);
                    else
                        sonuc.islemSonuc = true;

                    if (sonuc.islemSonuc)
                        sonuc = to.Kaydet(sablon);

                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        if (sablon.durum == 1)
                        {
                            sonuc.bilgiStr = string.Format(ButceMuhasebeResI.MUH187, sablon.ad);
                            sonuc.anahtar = sablon.kod.ToString();
                        }
                        else
                        {
                            sonuc.bilgiStr = string.Format(ButceMuhasebeResI.MUH188, sablon.ad);
                            sonuc.anahtar = sablon.kod.ToString();
                        }
                    }
                    else
                        ve.Rollback();
                }

                ve.VeriErisimKapat();
            }

            return sonuc;
        }

        /// <summary>
        /// Sablon sil.
        /// </summary>
        /// <param name="sablon">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public Sonuc Sil(Sablon sablon)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH068, "");

            if (sablon.kod > 0)
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
                ve.BeginTransaction();
                TABSablon to = new TABSablon(ve);

                sonuc = to.Sil(sablon.yil, sablon.kod);

                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();

                ve.VeriErisimKapat();
            }

            return sonuc;
        }

        /// <summary>
        /// Sablon listele.
        /// </summary>
        /// <param name="sablon">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public ObjectArray Liste(Sablon sablon)
        {
            ObjectArray sVAN = new ObjectArray();
            string hata = "";

            if (sablon.kod == 0 &&
                sablon.butceTur.Trim() == "" &&
                sablon.icerikTur == 0 &&
                sablon.islemYapan == "" &&
                sablon.gormeYetkisi == 0)
                hata += ButceMuhasebeResI.MUH189;

            if (hata != "")
            {
                sVAN.sonuc.hataStr = hata;
                sVAN.sonuc.islemSonuc = false;
                return sVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));

            TABSablon to = new TABSablon(ve);
            IDataReader r = to.Liste(sablon);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Sablon s = new Sablon();

                    s.kod = VeriErisim.DegerAl(r, 0, 0);
                    s.yil = VeriErisim.DegerAl(r, 1, 0);
                    s.butceTur = VeriErisim.DegerAl(r, 3, "");
                    s.icerikTur = VeriErisim.DegerAl(r, 4, 0);
                    s.ad = VeriErisim.DegerAl(r, 5, "");
                    s.durum = VeriErisim.DegerAl(r, 6, 0);
                    s.durumTarih = VeriErisim.DegerAl(r, 7);
                    s.islemYapan = VeriErisim.DegerAl(r, 8, "");
                    s.gormeYetkisi = VeriErisim.DegerAl(r, 9, 0);
                    s.kurum = VeriErisim.DegerAl(r, 10, "");
                    s.muhasebe = VeriErisim.DegerAl(r, 11, 0);
                    s.birim = VeriErisim.DegerAl(r, 12, "");
                    s.ilgiliAd = VeriErisim.DegerAl(r, 13, "");
                    s.ilgiliNo = VeriErisim.DegerAl(r, 14, "");
                    s.ilgiliBankaAd = VeriErisim.DegerAl(r, 15, "");
                    s.ilgiliBankaNo = VeriErisim.DegerAl(r, 16, "");
                    s.ilgiliVD = VeriErisim.DegerAl(r, 17, "");
                    s.aciklama = VeriErisim.DegerAl(r, 18, "");

                    if (sablon.detayVer)
                        ListeDetay(s);

                    sVAN.objeler.Add(s);
                }
                r.Close();
            }


            ve.VeriErisimKapat();

            return sVAN;
        }

        Sablon ListeDetay(Sablon sablon)
        {
            if (sablon.kod == 0)
                return sablon;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));

            TABSablon to = new TABSablon(ve);
            IDataReader r = to.ListeDetay(sablon);

            if (to.Hata() == "")
            {
                Tertip t = new Tertip(sablon.yil, ENUMTabloKod.TABPEBOdenek);
                Tertip th = new Tertip(sablon.yil, ENUMTabloKod.TABOdemeEmri);

                while (r.Read())
                {
                    SablonDetay sd = new SablonDetay();

                    sd.kod = VeriErisim.DegerAl(r, 0, 0);
                    sd.sira = VeriErisim.DegerAl(r, 1, 0);
                    sd.yil = VeriErisim.DegerAl(r, 2, 0);
                    sd.hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)th.Kirilimlar(0), VeriErisim.DegerAl(r, 3, ""));


                    sd.tertip.kodlar[0] = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), VeriErisim.DegerAl(r, 4, ""));
                    sd.tertip.kodlar[1] = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(1), VeriErisim.DegerAl(r, 5, ""));
                    sd.tertip.kodlar[2] = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(2), VeriErisim.DegerAl(r, 6, ""));
                    sd.tertip.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(3), VeriErisim.DegerAl(r, 7, ""));
                    sd.tertip.kodlar[4] = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(4), VeriErisim.DegerAl(r, 11, ""));

                    sd.borc = VeriErisim.DegerAl(r, 8, 0.0);
                    sd.alacak = VeriErisim.DegerAl(r, 9, 0.0);
                    sd.kesinti = VeriErisim.DegerAl(r, 10, 0);

                    sd.doviz = VeriErisim.DegerAl(r, 12, "");
                    sd.dovizKurTur = VeriErisim.DegerAl(r, 13, "");
                    sd.dovizBorc = VeriErisim.DegerAl(r, 14, 0.0);
                    sd.dovizAlacak = VeriErisim.DegerAl(r, 15, 0.0);
                    sd.dovizKur = VeriErisim.DegerAl(r, 16, (decimal)0.0);

                    sd.detayBirim = VeriErisim.DegerAl(r, 17, "");
                    sd.altHesapKod = VeriErisim.DegerAl(r, 18, "");
                    sd.satirAciklama = VeriErisim.DegerAl(r, 19, "");

                    sd.aciklama = VeriErisim.DegerAl(r, 20, "");

                    sablon.detay.objeler.Add(sd);
                }
                r.Close();
            }


            ve.VeriErisimKapat();

            return sablon;
        }

        /// <summary>
        /// Sablon icerik tür liste.
        /// </summary>
        /// <returns></returns>
        public ObjectArray IcerikTurListe()
        {
            ObjectArray sVAN = new ObjectArray();
            string hata = "";

            if (hata != "")
            {
                sVAN.sonuc.hataStr = hata;
                sVAN.sonuc.islemSonuc = false;
                return sVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));

            TABSablon to = new TABSablon(ve);
            IDataReader r = to.IcerikTurListe();

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    SablonIcerikTur tur = new SablonIcerikTur();

                    tur.kod = VeriErisim.DegerAl(r, 1, 0);
                    tur.ad = VeriErisim.DegerAl(r, 2, "");

                    sVAN.objeler.Add(tur);
                }
                r.Close();
            }


            ve.VeriErisimKapat();

            return sVAN;
        }
    }
}
