using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.KYM;
using TNS.KYM.IsKurallari.Veri;
using System.Collections;

namespace TNS.KYM.IsKurallari
{
    /// <summary>
    /// KYMCografya formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class KYMCografya
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD="BaglantiSatiriKYM";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, Cografya form)
        {
            return true;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, Cografya form)
        {
            return new Sonuc(true, "", "");
        }


        /// <summary>
        /// Cografya nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Cografya yapısında Cografya:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Cografya form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta(KullaniciResI.Cografya001);
                return s;
            }

            s = AlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABORGCOGRAFYA tabKYMCOGRAFYA = new TABORGCOGRAFYA(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabKYMCOGRAFYA.Sil(form);

            if (s.islemSonuc)
                s = tabKYMCOGRAFYA.Ekle(form);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }


        /// <summary>
        /// Cografya nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Cografya yapısında Cografya:  bilgilerini tutan değişken</param>
        public static Cografya Oku(VeriErisim veriErisim, Kullanici kullanan, Cografya form)
        {
            Cografya[] s = Listele(veriErisim, kullanan, form);
            if (s == null || s.Length == 0) return null;
            return s[0];
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, Cografya form)
        {
            return true;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, Cografya form)
        {
            return new Sonuc(true, "", "");
        }


        /// <summary>
        /// Cografya listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Cografya yapısında Cografya:  bilgilerini tutan değişken</param>
        public static Cografya[] Listele(VeriErisim veriErisim, Kullanici kullanan, Cografya form)
        {
            if (!GormeYetkisiVarMi(kullanan, form))
            {
                throw new Exception(KullaniciResI.Cografya002);
            }

            Sonuc s = AnahtarAlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                throw new Exception(s.hataStr);
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABORGCOGRAFYA tabKYMCOGRAFYA = new TABORGCOGRAFYA(ve);

            ArrayList alAna;
            IDataReader dr;
            dr = tabKYMCOGRAFYA.Oku(form, TNS.DEG.Arac.KodYapiBul(0, 20));
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            alAna = new ArrayList();
            while (dr.Read())
            {
                Cografya detay = new Cografya();
                detay.kod = VeriErisim.DegerAl(dr, (int)TABORGCOGRAFYA.ENUMAlanlar.KOD, "");
                detay.ad = VeriErisim.DegerAl(dr, (int)TABORGCOGRAFYA.ENUMAlanlar.AD, "");
                detay.ad2 = VeriErisim.DegerAl(dr, (int)TABORGCOGRAFYA.ENUMAlanlar.AD2, "");
                detay.kisaAd = VeriErisim.DegerAl(dr, (int)TABORGCOGRAFYA.ENUMAlanlar.KISAAD, "");
                detay.plakaKod = VeriErisim.DegerAl(dr, (int)TABORGCOGRAFYA.ENUMAlanlar.PLAKAKOD, "");
                detay.nufus = VeriErisim.DegerAl(dr, (int)TABORGCOGRAFYA.ENUMAlanlar.NUFUS, 0);
                detay.muhasebe = VeriErisim.DegerAl(dr, (int)TABORGCOGRAFYA.ENUMAlanlar.MUHASEBE, 0);
                detay.mernisKod = VeriErisim.DegerAl(dr, (int)TABORGCOGRAFYA.ENUMAlanlar.MERNISKOD, 0);
                alAna.Add(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return (Cografya[])alAna.ToArray(typeof(Cografya));
        }

    }
}
