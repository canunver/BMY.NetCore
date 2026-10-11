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
    /// HRCPersonelBordroParam formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class HRCPersonelBordroParam
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD="BaglantiSatiriHRC";

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

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, BordroGenelDegisken form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (string.IsNullOrEmpty(form.aciklama)) s.DegerAta("Etiket alanını boş bırakmayınız!");
            if (string.IsNullOrEmpty(form.degiskenAd)) s.DegerAta("Değişken adı boş bırakmayınız!");
            return s;
        }

        /// <summary>
        /// BordroParamAna nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BordroParamAna yapısında BordroParamAna: Bordro Param Ana bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TNSCollection degiskenler)
        {
            BordroGenelDegisken.DegiskenleriSifirla();
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan))
            {
                s.DegerAta("Yazma yetkiniz yok!");
                return s;
            }

            foreach (BordroGenelDegisken  item in degiskenler)
            {
                s = AlanlarGecerliMi(kullanan, item);
                if (!s.islemSonuc)
                {
                    return s;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCBORDROPARAM tab = new TABHRCBORDROPARAM(ve);
            ve.BeginTransaction();

            foreach (BordroGenelDegisken item in degiskenler)
            {
                if (item.yeni == 0)
                    s = tab.Guncelle(item.degiskenAd, item.aciklama, item.sira, item.veriTipi, item.yontem, item.liste);
                else
                    s = tab.Ekle(item.degiskenAd, item.aciklama, item.sira, item.veriTipi, item.yontem, item.liste);
                if (!s.islemSonuc) break;
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// BordroParamAna listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BordroParamAna yapısında BordroParamAna: Bordro Param Ana bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan)
        {
            ObjectArray oa = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCBORDROPARAM tab = new TABHRCBORDROPARAM(ve);

            IDataReader dr = tab.Listele();
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                BordroGenelDegisken detay = new BordroGenelDegisken(VeriErisim.DegerAl(dr, (int)TABHRCBORDROPARAM.ENUMAlanlar.DEGISKENAD, ""), ENUMBordroDegiskenTur.KisiBordroBilgi, (VariableType)VeriErisim.DegerAl(dr, (int)TABHRCBORDROPARAM.ENUMAlanlar.TIP, 0), VeriErisim.DegerAl(dr, (int)TABHRCBORDROPARAM.ENUMAlanlar.DEGISKENETIKET, ""));
                detay.sira = VeriErisim.DegerAl(dr, (int)TABHRCBORDROPARAM.ENUMAlanlar.SIRA, "");
                detay.yontem = VeriErisim.DegerAl(dr, (int)TABHRCBORDROPARAM.ENUMAlanlar.YONTEM, 0);
                detay.liste = VeriErisim.DegerAl(dr, (int)TABHRCBORDROPARAM.ENUMAlanlar.LISTE, "");
                detay.yeni = 0;
                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }
    }
}
