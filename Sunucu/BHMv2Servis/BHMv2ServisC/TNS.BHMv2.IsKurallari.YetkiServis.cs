using System;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.KYM;
using TNS.BKD;

namespace TNS.BHMv2.IsKurallari
{
    public class YetkiServis
    {
        private static string[,] YetkiAdlari ={
            {"Bütçe Hazırlığı Görebileceği Birimler", "Bütçe Hazırlığı Görebileceği Aşamalar", "Bütçe Hazırlığı Görebileceği Altfaaliyetler"},
            {"Bütçe Hazırlığı Kaydedebileceği Birimler", "Bütçe Hazırlığı Kaydedebileceği Aşamalar", "Bütçe Hazırlığı Kaydedebileceği Altfaaliyetler"},
            {"Bütçe Hazırlığı Onaylayabileceği Birimler", "Bütçe Hazırlığı Onaylayabileceği Aşamalar", "Bütçe Hazırlığı Onaylayabileceği Altfaaliyetler"}
        };

        public static bool OGYYetkisiVarMi(Kullanici kullanan, string senaryoKod, Tertip tertip, ENUMOGY ogy)
        {
            if (tertip == null)
                return false;
            if (tertip.kodlar.Length == 0)
                return false;
            if (kullanan == null)
                return false;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;

            int indis;
            if (ogy == ENUMOGY.Onaylama || ogy == ENUMOGY.OnayKaldir)
                indis = 2;
            else if (ogy == ENUMOGY.Yazma)
                indis = 1;
            else
                indis = 0;
            string pebKod = tertip.kodlar[4];
            if (pebKod == null) pebKod = "";
            
            bool tertipVar = true;

            if (tertip.kodlar[0] != null && tertip.kodlar[0] != "*")
                tertipVar = tertipVar && kullanan.YetkiTipiDegeriIceriyorMu(YetkiAdlari[indis, 0], tertip.kodlar[0], true);

            if (tertipVar && pebKod != null && pebKod != "*")
                tertipVar = tertipVar && kullanan.YetkiTipiDegeriIceriyorMu(YetkiAdlari[indis, 2], pebKod, true);

            if (tertipVar && !string.IsNullOrWhiteSpace(senaryoKod))
            {
                if (senaryoKod.Length == 1) senaryoKod = "0" + senaryoKod;
                string ytDeger = kullanan.YetkiTipiDegeriBul(YetkiAdlari[indis, 1]);
                if (string.IsNullOrEmpty(ytDeger) || ytDeger.Equals("*"))
                    return true;
                string[] senaryolar = ytDeger.Split(';');
                for (int i = 0; i < senaryolar.Length; i++)
                {
                    string senaryo = "";
                    if (senaryolar[i].Length == 1)
                        senaryo = "0" + senaryolar[i];
                    else
                        senaryo = senaryolar[i];
                    if (senaryoKod.StartsWith(senaryo)) return true;
                }
                return false;
            }
            else
                return tertipVar;
        }
    }
}
