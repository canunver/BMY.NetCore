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
    public class BHMv2ProjeGirdi
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriBHMv2";

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ProjeGirdi form)
        {
            Sonuc sonuc = Kontrol("KAYDET", null, kullanan, form);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABProjeGirdi to = new TABProjeGirdi(ve);

            if (form.toplamMaliyet == 0)
                form.toplamMaliyet = (decimal)form.miktar * form.birimMaliyet;

            sonuc = to.Kaydet(form);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, ProjeGirdi form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            Sonuc sonuc = Kontrol("SIL", ve, kullanan, form);
            if (sonuc.islemSonuc)
            {
                TABProjeGirdi to = new TABProjeGirdi(ve);

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

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, ProjeGirdi form)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABProjeGirdi to = new TABProjeGirdi(ve);
            IDataReader dr = to.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    ProjeGirdi b = new ProjeGirdi();
                    b.kod = VeriErisim.DegerAl(dr, (int)TABProjeGirdi.ENUMAlanlar.KOD, "");
                    b.projeKod = VeriErisim.DegerAl(dr, (int)TABProjeGirdi.ENUMAlanlar.PROJEKOD, "");
                    b.tur = (ENUMProjeGirdiTur)VeriErisim.DegerAl(dr, (int)TABProjeGirdi.ENUMAlanlar.TUR, 0);
                    b.girdi = VeriErisim.DegerAl(dr, (int)TABProjeGirdi.ENUMAlanlar.GIRDI, "");
                    b.birim = VeriErisim.DegerAl(dr, (int)TABProjeGirdi.ENUMAlanlar.BIRIM, "");
                    b.miktar = VeriErisim.DegerAl(dr, (int)TABProjeGirdi.ENUMAlanlar.MIKTAR, 0.0);
                    b.birimMaliyet = VeriErisim.DegerAl(dr, (int)TABProjeGirdi.ENUMAlanlar.BIRIMMALIYET, (decimal)0);
                    b.toplamMaliyet = VeriErisim.DegerAl(dr, (int)TABProjeGirdi.ENUMAlanlar.TOPLAMMALIYET, (decimal)0);

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

        private static Sonuc Kontrol(string tip, VeriErisim ve, Kullanici kullanan, ProjeGirdi form)
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
                        sonuc.hataStr += "Girdi bilgisi boş bırakılamaz." + "<br>";
                }
                if (string.IsNullOrEmpty(form.projeKod))
                    sonuc.hataStr += "Öncelikle proje bilgisini kaydeniz." + "<br>";
                if (form.tur == 0)
                    sonuc.hataStr += "Girdi türü boş bırakılamaz." + "<br>";
                if (string.IsNullOrEmpty(form.girdi))
                    sonuc.hataStr += "Girdi adı boş bırakılamaz." + "<br>";
                if (string.IsNullOrEmpty(form.birim))
                    sonuc.hataStr += "Girdi birimi boş bırakılamaz." + "<br>";
                if (form.miktar == 0)
                    sonuc.hataStr += "Girdi miktarı boş bırakılamaz." + "<br>";
                if (form.birimMaliyet == 0)
                    sonuc.hataStr += "Girdi birim maliyeti boş bırakılamaz." + "<br>";
               
            }

            if (!string.IsNullOrEmpty(sonuc.hataStr))
                sonuc.islemSonuc = false;
            else
                sonuc.islemSonuc = true;

            return sonuc;
        }

    }
}
