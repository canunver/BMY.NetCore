using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// Faaliyet/projelerin performans hedeflerle bağını kuran ağırlık bilgileriyle ilgili,
    /// iş kuralları kontrollerini yapıp, veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını
    /// çağırarak okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class PEBFaaliyetProjeAgirlik
    {
        /// <summary>
        /// Parametre olarak verilen faaliyet/proje ağırlık bilgilerinin doğruluğunu kontrol eden yordam
        /// </summary>
        /// <param name="agirlik">Faaliyet/proje ağırlık bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private static Sonuc AgirlikKontrol(TNSCollection agirlik)
        {
            Sonuc sonuc = new Sonuc();

            double toplam = 0;
            foreach (FaaliyetProjeAgirlik fps in agirlik)
            {
                if (fps.agirlik < 0)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = PerformansButceResI.PEB042;
                    return sonuc;
                }
                toplam += fps.agirlik;
            }

            if (Math.Round(toplam, 4) != 100)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = string.Format(PerformansButceResI.PEB043, toplam.ToString());
            }
            return sonuc;
        }

        /// <summary>
        /// Faaliyet/projelerin performans hedeflerle bağını kuran ağırlık bilgilerini kaydeden yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="fpAgirlik">Faaliyet/proje ağırlık bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TNSCollection fpAgirlik)
        {
            Sonuc sonuc = AgirlikKontrol(fpAgirlik);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABFaaliyetProjeAgirlik tAgirlik = new TABFaaliyetProjeAgirlik(ve);
            ve.BeginTransaction();

            FaaliyetProjeAgirlik fpa = (FaaliyetProjeAgirlik)fpAgirlik[0];
            tAgirlik.Sil(fpa);
            foreach (FaaliyetProjeAgirlik fps in fpAgirlik)
            {
                if (!PEBGenel.PerformansHedefiVarMi(ve, fps.yil, fps.phKurKod, fps.perHedefKod))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = string.Format(PerformansButceResI.PEB044, fps.perHedefKod);
                    break;
                }

                if (fps.agirlik > 0)
                {
                    sonuc = tAgirlik.Kaydet(fps);
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
        /// Verilen kriterlere uyan faaliyet/projelerin performans hedeflerle bağını kuran ağırlık bilgilerini listeleyen yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="fpKod">Faaliyet/proje programatik kodu</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında faaliyet/projeye ait ağırlıklar liste şeklinde döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurKod, int fpKod)
        {
            ObjectArray fpAgirlikVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABFaaliyetProjeAgirlik tAgirlik = new TABFaaliyetProjeAgirlik(ve);

            IDataReader dr = tAgirlik.Listele(yil, kurKod, fpKod);
            while (dr.Read())
            {
                FaaliyetProjeAgirlik gelen = new FaaliyetProjeAgirlik();
                gelen.yil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.fpKurKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.fpKurKod = PEBGenel.KurKodNoktala(gelen.fpKurKod);
                //gelen.fpKurAd = PEBServis.UzayYaziDegerGetir("ABSKUR", gelen.yil.ToString() + "-" + gelen.fpKurKod, true, "");
                gelen.fpKod = VeriErisim.DegerAl(dr, 2, 0);
                gelen.phKurKod = VeriErisim.DegerAl(dr, 3, string.Empty);
                gelen.phKurKod = PEBGenel.KurKodNoktala(gelen.phKurKod);
                //gelen.phKurAd = PEBServis.UzayYaziDegerGetir("ABSKUR", gelen.yil.ToString() + "-" + gelen.phKurKod, true, "");
                gelen.perHedefKod = VeriErisim.DegerAl(dr, 4, 0);
                gelen.agirlik = VeriErisim.DegerAl(dr, 5, (double)0);
                gelen.perHedefAd = VeriErisim.DegerAl(dr, 6, "");
                gelen.perHedefEbutceKod = VeriErisim.DegerAl(dr, 7, "");
                gelen.perHedefGorKod = VeriErisim.DegerAl(dr, 8, "");

                fpAgirlikVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (fpAgirlikVAN.objeler.Count == 0)
                fpAgirlikVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return fpAgirlikVAN;
        }
    }
}