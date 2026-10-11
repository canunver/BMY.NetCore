using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TRM.IsKurallari
{
    public static class TRMTabloDegisiklikleri
    {
        public static void Kaydet()
        {
            TabloDegisiklikleri.Kaydet("BaglantiSatiriTRM", komutlar);
        }

        private static SQLKomut[] komutlar ={

            new SQLKomut(1,"",@"ALTER TABLE TRMHIBEONLINEBASVURU ADD DOGUMTARIH DATE NULL",""),
            new SQLKomut(2,"",@"ALTER TABLE TRMHIBESULAMA ADD CINSIYET varchar(2) NULL", ""),
            new SQLKomut(3,"",@"ALTER TABLE TRMHIBEDEGERLENDIRMEKOMISYONUDETAY ADD DOGUMTARIH date NULL", ""),
            new SQLKomut(4,"",@"ALTER TABLE TRMHIBEYATIRIMHAKEDIS ADD SONODEME INT NULL", ""),
            new SQLKomut(5,"",@"ALTER TABLE TRMHIBEYATIRIMYONETIM ADD IZLEMESURESI INT NULL, IZLEMESURESIALTYAPI INT NULL", ""),
        };
    }
}
