using System.Data;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// İmza işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHImzaServis
    {
        /// <summary>
        /// Imza kaydet
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="imzaBilgileri">İmza bilgileri</param>
        /// <returns></returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray imzaBilgileri)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABImza to = new TABImza(ve);

            bool ilk = true;
            foreach (ImzaBilgisi imza in imzaBilgileri.objeler)
            {
                if (ilk)
                {
                    sonuc = to.ImzaSil(kullanan.kullaniciKodu, imza.belgeTur, imza.muhasebe, imza.birim);
                    ilk = false;
                }
                if (imza.birim == "999") imza.birim = "";//Bütün birimdeki silmesi için Melih 09.04.2020 Manas Tek imza kullanacak (ImzaTanım)
                sonuc = to.ImzaKaydet(imza);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH079;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Imza listele
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="belgeTur">Belge türü</param>
        /// <param name="muhasebe">Muhasebe kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <returns></returns>
        public ObjectArray Listele(Kullanici kullanan, ENUMBelgeTuru belgeTur, int muhasebe, string birim)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABImza to = new TABImza(ve);

            IDataReader r = to.ImzaListele(kullanan.kullaniciKodu, belgeTur, muhasebe, birim);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    ImzaBilgisi imza = new ImzaBilgisi();

                    imza.kullaniciKodu = VeriErisim.DegerAl(r, 0, "");
                    imza.belgeTur = (ENUMBelgeTuru)VeriErisim.DegerAl(r, 1, 0);
                    imza.muhasebe = VeriErisim.DegerAl(r, 2, 0);
                    imza.birim = VeriErisim.DegerAl(r, 3, "");
                    imza.imzaYer = VeriErisim.DegerAl(r, 4, "");
                    imza.adSoyad = VeriErisim.DegerAl(r, 5, "");
                    imza.unvan = VeriErisim.DegerAl(r, 6, "");
                    imza.gorevUnvan = VeriErisim.DegerAl(r, 7, "");
                    imza.tarih = VeriErisim.DegerAl(r, 8, 0);
                    imza.kullanilmiyor = VeriErisim.DegerAl(r, 9, 0);

                    iVAN.objeler.Add(imza);
                }
                r.Close();
            }
            ve.VeriErisimKapat();

            return iVAN;
        }
    }
}
