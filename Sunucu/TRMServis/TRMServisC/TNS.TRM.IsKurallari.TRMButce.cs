using System.Data;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TRM.IsKurallari
{
    public class TRMButce
    {
        /// <summary>
        /// Arguman olarak verilen, malzeme grubu bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            //yatirimtur gelmeli

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABButce to = new TABButce(ve);
            ve.BeginTransaction();

            foreach (Butce o in bilgi.objeler)
            {
                sonuc = to.Sil(o);

                if (sonuc.islemSonuc)
                    sonuc = to.Kaydet(o);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Bilgiler başarıyla kayıt edildi";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu" + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, malzeme grubu bilgisini veritabanından okur 
        /// ve YatirimKonu nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, malzeme grubu listesini YatirimKonu nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, Butce kriter)
        {
            //Yatirimtur gelmeli
            
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABButce to = new TABButce(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Butce f = new Butce();

                    f.etapNo = VeriErisim.DegerAl(r, 0, 0);
                    f.ilKod = VeriErisim.DegerAl(r, 1, string.Empty);
                    f.tip = VeriErisim.DegerAl(r, 2, 0);
                    f.yatirimOncelikTur = VeriErisim.DegerAl(r, 3, 0);
                    f.butce = VeriErisim.DegerAl(r, 4, 0.0);
                    f.katsayi = VeriErisim.DegerAl(r, 5, 0.0);
                    f.talepEdilen = VeriErisim.DegerAl(r, 6, 0.0);
                    f.ilAd = VeriErisim.DegerAl(r, 7, "");

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public static double ButceBul(Kullanici kullanan, Butce kriter)
        {
            //yatirimtur gelmeli

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABButce to = new TABButce(ve);
            double butce = to.ButceBul(kriter);
            ve.VeriErisimKapat();
            return butce;
        }
    }
}
