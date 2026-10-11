using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHF;
using TNS.BHF.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.BHF.IsKurallari
{
    /// <summary>
    /// BHFTasitTip formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class BHFTasitCins
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriBHF";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, TasitCins form)
        {
            return true;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, TasitCins form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (string.IsNullOrWhiteSpace(form.ad))
                s.DegerAta("Cins adı alanını girmelisiniz");
            return s;
        }

        /// <summary>
        /// TasitCins nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">TasitCins yapısında TasitCins: İstatistik Kodu bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TasitCins form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta(ButceFormResI.TasitMarkaModel001);
                return s;
            }

            s = AlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHFTASITCINS tabBHFTASITTIP = new TABBHFTASITCINS(ve);

            ve.BeginTransaction();

            if (form.kod == 0)
            {
                form.kod = tabBHFTASITTIP.EnBuyukNo();
            }
            else
            {
                if (s.islemSonuc)
                    s = tabBHFTASITTIP.Sil(form);
            }
            if (s.islemSonuc)
                s = tabBHFTASITTIP.Ekle(form);

            if (s.islemSonuc)
            {
                ve.Commit();
                s.anahtar = form.kod.ToString();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// TasitCins nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">TasitCins yapısında TasitCins: İstatistik Kodu bilgilerini tutan değişken</param>
        public static TasitCins Oku(VeriErisim veriErisim, Kullanici kullanan, TasitCins form)
        {
            ObjectArray s = Listele(veriErisim, kullanan, form);
            if (s == null || s.ObjeSayisi == 0) return null;
            return (TasitCins)s.objeler[0];
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, TasitCins form)
        {
            return true;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, TasitCins form)
        {
            return new Sonuc(true, "", "");
        }

        /// <summary>
        /// TasitCins listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">TasitCins yapısında TasitCins: İstatistik Kodu bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, TasitCins form)
        {
            ObjectArray alAna = new ObjectArray();
            if (!GormeYetkisiVarMi(kullanan, form))
            {
                alAna.sonuc.DegerAta(ButceFormResI.TasitMarkaModel002);
                return alAna;
            }

            Sonuc s = AnahtarAlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                alAna.sonuc.DegerAta(s.hataStr);
                return alAna;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHFTASITCINS tabBHFTASITTIP = new TABBHFTASITCINS(ve);

            IDataReader dr = tabBHFTASITTIP.Listele(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                alAna.sonuc.DegerAta(ve.hata);
                return alAna;
            }
            while (dr.Read())
            {
                TasitCins detay = new TasitCins();
                detay.kod = VeriErisim.DegerAl(dr, (int)TABBHFTASITCINS.ENUMAlanlar.KOD, 0);
                detay.ad = VeriErisim.DegerAl(dr, (int)TABBHFTASITCINS.ENUMAlanlar.CINS, "");
                detay.tipKodu = VeriErisim.DegerAl(dr, (int)TABBHFTASITCINS.ENUMAlanlar.TIPKODU, 0);
                detay.tkodu = VeriErisim.DegerAl(dr, (int)TABBHFTASITCINS.ENUMAlanlar.TKODU, "");
                detay.motorsuz = VeriErisim.DegerAl(dr, (int)TABBHFTASITCINS.ENUMAlanlar.MOTORSUZ, 0);
                detay.tip = VeriErisim.DegerAl(dr, ((int)TABBHFTASITCINS.ENUMAlanlar.MOTORSUZ) + 1, "");
                alAna.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return alAna;
        }
    }
}
