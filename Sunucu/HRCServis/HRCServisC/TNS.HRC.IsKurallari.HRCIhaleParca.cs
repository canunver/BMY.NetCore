using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// İlişkilendirilmiş ihale ve parça bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCIhaleParca
    {
        /// <summary>
        /// İlişkilendirilmiş ihale ve parça bilgilerini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek parça bilgilerini tutan IhaleParca nesnesi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, IhaleParca bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (bilgi.ihaleUsulu == 0)
                hata = TNS.HRC.HarcamaSurecleriResI.HRC00147 + "<br>";
            if (bilgi.ihaleMaddesi == 0)
                hata = TNS.HRC.HarcamaSurecleriResI.HRC00197 + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIhaleParca to = new TABIhaleParca(ve);
            ve.BeginTransaction();

            sonuc = to.Sil(bilgi.ihaleUsulu, bilgi.ihaleMaddesi, bilgi.komisyon, bilgi.tur);

            if (sonuc.islemSonuc)
                sonuc = to.Kaydet(bilgi);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// İlişkilendirilmiş ihale ve parça bilgilerini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="list">Kaydedilecek parça bilgilerini IhaleParcaBag nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Guncelle(Kullanici kullanan, ObjectArray list)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YoneticiMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIhaleParca to = new TABIhaleParca(ve);
            ve.BeginTransaction();

            sonuc = to.Sil(0, 0, 0, 0);

            if (sonuc.islemSonuc)
                sonuc = to.Guncelle(list);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere bağlı ilişkili parçaları veritabanından okur 
        /// ve IhaleParca nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="ihaleUsul">Listelemede kriter olarak kullanılacak ihalu usul kodu</param>
        /// <param name="ihaleMadde">Listelemede kriter olarak kullanılacak ihale madde kodu</param>
        /// <param name="komisyon">Listelemede kriter olarak kullanılacak komisyon var mı yok mu bilgisi</param>
        /// <param name="tur">Listelemede kriter olarak kullanılacak işlem türü bilgisi</param>
        /// <returns>Verilen kriterlere ait ihale parça bilgilerini tutan nesne</returns>
        public IhaleParca Listele(Kullanici kullanan, int ihaleUsul, int ihaleMadde, int komisyon, int tur)
        {
            IhaleParca bilgi = new IhaleParca();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABIhaleParca to = new TABIhaleParca(ve);
            IDataReader r = to.Listele(ihaleUsul, ihaleMadde, komisyon, tur);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    bilgi.ihaleUsulu = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.ihaleMaddesi = VeriErisim.DegerAl(r, 1, 0);
                    bilgi.komisyon = VeriErisim.DegerAl(r, 2, 0);
                    bilgi.tur = VeriErisim.DegerAl(r, 3, 0);

                    Parca pBilgi = new Parca();
                    pBilgi.kod = VeriErisim.DegerAl(r, 8, 0);
                    pBilgi.ad = VeriErisim.DegerAl(r, 9, "");
                    pBilgi.sayfa = VeriErisim.DegerAl(r, 10, "");
                    pBilgi.yukseklik = VeriErisim.DegerAl(r, 11, 0);
                    pBilgi.genislik = VeriErisim.DegerAl(r, 12, 0);
                    pBilgi.duzey = VeriErisim.DegerAl(r, 6, 0);
                    pBilgi.zorunlu = VeriErisim.DegerAl(r, 7, 0);

                    bilgi.parcalar.objeler.Add(pBilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Güncelleme servisinde kullanılmak üzere ilişkilendirilmiş tüm ihale ve parça listesini veritabanından okur
        /// ve IhaleParcaBag nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <returns>İlişkilendirilmiş tüm ihale ve parça listesini IhaleParcaBag nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray GuncellemeListesi(Kullanici kullanan)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABIhaleParca to = new TABIhaleParca(ve);
            IDataReader r = to.Listele(0, 0, -1, -1);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    IhaleParcaBag pBilgi = new IhaleParcaBag();
                    pBilgi.ihaleUsulu = VeriErisim.DegerAl(r, 0, 0);
                    pBilgi.ihaleMaddesi = VeriErisim.DegerAl(r, 1, 0);
                    pBilgi.komisyon = VeriErisim.DegerAl(r, 2, 0);
                    pBilgi.tur = VeriErisim.DegerAl(r, 3, 0);
                    pBilgi.kod = VeriErisim.DegerAl(r, 4, 0);
                    pBilgi.sira = VeriErisim.DegerAl(r, 5, 0);
                    pBilgi.duzey = VeriErisim.DegerAl(r, 6, 0);
                    pBilgi.zorunlu = VeriErisim.DegerAl(r, 7, 0);

                    liste.objeler.Add(pBilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return liste;
        }

    }
}
