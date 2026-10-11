using System;
using System.Data;
using TNS.PER;
using TNS.PER.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;
using System.Collections;

namespace TNS.PER.IsKurallari
{
    class PERUnvan
    {
        public ObjectArray Listele(Unvan p)
        {
            ObjectArray pVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(null));

            TABUnvan tab = new TABUnvan(ve);

            IDataReader r = null;

            try
            {
                r = tab.Listele(p);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            while (r.Read())
            {
                Unvan pp = new Unvan();

                pp.kod = VeriErisim.DegerAl(r, 0, 0);
                pp.ad = VeriErisim.DegerAl(r, 1, "");
                pp.adKisa = VeriErisim.DegerAl(r, 2, "");
                pp.siraNo = VeriErisim.DegerAl(r, 3, 0);
                pp.yonetici = VeriErisim.DegerAl(r, 4, 0);

                pVAN.objeler.Add(pp);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public Sonuc Kaydet(Kullanici kullanan, Unvan p)
        {
            Sonuc sonuc = new Sonuc();

            if (p.ad.Trim() == "")
            {
                sonuc.hataStr = "Unvan adı boş bırakılamaz";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABUnvan tab = new TABUnvan(ve);

            sonuc = tab.Kaydet(p);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc Sil(Kullanici kullanan, Unvan p)
        {
            Sonuc sonuc = new Sonuc();

            if (p.kod == 0)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin kodu</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABUnvan tab = new TABUnvan(ve);

            sonuc = tab.Sil(p);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static ObjectArray UnvanBazliPersonelSayilariListele(int muhasebeKod, string birimKod)
        {
            ObjectArray donusDeger = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(null));
            TABUnvan tab = new TABUnvan(ve);

            IDataReader r = null;

            try
            {
                r = tab.UnvanBazliPersonelSayilariListele(muhasebeKod, birimKod);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;

                return donusDeger;
            }

            while (r.Read())
            {
                UnvanBazliPersonelSayi gelen = new UnvanBazliPersonelSayi();

                gelen.unvan = new Unvan();
                gelen.unvan.kod = VeriErisim.DegerAl(r, 0, 0);
                gelen.unvan.ad = VeriErisim.DegerAl(r, 1, string.Empty);
                gelen.unvan.adKisa = VeriErisim.DegerAl(r, 2, string.Empty);
                gelen.unvan.siraNo = VeriErisim.DegerAl(r, 3, 0);
                gelen.unvan.yonetici = VeriErisim.DegerAl(r, 4, 0);
                gelen.personelSayi = VeriErisim.DegerAl(r, 5, 0);

                donusDeger.objeler.Add(gelen);
            }
            r.Close();

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();
            return donusDeger;
        }

        public static ObjectArray UnvanVeKadroDereceBazliPersonelSayilariListele(Kullanici kullanan, KadroKontenjan kk)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ObjectArray personelSayilari = UnvanVeKadroDereceBazliPersonelSayilariListele(ve, kk.mb.muhasebe, kk.mb.birimKod);
            if (!personelSayilari.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return personelSayilari;
            }

            KadroKontenjanDetay kkDetay = new KadroKontenjanDetay();
            kkDetay.orgBirimKod = kk.orgBirimKod;
            kkDetay.mb = kk.mb;
            KadroKontenjanDetay[] kontenjanlar = PERKadroKontenjanDetay.UnvanVeKadroDereceBazliKontenjanBul(ve, kullanan, kkDetay);

            TNSCollection kontenjanSayilari = new TNSCollection(kontenjanlar);
            foreach (UnvanVeKadroDereceBazliPersonelSayi p in personelSayilari.objeler)
            {
                for (int i = 0; i < kontenjanSayilari.Count; i++)
                {
                    KadroKontenjanDetay k = (KadroKontenjanDetay)kontenjanSayilari[i];
                    if (p.unvan.kod == k.unvanKod && p.kadroDerece == k.kadroDerece)
                    {
                        p.kontenjan = k.kontenjan;
                        kontenjanSayilari.RemoveAt(i);
                        i--;
                    }
                }
            }

            if (kontenjanSayilari.Count > 0)
            {
                for (int i = 0; i < kontenjanSayilari.Count; i++)
                {
                    KadroKontenjanDetay k = (KadroKontenjanDetay)kontenjanSayilari[i];

                    UnvanVeKadroDereceBazliPersonelSayi eslesmeyenKontenjan = new UnvanVeKadroDereceBazliPersonelSayi();
                    eslesmeyenKontenjan.unvan = new Unvan();
                    eslesmeyenKontenjan.unvan.kod = k.unvanKod;
                    eslesmeyenKontenjan.unvan.ad = k.unvanAd;
                    eslesmeyenKontenjan.kadroDerece = k.kadroDerece;
                    eslesmeyenKontenjan.kontenjan = k.kontenjan;

                    personelSayilari.objeler.Add(eslesmeyenKontenjan);
                }
            }

            ve.VeriErisimKapat();
            return personelSayilari;
        }

        private static ObjectArray UnvanVeKadroDereceBazliPersonelSayilariListele(VeriErisim veriErisim, int muhasebeKod, string birimKod)
        {
            ObjectArray donusDeger = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(null));
            TABUnvan tab = new TABUnvan(ve);

            IDataReader r = null;

            try
            {
                r = tab.UnvanVeKadroDereceBazliPersonelSayilariListele(muhasebeKod, birimKod);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;

                return donusDeger;
            }

            while (r.Read())
            {
                UnvanVeKadroDereceBazliPersonelSayi gelen = new UnvanVeKadroDereceBazliPersonelSayi();

                gelen.unvan = new Unvan();
                gelen.unvan.kod = VeriErisim.DegerAl(r, 0, 0);
                gelen.unvan.ad = VeriErisim.DegerAl(r, 1, string.Empty);
                gelen.unvan.adKisa = VeriErisim.DegerAl(r, 2, string.Empty);
                gelen.unvan.siraNo = VeriErisim.DegerAl(r, 3, 0);
                gelen.unvan.yonetici = VeriErisim.DegerAl(r, 4, 0);
                gelen.kadroDerece = VeriErisim.DegerAl(r, 5, 0);
                gelen.personelSayi = VeriErisim.DegerAl(r, 6, 0);

                donusDeger.objeler.Add(gelen);
            }
            r.Close();

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();
            return donusDeger;
        }
   }
}