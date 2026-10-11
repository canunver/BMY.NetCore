using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Taşınır işlem fişi veya zimmet fişi bilgileriyle ilgili, iş kuralları
    /// kontrollerini yapıp, veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını
    /// çağırarak okuma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMTarihce
    {
        /// <summary>
        /// Parametre olarak verilen taşınır işlem fişine ait işlem tarihçesi bilgilerini döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="muhasebe">Muhasebe birimi kodu</param>
        /// <param name="harcamaBirimi">Harcama birimi kodu</param>
        /// <param name="belgeNo">Belge numarası</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır işlem fişine ait işlem tarihçesi listesi döndürülür.</returns>
        public ObjectArray IslemFormListele(Kullanici kullanan, int yil, string muhasebe, string harcamaBirimi, string belgeNo)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTarihce to = new TABTarihce(ve);
            IDataReader r = to.IslemFormListele(yil, muhasebe, harcamaBirimi.Replace(".", ""), belgeNo);

            if (to.Hata() == "")
            {
                int sonKolon = (int)TABTarihce.ENUMAlanlar.TASISLEMFORMKOD;

                while (r.Read())
                {
                    TarihceBilgisi tarih = new TarihceBilgisi();

                    tarih.yil = VeriErisim.DegerAl(r, (int)TABTarihce.ENUMAlanlar.YIL, 0);
                    tarih.muhasebe = VeriErisim.DegerAl(r, (int)TABTarihce.ENUMAlanlar.MUHASEBEKOD, "");
                    tarih.harcamaBirimi = VeriErisim.DegerAl(r, (int)TABTarihce.ENUMAlanlar.HARCAMABIRIMKOD, "");
                    tarih.belgeNo = VeriErisim.DegerAl(r, (int)TABTarihce.ENUMAlanlar.FISNO, "");
                    tarih.islemYapan = VeriErisim.DegerAl(r, (int)TABTarihce.ENUMAlanlar.ISLEMYAPAN, "");
                    tarih.islemTarih = VeriErisim.DegerAl(r, (int)TABTarihce.ENUMAlanlar.ISLEMTARIH);
                    tarih.islem = VeriErisim.DegerAl(r, (int)TABTarihce.ENUMAlanlar.ISLEM, "");
                    tarih.tasinirIslemFormKod = VeriErisim.DegerAl(r, (int)TABTarihce.ENUMAlanlar.TASISLEMFORMKOD, "");
                    tarih.onayDurum = VeriErisim.DegerAl(r, sonKolon + 2, 0);

                    iVAN.objeler.Add(tarih);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return iVAN;
        }

        /// <summary>
        /// Parametre olarak verilen zimmet fişine ait işlem tarihçesi bilgilerini döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="muhasebe">Muhasebe birimi kodu</param>
        /// <param name="harcamaBirimi">Harcama birimi kodu</param>
        /// <param name="belgeNo">Belge numarası</param>
        /// <param name="belgeTur">ENUMZimmetBelgeTur listesindeki değerlerden biri olmalıdır.</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında zimmet fişine ait işlem tarihçesi listesi döndürülür.</returns>
        public ObjectArray ZimmetListele(Kullanici kullanan, int yil, string muhasebe, string harcamaBirimi, string belgeNo, int belgeTur)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTarihce to = new TABTarihce(ve);
            IDataReader r = to.ZimmetListele(yil, muhasebe, harcamaBirimi.Replace(".", ""), belgeNo, belgeTur);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TarihceBilgisi tarih = new TarihceBilgisi();

                    tarih.yil = VeriErisim.DegerAl(r, 0, 0);
                    tarih.muhasebe = VeriErisim.DegerAl(r, 1, "");
                    tarih.harcamaBirimi = VeriErisim.DegerAl(r, 2, "");
                    tarih.belgeNo = VeriErisim.DegerAl(r, 3, "");
                    tarih.islemYapan = VeriErisim.DegerAl(r, 5, "");
                    tarih.islemTarih = VeriErisim.DegerAl(r, 6);
                    tarih.islem = VeriErisim.DegerAl(r, 7, "");

                    iVAN.objeler.Add(tarih);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return iVAN;
        }

        public ObjectArray Rapor(Kullanici kullanan, string muhasebe, string harcamaBirimi, TNSDateTime baslamaTarihi, TNSDateTime bitisTarihi)
        {
            ObjectArray bilgiler = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTarihce to = new TABTarihce(ve);
            IDataReader r = to.Rapor(muhasebe, harcamaBirimi.Replace(".", ""), baslamaTarihi, bitisTarihi);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TarihceBilgisi tarih = new TarihceBilgisi();

                    tarih.yil = VeriErisim.DegerAl(r, (int)TABTarihce.ENUMAlanlar.YIL, 0);
                    tarih.muhasebe = VeriErisim.DegerAl(r, (int)TABTarihce.ENUMAlanlar.MUHASEBEKOD, "");
                    tarih.harcamaBirimi = VeriErisim.DegerAl(r, (int)TABTarihce.ENUMAlanlar.HARCAMABIRIMKOD, "");
                    tarih.belgeNo = VeriErisim.DegerAl(r, (int)TABTarihce.ENUMAlanlar.FISNO, "");
                    tarih.islemYapan = VeriErisim.DegerAl(r, (int)TABTarihce.ENUMAlanlar.ISLEMYAPAN, "");
                    tarih.islemTarih = VeriErisim.DegerAl(r, (int)TABTarihce.ENUMAlanlar.ISLEMTARIH);
                    tarih.islem = VeriErisim.DegerAl(r, (int)TABTarihce.ENUMAlanlar.ISLEM, "");
                    tarih.tasinirIslemFormKod = VeriErisim.DegerAl(r, (int)TABTarihce.ENUMAlanlar.TASISLEMFORMKOD, "");
                    tarih.ipAdresi = VeriErisim.DegerAl(r, (int)TABTarihce.ENUMAlanlar.IPADRESI, "");

                    bilgiler.objeler.Add(tarih);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgiler;
        }
    }
}
