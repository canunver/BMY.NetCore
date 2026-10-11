using System.Data;
using TNS.KYM;
using TNS.BKD;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;
using System;
using System.Collections;

namespace TNS.TRM.IsKurallari
{
    public class TRMRapor
    {

        public ObjectArray HibeMakineIlBazindaIcmal(Kullanici kullanan, HibeMakine kriter, bool ilListele)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABRapor to = new TABRapor(ve);
            IDataReader r;
            if (ilListele)
                r = to.HibeMakineIlBazindaIcmal(kriter);
            else
                r = to.HibeMakineIlceBazindaIcmal(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    IlBazindaIcmal f = new IlBazindaIcmal();

                    f.ilKod = VeriErisim.DegerAl(r, 0, "");
                    f.ilAdi = VeriErisim.DegerAl(r, 1, "");
                    f.koordinat = VeriErisim.DegerAl(r, 2, "");
                    f.proforma = VeriErisim.DegerAl(r, 3, 0);
                    f.hibe = VeriErisim.DegerAl(r, 4, 0);
                    f.adet = VeriErisim.DegerAl(r, 5, 0);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public ObjectArray HibeSulamaIlBazindaIcmal(Kullanici kullanan, HibeSulama kriter, bool ilListele)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABRapor to = new TABRapor(ve);
            IDataReader r;
            if (ilListele)
                r = to.HibeSulamaIlBazindaIcmal(kriter);
            else
                r = to.HibeSulamaIlceBazindaIcmal(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    IlBazindaIcmal f = new IlBazindaIcmal();

                    f.ilKod = VeriErisim.DegerAl(r, 0, "");
                    f.ilAdi = VeriErisim.DegerAl(r, 1, "");
                    f.koordinat = VeriErisim.DegerAl(r, 2, "");
                    f.proforma = VeriErisim.DegerAl(r, 3, (decimal)0);
                    f.hibe = VeriErisim.DegerAl(r, 4, (decimal)0);
                    f.adet = VeriErisim.DegerAl(r, 5, 0);
                    f.sulanacakAlan = VeriErisim.DegerAl(r, 6, (double)0);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public ObjectArray HibeYatirimIlBazindaIcmal(Kullanici kullanan, HibeYatirim kriter, bool ilListele)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABRapor to = new TABRapor(ve);
            IDataReader r;
            if (ilListele)
                r = to.HibeYatirimIlBazindaIcmal(kriter);
            else
                r = to.HibeYatirimIlceBazindaIcmal(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    IlBazindaIcmal f = new IlBazindaIcmal();

                    f.ilKod = VeriErisim.DegerAl(r, 0, "");
                    f.ilAdi = VeriErisim.DegerAl(r, 1, "");
                    f.koordinat = VeriErisim.DegerAl(r, 2, "");
                    f.proforma = VeriErisim.DegerAl(r, 3, 0);
                    f.hibe = VeriErisim.DegerAl(r, 4, 0);
                    f.adet = VeriErisim.DegerAl(r, 5, 0);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public ObjectArray MalzemeFiyatAnalizi(Kullanici kullanan, MalzemeFiyatAnalizi mfa)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABRapor to = new TABRapor(ve);
            IDataReader r = to.MalzemeFiyatAnalizi(mfa);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    MalzemeFiyatAnalizi m = new MalzemeFiyatAnalizi();
                    m.etapNo = VeriErisim.DegerAl(r, 0, 0);
                    //m.ilKod = VeriErisim.DegerAl(r, 1, "");
                    m.malzemeGrupKod = VeriErisim.DegerAl(r, 1, 0);
                    m.malzemeKod = VeriErisim.DegerAl(r, 2, 0);
                    m.referansTutar = VeriErisim.DegerAl(r, 3, 0);
                    m.dusukTutar = VeriErisim.DegerAl(r, 4, 0);  //Düşük
                    m.yuksekTutar = VeriErisim.DegerAl(r, 5, 0); // Yüksek
                    m.adet = VeriErisim.DegerAl(r, 6, 0);  //Adet
                    m.malzemeGrupAd = VeriErisim.DegerAl(r, 7, "");
                    m.malzemeAd = VeriErisim.DegerAl(r, 8, "");
                    m.ilAd = VeriErisim.DegerAl(r, 9, "");

                    bilgi.objeler.Add(m);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public ArrayList IlListesi(Kullanici kullanan, int ilTur)
        {
            ArrayList al = new ArrayList();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABRapor to = new TABRapor(ve);
            IDataReader r = to.IlListesi(ilTur);
            int i = 0;

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    al.Add(new object[] { VeriErisim.DegerAl(r, 0, ""), VeriErisim.DegerAl(r, 1, ""), VeriErisim.DegerAl(r, 2, "") });
                    i++;
                }
            }

            return al;
        }

        public YatirimIzlemeCetveli[] YatirimIzlemeCetveliListesi(Kullanici kullanan, YatirimIzlemeCetveli[] izlemeCetveli, int etapNo, int ilTur, int konuTur)
        {
            ObjectArray[] listeList = new ObjectArray[8];
            listeList[0] = YatirimIzlemeCetveli(kullanan, etapNo, ENUMYatirimRaporAsama.BASVURU, ilTur, konuTur);
            listeList[1] = YatirimIzlemeCetveli(kullanan, etapNo, ENUMYatirimRaporAsama.ILKABUL, ilTur, konuTur);
            listeList[2] = YatirimIzlemeCetveli(kullanan, etapNo, ENUMYatirimRaporAsama.MERKEZKABUL, ilTur, konuTur);
            listeList[3] = YatirimIzlemeCetveli(kullanan, etapNo, ENUMYatirimRaporAsama.SOZLESMEYAPILAN, ilTur, konuTur);
            listeList[4] = YatirimIzlemeCetveli(kullanan, etapNo, ENUMYatirimRaporAsama.SOZLESMESIDEVAMEDEN, ilTur, konuTur);
            listeList[5] = YatirimIzlemeCetveli(kullanan, etapNo, ENUMYatirimRaporAsama.SOZLESMESIBITEN, ilTur, konuTur);
            listeList[6] = YatirimIzlemeCetveli(kullanan, etapNo, ENUMYatirimRaporAsama.SOZLESMESIIPTALOLAN, ilTur, konuTur);
            listeList[7] = YatirimIzlemeCetveli(kullanan, etapNo, ENUMYatirimRaporAsama.HAKEDISODENEN, ilTur, konuTur);

            for (int i = 0; i < izlemeCetveli.Length; i++)
            {
                if (listeList[0].sonuc.islemSonuc && listeList[0].ObjeSayisi > 0)
                {
                    foreach (YatirimIzlemeCetveli c in listeList[0].objeler)
                    {
                        if (c.ilKod == izlemeCetveli[i].ilKod)
                        {
                            izlemeCetveli[i].basvuru = new YatirimSayisi();
                            izlemeCetveli[i].basvuru.adet = c.basvuru.adet;
                            izlemeCetveli[i].basvuru.projeToplamTutar = c.basvuru.projeToplamTutar;
                            izlemeCetveli[i].basvuru.hibeTutar = c.basvuru.hibeTutar;
                            break;
                        }
                    }
                }

                if (listeList[1].sonuc.islemSonuc && listeList[1].ObjeSayisi > 0)
                {
                    foreach (YatirimIzlemeCetveli c in listeList[1].objeler)
                    {
                        if (c.ilKod == izlemeCetveli[i].ilKod)
                        {
                            izlemeCetveli[i].ilKabul = new YatirimSayisi();
                            izlemeCetveli[i].ilKabul.adet = c.ilKabul.adet;
                            izlemeCetveli[i].ilKabul.projeToplamTutar = c.ilKabul.projeToplamTutar;
                            izlemeCetveli[i].ilKabul.hibeTutar = c.ilKabul.hibeTutar;
                            break;
                        }
                    }
                }

                if (listeList[2].sonuc.islemSonuc && listeList[2].ObjeSayisi > 0)
                {
                    foreach (YatirimIzlemeCetveli c in listeList[2].objeler)
                    {
                        if (c.ilKod == izlemeCetveli[i].ilKod)
                        {
                            izlemeCetveli[i].merkezKabul = new YatirimSayisi();
                            izlemeCetveli[i].merkezKabul.adet = c.merkezKabul.adet;
                            izlemeCetveli[i].merkezKabul.projeToplamTutar = c.merkezKabul.projeToplamTutar;
                            izlemeCetveli[i].merkezKabul.hibeTutar = c.merkezKabul.hibeTutar;
                            break;
                        }
                    }
                }

                if (listeList[3].sonuc.islemSonuc && listeList[3].ObjeSayisi > 0)
                {
                    foreach (YatirimIzlemeCetveli c in listeList[3].objeler)
                    {
                        if (c.ilKod == izlemeCetveli[i].ilKod)
                        {
                            izlemeCetveli[i].sozlesmeYapilan = new YatirimSayisi();
                            izlemeCetveli[i].sozlesmeYapilan.adet = c.sozlesmeYapilan.adet;
                            izlemeCetveli[i].sozlesmeYapilan.projeToplamTutar = c.sozlesmeYapilan.projeToplamTutar;
                            izlemeCetveli[i].sozlesmeYapilan.sozlesmehibeTutar = c.sozlesmeYapilan.sozlesmehibeTutar;
                            izlemeCetveli[i].sozlesmeYapilan.hibeTutar = c.sozlesmeYapilan.hibeTutar;
                            break;
                        }
                    }
                }

                if (listeList[4].sonuc.islemSonuc && listeList[4].ObjeSayisi > 0)
                {
                    foreach (YatirimIzlemeCetveli c in listeList[4].objeler)
                    {
                        if (c.ilKod == izlemeCetveli[i].ilKod)
                        {
                            izlemeCetveli[i].sozlesmesiDevamEden = new YatirimSayisi();
                            izlemeCetveli[i].sozlesmesiDevamEden.adet = c.sozlesmesiDevamEden.adet;
                            izlemeCetveli[i].sozlesmesiDevamEden.projeToplamTutar = c.sozlesmesiDevamEden.projeToplamTutar;
                            izlemeCetveli[i].sozlesmesiDevamEden.sozlesmehibeTutar = c.sozlesmesiDevamEden.sozlesmehibeTutar;
                            izlemeCetveli[i].sozlesmesiDevamEden.hibeTutar = c.sozlesmesiDevamEden.hibeTutar;
                            break;
                        }
                    }
                }

                if (listeList[5].sonuc.islemSonuc && listeList[5].ObjeSayisi > 0)
                {
                    foreach (YatirimIzlemeCetveli c in listeList[5].objeler)
                    {
                        if (c.ilKod == izlemeCetveli[i].ilKod)
                        {
                            izlemeCetveli[i].sozlesmesiBiten = new YatirimSayisi();
                            izlemeCetveli[i].sozlesmesiBiten.adet = c.sozlesmesiBiten.adet;
                            izlemeCetveli[i].sozlesmesiBiten.projeToplamTutar = c.sozlesmesiBiten.projeToplamTutar;
                            izlemeCetveli[i].sozlesmesiBiten.sozlesmehibeTutar = c.sozlesmesiBiten.sozlesmehibeTutar;
                            izlemeCetveli[i].sozlesmesiBiten.hibeTutar = c.sozlesmesiBiten.hibeTutar;
                            break;
                        }
                    }
                }

                if (listeList[6].sonuc.islemSonuc && listeList[6].ObjeSayisi > 0)
                {
                    foreach (YatirimIzlemeCetveli c in listeList[6].objeler)
                    {
                        if (c.ilKod == izlemeCetveli[i].ilKod)
                        {
                            izlemeCetveli[i].sozlesmesiIptalOlan = new YatirimSayisi();
                            izlemeCetveli[i].sozlesmesiIptalOlan.adet = c.sozlesmesiIptalOlan.adet;
                            izlemeCetveli[i].sozlesmesiIptalOlan.projeToplamTutar = c.sozlesmesiIptalOlan.projeToplamTutar;
                            izlemeCetveli[i].sozlesmesiIptalOlan.sozlesmehibeTutar = c.sozlesmesiIptalOlan.sozlesmehibeTutar;
                            izlemeCetveli[i].sozlesmesiIptalOlan.hibeTutar = c.sozlesmesiIptalOlan.hibeTutar;
                            break;
                        }
                    }
                }

                if (listeList[7].sonuc.islemSonuc && listeList[7].ObjeSayisi > 0)
                {
                    foreach (YatirimIzlemeCetveli c in listeList[7].objeler)
                    {
                        if (c.ilKod == izlemeCetveli[i].ilKod)
                        {
                            izlemeCetveli[i].hakedisiOdenen = new YatirimSayisi();
                            izlemeCetveli[i].hakedisiOdenen.hibeTutar = c.hakedisiOdenen.hibeTutar;
                            break;
                        }
                    }
                }
            }

            return izlemeCetveli;
        }

        public ObjectArray YatirimIzlemeCetveli(Kullanici kullanan, int etapNo, ENUMYatirimRaporAsama asama, int ilTur, int konuTur)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABRapor to = new TABRapor(ve);
            IDataReader r;

            if ((int)asama >= (int)ENUMYatirimRaporAsama.SOZLESMEYAPILAN)
            {
                if (asama == ENUMYatirimRaporAsama.HAKEDISODENEN)
                    r = to.YatirimIzlemeCetveliOdenenHakedis(etapNo, ilTur, konuTur);
                else
                    r = to.YatirimIzlemeCetveliDiger(etapNo, asama, ilTur, konuTur);
            }
            else
                r = to.YatirimIzlemeCetveli(etapNo, asama, ilTur, konuTur);

            int i = 0;

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    YatirimIzlemeCetveli cetvel = new TRM.YatirimIzlemeCetveli();
                    cetvel.ilKod = VeriErisim.DegerAl(r, 0, "");

                    switch (asama)
                    {
                        case ENUMYatirimRaporAsama.BASVURU:
                            {
                                cetvel.basvuru = new YatirimSayisi();
                                cetvel.basvuru.adet = VeriErisim.DegerAl(r, 1, 0);
                                cetvel.basvuru.projeToplamTutar = VeriErisim.DegerAl(r, 2, decimal.Zero);
                                cetvel.basvuru.hibeTutar = VeriErisim.DegerAl(r, 3, decimal.Zero);
                                break;
                            }
                        case ENUMYatirimRaporAsama.ILKABUL:
                            {
                                cetvel.ilKabul = new YatirimSayisi();
                                cetvel.ilKabul.adet = VeriErisim.DegerAl(r, 1, 0);
                                cetvel.ilKabul.projeToplamTutar = VeriErisim.DegerAl(r, 2, decimal.Zero);
                                cetvel.ilKabul.hibeTutar = VeriErisim.DegerAl(r, 3, decimal.Zero);
                                break;
                            }
                        case ENUMYatirimRaporAsama.MERKEZKABUL:
                            {
                                cetvel.merkezKabul = new YatirimSayisi();
                                cetvel.merkezKabul.adet = VeriErisim.DegerAl(r, 1, 0);
                                cetvel.merkezKabul.projeToplamTutar = VeriErisim.DegerAl(r, 2, decimal.Zero);
                                cetvel.merkezKabul.hibeTutar = VeriErisim.DegerAl(r, 3, decimal.Zero);
                                break;
                            }
                        case ENUMYatirimRaporAsama.SOZLESMEYAPILAN:
                            {
                                cetvel.sozlesmeYapilan = new YatirimSayisi();
                                cetvel.sozlesmeYapilan.adet = VeriErisim.DegerAl(r, 1, 0);
                                cetvel.sozlesmeYapilan.projeToplamTutar = VeriErisim.DegerAl(r, 2, decimal.Zero);
                                cetvel.sozlesmeYapilan.hibeTutar = VeriErisim.DegerAl(r, 3, decimal.Zero);
                                cetvel.sozlesmeYapilan.sozlesmehibeTutar = VeriErisim.DegerAl(r, 4, decimal.Zero);
                                break;
                            }
                        case ENUMYatirimRaporAsama.SOZLESMESIDEVAMEDEN:
                            {
                                cetvel.sozlesmesiDevamEden = new YatirimSayisi();
                                cetvel.sozlesmesiDevamEden.adet = VeriErisim.DegerAl(r, 1, 0);
                                cetvel.sozlesmesiDevamEden.projeToplamTutar = VeriErisim.DegerAl(r, 2, decimal.Zero);
                                cetvel.sozlesmesiDevamEden.hibeTutar = VeriErisim.DegerAl(r, 3, decimal.Zero);
                                cetvel.sozlesmesiDevamEden.sozlesmehibeTutar = VeriErisim.DegerAl(r, 4, decimal.Zero);
                                break;
                            }
                        case ENUMYatirimRaporAsama.SOZLESMESIBITEN:
                            {
                                cetvel.sozlesmesiBiten = new YatirimSayisi();
                                cetvel.sozlesmesiBiten.adet = VeriErisim.DegerAl(r, 1, 0);
                                cetvel.sozlesmesiBiten.projeToplamTutar = VeriErisim.DegerAl(r, 2, decimal.Zero);
                                cetvel.sozlesmesiBiten.hibeTutar = VeriErisim.DegerAl(r, 3, decimal.Zero);
                                cetvel.sozlesmesiBiten.sozlesmehibeTutar = VeriErisim.DegerAl(r, 4, decimal.Zero);
                                break;
                            }
                        case ENUMYatirimRaporAsama.SOZLESMESIIPTALOLAN:
                            {
                                cetvel.sozlesmesiIptalOlan = new YatirimSayisi();
                                cetvel.sozlesmesiIptalOlan.adet = VeriErisim.DegerAl(r, 1, 0);
                                cetvel.sozlesmesiIptalOlan.projeToplamTutar = VeriErisim.DegerAl(r, 2, decimal.Zero);
                                cetvel.sozlesmesiIptalOlan.hibeTutar = VeriErisim.DegerAl(r, 3, decimal.Zero);
                                cetvel.sozlesmesiIptalOlan.sozlesmehibeTutar = VeriErisim.DegerAl(r, 4, decimal.Zero);
                                break;
                            }
                        case ENUMYatirimRaporAsama.HAKEDISODENEN:
                            {
                                cetvel.hakedisiOdenen = new YatirimSayisi();
                                cetvel.hakedisiOdenen.hibeTutar = VeriErisim.DegerAl(r, 1, decimal.Zero);
                                break;
                            }
                    }

                    bilgi.objeler.Add(cetvel);
                    i++;
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public ObjectArray MakinaEkipmanDesteklemeleri(Kullanici kullanan, int etapNo, int odemeIlTur)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABRapor to = new TABRapor(ve);
            IDataReader r = to.MakinaEkipmanDesteklemeleri(etapNo, odemeIlTur);
            int i = 0;

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    MakineEkipmanDestek destek = new TRM.MakineEkipmanDestek();
                    destek.makinaAd = VeriErisim.DegerAl(r, 0, "");
                    destek.adet = VeriErisim.DegerAl(r, 1, 0);
                    destek.toplamTutar = VeriErisim.DegerAl(r, 2, decimal.Zero);

                    bilgi.objeler.Add(destek);
                    i++;
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public ObjectArray KKYDPOdeme(Kullanici kullanan, int etapNo, int odemeIlTur)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABRapor to = new TABRapor(ve);
            IDataReader r = to.KKYDPOdeme(etapNo, odemeIlTur);
            int i = 0;

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    MakineEkipmanDestek destek = new TRM.MakineEkipmanDestek();
                    destek.yil = VeriErisim.DegerAl(r, 0, 0);
                    destek.adet = VeriErisim.DegerAl(r, 1, 0);
                    destek.toplamTutar = VeriErisim.DegerAl(r, 2, decimal.Zero);

                    bilgi.objeler.Add(destek);
                    i++;
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public ObjectArray EYHibeTutariGetir(Kullanici kullanan, int etapNo, int odemeIlTur)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABRapor to = new TABRapor(ve);
            IDataReader r = to.EYHibeTutariGetir(etapNo, odemeIlTur);
            int i = 0;

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    MakineEkipmanDestek destek = new TRM.MakineEkipmanDestek();
                    destek.yil = VeriErisim.DegerAl(r, 0, 0);
                    destek.toplamHibeTutar = VeriErisim.DegerAl(r, 1, decimal.Zero);

                    bilgi.objeler.Add(destek);
                    i++;
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public string PlakaGetir(VeriErisim ve, string ilKod)
        {
            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);

            return new TABGenel(ve).IlPlakaKodBul(ilKod);
        }

