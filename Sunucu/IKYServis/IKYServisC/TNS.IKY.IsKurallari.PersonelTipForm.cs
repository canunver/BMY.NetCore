using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.IKY.IsKurallari
{
    /// <summary>
    /// IKYPersonelTipForm formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class IKYPersonelTipForm
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriIKY";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, PersonelTipi form)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            return false;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, PersonelTipi form)
        {
            return new Sonuc(true, "", "");
        }

        /// <summary>
        /// PersonelTipi nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">PersonelTipi yapısında PersonelTipi: Personel Tipi bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, PersonelTipi form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta(PersonelResI.PersonelTipForm001);
                return s;
            }

            s = AlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYPERSONELTIPI tabIKYPERSONELTIPI = new TABIKYPERSONELTIPI(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabIKYPERSONELTIPI.Sil(form);

            if (s.islemSonuc)
                s = tabIKYPERSONELTIPI.Ekle(form);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            TNS.IKY.Arac.PersonelTipSifirla();
            return s;
        }


        /// <summary>
        /// PersonelTipi nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">PersonelTipi yapısında PersonelTipi: Personel Tipi bilgilerini tutan değişken</param>
        public static ObjectVAN Oku(VeriErisim veriErisim, Kullanici kullanan, PersonelTipi form)
        {
            ObjectVAN ov = new ObjectVAN();
            ObjectArray s = Listele(veriErisim, kullanan, form);
            ov.sonuc = s.sonuc;
            if (s != null && s.ObjeSayisi > 0) ov.o = s[0];
            return ov;
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, PersonelTipi form)
        {
            return true;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, PersonelTipi form)
        {
            return new Sonuc(true, "", "");
        }


        /// <summary>
        /// PersonelTipi listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">PersonelTipi yapısında PersonelTipi: Personel Tipi bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, PersonelTipi form)
        {
            ObjectArray oa = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYPERSONELTIPI tabIKYPERSONELTIPI = new TABIKYPERSONELTIPI(ve);

            IDataReader dr = tabIKYPERSONELTIPI.Listele(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }

            while (dr.Read())
            {
                PersonelTipi detay = new PersonelTipi();
                detay.kod = VeriErisim.DegerAl(dr, (int)TABIKYPERSONELTIPI.ENUMAlanlar.KOD, 0);
                detay.ad = VeriErisim.DegerAl(dr, (int)TABIKYPERSONELTIPI.ENUMAlanlar.AD, "");
                detay.kurumDisi = VeriErisim.DegerAl(dr, (int)TABIKYPERSONELTIPI.ENUMAlanlar.KURUMDISI, 0);
                detay.sira = VeriErisim.DegerAl(dr, (int)TABIKYPERSONELTIPI.ENUMAlanlar.SIRA, 0);
                detay.kadrolu = VeriErisim.DegerAl(dr, (int)TABIKYPERSONELTIPI.ENUMAlanlar.KADROLU, 0);
                detay.goster = VeriErisim.DegerAl(dr, (int)TABIKYPERSONELTIPI.ENUMAlanlar.GOSTER, 0);
                detay.grupAd = VeriErisim.DegerAl(dr, (int)TABIKYPERSONELTIPI.ENUMAlanlar.GRUPAD, "");
                detay.maasGrupAd = VeriErisim.DegerAl(dr, (int)TABIKYPERSONELTIPI.ENUMAlanlar.MAASGRUPAD, "");
                detay.aktif = VeriErisim.DegerAl(dr, (int)TABIKYPERSONELTIPI.ENUMAlanlar.AKTIF, 0);
                detay.terfiyeDahil = VeriErisim.DegerAl(dr, (int)TABIKYPERSONELTIPI.ENUMAlanlar.TERFIYEDAHIL, 0);
                detay.maasaDahil = VeriErisim.DegerAl(dr, (int)TABIKYPERSONELTIPI.ENUMAlanlar.MAASADAHIL, 0);
                detay.hitapMi = VeriErisim.DegerAl(dr, (int)TABIKYPERSONELTIPI.ENUMAlanlar.HITAPMI, 0);

                detay.H1 = VeriErisim.DegerAl(dr, (int)TABIKYPERSONELTIPI.ENUMAlanlar.H1, 0);
                detay.H2 = VeriErisim.DegerAl(dr, (int)TABIKYPERSONELTIPI.ENUMAlanlar.H2, 0);
                detay.H3 = VeriErisim.DegerAl(dr, (int)TABIKYPERSONELTIPI.ENUMAlanlar.H3, 0);
                detay.H4 = VeriErisim.DegerAl(dr, (int)TABIKYPERSONELTIPI.ENUMAlanlar.H4, 0);
                detay.H5 = VeriErisim.DegerAl(dr, (int)TABIKYPERSONELTIPI.ENUMAlanlar.H5, 0);
                detay.H6 = VeriErisim.DegerAl(dr, (int)TABIKYPERSONELTIPI.ENUMAlanlar.H6, 0);
                detay.H7 = VeriErisim.DegerAl(dr, (int)TABIKYPERSONELTIPI.ENUMAlanlar.H7, 0);
                detay.HITAP = (ENUMHizmetTablolari) VeriErisim.DegerAl(dr, (int)TABIKYPERSONELTIPI.ENUMAlanlar.HITAP, 0);
                detay.pesinMi = VeriErisim.DegerAl(dr, (int)TABIKYPERSONELTIPI.ENUMAlanlar.PESINMI, 0);

                detay.maasDonemGun = VeriErisim.DegerAl(dr, (int)TABIKYPERSONELTIPI.ENUMAlanlar.MAASDONEMGUN, 0);
                detay.maasOdemeGun = VeriErisim.DegerAl(dr, (int)TABIKYPERSONELTIPI.ENUMAlanlar.MAASODEMEGUN, 0);

                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        internal static ObjectArray ListeleGrup(VeriErisim veriErisim, Kullanici kullanan, bool sadeceKullanilanlar)
        {
            ObjectArray oa = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYPERSONELTIPI tabIKYPERSONELTIPI = new TABIKYPERSONELTIPI(ve);

            IDataReader dr = tabIKYPERSONELTIPI.ListeleGrup(sadeceKullanilanlar);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }

            while (dr.Read())
            {
                PersonelTipi detay = new PersonelTipi();
                detay.ad = VeriErisim.DegerAl(dr, 0, "");
                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }
    }
}
