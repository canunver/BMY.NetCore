using System;
using System.Collections.Generic;
using System.Text;
using TNS.BKD;
using TNS.KYM;
using TNS.Veri;
using TNS.BHMv2.IsKurallari.Veri;
using System.Collections;
using TNS.UZY;
using System.Data;

namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// Bütçe yıl sonu tahminlerinin (gelir ve gider) veri tabanına yazılması ve okunması ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class BHMv2ButceYSTahmin
    {
        /// <summary>
        /// Verilen tertip listesindeki yıl sonu ödenekleri veri tabanına kaydedilir. 
        /// Sıfır değeri olan ödenekler kaydedilmez. Ödenekler tertibin yılına ve verilen senaryoya kaydedilir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="senaryoKod">Yıl sonu gerçekleşme tahmininin kaydedileceği senaryo kodu</param>
        /// <param name="tertipListesi">Tertip ve yıl sonu gerçekleşme tahmini değerlerinin bulunduğu dizi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, string senaryoKod, TertipOdenek[] tertipListesi)
        {
            Sonuc sonuc = new Sonuc(true, string.Empty, string.Empty);
            string kodAd, kod;
            if (string.IsNullOrEmpty(senaryoKod))
                sonuc.hataStr = ButceHazirlikResI.BHMBYST001;

            if (tertipListesi == null || tertipListesi.Length == 0)
                sonuc.hataStr = ButceHazirlikResI.BHMBYST002;

            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            int yil = tertipListesi[0].tertip.yil;
            string kurumsalKod = string.Empty;
            bool yetkikontrol = false;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(yil), kullanan.KimlikBul());
            string hata = "";
            if (BHMv2ButceOdenek.KilitliMiSenaryo(ve, kullanan, yil, TNS.BKD.Arac.KurumsaldanKuruma(tertipListesi[0].tertip.kodlar[0], tertipListesi[0].tertip.Kirilimlar(0)), senaryoKod, out hata, true))
            {
                sonuc.DegerAta(hata);
                ve.VeriErisimKapat();
                return sonuc;
            }

            sonuc = BHMv2ButceOdenek.TertipListesiniPEBOdenekYap(ref tertipListesi, senaryoKod, true);
            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }


            for (int oNo = 0; oNo < tertipListesi.Length; oNo++)
            {
                Tertip tertip = tertipListesi[oNo].tertip;

                tertip.AdlariBul(UZY.Arac.Tanimla());

                if (yil != tertip.yil)
                {
                    sonuc.DegerAta(string.Format(ButceHazirlikResI.BHMBYST003, yil.ToString(), tertip.yil.ToString()));
                    break;
                }

                if (kurumsalKod != tertip.kodlar[0])
                {
                    kurumsalKod = tertip.kodlar[0];
                    yetkikontrol = BHMv2Genel.HazirlikYazmaYetkisiVarMi(kullanan, kurumsalKod, senaryoKod);
                    if (!yetkikontrol)
                    {
                        sonuc.DegerAta(string.Format(ButceHazirlikResI.BHMBYST004, tertip.TertipStrOl()));
                        break;
                    }
                    if (BHMv2ButceOdenek.KilitliMiSenaryo(ve, kullanan, yil, kurumsalKod, senaryoKod, out hata, false))
                    {
                        sonuc.DegerAta(hata);
                        return sonuc;
                    }
                }

                if (!tertip.TamMi())
                {
                    sonuc.DegerAta(string.Format(ButceHazirlikResI.BHMBYST005, tertip.TertipStrOl()));
                    break;
                }
                else if (!tertip.IsimlerTamMi(out kodAd, out kod))
                {
                    sonuc.DegerAta(string.Format(ButceHazirlikResI.BHMBYST006, kodAd, kod));
                    break;
                }
                string pKod = TNS.MUH.Arac.DiziElemanAl(tertip.kodlar, 4, "");
                if (pKod.Length > 2)
                {
                    pKod = pKod.Substring(0, 2);
                    //BHMv2ButceOdenek.ProgramlaraEkle(programDizi, pKod);
                }
            }

            if (yil <= 0)
                sonuc.DegerAta(ButceHazirlikResI.BHMBO007);
            //BHMv2ButceOdenek.ProgramKontrol(kullanan, yil, senaryoKod, programDizi, sonuc);

            if (sonuc.islemSonuc)
            {
                TABBHMTAHMIN tabBHMTAHMIN = TahminTabloOl(tertipListesi[0].tertip.tabloKod, ve);

                for (int oNo = 0; oNo < tertipListesi.Length; oNo++)
                {
                    //bir önceki yıla ait yapılacağı için yıl-1 olarak silme ve kayıt etme işlemi yapıyor
                    sonuc = tabBHMTAHMIN.CokYilliOdenekSil(yil - 1, senaryoKod, tertipListesi[oNo].tertip);
                    if (sonuc.islemSonuc)
                        sonuc = tabBHMTAHMIN.CokYilliOdenekSilAcik(yil - 1, senaryoKod, tertipListesi[oNo].tertip);
                    if (sonuc.islemSonuc && (tertipListesi[oNo].odenek[0] != 0 || !string.IsNullOrWhiteSpace(tertipListesi[oNo].aciklama)))
                    {
                        sonuc = tabBHMTAHMIN.Ekle(yil - 1, senaryoKod, 0, tertipListesi[oNo].tertip, tertipListesi[oNo].odenek[0]);
                        if (sonuc.islemSonuc)
                            sonuc = tabBHMTAHMIN.EkleAcik(yil - 1, senaryoKod, 0, tertipListesi[oNo].tertip, tertipListesi[oNo].aciklama);
                    }
                    if (sonuc.islemSonuc == false)
                        break;
                }
                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();

                ve.VeriErisimKapat();
            }

            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, string senaryoKod, Tertip kistas)
        {
            ObjectArray oa = new ObjectArray();
            if (string.IsNullOrEmpty(senaryoKod))
                oa.sonuc.hataStr = ButceHazirlikResI.BHMBYST001;

            if (!string.IsNullOrEmpty(oa.sonuc.hataStr))
            {
                oa.sonuc.islemSonuc = false;
                return oa;
            }

            int yil = kistas.yil;
            string kurumsalKod = kistas.kodlar[0];
            bool yetkikontrol = false;
            yetkikontrol = BHMv2Genel.HazirlikYazmaYetkisiVarMi(kullanan, kurumsalKod, senaryoKod);
            if (!yetkikontrol)
            {
                oa.sonuc.DegerAta(string.Format(ButceHazirlikResI.BHMBYST004, kistas.TertipStrOl()));
                return oa;
            }


            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(yil), kullanan.KimlikBul());
            TABBHMTAHMIN tabBHMTAHMIN = TahminTabloOl(kistas.tabloKod, ve);
            IDataReader dr = tabBHMTAHMIN.Listele(senaryoKod, kistas);
            if (dr != null)
            {
                while (dr.Read())
                {
                    TertipOdenek to = new TertipOdenek(1);
                    to.tertip = new Tertip(kistas.yil, kistas.tabloKod);
                    to.tertip.kodlar[0] = VeriErisim.DegerAl(dr, 3, "");
                    to.tertip.kodlar[1] = VeriErisim.DegerAl(dr, 4, "");
                    to.tertip.kodlar[2] = VeriErisim.DegerAl(dr, 5, "");
                    to.tertip.kodlar[3] = VeriErisim.DegerAl(dr, 6, "");
                    to.tertip.kodlar[4] = VeriErisim.DegerAl(dr, 7, "");
                    to.odenek[0] = VeriErisim.DegerAl(dr, 8, 0.0);
                    to.aciklama = VeriErisim.DegerAl(dr, 9, "");
                    oa.objeler.Add(to);
                }
                dr.Close();
            }
            else
                oa.sonuc.DegerAta(ve.hata);

            ve.VeriErisimKapat();
            return oa;
        }

        /// <summary>
        /// Tertip içi tablo koduna göre, gider tahmini veya gider tahmini tablosu nesnesi, veri erişim nesnesi kullanılarak oluşturulur ve döndürülür.
        /// </summary>
        /// <param name="tabloKod">Yıl sonu tahmini tablosu nesnesi oluşturulacak, ödenek tertip türü</param>
        /// <param name="ve">Tablo oluşumunda kullanılacak veri erişim nesnesi</param>
        /// <returns>Tahmin tabloları için ortak tablo nesnesi, verilen tablo koduna göre oluşturulur ve döndürülür.</returns>
        private static TABBHMTAHMIN TahminTabloOl(ENUMTabloKod tabloKod, VeriErisim ve)
        {
            if (tabloKod == ENUMTabloKod.TABGelirTahmini)
                return new TABBHMTAHMINGELIR(ve);
            else
                return new TABBHMTAHMINGIDER(ve);
        }
    }
}
