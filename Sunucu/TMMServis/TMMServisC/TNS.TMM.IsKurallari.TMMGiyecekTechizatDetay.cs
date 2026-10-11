using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// 
    /// </summary>
    public class TMMGiyecekTechizatDetay
    {
        /// <summary>
        /// Kaydets the specified veri erisim.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="tu">The tu.</param>
        /// <returns></returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, GiyecekTechizat tu)
        {
            Sonuc sonuc = new Sonuc();

            int i = 0;
            string hata = null;
            foreach (GiyecekTechizatDetay detay in tu.detay)
            {
                i++;
                hata += Genel.KontrolHesapPlan(kullanan, detay.hesapPlani.hesapKod, i, true, false);
                if (!string.IsNullOrEmpty(hata))
                    hata += "<br>";
            }
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGiyecekTechizatDetay tabTUD = new TABGiyecekTechizatDetay(ve);
            ve.BeginTransaction();

            try
            {
                tabTUD.Sil(tu);
                foreach (GiyecekTechizatDetay detay in tu.detay)
                {
                    if (detay.miktar > 0)
                    {
                        sonuc = tabTUD.Kaydet(tu, detay);
                        if (!sonuc.islemSonuc)
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.Message;
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Listeles the specified veri erisim.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="tu">The tu.</param>
        /// <returns></returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, GiyecekTechizat tu)
        {
            ObjectArray donusDeger = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGiyecekTechizatDetay tabTUD = new TABGiyecekTechizatDetay(ve);
            IDataReader dr = null;

            try
            {
                dr = tabTUD.Listele(tu);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                GiyecekTechizatDetay gelen = new GiyecekTechizatDetay();
                gelen.hesapPlani = new HesapPlaniSatir();
                gelen.hesapPlani.hesapKod = VeriErisim.DegerAl(dr, (int)TABGiyecekTechizatDetay.ENUMAlanlar.HESAPPLANKOD, string.Empty);
                gelen.hesapPlani.hesapKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), gelen.hesapPlani.hesapKod);
                gelen.hesapPlani.aciklama = VeriErisim.DegerAl(dr, (int)TABGiyecekTechizatDetay.ENUMAlanlar.TESLIMTARIHI + 1, string.Empty);
                gelen.hesapPlani.olcuBirimAd = VeriErisim.DegerAl(dr, (int)TABGiyecekTechizatDetay.ENUMAlanlar.TESLIMTARIHI + 2, string.Empty);

                gelen.perSicilNo = VeriErisim.DegerAl(dr, (int)TABGiyecekTechizatDetay.ENUMAlanlar.PERSICILNO, string.Empty);
                gelen.miktar = VeriErisim.DegerAl(dr, (int)TABGiyecekTechizatDetay.ENUMAlanlar.MIKTAR, (decimal)0);
                gelen.perUnvan = VeriErisim.DegerAl(dr, (int)TABGiyecekTechizatDetay.ENUMAlanlar.TESLIMTARIHI + 4, string.Empty);
                gelen.perAdSoyad = VeriErisim.DegerAl(dr, (int)TABGiyecekTechizatDetay.ENUMAlanlar.TESLIMTARIHI + 3, string.Empty);
                gelen.teslimTarih = VeriErisim.DegerAl(dr, (int)TABGiyecekTechizatDetay.ENUMAlanlar.TESLIMTARIHI);

                donusDeger.objeler.Add(gelen);
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }
    }
}