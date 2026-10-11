using System.Data;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.MUH.IsKurallari
{
    public class MUHTaahhutKartiSozlesme
    {
        public static Sonuc Kaydet(VeriErisim ve, Kullanici kullanan, TaahhutKartiSozlesme form)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            if (form.taahhutKartiKod == 0)
                sonuc.hataStr += "Taahhüt bilgisi bulunamadı. Öncelikle taahhüt kartını kaydediniz." + "<br>";
            else
            {
                if (string.IsNullOrEmpty(form.aciklama))
                    sonuc.hataStr += "Açıklama bilgisi boş bırakılamaz." + "<br>";
                if (form.tur == 0)
                    sonuc.hataStr += "Tür bilgisi boş bırakılamaz." + "<br>";
            }

            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }


            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTaahhutKartiSozlesme to = new TABTaahhutKartiSozlesme(ve);


            TABGenel tg = new TABGenel(ve);
            if (form.kod == 0)
            {
                //kod alınıyor
                //**********************************************************
                string sayacKod = "TAAHHUTKODSOZLESME";
                form.kod = OrtakFonksiyonlar.ConvertToInt(tg.BelgeNoAl(sayacKod, "0000"), 0);
                //**********************************************************
            }
            else
                sonuc = to.Sil(form);

            if (sonuc.islemSonuc)
                sonuc = to.Kaydet(form);

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

        public static ObjectArray Listele(VeriErisim ve, Kullanici kullanan, TaahhutKartiSozlesme bilgi)
        {
            ObjectArray iVAN = new ObjectArray();

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTaahhutKartiSozlesme to = new TABTaahhutKartiSozlesme(ve);

            IDataReader r = to.Listele(bilgi);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TaahhutKartiSozlesme form = new TaahhutKartiSozlesme();

                    form.kod = VeriErisim.DegerAl(r, (int)TABTaahhutKartiSozlesme.ENUMAlanlar.KOD, 0);
                    form.taahhutKartiKod = VeriErisim.DegerAl(r, (int)TABTaahhutKartiSozlesme.ENUMAlanlar.TAAHHUTKARTIKOD, 0);
                    form.tur = (ENUMSozlesmeTur)VeriErisim.DegerAl(r, (int)TABTaahhutKartiSozlesme.ENUMAlanlar.TUR, 0);
                    form.maddeNo = VeriErisim.DegerAl(r, (int)TABTaahhutKartiSozlesme.ENUMAlanlar.MADDENO, 0);
                    form.ad = VeriErisim.DegerAl(r, (int)TABTaahhutKartiSozlesme.ENUMAlanlar.AD, "");
                    form.aciklama = VeriErisim.DegerAl(r, (int)TABTaahhutKartiSozlesme.ENUMAlanlar.ACIKLAMA, "");

                    iVAN.objeler.Add(form);
                }
                r.Close();
            }


            ve.VeriErisimKapat();

            return iVAN;
        }

        public static Sonuc Sil(VeriErisim ve, Kullanici kullanan, TaahhutKartiSozlesme bilgi)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH068, "");

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTaahhutKartiSozlesme to = new TABTaahhutKartiSozlesme(ve);
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
