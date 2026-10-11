using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.Veri;

namespace TNS.ATM.IsKurallari
{
    public static class ATMTabloDegisiklikleri
    {
        private static SQLKomut[] komutlar ={
            new SQLKomut(5,"",@"ALTER TABLE ATMDOSYAABTAKSIT ADD GIRISTARIHI DATETIME","")
            , new SQLKomut(6,"",@"UPDATE ATMDOSYAABTAKSIT SET GIRISTARIHI = TAKSITKARARTARIHI where GIRISTARIHI IS NULL AND TAKSITKARARTARIHI IS NOT NULL","")
            , new SQLKomut(7,"",@"ALTER TABLE ATMDOSYAABONBORC ADD AVANS INT DEFAULT 0","")
            , new SQLKomut(8,"",@"ALTER TABLE ATMDOSYAABTAKSIT ADD TECILTUR INT DEFAULT 0","")

        };

        public static void Kaydet()
        {
            TabloDegisiklikleri.Kaydet("BaglantiSatiriATM", komutlar);
        }
    }
}
