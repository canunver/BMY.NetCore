using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Ek ders ücret hesaplamasında kullanılacak puantaj tanım bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    class HRCEkDersPuantaj
    {
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ek ders ücret hesaplamasında kullanılacak puantaj tanım bilgisini veritabanından okur 
        /// ve EkDersPuantajlar nesnesi olarak döner
        /// </summary>
        /// <param name="islemYapan">The islemi yapan.</param>
        /// <param name="f">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ek ders ücret hesaplamasında kullanılacak puantaj tanım bilgisini EkDersPuantajlar nesnesi olarak tutan nesne</returns>
        public EkDersPuantajlar Listele(Kullanici islemYapan, EkDersPuantaj f)
        {
            EkDersPuantajlar fler = new EkDersPuantajlar();
            string hata = "";

            if (f.yil == 0)
            {
                if (hata != "") hata += "<br>";
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00003;
            }
            if (hata != "")
            {
                fler.sonuc.hataStr = hata;
                fler.sonuc.islemSonuc = false;
                return fler;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(islemYapan));

            TABEkDersPuantaj tab = new TABEkDersPuantaj(ve);

            IDataReader r = null;

            try
            {
                r = tab.Listele(f);
            }
            catch (Exception e)
            {
                fler.sonuc.islemSonuc = false;
                fler.sonuc.hataStr = e.Message;

                ve.VeriErisimKapat();

                return fler;
            }

            while (r.Read())
            {
                EkDersPuantaj fm = new EkDersPuantaj();
                fm.yil = VeriErisim.DegerAl(r, 0, 0);
                fm.ay = VeriErisim.DegerAl(r, 1, 0);
                fm.kod = VeriErisim.DegerAl(r, 2, 0);
                fm.aciklama = VeriErisim.DegerAl(r, 3, "");
                fm.yillikSaat = VeriErisim.DegerAl(r, 4, 0.0);
                fm.haftalikSaat = VeriErisim.DegerAl(r, 5, 0.0);
                fm.haftaiciUcret = VeriErisim.DegerAl(r, 6, 0.0);
                fm.haftasonuUcret = VeriErisim.DegerAl(r, 7, 0.0);
                fm.varsayilan = VeriErisim.DegerAl(r, 8, 0);
                fm.durum = VeriErisim.DegerAl(r, 9, 0);

                fler.liste.Add(fm);
            }
            fler.sonuc.islemSonuc = true;

            if (fler.liste.Count == 0)
                fler.sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00072;

            ve.VeriErisimKapat();

            return fler;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ek ders ücret hesaplamasında kullanılacak puantaj tanımının açıklama bilgisini veritabanından okur 
        /// ve EkDersPuantajlar nesnesi olarak döner
        /// </summary>
        /// <param name="islemYapan">The islemi yapan.</param>
        /// <param name="f">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ek ders ücret hesaplamasında kullanılacak puantaj tanımının açıklama bilgisini EkDersPuantajlar nesnesi olarak tutan nesne</returns>
        public EkDersPuantajlar ListeleAciklama(Kullanici islemYapan, EkDersPuantaj f)
        {
            EkDersPuantajlar fler = new EkDersPuantajlar();
            string hata = "";

            if (f.yil == 0)
            {
                if (hata != "") hata += "<br>";
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00003;
            }
            if (hata != "")
            {
                fler.sonuc.hataStr = hata;
                fler.sonuc.islemSonuc = false;
                return fler;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(islemYapan));

            TABEkDersPuantaj tab = new TABEkDersPuantaj(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListeleAciklama(f);
            }
            catch (Exception e)
            {
                fler.sonuc.islemSonuc = false;
                fler.sonuc.hataStr = e.Message;

                ve.VeriErisimKapat();

                return fler;
            }

            while (r.Read())
            {
                EkDersPuantaj fm = new EkDersPuantaj();
                fm.kod = VeriErisim.DegerAl(r, 0, 0);
                fm.yil = VeriErisim.DegerAl(r, 1, 1);
                fm.aciklama = VeriErisim.DegerAl(r, 2, "");
                fler.liste.Add(fm);
            }
            fler.sonuc.islemSonuc = true;

            if (fler.liste.Count == 0)
                fler.sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00072;

            ve.VeriErisimKapat();

            return fler;
        }

        /// <summary>
        /// Arguman olarak verilen, ek ders ücret hesaplamasında kullanılacak puantaj tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="f">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, EkDersPuantaj f)
        {
            Sonuc sonuc = new Sonuc();

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (f.yil == 0)
            {
                if (hata != "") hata += "<br>";
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00003;
            }
            if (f.aciklama.Trim() == "")
            {
                if (hata != "") hata += "<br>";
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00082;
            }
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABEkDersPuantaj tab = new TABEkDersPuantaj(ve);

            sonuc = tab.Kaydet(f);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, ek ders ücret hesaplamasında kullanılacak puantaj tanım bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanici">The islemi yapan.</param>
        /// <param name="f">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Sil(Kullanici kullanici, EkDersPuantaj f)
        {
            Sonuc sonuc = new Sonuc();
            string hata = "";

            if (f.yil == 0)
            {
                if (hata != "") hata += "<br>";
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00003;
            }
            if (f.kod == 0)
            {
                if (hata != "") hata += "<br>";
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00083;
            }
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanici));

            ve.BeginTransaction();

            TABEkDersPuantaj tab = new TABEkDersPuantaj(ve);

            sonuc = tab.Sil(f);

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
