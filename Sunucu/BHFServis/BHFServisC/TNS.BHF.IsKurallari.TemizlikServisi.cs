using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using TNS.KYM;
using System.Data;
using System.Collections;
using TNS.BHF.IsKurallari.Veri;

namespace TNS.BHF.IsKurallari
{
    /// <summary>
    /// Temizlik hizmeti ihaleleriyle ilgili sunucu yordamlarını tutan sınıf.
    /// </summary>
    public class TemizlikServisi
    {
        /// <summary>
        /// Temizlik hizmeti ihaleleriyle ilgili bilgilerin OzelGuvenlikBilgi nesnesi olarak döndüren yordam.
        /// Kullanıcının bu birime ait verileri görme yetkisi varsa, verilen kriterlere uyan ihale bilgileri veri tabanından alınır ve listeye doldurulur.
        /// Okunan her bir ihale bilgisi için, ihalenin tertipleri veri tabanından alınır ve tertip dizisine doldurulur.
        /// Son olarak da, ihale maliyet bilgileri veri tabanından okunur ve maliyet dizisine doldurulur.
        /// İhalelerle ilgili tüm bilgiler, okunduktan sonra, liste diziye çevrilerek döndürülür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Temizlik hizmeti ihalelesinin bilgisinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Temizlik hizmeti ihalelesinin bilgisinin listeleneceği birimin kurumsal kodu.</param>
        /// <param name="ihaleNo">Temizlik hizmeti ihalelesinin birim içi sıra numarası. 0 verilirse tüm ihaleler listelenir.</param>
        /// <returns>Birimin verilen sıradaki ihalesinin yada tüm ihalelerinin bilgileri döndürülür.</returns>
        public static TemizlikBilgi[] TemizlikOku(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod, int ihaleNo)
        {
            IDataReader r;
            GuvenlikTemizlikIhaleTertip tertip;
            TemizlikMaliyet degerler;
            TemizlikBilgi aciklamaAlanlar;

            if (!Servis.GormeYetkisiVarMi(kullanan, kurumsalKod))
            {
                throw new Exception(ButceFormResI.BHMTS001);
            }

            kurumsalKod = kurumsalKod.Replace(".", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
            TABTemizlik tabTemizlik = new TABTemizlik(ve);

            r = tabTemizlik.TemizlikListele(yil, kurumsalKod, ihaleNo);

            if (tabTemizlik.Hata() != "")
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }

            ArrayList al = new ArrayList();
            while (r.Read())
            {
                aciklamaAlanlar = new TemizlikBilgi();
                aciklamaAlanlar.kurKod = VeriErisim.DegerAl(r, 1, "");
                aciklamaAlanlar.ihaleNo = VeriErisim.DegerAl(r, 2, (int)(0));
                aciklamaAlanlar.aciklama = VeriErisim.DegerAl(r, 3, "");
                aciklamaAlanlar.hzmAlanOnceki = VeriErisim.DegerAl(r, 4, (int)(0));
                aciklamaAlanlar.hzmAlanBuYil = VeriErisim.DegerAl(r, 5, (int)(0));
                aciklamaAlanlar.hzmAlanSonraki = VeriErisim.DegerAl(r, 6, (int)(0));
                aciklamaAlanlar.gorSayisiOnceki = VeriErisim.DegerAl(r, 7, (int)(0));
                aciklamaAlanlar.gorSayisiBuYil = VeriErisim.DegerAl(r, 8, (int)(0));
                aciklamaAlanlar.gorSayisiSonraki = VeriErisim.DegerAl(r, 9, (int)(0));
                aciklamaAlanlar.basOnceki = VeriErisim.DegerAl(r, 10, "");
                aciklamaAlanlar.basBuYil = VeriErisim.DegerAl(r, 11, "");
                aciklamaAlanlar.basSonraki = VeriErisim.DegerAl(r, 12, "");
                aciklamaAlanlar.bitisOnceki = VeriErisim.DegerAl(r, 13, "");
                aciklamaAlanlar.bitisBuYil = VeriErisim.DegerAl(r, 14, "");
                aciklamaAlanlar.bitisSonraki = VeriErisim.DegerAl(r, 15, "");

                al.Add(aciklamaAlanlar);
            }
            r.Close();

            foreach (TemizlikBilgi bilgi in al)
            {
                ArrayList alTertip = new ArrayList();
                r = tabTemizlik.TertipListele(yil, bilgi.kurKod, bilgi.ihaleNo);
                if (r != null)
                {
                    while (r.Read())
                    {
                        tertip = new GuvenlikTemizlikIhaleTertip();
                        tertip.kurKod = VeriErisim.DegerAl(r, 3, "");
                        tertip.fonKod = VeriErisim.DegerAl(r, 4, "");
                        tertip.finKod = VeriErisim.DegerAl(r, 5, "");
                        tertip.ekoKod = VeriErisim.DegerAl(r, 6, "");

                        alTertip.Add(tertip);
                    }
                    r.Close();
                }
                bilgi.tertipler = (GuvenlikTemizlikIhaleTertip[])alTertip.ToArray(typeof(GuvenlikTemizlikIhaleTertip));
                ve.hata = "";

                alTertip.Clear();
                r = tabTemizlik.IhaleMaliyetListele(yil, bilgi.kurKod, bilgi.ihaleNo);
                if (r != null)
                {
                    while (r.Read())
                    {
                        degerler = new TemizlikMaliyet();
                        degerler.kod = VeriErisim.DegerAl(r, 3, "");
                        degerler.Harcama = VeriErisim.DegerAl(r, 4, (double)(0));
                        degerler.Tahmin = VeriErisim.DegerAl(r, 5, (double)(0));
                        degerler.Teklif = VeriErisim.DegerAl(r, 6, (double)(0));
                        alTertip.Add(degerler);
                    }
                    r.Close();
                }
                ve.hata = "";
                bilgi.maliyetler = (TemizlikMaliyet[])alTertip.ToArray(typeof(TemizlikMaliyet));
            }
            ve.VeriErisimKapat();
            return (TemizlikBilgi[])al.ToArray(typeof(TemizlikBilgi));
        }

