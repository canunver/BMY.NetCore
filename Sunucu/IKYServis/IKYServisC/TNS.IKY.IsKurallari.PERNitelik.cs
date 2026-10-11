using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.IKY.IsKurallari.Veri;

namespace TNS.IKY.IsKurallari
{
    /// <summary>
    /// Personel nitelik bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class PERNitelik
    {
        /// <summary>
        /// Personel nitelik bilgilerini varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Personel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// Personelin sistemde kayıtlı olması gerekmektedir, eğer kayıtlı değilse hata döndürülür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="personel">Personel nitelik bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, PersonelBilgiBirikimi personel)
        {
            //Sonuc sonuc = new Sonuc(true, "", "");
            //if (!TNS.IKY.IsKurallari.IKYYetki.YetkiVarMiYazmaya(kullanan, personel.))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = PersonelResI.PER011;
            //    return sonuc;
            //}

            //string hata = string.Empty;
            //if (string.IsNullOrEmpty(personel.tcKimlikNo))
            //    hata = "TC Kimlik numarası boş bırakılamaz!" + "<br>";
            //if (string.IsNullOrEmpty(personel.gorev)
            //    && string.IsNullOrEmpty(personel.bilgi)
            //    && string.IsNullOrEmpty(personel.beceri))
            //    hata += "Görev, bilgi ve beceri alanlarında bir tanesi doldurulmalıdır!" + "<br>";
            //if (!string.IsNullOrEmpty(hata))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = hata;
            //    return sonuc;
            //}

            //VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            //TABNitelik tPersonel = new TABNitelik(ve);
            //ve.BeginTransaction();

            //if (!tPersonel.PersonelVarMiView(personel.tcKimlikNo))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = string.Format("TC Kimlik numarası personel listesinde bulunamadı!", personel.tcKimlikNo) + "<br>";
            //}

            //if (sonuc.islemSonuc)
            //{
            //    if (tPersonel.PersonelVarMi(personel.tcKimlikNo))
            //        sonuc = tPersonel.Guncelle(personel);
            //    else
            //        sonuc = tPersonel.Kaydet(personel);
            //}

            //if (sonuc.islemSonuc)
            //{
            //    sonuc.bilgiStr = "İşlem Başarılı.";
            //    ve.Commit();
            //}
            //else
            //{
            //    sonuc.hataStr = "Kayıt işlemi başarısız oldu!" + "<br>" + sonuc.hataStr;
            //    ve.Rollback();
            //}

            //ve.VeriErisimKapat();
            return new Sonuc("Hazır değil");
        }

        ///// <summary>
        ///// Personel nitelik bilgilerini silen yordam
        ///// Kullanıcı işleme yetkili değilse veya gönderilen Personel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="personel">Personel nitelik bilgilerini tutan nesne</param>
        ///// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        //public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, PersonelBilgiBirikimi personel)
        //{
        //    Sonuc sonuc = Yetki.YazmaHakkiVarMi(kullanan);
        //    if (!sonuc.islemSonuc)
        //        return sonuc;

        //    string hata = string.Empty;
        //    if (string.IsNullOrEmpty(personel.tcKimlikNo))
        //        hata = PerformansButceResI.PEB095 + "<br>";
        //    if (!string.IsNullOrEmpty(hata))
        //    {
        //        sonuc.islemSonuc = false;
        //        sonuc.hataStr = hata;
        //        return sonuc;
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB");
        //    TABPersonel tPersonel = new TABPersonel(ve);
        //    ve.BeginTransaction();

        //    //if (tPersonel.PersonelKullanilmisMi(personel.tcKimlikNo))
        //    //{
        //    //    ve.VeriErisimKapat();
        //    //    sonuc.hataStr = "Daha önce kullanıldığı için bu personel silinemez!";
        //    //    return sonuc;
        //    //}

        //    sonuc = tPersonel.Sil(personel);
        //    if (sonuc.islemSonuc)
        //    {
        //        ve.Commit();
        //        sonuc.bilgiStr = PerformansButceResI.PEB020;
        //    }
        //    else
        //    {
        //        ve.Rollback();
        //        sonuc.hataStr = PerformansButceResI.PEB021 + "<br>" + sonuc.hataStr;
        //    }

        //    ve.VeriErisimKapat();
        //    return sonuc;
        //}

        /// <summary>
        /// Verilen kriterlere uygun olan personel nitelik bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="personel">Personel nitelik bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında personel nitelik listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, PersonelBilgiBirikimi personel)
        {
            ObjectArray perVAN = new ObjectArray();
            //VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            //TABNitelik tPersonel = new TABNitelik(ve);

            //IDataReader dr = tPersonel.Listele(personel);
            //if (dr != null)
            //{
            //    while (dr.Read())
            //    {
            //        PersonelBilgiBirikimi gelen = new PersonelBilgiBirikimi();
            //        gelen.tcKimlikNo = VeriErisim.DegerAl(dr, 0, string.Empty);
            //        gelen.gorev = VeriErisim.DegerAl(dr, 4, string.Empty);
            //        gelen.bilgi = VeriErisim.DegerAl(dr, 5, string.Empty);
            //        gelen.beceri = VeriErisim.DegerAl(dr, 6, string.Empty);

            //        perVAN.objeler.Add(gelen);
            //    }
            //    dr.Close();
            //}
            //else
            //{
            //    ve.VeriErisimKapat();
            //    throw new Exception(ve.hata);
            //}
            //if (perVAN.objeler.Count == 0)
            //    perVAN.sonuc.bilgiStr = "Verdiğiniz kriterlere uyan kayıt bulunamadı!";

            //ve.VeriErisimKapat();
            perVAN.sonuc.DegerAta("Hazır değil");
            return perVAN;
        }
    }
}