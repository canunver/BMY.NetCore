using System;
using TNS;
using TNS.KYM;
using TNS.BHF;
using TNS.Veri;
using System.Data;
using TNS.BHF.IsKurallari.Veri;
using System.Collections;

namespace TNS.BHF.IsKurallari
{
    /// <summary>
    /// Yeni başlıyacak hizmetlerin ekonomik kod bazında maliyetleri ve bitirilecek hizmetlerin ekonomik kod bazında maliyetleriyle ilgili işlemleri tutan sınıf.
    /// Yetki kontrolü yapıldıktan sonra, veri tabanından çekilen sözlük bilgileri listeye doldurulur. Her bir sözlük nesnesi için yıl bilgisi okunur ve böylece bir fiziksel değerin tüm yılları okunmuş olur. Okunan yıl bilgileri diziye çevrilir.
    /// </summary>
    public class HizmetBilgisiServisi
    {
        /// <summary>
        /// Yeni başlıyacak hizmetlerin ekonomik kod bazında maliyetleri ve bitirilecek hizmetlerin ekonomik kod bazında maliyetlerinin listelendiği yordam.
        /// Yetki kontrolü yapıldıktan sonra, verilen birim için kaydedilen hizmet bilgisi (açıklama ve tarihler) okunur. Okunan her bir hizmet satırının, ekonomik kod bazında bilgileri okunur ve hizmet satırı dizisine çevrilir. Maliyetleriyle birlikte hazırlanan hizmet satırları döndürülecek diziye eklenir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birime ait tamamlanacak yada başlayacak hizmetlerin maliyetlerinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birime ait tamamlanacak yada başlayacak hizmetlerin maliyetlerinin listeleneceği birimin kurumsal kodu.</param>
        /// <param name="tamamMiIlkMi">1 ise ilk defa yapılması planlanan hizmetler, 1 değil ise tamamlanması planlanan hizmetler</param>
        /// <param name="hizmetYili">3 yıllık süreçdeki hizmetlerle ilgili olduğundan hizmet yılı, yıl, yıl+1, yıl+2 değerleri almalıdır.</param>
        /// <returns>Verilen kriterlere uyan hizmetlerin maliyetlerinin tutulduğu satırlar döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod, int tamamMiIlkMi, int hizmetYili)
        {
            ObjectArray donusDeger = new ObjectArray();
            if (Servis.GormeYetkisiVarMi(kullanan, kurumsalKod))
            {
                kurumsalKod = kurumsalKod.Replace(".", "");
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
                TABHizmetBilgisi thb = new TABHizmetBilgisi(ve);
                IDataReader r = thb.SatirlariAl(yil, kurumsalKod, tamamMiIlkMi, hizmetYili);
                if (thb.Hata() == "")
                {
                    while (r.Read())
                    {
                        HizmetBilgisiSatiri hbs = new HizmetBilgisiSatiri();
                        hbs.birimKod = OrtakFonksiyonlar.KodNoktaKoy("99.99.99.99", r.GetString(0));
                        hbs.birimAd = Servis.UzayDegeriStr(kullanan, "ABSKUR", yil.ToString() + "-" + hbs.birimKod, true);
                        hbs.siraNo = r.GetInt32(1);
                        if (!r.IsDBNull(2))
                            hbs.baslangicTarihi = new TNSDateTime(r.GetDateTime(2));
                        if (!r.IsDBNull(3))
                            hbs.bitisTarihi = new TNSDateTime(r.GetDateTime(3));
                        if (r.IsDBNull(4))
                            hbs.hizmetAdi = "";
                        else
                            hbs.hizmetAdi = r.GetString(4);
                        if (r.IsDBNull(5))
                            hbs.aciklama = "";
                        else
                            hbs.aciklama = r.GetString(5);
                        donusDeger.Ekle(hbs);
                    }
                    r.Close();

                    donusDeger.sonuc.islemSonuc = true;
                    for (int i = 0; i < donusDeger.ObjeSayisi; i++)
                    {
                        HizmetBilgisiSatiri hbs = (HizmetBilgisiSatiri)donusDeger[i];
                        r = thb.EkolariAl(yil, hbs.birimKod.Replace(".", ""), tamamMiIlkMi, hizmetYili, hbs.siraNo);
                        if (thb.Hata() == "")
                        {
                            ArrayList hbs_miktarlar = new ArrayList();
                            while (r.Read())
                            {
                                HizmetBilgiEko hbe = new HizmetBilgiEko();
                                hbe.ekoKod = r.GetString(0);
                                hbe.miktar = (double)r.GetDecimal(1);
                                hbs_miktarlar.Add(hbe);
                            }
                            r.Close();
                            hbs.miktarlar = (HizmetBilgiEko[])hbs_miktarlar.ToArray(typeof(HizmetBilgiEko));
                        }
                        else
                        {
                            donusDeger.sonuc.DegerAta(thb.Hata());
                            break;
                        }
                    }
                }
                else
                    donusDeger.sonuc.DegerAta(thb.Hata());
                ve.VeriErisimKapat();

            }
            else
                donusDeger.sonuc.DegerAta(string.Format(ButceFormResI.BHFHBS001, kurumsalKod));

            return donusDeger;
        }

