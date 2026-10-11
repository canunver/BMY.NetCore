using System.Data;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TRM.IsKurallari
{
    public class TRMDosya
    {
        /// <summary>
        /// Arguman olarak verilen, Rapor Türü bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, EklenenDosya bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = "";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABDosya to = new TABDosya(ve);

            ve.BeginTransaction();

            sonuc = to.Kaydet(bilgi);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
            {
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu" + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, Rapor Türü bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Sil(Kullanici kullanan, string kod)
        {
            Sonuc sonuc = new Sonuc();

            if (string.IsNullOrEmpty(kod))
                sonuc.hataStr = "Kod bilgisi boş bırakılamaz" + "<br>";

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABDosya to = new TABDosya(ve);

            ve.BeginTransaction();

            sonuc = to.Sil(kod);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Bilgi silinirken bir hata oluştu" + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, Kaynak Türü bilgisini veritabanından okur 
        /// ve KayankTuru nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, Kaynak Türü listesini KayankTuru nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, EklenenDosya kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            if (string.IsNullOrEmpty(kriter.kayitNo))
                return bilgi;
            

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABDosya to = new TABDosya(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    EklenenDosya f = new EklenenDosya();

                    f.kod = VeriErisim.DegerAl(r, 0, string.Empty);
                    f.ad = VeriErisim.DegerAl(r, 1, string.Empty);
                    f.dosyaTarihi = VeriErisim.DegerAl(r, 2);
                    f.boyut = VeriErisim.DegerAl(r, 3, 0);
                    f.tip = VeriErisim.DegerAl(r, 4, string.Empty);
                    f.tur = VeriErisim.DegerAl(r, 5, string.Empty);
                    f.kayitNo = VeriErisim.DegerAl(r, 6, string.Empty);
                    f.evrakNo = VeriErisim.DegerAl(r, 7, string.Empty);
                    f.kayitEden = VeriErisim.DegerAl(r, 8, string.Empty);
                    f.kayitTarihi = VeriErisim.DegerAl(r, 9);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }
    }
}
