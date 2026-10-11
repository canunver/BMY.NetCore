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
    /// Özel güvenlik ihaleleriyle ilgili sunucu yordamlarını tutan sınıf.
    /// </summary>
    public class OzelGuvenlikServisi
    {
        /// <summary>
        /// Özel güvenlik ihaleleriyle ilgili bilgilerin OzelGuvenlikBilgi nesnesi olarak döndüren yordam.
        /// Kullanıcının bu birime ait verileri görme yetkisi varsa, verilen kriterlere uyan ihale bilgileri veri tabanından alınır ve listeye doldurulur.
        /// Okunan her bir ihale bilgisi için, ihalenin tertipleri veri tabanından alınır ve tertip dizisine doldurulur.
        /// Son olarak da, ihale maliyet bilgileri veri tabanından okunur ve maliyet dizisine doldurulur.
        /// İhalelerle ilgili tüm bilgiler, okunduktan sonra, liste diziye çevrilerek döndürülür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Özel güvenlik ihalelesinin bilgisinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Özel güvenlik ihalelesinin bilgisinin listeleneceği birimin kurumsal kodu.</param>
        /// <param name="ihaleNo">Özel güvenlik ihalelesinin birim içi sıra numarası. 0 verilirse tüm ihaleler listelenir.</param>
        /// <returns>Birimin verilen sıradaki ihalesinin bilgileri döndürülür.</returns>
        public static OzelGuvenlikBilgi[] OzelGuvenlikOku(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod, int ihaleNo)
        {
            GuvenlikTemizlikIhaleTertip tertip;
            OzelGuvenlikMaliyet degerler;
            OzelGuvenlikBilgi aciklamaAlanlar;

            if (!Servis.GormeYetkisiVarMi(kullanan, kurumsalKod))
            {
                throw new Exception(ButceFormResI.BHMOGS001);
            }

            kurumsalKod = kurumsalKod.Replace(".", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
            TABOzelGuvenlik tabOzelGuv = new TABOzelGuvenlik(ve);


            IDataReader r = tabOzelGuv.OzelGuvenlikListele(yil, kurumsalKod, ihaleNo);

            if (tabOzelGuv.Hata() != "")
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }

            ArrayList al = new ArrayList();
            while (r.Read())
            {
                aciklamaAlanlar = new OzelGuvenlikBilgi();
                aciklamaAlanlar.kurKod = VeriErisim.DegerAl(r, 1, "");
                aciklamaAlanlar.ihaleTanim.ihaleNo = VeriErisim.DegerAl(r, 2, (int)(0));
                aciklamaAlanlar.ihaleTanim.aciklama = VeriErisim.DegerAl(r, 3, "");
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

            foreach (OzelGuvenlikBilgi bilgi in al)
            {
                ArrayList alTertip = new ArrayList();
                r = tabOzelGuv.TertipListele(yil, bilgi.kurKod, bilgi.ihaleTanim.ihaleNo);
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
                r = tabOzelGuv.IhaleMaliyetListele(yil, bilgi.kurKod, bilgi.ihaleTanim.ihaleNo);
                if (r != null)
                {
                    while (r.Read())
                    {
                        degerler = new OzelGuvenlikMaliyet();
                        degerler.kod = VeriErisim.DegerAl(r, 3, "");
                        degerler.Harcama = VeriErisim.DegerAl(r, 4, (double)(0));
                        degerler.Tahmin = VeriErisim.DegerAl(r, 5, (double)(0));
                        degerler.Teklif = VeriErisim.DegerAl(r, 6, (double)(0));
                        alTertip.Add(degerler);
                    }
                    r.Close();
                }
                ve.hata = "";
                bilgi.maliyetler = (OzelGuvenlikMaliyet[])alTertip.ToArray(typeof(OzelGuvenlikMaliyet));
            }
            ve.VeriErisimKapat();
            return (OzelGuvenlikBilgi[])al.ToArray(typeof(OzelGuvenlikBilgi));
        }

        /// <summary>
        /// Özel güvenlik ihaleleriyle ilgili bilgilerin sadece sıra numarası ve ihale adı döndüren yordam.
        /// Kullanıcının bu birime ait verileri görme yetkisi varsa, verilen kriterlere uyan ihale bilgileri veri tabanından alınır ve listeye doldurulur ve liste diziye çevrilerek döndürülür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Özel güvenlik ihalelesinin kimlik bilgisinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Özel güvenlik ihalelesinin kimlik bilgisinin listeleneceği birimin kurumsal kodu.</param>
        /// <returns>Özel güvenlik ihaleleriyle ilgili bilgilerin sadece sıra numarası ve ihale adı OzelGuvenlikBilgi yapısında döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod)
        {
            IDataReader r;

            OzelGuvenlikIhaleTanim ihaleler;
            ObjectArray donusDeger = new ObjectArray();

            if (Servis.GormeYetkisiVarMi(kullanan, kurumsalKod))
            {
                kurumsalKod = kurumsalKod.Replace(".", "");

                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
                TABOzelGuvenlik tabOzelGuv = new TABOzelGuvenlik(ve);

                r = tabOzelGuv.OzelGuvenlikListele(yil, kurumsalKod, 0);

                if (tabOzelGuv.Hata() == "")
                {
                    while (r.Read())
                    {
                        ihaleler = new OzelGuvenlikIhaleTanim();
                        ihaleler.ihaleNo = VeriErisim.DegerAl(r, 2, (int)(0));
                        ihaleler.aciklama = VeriErisim.DegerAl(r, 3, "");

                        donusDeger.Ekle(ihaleler);
                    }
                    r.Close();
                    donusDeger.sonuc.islemSonuc = true;
                }
                else
                {
                    donusDeger.sonuc.hataStr = tabOzelGuv.Hata();
                    donusDeger.sonuc.islemSonuc = false;
                }

                ve.VeriErisimKapat();
            }
            else
                donusDeger.sonuc.DegerAta(ButceFormResI.BHMOGS002);
            return donusDeger;
        }

        /// <summary>
        /// Özel güvenlik ihaleleriyle ilgili bilgilerin kaydedildiği yordam. İhale bilgisi OzelGuvenlikBilgi nesnesi olarak verilir.
        /// Kullanıcının yetkisi varsa, önce ihale, ihalenin tertipleri ve maliyet bilgileri silinir. Sonra ihale bilgisi , tertipleri ve maliyet bilgileri veri tabanına eklenir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Özel güvenlik ihaleleriyle ilgili bilgilerin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Özel güvenlik ihaleleriyle ilgili bilgilerin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="ihaleNo">Özel güvenlik ihalelesinin birim içi sıra numarası.</param>
        /// <param name="ihale">Birimin verilen sıradaki ihalesinin bilgilerini tutan yapı.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod, int ihaleNo, OzelGuvenlikBilgi ihale)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            kurumsalKod = kurumsalKod.Replace(".", "");
            if (kurumsalKod.Length != 8)
                sonuc.DegerAta("Lütfen Birim Seçiniz");

            if (!Servis.YazmaYetkisiVarMi(kullanan, yil, kurumsalKod))
                sonuc.DegerAta(ButceFormResI.BHMOGS003);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));

            TABOzelGuvenlik tabOzelGuv = new TABOzelGuvenlik(ve);
            ve.BeginTransaction();
            int ihaleNoVer = ihaleNo;
            if (ihaleNoVer == 0)
            {
                ihaleNoVer = Convert.ToInt16(tabOzelGuv.IhaleNoBul(yil, kurumsalKod));
                ihaleNoVer = ihaleNoVer + 1;
            }
            else
            {
                sonuc = tabOzelGuv.IhaleMaliyetleriSil(yil, kurumsalKod, ihaleNo);
                if (sonuc.islemSonuc)
                    sonuc = tabOzelGuv.IhaleTertipleriSil(yil, kurumsalKod, ihaleNo);
                if (sonuc.islemSonuc)
                    sonuc = tabOzelGuv.IhaleSil(yil, kurumsalKod, ihaleNo);
            }

            if (sonuc.islemSonuc)
            {
                sonuc = tabOzelGuv.IhaleEkle(yil, kurumsalKod, ihaleNoVer, ihale);
                for (int i = 0; i < ihale.tertipler.Length; i++)
                {
                    GuvenlikTemizlikIhaleTertip tertip = ihale.tertipler[i];
                    sonuc = tabOzelGuv.TertipEkle(yil, kurumsalKod, ihaleNoVer, tertip.kurKod, tertip.fonKod, tertip.finKod, tertip.ekoKod);
                }

                for (int j = 0; j < ihale.maliyetler.Length; j++)
                {
                    OzelGuvenlikMaliyet maliyet = ihale.maliyetler[j];
                    sonuc = tabOzelGuv.IhaleMaliyetEkle(yil, kurumsalKod, ihaleNoVer, maliyet.kod, maliyet.Harcama, maliyet.Tahmin, maliyet.Teklif);
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
