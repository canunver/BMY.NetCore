using System.Data;
using TNS.KYM;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// 
    /// </summary>
    public class BHMv2ProjeAciklamaTur
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriBHMv2";

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ProjeAciklamaTur form)
        {
            Sonuc sonuc = Kontrol("KAYDET", null, kullanan, form);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABProjeAciklamaTur to = new TABProjeAciklamaTur(ve);

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

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, ProjeAciklamaTur form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            Sonuc sonuc = Kontrol("SIL", ve, kullanan, form);
            if (sonuc.islemSonuc)
            {
                TABProjeAciklamaTur to = new TABProjeAciklamaTur(ve);

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

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, ProjeAciklamaTur form)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABProjeAciklamaTur to = new TABProjeAciklamaTur(ve);
            IDataReader dr = to.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    ProjeAciklamaTur b = new ProjeAciklamaTur();
                    b.kod = VeriErisim.DegerAl(dr, (int)TABProjeAciklamaTur.ENUMAlanlar.KOD, "");
                    b.ad = VeriErisim.DegerAl(dr, (int)TABProjeAciklamaTur.ENUMAlanlar.AD, "");
                    b.tur = (ENUMProjeAciklamaTur)VeriErisim.DegerAl(dr, (int)TABProjeAciklamaTur.ENUMAlanlar.TUR, 0);
                    b.siraNo = VeriErisim.DegerAl(dr, (int)TABProjeAciklamaTur.ENUMAlanlar.SIRANO, 0);

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

        private static Sonuc Kontrol(string tip, VeriErisim ve, Kullanici kullanan, ProjeAciklamaTur form)
        {
            Sonuc sonuc = new Sonuc();

            if (!string.IsNullOrEmpty(form.kod))
                tip = "GUNCELLE";

            if (tip == "SIL")
            {
                if (string.IsNullOrWhiteSpace(form.kod))
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
