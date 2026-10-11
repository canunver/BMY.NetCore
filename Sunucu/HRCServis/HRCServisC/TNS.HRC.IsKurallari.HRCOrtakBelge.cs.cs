using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.HRC.IsKurallari
{
    public class HRCOratakBelge
    {
        public static ObjectArray Listele(Kullanici kullanan, OrtakBelge form)
        {
            ObjectArray pVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
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

        public static Sonuc Oku(Kullanici kullanan, OrtakBelge form)
        {
            Sonuc sonuc = new Sonuc();
            if (string.IsNullOrWhiteSpace(form.kod))
            {
                sonuc.islemSonuc = false;
                sonuc.bilgiStr = "Verilen kriterlere uygun belge <b>bulunamadı</b>";
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOrtakBelge tab = new TABOrtakBelge(ve);

            IDataReader r = tab.ListeleBelgeler(form);

            sonuc.islemSonuc = false;
            sonuc.bilgiStr = "Verilen kriterlere göre listelenecek belge <b>bulunamadı</b>.";

            if (r != null)
            {

                while (r.Read())
                {
                    string belgeAdi = VeriErisim.DegerAl(r, (int)TABOrtakBelge.ENUMAlanlar.BELGEADI, "");
                    byte[] belge = (byte[])(r.GetValue((int)TABOrtakBelge.ENUMAlanlar.BELGE));

                    sonuc.islemSonuc = true;
                    sonuc.bilgiStr = "";

                    string sonucDosyaAd = System.IO.Path.GetTempFileName();
                    System.IO.FileStream dosya = new System.IO.FileStream(sonucDosyaAd, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                    dosya.Write(belge, 0, belge.Length);
                    dosya.Close();

                    string uzanti = "xls";
                    if (belgeAdi.Contains("."))
                        uzanti = belgeAdi.Substring(belgeAdi.LastIndexOf('.') + 1);

                    sonuc.anahtar = sonucDosyaAd + "\n" + belgeAdi + "\n" + uzanti;
                    //OrtakClass.DosyaIslem.DosyaGonderX(sonucDosyaAd, belgeAdi, true, uzanti);
                }
                r.Close();
            }
            else
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = tab.Hata();
            }

            ve.VeriErisimKapat();
            return sonuc;
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

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABOrtakBelge tab = new TABOrtakBelge(ve);
            form.kod = Guid.NewGuid().ToString("N");
            sonuc = tab.KaydetBelgeler(form);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Kayıt işlemi başarıyla gerçekleşti.";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Kayıt işlemi sırasında hata oluştu.";
            }

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

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABOrtakBelge tab = new TABOrtakBelge(ve);
            sonuc = tab.SilBelgeler(form);

            if (sonuc.islemSonuc)
            {
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
