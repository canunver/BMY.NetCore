using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.BKD;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Para birimi bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCTurkcellLog
    {
        /// <summary>
        /// Arguman olarak verilen, para birimi bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, TurkcellLog bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YoneticiMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";


            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTurkcellLog to = new TABTurkcellLog(ve);
            ve.BeginTransaction();

            sonuc = to.Kaydet(bilgi);

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
        /// Arguman olarak verilen, para birimi bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Sil(Kullanici kullanan, TurkcellLog bilgi)
        {
            Sonuc sonuc = new Sonuc();

            if (!HRCYetkiKontrol.YoneticiMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (bilgi.transactionId != "")
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00211;

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTurkcellLog to = new TABTurkcellLog(ve);

            ve.BeginTransaction();

            sonuc = to.Sil(bilgi);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ""; //string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00213, bilgi.kod) + "<br>";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00019 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, para birimi bilgisini veritabanından okur 
        /// ve TurkcellLog nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, para birimi listesini TurkcellLog nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, TurkcellLog kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTurkcellLog to = new TABTurkcellLog(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TurkcellLog f = new TurkcellLog();

                    f.yil = VeriErisim.DegerAl(r, 0, 0);
                    f.surecNo = VeriErisim.DegerAl(r, 1, "");
                    f.islemTarih = VeriErisim.DegerAl(r, 2);
                    f.islemTur = VeriErisim.DegerAl(r, 3, 0);
                    f.transactionId = VeriErisim.DegerAl(r, 4, "");
                    f.dosyaAdi = VeriErisim.DegerAl(r, 5, "");
                    f.faturaBilgi = VeriErisim.DegerAl(r, 6, "");
                    f.islemSonuc = VeriErisim.DegerAl(r, 7, "");
                    f.aciklama = VeriErisim.DegerAl(r, 8, "");

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public string DosyaAdiAl(Kullanici kullanan)
        {
            TurkcellLog kriter = new TurkcellLog();
            kriter.islemTarih = new TNSDateTime(DateTime.Now);
            ObjectArray liste = Listele(kullanan, kriter);
            string dosyaAdi = string.Empty;
            int numara = 1;
            if (liste.ObjeSayisi > 0)
            {
                dosyaAdi = ((TurkcellLog)liste.objeler[0]).dosyaAdi;
                numara = OrtakFonksiyonlar.ConvertToInt(dosyaAdi.Substring(dosyaAdi.Length - 10 - 1, 10), 0);
                numara++;
            }
            return "O" + DateTime.Now.Year.ToString()
                     + DateTime.Now.Month.ToString().PadLeft(2, '0')
                     + DateTime.Now.Day.ToString()
                     + numara.ToString().PadLeft(10, '0');
        }
    }
}
