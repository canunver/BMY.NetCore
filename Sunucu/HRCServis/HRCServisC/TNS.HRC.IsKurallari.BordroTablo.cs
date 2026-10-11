using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// HRCBordroTablo formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class HRCBordroTablo
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriHRC";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            else
                return false;
        }

        public static ObjectArray ListeleTanim(VeriErisim veriErisim, Kullanici kullanan)
        {
            ObjectArray oa = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCBORDROTABLOTANIM tab = new TABHRCBORDROTABLOTANIM(ve);

            IDataReader dr = tab.OkuTanim(0);
            if (dr == null)
            {
                oa.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return oa;
            }

            while (dr.Read())
            {
                BordroTabloAna ana = new BordroTabloAna();
                ana.kod = VeriErisim.DegerAl(dr, 0, 0);
                ana.ad = VeriErisim.DegerAl(dr, 1, "");
                ana.tur = (BordroTabloAna.ENUMBordroTabloTur)VeriErisim.DegerAl(dr, 2, 0);
                oa.Ekle(ana);
            }
            ve.VeriErisimKapat();
            return oa;
        }

        internal static Sonuc KaydetBordroTabloTanim(VeriErisim veriErisim, Kullanici kullanan, BordroTabloAna tabloTanim)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan))
                s.DegerAta(HarcamaSurecleriResI.HRC00013);
            if (tabloTanim == null)
                s.DegerAta("Lütfen veri giriniz!");
            else
            {
                if (tabloTanim.kod <= 0)
                    s.DegerAta("Kod için sıfırdan büyün bir değer giriniz!");
                if (string.IsNullOrWhiteSpace(tabloTanim.ad))
                    s.DegerAta("Lütfen ad alanını doldurunuz!");
            }
            if (!s.islemSonuc)
                return s;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCBORDROTABLOTANIM tab = new TABHRCBORDROTABLOTANIM(ve);

            IDataReader dr = tab.OkuTanim(tabloTanim.kod);
            if (dr == null)
            {
                s.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return s;
            }
            bool kayitVar = false;
            if (dr.Read())
            {
                kayitVar = true;
            }
            dr.Close();
            ve.BeginTransaction();
            if (kayitVar)
                s = tab.GuncelleTanim(tabloTanim);
            else
                s = tab.EkleTanim(tabloTanim);
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc KopyalaBordroTablo(VeriErisim veriErisim, Kullanici kullanan, int yil, int hedefKod, int kaynakKod, int unvanKod, bool onceSil)
        {
            Sonuc s = new Sonuc();
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                s.DegerAta("Sistem yöneticileri kopyalayabilir!");
            }
            if (hedefKod == 0)
            {
                s.DegerAta("Hedef tablo satırını açınız!");
            }
            if (kaynakKod == 0)
            {
                s.DegerAta("Kaynak tablo kodunu giriniz!");
            }

            if (!s.islemSonuc)
                return s;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCBORDROTABLOTANIM tab = new TABHRCBORDROTABLOTANIM(ve);

            ve.BeginTransaction();
            if (onceSil)
            {
                s = tab.SilDetay(hedefKod, yil, unvanKod);
            }
            if (s.islemSonuc)
            {
                s = tab.KopyalaDetay(hedefKod, yil, unvanKod, kaynakKod);
            }
            if (s.islemSonuc)
            {
                ve.Commit();
            }
            else
            {
                ve.Rollback();
            }

            return s;
        }

        /// <summary>
        /// BordroTabloTanim nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="degerler">BordroTabloTanim yapısında BordroTabloTanim: Bordro Tablo Tanım bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int tabloKod, int yil, int unvanKod, TNSCollection degerler)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan))
            {
                s.DegerAta(HarcamaSurecleriResI.HRC00013);
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCBORDROTABLOTANIM tab = new TABHRCBORDROTABLOTANIM(ve);

            IDataReader dr = tab.OkuTanim(tabloKod);
            if (dr == null)
            {
                s.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return s;
            }
            bool kayitVar = false;
            if (dr.Read())
            {
                kayitVar = true;
            }
            dr.Close();
            if (!kayitVar)
            {
                s.DegerAta("Verilen tablo kodu bulunamadı!");
                ve.VeriErisimKapat();
                return s;
            }

            ve.BeginTransaction();
            if (s.islemSonuc)
                s = tab.SilDetay(tabloKod, yil, unvanKod);

            for (int i = 0; i < degerler.Count && s.islemSonuc; i++)
            {
                BordroTabloDetay btd = (BordroTabloDetay)degerler[i];
                if (btd.deger != 0)
                    s = tab.EkleDetay(tabloKod, yil, unvanKod, btd);
            }

            if (s.islemSonuc)
            {
                ve.Commit();

            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// BordroTabloTanim nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BordroTabloTanim yapısında BordroTabloTanim: Bordro Tablo Tanım bilgilerini tutan değişken</param>
        public static decimal Oku(VeriErisim veriErisim, Kullanici kullanan, int tabloKod, int unvanKod, int derece, int yil)
        {
            BordroTabloDetay form = new BordroTabloDetay();
            form.kod = tabloKod;
            form.unvanKod = unvanKod;
            form.derece = derece;
            form.yil = yil;
            ObjectArray s = Listele(veriErisim, kullanan, form, false);
            if (s.sonuc.islemSonuc && s.ObjeSayisi == 1)
            {
                form = (BordroTabloDetay)s.objeler[0];
                return form.deger * form.carpan;
            }
            return 0;
        }

        /// <summary>
        /// BordroTabloTanim listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BordroTabloTanim yapısında BordroTabloTanim: Bordro Tablo Tanım bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, BordroTabloDetay form, bool bosEkle)
        {
            ObjectArray oa = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCBORDROTABLOTANIM tabHRCBORDROTABLODETAY = new TABHRCBORDROTABLOTANIM(ve);

            IDataReader dr = tabHRCBORDROTABLODETAY.ListeleDetay(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            BordroTabloDetay onceki = null;
            while (dr.Read())
            {
                BordroTabloDetay detay = new BordroTabloDetay();
                detay.kod = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTABLOTANIM.ENUMAlanlarDETAY.KOD, 0);
                detay.yil = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTABLOTANIM.ENUMAlanlarDETAY.YIL, 0);
                detay.unvanKod = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTABLOTANIM.ENUMAlanlarDETAY.UNVANKOD, 0);
                detay.derece = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTABLOTANIM.ENUMAlanlarDETAY.DERECE, 0);
                detay.deger = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTABLOTANIM.ENUMAlanlarDETAY.DEGER, (decimal)0.0);
                detay.carpan = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTABLOTANIM.ENUMAlanlarDETAY.CARPAN, (decimal)0.0);
                detay.ad = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTABLOTANIM.ENUMAlanlarDETAY.CARPAN + 1, "");
                detay.unvanAd = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTABLOTANIM.ENUMAlanlarDETAY.CARPAN + 2, "");
                if (onceki == null || onceki.kod != detay.kod || onceki.unvanKod != detay.unvanKod)
                {
                    if (onceki != null)
                    {
                        if (bosEkle)
                            SiraliEkle(oa, onceki, 16);
                        onceki = null;
                    }
                }

                if (onceki == null)
                {
                    onceki = new BordroTabloDetay(detay);
                    onceki.derece = 0;
                }
                if (bosEkle)
                    SiraliEkle(oa, onceki, detay.derece);
                onceki = detay;
                oa.Ekle(detay);
            }
            dr.Close();
            if (onceki == null)
            {
                onceki = new BordroTabloDetay(form);
                onceki.derece = 0;
            }
            if (bosEkle)
                SiraliEkle(oa, onceki, 16);
            ve.VeriErisimKapat();
            return oa;
        }

        private static void SiraliEkle(ObjectArray oa, BordroTabloDetay onceki, int bitisDerece)
        {
            for (int i = onceki.derece + 1; i < bitisDerece; i++)
            {
                BordroTabloDetay btd = new BordroTabloDetay(onceki);
                btd.derece = i;
                btd.deger = 0;
                btd.carpan = 1;
                oa.Ekle(btd);
            }
        }

        internal static Sonuc BordroTabloArttir(VeriErisim veriErisim, Kullanici kullanan, int tabloKod, int yil, double artisKatsayisi)
        {
            Sonuc s = new Sonuc();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            TABHRCBORDROTABLOTANIM tabHRCBORDROTABLODETAY = new TABHRCBORDROTABLOTANIM(ve);
            ve.BeginTransaction();

            s = tabHRCBORDROTABLODETAY.SilDetay(tabloKod + 1000, yil, -1);
            if (s.islemSonuc)
                s = tabHRCBORDROTABLODETAY.Kopyala(tabloKod, tabloKod + 1000, yil);
            if (s.islemSonuc)
                s = tabHRCBORDROTABLODETAY.Carp(tabloKod, yil, artisKatsayisi);
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }
    }
}
