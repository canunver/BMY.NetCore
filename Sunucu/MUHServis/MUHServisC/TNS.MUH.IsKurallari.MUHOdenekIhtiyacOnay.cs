using System;
using System.Data;
using TNS.BKD;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.MUH.IsKurallari
{
    /// <summary> 
    /// Odenek ihtiyaç işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHOdenekIhtiyacOnay
    {
        static Tertip to = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdenek);

        /// <summary>
        /// Ödenek ihtiyaç onay kaydet.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="onay">odenek ihtiyac onay</param>
        /// <returns></returns>
        public Sonuc Kaydet(Kullanici kullanan, OdenekIhtiyacOnay onay)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            string hata = string.Empty;
            ObjectArray kayitYapmayanlar = ListeleKayitYapmayanBirimler(kullanan, onay);
            if (kayitYapmayanlar.objeler.Count > 0)
            {
                foreach (OdenekIhtiyac kayitYapmayanBirim in kayitYapmayanlar.objeler)
                    hata += string.Format(ButceMuhasebeResI.MUH185, kayitYapmayanBirim.birim) + "<br>";
            }

            if (onay.yil <= 0)
                hata = ButceMuhasebeResI.MUH082 + "<br>";
            if (onay.muhasebe <= 0)
                hata += ButceMuhasebeResI.MUH083 + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            ve.BeginTransaction();
            TABOdenekIhtiyacOnay tab = new TABOdenekIhtiyacOnay(ve);

            sonuc = tab.Kaydet(onay);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH186;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Ödenek ihtiyaç onay listele.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="onay">odenek ihtiyac onay</param>
        /// <returns></returns>
        public ObjectArray Listele(Kullanici kullanan, OdenekIhtiyacOnay onay)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdenekIhtiyacOnay tab = new TABOdenekIhtiyacOnay(ve);

            IDataReader dr = tab.Listele(onay);

            while (dr.Read())
            {
                OdenekIhtiyacOnay m = new OdenekIhtiyacOnay();
                m.yil = VeriErisim.DegerAl(dr, 0, 0);
                m.muhasebe = VeriErisim.DegerAl(dr, 1, 0);
                m.asama = VeriErisim.DegerAl(dr, 2, 0);
                m.onay = VeriErisim.DegerAl(dr, 3, 0);
                m.kayitTarihi = VeriErisim.DegerAl(dr, 4);
                m.kayitEden = VeriErisim.DegerAl(dr, 5, "");

                bilgi.objeler.Add(m);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return bilgi;
        }

        private ObjectArray ListeleKayitYapmayanBirimler(Kullanici kullanan, OdenekIhtiyacOnay onay)
        {
            ObjectArray birimler = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdenekIhtiyacOnay tab = new TABOdenekIhtiyacOnay(ve);

            IDataReader dr = tab.ListeleKayitYapmayanBirimler(onay);

            while (dr.Read())
            {
                OdenekIhtiyac m = new OdenekIhtiyac();
                m.birim = VeriErisim.DegerAl(dr, 0, "");

                birimler.objeler.Add(m);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return birimler;
        }
    }
}