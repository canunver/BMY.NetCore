using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// RFID entegrasyonu kullanılarak yapılan sayım bilgileriyle ilgili, iş kuralları
    /// kontrollerini yapıp, veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını
    /// çağırarak okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMSayimRFID
    {
        /// <summary>
        /// Parametre olarak verilen yıl için RFID sayım numarası üreten yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Sayım yapılacak yıl</param>
        /// <returns>RFID sayım numarası döndürülür.</returns>
        private static string SayimNoUret(VeriErisim veriErisim, Kullanici kullanan, int yil)
        {
            if (kullanan == null)
                return TasinirMalResI.TMM201;

            if (yil <= 0)
                return TasinirMalResI.TMM202;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGenel tabGenel = new TABGenel(ve);
            string sn = tabGenel.BelgeNoAl("SAYIMRFID" + yil.ToString(), "000000");
            ve.VeriErisimKapat();
            return sn;
        }

        public static Sonuc YeniSayimBaslat(VeriErisim veriErisim, Kullanici kullanan, SayimRFID srfid)
        {
            Sonuc sonuc = new Sonuc();

            sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, srfid.muhasebeKod, srfid.harcamaKod, srfid.ambarKod);
            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = TasinirMalResI.TMM050;
                return sonuc;
            }

            sonuc = Kontrol(srfid, true);
            if (sonuc.hataStr != String.Empty)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            srfid.sayimNo = SayimNoUret(ve, kullanan, srfid.yil);
            if (!string.IsNullOrEmpty(srfid.sayimNo))
            {
                srfid.durum = ENUMSayimRFIDDurum.DEVAMEDIYOR;

                TABSayimRFID tabSayimRFID = new TABSayimRFID(ve);
                sonuc = tabSayimRFID.Kaydet(srfid);
                
                sonuc.anahtar = srfid.sayimNo;
            }
            else
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM203;
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TasinirMalResI.TMM322;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TasinirMalResI.TMM323 + "<br />" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen RFID sayım bilgilerinin doğruluğunu kontrol eden yordam
        /// </summary>
        /// <param name="srfid">RFID sayım bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private static Sonuc Kontrol(SayimRFID srfid, bool yeniSayim)
        {
            Sonuc sonuc = new Sonuc();

            if (srfid.yil <= 0)
                sonuc.hataStr += TasinirMalResI.TMM036 + "<br>";
            if (!yeniSayim && string.IsNullOrEmpty(srfid.sayimNo))
                sonuc.hataStr += TasinirMalResI.TMM203 + "<br>";
            if (string.IsNullOrEmpty(srfid.sayimAdi))
                sonuc.hataStr += TasinirMalResI.TMM204 + "<br>";
            if (string.IsNullOrEmpty(srfid.muhasebeKod))
                sonuc.hataStr += TasinirMalResI.TMM013 + "<br>";
            if (string.IsNullOrEmpty(srfid.harcamaKod))
                sonuc.hataStr += TasinirMalResI.TMM014 + "<br>";
            if (string.IsNullOrEmpty(srfid.ambarKod))
                sonuc.hataStr += TasinirMalResI.TMM031 + "<br>";
            if (srfid.sayimTarih.isNull)
                sonuc.hataStr += TasinirMalResI.TMM197 + "<br>";

            sonuc.hataStr += Genel.KontrolMuhasebeHarcamaAmbar(srfid.muhasebeKod, srfid.harcamaKod, srfid.ambarKod);

            //Detay kontrol
            if (!yeniSayim)
            {
                if (srfid.detay.Count <= 0)
                    sonuc.hataStr += TasinirMalResI.TMM205 + "<br>";
                else
                {
                    foreach (SayimRFIDDetay sd in srfid.detay)
                    {
                        if (sd.prSicilNo <= 0)
                            sonuc.hataStr += TasinirMalResI.TMM206 + "<br>";
                        if (sd.durum < ENUMSayimRFIDDemirbasDurum.SAYIMDABULUNAMAYAN || sd.durum > ENUMSayimRFIDDemirbasDurum.SAYIMFAZLASI)
                            sonuc.hataStr += TasinirMalResI.TMM207 + "<br>";
                    }
                }
            }

            if (!string.IsNullOrEmpty(sonuc.hataStr))
                sonuc.islemSonuc = false;

            return sonuc;
        }

        /// <summary>
        /// RFID sayım bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen SayimRFID nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="srfid">RFID sayım bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, SayimRFID srfid, bool detaySifirla)
        {
            Sonuc sonuc = new Sonuc();

            sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, srfid.muhasebeKod, srfid.harcamaKod, srfid.ambarKod);
            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = TasinirMalResI.TMM050;
                return sonuc;
            }

            sonuc = Kontrol(srfid, false);
            if (sonuc.hataStr != String.Empty)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSayimRFID tabSayimRFID = new TABSayimRFID(ve);
            TABSayimRFIDDetay tabSayimRFIDDetay = new TABSayimRFIDDetay(ve);

            ve.BeginTransaction();

            if (detaySifirla)
                tabSayimRFIDDetay.Sil(srfid.yil, srfid.sayimNo);

            sonuc = tabSayimRFID.Guncelle(srfid);
            if (!sonuc.islemSonuc)
            {
                ve.Rollback();
                ve.VeriErisimKapat();
                return sonuc;
            }

            foreach (SayimRFIDDetay sd in srfid.detay)
            {
                sd.yil = srfid.yil;
                sd.sayimNo = srfid.sayimNo;

                sonuc = tabSayimRFIDDetay.Kaydet(sd);

                if (!sonuc.islemSonuc)
                {
                    ve.Rollback();
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = string.Format(TasinirMalResI.TMM208, srfid.sayimNo, srfid.sayimAdi, srfid.detay.Count);
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = string.Format(TasinirMalResI.TMM209, srfid.sayimNo, srfid.sayimAdi) + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan RFID sayım bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="srfid">RFID sayım kriterlerini tutan nesne</param>
        /// <param name="detay">RFID sayımının detay bilgileri listelensin mi bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında RFID sayım bilgileri listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, SayimRFID srfid, bool detay)
        {
            ObjectArray donusDeger = new ObjectArray();

            donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan);
            if (!donusDeger.sonuc.islemSonuc)
            {
                donusDeger.sonuc.hataStr = TasinirMalResI.TMM210;
                return donusDeger;
            }

            //if (srfid.yil == 0)
            //{
            //    donusDeger.sonuc.hataStr += TasinirMalResI.TMM036 + "<br>";
            //    donusDeger.sonuc.islemSonuc = false;
            //    return donusDeger;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSayimRFID tabRFID = new TABSayimRFID(ve);
            IDataReader dr = null;

            try
            {
                dr = tabRFID.Listele(srfid, Yetki.YetkiTipiDegerleriDondur(kullanan));
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                SayimRFID gelen = new SayimRFID();
                gelen.yil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.sayimNo = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.sayimAdi = VeriErisim.DegerAl(dr, 2, string.Empty);
                gelen.muhasebeKod = VeriErisim.DegerAl(dr, 3, string.Empty);
                gelen.harcamaKod = VeriErisim.DegerAl(dr, 4, string.Empty);
                gelen.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), gelen.harcamaKod);
                gelen.ambarKod = VeriErisim.DegerAl(dr, 5, string.Empty);
                gelen.sayimTarih = VeriErisim.DegerAl(dr, 6);
                gelen.tcKimlikNo = VeriErisim.DegerAl(dr, 7, string.Empty);
                gelen.odaKod = VeriErisim.DegerAl(dr, 8, string.Empty);
                gelen.durum = (ENUMSayimRFIDDurum)System.Enum.ToObject(typeof(ENUMSayimRFIDDurum), VeriErisim.DegerAl(dr, 9, (int)ENUMSayimRFIDDurum.TANIMSIZ));
                gelen.muhasebeAd = VeriErisim.DegerAl(dr, 10, string.Empty);
                gelen.harcamaAd = VeriErisim.DegerAl(dr, 11, string.Empty);
                gelen.ambarAd = VeriErisim.DegerAl(dr, 12, string.Empty);

                if (!string.IsNullOrEmpty(gelen.tcKimlikNo))
                    gelen.kisiAd = Genel.AdGetir(isimler, gelen.tcKimlikNo, "TASPERSONEL");

                if (!string.IsNullOrEmpty(gelen.odaKod))
                    gelen.odaAd = Genel.AdGetir(isimler, gelen.muhasebeKod + "-" + gelen.harcamaKod + "-" + gelen.odaKod, "TASODA");

                donusDeger.objeler.Add(gelen);
            }
            dr.Close();

            if (detay && donusDeger.objeler.Count == 1)
            {
                SayimRFID sf = (SayimRFID)donusDeger.objeler[0];
                ObjectArray d = DetayListele(ve, sf);
                if (d.sonuc.islemSonuc)
                    sf.detay = d.objeler;
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Verilen RFID sayım formunun detay bilgilerini döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="sf">RFID sayım formunun kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında RFID sayım formuna ait detay bilgileri listesi döndürülür.</returns>
        private static ObjectArray DetayListele(VeriErisim veriErisim, SayimRFID sf)
        {
            ObjectArray donusDeger = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            TABSayimRFIDDetay tabSD = new TABSayimRFIDDetay(ve);
            IDataReader dr = null;

            try
            {
                dr = tabSD.Listele(sf.yil, sf.sayimNo);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                SayimRFIDDetay gelen = new SayimRFIDDetay();
                gelen.yil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.sayimNo = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.prSicilNo = VeriErisim.DegerAl(dr, 2, 0);
                gelen.durum = (ENUMSayimRFIDDemirbasDurum)System.Enum.ToObject(typeof(ENUMSayimRFIDDemirbasDurum), VeriErisim.DegerAl(dr, 3, 0));
                gelen.terminalKod = VeriErisim.DegerAl(dr, 4, 0);
                gelen.bulunanOdaKod = VeriErisim.DegerAl(dr, 5, string.Empty);
                gelen.beklenenOdaKod = VeriErisim.DegerAl(dr, 6, string.Empty);
                gelen.bulunanKisiKod = VeriErisim.DegerAl(dr, 7, string.Empty);
                gelen.beklenenKisiKod = VeriErisim.DegerAl(dr, 8, string.Empty);
                gelen.bulunanTarih = VeriErisim.DegerAl(dr, 9) != null ? VeriErisim.DegerAl(dr, 9).Oku() : new DateTime();
                gelen.beklenenMuhasebeKod = VeriErisim.DegerAl(dr, 10, string.Empty);
                gelen.beklenenHarcamaBirimKod = VeriErisim.DegerAl(dr, 11, string.Empty);
                gelen.beklenenHarcamaBirimKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), gelen.beklenenHarcamaBirimKod);
                gelen.beklenenAmbarKod = VeriErisim.DegerAl(dr, 12, string.Empty);

                gelen.rfid = VeriErisim.DegerAl(dr, 13, 0);
                gelen.tckimlikno = VeriErisim.DegerAl(dr, 14, string.Empty);
                gelen.oda = VeriErisim.DegerAl(dr, 15, string.Empty);

                if (gelen.prSicilNo > 0)
                {
                    if (!string.IsNullOrEmpty(gelen.beklenenMuhasebeKod) && !string.IsNullOrEmpty(gelen.beklenenHarcamaBirimKod))
                        gelen.sicilNo = Genel.GorSicilNoGetir(gelen.beklenenMuhasebeKod, gelen.beklenenHarcamaBirimKod, gelen.prSicilNo);
                    else if (!string.IsNullOrEmpty(sf.muhasebeKod) && !string.IsNullOrEmpty(sf.harcamaKod))
                        gelen.sicilNo = Genel.GorSicilNoGetir(sf.muhasebeKod, sf.harcamaKod, gelen.prSicilNo);
                    if (!string.IsNullOrEmpty(gelen.sicilNo))
                        gelen.sicilAd = Genel.AdGetir(isimler, gelen.sicilNo.Substring(0, gelen.sicilNo.Length - 8), "TASHESAPPLAN");
                }

                if (!string.IsNullOrEmpty(gelen.bulunanOdaKod))
                    gelen.bulunanOdaAd = Genel.AdGetir(isimler, sf.muhasebeKod + "-" + sf.harcamaKod + "-" + gelen.bulunanOdaKod, "TASODA");

                if (!string.IsNullOrEmpty(gelen.beklenenOdaKod))
                {
                    if (!string.IsNullOrEmpty(gelen.beklenenMuhasebeKod) && !string.IsNullOrEmpty(gelen.beklenenHarcamaBirimKod))
                        gelen.beklenenOdaAd = Genel.AdGetir(isimler, gelen.beklenenMuhasebeKod + "-" + gelen.beklenenHarcamaBirimKod + "-" + gelen.beklenenOdaKod, "TASODA");
                    else if (!string.IsNullOrEmpty(sf.muhasebeKod) && !string.IsNullOrEmpty(sf.harcamaKod))
                        gelen.beklenenOdaAd = Genel.AdGetir(isimler, sf.muhasebeKod + "-" + sf.harcamaKod + "-" + gelen.beklenenOdaKod, "TASODA");
                }

                if (!string.IsNullOrEmpty(gelen.bulunanKisiKod))
                    gelen.bulunanKisiAd = Genel.AdGetir(isimler, gelen.bulunanKisiKod, "TASPERSONEL");

                if (!string.IsNullOrEmpty(gelen.beklenenKisiKod))
                    gelen.beklenenKisiAd = Genel.AdGetir(isimler, gelen.beklenenKisiKod, "TASPERSONEL");

                if (!string.IsNullOrEmpty(gelen.beklenenMuhasebeKod))
                {
                    gelen.beklenenMuhasebeAd = Genel.AdGetir(isimler, gelen.beklenenMuhasebeKod, "TASMUHASEBE");

                    if (!string.IsNullOrEmpty(gelen.beklenenHarcamaBirimKod))
                        gelen.beklenenHarcamaBirimAd = Genel.AdGetir(isimler, gelen.beklenenMuhasebeKod + "-" + gelen.beklenenHarcamaBirimKod, "TASHARCAMABIRIM");

                    if (!string.IsNullOrEmpty(gelen.beklenenAmbarKod))
                        gelen.beklenenAmbarAd = Genel.AdGetir(isimler, gelen.beklenenMuhasebeKod + "-" + gelen.beklenenHarcamaBirimKod + "-" + gelen.beklenenAmbarKod, "TASAMBAR");
                }

                donusDeger.objeler.Add(gelen);
            }
            dr.Close();

            if (donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            ve.VeriErisimKapat();
            return donusDeger;
        }
    }
}