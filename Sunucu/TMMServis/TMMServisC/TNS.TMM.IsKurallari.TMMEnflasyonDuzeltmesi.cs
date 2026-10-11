using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Enflasyon Düzeltmesi bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMEnflasyonDuzeltmesi
    {
        /// <summary>
        /// Enflasyon Düzeltmesi varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen EnflasyonDuzeltmesi nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="enflasyonDuzeltmesi">Enflasyon Düzeltmesi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, EnflasyonDuzeltmesi enflasyonDuzeltmesi)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (enflasyonDuzeltmesi.yil <= 0)
            {
                if (hata != "") hata += "<br>";
                hata += "Yıl alanı sıfırdan büyük olmalıdır.";
            }

            if (enflasyonDuzeltmesi.ay > 12 && enflasyonDuzeltmesi.ay < 0)
            {
                if (hata != "") hata += "<br>";
                hata += "Ay alanı 0-12 aralığında olmalıdır.";
            }

            if (enflasyonDuzeltmesi.yiUfe == 0)
            {
                if (hata != "") hata += "<br>";
                hata += "Yİ-ÜFE değeri sıfırdan farklı olmalıdır.";
            }

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABEnflasyonDuzeltmesi tabEnflasyonDuzeltmesi = new TABEnflasyonDuzeltmesi(ve);
            ve.BeginTransaction();

            try
            {
                if (enflasyonDuzeltmesi.kod == "")
                {
                    enflasyonDuzeltmesi.kod = Guid.NewGuid().ToString();
                    sonuc = tabEnflasyonDuzeltmesi.Kaydet(enflasyonDuzeltmesi);
                }
                else
                    sonuc = tabEnflasyonDuzeltmesi.Guncelle(enflasyonDuzeltmesi);
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
        /// Enflasyon Düzeltmesi, taşınır hesap planında kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen EnflasyonDuzeltmesi nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="enflasyonDuzeltmesi">Enflasyon Düzeltmesi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, EnflasyonDuzeltmesi enflasyonDuzeltmesi)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM071, "");

            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (enflasyonDuzeltmesi.kod == "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM074;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABEnflasyonDuzeltmesi tabEnflasyonDuzeltmesi = new TABEnflasyonDuzeltmesi(ve);
            ve.BeginTransaction();

            try
            {
                sonuc = tabEnflasyonDuzeltmesi.Sil(enflasyonDuzeltmesi);
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
        /// Verilen kriterlere uygun olan Enflasyon Düzeltmesi bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="enflasyonDuzeltmesi">Enflasyon Düzeltmesi bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında Enflasyon Düzeltmesi listesi döndürülür.</returns>
        public ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, EnflasyonDuzeltmesi enflasyonDuzeltmesi)
        {
            ObjectArray listeVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABEnflasyonDuzeltmesi tabEnflasyonDuzeltmesi = new TABEnflasyonDuzeltmesi(ve);
            IDataReader dr = null;

            try
            {
                dr = tabEnflasyonDuzeltmesi.Listele(enflasyonDuzeltmesi);
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
                EnflasyonDuzeltmesi gelen = new EnflasyonDuzeltmesi();
                gelen.kod = VeriErisim.DegerAl(dr, (int)TABEnflasyonDuzeltmesi.ENUMAlanlar.KOD, "");
                gelen.yil = VeriErisim.DegerAl(dr, (int)TABEnflasyonDuzeltmesi.ENUMAlanlar.YIL, 0);
                gelen.ay = VeriErisim.DegerAl(dr, (int)TABEnflasyonDuzeltmesi.ENUMAlanlar.AY, 0);
                gelen.yiUfe = VeriErisim.DegerAl(dr, (int)TABEnflasyonDuzeltmesi.ENUMAlanlar.YIUFE, 0.0);
                gelen.sinir = VeriErisim.DegerAl(dr, (int)TABEnflasyonDuzeltmesi.ENUMAlanlar.SINIR, 0.0);

                listeVAN.objeler.Add(gelen);
            }


            if (listeVAN.objeler.Count == 0)
                listeVAN.sonuc.bilgiStr = TasinirMalResI.TMM029;

            listeVAN.sonuc.islemSonuc = true;

            dr.Close();
            ve.VeriErisimKapat();
            return listeVAN;
        }
    }
}
