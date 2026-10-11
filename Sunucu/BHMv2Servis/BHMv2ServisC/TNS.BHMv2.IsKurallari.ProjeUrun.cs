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
    public class BHMv2ProjeUrun
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriBHMv2";

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ProjeUrun form)
        {
            Sonuc sonuc = Kontrol("KAYDET", null, kullanan, form);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABProjeUrun to = new TABProjeUrun(ve);

            sonuc = to.Kaydet(form);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, ProjeUrun form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            Sonuc sonuc = Kontrol("SIL", ve, kullanan, form);
            if (sonuc.islemSonuc)
            {
                TABProjeUrun to = new TABProjeUrun(ve);

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

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, ProjeUrun form)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABProjeUrun to = new TABProjeUrun(ve);
            IDataReader dr = to.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    int sonSira = (int)TABProjeUrun.ENUMAlanlar.URUNKOD + 1;

                    ProjeUrun b = new ProjeUrun();
                    b.projeKod = VeriErisim.DegerAl(dr, (int)TABProjeUrun.ENUMAlanlar.PROJEKOD, "");
                    b.urunKod = VeriErisim.DegerAl(dr, (int)TABProjeUrun.ENUMAlanlar.URUNKOD, "");

                    b.urunAd = VeriErisim.DegerAl(dr, sonSira++, "");
                    b.urunGrupAd = VeriErisim.DegerAl(dr, sonSira++, "");

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

        private static Sonuc Kontrol(string tip, VeriErisim ve, Kullanici kullanan, ProjeUrun form)
        {
            Sonuc sonuc = new Sonuc();

            if (tip == "SIL")
            {
                if (string.IsNullOrWhiteSpace(form.projeKod) || string.IsNullOrWhiteSpace(form.urunKod))
                    sonuc.hataStr = "Silinecek kaydı seçiniz.";
            }
            else if (tip == "KAYDET" || (tip == "GUNCELLE"))
            {
                if (string.IsNullOrEmpty(form.projeKod))
                    sonuc.hataStr += "Öncelikle proje bilgisini kaydeniz." + "<br>";
                if (string.IsNullOrEmpty(form.urunKod))
                    sonuc.hataStr += "Urun bilgisi boş bırakılamaz." + "<br>";
            }

            if (!string.IsNullOrEmpty(sonuc.hataStr))
                sonuc.islemSonuc = false;
            else
                sonuc.islemSonuc = true;

            return sonuc;
        }

    }
}
