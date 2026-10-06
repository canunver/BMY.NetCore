using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net.Mail;
using System.Collections;

namespace TNS
{
    public class YaziylaTR
    {
        public static string YaziylaTamSayiTR(double sayi, bool sifirBos)
        {
            string[] birler = { "", "BİR", "İKİ", "ÜÇ", "DÖRT", "BEŞ", "ALTI", "YEDİ", "SEKİZ", "DOKUZ" };
            string[] onlar = { "", "ON", "YİRMİ", "OTUZ", "KIRK", "ELLİ", "ALTMIŞ", "YETMİŞ", "SEKSEN", "DOKSAN" };
            string[] boluk = { "", "BİN", "MİLYON", "MİLYAR", "TRİLYON", "KATRİLYON" };
            string donDeger = "";
            sayi = Math.Round(sayi);
            if (sayi == 0)
            {
                if (sifirBos) return "";
                else return "SIFIR";
            }

            double cikan = 1E15;
            for (int i = 5; i > -1; i--)
            {
                if (cikan <= sayi)
                {
                    int bol = Convert.ToInt32(Math.Floor(sayi / cikan));

                    int k = bol / 100;
                    if (k > 0)
                    {
                        if (k > 1)
                            donDeger += birler[k];
                        donDeger += "YÜZ";
                    }

                    k = (bol % 100) / 10;
                    if (k > 0)
                        donDeger += onlar[k];

                    k = bol % 10;
                    if (((i != 1) || (bol > 1)) && (k > 0))
                        donDeger += birler[k];

                    if (bol > 0)
                        donDeger += boluk[i];

                    if (k > 0)
                        donDeger += " ";

                    sayi -= bol * cikan;
                }
                cikan = cikan / 1000;
            }
            return donDeger;
        }

        public static string YaziylaIcTR(double sayi, ParaIsmi paraIsmi, bool sifirBos)
        {
            string tlYazi = YaziylaTamSayiTR(Math.Floor(sayi), sifirBos);
            if (paraIsmi != null) tlYazi = paraIsmi.LiraEkle(tlYazi);

            string krYazi = YaziylaTamSayiTR((sayi - Math.Floor(sayi)) * 100, true);
            if (paraIsmi != null) krYazi = paraIsmi.KurusEkle(krYazi);

            return (tlYazi + " " + krYazi).Trim();
        }
    }
}
