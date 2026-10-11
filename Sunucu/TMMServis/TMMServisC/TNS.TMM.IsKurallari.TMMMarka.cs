using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Marka bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMMarka
    {
        /// <summary>
        /// Marka varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Marka nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sozluk">Marka bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(Kullanici kullanan, Marka sozluk)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");
            string hata = "";

            if (sozluk.ad == string.Empty)
                hata += TasinirMalResI.TMM075 + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABMarka to = new TABMarka(ve);
            TABGenel tg = new TABGenel(ve2);

            //***********************************************
            //Kontrol
            bool kayitBulundu = false;

            Marka kontrol = new Marka();
            kontrol.ad = sozluk.ad;
            IDataReader r = to.Listele(kontrol);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    if (kontrol.ad.Trim().ToUpper() == VeriErisim.DegerAl(r, 1, "").ToUpper())
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
                sonuc.hataStr = string.Format(TasinirMalResI.TMM164, sozluk.ad) + "<br>";
                ve.VeriErisimKapat();
                ve2.VeriErisimKapat();
                return sonuc;
            }
            //***********************************
            ve.BeginTransaction();

            if (sozluk.kod == 0)
                sozluk.kod = OrtakFonksiyonlar.ConvertToInt(tg.BelgeNoAl("MARKA", "0"), 0);

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
            ve2.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Marka, modellerde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Marka nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sozluk">Marka bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(Kullanici kullanan, Marka sozluk)
        {
            Sonuc sonuc = new Sonuc();

            if (sozluk.kod == 0)
                sonuc.hataStr = TasinirMalResI.TMM165;

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABMarka to = new TABMarka(ve);

            //Bu Markaya ait model var mı?
            TABModel tabModel = new TABModel(ve);
            Model model = new Model();
            model.markaKodu = sozluk.kod;
            IDataReader drModel = tabModel.Listele(model);

            if (drModel.Read())
            {
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                sonuc.hataStr = TasinirMalResI.TMM166;
                drModel.Close();
                return sonuc;
            }

            drModel.Close();

            ve.BeginTransaction();

            sonuc = to.Sil(sozluk);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = string.Format(TasinirMalResI.TMM167, sozluk.ad);
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
        /// Verilen kriterlere uygun olan marka bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pSozluk">Marka bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında marka listesi döndürülür.</returns>
        public ObjectArray Listele(Kullanici kullanan, Marka pSozluk)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABMarka to = new TABMarka(ve);
            IDataReader r = to.Listele(pSozluk);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Marka sozluk = new Marka();

                    sozluk.kod = VeriErisim.DegerAl(r, 0, 0);
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
