using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PER.IsKurallari.Veri;
using TNS.MUH;

namespace TNS.PER.IsKurallari
{
    public class PERKadroKontenjanDetay
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, KadroKontenjanDetay[] detaylar)
        {
            Sonuc sonuc = new Sonuc();

            if (detaylar == null || detaylar.Length <= 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PersonelResI.PER003;
                return sonuc;
            }

            foreach (KadroKontenjanDetay detay in detaylar)
            {
                string hata = string.Empty;
                if (detay.orgBirimKod <= 0 && detay.mb == null)
                    hata = PersonelResI.PER001 + "<br>";
                if (detay.siraNo <= 0)
                    hata += PersonelResI.PER004 + "<br>";
                if (detay.unvanKod <= 0)
                    hata += PersonelResI.PER005 + "<br>";
                if (detay.kadroDerece <= 0)
                    hata += PersonelResI.PER006;
                if (!string.IsNullOrEmpty(hata))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = hata;
                    return sonuc;
                }
            }

            int tekrarUnvanKod = UnvanTekrariVarMi(detaylar);
            if (tekrarUnvanKod > 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = string.Format(PersonelResI.PER016, PERServis.UzayYaziDegerGetir("PERUNVAN", tekrarUnvanKod.ToString(), true, string.Empty));
                return sonuc;
            }

            if (!Yetki.YetkiVarMiYazmaya(kullanan, (string)null))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PersonelResI.PER011;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKadroKontenjanDetay tKKDetay = new TABKadroKontenjanDetay(ve);
            ve.BeginTransaction();

            tKKDetay.Sil(detaylar[0]);
            foreach (KadroKontenjanDetay detay in detaylar)
            {
                if (detay.kontenjan == 0)
                    continue;

                sonuc = tKKDetay.Kaydet(detay);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = PersonelResI.PER007;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = PersonelResI.PER008 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, KadroKontenjanDetay detay)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (detay.orgBirimKod <= 0 && detay.mb == null)
                hata = PersonelResI.PER001 + "<br>";
            if (detay.siraNo <= 0)
                hata += PersonelResI.PER004;
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            if (!Yetki.YetkiVarMiYazmaya(kullanan, (string)null))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PersonelResI.PER012;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKadroKontenjanDetay tKKDetay = new TABKadroKontenjanDetay(ve);
            ve.BeginTransaction();

            sonuc = tKKDetay.Sil(detay);
            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER009;
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = PersonelResI.PER010 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static KadroKontenjanDetay[] Listele(VeriErisim veriErisim, Kullanici kullanan, KadroKontenjanDetay detay)
        {
            if (!Yetki.YetkiVarMiOkumaya(kullanan))
                throw new Exception(PersonelResI.PER013);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKadroKontenjanDetay tKKDetay = new TABKadroKontenjanDetay(ve);

            IDataReader dr = null;
            try
            {
                dr = tKKDetay.Listele(detay);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            TNSCollection detaylar = new TNSCollection();
            while (dr.Read())
            {
                KadroKontenjanDetay gelen = new KadroKontenjanDetay();
                gelen.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABKadroKontenjanDetay.ENUMAlanlar.ORGBIRIMKOD, 0);
                gelen.mb = new MuhasebeBirim();
                gelen.mb.kurum = VeriErisim.DegerAl(dr, (int)TABKadroKontenjan.ENUMAlanlar.KURKOD, string.Empty);
                gelen.mb.muhasebe = VeriErisim.DegerAl(dr, (int)TABKadroKontenjan.ENUMAlanlar.MUHASEBEKOD, 0);
                gelen.mb.birimKod = VeriErisim.DegerAl(dr, (int)TABKadroKontenjan.ENUMAlanlar.BIRIMKOD, string.Empty);
                gelen.siraNo = VeriErisim.DegerAl(dr, (int)TABKadroKontenjanDetay.ENUMAlanlar.SIRANO, 0);
                gelen.unvanKod = VeriErisim.DegerAl(dr, (int)TABKadroKontenjanDetay.ENUMAlanlar.UNVANKOD, 0);
                gelen.unvanAd = PERServis.UzayYaziDegerGetir("PERUNVAN", gelen.unvanKod.ToString(), true, string.Empty);
                gelen.kadroDerece = VeriErisim.DegerAl(dr, (int)TABKadroKontenjanDetay.ENUMAlanlar.KADRODERECE, 0);
                gelen.kontenjan = VeriErisim.DegerAl(dr, (int)TABKadroKontenjanDetay.ENUMAlanlar.KONTENJAN, 0);

                detaylar.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (KadroKontenjanDetay[])detaylar.ToArray(typeof(KadroKontenjanDetay));
        }

        public static KadroKontenjanDetay[] UnvanVeKadroDereceBazliKontenjanBul(VeriErisim veriErisim, Kullanici kullanan, KadroKontenjanDetay detay)
        {
            if (!Yetki.YetkiVarMiOkumaya(kullanan))
                throw new Exception(PersonelResI.PER013);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKadroKontenjanDetay tKKDetay = new TABKadroKontenjanDetay(ve);

            IDataReader dr = null;
            try
            {
                dr = tKKDetay.UnvanVeKadroDereceBazliKontenjanBul(detay);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            TNSCollection detaylar = new TNSCollection();
            while (dr.Read())
            {
                KadroKontenjanDetay gelen = new KadroKontenjanDetay();
                gelen.unvanKod = VeriErisim.DegerAl(dr, 0, 0);
                gelen.unvanAd = PERServis.UzayYaziDegerGetir("PERUNVAN", gelen.unvanKod.ToString(), true, string.Empty);
                gelen.kadroDerece = VeriErisim.DegerAl(dr, 1, 0);
                gelen.kontenjan = VeriErisim.DegerAl(dr, 2, 0);

                detaylar.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (KadroKontenjanDetay[])detaylar.ToArray(typeof(KadroKontenjanDetay));
        }

        private static int UnvanTekrariVarMi(KadroKontenjanDetay[] detaylar)
        {
            for (int i = 0; i < detaylar.Length; i++)
            {
                KadroKontenjanDetay detay1 = (KadroKontenjanDetay)detaylar[i];
                for (int j = i + 1; j < detaylar.Length; j++)
                {
                    KadroKontenjanDetay detay2 = (KadroKontenjanDetay)detaylar[j];
                    if (detay1.unvanKod == detay2.unvanKod && detay1.kadroDerece == detay2.kadroDerece)
                        return detay1.unvanKod;
                }
            }
            return 0;
        }
    }
}