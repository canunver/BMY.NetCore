using System;
using System.Data;
using System.IO;
using TNS.KYM;
using TNS.MUH;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Taşınır mal modülü raporlarıyla ilgili, iş kuralları kontrollerini
    /// yapıp, veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını
    /// çağırarak raporları üreten fonksiyonları tutan sınıf
    /// </summary>
    public class TMMRaporlar
    {
        private static string kurumKod = null;

        /// <summary>
        /// Verilen kriterlere uygun olan malzemelerin ortalama birim fiyat ve
        /// toplam miktar bilgilerini taşınır hesap planı bazında döndüren yordam
        /// hesapKod parametresinde "," ayracı kullanılarak birden fazla taşınır hesap kodu verilebilir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="harcamaBirimKod">Harcama birimi kodu</param>
        /// <param name="hesapKod">Taşınır hesap planı kodu/kodları bilgisi</param>
        /// <param name="sadeceRakamOlanlar">Taşınır hesap planı listesinin tamamı mı yoksa sadece rakam olanlar mı listelensin bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır hesap planları, ortalama birim fiyatları ve toplam miktarları listesi döndürülür.</returns>
        public static ObjectArray HesapPlaniOrtalamaFiyatMiktarListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string harcamaBirimKod, string hesapKod, bool sadeceRakamOlanlar)
        {
            ObjectArray donusDeger = new ObjectArray();

            //if (!TMMYetkiKontrol.GormeYetkisiVarMi(kullanan))
            //{
            //    donusDeger.sonuc.islemSonuc = false;
            //    donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
            //    return donusDeger;
            //}

            string hata = string.Empty;
            if (yil <= 0)
                hata += TasinirMalResI.TMM036 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = hata;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tabRapor = new TABRaporlar(ve);
            IDataReader dr = null;

            try
            {
                dr = tabRapor.HesapPlaniOrtalamaFiyatMiktarListele(yil, harcamaBirimKod, hesapKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                if (dr != null) dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            while (dr.Read())
            {
                HesapPlaniOrtalamaFiyatMiktar gelen = new HesapPlaniOrtalamaFiyatMiktar();
                gelen.hesapPlan.hesapKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                gelen.hesapPlan.hesapKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), gelen.hesapPlan.hesapKod);
                gelen.hesapPlan.aciklama = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.hesapPlan.olcuBirim = VeriErisim.DegerAl(dr, 2, 0);
                gelen.hesapPlan.olcuBirimAd = VeriErisim.DegerAl(dr, 3, string.Empty);
                gelen.ortFiyat = VeriErisim.DegerAl(dr, 4, (decimal)0);
                gelen.miktar = VeriErisim.DegerAl(dr, 5, (double)0);

                donusDeger.objeler.Add(gelen);
            }

            if (!sadeceRakamOlanlar)
            {
                HesapPlaniSatir kriterHesap = new HesapPlaniSatir();
                kriterHesap.hesapKod = hesapKod;
                kriterHesap.detay = true;
                ObjectArray hesaptanGelenler = new TMMHesapPlani().Listele(kullanan, kriterHesap, new Sayfalama());
                if (hesaptanGelenler.sonuc.islemSonuc)
                {
                    bool ekle;
                    foreach (HesapPlaniSatir hps in hesaptanGelenler.objeler)
                    {
                        ekle = true;
                        foreach (HesapPlaniOrtalamaFiyatMiktar hpo in donusDeger.objeler)
                        {
                            if (hps.hesapKod == hpo.hesapPlan.hesapKod)
                            {
                                ekle = false;
                                break;
                            }
                        }
                        if (ekle)
                            donusDeger.objeler.Add(new HesapPlaniOrtalamaFiyatMiktar(hps));
                    }
                }
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            if (dr != null) dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen yılda harcama birimlerinin kesmiş
        /// oldukları son taşınır işlem fişi numaralarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="islemTarihIlk">The islem tarih ilk.</param>
        /// <param name="islemTarihSon">The islem tarih son.</param>
        /// <returns>
        /// Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında harcama birimi bazında taşınır işlem fişi numaraları listesi döndürülür.
        /// </returns>
        public static ObjectArray HarcamaBirimiSonTIFListele(VeriErisim veriErisim, Kullanici kullanan, int yil, TNSDateTime islemTarihIlk, TNSDateTime islemTarihSon)
        {
            ObjectArray donusDeger = new ObjectArray();

            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.TASINIRMALKONSOLIDEYETKILISI))
            {
                donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan);
                if (!donusDeger.sonuc.islemSonuc)
                {
                    donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
                    return donusDeger;
                }
            }

            string hata = string.Empty;
            if (yil <= 0)
                hata += TasinirMalResI.TMM036 + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = hata;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tabRapor = new TABRaporlar(ve);
            IDataReader dr = null;

            try
            {
                dr = tabRapor.HarcamaBirimiSonTIFListele(yil, islemTarihIlk, islemTarihSon, Yetki.YetkiTipiDegerleriDondur(kullanan));
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                if (dr != null) dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            while (dr.Read())
            {
                HarcamaSonTIF gelen = new HarcamaSonTIF();
                gelen.yil = yil;
                gelen.ilKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                gelen.ilAd = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.ilceKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                gelen.ilceAd = VeriErisim.DegerAl(dr, 3, string.Empty);
                gelen.muhasebeKod = VeriErisim.DegerAl(dr, 4, string.Empty);
                gelen.muhasebeAd = VeriErisim.DegerAl(dr, 5, string.Empty);
                gelen.harcamaKod = VeriErisim.DegerAl(dr, 6, string.Empty);
                gelen.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), gelen.harcamaKod);
                gelen.harcamaAd = VeriErisim.DegerAl(dr, 7, string.Empty);
                gelen.tifNo = VeriErisim.DegerAl(dr, 8, string.Empty);

                donusDeger.objeler.Add(gelen);
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            if (dr != null) dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan tüketim malzemeleri defteri bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen DefterKriter nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Tüketim malzemeleri defteri kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında tüketim malzemeleri defteri listesi döndürülür.</returns>
        public static ObjectArray TMDOlustur(VeriErisim veriErisim, Kullanici kullanan, DefterKriter kriter)
        {
            ObjectArray donusDeger = new ObjectArray();

            donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, kriter.muhasebeKod, kriter.harcamaKod, kriter.ambarKod);
            if (!donusDeger.sonuc.islemSonuc)
            {
                donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
                return donusDeger;
            }

            string hata = string.Empty;
            if (kriter.yil <= 0)
                hata += TasinirMalResI.TMM036 + "<br>";
            if (string.IsNullOrEmpty(kriter.muhasebeKod))
                hata += TasinirMalResI.TMM142 + "<br>";
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                if (string.IsNullOrEmpty(kriter.harcamaKod))
                    hata += TasinirMalResI.TMM014 + "<br>";
                if (string.IsNullOrEmpty(kriter.ambarKod))
                    hata += TasinirMalResI.TMM031 + "<br>";
            }
            //if (string.IsNullOrEmpty(kriter.hesapPlanKod))
            //    hata += "Hesap Plan Kodu boş bırakılamaz.<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = hata;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tabRapor = new TABRaporlar(ve);
            IDataReader dr = null;

            try
            {
                dr = tabRapor.TuketimMalzemeleriDefteriOlustur(kriter, Yetki.YetkiTipiDegerleriDondur(kullanan));
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                if (dr != null) dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            TMDBilgi tmd = new TMDBilgi();
            int girisSayac = 0, cikisSayac = 0;
            string eskiHesapKod = string.Empty;

            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                if (string.IsNullOrEmpty(tmd.hesapKod) || eskiHesapKod != VeriErisim.DegerAl(dr, 10, string.Empty))
                {
                    tmd = new TMDBilgi();
                    girisSayac = 0;
                    cikisSayac = 0;
                    tmd.ilKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                    tmd.ilAd = VeriErisim.DegerAl(dr, 1, string.Empty);
                    tmd.ilceKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                    tmd.ilceAd = VeriErisim.DegerAl(dr, 3, string.Empty);
                    tmd.muhasebeKod = VeriErisim.DegerAl(dr, 4, string.Empty);
                    tmd.muhasebeAd = VeriErisim.DegerAl(dr, 5, string.Empty);
                    if (!string.IsNullOrEmpty(kriter.harcamaKod))
                    {
                        tmd.harcamaKod = VeriErisim.DegerAl(dr, 6, string.Empty);
                        tmd.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), tmd.harcamaKod);
                        tmd.harcamaAd = VeriErisim.DegerAl(dr, 7, string.Empty);
                        tmd.ambarKod = VeriErisim.DegerAl(dr, 8, string.Empty);
                        tmd.ambarAd = VeriErisim.DegerAl(dr, 9, string.Empty);
                    }
                    tmd.hesapKod = VeriErisim.DegerAl(dr, 10, string.Empty);
                    eskiHesapKod = tmd.hesapKod;
                    tmd.hesapKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), tmd.hesapKod);
                    tmd.hesapAd = VeriErisim.DegerAl(dr, 11, string.Empty);
                    tmd.olcuBirimKod = VeriErisim.DegerAl(dr, 12, 0);
                    tmd.olcuBirimAd = VeriErisim.DegerAl(dr, 13, string.Empty);

                    donusDeger.objeler.Add(tmd);
                }

                TMDBilgiDetay tmdDetay = new TMDBilgiDetay();
                tmdDetay.tifTarih = VeriErisim.DegerAl(dr, 14);
                tmdDetay.tifNo = VeriErisim.DegerAl(dr, 15, string.Empty);
                tmdDetay.nereyeGitti = VeriErisim.DegerAl(dr, 16, string.Empty);
                tmdDetay.islemTipi.kod = VeriErisim.DegerAl(dr, 17, 0);
                tmdDetay.islemTipi.ad = VeriErisim.DegerAl(dr, 18, string.Empty);
                tmdDetay.islemTipi.tur = VeriErisim.DegerAl(dr, 19, 0);
                if (tmdDetay.islemTipi.tur >= (int)ENUMIslemTipi.SATINALMAGIRIS &&
                    tmdDetay.islemTipi.tur <= (int)ENUMIslemTipi.ACILIS)
                {
                    girisSayac++;
                    tmdDetay.siraNo = girisSayac;
                }
                else if (tmdDetay.islemTipi.tur > (int)ENUMIslemTipi.ACILIS ||
                    tmdDetay.islemTipi.tur == (int)ENUMIslemTipi.DEGERARTTIR)
                {
                    cikisSayac++;
                    tmdDetay.siraNo = cikisSayac;
                }
                tmdDetay.miktar = VeriErisim.DegerAl(dr, 20, (decimal)0);
                tmdDetay.birimFiyatKDVLi = VeriErisim.DegerAl(dr, 21, (decimal)0);
                tmdDetay.tutar = (decimal)tmdDetay.miktar * tmdDetay.birimFiyatKDVLi;

                tmdDetay.neredenGeldi = VeriErisim.DegerAl(dr, 22, string.Empty);

                tmdDetay.gonMuhasebeKod = VeriErisim.DegerAl(dr, 23, string.Empty);
                tmdDetay.gonMuhasebeAd = Genel.AdGetir(isimler, tmdDetay.gonMuhasebeKod, "TASMUHASEBE");

                tmdDetay.gonHarcamaKod = VeriErisim.DegerAl(dr, 24, string.Empty);
                tmdDetay.gonHarcamaAd = Genel.AdGetir(isimler, tmdDetay.gonMuhasebeKod + "-" + tmdDetay.gonHarcamaKod, "TASHARCAMABIRIM");

                tmdDetay.gonAmbarKod = VeriErisim.DegerAl(dr, 25, string.Empty);
                tmdDetay.gonAmbarAd = Genel.AdGetir(isimler, tmdDetay.gonMuhasebeKod + "-" + tmdDetay.gonHarcamaKod + "-" + tmdDetay.gonAmbarKod, "TASAMBAR");

                tmdDetay.kimeGitti = VeriErisim.DegerAl(dr, 26, string.Empty);

                tmd.detaylar.Add(tmdDetay);
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            if (dr != null) dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan dayanıklı taşınır defteri bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen DefterKriter nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Dayanıklı taşınır defteri kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında dayanıklı taşınır defteri listesi döndürülür.</returns>
        public static ObjectArray DTDOlustur(VeriErisim veriErisim, Kullanici kullanan, DefterKriter kriter)
        {
            ObjectArray donusDeger = new ObjectArray();

            donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, kriter.muhasebeKod, kriter.harcamaKod, kriter.ambarKod);
            if (!donusDeger.sonuc.islemSonuc)
            {
                donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
                return donusDeger;
            }

            string hata = string.Empty;
            if (kriter.yil <= 0)
                hata += TasinirMalResI.TMM036 + "<br>";
            if (string.IsNullOrEmpty(kriter.muhasebeKod))
                hata += TasinirMalResI.TMM142 + "<br>";
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                if (string.IsNullOrEmpty(kriter.harcamaKod))
                    hata += TasinirMalResI.TMM014 + "<br>";
                if (string.IsNullOrEmpty(kriter.ambarKod))
                    hata += TasinirMalResI.TMM031 + "<br>";
            }
            //if (string.IsNullOrEmpty(kriter.hesapPlanKod))
            //    hata += "Hesap Plan Kodu boş bırakılamaz.<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = hata;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tabRapor = new TABRaporlar(ve);
            IDataReader dr = null;

            try
            {
                dr = tabRapor.DayanikliTasinirlarDefteriOlustur(kriter, Yetki.YetkiTipiDegerleriDondur(kullanan));
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                if (dr != null) dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            DTDBilgi dtd = new DTDBilgi();
            string eskiHesapKod = string.Empty;

            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                if (string.IsNullOrEmpty(eskiHesapKod) || eskiHesapKod != VeriErisim.DegerAl(dr, 10, string.Empty))
                {
                    dtd = new DTDBilgi();
                    dtd.ilKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                    dtd.ilAd = VeriErisim.DegerAl(dr, 1, string.Empty);
                    dtd.ilceKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                    dtd.ilceAd = VeriErisim.DegerAl(dr, 3, string.Empty);
                    dtd.muhasebeKod = VeriErisim.DegerAl(dr, 4, string.Empty);
                    dtd.muhasebeAd = VeriErisim.DegerAl(dr, 5, string.Empty);
                    if (!string.IsNullOrEmpty(kriter.harcamaKod))
                    {
                        dtd.harcamaKod = VeriErisim.DegerAl(dr, 6, string.Empty);
                        dtd.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), dtd.harcamaKod);
                        dtd.harcamaAd = VeriErisim.DegerAl(dr, 7, string.Empty);
                        dtd.ambarKod = VeriErisim.DegerAl(dr, 8, string.Empty);
                        dtd.ambarAd = VeriErisim.DegerAl(dr, 9, string.Empty);
                    }
                    dtd.hesapKod = VeriErisim.DegerAl(dr, 10, string.Empty);
                    eskiHesapKod = dtd.hesapKod;
                    dtd.hesapKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), dtd.hesapKod);
                    dtd.hesapAd = VeriErisim.DegerAl(dr, 11, string.Empty);
                    dtd.olcuBirimKod = VeriErisim.DegerAl(dr, 12, 0);
                    dtd.olcuBirimAd = VeriErisim.DegerAl(dr, 13, string.Empty);

                    donusDeger.objeler.Add(dtd);
                }

                DTDBilgiDetay dtdDetay = new DTDBilgiDetay();
                dtdDetay.belgeTarih = VeriErisim.DegerAl(dr, 14);
                dtdDetay.belgeNo = VeriErisim.DegerAl(dr, 15, string.Empty);
                dtdDetay.neredenGeldi = VeriErisim.DegerAl(dr, 16, string.Empty);
                dtdDetay.nereyeGonderildi = VeriErisim.DegerAl(dr, 17, string.Empty);
                dtdDetay.birimFiyatKDVLi = VeriErisim.DegerAl(dr, 18, (decimal)0);
                dtdDetay.miktar = (decimal)1;
                dtdDetay.toplamDeger = dtdDetay.birimFiyatKDVLi * (decimal)dtdDetay.miktar;

                dtdDetay.sicilNo = VeriErisim.DegerAl(dr, 19, string.Empty);
                dtdDetay.islemTipi.kod = VeriErisim.DegerAl(dr, 20, 0);
                dtdDetay.islemTipi.ad = VeriErisim.DegerAl(dr, 21, string.Empty);
                dtdDetay.islemTipi.tur = VeriErisim.DegerAl(dr, 22, 0);
                if (!(dtdDetay.islemTipi.tur == (int)ENUMIslemTipi.ZFDUSME ||
                    dtdDetay.islemTipi.tur == (int)ENUMIslemTipi.ZFVERME ||
                    dtdDetay.islemTipi.tur == (int)ENUMIslemTipi.DTLDUSME ||
                    dtdDetay.islemTipi.tur == (int)ENUMIslemTipi.DTLVERME))
                    dtdDetay.cikisCinsi = TasinirMalResI.TMM063;
                else if (dtdDetay.islemTipi.tur == (int)ENUMIslemTipi.ZFDUSME ||
                    dtdDetay.islemTipi.tur == (int)ENUMIslemTipi.ZFVERME)
                {
                    dtdDetay.cikisCinsi = TasinirMalResI.TMM064;
                    if (dtdDetay.islemTipi.tur == (int)ENUMIslemTipi.ZFVERME)
                        dtdDetay.islemTipi.ad = TasinirMalResI.TMM065;
                    else if (dtdDetay.islemTipi.tur == (int)ENUMIslemTipi.ZFDUSME)
                        dtdDetay.islemTipi.ad = TasinirMalResI.TMM066;
                }
                else if (dtdDetay.islemTipi.tur == (int)ENUMIslemTipi.DTLDUSME ||
                    dtdDetay.islemTipi.tur == (int)ENUMIslemTipi.DTLVERME)
                {
                    dtdDetay.cikisCinsi = TasinirMalResI.TMM067;
                    if (dtdDetay.islemTipi.tur == (int)ENUMIslemTipi.DTLVERME)
                        dtdDetay.islemTipi.ad = TasinirMalResI.TMM065;
                    else if (dtdDetay.islemTipi.tur == (int)ENUMIslemTipi.DTLDUSME)
                        dtdDetay.islemTipi.ad = TasinirMalResI.TMM066;
                }

                dtdDetay.kimeVerildi = VeriErisim.DegerAl(dr, 23, string.Empty);
                dtdDetay.nereyeVerildi = VeriErisim.DegerAl(dr, 17, string.Empty);

                if (dtdDetay.cikisCinsi == TasinirMalResI.TMM063)
                {
                    dtdDetay.ozellikler += VeriErisim.DegerAl(dr, 29, string.Empty);//Plaka
                    if (dtdDetay.ozellikler != "") dtdDetay.ozellikler += "-";
                    dtdDetay.ozellikler += VeriErisim.DegerAl(dr, 30, string.Empty);//Şase no
                    if (dtdDetay.ozellikler != "") dtdDetay.ozellikler += "-";
                    dtdDetay.ozellikler += VeriErisim.DegerAl(dr, 31, string.Empty);//Motor No
                }
                else
                    dtdDetay.ozellikler = VeriErisim.DegerAl(dr, 24, string.Empty);

                dtdDetay.gonMuhasebeKod = VeriErisim.DegerAl(dr, 25, string.Empty);
                dtdDetay.gonMuhasebeAd = Genel.AdGetir(isimler, dtdDetay.gonMuhasebeKod, "TASMUHASEBE");

                dtdDetay.gonHarcamaKod = VeriErisim.DegerAl(dr, 26, string.Empty);
                dtdDetay.gonHarcamaAd = Genel.AdGetir(isimler, dtdDetay.gonMuhasebeKod + "-" + dtdDetay.gonHarcamaKod, "TASHARCAMABIRIM");

                dtdDetay.gonAmbarKod = VeriErisim.DegerAl(dr, 27, string.Empty);
                dtdDetay.gonAmbarAd = Genel.AdGetir(isimler, dtdDetay.gonMuhasebeKod + "-" + dtdDetay.gonHarcamaKod + "-" + dtdDetay.gonAmbarKod, "TASAMBAR");

                dtdDetay.degerArtisi = VeriErisim.DegerAl(dr, 28, (decimal)0);
                dtdDetay.toplamDeger += dtdDetay.degerArtisi;
                dtdDetay.markaAd = VeriErisim.DegerAl(dr, 33, string.Empty);
                dtdDetay.modelAd = VeriErisim.DegerAl(dr, 34, string.Empty);
                dtdDetay.seriNo = VeriErisim.DegerAl(dr, 35, string.Empty);

                dtd.detaylar.Add(dtdDetay);
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            if (dr != null) dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan müze defteri bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen DefterKriter nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Müze defteri kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında müze defteri listesi döndürülür.</returns>
        public static ObjectArray MuzeDefteriOlustur(VeriErisim veriErisim, Kullanici kullanan, DefterKriter kriter)
        {
            ObjectArray donusDeger = new ObjectArray();

            donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, kriter.muhasebeKod, kriter.harcamaKod, kriter.ambarKod);
            if (!donusDeger.sonuc.islemSonuc)
            {
                donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
                return donusDeger;
            }

            string hata = string.Empty;
            if (kriter.yil <= 0)
                hata += TasinirMalResI.TMM036 + "<br>";
            if (string.IsNullOrEmpty(kriter.muhasebeKod))
                hata += TasinirMalResI.TMM142 + "<br>";
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                if (string.IsNullOrEmpty(kriter.harcamaKod))
                    hata += TasinirMalResI.TMM014 + "<br>";
                if (string.IsNullOrEmpty(kriter.ambarKod))
                    hata += TasinirMalResI.TMM031 + "<br>";
            }
            //if (string.IsNullOrEmpty(kriter.hesapPlanKod))
            //    hata += "Hesap Plan Kodu boş bırakılamaz.<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = hata;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tabRapor = new TABRaporlar(ve);
            IDataReader dr = null;

            try
            {
                dr = tabRapor.MuzeDefteriOlustur(kriter, Yetki.YetkiTipiDegerleriDondur(kullanan));
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                if (dr != null) dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            MuzeBilgi muze = new MuzeBilgi();
            string eskiHesapKod = string.Empty;

            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                if (string.IsNullOrEmpty(eskiHesapKod) || eskiHesapKod != VeriErisim.DegerAl(dr, 11, string.Empty))
                {
                    muze = new MuzeBilgi();
                    muze.ilKod = VeriErisim.DegerAl(dr, 19, string.Empty);
                    muze.ilAd = VeriErisim.DegerAl(dr, 20, string.Empty);
                    muze.ilceKod = VeriErisim.DegerAl(dr, 21, string.Empty);
                    muze.ilceAd = VeriErisim.DegerAl(dr, 22, string.Empty);
                    muze.muhasebeKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                    muze.muhasebeAd = VeriErisim.DegerAl(dr, 2, string.Empty);
                    if (!string.IsNullOrEmpty(kriter.harcamaKod))
                    {
                        muze.harcamaKod = VeriErisim.DegerAl(dr, 3, string.Empty);
                        muze.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), muze.harcamaKod);
                        muze.harcamaAd = VeriErisim.DegerAl(dr, 4, string.Empty);
                        muze.ambarKod = VeriErisim.DegerAl(dr, 5, string.Empty);
                        muze.ambarAd = VeriErisim.DegerAl(dr, 6, string.Empty);
                    }
                    muze.hesapKod = VeriErisim.DegerAl(dr, 11, string.Empty);
                    eskiHesapKod = muze.hesapKod;
                    muze.hesapKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), muze.hesapKod);
                    muze.hesapAd = VeriErisim.DegerAl(dr, 12, string.Empty);
                    muze.olcuBirimKod = VeriErisim.DegerAl(dr, 17, 0);
                    muze.olcuBirimAd = VeriErisim.DegerAl(dr, 18, string.Empty);

                    donusDeger.objeler.Add(muze);
                }

                MuzeBilgiDetay muzeDetay = new MuzeBilgiDetay();
                muzeDetay.belgeTarih = VeriErisim.DegerAl(dr, 7);
                muzeDetay.belgeNo = VeriErisim.DegerAl(dr, 8, string.Empty);
                muzeDetay.islemTipi.kod = VeriErisim.DegerAl(dr, 23, 0);
                muzeDetay.islemTipi.ad = VeriErisim.DegerAl(dr, 24, string.Empty);
                muzeDetay.islemTipi.tur = VeriErisim.DegerAl(dr, 25, 0);
                if (!(muzeDetay.islemTipi.tur == (int)ENUMIslemTipi.ZFDUSME ||
                      muzeDetay.islemTipi.tur == (int)ENUMIslemTipi.ZFVERME ||
                      muzeDetay.islemTipi.tur == (int)ENUMIslemTipi.DTLDUSME ||
                      muzeDetay.islemTipi.tur == (int)ENUMIslemTipi.DTLVERME))
                    muzeDetay.cikisCinsi = "TİF";
                else if (muzeDetay.islemTipi.tur == (int)ENUMIslemTipi.ZFDUSME ||
                         muzeDetay.islemTipi.tur == (int)ENUMIslemTipi.ZFVERME)
                {
                    muzeDetay.cikisCinsi = "ZİMMET (Kişi)";
                    if (muzeDetay.islemTipi.tur == (int)ENUMIslemTipi.ZFVERME)
                        muzeDetay.islemTipi.ad = "ZİMMET VERME";
                    else if (muzeDetay.islemTipi.tur == (int)ENUMIslemTipi.ZFDUSME)
                        muzeDetay.islemTipi.ad = "ZİMMET DÜŞME";
                }
                else if (muzeDetay.islemTipi.tur == (int)ENUMIslemTipi.DTLDUSME ||
                    muzeDetay.islemTipi.tur == (int)ENUMIslemTipi.DTLVERME)
                {
                    muzeDetay.cikisCinsi = "ZİMMET (Ortak Alan)";
                    if (muzeDetay.islemTipi.tur == (int)ENUMIslemTipi.DTLVERME)
                        muzeDetay.islemTipi.ad = "ZİMMET VERME";
                    else if (muzeDetay.islemTipi.tur == (int)ENUMIslemTipi.DTLDUSME)
                        muzeDetay.islemTipi.ad = "ZİMMET DÜŞME";
                }

                muzeDetay.sicilNo = VeriErisim.DegerAl(dr, 16, string.Empty);
                muzeDetay.adi = VeriErisim.DegerAl(dr, 39, string.Empty);
                muzeDetay.gelisTarihi = VeriErisim.DegerAl(dr, 50, string.Empty);
                muzeDetay.neredenGeldi = VeriErisim.DegerAl(dr, 42, string.Empty);
                muzeDetay.neredeBulundu = VeriErisim.DegerAl(dr, 51, string.Empty);
                muzeDetay.cagi = VeriErisim.DegerAl(dr, 52, string.Empty);
                muzeDetay.boyutlari = VeriErisim.DegerAl(dr, 43, string.Empty);
                muzeDetay.durumuMaddesi = VeriErisim.DegerAl(dr, 53, string.Empty);
                muzeDetay.onYuz = VeriErisim.DegerAl(dr, 54, string.Empty);
                muzeDetay.arkaYuz = VeriErisim.DegerAl(dr, 55, string.Empty);
                muzeDetay.fotograf = VeriErisim.DegerAl(dr, 56, string.Empty);
                muzeDetay.puan = VeriErisim.DegerAl(dr, 57, string.Empty);
                muzeDetay.birimFiyatKDVLi = VeriErisim.DegerAl(dr, 13, (decimal)0);
                muzeDetay.yeriKonusu = VeriErisim.DegerAl(dr, 49, string.Empty);
                muzeDetay.nereyeVerildi = VeriErisim.DegerAl(dr, 10, string.Empty);
                muzeDetay.kimeVerildi = VeriErisim.DegerAl(dr, 26, string.Empty);
                muzeDetay.nereyeGonderildi = VeriErisim.DegerAl(dr, 10, string.Empty);

                muzeDetay.gonMuhasebeKod = VeriErisim.DegerAl(dr, 28, string.Empty);
                muzeDetay.gonMuhasebeAd = Genel.AdGetir(isimler, muzeDetay.gonMuhasebeKod, "TASMUHASEBE");

                muzeDetay.gonHarcamaKod = VeriErisim.DegerAl(dr, 29, string.Empty);
                muzeDetay.gonHarcamaAd = Genel.AdGetir(isimler, muzeDetay.gonMuhasebeKod + "-" + muzeDetay.gonHarcamaKod, "TASHARCAMABIRIM");

                muzeDetay.gonAmbarKod = VeriErisim.DegerAl(dr, 30, string.Empty);
                muzeDetay.gonAmbarAd = Genel.AdGetir(isimler, muzeDetay.gonMuhasebeKod + "-" + muzeDetay.gonHarcamaKod + "-" + muzeDetay.gonAmbarKod, "TASAMBAR");

                muze.detaylar.Add(muzeDetay);
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            if (dr != null) dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan kütüphane defteri bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen DefterKriter nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Kütüphane defteri kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında kütüphane defteri listesi döndürülür.</returns>
        public static ObjectArray KutuphaneDefteriOlustur(VeriErisim veriErisim, Kullanici kullanan, DefterKriter kriter)
        {
            ObjectArray donusDeger = new ObjectArray();

            donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, kriter.muhasebeKod, kriter.harcamaKod, kriter.ambarKod);
            if (!donusDeger.sonuc.islemSonuc)
            {
                donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
                return donusDeger;
            }

            string hata = string.Empty;
            if (kriter.yil <= 0)
                hata += TasinirMalResI.TMM036 + "<br>";
            if (string.IsNullOrEmpty(kriter.muhasebeKod))
                hata += TasinirMalResI.TMM142 + "<br>";
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                if (string.IsNullOrEmpty(kriter.harcamaKod))
                    hata += TasinirMalResI.TMM014 + "<br>";
                if (string.IsNullOrEmpty(kriter.ambarKod))
                    hata += TasinirMalResI.TMM031 + "<br>";
            }
            if (kriter.ilkKayit < 0)
                hata += TasinirMalResI.TMM189 + "<br>";
            if (kriter.kayitSayisi < 0)
                hata += TasinirMalResI.TMM190 + "<br>";
            //if (string.IsNullOrEmpty(kriter.hesapPlanKod))
            //    hata += "Hesap Plan Kodu boş bırakılamaz.<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = hata;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tabRapor = new TABRaporlar(ve);
            IDataReader dr = null;

            try
            {
                dr = tabRapor.KutuphaneDefteriOlustur(kriter, Yetki.YetkiTipiDegerleriDondur(kullanan));
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                if (dr != null) dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            KutuphaneBilgi ktp = new KutuphaneBilgi();
            string eskiHesapKod = string.Empty;

            int sayac = 0;
            int alinanKayit = 0;
            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                if (kriter.kayitSayisi > 0 && alinanKayit == kriter.kayitSayisi)
                    break;

                sayac++;
                if (sayac < kriter.ilkKayit)
                    continue;
                else
                    alinanKayit++;

                if (string.IsNullOrEmpty(eskiHesapKod) || eskiHesapKod != VeriErisim.DegerAl(dr, 11, string.Empty))
                {
                    ktp = new KutuphaneBilgi();
                    ktp.ilKod = VeriErisim.DegerAl(dr, 19, string.Empty);
                    ktp.ilAd = VeriErisim.DegerAl(dr, 20, string.Empty);
                    ktp.ilceKod = VeriErisim.DegerAl(dr, 21, string.Empty);
                    ktp.ilceAd = VeriErisim.DegerAl(dr, 22, string.Empty);
                    ktp.muhasebeKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                    ktp.muhasebeAd = VeriErisim.DegerAl(dr, 2, string.Empty);
                    if (!string.IsNullOrEmpty(kriter.harcamaKod))
                    {
                        ktp.harcamaKod = VeriErisim.DegerAl(dr, 3, string.Empty);
                        ktp.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), ktp.harcamaKod);
                        ktp.harcamaAd = VeriErisim.DegerAl(dr, 4, string.Empty);
                        ktp.ambarKod = VeriErisim.DegerAl(dr, 5, string.Empty);
                        ktp.ambarAd = VeriErisim.DegerAl(dr, 6, string.Empty);
                    }
                    ktp.hesapKod = VeriErisim.DegerAl(dr, 11, string.Empty);
                    eskiHesapKod = ktp.hesapKod;
                    ktp.hesapKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), ktp.hesapKod);
                    ktp.hesapAd = VeriErisim.DegerAl(dr, 12, string.Empty);
                    ktp.olcuBirimKod = VeriErisim.DegerAl(dr, 17, 0);
                    ktp.olcuBirimAd = VeriErisim.DegerAl(dr, 18, string.Empty);

                    donusDeger.objeler.Add(ktp);
                }

                KutuphaneBilgiDetay ktpDetay = new KutuphaneBilgiDetay();
                ktpDetay.belgeTarih = VeriErisim.DegerAl(dr, 7);
                ktpDetay.belgeNo = VeriErisim.DegerAl(dr, 8, string.Empty);
                ktpDetay.islemTipi.kod = VeriErisim.DegerAl(dr, 23, 0);
                ktpDetay.islemTipi.ad = VeriErisim.DegerAl(dr, 24, string.Empty);
                ktpDetay.islemTipi.tur = VeriErisim.DegerAl(dr, 25, 0);
                if (!(ktpDetay.islemTipi.tur == (int)ENUMIslemTipi.ZFDUSME ||
                      ktpDetay.islemTipi.tur == (int)ENUMIslemTipi.ZFVERME ||
                      ktpDetay.islemTipi.tur == (int)ENUMIslemTipi.DTLDUSME ||
                      ktpDetay.islemTipi.tur == (int)ENUMIslemTipi.DTLVERME))
                    ktpDetay.cikisCinsi = "TİF";
                else if (ktpDetay.islemTipi.tur == (int)ENUMIslemTipi.ZFDUSME ||
                         ktpDetay.islemTipi.tur == (int)ENUMIslemTipi.ZFVERME)
                {
                    ktpDetay.cikisCinsi = "ZİMMET (Kişi)";
                    if (ktpDetay.islemTipi.tur == (int)ENUMIslemTipi.ZFVERME)
                        ktpDetay.islemTipi.ad = "ZİMMET VERME";
                    else if (ktpDetay.islemTipi.tur == (int)ENUMIslemTipi.ZFDUSME)
                        ktpDetay.islemTipi.ad = "ZİMMET DÜŞME";
                }
                else if (ktpDetay.islemTipi.tur == (int)ENUMIslemTipi.DTLDUSME ||
                    ktpDetay.islemTipi.tur == (int)ENUMIslemTipi.DTLVERME)
                {
                    ktpDetay.cikisCinsi = "ZİMMET (Ortak Alan)";
                    if (ktpDetay.islemTipi.tur == (int)ENUMIslemTipi.DTLVERME)
                        ktpDetay.islemTipi.ad = "ZİMMET VERME";
                    else if (ktpDetay.islemTipi.tur == (int)ENUMIslemTipi.DTLDUSME)
                        ktpDetay.islemTipi.ad = "ZİMMET DÜŞME";
                }

                ktpDetay.sicilNo = VeriErisim.DegerAl(dr, 16, string.Empty);
                ktpDetay.ciltNo = VeriErisim.DegerAl(dr, 36, string.Empty);
                ktpDetay.dil = VeriErisim.DegerAl(dr, 37, string.Empty);
                ktpDetay.yazarAdi = VeriErisim.DegerAl(dr, 38, string.Empty);
                ktpDetay.adi = VeriErisim.DegerAl(dr, 39, string.Empty);
                ktpDetay.yayinYeri = VeriErisim.DegerAl(dr, 40, string.Empty);
                ktpDetay.yayinTarihi = VeriErisim.DegerAl(dr, 41, string.Empty);
                ktpDetay.neredenGeldi = VeriErisim.DegerAl(dr, 42, string.Empty);
                ktpDetay.boyutlari = VeriErisim.DegerAl(dr, 43, string.Empty);
                ktpDetay.satirSayisi = VeriErisim.DegerAl(dr, 44, string.Empty);
                ktpDetay.yaprakSayisi = VeriErisim.DegerAl(dr, 45, string.Empty);
                ktpDetay.sayfaSayisi = VeriErisim.DegerAl(dr, 46, string.Empty);
                ktpDetay.ciltTuru = VeriErisim.DegerAl(dr, 47, string.Empty);
                ktpDetay.fotograf = VeriErisim.DegerAl(dr, 56, string.Empty);
                ktpDetay.birimFiyatKDVLi = VeriErisim.DegerAl(dr, 13, (decimal)0);
                ktpDetay.yeriKonusu = VeriErisim.DegerAl(dr, 49, string.Empty);
                ktpDetay.nereyeVerildi = VeriErisim.DegerAl(dr, 10, string.Empty);
                ktpDetay.kimeVerildi = VeriErisim.DegerAl(dr, 26, string.Empty);
                ktpDetay.nereyeGonderildi = VeriErisim.DegerAl(dr, 10, string.Empty);

                ktpDetay.gonMuhasebeKod = VeriErisim.DegerAl(dr, 28, string.Empty);
                ktpDetay.gonMuhasebeAd = Genel.AdGetir(isimler, ktpDetay.gonMuhasebeKod, "TASMUHASEBE");

                ktpDetay.gonHarcamaKod = VeriErisim.DegerAl(dr, 29, string.Empty);
                ktpDetay.gonHarcamaAd = Genel.AdGetir(isimler, ktpDetay.gonMuhasebeKod + "-" + ktpDetay.gonHarcamaKod, "TASHARCAMABIRIM");

                ktpDetay.gonAmbarKod = VeriErisim.DegerAl(dr, 30, string.Empty);
                ktpDetay.gonAmbarAd = Genel.AdGetir(isimler, ktpDetay.gonMuhasebeKod + "-" + ktpDetay.gonHarcamaKod + "-" + ktpDetay.gonAmbarKod, "TASAMBAR");

                ktp.detaylar.Add(ktpDetay);
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            if (dr != null) dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan harcama birimi taşınır yönetim hesabı cetveli bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Cetvel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Harcama birimi taşınır yönetim hesabı cetveli kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında harcama birimi taşınır yönetim hesabı cetveli listesi döndürülür.</returns>
        public static ObjectArray TasinirYonetimHesabiCetveli(VeriErisim veriErisim, Kullanici kullanan, Cetvel kriter)
        {
            return TasinirSayimCetveli(veriErisim, kullanan, kriter);
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır sayım ve döküm cetveli bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Cetvel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Taşınır sayım ve döküm cetveli kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır sayım ve döküm cetveli listesi döndürülür.</returns>
        public static ObjectArray TasinirSayimCetveli(VeriErisim veriErisim, Kullanici kullanan, Cetvel kriter)
        {
            ObjectArray donusDeger = new ObjectArray();

            if (string.IsNullOrEmpty(kurumKod))
            {
                kurumKod = TNS.TMM.Arac.DegiskenDegerBul(0, "/KURUMKOD");
            }

            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.TASINIRMALKONSOLIDEYETKILISI))
            {
                donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMiHarcamaBirimi(kullanan, kriter.muhasebeKod, kriter.harcamaKod);
                if (!donusDeger.sonuc.islemSonuc)
                {
                    donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
                    return donusDeger;
                }
            }

            string hata = string.Empty;
            if (kriter.yil <= 0)
                hata += TasinirMalResI.TMM036 + "<br>";
            if (string.IsNullOrEmpty(kriter.muhasebeKod))
                hata += TasinirMalResI.TMM142 + "<br>";
            if (!kriter.harcamaZorunluDegil)
            {
                if (string.IsNullOrEmpty(kriter.harcamaKod))
                    hata += TasinirMalResI.TMM014 + "<br>";
            }

            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = hata;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tabRapor = new TABRaporlar(ve);
            IDataReader dr = null;

            try
            {
                dr = tabRapor.TasinirSayimCetveli(kriter, ENUMCetvelRakamTipi.GECENYILDANDEVREDENGIRIS, Yetki.YetkiTipiDegerleriDondur(kullanan), kurumKod);
                if (dr != null)
                    TasinirSayimCetveliBilgileriAl(dr, donusDeger.objeler, ENUMCetvelRakamTipi.GECENYILDANDEVREDENGIRIS, kriter.harcamaZorunluDegil);

                dr = tabRapor.TasinirSayimCetveli(kriter, ENUMCetvelRakamTipi.GECENYILDANDEVREDENCIKIS, Yetki.YetkiTipiDegerleriDondur(kullanan), kurumKod);
                if (dr != null)
                    TasinirSayimCetveliBilgileriAl(dr, donusDeger.objeler, ENUMCetvelRakamTipi.GECENYILDANDEVREDENCIKIS, kriter.harcamaZorunluDegil);

                dr = tabRapor.TasinirSayimCetveli(kriter, ENUMCetvelRakamTipi.YILICINDEGIREN, Yetki.YetkiTipiDegerleriDondur(kullanan), kurumKod);
                if (dr != null)
                    TasinirSayimCetveliBilgileriAl(dr, donusDeger.objeler, ENUMCetvelRakamTipi.YILICINDEGIREN, kriter.harcamaZorunluDegil);

                dr = tabRapor.TasinirSayimCetveli(kriter, ENUMCetvelRakamTipi.YILICINDECIKAN, Yetki.YetkiTipiDegerleriDondur(kullanan), kurumKod);
                if (dr != null)
                    TasinirSayimCetveliBilgileriAl(dr, donusDeger.objeler, ENUMCetvelRakamTipi.YILICINDECIKAN, kriter.harcamaZorunluDegil);

                if (donusDeger.objeler.Count > 0)
                    CetvelDegerArtislari(ve, kullanan, kriter, donusDeger.objeler, ENUMCetvelTipi.SAYIMCETVELI);

                if (!kriter.harcamaZorunluDegil)
                {
                    foreach (Cetvel c in donusDeger.objeler)
                        DetaySirala(ref c.detay);
                }
                else
                {
                    foreach (Cetvel c in donusDeger.objeler)
                        DetaySiralaHarcama(ref c.detay);
                    foreach (Cetvel c in donusDeger.objeler)
                        DetaySiralaHarcamaHesapPlani(ref c.detay);
                }
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            dr.Close();
            ve.VeriErisimKapat();

            if (!string.IsNullOrWhiteSpace(kriter.ambarKod))
            {
                string ambarAd = Genel.AdGetir(new System.Collections.Hashtable(), kriter.muhasebeKod + "-" + kriter.harcamaKod + "-" + kriter.ambarKod, "TASAMBAR");
                foreach (Cetvel cetvel in donusDeger.objeler)
                {
                    cetvel.ambarKod = kriter.ambarKod;
                    cetvel.ambarAd = ambarAd;
                }
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen cetvel detay veya sayım detay bilgileri
        /// listesini taşınır hesap planı koduna göre sıralayan yordam
        /// </summary>
        /// <param name="detay">Sıralanacak nesne listesi</param>
        public static void DetaySirala(ref TNSCollection detay)
        {
            //Hesap kodlarını arraya atıp sıralıyorum
            string[] a = new string[detay.Count];
            for (int i = 0; i < detay.Count; i++)
            {
                if (detay[i].GetType() == typeof(CetvelDetay))
                {
                    CetvelDetay cd = (CetvelDetay)detay[i];
                    a[i] = cd.hesapPlanKod;
                }
                else if (detay[i].GetType() == typeof(SayimDetay))
                {
                    SayimDetay sd = (SayimDetay)detay[i];
                    a[i] = sd.hesapPlanKod;
                }
            }
            Array.Sort(a);

            //Sıraladığım arraya göre yeni detay collectionı oluşturuyorum
            TNSCollection yeniDetay = new TNSCollection();
            for (int i = 0; i < a.Length; i++)
            {
                for (int j = 0; j < detay.Count; j++)
                {
                    if (detay[j].GetType() == typeof(CetvelDetay))
                    {
                        CetvelDetay cd = (CetvelDetay)detay[j];
                        if (a[i] == cd.hesapPlanKod)
                        {
                            yeniDetay.Add(cd);
                            detay.RemoveAt(j);
                            j--;
                            break;
                        }
                    }
                    else if (detay[j].GetType() == typeof(SayimDetay))
                    {
                        SayimDetay sd = (SayimDetay)detay[j];
                        if (a[i] == sd.hesapPlanKod)
                        {
                            yeniDetay.Add(sd);
                            detay.RemoveAt(j);
                            j--;
                            break;
                        }
                    }
                }
            }
            detay = yeniDetay;
        }

        /// <summary>
        /// Parametre olarak verilen cetvel detay veya sayım detay bilgileri
        /// listesini taşınır hesap planı koduna göre sıralayan yordam
        /// </summary>
        /// <param name="detay">Sıralanacak nesne listesi</param>
        public static void DetaySiralaHarcama(ref TNSCollection detay)
        {
            //Hesap kodlarını arraya atıp sıralıyorum
            string[] a = new string[detay.Count];
            for (int i = 0; i < detay.Count; i++)
            {
                if (detay[i].GetType() == typeof(CetvelDetay))
                {
                    CetvelDetay cd = (CetvelDetay)detay[i];
                    a[i] = cd.harcamaKod;
                }
            }
            Array.Sort(a);

            //Sıraladığım arraya göre yeni detay collectionı oluşturuyorum
            TNSCollection yeniDetay = new TNSCollection();
            for (int i = 0; i < a.Length; i++)
            {
                for (int j = 0; j < detay.Count; j++)
                {
                    if (detay[j].GetType() == typeof(CetvelDetay))
                    {
                        CetvelDetay cd = (CetvelDetay)detay[j];
                        if (a[i] == cd.harcamaKod)
                        {
                            yeniDetay.Add(cd);
                            detay.RemoveAt(j);
                            j--;
                            break;
                        }
                    }
                }
            }
            detay = yeniDetay;
        }

        public static void DetaySiralaHarcamaHesapPlani(ref TNSCollection detay)
        {
            TNSCollection col = new TNSCollection();
            TNSCollection yeniDetay = new TNSCollection();

            string harcamaKod = string.Empty;
            int colIndex = 0;

            for (int i = 0; i < detay.Count; i++)
            {
                if (detay[i].GetType() == typeof(CetvelDetay))
                {
                    CetvelDetay cd = (CetvelDetay)detay[i];

                    if (string.IsNullOrEmpty(harcamaKod))
                        harcamaKod = cd.harcamaKod;

                    if (harcamaKod != cd.harcamaKod)
                    {
                        DetaySirala(ref col);
                        for (int ii = 0; ii < col.Count; ii++)
                        {
                            if (col[ii].GetType() == typeof(CetvelDetay))
                            {
                                CetvelDetay cdi = (CetvelDetay)col[ii];
                                yeniDetay.Add(cdi);
                            }
                        }
                        colIndex = col.Count;
                        for (int a = 0; a < colIndex; a++)
                            col.RemoveAt(0);

                        harcamaKod = cd.harcamaKod;
                    }

                    col.Add(cd);
                }
            }

            DetaySirala(ref col);
            for (int ii = 0; ii < col.Count; ii++)
            {
                if (col[ii].GetType() == typeof(CetvelDetay))
                {
                    CetvelDetay cdi = (CetvelDetay)col[ii];
                    yeniDetay.Add(cdi);
                }
            }

            detay = yeniDetay;
        }

        /// <summary>
        /// Parametre olarak verilen taşınır hesap planı kodunu ve ölçü birimini
        /// verilen nesne listesinde arayan ve listedeki indeksini döndüren yordam
        /// </summary>
        /// <param name="hesapKodu">Taşınır hesap planı kodu</param>
        /// <param name="olcuBirimKod">Taşınır hesap planının ölçü birimi</param>
        /// <param name="detay">Arama yapılacak nesne listesi</param>
        /// <returns>Aranan taşınır hesap planı bulunursa indeksi, bulunamazsa -1 döndürülür.</returns>
        private static int DetayIndexDondur(string hesapKodu, int olcuBirimKod, TNSCollection detay)
        {
            for (int i = 0; i < detay.Count; i++)
            {
                if (detay[i].GetType() == typeof(CetvelDetay))
                {
                    CetvelDetay cd = (CetvelDetay)detay[i];
                    if (hesapKodu == cd.hesapPlanKod && olcuBirimKod == cd.olcuBirimKod)
                        return i;
                }
                else if (detay[i].GetType() == typeof(TuketimCikisDetay))
                {
                    TuketimCikisDetay tkd = (TuketimCikisDetay)detay[i];
                    if (hesapKodu == tkd.hesapPlanKod && olcuBirimKod == tkd.olcuBirimKod)
                        return i;
                }
            }
            return -1;
        }

        private static int DetayIndexDondur(string muhasebeKodu, string hesapKodu, int olcuBirimKod, TNSCollection detay)
        {
            for (int i = 0; i < detay.Count; i++)
            {
                CetvelDetay cd = (CetvelDetay)detay[i];
                if (muhasebeKodu == cd.muhasebeKod && hesapKodu == cd.hesapPlanKod && olcuBirimKod == cd.olcuBirimKod)
                    return i;
            }
            return -1;
        }

        private static int DetayIndexDondurHarcama(string harcamaKodu, string hesapKodu, int olcuBirimKod, TNSCollection detay)
        {
            for (int i = 0; i < detay.Count; i++)
            {
                CetvelDetay cd = (CetvelDetay)detay[i];
                if (harcamaKodu == cd.harcamaKod && hesapKodu == cd.hesapPlanKod && olcuBirimKod == cd.olcuBirimKod)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Parametre olarak verilen kod ve ölçü birimini verilen nesne listesinde
        /// raporTur değişkenine göre arayan ve listedeki indeksini döndüren yordam
        /// </summary>
        /// <param name="kod">Rapor türüyle ilişkili aranacak kod</param>
        /// <param name="olcuBirimKod">Ölçü birimi</param>
        /// <param name="detay">Arama yapılacak nesne listesi</param>
        /// <param name="raporTur">ENUMDepoDurumRaporTur listesindeki değerlerden biri olmalıdır.</param>
        /// <returns>Aranan kod bulunursa indeksi, bulunamazsa -1 döndürülür.</returns>
        private static int DetayIndexDondurDepoDurum(string kod, int olcuBirimKod, TNSCollection detay, int raporTur)
        {
            for (int i = 0; i < detay.Count; i++)
            {
                DepoDurum dp = (DepoDurum)detay[i];
                if (raporTur == (int)ENUMDepoDurumRaporTur.MUHASEBE && kod == dp.muhasebeKod && olcuBirimKod == dp.olcuBirimKod)
                    return i;
                else if (raporTur == (int)ENUMDepoDurumRaporTur.HARCAMA && kod.Replace(".", "") == dp.muhasebeKod + "-" + dp.harcamaKod.Replace(".", "") && olcuBirimKod == dp.olcuBirimKod)
                    return i;
                else if (raporTur == (int)ENUMDepoDurumRaporTur.IL && kod == dp.ilKod && olcuBirimKod == dp.olcuBirimKod)
                    return i;
                else if ((raporTur == (int)ENUMDepoDurumRaporTur.AMBAR || raporTur == (int)ENUMDepoDurumRaporTur.KURUM) && kod == dp.hesapPlanKod && olcuBirimKod == dp.olcuBirimKod)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Parametre olarak verilen DataReader nesnesini dolaşarak taşınır sayım ve
        /// döküm cetveli bilgilerinin tip parametresine uygun olanlarını toplayan ve
        /// varsa bilgi listesindeki nesneye, yoksa yeni yarattığı nesneye ekleyen yordam
        /// </summary>
        /// <param name="dr">DataReader nesnesi</param>
        /// <param name="bilgi">Daha önce toplanan bilgileri tutan nesne</param>
        /// <param name="tip">DataReader nesnesinden toplanacak rakam tipi</param>
        private static void TasinirSayimCetveliBilgileriAl(IDataReader dr, TNSCollection bilgi, ENUMCetvelRakamTipi tip, bool harcamaBazli)
        {
            Cetvel cetvel = new Cetvel();
            //string eskiHesapKod = string.Empty;

            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                if (bilgi.Count <= 0)
                {
                    cetvel = new Cetvel();
                    //cetvel.yil = VeriErisim.DegerAl(dr, 0, 0);
                    cetvel.ilKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                    cetvel.ilAd = VeriErisim.DegerAl(dr, 1, string.Empty);
                    cetvel.ilceKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                    cetvel.ilceAd = VeriErisim.DegerAl(dr, 3, string.Empty);
                    cetvel.muhasebeKod = VeriErisim.DegerAl(dr, 4, string.Empty);
                    cetvel.muhasebeAd = VeriErisim.DegerAl(dr, 5, string.Empty);
                    cetvel.harcamaKod = VeriErisim.DegerAl(dr, 6, string.Empty);
                    cetvel.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), cetvel.harcamaKod);
                    cetvel.harcamaAd = VeriErisim.DegerAl(dr, 7, string.Empty);

                    bilgi.Add(cetvel);
                }
                else
                    cetvel = (Cetvel)bilgi[0];

                string hesapKod = VeriErisim.DegerAl(dr, 8, string.Empty);
                hesapKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), hesapKod);

                if (hesapKod.Replace(".", "").StartsWith("2530199"))//EGO Talebi 14 nolu cetvelde gösterilmesin 04.12.2020
                    continue;

                int olcuBirimKod = VeriErisim.DegerAl(dr, 9, 0);
                string harcamaKod = VeriErisim.DegerAl(dr, 6, string.Empty);
                harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), harcamaKod);
                string harcamaAd = VeriErisim.DegerAl(dr, 7, string.Empty);

                CetvelDetay detay;
                int index = 0;
                if (!harcamaBazli)
                    index = DetayIndexDondur(hesapKod, olcuBirimKod, cetvel.detay);
                else
                    index = DetayIndexDondurHarcama(harcamaKod, hesapKod, olcuBirimKod, cetvel.detay);
                if (index != -1)
                    detay = (CetvelDetay)cetvel.detay[index];
                else
                {
                    detay = new CetvelDetay();
                    cetvel.detay.Add(detay);
                }

                detay.hesapPlanKod = hesapKod;
                if (string.IsNullOrEmpty(detay.hesapPlanAd))
                    detay.hesapPlanAd = Genel.AdGetir(isimler, detay.hesapPlanKod, "TASHESAPPLAN");

                detay.olcuBirimKod = olcuBirimKod;
                detay.olcuBirimAd = VeriErisim.DegerAl(dr, 10, string.Empty);
                detay.harcamaKod = harcamaKod;
                detay.harcamaAd = harcamaAd;
                //detay.olcuBirimAd = Genel.OlcuBirimAdGetir(detay.hesapPlanKod);


                if (tip == ENUMCetvelRakamTipi.GECENYILDANDEVREDENGIRIS)
                {
                    detay.gecenYildanDevreden.miktar += VeriErisim.DegerAl(dr, 11, (decimal)0);
                    detay.gecenYildanDevreden.tutar += VeriErisim.DegerAl(dr, 12, (decimal)0);
                }
                else if (tip == ENUMCetvelRakamTipi.GECENYILDANDEVREDENCIKIS)
                {
                    detay.gecenYildanDevreden.miktar -= VeriErisim.DegerAl(dr, 11, (decimal)0);
                    detay.gecenYildanDevreden.tutar -= VeriErisim.DegerAl(dr, 12, (decimal)0);
                }
                else if (tip == ENUMCetvelRakamTipi.YILICINDEGIREN)
                {
                    detay.yilIcindeGiren.miktar += VeriErisim.DegerAl(dr, 11, (decimal)0);
                    detay.yilIcindeGiren.tutar += VeriErisim.DegerAl(dr, 12, (decimal)0);
                }
                else if (tip == ENUMCetvelRakamTipi.YILICINDECIKAN)
                {
                    detay.yilIcindeCikan.miktar += VeriErisim.DegerAl(dr, 11, (decimal)0);
                    detay.yilIcindeCikan.tutar += VeriErisim.DegerAl(dr, 12, (decimal)0);
                }

                detay.toplam.miktar = detay.gecenYildanDevreden.miktar + detay.yilIcindeGiren.miktar;
                detay.toplam.tutar = detay.gecenYildanDevreden.tutar + detay.yilIcindeGiren.tutar;

                detay.gelecekYilaDevir.miktar = detay.toplam.miktar - detay.yilIcindeCikan.miktar;
                detay.gelecekYilaDevir.tutar = detay.toplam.tutar - detay.yilIcindeCikan.tutar;
            }
            dr.Close();
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır hesap cetveli bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Cetvel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Taşınır hesap cetveli kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır hesap cetveli listesi döndürülür.</returns>
        public static ObjectArray TasinirHesapCetveli(VeriErisim veriErisim, Kullanici kullanan, Cetvel kriter)
        {
            ObjectArray donusDeger = new ObjectArray();

            if (string.IsNullOrEmpty(kurumKod))
            {
                kurumKod = TNS.TMM.Arac.DegiskenDegerBul(0, "/KURUMKOD");
            }

            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.TASINIRMALKONSOLIDEYETKILISI))
            {
                donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan);
                if (!donusDeger.sonuc.islemSonuc)
                {
                    donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
                    return donusDeger;
                }
            }

            string hata = string.Empty;
            if (kriter.yil <= 0)
                hata += TasinirMalResI.TMM036 + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = hata;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tabRapor = new TABRaporlar(ve);
            IDataReader dr = null;

            try
            {
                dr = tabRapor.TasinirHesapCetveli(kriter, ENUMCetvelRakamTipi.GECENYILDANDEVREDENGIRIS, Yetki.YetkiTipiDegerleriDondur(kullanan), kurumKod);
                if (dr != null)
                    TasinirHesapCetveliBilgileriAl(dr, donusDeger.objeler, ENUMCetvelRakamTipi.GECENYILDANDEVREDENGIRIS);

                dr = tabRapor.TasinirHesapCetveli(kriter, ENUMCetvelRakamTipi.GECENYILDANDEVREDENCIKIS, Yetki.YetkiTipiDegerleriDondur(kullanan), kurumKod);
                if (dr != null)
                    TasinirHesapCetveliBilgileriAl(dr, donusDeger.objeler, ENUMCetvelRakamTipi.GECENYILDANDEVREDENCIKIS);

                dr = tabRapor.TasinirHesapCetveli(kriter, ENUMCetvelRakamTipi.YILICINDEGIREN, Yetki.YetkiTipiDegerleriDondur(kullanan), kurumKod);
                if (dr != null)
                    TasinirHesapCetveliBilgileriAl(dr, donusDeger.objeler, ENUMCetvelRakamTipi.YILICINDEGIREN);

                dr = tabRapor.TasinirHesapCetveli(kriter, ENUMCetvelRakamTipi.YILICINDECIKAN, Yetki.YetkiTipiDegerleriDondur(kullanan), kurumKod);
                if (dr != null)
                    TasinirHesapCetveliBilgileriAl(dr, donusDeger.objeler, ENUMCetvelRakamTipi.YILICINDECIKAN);

                if (donusDeger.objeler.Count > 0)
                    CetvelDegerArtislari(ve, kullanan, kriter, donusDeger.objeler, ENUMCetvelTipi.HESAPCETVELI);

                foreach (Cetvel c in donusDeger.objeler)
                    DetaySirala(ref c.detay);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                if (dr != null) dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            if (dr != null) dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }


        /// <summary>
        /// Parametre olarak verilen DataReader nesnesini dolaşarak taşınır hesap cetveli
        /// bilgilerinin tip parametresine uygun olanlarını toplayan ve bulabilirse bilgi
        /// listesindeki ilgili nesneye, bulamazsa yeni yarattığı nesneye ekleyen yordam
        /// </summary>
        /// <param name="dr">DataReader nesnesi</param>
        /// <param name="bilgi">Daha önce toplanan bilgileri tutan nesne</param>
        /// <param name="tip">DataReader nesnesinden toplanacak rakam tipi</param>
        private static void TasinirHesapCetveliBilgileriAl(IDataReader dr, TNSCollection bilgi, ENUMCetvelRakamTipi tip)
        {
            Cetvel cetvel = new Cetvel();
            //string eskiHesapKod = string.Empty;

            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                if (bilgi.Count <= 0)
                {
                    cetvel = new Cetvel();
                    //cetvel.yil = VeriErisim.DegerAl(dr, 0, 0);
                    cetvel.ilKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                    cetvel.ilAd = VeriErisim.DegerAl(dr, 1, string.Empty);
                    cetvel.ilceKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                    cetvel.ilceAd = VeriErisim.DegerAl(dr, 3, string.Empty);

                    bilgi.Add(cetvel);
                }
                else
                {
                    bool buldunMU = false;
                    foreach (Cetvel c in bilgi)
                        if (c.ilKod == VeriErisim.DegerAl(dr, 0, string.Empty) && c.ilceKod == VeriErisim.DegerAl(dr, 2, string.Empty))
                        {
                            buldunMU = true;
                            cetvel = c;
                            break;
                        }

                    if (!buldunMU)
                    {
                        cetvel = new Cetvel();
                        //cetvel.yil = VeriErisim.DegerAl(dr, 0, 0);
                        cetvel.ilKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                        cetvel.ilAd = VeriErisim.DegerAl(dr, 1, string.Empty);
                        cetvel.ilceKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                        cetvel.ilceAd = VeriErisim.DegerAl(dr, 3, string.Empty);

                        bilgi.Add(cetvel);
                    }
                }

                string hesapKod = VeriErisim.DegerAl(dr, 4, string.Empty);
                hesapKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), hesapKod);
                int olcuBirimKod = VeriErisim.DegerAl(dr, 5, 0);

                CetvelDetay detay;
                int index = DetayIndexDondur(hesapKod, olcuBirimKod, cetvel.detay);
                if (index != -1)
                    detay = (CetvelDetay)cetvel.detay[index];
                else
                {
                    detay = new CetvelDetay();
                    cetvel.detay.Add(detay);
                }

                detay.hesapPlanKod = hesapKod;
                if (string.IsNullOrEmpty(detay.hesapPlanAd))
                    detay.hesapPlanAd = Genel.AdGetir(isimler, detay.hesapPlanKod, "TASHESAPPLAN");

                detay.olcuBirimKod = olcuBirimKod;
                detay.olcuBirimAd = VeriErisim.DegerAl(dr, 6, string.Empty);
                //detay.olcuBirimAd = Genel.OlcuBirimAdGetir(detay.hesapPlanKod);

                if (tip == ENUMCetvelRakamTipi.GECENYILDANDEVREDENGIRIS)
                {
                    detay.gecenYildanDevreden.miktar += VeriErisim.DegerAl(dr, 7, (decimal)0);
                    detay.gecenYildanDevreden.tutar += VeriErisim.DegerAl(dr, 8, (decimal)0);
                }
                else if (tip == ENUMCetvelRakamTipi.GECENYILDANDEVREDENCIKIS)
                {
                    detay.gecenYildanDevreden.miktar -= VeriErisim.DegerAl(dr, 7, (decimal)0);
                    detay.gecenYildanDevreden.tutar -= VeriErisim.DegerAl(dr, 8, (decimal)0);
                }
                else if (tip == ENUMCetvelRakamTipi.YILICINDEGIREN)
                {
                    detay.yilIcindeGiren.miktar = VeriErisim.DegerAl(dr, 7, (decimal)0);
                    detay.yilIcindeGiren.tutar = VeriErisim.DegerAl(dr, 8, (decimal)0);
                }
                else if (tip == ENUMCetvelRakamTipi.YILICINDECIKAN)
                {
                    detay.yilIcindeCikan.miktar = VeriErisim.DegerAl(dr, 7, (decimal)0);
                    detay.yilIcindeCikan.tutar = VeriErisim.DegerAl(dr, 8, (decimal)0);
                }

                detay.toplam.miktar = detay.gecenYildanDevreden.miktar + detay.yilIcindeGiren.miktar;
                detay.toplam.tutar = detay.gecenYildanDevreden.tutar + detay.yilIcindeGiren.tutar;

                detay.gelecekYilaDevir.miktar = detay.toplam.miktar - detay.yilIcindeCikan.miktar;
                detay.gelecekYilaDevir.tutar = detay.toplam.tutar - detay.yilIcindeCikan.tutar;
            }
            if (dr != null) dr.Close();
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır hesap cetveli (bölge) bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Cetvel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Taşınır hesap cetveli (bölge) kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır hesap cetveli (bölge) listesi döndürülür.</returns>
        public static ObjectArray TasinirHesapCetveliBolge(VeriErisim veriErisim, Kullanici kullanan, Cetvel kriter)
        {
            ObjectArray donusDeger = new ObjectArray();

            if (string.IsNullOrEmpty(kurumKod))
            {
                kurumKod = TNS.TMM.Arac.DegiskenDegerBul(0, "/KURUMKOD");
            }

            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.TASINIRMALKONSOLIDEYETKILISI))
            {
                donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan);
                if (!donusDeger.sonuc.islemSonuc)
                {
                    donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
                    return donusDeger;
                }
            }

            string hata = string.Empty;
            if (kriter.yil <= 0)
                hata += TasinirMalResI.TMM036 + "<br>";
            //if (string.IsNullOrEmpty(kriter.bolgeKod))
            //    hata += "Bölge boş bırakılamaz.<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = hata;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tabRapor = new TABRaporlar(ve);
            IDataReader dr = null;

            try
            {
                dr = tabRapor.TasinirHesapCetveliBolge(kriter, ENUMCetvelRakamTipi.GECENYILDANDEVREDENGIRIS, Yetki.YetkiTipiDegerleriDondur(kullanan), kurumKod);
                if (dr != null)
                    TasinirHesapCetveliBolgeBilgileriAl(dr, donusDeger.objeler, ENUMCetvelRakamTipi.GECENYILDANDEVREDENGIRIS);

                dr = tabRapor.TasinirHesapCetveliBolge(kriter, ENUMCetvelRakamTipi.GECENYILDANDEVREDENCIKIS, Yetki.YetkiTipiDegerleriDondur(kullanan), kurumKod);
                if (dr != null)
                    TasinirHesapCetveliBolgeBilgileriAl(dr, donusDeger.objeler, ENUMCetvelRakamTipi.GECENYILDANDEVREDENCIKIS);

                dr = tabRapor.TasinirHesapCetveliBolge(kriter, ENUMCetvelRakamTipi.YILICINDEGIREN, Yetki.YetkiTipiDegerleriDondur(kullanan), kurumKod);
                if (dr != null)
                    TasinirHesapCetveliBolgeBilgileriAl(dr, donusDeger.objeler, ENUMCetvelRakamTipi.YILICINDEGIREN);

                dr = tabRapor.TasinirHesapCetveliBolge(kriter, ENUMCetvelRakamTipi.YILICINDECIKAN, Yetki.YetkiTipiDegerleriDondur(kullanan), kurumKod);
                if (dr != null)
                    TasinirHesapCetveliBolgeBilgileriAl(dr, donusDeger.objeler, ENUMCetvelRakamTipi.YILICINDECIKAN);

                if (donusDeger.objeler.Count > 0)
                    CetvelDegerArtislari(ve, kullanan, kriter, donusDeger.objeler, ENUMCetvelTipi.HESAPCETVELIIL);

                foreach (Cetvel c in donusDeger.objeler)
                    DetaySirala(ref c.detay);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                if (dr != null) dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            if (dr != null) dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen DataReader nesnesini dolaşarak taşınır hesap cetveli
        /// (bölge) bilgilerinin tip parametresine uygun olanlarını toplayan ve bulabilirse
        /// bilgi listesindeki ilgili nesneye, bulamazsa yeni yarattığı nesneye ekleyen yordam
        /// </summary>
        /// <param name="dr">DataReader nesnesi</param>
        /// <param name="bilgi">Daha önce toplanan bilgileri tutan nesne</param>
        /// <param name="tip">DataReader nesnesinden toplanacak rakam tipi</param>
        private static void TasinirHesapCetveliBolgeBilgileriAl(IDataReader dr, TNSCollection bilgi, ENUMCetvelRakamTipi tip)
        {
            Cetvel cetvel = new Cetvel();
            //string eskiHesapKod = string.Empty;

            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                if (bilgi.Count <= 0)
                {
                    cetvel = new Cetvel();
                    //cetvel.yil = VeriErisim.DegerAl(dr, 0, 0);
                    cetvel.bolgeKod = VeriErisim.DegerAl(dr, 0, 0);
                    cetvel.bolgeAd = VeriErisim.DegerAl(dr, 1, string.Empty);

                    bilgi.Add(cetvel);
                }
                else
                {
                    bool buldunMU = false;
                    foreach (Cetvel c in bilgi)
                    {
                        if (c.bolgeKod == VeriErisim.DegerAl(dr, 0, 0))
                        {
                            buldunMU = true;
                            cetvel = c;
                            break;
                        }
                    }

                    if (!buldunMU)
                    {
                        cetvel = new Cetvel();
                        //cetvel.yil = VeriErisim.DegerAl(dr, 0, 0);
                        cetvel.bolgeKod = VeriErisim.DegerAl(dr, 0, 0);
                        cetvel.bolgeAd = VeriErisim.DegerAl(dr, 1, string.Empty);

                        bilgi.Add(cetvel);
                    }
                }

                string hesapKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                hesapKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), hesapKod);
                int olcuBirimKod = VeriErisim.DegerAl(dr, 3, 0);

                CetvelDetay detay;
                int index = DetayIndexDondur(hesapKod, olcuBirimKod, cetvel.detay);
                if (index != -1)
                    detay = (CetvelDetay)cetvel.detay[index];
                else
                {
                    detay = new CetvelDetay();
                    cetvel.detay.Add(detay);
                }

                detay.hesapPlanKod = hesapKod;
                if (string.IsNullOrEmpty(detay.hesapPlanAd))
                    detay.hesapPlanAd = Genel.AdGetir(isimler, detay.hesapPlanKod, "TASHESAPPLAN");

                detay.olcuBirimKod = olcuBirimKod;
                detay.olcuBirimAd = VeriErisim.DegerAl(dr, 4, string.Empty);
                //detay.olcuBirimAd = Genel.OlcuBirimAdGetir(detay.hesapPlanKod);

                if (tip == ENUMCetvelRakamTipi.GECENYILDANDEVREDENGIRIS)
                {
                    detay.gecenYildanDevreden.miktar += VeriErisim.DegerAl(dr, 5, (decimal)0);
                    detay.gecenYildanDevreden.tutar += VeriErisim.DegerAl(dr, 6, (decimal)0);
                }
                else if (tip == ENUMCetvelRakamTipi.GECENYILDANDEVREDENCIKIS)
                {
                    detay.gecenYildanDevreden.miktar -= VeriErisim.DegerAl(dr, 5, (decimal)0);
                    detay.gecenYildanDevreden.tutar -= VeriErisim.DegerAl(dr, 6, (decimal)0);
                }
                else if (tip == ENUMCetvelRakamTipi.YILICINDEGIREN)
                {
                    detay.yilIcindeGiren.miktar = VeriErisim.DegerAl(dr, 5, (decimal)0);
                    detay.yilIcindeGiren.tutar = VeriErisim.DegerAl(dr, 6, (decimal)0);
                }
                else if (tip == ENUMCetvelRakamTipi.YILICINDECIKAN)
                {
                    detay.yilIcindeCikan.miktar = VeriErisim.DegerAl(dr, 5, (decimal)0);
                    detay.yilIcindeCikan.tutar = VeriErisim.DegerAl(dr, 6, (decimal)0);
                }

                detay.toplam.miktar = detay.gecenYildanDevreden.miktar + detay.yilIcindeGiren.miktar;
                detay.toplam.tutar = detay.gecenYildanDevreden.tutar + detay.yilIcindeGiren.tutar;

                detay.gelecekYilaDevir.miktar = detay.toplam.miktar - detay.yilIcindeCikan.miktar;
                detay.gelecekYilaDevir.tutar = detay.toplam.tutar - detay.yilIcindeCikan.tutar;
            }
            if (dr != null) dr.Close();
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır hesap cetveli (il) bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Cetvel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Taşınır hesap cetveli (il) kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır hesap cetveli (il) listesi döndürülür.</returns>
        public static ObjectArray TasinirHesapCetveliIl(VeriErisim veriErisim, Kullanici kullanan, Cetvel kriter)
        {
            ObjectArray donusDeger = new ObjectArray();

            if (string.IsNullOrEmpty(kurumKod))
            {
                kurumKod = TNS.TMM.Arac.DegiskenDegerBul(0, "/KURUMKOD");
            }

            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.TASINIRMALKONSOLIDEYETKILISI))
            {
                donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan);
                if (!donusDeger.sonuc.islemSonuc)
                {
                    donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
                    return donusDeger;
                }
            }

            string hata = string.Empty;
            if (kriter.yil <= 0)
                hata += TasinirMalResI.TMM036 + "<br>";
            if (string.IsNullOrEmpty(kriter.ilKod))
                hata += TasinirMalResI.TMM191 + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = hata;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tabRapor = new TABRaporlar(ve);
            IDataReader dr = null;

            try
            {
                dr = tabRapor.TasinirHesapCetveliIl(kriter, ENUMCetvelRakamTipi.GECENYILDANDEVREDENGIRIS, Yetki.YetkiTipiDegerleriDondur(kullanan), kurumKod);
                if (dr != null)
                    TasinirHesapCetveliIlBilgileriAl(dr, donusDeger.objeler, ENUMCetvelRakamTipi.GECENYILDANDEVREDENGIRIS);

                dr = tabRapor.TasinirHesapCetveliIl(kriter, ENUMCetvelRakamTipi.GECENYILDANDEVREDENCIKIS, Yetki.YetkiTipiDegerleriDondur(kullanan), kurumKod);
                if (dr != null)
                    TasinirHesapCetveliIlBilgileriAl(dr, donusDeger.objeler, ENUMCetvelRakamTipi.GECENYILDANDEVREDENCIKIS);

                dr = tabRapor.TasinirHesapCetveliIl(kriter, ENUMCetvelRakamTipi.YILICINDEGIREN, Yetki.YetkiTipiDegerleriDondur(kullanan), kurumKod);
                if (dr != null)
                    TasinirHesapCetveliIlBilgileriAl(dr, donusDeger.objeler, ENUMCetvelRakamTipi.YILICINDEGIREN);

                dr = tabRapor.TasinirHesapCetveliIl(kriter, ENUMCetvelRakamTipi.YILICINDECIKAN, Yetki.YetkiTipiDegerleriDondur(kullanan), kurumKod);
                if (dr != null)
                    TasinirHesapCetveliIlBilgileriAl(dr, donusDeger.objeler, ENUMCetvelRakamTipi.YILICINDECIKAN);

                if (donusDeger.objeler.Count > 0)
                    CetvelDegerArtislari(ve, kullanan, kriter, donusDeger.objeler, ENUMCetvelTipi.HESAPCETVELIIL);

                foreach (Cetvel c in donusDeger.objeler)
                    DetaySirala(ref c.detay);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                if (dr != null) dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            if (dr != null) dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen DataReader nesnesini dolaşarak taşınır hesap cetveli
        /// (il) bilgilerinin tip parametresine uygun olanlarını toplayan ve bulabilirse
        /// bilgi listesindeki ilgili nesneye, bulamazsa yeni yarattığı nesneye ekleyen yordam
        /// </summary>
        /// <param name="dr">DataReader nesnesi</param>
        /// <param name="bilgi">Daha önce toplanan bilgileri tutan nesne</param>
        /// <param name="tip">DataReader nesnesinden toplanacak rakam tipi</param>
        private static void TasinirHesapCetveliIlBilgileriAl(IDataReader dr, TNSCollection bilgi, ENUMCetvelRakamTipi tip)
        {
            Cetvel cetvel = new Cetvel();
            //string eskiHesapKod = string.Empty;

            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                if (bilgi.Count <= 0)
                {
                    cetvel = new Cetvel();
                    //cetvel.yil = VeriErisim.DegerAl(dr, 0, 0);
                    cetvel.ilKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                    cetvel.ilAd = VeriErisim.DegerAl(dr, 1, string.Empty);
                    //cetvel.muhasebeKod = VeriErisim.DegerAl(dr, 3, string.Empty);
                    //cetvel.muhasebeAd = VeriErisim.DegerAl(dr, 4, string.Empty);

                    /*
                     * Erdal harcama bazında dönmesin diye
                            cetvel.harcamaKod = VeriErisim.DegerAl(dr, 3, string.Empty);
                            cetvel.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), cetvel.harcamaKod);
                     * Erdal harcama bazında dönmesin diye
                     */

                    //cetvel.harcamaAd = VeriErisim.DegerAl(dr, 4, string.Empty);

                    bilgi.Add(cetvel);
                }
                else
                {
                    bool buldunMU = false;
                    foreach (Cetvel c in bilgi)
                    {
                        //Erdal harcama bazında dönmesin diye
                        //string harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), VeriErisim.DegerAl(dr, 3, string.Empty));
                        //if (c.ilKod == VeriErisim.DegerAl(dr, 1, string.Empty) && c.harcamaKod == harcamaKod)
                        if (c.ilKod == VeriErisim.DegerAl(dr, 0, string.Empty))
                        {
                            buldunMU = true;
                            cetvel = c;
                            break;
                        }
                    }

                    if (!buldunMU)
                    {
                        cetvel = new Cetvel();
                        //cetvel.yil = VeriErisim.DegerAl(dr, 0, 0);
                        cetvel.ilKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                        cetvel.ilAd = VeriErisim.DegerAl(dr, 1, string.Empty);
                        //cetvel.muhasebeKod = VeriErisim.DegerAl(dr, 3, string.Empty);
                        //cetvel.muhasebeAd = VeriErisim.DegerAl(dr, 4, string.Empty);

                        //Erdal harcama bazında dönmesin diye
                        //cetvel.harcamaKod = VeriErisim.DegerAl(dr, 3, string.Empty);
                        //cetvel.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), cetvel.harcamaKod);

                        //cetvel.harcamaAd = VeriErisim.DegerAl(dr, 4, string.Empty);

                        bilgi.Add(cetvel);
                    }
                }

                string hesapKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                hesapKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), hesapKod);
                int olcuBirimKod = VeriErisim.DegerAl(dr, 3, 0);

                CetvelDetay detay;
                int index = DetayIndexDondur(hesapKod, olcuBirimKod, cetvel.detay);
                if (index != -1)
                    detay = (CetvelDetay)cetvel.detay[index];
                else
                {
                    detay = new CetvelDetay();
                    cetvel.detay.Add(detay);
                }

                detay.hesapPlanKod = hesapKod;
                if (string.IsNullOrEmpty(detay.hesapPlanAd))
                    detay.hesapPlanAd = Genel.AdGetir(isimler, detay.hesapPlanKod, "TASHESAPPLAN");

                detay.olcuBirimKod = olcuBirimKod;
                detay.olcuBirimAd = VeriErisim.DegerAl(dr, 4, string.Empty);
                //detay.olcuBirimAd = Genel.OlcuBirimAdGetir(detay.hesapPlanKod);

                if (tip == ENUMCetvelRakamTipi.GECENYILDANDEVREDENGIRIS)
                {
                    detay.gecenYildanDevreden.miktar += VeriErisim.DegerAl(dr, 5, (decimal)0);
                    detay.gecenYildanDevreden.tutar += VeriErisim.DegerAl(dr, 6, (decimal)0);
                }
                else if (tip == ENUMCetvelRakamTipi.GECENYILDANDEVREDENCIKIS)
                {
                    detay.gecenYildanDevreden.miktar -= VeriErisim.DegerAl(dr, 5, (decimal)0);
                    detay.gecenYildanDevreden.tutar -= VeriErisim.DegerAl(dr, 6, (decimal)0);
                }
                else if (tip == ENUMCetvelRakamTipi.YILICINDEGIREN)
                {
                    detay.yilIcindeGiren.miktar = VeriErisim.DegerAl(dr, 5, (decimal)0);
                    detay.yilIcindeGiren.tutar = VeriErisim.DegerAl(dr, 6, (decimal)0);
                }
                else if (tip == ENUMCetvelRakamTipi.YILICINDECIKAN)
                {
                    detay.yilIcindeCikan.miktar = VeriErisim.DegerAl(dr, 5, (decimal)0);
                    detay.yilIcindeCikan.tutar = VeriErisim.DegerAl(dr, 6, (decimal)0);
                }

                detay.toplam.miktar = detay.gecenYildanDevreden.miktar + detay.yilIcindeGiren.miktar;
                detay.toplam.tutar = detay.gecenYildanDevreden.tutar + detay.yilIcindeGiren.tutar;

                detay.gelecekYilaDevir.miktar = detay.toplam.miktar - detay.yilIcindeCikan.miktar;
                detay.gelecekYilaDevir.tutar = detay.toplam.tutar - detay.yilIcindeCikan.tutar;
            }
            if (dr != null) dr.Close();
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır kesin hesap cetveli bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Cetvel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Taşınır kesin hesap cetveli kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır kesin hesap cetveli listesi döndürülür.</returns>
        public static ObjectArray TasinirKesinHesapCetveli(VeriErisim veriErisim, Kullanici kullanan, Cetvel kriter, bool muhasebeEkle)
        {
            ObjectArray donusDeger = new ObjectArray();

            if (string.IsNullOrEmpty(kurumKod))
            {
                kurumKod = TNS.TMM.Arac.DegiskenDegerBul(0, "/KURUMKOD");
            }

            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.TASINIRMALKONSOLIDEYETKILISI))
            {
                donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMiHarcamaBirimi(kullanan, kriter.muhasebeKod, kriter.harcamaKod);
                if (!donusDeger.sonuc.islemSonuc)
                {
                    donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
                    return donusDeger;
                }
            }

            string hata = string.Empty;
            if (kriter.yil <= 0)
                hata += TasinirMalResI.TMM036 + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = hata;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tabRapor = new TABRaporlar(ve);
            IDataReader dr = null;

            try
            {
                dr = tabRapor.TasinirKesinHesapCetveli(kriter, ENUMCetvelRakamTipi.GECENYILDANDEVREDENGIRIS, Yetki.YetkiTipiDegerleriDondur(kullanan), muhasebeEkle, kurumKod);
                if (dr != null)
                    TasinirKesinHesapCetveliVEIcmalBilgileriAl(dr, donusDeger.objeler, ENUMCetvelRakamTipi.GECENYILDANDEVREDENGIRIS, false, muhasebeEkle);

                dr = tabRapor.TasinirKesinHesapCetveli(kriter, ENUMCetvelRakamTipi.GECENYILDANDEVREDENCIKIS, Yetki.YetkiTipiDegerleriDondur(kullanan), muhasebeEkle, kurumKod);
                if (dr != null)
                    TasinirKesinHesapCetveliVEIcmalBilgileriAl(dr, donusDeger.objeler, ENUMCetvelRakamTipi.GECENYILDANDEVREDENCIKIS, false, muhasebeEkle);

                dr = tabRapor.TasinirKesinHesapCetveli(kriter, ENUMCetvelRakamTipi.YILICINDEGIREN, Yetki.YetkiTipiDegerleriDondur(kullanan), muhasebeEkle, kurumKod);
                if (dr != null)
                    TasinirKesinHesapCetveliVEIcmalBilgileriAl(dr, donusDeger.objeler, ENUMCetvelRakamTipi.YILICINDEGIREN, false, muhasebeEkle);

                dr = tabRapor.TasinirKesinHesapCetveli(kriter, ENUMCetvelRakamTipi.YILICINDECIKAN, Yetki.YetkiTipiDegerleriDondur(kullanan), muhasebeEkle, kurumKod);
                if (dr != null)
                    TasinirKesinHesapCetveliVEIcmalBilgileriAl(dr, donusDeger.objeler, ENUMCetvelRakamTipi.YILICINDECIKAN, false, muhasebeEkle);

                if (donusDeger.objeler.Count > 0)
                    CetvelDegerArtislari(ve, kullanan, kriter, donusDeger.objeler, ENUMCetvelTipi.KESINHESAPCETVELI, muhasebeEkle);

                if (!muhasebeEkle)
                {
                    foreach (Cetvel c in donusDeger.objeler)
                        DetaySirala(ref c.detay);
                }
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                if (dr != null) dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            if (dr != null) dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır kesin hesap icmal cetveli bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Cetvel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Taşınır kesin hesap icmal cetveli kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır kesin hesap icmal cetveli listesi döndürülür.</returns>
        public static ObjectArray TasinirKesinHesapIcmalCetveli(VeriErisim veriErisim, Kullanici kullanan, Cetvel kriter, bool muhasebeEkle)
        {
            ObjectArray donusDeger = new ObjectArray();

            if (string.IsNullOrEmpty(kurumKod))
            {
                kurumKod = TNS.TMM.Arac.DegiskenDegerBul(0, "/KURUMKOD");
            }

            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.TASINIRMALKONSOLIDEYETKILISI))
            {
                donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMiHarcamaBirimi(kullanan, kriter.muhasebeKod, kriter.harcamaKod);
                if (!donusDeger.sonuc.islemSonuc)
                {
                    donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
                    return donusDeger;
                }
            }

            string hata = string.Empty;
            if (kriter.yil <= 0)
                hata += TasinirMalResI.TMM036 + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = hata;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tabRapor = new TABRaporlar(ve);
            IDataReader dr = null;

            try
            {
                dr = tabRapor.TasinirKesinHesapIcmalCetveli(kriter, ENUMCetvelRakamTipi.GECENYILDANDEVREDENGIRIS, Yetki.YetkiTipiDegerleriDondur(kullanan), muhasebeEkle, kurumKod, true);
                if (dr != null)
                    TasinirKesinHesapCetveliVEIcmalBilgileriAl(dr, donusDeger.objeler, ENUMCetvelRakamTipi.GECENYILDANDEVREDENGIRIS, true, muhasebeEkle);

                dr = tabRapor.TasinirKesinHesapIcmalCetveli(kriter, ENUMCetvelRakamTipi.GECENYILDANDEVREDENCIKIS, Yetki.YetkiTipiDegerleriDondur(kullanan), muhasebeEkle, kurumKod, true);
                if (dr != null)
                    TasinirKesinHesapCetveliVEIcmalBilgileriAl(dr, donusDeger.objeler, ENUMCetvelRakamTipi.GECENYILDANDEVREDENCIKIS, true, muhasebeEkle);

                dr = tabRapor.TasinirKesinHesapIcmalCetveli(kriter, ENUMCetvelRakamTipi.YILICINDEGIREN, Yetki.YetkiTipiDegerleriDondur(kullanan), muhasebeEkle, kurumKod, true);
                if (dr != null)
                    TasinirKesinHesapCetveliVEIcmalBilgileriAl(dr, donusDeger.objeler, ENUMCetvelRakamTipi.YILICINDEGIREN, true, muhasebeEkle);

                dr = tabRapor.TasinirKesinHesapIcmalCetveli(kriter, ENUMCetvelRakamTipi.YILICINDECIKAN, Yetki.YetkiTipiDegerleriDondur(kullanan), muhasebeEkle, kurumKod, true);
                if (dr != null)
                    TasinirKesinHesapCetveliVEIcmalBilgileriAl(dr, donusDeger.objeler, ENUMCetvelRakamTipi.YILICINDECIKAN, true, muhasebeEkle);

                if (donusDeger.objeler.Count > 0)
                    CetvelDegerArtislari(ve, kullanan, kriter, donusDeger.objeler, ENUMCetvelTipi.KESINHESAPCETVELIICMAL, muhasebeEkle);

                if (!muhasebeEkle)
                {
                    foreach (Cetvel c in donusDeger.objeler)
                        DetaySirala(ref c.detay);
                }
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                if (dr != null) dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            if (dr != null) dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen DataReader nesnesini dolaşarak taşınır kesin hesap cetveli
        /// veya taşınır kesin hesap icmal cetveli bilgilerinin tip parametresine uygun olanlarını
        /// toplayan ve varsa bilgi listesindeki nesneye, yoksa yeni yarattığı nesneye ekleyen yordam
        /// </summary>
        /// <param name="dr">DataReader nesnesi</param>
        /// <param name="bilgi">Daha önce toplanan bilgileri tutan nesne</param>
        /// <param name="tip">DataReader nesnesinden toplanacak rakam tipi</param>
        /// <param name="icmalMi">DataReader nesnesi icmal bilgileri mi içeriyor bilgisi</param>
        private static void TasinirKesinHesapCetveliVEIcmalBilgileriAl(IDataReader dr, TNSCollection bilgi, ENUMCetvelRakamTipi tip, bool icmalMi, bool muhasebeEkle)
        {
            Cetvel cetvel = new Cetvel();
            //string eskiHesapKod = string.Empty;

            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                if (bilgi.Count <= 0)
                {
                    cetvel = new Cetvel();
                    //cetvel.yil = VeriErisim.DegerAl(dr, 0, 0);

                    bilgi.Add(cetvel);
                }
                else
                    cetvel = (Cetvel)bilgi[0];

                string hesapKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                hesapKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), hesapKod);

                int olcuBirimKod = 0;
                string muhasebeKod = string.Empty;
                if (!icmalMi)
                {
                    olcuBirimKod = VeriErisim.DegerAl(dr, 1, 0);
                    if (muhasebeEkle)
                        muhasebeKod = VeriErisim.DegerAl(dr, 3, string.Empty);
                }
                else if (muhasebeEkle)
                    muhasebeKod = VeriErisim.DegerAl(dr, 1, string.Empty);

                CetvelDetay detay;
                int index;
                if (muhasebeEkle)
                    index = DetayIndexDondur(muhasebeKod, hesapKod, olcuBirimKod, cetvel.detay);
                else
                    index = DetayIndexDondur(hesapKod, olcuBirimKod, cetvel.detay);
                if (index != -1)
                    detay = (CetvelDetay)cetvel.detay[index];
                else
                {
                    detay = new CetvelDetay();
                    cetvel.detay.Add(detay);
                }

                detay.hesapPlanKod = hesapKod;
                if (string.IsNullOrEmpty(detay.hesapPlanAd))
                    detay.hesapPlanAd = Genel.AdGetir(isimler, detay.hesapPlanKod, "TASHESAPPLAN");

                int rakamKolon = 0;
                if (!icmalMi)
                {
                    detay.olcuBirimKod = olcuBirimKod;
                    detay.olcuBirimAd = VeriErisim.DegerAl(dr, 2, string.Empty);
                    //detay.olcuBirimAd = Genel.OlcuBirimAdGetir(detay.hesapPlanKod);

                    rakamKolon = 2;
                    if (muhasebeEkle)
                    {
                        rakamKolon = 4;
                        detay.muhasebeKod = muhasebeKod;
                        detay.muhasebeAd = VeriErisim.DegerAl(dr, 4, string.Empty);
                    }
                }
                else if (muhasebeEkle)
                {
                    rakamKolon = 2;
                    detay.muhasebeKod = muhasebeKod;
                    detay.muhasebeAd = VeriErisim.DegerAl(dr, 2, string.Empty);
                }

                if (tip == ENUMCetvelRakamTipi.GECENYILDANDEVREDENGIRIS)
                {
                    detay.gecenYildanDevreden.miktar += VeriErisim.DegerAl(dr, 1 + rakamKolon, (decimal)0);
                    detay.gecenYildanDevreden.tutar += VeriErisim.DegerAl(dr, 2 + rakamKolon, (decimal)0);
                }
                else if (tip == ENUMCetvelRakamTipi.GECENYILDANDEVREDENCIKIS)
                {
                    detay.gecenYildanDevreden.miktar -= VeriErisim.DegerAl(dr, 1 + rakamKolon, (decimal)0);
                    detay.gecenYildanDevreden.tutar -= VeriErisim.DegerAl(dr, 2 + rakamKolon, (decimal)0);
                }
                else if (tip == ENUMCetvelRakamTipi.YILICINDEGIREN)
                {
                    detay.yilIcindeGiren.miktar = VeriErisim.DegerAl(dr, 1 + rakamKolon, (decimal)0);
                    detay.yilIcindeGiren.tutar = VeriErisim.DegerAl(dr, 2 + rakamKolon, (decimal)0);
                }
                else if (tip == ENUMCetvelRakamTipi.YILICINDECIKAN)
                {
                    detay.yilIcindeCikan.miktar = VeriErisim.DegerAl(dr, 1 + rakamKolon, (decimal)0);
                    detay.yilIcindeCikan.tutar = VeriErisim.DegerAl(dr, 2 + rakamKolon, (decimal)0);
                }

                detay.toplam.miktar = detay.gecenYildanDevreden.miktar + detay.yilIcindeGiren.miktar;
                detay.toplam.tutar = detay.gecenYildanDevreden.tutar + detay.yilIcindeGiren.tutar;

                detay.gelecekYilaDevir.miktar = detay.toplam.miktar - detay.yilIcindeCikan.miktar;
                detay.gelecekYilaDevir.tutar = detay.toplam.tutar - detay.yilIcindeCikan.tutar;
            }
            if (dr != null) dr.Close();
        }

        /// <summary>
        /// Verilen kriterlere uygun olan müze/kütüphane yönetim hesabı cetveli bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Cetvel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Müze/Kütüphane yönetim hesabı cetveli kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında müze/kütüphane yönetim hesabı cetveli listesi döndürülür.</returns>
        public static ObjectArray MuzeKutuphaneCetveli(VeriErisim veriErisim, Kullanici kullanan, Cetvel kriter)
        {
            ObjectArray donusDeger = new ObjectArray();

            if (string.IsNullOrEmpty(kurumKod))
            {
                kurumKod = TNS.TMM.Arac.DegiskenDegerBul(0, "/KURUMKOD");
            }

            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.TASINIRMALKONSOLIDEYETKILISI))
            {
                donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan);
                if (!donusDeger.sonuc.islemSonuc)
                {
                    donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
                    return donusDeger;
                }
            }

            string hata = string.Empty;
            if (kriter.yil <= 0)
                hata += TasinirMalResI.TMM036 + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = hata;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tabRapor = new TABRaporlar(ve);
            IDataReader dr = null;

            try
            {
                dr = tabRapor.MuzeKutuphaneCetveli(kriter, ENUMCetvelRakamTipi.GECENYILDANDEVREDENGIRIS, Yetki.YetkiTipiDegerleriDondur(kullanan), kurumKod);
                if (dr != null)
                    MuzeKutuphaneCetveliBilgileriAl(dr, donusDeger.objeler, ENUMCetvelRakamTipi.GECENYILDANDEVREDENGIRIS);

                dr = tabRapor.MuzeKutuphaneCetveli(kriter, ENUMCetvelRakamTipi.GECENYILDANDEVREDENCIKIS, Yetki.YetkiTipiDegerleriDondur(kullanan), kurumKod);
                if (dr != null)
                    MuzeKutuphaneCetveliBilgileriAl(dr, donusDeger.objeler, ENUMCetvelRakamTipi.GECENYILDANDEVREDENCIKIS);

                dr = tabRapor.MuzeKutuphaneCetveli(kriter, ENUMCetvelRakamTipi.YILICINDEGIREN, Yetki.YetkiTipiDegerleriDondur(kullanan), kurumKod);
                if (dr != null)
                    MuzeKutuphaneCetveliBilgileriAl(dr, donusDeger.objeler, ENUMCetvelRakamTipi.YILICINDEGIREN);

                dr = tabRapor.MuzeKutuphaneCetveli(kriter, ENUMCetvelRakamTipi.YILICINDECIKAN, Yetki.YetkiTipiDegerleriDondur(kullanan), kurumKod);
                if (dr != null)
                    MuzeKutuphaneCetveliBilgileriAl(dr, donusDeger.objeler, ENUMCetvelRakamTipi.YILICINDECIKAN);

                if (donusDeger.objeler.Count > 0)
                    CetvelDegerArtislari(ve, kullanan, kriter, donusDeger.objeler, ENUMCetvelTipi.MUZEKUTUPHANECETVELI);

                foreach (Cetvel c in donusDeger.objeler)
                    DetaySirala(ref c.detay);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                if (dr != null) dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            if (dr != null) dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen DataReader nesnesini dolaşarak müze/kütüphane yönetim
        /// hesabı cetveli bilgilerinin tip parametresine uygun olanlarını toplayan ve
        /// varsa bilgi listesindeki nesneye, yoksa yeni yarattığı nesneye ekleyen yordam
        /// </summary>
        /// <param name="dr">DataReader nesnesi</param>
        /// <param name="bilgi">Daha önce toplanan bilgileri tutan nesne</param>
        /// <param name="tip">DataReader nesnesinden toplanacak rakam tipi</param>
        private static void MuzeKutuphaneCetveliBilgileriAl(IDataReader dr, TNSCollection bilgi, ENUMCetvelRakamTipi tip)
        {
            Cetvel cetvel = new Cetvel();
            //string eskiHesapKod = string.Empty;

            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                if (bilgi.Count <= 0)
                {
                    cetvel = new Cetvel();
                    //cetvel.yil = VeriErisim.DegerAl(dr, 0, 0);

                    bilgi.Add(cetvel);
                }
                else
                    cetvel = (Cetvel)bilgi[0];

                string hesapKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                hesapKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), hesapKod);
                int olcuBirimKod = VeriErisim.DegerAl(dr, 1, 0);

                CetvelDetay detay;
                int index = DetayIndexDondur(hesapKod, olcuBirimKod, cetvel.detay);
                if (index != -1)
                    detay = (CetvelDetay)cetvel.detay[index];
                else
                {
                    detay = new CetvelDetay();
                    cetvel.detay.Add(detay);
                }

                detay.hesapPlanKod = hesapKod;
                if (string.IsNullOrEmpty(detay.hesapPlanAd))
                    detay.hesapPlanAd = Genel.AdGetir(isimler, detay.hesapPlanKod, "TASHESAPPLAN");

                detay.olcuBirimKod = olcuBirimKod;
                detay.olcuBirimAd = VeriErisim.DegerAl(dr, 2, string.Empty);

                if (tip == ENUMCetvelRakamTipi.GECENYILDANDEVREDENGIRIS)
                {
                    detay.gecenYildanDevreden.miktar += VeriErisim.DegerAl(dr, 3, (decimal)0);
                    detay.gecenYildanDevreden.tutar += VeriErisim.DegerAl(dr, 4, (decimal)0);
                }
                else if (tip == ENUMCetvelRakamTipi.GECENYILDANDEVREDENCIKIS)
                {
                    detay.gecenYildanDevreden.miktar -= VeriErisim.DegerAl(dr, 3, (decimal)0);
                    detay.gecenYildanDevreden.tutar -= VeriErisim.DegerAl(dr, 4, (decimal)0);
                }
                else if (tip == ENUMCetvelRakamTipi.YILICINDEGIREN)
                {
                    detay.yilIcindeGiren.miktar = VeriErisim.DegerAl(dr, 3, (decimal)0);
                    detay.yilIcindeGiren.tutar = VeriErisim.DegerAl(dr, 4, (decimal)0);
                }
                else if (tip == ENUMCetvelRakamTipi.YILICINDECIKAN)
                {
                    detay.yilIcindeCikan.miktar = VeriErisim.DegerAl(dr, 3, (decimal)0);
                    detay.yilIcindeCikan.tutar = VeriErisim.DegerAl(dr, 4, (decimal)0);
                }

                detay.toplam.miktar = detay.gecenYildanDevreden.miktar + detay.yilIcindeGiren.miktar;
                detay.toplam.tutar = detay.gecenYildanDevreden.tutar + detay.yilIcindeGiren.tutar;

                detay.gelecekYilaDevir.miktar = detay.toplam.miktar - detay.yilIcindeCikan.miktar;
                detay.gelecekYilaDevir.tutar = detay.toplam.tutar - detay.yilIcindeCikan.tutar;
            }
            if (dr != null) dr.Close();
        }

        /// <summary>
        /// Verilen DepoDurum kriterlerine uygun olan taşınır malzemelerin giren,
        /// çıkan, zimmet miktarları/tutarları gibi bilgileri döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">DepoDurum listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır malzemelerinin miktar, tutar bilgileri listesi döndürülür.</returns>
        public static ObjectArray TasinirDepoDurumu(VeriErisim veriErisim, Kullanici kullanan, DepoDurum kriter)
        {
            ObjectArray donusDeger = new ObjectArray();

            //Harcama Yönetiminden alındığında yetki problemi oluyor.Melih
            //donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, kriter.muhasebeKod, kriter.harcamaKod, kriter.ambarKod);
            //if (!donusDeger.sonuc.islemSonuc)
            //{
            //    donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
            //    return donusDeger;
            //}

            string hata = string.Empty;
            if (kriter.yil <= 0)
                hata += TasinirMalResI.TMM036 + "<br>";
            if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.AMBAR)
            {
                if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.TASINIRMALKONSOLIDEYETKILISI) &&
                    !kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                {
                    if (string.IsNullOrEmpty(kriter.muhasebeKod))
                        hata += TasinirMalResI.TMM142 + "<br>";
                    if (string.IsNullOrEmpty(kriter.harcamaKod))
                        hata += TasinirMalResI.TMM014 + "<br>";

                    //Harcama Yönetiminden alındığında yetki problemi oluyor.Melih
                    //if (string.IsNullOrEmpty(kriter.ambarKod))
                    //    hata += TasinirMalResI.TMM031 + "<br>";
                }
            }
            else if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.MUHASEBE || kriter.raporTur == (int)ENUMDepoDurumRaporTur.HARCAMA || kriter.raporTur == (int)ENUMDepoDurumRaporTur.IL)
            {
                if (string.IsNullOrEmpty(kriter.hesapPlanKod))
                    hata += TasinirMalResI.TMM027 + "<br>";
                //else
                //    hata += Genel.KontrolHesapPlan(kullanan, kriter.hesapPlanKod, true, 0);
            }
            if (kriter.raporTur < 0)
                hata += TasinirMalResI.TMM192 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = hata;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tabRapor = new TABRaporlar(ve);
            IDataReader dr = null;

            try
            {
                //string[] depoDurumIslem = { "giren", "cikan", "zimmetKisi", "zimmetOrtak" };
                string[] depoDurumIslem;
                if (kriter.khkDurumu != 2)
                    depoDurumIslem = new String[] { "giren", "cikan", "zimmetKisi", "zimmetOrtak" };
                else
                    depoDurumIslem = new String[] { "giren", "cikan" };

                for (int i = 0; i < depoDurumIslem.Length; i++)
                {
                    dr = tabRapor.TasinirDepoDurumu(kriter, depoDurumIslem[i], Yetki.YetkiTipiDegerleriDondur(kullanan));
                    if (dr != null)
                    {
                        TasinirDepoDurumuBilgileriAl(dr, donusDeger.objeler, depoDurumIslem[i], kriter.raporTur);
                        if (dr != null) dr.Close();
                    }
                }
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                ve.VeriErisimKapat();
                return donusDeger;
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen DataReader nesnesini dolaşarak taşınır depo durum
        /// bilgilerinin depoDurumIslem parametresine uygun olanlarını toplayan ve bulabilirse
        /// bilgi listesindeki ilgili nesneye, bulamazsa yeni yarattığı nesneye ekleyen yordam
        /// </summary>
        /// <param name="dr">DataReader nesnesi</param>
        /// <param name="bilgi">Daha önce toplanan bilgileri tutan nesne</param>
        /// <param name="depoDurumIslem">DataReader nesnesinden toplanacak rakam tipi
        /// Alabileceği değerler "giren", "cikan", "zimmetKisi" ve "zimmetOrtak" değerleridir.</param>
        /// <param name="raporTur">ENUMDepoDurumRaporTur listesindeki değerlerden biri olmalıdır.</param>
        private static void TasinirDepoDurumuBilgileriAl(IDataReader dr, TNSCollection bilgi, string depoDurumIslem, int raporTur)
        {
            int hesapSutun = 0;
            int rakamSutun = 0;
            if (raporTur == (int)ENUMDepoDurumRaporTur.MUHASEBE || raporTur == (int)ENUMDepoDurumRaporTur.HARCAMA || raporTur == (int)ENUMDepoDurumRaporTur.IL)
            {
                hesapSutun = 1;
                rakamSutun = -1;
            }
            else if (raporTur == (int)ENUMDepoDurumRaporTur.AMBAR)
            {
                hesapSutun = 11;
                rakamSutun = -2;
            }
            else if (raporTur == (int)ENUMDepoDurumRaporTur.KURUM)
            {
                hesapSutun = 1;
                rakamSutun = -2;
            }

            DepoDurum dp;
            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                string kod = string.Empty;
                int olcuBirimKod = 0;
                if (raporTur == (int)ENUMDepoDurumRaporTur.MUHASEBE || raporTur == (int)ENUMDepoDurumRaporTur.IL)
                {
                    kod = VeriErisim.DegerAl(dr, hesapSutun + 3, string.Empty);
                    olcuBirimKod = VeriErisim.DegerAl(dr, hesapSutun + 1, 0);
                }
                else if (raporTur == (int)ENUMDepoDurumRaporTur.HARCAMA)
                {
                    kod = VeriErisim.DegerAl(dr, hesapSutun + 4, string.Empty);
                    kod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), kod);
                    kod = VeriErisim.DegerAl(dr, hesapSutun + 3, string.Empty) + "-" + kod;
                    olcuBirimKod = VeriErisim.DegerAl(dr, hesapSutun + 1, 0);
                }
                else if (raporTur == (int)ENUMDepoDurumRaporTur.AMBAR || raporTur == (int)ENUMDepoDurumRaporTur.KURUM)
                {
                    kod = VeriErisim.DegerAl(dr, hesapSutun, string.Empty);
                    kod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), kod);
                    olcuBirimKod = VeriErisim.DegerAl(dr, hesapSutun + 2, 0);
                }

                int index = DetayIndexDondurDepoDurum(kod, olcuBirimKod, bilgi, raporTur);
                if (index != -1)
                    dp = (DepoDurum)bilgi[index];
                else
                {
                    dp = new DepoDurum();
                    dp.yil = VeriErisim.DegerAl(dr, 0, 0);
                    if (raporTur == (int)ENUMDepoDurumRaporTur.MUHASEBE)
                    {
                        dp.muhasebeKod = VeriErisim.DegerAl(dr, hesapSutun + 3, string.Empty);
                        dp.muhasebeAd = VeriErisim.DegerAl(dr, hesapSutun + 4, string.Empty);
                    }
                    else if (raporTur == (int)ENUMDepoDurumRaporTur.HARCAMA)
                    {
                        dp.muhasebeKod = VeriErisim.DegerAl(dr, hesapSutun + 3, string.Empty);
                        dp.muhasebeAd = Genel.AdGetir(isimler, dp.muhasebeKod, "TASMUHASEBE");
                        dp.harcamaKod = VeriErisim.DegerAl(dr, hesapSutun + 4, string.Empty);
                        dp.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), dp.harcamaKod);
                        //dp.harcamaAd = VeriErisim.DegerAl(dr, 3, string.Empty);
                        dp.harcamaAd = Genel.AdGetir(isimler, dp.muhasebeKod + "-" + dp.harcamaKod, "TASHARCAMABIRIM");
                    }
                    else if (raporTur == (int)ENUMDepoDurumRaporTur.IL)
                    {
                        dp.ilKod = VeriErisim.DegerAl(dr, hesapSutun + 3, string.Empty);
                        dp.ilAd = VeriErisim.DegerAl(dr, hesapSutun + 4, string.Empty);
                    }
                    else if (raporTur == (int)ENUMDepoDurumRaporTur.AMBAR)
                    {
                        dp.ilKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                        dp.ilAd = VeriErisim.DegerAl(dr, 2, string.Empty);
                        dp.ilceKod = VeriErisim.DegerAl(dr, 3, string.Empty);
                        dp.ilceAd = VeriErisim.DegerAl(dr, 4, string.Empty);
                        dp.muhasebeKod = VeriErisim.DegerAl(dr, 5, string.Empty);
                        dp.muhasebeAd = VeriErisim.DegerAl(dr, 6, string.Empty);
                        dp.harcamaKod = VeriErisim.DegerAl(dr, 7, string.Empty);
                        dp.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), dp.harcamaKod);
                        dp.harcamaAd = VeriErisim.DegerAl(dr, 8, string.Empty);
                        dp.ambarKod = VeriErisim.DegerAl(dr, 9, string.Empty);
                        dp.ambarAd = VeriErisim.DegerAl(dr, 10, string.Empty);
                    }

                    dp.hesapPlanKod = VeriErisim.DegerAl(dr, hesapSutun, string.Empty);
                    dp.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), dp.hesapPlanKod);
                    if (!string.IsNullOrEmpty(dp.hesapPlanKod))
                    {
                        if (raporTur == (int)ENUMDepoDurumRaporTur.AMBAR || raporTur == (int)ENUMDepoDurumRaporTur.KURUM)
                        {
                            dp.hesapPlanAd = VeriErisim.DegerAl(dr, hesapSutun + 1, string.Empty);
                            dp.olcuBirimKod = VeriErisim.DegerAl(dr, hesapSutun + 2, 0);
                            dp.olcuBirimAd = VeriErisim.DegerAl(dr, hesapSutun + 3, string.Empty);
                        }
                        else
                        {
                            dp.hesapPlanAd = Genel.AdGetir(isimler, dp.hesapPlanKod, "TASHESAPPLAN");
                            dp.olcuBirimKod = VeriErisim.DegerAl(dr, hesapSutun + 1, 0);
                            dp.olcuBirimAd = VeriErisim.DegerAl(dr, hesapSutun + 2, string.Empty);
                        }
                    }

                    bilgi.Add(dp);
                }

                if (depoDurumIslem == "giren")
                {
                    dp.girenMiktar += VeriErisim.DegerAl(dr, hesapSutun + rakamSutun + 6, (decimal)0);
                    dp.girenTutar += VeriErisim.DegerAl(dr, hesapSutun + rakamSutun + 7, (decimal)0);
                }
                else if (depoDurumIslem == "cikan")
                {
                    dp.cikanMiktar += VeriErisim.DegerAl(dr, hesapSutun + rakamSutun + 6, (decimal)0);
                    dp.cikanTutar += VeriErisim.DegerAl(dr, hesapSutun + rakamSutun + 7, (decimal)0);
                }
                else if (depoDurumIslem == "zimmetKisi")
                {
                    dp.zimmetKisiMiktar += VeriErisim.DegerAl(dr, hesapSutun + rakamSutun + 6, (decimal)0);
                    dp.zimmetKisiTutar += VeriErisim.DegerAl(dr, hesapSutun + rakamSutun + 7, (decimal)0);
                }
                else if (depoDurumIslem == "zimmetOrtak")
                {
                    dp.zimmetOrtakMiktar += VeriErisim.DegerAl(dr, hesapSutun + rakamSutun + 6, (decimal)0);
                    dp.zimmetOrtakTutar += VeriErisim.DegerAl(dr, hesapSutun + rakamSutun + 7, (decimal)0);
                }

                dp.kalanMiktar = dp.girenMiktar - dp.cikanMiktar - (dp.zimmetKisiMiktar + dp.zimmetOrtakMiktar);
                dp.kalanTutar = dp.girenTutar - dp.cikanTutar - (dp.zimmetKisiTutar + dp.zimmetOrtakTutar);
            }
            if (dr != null) dr.Close();
        }

        /// <summary>
        /// Verilen TuketimCikis kriterlerine uygun olan tüketim malzemelerinin
        /// dönemsel kullanım miktarları bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">TuketimCikis listeleme kriterleri</param>
        /// <param name="fark">Tüketim fark raporu mu, yoksa tüketim raporu mu hazırlansın bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında tüketim malzemelerinin dönemsel kullanım bilgileri listesi döndürülür.</returns>
        public static ObjectArray TuketimCikis(VeriErisim veriErisim, Kullanici kullanan, TuketimCikis kriter, bool fark)
        {
            ObjectArray donusDeger = new ObjectArray();

            donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, kriter.muhasebeKod, kriter.harcamaKod, kriter.ambarKod);
            if (!donusDeger.sonuc.islemSonuc)
            {
                donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
                return donusDeger;
            }

            string hata = string.Empty;
            if (kriter.yil <= 0)
                hata += TasinirMalResI.TMM036 + "<br>";
            if (string.IsNullOrEmpty(kriter.muhasebeKod))
                hata += TasinirMalResI.TMM142 + "<br>";
            if (string.IsNullOrEmpty(kriter.harcamaKod))
                hata += TasinirMalResI.TMM014 + "<br>";
            //Erdal
            //if (string.IsNullOrEmpty(kriter.ambarKod))
            //    hata += TasinirMalResI.TMM031 + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = hata;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tabRapor = new TABRaporlar(ve);
            IDataReader dr = null;

            try
            {
                if (fark && kriter.tarih1.isNull && kriter.tarih2.isNull)
                {
                    TNSDateTime[,] donemTarihleri = new TNSDateTime[4, 2];
                    donemTarihleri[0, 0] = new TNSDateTime("01.01." + kriter.yil.ToString());
                    donemTarihleri[0, 1] = new TNSDateTime("31.03." + kriter.yil.ToString());
                    donemTarihleri[1, 0] = new TNSDateTime("01.04." + kriter.yil.ToString());
                    donemTarihleri[1, 1] = new TNSDateTime("30.06." + kriter.yil.ToString());
                    donemTarihleri[2, 0] = new TNSDateTime("01.07." + kriter.yil.ToString());
                    donemTarihleri[2, 1] = new TNSDateTime("30.09." + kriter.yil.ToString());
                    donemTarihleri[3, 0] = new TNSDateTime("01.10." + kriter.yil.ToString());
                    donemTarihleri[3, 1] = new TNSDateTime("31.12." + kriter.yil.ToString());

                    for (int i = 0; i < donemTarihleri.Length / 2; i++)
                    {
                        kriter.tarih1 = donemTarihleri[i, 0];
                        kriter.tarih2 = donemTarihleri[i, 1];

                        dr = tabRapor.TuketimCikis(kriter, Yetki.YetkiTipiDegerleriDondur(kullanan), fark);
                        if (dr != null)
                            TuketimListeBilgileriAl(dr, donusDeger.objeler, kriter, i + 1);
                    }
                }
                else
                {
                    dr = tabRapor.TuketimCikis(kriter, Yetki.YetkiTipiDegerleriDondur(kullanan), fark);
                    if (dr != null)
                        TuketimListeBilgileriAl(dr, donusDeger.objeler, kriter, 1);
                }
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                if (dr != null) dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            if (dr != null) dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen DataReader nesnesini dolaşarak tüketim
        /// malzemelerinin dönemsel kullanım miktarları bilgilerini toplayan ve varsa
        /// bilgi listesindeki nesneye, yoksa yeni yarattığı nesneye ekleyen yordam
        /// </summary>
        /// <param name="dr">DataReader nesnesi</param>
        /// <param name="bilgi">Daha önce toplanan bilgileri tutan nesne</param>
        /// <param name="kriter">TuketimCikis listeleme kriterleri</param>
        /// <param name="donem">Dönem bilgisi</param>
        private static void TuketimListeBilgileriAl(IDataReader dr, TNSCollection bilgi, TuketimCikis kriter, int donem)
        {
            TuketimCikis tk = null;
            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                if (bilgi.Count <= 0)
                {
                    tk = new TuketimCikis();
                    tk.yil = VeriErisim.DegerAl(dr, 0, 0);
                    tk.ilKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                    tk.ilAd = VeriErisim.DegerAl(dr, 2, string.Empty);
                    tk.ilceKod = VeriErisim.DegerAl(dr, 3, string.Empty);
                    tk.ilceAd = VeriErisim.DegerAl(dr, 4, string.Empty);
                    tk.muhasebeKod = VeriErisim.DegerAl(dr, 5, string.Empty);
                    tk.muhasebeAd = VeriErisim.DegerAl(dr, 6, string.Empty);
                    tk.harcamaKod = VeriErisim.DegerAl(dr, 7, string.Empty);
                    tk.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), tk.harcamaKod);
                    tk.harcamaAd = VeriErisim.DegerAl(dr, 8, string.Empty);

                    tk.ambarKod = kriter.ambarKod;
                    if (!string.IsNullOrEmpty(tk.ambarKod))
                        tk.ambarAd = Genel.AdGetir(isimler, tk.muhasebeKod + "-" + tk.harcamaKod + "-" + tk.ambarKod, "TASAMBAR");
                    else
                    {
                        tk.ambarKod = "Genel";
                        tk.ambarAd = "Genel";
                    }

                    bilgi.Add(tk);
                }
                else
                    tk = (TuketimCikis)bilgi[0];

                string hesapKod = VeriErisim.DegerAl(dr, 9, string.Empty);
                hesapKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), hesapKod);
                int olcuBirimKod = VeriErisim.DegerAl(dr, 10, 0);

                TuketimCikisDetay detay;
                int index = DetayIndexDondur(hesapKod, olcuBirimKod, tk.detay);
                if (index != -1)
                    detay = (TuketimCikisDetay)tk.detay[index];
                else
                {
                    detay = new TuketimCikisDetay();
                    tk.detay.Add(detay);
                }

                detay.hesapPlanKod = hesapKod;
                if (string.IsNullOrEmpty(detay.hesapPlanAd))
                    detay.hesapPlanAd = Genel.AdGetir(isimler, detay.hesapPlanKod, "TASHESAPPLAN");

                detay.olcuBirimKod = olcuBirimKod;
                detay.olcuBirimAd = VeriErisim.DegerAl(dr, 11, string.Empty);
                //detay.olcuBirimAd = Genel.OlcuBirimAdGetir(detay.hesapPlanKod);
                detay.miktar = VeriErisim.DegerAl(dr, 12, (decimal)0);
                detay.tutar[donem - 1] = VeriErisim.DegerAl(dr, 13, (decimal)0);
            }
            if (dr != null) dr.Close();
        }

        /// <summary>
        /// Verilen kriterlere uygun olan ortak alan zimmet bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Zimmet listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ortak alan zimmet bilgileri listesi döndürülür.</returns>
        public static ObjectArray ZimmetOrtakAlan(VeriErisim veriErisim, Kullanici kullanan, ZimmetOrtakAlanVeKisi kriter)
        {
            return ZimmetOrtakAlanVeKisi(veriErisim, kullanan, kriter, true, true);
        }

        /// <summary>
        /// Verilen kriterlere uygun olan kişi zimmet bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Zimmet listeleme kriterleri</param>
        /// <param name="kontrolYap">Yetki kontrolü yapılsın mı yapılmasın mı bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında kişi zimmet bilgileri listesi döndürülür.</returns>
        public static ObjectArray ZimmetKisi(VeriErisim veriErisim, Kullanici kullanan, ZimmetOrtakAlanVeKisi kriter, bool kontrolYap)
        {
            //melih ortakalan true olmasının sebebi kişi üstünde ortak alan zimmet varsa gelsin diye
            //erdal ortakalan false ama kişi üstünde ortak alan zimmet varsa gelecek
            return ZimmetOrtakAlanVeKisi(veriErisim, kullanan, kriter, false, kontrolYap);
        }

        /// <summary>
        /// Verilen kriterlere uygun olan kişi zimmet veya ortak alan zimmet bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Zimmet listeleme kriterleri</param>
        /// <param name="ortakAlan">Ortak alan zimmet mi değil mi bilgisi</param>
        /// <param name="kontrolYap">Yetki kontrolü yapılsın mı yapılmasın mı bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında kişi zimmet veya ortak alan zimmet bilgileri listesi döndürülür.</returns>
        private static ObjectArray ZimmetOrtakAlanVeKisi(VeriErisim veriErisim, Kullanici kullanan, ZimmetOrtakAlanVeKisi kriter, bool ortakAlan, bool kontrolYap)
        {
            ObjectArray donusDeger = new ObjectArray();

            if (kontrolYap)
            {
                donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMiHarcamaBirimi(kullanan, kriter.muhasebeKod, kriter.harcamaKod);
                if (!donusDeger.sonuc.islemSonuc)
                {
                    donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
                    return donusDeger;
                }

                string hata = string.Empty;
                if (kriter.yil <= 0)
                    hata += TasinirMalResI.TMM036 + "<br>";
                //if (string.IsNullOrEmpty(kriter.muhasebeKod))
                //    hata += TasinirMalResI.TMM142 + "<br>";
                //if (string.IsNullOrEmpty(kriter.harcamaKod))
                //    hata += TasinirMalResI.TMM014 + "<br>";
                if (ortakAlan)
                {
                    ////melih tckimlik eklenmesinin sebebi kişi üstünde ortak alan zimmet varsa gelsin diye
                    //if (string.IsNullOrEmpty(kriter.odaKod) && string.IsNullOrEmpty(kriter.tcKimlikNo))
                    //    hata += TasinirMalResI.TMM193 + "<br>";

                    //erdal commentli yere gerek kalmadı
                    if (string.IsNullOrEmpty(kriter.muhasebeKod))
                        hata += TasinirMalResI.TMM142 + "<br>";
                    if (string.IsNullOrEmpty(kriter.harcamaKod))
                        hata += TasinirMalResI.TMM014 + "<br>";
                    if (string.IsNullOrEmpty(kriter.odaKod))
                        hata += TasinirMalResI.TMM193 + "<br>";
                }
                else
                {
                    if (string.IsNullOrEmpty(kriter.tcKimlikNo))
                        hata += TasinirMalResI.TMM194 + "<br>";
                }

                if (!string.IsNullOrEmpty(hata))
                {
                    donusDeger.sonuc.islemSonuc = false;
                    donusDeger.sonuc.hataStr = hata;
                    return donusDeger;
                }
            }
            else if (string.IsNullOrEmpty(kriter.tcKimlikNo) && string.IsNullOrEmpty(kriter.sicilNo))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = TasinirMalResI.TMM195;
                return donusDeger;

            }

            //Bazı sorgularda sadece odaya göre yapılması gerekiyor
            if (kriter.tcKimlikNo == "||") kriter.tcKimlikNo = "";

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tabRapor = new TABRaporlar(ve);
            IDataReader dr = null;

            try
            {
                dr = tabRapor.ZimmetOrtakAlanVeKisi(kriter, Yetki.YetkiTipiDegerleriDondur(kullanan));
                if (dr != null)
                    ZimmetOrtakAlanVeKisiBilgileriAl(dr, donusDeger.objeler);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                if (dr != null) dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;


            if (dr != null)
                if (dr != null) dr.Close();

            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen DataReader nesnesini dolaşarak kişi zimmet
        /// veya ortak alan zimmet bilgilerini toplayan ve varsa bilgi
        /// listesindeki nesneye, yoksa yeni yarattığı nesneye ekleyen yordam
        /// </summary>
        /// <param name="dr">DataReader nesnesi</param>
        /// <param name="bilgi">Daha önce toplanan bilgileri tutan nesne</param>
        private static void ZimmetOrtakAlanVeKisiBilgileriAl(IDataReader dr, TNSCollection bilgi)
        {
            ZimmetOrtakAlanVeKisi zoa = new ZimmetOrtakAlanVeKisi();

            while (dr.Read())
            {
                if (bilgi.Count <= 0)
                {
                    zoa = new ZimmetOrtakAlanVeKisi();
                    zoa.yil = VeriErisim.DegerAl(dr, 0, 0);
                    zoa.muhasebeKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                    zoa.muhasebeAd = VeriErisim.DegerAl(dr, 2, string.Empty);
                    zoa.harcamaKod = VeriErisim.DegerAl(dr, 3, string.Empty);
                    zoa.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), zoa.harcamaKod);
                    zoa.harcamaAd = VeriErisim.DegerAl(dr, 4, string.Empty);
                    zoa.odaKod = VeriErisim.DegerAl(dr, 5, string.Empty);
                    zoa.odaAd = VeriErisim.DegerAl(dr, 6, string.Empty);
                    zoa.tcKimlikNo = VeriErisim.DegerAl(dr, 12, string.Empty);
                    zoa.kisiAd = VeriErisim.DegerAl(dr, 13, string.Empty);

                    bilgi.Add(zoa);
                }
                else
                    zoa = (ZimmetOrtakAlanVeKisi)bilgi[0];

                ZimmetOrtakAlanVeKisiDetay detay = new ZimmetOrtakAlanVeKisiDetay();

                detay.odaKod = VeriErisim.DegerAl(dr, 5, string.Empty);
                detay.odaAd = VeriErisim.DegerAl(dr, 6, string.Empty);
                detay.prSicilNo = VeriErisim.DegerAl(dr, 7, 0);
                detay.gorSicilNo = VeriErisim.DegerAl(dr, 8, string.Empty);
                detay.sicilAd = VeriErisim.DegerAl(dr, 9, string.Empty);
                detay.fisNo = VeriErisim.DegerAl(dr, 10, string.Empty);
                detay.fisTarih = VeriErisim.DegerAl(dr, 11);
                detay.sorumluKisiTC = VeriErisim.DegerAl(dr, 12, string.Empty);
                detay.sorumluKisiAd = VeriErisim.DegerAl(dr, 13, string.Empty);
                detay.aciklama = VeriErisim.DegerAl(dr, 14, string.Empty);
                detay.ambarKod = VeriErisim.DegerAl(dr, 15, string.Empty);
                detay.ambarAd = VeriErisim.DegerAl(dr, 16, string.Empty);
                detay.saseNo = VeriErisim.DegerAl(dr, 17, string.Empty);
                detay.harcamaKod = VeriErisim.DegerAl(dr, 3, string.Empty);
                detay.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), detay.harcamaKod);
                detay.harcamaAd = VeriErisim.DegerAl(dr, 4, string.Empty);
                detay.muhasebeKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                detay.muhasebeAd = VeriErisim.DegerAl(dr, 2, string.Empty);

                if (Genel.rfIdVarMi)
                {
                    detay.ambarKod = VeriErisim.DegerAl(dr, 5, string.Empty);
                    detay.ambarAd = VeriErisim.DegerAl(dr, 6, string.Empty);
                    detay.rfIdNo = Genel.PrSicilNoRFIDDondur(detay.prSicilNo);
                }
                zoa.detay.Add(detay);
            }
            if (dr != null) dr.Close();
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır işlem bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi kriter bilgilerini tutan nesne</param>
        /// <param name="tfKriter">Ek kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır işlem bilgileri listesi döndürülür.</returns>
        public static ObjectArray TasinirIslemMalzemeTarihceRaporu(VeriErisim veriErisim, Kullanici kullanan, TasinirIslemForm tf, TasinirFormKriter tfKriter)
        {
            ObjectArray bilgiVAN = new ObjectArray();

            bilgiVAN.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod);
            if (!bilgiVAN.sonuc.islemSonuc)
            {
                bilgiVAN.sonuc.hataStr = TasinirMalResI.TMM062;
                return bilgiVAN;
            }

            if (tf.yil <= 0)
                bilgiVAN.sonuc.hataStr += TasinirMalResI.TMM036 + "<br>";

            //Taşınır Hurda Raporu için Muhasebe kodu boş brakılabilir
            if (tf.muhasebeKod == "empty")
                tf.muhasebeKod = string.Empty;
            else if (string.IsNullOrEmpty(tf.muhasebeKod))
                bilgiVAN.sonuc.hataStr += TasinirMalResI.TMM013 + "<br>";

            if (!string.IsNullOrEmpty(bilgiVAN.sonuc.hataStr))
            {
                bilgiVAN.sonuc.islemSonuc = false;
                return bilgiVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.sureAsimi = 60 * 10; //10 dakika

            TABRaporlar tRapor = new TABRaporlar(ve);

            TasinirIslemForm gelen = null;
            IDataReader dr = tRapor.TasinirIslemMalzemeTarihceRaporu(tf, tfKriter, Yetki.YetkiTipiDegerleriDondur(kullanan));
            //Uzay ile çekilen bilgiler işlemi çok yavaşlatıyor. Yıl sonu kontrolleri sırasında EGO da kullandım. Yedek parça 1501303 30dk gibi bir sürede tamamlandı. 100.000 kayıt vardı.
            //Uzayları kapatıp hesapadı ve işlemadı alanlarını sql kısmına koydum
            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                if (gelen == null ||
                    gelen.yil != VeriErisim.DegerAl(dr, 0, 0) ||
                    gelen.muhasebeKod != VeriErisim.DegerAl(dr, 1, string.Empty) ||
                    gelen.harcamaKod != OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), VeriErisim.DegerAl(dr, 2, string.Empty)) ||
                    gelen.fisNo != VeriErisim.DegerAl(dr, 3, string.Empty))
                {
                    gelen = new TasinirIslemForm();
                    gelen.yil = VeriErisim.DegerAl(dr, 0, 0);
                    gelen.muhasebeKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                    gelen.muhasebeAd = Genel.AdGetir(isimler, gelen.muhasebeKod, "TASMUHASEBE");
                    gelen.harcamaKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                    gelen.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), gelen.harcamaKod);
                    gelen.harcamaAd = Genel.AdGetir(isimler, gelen.muhasebeKod + "-" + gelen.harcamaKod, "TASHARCAMABIRIM");
                    gelen.fisNo = VeriErisim.DegerAl(dr, 3, string.Empty);
                    gelen.fisTarih = VeriErisim.DegerAl(dr, 4);
                    gelen.ambarKod = VeriErisim.DegerAl(dr, 5, string.Empty);
                    gelen.ambarAd = Genel.AdGetir(isimler, gelen.muhasebeKod + "-" + gelen.harcamaKod + "-" + gelen.ambarKod, "TASAMBAR");
                    gelen.islemTipKod = VeriErisim.DegerAl(dr, 6, 0);
                    gelen.islemTipAd = VeriErisim.DegerAl(dr, 40, string.Empty);
                    gelen.islemTipTur = VeriErisim.DegerAl(dr, 39, 0);
                    gelen.muayeneNo = VeriErisim.DegerAl(dr, 7, string.Empty);
                    gelen.muayeneTarih = VeriErisim.DegerAl(dr, 8);
                    gelen.dayanakNo = VeriErisim.DegerAl(dr, 9, string.Empty);
                    gelen.dayanakTarih = VeriErisim.DegerAl(dr, 10);
                    gelen.neredenGeldi = VeriErisim.DegerAl(dr, 11, string.Empty);
                    gelen.nereyeGitti = VeriErisim.DegerAl(dr, 12, string.Empty);
                    gelen.gMuhasebeKod = VeriErisim.DegerAl(dr, 13, string.Empty);
                    gelen.gMuhasebeAd = Genel.AdGetir(isimler, gelen.gMuhasebeKod, "TASMUHASEBE");
                    gelen.gHarcamaKod = VeriErisim.DegerAl(dr, 14, string.Empty);
                    gelen.gHarcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), gelen.gHarcamaKod);
                    gelen.gHarcamaAd = Genel.AdGetir(isimler, gelen.gMuhasebeKod + "-" + gelen.gHarcamaKod, "TASHARCAMABIRIM");
                    gelen.gAmbarKod = VeriErisim.DegerAl(dr, 15, string.Empty);
                    gelen.gAmbarAd = Genel.AdGetir(isimler, gelen.gMuhasebeKod + "-" + gelen.gHarcamaKod + "-" + gelen.gAmbarKod, "TASAMBAR");
                    gelen.durum = VeriErisim.DegerAl(dr, 16, 0);
                    gelen.islemTarih = VeriErisim.DegerAl(dr, 17);
                    gelen.islemYapan = VeriErisim.DegerAl(dr, 18, string.Empty);
                    gelen.faturaNo = VeriErisim.DegerAl(dr, 19, string.Empty);
                    gelen.faturaTarih = VeriErisim.DegerAl(dr, 20);
                    gelen.kimeGitti = VeriErisim.DegerAl(dr, 21, string.Empty);
                    gelen.parselNo = VeriErisim.DegerAl(dr, 41, string.Empty);
                    gelen.aciklama = VeriErisim.DegerAl(dr, 42, string.Empty);

                    bilgiVAN.objeler.Add(gelen);
                }

                TasinirIslemDetay detay = new TasinirIslemDetay();
                detay.yil = gelen.yil;
                detay.muhasebeKod = gelen.muhasebeKod;
                detay.harcamaKod = gelen.harcamaKod;
                detay.fisNo = gelen.fisNo;
                detay.siraNo = VeriErisim.DegerAl(dr, 26, 0);
                detay.hesapPlanKod = VeriErisim.DegerAl(dr, 27, string.Empty);
                detay.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), detay.hesapPlanKod);
                detay.hesapPlanAd = VeriErisim.DegerAl(dr, 38, string.Empty);
                detay.sicilNo = VeriErisim.DegerAl(dr, 28, 0);
                detay.gorSicilNo = Genel.AdGetir(isimler, detay.muhasebeKod + "-" + detay.harcamaKod.Replace(".", "") + "-" + detay.sicilNo.ToString(), "TASSICILGETIR");
                detay.olcuBirimAd = Genel.AdGetir(isimler, detay.hesapPlanKod.Replace(".", ""), "TASOLCUBIRIMAD");
                detay.miktar = VeriErisim.DegerAl(dr, 29, (decimal)0);
                detay.kdvOran = VeriErisim.DegerAl(dr, 30, 0);
                detay.birimFiyat = VeriErisim.DegerAl(dr, 31, (decimal)0);
                detay.birimFiyatKDVLi = VeriErisim.DegerAl(dr, 32, (decimal)0);
                detay.eSicilNo = VeriErisim.DegerAl(dr, 33, string.Empty);
                detay.eAlimTarihi = VeriErisim.DegerAl(dr, 34);
                detay.eTedarikSekli = VeriErisim.DegerAl(dr, 35, string.Empty);
                detay.eAciklama = VeriErisim.DegerAl(dr, 36, string.Empty);
                detay.sonTarih = VeriErisim.DegerAl(dr, 37);

                gelen.detay.objeler.Add(detay);
            }
            if (dr != null) dr.Close();

            if (bilgiVAN.sonuc.islemSonuc && bilgiVAN.objeler.Count == 0)
                bilgiVAN.sonuc.bilgiStr = TasinirMalResI.TMM029;

            ve.VeriErisimKapat();
            return bilgiVAN;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan demirbaş bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Demirbaş listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaş bilgileri listesi döndürülür.</returns>
        public static ObjectArray SicilRaporu(VeriErisim veriErisim, Kullanici kullanan, SicilNoHareket kriter)
        {
            ObjectArray bilgiVAN = new ObjectArray();

            string kodYapisi = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD13/KODYAPI");

            bilgiVAN.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, kriter.muhasebeKod, kriter.harcamaBirimKod, kriter.ambarKod);
            if (!bilgiVAN.sonuc.islemSonuc)
            {
                bilgiVAN.sonuc.hataStr = TasinirMalResI.TMM062;
                return bilgiVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tRapor = new TABRaporlar(ve);

            StreamWriter writeFile = null;
            if (!string.IsNullOrWhiteSpace(kriter.listeYazdir))
            {
                if (File.Exists(kriter.listeYazdir))
                    File.Delete(kriter.listeYazdir);

                writeFile = new StreamWriter(kriter.listeYazdir, false);
            }

            IDataReader dr;
            if (Genel.tasinirSicilNoRFIDFarkli)
                dr = tRapor.SicilRaporuRFID(kriter, Yetki.YetkiTipiDegerleriDondur(kullanan));
            else
                dr = tRapor.SicilRaporu(kriter, Yetki.YetkiTipiDegerleriDondur(kullanan));

            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                SicilNoHareket gelen = new SicilNoHareket();
                gelen.prSicilNo = VeriErisim.DegerAl(dr, 0, 0);
                gelen.sicilNo = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.hesapPlanAd = VeriErisim.DegerAl(dr, 2, string.Empty);
                gelen.kdvOran = VeriErisim.DegerAl(dr, 3, 0);

                try
                {
                    gelen.kdvliBirimFiyat = VeriErisim.DegerAl(dr, 4, (decimal)0);
                    gelen.fiyat = VeriErisim.DegerAl(dr, 5, (decimal)0);
                }
                catch (Exception)
                {
                    OrtakFonksiyonlar.HataStrYaz("Birim fiyat okuma hatası, prsicilno:" + gelen.prSicilNo);
                }

                gelen.islemTipKod = VeriErisim.DegerAl(dr, 6, 0);

                if (!Genel.tasinirSicilNoRFIDFarkli)
                    gelen.islemTipiAd = Genel.AdGetir(isimler, gelen.islemTipKod.ToString(), "TASISLEMTIPAD");

                gelen.islemTurKod = VeriErisim.DegerAl(dr, 7, 0);
                gelen.islemTarih = VeriErisim.DegerAl(dr, 8);
                gelen.muhasebeKod = VeriErisim.DegerAl(dr, 9, string.Empty);
                gelen.muhasebeAd = VeriErisim.DegerAl(dr, 10, string.Empty);
                gelen.harcamaBirimKod = VeriErisim.DegerAl(dr, 11, string.Empty);

                if (!Genel.tasinirSicilNoRFIDFarkli)
                    gelen.harcamaBirimKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), gelen.harcamaBirimKod);

                gelen.harcamaBirimAd = VeriErisim.DegerAl(dr, 12, string.Empty);
                gelen.ambarKod = VeriErisim.DegerAl(dr, 13, string.Empty);
                gelen.ambarAd = VeriErisim.DegerAl(dr, 14, string.Empty);
                gelen.personelKod = VeriErisim.DegerAl(dr, 15, string.Empty);
                gelen.kisiAd = VeriErisim.DegerAl(dr, 16, string.Empty);
                gelen.odaKod = VeriErisim.DegerAl(dr, 17, string.Empty);
                gelen.odaAd = VeriErisim.DegerAl(dr, 18, string.Empty);
                gelen.zimmetOzellik = VeriErisim.DegerAl(dr, 19, string.Empty);
                gelen.ozellik.markaAd = VeriErisim.DegerAl(dr, 20, string.Empty);
                gelen.ozellik.modelAd = VeriErisim.DegerAl(dr, 21, string.Empty);
                gelen.ozellik.saseNo = VeriErisim.DegerAl(dr, 22, string.Empty);
                gelen.ozellik.motorNo = VeriErisim.DegerAl(dr, 23, string.Empty);
                gelen.ozellik.disSicilNo = VeriErisim.DegerAl(dr, 24, string.Empty);

                if (!Genel.tasinirSicilNoRFIDFarkli)
                    gelen.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisi, VeriErisim.DegerAl(dr, 25, string.Empty));

                gelen.ozellik.plaka = VeriErisim.DegerAl(dr, 26, string.Empty);
                gelen.ozellik.yeriKonusu = VeriErisim.DegerAl(dr, 27, string.Empty);

                gelen.ilkGirisIslemTip.kod = VeriErisim.DegerAl(dr, 41, 0);

                if (!Genel.tasinirSicilNoRFIDFarkli)
                    gelen.ilkGirisIslemTip.ad = Genel.AdGetir(isimler, gelen.ilkGirisIslemTip.kod.ToString(), "TASISLEMTIPAD");

                gelen.ilkGirisTarih = VeriErisim.DegerAl(dr, 43);

                if (Genel.tasinirSicilNoRFIDFarkli)
                {
                    gelen.rfIdNo = VeriErisim.DegerAl(dr, 56, 0);
                    gelen.islemTipiAd = VeriErisim.DegerAl(dr, 57, "");
                    gelen.ilkGirisIslemTip.ad = VeriErisim.DegerAl(dr, 58, "");
                }
                else
                    gelen.amortismanYil = VeriErisim.DegerAl(dr, 70, 0);

                bilgiVAN.objeler.Add(gelen);

                if (!string.IsNullOrWhiteSpace(kriter.listeYazdir))
                    ListeYazdir(kriter.listeYazdir, gelen, writeFile);

            }

            if (dr != null) dr.Close();

            if (bilgiVAN.sonuc.islemSonuc && bilgiVAN.objeler.Count == 0)
                bilgiVAN.sonuc.bilgiStr = TasinirMalResI.TMM029;


            if (!string.IsNullOrWhiteSpace(kriter.listeYazdir))
            {
                bilgiVAN = new ObjectArray();

                writeFile.Close();
            }


            ve.VeriErisimKapat();
            return bilgiVAN;
        }

        private static void ListeYazdir(string listeYazdir, SicilNoHareket snh, StreamWriter writeFile)
        {
            string bilgi = "";

            bilgi += snh.sicilNo;
            bilgi += "|" + snh.rfIdNo;
            bilgi += "|" + snh.amortismanYil;//2
            bilgi += "|" + snh.hesapPlanAd;
            bilgi += "|" + snh.kdvOran;
            bilgi += "|" + snh.fiyat;//5
            bilgi += "|" + snh.kdvliBirimFiyat;
            bilgi += "|" + snh.ilkGirisIslemTip.ad;
            bilgi += "|" + snh.fisNo;
            bilgi += "|" + snh.ilkGirisTarih.ToString();//9
            bilgi += "|" + snh.islemTipiAd;
            bilgi += "|" + snh.islemTurKod;
            bilgi += "|" + snh.ozellik.saseNo;//12
            bilgi += "|" + snh.ozellik.yeriKonusu;
            bilgi += "|" + snh.islemTarih.ToString();
            bilgi += "|" + snh.muhasebeAd;
            bilgi += "|" + snh.harcamaBirimAd;
            bilgi += "|" + snh.ambarAd;
            bilgi += "|" + snh.kisiAd;
            bilgi += "|" + snh.odaKod;//19
            bilgi += "|" + snh.odaAd;
            bilgi += "|" + snh.zimmetOzellik;
            bilgi += "|" + snh.ozellik.markaAd;
            bilgi += "|" + snh.ozellik.modelAd;
            bilgi += "|" + snh.ozellik.motorNo;
            bilgi += "|" + snh.ozellik.plaka;
            bilgi += "|" + snh.ozellik.disSicilNo;

            bilgi = bilgi.Replace("\n", "");
            bilgi = bilgi.Replace("\r", "");
            //OrtakFonksiyonlar.DosyayaYaz(listeYazdir, bilgi, false);
            writeFile.WriteLine(bilgi);
        }


        /// <summary>
        /// Parametre olarak verilen cetvel kriterlerine uygun olan değer artışlarını
        /// bulan ve tip parametresine bakarak verilen cetveller nesne listesindeki
        /// ilgili nesneye bulduğu değer artışı bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Değer artışı kriterlerini tutan nesne</param>
        /// <param name="cetveller">Daha önce toplanan bilgileri tutan nesne listesi</param>
        /// <param name="tip">DataReader nesnesinden toplanacak cetvel tipi</param>
        private static void CetvelDegerArtislari(VeriErisim veriErisim, Kullanici kullanan, Cetvel kriter, TNSCollection cetveller, ENUMCetvelTipi tip)
        {
            CetvelDegerArtislari(veriErisim, kullanan, kriter, cetveller, tip, false);
        }

        private static void CetvelDegerArtislari(VeriErisim veriErisim, Kullanici kullanan, Cetvel kriter, TNSCollection cetveller, ENUMCetvelTipi tip, bool muhasebeEkle)
        {
            int hesapKodUzunluk = 5;
            if (kriter.seviye == 1) hesapKodUzunluk = 3;
            else if (kriter.seviye == 2) hesapKodUzunluk = 5;
            else if (kriter.seviye == 3) hesapKodUzunluk = 7;

            TABRaporlar tRapor = new TABRaporlar(veriErisim);

            IDataReader dr = tRapor.CetvelDegerArtislari(kriter, tip, Yetki.YetkiTipiDegerleriDondur(kullanan), hesapKodUzunluk);

            Cetvel cetvel = null;
            if (tip == ENUMCetvelTipi.SAYIMCETVELI || tip == ENUMCetvelTipi.YONETIMHESABICETVELI
                || tip == ENUMCetvelTipi.KESINHESAPCETVELI || tip == ENUMCetvelTipi.KESINHESAPCETVELIICMAL
                || tip == ENUMCetvelTipi.MUZEKUTUPHANECETVELI)
                cetvel = (Cetvel)cetveller[0];

            while (dr.Read())
            {
                if (tip == ENUMCetvelTipi.HESAPCETVELI || tip == ENUMCetvelTipi.HESAPCETVELIIL || tip == ENUMCetvelTipi.HESAPCETVELIBOLGE)
                {
                    cetvel = null;
                    foreach (Cetvel c in cetveller)
                    {
                        if (tip == ENUMCetvelTipi.HESAPCETVELI)
                        {
                            if (c.ilKod == VeriErisim.DegerAl(dr, 1, string.Empty) && c.ilceKod == VeriErisim.DegerAl(dr, 2, string.Empty))
                            {
                                cetvel = c;
                                break;
                            }
                        }
                        else if (tip == ENUMCetvelTipi.HESAPCETVELIIL)
                        {
                            if (c.ilKod == VeriErisim.DegerAl(dr, 1, string.Empty))
                            {
                                cetvel = c;
                                break;
                            }
                        }
                        else
                        {
                            if (c.bolgeKod == VeriErisim.DegerAl(dr, 1, 0))
                            {
                                cetvel = c;
                                break;
                            }
                        }
                    }
                }

                //Cetveli bulmuş olması lazım
                if (cetvel != null)
                {
                    string hesapKod = string.Empty;
                    string muhasebeKod = string.Empty;
                    int olcuBirimKod = 0;
                    decimal degerArtisi = (decimal)0;
                    if (tip == ENUMCetvelTipi.SAYIMCETVELI || tip == ENUMCetvelTipi.YONETIMHESABICETVELI)
                    {
                        hesapKod = VeriErisim.DegerAl(dr, 5, string.Empty);
                        olcuBirimKod = VeriErisim.DegerAl(dr, 6, 0);
                        degerArtisi = VeriErisim.DegerAl(dr, 7, (decimal)0);
                    }
                    else if (tip == ENUMCetvelTipi.HESAPCETVELI)
                    {
                        hesapKod = VeriErisim.DegerAl(dr, 3, string.Empty);
                        olcuBirimKod = VeriErisim.DegerAl(dr, 4, 0);
                        degerArtisi = VeriErisim.DegerAl(dr, 5, (decimal)0);
                    }
                    else if (tip == ENUMCetvelTipi.HESAPCETVELIIL)
                    {
                        hesapKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                        olcuBirimKod = VeriErisim.DegerAl(dr, 3, 0);
                        degerArtisi = VeriErisim.DegerAl(dr, 4, (decimal)0);
                    }
                    else if (tip == ENUMCetvelTipi.KESINHESAPCETVELI)
                    {
                        hesapKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                        olcuBirimKod = VeriErisim.DegerAl(dr, 2, 0);
                        muhasebeKod = VeriErisim.DegerAl(dr, 3, string.Empty);
                        degerArtisi = VeriErisim.DegerAl(dr, 4, (decimal)0);
                    }
                    else if (tip == ENUMCetvelTipi.KESINHESAPCETVELIICMAL)
                    {
                        hesapKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                        muhasebeKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                        degerArtisi = VeriErisim.DegerAl(dr, 3, (decimal)0);
                    }
                    else if (tip == ENUMCetvelTipi.MUZEKUTUPHANECETVELI)
                    {
                        hesapKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                        olcuBirimKod = VeriErisim.DegerAl(dr, 2, 0);
                        degerArtisi = VeriErisim.DegerAl(dr, 3, (decimal)0);
                    }
                    else if (tip == ENUMCetvelTipi.HESAPCETVELIBOLGE)
                    {
                        hesapKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                        olcuBirimKod = VeriErisim.DegerAl(dr, 3, 0);
                        degerArtisi = VeriErisim.DegerAl(dr, 4, (decimal)0);
                    }

                    hesapKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), hesapKod);

                    int index;
                    if (muhasebeEkle)
                        index = DetayIndexDondur(muhasebeKod, hesapKod, olcuBirimKod, cetvel.detay);
                    else
                        index = DetayIndexDondur(hesapKod, olcuBirimKod, cetvel.detay);
                    //Detayı bulmuş olması lazım
                    if (index != -1)
                    {
                        CetvelDetay detay = (CetvelDetay)cetvel.detay[index];

                        int yil = VeriErisim.DegerAl(dr, 0, 0);
                        if (yil < kriter.yil)
                            detay.gecenYildanDevreden.tutar += degerArtisi;
                        else if (yil == kriter.yil)
                            detay.yilIcindeGiren.tutar += degerArtisi;

                        detay.toplam.tutar = detay.gecenYildanDevreden.tutar + detay.yilIcindeGiren.tutar;
                        detay.gelecekYilaDevir.tutar = detay.toplam.tutar - detay.yilIcindeCikan.tutar;
                    }
                }
            }
            if (dr != null) dr.Close();
        }

        /// <summary>
        /// Verilen kriterlere uygun olan girişi yapılmamış devir çıkış belgeleri bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında girişi yapılmamış devir çıkış belgeleri listesi döndürülür.</returns>
        public static ObjectArray GirisiYapilmamisDevirCikislari(VeriErisim veriErisim, Kullanici kullanan, TasinirIslemForm kriter)
        {
            ObjectArray bilgiVAN = new ObjectArray();

            bilgiVAN.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, kriter.muhasebeKod, kriter.harcamaKod, kriter.ambarKod);

            if (!bilgiVAN.sonuc.islemSonuc)
                bilgiVAN.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMiKullaniciBirimi(kullanan, kriter.muhasebeKod, kriter.harcamaKod, kriter.ambarKod);

            if (!bilgiVAN.sonuc.islemSonuc)
            {
                bilgiVAN.sonuc.hataStr = TasinirMalResI.TMM062;
                return bilgiVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tRapor = new TABRaporlar(ve);

            IDataReader dr = tRapor.GirisiYapilmamisDevirCikislari(kriter, Yetki.YetkiTipiDegerleriDondur(kullanan));
            if (dr == null)
            {
                ve.VeriErisimKapat();
                return bilgiVAN;
            }

            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                TasinirIslemForm gelen = new TasinirIslemForm();
                gelen.yil = VeriErisim.DegerAl(dr, 0, 0);

                gelen.muhasebeKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.muhasebeAd = Genel.AdGetir(isimler, gelen.muhasebeKod, "TASMUHASEBE");

                gelen.harcamaKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                gelen.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), gelen.harcamaKod);
                gelen.harcamaAd = Genel.AdGetir(isimler, gelen.muhasebeKod + "-" + gelen.harcamaKod, "TASHARCAMABIRIM");

                gelen.fisNo = VeriErisim.DegerAl(dr, 3, string.Empty);
                gelen.fisTarih = VeriErisim.DegerAl(dr, 4);

                gelen.ambarKod = VeriErisim.DegerAl(dr, 5, string.Empty);
                gelen.ambarAd = Genel.AdGetir(isimler, gelen.muhasebeKod + "-" + gelen.harcamaKod + "-" + gelen.ambarKod, "TASAMBAR");

                gelen.gMuhasebeKod = VeriErisim.DegerAl(dr, 13, string.Empty);
                gelen.gMuhasebeAd = Genel.AdGetir(isimler, gelen.gMuhasebeKod, "TASMUHASEBE");

                gelen.gHarcamaKod = VeriErisim.DegerAl(dr, 14, string.Empty);
                gelen.gHarcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), gelen.gHarcamaKod);
                gelen.gHarcamaAd = Genel.AdGetir(isimler, gelen.gMuhasebeKod + "-" + gelen.gHarcamaKod, "TASHARCAMABIRIM");

                gelen.gAmbarKod = VeriErisim.DegerAl(dr, 15, string.Empty);
                gelen.gAmbarAd = Genel.AdGetir(isimler, gelen.gMuhasebeKod + "-" + gelen.gHarcamaKod + "-" + gelen.gAmbarKod, "TASAMBAR");

                gelen.islemTipKod = VeriErisim.DegerAl(dr, 6, 0);
                gelen.islemTipAd = VeriErisim.DegerAl(dr, 22, string.Empty);
                gelen.islemTipTur = VeriErisim.DegerAl(dr, 23, 0);
                gelen.aciklama = VeriErisim.DegerAl(dr, 24, "");

                bilgiVAN.objeler.Add(gelen);
            }
            if (dr != null) dr.Close();

            if (bilgiVAN.sonuc.islemSonuc && bilgiVAN.objeler.Count == 0)
                bilgiVAN.sonuc.bilgiStr = TasinirMalResI.TMM029;

            ve.VeriErisimKapat();
            return bilgiVAN;
        }

        /// <summary>
        /// Verilen KurusFarki kriterlerine uygun olan taşınır malzemelerinin
        /// kuruş fark tutarları bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">KurusFarki listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır malzemelerinin kuruş fark tutarları listesi döndürülür.</returns>
        public static ObjectArray KurusFarklariRaporu(VeriErisim veriErisim, Kullanici kullanan, KurusFarki kriter)
        {
            ObjectArray donusDeger = new ObjectArray();

            donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, kriter.muhasebeKod, kriter.harcamaKod, kriter.ambarKod);
            if (!donusDeger.sonuc.islemSonuc)
            {
                donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
                return donusDeger;
            }

            string hata = string.Empty;
            if (kriter.yil <= 0)
                hata += TasinirMalResI.TMM036 + "<br>";
            if (string.IsNullOrEmpty(kriter.muhasebeKod))
                hata += TasinirMalResI.TMM142 + "<br>";
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                if (string.IsNullOrEmpty(kriter.harcamaKod))
                    hata += TasinirMalResI.TMM014 + "<br>";
            }
            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = hata;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tabRapor = new TABRaporlar(ve);
            IDataReader dr = null;

            try
            {
                dr = tabRapor.KurusFarklariRaporu(kriter, Yetki.YetkiTipiDegerleriDondur(kullanan));
                if (dr != null)
                    KurusFarklariBilgileriAl(dr, donusDeger.objeler, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                if (dr != null) dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            if (dr != null) dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen DataReader nesnesini dolaşarak taşınır
        /// malzemelerinin kuruş fark tutarları bilgilerini toplayan ve varsa
        /// bilgi listesindeki nesneye, yoksa yeni yarattığı nesneye ekleyen yordam
        /// </summary>
        /// <param name="dr">DataReader nesnesi</param>
        /// <param name="bilgi">Daha önce toplanan bilgileri tutan nesne</param>
        /// <param name="kriter">KurusFarki listeleme kriterleri</param>
        private static void KurusFarklariBilgileriAl(IDataReader dr, TNSCollection bilgi, KurusFarki kriter)
        {
            KurusFarki kf = null;
            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                KurusFarki yenkf = new KurusFarki();
                yenkf.yil = VeriErisim.DegerAl(dr, 0, 0);
                yenkf.ilKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                yenkf.ilAd = VeriErisim.DegerAl(dr, 2, string.Empty);
                yenkf.ilceKod = VeriErisim.DegerAl(dr, 3, string.Empty);
                yenkf.ilceAd = VeriErisim.DegerAl(dr, 4, string.Empty);
                yenkf.muhasebeKod = VeriErisim.DegerAl(dr, 5, string.Empty);
                yenkf.muhasebeAd = VeriErisim.DegerAl(dr, 6, string.Empty);
                yenkf.harcamaKod = VeriErisim.DegerAl(dr, 7, string.Empty);
                yenkf.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), yenkf.harcamaKod);
                yenkf.harcamaAd = VeriErisim.DegerAl(dr, 8, string.Empty);


                if (Farkli(kf, yenkf))
                {
                    yenkf.ambarKod = kriter.ambarKod;
                    if (!string.IsNullOrEmpty(yenkf.ambarKod))
                        yenkf.ambarAd = Genel.AdGetir(isimler, yenkf.muhasebeKod + "-" + yenkf.harcamaKod + "-" + yenkf.ambarKod, "TASAMBAR");
                    else
                    {
                        yenkf.ambarKod = "Genel";
                        yenkf.ambarAd = "Genel";
                    }
                    kf = yenkf;
                    bilgi.Add(kf);
                }

                KurusFarkiDetay detay = new KurusFarkiDetay();
                detay.fisNo = VeriErisim.DegerAl(dr, 9, string.Empty);
                detay.islemTipiTur = VeriErisim.DegerAl(dr, 10, 0);

                detay.hesapPlanKod = VeriErisim.DegerAl(dr, 11, string.Empty);
                detay.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), detay.hesapPlanKod);
                detay.hesapPlanAd = Genel.AdGetir(isimler, detay.hesapPlanKod, "TASHESAPPLAN");

                detay.tutar[0] = VeriErisim.DegerAl(dr, 12, (decimal)0);

                if (kf != null && kf.detay != null)
                    kf.detay.Add(detay);
            }
            if (dr != null) dr.Close();
        }

        private static bool Farkli(KurusFarki kf, KurusFarki yenkf)
        {
            if (kf == null || yenkf == null) return true;

            if (kf.muhasebeKod != yenkf.muhasebeKod || kf.harcamaKod != yenkf.harcamaKod)
                return true;

            return false;
        }

        /// <summary>
        /// Verilen DepoDurum kriterlerine uygun olan taşınır malzemelerin giren, çıkan, önceki
        /// yılın tüketim, dönemsel tüketim miktarları/tutarları gibi bilgileri döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">DepoDurum listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır malzemelerinin miktar, tutar bilgileri listesi döndürülür.</returns>
        public static ObjectArray EnCokKullanilanTasinirlar(VeriErisim veriErisim, Kullanici kullanan, DepoDurum kriter)
        {
            ObjectArray donusDeger = new ObjectArray();

            donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, kriter.muhasebeKod, kriter.harcamaKod, kriter.ambarKod);
            if (!donusDeger.sonuc.islemSonuc)
            {
                donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tabRapor = new TABRaporlar(ve);
            IDataReader dr = null;

            try
            {
                string[] depoDurumIslem = { "giren", "cikan", "gecenYilTuketim", "donemselTuketim" };
                for (int i = 0; i < depoDurumIslem.Length; i++)
                {
                    dr = tabRapor.EnCokKullanilanTasinirlar(kriter, depoDurumIslem[i], Yetki.YetkiTipiDegerleriDondur(kullanan));
                    if (dr != null)
                        EnCokKullanilanTasinirlarBilgileriAl(dr, donusDeger.objeler, depoDurumIslem[i]);
                }
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                if (dr != null) dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            if (dr != null) dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen DataReader nesnesini dolaşarak en çok kullanılan taşınır
        /// bilgilerinin depoDurumIslem parametresine uygun olanlarını toplayan ve bulabilirse
        /// bilgi listesindeki ilgili nesneye, bulamazsa yeni yarattığı nesneye ekleyen yordam
        /// </summary>
        /// <param name="dr">DataReader nesnesi</param>
        /// <param name="bilgi">Daha önce toplanan bilgileri tutan nesne</param>
        /// <param name="depoDurumIslem">DataReader nesnesinden toplanacak rakam tipi
        /// Alabileceği değerler "giren", "cikan", "gecenYilTuketim" ve "donemselTuketim" değerleridir.</param>
        private static void EnCokKullanilanTasinirlarBilgileriAl(IDataReader dr, TNSCollection bilgi, string depoDurumIslem)
        {
            DepoDurum dp;
            while (dr.Read())
            {
                string hesapKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                hesapKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), hesapKod);
                int olcuBirimKod = VeriErisim.DegerAl(dr, 2, 0);

                int index = DetayIndexDondurDepoDurum(hesapKod, olcuBirimKod, bilgi, (int)ENUMDepoDurumRaporTur.AMBAR);
                if (index != -1)
                    dp = (DepoDurum)bilgi[index];
                else
                {
                    dp = new DepoDurum();
                    dp.hesapPlanKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                    dp.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), dp.hesapPlanKod);
                    dp.hesapPlanAd = VeriErisim.DegerAl(dr, 1, string.Empty);
                    dp.olcuBirimKod = VeriErisim.DegerAl(dr, 2, 0);
                    dp.olcuBirimAd = VeriErisim.DegerAl(dr, 3, string.Empty);

                    bilgi.Add(dp);
                }

                if (depoDurumIslem == "giren")
                {
                    dp.girenMiktar = VeriErisim.DegerAl(dr, 4, (decimal)0);
                    dp.girenTutar = VeriErisim.DegerAl(dr, 5, (decimal)0);
                }
                else if (depoDurumIslem == "cikan")
                {
                    dp.cikanMiktar = VeriErisim.DegerAl(dr, 4, (decimal)0);
                    dp.cikanTutar = VeriErisim.DegerAl(dr, 5, (decimal)0);
                }
                else if (depoDurumIslem == "gecenYilTuketim")
                {
                    dp.gecenYilTuketimMiktar = VeriErisim.DegerAl(dr, 4, (decimal)0);
                    dp.gecenYilTuketimTutar = VeriErisim.DegerAl(dr, 5, (decimal)0);
                }
                else if (depoDurumIslem == "donemselTuketim")
                {
                    dp.donemselTuketimMiktar = VeriErisim.DegerAl(dr, 4, (decimal)0);
                    dp.donemselTuketimTutar = VeriErisim.DegerAl(dr, 5, (decimal)0);
                }

                dp.kalanMiktar = dp.girenMiktar - dp.cikanMiktar;
                dp.kalanTutar = dp.girenTutar - dp.cikanTutar;
            }
            if (dr != null) dr.Close();
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır istek ve karşılanan miktarlarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Taşınır istek listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır istek bilgileri listesi döndürülür.</returns>
        public static ObjectArray IstekKarsilanmaListele(VeriErisim veriErisim, Kullanici kullanan, IstekForm kriter)
        {
            ObjectArray donusDeger = new ObjectArray();

            donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, kriter.muhasebeKod, kriter.harcamaKod, kriter.ambarKod);
            if (!donusDeger.sonuc.islemSonuc)
            {
                donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tabRapor = new TABRaporlar(ve);

            IDataReader dr = tabRapor.IstekKarsilananRaporu(kriter, Yetki.YetkiTipiDegerleriDondur(kullanan));

            while (dr.Read())
            {
                IstekDetay id = new IstekDetay();

                id.yil = VeriErisim.DegerAl(dr, 0, 0);
                id.muhasebeKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                id.muhasebeAd = VeriErisim.DegerAl(dr, 2, string.Empty);
                id.harcamaKod = VeriErisim.DegerAl(dr, 3, string.Empty);
                id.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), id.harcamaKod);
                id.harcamaAd = VeriErisim.DegerAl(dr, 4, string.Empty);
                id.istekYapanKod = VeriErisim.DegerAl(dr, 5, string.Empty);
                id.istekYapanAd = VeriErisim.DegerAl(dr, 6, string.Empty);
                id.hesapPlanKod = VeriErisim.DegerAl(dr, 7, string.Empty);
                id.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), id.hesapPlanKod);
                id.hesapPlanAd = VeriErisim.DegerAl(dr, 8, string.Empty);
                id.istenilenMiktar = VeriErisim.DegerAl(dr, 9, (decimal)0);
                id.karsilananMiktar = VeriErisim.DegerAl(dr, 10, (decimal)0);
                id.tuketilenMiktar = VeriErisim.DegerAl(dr, 11, (decimal)0);

                donusDeger.Ekle(id);
            }

            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır malzemelerinin giren, çıkan, kalan tutarlarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır malzemelerinin tutar bilgileri listesi döndürülür.</returns>
        public static ObjectArray MuhasebatKontrolRaporu(VeriErisim veriErisim, Kullanici kullanan, MuhasebatKontrol kriter)
        {
            ObjectArray donusDeger = new ObjectArray();

            donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, kriter.muhasebeKod, kriter.harcamaKod, string.Empty);
            if (!donusDeger.sonuc.islemSonuc)
            {
                donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
                return donusDeger;
            }

            string hata = string.Empty;
            if (kriter.yil <= 0)
                hata += TasinirMalResI.TMM036 + "<br>";
            if (kriter.hesapKodUzunluk <= 0)
                hata += TasinirMalResI.TMM196 + "<br>";
            if (kriter.raporTur < 0)
                hata += TasinirMalResI.TMM192 + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = hata;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tabRapor = new TABRaporlar(ve);

            IDataReader dr = tabRapor.MuhasebatKontrolRaporu(kriter, Yetki.YetkiTipiDegerleriDondur(kullanan));

            while (dr.Read())
            {
                MuhasebatKontrol mk = new MuhasebatKontrol();
                mk.hesapKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                mk.hesapKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), mk.hesapKod);
                int islemTur = VeriErisim.DegerAl(dr, 1, 0);
                decimal tutar = VeriErisim.DegerAl(dr, 2, (decimal)0);
                if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.MUHASEBE || kriter.raporTur == (int)ENUMDepoDurumRaporTur.MUHASEBE_HARCAMA)
                {
                    mk.muhasebeKod = VeriErisim.DegerAl(dr, 3, string.Empty);
                    if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.MUHASEBE_HARCAMA)
                        mk.harcamaKod = VeriErisim.DegerAl(dr, 4, string.Empty);
                }
                else if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.HARCAMA)
                    mk.harcamaKod = VeriErisim.DegerAl(dr, 3, string.Empty);

                mk.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), mk.harcamaKod);

                bool buldu = false;
                foreach (MuhasebatKontrol o in donusDeger.objeler)
                {
                    if (mk.muhasebeKod == o.muhasebeKod && mk.harcamaKod == o.harcamaKod && mk.hesapKod == o.hesapKod)
                    {
                        o.TutarEkle(islemTur, tutar);
                        buldu = true;
                        break;
                    }
                }

                if (!buldu)
                {
                    mk.TutarEkle(islemTur, tutar);
                    donusDeger.Ekle(mk);
                }
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            return donusDeger;
        }

        public static ObjectArray DevirCikis(VeriErisim veriErisim, Kullanici kullanan, DevirCikis kriter)
        {
            ObjectArray donusDeger = new ObjectArray();

            donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, kriter.muhasebeKod, kriter.harcamaKod, "");
            if (!donusDeger.sonuc.islemSonuc)
            {
                donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
                return donusDeger;
            }

            string hata = string.Empty;
            if (kriter.yil <= 0)
                hata += TasinirMalResI.TMM036 + "<br>";
            if (string.IsNullOrEmpty(kriter.muhasebeKod))
                hata += TasinirMalResI.TMM142 + "<br>";
            //if (string.IsNullOrEmpty(kriter.harcamaKod))
            //    hata += TasinirMalResI.TMM014 + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = hata;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tabRapor = new TABRaporlar(ve);
            IDataReader dr = null;

            int islemTur = OrtakFonksiyonlar.ConvertToInt(kriter.miktar, 0);
            try
            {
                dr = tabRapor.DevirCikis(kriter, islemTur);
                if (dr != null)
                    DevirCikisListeBilgileriAl(dr, donusDeger.objeler);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                if (dr != null) dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            if (dr != null) dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        private static void DevirCikisListeBilgileriAl(IDataReader dr, TNSCollection bilgi)
        {
            DevirCikis tk = null;
            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                tk = new DevirCikis();
                tk.fisNo = VeriErisim.DegerAl(dr, 0, string.Empty);

                tk.muhasebeKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                tk.harcamaKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                tk.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), tk.harcamaKod);

                tk.gMuhasebeKod = VeriErisim.DegerAl(dr, 3, string.Empty);

                tk.gHarcamaKod = VeriErisim.DegerAl(dr, 4, string.Empty);
                tk.gHarcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), tk.gHarcamaKod);

                tk.hesapPlanKod = VeriErisim.DegerAl(dr, 5, string.Empty);
                tk.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), tk.hesapPlanKod);

                tk.tutar = VeriErisim.DegerAl(dr, 6, (decimal)0);
                tk.tutarKDVli = VeriErisim.DegerAl(dr, 7, (decimal)0);

                tk.yil = VeriErisim.DegerAl(dr, 8, 0);

                bilgi.Add(tk);
            }
            if (dr != null) dr.Close();
        }

        /// <summary>
        /// Verilen DepoDurum kriterlerine uygun olan taşınır malzemelerin giren,
        /// çıkan, zimmet miktarları/tutarları gibi bilgileri döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">DepoDurum listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır malzemelerinin miktar, tutar bilgileri listesi döndürülür.</returns>
        public static ObjectArray KHKRaporu(VeriErisim veriErisim, Kullanici kullanan, DepoDurum kriter)
        {
            ObjectArray donusDeger = new ObjectArray();

            string hata = string.Empty;

            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = hata;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tabRapor = new TABRaporlar(ve);
            IDataReader dr = null;

            try
            {
                string[] depoDurumIslem;

                depoDurumIslem = new String[] { "giren", "cikan" };

                for (int i = 0; i < depoDurumIslem.Length; i++)
                {
                    dr = tabRapor.KHKRaporu(kriter, depoDurumIslem[i], Yetki.YetkiTipiDegerleriDondur(kullanan));
                    if (dr != null)
                        KHKBilgileriAl(dr, donusDeger.objeler, depoDurumIslem[i]);
                }
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                if (dr != null) dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            if (dr != null) dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen DataReader nesnesini dolaşarak taşınır depo durum
        /// bilgilerinin depoDurumIslem parametresine uygun olanlarını toplayan ve bulabilirse
        /// bilgi listesindeki ilgili nesneye, bulamazsa yeni yarattığı nesneye ekleyen yordam
        /// </summary>
        /// <param name="dr">DataReader nesnesi</param>
        /// <param name="bilgi">Daha önce toplanan bilgileri tutan nesne</param>
        /// <param name="depoDurumIslem">DataReader nesnesinden toplanacak rakam tipi
        /// Alabileceği değerler "giren", "cikan", "zimmetKisi" ve "zimmetOrtak" değerleridir.</param>
        /// <param name="raporTur">ENUMDepoDurumRaporTur listesindeki değerlerden biri olmalıdır.</param>
        private static void KHKBilgileriAl(IDataReader dr, TNSCollection bilgi, string depoDurumIslem)
        {
            DepoDurum dp;
            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                string kod = string.Empty;
                string firma = string.Empty;

                kod = VeriErisim.DegerAl(dr, 1, string.Empty);
                kod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), kod);
                firma = VeriErisim.DegerAl(dr, 0, "");

                int index = DetayIndexDondurKHKDurum(kod, firma, bilgi);
                if (index != -1)
                    dp = (DepoDurum)bilgi[index];
                else
                {
                    dp = new DepoDurum();
                    dp.firma = firma;
                    dp.hesapPlanKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                    dp.hesapPlanAd = Genel.AdGetir(isimler, dp.hesapPlanKod, "TASHESAPPLAN");
                    dp.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), dp.hesapPlanKod);

                    bilgi.Add(dp);
                }

                if (depoDurumIslem == "giren")
                {
                    dp.girenMiktar += VeriErisim.DegerAl(dr, 2, (decimal)0);
                    dp.girenTutar += VeriErisim.DegerAl(dr, 3, (decimal)0);
                }
                else if (depoDurumIslem == "cikan")
                {
                    dp.cikanMiktar += VeriErisim.DegerAl(dr, 2, (decimal)0);
                    dp.cikanTutar += VeriErisim.DegerAl(dr, 3, (decimal)0);
                }

                dp.kalanMiktar = dp.girenMiktar - dp.cikanMiktar;// -(dp.zimmetKisiMiktar + dp.zimmetOrtakMiktar);
                dp.kalanTutar = dp.girenTutar - dp.cikanTutar;// -(dp.zimmetKisiTutar + dp.zimmetOrtakTutar);
            }
            if (dr != null) dr.Close();
        }

        /// <summary>
        /// Parametre olarak verilen kod ve ölçü birimini verilen nesne listesinde
        /// raporTur değişkenine göre arayan ve listedeki indeksini döndüren yordam
        /// </summary>
        /// <param name="kod">Rapor türüyle ilişkili aranacak kod</param>
        /// <param name="olcuBirimKod">Ölçü birimi</param>
        /// <param name="detay">Arama yapılacak nesne listesi</param>
        /// <param name="raporTur">ENUMDepoDurumRaporTur listesindeki değerlerden biri olmalıdır.</param>
        /// <returns>Aranan kod bulunursa indeksi, bulunamazsa -1 döndürülür.</returns>
        private static int DetayIndexDondurKHKDurum(string kod, string firma, TNSCollection detay)
        {
            for (int i = 0; i < detay.Count; i++)
            {
                DepoDurum dp = (DepoDurum)detay[i];
                if (kod == dp.hesapPlanKod && firma == dp.firma)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır işlem bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi kriter bilgilerini tutan nesne</param>
        /// <param name="tfKriter">Ek kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır işlem bilgileri listesi döndürülür.</returns>
        public static ObjectArray TasinirKodGrupluTasinirHurdaRaporu(VeriErisim veriErisim, Kullanici kullanan, TasinirIslemForm tf, TasinirFormKriter tfKriter)
        {
            ObjectArray bilgiVAN = new ObjectArray();

            bilgiVAN.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod);
            if (!bilgiVAN.sonuc.islemSonuc)
            {
                bilgiVAN.sonuc.hataStr = TasinirMalResI.TMM062;
                return bilgiVAN;
            }

            if (tf.yil <= 0)
                bilgiVAN.sonuc.hataStr += TasinirMalResI.TMM036 + "<br>";

            //Taşınır Hurda Raporu için Muhasebe kodu boş brakılabilir
            if (tf.muhasebeKod == "empty")
                tf.muhasebeKod = string.Empty;
            else if (string.IsNullOrEmpty(tf.muhasebeKod))
                bilgiVAN.sonuc.hataStr += TasinirMalResI.TMM013 + "<br>";

            if (!string.IsNullOrEmpty(bilgiVAN.sonuc.hataStr))
            {
                bilgiVAN.sonuc.islemSonuc = false;
                return bilgiVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tRapor = new TABRaporlar(ve);

            TasinirIslemForm gelen = null;
            IDataReader dr = tRapor.TasinirKodGrupluTasinirHurdaRaporu(tf, tfKriter, Yetki.YetkiTipiDegerleriDondur(kullanan));
            //Uzay ile çekilen bilgiler işlemi çok yavaşlatıyor. Yıl sonu kontrolleri sırasında EGO da kullandım. Yedek parça 1501303 30dk gibi bir sürede tamamlandı. 100.000 kayıt vardı.
            //Uzayları kapatıp hesapadı ve işlemadı alanlarını sql kısmına koydum
            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                //if (gelen == null)
                //||
                //gelen.yil != VeriErisim.DegerAl(dr, 0, 0) ||
                //gelen.muhasebeKod != VeriErisim.DegerAl(dr, 1, string.Empty) ||
                //gelen.harcamaKod != OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), VeriErisim.DegerAl(dr, 2, string.Empty)) ||
                //gelen.fisNo != VeriErisim.DegerAl(dr, 3, string.Empty))
                //{
                gelen = new TasinirIslemForm();
                //gelen.yil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.muhasebeKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                gelen.muhasebeAd = Genel.AdGetir(isimler, gelen.muhasebeKod, "TASMUHASEBE");
                gelen.harcamaKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), gelen.harcamaKod);
                gelen.harcamaAd = Genel.AdGetir(isimler, gelen.muhasebeKod + "-" + gelen.harcamaKod, "TASHARCAMABIRIM");
                gelen.fisNo = VeriErisim.DegerAl(dr, 3, string.Empty);
                gelen.fisTarih = VeriErisim.DegerAl(dr, 4);
                gelen.ambarKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                gelen.ambarAd = Genel.AdGetir(isimler, gelen.muhasebeKod + "-" + gelen.harcamaKod + "-" + gelen.ambarKod, "TASAMBAR");
                //gelen.islemTipKod = VeriErisim.DegerAl(dr, 5, 0);
                gelen.islemTipAd = VeriErisim.DegerAl(dr, 5, string.Empty);
                //gelen.islemTipTur = VeriErisim.DegerAl(dr, 39, 0);
                //gelen.muayeneNo = VeriErisim.DegerAl(dr, 7, string.Empty);
                //gelen.muayeneTarih = VeriErisim.DegerAl(dr, 8);
                //gelen.dayanakNo = VeriErisim.DegerAl(dr, 9, string.Empty);
                //gelen.dayanakTarih = VeriErisim.DegerAl(dr, 10);
                //gelen.neredenGeldi = VeriErisim.DegerAl(dr, 11, string.Empty);
                gelen.nereyeGitti = VeriErisim.DegerAl(dr, 6, string.Empty);
                //gelen.gMuhasebeKod = VeriErisim.DegerAl(dr, 13, string.Empty);
                //gelen.gMuhasebeAd = Genel.AdGetir(isimler, gelen.gMuhasebeKod, "TASMUHASEBE");
                //gelen.gHarcamaKod = VeriErisim.DegerAl(dr, 14, string.Empty);
                //gelen.gHarcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), gelen.gHarcamaKod);
                //gelen.gHarcamaAd = Genel.AdGetir(isimler, gelen.gMuhasebeKod + "-" + gelen.gHarcamaKod, "TASHARCAMABIRIM");
                //gelen.gAmbarKod = VeriErisim.DegerAl(dr, 15, string.Empty);
                //gelen.gAmbarAd = Genel.AdGetir(isimler, gelen.gMuhasebeKod + "-" + gelen.gHarcamaKod + "-" + gelen.gAmbarKod, "TASAMBAR");
                //gelen.durum = VeriErisim.DegerAl(dr, 16, 0);
                //gelen.islemTarih = VeriErisim.DegerAl(dr, 17);
                //gelen.islemYapan = VeriErisim.DegerAl(dr, 18, string.Empty);
                //gelen.faturaNo = VeriErisim.DegerAl(dr, 19, string.Empty);
                //gelen.faturaTarih = VeriErisim.DegerAl(dr, 20);
                //gelen.kimeGitti = VeriErisim.DegerAl(dr, 21, string.Empty);                  


                //gelen.olcuBirimAd = VeriErisim.DegerAl(dr, 8, string.Empty); //Genel.AdGetir(isimler, detay.hesapPlanKod.Replace(".", ""), "TASOLCUBIRIMAD");              
                //gelen.miktar = VeriErisim.DegerAl(dr, 9, (decimal)0);
                //gelen.tutar = VeriErisim.DegerAl(dr, 10, (decimal)0);

                //}

                TasinirIslemDetay detay = new TasinirIslemDetay();
                //detay.yil = gelen.yil;
                detay.muhasebeKod = gelen.muhasebeKod;
                detay.harcamaKod = gelen.harcamaKod;
                detay.fisNo = gelen.fisNo;
                //detay.siraNo = VeriErisim.DegerAl(dr, 26, 0);
                detay.hesapPlanKod = VeriErisim.DegerAl(dr, 7, string.Empty);
                detay.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), detay.hesapPlanKod);
                //detay.hesapPlanAd = VeriErisim.DegerAl(dr, 38, string.Empty);
                detay.hesapPlanAd = Genel.AdGetir(isimler, detay.hesapPlanKod, "TASHESAPPLAN");
                //detay.sicilNo = VeriErisim.DegerAl(dr, 28, 0);
                //detay.gorSicilNo = Genel.AdGetir(isimler, detay.muhasebeKod + "-" + detay.harcamaKod.Replace(".", "") + "-" + detay.sicilNo.ToString(), "TASSICILGETIR");
                detay.olcuBirimAd = VeriErisim.DegerAl(dr, 8, string.Empty); //Genel.AdGetir(isimler, detay.hesapPlanKod.Replace(".", ""), "TASOLCUBIRIMAD");              
                detay.miktar = VeriErisim.DegerAl(dr, 9, (decimal)0);
                detay.tutar = VeriErisim.DegerAl(dr, 10, (decimal)0);
                //detay.kdvOran = VeriErisim.DegerAl(dr, 30, 0);
                //detay.birimFiyat = VeriErisim.DegerAl(dr, 31, (decimal)0);
                //detay.birimFiyatKDVLi = VeriErisim.DegerAl(dr, 32, (decimal)0);
                //detay.eSicilNo = VeriErisim.DegerAl(dr, 33, string.Empty);
                //detay.eAlimTarihi = VeriErisim.DegerAl(dr, 34);
                //detay.eTedarikSekli = VeriErisim.DegerAl(dr, 35, string.Empty);
                //detay.eAciklama = VeriErisim.DegerAl(dr, 36, string.Empty);
                //detay.sonTarih = VeriErisim.DegerAl(dr, 37);

                gelen.detay.objeler.Add(detay);

                bilgiVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (bilgiVAN.sonuc.islemSonuc && bilgiVAN.objeler.Count == 0)
                bilgiVAN.sonuc.bilgiStr = TasinirMalResI.TMM029;

            ve.VeriErisimKapat();
            return bilgiVAN;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır işlem bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi kriter bilgilerini tutan nesne</param>
        /// <param name="tfKriter">Ek kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır işlem bilgileri listesi döndürülür.</returns>
        public static ObjectArray IlBazliTasinirHurdaRaporu(VeriErisim veriErisim, Kullanici kullanan, TasinirIslemForm tf, TasinirFormKriter tfKriter)
        {
            ObjectArray bilgiVAN = new ObjectArray();

            bilgiVAN.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod);
            if (!bilgiVAN.sonuc.islemSonuc)
            {
                bilgiVAN.sonuc.hataStr = TasinirMalResI.TMM062;
                return bilgiVAN;
            }

            if (tf.yil <= 0)
                bilgiVAN.sonuc.hataStr += TasinirMalResI.TMM036 + "<br>";

            //Taşınır Hurda Raporu için Muhasebe kodu boş brakılabilir
            if (tf.muhasebeKod == "empty")
                tf.muhasebeKod = string.Empty;
            else if (string.IsNullOrEmpty(tf.muhasebeKod))
                bilgiVAN.sonuc.hataStr += TasinirMalResI.TMM013 + "<br>";

            if (!string.IsNullOrEmpty(bilgiVAN.sonuc.hataStr))
            {
                bilgiVAN.sonuc.islemSonuc = false;
                return bilgiVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tRapor = new TABRaporlar(ve);

            IDataReader dr = tRapor.IlBazliTasinirHurdaRaporu(tf, tfKriter, Yetki.YetkiTipiDegerleriDondur(kullanan));
            //Uzay ile çekilen bilgiler işlemi çok yavaşlatıyor. Yıl sonu kontrolleri sırasında EGO da kullandım. Yedek parça 1501303 30dk gibi bir sürede tamamlandı. 100.000 kayıt vardı.
            //Uzayları kapatıp hesapadı ve işlemadı alanlarını sql kısmına koydum
            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                HurdaRaporu rapor = rapor = new HurdaRaporu();
                rapor.ilKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                rapor.ilAd = VeriErisim.DegerAl(dr, 1, string.Empty);
                rapor.hesapPlanKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                rapor.miktar = VeriErisim.DegerAl(dr, 3, (decimal)0);
                rapor.tutar = VeriErisim.DegerAl(dr, 4, (decimal)0);

                bilgiVAN.objeler.Add(rapor);
            }
            dr.Close();

            if (bilgiVAN.sonuc.islemSonuc && bilgiVAN.objeler.Count == 0)
                bilgiVAN.sonuc.bilgiStr = TasinirMalResI.TMM029;

            ve.VeriErisimKapat();
            return bilgiVAN;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır işlem bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi kriter bilgilerini tutan nesne</param>
        /// <param name="tfKriter">Ek kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır işlem bilgileri listesi döndürülür.</returns>
        public static ObjectArray BirimItibariylaTasinirHurdaRaporu(VeriErisim veriErisim, Kullanici kullanan, TasinirIslemForm tf, TasinirFormKriter tfKriter)
        {
            ObjectArray bilgiVAN = new ObjectArray();

            bilgiVAN.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod);
            if (!bilgiVAN.sonuc.islemSonuc)
            {
                bilgiVAN.sonuc.hataStr = TasinirMalResI.TMM062;
                return bilgiVAN;
            }

            if (tf.yil <= 0)
                bilgiVAN.sonuc.hataStr += TasinirMalResI.TMM036 + "<br>";

            //Taşınır Hurda Raporu için Muhasebe kodu boş brakılabilir
            if (tf.muhasebeKod == "empty")
                tf.muhasebeKod = string.Empty;
            else if (string.IsNullOrEmpty(tf.muhasebeKod))
                bilgiVAN.sonuc.hataStr += TasinirMalResI.TMM013 + "<br>";

            if (!string.IsNullOrEmpty(bilgiVAN.sonuc.hataStr))
            {
                bilgiVAN.sonuc.islemSonuc = false;
                return bilgiVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tRapor = new TABRaporlar(ve);

            IDataReader dr = tRapor.BirimItibariylaTasinirHurdaRaporu(tf, tfKriter, Yetki.YetkiTipiDegerleriDondur(kullanan));
            //Uzay ile çekilen bilgiler işlemi çok yavaşlatıyor. Yıl sonu kontrolleri sırasında EGO da kullandım. Yedek parça 1501303 30dk gibi bir sürede tamamlandı. 100.000 kayıt vardı.
            //Uzayları kapatıp hesapadı ve işlemadı alanlarını sql kısmına koydum
            //System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                HurdaRaporu rapor = rapor = new HurdaRaporu();
                rapor.hBirimKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                rapor.hesapPlanKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                rapor.miktar = VeriErisim.DegerAl(dr, 2, (decimal)0);
                rapor.tutar = VeriErisim.DegerAl(dr, 3, (decimal)0);

                bilgiVAN.objeler.Add(rapor);
            }
            dr.Close();

            if (bilgiVAN.sonuc.islemSonuc && bilgiVAN.objeler.Count == 0)
                bilgiVAN.sonuc.bilgiStr = TasinirMalResI.TMM029;

            ve.VeriErisimKapat();
            return bilgiVAN;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır işlem bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi kriter bilgilerini tutan nesne</param>
        /// <param name="tfKriter">Ek kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır işlem bilgileri listesi döndürülür.</returns>
        /// 
        public static ObjectArray BakanlikDuzeyiTasinirHurdaRaporu(VeriErisim veriErisim, Kullanici kullanan, TasinirIslemForm tf, TasinirFormKriter tfKriter)
        {
            ObjectArray bilgiVAN = new ObjectArray();

            bilgiVAN.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod);
            if (!bilgiVAN.sonuc.islemSonuc)
            {
                bilgiVAN.sonuc.hataStr = TasinirMalResI.TMM062;
                return bilgiVAN;
            }

            if (tf.yil <= 0)
                bilgiVAN.sonuc.hataStr += TasinirMalResI.TMM036 + "<br>";

            //Taşınır Hurda Raporu için Muhasebe kodu boş brakılabilir
            if (tf.muhasebeKod == "empty")
                tf.muhasebeKod = string.Empty;
            else if (string.IsNullOrEmpty(tf.muhasebeKod))
                bilgiVAN.sonuc.hataStr += TasinirMalResI.TMM013 + "<br>";

            if (!string.IsNullOrEmpty(bilgiVAN.sonuc.hataStr))
            {
                bilgiVAN.sonuc.islemSonuc = false;
                return bilgiVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tRapor = new TABRaporlar(ve);

            IDataReader dr = tRapor.BakanlikDuzeyiTasinirHurdaRaporu(tf, tfKriter, Yetki.YetkiTipiDegerleriDondur(kullanan));
            //Uzay ile çekilen bilgiler işlemi çok yavaşlatıyor. Yıl sonu kontrolleri sırasında EGO da kullandım. Yedek parça 1501303 30dk gibi bir sürede tamamlandı. 100.000 kayıt vardı.
            //Uzayları kapatıp hesapadı ve işlemadı alanlarını sql kısmına koydum
            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                HurdaRaporu rapor = rapor = new HurdaRaporu();
                rapor.hesapPlanKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                rapor.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), rapor.hesapPlanKod);
                rapor.seviye = VeriErisim.DegerAl(dr, 1, 0);
                rapor.hesapPlanAd = Genel.AdGetir(isimler, rapor.hesapPlanKod, "TASHESAPPLAN");
                rapor.miktar = VeriErisim.DegerAl(dr, 2, (decimal)0);
                rapor.tutar = VeriErisim.DegerAl(dr, 3, (decimal)0);

                bilgiVAN.objeler.Add(rapor);
            }
            dr.Close();

            if (bilgiVAN.sonuc.islemSonuc && bilgiVAN.objeler.Count == 0)
                bilgiVAN.sonuc.bilgiStr = TasinirMalResI.TMM029;

            ve.VeriErisimKapat();
            return bilgiVAN;
        }

        /// <summary>
        /// Verilen DepoDurum kriterlerine uygun olan taşınır malzemelerin giren,
        /// çıkan, zimmet miktarları/tutarları gibi bilgileri döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">DepoDurum listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır malzemelerinin miktar, tutar bilgileri listesi döndürülür.</returns>
        public static ObjectArray HesapPlaniBazindaYillikTuketimRaporu(VeriErisim veriErisim, Kullanici kullanan, DepoDurum kriter)
        {
            //Yetki Görüşülmeli

            ObjectArray donusDeger = new ObjectArray();

            //Harcama Yönetiminden alındığında yetki problemi oluyor.Melih
            //donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, kriter.muhasebeKod, kriter.harcamaKod, kriter.ambarKod);
            //if (!donusDeger.sonuc.islemSonuc)
            //{
            //    donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
            //    return donusDeger;
            //}

            string hata = string.Empty;
            if (string.IsNullOrEmpty(kriter.muhasebeKod))
                hata += TasinirMalResI.TMM142 + "<br>";
            if (string.IsNullOrEmpty(kriter.harcamaKod))
                hata += TasinirMalResI.TMM014 + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = hata;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tabRapor = new TABRaporlar(ve);
            IDataReader dr = null;

            try
            {
                dr = tabRapor.HesapPlaniBazindaYillikTuketimRaporu(kriter);
                while (dr.Read())
                {
                    DepoDurum dp = new DepoDurum();

                    dp.yil = VeriErisim.DegerAl(dr, 0, 0);
                    dp.tip = VeriErisim.DegerAl(dr, 1, 0);
                    dp.muhasebeKod = VeriErisim.DegerAl(dr, 2, "");
                    dp.muhasebeAd = VeriErisim.DegerAl(dr, 3, "");
                    dp.harcamaKod = VeriErisim.DegerAl(dr, 4, "");
                    dp.harcamaAd = VeriErisim.DegerAl(dr, 5, "");
                    dp.hesapPlanKod = VeriErisim.DegerAl(dr, 6, string.Empty);
                    dp.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), dp.hesapPlanKod);
                    dp.girenMiktar = VeriErisim.DegerAl(dr, 7, (decimal)0);
                    dp.girenTutar = VeriErisim.DegerAl(dr, 8, (decimal)0);

                    donusDeger.Ekle(dp);
                }
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Verilen DepoDurum kriterlerine uygun olan taşınır malzemelerin giren,
        /// çıkan, zimmet miktarları/tutarları gibi bilgileri döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">DepoDurum listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır malzemelerinin miktar, tutar bilgileri listesi döndürülür.</returns>
        public static ObjectArray ToplamYillikTuketimRaporu(VeriErisim veriErisim, Kullanici kullanan, DepoDurum kriter)
        {
            //Yetki Görüşülmeli

            ObjectArray donusDeger = new ObjectArray();

            //Harcama Yönetiminden alındığında yetki problemi oluyor.Melih
            //donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, kriter.muhasebeKod, kriter.harcamaKod, kriter.ambarKod);
            //if (!donusDeger.sonuc.islemSonuc)
            //{
            //    donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
            //    return donusDeger;
            //}

            string hata = string.Empty;
            if (string.IsNullOrEmpty(kriter.muhasebeKod))
                hata += TasinirMalResI.TMM142 + "<br>";
            if (string.IsNullOrEmpty(kriter.harcamaKod))
                hata += TasinirMalResI.TMM014 + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = hata;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tabRapor = new TABRaporlar(ve);
            IDataReader dr = null;

            try
            {
                dr = tabRapor.ToplamYillikTuketimRaporu(kriter);
                while (dr.Read())
                {
                    DepoDurum dp = new DepoDurum();

                    dp.yil = VeriErisim.DegerAl(dr, 0, 0);
                    dp.tip = VeriErisim.DegerAl(dr, 1, 0);
                    dp.muhasebeKod = VeriErisim.DegerAl(dr, 2, "");
                    dp.muhasebeAd = VeriErisim.DegerAl(dr, 3, "");
                    dp.harcamaKod = VeriErisim.DegerAl(dr, 4, "");
                    dp.harcamaAd = VeriErisim.DegerAl(dr, 5, "");
                    dp.girenMiktar = VeriErisim.DegerAl(dr, 6, (decimal)0);
                    dp.girenTutar = VeriErisim.DegerAl(dr, 7, (decimal)0);

                    donusDeger.Ekle(dp);
                }
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        public static ObjectArray BirimlereGoreDemirbasRaporu(VeriErisim veriErisim, Kullanici kullanan, TasinirIslemDetay form)
        {
            ObjectArray donusDeger = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tabRapor = new TABRaporlar(ve);
            IDataReader dr = null;

            dr = tabRapor.BirimlereGoreDemirbasRaporu(form);
            while (dr.Read())
            {
                form = new TasinirIslemDetay();

                form.harcamaKod = VeriErisim.DegerAl(dr, 0, "");
                form.gonHarcamaKod = VeriErisim.DegerAl(dr, 1, "");
                form.hesapPlanKod = VeriErisim.DegerAl(dr, 2, "");
                form.hesapPlanAd = VeriErisim.DegerAl(dr, 3, "");
                form.miktar = VeriErisim.DegerAl(dr, 4, (decimal)0);
                form.tutar = VeriErisim.DegerAl(dr, 5, (decimal)0);

                donusDeger.Ekle(form);
            }

            if (dr != null)
                dr.Close();

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            ve.VeriErisimKapat();
            return donusDeger;
        }

        public static ObjectArray KontrolRaporu(VeriErisim veriErisim, Kullanici kullanan, KontrolRaporu form)
        {
            ObjectArray donusDeger = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tabRapor = new TABRaporlar(ve);
            IDataReader dr = null;

            if (form.raporTur == 1 || form.raporTur == 2)
            {
                if (form.raporTur == 1)
                    dr = tabRapor.KontrolRaporu01(form);
                else if (form.raporTur == 2)
                    dr = tabRapor.KontrolRaporu02(form);

                while (dr.Read())
                {
                    form = new KontrolRaporu();

                    form.prSicilNo = VeriErisim.DegerAl(dr, 0, 0);
                    form.kdvliBirimFiyatTif = VeriErisim.DegerAl(dr, 1, 0.0);
                    form.kdvliBirimFiyatIlk = VeriErisim.DegerAl(dr, 2, 0.0);
                    form.kdvliBirimFiyatSon = VeriErisim.DegerAl(dr, 3, 0.0);
                    form.yil = VeriErisim.DegerAl(dr, 4, 0);
                    form.muhasebeKod = VeriErisim.DegerAl(dr, 5, "");
                    form.muhasebeAd = VeriErisim.DegerAl(dr, 6, "");
                    form.harcamaKod = VeriErisim.DegerAl(dr, 7, "");
                    form.harcamaAd = VeriErisim.DegerAl(dr, 8, "");
                    form.fisNo = VeriErisim.DegerAl(dr, 9, "");
                    form.sicilNo = VeriErisim.DegerAl(dr, 10, "");
                    form.sicilNoAd = VeriErisim.DegerAl(dr, 11, "");

                    donusDeger.Ekle(form);
                }

                if (dr != null)
                    dr.Close();
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            ve.VeriErisimKapat();
            return donusDeger;
        }

    }
}
