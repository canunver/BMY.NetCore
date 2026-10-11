using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.BKD;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// Sabit maliyet ödenek bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class PEBSabitMaliyetOdenek
    {
        /// <summary>
        /// Verilen kriterlere uygun olan sabit maliyet ödenek bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="smOdenek">Sabit maliyet ödenek bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında sabit maliyet ödenek listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, SabitMaliyetOdenek smOdenek)
        {
            ObjectArray smOdenekVAN = new ObjectArray();
            smOdenekVAN.sonuc = Yetki.OkumaHakkiVarMi(kullanan, smOdenek.kurKod);
            if (!smOdenekVAN.sonuc.islemSonuc)
                return smOdenekVAN;

            string hata = string.Empty;
            if (string.IsNullOrEmpty(smOdenek.kurKod))
                hata += PerformansButceResI.PEB005 + "<br>";
            if (smOdenek.smKod <= 0)
                hata += PerformansButceResI.PEB105 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                smOdenekVAN.sonuc.islemSonuc = false;
                smOdenekVAN.sonuc.hataStr = hata;
                return smOdenekVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdenek tOdenek = new TABOdenek(ve);

            IDataReader dr = tOdenek.Listele(smOdenek.yil, smOdenek.kurKod, smOdenek.senaryoKod, smOdenek.smKod, (int)ENUMOdenekTur.SabitMaliyet);
            while (dr.Read())
            {
                SabitMaliyetOdenek gelen = new SabitMaliyetOdenek();
                gelen.yil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.kurKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.kurKod = PEBGenel.KurKodNoktala(gelen.kurKod);
                gelen.kurAd = PEBServis.UzayYaziDegerGetir("ABSKUR", gelen.yil.ToString() + "-" + gelen.kurKod, true, "");
                gelen.senaryoKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                gelen.smKod = VeriErisim.DegerAl(dr, 3, 0);

                string kurKod = VeriErisim.DegerAl(dr, 5, string.Empty);
                kurKod = PEBGenel.KurKodNoktala(kurKod);

                string fonKod = VeriErisim.DegerAl(dr, 6, string.Empty);
                fonKod = PEBGenel.FonKodNoktala(fonKod);

                string finKod = VeriErisim.DegerAl(dr, 7, string.Empty);

                string ekoKod = VeriErisim.DegerAl(dr, 8, string.Empty);
                ekoKod = PEBGenel.EkoKodNoktala(ekoKod);

                gelen.tertip.TertipAta(kurKod + "-" + fonKod + "-" + finKod + "-" + ekoKod);

                gelen.odenek = VeriErisim.DegerAl(dr, 9, (double)0);

                gelen.smFonKod = VeriErisim.DegerAl(dr, 10, string.Empty);
                gelen.smFonKod = PEBGenel.FonKodNoktala(gelen.smFonKod);

                smOdenekVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (smOdenekVAN.objeler.Count == 0)
                smOdenekVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return smOdenekVAN;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan sabit maliyet ödenek bilgilerini döndüren yordam
        /// Ödenekler e-butce sisteminin istediği yapıda yani ekonomik kodun ilk kırılımı bazında toplanıyor.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında sabit maliyet ödenek listesi döndürülür.</returns>
        public static ObjectArray SabitMaliyetOdenekleriEbutce(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod)
        {
            ObjectArray smVAN = new ObjectArray();

            string hata = string.Empty;
            if (yil <= 0)
                hata += PerformansButceResI.PEB039 + "<br>";
            if (string.IsNullOrEmpty(senaryoKod))
                hata += PerformansButceResI.PEB045 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                smVAN.sonuc.islemSonuc = false;
                smVAN.sonuc.hataStr = hata;
                return smVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdenek tOdenek = new TABOdenek(ve);

            SabitMaliyet gelen = null;
            IDataReader dr = tOdenek.SabitMaliyetOdenekleriEbutce(yil, senaryoKod);
            while (dr.Read())
            {
                if (gelen == null || gelen.kurKod != VeriErisim.DegerAl(dr, 0, string.Empty))
                {
                    gelen = new SabitMaliyet();
                    gelen.yil = yil;
                    gelen.kurKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                    smVAN.objeler.Add(gelen);
                }

                TertipOdenek odenek = new TertipOdenek(yil);
                odenek.tertip.kodlar[(int)ENUMAbsKodSinif.KURKOD - 1] = gelen.kurKod;
                odenek.tertip.kodlar[(int)ENUMAbsKodSinif.EKOKOD - 1] = VeriErisim.DegerAl(dr, 1, string.Empty);
                odenek.odenek = VeriErisim.DegerAl(dr, 2, (double)0);
                gelen.smOdenek.Add(odenek);
            }
            dr.Close();

            if (smVAN.objeler.Count == 0)
                smVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return smVAN;
        }
    }
}