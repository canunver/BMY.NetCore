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
    public class BHMv2ProjeFaaliyetGosterge
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriBHMv2";

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ProjeFaaliyetGosterge form)
        {
            Sonuc sonuc = Kontrol("KAYDET", null, kullanan, form);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABProjeFaaliyetGosterge to = new TABProjeFaaliyetGosterge(ve);

            sonuc = to.Kaydet(form);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, ProjeFaaliyetGosterge form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            Sonuc sonuc = Kontrol("SIL", ve, kullanan, form);
            if (sonuc.islemSonuc)
            {
                TABProjeFaaliyetGosterge to = new TABProjeFaaliyetGosterge(ve);

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

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, ProjeFaaliyetGosterge form)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABProjeFaaliyetGosterge to = new TABProjeFaaliyetGosterge(ve);
            IDataReader dr = to.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    int sonSira = (int)TABProjeFaaliyetGosterge.ENUMAlanlar.PLANLANANDEGER + 1;

                    ProjeFaaliyetGosterge b = new ProjeFaaliyetGosterge();
                    b.kod = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyetGosterge.ENUMAlanlar.KOD, "");
                    b.faaliyetKod = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyetGosterge.ENUMAlanlar.FAALIYETKOD, "");
                    b.yil = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyetGosterge.ENUMAlanlar.YIL, 0);
                    b.gosterge = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyetGosterge.ENUMAlanlar.GOSTERGE, "");
                    b.birim = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyetGosterge.ENUMAlanlar.BIRIM, "");
                    b.planlananDeger = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyetGosterge.ENUMAlanlar.PLANLANANDEGER, 0.0);

                    b.faaliyetAd = VeriErisim.DegerAl(dr, sonSira++, "");

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

        private static Sonuc Kontrol(string tip, VeriErisim ve, Kullanici kullanan, ProjeFaaliyetGosterge form)
        {
            Sonuc sonuc = new Sonuc();

            if (tip == "SIL")
            {
                if (string.IsNullOrWhiteSpace(form.kod))
                    sonuc.hataStr = "Silinecek kaydı seçiniz.";
            }
            else if (tip == "KAYDET" || tip == "GUNCELLE")
            {
                if (string.IsNullOrEmpty(form.faaliyetKod))
                    sonuc.hataStr += "Öncelikle faaliyet bilgisini kaydeniz." + "<br>";
                if (string.IsNullOrEmpty(form.gosterge))
                    sonuc.hataStr += "Göstege bilgisi boş bırakılamaz." + "<br>";
            }

            if (!string.IsNullOrEmpty(sonuc.hataStr))
                sonuc.islemSonuc = false;
            else
                sonuc.islemSonuc = true;

            return sonuc;
        }

    }
}
