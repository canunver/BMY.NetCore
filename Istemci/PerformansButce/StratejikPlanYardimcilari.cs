using System;
using TNS;
using TNS.KYM;
using TNS.PEB;

// Eski PerformansButce sayfa sınıflarındaki durağan yardımcılar; raporlar (HedefIzlemeRaporlari, SPIzlemeKurum) bunları
// eski adlarıyla çağırır. Yordamlar eski dosyalardan olduğu gibi alınmıştır.
namespace PerformansButce
{
    /// <summary>Eski StratejikHedef.aspx.cs (StratejikHedefYeni) içindeki biçimlendirme yardımcıları.</summary>
    public static class StratejikHedefYeni
    {
        public static string MetinBul(TNSCollection liste, int ind)
        {
            if (ind < liste.Count)
                return (string)liste[ind];
            return "";
        }

        public static double ToplamAl(ObjectArray odenekler)
        {
            double toplam = 0;
            foreach (StrHedefOdenek item in odenekler.objeler)
            {
                toplam += (item.d0 + item.d1 + item.d2 + item.d3 + item.d4);
            }
            return toplam;
        }

        public static string StringYap(TNSCollection metinler)
        {
            string donDeger = "";
            foreach (string item in metinler)
            {
                if (donDeger != "") donDeger += "\r\n";
                donDeger += item;
            }
            return donDeger;
        }

        public static string StrYap(double p, int ondalik = 0)
        {
            string fEk = "";
            if (ondalik > 0)
            {
                fEk = "." + "0".PadRight(ondalik, '0');
            }
            return StrYap(p, "#,##0" + fEk);
        }

        public static string StrYap(double p, string format)
        {
            return p.ToString(format);
        }

        public static string BirimBul(TNSCollection birimler, int basla, int say)
        {
            string birimAd = "";
            for (int i = basla; i < basla + say && i < birimler.Count; i++)
            {
                StrKodStrAd birim = (StrKodStrAd)birimler[i];
                if (birim != null)
                {
                    if (birimAd != "") birimAd += ", ";
                    birimAd += birim.ad;
                }
            }
            return birimAd;
        }
    }

    /// <summary>Eski StratejikPlanAna.aspx.cs içindeki dönem bulma yardımcısı.</summary>
    public static class StratejikPlanAna
    {
        public static Misyon KurumDonemBul(TNS.PEB.IPEBServis servis, Kullanici kullanan, string kurumKod, int yil)
        {
            Misyon misyon = new Misyon();
            misyon.donemBYil = yil;
            misyon.kurumKod = kurumKod;
            misyon.aralikBasiYil = yil;
            misyon.aralikSonuYil = yil + 4;
            if (yil > 0 && !string.IsNullOrWhiteSpace(kurumKod))
            {
                ObjectArray bilgiler = servis.MisyonListele(kullanan, misyon);

                if (bilgiler.sonuc.islemSonuc && bilgiler.objeler.Count > 0)
                {
                    misyon = (TNS.PEB.Misyon)bilgiler.objeler[0];
                }
            }
            return misyon;
        }
    }

    /// <summary>Eski PerformansButce\Ortak.cs içinden.</summary>
    public static class Ortak
    {
        public static int SPNotBul(double basarimOran)
        {
            if (basarimOran >= 90) return 1;
            if (basarimOran >= 75) return 2;
            if (basarimOran >= 50) return 3;
            return 4;

        }
    }
}
