using System;
using System.Data;
using System.Collections.Generic;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// RFID etiketi olan taşınır demirbaşlarının kapılardan geçişi log bilgileri ile ilgili,
    /// iş kuralları kontrollerini yapıp, veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını
    /// çağırarak okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMKapiGecisLog
    {
        /// <summary>
        /// RFID entegrasyonunda kurumdan çıkışına izin verilmeyen malzemeler
        /// kurumdan çıkarılacak olursa bilgilendirme loglarını kaydeden yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kgller">Bilgilendirme logları listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TNSCollection kgller)
        {
            Sonuc sonuc = new Sonuc();

            if (!TMMYetkiKontrol.TasinirMalYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKapiGecisLog tabKGL = new TABKapiGecisLog(ve);
            ve.BeginTransaction();

            foreach (KapiGecisLog kgl in kgller)
            {
                if (kgl.prSicilNo <= 0 && kgl.rfIdNo <= 0)
                    sonuc.hataStr = TasinirMalResI.TMM145 + "<br>";
                //if (string.IsNullOrEmpty(kgl.tcKimlikNo))
                //    sonuc.hataStr += TasinirMalResI.TMM152 + "<br>";
                if (!string.IsNullOrEmpty(kgl.tcKimlikNo) && !Genel.PersonelVarMi(ve, kgl.tcKimlikNo))
                    sonuc.hataStr += TasinirMalResI.TMM146 + "<br>";

                if (!string.IsNullOrEmpty(sonuc.hataStr))
                {
                    ve.VeriErisimKapat();
                    sonuc.islemSonuc = false;
                    return sonuc;
                }
            }

            try
            {
                foreach (KapiGecisLog kgl in kgller)
                {
                    if (kgl.prSicilNo <= 0)
                    {
                        kgl.prSicilNo = Genel.RFIDPrSicilNoDondur(kgl.rfIdNo);

                        if (kgl.prSicilNo <= 0)
                        {
                            sonuc.islemSonuc = false;
                            sonuc.hataStr = TasinirMalResI.TMM147;
                            break;
                        }
                    }

                    kgl.tarih.Yaz(DateTime.Now);
                    sonuc = tabKGL.Kaydet(kgl);
                    if (!sonuc.islemSonuc)
                        break;

                    while (kgl.tarih.Saniye - DateTime.Now.Second == 0) { } //Primary Key hatası vermesin diye 1 saniye bekleme yapıyorum.
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
        /// Verilen kriterlere uygun olan bilgilendirme loglarını döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kgl">Bilgilendirme logları listeleme kriterleri</param>
        /// <param name="tarih1">Tarih aralığının alt sınırı</param>
        /// <param name="tarih2">Tarih aralığının üst sınırı</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında bilgilendirme logları listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, KapiGecisLog kgl, TNSDateTime tarih1, TNSDateTime tarih2)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKapiGecisLog tabKGL = new TABKapiGecisLog(ve);

            IDataReader dr = tabKGL.Listele(kgl, tarih1, tarih2);


            List<KodAd> odalar = new List<KodAd>();
            List<KodAd> kisiler = new List<KodAd>();

            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                KapiGecisLog gelen = new KapiGecisLog();
                gelen.snh.prSicilNo = VeriErisim.DegerAl(dr, 0, 0);
                gelen.snh.siraNo = VeriErisim.DegerAl(dr, 1, 0);
                gelen.snh.yil = VeriErisim.DegerAl(dr, 2, 0);
                gelen.snh.muhasebeKod = VeriErisim.DegerAl(dr, 3, string.Empty);
                gelen.snh.harcamaBirimKod = VeriErisim.DegerAl(dr, 4, string.Empty);
                gelen.snh.harcamaBirimKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), gelen.snh.harcamaBirimKod);
                gelen.snh.fisNo = VeriErisim.DegerAl(dr, 5, string.Empty);
                gelen.snh.fisSiraNo = VeriErisim.DegerAl(dr, 6, 0);
                gelen.snh.sicilNo = VeriErisim.DegerAl(dr, 7, string.Empty);
                gelen.snh.sicilSiraNo = VeriErisim.DegerAl(dr, 8, string.Empty);
                gelen.snh.hesapPlanKod = VeriErisim.DegerAl(dr, 9, string.Empty);
                gelen.snh.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), gelen.snh.hesapPlanKod);
                gelen.snh.ambarKod = VeriErisim.DegerAl(dr, 10, string.Empty);
                gelen.snh.fiyat = VeriErisim.DegerAl(dr, 11, (decimal)0);
                gelen.snh.kdvOran = VeriErisim.DegerAl(dr, 12, 0);
                gelen.snh.islemTipKod = VeriErisim.DegerAl(dr, 13, 0);
                gelen.snh.islemTurKod = VeriErisim.DegerAl(dr, 14, 0);
                gelen.snh.islemTarih = VeriErisim.DegerAl(dr, 15);
                gelen.snh.islemYapan = VeriErisim.DegerAl(dr, 16, string.Empty);
                gelen.snh.durum = VeriErisim.DegerAl(dr, 17, 0);
                gelen.snh.fisTarih = VeriErisim.DegerAl(dr, 18);
                gelen.snh.neredenGeldi = VeriErisim.DegerAl(dr, 19, string.Empty);

                gelen.snh.nereyeGitti = VeriErisim.DegerAl(dr, 20, string.Empty);
                gelen.snh.odaKod = gelen.snh.nereyeGitti;

                if (!string.IsNullOrEmpty(gelen.snh.odaKod))
                {
                    if ((odalar.Count > 0) && (odalar.Contains(new KodAd { kod = gelen.snh.odaKod, ad = "" })))
                    {
                        gelen.snh.odaAd = odalar.Find(x => x.kod.Contains(gelen.snh.odaKod)).ad;
                    }
                    else
                    {
                        gelen.snh.odaAd = Genel.AdGetir(isimler, gelen.snh.muhasebeKod + "-" + gelen.snh.harcamaBirimKod + "-" + gelen.snh.odaKod, "TASODA");
                        odalar.Add(new KodAd { kod = gelen.snh.odaKod, ad = gelen.snh.odaAd });
                    }
                }
                gelen.snh.kimeGitti = VeriErisim.DegerAl(dr, 21, string.Empty);
                gelen.snh.personelKod = gelen.snh.kimeGitti;
                if (!string.IsNullOrEmpty(gelen.snh.personelKod))
                {
                    if ((kisiler.Count > 0) && (kisiler.Contains(new KodAd { kod = gelen.snh.personelKod, ad = "" })))
                    {
                        gelen.snh.kisiAd = kisiler.Find(x => x.kod.Contains(gelen.snh.odaKod)).ad;
                    }
                    else
                    {
                        gelen.snh.kisiAd = Genel.AdGetir(isimler, gelen.snh.personelKod, "TASPERSONEL");
                        kisiler.Add(new KodAd { kod = gelen.snh.personelKod, ad = gelen.snh.kisiAd });
                    }
                }

                gelen.snh.zimmetOzellik = VeriErisim.DegerAl(dr, 22, string.Empty);
                gelen.snh.kdvliBirimFiyat = VeriErisim.DegerAl(dr, 23, (decimal)0);
                gelen.snh.degerArtisi = VeriErisim.DegerAl(dr, 24, (decimal)0);
                gelen.snh.gonMuhasebeKod = VeriErisim.DegerAl(dr, 25, string.Empty);
                gelen.snh.gonHarcamaBirimKod = VeriErisim.DegerAl(dr, 26, string.Empty);
                gelen.snh.gonHarcamaBirimKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), gelen.snh.gonHarcamaBirimKod);
                gelen.snh.gonAmbarKod = VeriErisim.DegerAl(dr, 27, string.Empty);
                gelen.snh.muhasebeAd = VeriErisim.DegerAl(dr, 28, string.Empty);
                gelen.snh.harcamaBirimAd = VeriErisim.DegerAl(dr, 29, string.Empty);
                gelen.snh.ambarAd = VeriErisim.DegerAl(dr, 30, string.Empty);
                gelen.snh.hesapPlanAd = VeriErisim.DegerAl(dr, 31, string.Empty);
                gelen.snh.islemTipiAd = VeriErisim.DegerAl(dr, 32, string.Empty);
                gelen.snh.rfIdNo = VeriErisim.DegerAl(dr, 33, 0);
                gelen.snh.killNo = VeriErisim.DegerAl(dr, 34, 0);
                gelen.snh.accessNo = VeriErisim.DegerAl(dr, 35, 0);
                gelen.snh.eSicilNo = VeriErisim.DegerAl(dr, 36, string.Empty);
                gelen.snh.tagType = VeriErisim.DegerAl(dr, 37, 0);

                gelen.prSicilNo = gelen.snh.prSicilNo;
                gelen.rfIdNo = gelen.snh.rfIdNo;
                gelen.tcKimlikNo = VeriErisim.DegerAl(dr, 39, string.Empty);
                if (!string.IsNullOrEmpty(gelen.tcKimlikNo))
                {
                    if ((kisiler.Count > 0) && (kisiler.Contains(new KodAd { kod = gelen.tcKimlikNo, ad = "" })))
                    {
                        gelen.personelAd = kisiler.Find(x => x.kod.Contains(gelen.tcKimlikNo)).ad;
                    }
                    else
                    {
                        gelen.personelAd = Genel.AdGetir(isimler, gelen.tcKimlikNo, "TASPERSONEL");
                        kisiler.Add(new KodAd { kod = gelen.tcKimlikNo, ad = gelen.personelAd });
                    }
                }
                gelen.tarih = VeriErisim.DegerAl(dr, 40);

                bilgi.Ekle(gelen);
            }

            odalar.Clear();
            kisiler.Clear();

            if (bilgi.objeler.Count == 0)
                bilgi.sonuc.bilgiStr = TasinirMalResI.TMM029;

            dr.Close();
            ve.VeriErisimKapat();
            return bilgi;
        }
    }
}