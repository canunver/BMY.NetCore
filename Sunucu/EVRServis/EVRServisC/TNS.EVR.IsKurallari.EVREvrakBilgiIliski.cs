using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System;

namespace TNS.EVR.IsKurallari
{
    public class EVREvrakBilgiIliski
    {
        public static EvrakBilgiIliski[] Listele(VeriErisim veriErisim, Kullanici kullanan, EvrakBilgiIliski form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakBilgiIliski tab = new TABEvrakBilgiIliski(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                int index = (int)TABEvrakBilgiIliski.ENUMAlanlar.ONAY;
                while (dr.Read())
                {
                    form = new EvrakBilgiIliski();
                    form.evrakBilgi.prEvrakNo = VeriErisim.DegerAl(dr, (int)TABEvrakBilgiIliski.ENUMAlanlar.PREVRAKNO, "");
                    form.prEvrakNoIliski = VeriErisim.DegerAl(dr, (int)TABEvrakBilgiIliski.ENUMAlanlar.PREVRAKNOILISKI, "");
                    form.evrakNoIliski = VeriErisim.DegerAl(dr, (int)TABEvrakBilgiIliski.ENUMAlanlar.EVRAKNOILISKI, "");
                    form.onay = VeriErisim.DegerAl(dr, (int)TABEvrakBilgiIliski.ENUMAlanlar.ONAY, 0);
                    form.evrakBilgi.kullaniciId = VeriErisim.DegerAl(dr, index + 1, "");
                    form.evrakBilgi.aciklama = VeriErisim.DegerAl(dr, index + 2, "");

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (EvrakBilgiIliski[])liste.objeler.ToArray(typeof(EvrakBilgiIliski));
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, EvrakBilgiIliski form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakBilgiIliski tab = new TABEvrakBilgiIliski(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = tab.Kaydet(form);
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
                OrtakFonksiyonlar.HataStrYaz("{8D9F1F19-3B81-4D99-AAFA-DE7E16022B63} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, EvrakBilgiIliski[] formList)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");
            if (formList == null || formList.Length == 0) return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakBilgiIliski tab = new TABEvrakBilgiIliski(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = tab.Sil(new EvrakBilgiIliski() { evrakBilgi = new EvrakBilgi() { prEvrakNo = formList[0].evrakBilgi.prEvrakNo } });
                if (sonuc.islemSonuc)
                {
                    foreach (EvrakBilgiIliski form in formList)
                    {
                        sonuc = tab.Kaydet(form);
                        if (!sonuc.islemSonuc)
                            break;
                    }
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
                OrtakFonksiyonlar.HataStrYaz("{4CDD99A3-2C63-45BB-80A4-E7A0971328A5} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, EvrakBilgiIliski form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = string.Empty;
            if (string.IsNullOrEmpty(form.evrakBilgi.prEvrakNo))
                hata = "Silinecek veri bulunamadı." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakBilgiIliski tab = new TABEvrakBilgiIliski(ve);

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
                OrtakFonksiyonlar.HataStrYaz("{87279C45-9A88-441A-B196-8C7A9112876F} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static EvrakBilgiIliski Oku(VeriErisim veriErisim, Kullanici kullanan, EvrakBilgiIliski form)
        {
            if (string.IsNullOrEmpty(form.evrakBilgi.prEvrakNo) && string.IsNullOrEmpty(form.prEvrakNoIliski)) return new EvrakBilgiIliski();

            EvrakBilgiIliski[] list = EVREvrakBilgiIliski.Listele(veriErisim, kullanan, form);

            if (list != null && list.Length == 1)
                form = list[0];

            return form;
        }

    }
}