        public ArrayList MakineSulamaOdenen(Kullanici kullanan, int etapNo, string ilKod, TNSDateTime t1, TNSDateTime t2)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            ArrayList list = new ArrayList();
            TABRapor to = new TABRapor(ve);
            IDataReader r = to.MakineSulamaToplamOdenen(etapNo, ilKod, 1, t1, t2);

            if (to.Hata() == "")
            {
                if (r.Read())
                    list.Add(new object[] { VeriErisim.DegerAl(r, 0, 0), VeriErisim.DegerAl(r, 1, decimal.Zero) });
                r.Dispose();
                r = to.MakineSulamaToplamOdenen(etapNo, ilKod, 2, t1, t2);

                if (to.Hata() == "")
                {
                    if (r.Read())
                        list.Add(new object[] { VeriErisim.DegerAl(r, 0, 0), VeriErisim.DegerAl(r, 1, decimal.Zero), VeriErisim.DegerAl(r, 2, decimal.Zero) });
                }
            }

            r.Close();
            ve.VeriErisimKapat();

            return list;
        }

        public ArrayList HibeYatirimGenelDurum(Kullanici kullanan, int etapNo, string ilKod, TNSDateTime t1, TNSDateTime t2)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            ArrayList list = new ArrayList();
            TABRapor to = new TABRapor(ve);
            IDataReader r = to.HibeYatirimGenelDurum(etapNo, ilKod, t1, t2);

