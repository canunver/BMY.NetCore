using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Üretici fiyatları Endeksi bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCUfe
    {
        /// <summary>
        /// Arguman olarak verilen, Üretici fiyatları Endeksi bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri Ufe nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YoneticiMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (liste.objeler.Count == 0)
                hata = TNS.HRC.HarcamaSurecleriResI.HRC00007;

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABUfe to = new TABUfe(ve);
            ve.BeginTransaction();

            sonuc = to.Kaydet(liste);

            if (sonuc.islemSonuc)
            {
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
        /// Arguman olarak verilen kriterlere bağlı olarak, Üretici Fiyatları Endeksi bilgisini veritabanından okur 
        /// ve Ufe nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, Üretici Fiyatları Endeksi bilgisi listesini Ufe nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, Ufe kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABUfe to = new TABUfe(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Ufe f = new Ufe();

                    f.yil = VeriErisim.DegerAl(r, 0, 0);
                    f.ay = VeriErisim.DegerAl(r, 1, 0);
                    f.oran = VeriErisim.DegerAl(r, 2, 0.0);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen yıl ve ay kriterlerinden bugune kadarki Üretici Fiyatları Endeksi oranlarını kümülatif toplam olarak veritabanından okur ve döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi</param>
        /// <returns>İstenilen kriterdeki, yıl ve ay kriterlerinden bugune kadarki Üretici Fiyatları Endeksi oranlarının kümülatif toplam bilgisi</returns>
        public double UfeToplami(Kullanici kullanan, int yil, int ay)
        {
            double deger = 0.0;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABUfe to = new TABUfe(ve);

            deger = to.ToplamBul(yil, ay);

            deger = 1 + (deger / 100);

            //Yönetmeliğe göre 2003 yılı dahil önceki dönemlerde için üfe toplamı %100 (2 katı) kabul edilir.
            if (yil <= 2003)
                deger = 2;

            ve.VeriErisimKapat();

            return Math.Round(deger, 4);
        }
    }
}
