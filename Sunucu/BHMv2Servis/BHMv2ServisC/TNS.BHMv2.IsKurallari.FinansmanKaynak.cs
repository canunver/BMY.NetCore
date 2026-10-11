using System.Data;
using TNS.KYM;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// 
    /// </summary>
    public class BHMv2FinansmanKaynak
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriBHMv2";

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, FinansmanKaynak form)
        {
            Sonuc sonuc = Kontrol("KAYDET", null, kullanan, form);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABFinansmanKaynak to = new TABFinansmanKaynak(ve);

            sonuc = to.Kaydet(form);

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = form.kod.ToString();
                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, FinansmanKaynak form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            Sonuc sonuc = Kontrol("SIL", ve, kullanan, form);
            if (sonuc.islemSonuc)
            {
                TABFinansmanKaynak to = new TABFinansmanKaynak(ve);

                ve.BeginTransaction();
                sonuc = to.Sil(form.kod);

                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, FinansmanKaynak form)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABFinansmanKaynak to = new TABFinansmanKaynak(ve);
            IDataReader dr = to.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    FinansmanKaynak b = new FinansmanKaynak();
                    b.kod = VeriErisim.DegerAl(dr, (int)TABFinansmanKaynak.ENUMAlanlar.KOD, 0);
                    b.ad = VeriErisim.DegerAl(dr, (int)TABFinansmanKaynak.ENUMAlanlar.AD, "");

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

        private static Sonuc Kontrol(string tip, VeriErisim ve, Kullanici kullanan, FinansmanKaynak form)
        {
            Sonuc sonuc = new Sonuc();

            if (form.kod > 0)
                tip = "GUNCELLE";

            if (tip == "SIL")
            {
                if (form.kod > 0)
                    sonuc.hataStr = "Silinecek kaydı seçiniz.";
            }
            else if (tip == "KAYDET" || tip == "GUNCELLE")
            {
                if (string.IsNullOrEmpty(form.ad))
                    sonuc.hataStr += "Ad bilgisi boş bırakılamaz." + "<br>";
            }

            if (!string.IsNullOrEmpty(sonuc.hataStr))
                sonuc.islemSonuc = false;
            else
                sonuc.islemSonuc = true;

            return sonuc;
        }

    }
}
