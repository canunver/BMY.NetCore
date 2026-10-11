using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// İl bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMIl
    {
        /// <summary>
        /// İl varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Il nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sozluk">İl bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(Kullanici kullanan, Il sozluk)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (sozluk.kod == string.Empty)
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
            TABIl to = new TABIl(ve);
            ve.BeginTransaction();

            sonuc = to.Kaydet(sozluk);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TasinirMalResI.TMM118;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TasinirMalResI.TMM119 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// İl, ilçelerde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Il nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sozluk">İl bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(Kullanici kullanan, Il sozluk)
        {
            Sonuc sonuc = new Sonuc();

            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (sozluk.kod == "")
                sonuc.hataStr = TasinirMalResI.TMM074 + "";

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIl to = new TABIl(ve);

            //Bu ile ait ilçe var mı?
            TABIlce tabIlce = new TABIlce(ve);
            Ilce ilce = new Ilce();
            ilce.ilKodu = sozluk.kod;
            IDataReader drIlce = tabIlce.Listele(ilce);

            if (drIlce.Read())
            {
                sonuc.islemSonuc = false;
                drIlce.Close();
                ve.VeriErisimKapat();
                sonuc.hataStr = TasinirMalResI.TMM130;
                return sonuc;
            }

            drIlce.Close();

            ve.BeginTransaction();

            sonuc = to.Sil(sozluk);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = string.Format(TasinirMalResI.TMM131, sozluk.kod);
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
        /// Verilen kriterlere uygun olan il bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pSozluk">İl bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında il listesi döndürülür.</returns>
        public ObjectArray Listele(Kullanici kullanan, Il pSozluk)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABIl to = new TABIl(ve);
            IDataReader r = to.Listele(pSozluk);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Il sozluk = new Il();

                    sozluk.kod = VeriErisim.DegerAl(r, 0, "");
                    sozluk.ad = VeriErisim.DegerAl(r, 1, "");

                    iVAN.objeler.Add(sozluk);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return iVAN;
        }
    }
}
