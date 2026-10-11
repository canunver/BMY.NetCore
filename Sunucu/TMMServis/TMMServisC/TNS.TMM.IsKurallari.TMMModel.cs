using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Model bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMModel
    {
        /// <summary>
        /// Model varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Model nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sozluk">Model bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(Kullanici kullanan, Model sozluk)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");
            string hata = "";

            if (sozluk.markaKodu == 0)
                hata = TasinirMalResI.TMM168;

            if (sozluk.ad == string.Empty)
            {
                if (hata != "") hata += "<br>";
                hata = TasinirMalResI.TMM169;
            }

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABModel to = new TABModel(ve);
            TABGenel tg = new TABGenel(ve2);

            //***********************************************
            //Kontrol
            bool kayitBulundu = false;

            Model kontrol = new Model();
            kontrol.ad = sozluk.ad;
            kontrol.markaKodu = sozluk.markaKodu;
            IDataReader r = to.Listele(kontrol);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    if (kontrol.ad.Trim().ToUpper() == VeriErisim.DegerAl(r, 2, "").ToUpper())
                    {
                        kayitBulundu = true;
                        break;
                    }
                }
            }
            r.Close();
            if (sozluk.kod > 0)//Eğer kayıtlı marka ise kayıt yapılmasına izin ver
                kayitBulundu = false;

            if (kayitBulundu)
            {
                sonuc.hataStr = string.Format(TasinirMalResI.TMM170, sozluk.ad) + "<br>";
                ve.VeriErisimKapat();
                ve2.VeriErisimKapat();
                return sonuc;
            }
            //***********************************

            ve.BeginTransaction();

            if (sozluk.kod == 0)
                sozluk.kod = OrtakFonksiyonlar.ConvertToInt(tg.BelgeNoAl("MODEL", "0"), 0);

            sonuc = to.Kaydet(sozluk);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TasinirMalResI.TMM135;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Model, taşınır malzemelerin özelliklerinde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Model nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sozluk">Model bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(Kullanici kullanan, Model sozluk)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM071, "");
            string hata = "";

            if (sozluk.markaKodu == 0)
                hata = TasinirMalResI.TMM171;

            if (sozluk.kod == 0)
            {
                if (hata != "") hata += "<br>";
                hata = TasinirMalResI.TMM172;
            }

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABModel to = new TABModel(ve);

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
        /// Verilen kriterlere uygun olan model bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pSozluk">Model bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında model listesi döndürülür.</returns>
        public ObjectArray Listele(Kullanici kullanan, Model pSozluk)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABModel to = new TABModel(ve);
            IDataReader r = to.Listele(pSozluk);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Model sozluk = new Model();

                    sozluk.markaKodu = VeriErisim.DegerAl(r, 0, 0);
                    sozluk.kod = VeriErisim.DegerAl(r, 1, 0);
                    sozluk.ad = VeriErisim.DegerAl(r, 2, "");
                    sozluk.markaAd = VeriErisim.DegerAl(r, 4, "");

                    iVAN.objeler.Add(sozluk);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return iVAN;
        }
    }
}
