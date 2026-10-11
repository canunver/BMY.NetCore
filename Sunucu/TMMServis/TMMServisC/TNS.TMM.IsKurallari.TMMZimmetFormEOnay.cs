using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Zimmet fişi bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMZimmetFormEOnay
    {
        /// <summary>
        /// Zimmet fişi eonay varsa güncelleyen, yoksa kaydeden yordam
        /// Zimmet fişi eonay bilgileri ile ilgili bir takım kontroller yapılmakta,
        /// sorunsuz bir bilgi kümesi olduğuna karar verilirse kayıt işlemi yapılmaktadır.
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetFormEOnay nesneleri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="zimmet">Zimmet fişi eonay bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ZimmetFormEOnay zfeonay)
        {
            Sonuc sonuc = new Sonuc();

            sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, zfeonay.muhasebeKod, zfeonay.harcamaBirimKod, null);
            if (!sonuc.islemSonuc)
                sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMiKullaniciBirimi(kullanan, zfeonay.muhasebeKod, zfeonay.harcamaBirimKod, null);

            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = TasinirMalResI.TMM050;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABZimmetFormEOnay tabZimmetFormEOnay = new TABZimmetFormEOnay(ve);

            ve.BeginTransaction();

            sonuc = tabZimmetFormEOnay.Kaydet(zfeonay);

            string mailAdres = TMMServis.UzayYaziDegerGetir("TASPERSONELTEL", zfeonay.emailKime, true, "");

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = string.Format(TasinirMalResI.TMM407, zfeonay.emailKimeAd + " - " + mailAdres);
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TasinirMalResI.TMM200 + "";
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            if (string.IsNullOrEmpty(mailAdres))
            {
                sonuc.hataStr = TasinirMalResI.TMM411;
                return sonuc;
            }

            string yazilimAd = System.Configuration.ConfigurationManager.AppSettings.Get("YazilimAd");
            string yazilimMailAdresi = System.Configuration.ConfigurationManager.AppSettings.Get("YazilimMailAdresi");
            string yazilimURL = System.Configuration.ConfigurationManager.AppSettings.Get("YazilimURL");
            string yazilimeOnayURL = System.Configuration.ConfigurationManager.AppSettings.Get("yazilimeOnayURL");

            string bilgi = yazilimAd + "<br />" + string.Format(TasinirMalResI.TMM410, zfeonay.fisNo, yazilimeOnayURL) + "<br />" + string.Format(TasinirMalResI.TMM413, TasinirMalResI.TMM412) + "<br />";

            try
            {
                OrtakFonksiyonlar.MailAt(yazilimMailAdresi, mailAdres, TasinirMalResI.TMM409, bilgi, true, false, new TNSCollection());
                sonuc.bilgiStr = mailAdres + "<br /> adresine mail gönderilmiştir.";
            }
            catch
            {
                sonuc.hataStr = TasinirMalResI.TMM408;
            }

            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan zimmet fişlerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="zimmet">Zimmet fişi bilgilerini tutan nesne</param>
        /// <param name="kriter">Sorgulama için ek kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında zimmet fişleri listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, ZimmetFormEOnay zfeonay, ZimmetFormEOnayKriter kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            bilgi.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, zfeonay.muhasebeKod, zfeonay.harcamaBirimKod, null);
            if (!bilgi.sonuc.islemSonuc)
                bilgi.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMiKullaniciBirimi(kullanan, zfeonay.muhasebeKod, zfeonay.harcamaBirimKod, null);

            if (!bilgi.sonuc.islemSonuc)
            {
                bilgi.sonuc.hataStr = TasinirMalResI.TMM060;
                return bilgi;
            }

            if (!(kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)))
            {
                if (zfeonay.yil == 0)
                    bilgi.sonuc.hataStr += TasinirMalResI.TMM036 + "<br>";
                //Yetkisi var ise boş bırakabilir
                //if (zimmet.muhasebeKod == string.Empty)
                //    bilgi.sonuc.hataStr += TasinirMalResI.TMM142 + "<br>";
                //if (zimmet.harcamaBirimKod == string.Empty)
                //    bilgi.sonuc.hataStr += TasinirMalResI.TMM014 + "<br>";
                //if (zimmet.ambarKod == string.Empty)
                //    bilgi.sonuc.hataStr += TasinirMalResI.TMM031 + "<br>";
                if (zfeonay.belgeTur == 0)
                    bilgi.sonuc.hataStr += TasinirMalResI.TMM292 + "<br>";

                if (bilgi.sonuc.hataStr != string.Empty)
                {
                    bilgi.sonuc.islemSonuc = false;
                    return bilgi;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABZimmetFormEOnay tabZimmetFormEOnay = new TABZimmetFormEOnay(ve);

            IDataReader dr = tabZimmetFormEOnay.Listele(zfeonay, kriter);

            while (dr.Read())
            {
                ZimmetFormEOnay zimFormEOnay = new ZimmetFormEOnay();

                zimFormEOnay.yil = zfeonay.yil;
                zimFormEOnay.muhasebeKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                zimFormEOnay.harcamaBirimKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                zimFormEOnay.fisNo = VeriErisim.DegerAl(dr, 3, string.Empty);
                zimFormEOnay.belgeTur = VeriErisim.DegerAl(dr, 4, 0);
                zimFormEOnay.emailKimden = VeriErisim.DegerAl(dr, 5, string.Empty);
                zimFormEOnay.emailKime = VeriErisim.DegerAl(dr, 6, string.Empty);
                zimFormEOnay.emailGondermeTarih = VeriErisim.DegerAl(dr, 7);
                zimFormEOnay.emailCevapTarih = VeriErisim.DegerAl(dr, 8);
                zimFormEOnay.durum = VeriErisim.DegerAl(dr, 9, 0);

                zimFormEOnay.muhasebeAd = TMMServis.UzayYaziDegerGetir("TASMUHASEBE", zimFormEOnay.muhasebeKod, true, "");
                //zimFormEOnay.harcamaBirimAd = TMMServis.UzayYaziDegerGetir("TASHARCAMA", zimFormEOnay.harcamaBirimKod, true, "");

                bilgi.Ekle(zimFormEOnay);
            }

            dr.Close();
            ve.VeriErisimKapat();

            return bilgi;
        }


        /// <summary>
        /// Verilen kriterlere uygun olan zimmet fişlerini kişi/ortak alan eonay bilgisi ile döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="zimmet">Zimmet fişi bilgilerini tutan nesne</param>
        /// <param name="kriter">Sorgulama için ek kriter bilgilerini tutan nesne</param>
        /// <param name="eonaykriter"> Sorgulama için eonay kriter bilgilerini tutan nesne</param> 
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında zimmet fişleri listesi döndürülür.</returns>
        public static ObjectArray ListeleKisiOrtakEOnayBilgisiyle(VeriErisim veriErisim, ZimmetFormEOnayKriter eonaykriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            TABZimmetFormEOnay tabZimmetFormEOnay = new TABZimmetFormEOnay(ve);

            IDataReader dr = tabZimmetFormEOnay.ListeleKisiOrtakEOnayli(eonaykriter);

            while (dr.Read())
            {
                ZimmetFormEOnay zimmetFormEonay = new ZimmetFormEOnay();

                zimmetFormEonay.yil = VeriErisim.DegerAl(dr, 0, 0);
                zimmetFormEonay.muhasebeKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                zimmetFormEonay.harcamaBirimKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                zimmetFormEonay.fisNo = VeriErisim.DegerAl(dr, 3, string.Empty);
                zimmetFormEonay.belgeTur = VeriErisim.DegerAl(dr, 4, 0);
                zimmetFormEonay.emailKimden = VeriErisim.DegerAl(dr, 5, string.Empty);
                zimmetFormEonay.emailKimdenAd = VeriErisim.DegerAl(dr, 6, string.Empty);
                zimmetFormEonay.emailKime = VeriErisim.DegerAl(dr, 7, string.Empty);
                zimmetFormEonay.emailKimeAd = VeriErisim.DegerAl(dr, 8, string.Empty);
                zimmetFormEonay.emailGondermeTarih = VeriErisim.DegerAl(dr, 9);
                zimmetFormEonay.emailCevapTarih = VeriErisim.DegerAl(dr, 10);
                zimmetFormEonay.durum = VeriErisim.DegerAl(dr, 11, 0);

                zimmetFormEonay.muhasebeAd = VeriErisim.DegerAl(dr, 12, string.Empty);
                zimmetFormEonay.harcamaBirimAd = VeriErisim.DegerAl(dr, 13, string.Empty);

                bilgi.Ekle(zimmetFormEonay);
            }

            dr.Close();
            ve.VeriErisimKapat();

            return bilgi;
        }


        /// <summary>
        /// Verilen zimmet fişinin detay bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="zimmet">Zimmet fişi bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında zimmet fişinin detay bilgileri döndürülür.</returns>
        //public static ObjectArray ListeleDetay(VeriErisim veriErisim, Kullanici kullanan, ZimmetForm zimmet)
        //{
        //    ObjectArray bilgi = new ObjectArray();

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABZimmetDetay tabZimmetDetay = new TABZimmetDetay(ve);

        //    ZimmetDetay zd = new ZimmetDetay();
        //    zd.yil = zimmet.yil;
        //    zd.muhasebeKod = zimmet.muhasebeKod;
        //    zd.harcamaBirimKod = zimmet.harcamaBirimKod;
        //    zd.fisNo = zimmet.fisNo;
        //    zd.belgeTur = zimmet.belgeTur;

        //    IDataReader dr = tabZimmetDetay.Listele(zd);

        //    while (dr.Read())
        //    {
        //        ZimmetDetay zDetay = new ZimmetDetay();

        //        zDetay.prSicilNo = VeriErisim.DegerAl(dr, 5, 0);
        //        zDetay.siraNo = VeriErisim.DegerAl(dr, 6, 0);
        //        zDetay.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisi, VeriErisim.DegerAl(dr, 7, ""));
        //        zDetay.gorSicilNo = TMM.IsKurallari.Genel.GorSicilNoGetir(zimmet.muhasebeKod, zimmet.harcamaBirimKod, zDetay.prSicilNo);
        //        zDetay.ozellik = VeriErisim.DegerAl(dr, 8, "");
        //        zDetay.kdvOran = VeriErisim.DegerAl(dr, 9, 0);
        //        zDetay.birimFiyat = VeriErisim.DegerAl(dr, 10, (decimal)0);
        //        zDetay.teslimDurum = VeriErisim.DegerAl(dr, 11, "");
        //        zDetay.hesapPlanAd = VeriErisim.DegerAl(dr, 12, "");

        //        //Barkod yazdırmada gerekli oldu.
        //        zDetay.ozellikSicil.adi = VeriErisim.DegerAl(dr, 13, "");
        //        zDetay.ozellikSicil.yazarAdi = VeriErisim.DegerAl(dr, 14, "");
        //        zDetay.ozellikSicil.disSicilNo = VeriErisim.DegerAl(dr, 15, "");
        //        zDetay.ozellikSicil.yayinTarihi = VeriErisim.DegerAl(dr, 16, "");
        //        zDetay.ozellikSicil.ciltNo = VeriErisim.DegerAl(dr, 17, "");
        //        zDetay.ozellikSicil.yeriKonusu = VeriErisim.DegerAl(dr, 18, "");

        //        if (Genel.rfIdVarMi)
        //            zDetay.rfIdNo = Genel.PrSicilNoRFIDDondur(zDetay.prSicilNo);

        //        bilgi.Ekle(zDetay);
        //    }

        //    if (bilgi.ObjeSayisi < 1)
        //    {
        //        bilgi.sonuc.islemSonuc = false;
        //        bilgi.sonuc.hataStr = TasinirMalResI.TMM088;
        //    }

        //    dr.Close();
        //    ve.VeriErisimKapat();

        //    return bilgi;
        //}



        /// <summary>
        /// Zimmet fişinin eOnay durumunu güncelleyen yordam
        /// Parametre olarak verilen yeniDurum bilgisine bakarak zimmet fişine ve detay bilgilerine bir takım
        /// kontroller uygular, verilen fiş bilgileri bütün kontrolleri sorunsuz geçerse belgeye yeni durum atanır.
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="zf">Zimmet fişi bilgilerini tutan nesne</param>
        /// <param name="yeniDurum">Belgeye atanması istenen yeni durum bilgisi - Gönderildi, Cevap KABUL, Cevap RED değerlerini alabilir.</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc BelgeEOnayDurumDegistir(VeriErisim veriErisim, Kullanici kullanan, ZimmetFormEOnay zfeOnay, string yeniDurum, string personelKod)
        {
            Sonuc sonuc = new Sonuc();
        
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABZimmetFormEOnay tabZimmetFormEOnay = new TABZimmetFormEOnay(ve);

        //    IDataReader dr = tabZimmetFormEOnay.BelgeDurumGetir(zf);

        //    if (!dr.Read())
        //    {
        //        sonuc.islemSonuc = false;
        //        sonuc.hataStr = TasinirMalResI.TMM086;
        //        dr.Close();
        //        ve.VeriErisimKapat();
        //        return sonuc;
        //    }

        //    int durum = VeriErisim.DegerAl(dr, 0, 0);
        //    dr.Close();


            ve.BeginTransaction();

 
            sonuc = tabZimmetFormEOnay.BelgeEOnayDurumDegistir(zfeOnay, yeniDurum, personelKod);
            if (sonuc.islemSonuc)
            {


                sonuc.bilgiStr = string.Format(TasinirMalResI.TMM318, zfeOnay.fisNo) + "<br>";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = string.Format(TasinirMalResI.TMM319, zfeOnay.fisNo) + "<br>";
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

    }
}