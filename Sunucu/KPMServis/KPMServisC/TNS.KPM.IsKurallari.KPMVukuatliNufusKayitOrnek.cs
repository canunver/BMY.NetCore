using Mernis.KPSv2.Types.BilesikKutukSorgulaKimlikNoServis;
using Mernis.KPSv2.Types.KimlikNoSorgulaAdresServis;
using TNS.KYM;

namespace TNS.KPM.IsKurallari
{
    public class KPMVukuatliNufusKayitOrnek
    {
        public static ObjectArray VukuatliNufusKayitOrnekRaporla(Kullanici kullanan, Istek istek)
        {
            ObjectArray nkoVAN = new ObjectArray();

            if (!Yetki.OkumaHakkiVarMi(kullanan))
            {
                nkoVAN.sonuc.hataStr = "Okuma hakkı yok";
                nkoVAN.sonuc.islemSonuc = false;
                return nkoVAN;
            }

            BilesikKutukSorgulaKimlikNoServis nkoServis = Mernis.KPSv2.Types.KPSv2Servis.BilesikKutukSorgulaTCKimlikNoServisOlustur();
            BilesikKutukSorgulaKimlikNoSorguKriteri[] nkoSorgulist = { new BilesikKutukSorgulaKimlikNoSorguKriteri { KimlikNo = long.Parse(istek.tcKimlikNo) } };
            BilesikKutukBilgileriSonucu kisiSonuc = nkoServis.Sorgula(nkoSorgulist);

            if (kisiSonuc.HataBilgisi == null)
            {
                var kisiListe = kisiSonuc.SorguSonucu;
                BilesikKutukBilgileri kisi = new BilesikKutukBilgileri();
                if (kisiListe.Length > 0)
                {
                    kisi = kisiListe[0];
                }

                Kisi k = new Kisi(kisi);
                nkoVAN.objeler.Add(k);

                if (nkoVAN.objeler.Count <= 0)
                    nkoVAN.sonuc.bilgiStr = "Verilen kriterlere uygun kayıt bulunamadı.";
                else
                {
                    //Kim, kaç defa bu raporu almış logları tutuluyor
                    nkoVAN.sonuc = KPMIstek.Kaydet(null, kullanan, istek);
                }
            }
            else
            {
                nkoVAN.sonuc.islemSonuc = false;
                nkoVAN.sonuc.hataStr = kisiSonuc.HataBilgisi.Aciklama;
                return nkoVAN;
            }

            return nkoVAN;
        }
    }
}
