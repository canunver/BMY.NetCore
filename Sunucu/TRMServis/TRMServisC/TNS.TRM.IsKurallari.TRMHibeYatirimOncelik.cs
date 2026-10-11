using System.Data;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TRM.IsKurallari
{
    public class TRMHibeYatirimOncelik
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

            //if (!TRMYetkiKontrol.YoneticiMi(kullanan))
            //{
            //    sonuc.hataStr = "TRM00013";
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeYatirimOncelik to = new TABHibeYatirimOncelik(ve);
            ve.BeginTransaction();

            string oncekiIlKod = "";
            foreach (HibeYatirimOncelik o in bilgi.objeler)
            {
                if (oncekiIlKod != o.ilKod)
                    sonuc = to.Sil(o);

                if (sonuc.islemSonuc && o.konu.kod != "")
                    sonuc = to.Kaydet(o);

                oncekiIlKod = o.ilKod;
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
        public ObjectArray Listele(Kullanici kullanan, HibeYatirimOncelik kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeYatirimOncelik to = new TABHibeYatirimOncelik(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    HibeYatirimOncelik f = new HibeYatirimOncelik();

                    f.etapNo = VeriErisim.DegerAl(r, 0, 0);
                    f.ilKod = VeriErisim.DegerAl(r, 1, string.Empty);
                    f.siraNo = VeriErisim.DegerAl(r, 2, 0);
                    f.konu.kod = VeriErisim.DegerAl(r, 3, "");
                    f.tur = VeriErisim.DegerAl(r, 4, 0);
                    f.konu.kisaAd = VeriErisim.DegerAl(r, 5, "");

                    bilgi.objeler.Add(f);
                }
                r.Close();
            }

            ve.VeriErisimKapat();

            return bilgi;
        }
    }
}
