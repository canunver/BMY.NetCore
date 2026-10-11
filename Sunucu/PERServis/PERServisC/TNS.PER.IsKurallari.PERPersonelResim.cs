using System;
using System.Data;
using TNS.PER;
using TNS.PER.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;

namespace TNS.PER.IsKurallari
{
    public class PERPersonelResim
    {
        /// <summary>
        /// Kullanıcı yetkisini göz önüne alarak girilen bilgilere bağlı olarak yeni bir kayıt ekler veya varolan kaydı günceller.
        /// </summary>
        /// <param name="ve">Bağlantıya ait bilgileri içerir</param>
        /// <param name="resim">The resim.</param>
        /// <returns>
        /// Sonuc yapısı içerisindeki; islemSonuc:komut çalışmışsa true, çalışmamışsa false; hataStr:Hata olmuşsa, hata bilgisini, döndürür.
        /// </returns>
        public static Sonuc Kaydet(VeriErisim ve, PersonelResim resim)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            //sonuc = Kontrol(ENUMKontrolTip.KAYDET, resim);
            //if (!sonuc.islemSonuc)
            //    return sonuc;

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(null));
            ve.BeginTransaction();

            TABPersonelResim to = new TABPersonelResim(ve);

            if (to.VarMi(resim))
                sonuc = to.Guncelle(resim);
            else
                sonuc = to.Kaydet(resim);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Girilen veriye uygun bilgileri listeleme işlemini yapar.
        /// </summary>
        /// <param name="ve">Bağlantıya ait bilgileri içerir</param>
        /// <param name="tcKimklikNo">The tc kimklik no.</param>
        /// <returns>
        /// ObjectArray yapısı içinde, uygun olan verileri döndürür.
        /// </returns>
        public static ObjectArray Getir(VeriErisim ve, string tcKimklikNo)
        {
            ObjectArray liste = new ObjectArray();

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(null));
            TABPersonelResim to = new TABPersonelResim(ve);
            IDataReader dr = null;

            try
            {
                dr = to.Getir(tcKimklikNo);
            }
            catch (Exception e)
            {
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = e.Message;
                ve.VeriErisimKapat();
                return liste;
            }

            while (dr.Read())
            {
                PersonelResim r = new PersonelResim();
                //r.kod = VeriErisim.DegerAl(dr, (int)TABResim.ENUMAlanlar.KOD, "");
                r.resim = (byte[])dr.GetValue(0); // Buraya tekrardan bak!!!

                liste.Ekle(r);
            }

            ve.VeriErisimKapat();
            return liste;
        }

        /// <summary>
        /// Girilen veri ile uyuşan verinin silinmesini gerçekleştirir.
        /// </summary>
        /// <param name="ve">Bağlantıya ait bilgileri içerir</param>
        /// <param name="resim">Girilen bilgilere bağlı olarak oluşturulmuş, işleme sokulacak verileri içerir</param>
        /// <returns>
        /// Sonuc yapısı içerisindeki; islemSonuc:komut çalışmışsa true, çalışmamışsa false; hataStr:Hata olmuşsa, hata bilgisini, döndürür.
        /// </returns>
        public static Sonuc Sil(VeriErisim ve, PersonelResim resim)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(null));
            ve.BeginTransaction();

            TABPersonelResim to = new TABPersonelResim(ve);

            sonuc = to.Sil(resim);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }
    }
}
