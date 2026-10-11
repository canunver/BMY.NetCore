using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TNS.GEL.IsKurallari
{
    public class GELOrtak
    {
        private static string KodyapisiBul()
        {
            return TNS.DEG.Arac.DegiskenDegerBul(0, "KODYAPI/KOD28/KODYAPI");
        }

        public static string KodNoktala(string kod)
        {
            return OrtakFonksiyonlar.KodNoktaKoy(KodyapisiBul(), kod);
        }

    }
}