        /// <summary>
        /// Yeni başlıyacak hizmetlerin ekonomik kod bazında maliyetleri ve bitirilecek hizmetlerin ekonomik kod bazında maliyetlerinin kaydedildiği yordam.
        /// Yetki kontrolü yapıldıktan sonra, verilen birimin, verilen anahtarlardaki verileri silinir. Sonra her bir hizmet satırı veri tabanına kaydedilir. Hizmetin kaydedilmesinden sonra, hizmetle ilgili ekonomik kod bazında maliyetler kaydedilir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birime ait tamamlanacak yada başlayacak hizmetlerin maliyetlerinin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birime ait tamamlanacak yada başlayacak hizmetlerin maliyetlerinin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="tamamMiIlkMi">1 ise ilk defa yapılması planlanan hizmetler, 1 değil ise tamamlanması planlanan hizmetler</param>
        /// <param name="hizmetYili">3 yıllık süreçdeki hizmetlerle ilgili olduğundan hizmet yılı, yıl, yıl+1, yıl+2 değerleri almalıdır.</param>
        /// <param name="hizmetBilgisiSatirlari">Ekonomik kod bazında, hizmet maliyetlerinin tutulduğu dizi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod, int tamamMiIlkMi, int hizmetYili, TNSCollection hizmetBilgisiSatirlari)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            TNS.BHF.IsKurallari.Servis.KurumsalKodBirimKontrol(kurumsalKod, sonuc);
            if (!Servis.YazmaYetkisiVarMi(kullanan, yil, kurumsalKod))
                sonuc.DegerAta(string.Format(ButceFormResI.BHFHBS002, kurumsalKod));
            if(sonuc.islemSonuc)
            {
                kurumsalKod = kurumsalKod.Replace(".", "");
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
                TABHizmetBilgisi thb = new TABHizmetBilgisi(ve);
                ve.BeginTransaction();
                sonuc = thb.Sil(yil, kurumsalKod, tamamMiIlkMi, hizmetYili);
                for (int i = 0; i < hizmetBilgisiSatirlari.Count && sonuc.islemSonuc; i++)
                {
                    HizmetBilgisiSatiri hbs = (HizmetBilgisiSatiri)hizmetBilgisiSatirlari[i];
                    sonuc = thb.EkleTanim(yil, kurumsalKod, tamamMiIlkMi, hizmetYili, i + 1, hbs);
                    for (int j = 0; j < hbs.miktarlar.Length && sonuc.islemSonuc; j++)
                    {
                        HizmetBilgiEko hbe = hbs.miktarlar[j];
                        sonuc = thb.EkleMaliyet(yil, kurumsalKod, tamamMiIlkMi, hizmetYili, i + 1, hbe.ekoKod, hbe.miktar);
                    }
                }
                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
                ve.VeriErisimKapat();
            }
            return sonuc;
        }
    }
}
