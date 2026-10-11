using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    public class TMMSicilRFIDStr
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ObjectArray rfIdler)
        {
            Sonuc sonuc = new Sonuc();
            if (rfIdler.objeler.Count <= 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM220;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilRFIDStr tabRFID = new TABSicilRFIDStr(ve);
            ve.BeginTransaction();

            string hata = "";
            try
            {
                foreach (SicilNoHareket rfId in rfIdler.objeler)
                {
                    bool rfIDIsleme = false;
                    int oncekiPrSicilNo = tabRFID.RfIdBostaMi(rfId.epc);
                    if (rfId.epc != "" && oncekiPrSicilNo != 0)
                    {
                        if (oncekiPrSicilNo != rfId.prSicilNo)
                        {
                            hata += string.Format("{0} nolu rfIdNo daha önce {1} nolu prSicilNo ile eşleştirilmiştir<br>", rfId.epc, oncekiPrSicilNo);
                            rfIDIsleme = true;
                        }
                    }

                    if (!rfIDIsleme)
                    {
                        sonuc = tabRFID.Sil(rfId);
                        if (rfId.epc != "")
                            sonuc = tabRFID.Kaydet(rfId);

                        if (!sonuc.islemSonuc)
                        {
                            sonuc.islemSonuc = true;//TİF Belge onayı verildiğinde unique index hatası veriyor
                            sonuc.hataStr = "";
                            //break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.Message;
            }

            sonuc.hataStr = hata;

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim ve, Kullanici kullanan, string rfIdNo, int prSicilNo)
        {
            ObjectArray iVAN = new ObjectArray();

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABSicilRFIDStr to = new TABSicilRFIDStr(ve);
            IDataReader r = to.Listele(rfIdNo);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    SicilNoHareket sicil = new SicilNoHareket();

                    sicil.epc = VeriErisim.DegerAl(r, 0, "");
                    sicil.prSicilNo = VeriErisim.DegerAl(r, 1, 0);

                    iVAN.objeler.Add(sicil);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return iVAN;
        }
    }
}