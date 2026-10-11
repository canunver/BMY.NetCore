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
    public class TMMSicilTekrarEden
    {
        static string verisSekli = System.Configuration.ConfigurationManager.AppSettings.Get("TasinirSicilNoVermeTuru") + "";

        /// <summary>
        /// Tekrars the eden sicilleri bul.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="sicilNo">The sicil no.</param>
        /// <returns></returns>
        public static ObjectArray TekrarEdenSicilleriBul(VeriErisim veriErisim, Kullanici kullanan, SicilNoHareket sicilNo)
        {
            ObjectArray bilgi = new ObjectArray();
            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = TasinirMalResI.TMM151;
                return bilgi;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilTekrarEden tabSicilTekrarEden = new TABSicilTekrarEden(ve);

            IDataReader dr = null;
            try
            {
                dr = tabSicilTekrarEden.TekrarEdenSicilleriBul(sicilNo);
                if (dr == null)
                    throw new Exception(ve.hata);
            }
            catch (Exception e)
            {
                if (dr != null)
                    dr.Close();

                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.Message;
                ve.VeriErisimKapat();
                return bilgi;
            }

            while (dr.Read())
            {
                SicilNoHareket gelen = new SicilNoHareket();
                gelen.sicilNo = VeriErisim.DegerAl(dr, 0, string.Empty);
                gelen.prSicilNo = VeriErisim.DegerAl(dr, 1, 0);
                gelen.siraNo = VeriErisim.DegerAl(dr, 2, 0);

                bilgi.Ekle(gelen);
            }
            dr.Close();

            if (bilgi.objeler.Count <= 0)
                bilgi.sonuc.bilgiStr = TasinirMalResI.TMM029;
            else
            {
                for (int i = 0; i < bilgi.objeler.Count; i++)
                {
                    SicilNoHareket s = (SicilNoHareket)bilgi.objeler[i];
                    s = (SicilNoHareket)TMMSicilNoHareket.SicilNoTarihce(ve, kullanan, s)[0];
                    bilgi.objeler.RemoveAt(i);
                    bilgi.objeler.Insert(i, s);
                }
            }

            ve.VeriErisimKapat();
            return bilgi;
        }

        /// <summary>
        /// Sicils the no guncelle.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="sicilNolar">The sicil nolar.</param>
        /// <returns></returns>
        public static Sonuc SicilNoGuncelle(VeriErisim veriErisim, Kullanici kullanan, SicilNoHareket[] sicilNolar)
        {
            Sonuc sonuc = new Sonuc();

            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM151;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilTekrarEden tabSicilTekrarEden = new TABSicilTekrarEden(ve);

            ve.BeginTransaction();

            foreach (SicilNoHareket sicilNo in sicilNolar)
            {
                if (string.IsNullOrEmpty(sicilNo.muhasebeKod))
                    sonuc.hataStr = TasinirMalResI.TMM013 + "<br />";
                if (string.IsNullOrEmpty(sicilNo.harcamaBirimKod))
                    sonuc.hataStr += TasinirMalResI.TMM014 + "<br />";
                if (sicilNo.prSicilNo <= 0)
                    sonuc.hataStr += TasinirMalResI.TMM214 + "<br />";
                if (string.IsNullOrEmpty(sicilNo.sicilNo))
                    sonuc.hataStr += TasinirMalResI.TMM212;
                if (!string.IsNullOrEmpty(sonuc.hataStr))
                {
                    sonuc.islemSonuc = false;
                    break;
                }

                bool yeniSicilUret = true;
                string sicilNoYeni = sicilNo.sicilNo;
                if (verisSekli.ToUpper() == "KURUM")
                {
                    sicilNoYeni = tabSicilTekrarEden.SicilNoBul(sicilNo.prSicilNo, 1);
                    if (!sicilNoYeni.Equals(sicilNo.sicilNo))
                        yeniSicilUret = false;
                }
                if (yeniSicilUret)
                {
                    int yil = OrtakFonksiyonlar.ConvertToInt("20" + sicilNoYeni.Substring(sicilNoYeni.Length - 8, 2), 0);
                    string hesapPlanKod = sicilNoYeni.Substring(0, sicilNoYeni.Length - 8);
                    sicilNoYeni = YeniSicilNoVer(ve, sicilNo.muhasebeKod, sicilNo.harcamaBirimKod, yil, hesapPlanKod);
                }

                if (string.IsNullOrEmpty(sicilNoYeni))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = TasinirMalResI.TMM216;
                    break;
                }

                sonuc = tabSicilTekrarEden.SicilNoGuncelle(sicilNo, sicilNoYeni);
                if (!sonuc.islemSonuc)
                    break;

                sonuc = tabSicilTekrarEden.SicilSiraNoGuncelle(sicilNo, sicilNoYeni, sicilNoYeni.Substring(sicilNoYeni.Length - 6));
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TasinirMalResI.TMM322;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TasinirMalResI.TMM323 + "<br />" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Yenis the sicil no ver.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="muhasebeKod">The muhasebe kod.</param>
        /// <param name="harcamaBirimKod">The harcama birim kod.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="hesapPlanKod">The hesap plan kod.</param>
        /// <returns></returns>
        public static string YeniSicilNoVer(VeriErisim veriErisim, string muhasebeKod, string harcamaBirimKod, int yil, string hesapPlanKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));

            TABGenel tabGenel = new TABGenel(ve2);
            TABSicilTekrarEden tabSicilTekrarEden = new TABSicilTekrarEden(ve);

            ve.BeginTransaction();

            hesapPlanKod = hesapPlanKod.Replace(".", "");

            string sayacKod = "SICIL" + yil.ToString() + muhasebeKod + harcamaBirimKod.Replace(".", "") + hesapPlanKod;
            if (verisSekli.ToUpper() == "KURUM")
                sayacKod = "SICIL" + yil + hesapPlanKod;

            string sicilNo;
            while (true)
            {
                string siraNo = tabGenel.BelgeNoAl(sayacKod, "000000");
                if (string.IsNullOrEmpty(siraNo))
                {
                    sicilNo = string.Empty;
                    break;
                }

                sicilNo = hesapPlanKod + yil.ToString().Substring(2) + siraNo;
                if (!tabSicilTekrarEden.SicilNoKullanimdaMi(muhasebeKod, harcamaBirimKod, sicilNo))
                    break;
            }

            if (!string.IsNullOrEmpty(sicilNo))
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();
            return sicilNo;
        }
    }
}