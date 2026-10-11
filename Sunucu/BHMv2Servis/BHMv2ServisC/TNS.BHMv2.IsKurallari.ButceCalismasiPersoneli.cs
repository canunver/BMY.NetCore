using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using TNS.BKD;

namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// BHMv2ButceCalismasiPersoneli formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class BHMv2ButceCalismasiPersoneli
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriBHMv2";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, ButceCalismasiPersoneli form)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)) return true;
            if (form == null) return false;
            return FaaliyetBul(kullanan, "Bütçe Hazırlığı Kaydedebileceği Altfaaliyetler", form.pebkod);
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static void AlanlarGecerliMi(Sonuc s, Kullanici kullanan, ButceCalismasiPersoneli ilkForm, ButceCalismasiPersoneli form)
        {
            if (ilkForm.yil == form.yil && ilkForm.pebkod == form.pebkod)
            {
                if (ilkForm.siraNo == form.siraNo)
                {
                    if (form.yil <= 2000)
                        s.DegerAta("Lütfen geçerli br yıl giriniz.");

                    if (form.pebkod.Replace(".", "").Length != 8)
                        s.DegerAta("Kaydetmek için en alt faaliyeti seçiniz.");
                }

                if (string.IsNullOrWhiteSpace(form.ad)) s.DegerAta(string.Format("{0}. satırdaki ad alanını doldurunuz!", form.siraNo));
                if (string.IsNullOrWhiteSpace(form.soyad)) s.DegerAta(string.Format("{0}. satırdaki soyad alanını doldurunuz!", form.siraNo));
                if (string.IsNullOrWhiteSpace(form.unvan)) s.DegerAta(string.Format("{0}. satırdaki unvan alanını doldurunuz!", form.siraNo));
                if (string.IsNullOrWhiteSpace(form.birim)) s.DegerAta(string.Format("{0}. satırdaki birim alanını doldurunuz!", form.siraNo));
                if (string.IsNullOrWhiteSpace(form.isTel)) s.DegerAta(string.Format("{0}. satırdaki iş telefonu alanını doldurunuz!", form.siraNo));
                if (string.IsNullOrWhiteSpace(form.ePosta)) s.DegerAta(string.Format("{0}. satırdaki elektronik posta alanını doldurunuz!", form.siraNo));
                else if (!OrtakFonksiyonlar.MailAdresiDogrumu(form.ePosta)) s.DegerAta(string.Format("{0}. satırdaki elektronik posta alanını hatalı!", form.siraNo));
            }
            else
                s.DegerAta("Tüm formlar aynı yıl ve faaliyete ait olmalıdır.");
        }

        /// <summary>
        /// ButceCalismasiPersoneli nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">ButceCalismasiPersoneli yapısında ButceCalismasiPersoneli: Bütçe Çalışmalarında Görevli Personel Listesi bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TNSCollection formlar)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (formlar.Count == 0)
            {
                s.DegerAta("Kayıt işlemi için en az bir satır göndermelisiniz!");
                return s;
            }
            ButceCalismasiPersoneli form = (ButceCalismasiPersoneli)formlar[0];
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta("Yetkiniz Yol");
                return s;
            }

            for (int i = 0; i < formlar.Count; i++)
            {
                AlanlarGecerliMi(s, kullanan, form, (ButceCalismasiPersoneli)formlar[i]);
            }

            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHMV2BUTCECALISMASIPERSONELI tabBHMV2BUTCECALISMASIPERSONELI = new TABBHMV2BUTCECALISMASIPERSONELI(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabBHMV2BUTCECALISMASIPERSONELI.Sil(form.yil, form.pebkod);

            for (int i = 0; i < formlar.Count && s.islemSonuc; i++)
            {
                s = tabBHMV2BUTCECALISMASIPERSONELI.Ekle((ButceCalismasiPersoneli)formlar[i]);
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
        public static bool GormeYetkisiVarMi(Kullanici kullanan, ButceCalismasiPersoneli form)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)) return true;
            return FaaliyetBul(kullanan, "Bütçe Hazırlığı Görebileceği Altfaaliyetler", form.pebkod);
        }

        private static bool FaaliyetBul(Kullanici kullanan, string yetkiTipiAdi, string bakilacakDeger)
        {
            string yetkiTipiDegeri = kullanan.YetkiTipiDegeriBul(yetkiTipiAdi);
            if (string.IsNullOrWhiteSpace(bakilacakDeger)) return false;
            bakilacakDeger = bakilacakDeger.Replace(".", "");
            if (yetkiTipiDegeri == null) return false;
            if (yetkiTipiDegeri == "*") return true;
            if (yetkiTipiDegeri == bakilacakDeger) return true;
            string[] d = yetkiTipiDegeri.Replace(".", "").Split(';', ',');
            foreach (string yetkiTipiDegeriParca in d)
            {
                if (!string.IsNullOrEmpty(yetkiTipiDegeriParca))
                {
                    if (bakilacakDeger.StartsWith(yetkiTipiDegeriParca))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, ButceCalismasiPersoneli form)
        {
            return new Sonuc(true, "", "");
        }

        /// <summary>
        /// ButceCalismasiPersoneli listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">ButceCalismasiPersoneli yapısında ButceCalismasiPersoneli: Bütçe Çalışmalarında Görevli Personel Listesi bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, ButceCalismasiPersoneli form)
        {
            ObjectArray oa = new ObjectArray();
            if (!GormeYetkisiVarMi(kullanan, form))
            {
                oa.sonuc.DegerAta("Görme yetkiniz yok!");
                return oa;
            }

            Sonuc s = AnahtarAlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                oa.sonuc.DegerAta(s.hataStr);
                return oa;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHMV2BUTCECALISMASIPERSONELI tab = new TABBHMV2BUTCECALISMASIPERSONELI(ve);

            IDataReader dr = tab.Listele(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            Tertip tUz = new Tertip(form.yil, ENUMTabloKod.TABPEBOdenek);

            
            while (dr.Read())
            {
                ButceCalismasiPersoneli detay = new ButceCalismasiPersoneli();
                detay.yil = VeriErisim.DegerAl(dr, (int)TABBHMV2BUTCECALISMASIPERSONELI.ENUMAlanlar.YIL, 0);
                detay.pebkod = VeriErisim.DegerAl(dr, (int)TABBHMV2BUTCECALISMASIPERSONELI.ENUMAlanlar.PEBKOD, "");
                detay.siraNo = VeriErisim.DegerAl(dr, (int)TABBHMV2BUTCECALISMASIPERSONELI.ENUMAlanlar.SIRANO, 0);
                detay.ad = VeriErisim.DegerAl(dr, (int)TABBHMV2BUTCECALISMASIPERSONELI.ENUMAlanlar.AD, "");
                detay.soyad = VeriErisim.DegerAl(dr, (int)TABBHMV2BUTCECALISMASIPERSONELI.ENUMAlanlar.SOYAD, "");
                detay.unvan = VeriErisim.DegerAl(dr, (int)TABBHMV2BUTCECALISMASIPERSONELI.ENUMAlanlar.UNVAN, "");
                detay.birim = VeriErisim.DegerAl(dr, (int)TABBHMV2BUTCECALISMASIPERSONELI.ENUMAlanlar.BIRIM, "");
                detay.isTel = VeriErisim.DegerAl(dr, (int)TABBHMV2BUTCECALISMASIPERSONELI.ENUMAlanlar.ISTEL, "");
                detay.cepTel = VeriErisim.DegerAl(dr, (int)TABBHMV2BUTCECALISMASIPERSONELI.ENUMAlanlar.CEPTEL, "");
                detay.faks = VeriErisim.DegerAl(dr, (int)TABBHMV2BUTCECALISMASIPERSONELI.ENUMAlanlar.FAKS, "");
                detay.ePosta = VeriErisim.DegerAl(dr, (int)TABBHMV2BUTCECALISMASIPERSONELI.ENUMAlanlar.EPOSTA, "");
                detay.pebad = VeriErisim.DegerAl(dr, (int)TABBHMV2BUTCECALISMASIPERSONELI.ENUMAlanlar.EPOSTA + 1, "");
                detay.pebkod = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(4), detay.pebkod);
                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        internal static Sonuc Kopyala(VeriErisim veriErisim, Kullanici kullanan, int yil)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, null))
            {
                s.DegerAta("Yetkiniz Yol");
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHMV2BUTCECALISMASIPERSONELI tabBHMV2BUTCECALISMASIPERSONELI = new TABBHMV2BUTCECALISMASIPERSONELI(ve);

            ve.BeginTransaction();

            if (s.islemSonuc && yil > DateTime.Now.Year)
                s = tabBHMV2BUTCECALISMASIPERSONELI.Sil(yil, null);

            if (s.islemSonuc)
                s = tabBHMV2BUTCECALISMASIPERSONELI.Kopyala(yil);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }
    }
}
