using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System.Collections;
using System;

namespace TNS.EVR.IsKurallari
{
    public class EVRHavaleGrup
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, HavaleGrup form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            string hata = string.Empty;
            if (string.IsNullOrEmpty(form.ad))
                hata += "Havale grup adı boş bırakılamaz." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABHavaleGrup tab = new TABHavaleGrup(ve);

            ve.BeginTransaction();
            try
            {
                if (form.kod == 0) { sonuc = tab.Kaydet(form); } else { sonuc = tab.Guncelle(form); }

                if (sonuc.islemSonuc && form.kod > 0)
                {
                    sonuc = tab.SilDetay(form.kod);
                    if (sonuc.islemSonuc && form.detaylar != null && form.detaylar.Length > 0)
                    {
                        foreach (HavaleGrupDetay detay in form.detaylar)
                        {
                            detay.havaleGrupKod = form.kod;

                            sonuc = tab.Kaydet(detay);
                            if (!sonuc.islemSonuc)
                                break;
                        }
                    }

                    if (sonuc.islemSonuc)
                    {
                        sonuc = tab.SilPaylas(form.kod);
                        if (sonuc.islemSonuc && form.paylasimlar != null && form.paylasimlar.Length > 0)
                        {
                            foreach (HavaleGrupPaylas paylasim in form.paylasimlar)
                            {
                                paylasim.havaleGrupKod = form.kod;

                                sonuc = tab.Kaydet(paylasim);
                                if (!sonuc.islemSonuc)
                                    break;
                            }
                        }
                    }
                }

                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = EvrakI.EVRI095 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
                else
                {
                    sonuc.anahtar = form.kod.ToString();
                    sonuc.bilgiStr = EvrakI.EVRI094;
                    ve.Commit();
                }
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{C1C036AA-E737-4814-906B-D97F18570699} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, HavaleGrup form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = string.Empty;
            if (form.kod <= 0)
                hata = "Silinecek veri bulunamadı." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABHavaleGrup tab = new TABHavaleGrup(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = tab.Sil(form);
                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = EvrakI.EVRI097;
                }
                else
                {
                    ve.Rollback();
                    sonuc.hataStr = EvrakI.EVRI098 + "<br>" + sonuc.hataStr;
                }
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{54F34588-2569-4C94-AEA2-ADC32D76E90B} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static HavaleGrup[] Listele(VeriErisim veriErisim, Kullanici kullanan, HavaleGrup form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABHavaleGrup tab = new TABHavaleGrup(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new HavaleGrup();
                    form.kod = VeriErisim.DegerAl(dr, (int)TABHavaleGrup.ENUMAlanlar.KOD, 0);
                    form.ad = VeriErisim.DegerAl(dr, (int)TABHavaleGrup.ENUMAlanlar.AD, "");
                    form.islemYapan = VeriErisim.DegerAl(dr, (int)TABHavaleGrup.ENUMAlanlar.ISLEMYAPAN, "");
                    form.islemYapanAd = VeriErisim.DegerAl(dr, (int)TABHavaleGrup.ENUMAlanlar.ISLEMYAPANAD, "");
                    form.evrakBirimKod = VeriErisim.DegerAl(dr, (int)TABHavaleGrup.ENUMAlanlar.EVRAKBIRIMKOD, "");
                    form.paylasimDurumu = VeriErisim.DegerAl(dr, (int)TABHavaleGrup.ENUMAlanlar.PAYLASIMDURUMU, 0);
                    form.varsayilan = VeriErisim.DegerAl(dr, (int)TABHavaleGrup.ENUMAlanlar.VARSAYILAN, 0);

                    if (dr.FieldCount > 7)
                    {
                        string tcKimlikNo = VeriErisim.DegerAl(dr, (int)TABHavaleGrup.ENUMAlanlar.VARSAYILAN + 1, "");
                        if (!string.IsNullOrEmpty(tcKimlikNo))
                            form.paylasimlar = new HavaleGrupPaylas[] { new HavaleGrupPaylas() { tcKimlikNo = tcKimlikNo } };
                    }

                    if (kullanan.mernis != form.islemYapan)
                        form.ad = string.Format("{0} - ({1})", form.ad, "Paylaşım");

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            HavaleGrup[] arr = null;
            if (liste.objeler.Count > 0)
                arr = (HavaleGrup[])liste.objeler.ToArray(typeof(HavaleGrup));

            return arr;
        }
        private static HavaleGrupDetay[] ListeleDetay(VeriErisim veriErisim, Kullanici kullanan, int havaleGrupKod)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABHavaleGrup tab = new TABHavaleGrup(ve);

            IDataReader dr = tab.ListeleDetay(havaleGrupKod);
            if (dr != null)
            {
                while (dr.Read())
                {
                    HavaleGrupDetay detay = new HavaleGrupDetay();
                    detay.havaleGrupKod = VeriErisim.DegerAl(dr, (int)TABHavaleGrup.ENUMAlanlarDetay.HAVALEGRUPKOD, 0);
                    detay.siraNo = VeriErisim.DegerAl(dr, (int)TABHavaleGrup.ENUMAlanlarDetay.SIRANO, 0);
                    detay.tcKimlikNo = VeriErisim.DegerAl(dr, (int)TABHavaleGrup.ENUMAlanlarDetay.TCKIMLIKNO, "");
                    detay.ad = VeriErisim.DegerAl(dr, (int)TABHavaleGrup.ENUMAlanlarDetay.AD, "");
                    detay.soyad = VeriErisim.DegerAl(dr, (int)TABHavaleGrup.ENUMAlanlarDetay.SOYAD, "");
                    detay.evrakBirimKod = VeriErisim.DegerAl(dr, (int)TABHavaleGrup.ENUMAlanlarDetay.EVRAKBIRIMKOD, "");
                    detay.evrakBirimAd = VeriErisim.DegerAl(dr, (int)TABHavaleGrup.ENUMAlanlarDetay.EVRAKBIRIMAD, "");
                    detay.sayac = VeriErisim.DegerAl(dr, (int)TABHavaleGrup.ENUMAlanlarDetay.SAYAC, 0);

                    liste.objeler.Add(detay);
                }
                dr.Close();
            }


            ve.VeriErisimKapat();

            HavaleGrupDetay[] arr = null;
            if (liste.objeler.Count > 0)
                arr = (HavaleGrupDetay[])liste.objeler.ToArray(typeof(HavaleGrupDetay));

            return arr;
        }
        private static HavaleGrupPaylas[] ListelePaylas(VeriErisim veriErisim, Kullanici kullanan, int havaleGrupKod)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABHavaleGrup tab = new TABHavaleGrup(ve);

            IDataReader dr = tab.ListelePaylas(havaleGrupKod);
            if (dr != null)
            {
                while (dr.Read())
                {
                    HavaleGrupPaylas detay = new HavaleGrupPaylas();
                    detay.havaleGrupKod = VeriErisim.DegerAl(dr, (int)TABHavaleGrup.ENUMAlanlarPaylas.HAVALEGRUPKOD, 0);
                    detay.tcKimlikNo = VeriErisim.DegerAl(dr, (int)TABHavaleGrup.ENUMAlanlarPaylas.TCKIMLIKNO, "");
                    detay.ad = VeriErisim.DegerAl(dr, (int)TABHavaleGrup.ENUMAlanlarPaylas.AD, "");
                    detay.soyad = VeriErisim.DegerAl(dr, (int)TABHavaleGrup.ENUMAlanlarPaylas.SOYAD, "");
                    detay.evrakBirimKod = VeriErisim.DegerAl(dr, (int)TABHavaleGrup.ENUMAlanlarPaylas.EVRAKBIRIMKOD, "");
                    detay.evrakBirimAd = VeriErisim.DegerAl(dr, (int)TABHavaleGrup.ENUMAlanlarPaylas.EVRAKBIRIMAD, "");

                    liste.objeler.Add(detay);
                }
                dr.Close();
            }


            ve.VeriErisimKapat();

            HavaleGrupPaylas[] arr = null;
            if (liste.objeler.Count > 0)
                arr = (HavaleGrupPaylas[])liste.objeler.ToArray(typeof(HavaleGrupPaylas));

            return arr;
        }

