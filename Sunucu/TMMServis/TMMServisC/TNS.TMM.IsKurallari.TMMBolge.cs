using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Bölge bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMBolge
    {
        /// <summary>
        /// Bölge varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Bolge nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sozluk">Bölge bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(Kullanici kullanan, Bolge sozluk)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (sozluk.kod <= 0)
                hata = TasinirMalResI.TMM074 + "<br>";

            if (sozluk.ad == string.Empty)
                hata += TasinirMalResI.TMM075 + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABBolge to = new TABBolge(ve);
            ve.BeginTransaction();

            sonuc = to.Kaydet(sozluk);

            if (sonuc.islemSonuc)
            {
                to.BolgeyeAitMuhasebeleriSifirla(sozluk.kod);
                to.BolgeMuhasebeEslestir(sozluk.kod, sozluk.muhasebe);

                sonuc.bilgiStr = TasinirMalResI.TMM017;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TasinirMalResI.TMM018 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Bölge, muhasebe birimlerinde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Bolge nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sozluk">Bölge bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(Kullanici kullanan, Bolge sozluk)
        {
            Sonuc sonuc = new Sonuc();

            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (sozluk.kod <= 0)
                sonuc.hataStr = TasinirMalResI.TMM074;

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABBolge to = new TABBolge(ve);

            ve.BeginTransaction();

            sonuc = to.Sil(sozluk);

            if (sonuc.islemSonuc)
            {
                to.BolgeyeAitMuhasebeleriSifirla(sozluk.kod);

                ve.Commit();
                sonuc.bilgiStr = string.Format(TasinirMalResI.TMM076, sozluk.kod);
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = TasinirMalResI.TMM026 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan bölge bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pSozluk">Bölge bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında bölge listesi döndürülür.</returns>
        public ObjectArray Listele(Kullanici kullanan, Bolge pSozluk)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABBolge to = new TABBolge(ve);
            TABBolge to2 = new TABBolge(ve2);
            IDataReader r = to.Listele(pSozluk);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Bolge sozluk = new Bolge();

                    sozluk.kod = VeriErisim.DegerAl(r, 0, 0);
                    sozluk.ad = VeriErisim.DegerAl(r, 1, "");
                    IDataReader r2 = to2.BolgeyeAitMuhasebeler(sozluk.kod);
                    while (r2.Read())
                    {
                        Muhasebe m = new Muhasebe();
                        m.kod = VeriErisim.DegerAl(r2, 0, "");
                        m.ad = VeriErisim.DegerAl(r2, 1, "");
                        sozluk.muhasebe.Add(m);
                    }
                    r2.Close();

                    iVAN.objeler.Add(sozluk);
                }
            }

            r.Close();

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();

            return iVAN;
        }
    }
}