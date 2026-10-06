using System;
using System.Data;
using TNS.KYM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.KYM.IsKurallari
{
    public class KYMZamanlayici
    {
        public static Sonuc Kaydet(VeriErisim ve, ZamanlayiciTip bilgi)
        {
            Sonuc sonuc = new Sonuc();

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriKYM", "");
            ve.BeginTransaction();

            TABZamanlayici to = new TABZamanlayici(ve);

            bilgi.kod = Guid.NewGuid().ToString();
            sonuc = to.Kaydet(bilgi);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim ve, ZamanlayiciTip bilgi)
        {
            ObjectArray liste = new ObjectArray();

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriKYM", "");
            TABZamanlayici to = new TABZamanlayici(ve);
            IDataReader dr = null;

            try
            {
                dr = to.Listele(bilgi);
            }
            catch { }

            if (dr == null)
            {
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = ve.hata;
                ve.VeriErisimKapat();
                return liste;
            }

            while (dr.Read())
            {
                ZamanlayiciTip t = new ZamanlayiciTip();
                t.kod = VeriErisim.DegerAl(dr, (int)TABZamanlayici.ENUMAlanlar.KOD, "");
                t.tur = VeriErisim.DegerAl(dr, (int)TABZamanlayici.ENUMAlanlar.TUR, "");
                t.islemAdi = VeriErisim.DegerAl(dr, (int)TABZamanlayici.ENUMAlanlar.ISLEMADI, "");
                t.zaman = VeriErisim.DegerAl(dr, (int)TABZamanlayici.ENUMAlanlar.ZAMAN);
                liste.Ekle(t);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return liste;
        }
    }
}
