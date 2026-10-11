using System.Data;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;
using TNS.BKD;
using System;

namespace TNS.MUH.IsKurallari
{
    public class MUHSayistayVeri
    {
        public Sonuc Kaydet(Kullanici kullanan, SayistayVeri veri)
        {
            Sonuc sonuc = new Sonuc(true, ButceMuhasebeResI.MUH001, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABSayistayVeri to = new TABSayistayVeri(ve);

            sonuc = to.Sil(veri);

            veri.kod = Guid.NewGuid().ToString();
            sonuc = to.Kaydet(veri);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH201;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public ObjectArray Listele(Kullanici kullanan, SayistayVeri bilgi)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABSayistayVeri to = new TABSayistayVeri(ve);

            IDataReader r = to.Listele(bilgi);
            ObjectArray liste = new ObjectArray();

            if (r != null)
            {
                while (r.Read())
                {
                    SayistayVeri veri = new SayistayVeri();

                    veri.kod = VeriErisim.DegerAl(r, 0, "");
                    veri.yil = VeriErisim.DegerAl(r, 1, 0);
                    veri.ay = VeriErisim.DegerAl(r, 2, 0);
                    veri.muhasebe = VeriErisim.DegerAl(r, 3, 0);
                    veri.tur = VeriErisim.DegerAl(r, 4, "");
                    veri.veriler = VeriErisim.DegerAl(r, 5, "");

                    liste.objeler.Add(veri);
                }
                r.Close();
            }

            ve.VeriErisimKapat();
            return liste;
        }

        public Sonuc Sil(Kullanici kullanan, SayistayVeri veri)
        {
            Sonuc sonuc = new Sonuc(true, ButceMuhasebeResI.MUH001, "");

            if (veri.kod == "" && (veri.yil == 0 || veri.ay == 0 || veri.muhasebe == 0))
            {
                sonuc.DegerAta(ButceMuhasebeResI.MUH133);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABSayistayVeri to = new TABSayistayVeri(ve);

            sonuc = to.Sil(veri);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Kayıt silindi";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}
