using System.Data;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TRM.IsKurallari
{
    public class TRMYatirimKonuNitelikKisit
    {
        /// <summary>
        /// Arguman olarak verilen, malzeme grubu bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, YatirimKonuNitelikKisit bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = "";

            if (bilgi.etapNo == 0)
                hata += "Etap bilgisi boş bırakılamaz" + "<br>";
            if (bilgi.ilKod == "")
                hata += "İl bilgisi boş bırakılamaz" + "<br>";
            if (bilgi.konuKod == "")
                hata += "Konu bilgisi boş bırakılamaz" + "<br>";
            if (bilgi.nitelikKod == 0)
                hata += "Nitelik bilgisi boş bırakılamaz" + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABYatirimKonuNitelikKisit to = new TABYatirimKonuNitelikKisit(ve);
            ve.BeginTransaction();

            if (bilgi.kod == "")
                bilgi.kod = System.Guid.NewGuid().ToString();

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
        /// Arguman olarak verilen, malzeme grubu bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Sil(Kullanici kullanan, YatirimKonuNitelikKisit bilgi)
        {
            Sonuc sonuc = new Sonuc();

            //if (!TRMYetkiKontrol.YoneticiMi(kullanan))
            //{
            //    sonuc.hataStr = TNS.TRM.HarcamaSurecleriResI.TRM00013;
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}

            if (bilgi.kod == "")
                sonuc.hataStr = "Kod bilgisi boş bırakılamaz" + "<br>";

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABYatirimKonuNitelikKisit to = new TABYatirimKonuNitelikKisit(ve);

            ve.BeginTransaction();

            sonuc = to.Sil(bilgi);

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
        /// Arguman olarak verilen kriterlere bağlı olarak, malzeme grubu bilgisini veritabanından okur 
        /// ve YatirimKonuNitelikKisit nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, malzeme grubu listesini YatirimKonuNitelikKisit nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, YatirimKonuNitelikKisit kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABYatirimKonuNitelikKisit to = new TABYatirimKonuNitelikKisit(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    YatirimKonuNitelikKisit f = new YatirimKonuNitelikKisit();

                    f.kod = VeriErisim.DegerAl(r, 0, "");
                    f.etapNo = VeriErisim.DegerAl(r, 1, 0);
                    f.ilKod = VeriErisim.DegerAl(r, 2, "");
                    f.ilceKod = VeriErisim.DegerAl(r, 3, "");
                    f.konuKod = VeriErisim.DegerAl(r, 4, "");
                    f.altKonuKod = VeriErisim.DegerAl(r, 5, "");
                    f.nitelikKod = VeriErisim.DegerAl(r, 6, 0);

                    f.ilAdi = VeriErisim.DegerAl(r, 7, "");
                    f.ilceAdi = VeriErisim.DegerAl(r, 8, "");
                    f.konuAdi = VeriErisim.DegerAl(r, 9, "");
                    f.altKonuAdi = VeriErisim.DegerAl(r, 10, "");
                    f.nitelikAdi= VeriErisim.DegerAl(r, 11, "");


                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }
    }
}
