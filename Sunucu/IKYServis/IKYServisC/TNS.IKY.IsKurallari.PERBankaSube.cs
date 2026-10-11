using System;
using System.Data;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;

namespace TNS.IKY.IsKurallari
{
    class PERBankaSube
    {
        public ObjectArray Listele(BankaSube p)
        {
            ObjectArray pVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(null));

            TABBankaSube tab = new TABBankaSube(ve);

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
                BankaSube pp = new BankaSube();

                pp.kod = VeriErisim.DegerAl(r, 0, "");
                pp.bankaKodu = VeriErisim.DegerAl(r, 1, "");
                pp.ad = VeriErisim.DegerAl(r, 2, "");
                pp.bankaAd = VeriErisim.DegerAl(r, 4, "");

                pVAN.objeler.Add(pp);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public Sonuc Kaydet(Kullanici kullanan, BankaSube p)
        {
            Sonuc sonuc = new Sonuc();
            string hata = "";

            if (p.bankaKodu.Trim() == "")
                hata = "Banka kodu boş bırakılamaz";

            if (p.kod.Trim() == "")
            {
                if (hata != "") hata += "<br>";
                hata += "Banka şube kodu boş bırakılamaz";
            }

            if (p.ad.Trim() == "")
            {
                if (hata != "") hata += "<br>";
                hata += "Banka şube adı boş bırakılamaz";
            }

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABBankaSube tab = new TABBankaSube(ve);

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

        public Sonuc Sil(Kullanici kullanan, BankaSube p)
        {
            Sonuc sonuc = new Sonuc();
            string hata = "";

            if (p.bankaKodu.Trim() == "")
                hata = "Banka kodu boş bırakılamaz";

            if (p.kod.Trim() == "")
            {
                if (hata != "") hata += "<br>";
                hata += "Banka şube kodu boş bırakılamaz";
            }

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABBankaSube tab = new TABBankaSube(ve);

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
    }
}