            if (to.Hata() == "")
            {
                int bittiSayi = 0;
                decimal bittiTutar = 0, devamTutar = 0;

                while (r.Read())
                {
                    bittiSayi += VeriErisim.DegerAl(r, 0, 0);
                    bittiTutar += VeriErisim.DegerAl(r, 1, decimal.Zero);
                    devamTutar += VeriErisim.DegerAl(r, 2, decimal.Zero);
                }
                list.Add(new object[] { OrtakFonksiyonlar.ConvertToDbl(devamTutar) / 1.002,
                                        OrtakFonksiyonlar.ConvertToDbl(bittiTutar)/1.002,
                                        bittiSayi });

            }
            r.Close();
            ve.VeriErisimKapat();

            return list;
        }

        public ArrayList TBasincliSulamaGenelDurum(Kullanici kullanan, int etapNo, string ilKod, TNSDateTime t1, TNSDateTime t2)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            ArrayList list = new ArrayList();
            TABRapor to = new TABRapor(ve);
            IDataReader r = to.TBasincliSulamaGenelDurum(etapNo, ilKod, t1, t2);

            if (to.Hata() == "")
            {
                if (r.Read())
                    list.Add(new object[] { VeriErisim.DegerAl(r, 0, 0), VeriErisim.DegerAl(r, 1, decimal.Zero) });

            }
            r.Close();
            ve.VeriErisimKapat();

            return list;
        }

        public ObjectArray KirsalKalkinmaYatirimTesisMakineSulamaOdenek(Kullanici kullanan, int etapNo, string ilKod, TNSDateTime t1, TNSDateTime t2)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABRapor to = new TABRapor(ve);
            int aktifEtapNo = IsKurallari.TRMHibeYatirimYonetim.HibeYatirimAktifEtapNo(ref ve);
            IDataReader r = to.KirsalKalkinmaYatirimTesisMakineSulamaOdenek(etapNo, ilKod, aktifEtapNo, t1, t2);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    YatirimMakineSulamaOdenek odenek = new TRM.YatirimMakineSulamaOdenek();
                    odenek.projeKonuKod = VeriErisim.DegerAl(r, 0, "");
                    odenek.projeKonuAd = VeriErisim.DegerAl(r, 1, "");
                    odenek.devamEdenSozlesmeSayisi = VeriErisim.DegerAl(r, 2, 0);
                    odenek.devamEdenTutar = VeriErisim.DegerAl(r, 3, decimal.Zero);
                    odenek.bitenSozlesmeSayisi = VeriErisim.DegerAl(r, 4, 0);
                    odenek.bitenTutar = VeriErisim.DegerAl(r, 5, decimal.Zero);
                    odenek.istihdam = VeriErisim.DegerAl(r, 6, 0);
                    odenek.kistihdam = VeriErisim.DegerAl(r, 7, 0);
                    odenek.oncekiEtapIptalEdilenSayi = VeriErisim.DegerAl(r, 8, 0);
                    odenek.oncekiEtap = aktifEtapNo - 1;

                    //Komisyonların düşülmüş hali
                    odenek.bitenTutar = odenek.bitenTutar / OrtakFonksiyonlar.ConvertToDecimal(1.002);
                    odenek.devamEdenTutar = odenek.devamEdenTutar / OrtakFonksiyonlar.ConvertToDecimal(1.002);

                    bilgi.objeler.Add(odenek);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public ObjectArray KKYDPProjelerMandaAyri(Kullanici kullanan, int etapNo, string ilKod, TNSDateTime t1, TNSDateTime t2)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABRapor to = new TABRapor(ve);
            IDataReader r = to.KKYDPProjelerMandaAyri(etapNo, ilKod, t1, t2);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    YatirimMakineSulamaOdenek odenek = new TRM.YatirimMakineSulamaOdenek();
                    odenek.projeKonuKod = VeriErisim.DegerAl(r, 0, "");
                    odenek.projeKonuAd = VeriErisim.DegerAl(r, 1, "");
                    odenek.devamEdenSozlesmeSayisi = VeriErisim.DegerAl(r, 2, 0);
                    odenek.devamEdenTutar = VeriErisim.DegerAl(r, 3, decimal.Zero);
                    odenek.bitenSozlesmeSayisi = VeriErisim.DegerAl(r, 4, 0);
                    odenek.bitenTutar = VeriErisim.DegerAl(r, 5, decimal.Zero);

                    //Komisyonların düşülmüş hali
                    odenek.bitenTutar = odenek.bitenTutar / OrtakFonksiyonlar.ConvertToDecimal(1.002);
                    odenek.devamEdenTutar = odenek.devamEdenTutar / OrtakFonksiyonlar.ConvertToDecimal(1.002);

                    bilgi.objeler.Add(odenek);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public ObjectArray KKYDPIlTurBazliProjeler(Kullanici kullanan, int etapNo, int ilTur)
        {
            ObjectArray bilgi = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABRapor to = new TABRapor(ve);
            int aktifEtapNo = IsKurallari.TRMHibeYatirimYonetim.HibeYatirimAktifEtapNo(ref ve);
            IDataReader r = to.KKYDPIlTurBazliProjeler(etapNo, ilTur, aktifEtapNo);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    YatirimMakineSulamaOdenek odenek = new TRM.YatirimMakineSulamaOdenek();
                    odenek.ilAd = VeriErisim.DegerAl(r, 0, "");
                    odenek.projeKonuAd = VeriErisim.DegerAl(r, 1, "");
                    odenek.bitenSozlesmeSayisi = VeriErisim.DegerAl(r, 2, 0);
                    odenek.bitenTutar = VeriErisim.DegerAl(r, 3, decimal.Zero);
                    odenek.istihdam = VeriErisim.DegerAl(r, 4, 0);
                    odenek.devamEdenSozlesmeSayisi = VeriErisim.DegerAl(r, 5, 0);
                    odenek.devamEdenTutar = VeriErisim.DegerAl(r, 6, decimal.Zero);
                    odenek.iptalEdilenSayi = VeriErisim.DegerAl(r, 7, 0);
                    odenek.oncekiEtapIptalEdilenSayi = VeriErisim.DegerAl(r, 8, 0);
                    odenek.oncekiEtap = aktifEtapNo - 1;

                    //Komisyonların düşülmüş hali
                    odenek.bitenTutar = odenek.bitenTutar / OrtakFonksiyonlar.ConvertToDecimal(1.002);
                    odenek.devamEdenTutar = odenek.devamEdenTutar / OrtakFonksiyonlar.ConvertToDecimal(1.002);

                    bilgi.Ekle(odenek);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public ObjectArray KKYDPIlTurBazliBitenProjeler(Kullanici kullanan, int etapNo, int ilTur)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABRapor to = new TABRapor(ve);
            IDataReader r = to.KKYDPIlTurBazliProjeler(etapNo, ilTur, 0);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    YatirimMakineSulamaOdenek odenek = new TRM.YatirimMakineSulamaOdenek();
                    odenek.ilAd = VeriErisim.DegerAl(r, 0, "");
                    odenek.projeKonuAd = VeriErisim.DegerAl(r, 1, "");
                    odenek.bitenSozlesmeSayisi = VeriErisim.DegerAl(r, 2, 0);
                    odenek.bitenTutar = VeriErisim.DegerAl(r, 3, decimal.Zero);

                    //Komisyonların düşülmüş hali
                    odenek.bitenTutar = odenek.bitenTutar / OrtakFonksiyonlar.ConvertToDecimal(1.002);

                    bilgi.objeler.Add(odenek);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public ObjectArray ProjeNiteligineGoreHibeTutarDagilimi(Kullanici kullanan, int etapNo, string ilKod, TNSDateTime t1, TNSDateTime t2)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABRapor to = new TABRapor(ve);
            int aktifEtapNo = IsKurallari.TRMHibeYatirimYonetim.HibeYatirimAktifEtapNo(ref ve);
            IDataReader r = to.ProjeNiteligineGoreHibeTutarDagilimi(etapNo, ilKod, aktifEtapNo, t1, t2);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    YatirimMakineSulamaOdenek odenek = new TRM.YatirimMakineSulamaOdenek();
                    odenek.projeKonuKod = VeriErisim.DegerAl(r, 0, 0).ToString();
                    odenek.projeKonuAd = VeriErisim.DegerAl(r, 1, "");
                    odenek.devamEdenSozlesmeSayisi = VeriErisim.DegerAl(r, 2, 0);
                    odenek.devamEdenTutar = VeriErisim.DegerAl(r, 3, decimal.Zero);
                    odenek.bitenSozlesmeSayisi = VeriErisim.DegerAl(r, 4, 0);
                    odenek.bitenTutar = VeriErisim.DegerAl(r, 5, decimal.Zero);
                    odenek.oncekiEtapIptalEdilenSayi = VeriErisim.DegerAl(r, 6, 0);

                    //Komisyonların düşülmüş hali
                    odenek.bitenTutar = odenek.bitenTutar / OrtakFonksiyonlar.ConvertToDecimal(1.002);
                    odenek.devamEdenTutar = odenek.devamEdenTutar / OrtakFonksiyonlar.ConvertToDecimal(1.002);

                    bilgi.objeler.Add(odenek);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public ObjectArray IlceListele(Kullanici kullanan, string ilAdi, string ilceAdi)
        {
            ObjectArray bilgi = new ObjectArray();

            string sql = "";
            string ilce = "";
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTUI");
            IDataReader r;

            sql = "select DISTINCT t.[ILCE ADI] from TARLA t where UPPER(t.[IL ADI])='" + ilAdi + "' ";
            sql += (string.IsNullOrEmpty(ilceAdi)) ? "" : (" and t.[ILCE ADI]='" + ilceAdi + "' ");
            sql += " order by t.[ILCE ADI]  ";
            r = ve.DataReaderDondur(sql);

            while (r.Read())
            {
                ilce = r[0].ToString();
                bilgi.objeler.Add(ilce);
            }

            r.Close();

            ve.VeriErisimKapat();
            return bilgi;
        }

        public double IlceNormListele(Kullanici kullanan, string ilAdi, string ilceAdi, int tip)
        {
            string sql = "";
            double norm = 0;
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTUI");
            IDataReader r;

            sql = "select t.[SULU NORM], t.[KURU NORM] from ILLER2 t where UPPER(t.[IL ADI])='" + ilAdi + "' ";
            sql += (string.IsNullOrEmpty(ilceAdi)) ? "" : (" and t.[ILCE ADI]='" + ilceAdi + "' ");
            sql += " order by t.[ILCE ADI]  ";
            r = ve.DataReaderDondur(sql);

            if (r.Read())
            {
                if (tip == 0)
                {
                    norm = (double)r.GetDecimal(0);
                }
                else
                {
                    norm = (double)r.GetDecimal(1);
                }
            }

            r.Close();

            ve.VeriErisimKapat();
            return norm;
        }

        public ObjectArray IlListele(Kullanici kullanan)
        {
            ObjectArray bilgi = new ObjectArray();

            string sql = "";
            string il = "";
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTUI");
            IDataReader r;

            sql = "select DISTINCT t.[IL ADI] from TARLA t order by t.[IL ADI]  ";
            r = ve.DataReaderDondur(sql);

            while (r.Read())
            {
                il = r[0].ToString();
                bilgi.objeler.Add(il);
            }

            r.Close();

            ve.VeriErisimKapat();
            return bilgi;
        }

        public ObjectArray IlceBilgiListele(Kullanici kullanan, int tip, string ilAd, string ilce)
        {
            ObjectArray bilgi = new ObjectArray();

            TUIKveri veriler;
            string sql = "";
            string sqlek = "";

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTUI");
            IDataReader r;


            sql = " select YIL,t.[URUN ADI] , ISNULL(m.[SATIS FIYATI],0) ";

            if (tip == 0)
            {
                sqlek += " ,ISNULL(m.[ORTALAMA MALIYET SULU],0), ISNULL(m.[ARAZI KIRA KATSAYISI SULU],0), ISNULL(m.[AILE ISCILIGI SULU],0), ISNULL(m.[YABANCI ISCILIK SULU],0) , ISNULL(m.[DEGISEN MASRAF YUZDESI SULU],0) ";
            }
            else
            {
                sqlek += " ,ISNULL(m.[ORTALAMA MALIYET KURU],0), ISNULL(m.[ARAZI KIRA KATSAYISI KURU],0), ISNULL(m.[AILE ISCILIGI KURU],0), ISNULL(m.[YABANCI ISCILIK KURU],0) , ISNULL(m.[DEGISEN MASRAF YUZDESI KURU],0) ";
            }

            sql += sqlek;

            sql += " ,SUM(t.[EKİLEN ALAN (Dekar)]) ,SUM(t.[HASAT EDİLEN ALAN (Dekar)]) ";
            sql += " ,SUM(t.[ÜRETİM (Ton)]) ,AVG(t.[VERİM (Kg/Da)]) ";
            sql += " from TARLA t ";
            sql += " LEFT JOIN URUNMALIYET m ON t.[URUN ADI]=m.[URUN ADI]";
            sql += " where UPPER(t.[IL ADI])='" + ilAd + "' ";
            sql += " and t.[ILCE ADI]='" + ilce + "' and YIL=2013 ";

            sql += " and ((UPPER([URUN ACIKLAMA]) NOT LIKE  '%PAMUK%KÜTLÜ%'  AND ";
            sql += "  UPPER([URUN ACIKLAMA]) NOT LIKE  '%PAMUK%LİF%')) ";

            if (tip == 0)
            {
                sql += " and UPPER(t.[URUN ACIKLAMA]) LIKE '%SULU%' ";
            }
            else
            {
                sql += " and UPPER(t.[URUN ACIKLAMA]) NOT LIKE '%SULU%' ";
            }

            sql += " group by t.YIL,t.[URUN ADI],m.[SATIS FIYATI] ";
            sql += sqlek;
            sql += " order by SUM(t.[HASAT EDİLEN ALAN (Dekar)]) desc";
            sql += "";


            r = ve.DataReaderDondur(sql);

            while (r.Read())
            {
                veriler = new TUIKveri();
                veriler.Yil = r.GetDouble(0);
                //veriler.Yil = VeriErisim.DegerAl(r, 0, (double) 0.0);
                veriler.UrunAdi = VeriErisim.DegerAl(r, 1, "");

                veriler.SatisFiyati = r.GetDecimal(2);

                veriler.OrtalamaMaliyet = r.GetDecimal(3);
                veriler.AraziKatsayisi = r.GetDecimal(4);
                veriler.AileIsciligi = r.GetDecimal(5);
                veriler.YabanciIscilik = r.GetDecimal(6);

                veriler.DegiskenMasrafYuzdeSulu = r.GetDecimal(7);
                veriler.DegiskenMasrafYuzdeKuru = veriler.DegiskenMasrafYuzdeSulu;

                veriler.EkilenAlanDA = r.GetDouble(8);
                veriler.HasatAlanDA = r.GetDouble(9);
                veriler.UretimTON = r.GetDouble(10);
                veriler.VerimKG_DA = r.GetDouble(11);

                bilgi.objeler.Add(veriler);
            }

            r.Close();

            ve.VeriErisimKapat();
            return bilgi;
        }

        public ObjectArray lIlceGuncelle(Kullanici kullanan, string ilAd, string ilce, double sulunorm, double suluyeninorm, double kurunorm, double kuruyeninorm)
        {
            ObjectArray bilgi = new ObjectArray();

            string sql = "";

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTUI");

            sql += " update ILLER ";
            //sql += " set [SULU NORM]=" + Convert.ToInt32(Math.Round(sulunorm, 0)) + ",[KURU NORM]=" + Convert.ToInt32(Math.Round(kurunorm, 0)) + " ";
            sql += " set [SULU NORM]=" + Math.Round(sulunorm, 0) + ",[YENI SULU NORM]=" + Math.Round(suluyeninorm, 0) + ",[KURU NORM]=" + Math.Round(kurunorm, 0) + ",[YENI KURU NORM]=" + Math.Round(kuruyeninorm, 0) + " ";
            sql += " where UPPER([IL ADI])='" + ilAd + "' ";
            sql += " and [ILCE ADI]='" + ilce + "' ";

            ve.ExecuteNonquery(sql);


            ve.VeriErisimKapat();
            return bilgi;
        }

        public ObjectArray OzetListele(Kullanici kullanan)
        {
            ObjectArray bilgi = new ObjectArray();

            TUIKozetveri veriler;
            string sql = "";

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTUI");
            IDataReader r;


            sql = " SELECT m.[IL ADI],m.[ILCE ADI], ";
            sql += " m.[SULU NORM],m.[KURU NORM], ";
            sql += " m.[DIKILI NORM],m.[ORTUALTI NORM], ";
            sql += " SUM(d.[EKİLEN ALAN (Dekar)]),SUM(d.[ÜRETİM (Ton)]),SUM(d.[VERİM (Kg/Da)]) ";
            sql += " FROM ILLER AS m ";
            sql += " JOIN TARLA AS d ";
            sql += " ON m.[IL ADI] = d.[IL ADI] AND m.[ILCE ADI] = d.[ILCE ADI] ";
            sql += " GROUP BY m.[IL ADI],m.[ILCE ADI], ";
            sql += " m.[SULU NORM],m.[KURU NORM],m.[DIKILI NORM],m.[ORTUALTI NORM] ";
            sql += " ORDER BY m.[IL ADI],m.[ILCE ADI]  ";

            sql += "";


            r = ve.DataReaderDondur(sql);

            while (r.Read())
            {
                veriler = new TUIKozetveri();

                veriler.IlAdi = VeriErisim.DegerAl(r, 0, "");
                veriler.IlceAdi = VeriErisim.DegerAl(r, 1, "");

                veriler.NormSulu = VeriErisim.DegerAl(r, 2, 0);
                veriler.NormKuru = VeriErisim.DegerAl(r, 3, 0);
                veriler.NormDikili = VeriErisim.DegerAl(r, 4, 0);
                veriler.NormOrtuAlti = VeriErisim.DegerAl(r, 5, 0);

                veriler.EkilenAlanDA = r.GetDouble(6);
                veriler.UretimTON = r.GetDouble(7);
                veriler.VerimKG_DA = r.GetDouble(8);

                bilgi.objeler.Add(veriler);
            }

            r.Close();

            ve.VeriErisimKapat();
            return bilgi;
        }

    }
}
