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
    public class BHMv2ProjeFinansman
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriBHMv2";

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ProjeFinansman form)
        {
            Sonuc sonuc = Kontrol("KAYDET", null, kullanan, form);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABProjeFinansman to = new TABProjeFinansman(ve);

            sonuc = to.Kaydet(form);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, ProjeFinansman form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            Sonuc sonuc = Kontrol("SIL", ve, kullanan, form);
            if (sonuc.islemSonuc)
            {
                TABProjeFinansman to = new TABProjeFinansman(ve);

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

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, ProjeFinansman form)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABProjeFinansman to = new TABProjeFinansman(ve);
            IDataReader dr = to.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    int sonSira = (int)TABProjeFinansman.ENUMAlanlar.TUTAR + 1;

                    ProjeFinansman b = new ProjeFinansman();
                    b.kod = VeriErisim.DegerAl(dr, (int)TABProjeFinansman.ENUMAlanlar.KOD, "");
                    b.projeKod = VeriErisim.DegerAl(dr, (int)TABProjeFinansman.ENUMAlanlar.PROJEKOD, "");
                    b.yil = VeriErisim.DegerAl(dr, (int)TABProjeFinansman.ENUMAlanlar.YIL, 0);
                    b.ad = VeriErisim.DegerAl(dr, (int)TABProjeFinansman.ENUMAlanlar.AD, "");
                    b.kaynak = VeriErisim.DegerAl(dr, (int)TABProjeFinansman.ENUMAlanlar.KAYNAK, 0);
                    b.tutar = VeriErisim.DegerAl(dr, (int)TABProjeFinansman.ENUMAlanlar.TUTAR, (decimal)0);

                    b.kaynakAd = VeriErisim.DegerAl(dr, sonSira++, "");

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

        private static Sonuc Kontrol(string tip, VeriErisim ve, Kullanici kullanan, ProjeFinansman form)
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
                        sonuc.hataStr += "Finansman bilgisi boş bırakılamaz." + "<br>";
                }
                if (string.IsNullOrEmpty(form.projeKod))
                    sonuc.hataStr += "Öncelikle proje bilgisini kaydeniz." + "<br>";
                if (form.yil == 0)
                    sonuc.hataStr += "Finansman yıl boş bırakılamaz." + "<br>";
                if (string.IsNullOrEmpty(form.ad))
                    sonuc.hataStr += "Finansman kaynak adı boş bırakılamaz." + "<br>";
                if (form.kaynak == 0)
                    sonuc.hataStr += "Finansman kaynak bilgisi boş bırakılamaz." + "<br>";
                if (form.tutar == 0)
                    sonuc.hataStr += "Finansman tutarı boş bırakılamaz." + "<br>";
            }

            if (!string.IsNullOrEmpty(sonuc.hataStr))
                sonuc.islemSonuc = false;
            else
                sonuc.islemSonuc = true;

            return sonuc;
        }

    }
}
