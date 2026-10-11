using System.Data;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TRM.IsKurallari
{
    public class TRMKoy
    {
        /// <summary>
        /// Arguman olarak verilen, Koy bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, Koy bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            //if (!TRMYetkiKontrol.YoneticiMi(kullanan))
            //{
            //    sonuc.hataStr = "TRM00013";
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}

            string hata = "";

            if (bilgi.adi == "")
                hata += "Ad bilgisi boş bırakılamaz" + "<br>";

            if (bilgi.ilKod == "")
                hata += "İl bilgisi boş bırakılamaz" + "<br>";

            if (bilgi.ilceKod == "")
                hata += "İlçe bilgisi boş bırakılamaz" + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABKoy to = new TABKoy(ve);
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

        public Sonuc KoyleriKapat(Kullanici kullanan, Koy bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABKoy to = new TABKoy(ve);
            ve.BeginTransaction();

            sonuc = to.KoyleriKapat(bilgi.ilceMernisNo, bilgi.ilceKod);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, Koy bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Sil(Kullanici kullanan, Koy bilgi)
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
            TABKoy to = new TABKoy(ve);

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
        /// Arguman olarak verilen kriterlere bağlı olarak, Koy bilgisini veritabanından okur 
        /// ve Koy nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, Koy listesini Koy nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, Koy kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABKoy to = new TABKoy(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Koy f = new Koy();

                    f.kod = VeriErisim.DegerAl(r, 0, 0);
                    f.ilKod = VeriErisim.DegerAl(r, 1, "");
                    f.ilceKod = VeriErisim.DegerAl(r, 2, "");
                    f.ilMernisNo = VeriErisim.DegerAl(r, 3, 0);
                    f.ilceMernisNo = VeriErisim.DegerAl(r, 4, 0);
                    f.bucakMernisNo = VeriErisim.DegerAl(r, 5, 0);
                    f.koyMernisNo = VeriErisim.DegerAl(r, 6, 0);
                    f.adi = VeriErisim.DegerAl(r, 7, "");
                    f.koordinat = VeriErisim.DegerAl(r, 8, "");
                    f.kapali = VeriErisim.DegerAl(r, 9, 0);
                    f.ilAdi = VeriErisim.DegerAl(r, 10, "");
                    f.ilceAdi = VeriErisim.DegerAl(r, 11, "");

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }


        public ObjectArray CografyaListele(string kod, bool sadeceDetay)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABKoy to = new TABKoy(ve);
            IDataReader r = to.CografyaListele(kod, sadeceDetay);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Koy f = new Koy();

                    f.ilKod = VeriErisim.DegerAl(r, 0, "");
                    f.adi = VeriErisim.DegerAl(r, 1, "");
                    f.koordinat = VeriErisim.DegerAl(r, 8, "");

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }
    }
}
