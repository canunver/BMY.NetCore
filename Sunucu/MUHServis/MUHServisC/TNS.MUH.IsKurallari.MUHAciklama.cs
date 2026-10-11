using System.Data;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.MUH.IsKurallari
{
    public class MUHAciklama
    {
        public static Sonuc Kaydet(Kullanici kullanan, AciklamaBilgi aciklama)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            string hata = string.Empty;
            if (string.IsNullOrEmpty(aciklama.adi))
                hata = "Adı alanı boş bırakılamaz" + "<br>";
            if (aciklama.muhasebeKod == 0)
                hata = "Muhasebe alanı boş bırakılamaz" + "<br>";
            //if (string.IsNullOrEmpty(aciklama.birimKod))
            //    hata = "Birim alanı boş bırakılamaz" + "<br>";
            if (aciklama.tipi == 0)
                hata += "Tip bilgisi boş bırakılamaz" + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABAciklama to = new TABAciklama(ve);

            if (aciklama.kod == 0)
            {
                TABGenel tg = new TABGenel(ve);
                aciklama.kod = OrtakFonksiyonlar.ConvertToInt(tg.BelgeNoAl("ACIKLAMABILGISAYAC", "000000"), 0);

                sonuc = to.Kaydet(aciklama);
            }
            else
                sonuc = to.Guncelle(aciklama);


            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH101;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(Kullanici kullanan, AciklamaBilgi aciklama)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            if (aciklama.kod == 0)
                sonuc.hataStr += "Kod bilgisi boş bırakılamaz" + "<br>";
            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABAciklama to = new TABAciklama(ve);

            sonuc = to.Sil(aciklama);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH104;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray Listele(Kullanici kullanan, AciklamaBilgi aciklama)
        {
            ObjectArray acklar = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAciklama to = new TABAciklama(ve);

            IDataReader dr = to.Listele(aciklama);

            if (string.IsNullOrEmpty(to.Hata()))
            {
                while (dr.Read())
                {
                    AciklamaBilgi ack = new AciklamaBilgi();
                    ack.kod = VeriErisim.DegerAl(dr, (int)TABAciklama.ENUMAlanlar.KOD, 0);
                    ack.tipi = VeriErisim.DegerAl(dr, (int)TABAciklama.ENUMAlanlar.TIPI, 0);
                    ack.muhasebeKod = VeriErisim.DegerAl(dr, (int)TABAciklama.ENUMAlanlar.MUHASEBEKOD, 0);
                    ack.birimKod = VeriErisim.DegerAl(dr, (int)TABAciklama.ENUMAlanlar.BIRIMKOD, "");
                    ack.adi = VeriErisim.DegerAl(dr, (int)TABAciklama.ENUMAlanlar.ADI, "");
                    ack.aciklama = VeriErisim.DegerAl(dr, (int)TABAciklama.ENUMAlanlar.ACIKLAMA, "");
                    ack.kayitEden = VeriErisim.DegerAl(dr, (int)TABAciklama.ENUMAlanlar.KAYITEDEN, "");
                    ack.paylasilan = VeriErisim.DegerAl(dr, (int)TABAciklama.ENUMAlanlar.PAYLASIM, 0);

                    acklar.objeler.Add(ack);
                }
            }
            else
            {
                acklar.sonuc.hataStr = to.Hata();
                acklar.sonuc.islemSonuc = false;
            }
            dr.Close();

            ve.VeriErisimKapat();
            return acklar;
        }
    }
}