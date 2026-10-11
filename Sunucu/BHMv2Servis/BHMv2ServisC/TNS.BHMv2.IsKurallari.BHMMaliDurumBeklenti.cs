using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// BHMv2BHMMaliDurumBeklenti formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class BHMv2BHMMaliDurumBeklenti
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
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, MaliDurumBeklenti form)
        {
            return TavanGenelIslemler.YazmaYetkisiVarMi(kullanan, form.faaliyetKod);
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, MaliDurumBeklenti form)
        {
            int hazYil = OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBul(0, "/BUTCEHAZIRLIK"), 0);
            Sonuc s = new Sonuc(true, "", "");

            if (form.faaliyetKod.Replace(".", "").Length != TNS.BKD.Arac.PEBOdenekKodUzunlukBul(form.yil, 4, -1, true))
                s.DegerAta("Faaliyet kodunu tam seçiniz");

            if (s.islemSonuc)
            {
                TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
                string faalAd = uzyServis.UzayDegeriStr(kullanan, "ABSPEBKOD", form.yil + "-" + form.faaliyetKod, true);
                if (string.IsNullOrWhiteSpace(faalAd))
                    s.DegerAta(string.Format("Verilen faaliyet kodu{0} geçerli değil", form.faaliyetKod));
            }
            return s;
        }

        /// <summary>
        /// MaliDurumBeklenti nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">MaliDurumBeklenti yapısında MaliDurumBeklenti:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, MaliDurumBeklenti form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta(ButceHazirlikResI.BHMMaliDurumBeklenti001);
                return s;
            }

            s = AlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHMV2MALIDURUMBEKLENTI tabBHMV2MALIDURUMBEKLENTI = new TABBHMV2MALIDURUMBEKLENTI(ve);

            int okunan = BKDBirimButceHazirlik.OnayKodOkuTasra(ve, kullanan, form.yil, 0, "000", "0000", ENUMBirimOnayTur.MALIBEKLENTI);
            
            if(okunan==0)
                s.DegerAta(string.Format("{0} yılı veri girişine açılmamıştır. ", form.yil));
            else if (okunan == 5)
                s.DegerAta(string.Format("{0} yılı veri girişine kapatılmıştır. ", form.yil));

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabBHMV2MALIDURUMBEKLENTI.Sil(form);

            if (s.islemSonuc)
                s = tabBHMV2MALIDURUMBEKLENTI.Ekle(form);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }


        /// <summary>
        /// MaliDurumBeklenti nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">MaliDurumBeklenti yapısında MaliDurumBeklenti:  bilgilerini tutan değişken</param>
        public static MaliDurumBeklenti Oku(VeriErisim veriErisim, Kullanici kullanan, MaliDurumBeklenti form)
        {
            MaliDurumBeklenti[] s = Listele(veriErisim, kullanan, form);
            if (s == null || s.Length == 0) return null;
            return s[0];
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, MaliDurumBeklenti form)
        {
            return TavanGenelIslemler.GormeYetkisiVarMi(kullanan, form.faaliyetKod);
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, MaliDurumBeklenti form)
        {
            return new Sonuc(true, "", "");
        }


        /// <summary>
        /// MaliDurumBeklenti listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">MaliDurumBeklenti yapısında MaliDurumBeklenti:  bilgilerini tutan değişken</param>
        public static MaliDurumBeklenti[] Listele(VeriErisim veriErisim, Kullanici kullanan, MaliDurumBeklenti form)
        {
            if (!GormeYetkisiVarMi(kullanan, form))
            {
                throw new Exception(ButceHazirlikResI.BHMMaliDurumBeklenti002);
            }

            Sonuc s = AnahtarAlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                throw new Exception(s.hataStr);
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHMV2MALIDURUMBEKLENTI tabBHMV2MALIDURUMBEKLENTI = new TABBHMV2MALIDURUMBEKLENTI(ve);

            ArrayList alAna;
            IDataReader dr;
            dr = tabBHMV2MALIDURUMBEKLENTI.Oku(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            alAna = new ArrayList();
            while (dr.Read())
            {
                MaliDurumBeklenti detay = new MaliDurumBeklenti();
                detay.yil = VeriErisim.DegerAl(dr, (int)TABBHMV2MALIDURUMBEKLENTI.ENUMAlanlar.YIL, 0);
                detay.faaliyetKod = VeriErisim.DegerAl(dr, (int)TABBHMV2MALIDURUMBEKLENTI.ENUMAlanlar.FAALIYETKOD, "");
                detay.butceUygulamaSonucu = VeriErisim.DegerAl(dr, (int)TABBHMV2MALIDURUMBEKLENTI.ENUMAlanlar.BUTCEUYGULAMASONUCU, "");
                detay.yurutulenFaaliyetler = VeriErisim.DegerAl(dr, (int)TABBHMV2MALIDURUMBEKLENTI.ENUMAlanlar.YURUTULENFAALIYETLER, "");
                detay.beklentilerveHedefler = VeriErisim.DegerAl(dr, (int)TABBHMV2MALIDURUMBEKLENTI.ENUMAlanlar.BEKLENTILERVEHEDEFLER, "");
                detay.yurutulecekFaaliyetler = VeriErisim.DegerAl(dr, (int)TABBHMV2MALIDURUMBEKLENTI.ENUMAlanlar.YURUTULECEKFAALIYETLER, "");
                alAna.Add(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return (MaliDurumBeklenti[])alAna.ToArray(typeof(MaliDurumBeklenti));
        }

    }
}
