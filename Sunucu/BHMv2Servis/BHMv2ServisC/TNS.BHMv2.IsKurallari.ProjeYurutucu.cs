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
    public class BHMv2ProjeYurutucu
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriBHMv2";

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ProjeYurutucu form)
        {
            Sonuc sonuc = Kontrol("KAYDET", null, kullanan, form);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABProjeYurutucu to = new TABProjeYurutucu(ve);

            sonuc = to.Kaydet(form);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, ProjeYurutucu form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            Sonuc sonuc = Kontrol("SIL", ve, kullanan, form);
            if (sonuc.islemSonuc)
            {
                TABProjeYurutucu to = new TABProjeYurutucu(ve);

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

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, ProjeYurutucu form)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABProjeYurutucu to = new TABProjeYurutucu(ve);
            IDataReader dr = to.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    ProjeYurutucu b = new ProjeYurutucu();
                    b.projeKod = VeriErisim.DegerAl(dr, (int)TABProjeYurutucu.ENUMAlanlar.PROJEKOD, "");
                    b.tcKimlikNo = VeriErisim.DegerAl(dr, (int)TABProjeYurutucu.ENUMAlanlar.TCKIMLIKNO, "");
                    b.adSoyad = VeriErisim.DegerAl(dr, (int)TABProjeYurutucu.ENUMAlanlar.ADSOYAD, "");
                    b.kurum = VeriErisim.DegerAl(dr, (int)TABProjeYurutucu.ENUMAlanlar.KURUM, "");
                    b.unvan = VeriErisim.DegerAl(dr, (int)TABProjeYurutucu.ENUMAlanlar.UNVAN, "");
                    b.tel = VeriErisim.DegerAl(dr, (int)TABProjeYurutucu.ENUMAlanlar.TEL, "");
                    b.eposta = VeriErisim.DegerAl(dr, (int)TABProjeYurutucu.ENUMAlanlar.EPOSTA, "");
                    b.projedekiRolu = VeriErisim.DegerAl(dr, (int)TABProjeYurutucu.ENUMAlanlar.PROJEDEKIROLU, "");

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

        private static Sonuc Kontrol(string tip, VeriErisim ve, Kullanici kullanan, ProjeYurutucu form)
        {
            Sonuc sonuc = new Sonuc();

            if (tip == "SIL")
            {
                if (string.IsNullOrWhiteSpace(form.projeKod))
                    sonuc.hataStr = "Silinecek kaydı seçiniz.";
            }

            if (tip == "KAYDET" || tip == "GUNCELLE")
            {
                if (string.IsNullOrEmpty(form.projeKod))
                    sonuc.hataStr += "Öncelikle proje bilgisini kaydeniz." + "<br>";
                if (string.IsNullOrEmpty(form.tcKimlikNo))
                    sonuc.hataStr += "TC Kimlik No bilgisi boş bırakılamaz." + "<br>";
            }

            if (!string.IsNullOrEmpty(sonuc.hataStr))
                sonuc.islemSonuc = false;
            else
                sonuc.islemSonuc = true;

            return sonuc;
        }

    }
}
