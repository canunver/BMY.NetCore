using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Ambar bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMAmbar
    {
        /// <summary>
        /// Ambar varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Ambar nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ambar">Ambar bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Ambar ambar)
        {
            string kodYapisi = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD11/KODYAPI");


            Sonuc sonuc = new Sonuc();

            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (ambar.muhasebeKod.Trim() == string.Empty)
                sonuc.hataStr = TasinirMalResI.TMM013 + "<br>";

            if (ambar.harcamaBirimKod.Trim() == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM014 + "<br>";

            if (ambar.kod.Trim() == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM031 + "<br>";

            if (ambar.ad.Trim() == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM032 + "<br>";

            if (sonuc.hataStr != string.Empty)
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            Sonuc kodSonuc = OrtakFonksiyonlar.KodYapiKontrol(kodYapisi, ambar.kod, true);

            if (!kodSonuc.islemSonuc)
            {
                sonuc.hataStr = kodSonuc.hataStr;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAmbar tabAmbar = new TABAmbar(ve);
            TABHarcamaBirimi tabHarcamaBirimi = new TABHarcamaBirimi(ve);

            HarcamaBirimi hb = new HarcamaBirimi();
            hb.muhasebeKod = ambar.muhasebeKod;
            hb.kod = ambar.harcamaBirimKod;
            IDataReader dr = tabHarcamaBirimi.Listele(hb);
            if (!dr.Read())
            {
                dr.Close();
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM033;
                return sonuc;
            }
            dr.Close();
            ve.BeginTransaction();

            sonuc = tabAmbar.Kaydet(ambar);

            if (sonuc.islemSonuc)
            {
                //TABImza ti = new TABImza(ve);
                //ImzaBilgisi imza = new ImzaBilgisi();
                //imza.muhasebe = ambar.muhasebeKod;
                //imza.birim = ambar.harcamaBirimKod;
                //imza.ambar = ambar.kod;
                //imza.imzaYer = (int)ENUMImzaYer.TASINIRKAYITVEKONTROLYETKILISI;
                //imza.adSoyad = ambar.TKKYAd;
                //ti.ImzaSil(imza.muhasebe, imza.birim, imza.ambar, imza.imzaYer);
                //ti.ImzaKaydet(imza);

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
        /// Parametre olarak verilen ambar bilgilerini silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Ambar nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ambar">Ambar bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, Ambar ambar)
        {
            Sonuc sonuc = new Sonuc();

            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (ambar.muhasebeKod.Trim() == string.Empty)
                sonuc.hataStr = TasinirMalResI.TMM013 + "<br>";

            if (ambar.harcamaBirimKod.Trim() == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM014 + "<br>";

            if (ambar.kod.Trim() == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM031 + "<br>";

            if (sonuc.hataStr != string.Empty)
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAmbar tabAmbar = new TABAmbar(ve);
            ve.BeginTransaction();

            sonuc = tabAmbar.Sil(ambar);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = string.Format(TasinirMalResI.TMM034, ambar.kod);
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
        /// Verilen kriterlere uygun olan ambar bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ambar">Ambar bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ambar listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Ambar ambar)
        {
            string kodYapisiHarcama = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD10/KODYAPI");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectArray bilgi = new ObjectArray();

            string[] hbler = new string[500];
            int hbSayac = 0;

            if (kullanan != null)
            {
                string yetkiTipiAdi = "";
                yetkiTipiAdi = "Taşınır Muhasebe Harcama Birim Yetkisi";
                string yetkiTipiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);

                yetkiTipiAdi = "Taşınır Kullanıcı Birimi Yetkisi";
                yetkiTipiDegeri += ";" + OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);

                if (yetkiTipiDegeri.Trim() != "")
                {
                    string[] d = yetkiTipiDegeri.Split(';');

                    for (int i = 0; i < d.Length; i++)
                    {
                        string[] muhasebeBirim = d[i].Split('-');
                        if (muhasebeBirim.Length != 3 || muhasebeBirim[0] == "*")
                            continue;

                        hbler[hbSayac] = muhasebeBirim[2];
                        hbSayac++;
                    }
                }
            }

            TABAmbar tabAmbar = new TABAmbar(ve);
            IDataReader dr = tabAmbar.Listele(ambar);

            while (dr.Read())
            {
                Ambar amb = new Ambar();
                amb.ilKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                amb.ilceKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                amb.muhasebeKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                amb.harcamaBirimKod = VeriErisim.DegerAl(dr, 3, string.Empty);
                amb.harcamaBirimKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHarcama, amb.harcamaBirimKod);
                amb.kod = VeriErisim.DegerAl(dr, 4, string.Empty);
                amb.ad = VeriErisim.DegerAl(dr, 5, string.Empty);
                amb.adres = VeriErisim.DegerAl(dr, 6, string.Empty);
                amb.harcamaBirimAd = VeriErisim.DegerAl(dr, 7, string.Empty);
                amb.muhasebeAd = VeriErisim.DegerAl(dr, 8, string.Empty);
                amb.il = VeriErisim.DegerAl(dr, 9, string.Empty);
                amb.ilce = VeriErisim.DegerAl(dr, 10, string.Empty);
                amb.TKKYAd = VeriErisim.DegerAl(dr, 11, string.Empty);
                amb.TKKYUnvan = VeriErisim.DegerAl(dr, 12, "");
                amb.kapali = VeriErisim.DegerAl(dr, 13, 0);
                amb.kullaniciBirimi = VeriErisim.DegerAl(dr, 14, 0);
                amb.hediye = VeriErisim.DegerAl(dr, 15, 0);
                amb.TKKYAd2 = VeriErisim.DegerAl(dr, 16, string.Empty);
                amb.TKKYUnvan2 = VeriErisim.DegerAl(dr, 17, "");

                bool vardir = false;
                if (hbSayac > 0)
                {
                    for (int i = 0; i < hbSayac; i++)
                    {
                        if (amb.kod.Replace(".", "") == hbler[i].Replace(".", "") || hbler[i] == "*")
                        {
                            vardir = true;
                            break;
                        }
                    }
                }
                else
                    vardir = true;

                if (vardir || amb.kullaniciBirimi == 0)
                    bilgi.objeler.Add(amb);
            }

            dr.Close();
            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Yenis the alan olustur.
        /// </summary>
        public void YeniAlanOlustur()
        {
            //VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));

            //TABAmbar to = new TABAmbar(ve);

            //to.YeniAlanOlustur();

            //ve.VeriErisimKapat();
        }
    }
}