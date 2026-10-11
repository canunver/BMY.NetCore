using System;
using System.Collections.Generic;
using System.Text;
using TNS.KYM;
using Mernis.KPS.Types.KPSWse;

namespace TNS.KPM.IsKurallari
{
    public class KPMIlce
    {
        public static ObjectArray IlceListele(Kullanici kullanan)
        {
            return IlceListele(kullanan, 0);
        }

        public static ObjectArray IlceListele(Kullanici kullanan, int ilKod)
        {
            ObjectArray ilceVAN = new ObjectArray();

            //Mernis servisten çekiyoruz ilçeleri
            KPS servis = Mernis.KPS.Types.KPSServis.Create();
            servis.Proxy = OrtakFonksiyonlar.ProxyAyarla();

            IlceBilgisi[] ilceler = null;
            if (ilKod == 0)
                ilceler = servis.IlceListesiGetir();
            else
                ilceler = servis.IleBagliIlceListesiGetir(OrtakFonksiyonlar.ConvertToInt(ilKod, 0));

            foreach (IlceBilgisi ilceB in ilceler)
            {
                Ilce ilce = new Ilce(ilceB);
                ilceVAN.objeler.Add(ilce);
            }

            if (ilceVAN.objeler.Count <= 0)
                ilceVAN.sonuc.bilgiStr = "Verilen kriterlere uygun kayıt bulunamadı.";

            return ilceVAN;
        }

        public static ObjectArray KoyListele(Kullanici kullanan, int ilKod, int ilceKod)
        {
            //Köy listesinin Tarım Mernisden alınması işlemi çok önce kapatıldı. Melih 14.07.2023

            ObjectArray liste = new ObjectArray();
            //string logDosyaAdi = "koyaktarim" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00") + ".txt";

            //TarimMernis.BirlesikKutukSorgulamaService mernis = new TarimMernis.BirlesikKutukSorgulamaService();

            //string mernisKullanici = System.Configuration.ConfigurationManager.AppSettings.Get("Mernisv2User");
            //string mernisParola = OrtakFonksiyonlar.SifreliCozOrtakAnahtar(System.Configuration.ConfigurationManager.AppSettings.Get("Mernisv2Password"));

            //OrtakFonksiyonlar.HataDosyaYaz(logDosyaAdi, "TRM_Mernis bağlanıyor-->" + mernisKullanici + "*" + mernisParola + "TRM_Mernis Sorgulayan Kimlik No-->" + kullanan.mernis + "TRM_Mernis IP-->" + kullanan.telefon, false);

            //var mernisModelKullanici = new TarimMernis.Kullanici();

            //try
            //{
            //    mernisModelKullanici = new TarimMernis.Kullanici
            //    {
            //        KullaniciAdi = mernisKullanici,
            //        Sifre = mernisParola,
            //        SorgulayanTcNo = long.Parse(kullanan.mernis),
            //        SorgulayanIp = kullanan.telefon//TODO: sorgulayan kişinin local ip si
            //    };
            //}
            //catch (Exception e)
            //{
            //    OrtakFonksiyonlar.HataDosyaYaz(logDosyaAdi, "TRM_Mernis Model hata-->" + e.Message, false);
            //    return liste;
            //}

            //OrtakFonksiyonlar.HataDosyaYaz(logDosyaAdi, "TRM_Mernis bağlandı", false);

            //kps.nvi.gov.tr._2011._01._011.AksOkuTumIlceyeBagliMahalleSorguKriteri kriter = new kps.nvi.gov.tr._2011._01._011.AksOkuTumIlceyeBagliMahalleSorguKriteri();
            //kps.nvi.gov.tr._2011._01._011.MahalleSonucu koySonucu = new kps.nvi.gov.tr._2011._01._011.MahalleSonucu();

            //try
            //{
            //    koySonucu = mernis.AksOkuTumIlceyeBagliMahalleServis(kriter, mernisModelKullanici);
            //}
            //catch (Exception e)
            //{
            //    OrtakFonksiyonlar.HataDosyaYaz(logDosyaAdi, "TRM_Mernis hata-->" + e.Message, false);
            //    return liste;
            //}

            //if (koySonucu == null) return liste;
            //if (koySonucu.SorguSonucu.Length == 0) return liste;

            //OrtakFonksiyonlar.HataDosyaYaz(logDosyaAdi, "TRM_Mernis 4", false);

            //if (koySonucu.HataBilgisi != null)
            //    OrtakFonksiyonlar.HataDosyaYaz(logDosyaAdi, "TRM_Mernis 5-->" + koySonucu.HataBilgisi.Aciklama, false);

            //if (koySonucu.HataBilgisi == null)
            //{
            //    OrtakFonksiyonlar.HataDosyaYaz(logDosyaAdi, "TRM_Mernis 6", false);

            //    var koyListe = koySonucu.SorguSonucu;

            //    OrtakFonksiyonlar.HataDosyaYaz(logDosyaAdi, "TRM_Mernis 7 - Liste sayısı" + koyListe.Length, false);

            //    if (koyListe == null || koyListe.Length == 0)
            //    {
            //        OrtakFonksiyonlar.HataDosyaYaz(logDosyaAdi, "Liste Hata:Liste Boş", false);
            //    }

            //    if (koyListe[0].HataBilgisi != null && !string.IsNullOrEmpty(koyListe[0].HataBilgisi.Aciklama))
            //    {
            //        OrtakFonksiyonlar.HataDosyaYaz(logDosyaAdi, "Liste Hata:" + koyListe[0].HataBilgisi.Aciklama, false);
            //    }

            //    string donenDeger = "";
            //    foreach (kps.nvi.gov.tr._2011._01._011.Mahalle bilgi in koyListe)
            //    {
            //        donenDeger = bilgi.KoyKod.Value + "|" + bilgi.Ad;
            //        liste.objeler.Add(donenDeger);

            //        string gelenBilgi = "Kod:" + bilgi.Kod.Value
            //                 + "*Ad:" + bilgi.Ad
            //                 + "*KoyKod:" + bilgi.KoyKod.Value
            //                 + "*TipKod:" + bilgi.Tipi.Kod.Value
            //                 + "*TipAd:" + bilgi.Tipi.Aciklama
            //                 + "*STN:" + bilgi.SabitTanitimNumarasi.Value
            //                 + "*YIK:" + bilgi.YetkiliIdareKodu.Value;

            //        OrtakFonksiyonlar.HataDosyaYaz(logDosyaAdi, gelenBilgi, false);
            //    }
            //}

            //OrtakFonksiyonlar.HataDosyaYaz(logDosyaAdi, "Liste Bitti", false);
            return liste;
        }

