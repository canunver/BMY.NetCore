using System;
using System.Data;
using TNS.BHF;
using TNS.KYM;
using TNS.Veri;
using TNS.BHF.IsKurallari.Veri;
using System.Collections;

namespace TNS.BHF.IsKurallari
{
    /// <summary>
    /// Summary description for BHFIsciServis.
    /// </summary>
    public class UluslarArasiKuruluslarServis
    {
        /// <summary>
        /// Veri tabanından okunan bir kaydın bilgilerini alıp UluslarArasiKurulus nesnesine çeviren yordam.
        /// </summary>
        /// <param name="r">Akfit satırı UluslarArasiKurulus nesnesine çevrilecek olan DataReader nesnesi.</param>
        /// <returns>Kayıt bilgilerini kullanılarak oluşturulan UluslarArasiKurulus nesnesi döndürülür.</returns>
        private static UluslarArasiKurulus DataReaderdanAktar(IDataReader r)
        {
            UluslarArasiKurulus uak = new UluslarArasiKurulus();

            uak.siraNo = VeriErisim.DegerAl(r, 2, 0);
            uak.kurulusAd = VeriErisim.DegerAl(r, 3, "");
            uak.yasalDayanak = VeriErisim.DegerAl(r, 4, "");
            uak.toplantiSayisi = VeriErisim.DegerAl(r, 5, 0);
            uak.toplantiKatsayisi = VeriErisim.DegerAl(r, 6, 0);
            uak.dovizCinsi = VeriErisim.DegerAl(r, 7, "");
            uak.oncekiyilAidat = VeriErisim.DegerAl(r, 8, 0.0);
            uak.hazirlikYilAidat = VeriErisim.DegerAl(r, 9, 0.0);
            uak.ertesiYilAidat = VeriErisim.DegerAl(r, 10, 0.0);
            uak.ertesiYil2Aidat = VeriErisim.DegerAl(r, 11, 0.0);
            uak.aciklama = VeriErisim.DegerAl(r, 12, "");
            uak.oncekiyilDoviz = VeriErisim.DegerAl(r, 13, 0.0);
            uak.hazirlikYilDoviz = VeriErisim.DegerAl(r, 14, 0.0);
            uak.ertesiYilDoviz = VeriErisim.DegerAl(r, 15, 0.0);
            uak.ertesiYil2Doviz = VeriErisim.DegerAl(r, 16, 0.0);

            return uak;
        }

        /// <summary>
        /// Uluslararası kuruluş üyelik bilgilerini listeleyen yordam.
        /// Kullanıcının yetkisi varsa, veri tabanından alınan kayıtlar UluslarArasiKurulus nesnesine çevrilir, listeye eklenir. Oluşturulan liste diziye çevrilir ve döndürülür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Uluslararası kuruluş üyelik bilgilerinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Uluslararası kuruluş üyelik bilgileri listelenen birimin kurumsal kodu.</param>
        /// <returns>Verilen yılda birimin üye olacağı uluslar arası kuruluşların listesi döndürülür.</returns>
        public static UluslarArasiKurulus[] UluslarArasiKuruluslarListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod)
        {
            ArrayList UluslarArasiKuruluslar = new ArrayList();
            kurumsalKod = kurumsalKod.Replace(".", "");

            if (Servis.GormeYetkisiVarMi(kullanan, kurumsalKod))
            {

                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
                TABUluslarArasiKuruluslar tuak = new TABUluslarArasiKuruluslar(ve);
                IDataReader r = tuak.UluslarArasiKuruluslarListele(yil, kurumsalKod);

                if (tuak.Hata() == "")
                {
                    while (r.Read())
                    {
                        UluslarArasiKuruluslar.Add(DataReaderdanAktar(r));
                    }
                    r.Close();
                }
                else
                {
                    ve.VeriErisimKapat();
                    throw new Exception(tuak.Hata());
                }

                ve.VeriErisimKapat();
            }
            return (UluslarArasiKurulus[])UluslarArasiKuruluslar.ToArray(typeof(UluslarArasiKurulus));
        }

        /// <summary>
        /// Uluslararası kuruluş üyelik bilgilerini kaydeden yordam.
        /// Yıl kurumsal kod geçerli ise ve kullanıcının yazma yetkisi varsa, önce birimin eski bilgileri silimir. Sonra döngü içerisinde sıra no geçerli ise, kayıtlar veri tabanına eklenir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Uluslararası kuruluş üyelik bilgilerinin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Uluslararası kuruluş üyelik bilgileri kaydedilen birimin kurumsal kodu.</param>
        /// <param name="uluslarArasiKuruluslar">Kaydedilecek üyelik bilgileri nesnelerini içeren dizi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc UluslarArasiKuruluslarKaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod, TNSCollection uluslarArasiKuruluslar)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
            Sonuc sonuc = new Sonuc(true, "", "");
            TNS.BHF.IsKurallari.Servis.KurumsalKodBirimKontrol(kurumsalKod, sonuc);
            if (!Servis.YazmaYetkisiVarMi(kullanan, yil, kurumsalKod))
                sonuc.DegerAta(ButceFormResI.BHFETS002);
            if (yil <= 0)
                sonuc.DegerAta(ButceFormResI.BHMUAK002);
            if (string.IsNullOrEmpty( kurumsalKod))
                sonuc.DegerAta(ButceFormResI.BHMUAK003);
            if (!Servis.YazmaYetkisiVarMi(kullanan, yil, kurumsalKod))
                sonuc.DegerAta(ButceFormResI.BHMUAK001);

            if(sonuc.islemSonuc)
            {
                kurumsalKod = kurumsalKod.Replace(".", "");
                TABUluslarArasiKuruluslar tuak = new TABUluslarArasiKuruluslar(ve);
                ve.BeginTransaction();
                sonuc = tuak.UluslarArasiKuruluslarSil(yil, kurumsalKod);
                if (sonuc.islemSonuc)
                {
                    int i = 1;
                    foreach (UluslarArasiKurulus uak in uluslarArasiKuruluslar)
                    {
                        if (uak.siraNo <= 0)
                            sonuc.DegerAta(ButceFormResI.BHMUAK004);
                        if (sonuc.islemSonuc)
                        {
                            sonuc = tuak.UluslarArasiKuruluslarKaydet(yil, kurumsalKod, i, uak);
                            i = i + 1;
                        }
                        if (!sonuc.islemSonuc)
                            break;
                    }
                }
                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }
            ve.VeriErisimKapat();
            return sonuc;
        }
    }
}
