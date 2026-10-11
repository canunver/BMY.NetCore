using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// İlçe bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMIlce
    {
        /// <summary>
        /// İlçe varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Ilce nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sozluk">İlçe bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(Kullanici kullanan, Ilce sozluk)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            
            string hata = "";

            if (sozluk.ilKodu == "")
                hata = TasinirMalResI.TMM132;

            if (sozluk.kod =="")
            {
                if (hata != "") hata += "<br>";
                hata = TasinirMalResI.TMM133;
            }

            if (sozluk.ad == string.Empty)
            {
                if (hata != "") hata += "<br>";
                hata = TasinirMalResI.TMM134;
            }

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIlce to = new TABIlce(ve);
            ve.BeginTransaction();

            sonuc = to.Kaydet(sozluk);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TasinirMalResI.TMM135;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// İlçe, muhasebe birimlerinde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Ilce nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sozluk">İlçe bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(Kullanici kullanan, Ilce sozluk)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM071, "");

            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (sozluk.ilKodu == "")
                hata = TasinirMalResI.TMM132;

            if (sozluk.kod == "")
            {
                if (hata != "") hata += "<br>";
                hata = TasinirMalResI.TMM133;
            }

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIlce to = new TABIlce(ve);

            //Bu ilçeye ait Muhasebe Birimi var mı?
            TABMuhasebe tabMuhasebe = new TABMuhasebe(ve);
            Muhasebe m = new Muhasebe();
            m.ilKod = sozluk.ilKodu;
            m.ilceKod = sozluk.kod;

            IDataReader drMuhasebe = tabMuhasebe.Listele(m);

            if (drMuhasebe.Read())
            {
                drMuhasebe.Close();
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM136;
                return sonuc;
            }
            drMuhasebe.Close();

            ve.BeginTransaction();

            sonuc = to.Sil(sozluk);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TasinirMalResI.TMM137;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan ilçe bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pSozluk">İlçe bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ilçe listesi döndürülür.</returns>
        public ObjectArray Listele(Kullanici kullanan, Ilce pSozluk)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABIlce to = new TABIlce(ve);
            IDataReader r = to.Listele(pSozluk);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Ilce sozluk = new Ilce();

                    sozluk.ilKodu = VeriErisim.DegerAl(r, 0, "");
                    sozluk.kod = VeriErisim.DegerAl(r, 1, "");
                    sozluk.ad = VeriErisim.DegerAl(r, 2, "");
                    sozluk.ilAd = VeriErisim.DegerAl(r, 3, "");

                    iVAN.objeler.Add(sozluk);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return iVAN;
        }
    }
}
