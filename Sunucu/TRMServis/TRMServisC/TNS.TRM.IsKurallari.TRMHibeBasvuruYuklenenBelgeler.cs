using System.Data;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TRM.IsKurallari
{
    public class TRMHibeBasvuruYuklenenBelgeler
    {
        /// <summary>
        /// Arguman olarak verilen, malzeme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, BasvurudaYuklenenBelgeler bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeBasvuruYuklenenBelgeler to = new TABHibeBasvuruYuklenenBelgeler(ve);

            ve.BeginTransaction();

            sonuc = to.Kaydet(bilgi);

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = bilgi.kod.ToString();
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
        /// Arguman olarak verilen, ceza bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Sil(Kullanici kullanan, BasvurudaYuklenenBelgeler bilgi)
        {
            Sonuc sonuc = new Sonuc();

            if (string.IsNullOrEmpty(bilgi.kod))
                sonuc.hataStr = "Kod bilgisi boş bırakılamaz" + "<br>";

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeBasvuruYuklenenBelgeler to = new TABHibeBasvuruYuklenenBelgeler(ve);

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
        /// ve BasvuruBelgeTanim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, malzeme listesini Malzeme nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, BasvurudaYuklenenBelgeler kriter, bool eksikBelgeler = false)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeBasvuruYuklenenBelgeler to = new TABHibeBasvuruYuklenenBelgeler(ve);

            bool[] belgeIslemler;
            if (eksikBelgeler)
                belgeIslemler = new bool[] { false, true };
            else
                belgeIslemler = new bool[] { false };

            foreach (bool eksikBelgeDurum in belgeIslemler)
            {
                IDataReader r = to.Listele(kriter, eksikBelgeDurum);

                if (to.Hata() == "")
                {
                    while (r.Read())
                    {
                        BasvurudaYuklenenBelgeler f = new BasvurudaYuklenenBelgeler();

                        f.kod = VeriErisim.DegerAl(r, 0, string.Empty);
                        f.yil = VeriErisim.DegerAl(r, 1, 0);
                        f.etapNo = VeriErisim.DegerAl(r, 2, 0);
                        f.basvuruNo = VeriErisim.DegerAl(r, 3, string.Empty);
                        f.belgeKod = VeriErisim.DegerAl(r, 4, 0);
                        f.siraNo = VeriErisim.DegerAl(r, 5, 0);
                        f.evrEvrakNo = VeriErisim.DegerAl(r, 6, string.Empty);
                        f.yukleyen = VeriErisim.DegerAl(r, 7, string.Empty);
                        f.dosyaAdi = VeriErisim.DegerAl(r, 8, string.Empty);
                        f.hibeKod = VeriErisim.DegerAl(r, 9, 0);
                        f.hibeTur = VeriErisim.DegerAl(r, 10, 0);
                        f.sablonAdi = VeriErisim.DegerAl(r, 11, "");
                        f.belgeTanimi.belgeKisaAdi = VeriErisim.DegerAl(r, 12, "");

                        bilgi.objeler.Add(f);
                    }
                    r.Close();
                }
            }
            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, malzeme bilgisini veritabanından okur 
        /// ve BasvuruBelgeTanim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, malzeme listesini Malzeme nesne koleksiyonu olarak tutan nesne</returns>
        //public ObjectArray ListeleBasvurununYuklenenBelgeleri(Kullanici kullanan, BasvurudaYuklenenBelgeler kriter)
        //{
        //    ObjectArray bilgi = new ObjectArray();

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

        //    TABHibeBasvuruYuklenenBelgeler to = new TABHibeBasvuruYuklenenBelgeler(ve);
        //    IDataReader r = to.ListeleBasvurununYuklenenBelgeleri(kriter);

        //    if (to.Hata() == "")
        //    {
        //        while (r.Read())
        //        {
        //            BasvurudaYuklenenBelgeler f = new BasvurudaYuklenenBelgeler();

        //            f.kod = VeriErisim.DegerAl(r, 0, string.Empty);
        //            f.yil = VeriErisim.DegerAl(r, 1, 0);
        //            f.etapNo = VeriErisim.DegerAl(r, 2, 0);
        //            f.basvuruNo = VeriErisim.DegerAl(r, 3, string.Empty);
        //            f.belgeKod = VeriErisim.DegerAl(r, 4, 0);
        //            f.siraNo = VeriErisim.DegerAl(r, 5, 0);
        //            f.evrEvrakNo = VeriErisim.DegerAl(r, 6, string.Empty);
        //            f.yukleyen = VeriErisim.DegerAl(r, 7, string.Empty);
        //            f.dosyaAdi = VeriErisim.DegerAl(r, 8, string.Empty);
        //            f.hibeKod = VeriErisim.DegerAl(r, 9, 0);
        //            f.hibeTur = VeriErisim.DegerAl(r, 10, 0);
        //            f.sablonAdi = VeriErisim.DegerAl(r, 11, "");

        //            bilgi.objeler.Add(f);
        //        }
        //        r.Close();
        //    }

        //    ve.VeriErisimKapat();

        //    return bilgi;
        //}
    }
}
