using System.Data;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TRM.IsKurallari
{
    public class TRMYatirimMalzemeGrubu
    {
        public Sonuc Kaydet(Kullanici kullanan, YatirimMalzemeTur bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = "";

            if (bilgi.anaKod == 0)
                hata += "Ana Grup bilgisi boş bırakılamaz" + "<br>";
            if (bilgi.ad == "")
                hata += "Ad bilgisi boş bırakılamaz" + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABYatirimMalzemeGrubu to = new TABYatirimMalzemeGrubu(ve);
            ve.BeginTransaction();

            if (bilgi.kod == 0)
                bilgi.kod = to.EnBuyukKod();

            sonuc = to.Kaydet(bilgi);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
            {
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu" + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc Sil(Kullanici kullanan, YatirimMalzemeTur bilgi)
        {
            Sonuc sonuc = new Sonuc();

            if (bilgi.kod == 0)
                sonuc.hataStr = "Kod bilgisi boş bırakılamaz" + "<br>";

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABYatirimMalzemeGrubu to = new TABYatirimMalzemeGrubu(ve);

            ve.BeginTransaction();

            sonuc = to.Sil(bilgi);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Bilgi silinirken bir hata oluştu" + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public ObjectArray Listele(Kullanici kullanan, YatirimMalzemeTur kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABYatirimMalzemeGrubu to = new TABYatirimMalzemeGrubu(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    YatirimMalzemeTur f = new YatirimMalzemeTur();

                    f.kod = VeriErisim.DegerAl(r, 0, 0);
                    f.anaKod = VeriErisim.DegerAl(r, 1, 0);
                    f.ad = VeriErisim.DegerAl(r, 2, string.Empty);
                    f.aktif = VeriErisim.DegerAl(r, 3, 0);
                    f.anaAd = VeriErisim.DegerAl(r, 4, string.Empty);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public Sonuc KaydetAna(Kullanici kullanan, YatirimMalzemeTur bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = "";

            if (bilgi.ad == "")
                hata += "Ad bilgisi boş bırakılamaz" + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABYatirimMalzemeGrubu to = new TABYatirimMalzemeGrubu(ve);
            ve.BeginTransaction();

            if (bilgi.kod == 0)
                bilgi.kod = to.EnBuyukKodAna();

            sonuc = to.KaydetAna(bilgi);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
            {
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu" + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc SilAna(Kullanici kullanan, YatirimMalzemeTur bilgi)
        {
            Sonuc sonuc = new Sonuc();

            if (bilgi.kod == 0)
                sonuc.hataStr = "Kod bilgisi boş bırakılamaz" + "<br>";

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABYatirimMalzemeGrubu to = new TABYatirimMalzemeGrubu(ve);

            ve.BeginTransaction();

            sonuc = to.SilAna(bilgi);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Bilgi silinirken bir hata oluştu" + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public ObjectArray ListeleAna(Kullanici kullanan, YatirimMalzemeTur kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABYatirimMalzemeGrubu to = new TABYatirimMalzemeGrubu(ve);
            IDataReader r = to.ListeleAna(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    YatirimMalzemeTur f = new YatirimMalzemeTur();

                    f.kod = VeriErisim.DegerAl(r, 0, 0);
                    f.ad = VeriErisim.DegerAl(r, 1, string.Empty);
                    f.aktif = VeriErisim.DegerAl(r, 2, 0);
                    f.coklu = VeriErisim.DegerAl(r, 3, 0);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }
    }
}
