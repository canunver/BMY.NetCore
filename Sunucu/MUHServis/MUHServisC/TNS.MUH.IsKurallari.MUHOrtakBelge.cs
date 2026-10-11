using System.Data;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;
using System;
using System.IO;

namespace TNS.MUH.IsKurallari
{

    public class MUHOratakBelge
    {
        public static ObjectArray Listele(Kullanici kullanan, OrtakBelge form)
        {
            ObjectArray pVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOrtakBelge tab = new TABOrtakBelge(ve);

            IDataReader r = tab.ListeleBelgeler(form);
            if (r != null)
            {
                while (r.Read())
                {
                    OrtakBelge d = new OrtakBelge();
                    d.kod = VeriErisim.DegerAl(r, (int)TABOrtakBelge.ENUMAlanlar.KOD, "");
                    d.ilgiKod = VeriErisim.DegerAl(r, (int)TABOrtakBelge.ENUMAlanlar.ILGIKOD, "");
                    d.tip = (ENUMOrtakBelgeTip)VeriErisim.DegerAl(r, (int)TABOrtakBelge.ENUMAlanlar.TIP, 0);
                    d.belgeAdi = VeriErisim.DegerAl(r, (int)TABOrtakBelge.ENUMAlanlar.BELGEADI, "");

                    pVAN.objeler.Add(d);
                }
                r.Close();
                pVAN.sonuc.islemSonuc = true;
            }
            else
            {
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = tab.Hata();
            }

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek belge <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public static OrtakBelge Oku(Kullanici kullanan, OrtakBelge form)
        {
            OrtakBelge belge = new OrtakBelge();
            if (string.IsNullOrWhiteSpace(form.kod))
            {
                return belge;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOrtakBelge tab = new TABOrtakBelge(ve);

            IDataReader r = tab.ListeleBelgeler(form);

            if (r != null)
            {

                while (r.Read())
                {
                    belge.kod = VeriErisim.DegerAl(r, (int)TABOrtakBelge.ENUMAlanlar.KOD, "");
                    belge.tip = (ENUMOrtakBelgeTip)VeriErisim.DegerAl(r, (int)TABOrtakBelge.ENUMAlanlar.TIP, 0);
                    belge.ilgiKod = VeriErisim.DegerAl(r, (int)TABOrtakBelge.ENUMAlanlar.ILGIKOD, "");
                    belge.belgeAdi = VeriErisim.DegerAl(r, (int)TABOrtakBelge.ENUMAlanlar.BELGEADI, "");
                }
                r.Close();
            }

            ve.VeriErisimKapat();
            return belge;
        }

        public static Sonuc Kaydet(Kullanici kullanan, OrtakBelge form)
        {
            Sonuc sonuc = new Sonuc();

            string hata = "";
            if (form.tip <= 0)
                hata += "Belge tipi boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            byte[] icerik = null;
            icerik = form.icerik;
            form.icerik = null;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABOrtakBelge tab = new TABOrtakBelge(ve);
            form.kod = Guid.NewGuid().ToString("N");
            sonuc = tab.KaydetBelgeler(form);

            if (sonuc.islemSonuc)
            {
                string dosya = Path.Combine(form.kayitEdilecekDosyaYol, ((int)form.tip).ToString());
                if (!Directory.Exists(dosya)) Directory.CreateDirectory(dosya);
                dosya = Path.Combine(dosya, form.kod);
                try
                {
                    if (File.Exists(dosya)) File.Delete(dosya);
                }
                catch { }

                File.WriteAllBytes(dosya, icerik);

                ve.Commit();
                sonuc.bilgiStr = "Kaydetme işlemi başarıyla gerçekleştirildi.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc Sil(Kullanici kullanan, OrtakBelge form)
        {
            Sonuc sonuc = new Sonuc();

            string hata = "";
            if (string.IsNullOrWhiteSpace(form.kod))
                hata = "Silinecek belge seçilmemiş";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            OrtakBelge formOkunan = Oku(kullanan, form);
            formOkunan.kayitEdilecekDosyaYol = form.kayitEdilecekDosyaYol;

            ve.BeginTransaction();

            TABOrtakBelge tab = new TABOrtakBelge(ve);
            sonuc = tab.SilBelgeler(form);

            if (sonuc.islemSonuc)
            {
                string dosya = Path.Combine(formOkunan.kayitEdilecekDosyaYol, ((int)formOkunan.tip).ToString());
                dosya = Path.Combine(dosya, formOkunan.kod);
                try
                {
                    if (File.Exists(dosya)) File.Delete(dosya);
                }
                catch { }

                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Silme işlemi sırasında hata oluştu.";
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

    }
}
