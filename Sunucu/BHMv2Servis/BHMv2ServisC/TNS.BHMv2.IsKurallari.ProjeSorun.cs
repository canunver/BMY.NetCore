using System;
using System.Data;
using TNS.KYM;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// 
    /// </summary>
    public class BHMv2ProjeSorun
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriBHMv2";

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ProjeSorun form)
        {
            Sonuc sonuc = Kontrol("KAYDET", null, kullanan, form);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABProjeSorun to = new TABProjeSorun(ve);

            sonuc = to.Kaydet(form);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, ProjeSorun form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            Sonuc sonuc = Kontrol("SIL", ve, kullanan, form);
            if (sonuc.islemSonuc)
            {
                TABProjeSorun to = new TABProjeSorun(ve);

                ve.BeginTransaction();
                sonuc = to.Sil(form);

                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, ProjeSorun form)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABProjeSorun to = new TABProjeSorun(ve);
            IDataReader dr = to.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    ProjeSorun b = new ProjeSorun();
                    b.kod = VeriErisim.DegerAl(dr, (int)TABProjeSorun.ENUMAlanlar.KOD, "");
                    b.projeKod = VeriErisim.DegerAl(dr, (int)TABProjeSorun.ENUMAlanlar.PROJEKOD, "");
                    b.tarih = VeriErisim.DegerAl(dr, (int)TABProjeSorun.ENUMAlanlar.TARIH);
                    b.sorun = VeriErisim.DegerAl(dr, (int)TABProjeSorun.ENUMAlanlar.SORUN, "");
                    b.cozumOnerisi = VeriErisim.DegerAl(dr, (int)TABProjeSorun.ENUMAlanlar.COZUMONERISI, "");

                    liste.Ekle(b);
                }
                dr.Close();
            }
            else
            {
                liste.sonuc.hataStr = "İşlem sırasında beklenmedik bir hata oluştu:" + ve.hata;
            }
            ve.VeriErisimKapat();

            return liste;
        }

        private static Sonuc Kontrol(string tip, VeriErisim ve, Kullanici kullanan, ProjeSorun form)
        {
            Sonuc sonuc = new Sonuc();

            if (tip == "SIL")
            {
                if (string.IsNullOrWhiteSpace(form.kod))
                    sonuc.hataStr = "Silinecek kaydı seçiniz.";
            }
            else if (tip == "KAYDET" || tip == "GUNCELLE")
            {
                if (tip == "GUNCELLE")
                {
                    if (string.IsNullOrEmpty(form.kod))
                        sonuc.hataStr += "Sorun bilgisi boş bırakılamaz." + "<br>";
                }
                if (string.IsNullOrEmpty(form.projeKod))
                    sonuc.hataStr += "Öncelikle proje bilgisini kaydeniz." + "<br>";
                if (string.IsNullOrEmpty(form.sorun))
                    sonuc.hataStr += "Sorun bilgisini boş bırakılamaz." + "<br>";
            }

            if (!string.IsNullOrEmpty(sonuc.hataStr))
                sonuc.islemSonuc = false;
            else
                sonuc.islemSonuc = true;

            return sonuc;
        }

    }
}
