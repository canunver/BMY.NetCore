using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Teklif yapacak firma bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCTeklifFirma
    {
        /// <summary>
        /// Arguman olarak verilen, teklif yapacak firma bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri TeklifFirma nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılan sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılan sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray liste, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";
            int satir = 0;
            foreach (TeklifFirma f in liste.objeler)
            {
                satir++;
                if (f.firmaNo == 0)
                    hata += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00278, satir) + "<br>";
            }

            if (liste.objeler.Count == 0)
                hata = TNS.HRC.HarcamaSurecleriResI.HRC00007;

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////
            bool tamamlandi = true;
            int yil = ((TeklifFirma)liste.objeler[0]).yil;
            string surecNo = ((TeklifFirma)liste.objeler[0]).surecNo;
            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = true;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTeklifFirma to = new TABTeklifFirma(ve);
            ve.BeginTransaction();

            sonuc = to.Sil(liste);

            if (sonuc.islemSonuc)
                sonuc = to.Kaydet(liste);

            if (sonuc.islemSonuc)
            {
                //Parca Adım devam ediyor olarak işaretle.
                ///////////////////////////////////////////////////////////////////////////
                new HRCSurec().ParcaDurumKaydet(null, kullanan, yil, surecNo, islemKod, parcaKod, ENUMSurecParcaDurum.DEVAMEDIYOR, kullanan.kullaniciKodu);
                ///////////////////////////////////////////////////////////////////////////

                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, teklif yapacak firma bilgisini veritabanından okur 
        /// ve TeklifFirma nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, teklif yapacak firma listesini TeklifFirma nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, TeklifFirma kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTeklifFirma to = new TABTeklifFirma(ve);
            IDataReader r = to.Listele(kriter);

            if (r != null)
            {
                while (r.Read())
                {
                    TeklifFirma f = new TeklifFirma();

                    f.yil = VeriErisim.DegerAl(r, 0, 0);
                    f.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    f.birim = VeriErisim.DegerAl(r, 2, "");
                    f.surecNo = VeriErisim.DegerAl(r, 3, "");
                    f.firmaNo = VeriErisim.DegerAl(r, 4, 0);
                    f.tip = VeriErisim.DegerAl(r, 5, 0);
                    f.firmaBilgisi = new TNS.HRC.IsKurallari.HRCServis().FirmaBilgisiListele(kullanan, f.firmaNo);

                    bilgi.objeler.Add(f);
                }
                r.Close();
            }
            if (bilgi.ObjeSayisi == 0 && kriter.tip == 3)
            {
                r = to.ListeleTeklifEdilenFiyat(kriter);
                if (r != null)
                {
                    while (r.Read())
                    {
                        TeklifFirma f = new TeklifFirma();

                        f.yil = VeriErisim.DegerAl(r, 0, 0);
                        f.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                        f.birim = VeriErisim.DegerAl(r, 2, "");
                        f.surecNo = VeriErisim.DegerAl(r, 3, "");
                        f.firmaNo = VeriErisim.DegerAl(r, 4, 0);
                        f.tip = VeriErisim.DegerAl(r, 5, 0);
                        f.firmaBilgisi = new TNS.HRC.IsKurallari.HRCServis().FirmaBilgisiListele(kullanan, f.firmaNo);

                        bilgi.objeler.Add(f);
                    }
                    r.Close();
                }
            }

            if (bilgi.objeler.Count == 0)
            {
                IhaleTeklifDosyasi kriterDosya = new IhaleTeklifDosyasi();
                kriterDosya.yil = kriter.yil;
                kriterDosya.surecNo = kriter.surecNo;
                kriterDosya.muhasebe = kriter.muhasebe;
                kriterDosya.birim = kriter.birim;
                kriterDosya.firmaDevam = (int)ENUMEvetHayir.EVET;

                ObjectArray donusDeger = new HRCTeklifDosyasiVeren().ListeleDevamEdenFirma(kullanan, kriterDosya);
                if (donusDeger.sonuc.islemSonuc && donusDeger.ObjeSayisi > 0)
                {
                    foreach (IhaleTeklifDosyasi item in donusDeger.objeler)
                    {
                        TeklifFirma f = new TeklifFirma();

                        f.yil = item.yil;
                        f.muhasebe = item.muhasebe;
                        f.birim = item.birim;
                        f.surecNo = item.surecNo;
                        f.firmaNo = item.firmaKodu;
                        f.tip = kriter.tip;
                        f.firmaBilgisi = new TNS.HRC.IsKurallari.HRCServis().FirmaBilgisiListele(kullanan, f.firmaNo);

                        bilgi.objeler.Add(f);
                    }
                }
            }
            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, kazanan firma bilgisini veritabanından okur 
        /// ve TeklifFirma nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak süreç yıl bilgisi</param>
        /// <param name="surecNo">İşlem yapılacak süreç no bilgisi</param>
        /// <returns>İstenilen kriterdeki, kazanan firma bilgi listesini TeklifFirma nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleKazananFirma(Kullanici kullanan, int yil, string surecNo)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTeklifFirma to = new TABTeklifFirma(ve);
            IDataReader r = to.ListeleKazananFirma(yil, surecNo);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TeklifFirma f = new TeklifFirma();

                    f.yil = VeriErisim.DegerAl(r, 0, 0);
                    f.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    f.birim = VeriErisim.DegerAl(r, 2, "");
                    f.surecNo = VeriErisim.DegerAl(r, 3, "");
                    f.firmaNo = VeriErisim.DegerAl(r, 4, 0);
                    f.tip = VeriErisim.DegerAl(r, 5, 0);
                    f.firmaBilgisi = new TNS.HRC.IsKurallari.HRCServis().FirmaBilgisiListele(kullanan, f.firmaNo);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, süreçte tanımlı ihale için kazanan firma sayısını veritabanından okur ve döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak süreç yıl bilgisi</param>
        /// <param name="surecNo">İşlem yapılacak süreç no bilgisi</param>
        /// <returns>İstenilen kriterdeki, süreçte tanımlı ihale için kazanan firma sayısı</returns>
        public int KazananFirmaSayisiGetir(Kullanici kullanan, int yil, string surecNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTeklifFirma to = new TABTeklifFirma(ve);
            int donusDeger = 0;
            IDataReader r = to.KazananFirmaSayisi(yil, surecNo);

            ObjectArray liste = new ObjectArray();
            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    liste.objeler.Add(VeriErisim.DegerAl(r, 0, 0));
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            donusDeger = liste.ObjeSayisi;
            return donusDeger;
        }
    }
}
