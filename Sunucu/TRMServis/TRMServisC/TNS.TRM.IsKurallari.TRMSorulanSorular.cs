using System.Data;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TRM.IsKurallari
{
    public class TRMSorulanSorular
    {
        /// <summary>
        /// Arguman olarak verilen, malzeme grubu bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, SorulanSorular bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = "";

            if (bilgi.soru == "")
                hata += "Soru bilgisi boş bırakılamaz" + "<br>";
            if (bilgi.cevap == "")
                hata += "Cevap bilgisi boş bırakılamaz" + "<br>";
            if (bilgi.etapNo < 0)
                hata += "Etap No bilgisi boş bırakılamaz" + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABSorulanSorular to = new TABSorulanSorular(ve);
            ve.BeginTransaction();

            if (bilgi.kod == 0)
                bilgi.kod = KodUret(ve);

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
        public Sonuc Sil(Kullanici kullanan, SorulanSorular bilgi)
        {
            Sonuc sonuc = new Sonuc();

            //if (!TRMYetkiKontrol.YoneticiMi(kullanan))
            //{
            //    sonuc.hataStr = TNS.TRM.HarcamaSurecleriResI.TRM00013;
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}

            if (bilgi.kod < 0)
                sonuc.hataStr = "Kod bilgisi boş bırakılamaz" + "<br>";

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABSorulanSorular to = new TABSorulanSorular(ve);

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
        /// Arguman olarak verilen kriterlere bağlı olarak, sorulan sorular bilgisini veritabanından okur 
        /// ve YatirimKonu nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, soru listesini SorulanSorular nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, SorulanSorular kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABSorulanSorular to = new TABSorulanSorular(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    SorulanSorular f = new SorulanSorular();

                    f.kod = VeriErisim.DegerAl(r, 0, 0);
                    f.etapNo = VeriErisim.DegerAl(r, 1, 0);
                    f.siraNo = VeriErisim.DegerAl(r, 2, 0);
                    f.tarih = VeriErisim.DegerAl(r, 3);
                    f.soru = VeriErisim.DegerAl(r, 4, string.Empty);
                    f.cevap = VeriErisim.DegerAl(r, 5, string.Empty);
                    f.aktif = VeriErisim.DegerAl(r, 6, 0);
                    f.tip = VeriErisim.DegerAl(r, 7, 0);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        private int KodUret(VeriErisim ve)
        {
            TABGenel tg = new TABGenel(ve);

            string sayacKod = "SORULANSORULAR";

            return tg.BelgeNoAlInt(sayacKod, "000000");
        }
    }
}
