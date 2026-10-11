using System;
using System.Data;
using TNS.KYM;
using TNS.PEB.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.PEB.IsKurallari
{
    public class STRSpGenelHavuzRapor
    {
        public static SpGenelHavuzRapor[] Listele(VeriErisim veriErisim, Kullanici kullanan, int donemNo, string birimTur, string ustKod)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            string hata = string.Empty;
            if (donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            //if (string.IsNullOrEmpty(birimTur))
            //    hata += PerformansButceResI.STR102 + "<br>";
            if (string.IsNullOrEmpty(ustKod))
                hata += PerformansButceResI.STR101 + "<br>";
            if (!string.IsNullOrEmpty(hata))
                throw new Exception(hata);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSpGenelHavuzRapor tsghr = new TABSpGenelHavuzRapor(ve);

            IDataReader dr = null;
            try
            {
                dr = tsghr.Listele(donemNo, birimTur, ustKod);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            TNSCollection rapor = new TNSCollection();
            while (dr.Read())
            {
                SpGenelHavuzRapor gelen = new SpGenelHavuzRapor();
                gelen.havuzKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                gelen.havuzAd = PEBServis.UzayYaziDegerGetir("STRGENELHAVUZ", gelen.havuzKod, true, string.Empty);
                gelen.kullanimSayisi = VeriErisim.DegerAl(dr, 1, 0);

                rapor.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (SpGenelHavuzRapor[])rapor.ToArray(typeof(SpGenelHavuzRapor));
        }
    }
}