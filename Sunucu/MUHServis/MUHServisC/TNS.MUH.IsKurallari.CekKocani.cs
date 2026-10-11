using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.MUH;
using TNS.MUH.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using TNS.BKD;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// MUHCekKocani formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class MUHCekKocani
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriBUM";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, CekKocani form)
        {
            return true;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, CekKocani form)
        {
            return new Sonuc(true, "", "");
        }

        internal static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, CekKocani form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta("Yazma yetkiniz yok!");
                return s;
            }

            s = AnahtarAlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABMUHCEKKOCANI tabMUHCEKKOCANI = new TABMUHCEKKOCANI(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabMUHCEKKOCANI.Sil(form);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// CekKocani nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">CekKocani yapısında CekKocani: Çek Koçanındaki Sayfalar bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, CekKocani form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta("Yazma yetkiniz yok!");
                return s;
            }

            s = AlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABMUHCEKKOCANI tabMUHCEKKOCANI = new TABMUHCEKKOCANI(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabMUHCEKKOCANI.Sil(form);

            if (s.islemSonuc)
                s = tabMUHCEKKOCANI.Ekle(form);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }


        /// <summary>
        /// CekKocani nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">CekKocani yapısında CekKocani: Çek Koçanındaki Sayfalar bilgilerini tutan değişken</param>
        //public static ObjectVAN Oku(VeriErisim veriErisim, Kullanici kullanan, CekKocani form)
        //{
        //    ObjectVAN ov = new ObjectVAN();
        //    ObjectArray s = Listele(veriErisim, kullanan, form);
        //    ov.sonuc = s.sonuc;
        //    if (s != null && s.ObjeSayisi > 0) ov.o = s[0];
        //    return ov;
        //}

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, CekKocani form)
        {
            return true;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, CekKocani form)
        {
            return new Sonuc(true, "", "");
        }

        /// <summary>
        /// CekKocani listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">CekKocani yapısında CekKocani: Çek Koçanındaki Sayfalar bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, CekKocani form, bool acik, bool bozdurulmus, bool iptalli, TNSDateTime baslaTarih, TNSDateTime bitisTarih)
        {
            ObjectArray oa = new ObjectArray();
            if (!GormeYetkisiVarMi(kullanan, form))
            {
                oa.sonuc.DegerAta("Görme yetkiniz yok");
                return oa;
            }

            Sonuc s = AnahtarAlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                oa.sonuc.DegerAta(s.hataStr);
                return oa;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABMUHCEKKOCANI tabMUHCEKKOCANI = new TABMUHCEKKOCANI(ve);

            IDataReader dr;
            dr = tabMUHCEKKOCANI.Listele(form, baslaTarih, bitisTarih);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            Tertip th = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdemeEmri);

            while (dr.Read())
            {
                CekKocani detay = new CekKocani();
                detay.hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)th.Kirilimlar(0), VeriErisim.DegerAl(dr, 0, ""));
                detay.cekNo = VeriErisim.DegerAl(dr, 1, "");

                detay.gun = VeriErisim.DegerAl(dr, 2);
                detay.mifBelgeNo = VeriErisim.DegerAl(dr, 3, "");
                detay.tcVno = VeriErisim.DegerAl(dr, 4, "");
                detay.ilgili = VeriErisim.DegerAl(dr, 5, "");
                detay.tutar = VeriErisim.DegerAl(dr, 6, 0.0);
                detay.odemeEmriNo = VeriErisim.DegerAl(dr, 7, "");
                detay.tahsilTarihi = VeriErisim.DegerAl(dr, 8);
                detay.iptalTarihi = VeriErisim.DegerAl(dr, 9);
                detay.iptalNedeni = VeriErisim.DegerAl(dr, 10, "");
                CekKocani.CekDurum cd = detay.cekDurum;
                if (acik && cd == CekKocani.CekDurum.Acik || bozdurulmus && cd == CekKocani.CekDurum.Bozduruldu || iptalli && cd == CekKocani.CekDurum.Iptalli)
                    oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }
    }
}
