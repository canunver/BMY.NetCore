using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Süreç parça bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCSurecParca
    {
        /// <summary>
        /// Arguman olarak verilen, süreç parça bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, SurecParca bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            //string hata = "";

            //if (hata != "")
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = hata;
            //    return sonuc;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABSurecParca to = new TABSurecParca(ve);

            sonuc = to.Kaydet(bilgi);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, süreç parça bilgisini veritabanından okur 
        /// ve SurecParca nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, süreç parça bilgisini SurecParca nesnesi olarak tutan nesne</returns>
        public SurecParca ListeleTek(Kullanici kullanan, SurecParca kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            if (!HRCYetkiKontrol.GormeYetkisiVarMi(kullanan))
            {
                bilgi.sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00269;
                bilgi.sonuc.islemSonuc = false;
                return kriter;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABSurecParca to = new TABSurecParca(ve);
            IDataReader r = to.Listele(kriter);

            SurecParca sr = new SurecParca();
            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    sr.yil = VeriErisim.DegerAl(r, 0, 0);
                    sr.surecNo = VeriErisim.DegerAl(r, 1, "");
                    sr.isKod = VeriErisim.DegerAl(r, 2, 0);
                    sr.parcaKod = VeriErisim.DegerAl(r, 3, 0);
                    sr.siraNo = VeriErisim.DegerAl(r, 4, 0);
                    sr.ihaleUsulu = VeriErisim.DegerAl(r, 5, 0);
                    sr.ihaleMaddesi = VeriErisim.DegerAl(r, 6, 0);
                    sr.duzey = VeriErisim.DegerAl(r, 7, 0);
                    sr.zorunlu = VeriErisim.DegerAl(r, 8, 0);

                    Parca parca = new Parca();
                    parca.kod = VeriErisim.DegerAl(r, 9, 0);
                    parca.ad = VeriErisim.DegerAl(r, 10, "");
                    parca.sayfa = VeriErisim.DegerAl(r, 11, "");
                    parca.yukseklik = VeriErisim.DegerAl(r, 12, 0);
                    parca.genislik = VeriErisim.DegerAl(r, 13, 0);
                    parca.duzey = VeriErisim.DegerAl(r, 7, 0);
                    parca.zorunlu = VeriErisim.DegerAl(r, 8, 0);

                    sr.parcalar.objeler.Add(parca);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return sr;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, süreç parça bilgisini veritabanından okur 
        /// ve SurecParca nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, süreç parça listesini SurecParca nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleCok(Kullanici kullanan, SurecParca kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            if (!HRCYetkiKontrol.GormeYetkisiVarMi(kullanan))
            //if (!HRCYetkiKontrol.YoneticiMi(kullanan))
            {
                bilgi.sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00269;
                bilgi.sonuc.islemSonuc = false;
                return bilgi;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABSurecParca to = new TABSurecParca(ve);
            IDataReader r = to.Listele(kriter);


            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    SurecParca sr = new SurecParca();
                    sr.yil = VeriErisim.DegerAl(r, 0, 0);
                    sr.surecNo = VeriErisim.DegerAl(r, 1, "");
                    sr.isKod = VeriErisim.DegerAl(r, 2, 0);
                    sr.parcaKod = VeriErisim.DegerAl(r, 3, 0);
                    sr.siraNo = VeriErisim.DegerAl(r, 4, 0);
                    sr.ihaleUsulu = VeriErisim.DegerAl(r, 5, 0);
                    sr.ihaleMaddesi = VeriErisim.DegerAl(r, 6, 0);
                    sr.duzey = VeriErisim.DegerAl(r, 7, 0);
                    sr.zorunlu = VeriErisim.DegerAl(r, 8, 0);

                    Parca parca = new Parca();
                    parca.kod = VeriErisim.DegerAl(r, 9, 0);
                    parca.ad = VeriErisim.DegerAl(r, 10, "");
                    parca.sayfa = VeriErisim.DegerAl(r, 11, "");
                    parca.yukseklik = VeriErisim.DegerAl(r, 12, 0);
                    parca.genislik = VeriErisim.DegerAl(r, 13, 0);
                    parca.duzey = VeriErisim.DegerAl(r, 7, 0);
                    parca.zorunlu = VeriErisim.DegerAl(r, 8, 0);

                    sr.parcalar.objeler.Add(parca);
                    bilgi.objeler.Add(sr);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen, süreç parça bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanici">The kullanan.</param>
        /// <param name="surecParca">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Sil(Kullanici kullanici, SurecParca surecParca)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanici));

            ve.BeginTransaction();

            TABSurecParca tab = new TABSurecParca(ve);

            sonuc = tab.Sil(surecParca);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00018;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}
