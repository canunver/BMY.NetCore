using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System;

namespace TNS.EVR.IsKurallari
{
    public class EVRDTVTBirim
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, DTVTBirim form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABDTVTBirim tab = new TABDTVTBirim(ve);
            ve.BeginTransaction();
            try
            {
                sonuc = tab.Kaydet(form);
                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = EvrakI.EVRI095 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
                else
                {
                    sonuc.bilgiStr = EvrakI.EVRI094;
                    ve.Commit();
                }

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{F381C8FB-1CF8-4ACC-8FB3-E71DC4FDF9E0} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static string[] Listele(VeriErisim veriErisim, string query)
        {
            if (string.IsNullOrEmpty(query)) return null;
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABDTVTBirim tab = new TABDTVTBirim(ve);

            IDataReader dr = tab.Listele(query);
            if (dr != null)
            {
                while (dr.Read())
                {
                    liste.objeler.Add(VeriErisim.DegerAl(dr, 0, ""));
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (string[])liste.objeler.ToArray(typeof(string));
        }

        public static DTVTBirim KurumOku(VeriErisim veriErisim, string idareKimlikKodu)
        {
            DTVTBirim form = new DTVTBirim();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABDTVTBirim tab = new TABDTVTBirim(ve);

            IDataReader dr = tab.OkuDTVTBirim(idareKimlikKodu);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form.idareKimlikKodu = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.IDAREKIMLIKKODU, "");
                    form.parentIdareKimlikKodu = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.PARENTIDAREKIMLIKKODU, "");
                    form.ad = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.AD, "");
                    form.kurumKodu = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.KURUMKODU, "");
                    form.tipkod = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.TIPKOD, "");
                    form.altKurumVarmi = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.ALTKURUMVARMI, 0);
                    form.anakod = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.ANAKOD, "");
                    form.birimkod = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.BIRIMKOD, "");
                    form.dagilimkod1 = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.DAGILIMKOD1, "");
                    form.dagilimkod2 = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.DAGILIMKOD2, "");
                    form.dagilimkod3 = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.DAGILIMKOD3, "");
                    form.dagilimkod4 = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.DAGILIMKOD4, "");
                    form.altbirimkod1 = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.ALTBIRIMKOD1, "");
                    form.altbirimkod2 = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.ALTBIRIMKOD2, "");
                    form.altbirimkod3 = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.ALTBIRIMKOD4, "");
                    form.altbirimkod4 = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.ALTBIRIMKOD4, "");
                    form.adres = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.ADRES, "");
                    form.telefon = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.TELEFON, "");
                    form.faks = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.FAKS, "");
                    form.webAdresi = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.WEBADRESI, "");
                    form.ePosta = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.EPOSTA, "");
                    form.kepAdres = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.KEPADRES, "");
                    if (!VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.SONGUNCELLEMEZAMANI).isNull)
                        form.sonGuncellemeZamani = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.SONGUNCELLEMEZAMANI).Oku();
                    form.disYazismaDurumu = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.DISYAZISMADURUMU, "");
                    form.durum = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.DURUM, "");
                    form.eYazismaServisTur = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.EYAZISMASERVISTUR, 0);

                    break;
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return form;
        }

        public static DTVTBirim[] AltKurumlariGetir(VeriErisim veriErisim, string idareKimlikKodu)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABDTVTBirim tab = new TABDTVTBirim(ve);

            IDataReader dr = tab.AltKurumlariGetir(idareKimlikKodu);
            if (dr != null)
            {
                while (dr.Read())
                {
                    DTVTBirim form = new DTVTBirim();
                    form.idareKimlikKodu = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.IDAREKIMLIKKODU, "");
                    form.parentIdareKimlikKodu = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.PARENTIDAREKIMLIKKODU, "");
                    form.ad = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.AD, "");
                    form.kurumKodu = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.KURUMKODU, "");
                    form.tipkod = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.TIPKOD, "");
                    form.altKurumVarmi = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.ALTKURUMVARMI, 0);
                    form.anakod = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.ANAKOD, "");
                    form.birimkod = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.BIRIMKOD, "");
                    form.dagilimkod1 = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.DAGILIMKOD1, "");
                    form.dagilimkod2 = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.DAGILIMKOD2, "");
                    form.dagilimkod3 = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.DAGILIMKOD3, "");
                    form.dagilimkod4 = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.DAGILIMKOD4, "");
                    form.altbirimkod1 = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.ALTBIRIMKOD1, "");
                    form.altbirimkod2 = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.ALTBIRIMKOD2, "");
                    form.altbirimkod3 = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.ALTBIRIMKOD4, "");
                    form.altbirimkod4 = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.ALTBIRIMKOD4, "");
                    form.adres = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.ADRES, "");
                    form.telefon = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.TELEFON, "");
                    form.faks = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.FAKS, "");
                    form.webAdresi = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.WEBADRESI, "");
                    form.ePosta = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.EPOSTA, "");
                    form.kepAdres = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.KEPADRES, "");
                    if (!VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.SONGUNCELLEMEZAMANI).isNull)
                        form.sonGuncellemeZamani = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.SONGUNCELLEMEZAMANI).Oku();
                    form.disYazismaDurumu = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.DISYAZISMADURUMU, "");
                    form.durum = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.DURUM, "");
                    form.eYazismaServisTur = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.EYAZISMASERVISTUR, 0);

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (DTVTBirim[])liste.objeler.ToArray(typeof(DTVTBirim));
        }

        public static Sonuc DTVTKepAdresGuncelle(VeriErisim veriErisim, Kullanici kullanan, DTVTBirim form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABDTVTBirim tab = new TABDTVTBirim(ve);
            ve.BeginTransaction();
            try
            {
                sonuc = tab.KepAdresGuncelle(form);
                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = EvrakI.EVRI095 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
                else
                {
                    sonuc.bilgiStr = EvrakI.EVRI094;
                    ve.Commit();
                }
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{69DE3FE3-42E8-4C08-8894-524E6FDD245A} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static int DisYazismaYapanKurumSayisi(VeriErisim veriErisim, string idareKimlikKodu, TNSDateTime guncellemeZamani)
        {
            int toplam = 0;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABDTVTBirim tab = new TABDTVTBirim(ve);
            toplam = tab.DisYazismaYapanKurumSayisi(idareKimlikKodu, guncellemeZamani);
            ve.VeriErisimKapat();

            return toplam;
        }

        public static DTVTBirim[] DisYazismaYapanKurumlariGetir(VeriErisim veriErisim, string idareKimlikKodu, int start, int limit, TNSDateTime guncellemeZamani)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABDTVTBirim tab = new TABDTVTBirim(ve);

            IDataReader dr = tab.DisYazismaYapanKurumlariGetir(idareKimlikKodu, guncellemeZamani);
            if (dr != null)
            {
                int sayac = 0;
                while (dr.Read())
                {
                    sayac++;
                    if (limit > 0)
                    {
                        if ((sayac - 1) == (start + limit)) break;
                        else if (sayac - 1 < start) continue;
                    }

                    DTVTBirim form = new DTVTBirim();
                    form.idareKimlikKodu = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.IDAREKIMLIKKODU, "");
                    form.parentIdareKimlikKodu = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.PARENTIDAREKIMLIKKODU, "");
                    form.ad = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.AD, "");
                    form.kurumKodu = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.KURUMKODU, "");
                    form.tipkod = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.TIPKOD, "");
                    form.altKurumVarmi = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.ALTKURUMVARMI, 0);
                    form.anakod = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.ANAKOD, "");
                    form.birimkod = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.BIRIMKOD, "");
                    form.dagilimkod1 = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.DAGILIMKOD1, "");
                    form.dagilimkod2 = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.DAGILIMKOD2, "");
                    form.dagilimkod3 = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.DAGILIMKOD3, "");
                    form.dagilimkod4 = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.DAGILIMKOD4, "");
                    form.altbirimkod1 = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.ALTBIRIMKOD1, "");
                    form.altbirimkod2 = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.ALTBIRIMKOD2, "");
                    form.altbirimkod3 = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.ALTBIRIMKOD4, "");
                    form.altbirimkod4 = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.ALTBIRIMKOD4, "");
                    form.adres = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.ADRES, "");
                    form.telefon = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.TELEFON, "");
                    form.faks = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.FAKS, "");
                    form.webAdresi = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.WEBADRESI, "");
                    form.ePosta = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.EPOSTA, "");
                    form.kepAdres = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.KEPADRES, "");
                    if (!VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.SONGUNCELLEMEZAMANI).isNull)
                        form.sonGuncellemeZamani = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.SONGUNCELLEMEZAMANI).Oku();
                    form.disYazismaDurumu = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.DISYAZISMADURUMU, "");
                    form.durum = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlar.DURUM, "");


                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (DTVTBirim[])liste.objeler.ToArray(typeof(DTVTBirim));
        }

        public static Sonuc GuncellemeBilgisiKaydet(VeriErisim veriErisim, Kullanici kullanan, DTVTBirimBilgi form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABDTVTBirim tab = new TABDTVTBirim(ve);
            ve.BeginTransaction();
            try
            {
                sonuc = tab.Kaydet(form);
                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = EvrakI.EVRI095 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
                else
                {
                    sonuc.bilgiStr = EvrakI.EVRI094;
                    ve.Commit();
                }
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{6E0487AB-90A5-4891-85D5-1B80FEC62D81} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }
        public static DTVTBirimBilgi GuncellemeBilgisiGetir(VeriErisim veriErisim, string idareKimlikKodu)
        {
            DTVTBirimBilgi form = new DTVTBirimBilgi();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABDTVTBirim tab = new TABDTVTBirim(ve);

            IDataReader dr = tab.GuncellemeBilgisiGetir(idareKimlikKodu);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form.idareKimlikKodu = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlarBilgi.IDAREKIMLIKKODU, "");
                    form.kurumAd = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlarBilgi.KURUMAD, "");
                    if (!VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlarBilgi.SONGUNCELLEMEZAMANI).isNull)
                        form.sonGuncellemeZamani = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlarBilgi.SONGUNCELLEMEZAMANI).Oku();
                    form.sonGuncellemeSayisi = VeriErisim.DegerAl(dr, (int)TABDTVTBirim.ENUMAlanlarBilgi.SONGUNCELLEMESAYISI, 0);
                    break;
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return form;
        }
    }
}
