using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// Performans hedeflerin stratejik hedeflerle bağını kuran ağırlık bilgileriyle ilgili,
    /// iş kuralları kontrollerini yapıp, veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını
    /// çağırarak okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class PEBPerHedefAgirlik
    {
        /// <summary>
        /// Parametre olarak verilen performans hedef ağırlık bilgilerinin doğruluğunu kontrol eden yordam
        /// </summary>
        /// <param name="agirlik">Performans hedef ağırlık bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private static Sonuc AgirlikKontrol(TNSCollection agirlik)
        {
            Sonuc sonuc = new Sonuc();

            double toplam = 0;
            foreach (PerformansHedefAgirlik phs in agirlik)
            {
                if (phs.agirlik < 0)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = PerformansButceResI.PEB042;
                    return sonuc;
                }
                toplam += phs.agirlik;
            }

            if (Math.Round(toplam, 4) != 100)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = string.Format(PerformansButceResI.PEB043, toplam.ToString());
            }
            return sonuc;
        }

        /// <summary>
        /// Performans hedeflerin stratejik hedeflerle bağını kuran ağırlık bilgilerini kaydeden yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="perAgirlik">Performans hedef ağırlık bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TNSCollection perAgirlik)
        {
            Sonuc sonuc = AgirlikKontrol(perAgirlik);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPerHedefAgirlik tAgirlik = new TABPerHedefAgirlik(ve);
            ve.BeginTransaction();

            PerformansHedefAgirlik pAgirlik = (PerformansHedefAgirlik)perAgirlik[0];
            tAgirlik.Sil(pAgirlik);
            foreach (PerformansHedefAgirlik hAgirlik in perAgirlik)
            {
                if (hAgirlik.strHedef.strHedefKod > 0 && hAgirlik.agirlik > 0)
                {
                    if (!PEBGenel.StratejikHedefVarMi(hAgirlik.strHedef.donemBYil, hAgirlik.strHedef.strHedefKod))
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = string.Format(PerformansButceResI.PEB094, hAgirlik.strHedef.strHedefKod);
                        break;
                    }

                    sonuc = tAgirlik.Kaydet(hAgirlik);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uyan performans hedeflerin stratejik hedeflerle bağını kuran ağırlık bilgilerini listeleyen yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="perHedefKod">Performans hedef programatik kodu</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans hedefe ait ağırlıklar liste şeklinde döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurKod, int perHedefKod)
        {
            ObjectArray hAgirlikVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPerHedefAgirlik tAgirlik = new TABPerHedefAgirlik(ve);

            IDataReader dr = tAgirlik.Listele(yil, kurKod, perHedefKod);
            while (dr.Read())
            {
                PerformansHedefAgirlik gelen = new PerformansHedefAgirlik();
                gelen.yil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.kurKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.kurKod = PEBGenel.KurKodNoktala(gelen.kurKod);
                gelen.perHedefKod = VeriErisim.DegerAl(dr, 2, 0);
                gelen.strHedef.donemBYil = VeriErisim.DegerAl(dr, 3, 0);
                gelen.strHedef.strHedefKod = VeriErisim.DegerAl(dr, 4, 0);
                gelen.agirlik = VeriErisim.DegerAl(dr, 5, (double)0);
                gelen.strHedef.strHedefKisa = PEBServis.UzayYaziDegerGetir("STRHEDEF", gelen.strHedef.donemBYil.ToString() + "-" + gelen.strHedef.strHedefKod.ToString(), true, "");
                gelen.strHedef.strHedef = PEBServis.UzayYaziDegerGetir("STRYAZI", gelen.strHedef.donemBYil.ToString() + "-" + ((int)ENUMStratejikPlan.StratejikHedef).ToString() + "-" + gelen.strHedef.strHedefKod.ToString() + "-" + "1", true, "");
                gelen.strHedef.ebutceKod = PEBServis.UzayYaziDegerGetir("STRYAZI", gelen.strHedef.donemBYil.ToString() + "-" + ((int)ENUMStratejikPlan.StratejikHedef).ToString() + "-" + gelen.strHedef.strHedefKod.ToString() + "-" + "1", true, "EBUTCEKOD");
                gelen.strHedef.strAmac.strAmacKod = (int)PEBServis.UzaySayiDegerGetir("STRHEDEFAMACKOD", gelen.strHedef.donemBYil.ToString() + "-" + gelen.strHedef.strHedefKod.ToString(), true, "");
                gelen.strHedef.strAmac.strAmac = PEBServis.UzayYaziDegerGetir("STRYAZI", gelen.strHedef.donemBYil.ToString() + "-" + ((int)ENUMStratejikPlan.StratejikAmac).ToString() + "-" + gelen.strHedef.strAmac.strAmacKod.ToString() + "-" + "1", true, "");

                hAgirlikVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (hAgirlikVAN.objeler.Count == 0)
                hAgirlikVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return hAgirlikVAN;
        }
    }
}