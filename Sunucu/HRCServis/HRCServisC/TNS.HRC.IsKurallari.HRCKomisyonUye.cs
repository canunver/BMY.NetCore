using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Komisyon üye bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCKomisyonUye
    {
        /// <summary>
        /// Arguman olarak verilen, komisyon üyeleri bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, KomisyonUye liste, int islemKod, int parcaKod)
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
            foreach (Uye k in liste.uyeler.objeler)
            {
                satir++;
                if (k.ad.Trim() == "")
                    hata += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00221, satir) + "<br>";
            }

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            ////Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            /////////////////////////////////////////////////////////////////////////////
            //int yil = 0;
            //string surecNo = "";
            //if (liste.tip == (int)ENUMKomisyon.IHALE)
            //{
            //    bool tamamlandi = true;
            //    yil = liste.yil;
            //    surecNo = liste.surecNo;
            //    tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            //    if (tamamlandi)
            //    {
            //        sonuc.islemSonuc = false;
            //        sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
            //        return sonuc;
            //    }
            //}
            /////////////////////////////////////////////////////////////////////////////

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKomisyonUye to = new TABKomisyonUye(ve);
            ve.BeginTransaction();

            sonuc = to.Sil(liste);

            if (sonuc.islemSonuc)
                sonuc = to.Kaydet(liste);

            if (sonuc.islemSonuc)
            {
                ////Parca Adım devam ediyor olarak işaretle.
                /////////////////////////////////////////////////////////////////////////////
                //if (liste.tip == (int)ENUMKomisyon.IHALE)
                //    new HRCSurec().ParcaDurumKaydet(null, yil, surecNo, islemKod, parcaKod, ENUMSurecParcaDurum.DEVAMEDIYOR, kullanan.kullaniciKodu);
                /////////////////////////////////////////////////////////////////////////////
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
        /// Arguman olarak verilen kriterlere bağlı olarak, komisyon üye bilgisini veritabanından okur 
        /// ve KomisyonUye nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, komisyon üye bilgisi listesini tutan KomisyonUye nesnesi</returns>
        public KomisyonUye Listele(Kullanici kullanan, KomisyonUye kriter)
        {
            KomisyonUye bilgi = new KomisyonUye();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABKomisyonUye to = new TABKomisyonUye(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    if (bilgi.yil == 0)
                    {
                        bilgi.yil = VeriErisim.DegerAl(r, 0, 0);
                        bilgi.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                        bilgi.birim = VeriErisim.DegerAl(r, 2, "");
                        bilgi.surecNo = VeriErisim.DegerAl(r, 3, "");
                        bilgi.tip = VeriErisim.DegerAl(r, 4, 0);
                        bilgi.gorevlendirmeTarih = VeriErisim.DegerAl(r, 13);
                        bilgi.gorevlendirmeNo = VeriErisim.DegerAl(r, 14, 0);
                        bilgi.teslimAlan = VeriErisim.DegerAl(r, 15, "");
                        bilgi.teslimTarih = VeriErisim.DegerAl(r, 16);
                    }

                    Uye f = new Uye();

                    f.komisyonGorev = VeriErisim.DegerAl(r, 6, 0);
                    f.ad = VeriErisim.DegerAl(r, 7, "");
                    f.unvan = VeriErisim.DegerAl(r, 8, "");
                    f.gorev = VeriErisim.DegerAl(r, 9, "");
                    f.sicilNo = VeriErisim.DegerAl(r, 10, "");
                    f.telefon = VeriErisim.DegerAl(r, 11, "");
                    f.cepTelefon = VeriErisim.DegerAl(r, 12, "");

                    bilgi.uyeler.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }
    }
}