        public static ObjectArray BucakListele(Kullanici kullanan, int ilKod, int ilceKod)
        {
            ObjectArray liste = new ObjectArray();
            OrtakFonksiyonlar.HataStrYaz("1");
            try
            {
                Mernis.KPSv2.Types.AksOkuTumIlceyeBagliMahalleServis.AksOkuTumIlceyeBagliMahalleServis adresServis = Mernis.KPSv2.Types.KPSv2Servis.AksOkuTumIlceyeBagliMahalleServisOlustur();
                OrtakFonksiyonlar.HataStrYaz("2");
                Mernis.KPSv2.Types.AksOkuTumIlceyeBagliMahalleServis.AksOkuTumIlceyeBagliMahalleSorguKriteri kriter = new Mernis.KPSv2.Types.AksOkuTumIlceyeBagliMahalleServis.AksOkuTumIlceyeBagliMahalleSorguKriteri();
                kriter.IlceKodu = 2032;

                OrtakFonksiyonlar.HataStrYaz("3");

                Mernis.KPSv2.Types.AksOkuTumIlceyeBagliMahalleServis.MahalleSonucu koySonucu = adresServis.Listele(kriter);
                OrtakFonksiyonlar.HataStrYaz("4");

                if (koySonucu.HataBilgisi != null)
                    OrtakFonksiyonlar.HataStrYaz("5" + koySonucu.HataBilgisi.Aciklama);
                if (koySonucu.HataBilgisi == null)
                {
                    OrtakFonksiyonlar.HataStrYaz("6");

                    var koyListe = koySonucu.SorguSonucu;

                    OrtakFonksiyonlar.HataStrYaz("7-Liste sayısı" + koyListe.Length);

                    if (koyListe == null || koyListe.Length == 0)
                    {
                        OrtakFonksiyonlar.HataStrYaz("Liste Hata:Liste Boş");
                    }

                    if (koyListe[0].HataBilgisi != null && !string.IsNullOrEmpty(koyListe[0].HataBilgisi.Aciklama))
                    {
                        OrtakFonksiyonlar.HataStrYaz("Liste Hata:" + koyListe[0].HataBilgisi.Aciklama);
                    }

                    foreach (Mernis.KPSv2.Types.AksOkuTumIlceyeBagliMahalleServis.Mahalle bilgi in koyListe)
                    {
                        OrtakFonksiyonlar.HataStrYaz("Kod:" + bilgi.Kod.Value
                            + "*Ad:" + bilgi.Ad
                            + "*KoyKod:" + bilgi.KoyKod.Value
                            + "*TipKod:" + bilgi.Tipi.Kod.Value
                            + "*TipAd:" + bilgi.Tipi.Aciklama
                            + "*STN:" + bilgi.SabitTanitimNumarasi.Value
                            + "*YIK:" + bilgi.YetkiliIdareKodu.Value);
                    }

                    //long koyKod = 0;
                    //int koyIlceKod = 0;
                    //int koyIsMerkezKoy = 0;
                    //if (koy.Kod.HasValue) koyKod = koy.Kod.Value;
                    //if (koy.IsMerkezKoy.HasValue) koyIsMerkezKoy = koy.IsMerkezKoy.Value ? 1 : 0;

                    //string koyler = koyKod.ToString();
                    //koyler += "|" + koy.Ad;
                    //koyler += "|" + koyIlceKod.ToString();
                    //koyler += "|" + koyIsMerkezKoy.ToString();
                }
            }
            catch (Exception e)
            {

                OrtakFonksiyonlar.HataStrYaz(e.Message + "*" + e.StackTrace);
            }

            OrtakFonksiyonlar.HataStrYaz("10");

            return liste;
        }

    }
}