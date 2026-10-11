using System;
using System.Data;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;

namespace TNS.IKY.IsKurallari
{
    class IKYBelgeler
    {
        public ObjectArray ListeleBelge(Kullanici kullanan, PersonelBelgeler p)
        {
            ObjectArray pVAN = new ObjectArray();

            //if (!IKY.Yetki.YetkiVarMiOkumaya(kullanan))
            //{
            //    pVAN.sonuc.islemSonuc = false;
            //    pVAN.sonuc.hataStr = "Bilgileri görme yetkiniz yok.";

            //    return pVAN;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            TABBelgeler tab = new TABBelgeler(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListeleBelgeler(p);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            while (r.Read())
            {
                PersonelBelgeler d = new PersonelBelgeler();
                d.kod = VeriErisim.DegerAl(r, (int)TABBelgeler.ENUMAlanlar.KOD, 0);
                d.pKod = VeriErisim.DegerAl(r, (int)TABBelgeler.ENUMAlanlar.PKOD, 0);
                d.tipKayitKod = VeriErisim.DegerAl(r, (int)TABBelgeler.ENUMAlanlar.TIPKAYITKOD, 0);
                d.tip = VeriErisim.DegerAl(r, (int)TABBelgeler.ENUMAlanlar.TIP, 0);
                d.belgeAdi = VeriErisim.DegerAl(r, (int)TABBelgeler.ENUMAlanlar.BELGEADI, "");

                #region geriye dönük uyumluluk için...
                string dosyaAdi = d.belgeAdi;
                if (dosyaAdi.LastIndexOf('/') > 0)
                    dosyaAdi = dosyaAdi.Substring(dosyaAdi.LastIndexOf('/')).TrimStart('/');
                if (dosyaAdi.LastIndexOf('\\') > 0)
                    dosyaAdi = dosyaAdi.Substring(dosyaAdi.LastIndexOf('\\')).TrimStart('\\');
                d.belgeAdi = dosyaAdi;
                #endregion

                pVAN.objeler.Add(d);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek belge <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public Sonuc ListeleBelgeGetir(Kullanici kullanan, PersonelBelgeler p)
        {
            Sonuc sonuc = new Sonuc();
            if (p.kod == 0)
            {
                sonuc.islemSonuc = false;
                sonuc.bilgiStr = "Verilen kriterlere uygun belge <b>bulunamadı</b>";
                return sonuc;
            }

            //if (!IKY.Yetki.YetkiVarMiOkumaya(kullanan))
            //{
            //    pVAN.sonuc.islemSonuc = false;
            //    pVAN.sonuc.hataStr = "Bilgileri görme yetkiniz yok.";

            //    return pVAN;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            TABBelgeler tab = new TABBelgeler(ve);

            IDataReader r = null;
            try
            {
                r = tab.ListeleBelgeler(p);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.Message;

                return sonuc;
            }

            sonuc.islemSonuc = false;
            sonuc.bilgiStr = "Verilen kriterlere göre listelenecek belge <b>bulunamadı</b>.";
            while (r.Read())
            {
                string belgeAdi = VeriErisim.DegerAl(r, (int)TABBelgeler.ENUMAlanlar.BELGEADI, "");
                byte[] belge = (byte[])(r.GetValue((int)TABBelgeler.ENUMAlanlar.BELGE));

                sonuc.islemSonuc = true;
                sonuc.bilgiStr = "";

                string sonucDosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
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
            ve.VeriErisimKapat();
            return sonuc;
        }

        public Sonuc KaydetBelge(Kullanici kullanan, PersonelBelgeler p)
        {
            Sonuc sonuc = new Sonuc();

            //if (!IKY.Yetki.YetkiVarMiYazmaya(kullanan, p.tcKimlikNo))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = PersonelResI.PER011;

            //    return sonuc;
            //}

            string hata = "";
            if (p.pKod <= 0)
                hata += "Personel bilgisi boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABBelgeler tab = new TABBelgeler(ve);
            p.kod = TABGenel.SayacDegerAlInt(ve, TABBelgeler.TABLOAD);
            sonuc = tab.KaydetBelgeler(p);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc SilBelge(Kullanici kullanan, PersonelBelgeler p)
        {
            Sonuc sonuc = new Sonuc();

            //if (!IKY.Yetki.YetkiVarMiSilmeye(kullanan))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = PersonelResI.PER011;

            //    return sonuc;
            //}

            string hata = "";
            if (p.kod == 0)
                hata = "Silinecek belge seçilmemiş";   

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABBelgeler tab = new TABBelgeler(ve);
            sonuc = tab.SilBelgeler(p);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}
