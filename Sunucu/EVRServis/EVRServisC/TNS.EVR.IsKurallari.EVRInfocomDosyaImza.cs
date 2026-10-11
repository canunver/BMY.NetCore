using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System;

namespace TNS.EVR.IsKurallari
{
    public class EVRInfocomDosyaImza
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, InfocomDosyaImza form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            string hata = string.Empty;
            if (string.IsNullOrEmpty(form.evrEvrakNo))
                hata += "EvrEvrakNo boş olamaz." + "<br>";
            if (form.siraNo < 1)
                hata += "Evrak dosya sırano boş olamaz." + "<br>";
            if (string.IsNullOrEmpty(form.personIdnp))
                hata += "PersonIdnp bilgisi boş olamaz." + "<br>";
            if (string.IsNullOrEmpty(form.hash))
                hata += "Hash bilgisi alınamadı." + "<br>";
            if (string.IsNullOrEmpty(form.sign))
                hata += "İmza bilgisi alınamadı." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABInfocomDosyaImza tab = new TABInfocomDosyaImza(ve);
            ve.BeginTransaction();
            try
            {
                form.islemYapan = kullanan.kullaniciKodu;
                form.islemYapanAd = kullanan.ADSOYAD;
                form.islemTarih = new TNSDateTime(DateTime.Now);

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
                OrtakFonksiyonlar.HataStrYaz("{9576F2D8-470F-412A-934E-9ED5FCA9DCD6} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();

            if (sonuc.islemSonuc)
            {
                VeriErisim ve2 = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

                Sonuc sonuc2 = EVREvrak.ImzalaParafla2(ve2, kullanan, form.evrEvrakNo, new EvrakHareket() { vekil = form.vekil, vekilAd = form.vekilAd }, true, form);

                ve2.VeriErisimKapat();
            }


            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, InfocomDosyaImza form)
        {
            ObjectArray returnForm = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABInfocomDosyaImza tab = new TABInfocomDosyaImza(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    InfocomDosyaImza o = new InfocomDosyaImza();
                    o.evrEvrakNo = VeriErisim.DegerAl(dr, (int)TABInfocomDosyaImza.ENUMAlanlar.EVREVRAKNO, "");
                    o.siraNo = VeriErisim.DegerAl(dr, (int)TABInfocomDosyaImza.ENUMAlanlar.SIRANO, 0);
                    o.personIdnp = VeriErisim.DegerAl(dr, (int)TABInfocomDosyaImza.ENUMAlanlar.PERSONIDNP, "");
                    o.hash = VeriErisim.DegerAl(dr, (int)TABInfocomDosyaImza.ENUMAlanlar.HASH, "");
                    o.sign = VeriErisim.DegerAl(dr, (int)TABInfocomDosyaImza.ENUMAlanlar.SIGN, "");
                    o.islemYapan = VeriErisim.DegerAl(dr, (int)TABInfocomDosyaImza.ENUMAlanlar.ISLEMYAPAN, "");
                    o.islemYapanAd = VeriErisim.DegerAl(dr, (int)TABInfocomDosyaImza.ENUMAlanlar.ISLEMYAPANAD, "");
                    o.islemTarih = VeriErisim.DegerAl(dr, (int)TABInfocomDosyaImza.ENUMAlanlar.ISLEMTARIH);
                    o.vekil = VeriErisim.DegerAl(dr, (int)TABInfocomDosyaImza.ENUMAlanlar.VEKIL, "");
                    o.vekilAd = VeriErisim.DegerAl(dr, (int)TABInfocomDosyaImza.ENUMAlanlar.VEKILAD, "");

                    returnForm.objeler.Add(o);
                }
                dr.Close();
            }
            ve.VeriErisimKapat();

            return returnForm;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, InfocomDosyaImza form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = string.Empty;
            if (string.IsNullOrEmpty(form.evrEvrakNo))
                hata = "Silinecek veri bulunamadı." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABInfocomDosyaImza tab = new TABInfocomDosyaImza(ve);

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
                OrtakFonksiyonlar.HataStrYaz("{C8577CA9-42AF-4853-9020-0BE15FE88827} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

    }
}
