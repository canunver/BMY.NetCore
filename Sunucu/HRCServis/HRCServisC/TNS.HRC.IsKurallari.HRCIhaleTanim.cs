using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// İhale tür tanım bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCIhaleTur
    {
        /// <summary>
        /// Arguman olarak verilen, ihale tür tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri IhaleTur nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YoneticiMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (liste.objeler.Count == 0)
                hata = TNS.HRC.HarcamaSurecleriResI.HRC00007;

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIhaleTur to = new TABIhaleTur(ve);
            ve.BeginTransaction();

            sonuc = to.Kaydet(liste);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale tür tanım bilgisini veritabanından okur 
        /// ve IhaleTur nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale tür tanım bilgisi listesini IhaleTur nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, IhaleTur kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABIhaleTur to = new TABIhaleTur(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    IhaleTur f = new IhaleTur();

                    f.kod = VeriErisim.DegerAl(r, 0, 0);
                    f.adi = VeriErisim.DegerAl(r, 1, "");
                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }
    }

    /// <summary>
    /// İhale türüne ait madde tanım bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCIhaleTurMadde
    {
        /// <summary>
        /// Arguman olarak verilen, ihale türüne ait madde tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri IhaleTurMadde nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YoneticiMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (liste.objeler.Count == 0)
                hata = TNS.HRC.HarcamaSurecleriResI.HRC00007;

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIhaleTurMadde to = new TABIhaleTurMadde(ve);
            ve.BeginTransaction();

            sonuc = to.Kaydet(liste);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale türüne ait madde tanım bilgisini veritabanından okur 
        /// ve IhaleTurMadde nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="ihaleKod">İşlem yapılacak ihale tür kodu</param>
        /// <returns>İstenilen kriterdeki, ihale türüne ait madde tanım bilgisi listesini IhaleTurMadde nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, int ihaleKod)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABIhaleTurMadde to = new TABIhaleTurMadde(ve);
            IDataReader r = to.Listele(ihaleKod);

            if (to.Hata() == "")
            {
                while (r.Read())
                {

                    IhaleTurMadde f = new IhaleTurMadde();
                    f.kod = VeriErisim.DegerAl(r, 0, 0);
                    f.ihaleKod = VeriErisim.DegerAl(r, 1, 0);
                    f.adi = VeriErisim.DegerAl(r, 2, "");
                    f.aciklama = VeriErisim.DegerAl(r, 3, "");
                    f.ilanZorunlu = VeriErisim.DegerAl(r, 4, 0);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale türüne ait madde tanım bilgisini veritabanından okur 
        /// ve IhaleTurMadde nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="ihaleKod">İşlem yapılacak ihale tür kodu</param>
        /// <param name="maddeKod">İşlem yapılacak ihale madde kodu</param>
        /// <returns>İstenilen kriterdeki, ihale türüne ait madde tanım bilgisini IhaleTurMadde nesnesi olarak tutan nesne</returns>
        public IhaleTurMadde IhaleUsulMaddeAl(Kullanici kullanan, int ihaleKod, int maddeKod)
        {
            IhaleTurMadde f = new IhaleTurMadde();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIhaleTurMadde to = new TABIhaleTurMadde(ve);
            IDataReader r = to.IhaleUsulMaddeAl(ihaleKod, maddeKod);

            if (to.Hata() == "")
            {
                if (r.Read())
                {

                    f.kod = VeriErisim.DegerAl(r, 0, 0);
                    f.ihaleKod = VeriErisim.DegerAl(r, 1, 0);
                    f.adi = VeriErisim.DegerAl(r, 2, "");
                    f.aciklama = VeriErisim.DegerAl(r, 3, "");
                    f.ilanZorunlu = VeriErisim.DegerAl(r, 4, 0);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return f;
        }
    }

    /// <summary>
    /// İhale türüne ait ilan tanım bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCIhaleIlanTanim
    {
        /// <summary>
        /// Arguman olarak verilen, ihale türüne ait ilan tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri IhaleIlanTanim nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (liste.objeler.Count == 0)
                hata = TNS.HRC.HarcamaSurecleriResI.HRC00007;

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIhaleIlanTanim to = new TABIhaleIlanTanim(ve);
            ve.BeginTransaction();

            sonuc = to.Kaydet(liste);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale türüne ait ilan tanım bilgisini veritabanından okur 
        /// ve IhaleIlanTanim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="ihaleKod">İşlem yapılacak ihale tür kodu</param>
        /// <param name="ihaleMaddeSira">İşlem yapılacak ihale türüne ait madde kodu</param>
        /// <returns>İstenilen kriterdeki, ihale türüne ait ilan tanım bilgisi listesini IhaleIlanTanim nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, int ihaleKod, int ihaleMaddeSira)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABIhaleIlanTanim to = new TABIhaleIlanTanim(ve);
            IDataReader r = to.Listele(ihaleKod, ihaleMaddeSira);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    IhaleIlanTanim f = new IhaleIlanTanim();
                    f.ihaleKod = VeriErisim.DegerAl(r, 0, 0);
                    f.ihaleMaddeSira = VeriErisim.DegerAl(r, 1, 0);
                    f.aciklama = VeriErisim.DegerAl(r, 2, "");
                    f.yaklasikMaliyetMalHizmet = VeriErisim.DegerAl(r, 3, 0.0);
                    f.yaklasikMaliyetMalHizmetUst = VeriErisim.DegerAl(r, 4, 0.0);
                    f.yaklasikMaliyetYapim = VeriErisim.DegerAl(r, 5, 0.0);
                    f.yaklasikMaliyetYapimUst = VeriErisim.DegerAl(r, 6, 0.0);
                    f.zaman = VeriErisim.DegerAl(r, 7, 0);
                    f.zamanBaslamaTur = VeriErisim.DegerAl(r, 8, 0);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale türüne ait ilan tanım bilgisini veritabanından okur 
        /// ve IhaleIlanTanim nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="ihaleKod">İşlem yapılacak ihale tür kodu</param>
        /// <param name="fiyat">İşlem yapılacak fiyat bilgisi</param>
        /// <param name="isTur">İşlem yapılacak süreç işlem kodu</param>
        /// <returns>İstenilen kriterdeki, ihale türüne ait ilan tanım bilgisini IhaleIlanTanim nesnesi olarak tutan nesne</returns>
        public IhaleIlanTanim IhaleIlanSekliAl(Kullanici kullanan, int ihaleKod, double fiyat,int isTur)
        {
            IhaleIlanTanim f = new IhaleIlanTanim();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABIhaleIlanTanim to = new TABIhaleIlanTanim(ve);
            IDataReader r = to.IhaleIlanSekliAl(ihaleKod, fiyat,isTur);
            if (to.Hata() == "")
            {
                if (r.Read())
                {

                    f.ihaleKod = VeriErisim.DegerAl(r, 0, 0);
                    f.ihaleMaddeSira = VeriErisim.DegerAl(r, 1, 0);
                    f.aciklama = VeriErisim.DegerAl(r, 2, "");
                    f.yaklasikMaliyetMalHizmet = VeriErisim.DegerAl(r, 3, 0.0);
                    f.yaklasikMaliyetMalHizmetUst = VeriErisim.DegerAl(r, 4, 0.0);
                    f.yaklasikMaliyetYapim = VeriErisim.DegerAl(r, 5, 0.0);
                    f.yaklasikMaliyetYapimUst = VeriErisim.DegerAl(r, 6, 0.0);
                    f.zaman = VeriErisim.DegerAl(r, 7, 0);
                    f.zamanBaslamaTur = VeriErisim.DegerAl(r, 8, 0);

                }
            }
            r.Close();

            ve.VeriErisimKapat();

            return f;
        }
    }
}
