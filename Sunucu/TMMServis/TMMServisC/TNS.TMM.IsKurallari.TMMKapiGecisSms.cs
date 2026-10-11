using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// RFID etiketi olan taşınır demirbaşlarının kapılardan izinsiz geçişi ile ilgili bilgilendirilecek
    /// personele ait iletişim bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMKapiGecisSms
    {
        /// <summary>
        /// RFID entegrasyonunda kurumdan çıkışına izin verilmeyen malzemeler kurumdan çıkarılacak
        /// olursa bilgilendirilecek personele ait iletişim bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Personel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="personel">Bilgi verilecek personele ait iletişim bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Personel personel)
        {
            Sonuc sonuc = new Sonuc();

            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (string.IsNullOrEmpty(personel.kod))
                sonuc.hataStr = TasinirMalResI.TMM152 + "<br>";
            if (string.IsNullOrEmpty(personel.cepTel))
                sonuc.hataStr += TasinirMalResI.TMM153 + "<br>";
            else if (!personel.cepTel.StartsWith("+90") || personel.cepTel.Length != 13)
                sonuc.hataStr += TasinirMalResI.TMM154 + "<br>";
            if (string.IsNullOrEmpty(personel.eposta))
                sonuc.hataStr += TasinirMalResI.TMM155;

            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKapiGecisSms tabKGS = new TABKapiGecisSms(ve);
            ve.BeginTransaction();

            if (!Genel.PersonelVarMi(ve, personel.kod))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM156;
            }

            if (sonuc.islemSonuc)
            {
                try
                {
                    tabKGS.Sil(personel);
                    sonuc = tabKGS.Kaydet(personel);
                }
                catch (Exception e)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = e.Message;
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
        /// RFID entegrasyonunda kurumdan çıkışına izin verilmeyen malzemeler kurumdan çıkarılacak
        /// olursa bilgilendirilecek personele ait iletişim bilgilerini silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Personel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="personel">İletişim bilgileri silinecek personele ait bilgileri tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, Personel personel)
        {
            Sonuc sonuc = new Sonuc();

            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (string.IsNullOrEmpty(personel.kod))
                sonuc.hataStr = TasinirMalResI.TMM152 + "<br>";
            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKapiGecisSms tabKGS = new TABKapiGecisSms(ve);
            ve.BeginTransaction();

            try
            {
                sonuc = tabKGS.Sil(personel);
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
        /// RFID entegrasyonunda kurumdan çıkışına izin verilmeyen malzemeler kurumdan çıkarılacak
        /// olursa bilgilendirilecek personele ait iletişim bilgilerini döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="personel">Listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında personel iletişim bilgileri listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Personel personel)
        {
            ObjectArray listeVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKapiGecisSms tabKGS = new TABKapiGecisSms(ve);
            IDataReader dr = null;

            try
            {
                dr = tabKGS.Listele(personel);
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
                Personel gelen = new Personel();
                gelen.kod = VeriErisim.DegerAl(dr, 0, string.Empty);
                gelen.cepTel = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.smsAt = VeriErisim.DegerAl(dr, 2, 0) == 1 ? true : false;
                gelen.eposta = VeriErisim.DegerAl(dr, 3, string.Empty);
                gelen.epostaAt = VeriErisim.DegerAl(dr, 4, 0) == 1 ? true : false;

                gelen.ad = TMMServis.UzayYaziDegerGetir("TASPERSONEL", gelen.kod, true, string.Empty);

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