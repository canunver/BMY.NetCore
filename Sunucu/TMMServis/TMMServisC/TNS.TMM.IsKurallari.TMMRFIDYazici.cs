using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    public class TMMRFIDYazici
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, RFIDYazici form)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

            string hata = "";
            if (string.IsNullOrWhiteSpace(form.harcamaBirimKod))
                hata += "Harcama Birim Kodu boş olamaz." + "<br>";
            if (string.IsNullOrWhiteSpace(form.ip))
                hata += "ip bilgisi boş olamaz." + "<br>";
            if (string.IsNullOrWhiteSpace(form.ad))
                hata += "ad bilgisi boş olamaz." + "<br>";

            if (hata != "")
                return new Sonuc(false, hata, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRFIDYazici to = new TABRFIDYazici(ve);
            TABGenel tg = new TABGenel(ve2);

            ve.BeginTransaction();

            if (form.kod == 0)
            {
                form.kod = OrtakFonksiyonlar.ConvertToInt(tg.BelgeNoAl("RFIDYAZICI", "0"), 0);
                sonuc = to.Kaydet(form);
            }
            else
            {
                sonuc = to.Guncelle(form);
            }

            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = TasinirMalResI.TMM119 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }
            else
            {
                sonuc.anahtar = form.kod + "";
                sonuc.bilgiStr = TasinirMalResI.TMM118;
                ve.Commit();
            }
            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, RFIDYazici form)
        {
            Sonuc sonuc = new Sonuc();

            if (form.kod == 0)
                sonuc.hataStr = TasinirMalResI.TMM165;

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRFIDYazici to = new TABRFIDYazici(ve);

            ve.BeginTransaction();

            sonuc = to.Sil(form);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = string.Format(TasinirMalResI.TMM167, form.ad);
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = TasinirMalResI.TMM026 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, RFIDYazici form)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABRFIDYazici to = new TABRFIDYazici(ve);
            IDataReader dr = to.Listele(form);

            if (to.Hata() == "")
            {
                while (dr.Read())
                {
                    form = new RFIDYazici();

                    form.kod = VeriErisim.DegerAl(dr, (int)TABRFIDYazici.ENUMAlanlar.KOD, 0);
                    form.harcamaBirimKod = VeriErisim.DegerAl(dr, (int)TABRFIDYazici.ENUMAlanlar.HARCAMABIRIMKOD, "");
                    form.ip = VeriErisim.DegerAl(dr, (int)TABRFIDYazici.ENUMAlanlar.IP, "");
                    form.ad = VeriErisim.DegerAl(dr, (int)TABRFIDYazici.ENUMAlanlar.AD, "");

                    iVAN.objeler.Add(form);
                }
            }

            if (dr != null)
                dr.Close();

            ve.VeriErisimKapat();

            return iVAN;
        }
    }
}
