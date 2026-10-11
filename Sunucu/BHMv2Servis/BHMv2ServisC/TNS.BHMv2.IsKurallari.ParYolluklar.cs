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
    /// Parametrik Yolluklar Sınıfı
    /// </summary>
    class BHMv2ParYolluklar
    {

        /// <summary>
        /// ParYolluk dizi yapısında tutulan bilgileri ilgili veri tabanı rutinini
        /// çağırarak kaydeden fonksiyon
        /// </summary>
        /// <param name="kullanici">Kullanıcı bilgisi, Kullanıcı tipinde nesne</param>
        /// <param name="parYollukDizi">Yolluk bilgisi, ParYolluk tipinde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda Sonuc tipinde nesne döndürür</returns>
        public static Sonuc Kaydet(Kullanici kullanici, ParYolluk[] parYollukDizi)
        {
            Sonuc sonuc = new Sonuc(false, "Kayıt için en az bir kayıt gönderilmelidir!", string.Empty);
            if (parYollukDizi.Length < 1)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(parYollukDizi[0].yil), kullanici.KimlikBul());
            ve.BeginTransaction();
            TABPARYOLLUKLAR to = new TABPARYOLLUKLAR(ve);

            if (parYollukDizi.Length > 0)
                sonuc = to.Kaydet(parYollukDizi);

            if (sonuc.islemSonuc) { ve.Commit(); sonuc.bilgiStr += "Kayıt işlemi başarıyla gerçekleşti."; }
            else { ve.Rollback(); }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// ParYolluk yapısında tutulan anahtar alanları kullanarak ilgili veri tabanı rutinini
        /// çağırarak verileri listeleyen fonksiyon
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="parYolluk">Anahtar alanları tutan ParYolluk tipinde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucundan ObjectArray tipinde nesne döndürür</returns>
        public static ObjectArray Listele(Kullanici kullanan, ParYolluk parYolluk)
        {
            ObjectArray hVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(parYolluk.yil), kullanan.KimlikBul());
            TABPARYOLLUKLAR to = new TABPARYOLLUKLAR(ve);
            IDataReader r = to.Listele(parYolluk);

            if (to.Hata() == string.Empty)
            {
                while (r.Read())
                {
                    parYolluk = new ParYolluk();
                    parYolluk.yil = VeriErisim.DegerAl(r, (int)TABPARYOLLUKLAR.ENUMAlanlar.YIL, 0);
                    parYolluk.tip = (ENUMParYollukTip)VeriErisim.DegerAl(r, (int)TABPARYOLLUKLAR.ENUMAlanlar.TIP, 0);
                    parYolluk.asama = (ENUMBHBirimAsama)VeriErisim.DegerAl(r, (int)TABPARYOLLUKLAR.ENUMAlanlar.ASAMA, 0);
                    parYolluk.kur = VeriErisim.DegerAl(r, (int)TABPARYOLLUKLAR.ENUMAlanlar.KURKOD, string.Empty);
                    parYolluk.fon = VeriErisim.DegerAl(r, (int)TABPARYOLLUKLAR.ENUMAlanlar.FONKOD, string.Empty);
                    parYolluk.fin = VeriErisim.DegerAl(r, (int)TABPARYOLLUKLAR.ENUMAlanlar.FINKOD, string.Empty);
                    parYolluk.eko = VeriErisim.DegerAl(r, (int)TABPARYOLLUKLAR.ENUMAlanlar.EKOKOD, string.Empty);
                    parYolluk.peb = VeriErisim.DegerAl(r, (int)TABPARYOLLUKLAR.ENUMAlanlar.PEBKOD, string.Empty);
                    parYolluk.kurumsalYapiKod = VeriErisim.DegerAl(r, (int)TABPARYOLLUKLAR.ENUMAlanlar.KURUMSALYAPIKOD, string.Empty);

                    parYolluk.altTip = VeriErisim.DegerAl(r, (int)TABPARYOLLUKLAR.ENUMAlanlar.ALTTIP, 0);
                    parYolluk.gorevSayisi = VeriErisim.DegerAl(r, (int)TABPARYOLLUKLAR.ENUMAlanlar.GOREVSAYISI, 0);
                    parYolluk.atamaSayisi = VeriErisim.DegerAl(r, (int)TABPARYOLLUKLAR.ENUMAlanlar.ATAMASAYISI, 0);
                    parYolluk.dosyaSayisi = VeriErisim.DegerAl(r, (int)TABPARYOLLUKLAR.ENUMAlanlar.DOSYASAYISI, 0);
                    parYolluk.toplamGunSayisi = VeriErisim.DegerAl(r, (int)TABPARYOLLUKLAR.ENUMAlanlar.TOPLAMGUNSAYISI, 0);
                    parYolluk.tutar = VeriErisim.DegerAl(r, (int)TABPARYOLLUKLAR.ENUMAlanlar.TUTAR, 0.0);
                    hVAN.objeler.Add(parYolluk);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return hVAN;
        }

    }
}
