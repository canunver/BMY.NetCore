using System;
using System.Collections.Generic;
using System.Text;
using TNS.KYM;
using Mernis.KPS.Types.KPSWse;

namespace TNS.KPM.IsKurallari
{
    public class KPMIl
    {
        public static ObjectArray IlListele(Kullanici kullanan)
        {
            ObjectArray ilVAN = new ObjectArray();

            //Mernis servisten çekiyoruz illeri
            KPS servis = Mernis.KPS.Types.KPSServis.Create();
            servis.Proxy = OrtakFonksiyonlar.ProxyAyarla();

            IlBilgisi[] iller = servis.IlListesiGetir();

            foreach (IlBilgisi ilB in iller)
            {
                Il il = new Il(ilB);
                ilVAN.objeler.Add(il);
            }

            if (ilVAN.objeler.Count <= 0)
                ilVAN.sonuc.bilgiStr = "Verilen kriterlere uygun kayıt bulunamadı.";

            return ilVAN;
        }
    }
}
