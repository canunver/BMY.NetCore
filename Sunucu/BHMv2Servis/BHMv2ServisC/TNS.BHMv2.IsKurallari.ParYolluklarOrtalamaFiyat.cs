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
    /// Parametrik Yolluklar Ortalama Fiyat Sınıfı
    /// </summary>
    class BHMv2ParYolluklarOrtalamaFiyat
    {
        /// <summary>
        /// ParYollukOrtalamaFiyat  yapısında tutulan bilgileri ilgili veri tabanı rutinini
        /// çağırarak kaydeden fonksiyon
        /// </summary>
        /// <param name="kullanici">Kullanıcı bilgisi, Kullanıcı tipinde nesne</param>
        /// <param name="parYollukOrtalamaFiyat">Yolluk Ortalama Fiyat bilgisi, ParYollukOrtalamaFiyat tipinde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda Sonuc tipinde nesne döndürür</returns>
        public static Sonuc Kaydet(Kullanici kullanici, ParYollukOrtalamaFiyat parYollukOrtalamaFiyat)
        {
            Sonuc sonuc = new Sonuc(false, "Kayıt sırasında denetim dışı bir hata oluştu.", string.Empty);

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(parYollukOrtalamaFiyat.yil), null);
            ve.BeginTransaction();
            TABPARYOLLUKLARORTALAMAFIYAT to = new TABPARYOLLUKLARORTALAMAFIYAT(ve);

            sonuc = to.Kaydet(parYollukOrtalamaFiyat);

            if (sonuc.islemSonuc) { ve.Commit(); sonuc.bilgiStr += "Kayıt işlemi başarıyla gerçekleşti."; }
            else { ve.Rollback(); }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// ParYollukOrtalamaFiyat yapısında tutulan anahtar alanları kullanarak ilgili veri tabanı rutinini
        /// çağırarak verileri listeleyen fonksiyon
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="parYollukOrtalamaFiyat">Anahtar alanları tutan ParYollukOrtalamaFiyat tipinde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucundan ObjectArray tipinde nesne döndürür</returns>
        public static ObjectArray Listele(Kullanici kullanan, ParYollukOrtalamaFiyat parYollukOrtalamaFiyat)
        {
            ObjectArray hVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(parYollukOrtalamaFiyat.yil), kullanan.KimlikBul());
            TABPARYOLLUKLARORTALAMAFIYAT to = new TABPARYOLLUKLARORTALAMAFIYAT(ve);
            IDataReader r = to.Listele(parYollukOrtalamaFiyat);

            if (to.Hata() == string.Empty)
            {
                while (r.Read())
                {
                    parYollukOrtalamaFiyat = new ParYollukOrtalamaFiyat();
                    parYollukOrtalamaFiyat.yil = VeriErisim.DegerAl(r, (int)TABPARYOLLUKLARORTALAMAFIYAT.ENUMAlanlar.YIL, 0);
                    parYollukOrtalamaFiyat.tip = (ENUMParYollukTip)VeriErisim.DegerAl(r, (int)TABPARYOLLUKLARORTALAMAFIYAT.ENUMAlanlar.TIP, 0);
                    parYollukOrtalamaFiyat.altTip = VeriErisim.DegerAl(r, (int)TABPARYOLLUKLARORTALAMAFIYAT.ENUMAlanlar.ALTTIP, 0);
                    parYollukOrtalamaFiyat.altTipAd = VeriErisim.DegerAl(r, (int)TABPARYOLLUKLARORTALAMAFIYAT.ENUMAlanlar.ALTTIPAD, string.Empty);
                    parYollukOrtalamaFiyat.ortFiyatGorev = VeriErisim.DegerAl(r, (int)TABPARYOLLUKLARORTALAMAFIYAT.ENUMAlanlar.ORTFIYATGOREV, 0.0);
                    parYollukOrtalamaFiyat.ortFiyatAtama = VeriErisim.DegerAl(r, (int)TABPARYOLLUKLARORTALAMAFIYAT.ENUMAlanlar.ORTFIYATATAMA, 0.0);
                    parYollukOrtalamaFiyat.ortFiyatDosya = VeriErisim.DegerAl(r, (int)TABPARYOLLUKLARORTALAMAFIYAT.ENUMAlanlar.ORTFIYATDOSYA, 0.0);
                    parYollukOrtalamaFiyat.ortFiyatToplamGun = VeriErisim.DegerAl(r, (int)TABPARYOLLUKLARORTALAMAFIYAT.ENUMAlanlar.ORTFIYATTOPLAMGUN, 0.0);
                    hVAN.objeler.Add(parYollukOrtalamaFiyat);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return hVAN;
        }

        /// <summary>
        /// ParYollukOrtalamaFiyat yapısınında tutulan anahtar alanları kullanarak ilgili veri tabanı rutinini 
        /// çağırarak silen fonksiyon
        /// </summary>
        /// <param name="kullanici">Kullanıcı bilgisi, Kullanıcı tipinde nesne</param>
        /// <param name="parYollukOrtalamaFiyat">Anahtar alanlarının tutulduğu ParYollukOrtalamaFiyat tipinde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda Sonuc tipinde nesne döndürür</returns>
        public static Sonuc Sil(Kullanici kullanici, ParYollukOrtalamaFiyat parYollukOrtalamaFiyat)
        {
            Sonuc sonuc = new Sonuc(false, "Silme sırasında denetim dışı bir hata oluştu.", string.Empty);

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(parYollukOrtalamaFiyat.yil), kullanici.KimlikBul());
            ve.BeginTransaction();
            TABPARYOLLUKLARORTALAMAFIYAT to = new TABPARYOLLUKLARORTALAMAFIYAT(ve);

            sonuc = to.Sil(parYollukOrtalamaFiyat);
            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silmek istenilen satır başarıyla silindi.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;

        }

    }
}
