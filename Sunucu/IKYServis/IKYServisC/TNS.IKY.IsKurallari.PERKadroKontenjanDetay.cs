using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.IKY.IsKurallari.Veri;
using TNS.MUH;

namespace TNS.IKY.IsKurallari
{
    public class PERKadroKontenjanDetay
    {
        public static KadroKontenjanDetay[] Listele(VeriErisim veriErisim, Kullanici kullanan, KadroKontenjanDetay detay)
        {
            //Sonuc sonuc = IKYYetki.YetkiVarMiOkumaya(kullanan, null, null, ENUMYetkiIslem.KadroKontenjan);
            //if (!sonuc.islemSonuc)
            //    throw new Exception(PersonelResI.PER013);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
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
                //gelen.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABKadroKontenjanDetay.ENUMAlanlar.ORGBIRIMKOD, 0);

                gelen.kkod = VeriErisim.DegerAl(dr, (int)TABKadroKontenjanDetay.ENUMAlanlar.KKOD, 0);
                gelen.unvanKod = VeriErisim.DegerAl(dr, (int)TABKadroKontenjanDetay.ENUMAlanlar.UNVANKOD, 0);
                gelen.kadroDerece = VeriErisim.DegerAl(dr, (int)TABKadroKontenjanDetay.ENUMAlanlar.KADRODERECE, 0);
                gelen.kontenjan = VeriErisim.DegerAl(dr, (int)TABKadroKontenjanDetay.ENUMAlanlar.KONTENJAN, 0);
                gelen.unvanAd = VeriErisim.DegerAl(dr, (int)TABKadroKontenjanDetay.ENUMAlanlar.KONTENJAN + 1, "");
                detaylar.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (KadroKontenjanDetay[])detaylar.ToArray(typeof(KadroKontenjanDetay));
        }

        public static KadroKontenjanDetay[] UnvanVeKadroDereceBazliKontenjanBul(VeriErisim veriErisim, Kullanici kullanan, KadroKontenjanDetay detay)
        {
            Sonuc s = IKYYetki.YetkiVarMiOkumaya(kullanan, null, 0, null, ENUMYetkiIslem.KadroKontenjan);
            if (!s.islemSonuc)
                throw new Exception(PersonelResI.PER013);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
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
                gelen.kadroDerece = VeriErisim.DegerAl(dr, 1, 0);
                gelen.unvanAd = VeriErisim.DegerAl(dr, 2, "");//IKYServis.UzayYaziDegerGetir("PERUNVAN", gelen.unvanKod.ToString(), true, string.Empty);
                gelen.kontenjan = VeriErisim.DegerAl(dr, 3, 0);

                detaylar.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (KadroKontenjanDetay[])detaylar.ToArray(typeof(KadroKontenjanDetay));
        }
    }
}