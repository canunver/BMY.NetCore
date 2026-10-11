using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System;

namespace TNS.EVR.IsKurallari
{
    public class EVREvrakImzaYolu
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, EvrakImzaYolu form)
        {
            return Kaydet(veriErisim, kullanan, form, EnumKaydet.EvrakImzaYolu);
        }
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, EvrakImzaYoluHareket form)
        {
            return Kaydet(veriErisim, kullanan, form, EnumKaydet.EvrakImzaYoluHareket);
        }
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, EvrakImzaYolu form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = string.Empty;
            if (string.IsNullOrEmpty(form.prEvrakNo))
                hata = "Silinecek veri bulunamadı." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakImzaYolu tab = new TABEvrakImzaYolu(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = tab.Sil(new EvrakImzaYoluHareket() { prEvrakNo = form.prEvrakNo });
                if (sonuc.islemSonuc)
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
                OrtakFonksiyonlar.HataStrYaz("{669D2833-51D6-4CDA-9FEB-CDC48FC083AF} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, EvrakImzaYoluHareket form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = string.Empty;
            if (string.IsNullOrEmpty(form.prEvrakNo))
                hata = "Silinecek veri bulunamadı." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakImzaYolu tab = new TABEvrakImzaYolu(ve);

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
                OrtakFonksiyonlar.HataStrYaz("{393A8A28-BB16-4604-9893-480519848FD5} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }
        public static EvrakImzaYolu Oku(VeriErisim veriErisim, Kullanici kullanan, EvrakImzaYolu form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakImzaYolu tab = new TABEvrakImzaYolu(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                ObjectArray detaylar = new ObjectArray();
                int index = 0;
                while (dr.Read())
                {
                    if (index == 0)
                    {
                        form = new EvrakImzaYolu();
                        form.prEvrakNo = VeriErisim.DegerAl(dr, (int)TABEvrakImzaYolu.ENUMAlanlar.PREVRAKNO, "");
                        form.imzaYolu.kod = VeriErisim.DegerAl(dr, (int)TABEvrakImzaYolu.ENUMAlanlar.IMZAYOLU, 0);
                        form.imzaYolu.ad = VeriErisim.DegerAl(dr, (int)TABEvrakImzaYolu.ENUMAlanlar.IMZAYOLUAD, "");
                    }

                    ImzaYoluDetay detay = new ImzaYoluDetay();
                    detay.siraNo = VeriErisim.DegerAl(dr, (int)TABEvrakImzaYolu.ENUMAlanlar.SIRANO, 0);
                    detay.islemTip = VeriErisim.DegerAl(dr, (int)TABEvrakImzaYolu.ENUMAlanlar.ISLEMTIP, 0);
                    detay.tcKimlikNo = VeriErisim.DegerAl(dr, (int)TABEvrakImzaYolu.ENUMAlanlar.TCKIMLIKNO, "");
                    detay.ad = VeriErisim.DegerAl(dr, (int)TABEvrakImzaYolu.ENUMAlanlar.AD, "");
                    detay.soyad = VeriErisim.DegerAl(dr, (int)TABEvrakImzaYolu.ENUMAlanlar.SOYAD, "");
                    detay.evrakBirimKod = VeriErisim.DegerAl(dr, (int)TABEvrakImzaYolu.ENUMAlanlar.EVRAKBIRIMKOD, "");
                    detay.unvan = VeriErisim.DegerAl(dr, (int)TABEvrakImzaYolu.ENUMAlanlar.UNVAN, "");
                    detay.akademikUnvan = VeriErisim.DegerAl(dr, (int)TABEvrakImzaYolu.ENUMAlanlar.AKADEMIKUNVAN, "");

                    detaylar.objeler.Add(detay);

                    index++;
                }
                dr.Close();

                if (detaylar.objeler.Count > 0)
                    form.imzaYolu.detaylar = (ImzaYoluDetay[])detaylar.objeler.ToArray(typeof(ImzaYoluDetay));
            }

            ve.VeriErisimKapat();

            return form;
        }
        public static EvrakImzaYoluHareket[] Listele(VeriErisim veriErisim, Kullanici kullanan, string prEvrakNo)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakImzaYolu tab = new TABEvrakImzaYolu(ve);

            IDataReader dr = tab.ListeleHareket(prEvrakNo);
            if (dr != null)
            {
                while (dr.Read())
                {
                    EvrakImzaYoluHareket hareket = new EvrakImzaYoluHareket();
                    hareket.prEvrakNo = VeriErisim.DegerAl(dr, (int)TABEvrakImzaYolu.ENUMAlanlarHareket.PREVRAKNO, "");
                    hareket.kisi1 = VeriErisim.DegerAl(dr, (int)TABEvrakImzaYolu.ENUMAlanlarHareket.KISI1, "");
                    hareket.kisi2 = VeriErisim.DegerAl(dr, (int)TABEvrakImzaYolu.ENUMAlanlarHareket.KISI2, "");
                    hareket.islemTip = VeriErisim.DegerAl(dr, (int)TABEvrakImzaYolu.ENUMAlanlarHareket.ISLEMTIP, 0);
                    hareket.islemTarih = VeriErisim.DegerAl(dr, (int)TABEvrakImzaYolu.ENUMAlanlarHareket.ISLEMTARIH);

                    liste.objeler.Add(hareket);
                }
                dr.Close();
            }


            ve.VeriErisimKapat();

            EvrakImzaYoluHareket[] arr = null;
            if (liste.objeler.Count > 0)
                arr = (EvrakImzaYoluHareket[])liste.objeler.ToArray(typeof(EvrakImzaYoluHareket));

            return arr;
        }

        private static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, object form, EnumKaydet enumKaydet)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakImzaYolu tab = new TABEvrakImzaYolu(ve);

            ve.BeginTransaction();
            try
            {
                switch (enumKaydet)
                {
                    case EnumKaydet.EvrakImzaYolu:
                        sonuc = tab.Kaydet((EvrakImzaYolu)form);
                        break;
                    case EnumKaydet.EvrakImzaYoluHareket:
                        sonuc = tab.Kaydet((EvrakImzaYoluHareket)form);
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
                OrtakFonksiyonlar.HataStrYaz("{F9F212FF-4711-4DB1-BFB5-E6FF55C6EA2B} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }
        private enum EnumKaydet { EvrakImzaYolu, EvrakImzaYoluHareket }
    }
}
