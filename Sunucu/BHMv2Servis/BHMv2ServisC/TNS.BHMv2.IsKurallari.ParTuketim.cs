using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using TNS.BHMv2.IsKurallari.Veri;
using System.Data;
using TNS.KYM;


namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// Parametrik Tüketim Sınıfı
    /// </summary>
    class BHMv2ParTuketim
    {

        /// <summary>
        /// ParTuketim yapısında tutulan anahtar alanları kullanarak ilgili veri tabanı rutinini
        /// çağırarak verileri listeleyen fonksiyon
        /// </summary>
        /// <param name="parTuketim">Anahtar alanları tutan ParTuketim tipinde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucundan ObjectArray tipinde nesne döndürür</returns>
        public static ObjectArray Listele(ParTuketim parTuketim)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(parTuketim.yil), null);

            TABPARTUKETIM tab = new TABPARTUKETIM(ve);

            IDataReader r = null;

            try
            {
                r = tab.Listele(parTuketim);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = e.Message;

                return liste;
            }

            while (r.Read())
            {
                ParTuketim pp = new ParTuketim();
                pp.yil = VeriErisim.DegerAl(r, 0, 0);
                pp.asama = (ENUMBHBirimAsama)VeriErisim.DegerAl(r, 1, 0);
                pp.kur = VeriErisim.DegerAl(r, 2, string.Empty);
                pp.fon = VeriErisim.DegerAl(r, 3, string.Empty);
                pp.fin = VeriErisim.DegerAl(r, 4, string.Empty);
                pp.eko = VeriErisim.DegerAl(r, 5, string.Empty);
                pp.peb = VeriErisim.DegerAl(r, 6, string.Empty);
                pp.kurumsalYapiKod = VeriErisim.DegerAl(r, 7, string.Empty);
                pp.tuketimTip = new TuketimTip(VeriErisim.DegerAl(r, 8, string.Empty));
                pp.miktar = VeriErisim.DegerAl(r, 9, 0);
                pp.ortalamaFiyat = VeriErisim.DegerAl(r, 10, 0.0);

                liste.objeler.Add(pp);
            }
            r.Close();

            liste.sonuc.islemSonuc = true;

            if (liste.objeler.Count == 0)
                liste.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return liste;
        }


        /// <summary>
        /// ParTuketim yapısında tutulan bilgileri ilgili veri tabanı rutinini
        /// çağırarak kaydeden fonksiyon
        /// </summary>
        /// <param name="kullanici">Kullanıcı bilgisi, Kullanıcı tipinde nesne</param>
        /// <param name="parTuketim">Tüketim bilgisi, ParTuketim tipinde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda Sonuc tipinde nesne döndürür</returns>
        public static Sonuc Kaydet(Kullanici kullanici, ParTuketim parTuketim)
        {
            Sonuc sonuc = new Sonuc();
            string hata = string.Empty;

         
            if (parTuketim.yil == 0)
                hata += "Yıl alanı boş bırakılamaz";


            if (parTuketim.kur.Trim() == string.Empty)
            {
                if (hata != string.Empty) hata += "<br>";
                hata += "Kur Kod adı boş bırakılamaz";
            }


            if (parTuketim.fon.Trim() == string.Empty)
            {
                if (hata != string.Empty) hata += "<br>";
                hata += "Fon Kod adı boş bırakılamaz";
            }


            if (parTuketim.fin.Trim() == string.Empty)
            {
                if (hata != string.Empty) hata += "<br>";
                hata += "Fin Kod adı boş bırakılamaz";
            }


            if (parTuketim.eko.Trim() == string.Empty)
            {
                if (hata != string.Empty) hata += "<br>";
                hata += "Eko Kod adı boş bırakılamaz";
            }


            if (parTuketim.peb.Trim() == string.Empty)
            {
                if (hata != string.Empty) hata += "<br>";
                hata += "Peb Kod adı boş bırakılamaz";
            }


            if (parTuketim.kurumsalYapiKod.Trim() == string.Empty)
            {
                if (hata != string.Empty) hata += "<br>";
                hata += "Kurumsal Yapı Kod adı boş bırakılamaz";
            }


            if (parTuketim.tuketimTip.kod.Trim() == string.Empty)
            {
                if (hata != string.Empty) hata += "<br>";
                hata += "Tüketim Tipi boş bırakılamaz";
            }

            if (hata != string.Empty)
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(parTuketim.yil), kullanici.KimlikBul());

            ve.BeginTransaction();

            TABPARTUKETIM tab = new TABPARTUKETIM(ve);

            sonuc = tab.Kaydet(parTuketim);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Kaydetme işlemi başarıyla gerçekleştirildi.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// ParTuketim yapısınında tutulan anahtar alanları kullanarak ilgili veri tabanı rutinini 
        /// çağırarak silen fonksiyon
        /// </summary>
        /// <param name="kullanici">Kullanıcı bilgisi, Kullanıcı tipinde nesne</param>
        /// <param name="parTuketim">Anahtar alanlarının tutulduğu ParTuketim tipinde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda Sonuc tipinde nesne döndürür</returns>
        public static Sonuc Sil(Kullanici kullanici, ParTuketim parTuketim)
        {
            Sonuc sonuc = new Sonuc();

            if (parTuketim.yil == 0)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin Yılı</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }


            if (parTuketim.asama == 0)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin Aşaması</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }



            if (parTuketim.kur.Trim() == string.Empty)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin kur kodu</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }


            if (parTuketim.fon.Trim() == string.Empty)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin fon kodu</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (parTuketim.fin.Trim() == string.Empty)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin fin kodu</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }


            if (parTuketim.eko.Trim() == string.Empty)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin eko kodu</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }


            if (parTuketim.peb.Trim() == string.Empty)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin peb kodu</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (parTuketim.kurumsalYapiKod.Trim() == string.Empty)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin kurumsal yapı kodu</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }


            if (parTuketim.tuketimTip.kod.Trim() == string.Empty)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin hesap kodu</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(parTuketim.yil), kullanici.KimlikBul());

            ve.BeginTransaction();

            TABPARTUKETIM tab = new TABPARTUKETIM(ve);

            sonuc = tab.Sil(parTuketim);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}
