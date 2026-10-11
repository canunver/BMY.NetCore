using System.Data;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TRM.IsKurallari
{
    public class TRMHibeMakineOzelligi
    {
        /// <summary>
        /// Arguman olarak verilen, malzeme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, Malzeme bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            //if (!TRMYetkiKontrol.YoneticiMi(kullanan))
            //{
            //    sonuc.hataStr = "TRM00013";
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}

            string hata = "";

            if (bilgi.ad == "")
                hata += "Makine Özelliği bilgisi boş bırakılamaz" + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABTRMHIBEMAKINAOZELLIGI to = new TABTRMHIBEMAKINAOZELLIGI(ve);
            ve.BeginTransaction();

            if (bilgi.kod == 0)
                bilgi.kod = to.EnBuyukKod();

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
        /// Arguman olarak verilen, malzeme bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Sil(Kullanici kullanan, Malzeme bilgi)
        {
            Sonuc sonuc = new Sonuc();

            //if (!TRMYetkiKontrol.YoneticiMi(kullanan))
            //{
            //    sonuc.hataStr = TNS.TRM.HarcamaSurecleriResI.TRM00013;
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}

            if (bilgi.kod == 0)
                sonuc.hataStr = "Kod bilgisi boş bırakılamaz" + "<br>";

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABTRMHIBEMAKINAOZELLIGI to = new TABTRMHIBEMAKINAOZELLIGI(ve);

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
        /// Arguman olarak verilen kriterlere bağlı olarak, malzeme bilgisini veritabanından okur 
        /// ve Malzeme nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, malzeme listesini Malzeme nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, Malzeme kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABTRMHIBEMAKINAOZELLIGI to = new TABTRMHIBEMAKINAOZELLIGI(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Malzeme f = new Malzeme();

                    f.kod = VeriErisim.DegerAl(r, 0, 0);
                    f.ad = VeriErisim.DegerAl(r, 1, string.Empty);
                    f.malzemeGrubu.kod = VeriErisim.DegerAl(r, 2, 0);
                    f.siraNo = VeriErisim.DegerAl(r, 3, 0);
                    f.cokluSecim = VeriErisim.DegerAl(r, 4, 0);
                    f.cokluSecimIcerik = VeriErisim.DegerAl(r, 5, string.Empty);
                    f.malzemeGrubu.ad = VeriErisim.DegerAl(r, 6, string.Empty);                    

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        //public static decimal ReferansFiyatListele(int malzemeKod)
        //{
        //    decimal bilgi = decimal.Zero;

        //    Malzeme kriter = new Malzeme();
        //    kriter.kod = malzemeKod;

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
        //    TABMalzeme to = new TABMalzeme(ve);
        //    IDataReader r = to.Listele(kriter);

        //    if (to.Hata() == "")
        //    {
        //        while (r.Read())
        //        {
        //            bilgi = VeriErisim.DegerAl(r, 4, decimal.Zero);
        //        }
        //    }

        //    r.Close();

        //    ve.VeriErisimKapat();

        //    return bilgi;
        //}
    }
}
