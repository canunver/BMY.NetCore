using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Taşınır hesap planı resim bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMHesapPlanResimx
    {
        /// <summary>
        /// Degisken.xml dosyasındaki değişkenlere ulaşan servis
        /// </summary>
        private static DEG.IDEGServis servis = DEG.Arac.Tanimla();

        /// <summary>
        /// Taşınır hesap planı kodunun kod yapısını tutan değişken
        /// </summary>
        private static string kodYapisi = servis.DegiskenDegerBul(0, "/KODYAPI/KOD13/KODYAPI");

        /// <summary>
        /// Taşınır hesap planına ait resim bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen HesapPlaniSatir nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="hpResim">Taşınır hesap planı resim bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, HesapPlaniSatir hpResim)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (string.IsNullOrEmpty(hpResim.hesapKod))
                hata = "Taşınır Hesap Kodu alanı boş bırakılamaz!<br>";

            if (string.IsNullOrEmpty(hpResim.resim))
                hata += "Resim alanı boş bırakılamaz!";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABHesapPlanResim tabHPR = new TABHesapPlanResim(ve);
            ve.BeginTransaction();

            try
            {
                tabHPR.Sil(hpResim);
                sonuc = tabHPR.Kaydet(hpResim);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.Message;
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır hesap planı resim bilgilerini döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="hpResim">Taşınır hesap planı resim bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır hesap planı resim bilgileri listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, HesapPlaniSatir hpResim)
        {
            ObjectArray listeVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABHesapPlanResim tabHPR = new TABHesapPlanResim(ve);
            IDataReader dr = null;

            try
            {
                dr = tabHPR.Listele(hpResim);
            }
            catch (Exception e)
            {
                listeVAN.sonuc.islemSonuc = false;
                listeVAN.sonuc.hataStr = e.Message;
                dr.Close();
                ve.VeriErisimKapat();
                return listeVAN;
            }

            while (dr.Read())
            {
                HesapPlaniSatir gelen = new HesapPlaniSatir();
                gelen.hesapKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                gelen.hesapKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisi, gelen.hesapKod);
                gelen.aciklama = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.resim = VeriErisim.DegerAl(dr, 2, string.Empty);

                listeVAN.objeler.Add(gelen);
            }

            if (listeVAN.objeler.Count == 0)
                listeVAN.sonuc.bilgiStr = TasinirMalResI.TMM029;

            dr.Close();
            ve.VeriErisimKapat();
            return listeVAN;
        }
    }
}