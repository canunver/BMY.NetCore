using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.MUH;
using TNS.MUH.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// MUHOdenekGondermeIcmal formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class MUHOdenekGondermeIcmal
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
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, GondermeTenkisIcmal form)
        {
            return true;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, GondermeTenkisIcmal form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (TNSDateTime.IsNull(form.kayitTarih))
                s.DegerAta("Kayıt tarihi giriniz!");

            if (form.yil < 2000)
                s.DegerAta("Geçerli bir yıl giriniz!");

            if (form.yil < form.kayitTarih.Yil)
                s.DegerAta("Kayıt tarihi ile belge yılı uyumsuz!");

            if (form.icmalNo <= 0)
                s.DegerAta("Hatalı icmal no:" + form.icmalNo + "!");

            if (string.IsNullOrWhiteSpace(form.kurKod))
                s.DegerAta("Hatalı kurumsal kod!");

            if (form.kullanicilar.Count < 1)
                s.DegerAta("En az bir kullanıcı eklemelisiniz!");

            return s;
        }

        /// <summary>
        /// GondermeTenkisIcmal nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">GondermeTenkisIcmal yapısında GondermeTenkisIcmal:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, GondermeTenkisIcmal form, bool icmalFormKontrolYapma)
        {
            Sonuc s = new Sonuc(true, "", "");

            s = AlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            GondermeTenkisIcmal okunan = Oku(ve, kullanan, form);
            if (okunan != null)
            {
                if (!icmalFormKontrolYapma)
                {
                    if (okunan.formDurum == 5)
                    {
                        s.DegerAta("Onaylanmış formu değiştiremezsiniz!");
                        ve.VeriErisimKapat();
                        return s;
                    }

                    if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                    {
                        if (!okunan.KullaniciVarMi(kullanan.kullaniciKodu))
                        {
                            s.DegerAta(string.Format("{0} numaralı icmale yazma hakkınız yok, eğer bu icmalde değişiklik yapmak istiyorsanız kullanıcı kodunuzu icmal formuna ekleyiniz!", okunan.icmalNo));
                            ve.VeriErisimKapat();
                            return s;
                        }
                    }
                }
            }

            TABMUHGONDERMETENKISICMAL tab = new TABMUHGONDERMETENKISICMAL(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tab.Sil(form);
            if (s.islemSonuc)
                s = tab.SilKul(form);

            if (s.islemSonuc)
                s = tab.Ekle(form);

            for (int i = 0; i < form.kullanicilar.Count && s.islemSonuc; i++)
            {
                GondermeTenkisIcmalKullanici kull = (GondermeTenkisIcmalKullanici)form.kullanicilar[i];
                s = tab.EkleKul(form, kull.kulKod);
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, GondermeTenkisIcmal form)
        {
            return true;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, GondermeTenkisIcmal form)
        {
            return new Sonuc(true, "", "");
        }

        /// <summary>
        /// GondermeTenkisIcmal listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Okunacak GondermeTenkisIcmal formunun yılı</param>
        /// <param name="icmalNo">Okunacak GondermeTenkisIcmal formunun icmal numarası</param>
        public static GondermeTenkisIcmal Oku(VeriErisim veriErisim, Kullanici kullanan, int yil, ENUMBelgeTuru belgeTur, int icmalNo)
        {
            ObjectArray alAna = Listele(veriErisim, kullanan, new GondermeTenkisIcmal(yil, belgeTur, icmalNo));
            if (alAna.ObjeSayisi == 1)
            {
                return (GondermeTenkisIcmal)alAna[0];
            }
            return null;
        }

        /// <summary>
        /// GondermeTenkisIcmal listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">GondermeTenkisIcmal yapısında GondermeTenkisIcmal:  bilgilerini tutan değişken</param>
        public static GondermeTenkisIcmal Oku(VeriErisim veriErisim, Kullanici kullanan, GondermeTenkisIcmal form)
        {
            ObjectArray alAna = Listele(veriErisim, kullanan, form);
            if (alAna.ObjeSayisi == 1)
            {
                return (GondermeTenkisIcmal)alAna[0];
            }
            return null;
        }

        /// <summary>
        /// GondermeTenkisIcmal listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">GondermeTenkisIcmal yapısında GondermeTenkisIcmal:  bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, GondermeTenkisIcmal form)
        {
            if (!GormeYetkisiVarMi(kullanan, form))
            {
                throw new Exception(ButceMuhasebeResI.OdenekGondermeIcmal002);
            }

            ObjectArray alAna = new ObjectArray();
            alAna.sonuc = AnahtarAlanlarGecerliMi(kullanan, form);
            if (!alAna.sonuc.islemSonuc)
            {
                return alAna;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABMUHGONDERMETENKISICMAL tab = new TABMUHGONDERMETENKISICMAL(ve);

            IDataReader dr = tab.Listele(form);
            if (dr == null)
            {
                alAna.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return alAna;
            }
            GondermeTenkisIcmal detay = null;
            int kulAlan = TABMUHGONDERMETENKISICMAL.ALANLAR.Length;
            while (dr.Read())
            {
                int yil = VeriErisim.DegerAl(dr, (int)TABMUHGONDERMETENKISICMAL.ENUMAlanlar.YIL, 0);
                int icmalNo = VeriErisim.DegerAl(dr, (int)TABMUHGONDERMETENKISICMAL.ENUMAlanlar.ICMALNO, 0);
                ENUMBelgeTuru belgeTur = (ENUMBelgeTuru)VeriErisim.DegerAl(dr, (int)TABMUHGONDERMETENKISICMAL.ENUMAlanlar.BELGETUR, 0);

                if (detay == null || !(detay.yil == yil && detay.icmalNo == icmalNo && detay.belgeTur == belgeTur))
                {
                    detay = new GondermeTenkisIcmal();
                    detay.yil = yil;
                    detay.icmalNo = icmalNo;
                    detay.belgeTur = belgeTur;
                    detay.kurKod = VeriErisim.DegerAl(dr, (int)TABMUHGONDERMETENKISICMAL.ENUMAlanlar.KURKOD, "");
                    detay.kurKod = OrtakFonksiyonlar.KodNoktaKoy(TNS.BKD.TertipYapi.KodYapiBul(yil, 1, false), detay.kurKod);
                    detay.formDurum = VeriErisim.DegerAl(dr, (int)TABMUHGONDERMETENKISICMAL.ENUMAlanlar.FORMDURUM, 0);
                    detay.kayitTarih.Yaz(VeriErisim.DegerAl(dr, (int)TABMUHGONDERMETENKISICMAL.ENUMAlanlar.KAYITTARIH));
                    detay.kullanicilar = new TNSCollection();
                    alAna.Ekle(detay);
                }
                detay.kullanicilar.Add(new GondermeTenkisIcmalKullanici(VeriErisim.DegerAl(dr, kulAlan, "")));
            }
            dr.Close();

            ve.VeriErisimKapat();
            return alAna;
        }

        internal static Sonuc DurumAta(VeriErisim veriErisim, Kullanici kullanan, TNSCollection formlar)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, null))
            {
                s.DegerAta(ButceMuhasebeResI.OdenekGondermeIcmal001);
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            TABMUHGONDERMETENKISICMAL tab = new TABMUHGONDERMETENKISICMAL(ve);

            ve.BeginTransaction();

            for (int i = 0; i < formlar.Count && s.islemSonuc; i++)
            {
                GondermeTenkisIcmal form = (GondermeTenkisIcmal)formlar[i];
                s = tab.OnayKodAta(form);
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }
    }
}
