using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.EVR;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using EVRServisI;

namespace TNS.EVR.IsKurallari
{
    /// <summary>
    /// Evrak dosya planı bilgileriyle ilgili, kaydetme ve okuma yordamlarını tutan sınıf. 
    /// </summary>
    public class EVRGenel
    {
        public static string DosyaPlaniAdGetir(string dosyaKodu)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriEVR");
            TABGenel to = new TABGenel(ve);
            string donenDeger = to.DosyaPlaniAdGetir(dosyaKodu);
            ve.VeriErisimKapat();
            return donenDeger;
        }

        public static string KullaniciAdGetir(string tcKimlikNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM");
            TABGenel to = new TABGenel(ve);
            string donenDeger = to.KullaniciAdGetir(tcKimlikNo);
            ve.VeriErisimKapat();
            return donenDeger;
        }

        public static bool KullaniciAktifMi(string tcKimlikNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM");
            TABGenel to = new TABGenel(ve);
            bool donenDeger = to.KullaniciAktifMi(tcKimlikNo);
            ve.VeriErisimKapat();
            return donenDeger;
        }

        public static string KullaniciAdGetir(VeriErisim veKYM, string tcKimlikNo)
        {
            TABGenel to = new TABGenel(veKYM);
            string donenDeger = to.KullaniciAdGetir(tcKimlikNo);
            return donenDeger;
        }

        public static string EvrakBirimAdGetir(string evrakBirimKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriEVR");
            TABGenel to = new TABGenel(ve);
            string donenDeger = to.EvrakBirimAdGetir(evrakBirimKod);
            ve.VeriErisimKapat();
            return donenDeger;
        }
        public static string EvrakBirimAdGetir(int idariKimlikKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriEVR");
            TABGenel to = new TABGenel(ve);
            string donenDeger = to.EvrakBirimAdGetir(idariKimlikKod);
            ve.VeriErisimKapat();
            return donenDeger;
        }

        public static string EvrakBirimKisaAdGetir(string evrakBirimKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriEVR");
            TABGenel to = new TABGenel(ve);
            string donenDeger = to.EvrakBirimKisaAdGetir(evrakBirimKod);
            ve.VeriErisimKapat();
            return donenDeger;
        }

        public static string EvrakGelisAdGetir(string kod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriEVR");
            TABGenel to = new TABGenel(ve);
            string donenDeger = to.EvrakGelisAdGetir(kod);
            ve.VeriErisimKapat();
            return donenDeger;
        }

        public static int EvrakTipGetir(string evrEvrakNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriEVR");
            TABGenel to = new TABGenel(ve);
            int donenDeger = to.EvrakTipGetir(evrEvrakNo);
            ve.VeriErisimKapat();
            return donenDeger;
        }
    }
}
