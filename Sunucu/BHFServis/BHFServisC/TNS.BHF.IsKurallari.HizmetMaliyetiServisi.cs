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
    /// Birimlerin hizmetlerini gerçekleştirmek amacıyla yaptıkları işlemlerle ilgili işlemleri tutan sınıf.
    /// </summary>
    public class HizmetMaliyetiServisi
    {
        /// <summary>
        /// Birimlerin hizmetlerini gerçekleştirmek amacıyla yaptıkları işlemlerin detaylandırıldığı formun detaylarını satır satır listeleyen yordam.
        /// Önce hizmet maliyeti sözlük bilgileri okunur. Sonra her bir sözlük satırı için, ödenek ve gerçekleşme bilgileri okunur.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birim hizmetlerinin maliyetlerinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birim hizmetlerin maliyetlerinin listeleneceği birimin kurumsal kodu.</param>
        /// <returns>Hizmet maliyeti tesbitinde kullanılan satırlar döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod)
        {
            IDataReader r;
            HizmetMaliyetiTesbitiSatir hms;
            ObjectArray donusDeger = new ObjectArray();
            donusDeger.sonuc.islemSonuc = false;
            string ustKod = "";

            if (Servis.GormeYetkisiVarMi(kullanan, kurumsalKod))
            {
                kurumsalKod = kurumsalKod.Replace(".", "");
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
                TABHizmetMaliyet thm = new TABHizmetMaliyet(ve);
                r = thm.KodlariAl();
                if (thm.Hata() == "")
                {
                    while (r.Read())
                    {
                        hms = new HizmetMaliyetiTesbitiSatir();
                        hms.kod = r.GetString(0);
                        ustKod = hms.kod;
                        hms.aciklama = r.GetString(1);
                        hms.degerGirilsinMi = r.GetInt32(2) == 1;
                        donusDeger.Ekle(hms);
                    }
                    r.Close();
                    donusDeger.sonuc.islemSonuc = true;
                    for (int i = 0; i < donusDeger.ObjeSayisi; i++)
                    {
                        hms = (HizmetMaliyetiTesbitiSatir)donusDeger[i];
                        r = thm.DegeriAl(yil, kurumsalKod, hms.kod);
                        hms.kod = TNS.OrtakFonksiyonlar.KodNoktaKoy("9.99.99.99.99", hms.kod);
                        if (thm.Hata() == "")
                        {
                            if (r.Read())
                            {
                                hms.gercek = VeriErisim.DegerAl(r, 0, (double)0);
                                hms.haziran = VeriErisim.DegerAl(r, 1, (double)0);
                                hms.tahmin = VeriErisim.DegerAl(r, 2, (double)0);
                                hms.tahmin1 = VeriErisim.DegerAl(r, 3, (double)0);
                                hms.tahmin2 = VeriErisim.DegerAl(r, 4, (double)0);
                            }
                            else
                            {
                                hms.gercek = 0;
                                hms.haziran = 0;
                                hms.tahmin = 0;
                                hms.tahmin1 = 0;
                                hms.tahmin2 = 0;
                            }
                            r.Close();
                        }
                        else
                        {
                            donusDeger.sonuc.hataStr = thm.Hata();
                            donusDeger.sonuc.islemSonuc = false;
                            break;
                        }
                    }
                    if (donusDeger.sonuc.islemSonuc && ustKod != "")
                    {
                        r = thm.DegerleriAlDiger(yil, kurumsalKod, ustKod);
                        if (thm.Hata() == "")
                        {
                            int i = 1;
                            while (r.Read())
                            {
                                hms = new HizmetMaliyetiTesbitiSatir();
                                hms.kod = ustKod + "." + i.ToString().PadLeft(2, '0');
                                hms.gercek = VeriErisim.DegerAl(r, 2, (double)0);
                                hms.haziran = VeriErisim.DegerAl(r, 3, (double)0);
                                hms.tahmin = VeriErisim.DegerAl(r, 4, (double)0);
                                hms.tahmin1 = VeriErisim.DegerAl(r, 5, (double)0);
                                hms.tahmin2 = VeriErisim.DegerAl(r, 6, (double)0);
                                hms.aciklama = VeriErisim.DegerAl(r, 7, "");
                                hms.degerGirilsinMi = true;
                                donusDeger.Ekle(hms);
                                i++;
                            }
                            r.Close();
                        }
                        else
                        {
                            donusDeger.sonuc.hataStr = thm.Hata();
                            donusDeger.sonuc.islemSonuc = false;
                        }
                    }
                }
                else
                    donusDeger.sonuc.hataStr = thm.Hata();
                ve.VeriErisimKapat();

            }
            else
                donusDeger.sonuc.hataStr = string.Format(ButceFormResI.BHFHMS001, kurumsalKod);

            return donusDeger;
        }

        /// <summary>
        /// Birimlerin hizmetlerini gerçekleştirmek amacıyla yaptıkları işlemlerin detaylandırıldığı formun detaylarını satır satır alıp kaydeden yordam.
        /// Değer girilebilir işareti konulmuş sözlük satırları veri tabanına yetki kontrolünden sonra kaydedilir.
        /// Fakat 6 ile başlayan grup, birimin kendi açıklamalarını girdiği ve sözlükte olmayan kayıtlar olduğu için, o satırlar için kod üretilir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birim hizmetlerinin maliyetlerinin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birim hizmetlerin maliyetlerinin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="hizmetMaliyeti">Hizmet maliyetinin tesbiti için kullanılan satırlar dizisi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod, TNSCollection hizmetMaliyeti)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string digerBilgi = "", kod = "";

            TNS.BHF.IsKurallari.Servis.KurumsalKodBirimKontrol(kurumsalKod, sonuc);
            if (!Servis.YazmaYetkisiVarMi(kullanan, yil, kurumsalKod))
                sonuc.DegerAta(string.Format(ButceFormResI.BHFHBS002, kurumsalKod));


            if (sonuc.islemSonuc)
            {
                kurumsalKod = kurumsalKod.Replace(".", "");
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
                TABHizmetMaliyet thm = new TABHizmetMaliyet(ve);
                ve.BeginTransaction();
                sonuc = thm.Sil(yil, kurumsalKod);
                if (sonuc.islemSonuc)
                {
                    int kod6Sayac = 0;
                    foreach (HizmetMaliyetiTesbitiSatir hms in hizmetMaliyeti)
                    {
                        if (hms.degerGirilsinMi)
                        {
                            kod = hms.kod.Replace(".", "");
                            if (kod.StartsWith("6") || kod == "")
                            {
                                kod6Sayac++;
                                kod = "6" + kod6Sayac.ToString().PadLeft(2, '0');
                                digerBilgi = hms.aciklama;
                            }
                            else
                                digerBilgi = "";
                            sonuc = thm.Ekle(yil, kurumsalKod, kod, hms, digerBilgi);
                            if (!sonuc.islemSonuc)
                                break;
                        }
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

        /// <summary>
        /// eBütçeden çekilen hizmet maliyetinin tesbiti sözlük bilgilerini kaydeden yordam.
        /// Kullanıcı sistem yöneticisi ise bu yordam çalışır. Önce tüm kayıtlar kontrol edilir. Yetki varsa ve kayıtlar kontrolden geçmiş ise, eski kayıtlar silinir, fizikselDegerlerSozlukler dizisindeki her bir satır veri tabanına kaydedilir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="hizmetMaliyetiSozlukler">eBütçeden çekilen hizmet maliyetinin tesbiti sözlük bilgilerini tutan dizi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc SozlukKaydet(VeriErisim veriErisim, Kullanici kullanan, TNSCollection hizmetMaliyetiSozlukler)
        {
            Sonuc sonuc = new Sonuc();

            if (!Servis.Yoneticimi(kullanan))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ButceFormResI.BHFHMS003;
                return sonuc;
            }

            foreach (HizmetMaliyetiTesbitiSatir hms in hizmetMaliyetiSozlukler)
            {
                if (string.IsNullOrEmpty(hms.kod))
                    sonuc.hataStr = ButceFormResI.BHFHMS004 + "<br>";
                if (string.IsNullOrEmpty(hms.aciklama))
                    sonuc.hataStr += ButceFormResI.BHFHMS005 + "<br>";

                if (!string.IsNullOrEmpty(sonuc.hataStr))
                {
                    sonuc.islemSonuc = false;
                    return sonuc;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(), TNS.KYM.Arac.KimlikBul(kullanan));
            TABHizmetMaliyet thm = new TABHizmetMaliyet(ve);
            ve.BeginTransaction();

            thm.KodSil();
            foreach (HizmetMaliyetiTesbitiSatir hms in hizmetMaliyetiSozlukler)
            {
                sonuc = thm.KodKaydet(hms);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = ButceFormResI.BHFHMS006;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = ButceFormResI.BHFHMS007 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Hizmet maliyeti ile ilgili sözlük bilgilerini listeleyen yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <returns>Bulunan sözlük bilgileri dizi olarak döndürülür.</returns>
        public static HizmetMaliyetiSatir[] SozlukListele(VeriErisim veriErisim)
        {
            ArrayList donusDeger = new ArrayList();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(), TNS.KYM.Arac.KimlikBul(null));
            TABHizmetMaliyet thm = new TABHizmetMaliyet(ve);

            IDataReader r = thm.KodlariAl();
            if (!string.IsNullOrEmpty(thm.Hata()))
            {
                throw new Exception(thm.Hata());
            }

            while (r.Read())
            {
                HizmetMaliyetiSatir hms = new HizmetMaliyetiSatir();
                hms.kod = VeriErisim.DegerAl(r, 0, string.Empty);
                hms.aciklama = VeriErisim.DegerAl(r, 1, string.Empty);
                hms.degerGirilsinMi = VeriErisim.DegerAl(r, 2, 0) == 1;

                donusDeger.Add(hms);
            }
            r.Close();

            if (donusDeger.Count <= 0)
            {
                ve.VeriErisimKapat();
                throw new Exception(ButceFormResI.BHFHMS008);
            }
            ve.VeriErisimKapat();
            return (HizmetMaliyetiSatir[])donusDeger.ToArray(typeof(HizmetMaliyetiSatir));
        }

    }
}
