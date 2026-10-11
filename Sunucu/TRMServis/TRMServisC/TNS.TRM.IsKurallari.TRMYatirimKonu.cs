using System;
using System.Data;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TRM.IsKurallari
{
    public class TRMYatirimKonu
    {
        /// <summary>
        /// Arguman olarak verilen, malzeme grubu bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, YatirimKonu bilgi)
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
                hata += "Ad bilgisi boş bırakılamaz" + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABYatirimKonu to = new TABYatirimKonu(ve);
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
        /// Arguman olarak verilen, malzeme grubu bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Sil(Kullanici kullanan, YatirimKonu bilgi)
        {
            Sonuc sonuc = new Sonuc();

            //if (!TRMYetkiKontrol.YoneticiMi(kullanan))
            //{
            //    sonuc.hataStr = TNS.TRM.HarcamaSurecleriResI.TRM00013;
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}

            if (string.IsNullOrEmpty(bilgi.kod))
                sonuc.hataStr = "Kod bilgisi boş bırakılamaz" + "<br>";

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABYatirimKonu to = new TABYatirimKonu(ve);

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
        /// ve YatirimKonu nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, malzeme grubu listesini YatirimKonu nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, YatirimKonu kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABYatirimKonu to = new TABYatirimKonu(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    YatirimKonu f = new YatirimKonu();

                    f.kod = VeriErisim.DegerAl(r, 0, "");
                    f.ad = VeriErisim.DegerAl(r, 1, string.Empty);
                    f.kisaAd = VeriErisim.DegerAl(r, 2, string.Empty);
                    f.aktif = VeriErisim.DegerAl(r, 3, 0);
                    f.nitelik = VeriErisim.DegerAl(r, 4, "");

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public Sonuc KaydetKonuIl(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = "";
            int etapNo = 0;
            string ilKod = "";
            foreach (YatirimKonuIl bilgi in liste.objeler)
            {
                if (bilgi.etapNo == 0)
                    hata += "Etap No bilgisi boş bırakılamaz" + "<br>";

                if (bilgi.ilKod == "")
                    hata += "İl bilgisi boş bırakılamaz" + "<br>";

                if (hata != "")
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = hata;
                    return sonuc;
                }

                etapNo = bilgi.etapNo;
                ilKod = bilgi.ilKod;
            }


            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABYatirimKonu to = new TABYatirimKonu(ve);
            ve.BeginTransaction();

            sonuc = to.SilKonuIl(etapNo, ilKod);
            foreach (YatirimKonuIl bilgi in liste.objeler)
            {
                sonuc = to.KaydetKonuIl(bilgi);
                if (!sonuc.islemSonuc)
                    break;
            }

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

        public ObjectArray ListeleKonuIl(Kullanici kullanan, YatirimKonuIl kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABYatirimKonu to = new TABYatirimKonu(ve);
            IDataReader r = to.ListeleKonuIl(kriter.etapNo, kriter.ilKod);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    YatirimKonuIl f = new YatirimKonuIl();

                    f.etapNo = VeriErisim.DegerAl(r, 0, 0);
                    f.ilKod = VeriErisim.DegerAl(r, 1, string.Empty);
                    f.konuKod = VeriErisim.DegerAl(r, 2, string.Empty);
                    f.konuAd = VeriErisim.DegerAl(r, 3, string.Empty);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public Sonuc AktarKonuIl(Kullanici kullanan, int eskiEtap, int yeniEtap)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = "";
            if (eskiEtap == 0)
                hata += "Eski Etap No bilgisi boş bırakılamaz" + "<br>";

            if (yeniEtap == 0)
                hata += "Yeni Etap No bilgisi boş bırakılamaz" + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABYatirimKonu to = new TABYatirimKonu(ve);
            ve.BeginTransaction();

            sonuc = to.SilKonuIl(yeniEtap, "");
            if (sonuc.islemSonuc)
                sonuc = to.AktarKonuIl(eskiEtap, yeniEtap);

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

        public Sonuc KaydetAltKonuIl(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = "";
            int etapNo = 0;
            string ilKod = "";
            foreach (YatirimAltKonuIl bilgi in liste.objeler)
            {
                if (bilgi.etapNo == 0)
                    hata += "Etap No bilgisi boş bırakılamaz" + "<br>";

                if (bilgi.ilKod == "")
                    hata += "İl bilgisi boş bırakılamaz" + "<br>";

                if (hata != "")
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = hata;
                    return sonuc;
                }

                etapNo = bilgi.etapNo;
                ilKod = bilgi.ilKod;
            }


            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABYatirimKonu to = new TABYatirimKonu(ve);
            ve.BeginTransaction();

            sonuc = to.SilAltKonuIl(etapNo, ilKod);
            foreach (YatirimAltKonuIl bilgi in liste.objeler)
            {
                sonuc = to.KaydetAltKonuIl(bilgi);
                if (!sonuc.islemSonuc)
                    break;
            }

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

        public ObjectArray ListeleAltKonuIl(Kullanici kullanan, YatirimAltKonuIl kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABYatirimKonu to = new TABYatirimKonu(ve);
            IDataReader r = to.ListeleAltKonuIl(kriter.etapNo, kriter.ilKod);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    YatirimAltKonuIl f = new YatirimAltKonuIl();

                    f.etapNo = VeriErisim.DegerAl(r, 0, 0);
                    f.ilKod = VeriErisim.DegerAl(r, 1, string.Empty);
                    f.altKonuKod = VeriErisim.DegerAl(r, 2, string.Empty);
                    f.altKonuAd = VeriErisim.DegerAl(r, 3, string.Empty);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public Sonuc AktarAltKonuIl(Kullanici kullanan, int eskiEtap, int yeniEtap)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = "";
            if (eskiEtap == 0)
                hata += "Eski Etap No bilgisi boş bırakılamaz" + "<br>";

            if (yeniEtap == 0)
                hata += "Yeni Etap No bilgisi boş bırakılamaz" + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABYatirimKonu to = new TABYatirimKonu(ve);
            ve.BeginTransaction();

            sonuc = to.SilAltKonuIl(yeniEtap, "");
            if (sonuc.islemSonuc)
                sonuc = to.AktarAltKonuIl(eskiEtap, yeniEtap);

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

        public Sonuc KaydetKonuUstSinir(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = "";
            int etapNo = 0;
            int nitelikKod = 0;
            foreach (YatirimKonuUstSinir bilgi in liste.objeler)
            {
                if (bilgi.etapNo == 0)
                    hata += "Etap No bilgisi boş bırakılamaz" + "<br>";
                if (bilgi.nitelikKod == 0)
                    hata += "Nitelik bilgisi boş bırakılamaz" + "<br>";

                if (hata != "")
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = hata;
                    return sonuc;
                }

                etapNo = bilgi.etapNo;
                nitelikKod = bilgi.nitelikKod;
            }


            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABYatirimKonu to = new TABYatirimKonu(ve);
            ve.BeginTransaction();

            sonuc = to.SilKonuUstSinir(etapNo, nitelikKod);
            foreach (YatirimKonuUstSinir bilgi in liste.objeler)
            {
                sonuc = to.KaydetKonuUstSinir(bilgi);
                if (!sonuc.islemSonuc)
                    break;
            }

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

        public ObjectArray ListeleKonuUstSinir(Kullanici kullanan, YatirimKonuUstSinir kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABYatirimKonu to = new TABYatirimKonu(ve);
            IDataReader r = to.ListeleKonuUstSinir(kriter.etapNo, kriter.nitelikKod, kriter.konuKod);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    YatirimKonuUstSinir f = new YatirimKonuUstSinir();

                    f.etapNo = VeriErisim.DegerAl(r, 0, 0);
                    f.konuKod = VeriErisim.DegerAl(r, 1, string.Empty);
                    f.nitelikKod = VeriErisim.DegerAl(r, 2, 0);
                    f.gercek = VeriErisim.DegerAl(r, 3, 0.0);
                    f.tuzel = VeriErisim.DegerAl(r, 4, 0.0);
                    f.konuAd = VeriErisim.DegerAl(r, 5, string.Empty);

                    //if (f.konuKod == "00.01") f.konuAd = "Yeni Tesis";
                    //if (f.konuKod == "00.02") f.konuAd = "Teknolojik Yenileme";
                    //if (f.konuKod == "00.03") f.konuAd = "Tamamlama";

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public Sonuc AktarKonuUstSinir(Kullanici kullanan, int eskiEtap, int yeniEtap)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = "";
            if (eskiEtap == 0)
                hata += "Eski Etap No bilgisi boş bırakılamaz" + "<br>";

            if (yeniEtap == 0)
                hata += "Yeni Etap No bilgisi boş bırakılamaz" + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABYatirimKonu to = new TABYatirimKonu(ve);
            ve.BeginTransaction();

            sonuc = to.SilKonuUstSinir(yeniEtap, 0);
            if (sonuc.islemSonuc)
                sonuc = to.AktarKonuUstSinir(eskiEtap, yeniEtap);

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

        public Sonuc KaydetAltKonuDetay(Kullanici kullanan, YatirimAltKonuDetay bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = "";

            if (bilgi.numara.Trim() == "")
                hata += "Numara bilgisi boş bırakılamaz" + "<br>";

            if (bilgi.ad.Trim() == "")
                hata += "Adı bilgisi boş bırakılamaz" + "<br>";

            if (bilgi.altKonuKod == "")
                hata += "Alt Konu bilgisi boş bırakılamaz" + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABYatirimKonu to = new TABYatirimKonu(ve);
            ve.BeginTransaction();

            if (bilgi.kod == "")
            {
                bilgi.kod = Guid.NewGuid().ToString().Replace("-", "").ToLower();
                sonuc = to.KaydetAltKonuDetay(bilgi);
            }
            else
                sonuc = to.GuncelleAltKonuDetay(bilgi);

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

        public ObjectArray ListeleAltKonuDetay(Kullanici kullanan, YatirimAltKonuDetay kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABYatirimKonu to = new TABYatirimKonu(ve);
            IDataReader r = to.ListeleAltKonuDetay(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    YatirimAltKonuDetay f = new YatirimAltKonuDetay();

                    f.kod = VeriErisim.DegerAl(r, 0, "");
                    f.altKonuKod = VeriErisim.DegerAl(r, 1, string.Empty);
                    f.numara = VeriErisim.DegerAl(r, 2, string.Empty);
                    f.ad = VeriErisim.DegerAl(r, 3, string.Empty);
                    f.aktif = VeriErisim.DegerAl(r, 4, 0);

                    f.altKonuAd = VeriErisim.DegerAl(r, 5, "");

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public Sonuc SilAltKonuDetay(Kullanici kullanan, YatirimAltKonuDetay bilgi)
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
            TABYatirimKonu to = new TABYatirimKonu(ve);

            ve.BeginTransaction();

            sonuc = to.SilAltKonuDetay(bilgi.kod);

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
    }
}
