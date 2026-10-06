using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.Veri;

namespace TNS.KHM.IsKurallari
{
    class KHMTabloDegisiklikleri
	{
		public static void Kaydet()
		{
			TabloDegisiklikleri.Kaydet("BaglantiSatiriKHM", komutlarKHM);
		}

		private static SQLKomut[] komutlarKHM = {
			new SQLKomut(1, "", @"CREATE TABLE SBONBULGU
                (BULGUID INT Identity(1, 1)
                , YIL INT
                , BUTCETURKOD INT
                , BULGUTURKOD INT
                , ONBULGUNO VARCHAR(50)
                , BULGU VARCHAR(MAX)
                , BULGUOZETI VARCHAR(MAX)
                , BULGUDAYANAGI VARCHAR(MAX)
                , IDARECEVABI VARCHAR(MAX)
                , IDARECEVAPOZETI VARCHAR(MAX)
                , SAYISTAYONERISI VARCHAR(MAX)
                , DURUMKOD INT
                , PRIMARY KEY (BULGUID))", ""),
            new SQLKomut(2, "", @"CREATE TABLE SBONBULGUBIRIM(BULGUID int NOT NULL, BIRIMKOD varchar(3) NOT NULL, 
                MUHASEBEKOD int NOT NULL, KURKOD varchar(20) NOT NULL,
                PRIMARY KEY (BULGUID, BIRIMKOD, MUHASEBEKOD, KURKOD))", ""),
            new SQLKomut(3, "", @"CREATE TABLE SBONBULGUBIRIMCEVAP(BULGUID int NOT NULL, BIRIMKOD varchar(3) NOT NULL, 
                MUHASEBEKOD int NOT NULL, KURKOD varchar(20) NOT NULL, YIL INT NOT NULL, CEVAP VARCHAR(MAX),
                PRIMARY KEY (BULGUID, BIRIMKOD, MUHASEBEKOD, KURKOD, YIL))", ""),
            new SQLKomut(4, "", @"CREATE TABLE SBONBULGUBIRIMIZIN(YIL int NOT NULL, BIRIMKOD varchar(3) NOT NULL, 
                MUHASEBEKOD int NOT NULL, KURKOD varchar(20) NOT NULL, TUR int NOT NULL, IZIN int NOT NULL,
                PRIMARY KEY (YIL, BIRIMKOD, MUHASEBEKOD, KURKOD, TUR))", ""),
            new SQLKomut(5, "", @"ALTER TABLE SBONBULGU ADD BITISYIL int NULL", ""),
            new SQLKomut(6, "", @"ALTER TABLE SBONBULGUBIRIMCEVAP ADD YIL int NOT NULL DEFAULT 0", ""),
            new SQLKomut(7, "", @"ALTER TABLE SBONBULGU ADD YERINEGETIRILDI int NULL, YAPILACAKFAALIYETLER VARCHAR(MAX) NULL, ILGILIBIRIMKURUM VARCHAR(500) NULL, GOREVORTAGI VARCHAR(500) NULL, ONGORULENTAMAMLANMATARIHI DATE NULL", ""),
            new SQLKomut(8, "", @"ALTER TABLE SBONBULGUBIRIMCEVAP ADD BIRIMISLER varchar(MAX) NULL", ""),
            new SQLKomut(9, "", @"ALTER TABLE SBONBULGUBIRIMCEVAP ADD YERINEGETIRILDI int NULL, ONGORULENTAMAMLANMATARIHI DATE NULL", ""),
        };
	}
}
