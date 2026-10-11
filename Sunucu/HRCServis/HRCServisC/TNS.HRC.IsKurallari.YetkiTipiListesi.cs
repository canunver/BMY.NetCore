using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.KYM;
using TNS.HRC;

namespace TNS
{
    public class HRCListe : YetkiTipiListesi
    {
        public ListeSatir[] YetkiTipiListeElemanlari(Kullanici kullanan, int yil, string ekParamCagri, string ekParamTanim)
        {
            if (ekParamTanim.Equals("1"))
                return SurecAdimlar();
            else if (ekParamTanim.Equals("2"))
                return IsAdlari(kullanan);
            else if (ekParamTanim.Equals("3"))
                return IsAdimAdlari(kullanan);

            return null;
        }

        private ListeSatir[] IsAdlari(Kullanici kullanan)
        {
            ListeSatir[] ls = null;
            Is bilgi = new Is();
            ObjectArray oa = new TNS.HRC.IsKurallari.HRCServis().IsListele(kullanan, bilgi);
            if (oa.sonuc.islemSonuc)
            {
                System.Collections.Generic.List<ListeSatir> al = new System.Collections.Generic.List<ListeSatir>();
                foreach (Is item in oa.objeler)
                {
                    ListeSatir ic = new ListeSatir();
                    ic.kod = item.kod.ToString();
                    ic.ad = item.ad;
                    al.Add(ic);
                }
                ls = al.ToArray();
            }
            return ls;
        }

        private ListeSatir[] IsAdimAdlari(Kullanici kullanan)
        {
            ListeSatir[] ls = null;
            Parca bilgi = new Parca();
            ObjectArray oa = new TNS.HRC.IsKurallari.HRCServis().ParcaListele(kullanan, bilgi);
            if (oa.sonuc.islemSonuc)
            {
                System.Collections.Generic.List<ListeSatir> al = new System.Collections.Generic.List<ListeSatir>();
                foreach (Parca item in oa.objeler)
                {
                    ListeSatir ic = new ListeSatir();
                    ic.kod = item.kod.ToString();
                    ic.ad = item.ad;
                    al.Add(ic);
                }
                ls = al.ToArray();
            }
            return ls;
        }

        private ListeSatir[] SurecAdimlar()
        {
            ListeSatir[] liste = new ListeSatir[9];
            liste[0] = new ListeSatir("01", "Talep");
            liste[1] = new ListeSatir("02", "Süreç Başlat");
            liste[2] = new ListeSatir("03", "Onay Belgesi");
            liste[3] = new ListeSatir("04", "İhale İşlemleri");
            liste[4] = new ListeSatir("05", "Fatura Girişi");
            liste[5] = new ListeSatir("06", "Varlık İşlem Fişi");
            liste[6] = new ListeSatir("07", "Ön Mali Kontrol");
            liste[7] = new ListeSatir("08", "Ödeme Emri Belgesi");
            liste[8] = new ListeSatir("09", "Birim Ödeme");
            return liste;
        }
    }
}