using System.Data;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.MUH.IsKurallari
{
    public class MUHTaahhutKartiBakimTarihce
    {
        public static Sonuc Kaydet(VeriErisim ve, Kullanici kullanan, TaahhutKartiBakimTarihce form)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            if (form.taahhutKartiKod == 0)
                sonuc.hataStr += "Taahhüt bilgisi bulunamadı. Öncelikle taahhüt kartını kaydediniz." + "<br>";
            else
            {
                if (form.girisTarihi == null || form.girisTarihi.isNull)
                    sonuc.hataStr += "Giriş açıklama bilgisi boş bırakılamaz." + "<br>";
            }

            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }


            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTaahhutKartiBakimTarihce to = new TABTaahhutKartiBakimTarihce(ve);


            TABGenel tg = new TABGenel(ve);
            if (form.kod == 0)
            {
                //kod alınıyor
                //**********************************************************
                string sayacKod = "TAAHHUTKODBAKIMTARIHCE";
                form.kod = OrtakFonksiyonlar.ConvertToInt(tg.BelgeNoAl(sayacKod, "0000"), 0);
                //**********************************************************

                sonuc = to.Kaydet(form);
            }
            else
                sonuc = to.Guncelle(form);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.anahtar = form.kod + "";
                sonuc.bilgiStr = ButceMuhasebeResI.MUH201;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim ve, Kullanici kullanan, TaahhutKartiBakimTarihce bilgi)
        {
            ObjectArray iVAN = new ObjectArray();

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTaahhutKartiBakimTarihce to = new TABTaahhutKartiBakimTarihce(ve);

            IDataReader r = to.Listele(bilgi);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TaahhutKartiBakimTarihce form = new TaahhutKartiBakimTarihce();

                    form.kod = VeriErisim.DegerAl(r, (int)TABTaahhutKartiBakimTarihce.ENUMAlanlar.KOD, 0);
                    form.taahhutKartiKod = VeriErisim.DegerAl(r, (int)TABTaahhutKartiBakimTarihce.ENUMAlanlar.TAAHHUTKARTIKOD, 0);
                    form.girisTarihi = VeriErisim.DegerAl(r, (int)TABTaahhutKartiBakimTarihce.ENUMAlanlar.GIRISTARIH);
                    form.girisAciklama = VeriErisim.DegerAl(r, (int)TABTaahhutKartiBakimTarihce.ENUMAlanlar.GIRISACIKLAMA, "");
                    form.cikisTarihi = VeriErisim.DegerAl(r, (int)TABTaahhutKartiBakimTarihce.ENUMAlanlar.CIKISTARIH);
                    form.cikisAciklama = VeriErisim.DegerAl(r, (int)TABTaahhutKartiBakimTarihce.ENUMAlanlar.CIKISACIKLAMA, "");

                    iVAN.objeler.Add(form);
                }
                r.Close();
            }


            ve.VeriErisimKapat();

            return iVAN;
        }

        public static Sonuc Sil(VeriErisim ve, Kullanici kullanan, TaahhutKartiBakimTarihce bilgi)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH068, "");

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTaahhutKartiBakimTarihce to = new TABTaahhutKartiBakimTarihce(ve);
            sonuc = to.Sil(bilgi);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH134;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;

        }
    }
}
