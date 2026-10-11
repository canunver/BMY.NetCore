using System;
using System.Data;
//using MUHServisC.ebutce.Muhasebe;
using TNS.BKD;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;
using System.Collections.Generic;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// Muhasebe-Birim işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHMudurluk
    {
        public static Sonuc Kaydet(VeriErisim ve, Kullanici kullanan, Mudurluk mudurluk)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH122, "");

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));

            ve.BeginTransaction();

            TABMudurluk to = new TABMudurluk(ve);
            TABGenel tg = new TABGenel(ve);

            if (mudurluk.kod != "")
                sonuc = to.Kaydet(mudurluk);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Bilgiler başarıyla kayıt edildi";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Muhasebe birimi sil.
        /// </summary>
        /// <param name="mudurluk">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public static Sonuc Sil(VeriErisim ve, Kullanici kullanan, Mudurluk mudurluk)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH124, "");

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));

            ve.BeginTransaction();
            TABMudurluk to = new TABMudurluk(ve);

            if (mudurluk.kod != "")
                sonuc = to.Sil(mudurluk);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "İstenen bilgi başarıyla silindi";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Muhasebe birim listele.
        /// </summary>
        /// <param name="mudurluk">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public static ObjectArray Listele(VeriErisim ve, Kullanici kullanan, Mudurluk mudurluk)
        {
            ObjectArray iVAN = new ObjectArray();

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));

            TABMudurluk to = new TABMudurluk(ve);
            IDataReader r = to.Listele(mudurluk);

            Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABButce);

            if (r != null)
            {
                while (r.Read())
                {
                    Mudurluk mb = new Mudurluk();

                    mb.kod = VeriErisim.DegerAl(r, 0, "");
                    mb.ad = VeriErisim.DegerAl(r, 1, "");
                    mb.aktif = VeriErisim.DegerAl(r, 2, 0);

                    iVAN.objeler.Add(mb);
                }
                r.Close();
            }

            ve.VeriErisimKapat();

            return iVAN;
        }
    }
}
