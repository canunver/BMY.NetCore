using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Taahhutname bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCTaahhutname
    {
        /// <summary>
        /// Arguman olarak verilen, taahhutname bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="tcKimlikNo">TC Kimlik Numarası</param>
        /// <param name="tur">Tür</param>
        /// <param name="tarih">Tarih</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, string tcKimlikNo, int tur, TNSDateTime tarih)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            string hata = "";

            if (tcKimlikNo == "")
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00236 + "<br>";
            if (tur == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00144 + "<br>";
            if (tarih.isNull)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00193 + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTaahhutname to = new TABTaahhutname(ve);

            ve.BeginTransaction();

            sonuc = to.Kaydet(tcKimlikNo, tur, tarih);

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
        /// Arguman olarak verilen kriterlere bağlı olarak, taahhutname bilgisini veritabanından okur 
        /// ve [tcKimlikNo|tur|tarih] string nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="tcKimlikNo">TC Kimlik Numarası</param>
        /// <param name="tur">Tür</param>
        /// <param name="tarih">Tarih</param>
        /// <returns></returns>
        public ObjectArray Listele(string tcKimlikNo, int tur, TNSDateTime tarih)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));

            TABTaahhutname to = new TABTaahhutname(ve);
            IDataReader r = to.Listele(tcKimlikNo, tur, tarih);

            if (to.Hata() == "")
            {
                while (r.Read())
                {

                    tcKimlikNo = VeriErisim.DegerAl(r, 0, "");
                    tur = VeriErisim.DegerAl(r, 1, 0);
                    tarih = VeriErisim.DegerAl(r, 2);

                    bilgi.objeler.Add(tcKimlikNo + "|" + tur + "|" + tarih);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }
    }
}
