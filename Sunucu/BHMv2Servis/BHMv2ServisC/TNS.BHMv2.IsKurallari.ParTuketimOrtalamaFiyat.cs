using System;
using System.Collections.Generic;
using System.Text;
using TNS.KYM;
using TNS.Veri;
using TNS.BHMv2.IsKurallari.Veri;
using System.Data;

namespace TNS.BHMv2.IsKurallari
{

    /// <summary>
    /// Parametrik Tüketim Ortalama Fiyat Sınıfı
    /// </summary>
    class BHMv2ParTuketimOrtalamaFiyat
    {
        /// <summary>
        /// ParTuketimOrtalamaFiyat yapısında tutulan anahtar alanları kullanarak ilgili veri tabanı rutinini
        /// çağırarak verileri listeleyen fonksiyon
        /// </summary>
        /// <param name="parTuketimOrtalamaFiyat">Anahtar alanları tutan ParTuketimOrtalamaFiyat tipinde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucundan ObjectArray tipinde nesne döndürür</returns>
        public static ObjectArray Listele(ParTuketimOrtalamaFiyat parTuketimOrtalamaFiyat)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(parTuketimOrtalamaFiyat.yil), null);

            TABPARTUKETIMORTALAMAFIYAT tab = new TABPARTUKETIMORTALAMAFIYAT(ve);

            IDataReader r = null;

            try
            {
                r = tab.Listele(parTuketimOrtalamaFiyat);
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
                ParTuketimOrtalamaFiyat pp = new ParTuketimOrtalamaFiyat();

                pp.yil = VeriErisim.DegerAl(r, 0, 0);
                pp.tuketimTip = new TuketimTip(VeriErisim.DegerAl(r, 1, string.Empty));
                pp.ortalamaFiyat = VeriErisim.DegerAl(r, 2, 0.0);

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
        /// ParTuketimOrtalamaFiyat yapısında tutulan bilgileri ilgili veri tabanı rutinini
        /// çağırarak kaydeden fonksiyon
        /// </summary>
        /// <param name="kullanici">Kullanıcı bilgisi, Kullanıcı tipinde nesne</param>
        /// <param name="parTuketimOrtalamaFiyat">Tüketim Ortalama Fiyat bilgisi, ParTuketimOrtalamaFiyat tipinde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda Sonuc tipinde nesne döndürür</returns>
        public static Sonuc Kaydet(Kullanici kullanici, ParTuketimOrtalamaFiyat parTuketimOrtalamaFiyat)
        {
            Sonuc sonuc = new Sonuc();
            string hata = string.Empty;

            
            if (parTuketimOrtalamaFiyat.yil == 0)
                hata += "Yıl alanı boş bırakılamaz";

            if (parTuketimOrtalamaFiyat.tuketimTip == null || string.IsNullOrEmpty(parTuketimOrtalamaFiyat.tuketimTip.kod))
            {
                if (hata != string.Empty) hata += "<br>";
                hata += "Tüketim tipi boş bırakılamaz";
            }

            if (hata != string.Empty)
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(parTuketimOrtalamaFiyat.yil), kullanici.KimlikBul());

            ve.BeginTransaction();

            TABPARTUKETIMORTALAMAFIYAT tab = new TABPARTUKETIMORTALAMAFIYAT(ve);

            sonuc = tab.Kaydet(parTuketimOrtalamaFiyat);

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
        /// ParTuketimOrtalamaFiyat yapısınında tutulan anahtar alanları kullanarak ilgili veri tabanı rutinini 
        /// çağırarak silen fonksiyon
        /// </summary>
        /// <param name="kullanici">Kullanıcı bilgisi, Kullanıcı tipinde nesne</param>
        /// <param name="parTuketimOrtalamaFiyat">Anahtar alanlarının tutulduğu ParTuketimOrtalamaFiyat tipinde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda Sonuc tipinde nesne döndürür</returns>
        public static Sonuc Sil(Kullanici kullanici, ParTuketimOrtalamaFiyat parTuketimOrtalamaFiyat)
        {
            Sonuc sonuc = new Sonuc();

            if (parTuketimOrtalamaFiyat.yil == 0)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin Yılı</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }



            if (parTuketimOrtalamaFiyat.tuketimTip.kod.Trim() == string.Empty)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin hesap kodu</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(parTuketimOrtalamaFiyat.yil), kullanici.KimlikBul());

            ve.BeginTransaction();

            TABPARTUKETIMORTALAMAFIYAT tab = new TABPARTUKETIMORTALAMAFIYAT(ve);

            sonuc = tab.Sil(parTuketimOrtalamaFiyat);

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
