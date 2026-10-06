﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TNS
{
    public class YaziyaCevirEN
    {
        public static string YaziylaTamSayiEN(int sayi, bool sifirBos)
        {
            if (sayi == 0)
            {
                if (sifirBos) return "";
                else return "ZERO";
            }

            if (sayi < 0)
                return "minus " + YaziylaTamSayiEN(Math.Abs(sayi), sifirBos);

            string words = "";

            if ((sayi / 1000000) > 0)
            {
                words += YaziylaTamSayiEN(sayi / 1000000, sifirBos) + " MILLION";
                sayi %= 1000000;
            }

            if ((sayi / 1000) > 0)
            {
                words += YaziylaTamSayiEN(sayi / 1000, sifirBos) + " THOUSAND ";
                sayi %= 1000;
            }

            if ((sayi / 100) > 0)
            {
                words += YaziylaTamSayiEN(sayi / 100, sifirBos) + " HUNDRED ";
                sayi %= 100;
            }

            if (sayi > 0)
            {
                if (words != "")
                    words += "AND ";

                var unitsMap = new[] { "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN", "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN" };
                var tensMap = new[] { "ZERO", "TEN", "TWENTY", "THIRTY", "FORTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY" };

                if (sayi < 20)
                    words += unitsMap[OrtakFonksiyonlar.ConvertToInt(sayi, 0)];
                else
                {
                    words += tensMap[OrtakFonksiyonlar.ConvertToInt(sayi, 0) / 10];
                    if ((sayi % 10) > 0)
                        words += "-" + unitsMap[OrtakFonksiyonlar.ConvertToInt(sayi, 0) % 10];
                }
            }

            return words;
        }

        public static string YaziylaIcEN(double sayi, ParaIsmi paraIsmi, bool sifirBos)
        {
            string tlYazi = YaziylaTamSayiEN(OrtakFonksiyonlar.ConvertToInt(Math.Floor(sayi), 0), sifirBos);
            if (paraIsmi != null) tlYazi = paraIsmi.LiraEkle(tlYazi);

            string krYazi = YaziylaTamSayiEN(OrtakFonksiyonlar.ConvertToInt((sayi - Math.Floor(sayi)) * 100, 0), true);
            if (paraIsmi != null) krYazi = paraIsmi.KurusEkle(krYazi);

            return (tlYazi + " " + krYazi).Trim();
        }
    }
}