        /// <summary>
        /// Temizlik hizmeti ihaleleriyle ilgili bilgilerin kaydedildiği yordam. İhale bilgisi OzelGuvenlikBilgi nesnesi olarak verilir.
        /// Kullanıcının yetkisi varsa, önce ihale, ihalenin tertipleri ve maliyet bilgileri silinir. Sonra ihale bilgisi , tertipleri ve maliyet bilgileri veri tabanına eklenir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Temizlik hizmeti ihaleleriyle ilgili bilgilerin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Temizlik hizmeti ihaleleriyle ilgili bilgilerin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="ihaleNo">Temizlik hizmeti ihalelesinin birim içi sıra numarası.</param>
        /// <param name="ihale">Birimin verilen sıradaki ihalesinin bilgilerini tutan yapı.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod, int ihaleNo, TemizlikBilgi ihale)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            kurumsalKod = kurumsalKod.Replace(".", "");
            if (kurumsalKod.Length != 8)
                sonuc.DegerAta("Lütfen Birim Seçiniz");

            if (!Servis.YazmaYetkisiVarMi(kullanan, yil, kurumsalKod))
                sonuc.DegerAta(ButceFormResI.BHMTS002);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));

            TABTemizlik tabTemizlik = new TABTemizlik(ve);
            ve.BeginTransaction();
            int ihaleNoVer = ihaleNo;
            if (ihaleNoVer <= 0)
            {
                ihaleNoVer = Convert.ToInt16(tabTemizlik.IhaleNoBul(yil, kurumsalKod));
                ihaleNoVer = ihaleNoVer + 1;
            }
            else
            {
                sonuc = tabTemizlik.IhaleMaliyetleriSil(yil, kurumsalKod, ihaleNo);
                if (sonuc.islemSonuc)
                    sonuc = tabTemizlik.IhaleTertipleriSil(yil, kurumsalKod, ihaleNo);
                if (sonuc.islemSonuc)
                    sonuc = tabTemizlik.IhaleSil(yil, kurumsalKod, ihaleNo);
            }

            if (sonuc.islemSonuc)
            {
                sonuc = tabTemizlik.IhaleEkle(yil, kurumsalKod, ihaleNoVer, ihale);
                for (int i = 0; i < ihale.tertipler.Length; i++)
                {
                    GuvenlikTemizlikIhaleTertip tertip = ihale.tertipler[i];
                    sonuc = tabTemizlik.TertipEkle(yil, kurumsalKod, ihaleNoVer, tertip.kurKod, tertip.fonKod, tertip.finKod, tertip.ekoKod);
                }

                for (int j = 0; j < ihale.maliyetler.Length; j++)
                {
                    TemizlikMaliyet maliyet = ihale.maliyetler[j];
                    sonuc = tabTemizlik.IhaleMaliyetEkle(yil, kurumsalKod, ihaleNoVer, maliyet.kod, maliyet.Harcama, maliyet.Tahmin, maliyet.Teklif);
                }
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.anahtar = ihaleNoVer.ToString();
            }
            else
                ve.Rollback();
            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}

/*

        //public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurum)
        //{
        //    IDataReader r;

        //    TemizlikIhaleler ihaleler = new TemizlikIhaleler();
        //    ObjectArray donusDeger = new ObjectArray();
        //    ihaleler.sonuc.islemSonuc = false;

        //    if (Servis.GormeYetkisiVarMi(kullanan, kurum))
        //    {
        //        kurum = kurum.Replace(".", "");

        //        VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil));
        //        TABTemizlik tabTemizlik = new TABTemizlik(ve);

        //        r = tabTemizlik.TemizlikListele(yil, kurum, 0);

        //        if (tabTemizlik.Hata() == "")
        //        {
        //            while (r.Read())
        //            {
        //                ihaleler = new TemizlikIhaleler();
        //                ihaleler.ihaleNo = VeriErisim.DegerAl(r, 2, (int)(0));
        //                ihaleler.aciklama = VeriErisim.DegerAl(r, 3, "");

        //                donusDeger.Ekle(ihaleler);
        //            }
        //            r.Close();
        //            ihaleler.sonuc.islemSonuc = true;
        //        }
        //        else
        //        {
        //            ihaleler.sonuc.hataStr = tabTemizlik.Hata();
        //            ihaleler.sonuc.islemSonuc = false;
        //        }

        //        ve.VeriErisimKapat();
        //    }
        //    return donusDeger;
        //}



*/