using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    public class TMMRFIDEtiketTuru
    {
        public static Sonuc Kaydet(Kullanici kullanan, RFIDEtiketTuru sozluk)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");
            string hata = "";

            if (sozluk.ad == string.Empty)
                hata += TasinirMalResI.TMM075 + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRFIDEtiketTuru to = new TABRFIDEtiketTuru(ve);
            TABGenel tg = new TABGenel(ve2);

            //***********************************************
            //Kontrol
            bool kayitBulundu = false;

            RFIDEtiketTuru kontrol = new RFIDEtiketTuru();
            kontrol.ad = sozluk.ad;
            IDataReader r = to.Listele(kontrol);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    if (kontrol.ad.Trim().ToUpper() == VeriErisim.DegerAl(r, 1, "").ToUpper())
                    {
                        kayitBulundu = true;
                        break;
                    }
                }
            }
            r.Close();
            if (sozluk.kod > 0)//Eğer kayıtlı RFID Etiket Türü ise kayıt yapılmasına izin ver
                kayitBulundu = false;

            if (kayitBulundu)
            {
                sonuc.hataStr = string.Format("{0} isimli RFID Etiket Türü daha önce kayıt edilmiştir.", sozluk.ad) + "<br>";
                ve.VeriErisimKapat();
                ve2.VeriErisimKapat();
                return sonuc;
            }
            //***********************************
            ve.BeginTransaction();

            if (sozluk.kod == 0)
                sozluk.kod = OrtakFonksiyonlar.ConvertToInt(tg.BelgeNoAl("RFIDETIKETTURU", "0"), 0);

            sonuc = to.Kaydet(sozluk);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TasinirMalResI.TMM118;
                sonuc.anahtar = sozluk.kod + "";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TasinirMalResI.TMM119 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(Kullanici kullanan, RFIDEtiketTuru sozluk)
        {
            Sonuc sonuc = new Sonuc();

            if (sozluk.kod == 0)
                sonuc.hataStr = TasinirMalResI.TMM165;

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRFIDEtiketTuru to = new TABRFIDEtiketTuru(ve);

            ve.BeginTransaction();

            sonuc = to.Sil(sozluk);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = string.Format(TasinirMalResI.TMM167, sozluk.ad);
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = TasinirMalResI.TMM026 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static ObjectArray Listele(Kullanici kullanan, RFIDEtiketTuru pSozluk)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABRFIDEtiketTuru to = new TABRFIDEtiketTuru(ve);
            IDataReader dr = to.Listele(pSozluk);

            if (to.Hata() == "")
            {
                while (dr.Read())
                {
                    RFIDEtiketTuru sozluk = new RFIDEtiketTuru();

                    sozluk.kod = VeriErisim.DegerAl(dr, (int)TABRFIDEtiketTuru.ENUMAlanlar.KOD, 0);
                    sozluk.ad = VeriErisim.DegerAl(dr, (int)TABRFIDEtiketTuru.ENUMAlanlar.AD, "");
                    sozluk.fabAd = VeriErisim.DegerAl(dr, (int)TABRFIDEtiketTuru.ENUMAlanlar.FABAD, "");

                    iVAN.objeler.Add(sozluk);
                }
            }

            if (dr != null)
                dr.Close();

            ve.VeriErisimKapat();

            return iVAN;
        }
    }
}
