using System;
using System.Collections;
using System.Data;
using TNS.BKD;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;
using System.Collections.Generic;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// 
    /// </summary>
    public class MUHPlaka
    {
        /// <summary>
        /// Kaydets the specified ve.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Plaka bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            sonuc = Kontrol("KAYDET", kullanan, bilgi);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABPlaka to = new TABPlaka(ve);

            if (bilgi.kod > 0)
                sonuc = to.Guncelle(bilgi);
            else
            {
                bilgi.kod = to.EnBuyukKod();
                sonuc = to.Kaydet(bilgi);
            }

            if (sonuc.islemSonuc)
            {
                sonuc = to.SahipSil(bilgi.kod);
                foreach (PlakaSahip fd in bilgi.plakaSahip.objeler)
                {
                    if (fd.kod == 0)
                        fd.kod = to.EnBuyukSahipKod();

                    fd.plakaKod = bilgi.kod;
                    sonuc = to.SahipKaydet(fd);
                }
            }

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = bilgi.kod.ToString();
                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Listeles the specified ve.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Plaka bilgi)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPlaka to = new TABPlaka(ve);
            IDataReader dr = null;

            try
            {
                dr = to.Listele(bilgi);
            }
            catch (Exception e)
            {
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = e.Message;
                ve.VeriErisimKapat();
                return liste;
            }

            Plaka b = new Plaka();
            int islenenPlakaKod = 0;
            int detayIndex = (int)TABPlaka.ENUMAlanlar.ACIKLAMA;
            while (dr.Read())
            {
                int kod = VeriErisim.DegerAl(dr, (int)TABPlaka.ENUMAlanlar.KOD, 0);
                if (islenenPlakaKod != kod)
                {
                    if (islenenPlakaKod != 0)
                    {
                        liste.Ekle(b);
                    }

                    b = new Plaka();
                    b.kod = VeriErisim.DegerAl(dr, (int)TABPlaka.ENUMAlanlar.KOD, 0);
                    b.plaka1 = VeriErisim.DegerAl(dr, (int)TABPlaka.ENUMAlanlar.PLAKA1, "");
                    b.plaka2 = VeriErisim.DegerAl(dr, (int)TABPlaka.ENUMAlanlar.PLAKA2, "");
                    b.plaka3 = VeriErisim.DegerAl(dr, (int)TABPlaka.ENUMAlanlar.PLAKA3, "");
                    b.aciklama = VeriErisim.DegerAl(dr, (int)TABPlaka.ENUMAlanlar.ACIKLAMA, "");
                }

                if ((islenenPlakaKod == 0 || islenenPlakaKod == b.kod) && b.kod > 0)
                {
                    PlakaSahip fd = new PlakaSahip();
                    fd.kod = VeriErisim.DegerAl(dr, detayIndex + 1, 0);
                    fd.plakaKod = VeriErisim.DegerAl(dr, detayIndex + 2, 0);
                    fd.vergiNo = VeriErisim.DegerAl(dr, detayIndex + 3, "");
                    fd.ad = VeriErisim.DegerAl(dr, detayIndex + 4, "");
                    fd.adres = VeriErisim.DegerAl(dr, detayIndex + 5, "");

                    b.plakaSahip.objeler.Add(fd);
                }

                islenenPlakaKod = b.kod;
            }

            if (b.kod > 0)
                liste.Ekle(b);

            dr.Close();
            ve.VeriErisimKapat();
            return liste;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, Plaka bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            sonuc = Kontrol("SIL", kullanan, bilgi);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABPlaka to = new TABPlaka(ve);

            if (bilgi.kod > 0)
            {
                sonuc = to.SahipSil(bilgi.kod);
                if (sonuc.islemSonuc)
                    sonuc = to.Sil(bilgi.kod);
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Kontrols the specified tip.
        /// </summary>
        /// <param name="tip">The tip.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        private static Sonuc Kontrol(string tip, Kullanici kullanan, Plaka bilgi)
        {
            Sonuc sonuc = new Sonuc();

            if (tip == "SIL")
            {
                if (bilgi.kod == 0)
                    sonuc.hataStr = "Silinecek kaydı seçiniz.";
            }

            if (tip == "KAYDET" || tip == "GUNCELLE")
            {
                if (string.IsNullOrEmpty(bilgi.plaka1.Trim()) || string.IsNullOrEmpty(bilgi.plaka2.Trim()) || string.IsNullOrEmpty(bilgi.plaka3))
                    sonuc.hataStr = "Plaka bilgisini boş bırakılamaz." + "<br>";
            }

            if (tip == "IPTAL")
            {
                if (bilgi.kod == 0)
                    sonuc.hataStr = "İlk önce kayıtlı plaka seçiniz." + "<br>";
            }

            if (!string.IsNullOrEmpty(sonuc.hataStr))
                sonuc.islemSonuc = false;
            else
                sonuc.islemSonuc = true;

            return sonuc;
        }
    }
}