        public static HavaleGrup Oku(VeriErisim veriErisim, Kullanici kullanan, HavaleGrup form)
        {
            if (form.kod == 0) return null;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            HavaleGrup[] list = EVRHavaleGrup.Listele(ve, kullanan, form);

            if (list != null && list.Length == 1)
                form = list[0];

            HavaleGrupDetay[] listDetay = ListeleDetay(ve, kullanan, form.kod);

            if (listDetay != null && listDetay.Length > 0)
                form.detaylar = listDetay;

            HavaleGrupPaylas[] listPaylas = ListelePaylas(ve, kullanan, form.kod);

            if (listPaylas != null && listPaylas.Length > 0)
                form.paylasimlar = listPaylas;

            ve.VeriErisimKapat();

            return form;
        }

        public static Sonuc VarsayilanYap(VeriErisim veriErisim, Kullanici kullanan, HavaleGrup form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABHavaleGrup tab = new TABHavaleGrup(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = tab.VarsayilanYap(form);
                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = EvrakI.EVRI095 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
                else
                {
                    sonuc.bilgiStr = "Varasayılan olarak kaydetme işlemi tamamlandı.";
                    ve.Commit();
                }
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{0FC5EF52-1CF9-4D00-B03D-1A3997AA29C9} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc PaylasimDurumuBelirle(VeriErisim veriErisim, Kullanici kullanan, HavaleGrup form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABHavaleGrup tab = new TABHavaleGrup(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = tab.PaylasimDurumuBelirle(form);
                if (sonuc.islemSonuc)
                {
                    sonuc.bilgiStr = "Birim paylaşım durumu güncellendi.";
                    ve.Commit();
                }
                else
                {
                    sonuc.hataStr = EvrakI.EVRI095 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{1023A644-2444-4AA8-995D-0F810C4BFDD4} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc SayacGuncelle(VeriErisim veriErisim, Kullanici kullanan, HavaleGrupDetay[] detaylar)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABHavaleGrup tab = new TABHavaleGrup(ve);

            ve.BeginTransaction();
            try
            {
                foreach (HavaleGrupDetay detay in detaylar)
                {
                    sonuc = tab.SayacGuncelle(detay);
                    if (!sonuc.islemSonuc)
                        break;
                }

                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = EvrakI.EVRI095 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
                else
                {
                    sonuc.bilgiStr = EvrakI.EVRI094;
                    ve.Commit();
                }
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{3ACF4AD1-C4E1-40DE-9863-73C4C88AA9EB} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

    }
}
