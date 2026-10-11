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
    /// Parametrik Taşınır Ortalama Fiyat Sınıfı
    /// </summary>
    class BHMv2ParTasinirOrtalamaFiyat
    {
        /// <summary>
        /// ParTasinirOrtalamaFiyat yapısında tutulan anahtar alanları kullanarak ilgili veri tabanı rutinini
        /// çağırarak verileri listeleyen fonksiyon
        /// </summary>
        /// <param name="parTasinirOrtalamaFiyat">Anahtar alanları tutan ParTasinirOrtalamaFiyat tipinde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucundan ObjectArray tipinde nesne döndürür</returns>
        public static ObjectArray Listele(ParTasinirOrtalamaFiyat parTasinirOrtalamaFiyat)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(parTasinirOrtalamaFiyat.yil), null);

            TABPARTASINIRORTALAMAFIYAT tab = new TABPARTASINIRORTALAMAFIYAT(ve);

            IDataReader r = null;

            try
            {
                r = tab.Listele(parTasinirOrtalamaFiyat);
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
                ParTasinirOrtalamaFiyat pp = new ParTasinirOrtalamaFiyat();

                pp.yil = VeriErisim.DegerAl(r, 0, 0);
                pp.hesapkod = OrtakFonksiyonlar.KodNoktaKoy(BHMv2KurumsalYapi.KurumsalYapiDondur(), VeriErisim.DegerAl(r, 1, string.Empty));
                pp.ortalamafiyat = VeriErisim.DegerAl(r, 2, 0.0);

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
        /// ParTasinirOrtalamaFiyat yapısında tutulan bilgileri ilgili veri tabanı rutinini
        /// çağırarak kaydeden fonksiyon
        /// </summary>
        /// <param name="kullanici">Kullanıcı bilgisi, Kullanıcı tipinde nesne</param>
        /// <param name="parTasinirOrtalamaFiyat">Taşınır Ortalama Fiyat bilgisi, ParTasinirOrtalamaFiyat tipinde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda Sonuc tipinde nesne döndürür</returns>
        public static Sonuc Kaydet(Kullanici kullanici, ParTasinirOrtalamaFiyat parTasinirOrtalamaFiyat)
        {
            Sonuc sonuc = new Sonuc();
            string hata = string.Empty;

            if (parTasinirOrtalamaFiyat.yil == 0)
                hata += "Yıl alanı boş bırakılamaz";

            if (parTasinirOrtalamaFiyat.hesapkod.Trim() == string.Empty)
            {
                if (hata != string.Empty) hata += "<br>";
                hata += "Hesap Kod adı boş bırakılamaz";
            }

            if (hata != string.Empty)
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(parTasinirOrtalamaFiyat.yil), kullanici.KimlikBul());

            ve.BeginTransaction();

            TABPARTASINIRORTALAMAFIYAT tab = new TABPARTASINIRORTALAMAFIYAT(ve);

            sonuc = tab.Kaydet(parTasinirOrtalamaFiyat);

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
        /// ParTasinirOrtalamaFiyat yapısınında tutulan anahtar alanları kullanarak ilgili veri tabanı rutinini 
        /// çağırarak silen fonksiyon
        /// </summary>
        /// <param name="kullanici">Kullanıcı bilgisi, Kullanıcı tipinde nesne</param>
        /// <param name="parTasinirOrtalamaFiyat">Anahtar alanlarının tutulduğu ParTasinirOrtalamaFiyat tipinde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda Sonuc tipinde nesne döndürür</returns>
        public static Sonuc Sil(Kullanici kullanici, ParTasinirOrtalamaFiyat parTasinirOrtalamaFiyat)
        {
            Sonuc sonuc = new Sonuc();

            if (parTasinirOrtalamaFiyat.yil == 0)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin Yılı</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }



            if (parTasinirOrtalamaFiyat.hesapkod.Trim() == string.Empty)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin hesap kodu</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(parTasinirOrtalamaFiyat.yil), kullanici.KimlikBul());

            ve.BeginTransaction();

            TABPARTASINIRORTALAMAFIYAT tab = new TABPARTASINIRORTALAMAFIYAT(ve);

            sonuc = tab.Sil(parTasinirOrtalamaFiyat);

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
