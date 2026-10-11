using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.YAT;
using TNS.YAT.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.YAT.IsKurallari
{
    /// <summary>
    /// YATYATProjeEkliDosya formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class YATProjeEkliDosya
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriYAT";

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, ProjeEkliDosya form)
        {
            Sonuc s = new Sonuc();
            if (string.IsNullOrWhiteSpace(form.aciklama))
                s.DegerAta("Açıklama boş olamaz");
            if (string.IsNullOrWhiteSpace(form.dosyaAd))
                s.DegerAta("Dosya adı boş olamaz");
            if (string.IsNullOrWhiteSpace(form.fizikiDosyaAd))
                s.DegerAta("Fiziki dosya adı boş olamaz");

            return s;
        }

        /// <summary>
        /// ProjeEkliDosya nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">ProjeEkliDosya yapısında ProjeEkliDosya:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, ProjeEkliDosya form, int yil, string senaryoKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            Sonuc s = ProjeClass.ProjeAltProjeYazmaHakkiVarMi(ve, kullanan, form.projeKodu, form.altProjeKodu, yil, senaryoKod);
            if (s != null)
            {
                ve.VeriErisimKapat();
                return s;
            }
            TABYATPROJEEKLIDOSYA tabYATPROJEEKLIDOSYA = new TABYATPROJEEKLIDOSYA(ve);
            ve.BeginTransaction();
            s = tabYATPROJEEKLIDOSYA.Sil(yil, form);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// ProjeEkliDosya nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">ProjeEkliDosya yapısında ProjeEkliDosya:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ProjeEkliDosya form, int yil, string senaryoKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            Sonuc s = ProjeClass.ProjeAltProjeYazmaHakkiVarMi(ve, kullanan, form.projeKodu, form.altProjeKodu, yil, senaryoKod);
            if (s != null)
            {
                ve.VeriErisimKapat();
                return s;
            }

            s = AlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            TABYATPROJEEKLIDOSYA tabYATPROJEEKLIDOSYA = new TABYATPROJEEKLIDOSYA(ve);
            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabYATPROJEEKLIDOSYA.Ekle(yil, form);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// ProjeEkliDosya listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">ProjeEkliDosya yapısında ProjeEkliDosya:  bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, ProjeEkliDosya form, int yil, string senaryoKod)
        {
            ObjectArray oa = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            Sonuc s = ProjeClass.ProjeAltProjeYazmaGormeHakkiVarMi(ve, kullanan, form.projeKodu, form.altProjeKodu, yil, senaryoKod, BHMv2.ENUMOGY.Gorme);
            if (s != null)
            {
                oa.sonuc = s;
                ve.VeriErisimKapat();
                return oa;
            }

            TABYATPROJEEKLIDOSYA tabYATPROJEEKLIDOSYA = new TABYATPROJEEKLIDOSYA(ve);

            IDataReader dr = tabYATPROJEEKLIDOSYA.Listele(yil, form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                ProjeEkliDosya detay = new ProjeEkliDosya();
                detay.projeKodu = VeriErisim.DegerAl(dr, (int)TABYATPROJEEKLIDOSYA.ENUMAlanlar.PROJEKODU, 0);
                detay.altProjeKodu = VeriErisim.DegerAl(dr, (int)TABYATPROJEEKLIDOSYA.ENUMAlanlar.ALTPROJEKODU, 0);
                detay.fizikiDosyaAd = VeriErisim.DegerAl(dr, (int)TABYATPROJEEKLIDOSYA.ENUMAlanlar.FIZIKIDOSYAAD, "");
                detay.siraNo = VeriErisim.DegerAl(dr, (int)TABYATPROJEEKLIDOSYA.ENUMAlanlar.SIRANO, "");
                detay.dosyaAd = VeriErisim.DegerAl(dr, (int)TABYATPROJEEKLIDOSYA.ENUMAlanlar.DOSYAAD, "");
                detay.aciklama = VeriErisim.DegerAl(dr, (int)TABYATPROJEEKLIDOSYA.ENUMAlanlar.ACIKLAMA, "");
                detay.yuklemeZamani = VeriErisim.DegerAl(dr, (int)TABYATPROJEEKLIDOSYA.ENUMAlanlar.YUKLEMEZAMANI);
                detay.kullanicikodu = VeriErisim.DegerAl(dr, (int)TABYATPROJEEKLIDOSYA.ENUMAlanlar.KULLANICIKODU, "");
                oa.objeler.Add(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }
    }
}
