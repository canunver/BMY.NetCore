using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.KYM;
using TNS.Veri;
using TNS.YAT.IsKurallari.Veri;

namespace TNS.YAT.IsKurallari
{
    public class ButceKrediDetayClass
    {
        public static Sonuc Kaydet(VeriErisim ve, int butceYil, string senaryoKod, int kaynakNo, TNSCollection krediDetaylar)
        {
            for (int i = 0; i < krediDetaylar.Count; i++)
            {
                KrediDetay krediYil = (KrediDetay)krediDetaylar[i];
                krediYil.butceYil = butceYil;
                krediYil.senaryoKod = senaryoKod;
                krediYil.kaynakNo = kaynakNo;
            }

            TABYATKrediDetay tabKrediDetay = new TABYATKrediDetay(ve);
            Sonuc donusDeger = tabKrediDetay.Sil(butceYil, senaryoKod, kaynakNo);

            foreach (KrediDetay kk in krediDetaylar)
            {
                if (!donusDeger.islemSonuc)
                    break;
                if (kk.anlasmaTutariIc > 0 || kk.anlasmaTutari > 0 || kk.kullanilanTutar > 0 || kk.kullanilanTutarIc > 0 || kk.kullanilacakTutar > 0 || kk.kullanilacakTutarIc > 0)
                    donusDeger = tabKrediDetay.Kaydet(kk);
            }
            return donusDeger;
        }

        public static ObjectArray Listele(VeriErisim ve, int butceYil, string senaryoKod, int kaynakNo)
        {
            ObjectArray donusDeger = new ObjectArray();

            TABYATKrediDetay tabKrediKullanim = new TABYATKrediDetay(ve);
            IDataReader dr = tabKrediKullanim.Listele(butceYil, senaryoKod, kaynakNo);

            if (dr == null)
            {
                donusDeger.sonuc.DegerAta(ve.hata);
                return donusDeger;
            }

            while (dr.Read())
            {
                KrediDetay gelen = new KrediDetay();
                gelen.butceYil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.senaryoKod = VeriErisim.DegerAl(dr, 1, "");
                gelen.kaynakNo = VeriErisim.DegerAl(dr, 2, 0);
                gelen.yil = VeriErisim.DegerAl(dr, 3, 0);
                gelen.anlasmaTutari = VeriErisim.DegerAl(dr, 4, 0.0);
                gelen.kullanilanTutar = VeriErisim.DegerAl(dr, 5, 0.0);
                gelen.kullanilacakTutar = VeriErisim.DegerAl(dr, 6, 0.0);
                gelen.anlasmaTutariIc = VeriErisim.DegerAl(dr, 7, 0.0);
                gelen.kullanilanTutarIc = VeriErisim.DegerAl(dr, 8, 0.0);
                gelen.kullanilacakTutarIc = VeriErisim.DegerAl(dr, 9, 0.0);

                donusDeger.Ekle(gelen);
            }
            dr.Close();
            return donusDeger;
        }

        public static Sonuc Sil(VeriErisim ve, int yil, string senaryoKod, int kaynakNo)
        {
            TABYATKrediDetay tabGO = new TABYATKrediDetay(ve);
            return tabGO.Sil(yil, senaryoKod, kaynakNo);
        }

        public static ObjectArray KrediliKaynaklariListele(VeriErisim veriErisim, Kullanici kullanan, int butceYil, string senaryoKod, int projeKodu, int altProjeKodu, ENUMButceKaynakTur[] kaynakTurler)
        {
            ObjectArray donusDeger = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATKrediDetay tabKrediDetay = new TABYATKrediDetay(ve);
            IDataReader dr = tabKrediDetay.KaynakListele(butceYil, senaryoKod, projeKodu, altProjeKodu, kaynakTurler);

            if (dr == null)
            {
                donusDeger.sonuc.DegerAta(ve.hata);
                return donusDeger;
            }

            while (dr.Read())
            {
                donusDeger.Ekle(VeriErisim.DegerAl(dr, 0, (int)0));
            }
            dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }
    }
}