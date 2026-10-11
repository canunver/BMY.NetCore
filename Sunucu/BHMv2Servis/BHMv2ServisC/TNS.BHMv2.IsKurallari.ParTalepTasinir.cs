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
    /// Parametrik Talep Taşınır Sınıfı
    /// </summary>
    class BHMv2ParTalepTasinir
    {
        /// <summary>
        /// ParTalepTasinir yapısında tutulan anahtar alanları kullanarak ilgili veri tabanı rutinini
        /// çağırarak verileri listeleyen fonksiyon
        /// </summary>
        /// <param name="parTalepTasinir">Anahtar alanları tutan ParTalepTasinir tipinde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucundan ObjectArray tipinde nesne döndürür</returns>
        public static ObjectArray Listele(ParTalepTasinir parTalepTasinir)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(parTalepTasinir.yil), null);

            TABPARTALEPTASINIR tab = new TABPARTALEPTASINIR(ve);

            IDataReader r = null;

            try
            {
                r = tab.Listele(parTalepTasinir);
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
                ParTalepTasinir pp = new ParTalepTasinir();
                pp.yil = VeriErisim.DegerAl(r, 0, 0);
                pp.asama = (ENUMBHBirimAsama)VeriErisim.DegerAl(r, 1, 0);
                pp.kur = VeriErisim.DegerAl(r, 2, string.Empty);
                pp.fon = VeriErisim.DegerAl(r, 3, string.Empty);
                pp.fin = VeriErisim.DegerAl(r, 4, string.Empty);
                pp.eko = VeriErisim.DegerAl(r, 5, string.Empty);
                pp.peb = VeriErisim.DegerAl(r, 6, string.Empty);
                pp.kurumsalYapiKod = VeriErisim.DegerAl(r, 7, string.Empty);
                pp.tasHesapKod = OrtakFonksiyonlar.KodNoktaKoy(BHMv2KurumsalYapi.KurumsalYapiDondur(), VeriErisim.DegerAl(r, 8, string.Empty));
                pp.miktar = VeriErisim.DegerAl(r, 9, 0);

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
        /// ParTalepTasinir yapısında tutulan bilgileri ilgili veri tabanı rutinini
        /// çağırarak kaydeden fonksiyon
        /// </summary>
        /// <param name="kullanici">Kullanıcı bilgisi, Kullanıcı tipinde nesne</param>
        /// <param name="parTalepTasinir">Talep Taşınır bilgisi, ParTalepTasinir tipinde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda Sonuc tipinde nesne döndürür</returns>
        public static Sonuc Kaydet(Kullanici kullanici, ParTalepTasinir parTalepTasinir)
        {
            Sonuc sonuc = new Sonuc();
            string hata = string.Empty;


            if (parTalepTasinir.yil == 0)
                hata += "Yıl alanı boş bırakılamaz";


            if (parTalepTasinir.kur.Trim() == string.Empty)
            {
                if (hata != string.Empty) hata += "<br>";
                hata += "Kur Kod adı boş bırakılamaz";
            }


            if (parTalepTasinir.fon.Trim() == string.Empty)
            {
                if (hata != string.Empty) hata += "<br>";
                hata += "Fon Kod adı boş bırakılamaz";
            }


            if (parTalepTasinir.fin.Trim() == string.Empty)
            {
                if (hata != string.Empty) hata += "<br>";
                hata += "Fin Kod adı boş bırakılamaz";
            }


            if (parTalepTasinir.eko.Trim() == string.Empty)
            {
                if (hata != string.Empty) hata += "<br>";
                hata += "Eko Kod adı boş bırakılamaz";
            }


            if (parTalepTasinir.peb.Trim() == string.Empty)
            {
                if (hata != string.Empty) hata += "<br>";
                hata += "Peb Kod adı boş bırakılamaz";
            }


            if (parTalepTasinir.kurumsalYapiKod.Trim() == string.Empty)
            {
                if (hata != string.Empty) hata += "<br>";
                hata += "Kurumsal Yapı Kod adı boş bırakılamaz";
            }


            if (parTalepTasinir.tasHesapKod.Trim() == string.Empty)
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

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(parTalepTasinir.yil), kullanici.KimlikBul());

            ve.BeginTransaction();

            TABPARTALEPTASINIR tab = new TABPARTALEPTASINIR(ve);

            sonuc = tab.Kaydet(parTalepTasinir);

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
        /// ParTalepTasinir yapısınında tutulan anahtar alanları kullanarak ilgili veri tabanı rutinini 
        /// çağırarak silen fonksiyon
        /// </summary>
        /// <param name="kullanici">Kullanıcı bilgisi, Kullanıcı tipinde nesne</param>
        /// <param name="parTalepTasinir">Anahtar alanlarının tutulduğu ParTalepTasinir tipinde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda Sonuc tipinde nesne döndürür</returns>
        public static Sonuc Sil(Kullanici kullanici, ParTalepTasinir parTalepTasinir)
        {
            Sonuc sonuc = new Sonuc();

            if (parTalepTasinir.yil == 0)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin Yılı</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }


            if (parTalepTasinir.asama == 0)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin Aşaması</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }



            if (parTalepTasinir.kur.Trim() == string.Empty)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin kur kodu</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }


            if (parTalepTasinir.fon.Trim() == string.Empty)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin fon kodu</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (parTalepTasinir.fin.Trim() == string.Empty)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin fin kodu</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }


            if (parTalepTasinir.eko.Trim() == string.Empty)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin eko kodu</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }


            if (parTalepTasinir.peb.Trim() == string.Empty)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin peb kodu</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (parTalepTasinir.kurumsalYapiKod.Trim() == string.Empty)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin kurumsal yapı kodu</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }


            if (parTalepTasinir.tasHesapKod.Trim() == string.Empty)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin hesap kodu</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(parTalepTasinir.yil), kullanici.KimlikBul());

            ve.BeginTransaction();

            TABPARTALEPTASINIR tab = new TABPARTALEPTASINIR(ve);

            sonuc = tab.Sil(parTalepTasinir);

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
