using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System;

namespace TNS.EVR.IsKurallari
{
    public class EVRKEPGonderi
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, KEPGonderi form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            string hata = string.Empty;
            if (string.IsNullOrEmpty(form.kimden))
                hata += "Gönderi sahibi boş bırakılamaz." + "<br>";
            if (string.IsNullOrEmpty(form.kime))
                hata += "Kime bilgisi boş bırakılamaz." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABKEPGonderi tab = new TABKEPGonderi(ve);
            ve.BeginTransaction();
            try
            {
                if (form.kod == "")
                {
                    form.kod = Guid.NewGuid().ToString();
                    sonuc = tab.Kaydet(form);
                }
                else
                    sonuc = tab.Guncelle(form);

                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = EvrakI.EVRI095 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
                else
                {
                    sonuc.bilgiStr = EvrakI.EVRI094;
                    sonuc.anahtar = form.kod;
                    ve.Commit();
                }

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{D36D4087-4B53-4020-A673-80FDF2E6584D} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc KaydetSMIME(VeriErisim veriErisim, Kullanici kullanan, KEPGonderi form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            string hata = string.Empty;
            if (string.IsNullOrEmpty(form.kod))
                hata += "Kod boş bırakılamaz." + "<br>";
            if (string.IsNullOrEmpty(form.smime))
                hata += "SMIME boş bırakılamaz." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABKEPGonderi tab = new TABKEPGonderi(ve);
            ve.BeginTransaction();
            try
            {
                sonuc = tab.GuncelleSMIME(form);

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
                OrtakFonksiyonlar.HataStrYaz("{D36D4087-4B53-4020-A673-80FDF2E65843} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc KaydetDurum(VeriErisim veriErisim, Kullanici kullanan, KEPGonderi form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            string hata = string.Empty;
            if (form.durum == 0)
                hata += "Durum boş bırakılamaz." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABKEPGonderi tab = new TABKEPGonderi(ve);
            ve.BeginTransaction();
            try
            {
                sonuc = tab.GuncelleDurum(form);

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
                OrtakFonksiyonlar.HataStrYaz("{D36D4087-4B53-4020-A673-80FDF2E6584XX} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, KEPGonderi form)
        {
            ObjectArray returnForm = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABKEPGonderi tab = new TABKEPGonderi(ve);
            IDataReader dr = tab.Oku(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    KEPGonderi l = new KEPGonderi();
                    l.kod = VeriErisim.DegerAl(dr, (int)TABKEPGonderi.ENUMAlanlar.KOD, "");
                    l.evrakBirimKod = VeriErisim.DegerAl(dr, (int)TABKEPGonderi.ENUMAlanlar.EVRAKBIRIMKODU, "");
                    l.evrEvrakNo = VeriErisim.DegerAl(dr, (int)TABKEPGonderi.ENUMAlanlar.EVREVRAKNO, "");
                    l.kimden = VeriErisim.DegerAl(dr, (int)TABKEPGonderi.ENUMAlanlar.KIMDEN, "");
                    l.kime = VeriErisim.DegerAl(dr, (int)TABKEPGonderi.ENUMAlanlar.KIME, "");
                    l.ilgiKisi = VeriErisim.DegerAl(dr, (int)TABKEPGonderi.ENUMAlanlar.ILGIKISI, "");
                    l.konu = VeriErisim.DegerAl(dr, (int)TABKEPGonderi.ENUMAlanlar.KONU, "");
                    l.tarih = VeriErisim.DegerAl(dr, (int)TABKEPGonderi.ENUMAlanlar.TARIH);
                    l.islemYapan = VeriErisim.DegerAl(dr, (int)TABKEPGonderi.ENUMAlanlar.ISLEMYAPAN, "");
                    l.durum = VeriErisim.DegerAl(dr, (int)TABKEPGonderi.ENUMAlanlar.DURUM, 0);
                    l.mime = VeriErisim.DegerAl(dr, (int)TABKEPGonderi.ENUMAlanlar.MIME, "");
                    l.smime = VeriErisim.DegerAl(dr, (int)TABKEPGonderi.ENUMAlanlar.SMIME, "");

                    returnForm.objeler.Add(l);
                }
                dr.Close();
            }
            ve.VeriErisimKapat();

            return returnForm;
        }
    }
}
