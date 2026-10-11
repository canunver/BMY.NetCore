using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// RFID etiketi olan taşınır demirbaşlarının kapılardan geçiş izni bilgileri ile ilgili,
    /// iş kuralları kontrollerini yapıp, veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını
    /// çağırarak okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMKapiGecisSicil
    {

        /// <summary>
        /// Parametre olarak verilen yetki kontrolü yapılmış SicilNoHareket nesneleri listesinde aranan
        /// değişkeninin muhasebe birimi, harcama birimi ve ambarı var mı kontrolünü yapan yordam
        /// </summary>
        /// <param name="kontrolEdilenler">Yetki kontrolü yapılmış SicilNoHareket nesneleri listesini tutan nesne</param>
        /// <param name="aranan">Aranan SicilNoHareket nesnesi</param>
        /// <returns>Aranan nesne listede bulunursa true, bulunamazsa false döndürülür.</returns>
        private static bool YetkiKontroluYapilmisMi(TNSCollection kontrolEdilenler, SicilNoHareket aranan)
        {
            foreach (SicilNoHareket s in kontrolEdilenler)
            {
                if (s.muhasebeKod == aranan.muhasebeKod &&
                    s.harcamaBirimKod == aranan.harcamaBirimKod &&
                    s.ambarKod == aranan.ambarKod)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// RFID entegrasyonunda kurumdan çıkışına izin verilen malzemeleri kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen demirbaşlardan herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="siciller">Geçişine izin verilen demirbaşların listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TNSCollection siciller)
        {
            Sonuc sonuc = new Sonuc();

            SicilNoHareket snh = null;
            if (siciller.Count <= 0)
                sonuc.hataStr = TasinirMalResI.TMM148;
            else
            {
                snh = (SicilNoHareket)siciller[0];
                if (string.IsNullOrEmpty(snh.muhasebeKod))
                    sonuc.hataStr = TasinirMalResI.TMM013 + "<br>";
                if (string.IsNullOrEmpty(snh.harcamaBirimKod))
                    sonuc.hataStr += TasinirMalResI.TMM014;
            }

            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            TNSCollection kontrolEdilenler = new TNSCollection();
            foreach (SicilNoHareket s in siciller)
            {
                if (YetkiKontroluYapilmisMi(kontrolEdilenler, s))
                    continue;

                if (snh != null)
                    sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, snh.muhasebeKod, snh.harcamaBirimKod, snh.ambarKod);

                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = TasinirMalResI.TMM050;
                    return sonuc;
                }
                kontrolEdilenler.Add(s);
            }

            foreach (SicilNoHareket sicil in siciller)
            {
                if (sicil.prSicilNo <= 0)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = TasinirMalResI.TMM149;
                    return sonuc;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKapiGecisSicil tabKGS = new TABKapiGecisSicil(ve);
            ve.BeginTransaction();

            try
            {
                //tabKGS.Sil(snh);
                foreach (SicilNoHareket sicil in siciller)
                {
                    sonuc = tabKGS.Kaydet(sicil.prSicilNo);
                    if (!sonuc.islemSonuc)
                        break;
                }
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
        /// RFID entegrasyonunda kurumdan çıkışına izin verilen malzemelerin izinlerini silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen demirbaşlardan herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="siciller">Geçiş izni silinen demirbaşların listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, TNSCollection siciller)
        {
            Sonuc sonuc = new Sonuc();

            SicilNoHareket snh = null;
            if (siciller.Count <= 0)
                sonuc.hataStr = TasinirMalResI.TMM148;
            else
            {
                snh = (SicilNoHareket)siciller[0];
                if (string.IsNullOrEmpty(snh.muhasebeKod))
                    sonuc.hataStr = TasinirMalResI.TMM013 + "<br>";
                if (string.IsNullOrEmpty(snh.harcamaBirimKod))
                    sonuc.hataStr += TasinirMalResI.TMM014;
            }

            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            TNSCollection kontrolEdilenler = new TNSCollection();
            foreach (SicilNoHareket s in siciller)
            {
                if (YetkiKontroluYapilmisMi(kontrolEdilenler, s))
                    continue;

                if (snh != null)
                    sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, snh.muhasebeKod, snh.harcamaBirimKod, snh.ambarKod);

                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = TasinirMalResI.TMM050;
                    return sonuc;
                }
                kontrolEdilenler.Add(s);
            }

            foreach (SicilNoHareket sicil in siciller)
            {
                if (sicil.prSicilNo <= 0)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = TasinirMalResI.TMM149;
                    return sonuc;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKapiGecisSicil tabKGS = new TABKapiGecisSicil(ve);
            ve.BeginTransaction();

            try
            {

                foreach (SicilNoHareket sicil in siciller)
                {
                    sonuc = tabKGS.Sil(sicil);
                    if (!sonuc.islemSonuc)
                        break;
                }
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
        /// Parametre olarak verilen demirbaşın RFID entegrasyonunda kurumdan
        /// çıkışına izin verilip verilmediği bilgisini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="prSicilNo">Demirbaşın programatik sicil numarası</param>
        /// <param name="rfIdNo">Demirbaşın RFID numarası</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc SicilGecebilirMi(VeriErisim veriErisim, Kullanici kullanan, int prSicilNo, int rfIdNo)
        {
            SicilNoHareket s = new SicilNoHareket();
            s.prSicilNo = prSicilNo;
            s.rfIdNo = rfIdNo;

            ObjectArray gelen = SicilListele(veriErisim, kullanan, s, true);

            if (gelen.sonuc.islemSonuc)
            {
                if (gelen.objeler.Count > 0)
                {
                    gelen.sonuc.islemSonuc = ((SicilNoHareket)gelen.objeler[0]).kapiGecis;
                    if (!gelen.sonuc.islemSonuc)
                        gelen.sonuc.hataStr = TasinirMalResI.TMM150;
                }
                else
                {
                    gelen.sonuc.islemSonuc = false;
                    gelen.sonuc.hataStr = TasinirMalResI.TMM029;
                }
            }

            return gelen.sonuc;
        }

        /// <summary>
        /// RFID entegrasyonunda kurumdan çıkışına izin verilen malzemelerin verilen kriterlere uyanlarını listeleyen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen SicilNoHareket nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicil">Geçişine izin verilen demirbaşları listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında geçişine izin verilen malzeme bilgileri listesi döndürülür.</returns>
        public static ObjectArray SicilListele(VeriErisim veriErisim, Kullanici kullanan, SicilNoHareket sicil)
        {
            return SicilListele(veriErisim, kullanan, sicil, false);
        }

        /// <summary>
        /// RFID entegrasyonunda kurumdan çıkışına izin verilen malzemelerin verilen kriterlere uyanlarını listeleyen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen SicilNoHareket nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicil">Geçişine izin verilen demirbaşları listeleme kriterleri</param>
        /// <param name="tekSicil">sicil parametresinde sadece demirbaş programatik sicil numarası ve/veya RFID numarası verilmiş ise true olmalı.</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında geçişine izin verilen malzeme bilgileri listesi döndürülür.</returns>
        private static ObjectArray SicilListele(VeriErisim veriErisim, Kullanici kullanan, SicilNoHareket sicil, bool tekSicil)
        {
            ObjectArray bilgi = new ObjectArray();

            if (!tekSicil)
            {
                if (string.IsNullOrEmpty(sicil.muhasebeKod))
                    bilgi.sonuc.hataStr = TasinirMalResI.TMM013 + "<br>";
                if (string.IsNullOrEmpty(sicil.harcamaBirimKod))
                    bilgi.sonuc.hataStr += TasinirMalResI.TMM014 + "<br>";

                if (!string.IsNullOrEmpty(bilgi.sonuc.hataStr))
                {
                    bilgi.sonuc.islemSonuc = false;
                    return bilgi;
                }

                bilgi.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, sicil.muhasebeKod, sicil.harcamaBirimKod, sicil.ambarKod);
                if (!bilgi.sonuc.islemSonuc)
                {
                    bilgi.sonuc.hataStr = TasinirMalResI.TMM060;
                    return bilgi;
                }
            }
            else
            {
                if (sicil.prSicilNo <= 0 && sicil.rfIdNo <= 0)
                    bilgi.sonuc.hataStr = TasinirMalResI.TMM145;

                if (!string.IsNullOrEmpty(bilgi.sonuc.hataStr))
                {
                    bilgi.sonuc.islemSonuc = false;
                    return bilgi;
                }

                if (kullanan == null || !TMMYetkiKontrol.TasinirMalYetkisiVarMi(kullanan))
                {
                    bilgi.sonuc.islemSonuc = false;
                    bilgi.sonuc.hataStr = TasinirMalResI.TMM151;
                    return bilgi;
                }
            }

            if (sicil.prSicilNo <= 0 && sicil.rfIdNo > 0)
            {
                sicil.prSicilNo = Genel.RFIDPrSicilNoDondur(sicil.rfIdNo);

                if (sicil.prSicilNo <= 0)
                {
                    bilgi.sonuc.islemSonuc = false;
                    bilgi.sonuc.hataStr = TasinirMalResI.TMM147;
                    return bilgi;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKapiGecisSicil tabKGS = new TABKapiGecisSicil(ve);

            System.Collections.Hashtable isimler = new System.Collections.Hashtable();

            IDataReader dr = tabKGS.SicilListele(sicil);

            while (dr.Read())
            {
                SicilNoHareket gelen = new SicilNoHareket();
                gelen.prSicilNo = VeriErisim.DegerAl(dr, 0, 0);
                gelen.siraNo = VeriErisim.DegerAl(dr, 1, 0);
                gelen.yil = VeriErisim.DegerAl(dr, 2, 0);
                gelen.muhasebeKod = VeriErisim.DegerAl(dr, 3, string.Empty);
                gelen.harcamaBirimKod = VeriErisim.DegerAl(dr, 4, string.Empty);
                gelen.harcamaBirimKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), gelen.harcamaBirimKod);
                gelen.fisNo = VeriErisim.DegerAl(dr, 5, string.Empty);
                gelen.fisSiraNo = VeriErisim.DegerAl(dr, 6, 0);
                gelen.sicilNo = VeriErisim.DegerAl(dr, 7, string.Empty);
                gelen.sicilSiraNo = VeriErisim.DegerAl(dr, 8, string.Empty);
                gelen.hesapPlanKod = VeriErisim.DegerAl(dr, 9, string.Empty);
                gelen.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), gelen.hesapPlanKod);
                gelen.ambarKod = VeriErisim.DegerAl(dr, 10, string.Empty);
                gelen.fiyat = VeriErisim.DegerAl(dr, 11, (decimal)0);
                gelen.kdvOran = VeriErisim.DegerAl(dr, 12, 0);
                gelen.islemTipKod = VeriErisim.DegerAl(dr, 13, 0);
                gelen.islemTurKod = VeriErisim.DegerAl(dr, 14, 0);
                gelen.islemTarih = VeriErisim.DegerAl(dr, 15);
                gelen.islemYapan = VeriErisim.DegerAl(dr, 16, string.Empty);
                gelen.durum = VeriErisim.DegerAl(dr, 17, 0);
                gelen.fisTarih = VeriErisim.DegerAl(dr, 18);
                gelen.neredenGeldi = VeriErisim.DegerAl(dr, 19, string.Empty);
                gelen.nereyeGitti = VeriErisim.DegerAl(dr, 20, string.Empty);
                gelen.odaKod = gelen.nereyeGitti;
                gelen.odaAd = Genel.AdGetir(isimler, gelen.muhasebeKod + "-" + gelen.harcamaBirimKod + "-" + gelen.odaKod, "TASODA");
                //sicil.odaAd = string.Empty;
                gelen.kimeGitti = VeriErisim.DegerAl(dr, 21, string.Empty);
                gelen.personelKod = gelen.kimeGitti;
                //sicil.kisiAd = string.Empty;
                gelen.zimmetOzellik = VeriErisim.DegerAl(dr, 22, string.Empty);
                gelen.kdvliBirimFiyat = VeriErisim.DegerAl(dr, 23, (decimal)0);
                gelen.degerArtisi = VeriErisim.DegerAl(dr, 24, (decimal)0);
                gelen.gonMuhasebeKod = VeriErisim.DegerAl(dr, 25, string.Empty);
                gelen.gonHarcamaBirimKod = VeriErisim.DegerAl(dr, 26, string.Empty);
                gelen.gonHarcamaBirimKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), gelen.gonHarcamaBirimKod);
                gelen.gonAmbarKod = VeriErisim.DegerAl(dr, 27, string.Empty);
                gelen.muhasebeAd = VeriErisim.DegerAl(dr, 28, string.Empty);
                gelen.harcamaBirimAd = VeriErisim.DegerAl(dr, 29, string.Empty);
                gelen.ambarAd = VeriErisim.DegerAl(dr, 30, string.Empty);
                gelen.hesapPlanAd = VeriErisim.DegerAl(dr, 31, string.Empty);
                gelen.islemTipiAd = VeriErisim.DegerAl(dr, 32, string.Empty);
                gelen.rfIdNo = VeriErisim.DegerAl(dr, 33, 0);
                gelen.killNo = VeriErisim.DegerAl(dr, 34, 0);
                gelen.accessNo = VeriErisim.DegerAl(dr, 35, 0);
                gelen.eSicilNo = VeriErisim.DegerAl(dr, 36, string.Empty);
                gelen.tagType = VeriErisim.DegerAl(dr, 37, 0);
                gelen.kapiGecis = VeriErisim.DegerAl(dr, 38, 0) == 0 ? false : true;

                bilgi.Ekle(gelen);
            }

            if (bilgi.objeler.Count == 0)
                bilgi.sonuc.bilgiStr = TasinirMalResI.TMM029;

            if (dr != null) dr.Close();
            ve.VeriErisimKapat();
            return bilgi;
        }
    }
}
