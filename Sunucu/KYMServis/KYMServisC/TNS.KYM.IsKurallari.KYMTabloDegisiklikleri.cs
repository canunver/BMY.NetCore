using System.Data;
using TNS;
using TNS.Veri;
using TNS.KYM;
using TNS.KYM.IsKurallari.Veri;
using System.Collections;

namespace TNS.KYM.IsKurallari
{
    public class KYMTabloDegisiklikleri
    {
        public static void Kaydet()
        {
            TabloDegisiklikleri.Kaydet("BaglantiSatiriKYM", komutlarKYM);
        }
      
        private static SQLKomut[] komutlarKYM = {
            new SQLKomut(1, "", @"CREATE TABLE IZLEMETABLOSU(ID int IDENTITY(1,1) PRIMARY KEY, TARIHSAAT DATETIME, IP VARCHAR(100), KULLANICIKODU VARCHAR(100), IZLEMEKOD VARCHAR(100), MODULKOD VARCHAR(20), SAYFAAD  VARCHAR(2000), EKBILGI VARCHAR(8000))", ""),

            new SQLKomut(2, "", @"ALTER TABLE KYMEKLIDOSYA ADD ACIKLAMA VARCHAR(500)", ""),

            new SQLKomut(3, "", @"CREATE TABLE KYMHATA(KOD varchar(50) NOT NULL, TUR varchar(20) NULL, ISLEMYAPAN varchar(300) NULL, ISLEMTARIH datetime NULL, HATA varchar(2000) NULL, IPADRESI varchar(50) NULL, CONSTRAINT PK_KYMHATA PRIMARY KEY (KOD)) ", ""),

        };
    }
}
